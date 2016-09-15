using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace Pipeline.Configuration {

    public class IllegalCharacterValidator : IValidator {

        private readonly string _illegalCharacters;
        private HashSet<char> _illegal;
        private HashSet<char> Illegal => _illegal ?? (_illegal = new HashSet<char>(_illegalCharacters.ToCharArray()));

        public IllegalCharacterValidator(string name, string illegalCharacters = ";'`\"") {
            Name = name;
            _illegalCharacters = illegalCharacters;
        }

        public string Name { get; set; }
        public void Validate(string name, string value, IDictionary<string, string> parameters, ILogger logger) {
            if (!string.IsNullOrEmpty(value) && value.ToCharArray().Any(c => Illegal.Contains(c))) {
                logger.Error($"The parameter {Name} contains an illegal character (e.g. {string.Join(",", Illegal)}).");
            }
        }
    }
}
