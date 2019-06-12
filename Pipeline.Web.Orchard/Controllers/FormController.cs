#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Transformalize.Contracts;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Pipeline.Web.Orchard.Services.Contracts;
using Process = Transformalize.Configuration.Process;
using Permissions = Orchard.Core.Contents.Permissions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed]
    public class FormController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISecureFileService _secureFileService;
        private readonly IFileService _fileService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public FormController(
            IOrchardServices services,
            IProcessService processService,
            ISecureFileService secureFileService,
            IFileService fileService
        ) {
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
            _fileService = fileService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed(true)]
        public ActionResult Index(int id) {
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult("Form not found.");
            }
            var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

            if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                var process = _processService.Resolve(part);
                var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);

                // so file required() validator is fooled
                if (Request.Files != null && Request.Files.Count > 0) {
                    foreach (var key in Request.Files.AllKeys) {
                        if (Request.Files[key] != null && Request.Files[key].ContentLength > 0) {
                            parameters[key] = "file.tmp";
                        }
                    }
                }

                process.Load(part.Configuration, parameters);

                if (process.Errors().Any() || process.Warnings().Any()) {
                    return View(new FormViewModel(part, process));
                }

                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                var entity = process.Entities.First();
                runner.Execute(process);

                if (Request.HttpMethod.Equals("POST")) {

                    if (entity.Rows.Count == 1 && (bool)entity.Rows[0][entity.ValidField]) {
                        // reset, modify for actual insert, and execute again
                        process = _processService.Resolve(part);
                        process.Load(part.Configuration, parameters);
                        var insert = entity.GetPrimaryKey().All(k => parameters.ContainsKey(k.Alias) && k.Default == parameters[k.Alias]);
                        process.Actions.Add(new Transformalize.Configuration.Action {
                            After = true,
                            Before = false,
                            Type = "run",
                            Connection = entity.Connection,
                            Command = insert ? entity.InsertCommand : entity.UpdateCommand,
                            Key = Guid.NewGuid().ToString(),
                            ErrorMode = "exception"
                        });

                        // files
                        if (Request.Files != null && Request.Files.Count > 0) {
                            var files = entity.Fields.Where(f => f.Input && f.InputType == "file").ToArray();
                            for (var i = 0; i < files.Length; i++) {
                                var field = files[i];
                                var input = Request.Files.Get(field.Alias);
                                var parameter = process.Parameters.FirstOrDefault(p => p.Name == field.Alias);
                                if (input != null && input.ContentLength > 0) {
                                    var filePart = _fileService.Upload(input, "Authenticated", "Forms", i + 1);
                                    if (parameter != null) {
                                        parameter.Value = Url.Action("View", "File", new { id = filePart.Id }) ?? string.Empty;
                                    }
                                } else {
                                    if (parameter != null && parameters.ContainsKey(field.Alias + "_Old")) {
                                        parameter.Value = parameters[field.Alias + "_Old"];
                                    }
                                }
                            }
                        }

                        try {
                            runner.Execute(process);
                            _orchardServices.Notifier.Information(insert ? T("{0} inserted", process.Name) : T("{0} updated", process.Name));
                            return Redirect(parameters["Orchard.ReturnUrl"]);
                        } catch (Exception ex) {
                            if (ex.Message.Contains("duplicate")) {
                                _orchardServices.Notifier.Error(T("The {0} save failed: {1}", process.Name, "The database has rejected this update due to a unique constraint violation."));
                            } else {
                                _orchardServices.Notifier.Error(T("The {0} save failed: {1}", process.Name, ex.Message));
                            }
                            Logger.Error(ex, ex.Message);
                        }
                    } else {
                        _orchardServices.Notifier.Error(T("The form did not pass validation.  Please correct it and re-submit."));
                    }

                }
                return View(new FormViewModel(part, process));
            }

            _orchardServices.Notifier.Warning(user == "Anonymous" ? T("Anonymous users do not have permission to view this form. You may need to login.") : T("Sorry {0}. You do not have permission to view this form.", user));
            return new HttpUnauthorizedResult();
        }

        [Themed(false)]
        public ActionResult Content(int id) {

            var process = new Process { Name = "Form", Id = id };
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Status = 404;
                process.Message = "Not Found";
            } else {
                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                    process = _processService.Resolve(part);
                    var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);
                    process.Load(part.Configuration, parameters);
                    runner.Execute(process);

                } else {
                    process.Message = "Unauthorized";
                    process.Status = 401;
                }
            }
            return View(new FormViewModel(part, process));
        }

    }
}