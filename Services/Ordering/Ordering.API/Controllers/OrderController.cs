﻿using Common.Logging.Correlation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;
using System.Net;

namespace Ordering.API.Controllers
{
    public class OrderController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public OrderController(IMediator mediator, ILogger<OrderController> logger, ICorrelationIdGenerator correlationIdGenerator)
        {
            _mediator = mediator;
            _logger = logger;
            _correlationIdGenerator = correlationIdGenerator;
            _logger.LogInformation("CorrelationId {correlationId}:", _correlationIdGenerator.Get());
        }

        [HttpGet("{userName}", Name = "GetOrdersByUserName")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByUserName(string userName)
        {
            var query = new GetOrderListQuery(userName);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        //Just for testing locally as it will be processed in queue
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var cmd = new DeleteOrderCommand() { Id = id };
            await _mediator.Send(cmd);
            return NoContent();
        }

    }
}
