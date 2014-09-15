using Transformalize.Libs.Nest.Domain.TermVector;

namespace Transformalize.Libs.Nest.DSL.TermVector
{
	public interface IMultiTermVectorDocumentDescriptor
	{
		MultiTermVectorDocument Document { get; set; }
		MultiTermVectorDocument GetDocument(); 
	}
}