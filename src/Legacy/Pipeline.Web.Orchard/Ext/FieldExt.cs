using System;
using System.Collections.Generic;
using System.Linq;
using Field = Transformalize.Configuration.Field;

namespace Pipeline.Web.Orchard.Ext {
   public static class FieldExt {
      public static string ToParsley(this Field f) {
         if (f.V == string.Empty)
            return string.Empty;

         var attributes = new Dictionary<string, string>();

         var expressions = new Cfg.Net.Shorthand.Expressions(f.V);
         foreach (var expression in expressions) {
            switch (expression.Method) {
               case "required":
                  attributes["data-parsley-required"] = "true";
                  switch (f.InputType) {
                     case "file":
                     case "scan":
                        attributes["data-parsley-required-message"] = "a " + f.InputType + " is required";
                        break;
                     default:
                        break;
                  }
                  break;
               case "length":
                  attributes["data-parsley-length"] = string.Format("[{0}, {1}]", expression.SingleParameter, expression.SingleParameter);
                  break;
               case "numeric":
                  attributes["data-parsley-type"] = "number";
                  break;
               case "matches":
                  attributes["data-parsley-pattern"] = expression.SingleParameter;	
                  break;
               case "min":
                  attributes["data-parsley-min"] = expression.SingleParameter;
                  break;
               case "max":
                  attributes["data-parsley-max"] = expression.SingleParameter;
                  break;
               case "is":
                  switch (expression.SingleParameter) {
                     case "int":
                     case "int32":
                        attributes["data-parsley-type"] = "integer";
                        break;
                     case "date":
                     case "datetime":
                        attributes["data-parsley-date"] = "true";
                        break;
                  }
                  break;
               case "alphanum":
                  attributes["data-parsley-type"] = "alphanum";
                  break;
               case "digits":
                  attributes["data-parsley-type"] = "digits";
                  break;
               case "email":
                  attributes["data-parsley-type"] = "email";
                  break;
               case "url":
                  attributes["data-parsley-type"] = "url";
                  break;
            }
         }


         return string.Join(" ", attributes.Select(i => string.Format("{0}=\"{1}\"", i.Key, i.Value)));
      }
   }
}