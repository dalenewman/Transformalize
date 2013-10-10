using Transformalize.Configuration;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process Create(string resource, Options options = null)
        {
            return new ProcessReader(new ConfigurationFactory(resource).Create(), options ?? new Options()).Read();
        }
    }
}
