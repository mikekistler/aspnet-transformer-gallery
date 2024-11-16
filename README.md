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
