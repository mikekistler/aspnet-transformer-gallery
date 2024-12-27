# ASP.NET Transformer Gallery

<!-- markdownlint-disable MD024 -->

A collection of OpenAPI transformers for ASP.NET applications.

## Transformers

### InfoContactTransformer

Adds contact information to the OpenAPI document. In the example transformer, the contact information is hard-coded,
but your implementation can retrieve the information from a database, a configuration file, or any other source.

#### Usage

Copy the InfoContactTransformer.cs file to your project and then configure the transformer in the `configureOptions`
delegate of the `AddOpenApi` extension method as shown below:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(new InfoContactTransformer());
});
```

### NullableTransformer

ASP.NET may generate two different schemas for a class or struct that may be nullable. In many cases the "nullability" of a property is already expressed in the schema by the omission of the property from the required keyword. Properties that are not required can be omitted from the body entirely and the Json deserializer will produce an object with a null value for that property. In this case, dropping the `nullable: true` keyword from the schema allows the same schema to be used for both nullable and non-nullable cases.
In addition, the `null` value is removed from the enum values if present so that nullable and non-nullable enums can share the same schema,

The [NullableTransformer](./TransformerGalleryTransformers/NullableTransformer.cs) transformer removes `nullable: true` from any properties that are not listed in the required keyword of the schema.

Additional work must be done for nullable value types, because the default schema reference ID for nullable value types will have a prefix of "NullableOf", which will differ from the schema reference ID for the underlying value type, causing two schemas to be generated. To address this issue, a custom CreateSchemaReferenceId method is added to the OpenApiOptions to remove the prefix from the schema reference for nullable value types.

#### Usage

Copy the NullableTransformer.cs file to your project and then configure the transformer in the `configureOptions`
delegate of the `AddOpenApi` extension method as shown below:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddNullableTransformer();
});
```

### ProblemResponseTransformer

The `TypedResults.Problem` method in ASP.NET Core returns a problem details response, but this method does not
implement `IEndpointMetadataProvider`, so the response is not documented in the OpenAPI document. This is because the status code is not known at compile time. But this situation can be described in OpenAPI 3.0.x using the `4XX` response code. The [ProblemResponseTransformer](./TransformerGallery/Transformers/ProblemResponseTransformer.cs) transformer adds a `4XX` response for the problem details response to the OpenAPI document.

#### Usage

Copy the ProblemResponseTransformer.cs file to your project and then configure the transformer in the `configureOptions`
delegate of the `AddOpenApi` extension method as shown below:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddProblemResponseTransformer();
});
```

### SecuritySchemeTransformer

Currently ASP.NET does not collect information about the security schemes used in the application. The
[SecuritySchemeTransformer](./TransformerGalleryTransformers/SecuritySchemeTransformer.cs) adds a JWT Bearer token
security scheme to the OpenAPI document, and then adds this as the security requirement for all operations
that require authorization.

#### Usage

Copy the SecuritySchemeTransformer.cs file to your project and then configure the transformer in the `configureOptions`
delegate of the `AddOpenApi` extension method as shown below:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddSecuritySchemeTransformer();
});
```

### TypeTransformer

Swashbuckle provides the [MapType method on the AddSwaggerGen] options to customize the
`type` and `format` for a C# type.
The [TypeTransformer](./TransformerGalleryTransformers/TypeTransformer.cs) that does more or less the same thing.

[MapType method on the AddSwaggerGen]: https://github.com/domaindrivendev/Swashbuckle.AspNetCore?tab=readme-ov-file#override-schema-for-specific-types

#### Usage

Copy the TypeTransformer.cs file to your project and then configure the transformer in the `configureOptions`
delegate of the `AddOpenApi` extension method as shown below:

```csharp
builder.Services.AddOpenApi(options =>
{
    TypeTransformer.MapType<decimal>(new OpenApiSchema { Type = "number", Format = "decimal" });
    options.AddSchemaTransformer(TypeTransformer.TransformAsync);
});
```
