namespace Transformalize.Contracts
{
    public interface IConvertFile {
        /// <summary>
        /// Convert a file to some other format and return new file name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string Convert(IConnectionContext context);
    }
}
