using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Transformalize.Configuration;
using Transformalize.Main.Transform;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {

    public class OperationsLoader {
        private const string DEFAULT = "[default]";
        private readonly Process _process;
        private readonly EntityElementCollection _entities;

        public OperationsLoader(ref Process process, EntityElementCollection entities) {
            _process = process;
            _entities = entities;
        }

        public void Load() {
            foreach (EntityConfigurationElement entityElement in _entities) {

                var entity = _process.Entities.First(e => e.Alias == entityElement.Alias);
                var factory = new TransformOperationFactory(_process);

                //fields can have prefixes and are limited to literal parameters (parameters with name and value provided in configuration)
                foreach (FieldConfigurationElement f in entityElement.Fields) {

                    var alias = Common.GetAlias(f, true, entityElement.Prefix);
                    var field = _process.GetField(alias, entity.Alias);

                    if (entity.TrimAll && field.Input && field.SimpleType.Equals("string")) {
                        field.Transforms.Insert(0, "trim");
                        entity.OperationsAfterAggregation.Add(new TrimOperation(field.Alias, field.Alias, " "));
                    }

                    AddShortHandTransforms(f);

                    foreach (TransformConfigurationElement t in f.Transforms) {
                        field.Transforms.Add(t.Method.ToLower());
                        var reader = new FieldParametersReader();
                        if (t.BeforeAggregation) {
                            entity.OperationsBeforeAggregation.Add(factory.Create(field, t, reader.Read(t)));
                        }
                        if (t.AfterAggregation) {
                            entity.OperationsAfterAggregation.Add(factory.Create(field, t, reader.Read(t)));
                        }
                        AddBranches(t.Branches, entity, field, reader);
                    }
                }

                // calculated fields do not have prefixes, and have access to all or some of an entity's parameters
                foreach (FieldConfigurationElement cf in entityElement.CalculatedFields) {

                    var field = _process.GetField(cf.Alias, entity.Alias);

                    AddShortHandTransforms(cf);

                    foreach (TransformConfigurationElement t in cf.Transforms) {
                        var reader = t.Parameter.Equals("*") ?
                            (ITransformParametersReader)new EntityParametersReader(entity) :
                            new EntityTransformParametersReader(entity);

                        if (t.BeforeAggregation) {
                            entity.OperationsBeforeAggregation.Add(factory.Create(field, t, reader.Read(t)));
                        }
                        if (t.AfterAggregation) {
                            entity.OperationsAfterAggregation.Add(factory.Create(field, t, reader.Read(t)));
                        }
                        AddBranches(t.Branches, entity, field, reader);
                    }
                }


            }

        }

        private static void AddShortHandTransforms(FieldConfigurationElement f) {
            if (f.ShortHand == string.Empty)
                return;
            var transforms = new List<TransformConfigurationElement>(f.ShortHand.Split(new[] { ';' }).Select(ShortHandFactory.Interpret));
            var collection = new TransformElementCollection();
            foreach (var transform in transforms) {
                collection.Add(transform);
            }
            foreach (TransformConfigurationElement transform in f.Transforms) {
                collection.Add(transform);
            }
            f.Transforms = collection;
        }

        private void AddBranches(IEnumerable branches, Entity entity, Field field, ITransformParametersReader reader) {
            foreach (BranchConfigurationElement branch in branches) {
                foreach (TransformConfigurationElement transform in branch.Transforms) {

                    Field f;
                    transform.RunField = branch.RunField.Equals(DEFAULT) ? (Common.IsValidator(transform.Method) ? (transform.ResultField.Equals(DEFAULT) ? transform.ResultField + "Result" : transform.ResultField) : field.Alias) : branch.RunField;
                    transform.RunType = _process.TryGetField(transform.RunField, entity.Name, out f) ? f.SimpleType : "boolean";
                    transform.RunOperator = branch.RunOperator;
                    transform.RunValue = branch.RunValue;

                    var operation = new TransformOperationFactory(_process).Create(field, transform, reader.Read(transform));
                    entity.OperationsAfterAggregation.Add(operation);
                    if (transform.Branches.Count > 0) {
                        AddBranches(transform.Branches, entity, field, reader);
                    }
                }
            }
        }

    }
}