using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {

    public class OperationsLoader {
        private const string DEFAULT = "[default]";
        private readonly Process _process;
        private readonly List<TflEntity> _entities;

        public OperationsLoader(ref Process process, List<TflEntity> entities) {
            _process = process;
            _entities = entities;
        }

        public void Load() {
            foreach (var entityElement in _entities) {

                var entity = _process.Entities.First(e => e.Alias == entityElement.Alias);
                var factory = new TransformOperationFactory(_process, entity.Name);

                //fields can have prefixes and are limited to literal parameters (parameters with name and value provided in configuration)
                foreach (var f in entityElement.Fields) {

                    var alias = Common.GetAlias(f, true, entityElement.Prefix);
                    var field = _process.GetField(entity.Alias, alias);

                    if (entity.TrimAll && field.Input && field.SimpleType.Equals("string")) {
                        field.Transforms.Insert(0, "trim");
                        entity.OperationsAfterAggregation.Add(new TrimOperation(field.Alias, field.Alias, " "));
                    }

                    foreach (var t in f.Transforms) {
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
                foreach (var cf in entityElement.CalculatedFields) {

                    var field = _process.GetField(entity.Alias, cf.Alias);

                    foreach (var t in cf.Transforms) {
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

        private void AddBranches(IEnumerable<TflBranch> branches, Entity entity, Field field, ITransformParametersReader reader) {
            foreach (var branch in branches) {
                foreach (var transform in branch.Transforms) {

                    Field f;
                    transform.RunField = branch.RunField;
                    transform.RunType = _process.TryGetField(entity.Name, transform.RunField, out f) ? f.SimpleType : "boolean";
                    transform.RunOperator = branch.RunOperator;
                    transform.RunValue = branch.RunValue;

                    var operation = new TransformOperationFactory(_process, entity.Name).Create(field, transform, reader.Read(transform));
                    entity.OperationsAfterAggregation.Add(operation);
                    if (transform.Branches.Count > 0) {
                        AddBranches(transform.Branches, entity, field, reader);
                    }
                }
            }
        }

    }
}