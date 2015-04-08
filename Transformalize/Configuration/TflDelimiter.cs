using System;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflDelimiter : CfgNode {

        [Cfg(value = default(char), required = true, unique = true)]
        public char Character { get; set; }

        [Cfg(value = "", required = true)]
        public string Name { get; set; }

        public double AveragePerLine { get; set; }
        public double StandardDeviation { get; set; }

        public double CoefficientOfVariance() {
            return StandardDeviation / AveragePerLine;
        }

    }
}