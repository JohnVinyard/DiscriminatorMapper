

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DiscriminatedTypes.Tests
{
    enum Types
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    interface IBase
    {
        Types Type { get; set; }
    }

    class ConcreteA : IBase
    {
        public Types Type { get; set; }
    }

    class ConcreteB : IBase
    {
        public Types Type { get; set; }
    }

    class ConcreteC : IBase
    {
        public Types Type { get; set; }
    }

    [TestFixture]
    class RegisterByTypeTests : BaseDiscriminatorMapperTests<Types, IBase>
    {
        [SetUp]
        public void SetUp()
        {
            Mapper = new EnumMapper<Types, IBase>(x => x.Type)
                .Register<ConcreteA>(Types.One)
                .Register<ConcreteB>(Types.Two);
        }

        [Test]
        public void Handles_base_interface()
        {
            AssertHandles<IBase>();
        }

        [Test]
        public void Does_not_handle_unregistered_type()
        {
            AssertDoesNotHandle<ConcreteC>();
        }

        [Test]
        public void Returns_correct_discriminator_for_registered_type()
        {
            AssertReturnsDiscriminator<ConcreteA>(Types.One);
        }

        [Test]
        public void Throws_when_trying_to_get_discriminator_for_unregistered_type()
        {
            AssertDiscriminatorThrows<ConcreteC, ArgumentException>();
        }

        [Test]
        public void Returns_concrete_type_for_registered_discriminator()
        {
            AssertGetsConcreteType<ConcreteA>(Types.One);
        }

        [Test]
        public void Throws_when_trying_to_get_concrete_type_for_unregistered_discriminator()
        {
            AssertConcreteTypeThrows<KeyNotFoundException>(Types.Four);
        }

        [Test]
        public void Can_get_new_instance_of_registered_type()
        {
            AssertGetNewInstance<ConcreteA>(Types.One);
        }

        [Test]
        public void Throws_when_trying_to_get_instance_of_unregistered_type()
        {
            AssertGetNewInstanceThrows<KeyNotFoundException>(Types.Four);
        }

        [Test]
        public void Can_get_discriminator_property_name()
        {
            AssertDiscriminatorPropertyName("Type");
        }

        
    }
}
