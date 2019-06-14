using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pipeline.Web.Orchard.Models {
   public class PipelineView {
      private string _label;
      private string _title;

      public PipelineView(string mode) {
         Mode = mode;
      }
      public string Mode { get; set; }
      public bool Active { get; set; }
      public string Title { get { return _title == null ? Mode + " view" : _title; } set { _title = value; } }
      public string Label { get { return _label == null ? Mode.ToUpper() : _label; } set { _label = value; }}
      public IHtmlString Link { get; set; }
      public string Glyphicon { get; set; }
   }
}