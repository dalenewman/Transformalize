using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Models {
   public class ReportViewModel {

      private Parameter[] _activeParameters;
      private Dictionary<string, Parameter> _parameterLookup;
      private Dictionary<string, Parameter> _inlines;
      private Process _process;
      private HashSet<string> _topParameters;

      public Process Process {
         get {
            return _process;
         }

         set {
            _process = value;
            _activeParameters = null;
            _topParameters = null;
            _inlines = null;
         }
      }

      public PipelineConfigurationPart Part { get; set; }

      public ReportViewModel(Process process, PipelineConfigurationPart part) {
         Process = process;
         Part = part;
      }

      public Parameter[] ActiveParameters {
         get { return _activeParameters ?? (_activeParameters = Process.Parameters.ToArray()); }
      }

      public Dictionary<string, Parameter> InlineParameters {
         get {
            if (_inlines != null) {
               return _inlines;
            }
            CalculateWhereParametersGo();
            return _inlines;
         }
      }

      private void CalculateWhereParametersGo() {

         _inlines = new Dictionary<string, Parameter>();
         _topParameters = new HashSet<string>();
         foreach (var parameter in ActiveParameters.Where(p => p.Prompt)) {
            TopParameters.Add(parameter.Name);
         }

         foreach (var field in Process.Entities.First().GetAllFields().Where(f => !f.System && f.Output)) {

            // opt out of inline field consideration
            if (field.Parameter != null && field.Parameter.Equals("None", StringComparison.OrdinalIgnoreCase)) {
               continue;
            }

            if (field.Parameter != null && ParameterLookup.ContainsKey(field.Parameter) && ParameterLookup[field.Parameter].Prompt && !ParameterLookup[field.Parameter].Required) {
               _inlines[field.Alias] = ParameterLookup[field.Parameter];
               _topParameters.Remove(field.Parameter);
            } else if (ParameterLookup.ContainsKey(field.Alias) && ParameterLookup[field.Alias].Prompt && !ParameterLookup[field.Alias].Required) {
               _inlines[field.Alias] = ParameterLookup[field.Alias];
               _topParameters.Remove(field.Alias);
            } else if (ParameterLookup.ContainsKey(field.SortField) && ParameterLookup[field.SortField].Prompt && !ParameterLookup[field.SortField].Required) {
               _inlines[field.Alias] = ParameterLookup[field.SortField];
               _topParameters.Remove(field.SortField);
            }
         }
      }

      public HashSet<string> TopParameters {
         get {
            if (_topParameters != null) {
               return _topParameters;
            }
            CalculateWhereParametersGo();
            return _topParameters;
         }
      }

      public Parameter GetParameterByName(string name) {
         return ParameterLookup[name];
      }

      public Dictionary<string, Parameter> ParameterLookup {
         get {
            if (_parameterLookup != null) {
               return _parameterLookup;
            }

            _parameterLookup = new Dictionary<string, Parameter>();
            foreach (var parameter in ActiveParameters) {
               _parameterLookup[parameter.Name] = parameter;
            }

            return _parameterLookup;
         }
      }

   }
}