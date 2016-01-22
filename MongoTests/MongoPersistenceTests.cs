using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiscriminatedTypes;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MongoTests
{
    [TestFixture]
    public class MongoPersistenceTests
    {
        private IMongoDatabase _database;
        private IMongoCollection<Container> _collection;
        private string _collectionName;

        [SetUp]
        protected void SetUp()
        {
            var f = new FileStream("config.json", FileMode.Open);
            var sr = new StreamReader(f);
            try
            {
                var settings = JsonConvert
                    .DeserializeObject<MongoSettings>(sr.ReadToEnd());
                var client = new MongoClient(new MongoUrl(settings.Url));
                _database = client.GetDatabase(settings.DatabaseName);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Talk to John about why this one fails");
            }
            finally
            {
                f.Dispose();
                sr.Dispose();
            }

            _collectionName = Guid.NewGuid().ToString();
            _collection = _database.GetCollection<Container>(_collectionName);
            var mapper = new StringMapper<BasePaymentModel>(x => x.PaymentType)
                .Register<AchInputModel>("ach")
                .Register<CreditCardInputModel>("cc")
                .Register<PayPalInputModel>("paypal");
            BsonSerializer.RegisterDiscriminatorConvention(
                typeof(BasePaymentModel),
                new DiscriminatorConvention<string, BasePaymentModel>(mapper));
        }

        protected Container Container
        {
            get
            {
                return new Container
                {
                    Id = new Guid().ToString(),
                    Name = "Something",
                    PaymentModels = new List<BasePaymentModel>
                    {
                        new AchInputModel
                        {
                            AccountNumber = "123"
                        },
                        new CreditCardInputModel
                        {
                            Name = "John"
                        },
                        new PayPalInputModel
                        {
                            CancelUrl = "https://cancel.com"
                        }
                    }
                };
            }
        }

        [Test, Ignore]
        public void ActAndAssert()
        {
            var container = Container;
            _collection.InsertOne(container);
            var retrieved = _collection.Find(x => x.Id == container.Id).First();
            Assert.AreNotSame(container, retrieved);
            var payementModels = retrieved.PaymentModels.ToList();
            Assert.AreEqual(3, payementModels.Count);
            Assert.IsInstanceOf<AchInputModel>(payementModels[0]);
            Assert.AreEqual("123", ((AchInputModel)payementModels[0]).AccountNumber);
            Assert.IsInstanceOf<CreditCardInputModel>(payementModels[1]);
            Assert.AreEqual("John", ((CreditCardInputModel)payementModels[1]).Name);
            Assert.IsInstanceOf<PayPalInputModel>(payementModels[2]);
            Assert.AreEqual("https://cancel.com", ((PayPalInputModel)payementModels[2]).CancelUrl);
        }

        [TearDown]
        protected void TearDown()
        {
            _database.DropCollection(_collectionName);
        }
    }
}