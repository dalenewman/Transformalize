#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestTimeAgoTransform {

      [TestMethod]
      public void TimeSummary() {

         DateTime utc = DateTime.UtcNow;
         Console.WriteLine(utc.ToString("yyyy-MM-ddTHH:mm:ssZ"));
         Console.WriteLine(utc.ToString("yyyy-MM-ddTHH:mm:ss"));
         string timeZone = string.Empty;

         // Use cross-platform timezone ID: Windows uses "Eastern Standard Time", Linux/macOS uses "America/New_York"
         TimeZoneInfo estZone;
         try {
            estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            timeZone = "Eastern Standard Time";
         } catch (TimeZoneNotFoundException) {
            estZone = TimeZoneInfo.FindSystemTimeZoneById("America/Detroit");
            timeZone = "America/Detroit";
         }
         DateTime est = TimeZoneInfo.ConvertTimeFromUtc(utc, estZone);
         Console.WriteLine(est.ToString("yyyy-MM-dd HH:mm:ss zzz"));
         Console.WriteLine(est.ToString("yyyy-MM-dd HH:mm:ss"));

         string xml = $@"
    <add name='TestProcess'>
      <entities>
        <add name='TestData'>
          <rows>
            <add utcdate='{utc.AddMinutes(-30.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddMinutes(-30.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddMinutes(-30.1).ToString("yyyy-MM-ddTHH:mm:ss")}' estdate='{est.AddMinutes(-30.1).ToString("yyyy-MM-dd HH:mm:ss")}' />
            <add utcdate='{utc.AddHours(2.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddHours(2.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddSeconds(-29.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddSeconds(-29.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddSeconds(-75.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddSeconds(-75.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddMinutes(-65.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddMinutes(-65.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddHours(36.1).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddHours(36.1).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddDays(62).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddDays(62).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddDays(366).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddDays(366).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{utc.AddDays(731).ToString("yyyy-MM-ddTHH:mm:ssZ")}' estdate='{est.AddDays(731).ToString("yyyy-MM-dd HH:mm:ss zzz")}' />
            <add utcdate='{new DateTime(9999,12,31,23,59,59, DateTimeKind.Utc)}' estdate='{new DateTime(9999,12,31,23,59,59, DateTimeKind.Local)}' /> 
         </rows>
          <fields>
            <add name='utcdate' type='datetime' />
            <add name='estdate' type='datetime' />
          </fields>
          <calculated-fields>
            <add name='utc1' t='copy(utcdate).timeAgo()' />
            <add name='est1' t='copy(estdate).timeAgo()' />
            <add name='est2' t='copy(estdate).timeAgo({timeZone})' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, logger)) {

               var results = inner.Resolve<IProcessController>().Read().ToArray();

               var utc1 = process.Entities[0].CalculatedFields[0];
               var est1 = process.Entities[0].CalculatedFields[1];
               var est2 = process.Entities[0].CalculatedFields[2];

               Assert.AreEqual("30 minutes ago", results[0][utc1], "A marked UTC date minus 30+ minutes");
               Assert.AreEqual("30 minutes ago", results[0][est1], "A marked EST date minus 30+ minutes");
               Assert.AreEqual(est.IsDaylightSavingTime() ? "3 hours" : "4 hours", results[0][est2], "A marked EST date with fromTimeZone also EST sees a difference of 4 or 5 hours minus 30+ minutes, and results in 3:30 and 4:30 rounded down to 3 and 4 hours.");

               Assert.AreEqual("30 minutes ago", results[1][utc1], "An unmarked UTC date (no Z or offset) minus 30+ minutes");
               Assert.AreNotEqual("30 minutes ago", results[1][est1], "An unmarked EST date (no offset) without from time zone set minus 30+ minutes");
               Assert.AreEqual("30 minutes ago", results[1][est2], "An unmarked EST date (no offset) with fromTimeZone EST minus 30+ minutes");

               Assert.AreEqual("2 hours", results[2][utc1], "A marked UTC date plus 2+ hours");
               Assert.AreEqual("2 hours", results[2][est1], "A marked EST date plus 2+ hours");
               Assert.AreEqual(est.IsDaylightSavingTime() ? "6 hours" : "7 hours", results[2][est2], "A marked EST date with fromTimeZone also EST sees a difference of 4 or 5 hours plus 2+ hours, and results in 6 or 7 hours");

               Assert.IsTrue("29 seconds ago" == (string)results[3][utc1] || "30 seconds ago" == (string)results[3][utc1], "A marked UTC date minus 29+ seconds");  
               Assert.AreEqual("a minute ago", results[4][utc1], "A marked UTC date minus 75+ seconds");
               Assert.AreEqual("an hour ago", results[5][utc1], "A marked UTC date minus 65+ minutes");
               Assert.AreEqual("tomorrow", results[6][utc1], "A marked UTC date plus 36+ hours");
               Assert.AreEqual("2 months", results[7][utc1], "A marked UTC date plus 2+ months");
               Assert.AreEqual("one year", results[8][utc1], "A marked UTC date plus 1+ year");
               Assert.AreEqual("2 years", results[9][utc1], "A marked UTC date plus 2+ years");

               Assert.AreEqual("never", results[10][utc1], "A UTC date in the year 9999");
               Assert.AreEqual("never", results[10][est1], "A EST date in the year 9999");

            }
         }



      }
   }
}
