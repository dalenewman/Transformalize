//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    internal struct ValidatorCacheKey : IEquatable<ValidatorCacheKey>
    {
        public ValidatorCacheKey(Type sourceType, string ruleset, bool generic) : this()
        {
            this.SourceType = sourceType;
            this.Ruleset = ruleset;
            this.Generic = generic;
        }

        public bool Generic { get; private set; }

        public Type SourceType { get; private set; }
        
        public string Ruleset { get; private set; }

        public override int GetHashCode()
        {
            return this.SourceType.GetHashCode()
                   ^ (this.Ruleset != null ? this.Ruleset.GetHashCode() : 0);
        }

        #region IEquatable<ValidatorCacheKey> Members

        bool IEquatable<ValidatorCacheKey>.Equals(ValidatorCacheKey other)
        {
            return (this.SourceType == other.SourceType)
                   && (this.Ruleset == null ? other.Ruleset == null : this.Ruleset.Equals(other.Ruleset))
                   && (this.Generic == other.Generic);
        }

        #endregion
    }
}
