using System;
using System.Collections.Generic;
using System.Linq;
using DiscriminatedTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace JsonTests
{
    [TestFixture]
    public class JsonDeserializationTests
    {
        private Container _container;
        private CamelCasePropertyNamesContractResolver _resolver;

        [SetUp]
        public void SetUp()
        {
            _container = new Container
            {
                Name = "Something",
                PaymentModels = new List<BasePaymentModel>
                {
                    new AchInputModel
                    {
                        PaymentType = "ach",
                        PaymentEnum = PaymentType.One,
                        PaymentNumber = 1,
                        AccountNumber = "123"
                    },
                    new CreditCardInputModel
                    {
                        PaymentType = "cc",
                        PaymentEnum = PaymentType.Two,
                        PaymentNumber = 2,
                        Name = "John"
                    },
                    new PayPalInputModel
                    {
                        PaymentType = "paypal",
                        PaymentEnum = PaymentType.Three,
                        PaymentNumber = 3,
                        CancelUrl = "https://cancel.com"
                    }
                }
            };
            _resolver = new CamelCasePropertyNamesContractResolver();
        }

        class StringConverter : BaseCustomConverter<string, BasePaymentModel>
        {
            public StringConverter(
                IDiscriminatorMapper<string, BasePaymentModel> discriminatorMapper,
                Func<string, string> propertyNameTransformer)
                : base(discriminatorMapper, propertyNameTransformer) { }
        }

        class EnumConverter : BaseCustomConverter<PaymentType, BasePaymentModel>
        {
            public EnumConverter(
                IDiscriminatorMapper<PaymentType, BasePaymentModel> discriminatorMapper,
                Func<string, string> propertyNameTransformer)
                : base(discriminatorMapper, propertyNameTransformer) { }
        }

        class IntConverter : BaseCustomConverter<int, BasePaymentModel>
        {
            public IntConverter(
                IDiscriminatorMapper<int, BasePaymentModel> discriminatorMapper,
                Func<string, string> propertyNameTransformer)
                : base(discriminatorMapper, propertyNameTransformer) { }
        }

        private Container RoundTrip(JsonConverter converter)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new[] { converter },
                ContractResolver = _resolver
            };

            var serialized = JsonConvert.SerializeObject(_container, settings);
            return JsonConvert.DeserializeObject<Container>(
                serialized, settings);
        }

        private void CheckPolymorphicModels(Container container)
        {
            var payementModels = container.PaymentModels.ToList();
            Assert.AreEqual(3, payementModels.Count);
            var ach = payementModels[0] as AchInputModel;
            Assert.NotNull(ach);
            Assert.AreEqual("123", ach.AccountNumber);
            var cc = payementModels[1] as CreditCardInputModel;
            Assert.NotNull(cc);
            Assert.AreEqual("John", cc.Name);
            var pp = payementModels[2] as PayPalInputModel;
            Assert.NotNull(pp);
            Assert.AreEqual("https://cancel.com", pp.CancelUrl);
        } 

        [Test]
        public void Use_string_as_discriminator()
        {
            var mapper = new StringMapper<BasePaymentModel>(
                x => x.PaymentType)
                .Register<AchInputModel>("ach")
                .Register<CreditCardInputModel>("cc")
                .Register<PayPalInputModel>("paypal");

            var container = RoundTrip(
                new StringConverter(mapper, _resolver.GetResolvedPropertyName));
            CheckPolymorphicModels(container);
        }

        [Test]
        public void Use_enum_as_discriminator()
        {
            var mapper = new EnumMapper<PaymentType, BasePaymentModel>(
                x => x.PaymentEnum)
                .Register<AchInputModel>(PaymentType.One)
                .Register<CreditCardInputModel>(PaymentType.Two)
                .Register<PayPalInputModel>(PaymentType.Three);
            var container = RoundTrip(
                new EnumConverter(mapper, _resolver.GetResolvedPropertyName));
            CheckPolymorphicModels(container);
        }

        [Test]
        public void Use_int_as_discriminator()
        {
            var mapper = new IntMapper<BasePaymentModel>(
                x => x.PaymentNumber)
                .Register<AchInputModel>(1)
                .Register<CreditCardInputModel>(2)
                .Register<PayPalInputModel>(3);

            var container = RoundTrip(
                new IntConverter(mapper, _resolver.GetResolvedPropertyName));
            CheckPolymorphicModels(container);
        }

    }
}
