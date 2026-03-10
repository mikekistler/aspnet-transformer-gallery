using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public static class ProblemResponseTransformer
{
    // An extension method to add the document and operation transformers that together will add
    // a 4XX response to every operation in the OpenAPI document.
    public static OpenApiOptions AddProblemResponseTransformer(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new();
            document.Components.Responses ??= new Dictionary<string, IOpenApiResponse>();
            document.Components.Responses["Problem"] = new OpenApiResponse()
            {
                Description = "A problem occurred",
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/problem+json"] = new()
                    {
                        Schema = new OpenApiSchemaReference("Problem", document)
                    }
                }
            };
            return Task.CompletedTask;
        });
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            operation.Responses ??= new();
            operation.Responses["4XX"] = new OpenApiResponseReference("Problem", context.Document);
            return Task.CompletedTask;
        });
        return options;
    }
}