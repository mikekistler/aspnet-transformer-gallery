using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

// This transformer adds "type: string" to enum schemas when all enum values are strings.
public static class EnumTypeTransformer
{
    public static OpenApiOptions AddEnumTypeTransformer(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            if (schema.Enum is { Count: > 0 } && schema.Type == default)
            {
                if (schema.Enum.All(e => e?.GetValueKind() == JsonValueKind.String))
                {
                    schema.Type = JsonSchemaType.String;
                }
            }
            return Task.CompletedTask;
        });

        return options;
    }
}
