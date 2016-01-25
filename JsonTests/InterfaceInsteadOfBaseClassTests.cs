using DiscriminatedTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace JsonTests
{
    [TestFixture]
    public class InterfaceInsteadOfBaseClassTests
    {
        private object _sequence;
        private CamelCasePropertyNamesContractResolver _resolver;

        public interface IHeterogeneousListItem
        {
            string Type { get; }
        }

        public class Polygon : IHeterogeneousListItem
        {
            public string Type { get { return GetType().Name; } }
            public int VertexCount { get; set; }
        }

        public class Color : IHeterogeneousListItem
        {
            public string Type { get { return GetType().Name; } }
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _resolver = new CamelCasePropertyNamesContractResolver();
            _sequence = new object[]
            {
                new Polygon { VertexCount = 5, },
                new Color { R = 0, G = 127, B = 255, },
            };
        }

        private object[] RoundTrip(JsonConverter converter)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new[] { converter },
                ContractResolver = _resolver
            };

            var serialized = JsonConvert.SerializeObject(_sequence, settings);
            return JsonConvert.DeserializeObject<object[]>(serialized, settings);
        }

        private static void CheckPolymorphicModels(IReadOnlyList<object> sequence)
        {
            Assert.AreEqual(2, sequence.Count);

            var p = sequence[0] as Polygon;
            Assert.NotNull(p);
            Assert.AreEqual(5, p.VertexCount);

            var c = sequence[1] as Color;
            Assert.NotNull(c);
            Assert.AreEqual(127, c.G);
        }

        // This could be <string, IHeterogeneousListItem>, but <string, object> is cooler.
        class StringConverter : BaseCustomConverter<string, object>
        {
            public StringConverter(
                IDiscriminatorMapper<string, object> discriminatorMapper,
                Func<string, string> propertyNameTransformer)
                : base(discriminatorMapper, propertyNameTransformer) { }
        }

        [Test]
        public void Use_string_as_discriminator()
        {
            var mapper = new StringMapper<object>(
                x => ((IHeterogeneousListItem)x).Type)
                .Register<Polygon>(typeof(Polygon).Name)
                .Register<Color>(typeof(Color).Name);

            var container = RoundTrip(
                new StringConverter(mapper, _resolver.GetResolvedPropertyName));
            CheckPolymorphicModels(container);
        }
    }
}
