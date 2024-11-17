using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

internal static class TypeTransformer
{
    internal static Dictionary<Type, OpenApiSchema> transforms = new Dictionary<Type, OpenApiSchema>();

    public static void MapType<T>(OpenApiSchema schema)
    {
        transforms[typeof(T)] = schema;
    }

    public static async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        // If transforms contains the schema's type, set the schema type and format from the transform schema
        if (transforms.ContainsKey(context.JsonTypeInfo.Type))
        {
            OpenApiSchema transformedSchema = transforms[context.JsonTypeInfo.Type];
            schema.Type = transformedSchema.Type;
            schema.Format = transformedSchema.Format;
        }

        return;
    }
}