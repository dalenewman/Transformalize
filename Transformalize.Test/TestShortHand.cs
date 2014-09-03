#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using NUnit.Framework;
using Transformalize.Main.Transform;

namespace Transformalize.Test {
    [TestFixture]
    public class TestShortHand {

        [Test]
        public void Replace() {
            const string expression = "r(x,y";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("replace", result.Method);
            Assert.AreEqual("x", result.OldValue);
            Assert.AreEqual("y", result.NewValue);
        }

        [Test]
        public void Left() {
            const string expression = "l(3";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("left", result.Method);
            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void Right() {
            const string expression = "ri(2";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("right", result.Method);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Append() {
            const string expression = "a(...";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("append", result.Method);
            Assert.AreEqual("...", result.Parameter);
        }

        [Test]
        public void If() {
            const string expression = "if(x,y,yes,no";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithOperator() {
            const string expression = "i(x,NotEqual,y,yes,no";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("NotEqual", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithEmpty() {
            const string expression = "i(x,,yes,no";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithoutElse() {
            const string expression = "i(x,y,yes";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("", result.Else);
        }

        [Test]
        public void Convert() {
            const string expression = "cv(p";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("p", result.Parameter);
        }

        [Test]
        public void ConvertDate() {
            const string expression = "cv(d,MMMM-DD-YYYY";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("d", result.Parameter);
            Assert.AreEqual("MMMM-DD-YYYY", result.Format);
        }

        [Test]
        public void ConvertWithEncoding() {
            const string expression = "cv(b,UTF-8";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("b", result.Parameter);
            Assert.AreEqual("UTF-8", result.Encoding);
        }

        [Test]
        public void ConvertWithType() {
            const string expression = "cv(b,int";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("b", result.Parameter);
            Assert.AreEqual("int32", result.To);
        }

        [Test]
        public void Copy() {
            const string expression = "cp(a";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("copy", result.Method);
            Assert.AreEqual("a", result.Parameter);
        }

        [Test]
        public void Concat() {
            const string expression = "cc(*";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("concat", result.Method);
            Assert.AreEqual("*", result.Parameter);
        }

        [Test]
        public void ConcatWithParameters() {
            const string expression = "cc(p1,p2";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("concat", result.Method);
            Assert.AreEqual("p1", result.Parameters[0].Field);
            Assert.AreEqual("p2", result.Parameters[1].Field);
        }

        [Test]
        public void Join() {
            const string expression = @"j(\,,*";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("join", result.Method);
            Assert.AreEqual(",", result.Separator);
            Assert.AreEqual("*", result.Parameter);
        }

        [Test]
        public void JoinWithParameters() {
            const string expression = "j( ,p1,p2";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("join", result.Method);
            Assert.AreEqual(" ", result.Separator);
            Assert.AreEqual("p1", result.Parameters[0].Field);
            Assert.AreEqual("p2", result.Parameters[1].Field);
        }

        [Test]
        public void HashCode() {
            const string expression = "hc(";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("gethashcode", result.Method);
        }

        [Test]
        public void CompressField() {
            const string expression = "co(";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("compress", result.Method);
            Assert.AreEqual("", result.Parameter);
        }

        [Test]
        public void CompressParameter() {
            const string expression = "co(p";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("compress", result.Method);
            Assert.AreEqual("p", result.Parameter);
        }

        [Test]
        public void DeCompressField() {
            const string expression = "de(";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("decompress", result.Method);
            Assert.AreEqual("", result.Parameter);
        }

        [Test]
        public void DeCompressParameter() {
            const string expression = "de(p";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("decompress", result.Method);
            Assert.AreEqual("p", result.Parameter);
        }

        [Test]
        public void Elipse() {
            const string expression = "e(20,.....";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("elipse", result.Method);
            Assert.AreEqual(20, result.Length);
            Assert.AreEqual(".....", result.Elipse);
        }

        [Test]
        public void RegexReplace() {
            const string expression = "rr(^x|y$,Z";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("regexreplace", result.Method);
            Assert.AreEqual("^x|y$", result.Pattern);
            Assert.AreEqual("Z", result.Replacement);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ElipseEscapeComma() {
            const string expression = @"e(20,\,\,\,";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("elipse", result.Method);
            Assert.AreEqual(20, result.Length);
            Assert.AreEqual(",,,", result.Elipse);
        }

        [Test]
        public void Format() {
            const string expression = @"f(mailto:{0},email";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("format", result.Method);
            Assert.AreEqual("mailto:{0}", result.Format);
            Assert.AreEqual("email", result.Parameter);
        }

        [Test]
        public void FormatTwoParameters() {
            const string expression = @"f(mailto:{0}@{1},username,domain";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("format", result.Method);
            Assert.AreEqual("mailto:{0}@{1}", result.Format);
            Assert.AreEqual("username", result.Parameters[0].Field);
            Assert.AreEqual("domain", result.Parameters[1].Field);
        }

        [Test]
        public void Insert() {
            const string expression = @"in(3,three";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("insert", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual("three", result.Parameter);
        }

        [Test]
        public void InsertInterval() {
            const string expression = @"ii(3,three";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("insertinterval", result.Method);
            Assert.AreEqual(3, result.Interval);
            Assert.AreEqual("three", result.Value);
        }

        [Test]
        public void Transliterate() {
            const string expression = @"tl(";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("transliterate", result.Method);
        }

        [Test]
        public void Slug() {
            const string expression = @"sl(50";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("slug", result.Method);
            Assert.AreEqual(50, result.Length);
        }

        [Test]
        public void DistinctWords() {
            const string expression = @"dw( ";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("distinctwords", result.Method);
            Assert.AreEqual(" ", result.Separator);
        }

        [Test]
        public void Now() {
            const string expression = @"now";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("now", result.Method);
        }

        [Test]
        public void Remove() {
            const string expression = @"rm(3,2";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("remove", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void TrimStart() {
            const string expression = @"ts(. ";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("trimstart", result.Method);
            Assert.AreEqual(". ", result.TrimChars);
        }

        [Test]
        public void TrimStartAppend() {
            const string expression = @"tsa(*+, ";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("trimstartappend", result.Method);
            Assert.AreEqual("*+", result.TrimChars);
            Assert.AreEqual(" ", result.Separator);
        }

        [Test]
        public void TrimEnd() {
            const string expression = @"te(^%";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("trimend", result.Method);
            Assert.AreEqual("^%", result.TrimChars);
        }

        [Test]
        public void Trim() {
            const string expression = @"t(|,";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("trim", result.Method);
            Assert.AreEqual("|,", result.TrimChars);
        }

        [Test]
        public void Substring() {
            const string expression = @"ss(3,2";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("substring", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Map() {
            const string expression = @"m(map,param";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("map", result.Method);
            Assert.AreEqual("map", result.Map);
            Assert.AreEqual("param", result.Parameter);
        }

        [Test]
        public void MapInline() {
            const string expression = @"m(x=1,y=2,z=3";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("map", result.Method);
            Assert.AreEqual("x=1,y=2,z=3", result.Map);
        }

        [Test]
        public void MapInlineWithParam() {
            const string expression = @"m(param,x=1,y=2,z=3";
            var result = ShortHandFactory.Interpret(expression);
            Assert.AreEqual("map", result.Method);
            Assert.AreEqual("x=1,y=2,z=3", result.Map);
            Assert.AreEqual("param", result.Parameter);
        }

    }
}