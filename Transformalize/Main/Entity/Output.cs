using System;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class Output {
        public Func<Row, bool> ShouldRun = row => true;
        public string Name { get; set; }
        public AbstractConnection Connection { get; set; }

        public bool IsActive()
        {
            return !ShouldRun.Equals(new Func<Row, bool>(row => true));
        }
    }
}