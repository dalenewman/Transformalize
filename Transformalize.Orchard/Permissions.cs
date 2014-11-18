using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Transformalize.Orchard {

    public class Permissions : IPermissionProvider {

        public static readonly Permission Upload = new Permission() { Description = "Upload", Name = "Upload" };
        public static readonly Permission Download = new Permission() { Description = "Download", Name = "Download" };
        public static readonly Permission Execute = new Permission() { Description = "Execute", Name = "Execute"};

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] { Upload, Download, Execute };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {Upload, Download, Execute}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {Upload, Download}
                },
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions =  Enumerable.Empty<Permission>()
                }
            };
        }
    }
}


