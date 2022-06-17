using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.WebApi.Versions.V1.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Product.WebApi.Versions.V1.Controllers
{
    /// <summary>
    /// The product v1 api controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("1"), Route("v{version:apiVersion}/products")]
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

        /// <summary>
        /// Retrieve a product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of products.</returns>
        [HttpGet("{productId}")]
        [Produces("application/json"), Consumes("application/json")]
        [SwaggerOperation(OperationId = "get_detail", Tags = new[] { "Product" })]
        [ProducesResponseType(typeof(IProductDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                var query = mapper.Map<GetProductQuery>(productId);

                var data = await mediator.Send(query);

                return Ok(data);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of products.</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "post", Tags = new[] { "Product" })]
        [ProducesResponseType(typeof(IProductDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                var request = mapper.Map<CreateProductCommand>(payload);
                var result = await mediator.Send(request, cancellationToken);

                return Accepted(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Modify existing product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of products.</returns>
        [HttpPut("{productId}")]
        [SwaggerOperation(OperationId = "put", Tags = new[] { "Product" })]
        [ProducesResponseType(typeof(IProductDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(Guid productId, UpdateProductRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = productId;
                var request = mapper.Map<UpdateProductCommand>(payload);
                var result = await mediator.Send(request, cancellationToken);

                return Accepted(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Remove existing product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The set of product.</returns>
        [HttpDelete("{productId}")]
        [SwaggerOperation(OperationId = "delete", Tags = new[] { "Product" })]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Delete(Guid productId, DeleteProductRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = productId;
                var request = mapper.Map<DeleteProductCommand>(payload);
                var result = await mediator.Send(request, cancellationToken);

                return Accepted();
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }
    }
}
