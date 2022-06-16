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
    /// The currency v1 api controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("1"), Route("v{version:apiVersion}/currencies")]
    public class CurrencyController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<CurrencyController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="logger">The logger instance.</param>
        public CurrencyController(IMapper mapper, IMediator mediator, ILogger<CurrencyController> logger)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieve all currencies.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of currencies.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "get", Tags = new[] { "Currency" })]
        [ProducesResponseType(typeof(GetAllCurrenciesResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] GetAllCurrenciesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = mapper.Map<GetAllCurrenciesQuery>(request);

                var data = await mediator.Send(query);

                var result = mapper.Map<GetAllCurrenciesResponse>(data);

                return Ok(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Retrieve a currency.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of currencies.</returns>
        [HttpGet("{currencyId}")]
        [Produces("application/json"), Consumes("application/json")]
        [SwaggerOperation(OperationId = "get_detail", Tags = new[] { "Currency" })]
        [ProducesResponseType(typeof(ICurrencyDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Guid currencyId, CancellationToken cancellationToken)
        {
            try
            {
                var query = mapper.Map<GetCurrencyQuery>(currencyId);

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
        /// Create a new currency.
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of currencies.</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "post", Tags = new[] { "Currency" })]
        [ProducesResponseType(typeof(ICurrencyDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] CreateCurrencyRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                var request = mapper.Map<CreateCurrencyCommand>(payload);
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
        /// Modify existing currency.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection set of currencies.</returns>
        [HttpPut("{currencyId}")]
        [SwaggerOperation(OperationId = "put", Tags = new[] { "Currency" })]
        [ProducesResponseType(typeof(ICurrencyDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(Guid currencyId, UpdateCurrencyRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = currencyId;
                var request = mapper.Map<UpdateCurrencyCommand>(payload);
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
        /// Remove existing currency.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <param name="payload">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The set of currency.</returns>
        [HttpDelete("{currencyId}")]
        [SwaggerOperation(OperationId = "delete", Tags = new[] { "Currency" })]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Delete(Guid currencyId, DeleteCurrencyRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = currencyId;
                var request = mapper.Map<DeleteCurrencyCommand>(payload);
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
