using System.Collections.Generic;

namespace JsonTests
{
    public enum PaymentType
    {
        One = 1,
        Two = 2,
        Three = 3
    }

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
}