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
using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Dates {

   public class TimeAgoTransform : RelativeTimeTransform {
      public TimeAgoTransform(IContext context = null) : base(context, true) { }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("timeago") {
            Parameters = new List<OperationParameter> {
                    new OperationParameter("from-time-zone", "UTC")
                }
         };
      }
   }

   public class TimeAheadTransform : RelativeTimeTransform {
      public TimeAheadTransform(IContext context = null) : base(context, false) {
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("timeahead") {
            Parameters = new List<OperationParameter> {
                    new OperationParameter("from-time-zone", "UTC")
                }
         };
      }
   }

   public class RelativeTimeTransform : BaseTransform {

      private readonly bool _past;
      private readonly long _nowTicks;
      private readonly Field _input;
      private const int Second = 1;
      private const int Minute = 60 * Second;
      private const int Hour = 60 * Minute;
      private const int Day = 24 * Hour;
      private const int Month = 30 * Day;

      public RelativeTimeTransform(IContext context, bool past) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         _input = SingleInput();
         _past = past;
         var fromTimeZone = context.Operation.FromTimeZone == Constants.DefaultSetting ? "UTC" : context.Operation.FromTimeZone;

#if NETS10
         _nowTicks = DateTime.UtcNow.Ticks;
         if (fromTimeZone != "UTC") {
            Context.Error($"The relative time transform (timeago, timeahead) in {Context.Field.Alias} can only work on UTC dates on the .net standard 1.0 platform.");
            Run = false;
         }
#else
         _nowTicks = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone).Ticks;
#endif
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = GetRelativeTime(_nowTicks, ((DateTime)row[_input]).Ticks, _past);

         return row;
      }

      public static string GetRelativeTime(long nowTicks, long thenTicks, bool past = true) {

         var suffix = past ? " ago" : string.Empty;
         var ts = past ? new TimeSpan(nowTicks - thenTicks) : new TimeSpan(thenTicks - nowTicks);
         var delta = Math.Abs(ts.TotalSeconds);

         if (delta < 1 * Minute) {
            return ts.Seconds == 1 ? "one second" + suffix : ts.Seconds + " seconds" + suffix;
         }
         if (delta < 2 * Minute) {
            return "a minute" + suffix;
         }
         if (delta < 45 * Minute) {
            return ts.Minutes + " minutes" + suffix;
         }
         if (delta < 90 * Minute) {
            return "an hour" + suffix;
         }
         if (delta < 24 * Hour) {
            return ts.Hours + " hours" + suffix;
         }
         if (delta < 48 * Hour) {
            return past ? "yesterday" : "tomorrow";
         }
         if (delta < 30 * Day) {
            return ts.Days + " days" + suffix;
         }
         if (delta < 12 * Month) {
            var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "one month" + suffix : months + " months" + suffix;
         }

         var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
         return years <= 1 ? "one year" + suffix : years + " years" + suffix;
      }

   }
}