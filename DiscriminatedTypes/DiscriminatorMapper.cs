using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;

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
    public abstract class DiscriminatorMapper<TDiscriminator, TBase> : 
        IDiscriminatorMapper<TDiscriminator, TBase> where TBase : class
    {
        private readonly IDictionary<TDiscriminator, Type> _map = 
            new Dictionary<TDiscriminator, Type>();

        private readonly Func<TBase, TDiscriminator> _func;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="expression">An expression that references the property
        /// that discriminates this type</param>
        protected DiscriminatorMapper(
            Expression<Func<TBase, TDiscriminator>> expression)
        {
            Expression = expression;
            _func = Expression.Compile();
            DiscriminatorName = Expression.GetMemberExpression(true).Member.Name;
        }

        /// <summary>
        /// Transform raw, string representations of a discriminator value
        /// int instances of type <see cref="TDiscriminator"/>
        /// </summary>
        /// <param name="s">The raw, string representation of a 
        /// discriminator value</param>
        /// <returns></returns>
        public abstract TDiscriminator Discriminator(string s);

        /// <summary>
        /// The property name of the discriminating field
        /// </summary>
        public string DiscriminatorName { get; private set; }

        /// <summary>
        /// Can this mapper handle members of type <see cref="t"/>
        /// </summary>
        /// <param name="t">The type to check against</param>
        /// <returns>True if t is equal to <see cref="TBase"/></returns>
        public bool Handles(Type t)
        {
            return t == typeof (TBase);
        }

        /// <summary>
        /// Return the discriminating value for type <see cref="t"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public TDiscriminator Discriminator(Type t)
        {
            foreach (var kvp in _map.Where(kvp => kvp.Value == t))
            {
                return kvp.Key;
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Return the concrete type, derived from <see cref="TBase"/>,
        /// given a discriminating value
        /// </summary>
        /// <param name="discriminator">The discriminating value</param>
        /// <returns>The concrete, <see cref="TBase"/>-derived type</returns>
        public Type ConcreteType(TDiscriminator discriminator)
        {
            return _map[discriminator];
        }

        /// <summary>
        /// Produce a new instance of a <see cref="TBase"/>-derived class,
        /// given a discriminating value
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public TBase GetNewInstance(TDiscriminator discriminator)
        {
            return (TBase)Activator.CreateInstance(_map[discriminator]);
        }

        /// <summary>
        /// Register a new discriminator, type pair
        /// </summary>
        /// <typeparam name="T">The concrete, <see cref="TBase"/>-derived 
        /// type being registered</typeparam>
        /// <param name="discriminator">The discriminating value</param>
        /// <returns>This instance</returns>
        public IDiscriminatorMapper<TDiscriminator, TBase> Register<T>(
            TDiscriminator discriminator) where T : TBase
        {
            _map.Add(discriminator, typeof(T));
            return this;
        }

        /// <summary>
        /// Register a new discriminator, type pair
        /// </summary>
        /// <typeparam name="T">The concrete, <see cref="TBase"/>-derived
        /// type being registered</typeparam>
        /// <param name="instance">The instance whose type will 
        /// be registered</param>
        /// <returns>This instance</returns>
        public IDiscriminatorMapper<TDiscriminator, TBase> Register<T>(
            T instance) where T : TBase
        {
            _map.Add(_func(instance), typeof(T));
            return this;
        }

        protected Expression<Func<TBase, TDiscriminator>> Expression
        {
            get; private set;
        }

        
    }
}