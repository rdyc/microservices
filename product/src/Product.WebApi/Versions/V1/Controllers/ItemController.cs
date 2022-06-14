using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Command;
using Product.Contract.Dto;
using Product.Contract.Query;
using Product.WebApi.Versions.V1.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Product.WebApi.Versions.V1.Controllers
{
    /// <summary>
    /// The item v1 api controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("1"), Route("v{version:apiVersion}/items")]
    public class ItemController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<ItemController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ItemController(IMapper mapper, IMediator mediator, ILogger<ItemController> logger)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieve all product items.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of items.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "get", Tags = new[] { "Product Item" })]
        [ProducesResponseType(typeof(IEnumerable<IItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] GetAllItemsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetAllItemsQuery query = mapper.Map<GetAllItemsQuery>(request);

                IEnumerable<IItemDto> data = await mediator.Send(query);

                return Ok(data);
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
        /// <param name="itemId">The product item id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of items.</returns>
        [HttpGet("{itemId}")]
        [Produces("application/json"), Consumes("application/json")]
        [SwaggerOperation(OperationId = "get_detail", Tags = new[] { "Product Item" })]
        [ProducesResponseType(typeof(IItemDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Guid itemId, CancellationToken cancellationToken)
        {
            try
            {
                GetItemQuery query = mapper.Map<GetItemQuery>(itemId);

                IItemDto data = await mediator.Send(query);

                return Ok(data);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new product item.
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of items.</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "post", Tags = new[] { "Product Item" })]
        [ProducesResponseType(typeof(IItemDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] CreateItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                CreateItemCommand request = mapper.Map<CreateItemCommand>(payload);
                IItemDto result = await mediator.Send(request, cancellationToken);

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
        /// <param name="itemId">The product item id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of items.</returns>
        [HttpPut("{itemId}")]
        [SwaggerOperation(OperationId = "put", Tags = new[] { "Product Item" })]
        [ProducesResponseType(typeof(IItemDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(Guid itemId, UpdateItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = itemId;
                UpdateItemCommand request = mapper.Map<UpdateItemCommand>(payload);
                IItemDto result = await mediator.Send(request, cancellationToken);

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
        /// <param name="itemId">The product item id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The set of item.</returns>
        [HttpDelete("{itemId}")]
        [SwaggerOperation(OperationId = "delete", Tags = new[] { "Product Item" })]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Delete(Guid itemId, DeleteItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = itemId;
                DeleteItemCommand request = mapper.Map<DeleteItemCommand>(payload);
                IItemDto result = await mediator.Send(request, cancellationToken);

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
