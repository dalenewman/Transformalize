using System;
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Main;

namespace Transformalize.Configuration.Builders {

    public class EntityBuilder : IFieldHolder {

        private readonly ProcessBuilder _processBuilder;
        private readonly TflEntity _entity;

        public EntityBuilder(ProcessBuilder processBuilder, TflEntity entity) {
            _processBuilder = processBuilder;
            _entity = entity;
        }

        public TflProcess Process() {
            return _processBuilder.Process();
        }

        public EntityBuilder Version(string version) {
            _entity.Version = version;
            return this;
        }

        public EntityBuilder Connection(string name) {
            _entity.Connection = name;
            return this;
        }

        public EntityBuilder Schema(string schema) {
            _entity.Schema = schema;
            return this;
        }

        public EntityBuilder Alias(string alias) {
            _entity.Alias = alias;
            return this;
        }

        public EntityBuilder Delete(bool delete = true) {
            _entity.Delete = delete;
            return this;
        }

        public EntityBuilder Sample(decimal sample) {
            _entity.Sample = Convert.ToInt32(sample);
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }

        public FieldBuilder Field(string name) {
            var field = new TflField { Name = name }.WithDefaults();
            _entity.Fields.Add(field);
            return new FieldBuilder(this, field);
        }

        public RelationshipBuilder Relationship() {
            return _processBuilder.Relationship();
        }

        public EntityBuilder Prefix(string prefix) {
            _entity.Prefix = prefix;
            return this;
        }

        public EntityBuilder PrependProcessNameToOutputName(bool prepend) {
            _entity.PrependProcessNameToOutputName = prepend;
            return this;
        }

        public FieldBuilder CalculatedField(string name) {
            var calculatedField = new TflField {Name = name}.WithDefaults();
            _entity.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public EntityBuilder PipelineThreading(PipelineThreading pipelineThreading) {
            _entity.PipelineThreading = pipelineThreading.ToString();
            return this;
        }

        public EntityBuilder Group(bool group = true) {
            _entity.Group = group;
            return this;
        }

        public EntityBuilder SqlKeysOverride(string sql) {
            _entity.QueryKeys = sql;
            return this;
        }

        public EntityBuilder SqlOverride(string sql) {
            _entity.Query = sql;
            return this;
        }

        public EntityBuilder SqlOverride(object script) {
            _entity.Script = script.ToString();
            return this;
        }

        public EntityBuilder DetectChanges(bool detect) {
            _entity.DetectChanges = detect;
            return this;
        }

        public EntityBuilder TrimAll(bool trimAll) {
            _entity.TrimAll = trimAll;
            return this;
        }

        public EntityBuilder NoLock(bool noLock = true) {
            _entity.NoLock = noLock;
            return this;
        }

        public IoBuilder Output(string name, string connectionName) {
            var output = new TflIo{
                Name = name,
                Connection = connectionName
            }.WithDefaults();
            _entity.Output.Add(output);
            return new IoBuilder(this, output);
        }

        public IoBuilder Input(string name, string connectionName) {
            var input = new TflIo {
                Name = name,
                Connection = connectionName
            }.WithDefaults();
            _entity.Input.Add(input);
            return new IoBuilder(this, input);
        }

    }
}