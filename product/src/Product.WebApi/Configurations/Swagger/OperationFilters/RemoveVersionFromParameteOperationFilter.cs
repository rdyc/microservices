// This source code (RemoveVersionFromParameteOperationFilter.cs) is Copyright © PT. Xsis Mitra Utama.
// You MAY NOT copied, reproduced, published, distributed or transmitted
// to or stored in any manner without priorwritten consent from PT. Xsis Mitra Utama.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Product.WebApi.Configurations.Swagger.OperationFilter
{
    /// <summary>
    /// Remove version parameter operation filter.
    /// </summary>
    public class RemoveVersionParameterOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters
                .SingleOrDefault(p => p.Name == "version");

            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }
}