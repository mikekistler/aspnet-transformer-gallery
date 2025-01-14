using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

// OpenAPI Document Transformer to convert the document to a canonical form
internal class CanonicalDocumentTransformer : IOpenApiDocumentTransformer
{
        private class OperationTypeComparer : IComparer<OperationType>
    {
        private static readonly List<OperationType> Order = new()
        {
            OperationType.Get,
            OperationType.Post,
            OperationType.Put,
            OperationType.Patch,
            OperationType.Delete,
            OperationType.Head,
            OperationType.Options
        };

        private string foo = OperationType.Get.GetDisplayName();

        public int Compare(OperationType x, OperationType y)
        {
            return Order.IndexOf(x).CompareTo(Order.IndexOf(y));
        }
    }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Sort the paths by key
        var sortedPaths = document.Paths.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
        document.Paths.Clear();
        foreach (var path in sortedPaths)
        {
            document.Paths.Add(path.Key, path.Value);
        }

        // Sort the operations of each path by type in this order: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS
        var comparer = new OperationTypeComparer();
        foreach (var path in document.Paths)
        {
            var sortedOperations = path.Value.Operations.OrderBy(o => o.Key, comparer).ToDictionary(o => o.Key, o => o.Value);
            path.Value.Operations.Clear();
            foreach (var operation in sortedOperations)
            {
                path.Value.AddOperation(operation.Key, operation.Value);
            }
        }

        // Sort the elements of the tags field by name
        if (document.Tags != null)
        {
            document.Tags = document.Tags.OrderBy(t => t.Name).ToList();
        }

        return Task.CompletedTask;
    }
}