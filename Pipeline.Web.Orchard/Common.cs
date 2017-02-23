using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Cfg.Net.Ext;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Tags.Models;
using Transformalize.Configuration;
using Pipeline.Web.Orchard.Services;

namespace Pipeline.Web.Orchard {
    public static class Common {

        public const string ModuleName = "Pipeline.Web.Orchard";
        public const string ModuleGroupName = "Pipeline.NET";
        public const string PipelineFileName = "PipelineFile";
        public const string PipelineConfigurationName = "PipelineConfiguration";
        public const string PipelineSettingsName = "PipelineSettings";
        public const string InputFileIdName = "InputFileId";
        public const string InputFilePath = "InputFilePath";
        public const string InputFileName = "InputFileName";
        public const string InputFileTitleName = "InputFileTitle";
        public const string OutputFileIdName = "OutputFileId";
        public const string OutputFileName = "OutputFile";
        public const string FileFolder = "Transformalize";
        public const string ArrangementIdName = "ArrangementId";
        public const string ReturnUrlName = "ReturnUrl";
        public const string AllTag = "All";
        public const string TagFilterName = "tagFilter";

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
        <add name='separator' value=',' />
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
    <add name='template'>
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
        <add name='target' value='' />
        <add name='body' value='' />
        <add name='encode' value='true' />
      </parameters>
    </add>
    <add name='decimals'>
      <parameters>
        <add name='decimals' value='0' />
      </parameters>
    </add>
    <add name='iif'>
      <parameters>
        <add name='expression' />
        <add name='truefield' />
        <add name='falsefield' />
      </parameters>
    </add>
    <add name='geohash'>
      <parameters>
        <add name='latitude' />
        <add name='longitude' />
        <add name='length' value='6'/>
      </parameters>
    </add>
    <add name='direction'>
      <parameters>
        <add name='direction' />
      </parameters>
    </add>
    <add name='distance'>
      <parameters>
        <add name='fromlat' />
        <add name='fromlon' />
        <add name='tolat' />
        <add name='tolon' />
      </parameters>
    </add>
    <add name='humanize'>
      <parameters>
        <add name='format' value='[default]' />
      </parameters>
    </add>
    <add name='web'>
      <parameters>
        <add name='url' value='' />
        <add name='webmethod' value='GET' />
        <add name='body' value='' />
      </parameters>
    </add>
    <add name='expression'>
      <parameters>
        <add name='expression' />
      </parameters>
    </add>
  </signatures>

  <methods>
    <add name='add' signature='none' />
    <add name='abs' signature='none' />
    <add name='any' signature='any' />
    <add name='ceiling' signature='none' />
    <add name='concat' signature='none' />
    <add name='connection' signature='property' />
    <add name='contains' signature='value' />
    <add name='convert' signature='type' />
    <add name='copy' signature='none' ignore='true' />
    <add name='cs' signature='script' />
    <add name='csharp' signature='script' />
    <add name='datediff' signature='datediff' />
    <add name='datepart' signature='timecomponent' />
    <add name='decompress' signature='none' />
    <add name='equal' signature='value' />
    <add name='equals' signature='value' />
    <add name='fileext' signature='none' />
    <add name='filename' signature='file' />
    <add name='filepath' signature='file' />
    <add name='floor' signature='none' />
    <add name='format' signature='format' />
    <add name='formatphone' signature='none' />
    <add name='formatxml' signature='none' />
    <add name='hashcode' signature='none' />
    <add name='htmldecode' signature='none' />
    <add name='insert' signature='insert' />
    <add name='is' signature='type' />
    <add name='javascript' signature='script' />
    <add name='join' signature='separator' />
    <add name='js' signature='script' />
    <add name='last' signature='dayofweek' />
    <add name='left' signature='length' />
    <add name='lower' signature='none' />
    <add name='map' signature='map' />
    <add name='multiply' signature='none' />
    <add name='next' signature='dayofweek' />
    <add name='now' signature='none' />
    <add name='padleft' signature='padding' />
    <add name='padright' signature='padding' />
    <add name='razor' signature='template' />
    <add name='regexreplace' signature='regexreplace' />
    <add name='remove' signature='remove' />
    <add name='replace' signature='replace' />
    <add name='right' signature='length' />
    <add name='round' signature='decimals' />
    <add name='splitlength' signature='separator' />
    <add name='substring' signature='substring' />
    <add name='sum' signature='none' />
    <add name='timeago' signature='fromtimezone' />
    <add name='timeahead' signature='fromtimezone' />
    <add name='timezone' signature='timezone' />
    <add name='tolower' signature='none' />
    <add name='tostring' signature='format' />
    <add name='totime' signature='timecomponent' />
    <add name='toupper' signature='none' />
    <add name='toyesno' signature='none' />
    <add name='trim' signature='trim' />
    <add name='trimend' signature='trim' />
    <add name='trimstart' signature='trim' />
    <add name='upper' signature='none' />
    <add name='utcnow' signature='none' />
    <add name='velocity' signature='template' />
    <add name='xmldecode' signature='none' />
    <add name='xpath' signature='xpath' />
    <add name='in' signature='domain' />
    <add name='match' signature='pattern' />
    <add name='coalesce' signature='none' />
    <add name='startswith' signature='value' />
    <add name='endswith' signature='value' />
    <add name='invert' signature='none' />
    <add name='isdefault' signature='none' />
    <add name='isempty' signature='none' />
    <add name='tag' signature='tag' />
    <add name='include' signature='any' />
    <add name='exclude' signature='any' />
    <add name='slugify' signature='none' />

    <add name='camelize' signature='none' />
    <add name='dasherize' signature='none' />
    <add name='hyphenate' signature='none' />
    <add name='frommetric' signature='none' />
    <add name='fromroman' signature='none' />
    <add name='humanize' signature='humanize' />
    <add name='dehumanize' signature='none' />
    <add name='ordinalize' signature='none' />
    <add name='pascalize' signature='none' />
    <add name='pluralize' signature='none' />
    <add name='singularize' signature='none' />
    <add name='titleize' signature='none' />
    <add name='tometric' signature='none' />
    <add name='toordinalwords' signature='none' />
    <add name='toroman' signature='none' />
    <add name='towords' signature='none' />
    <add name='underscore' signature='none' />
    <add name='bytes' signature='none' />
    
    <add name='addticks' signature='value' />
    <add name='addmillisecnds' signature='value' />
    <add name='addseconds' signature='value' />
    <add name='addminutes' signature='value' />
    <add name='addhours' signature='value' />
    <add name='adddays' signature='value' />
    
    <add name='iif' signature='iif' />
    <add name='geohashencode' signature='geohash' />
    <add name='isnumeric' signature='none' />
    <add name='geohashneighbor' signature='direction' />
    <add name='commonprefix' signature='none' />
    <add name='commonprefixes' signature='separator' />
    <add name='distance' signature='distance' />
    
    <add name='web' signature='web' />
    <add name='urlencode' signature='none' />
    <add name='fromjson' signature='none' />

    <add name='isdaylightsavings' signature='none' />
    <add name='htmlencode' signature='none' />
    <add name='datemath' signature='expression' />
    <add name='eval' signature='expression' />
    
  </methods>

</cfg>";


        public static string CacheKey(int id, string feature) {
            return ModuleName + "." + feature + "." + id;
        }

        public static IDictionary<string, string> GetParameters(
            HttpRequestBase request,
            ISecureFileService secureFileService,
            IOrchardServices orchard
        ) {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (request != null) {
                if (request.QueryString != null) {
                    foreach (string key in request.QueryString) {
                        if (key == null)
                            continue;
                        parameters[key] = request.QueryString[key];
                    }
                }
                if (request.Form != null) {
                    foreach (string key in request.Form) {
                        if (key == null)
                            continue;
                        if (!key.Equals("__requestverificationtoken", StringComparison.OrdinalIgnoreCase)) {
                            parameters[key] = request.Form[key];
                        }
                    }
                }
            }
            if (!parameters.ContainsKey("user")) {
                var defaultUser = ConfigurationManager.AppSettings["default-user"];
                if (!string.IsNullOrEmpty(defaultUser)) {
                    parameters["user"] = defaultUser;
                }
            }
            if (!parameters.ContainsKey("password")) {
                var defaultPassword = ConfigurationManager.AppSettings["default-password"];
                if (!string.IsNullOrEmpty(defaultPassword)) {
                    parameters["password"] = defaultPassword;
                }
            }

            // handle input file
            int inputFileId;
            if (parameters.ContainsKey(Common.InputFileIdName) && int.TryParse(parameters[Common.InputFileIdName], out inputFileId)) {
                var response = secureFileService.Get(inputFileId);
                if (response.Status == 200) {
                    parameters[Common.InputFilePath] = response.Part.FullPath;
                    parameters[Common.InputFileName] = response.Part.FileName();
                    parameters[Common.InputFileTitleName] = response.Part.Title();
                } else {
                    parameters[Common.InputFilePath] = response.Message;
                    parameters[Common.InputFileName] = response.Message;
                    parameters[Common.InputFileTitleName] = response.Message;
                }
            } else {
                parameters[Common.InputFileIdName] = "0";
            }

            parameters["Orchard.User"] = orchard.WorkContext.CurrentUser == null ? string.Empty : orchard.WorkContext.CurrentUser.UserName;
            parameters["Orchard.Email"] = orchard.WorkContext.CurrentUser == null ? string.Empty : orchard.WorkContext.CurrentUser.Email;

            return parameters;
        }

        public static void TranslatePageParametersToEntities(Process process, IDictionary<string, string> parameters, string defaultOutput) {

            var output = parameters.ContainsKey("output") ? parameters["output"] : defaultOutput;

            var isOutput = Outputs.Contains(output);
            var isMode = Modes.Contains(output);

            foreach (var entity in process.Entities) {
                if (isMode) {
                    if (output == "map") {
                        entity.Page = 1;
                        entity.PageSize = 0;
                    } else {
                        if (entity.Page > 0) {
                            int page;
                            if (parameters.ContainsKey("page")) {
                                if (!int.TryParse(parameters["page"], out page)) {
                                    page = 1;
                                }
                            } else {
                                page = 1;
                            }

                            entity.Page = page;

                            var size = 0;
                            if (parameters.ContainsKey("size")) {
                                int.TryParse(parameters["size"], out size);
                            }

                            if (size > 0 || output == "page") {
                                entity.PageSize = size > 0 && entity.PageSizes.Any(s => s.Size == size) ? size : entity.PageSizes.First().Size;
                            } else {
                                entity.PageSize = size;
                            }
                        }

                    }
                } else {
                    if (isOutput) {
                        entity.Page = 0;
                        entity.PageSize = 0;
                    } else {  // unknown output request
                        entity.Page = 1;
                        entity.PageSize = 0;
                    }
                }
            }
        }

        public static readonly HashSet<string> Outputs = new HashSet<string>(new[] { "csv", "xlsx", "geojson", "kml" });
        public static readonly HashSet<string> Modes = new HashSet<string>(new[] { "map", "page", "api" });

        /// <summary>
        /// http://stackoverflow.com/questions/1029740/get-mime-type-from-filename-extension
        /// </summary>
        private static readonly IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
            {"323", "text/h323"},
            {"3g2", "video/3gpp2"},
            {"3gp", "video/3gpp"},
            {"3gp2", "video/3gpp2"},
            {"3gpp", "video/3gpp"},
            {"7z", "application/x-7z-compressed"},
            {"aa", "audio/audible"},
            {"AAC", "audio/aac"},
            {"aaf", "application/octet-stream"},
            {"aax", "audio/vnd.audible.aax"},
            {"ac3", "audio/ac3"},
            {"aca", "application/octet-stream"},
            {"accda", "application/msaccess.addin"},
            {"accdb", "application/msaccess"},
            {"accdc", "application/msaccess.cab"},
            {"accde", "application/msaccess"},
            {"accdr", "application/msaccess.runtime"},
            {"accdt", "application/msaccess"},
            {"accdw", "application/msaccess.webapplication"},
            {"accft", "application/msaccess.ftemplate"},
            {"acx", "application/internet-property-stream"},
            {"AddIn", "text/xml"},
            {"ade", "application/msaccess"},
            {"adobebridge", "application/x-bridge-url"},
            {"adp", "application/msaccess"},
            {"ADT", "audio/vnd.dlna.adts"},
            {"ADTS", "audio/aac"},
            {"afm", "application/octet-stream"},
            {"ai", "application/postscript"},
            {"aif", "audio/x-aiff"},
            {"aifc", "audio/aiff"},
            {"aiff", "audio/aiff"},
            {"air", "application/vnd.adobe.air-application-installer-package+zip"},
            {"amc", "application/x-mpeg"},
            {"application", "application/x-ms-application"},
            {"art", "image/x-jg"},
            {"asa", "application/xml"},
            {"asax", "application/xml"},
            {"ascx", "application/xml"},
            {"asd", "application/octet-stream"},
            {"asf", "video/x-ms-asf"},
            {"ashx", "application/xml"},
            {"asi", "application/octet-stream"},
            {"asm", "text/plain"},
            {"asmx", "application/xml"},
            {"aspx", "application/xml"},
            {"asr", "video/x-ms-asf"},
            {"asx", "video/x-ms-asf"},
            {"atom", "application/atom+xml"},
            {"au", "audio/basic"},
            {"avi", "video/x-msvideo"},
            {"axs", "application/olescript"},
            {"bas", "text/plain"},
            {"bcpio", "application/x-bcpio"},
            {"bin", "application/octet-stream"},
            {"bmp", "image/bmp"},
            {"c", "text/plain"},
            {"cab", "application/octet-stream"},
            {"caf", "audio/x-caf"},
            {"calx", "application/vnd.ms-office.calx"},
            {"cat", "application/vnd.ms-pki.seccat"},
            {"cc", "text/plain"},
            {"cd", "text/plain"},
            {"cdda", "audio/aiff"},
            {"cdf", "application/x-cdf"},
            {"cer", "application/x-x509-ca-cert"},
            {"chm", "application/octet-stream"},
            {"class", "application/x-java-applet"},
            {"clp", "application/x-msclip"},
            {"cmx", "image/x-cmx"},
            {"cnf", "text/plain"},
            {"cod", "image/cis-cod"},
            {"config", "application/xml"},
            {"contact", "text/x-ms-contact"},
            {"coverage", "application/xml"},
            {"cpio", "application/x-cpio"},
            {"cpp", "text/plain"},
            {"crd", "application/x-mscardfile"},
            {"crl", "application/pkix-crl"},
            {"crt", "application/x-x509-ca-cert"},
            {"cs", "text/plain"},
            {"csdproj", "text/plain"},
            {"csh", "application/x-csh"},
            {"csproj", "text/plain"},
            {"css", "text/css"},
            {"csv", "text/plain"},
            {"cur", "application/octet-stream"},
            {"cxx", "text/plain"},
            {"dat", "application/octet-stream"},
            {"datasource", "application/xml"},
            {"dbproj", "text/plain"},
            {"dcr", "application/x-director"},
            {"def", "text/plain"},
            {"deploy", "application/octet-stream"},
            {"der", "application/x-x509-ca-cert"},
            {"dgml", "application/xml"},
            {"dib", "image/bmp"},
            {"dif", "video/x-dv"},
            {"dir", "application/x-director"},
            {"disco", "text/xml"},
            {"dll", "application/x-msdownload"},
            {"dll.config", "text/xml"},
            {"dlm", "text/dlm"},
            {"doc", "application/msword"},
            {"docm", "application/vnd.ms-word.document.macroEnabled.12"},
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"dot", "application/msword"},
            {"dotm", "application/vnd.ms-word.template.macroEnabled.12"},
            {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
            {"dsp", "application/octet-stream"},
            {"dsw", "text/plain"},
            {"dtd", "text/xml"},
            {"dtsConfig", "text/xml"},
            {"dv", "video/x-dv"},
            {"dvi", "application/x-dvi"},
            {"dwf", "drawing/x-dwf"},
            {"dwp", "application/octet-stream"},
            {"dxr", "application/x-director"},
            {"eml", "message/rfc822"},
            {"emz", "application/octet-stream"},
            {"eot", "application/octet-stream"},
            {"eps", "application/postscript"},
            {"etl", "application/etl"},
            {"etx", "text/x-setext"},
            {"evy", "application/envoy"},
            {"exe", "application/octet-stream"},
            {"exe.config", "text/xml"},
            {"fdf", "application/vnd.fdf"},
            {"fif", "application/fractals"},
            {"filters", "Application/xml"},
            {"fla", "application/octet-stream"},
            {"flr", "x-world/x-vrml"},
            {"flv", "video/x-flv"},
            {"fsscript", "application/fsharp-script"},
            {"fsx", "application/fsharp-script"},
            {"generictest", "application/xml"},
            {"gif", "image/gif"},
            {"group", "text/x-ms-group"},
            {"gsm", "audio/x-gsm"},
            {"gtar", "application/x-gtar"},
            {"gz", "application/x-gzip"},
            {"h", "text/plain"},
            {"hdf", "application/x-hdf"},
            {"hdml", "text/x-hdml"},
            {"hhc", "application/x-oleobject"},
            {"hhk", "application/octet-stream"},
            {"hhp", "application/octet-stream"},
            {"hlp", "application/winhlp"},
            {"hpp", "text/plain"},
            {"hqx", "application/mac-binhex40"},
            {"hta", "application/hta"},
            {"htc", "text/x-component"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"htt", "text/webviewhtml"},
            {"hxa", "application/xml"},
            {"hxc", "application/xml"},
            {"hxd", "application/octet-stream"},
            {"hxe", "application/xml"},
            {"hxf", "application/xml"},
            {"hxh", "application/octet-stream"},
            {"hxi", "application/octet-stream"},
            {"hxk", "application/xml"},
            {"hxq", "application/octet-stream"},
            {"hxr", "application/octet-stream"},
            {"hxs", "application/octet-stream"},
            {"hxt", "text/html"},
            {"hxv", "application/xml"},
            {"hxw", "application/octet-stream"},
            {"hxx", "text/plain"},
            {"i", "text/plain"},
            {"ico", "image/x-icon"},
            {"ics", "application/octet-stream"},
            {"idl", "text/plain"},
            {"ief", "image/ief"},
            {"iii", "application/x-iphone"},
            {"inc", "text/plain"},
            {"inf", "application/octet-stream"},
            {"inl", "text/plain"},
            {"ins", "application/x-internet-signup"},
            {"ipa", "application/x-itunes-ipa"},
            {"ipg", "application/x-itunes-ipg"},
            {"ipproj", "text/plain"},
            {"ipsw", "application/x-itunes-ipsw"},
            {"iqy", "text/x-ms-iqy"},
            {"isp", "application/x-internet-signup"},
            {"ite", "application/x-itunes-ite"},
            {"itlp", "application/x-itunes-itlp"},
            {"itms", "application/x-itunes-itms"},
            {"itpc", "application/x-itunes-itpc"},
            {"IVF", "video/x-ivf"},
            {"jar", "application/java-archive"},
            {"java", "application/octet-stream"},
            {"jck", "application/liquidmotion"},
            {"jcz", "application/liquidmotion"},
            {"jfif", "image/pjpeg"},
            {"jnlp", "application/x-java-jnlp-file"},
            {"jpb", "application/octet-stream"},
            {"jpe", "image/jpeg"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"js", "application/x-javascript"},
            {"json", "text/json"},
            {"jsx", "text/jscript"},
            {"jsxbin", "text/plain"},
            {"latex", "application/x-latex"},
            {"library-ms", "application/windows-library+xml"},
            {"lit", "application/x-ms-reader"},
            {"loadtest", "application/xml"},
            {"lpk", "application/octet-stream"},
            {"lsf", "video/x-la-asf"},
            {"lst", "text/plain"},
            {"lsx", "video/x-la-asf"},
            {"lzh", "application/octet-stream"},
            {"m13", "application/x-msmediaview"},
            {"m14", "application/x-msmediaview"},
            {"m1v", "video/mpeg"},
            {"m2t", "video/vnd.dlna.mpeg-tts"},
            {"m2ts", "video/vnd.dlna.mpeg-tts"},
            {"m2v", "video/mpeg"},
            {"m3u", "audio/x-mpegurl"},
            {"m3u8", "audio/x-mpegurl"},
            {"m4a", "audio/m4a"},
            {"m4b", "audio/m4b"},
            {"m4p", "audio/m4p"},
            {"m4r", "audio/x-m4r"},
            {"m4v", "video/x-m4v"},
            {"mac", "image/x-macpaint"},
            {"mak", "text/plain"},
            {"man", "application/x-troff-man"},
            {"manifest", "application/x-ms-manifest"},
            {"map", "text/plain"},
            {"master", "application/xml"},
            {"mda", "application/msaccess"},
            {"mdb", "application/x-msaccess"},
            {"mde", "application/msaccess"},
            {"mdp", "application/octet-stream"},
            {"me", "application/x-troff-me"},
            {"mfp", "application/x-shockwave-flash"},
            {"mht", "message/rfc822"},
            {"mhtml", "message/rfc822"},
            {"mid", "audio/mid"},
            {"midi", "audio/mid"},
            {"mix", "application/octet-stream"},
            {"mk", "text/plain"},
            {"mmf", "application/x-smaf"},
            {"mno", "text/xml"},
            {"mny", "application/x-msmoney"},
            {"mod", "video/mpeg"},
            {"mov", "video/quicktime"},
            {"movie", "video/x-sgi-movie"},
            {"mp2", "video/mpeg"},
            {"mp2v", "video/mpeg"},
            {"mp3", "audio/mpeg"},
            {"mp4", "video/mp4"},
            {"mp4v", "video/mp4"},
            {"mpa", "video/mpeg"},
            {"mpe", "video/mpeg"},
            {"mpeg", "video/mpeg"},
            {"mpf", "application/vnd.ms-mediapackage"},
            {"mpg", "video/mpeg"},
            {"mpp", "application/vnd.ms-project"},
            {"mpv2", "video/mpeg"},
            {"mqv", "video/quicktime"},
            {"ms", "application/x-troff-ms"},
            {"msi", "application/octet-stream"},
            {"mso", "application/octet-stream"},
            {"mts", "video/vnd.dlna.mpeg-tts"},
            {"mtx", "application/xml"},
            {"mvb", "application/x-msmediaview"},
            {"mvc", "application/x-miva-compiled"},
            {"mxp", "application/x-mmxp"},
            {"nc", "application/x-netcdf"},
            {"nsc", "video/x-ms-asf"},
            {"nws", "message/rfc822"},
            {"ocx", "application/octet-stream"},
            {"oda", "application/oda"},
            {"odc", "text/x-ms-odc"},
            {"odh", "text/plain"},
            {"odl", "text/plain"},
            {"odp", "application/vnd.oasis.opendocument.presentation"},
            {"ods", "application/oleobject"},
            {"odt", "application/vnd.oasis.opendocument.text"},
            {"one", "application/onenote"},
            {"onea", "application/onenote"},
            {"onepkg", "application/onenote"},
            {"onetmp", "application/onenote"},
            {"onetoc", "application/onenote"},
            {"onetoc2", "application/onenote"},
            {"orderedtest", "application/xml"},
            {"osdx", "application/opensearchdescription+xml"},
            {"p10", "application/pkcs10"},
            {"p12", "application/x-pkcs12"},
            {"p7b", "application/x-pkcs7-certificates"},
            {"p7c", "application/pkcs7-mime"},
            {"p7m", "application/pkcs7-mime"},
            {"p7r", "application/x-pkcs7-certreqresp"},
            {"p7s", "application/pkcs7-signature"},
            {"pbm", "image/x-portable-bitmap"},
            {"pcast", "application/x-podcast"},
            {"pct", "image/pict"},
            {"pcx", "application/octet-stream"},
            {"pcz", "application/octet-stream"},
            {"pdf", "application/pdf"},
            {"pfb", "application/octet-stream"},
            {"pfm", "application/octet-stream"},
            {"pfx", "application/x-pkcs12"},
            {"pgm", "image/x-portable-graymap"},
            {"pic", "image/pict"},
            {"pict", "image/pict"},
            {"pkgdef", "text/plain"},
            {"pkgundef", "text/plain"},
            {"pko", "application/vnd.ms-pki.pko"},
            {"pls", "audio/scpls"},
            {"pma", "application/x-perfmon"},
            {"pmc", "application/x-perfmon"},
            {"pml", "application/x-perfmon"},
            {"pmr", "application/x-perfmon"},
            {"pmw", "application/x-perfmon"},
            {"png", "image/png"},
            {"pnm", "image/x-portable-anymap"},
            {"pnt", "image/x-macpaint"},
            {"pntg", "image/x-macpaint"},
            {"pnz", "image/png"},
            {"pot", "application/vnd.ms-powerpoint"},
            {"potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
            {"potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
            {"ppa", "application/vnd.ms-powerpoint"},
            {"ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
            {"ppm", "image/x-portable-pixmap"},
            {"pps", "application/vnd.ms-powerpoint"},
            {"ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
            {"ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {"ppt", "application/vnd.ms-powerpoint"},
            {"pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
            {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {"prf", "application/pics-rules"},
            {"prm", "application/octet-stream"},
            {"prx", "application/octet-stream"},
            {"ps", "application/postscript"},
            {"psc1", "application/PowerShell"},
            {"psd", "application/octet-stream"},
            {"psess", "application/xml"},
            {"psm", "application/octet-stream"},
            {"psp", "application/octet-stream"},
            {"pub", "application/x-mspublisher"},
            {"pwz", "application/vnd.ms-powerpoint"},
            {"qht", "text/x-html-insertion"},
            {"qhtm", "text/x-html-insertion"},
            {"qt", "video/quicktime"},
            {"qti", "image/x-quicktime"},
            {"qtif", "image/x-quicktime"},
            {"qtl", "application/x-quicktimeplayer"},
            {"qxd", "application/octet-stream"},
            {"ra", "audio/x-pn-realaudio"},
            {"ram", "audio/x-pn-realaudio"},
            {"rar", "application/octet-stream"},
            {"ras", "image/x-cmu-raster"},
            {"rat", "application/rat-file"},
            {"rc", "text/plain"},
            {"rc2", "text/plain"},
            {"rct", "text/plain"},
            {"rdlc", "application/xml"},
            {"resx", "application/xml"},
            {"rf", "image/vnd.rn-realflash"},
            {"rgb", "image/x-rgb"},
            {"rgs", "text/plain"},
            {"rm", "application/vnd.rn-realmedia"},
            {"rmi", "audio/mid"},
            {"rmp", "application/vnd.rn-rn_music_package"},
            {"roff", "application/x-troff"},
            {"rpm", "audio/x-pn-realaudio-plugin"},
            {"rqy", "text/x-ms-rqy"},
            {"rtf", "application/rtf"},
            {"rtx", "text/richtext"},
            {"ruleset", "application/xml"},
            {"s", "text/plain"},
            {"safariextz", "application/x-safari-safariextz"},
            {"scd", "application/x-msschedule"},
            {"sct", "text/scriptlet"},
            {"sd2", "audio/x-sd2"},
            {"sdp", "application/sdp"},
            {"sea", "application/octet-stream"},
            {"searchConnector-ms", "application/windows-search-connector+xml"},
            {"setpay", "application/set-payment-initiation"},
            {"setreg", "application/set-registration-initiation"},
            {"settings", "application/xml"},
            {"sgimb", "application/x-sgimb"},
            {"sgml", "text/sgml"},
            {"sh", "application/x-sh"},
            {"shar", "application/x-shar"},
            {"shtml", "text/html"},
            {"sit", "application/x-stuffit"},
            {"sitemap", "application/xml"},
            {"skin", "application/xml"},
            {"sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
            {"sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
            {"slk", "application/vnd.ms-excel"},
            {"sln", "text/plain"},
            {"slupkg-ms", "application/x-ms-license"},
            {"smd", "audio/x-smd"},
            {"smi", "application/octet-stream"},
            {"smx", "audio/x-smd"},
            {"smz", "audio/x-smd"},
            {"snd", "audio/basic"},
            {"snippet", "application/xml"},
            {"snp", "application/octet-stream"},
            {"sol", "text/plain"},
            {"sor", "text/plain"},
            {"spc", "application/x-pkcs7-certificates"},
            {"spl", "application/futuresplash"},
            {"src", "application/x-wais-source"},
            {"srf", "text/plain"},
            {"SSISDeploymentManifest", "text/xml"},
            {"ssm", "application/streamingmedia"},
            {"sst", "application/vnd.ms-pki.certstore"},
            {"stl", "application/vnd.ms-pki.stl"},
            {"sv4cpio", "application/x-sv4cpio"},
            {"sv4crc", "application/x-sv4crc"},
            {"svc", "application/xml"},
            {"swf", "application/x-shockwave-flash"},
            {"t", "application/x-troff"},
            {"tar", "application/x-tar"},
            {"tcl", "application/x-tcl"},
            {"testrunconfig", "application/xml"},
            {"testsettings", "application/xml"},
            {"tex", "application/x-tex"},
            {"texi", "application/x-texinfo"},
            {"texinfo", "application/x-texinfo"},
            {"tgz", "application/x-compressed"},
            {"thmx", "application/vnd.ms-officetheme"},
            {"thn", "application/octet-stream"},
            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},
            {"tlh", "text/plain"},
            {"tli", "text/plain"},
            {"toc", "application/octet-stream"},
            {"tr", "application/x-troff"},
            {"trm", "application/x-msterminal"},
            {"trx", "application/xml"},
            {"ts", "video/vnd.dlna.mpeg-tts"},
            {"tsv", "text/tab-separated-values"},
            {"ttf", "application/octet-stream"},
            {"tts", "video/vnd.dlna.mpeg-tts"},
            {"txt", "text/plain"},
            {"u32", "application/octet-stream"},
            {"uls", "text/iuls"},
            {"user", "text/plain"},
            {"ustar", "application/x-ustar"},
            {"vb", "text/plain"},
            {"vbdproj", "text/plain"},
            {"vbk", "video/mpeg"},
            {"vbproj", "text/plain"},
            {"vbs", "text/vbscript"},
            {"vcf", "text/x-vcard"},
            {"vcproj", "Application/xml"},
            {"vcs", "text/plain"},
            {"vcxproj", "Application/xml"},
            {"vddproj", "text/plain"},
            {"vdp", "text/plain"},
            {"vdproj", "text/plain"},
            {"vdx", "application/vnd.ms-visio.viewer"},
            {"vml", "text/xml"},
            {"vscontent", "application/xml"},
            {"vsct", "text/xml"},
            {"vsd", "application/vnd.visio"},
            {"vsi", "application/ms-vsi"},
            {"vsix", "application/vsix"},
            {"vsixlangpack", "text/xml"},
            {"vsixmanifest", "text/xml"},
            {"vsmdi", "application/xml"},
            {"vspscc", "text/plain"},
            {"vss", "application/vnd.visio"},
            {"vsscc", "text/plain"},
            {"vssettings", "text/xml"},
            {"vssscc", "text/plain"},
            {"vst", "application/vnd.visio"},
            {"vstemplate", "text/xml"},
            {"vsto", "application/x-ms-vsto"},
            {"vsw", "application/vnd.visio"},
            {"vsx", "application/vnd.visio"},
            {"vtx", "application/vnd.visio"},
            {"wav", "audio/wav"},
            {"wave", "audio/wav"},
            {"wax", "audio/x-ms-wax"},
            {"wbk", "application/msword"},
            {"wbmp", "image/vnd.wap.wbmp"},
            {"wcm", "application/vnd.ms-works"},
            {"wdb", "application/vnd.ms-works"},
            {"wdp", "image/vnd.ms-photo"},
            {"webarchive", "application/x-safari-webarchive"},
            {"webtest", "application/xml"},
            {"wiq", "application/xml"},
            {"wiz", "application/msword"},
            {"wks", "application/vnd.ms-works"},
            {"WLMP", "application/wlmoviemaker"},
            {"wlpginstall", "application/x-wlpg-detect"},
            {"wlpginstall3", "application/x-wlpg3-detect"},
            {"wm", "video/x-ms-wm"},
            {"wma", "audio/x-ms-wma"},
            {"wmd", "application/x-ms-wmd"},
            {"wmf", "application/x-msmetafile"},
            {"wml", "text/vnd.wap.wml"},
            {"wmlc", "application/vnd.wap.wmlc"},
            {"wmls", "text/vnd.wap.wmlscript"},
            {"wmlsc", "application/vnd.wap.wmlscriptc"},
            {"wmp", "video/x-ms-wmp"},
            {"wmv", "video/x-ms-wmv"},
            {"wmx", "video/x-ms-wmx"},
            {"wmz", "application/x-ms-wmz"},
            {"wpl", "application/vnd.ms-wpl"},
            {"wps", "application/vnd.ms-works"},
            {"wri", "application/x-mswrite"},
            {"wrl", "x-world/x-vrml"},
            {"wrz", "x-world/x-vrml"},
            {"wsc", "text/scriptlet"},
            {"wsdl", "text/xml"},
            {"wvx", "video/x-ms-wvx"},
            {"x", "application/directx"},
            {"xaf", "x-world/x-vrml"},
            {"xaml", "application/xaml+xml"},
            {"xap", "application/x-silverlight-app"},
            {"xbap", "application/x-ms-xbap"},
            {"xbm", "image/x-xbitmap"},
            {"xdr", "text/plain"},
            {"xht", "application/xhtml+xml"},
            {"xhtml", "application/xhtml+xml"},
            {"xla", "application/vnd.ms-excel"},
            {"xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
            {"xlc", "application/vnd.ms-excel"},
            {"xld", "application/vnd.ms-excel"},
            {"xlk", "application/vnd.ms-excel"},
            {"xll", "application/vnd.ms-excel"},
            {"xlm", "application/vnd.ms-excel"},
            {"xls", "application/vnd.ms-excel"},
            {"xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {"xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
            {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"xlt", "application/vnd.ms-excel"},
            {"xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
            {"xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {"xlw", "application/vnd.ms-excel"},
            {"xml", "text/xml"},
            {"xmta", "application/xml"},
            {"xof", "x-world/x-vrml"},
            {"XOML", "text/plain"},
            {"xpm", "image/x-xpixmap"},
            {"xps", "application/vnd.ms-xpsdocument"},
            {"xrm-ms", "text/xml"},
            {"xsc", "application/xml"},
            {"xsd", "text/xml"},
            {"xsf", "text/xml"},
            {"xsl", "text/xml"},
            {"xslt", "text/xml"},
            {"xsn", "application/octet-stream"},
            {"xss", "application/xml"},
            {"xtp", "application/octet-stream"},
            {"xwd", "image/x-xwindowdump"},
            {"z", "application/x-compress"},
            {"zip", "application/x-zip-compressed"}
        };

        public static string GetMimeType(string extension) {
            if (string.IsNullOrEmpty(extension)) {
                return "application/octet-stream";
            }

            if (extension.StartsWith(".")) {
                extension = extension.Substring(1);
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        /// <summary>
        /// Geez how to you get a distinct list of tags used on a particular content type (without writing sql)
        /// </summary>
        /// <typeparam name="TPart"></typeparam>
        /// <typeparam name="TRecord"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEnumerable<string> Tags<TPart, TRecord>(IOrchardServices services)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord {
            return services.ContentManager.Query<TPart, TRecord>(VersionOptions.Published)
                .Join<TagsPartRecord>()
                .Where(tpr => tpr.Tags.Any())
                .List()
                .SelectMany(pfp => pfp.As<TagsPart>().CurrentTags)
                .Distinct()
                .Union(new[] { Common.AllTag })
                .OrderBy(s => s);
        }


        public static void ApplyFacet(Process process, HttpRequestBase request) {
            // auto facet
            foreach (var entity in process.Entities) {
                foreach (var field in entity.Fields.Where(f => f.Facet)) {
                    if (process.Maps.All(m => m.Name != field.Alias)) {
                        process.Maps.Add(new Map { Name = field.Alias });
                    }
                    var parameters = process.GetActiveParameters();
                    if (parameters.All(pr => pr.Name != field.Alias)) {
                        parameters.Add(
                            new Parameter {
                                Name = field.Alias,
                                Type = field.Type,
                                Label = field.Label,
                                Prompt = true,
                                Value = "*",
                                Map = field.Alias
                            }
                        );
                    }
                    if (entity.Filter.All(f => f.Field != field.Name)) {
                        entity.Filter.Add(new Filter {
                            Field = field.Name,
                            LeftField = field,
                            IsField = true,
                            Type = "facet",
                            Map = field.Alias,
                            Value = request.QueryString[field.Alias] ?? "*"
                        });
                    }
                }
            }

        }
    }
}