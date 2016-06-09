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
                RouteDescriptorWithId("Api", "Api/Cfg"),
                RouteDescriptorWithId("Api", "Api/Cfg", "Api/Configuration"),
                RouteDescriptorWithId("Api", "Api/Check"),
                RouteDescriptorWithId("Api", "Api/Run"),
                RouteDescriptor("List"),
                RouteDescriptorWithId("Report")
            };
        }

        private static RouteDescriptor RouteDescriptorWithId(string controller) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + controller + "/{id}",
                    new RouteValueDictionary {
                        {"area", "Pipeline.Web.Orchard" },
                        {"controller", controller },
                        {"action", controller},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", "Pipeline.Web.Orchard" } },
                    new MvcRouteHandler()
                    )
            };
        }

        private static RouteDescriptor RouteDescriptorWithId(string controller, string action, string alias = null) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + (alias ?? action) + "/{id}",
                    new RouteValueDictionary {
                        {"area", "Pipeline.Web.Orchard" },
                        {"controller", controller },
                        {"action", action},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", "Pipeline.Web.Orchard" } },
                    new MvcRouteHandler()
                    )
            };
        }

        private static RouteDescriptor RouteDescriptor(string controller) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + controller,
                    new RouteValueDictionary {
                        {"area", "Pipeline.Web.Orchard" },
                        {"controller", controller },
                        {"action", controller}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", "Pipeline.Web.Orchard" } },
                    new MvcRouteHandler()
                    )
            };
        }
    }
}