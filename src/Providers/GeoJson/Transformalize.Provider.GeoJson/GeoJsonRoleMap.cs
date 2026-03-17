#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;

namespace Transformalize.Providers.GeoJson {
   internal class GeoJsonRoleMap {
      public Field IdField { get; }
      public Field LatitudeField { get; }
      public Field LongitudeField { get; }
      public Field AltitudeField { get; }
      public Field[] PropertyFields { get; }
      public double[] BBox { get; }

      public GeoJsonRoleMap(OutputContext context) {
         var fields = context.GetAllEntityFields().ToArray();

         IdField = fields.FirstOrDefault(f => RoleEquals(f, "id"));
         LatitudeField = fields.FirstOrDefault(f => RoleEquals(f, "latitude"));
         LongitudeField = fields.FirstOrDefault(f => RoleEquals(f, "longitude"));
         AltitudeField = fields.FirstOrDefault(f => RoleEquals(f, "altitude"));
         PropertyFields = fields.Where(f => RoleEquals(f, "property")).ToArray();

         if (LatitudeField == null || LongitudeField == null) {
            throw new InvalidOperationException($"GeoJson geo writer requires fields with geo='latitude' and geo='longitude' for entity '{context.Entity.Alias}'.");
         }

         if (!double.IsNaN(context.Connection.MinLat) &&
             !double.IsNaN(context.Connection.MinLon) &&
             !double.IsNaN(context.Connection.MaxLat) &&
             !double.IsNaN(context.Connection.MaxLon)) {
            BBox = new[] {
               context.Connection.MinLon,
               context.Connection.MinLat,
               context.Connection.MaxLon,
               context.Connection.MaxLat
            };
         }
      }

      private static bool RoleEquals(Field field, string role) {
         return field != null && field.Geo == role;
      }

      public static string PropertyName(Field field) {
         return string.IsNullOrEmpty(field.Label) ? field.Alias : field.Label;
      }
   }
}
