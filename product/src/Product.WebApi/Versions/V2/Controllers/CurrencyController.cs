using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Commands;
using Core.EventStoreDB.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Dtos;
using Product.Domain.Commands;
using Product.Domain.Models;
using Product.WebApi.Versions.V2.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Product.WebApi.Versions.V2.Controllers
{
    /// <summary>
    /// The currency v2 api controller
    /// </summary>
    [ApiController]
    [Produces("application/json"), Consumes("application/json")]
    [ApiVersion("2"), Route("v{version:apiVersion}/currencies")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IEventStoreDBRepository<CurrencyModel> repository;
        private readonly IMapper mapper;
        private readonly ILogger<CurrencyController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyController"/> class.
        /// </summary>
        /// <param name="commandBus">The command bus instance.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public CurrencyController(ICommandBus commandBus, IMapper mapper, ILogger<CurrencyController> logger, IEventStoreDBRepository<CurrencyModel> repository)
        {
            this.commandBus = commandBus;
            this.mapper = mapper;
            this.logger = logger;
            this.repository = repository;
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
                var command = new CreateCurrencyCmd(payload.Name, payload.Code, payload.Symbol);

                await commandBus.Send(command);

                return Accepted();
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
                var command = new UpdateCurrencyCmd(currencyId, payload.Name, payload.Code, payload.Symbol);

                await commandBus.Send(command);

                return Accepted();
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
                var data = await repository.Find(currencyId, cancellationToken);

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
