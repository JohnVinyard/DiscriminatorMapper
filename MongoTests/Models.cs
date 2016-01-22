using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoTests
{
    public abstract class BasePaymentModel
    {
        public abstract string PaymentType { get; }
    }

    public class PayPalInputModel : BasePaymentModel
    {
        public int Identifier { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }

        public override string PaymentType
        {
            get { return "paypal"; }
        }
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

        public override string PaymentType
        {
            get { return "cc"; }
        }
    }

    public class AchInputModel : BasePaymentModel
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public bool IsBusinessAccount { get; set; }

        public string Signature { get; set; }

        public override string PaymentType
        {
            get { return "ach"; }
        }
    }

    public class Container
    {
        [BsonId]
        public string Id { get; set; }
        public IEnumerable<BasePaymentModel> PaymentModels { get; set; }
        public string Name { get; set; }
    }
}