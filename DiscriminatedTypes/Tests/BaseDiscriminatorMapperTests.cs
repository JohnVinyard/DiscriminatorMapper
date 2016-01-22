using System;
using NUnit.Framework;

namespace DiscriminatedTypes.Tests
{
    public class BaseDiscriminatorMapperTests<TDiscriminator, TBase> 
        where TBase : class
    {
        protected IDiscriminatorMapper<TDiscriminator, TBase> Mapper;
 
        protected void AssertHandles<T>()
        {
            Assert.True(Mapper.Handles(typeof(T)));
        }

        protected void AssertDoesNotHandle<T>()
        {
            Assert.False(Mapper.Handles(typeof(T)));
        }

        protected void AssertReturnsDiscriminator<T>(TDiscriminator expected)
        {
            Assert.AreEqual(expected, Mapper.Discriminator(typeof(T)));
        }

        protected void AssertDiscriminatorThrows<T, TException>() where TException : Exception
        {
            Assert.Throws<TException>(() =>
                Mapper.Discriminator(typeof (T)));
        }

        protected void AssertGetsConcreteType<T>(TDiscriminator discriminator)
        {
            Assert.AreEqual(typeof(T), Mapper.ConcreteType(discriminator));
        }

        protected void AssertConcreteTypeThrows<TException>(TDiscriminator discriminator)
            where TException : Exception
        {
            Assert.Throws<TException>(() =>
                Mapper.ConcreteType(discriminator));
        }

        protected void AssertGetNewInstance<T>(TDiscriminator discriminator)
        {
            var instance = Mapper.GetNewInstance(discriminator);
            Assert.IsInstanceOf<T>(instance);
        }

        protected void AssertGetNewInstanceThrows<TException>(TDiscriminator discriminator) where TException : Exception
        {
            Assert.Throws<TException>(() =>
                Mapper.GetNewInstance(discriminator));
        }

        protected void AssertDiscriminatorPropertyName(string name)
        {
            Assert.AreEqual(name, Mapper.DiscriminatorName);
        }
    }
}