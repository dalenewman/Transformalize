using Cfg.Net;
using System.Collections.Generic;

namespace Pipeline.Web.Orchard.Models {
   public class MapCfg : CfgNode {
      [Cfg(value= "mapbox://styles/mapbox/streets-v10")]
      public string DefaultStyleUrl { get; set; }

      [Cfg()]
      public List<MapCfgStyle> Styles { get; set; }
     
   }

   public class MapCfgStyle : CfgNode {
      [Cfg(required=true, unique=true)]
      public string Name { get; set; }
      [Cfg(required=true)]
      public string Url { get; set; }
   }

}