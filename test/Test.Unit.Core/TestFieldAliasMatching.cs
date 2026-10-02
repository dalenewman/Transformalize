using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;

namespace Tests {
    [TestClass]
    public class TestFieldAliasMatching {
        [TestMethod]
        [DataRow("Order.Total", "OrderXTotal")] // A dot must not act as a regex wildcard.
        [DataRow("Order)Total", "OrderTotal")] // An unmatched parenthesis must not break the regex.
        public void TreatsColumnAliasesAsLiteralText(string alias, string differentColumn) {
            using var process = new Process($@"
<cfg name='AliasMatching{Guid.NewGuid():N}'>
    <entities>
        <add name='Input'>
            <fields>
                <add name='Source' alias='{alias}' />
            </fields>
        </add>
    </entities>
    <calculated-fields>
        <add name='Result'>
            <transforms>
                <add method='format' format='Value: {alias}' />
            </transforms>
        </add>
    </calculated-fields>
</cfg>");

            Assert.AreEqual(0, process.Errors().Length, string.Join(Environment.NewLine, process.Errors()));

            var source = process.Entities.Single();
            Assert.AreEqual(alias, source.GetFieldMatches(alias).Single().Alias);
            Assert.IsFalse(source.GetFieldMatches(differentColumn).Any());

            using var calculated = process.ToCalculatedFieldsProcess();
            var entity = calculated.Entities.Single();

            // Calculated-field discovery and matching must preserve the same literal alias.
            Assert.IsTrue(entity.Fields.Any(f => f.Alias == alias), "The literal alias dependency was not included.");
            Assert.AreEqual(alias, entity.GetFieldMatches(alias).Single().Alias);
            Assert.IsFalse(entity.GetFieldMatches(differentColumn).Any());
        }
    }
}
