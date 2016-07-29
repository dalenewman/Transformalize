using System;
using System.Collections.Generic;
using System.Web;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard {
    public static class Common {

        public const string ModuleName = "Pipeline.Web.Orchard";
        public const string ModuleGroupName = "Pipeline.NET";
        public const string PipelineFileName = "PipelineFile";
        public const string PipelineConfigurationName = "PipelineConfiguration";
        public const string PipelineSettingsName = "PipelineSettings";

        public const string DefaultShortHand = @"<cfg>

  <signatures>
    <add name='none' />
    <add name='format'>
      <parameters>
        <add name='format' />
      </parameters>
    </add>
    <add name='length'>
      <parameters>
        <add name='length' />
      </parameters>
    </add>
    <add name='separator'>
      <parameters>
        <add name='separator' />
      </parameters>
    </add>
    <add name='padding'>
      <parameters>
        <add name='total-width' />
        <add name='padding-char' value='0' />
      </parameters>
    </add>
    <add name='timezone'>
      <parameters>
        <add name='from-time-zone' />
        <add name='to-time-zone' />
      </parameters>
    </add>
    <add name='fromtimezone'>
      <parameters>
        <add name='from-time-zone' value='UTC' />
      </parameters>
    </add>
    <add name='value'>
      <parameters>
        <add name='value' value='[default]' />
      </parameters>
    </add>
    <add name='type'>
      <parameters>
        <add name='type' value='[default]' />
      </parameters>
    </add>
    <add name='trim'>
      <parameters>
        <add name='trim-chars' value=' ' />
      </parameters>
    </add>
    <add name='script'>
      <parameters>
        <add name='script'  />
      </parameters>
    </add>
    <add name='map'>
      <parameters>
        <add name='map' />
      </parameters>
    </add>
    <add name='dayofweek'>
      <parameters>
        <add name='dayofweek' />
      </parameters>
    </add>
    <add name='substring'>
      <parameters>
        <add name='startindex' />
        <add name='length' value='0' />
      </parameters>
    </add>
    <add name='timecomponent'>
      <parameters>
        <add name='timecomponent' />
      </parameters>
    </add>
    <add name='replace'>
      <parameters>
        <add name='oldvalue' />
        <add name='newvalue' value='' />
      </parameters>
    </add>
    <add name='regexreplace'>
      <parameters>
        <add name='pattern' />
        <add name='newvalue' />
        <add name='count' value='0' />
      </parameters>
    </add>
    <add name='pattern'>
      <parameters>
        <add name='pattern' />
      </parameters>
    </add>
    <add name='insert'>
      <parameters>
        <add name='startindex' />
        <add name='value' />
      </parameters>
    </add>
    <add name='remove'>
      <parameters>
        <add name='startindex' />
        <add name='count' value='0' />
      </parameters>
    </add>
    <add name='razor'>
      <parameters>
        <add name='template' />
        <add name='contenttype' value='raw' />
      </parameters>
    </add>
    <add name='any'>
      <parameters>
        <add name='value'/>
        <add name='operator' value='equal' />
      </parameters>
    </add>
    <add name='property'>
      <parameters>
        <add name='name' />
        <add name='property' />
      </parameters>
    </add>
    <add name='file'>
      <parameters>
        <add name='extension' value='true'/>
      </parameters>
    </add>
    <add name='xpath'>
      <parameters>
        <add name='xpath' />
        <add name='namespace' value='' />
        <add name='url' value='' />
      </parameters>
    </add>
    <add name='datediff'>
      <parameters>
        <add name='timecomponent' />
        <add name='fromtimezone' value='UTC' />
      </parameters>
    </add>
    <add name='domain'>
      <parameters>
        <add name='domain' />
      </parameters>
    </add>
    <add name='tag'>
      <parameters>
        <add name='tag' />
        <add name='class' value='' />
        <add name='style' value='' />
        <add name='title' value='' />
        <add name='href' value='' />
        <add name='role' value='' />
      </parameters>
    </add>
  </signatures>

  <targets>
    <add name='t' collection='transforms' property='method' />
    <add name='ignore' collection='' property='' />
  </targets>

  <methods>
    <add name='add' signature='none' target='t' />
    <add name='any' signature='any' target='t' />
    <add name='concat' signature='none' target='t' />
    <add name='connection' signature='property' target='t' />
    <add name='contains' signature='value' target='t' />
    <add name='convert' signature='type' target='t' />
    <add name='copy' signature='none' target='ignore' />
    <add name='cs' signature='script' target='t' />
    <add name='csharp' signature='script' target='t' />
    <add name='datediff' signature='datediff' target='t' />
    <add name='datepart' signature='timecomponent' target='t' />
    <add name='decompress' signature='none' target='t' />
    <add name='equal' signature='value' target='t' />
    <add name='equals' signature='value' target='t' />
    <add name='fileext' signature='none' target='t' />
    <add name='filename' signature='file' target='t' />
    <add name='filepath' signature='file' target='t' />
    <add name='format' signature='format' target='t' />
    <add name='formatphone' signature='none' target='t' />
    <add name='hashcode' signature='none' target='t' />
    <add name='htmldecode' signature='none' target='t' />
    <add name='insert' signature='insert' target='t' />
    <add name='is' signature='type' target='t' />
    <add name='javascript' signature='script' target='t' />
    <add name='join' signature='separator' target='t' />
    <add name='js' signature='script' target='t' />
    <add name='last' signature='dayofweek' target='t' />
    <add name='left' signature='length' target='t' />
    <add name='lower' signature='none' target='t' />
    <add name='map' signature='map' target='t' />
    <add name='multiply' signature='none' target='t' />
    <add name='next' signature='dayofweek' target='t' />
    <add name='now' signature='none' target='t' />
    <add name='padleft' signature='padding' target='t' />
    <add name='padright' signature='padding' target='t' />
    <add name='razor' signature='razor' target='t' />
    <add name='regexreplace' signature='regexreplace' target='t' />
    <add name='remove' signature='remove' target='t' />
    <add name='replace' signature='replace' target='t' />
    <add name='right' signature='length' target='t' />
    <add name='splitlength' signature='separator' target='t' />
    <add name='substring' signature='substring' target='t' />
    <add name='sum' signature='none' target='t' />
    <add name='timeago' signature='fromtimezone' target='t' />
    <add name='timeahead' signature='fromtimezone' target='t' />
    <add name='timezone' signature='timezone' target='t' />
    <add name='tolower' signature='none' target='t' />
    <add name='tostring' signature='format' target='t' />
    <add name='totime' signature='timecomponent' target='t' />
    <add name='toupper' signature='none' target='t' />
    <add name='toyesno' signature='none' target='t' />
    <add name='trim' signature='trim' target='t' />
    <add name='trimend' signature='trim' target='t' />
    <add name='trimstart' signature='trim' target='t' />
    <add name='upper' signature='none' target='t' />
    <add name='utcnow' signature='none' target='t' />
    <add name='xmldecode' signature='none' target='t' />
    <add name='xpath' signature='xpath' target='t' />
    <add name='in' signature='domain' target='t' />
    <add name='match' signature='pattern' target='t' />
    <add name='coalesce' signature='none' target='t' />
    <add name='startswith' signature='value' target='t' />
    <add name='endswith' signature='value' target='t' />
    <add name='invert' signature='none' target='t' />
    <add name='isdefault' signature='none' target='t' />
    <add name='isempty' signature='none' target='t' />
    <add name='tag' signature='tag' target='t' />
    <add name='filter' signature='any' target='t' />
  </methods>

</cfg>";



        public static string CacheKey(int id, string feature) {
            return ModuleName + "." + feature + "." + id;
        }

        public static IDictionary<string, string> GetParameters(HttpRequestBase request) {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (request != null && request.QueryString != null) {
                foreach (string key in request.QueryString) {
                    parameters[key] = request.QueryString[key];
                }
            }
            return parameters;
        }

        public static void PageHelper(Process process, HttpRequestBase request) {
            if (request.QueryString["page"] == null) {
                return;
            }

            var page = 0;
            if (!int.TryParse(request.QueryString["page"], out page) || page <= 0) {
                return;
            }

            var size = 0;
            if (!int.TryParse((request.QueryString["size"] ?? "0"), out size)) {
                return;
            }

            foreach (var entity in process.Entities) {
                entity.Page = page;
                entity.PageSize = size > 0 ? size : entity.PageSize;
            }
        }
    }
}