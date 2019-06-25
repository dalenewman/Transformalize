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
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Pipeline.Web.Orchard {

    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                RouteDescriptorWithId("Api", "Cfg"),
                RouteDescriptorWithId("Api", "Check"),
                RouteDescriptorWithId("Api", "Run"),

                RouteDescriptor("File","Upload"),
                RouteDescriptorWithId("File", "Download"),
                RouteDescriptorWithId("File", "Delete"),
                RouteDescriptorWithId("File", "View"),
                RouteDescriptorWithTagFilter("File", "List"),

                RouteDescriptorWithId("Cfg", "Download"),
                RouteDescriptorWithTagFilter("Cfg", "List"),

                RouteDescriptorWithId("HandsOnTable", null),
                RouteDescriptorWithId("HandsOnTable","Load"),
                RouteDescriptorWithId("HandsOnTable","Save"),

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/Report/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Report" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/Form/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Form" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/FormContent/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Form" },
                            {"action", "Content"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/Export/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Export" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                // not url("~/Pipeline/Headless*") in layer rule, set priority lower than the main one
                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "Pipeline/Headless/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Report" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "Pipeline/Headless/Report/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Report" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "Pipeline/Headless/Map/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Map" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "Pipeline/Headless/Calendar/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Calendar" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                        "Pipeline/Map/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Map" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                        "Pipeline/Calendar/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Calendar" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/Action/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "Action" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                },

                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        "Pipeline/PivotTable/{id}",
                        new RouteValueDictionary {
                            {"area", Common.ModuleName },
                            {"controller", "PivotTable" },
                            {"action", "Index"},
                            {"id", 0}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { { "area", Common.ModuleName } },
                        new MvcRouteHandler()
                    )
                }

            };
        }

        private static RouteDescriptor RouteDescriptorWithId(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    action == null ? "Pipeline/" + controller + "/{id}" : "Pipeline/" + (controller == "Cfg" ? string.Empty : controller + "/") + action + "/{id}",
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action ?? "Index"},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }

        private static RouteDescriptor RouteDescriptor(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + controller + (controller == action ? string.Empty : "/" + action),
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }


        private static RouteDescriptor RouteDescriptorWithTagFilter(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + (controller == "Cfg" ? string.Empty : controller + "/") + action + "/{tagFilter}",
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action},
                        {"tagFilter", Common.AllTag}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }

    }
}