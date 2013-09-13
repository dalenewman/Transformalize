#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Main.Template_;

namespace Transformalize.Main
{
    public class ProcessReader : IReader<Process>
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly ProcessConfigurationElement _config;
        private readonly ConversionFactory _conversionFactory = new ConversionFactory();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Options _options;
        private readonly string _processName = string.Empty;
        private int _connectionCount;
        private int _entityCount;
        private int _mapCount;
        private Process _process;
        private int _providerCount;
        private int _relationshipCount;
        private int _scriptCount;
        private int _templateCount;
        private int _transformCount;

        public ProcessReader(ProcessConfigurationElement process, Options options)
        {
            _config = Adapt(process);
            _options = options;
        }

        public Process Read()
        {
            if (_config == null)
            {
                _log.Error("Sorry.  I can't find a process named {0}.", _processName);
                return new Process();
            }

            _process = new Process(_config.Name)
                           {
                               Options = _options,
                               TemplateContentType =
                                   _config.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html
                           };

            SetupRazorTemplateService();

            //process level
            _providerCount = ReadProviders();
            _connectionCount = ReadConnections();
            _scriptCount = ReadScripts();
            _templateCount = ReadTemplates();
            _process.SearchTypes = ReadSearchTypes();
            _mapCount = ReadMaps();

            //depend on process level
            _entityCount = ReadEntities();
            _relationshipCount = ReadRelationships();
            _transformCount = ReadProcessCalculatedFields();
            _process.RelatedKeys = ReadRelatedKeys();
            _process.View = _process.MasterEntity.OutputName() + "Star";

            foreach (var entity in _process.Entities)
            {
                entity.RelationshipToMaster = ReadRelationshipToMaster(entity);
                if (!entity.RelationshipToMaster.Any() && !entity.IsMaster())
                {
                    _log.Error("The entity {0} must have a relationship to the master entity {1}.", entity.Name,
                               _process.MasterEntity.Name);
                    Environment.Exit(1);
                }
            }

            LogProcessConfiguration();
            return _process;
        }

        private static ProcessConfigurationElement Adapt(ProcessConfigurationElement process)
        {
            new FromXmlTransformFieldsToParametersAdapter(process).Adapt();
            new FromXmlTransformFieldsMoveAdapter(process).Adapt();
            return process;
        }

        private Dictionary<string, SearchType> ReadSearchTypes()
        {
            var searchTypes = new Dictionary<string, SearchType>();

            searchTypes["none"] = new SearchType
                                      {
                                          Name = "none",
                                          Index = false,
                                          Store = false,
                                          Type = "inherit"
                                      };

            var configuredTypes = _config.SearchTypes.Cast<SearchTypeConfigurationElement>().ToArray();
            if (configuredTypes.Any())
            {
                foreach (var st in configuredTypes)
                {
                    searchTypes[st.Name] = new SearchType
                                               {
                                                   Index = st.Index,
                                                   Name = st.Name,
                                                   Store = st.Store,
                                                   Type = st.Type
                                               };
                }
            }
            return searchTypes;
        }

        private int ReadScripts()
        {
            var s = new[] { '\\' };
            ScriptConfigurationElement[] scripts = _config.Scripts.Cast<ScriptConfigurationElement>().ToArray();
            string path = _config.Scripts.Path;
            foreach (ScriptConfigurationElement script in scripts)
            {
                var fileInfo = new FileInfo(path.TrimEnd(s) + @"\" + script.File);
                if (!fileInfo.Exists)
                {
                    _log.Warn("Missing Script: {0}.", fileInfo.FullName);
                }
                else
                {
                    _process.Scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName),
                                                               fileInfo.FullName);
                    _log.Debug("Loaded script {0}.", fileInfo.FullName);
                }
            }

            return scripts.Count();
        }

        private int ReadTemplates()
        {
            var s = new[] { '\\' };
            TemplateConfigurationElement[] templates = _config.Templates.Cast<TemplateConfigurationElement>().ToArray();
            string path = _config.Templates.Path;
            foreach (TemplateConfigurationElement element in templates)
            {
                var fileInfo = new FileInfo(path.TrimEnd(s) + @"\" + element.File);
                if (!fileInfo.Exists)
                {
                    _log.Warn("Missing Template {0}.", fileInfo.FullName);
                }
                else
                {
                    var template = new Template(element.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName,
                                                element.ContentType, _process);

                    foreach (SettingConfigurationElement setting in element.Settings)
                    {
                        template.Settings[setting.Name] = _conversionFactory.Convert(setting.Value, setting.Type);
                    }

                    foreach (ActionConfigurationElement action in element.Actions)
                    {
                        var templateAction = new TemplateAction
                                                 {
                                                     Action = action.Action,
                                                     File = action.File,
                                                     Method = action.Method,
                                                     Url = action.Url,
                                                     ProcessName = _process.Name,
                                                     TemplateName = template.Name
                                                 };

                        if (!String.IsNullOrEmpty(action.Connection) &&
                            _process.Connections.ContainsKey(action.Connection))
                        {
                            templateAction.Connection = _process.Connections[action.Connection];
                        }

                        template.Actions.Add(templateAction);
                    }

                    _process.Templates[element.Name] = template;
                    _log.Debug("Loaded template {0} with {1} setting{2}.", fileInfo.FullName, template.Settings.Count,
                               template.Settings.Count == 1 ? string.Empty : "s");
                }
            }

            return templates.Count();
        }

        private IEnumerable<Relationship> ReadRelationshipToMaster(Entity rightEntity)
        {
            List<Relationship> relationships =
                _process.Relationships.Where(r => r.RightEntity.Equals(rightEntity)).ToList();

            if (relationships.Any() && !relationships.Any(r => r.LeftEntity.IsMaster()))
            {
                Entity leftEntity = relationships.Last().LeftEntity;
                relationships.AddRange(ReadRelationshipToMaster(leftEntity));
            }
            return relationships;
        }

        private void LogProcessConfiguration()
        {
            _log.Debug("Process Loaded.");
            _log.Debug("{0} Providers{1}.", _providerCount, _providerCount == 1 ? string.Empty : "s");
            _log.Debug("{0} Connection{1}.", _connectionCount, _connectionCount == 1 ? string.Empty : "s");
            _log.Debug("{0} Entit{1}.", _entityCount, _entityCount == 1 ? "y" : "ies");
            _log.Debug("{0} Relationship{1}.", _relationshipCount, _relationshipCount == 1 ? string.Empty : "s");
            _log.Debug("{0} Script{1}.", _scriptCount, _scriptCount == 1 ? string.Empty : "s");
            _log.Debug("{0} Template{1}.", _templateCount, _templateCount == 1 ? string.Empty : "s");
            _log.Debug("{0} Map{1}.", _mapCount, _mapCount == 1 ? string.Empty : "s");

            _transformCount +=
                _process.Entities.Sum(
                    e => e.CalculatedFields.Count + e.Fields.ToEnumerable().Sum(f => f.Transforms.Count));
            _log.Debug("{0} Transform{1}.", _transformCount, _transformCount == 1 ? string.Empty : "s");
        }

        private int ReadProcessCalculatedFields()
        {
            foreach (FieldConfigurationElement field in _config.CalculatedFields)
            {
                _process.CalculatedFields.Add(field.Alias,
                                              new FieldReader(_process, null,
                                                              new ProcessTransformParametersReader(_process),
                                                              new ProcessParametersReader(_process)).Read(field));
            }

            return _process.CalculatedFields.Count;
        }

        private int ReadRelationships()
        {
            int count = 0;
            foreach (RelationshipConfigurationElement r in _config.Relationships)
            {
                Entity leftEntity = _process.Entities.First(e => e.Alias.Equals(r.LeftEntity, IC));
                Entity rightEntity = _process.Entities.First(e => e.Alias.Equals(r.RightEntity, IC));
                List<Join> join = GetJoins(r, leftEntity, rightEntity);
                var relationship = new Relationship
                                       {
                                           LeftEntity = leftEntity,
                                           RightEntity = rightEntity,
                                           Join = join
                                       };

                _process.Relationships.Add(relationship);
                count++;
            }
            return count;
        }

        private List<Join> GetJoins(RelationshipConfigurationElement r, Entity leftEntity, Entity rightEntity)
        {
            if (string.IsNullOrEmpty(r.LeftField))
            {
                return (
                           from JoinConfigurationElement j in r.Join
                           select GetJoin(leftEntity, j.LeftField, rightEntity, j.RightField)
                       ).ToList();
            }

            // if it's a single field join, you can use leftField and rightField on the relationship element
            return new List<Join>
                       {
                           GetJoin(leftEntity, r.LeftField, rightEntity, r.RightField)
                       };
        }

        public Join GetJoin(Entity leftEntity, string leftField, Entity rightEntity, string rightField)
        {
            if (!leftEntity.All.ContainsKey(leftField) &&
                !leftEntity.All.ToEnumerable().Any(Common.FieldFinder(leftField)))
            {
                _log.Error(
                    "The left entity {0} does not have a field named {1} for joining to the right entity {2} with field {3}.",
                    leftEntity.Alias, leftField, rightEntity.Alias, rightField);
                Environment.Exit(0);
            }

            if (!rightEntity.All.ContainsKey(rightField) &&
                !rightEntity.All.ToEnumerable().Any(Common.FieldFinder(rightField)))
            {
                _log.Error(
                    "The right entity {0} does not have a field named {1} for joining to the left entity {2} with field {3}.",
                    rightEntity.Alias, rightField, leftEntity.Alias, leftField);
                Environment.Exit(0);
            }

            var join = new Join
                           {
                               LeftField =
                                   leftEntity.All.ContainsKey(leftField)
                                       ? leftEntity.All[leftField]
                                       : leftEntity.All.ToEnumerable().First(Common.FieldFinder(leftField)),
                               RightField =
                                   rightEntity.All.ContainsKey(rightField)
                                       ? rightEntity.All[rightField]
                                       : rightEntity.All.ToEnumerable().First(Common.FieldFinder(rightField))
                           };

            if (join.LeftField.FieldType.HasFlag(FieldType.MasterKey) ||
                join.LeftField.FieldType.HasFlag(FieldType.PrimaryKey))
            {
                join.LeftField.FieldType |= FieldType.ForeignKey;
            }
            else
            {
                join.LeftField.FieldType = FieldType.ForeignKey;
            }

            return join;
        }

        private int ReadEntities()
        {
            int count = 0;
            foreach (EntityConfigurationElement element in _config.Entities)
            {
                var reader = new EntityConfigurationReader(_process);
                Entity entity = reader.Read(element, count == 0);

                GuardAgainstFieldOverlap(entity);

                _process.Entities.Add(entity);
                if (entity.IsMaster())
                    _process.MasterEntity = entity;
                count++;
            }
            return count;
        }

        private IEnumerable<Field> ReadRelatedKeys()
        {
            Entity entity = _process.Entities.First(e => e.IsMaster());
            return GetRelatedKeys(entity);
        }

        private IEnumerable<Field> GetRelatedKeys(Entity entity)
        {
            List<Field> foreignKeys =
                entity.All.ToEnumerable().Where(f => f.FieldType.HasFlag(FieldType.ForeignKey)).ToList();
            if (foreignKeys.Any())
            {
                foreach (string alias in foreignKeys.Select(fk => fk.Alias).ToArray())
                {
                    Entity nextEntity =
                        _process.Relationships.Where(
                            r =>
                            r.LeftEntity.Alias.Equals(entity.Alias) && r.Join.Any(j => j.LeftField.Alias.Equals(alias)))
                                .Select(r => r.RightEntity)
                                .First();
                    foreignKeys.AddRange(GetRelatedKeys(nextEntity));
                }
            }
            return foreignKeys;
        }

        private int ReadMaps()
        {
            int count = 0;
            foreach (MapConfigurationElement m in _config.Maps)
            {
                if (string.IsNullOrEmpty(m.Connection))
                {
                    _process.MapEquals[m.Name] = new MapConfigurationReader(m.Items, "equals").Read();
                    _process.MapStartsWith[m.Name] = new MapConfigurationReader(m.Items, "startswith").Read();
                    _process.MapEndsWith[m.Name] = new MapConfigurationReader(m.Items, "endswith").Read();
                }
                else
                {
                    if (_process.Connections.ContainsKey(m.Connection))
                    {
                        _process.MapEquals[m.Name] =
                            new SqlServerMapReader(m.Items.Sql, _process.Connections[m.Connection].ConnectionString)
                                .Read();
                    }
                    else
                    {
                        _log.Error("Map {0} references connection {1}, which does not exist.", m.Name, m.Connection);
                        Environment.Exit(0);
                    }
                }
                count++;
            }
            return count;
        }

        private int ReadProviders()
        {
            int count = 0;
            foreach (ProviderConfigurationElement element in _config.Providers)
            {
                _process.Providers[element.Name.ToLower()] = element.Type;
                count++;
            }
            return count;
        }

        private int ReadConnections()
        {
            int count = 0;
            foreach (ConnectionConfigurationElement element in _config.Connections)
            {
                switch (element.Provider.ToLower())
                {
                    case "mysql":
                        _process.Connections.Add(element.Name,
                                                 _process.Kernal.Get<MySqlConnection>(new Libs.Ninject.Parameters.IParameter[]
                                                                                          {
                                                                                              new ConstructorArgument(
                                                                                                  "process", _process),
                                                                                              new ConstructorArgument(
                                                                                                  "element", element)
                                                                                          }));
                        break;
                    case "analysisservices":
                        _process.Connections.Add(element.Name,
                                                 _process.Kernal.Get<AnalysisServicesConnection>(new Libs.Ninject.Parameters.IParameter[]
                                                                                                     {
                                                                                                         new ConstructorArgument
                                                                                                             ("process",
                                                                                                              _process),
                                                                                                         new ConstructorArgument
                                                                                                             ("element",
                                                                                                              element)
                                                                                                     }));
                        break;
                    default:
                        _process.Connections.Add(element.Name,
                                                 _process.Kernal.Get<SqlServerConnection>(new Libs.Ninject.Parameters.IParameter[]
                                                                                              {
                                                                                                  new ConstructorArgument
                                                                                                      ("process",
                                                                                                       _process),
                                                                                                  new ConstructorArgument
                                                                                                      ("element",
                                                                                                       element)
                                                                                              }));
                        break;
                }
                count++;
            }
            return count;
        }


        private void GuardAgainstFieldOverlap(Entity entity)
        {
            var entityKeys = new HashSet<string>(entity.Fields.ToEnumerable().Where(f => f.Output).Select(f => f.Alias));
            var processKeys =
                new HashSet<string>(
                    _process.Entities.SelectMany(e2 => e2.Fields.ToEnumerable())
                            .Where(f => f.Output)
                            .Select(f => f.Alias));
            entityKeys.IntersectWith(processKeys);

            if (!entityKeys.Any()) return;

            int count = entityKeys.Count;
            _log.Error(
                "field overlap error in {3}.  The field{1}: {0} {2} already defined in previous entities.  You must alias (rename) these.",
                string.Join(", ", entityKeys), count == 1 ? string.Empty : "s", count == 1 ? "is" : "are", entity.Alias);
            Environment.Exit(0);
        }

        private void SetupRazorTemplateService()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(_process.TemplateContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);
            _log.Debug("Set RazorEngine to {0} content type.", _process.TemplateContentType);
        }
    }
}