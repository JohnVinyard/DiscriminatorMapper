using System;
using System.Linq.Expressions;

namespace DiscriminatedTypes
{
    /// <summary>
    /// A discriminator mapper whose discriminating type is a string
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class StringMapper<TBase> : DiscriminatorMapper<string, TBase>
        where TBase : class
    {
        public StringMapper(Expression<Func<TBase, string>> expression)
            : base(expression) { }

        /// <summary>
        /// Easy.  The identity function.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override string Discriminator(string s)
        {
            return s;
        }
    }
}