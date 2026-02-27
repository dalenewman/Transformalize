using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Compression;

namespace Tests {

   [TestClass]
   public class TestCompression {

      [TestMethod]
      public void TestCompressIsNotDefined() {

         var cfg = @"<cfg name='test'><parameters><add name='x' value='could someone please compress me? could someone please compress me?  i need to be compressed.  someone should compress me.' t='compress()' /></parameters></cfg>";
         using (var c = new ConfigurationContainer().CreateScope(cfg, new ConsoleLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual(1, process.Warnings().Length);
            Assert.AreEqual("The short-hand expression method compress is undefined.", process.Warnings().First());
            Assert.AreEqual("could someone please compress me? could someone please compress me?  i need to be compressed.  someone should compress me.", process.Parameters.First().Value);
         }
      }

      [TestMethod]
      public void TestCompressIsDefined() {

         const string originalValue = "could someone please compress me? could someone please compress me?  i need to be compressed.  someone should compress me.";
         var cfg = $@"<cfg name='test'><parameters><add name='x' value='{originalValue}' t='compress()' /></parameters></cfg>";
         var container = new ConfigurationContainer();
         container.AddTransform((c) => new CompressTransform(c), new CompressTransform().GetSignatures());

         using (var c = container.CreateScope(cfg, new ConsoleLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual(0, process.Warnings().Length);

            var compressed = process.Parameters.First().Value;

            // Verify compression actually happened (output differs from input)
            Assert.AreNotEqual(originalValue, compressed);

            // Verify the compressed output is not empty
            Assert.IsFalse(string.IsNullOrWhiteSpace(compressed));

            // Verify it's base64 encoded (basic check)
            Assert.IsTrue(compressed.Length > 0 && compressed.Length % 4 == 0);
         }
      }

      [TestMethod]
      public void TestDecompressIsDefined() {

         var cfg = @"<cfg name='test'><parameters><add name='x' value='egAAAB+LCAAAAAAABABLzi/NSVEozs9Nzc9LVSjISU0sTlVIzs8tKEotLlbITbUHcgipUMhUyEtNTVEoyVdIQkilpugpwLUVZ4BNQdKmBwCqA+CnegAAAA==' t='decompress()' /></parameters></cfg>";
         var container = new ConfigurationContainer();
         container.AddTransform((c) => new DecompressTransform(c), new DecompressTransform().GetSignatures());

         using (var scope = container.CreateScope(cfg, new ConsoleLogger())) {
            var process = scope.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual(0, process.Warnings().Length);
            Assert.AreEqual("could someone please compress me? could someone please compress me?  i need to be compressed.  someone should compress me.", process.Parameters.First().Value);
         }
      }

      [TestMethod]
      public void TestDecompressIsDefinedInLine() {

         var cfg = @"<cfg name='test'><parameters><add name='x' value='egAAAB+LCAAAAAAABABLzi/NSVEozs9Nzc9LVSjISU0sTlVIzs8tKEotLlbITbUHcgipUMhUyEtNTVEoyVdIQkilpugpwLUVZ4BNQdKmBwCqA+CnegAAAA==' t='decompress()' /></parameters></cfg>";
 
         using (var scope = new ConfigurationContainer(new TransformHolder((c) => new DecompressTransform(c), new DecompressTransform().GetSignatures())).CreateScope(cfg, new ConsoleLogger())) {
            var process = scope.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual(0, process.Warnings().Length);
            Assert.AreEqual("could someone please compress me? could someone please compress me?  i need to be compressed.  someone should compress me.", process.Parameters.First().Value);
         }
      }

   }
}
