using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Ioc.Autofac {
    public class RecoverOutputAction : IAction {
        private readonly IContext _context;

        public RecoverOutputAction(IContext context) {
            _context = context;
        }

        public ActionResponse Execute() {

            var process = _context.Process;
            var originalOutput = process.Connections.First(c => c.Name == "original-output");
            process.Connections.Remove(originalOutput);
            originalOutput.Name = "output";

            var threshold = process.Entities.Min(e => e.BatchId) - 1;
            var reversed = new Process {
                Name = process.Name,
                System = false,
                Connections = new List<Connection>(2){
                    new Connection { Name = "input", Provider = process.InternalProvider, File = process.Output().File},
                    originalOutput
                },
                Entities = new List<Entity>(1) {
                    new Entity {
                        Name = process.InternalProvider == "sqlce" ?process.Flat : process.Star,
                        System = false,
                        CalculateHashCode = false,
                        Connection = "input",
                        Fields = process.GetStarFields().SelectMany(f => f).Select(field => new Field {
                            Name = field.Alias,
                            Alias = field.Alias,
                            Type = field.Type,
                            Input = true,
                            PrimaryKey = field.Name == Constants.TflKey
                        }).ToList(),
                        Filter = new List<Filter> {
                            new Filter {
                                Field = Constants.TflBatchId,
                                Operator = "greaterthan",
                                Value = threshold.ToString()
                            }
                        }
                    }
                }
            };

            reversed.Check();

            if (reversed.Errors().Any()) {
                foreach (var error in reversed.Errors()) {
                    _context.Error(error);
                }
                return new ActionResponse(500, "See error log");
            }

            using (var scope = DefaultContainer.Create(reversed)) {
                scope.Resolve<IProcessController>().Execute();
                if (originalOutput.Provider == "internal") {
                    process.Rows = reversed.Entities.First().Rows;
                }
            }

            return new ActionResponse(200, string.Empty);
        }
    }
}
