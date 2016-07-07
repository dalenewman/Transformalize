using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;
using Pipeline.Transforms.System;

namespace Pipeline.DotNetFiddle.Impl {

    // This manually composes an process controller for dotNetFiddle that reads from internal, and writes to console (at entity level)
    public class ControllerFactory {

        public static IProcessController Create(Process process, LogLevel logLevel = LogLevel.None) {

            var logger = new ConsoleLogger(logLevel);
            var pipelines = new List<IPipeline>();

            // entity-level pipelines
            foreach (var entity in process.Entities) {
                var entityContext = new PipelineContext(logger, process, entity);
                var entityPipeline = new DefaultPipeline(new NullOutputController(), entityContext);
                var entityInputContext = new InputContext(entityContext, new Incrementer(entityContext));
                var entityRowFactory = new RowFactory(entityInputContext.RowCapacity, entity.IsMaster, false);
                var entityOutputContext = new OutputContext(entityContext, new Incrementer(entityContext));

                entityPipeline.Register(new InternalReader(entityInputContext, entityRowFactory));

                entityPipeline.Register(new SetSystemFields(entityContext));
                entityPipeline.Register(new DefaultTransform(entityContext, entityContext.GetAllEntityFields()));

                // js and razor not active (for now)
                var js = new NullTransform(entityContext);
                var razor = new NullTransform(entityContext);

                entityPipeline.Register(TransformFactory.GetTransforms(process, entity, entity.GetAllFields().Where(f => f.Transforms.Any()), logger, js, razor));
                entityPipeline.Register(new StringTruncateTransfom(entityContext));

                entityPipeline.Register(new NullUpdater(entityContext, false));
                entityPipeline.Register(new ConsoleWriter(new CsvSerializer(entityOutputContext)));

                pipelines.Add(entityPipeline);
            }

            var calculatedProcess = process.ToCalculatedFieldsProcess();
            var calculatedEntity = calculatedProcess.Entities.First();
            var calculatedContext = new PipelineContext(logger, calculatedProcess, calculatedEntity);
            var calculatedPipeline = new DefaultPipeline(new NullOutputController(), calculatedContext);

            calculatedPipeline.Register(new NullUpdater(calculatedContext, false));
            calculatedPipeline.Register(new NullWriter(calculatedContext, false));  // future home of console writer that joins entities together for process output
            calculatedPipeline.Register(new NullReader(calculatedContext, false));

            calculatedPipeline.Register(new DefaultTransform(calculatedContext, calculatedEntity.CalculatedFields));
            calculatedPipeline.Register(TransformFactory.GetTransforms(calculatedProcess, calculatedEntity, calculatedEntity.CalculatedFields, logger, new NullTransform(calculatedContext), new NullTransform(calculatedContext)));
            calculatedPipeline.Register(new StringTruncateTransfom(calculatedContext));

            pipelines.Add(calculatedPipeline);
            return new ProcessController(pipelines, new PipelineContext(logger, process));
        }
    }
}
