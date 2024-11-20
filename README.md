# ASP.NET Transformer Gallery

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

This example maps the `decimal` type to the `number` type with the `decimal` format.

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