using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflFilter : CfgNode {

        /// <summary>
        /// Optional.  Default is `AND`
        /// 
        /// A continuation operator.  Valid values are:
        /// 
        /// * AND
        /// * OR
        /// </summary>
        [Cfg(value = "AND", domain = "AND,OR")]
        public string Continuation { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// A free-form, un-checked expression.  This is passed directly into a generated query.
        /// </summary>
        [Cfg(value = "")]
        public string Expression { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the left side of an expression.
        /// </summary>
        [Cfg(value = "")]
        public string Left { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the right side of an expression.
        /// </summary>
        [Cfg(value = "")]
        public string Right { get; set; }

        /// <summary>
        /// Optional.  Default is `Equal`
        /// 
        /// A comparison operator.  Valid values are:
        /// 
        /// * Equal
        /// * GreaterThan
        /// * GreaterThanEqual 
        /// * LessThan
        /// * LessThanEqual
        /// * NotEqual
        /// </summary>
        [Cfg(value = "Equal", domain = "Equal,GreaterThan,GreaterThanEqual,LessThan,LessThanEqual,NotEqual")]
        public string Operator { get; set; }

    }
}