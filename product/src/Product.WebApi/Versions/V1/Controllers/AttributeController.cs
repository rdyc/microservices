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

namespace Product.WebApi.Versions.V1.Controllers;

/// <summary>
/// The attribute v1 api controller
/// </summary>
[ApiController]
[Produces("application/json"), Consumes("application/json")]
[ApiVersion("1"), Route("v{version:apiVersion}/attributes")]
public class AttributeController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;
    private readonly ILogger<AttributeController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeController"/> class.
    /// </summary>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AttributeController(IMapper mapper, IMediator mediator, ILogger<AttributeController> logger)
    {
        this.mapper = mapper;
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <summary>
    /// Retrieve all attributes.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection set of attributes.</returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "get", Tags = new[] { "Attribute" })]
    [ProducesResponseType(typeof(GetAllAttributesResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromQuery] GetAllAttributesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var query = mapper.Map<GetAllAttributesQuery>(request);

            var data = await mediator.Send(query);

            var result = mapper.Map<GetAllAttributesResponse>(data);

            return Ok(result);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Retrieve a attribute.
    /// </summary>
    /// <param name="attributeId">The attribute id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection set of attributes.</returns>
    [HttpGet("{attributeId}")]
    [Produces("application/json"), Consumes("application/json")]
    [SwaggerOperation(OperationId = "get_detail", Tags = new[] { "Attribute" })]
    [ProducesResponseType(typeof(IAttributeDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(Guid attributeId, CancellationToken cancellationToken)
    {
        try
        {
            var query = mapper.Map<GetAttributeQuery>(attributeId);

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
    /// Create a new attribute.
    /// </summary>
    /// <param name="payload">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection set of attributes.</returns>
    [HttpPost]
    [SwaggerOperation(OperationId = "post", Tags = new[] { "Attribute" })]
    [ProducesResponseType(typeof(IAttributeDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Post([FromBody] CreateAttributeRequest payload, CancellationToken cancellationToken)
    {
        try
        {
            var request = mapper.Map<CreateAttributeCommand>(payload);
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
    /// Modify existing attribute.
    /// </summary>
    /// <param name="attributeId">The attribute id.</param>
    /// <param name="payload">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection set of attributes.</returns>
    [HttpPut("{attributeId}")]
    [SwaggerOperation(OperationId = "put", Tags = new[] { "Attribute" })]
    [ProducesResponseType(typeof(IAttributeDto), (int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> Put(Guid attributeId, UpdateAttributeRequest payload, CancellationToken cancellationToken)
    {
        try
        {
            payload.Id = attributeId;
            var request = mapper.Map<UpdateAttributeCommand>(payload);
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
    /// Remove existing attribute.
    /// </summary>
    /// <param name="attributeId">The attribute id.</param>
    /// <param name="payload">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The set of attribute.</returns>
    [HttpDelete("{attributeId}")]
    [SwaggerOperation(OperationId = "delete", Tags = new[] { "Attribute" })]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> Delete(Guid attributeId, DeleteAttributeRequest payload, CancellationToken cancellationToken)
    {
        try
        {
            payload.Id = attributeId;
            var request = mapper.Map<DeleteAttributeCommand>(payload);
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