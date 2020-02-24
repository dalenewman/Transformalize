using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transformalize.Configuration;
using Transformalize.Impl;

namespace Pipeline.Web.Orchard.Models {
   public class FormViewModel {

      private readonly IOrchardServices _orchard;

      public FormViewModel(HttpRequestBase request, IOrchardServices orchard, PipelineConfigurationPart part, Process process) {

         _orchard = orchard;
         Part = part;
         Process = process;
         Geo = process.Parameters.Any(p => p.Name.EndsWith(".Latitude", StringComparison.OrdinalIgnoreCase));
         Latitude = string.Empty;
         Longitude = string.Empty;
         Accuracy = string.Empty;
         Request = request;
         Entity = process.Entities.FirstOrDefault();

         SectionsDisplayed = new HashSet<string>();

         if (Entity != null) {

            Row = Entity.Rows.FirstOrDefault();
            InputFields = Entity.Fields.Where(f => f.Input).ToArray();
            HasFile = Entity.Fields.Any(f => f.InputType == "file");

            // determine focus
            if (Request.HttpMethod == "GET") {
               Focus = InputFields.First(f => !f.PrimaryKey).Alias;
            } else {
               var previous = Request.Form["Orchard.Focus"] == "Orchard.Submit" ? InputFields.Last() : InputFields.First(f => f.Name == Request.Form["Orchard.Focus"]);
               var maxIndex = InputFields.Where(f => !f.PrimaryKey).Max(f => f.Index);
               if (previous.Index < maxIndex) {
                  var next = InputFields.OrderBy(f => f.Index).FirstOrDefault(f => f.Index > previous.Index);
                  if (next != null) {
                     Focus = next.Alias;
                  } else {
                     var invalid = InputFields.FirstOrDefault(f => f.ValidField != string.Empty && !(bool)Row[f.ValidField]);
                     Focus = invalid == null ? "Orchard.Submit" : invalid.Alias;
                  }
               } else {
                  var invalid = InputFields.FirstOrDefault(f => f.ValidField != string.Empty && !(bool)Row[f.ValidField]);
                  Focus = invalid == null ? "Orchard.Submit" : invalid.Alias;
               }
            }

            Valid = Entity.ValidField != string.Empty && (Row != null && (bool)Row[Entity.ValidField]);
         }
      }

      public bool HasFile { get; set; }
      public PipelineConfigurationPart Part { get; set; }
      public Process Process { get; set; }
      public bool Geo { get; set; }
      public string Latitude { get; set; }
      public string Longitude { get; set; }
      public string Accuracy { get; set; }
      public HttpRequestBase Request { get; set; }
      public Entity Entity { get; set; }
      public CfgRow Row { get; set; }
      public Field[] InputFields { get; set; }
      public string Focus { get; set; }
      public bool Valid { get; set; }

      public bool IsValid(CfgRow row, Field field) {
         return Request.HttpMethod == "GET" || field.ValidField == string.Empty || (bool)row[field.ValidField];
      }

      public string Status(CfgRow row, Field field) {
         return IsValid(row, field) ? string.Empty : "has-error";
      }

      public bool UseTextArea(Field field, out int length) {
         var useTextArea = field.Length == "max";
         length = 4000;
         if (!useTextArea) {
            if (int.TryParse(field.Length, out length)) {
               useTextArea = length >= 255;
            }
         }
         if(length == 0) {
            length = 4000;
         }
         return useTextArea;
      }

      public PipelineFilePart GetFile(Field field, object value) {

         int id = 0;
         if (int.TryParse(Request.Form[field.Alias + "_Old"], out id)) {
            // preserved id before committing is in *_Old field
            return _orchard.WorkContext.Resolve<IContentManager>().Get(id).As<PipelineFilePart>();
         } else {
            if(value != null) {
               if(int.TryParse(value.ToString(), out id)) {
                  // new files are saved as just the content item id
                  return _orchard.WorkContext.Resolve<IContentManager>().Get(id).As<PipelineFilePart>();
               } else {
                  // old files were saved like this /Pipeline/File/View/{id} which is the path to view them
                  var str = value.ToString();
                  if (str != string.Empty) {
                     var sid = str.Split(new[] { '/' }, StringSplitOptions.None).LastOrDefault();
                     if (sid != null) {
                        if (int.TryParse(sid, out id)) {
                           return _orchard.WorkContext.Resolve<IContentManager>().Get(id).As<PipelineFilePart>();
                        }
                     }
                  }
               }
            }
         }

         return null;
      }

      public HashSet<string> SectionsDisplayed { get; set; }
   }
}