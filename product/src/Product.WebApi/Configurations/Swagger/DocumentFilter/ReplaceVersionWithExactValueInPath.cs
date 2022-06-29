// This source code (ReplaceVersionWithExactValueInPath.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Product.WebApi.Configurations.Swagger.DocumentFilter;

/// <summary>
/// Replace version with exact value in path.
/// </summary>
public class ReplaceVersionWithExactValueInPath : IDocumentFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths
            .ToDictionary(
                path => path.Key.Replace("v{version}", $"v{swaggerDoc.Info.Version}", StringComparison.CurrentCulture),
                path => path.Value
            );

        swaggerDoc.Paths.Clear();

        foreach (var item in paths)
        {
            swaggerDoc.Paths.Add(item.Key, item.Value);
        }
    }
}