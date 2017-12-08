#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests
{

    [TestClass]
    public class Forms
    {

        [TestMethod]
        [Ignore] // requires postgresql
        public void TryForm()
        {
            const string xml = @"
    <add name='Person' mode='form'>

        <parameters>
            <add name='User' value='' />
            <add name='Password' value='' />
            <add name='FormId' type='int' value='0' />
        </parameters>

        <connections>
            <add name='input' provider='postgresql' server='localhost' database='Junk' user='@[User]' password='@[Password]' />
            <add name='output' provider='postgresql' server='localhost' database='Junk' user='@[User]' password='@[Password]' />
        </connections>

        <entities>
            <add name='Form'>
                <filter>
                    <add field='FormId' value='@[FormId]' />
                </filter>
                <fields>
                    <add name='FormId' type='int' primary-key='true' />
                    <add name='FirstName' v='required()' />
                    <add name='LastName' v='required()' />
                </fields>
                <calculated-fields>
                    <add name='Created' type='datetime' default='now' />
                    <add name='CreatedBy' default='@[User]' />
                </calculated-fields>
            </add>
        </entities>
    </add>";

            var parameters = new Dictionary<string, string> {
                { "User", "postgres" },
                { "Password", "****" },
                { "FormId", "1" },
                { "FirstName", "D" },
                { "LastName", "Pooh" }
            };
            var composer = new CompositionRoot();
            var controller = composer.Compose(xml, Transformalize.Contracts.LogLevel.Debug, parameters, "@[]");
            controller.Execute();

        }

    }
}
