using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Dto;
using Product.Contract.Query;
using Product.WebApi.Versions.V2.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Product.WebApi.Controllers
{
    /// <summary>
    /// The item api controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("2"), Route("v{version:apiVersion}/items")]
    public class ItemControllerV2 : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<ItemControllerV2> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemControllerV2"/> class.
        /// </summary>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ItemControllerV2(IMapper mapper, IMediator mediator, ILogger<ItemControllerV2> logger)
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
    }
}
