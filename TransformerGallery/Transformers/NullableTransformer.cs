using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

internal class NullableTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Properties is not null)
        {
            foreach (var property in schema.Properties)
            {
                if (schema.Required?.Contains(property.Key) != true)
                {
                    property.Value.Nullable = false;
                }
            }
        }

        return Task.CompletedTask;
    }
}