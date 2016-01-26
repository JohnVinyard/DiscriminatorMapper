using System.Collections.Generic;
using System.Linq;
using DiscriminatedTypes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JsonTests.AttributeBasedConfig
{
    public enum PaymentType
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    [JsonConverter(typeof(PaymentModelConverter))]
    public abstract class BasePaymentModel
    {
        public string PaymentType { get; set; }
        public int PaymentNumber { get; set; }
        public PaymentType PaymentEnum { get; set; }
    }

    public class PayPalInputModel : BasePaymentModel
    {
        public int Identifier { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class CreditCardInputModel : BasePaymentModel
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string Cvv2 { get; set; }
        public int AddressId { get; set; }
        public bool VerifyAddress { get; set; }
        public bool UseThreeDSecure { get; set; }
        public string ThreeDSecurePayLoadResponse { get; set; }
        public string ThreeDSecureTermUrl { get; set; }
    }

    public class DirecteBankingInputeModel : BasePaymentModel
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public bool IsBusinessAccount { get; set; }

        public string Title { get; set; }
        public int Identifier { get; set; }

        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class AchInputModel : BasePaymentModel
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public bool IsBusinessAccount { get; set; }

        public string Signature { get; set; }
    }

    public class Container
    {
        public IEnumerable<BasePaymentModel> PaymentModels { get; set; }
        public string Name { get; set; }
    }

    public class PaymentModelDiscriminator : StringMapper<BasePaymentModel>
    {
        public PaymentModelDiscriminator()
            : base(x => x.PaymentType)
        {
            Register<PayPalInputModel>("paypal");
            Register<AchInputModel>("ach");
            Register<CreditCardInputModel>("cc");
        }
    }

    public class PaymentModelConverter : BaseCustomConverter<string, BasePaymentModel>
    {
        public PaymentModelConverter()
            : base(
                new PaymentModelDiscriminator(),
                x => x) { }
    }

    [TestFixture]
    public class AttributeBasedConfguration
    {
        private Container _container;

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
                        AccountNumber = "123"
                    },
                    new CreditCardInputModel
                    {
                        PaymentType = "cc",
                        Name = "John"
                    },
                    new PayPalInputModel
                    {
                        PaymentType = "paypal",
                        CancelUrl = "https://cancel.com"
                    }
                }
            };
        }

        [Test]
        public void Use_string_as_discriminator()
        {
            var container = RoundTrip();
            CheckPolymorphicModels(container);
        }

        private Container RoundTrip()
        {
            var serialized = JsonConvert.SerializeObject(_container);
            return JsonConvert.DeserializeObject<Container>(serialized);
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
    }
}