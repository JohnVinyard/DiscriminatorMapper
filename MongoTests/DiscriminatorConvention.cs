using System;
using DiscriminatedTypes;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace MongoTests
{
    public class DiscriminatorConvention<TDiscriminator, TBase> 
        : IDiscriminatorConvention
        where TBase : class
    {
        private readonly IDiscriminatorMapper<TDiscriminator, TBase> _discriminatorMapper;

        public DiscriminatorConvention(
            IDiscriminatorMapper<TDiscriminator, TBase> discriminatorMapper)
        {
            _discriminatorMapper = discriminatorMapper;
        }

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            var bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            var t = nominalType;
            if (bsonReader.FindElement(ElementName))
            {
                var raw = bsonReader.ReadString();
                var discriminator = _discriminatorMapper.Discriminator(raw);
                t = _discriminatorMapper.ConcreteType(discriminator);
            }
            bsonReader.ReturnToBookmark(bookmark);
            return t;
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            return _discriminatorMapper.Discriminator(actualType).ToString();
        }

        public string ElementName
        {
            get { return _discriminatorMapper.DiscriminatorName; }
        }
    }
}