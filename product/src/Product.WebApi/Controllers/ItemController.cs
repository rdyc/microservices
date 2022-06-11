using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Product.Contract.Command;
using Product.Contract.Query;
using Product.WebApi.Model;

namespace Product.WebApi.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<ItemController> logger;

        public ItemController(IMapper mapper, IMediator mediator, ILogger<ItemController> logger)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var data = await mediator.Send(new GetAllItemsQuery(), cancellationToken);

                return Ok(data);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                var request = mapper.Map<CreateItemCommand>(payload);
                var result = await mediator.Send(request, cancellationToken);

                return Accepted(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = id;
                var request = mapper.Map<UpdateItemCommand>(payload);
                var result = await mediator.Send(request, cancellationToken);

                return Accepted(result);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Post(Guid id, DeleteItemRequest payload, CancellationToken cancellationToken)
        {
            try
            {
                payload.Id = id;
                var request = mapper.Map<DeleteItemCommand>(payload);
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
