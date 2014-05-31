using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Transformalize.Main;
using Transformalize.Main.Providers.File;

namespace Transformalize.Test {

    [TestFixture]
    public class TestColumnNameValidator {

        [Test]
        public void Test1()
        {
            var validator = new ColumnNameValidator(new[] {"Column1", "2Column;"});
            Assert.IsTrue(validator.Valid());
        }

        [Test]
        public void Test2()
        {
            var name = Common.CleanIdentifier("2Column;");
            Assert.AreEqual("Column2", name);
        }

    }
}
