#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
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
using System;
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Pipeline.Scheduler.Quartz {
    public class CronExpressionValidator : IValidator {

        public CronExpressionValidator() : this("cron") { }

        public CronExpressionValidator(string name) {
            Name = name;
        }

        public void Validate(string name, string value, IDictionary<string, string> parameters, ILogger logger) {
            if (value == null) {
                logger.Error("A null value was passed into the CronExpressionValidator");
            }

            if (value == string.Empty) {
                return;
            }

            try {
                var expression = new global::Quartz.CronExpression(value);
                var date = expression.GetNextValidTimeAfter(new DateTimeOffset(DateTime.UtcNow));
                if (date == null) {
                    logger.Error($"The expression {value} is invalid.");
                }
            } catch (Exception ex) {
                logger.Error($"The expression {value} is invalid. {ex.Message}");
            }
        }

        public string Name { get; set; }
    }
}
