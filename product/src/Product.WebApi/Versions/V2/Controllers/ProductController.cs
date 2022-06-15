using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Queries;
using Product.WebApi.Versions.V2.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Product.WebApi.Versions.V2.Controllers
{
    /// <summary>
    /// The product api v2 controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("2"), Route("v{version:apiVersion}/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<ProductController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ProductController(IMapper mapper, IMediator mediator, ILogger<ProductController> logger)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieve all products.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of products.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "get", Tags = new[] { "Product" })]
        [ProducesResponseType(typeof(GetAllProductsResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = mapper.Map<GetAllProductsQuery>(request);

                var data = await mediator.Send(query);

                var result = mapper.Map<GetAllProductsResponse>(data);

                return Ok(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }
    }
}
