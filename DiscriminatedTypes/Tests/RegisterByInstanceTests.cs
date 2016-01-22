using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DiscriminatedTypes.Tests
{
    abstract class BaseClass
    {
        public abstract string Name { get; }
    }

    class Concrete1 : BaseClass
    {
        int Age { get; set; }

        public override string Name
        {
            get { return "Hal"; }
        }
    }

    class Concrete2 : BaseClass
    {
        int Height { get; set; }

        public override string Name
        {
            get { return "Mike"; }
        }
    }

    class Concrete3 : BaseClass
    {
        int Something { get; set; }

        public override string Name
        {
            get { return "Something"; }
        }
    }

    [TestFixture]
    class RegisterByInstanceTests : BaseDiscriminatorMapperTests<string, BaseClass>
    {

        [SetUp]
        public void SetUp()
        {
            Mapper = new StringMapper<BaseClass>(x => x.Name)
                .Register(new Concrete1())
                .Register(new Concrete2());
        }

        [Test]
        public void Handles_base_type()
        {
            AssertHandles<BaseClass>();
        }
        [Test]
        public void Does_not_handle_unregistered_type()
        {
            AssertDoesNotHandle<Concrete3>();
        }

        [Test]
        public void Returns_correct_discriminator_for_registered_type()
        {
            AssertReturnsDiscriminator<Concrete1>("Hal");
        }

        [Test]
        public void Throws_when_trying_to_get_discriminator_for_unregistered_type()
        {
            AssertDiscriminatorThrows<Concrete3, ArgumentException>();            
        }

        [Test]
        public void Returns_concrete_type_for_registered_discriminator()
        {
            AssertGetsConcreteType<Concrete1>("Hal");
        }

        [Test]
        public void Throws_when_trying_to_get_concrete_type_for_unregistered_type()
        {
            AssertConcreteTypeThrows<KeyNotFoundException>("Blah");            
        }

        [Test]
        public void Can_get_new_instance_of_registered_type()
        {
            AssertGetNewInstance<Concrete2>("Mike");
        }

        [Test]
        public void Throws_when_trying_to_get_instance_of_unregistered_type()
        {
            AssertGetNewInstanceThrows<KeyNotFoundException>("Blah");
        }

        [Test]
        public void Can_get_discriminator_property_name()
        {
            AssertDiscriminatorPropertyName("Name");
        }
        
    }
}
