/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;
using Transformalize.Providers;
using Transformalize.Providers.AnalysisServices;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Core.Process_
{

    public class ProcessReader : IConfigurationReader<Process>
    {
        private readonly string _processName = string.Empty;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ConversionFactory _conversionFactory = new ConversionFactory();
        private Process _process;
        private readonly ProcessConfigurationElement _config;
        private int _connectionCount;
        private int _mapCount;
        private int _entityCount;
        private int _relationshipCount;
        private int _transformCount;
        private int _scriptCount;
        private int _templateCount;
        public int Count { get { return 1; } }
        
        public ProcessReader(ProcessConfigurationElement process)
        {
            _config = process;
        }

        public ProcessReader(string processName)
        {
            _processName = processName;
            _config = ((TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize")).Processes.Get(processName);
        }

        public Process Read()
        {
            if (_config == null)
            {
                _log.Error("Sorry.  I can't find a process named {0}.", _processName);
                return new Process();
            }

            _process = new Process(_config.Name);

            _connectionCount = ReadConnections();
            _scriptCount = ReadScripts();
            _mapCount = ReadMaps();
            _entityCount = ReadEntities();
            _relationshipCount = ReadRelationships();
            _transformCount = ReadProcessTransforms();
            _process.RelatedKeys = ReadRelatedKeys();
            _process.View = _process.MasterEntity.OutputName() + "Star";

            foreach (var entity in _process.Entities)
            {
                entity.RelationshipToMaster = ReadRelationshipToMaster(entity);
            }

            LogProcessConfiguration();

            _templateCount = ReadTemplates();
            return _process;
        }

        private int ReadScripts()
        {
            var s = new[] { '\\' };
            var scripts = _config.Scripts.Cast<ScriptConfigurationElement>().ToArray();
            var path = _config.Scripts.Path;
            foreach (var script in scripts)
            {
                var fileInfo = new FileInfo(path.TrimEnd(s) + @"\" + script.File);
                if (!fileInfo.Exists)
                {
                    _log.Warn("Missing Script: {0}.", fileInfo.FullName);
                }
                else
                {
                    _process.Scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);
                    _log.Debug("Loaded script {0}.", fileInfo.FullName);
                }

            }

            return scripts.Count();
        }

        private int ReadTemplates()
        {
            var s = new[] { '\\' };
            var templates = _config.Templates.Cast<TemplateConfigurationElement>().ToArray();
            var path = _config.Templates.Path;
            foreach (var element in templates)
            {

                var fileInfo = new FileInfo(path.TrimEnd(s) + @"\" + element.File);
                if (!fileInfo.Exists)
                {
                    _log.Warn("Missing Template {0}.", fileInfo.FullName);
                }
                else
                {
                    var template = new Template(element.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName, _config);

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

                        if (!String.IsNullOrEmpty(action.Connection) && _process.Connections.ContainsKey(action.Connection))
                        {
                            templateAction.Connection = _process.Connections[action.Connection];
                        }

                        template.Actions.Add(templateAction);
                    }

                    _process.Templates[element.Name] = template;
                    _log.Debug("Loaded template {0} with {1} setting{2}.", fileInfo.FullName, template.Settings.Count, template.Settings.Count == 1 ? string.Empty : "s");
                }

            }

            return templates.Count();
        }

        private IEnumerable<Relationship> ReadRelationshipToMaster(Entity rightEntity)
        {
            var relationships = _process.Relationships.Where(r => r.RightEntity.Equals(rightEntity)).ToList();

            if (relationships.Any() && !relationships.Any(r => r.LeftEntity.IsMaster()))
            {
                var leftEntity = relationships.Last().LeftEntity;
                relationships.AddRange(ReadRelationshipToMaster(leftEntity));
            }
            return relationships;
        }

        private void LogProcessConfiguration()
        {
            _log.Debug("{0} | Process Loaded.", _process.Name);
            _log.Debug("{0} | {1} Connection{2}.", _process.Name, _connectionCount, _connectionCount == 1 ? string.Empty : "s");
            _log.Debug("{0} | {1} Entit{2}.", _process.Name, _entityCount, _entityCount == 1 ? "y" : "ies");
            _log.Debug("{0} | {1} Relationship{2}.", _process.Name, _relationshipCount, _relationshipCount == 1 ? string.Empty : "s");
            _log.Debug("{0} | {1} Script{2}.", _process.Name, _scriptCount, _scriptCount == 1 ? string.Empty : "s");
            _log.Debug("{0} | {1} Template{2}.", _process.Name, _templateCount, _templateCount == 1 ? string.Empty : "s");
            _log.Debug("{0} | {1} Map{2}.", _process.Name, _mapCount, _mapCount == 1 ? string.Empty : "s");

            _transformCount += _process.Entities.SelectMany(e => e.All.ToEnumerable()).SelectMany(f => f.Transforms).Count();
            _log.Debug("{0} | {1} Transform{2}.", _process.Name, _transformCount, _transformCount == 1 ? string.Empty : "s");
        }

        private int ReadProcessTransforms()
        {
            _process.Transforms = new ProcessTransformReader(_process, _config.Transforms).Read();
            
            foreach (var transform in _process.Transforms)
            {
                foreach (var pair in transform.Parameters)
                {
                    _process.Parameters.Add(pair.Key, pair.Value);
                }
            }

            foreach (var r in _process.Transforms.SelectMany(t => t.Results.ToEnumerable()))
            {
                _process.Results[r.Alias] = r;
            }

            return _process.Transforms.Count();
        }

        private int ReadRelationships()
        {
            var count = 0;
            foreach (RelationshipConfigurationElement r in _config.Relationships)
            {

                var leftEntity = _process.Entities.First(e => e.Name.Equals(r.LeftEntity, StringComparison.OrdinalIgnoreCase));
                var rightEntity = _process.Entities.First(e => e.Name.Equals(r.RightEntity, StringComparison.OrdinalIgnoreCase));
                var join = GetJoins(r, leftEntity, rightEntity);
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
            return new List<Join> { GetJoin(leftEntity, r.LeftField, rightEntity, r.RightField) };
        }

        public Join GetJoin(Entity leftEntity, string leftField, Entity rightEntity, string rightField)
        {

            var join = new Join
            {
                LeftField = leftEntity.All.ContainsKey(leftField) ? leftEntity.All[leftField] : leftEntity.All.ToEnumerable().First(v => v.Name.Equals(leftField, StringComparison.OrdinalIgnoreCase)),
                RightField = rightEntity.All.ContainsKey(rightField) ? rightEntity.All[rightField] : rightEntity.All.ToEnumerable().First(v => v.Name.Equals(rightField, StringComparison.OrdinalIgnoreCase))
            };

            if (join.LeftField.FieldType.HasFlag(FieldType.MasterKey) || join.LeftField.FieldType.HasFlag(FieldType.PrimaryKey))
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
            var count = 0;
            foreach (EntityConfigurationElement e in _config.Entities)
            {
                var entity = GetEntity(e, count);
                _process.Entities.Add(entity);
                if (entity.IsMaster())
                    _process.MasterEntity = entity;
                count++;
            }
            return count;
        }

        private IEnumerable<Field> ReadRelatedKeys()
        {
            var entity = _process.Entities.First(e => e.IsMaster());
            return GetRelatedKeys(entity);
        }

        private IEnumerable<Field> GetRelatedKeys(Entity entity)
        {
            var foreignKeys = entity.All.ToEnumerable().Where(f => f.FieldType.HasFlag(FieldType.ForeignKey)).ToList();
            if (foreignKeys.Any())
            {
                foreach (var alias in foreignKeys.Select(fk => fk.Alias).ToArray())
                {
                    var nextEntity = _process.Relationships.Where(r => r.LeftEntity.Name.Equals(entity.Name) && r.Join.Any(j => j.LeftField.Alias.Equals(alias))).Select(r => r.RightEntity).First();
                    foreignKeys.AddRange(GetRelatedKeys(nextEntity));
                }
            }
            return foreignKeys;
        }

        private int ReadMaps()
        {
            var count = 0;
            foreach (MapConfigurationElement m in _config.Maps)
            {
                if (string.IsNullOrEmpty(m.Connection))
                {
                    _process.MapEquals[m.Name] = new ConfigurationMapReader(m.Items, "equals").Read();
                    _process.MapStartsWith[m.Name] = new ConfigurationMapReader(m.Items, "startswith").Read();
                    _process.MapEndsWith[m.Name] = new ConfigurationMapReader(m.Items, "endswith").Read();
                }
                else
                {
                    if (_process.Connections.ContainsKey(m.Connection))
                    {
                        _process.MapEquals[m.Name] = new SqlServerMapReader(m.Items.Sql, _process.Connections[m.Connection].ConnectionString).Read();
                    }
                }
                count++;
            }
            return count;
        }

        private int ReadConnections()
        {
            var count = 0;
            foreach (ConnectionConfigurationElement element in _config.Connections)
            {
                IConnection connection;
                var type = element.Type.ToLower();
                if (type == "analysisservices" || type == "ssas")
                {
                    connection = new AnalysisServicesConnection(element.Value)
                                     {
                                         BatchSize = element.BatchSize,
                                         ConnectionType = ConnectionType.AnalysisServices,
                                         CompatibilityLevel = element.CompatabilityLevel
                                     };
                }
                else
                {
                    connection = new SqlServerConnection(element.Value, _process.Name)
                    {
                        ConnectionType = ConnectionType.SqlServer,
                        CompatibilityLevel = element.CompatabilityLevel,
                        BatchSize = element.BatchSize
                    };
                }
                _process.Connections.Add(element.Name, connection);
                count++;
            }
            return count;
        }

        private Entity GetEntity(EntityConfigurationElement e, int entityCount)
        {

            var entity = new Entity
            {
                ProcessName = _process.Name,
                Schema = e.Schema,
                Name = e.Name,
                InputConnection = _process.Connections[e.Connection],
                OutputConnection = _process.Connections["output"],
                Prefix = e.Prefix == "Default" ? e.Name.Replace(" ", string.Empty) : e.Prefix,
                Group = e.Group,
                Auto = e.Auto
            };

            if (entity.Auto)
            {
                var autoReader = new SqlServerEntityAutoFieldReader(entity, entityCount);
                entity.All = autoReader.ReadAll();
                entity.Fields = autoReader.ReadFields();
                entity.PrimaryKey = autoReader.ReadPrimaryKey();
            }

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in e.PrimaryKey)
            {
                var fieldType = entityCount == 0 ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = new FieldReader(_process, entity).Read(pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey[pk.Alias] = keyField;
                entity.All[pk.Alias] = keyField;

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in e.Fields)
            {
                var field = new FieldReader(_process, entity).Read(f);
                field.Index = fieldIndex;

                foreach (XmlConfigurationElement x in f.Xml)
                {
                    var xmlField = new Field(x.Type, x.Length, FieldType.Xml, x.Output, x.Default)
                    {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Parent = f.Name,
                        XPath = f.Xml.XPath + x.XPath,
                        Name = x.XPath,
                        Alias = x.Alias,
                        Index = x.Index,
                        Precision = x.Precision,
                        Scale = x.Scale,
                        Aggregate = x.Aggregate.ToLower()
                    };
                    xmlField.Transforms = new FieldTransformReader(_process, xmlField, x.Transforms).Read();
                    field.InnerXml.Add(x.Alias, xmlField);
                }

                if (entity.Auto && entity.Fields.ContainsKey(field.Name))
                {
                    entity.Fields.Remove(field.Name);
                    entity.All.Remove(field.Name);
                }

                entity.Fields[f.Alias] = field;
                entity.All[f.Alias] = field;

                fieldIndex++;
            }

            if (entity.All.ContainsKey(e.Version))
            {
                entity.Version = entity.All[e.Version];
            }
            else
            {
                if (entity.All.Any(kv => kv.Value.Name.Equals(e.Version, StringComparison.OrdinalIgnoreCase)))
                {
                    entity.Version = entity.All.ToEnumerable().First(v => v.Name.Equals(e.Version, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    var message = string.Format("{0} | version field reference '{1}' is undefined in {2}.", _process.Name, e.Version, e.Name);
                    _log.Error(message);
                    throw new TransformalizeException(message);
                }
            }

            var entityKeys = new HashSet<string>(entity.Fields.ToEnumerable().Where(f => f.Output).Select(f => f.Alias));
            var processKeys = new HashSet<string>(_process.Entities.SelectMany(e2 => e2.Fields.ToEnumerable()).Where(f => f.Output).Select(f => f.Alias));
            entityKeys.IntersectWith(processKeys);
            if (entityKeys.Any())
            {
                var count = entityKeys.Count;
                var message = String.Format("{0} | field overlap error.  The field{2}: {1} {3} already defined in previous entities.  You must alias (rename) these.", _process.Name, string.Join(", ", entityKeys), count == 1 ? string.Empty : "s", count == 1 ? "is" : "are");
                throw new TransformalizeException(message);
            }

            entity.Transforms = new EntityTransformReader(_process, entity, e.Transforms).Read();

            return entity;
        }

    }
}