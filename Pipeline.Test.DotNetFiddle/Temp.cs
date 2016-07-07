#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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

using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.DotNetFiddle.Impl;

namespace Pipeline.Test.DotNetFiddle {

    [TestFixture]
    public class Temp {

        [Test]
        public void RunInit() {

            const string cfg = @"
<cfg name='test'>
	<connections>
		<add name='input' provider='internal' />
		<add name='output' provider='internal' />
	</connections>
	<entities>
		<add name='Greeting'>
			<rows>
				<add greeting='Hello Planet Earth' />
			</rows>
			<fields>
				<add name='greeting' primary-key='true'>
					<transforms>
						<add method='replace' old-value='Planet Earth' new-value='World' />
					</transforms>
				</add>
			</fields>
		</add>
	</entities>
</cfg>";

            var process = new Process(cfg);
            var controller = ControllerFactory.Create(process);
            controller.Execute();

            System.Diagnostics.Trace.WriteLine(string.Join(System.Environment.NewLine, process.Errors()));


        }


    }


}
