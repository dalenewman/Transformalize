#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine.Compilation.CSharp")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine.Compilation.VisualBasic")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Templating", Scope = "namespace", Target = "RazorEngine.Templating")]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "RazorEngine.Compilation.CSharp.CSharpDirectCompilerService.#.ctor(System.Boolean,System.Web.Razor.Parser.MarkupParser)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "RazorEngine.Compilation.VisualBasic.VBDirectCompilerService.#.ctor(System.Boolean,System.Web.Razor.Parser.MarkupParser)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "RazorEngine.Templating.IsolatedTemplateService.#.ctor(RazorEngine.Compilation.ICompilerService,RazorEngine.Templating.IActivator)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "RazorEngine.Templating.TemplateService.#.ctor(RazorEngine.Compilation.ICompilerService,RazorEngine.Templating.IActivator)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine.Compilation.Inspectors")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine.Spans")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "RazorEngine.Templating.Parallel")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Templating", Scope = "namespace", Target = "RazorEngine.Templating.Parallel")]