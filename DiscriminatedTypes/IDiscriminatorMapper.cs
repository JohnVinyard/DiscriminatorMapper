using System;

namespace DiscriminatedTypes
{
    /// <summary>
    /// Maps values of type <see cref="TDiscriminator"/> to concrete
    /// classes derived from, or implementing <see cref="TBase"/>
    /// </summary>
    /// <typeparam name="TDiscriminator">The type of value that 
    /// discriminates between concrete classes</typeparam>
    /// <typeparam name="TBase">A base class or interface from which
    /// concrete classes are derived</typeparam>
    public interface IDiscriminatorMapper<TDiscriminator, TBase>
        where TBase : class
    {
        /// <summary>
        /// The property name of the discriminating field
        /// </summary>
        string DiscriminatorName { get; }

        /// <summary>
        /// Can this mapper handle members of type <see cref="t"/>
        /// </summary>
        /// <param name="t">The type to check against</param>
        /// <returns>True if t is equal to <see cref="TBase"/></returns>
        bool Handles(Type t);

        /// <summary>
        /// Return the discriminating value for type <see cref="t"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        TDiscriminator Discriminator(Type t);

        /// <summary>
        /// Transform the raw string representation of a discriminating
        /// value into values of type <see cref="TDiscriminator"/>
        /// </summary>
        /// <param name="s">The raw string representation of 
        /// a discriminating value</param>
        /// <returns></returns>
        TDiscriminator Discriminator(string s);

        /// <summary>
        /// Return the concrete type, derived from <see cref="TBase"/>,
        /// given a discriminating value
        /// </summary>
        /// <param name="discriminator">The discriminating value</param>
        /// <returns>The concrete, <see cref="TBase"/>-derived type</returns>
        Type ConcreteType(TDiscriminator discriminator);

        /// <summary>
        /// Produce a new instance of a <see cref="TBase"/>-derived class,
        /// given a discriminating value
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        TBase GetNewInstance(TDiscriminator discriminator);

        /// <summary>
        /// Register a new discriminator, type pair
        /// </summary>
        /// <typeparam name="T">The concrete, <see cref="TBase"/>-derived 
        /// type being registered</typeparam>
        /// <param name="discriminator">The discriminating value</param>
        /// <returns>This instance</returns>
        IDiscriminatorMapper<TDiscriminator, TBase> Register<T>(
            TDiscriminator discriminator) where T : TBase;

        /// <summary>
        /// Register a new discriminator, type pair
        /// </summary>
        /// <typeparam name="T">The concrete, <see cref="TBase"/>-derived
        /// type being registered</typeparam>
        /// <param name="instance">The instance whose type will 
        /// be registered</param>
        /// <returns>This instance</returns>
        IDiscriminatorMapper<TDiscriminator, TBase> Register<T>(
            T instance) where T : TBase;
    }
}