using System;
using System.Collections;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Transformalize.Orchard.Models {

    public class ConfigurationPart : ContentPart<ConfigurationPartRecord> {

        private bool? _requiresInputFile;
        private bool? _requiresOutputFile;
        private Mode[] _modes;

        public string Configuration {
            get {
                if (string.IsNullOrEmpty(Record.Configuration)) {
                    return @"<transformalize>
    <environments>
    </environments>
    <processes>
        <add name=""default"">
            <connections>
                <add name=""input"" />
                <add name=""output"" />
            </connections>
            <entities>
                <add name=""e1"" />
            </entities>
        </add>
    </processes>
</transformalize>
";
                }
                return Record.Configuration;
            }
            set {
                Record.Configuration = value;
                _requiresInputFile = null;
                _requiresOutputFile = null;
            }
        }

        public string Modes {
            get { return Record.Modes; }
            set { Record.Modes = value; }
        }

        public string Title() {
            return this.As<TitlePart>().Title;
        }

        public bool TryCatch {
            get { return Record.TryCatch; }
            set { Record.TryCatch = value; }
        }

        public bool DisplayLog {
            get {
                return Record.DisplayLog;
            }
            set { Record.DisplayLog = value; }
        }

        public string LogLevel {
            get {
                return Record.LogLevel;
            }
            set { Record.LogLevel = value; }
        }

        public string StartAddress {
            get { return Record.StartAddress; }
            set { Record.StartAddress = value; }
        }

        public string EndAddress {
            get { return Record.EndAddress; }
            set { Record.EndAddress = value; }
        }

        public string EditorTheme {
            get {
                if (string.IsNullOrEmpty(Record.EditorTheme)) {
                    return "solarized";
                }
                return Record.EditorTheme;
            }
            set { Record.EditorTheme = value; }
        }

        public string OutputFileExtension {
            get {
                return string.IsNullOrEmpty(Record.OutputFileExtension) ?
                    "csv" :
                    Record.OutputFileExtension.TrimStart(new[] { '.' });
            }
            set { Record.OutputFileExtension = value; }
        }

        public IEnumerable AvailableLogLevels {
            get { return new[] { "info", "debug", "warn", "error" }; }
        }

        public IEnumerable AvailableThemes {
            get { return new[] { "3024-day", "3024-night", "ambiance-mobile", "ambiance", "base16-dark", "base16-light", "blackboard", "cobalt", "eclipse", "elegant", "erlang-dark", "lesser-dark", "mbo", "mdn-like", "midnight", "monokai", "neat", "neo", "night", "paraiso-dark", "paraiso-light", "pastel-on-dark", "rubyblue", "solarized", "the-matrix", "tomorrow-night-eighties", "twilight", "vibrant-ink", "xq-dark", "xq-light" }; }
        }

        public bool? RequiresInputFile() {
            return _requiresInputFile ?? (_requiresInputFile = Record.Configuration.Contains("@(InputFile)"));
        }

        public bool? RequiresOutputFile() {
            return _requiresOutputFile ?? (_requiresOutputFile = Record.Configuration.Contains("@(OutputFile)"));
        }

        public bool IsValid() {
            return true;
        }

        public bool HasModes() {
            return !string.IsNullOrEmpty(Modes);
        }

        public Mode[] ToModes() {
            if (_modes != null)
                return _modes;
            _modes = Modes
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(mode => mode.EndsWith("*") ? new Mode() { IsDefault = true, Name = mode.TrimEnd(new[] { '*' }) } : new Mode() { IsDefault = false, Name = mode })
                .ToArray();
            return _modes;
        }

        public EventLevel ToLogLevel() {
            switch (LogLevel) {
                case "error":
                    return EventLevel.Error;
                case "warn":
                    return EventLevel.Warning;
                case "debug":
                    return EventLevel.Verbose;
                default:
                    return EventLevel.Informational;
            }
        }

        public bool HasStartAddress() {
            return !string.IsNullOrEmpty(Record.StartAddress);
        }

        public bool HasEndAddress() {
            return !string.IsNullOrEmpty(Record.EndAddress);
        }

        public bool IsInAllowedRange(string userHostAddress) {
            if (string.IsNullOrEmpty(userHostAddress) || !HasStartAddress()) {
                return false;
            }

            IPAddress ipAddress;
            if (!IPAddress.TryParse(userHostAddress, out ipAddress)) {
                return false;
            }

            var start = IPAddress.Parse(StartAddress);
            var end = HasEndAddress() ? IPAddress.Parse(EndAddress) : start;

            return new IpAddressRange(start, end).IsInRange(ipAddress);
        }
    }
}