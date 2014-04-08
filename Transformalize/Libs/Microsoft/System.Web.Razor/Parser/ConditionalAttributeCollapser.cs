// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Transformalize.Libs.Microsoft.System.Web.Razor.Editor;
using Transformalize.Libs.Microsoft.System.Web.Razor.Generator;
using Transformalize.Libs.Microsoft.System.Web.Razor.Parser.SyntaxTree;
using Transformalize.Libs.Microsoft.System.Web.Razor.Text;
using Transformalize.Libs.Microsoft.System.Web.Razor.Tokenizer;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Parser
{
    internal class ConditionalAttributeCollapser : MarkupRewriter
    {
        public ConditionalAttributeCollapser(Action<SpanBuilder, SourceLocation, string> markupSpanFactory) : base(markupSpanFactory)
        {
        }

        protected override bool CanRewrite(Block block)
        {
            AttributeBlockCodeGenerator gen = block.CodeGenerator as AttributeBlockCodeGenerator;
            return gen != null && block.Children.Any() && block.Children.All(IsLiteralAttributeValue);
        }

        protected override SyntaxTreeNode RewriteBlock(BlockBuilder parent, Block block)
        {
            // Collect the content of this node
            string content = String.Concat(block.Children.Cast<Span>().Select(s => s.Content));

            // Create a new span containing this content
            SpanBuilder span = new SpanBuilder();
            span.EditHandler = new SpanEditHandler(HtmlTokenizer.Tokenize);
            FillSpan(span, block.Children.Cast<Span>().First().Start, content);
            return span.Build();
        }

        private bool IsLiteralAttributeValue(SyntaxTreeNode node)
        {
            if (node.IsBlock)
            {
                return false;
            }
            Span span = node as Span;
            Debug.Assert(span != null);

            LiteralAttributeCodeGenerator litGen = span.CodeGenerator as LiteralAttributeCodeGenerator;

            return span != null &&
                   ((litGen != null && litGen.ValueGenerator == null) ||
                    span.CodeGenerator == SpanCodeGenerator.Null ||
                    span.CodeGenerator is MarkupCodeGenerator);
        }
    }
}
