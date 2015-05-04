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

using System.Diagnostics.Tracing;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Main.Transform;

namespace Transformalize.Test {
    [TestFixture]
    public class TestShortHand {

        private readonly TflField _field = new TflField();
        private readonly ShortHandFactory _factory = new ShortHandFactory(new TflProcess());

        [Test]
        public void Replace() {
            const string expression = "replace(x,y";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("replace", result.Method);
            Assert.AreEqual("x", result.OldValue);
            Assert.AreEqual("y", result.NewValue);
        }

        [Test]
        public void Left() {
            const string expression = "left(3";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("left", result.Method);
            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void Right() {
            const string expression = "right(2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("right", result.Method);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Append() {
            const string expression = "append(...";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("append", result.Method);
            Assert.AreEqual("...", result.Parameter);
        }

        [Test]
        public void If() {
            const string expression = "if(x,y,yes,no";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithOperator() {
            const string expression = "if(x,NotEqual,y,yes,no";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("NotEqual", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithEmpty() {
            const string expression = "if(x,,yes,no";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("no", result.Else);
        }

        [Test]
        public void IfWithoutElse() {
            const string expression = "if(x,y,yes";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("if", result.Method);
            Assert.AreEqual("x", result.Left);
            Assert.AreEqual("Equal", result.Operator);
            Assert.AreEqual("y", result.Right);
            Assert.AreEqual("yes", result.Then);
            Assert.AreEqual("", result.Else);
        }

        [Test]
        public void DatePart() {
            const string expression = "datepart(year)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("datepart", result.Method);
            Assert.AreEqual("year", result.TimeComponent);
        }

        [Test]
        public void Convert() {
            const string expression = "convert(p";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("p", result.Format);
        }

        [Test]
        public void ConvertDate() {
            const string expression = "convert(MMMM-DD-YYYY";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("MMMM-DD-YYYY", result.Format);
        }

        [Test]
        public void ConvertWithEncoding() {
            const string expression = "convert(UTF-8";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("UTF-8", result.Encoding);
        }

        [Test]
        public void ConvertWithType() {
            const string expression = "convert(int";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("convert", result.Method);
            Assert.AreEqual("int32", result.To);
        }

        [Test]
        public void Copy() {
            const string expression = "copy(a";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("copy", result.Method);
            Assert.AreEqual("a", result.Parameter);
        }

        [Test]
        public void Concat() {
            const string expression = "concat(";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("concat", result.Method);
        }

        [Test]
        public void Join() {
            const string expression = @"join(\,";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("join", result.Method);
            Assert.AreEqual(",", result.Separator);
        }

        [Test]
        public void HashCode() {
            const string expression = "hashcode(";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("hashcode", result.Method);
        }

        [Test]
        public void CompressField() {
            const string expression = "compress(";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("compress", result.Method);
            Assert.AreEqual("", result.Parameter);
        }

        [Test]
        public void DeCompressField() {
            const string expression = "decompress(";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("decompress", result.Method);
            Assert.AreEqual("", result.Parameter);
        }

        [Test]
        public void Elipse() {
            const string expression = "elipse(20,.....";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("elipse", result.Method);
            Assert.AreEqual(20, result.Length);
            Assert.AreEqual(".....", result.Elipse);
        }

        [Test]
        public void RegexReplace() {
            const string expression = "regexreplace(^x|y$,Z";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("regexreplace", result.Method);
            Assert.AreEqual("^x|y$", result.Pattern);
            Assert.AreEqual("Z", result.Replacement);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ElipseEscapeComma() {
            const string expression = @"elipse(20,\,\,\,";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("elipse", result.Method);
            Assert.AreEqual(20, result.Length);
            Assert.AreEqual(",,,", result.Elipse);
        }

        [Test]
        public void Format() {
            const string expression = @"format(mailto:{0}";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("format", result.Method);
            Assert.AreEqual("mailto:{0}", result.Format);
        }

        [Test]
        public void Insert() {
            const string expression = @"insert(3,three";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("insert", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual("three", result.Parameter);
        }

        [Test]
        public void InsertInterval() {
            const string expression = @"insertinterval(3,three";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("insertinterval", result.Method);
            Assert.AreEqual(3, result.Interval);
            Assert.AreEqual("three", result.Value);
        }

        [Test]
        public void Transliterate() {
            const string expression = @"transliterate(";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("transliterate", result.Method);
        }

        [Test]
        public void Slug() {
            const string expression = @"slug(50";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("slug", result.Method);
            Assert.AreEqual(50, result.Length);
        }

        [Test]
        public void DistinctWords() {
            const string expression = @"distinctwords( ";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("distinctwords", result.Method);
            Assert.AreEqual(" ", result.Separator);
        }

        [Test]
        public void Now() {
            const string expression = @"now";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("now", result.Method);
        }

        [Test]
        public void Remove() {
            const string expression = @"remove(3,2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("remove", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void TrimStart() {
            const string expression = @"trimstart(. ";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("trimstart", result.Method);
            Assert.AreEqual(". ", result.TrimChars);
        }

        [Test]
        public void TrimStartAppend() {
            const string expression = @"trimstartappend(*+, ";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("trimstartappend", result.Method);
            Assert.AreEqual("*+", result.TrimChars);
            Assert.AreEqual(" ", result.Separator);
        }

        [Test]
        public void TrimEnd() {
            const string expression = @"trimend(^%";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("trimend", result.Method);
            Assert.AreEqual("^%", result.TrimChars);
        }

        [Test]
        public void Trim() {
            const string expression = @"trim(|,";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("trim", result.Method);
            Assert.AreEqual("|,", result.TrimChars);
        }

        [Test]
        public void Substring() {
            const string expression = @"substr(3,2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("substring", result.Method);
            Assert.AreEqual(3, result.StartIndex);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void MapInline() {
            const string expression = @"map(x=1,y=2,z=3";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("map", result.Method);
            Assert.AreEqual("x=1,y=2,z=3", result.Map);
        }

        [Test]
        public void Add() {
            const string expression = "add(p1,7.2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("add", result.Method);
            Assert.AreEqual("p1", result.Parameters[0].Field);
            Assert.AreEqual("7.2", result.Parameters[1].Value);
        }

        [Test]
        public void PadLeft() {
            const string expression = "padleft(10,*";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("padleft", result.Method);
            Assert.AreEqual(10, result.TotalWidth);
            Assert.AreEqual('*', result.PaddingChar);
        }

        [Test]
        public void PadLeftWithParam() {
            const string expression = "padleft(10,*,p2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("padleft", result.Method);
            Assert.AreEqual(10, result.TotalWidth);
            Assert.AreEqual('*', result.PaddingChar);
            Assert.AreEqual("p2", result.Parameter);
        }

        [Test]
        public void PadRight() {
            const string expression = "padright(10,*";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("padright", result.Method);
            Assert.AreEqual(10, result.TotalWidth);
            Assert.AreEqual('*', result.PaddingChar);
        }

        [Test]
        public void ToStringTest() {
            const string expression = @"tostring(#\,##0.00";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tostring", result.Method);
            Assert.AreEqual("#,##0.00", result.Format);
        }

        [Test]
        public void ToLower() {
            const string expression = "tolower";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tolower", result.Method);
        }

        [Test]
        public void ToUpper() {
            const string expression = "toupper";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("toupper", result.Method);
        }

        [Test]
        public void JavaScript() {
            const string expression = "javascript(JSON.parse(x)[0].value)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("javascript", result.Method);
            Assert.AreEqual("JSON.parse(x)[0].value", result.Script);
        }

        [Test]
        public void JavaScriptWithTwoParameters() {
            const string expression = @"js(x*y)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("javascript", result.Method);
            Assert.AreEqual("x*y", result.Script);
        }

        [Test]
        public void CSharp() {
            const string expression = "cs(return d.AddDays(-1);)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("csharp", result.Method);
            Assert.AreEqual("return d.AddDays(-1);", result.Script);
        }

        [Test]
        public void Template() {
            const string expression = "template(@{ var x = Model.theParameter.PadLeft(5); }@{x} plus more text)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("template", result.Method);
            Assert.AreEqual("@{ var x = Model.theParameter.PadLeft(5); }@{x} plus more text", result.Template);
        }

        [Test]
        public void Velocity() {
            const string expression = "velocity(#set( $x = $theParameter.PadLeft(5))$x plus more text";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("velocity", result.Method);
            Assert.AreEqual("#set( $x = $theParameter.PadLeft(5))$x plus more text", result.Template);
        }

        [Test]
        public void TitleCase() {
            const string expression = "titlecase";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("totitlecase", result.Method);
            Assert.AreEqual("", result.Parameter);
        }

        [Test]
        public void TimeZone() {
            const string expression = "timezone(UTC,Eastern Standard Time,dateParam";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("timezone", result.Method);
            Assert.AreEqual("UTC", result.FromTimeZone);
            Assert.AreEqual("Eastern Standard Time", result.ToTimeZone);
            Assert.AreEqual("dateParam", result.Parameter);
        }

        [Test]
        public void ToJson() {
            const string expression = "tojson(p1,p2,p3";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tojson", result.Method);
            Assert.AreEqual("p1", result.Parameters[0].Field);
            Assert.AreEqual("p2", result.Parameters[1].Field);
            Assert.AreEqual("p3", result.Parameters[2].Field);
        }

        [Test]
        public void ToJsonWithLiterals() {
            const string expression = "tojson(p1,k:v,p3";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tojson", result.Method);
            Assert.AreEqual("p1", result.Parameters[0].Field);
            Assert.AreEqual("k", result.Parameters[1].Name);
            Assert.AreEqual("v", result.Parameters[1].Value);
            Assert.AreEqual("p3", result.Parameters[2].Field);
        }

        [Test]
        public void Tag() {
            const string expression = "tag(a,field1,content:stuff,target:field2";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tag", result.Method);
            Assert.AreEqual("a", result.Tag);

            Assert.AreEqual(string.Empty, result.Parameters[0].Name);
            Assert.AreEqual("field1", result.Parameters[0].Field);
            Assert.AreEqual(null, result.Parameters[0].Value);

            Assert.AreEqual("content", result.Parameters[1].Name);
            Assert.AreEqual(string.Empty, result.Parameters[1].Field);
            Assert.AreEqual("stuff", result.Parameters[1].Value);

            Assert.AreEqual("target", result.Parameters[2].Name);
            Assert.AreEqual(string.Empty, result.Parameters[2].Field);
            Assert.AreEqual("field2", result.Parameters[2].Value);
        }

        [Test]
        public void TagInput() {
            const string expression = "tag(input,type:radio,name:slot,class:required,value:Info)";
            var result = _factory.Interpret(expression, _field);
            Assert.AreEqual("tag", result.Method);
            Assert.AreEqual("input", result.Tag);

            Assert.AreEqual("type", result.Parameters[0].Name);
            Assert.AreEqual("", result.Parameters[0].Field);
            Assert.AreEqual("radio", result.Parameters[0].Value);

            Assert.AreEqual("name", result.Parameters[1].Name);
            Assert.AreEqual(string.Empty, result.Parameters[1].Field);
            Assert.AreEqual("slot", result.Parameters[1].Value);

            Assert.AreEqual("class", result.Parameters[2].Name);
            Assert.AreEqual(string.Empty, result.Parameters[2].Field);
            Assert.AreEqual("required", result.Parameters[2].Value);

            Assert.AreEqual("value", result.Parameters[3].Name);
            Assert.AreEqual("", result.Parameters[3].Field);
            Assert.AreEqual("Info", result.Parameters[3].Value);

        }


        [Test]
        public void TwoMethods() {
            var field = new TflField().GetDefaultOf<TflField>(f => {
                f.Name = "test";
                f.T = "left(10).right(2)";
            });
            _factory.ExpandShortHandTransforms(field);

            Assert.AreEqual(2, field.Transforms.Count);
            Assert.AreEqual("left", field.Transforms[0].Method);
            Assert.AreEqual(10, field.Transforms[0].Length);
            Assert.AreEqual("right", field.Transforms[1].Method);
            Assert.AreEqual(2, field.Transforms[1].Length);
        }

        [Test]
        public void TwoMethodsShorter() {
            var field = new TflField().GetDefaultOf<TflField>(f => {
                f.Name = "test";
                f.T = "left(10).right(2)";
            });
            _factory.ExpandShortHandTransforms(field);

            Assert.AreEqual(2, field.Transforms.Count);
            Assert.AreEqual("left", field.Transforms[0].Method);
            Assert.AreEqual(10, field.Transforms[0].Length);
            Assert.AreEqual("right", field.Transforms[1].Method);
            Assert.AreEqual(2, field.Transforms[1].Length);
        }

    }
}