using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public static class SecuritySchemeTransformer
{
    // An extension method to add JWT Bearer security scheme/requirements to the OpenAPI document.
    // We use an extension method since this transformation is implemented as two separate transformers,
    // a document transformer to add the security scheme to the document's components, and an operation transformer
    // to add the security requirement to operations that have the [Authorize] attribute.
    public static OpenApiOptions AddSecuritySchemeTransformer(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
        };
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, scheme);
            return Task.CompletedTask;
        });
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            {
                operation.Security = null;
            }
            else if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                var schemeRef = new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, context.Document);
                operation.Security = [new() { [schemeRef] = [] }];
            }
            return Task.CompletedTask;
        });
        return options;
    }
}