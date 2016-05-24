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
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineConfigurationPart : ContentPart<PipelineConfigurationPartRecord> {

        public string Configuration {
            get {
                if (string.IsNullOrEmpty(Record.Configuration)) {
                    return @"<process name='me' environment='default'>
    <environments>
        <add name='default'>
            <parameters>
                <add name='suffix' value='In Production' />
            </parameters>
        </add>
        <add name='test'>
            <parameters>
                <add name='suffix' value='In Test' />
            </parameters>
        </add>
    </environments>

    <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='internal' />
    </connections>

    <entities>
        <add name='entity'>
            <rows>
                <add first='dale' last='newman' />
            </rows>
            <fields>
                <add name='first' />
                <add name='last' />
            </fields>
            <calculated-fields>
                <add name='suffix' default='@(suffix)' />
                <add name='full' t='copy(first,last,suffix).join( )' />
            </calculated-fields>
        </add>
    </entities>

</process>
";
                }
                return Record.Configuration;
            }
            set {
                Record.Configuration = value;
            }
        }

        public string Title() {
            return this.As<TitlePart>().Title;
        }

        public string StartAddress {
            get { return Record.StartAddress ?? string.Empty; }
            set { Record.StartAddress = value; }
        }

        public string EndAddress {
            get { return Record.EndAddress ?? string.Empty; }
            set { Record.EndAddress = value; }
        }

        public bool IsValid() {
            return true;
        }

    }
}