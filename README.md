Provide a single base `DiscriminatorMapper` class that implements all the primitives needed for polymorphic deserialization, be it from JSON, BSON, XML, or something else.

This approach has the advantage that knowledge about the mapping from discriminator values to concrete classes lives in a single place.

For example, given classes like this:
```
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
```

You might register discriminator values like this:
```
var mapper = new EnumMapper<Types, IBase>(x => x.Type)
    .Register<ConcreteA>(Types.One)
    .Register<ConcreteB>(Types.Two);
```

Finally, you might set up your JSON serializer with the following settings:
```
var resolver = new CamelCasePropertyNamesContractResolver();
var converter = new EnumConverter(mapper, resolver.GetResolvedPropertyName);
var settings = new JsonSerializerSettings
{
    Converters = new[] { converter },
    ContractResolver = _resolver
};
```
