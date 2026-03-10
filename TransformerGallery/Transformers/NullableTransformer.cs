using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

// This transformer attempts to coalesce nullable and non-nullable schemas by removing the null type
// wherever nullability is already implied by the `required` property.
// It also removes `null` from enum values if present.
// Finally, it removes the "NullableOf" prefix from schema reference IDs if present, being careful to preserve
// the original reference ID for non-nullable types.
public static class NullableTransformer
{
    internal class ChainedDelegate(Func<JsonTypeInfo, string?> next) {

        public string? Invoke(JsonTypeInfo type) {
            // Get the result of the next delegate in the chain
            var result = next(type);
            // remove the "NullableOf" prefix for nullable types if present
            if (result is not null && type.Type.IsGenericType && type.Type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                result = Regex.Replace(result, "^NullableOf", "");
            }
            return result;
        }
    }

    public static OpenApiOptions AddNullableTransformer(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            if (schema.Properties is not null)
            {
                foreach (var property in schema.Properties)
                {
                    if (property.Value is OpenApiSchema propSchema)
                    {
                        // Remove the null type for non-required properties
                        if (schema.Required?.Contains(property.Key) != true)
                        {
                            propSchema.Type &= ~JsonSchemaType.Null;
                        }
                        // Also need to remove `null` from enum values if present
                        if (propSchema.Enum is not null)
                        {
                            propSchema.Enum = propSchema.Enum
                                .Where(e => e is not null)
                                .ToList();
                        }
                        // And remove default value of null if set
                        if (propSchema.Default is null)
                        {
                            propSchema.Default = null;
                        }
                    }
                }
            }
            return Task.CompletedTask;
        });

        var chainedDelegate = new ChainedDelegate(options.CreateSchemaReferenceId);
        options.CreateSchemaReferenceId = chainedDelegate.Invoke;

        return options;
    }
}