namespace Transformalize.Libs.Nest.Domain
{
	//If an object implements this then it can handle its own json representation
	public interface ICustomJson
	{
		object GetCustomJson();
	}
}