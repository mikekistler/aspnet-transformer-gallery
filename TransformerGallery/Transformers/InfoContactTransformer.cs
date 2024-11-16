using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

// OpenAPI Document Transformer to add a contact to the Info object
internal class InfoContactTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Add a contact
        document.Info.Contact = new OpenApiContact()
        {
            Name = "Contoso Admin",
            Email = "admin@contoso.com",
        };
    }
}