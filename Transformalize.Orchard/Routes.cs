using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Transformalize.Orchard {

    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {

                RouteDescriptor("Transformalize", "Execute"),
                RouteDescriptor("Api", "Api/Configuration"),
                RouteDescriptor("Api", "Api/Execute"),
                RouteDescriptor("Api", "Api/MetaData"),
                RouteDescriptor("File", "Files"),
                RouteDescriptor("File", "Upload"),
                RouteDescriptor("File", "Download"),
                RouteDescriptor("File", "Delete"),

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "Transformalize" },
                        {"action", "Configurations"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/File/Upload",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "File" },
                        {"action", "Upload"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/HandsOnTable/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "HandsOnTable" },
                        {"action", "Index"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/HandsOnTable/Load/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "HandsOnTable" },
                        {"action", "Load"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/HandsOnTable/Save/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "HandsOnTable" },
                        {"action", "Save"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                },

                // Legacy configuration path, new is Transformalize/Api/Configuration/{id}
                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/Configuration/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", "Api" },
                        {"action", "Api/Configuration"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { {"area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
                }

                };
        }

        private static RouteDescriptor RouteDescriptor(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Transformalize/" + action + "/{id}",
                    new RouteValueDictionary {
                        {"area", "Transformalize.Orchard" },
                        {"controller", controller },
                        {"action", action},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", "Transformalize.Orchard" } },
                    new MvcRouteHandler()
                    )
            };
        }
    }
}