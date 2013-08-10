using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;

namespace Transformalize.Core.Transform_
{
    public class Transforms : IEnumerable
    {
        private readonly List<AbstractTransform> _transforms;

        public Transforms()
        {
            _transforms = new List<AbstractTransform>();
        }

        public int Count
        {
            get
            {
                return _transforms.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return (_transforms as IEnumerable<AbstractTransform>).GetEnumerator();
        }

        public AbstractTransform this[int i]
        {
            get { return _transforms[i]; }
        }

        public void Add(AbstractTransform account)
        {
            _transforms.Add(account);
        }

        public IFields Results()
        {
            var results = new Fields();
            foreach (var transform in _transforms.Where(t=>t.HasResults))
            {
                results.AddRange(transform.Results);
            }
            return results;
        }

        public IParameters Parameters()
        {
            var parameters = new Parameters();
            foreach (var transform in _transforms.Where(t=>t.HasParameters))
            {
                parameters.AddRange(transform.Parameters);
            }
            return parameters;
        } 


    }
}