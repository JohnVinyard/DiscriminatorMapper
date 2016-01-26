using System;
using DiscriminatedTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTests
{
    /// <summary>
    /// A json converter that can handle polymorphic, 
    /// <see cref="TBase"/>-derived values
    /// </summary>
    /// <typeparam name="TDiscriminator"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    public abstract class BaseCustomConverter<TDiscriminator, TBase>
        : JsonConverter
        where TBase : class
    {
        private readonly IDiscriminatorMapper<TDiscriminator, TBase> _discriminatorMapper;
        private readonly Func<string, string> _propertyNameTransformer;

        protected BaseCustomConverter(
            IDiscriminatorMapper<TDiscriminator, TBase> discriminatorMapper,
            Func<string, string> propertyNameTransformer)
        {
            _discriminatorMapper = discriminatorMapper;
            _propertyNameTransformer = propertyNameTransformer;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var name = _propertyNameTransformer(_discriminatorMapper.DiscriminatorName);
            var raw = jObject[name].ToString();
            var discriminator = _discriminatorMapper.Discriminator(raw);
            var instance = _discriminatorMapper.GetNewInstance(discriminator);
            serializer.Populate(jObject.CreateReader(), instance);
            return instance;
        }

        public override bool CanConvert(Type objectType)
        {
            return _discriminatorMapper.Handles(objectType);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}