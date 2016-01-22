using System;
using System.Linq.Expressions;

namespace DiscriminatedTypes
{
    /// <summary>
    /// A discriminator mapper whose discriminating type is an enum
    /// </summary>
    /// <typeparam name="TDiscriminator"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    public class EnumMapper<TDiscriminator, TBase>
        : DiscriminatorMapper<TDiscriminator, TBase>
        where TDiscriminator : struct
        where TBase : class
    {
        public EnumMapper(Expression<Func<TBase, TDiscriminator>> expression)
            : base(expression) { }

        /// <summary>
        /// Assume <see cref="s"/> is the string representation 
        /// of an enum value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override TDiscriminator Discriminator(string s)
        {
            return (TDiscriminator)Enum.Parse(typeof (TDiscriminator), s);
        }
    }
}