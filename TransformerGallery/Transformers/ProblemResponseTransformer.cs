using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

public static class ProblemResponseTransformer
{
    // An extension method to add the document and operation transformers that together will add
    // a 4XX response to every operation in the OpenAPI document.
    public static OpenApiOptions AddProblemResponseTransformer(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new();
            document.Components.Responses ??= new Dictionary<string, OpenApiResponse>();

            // Define the ProblemDetails schema based on RFC7807 explicitly
            document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();
            if (!document.Components.Schemas.ContainsKey("ProblemDetails"))
            {
                document.Components.Schemas["ProblemDetails"] = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        { "type", new OpenApiSchema { Type = "string", Format = "uri" } },
                        { "title", new OpenApiSchema { Type = "string" } },
                        { "status", new OpenApiSchema { Type = "integer", Format = "int32" } },
                        { "detail", new OpenApiSchema { Type = "string" } },
                    },
                    AdditionalPropertiesAllowed = true
                };
            }

            document.Components.Responses["Problem"] = new()
            {
                Description = "A problem occurred",
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/problem+json"] = new()
                    {
                        Schema = new()
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            };
            return Task.CompletedTask;
        });
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            operation.Responses ??= new();
            operation.Responses["4XX"] = new()
            {
                Reference = new()
                {
                    Type = ReferenceType.Response,
                    Id = "Problem"
                }
            };
            return Task.CompletedTask;
        });
        return options;
    }
}