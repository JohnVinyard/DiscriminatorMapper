using System;
using System.Linq.Expressions;

namespace DiscriminatedTypes
{
    /// <summary>
    /// A discriminator mapper whose discriminating type is an int
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class IntMapper<TBase> : DiscriminatorMapper<int, TBase>
        where TBase : class
    {
        public IntMapper(Expression<Func<TBase, int>> expression)
            : base(expression) { }

        /// <summary>
        /// Assume <see cref="s"/> is the string representation
        /// of an integer
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override int Discriminator(string s)
        {
            return Convert.ToInt32(s);
        }
    }
}