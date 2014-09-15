namespace Transformalize.Libs.Nest.Domain.Mapping.Attributes
{
	public interface IElasticPropertyVisitor
	{
		void Visit(ElasticPropertyAttribute attribute);
	}
}