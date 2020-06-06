using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class OrderController : ControllerBase
    {
        private static readonly Dictionary<Guid, List<OrderReadModel>> Orders =
            new Dictionary<Guid, List<OrderReadModel>>();

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet("customers/{customerId}/orders")]
        public IEnumerable<OrderReadModel> GetList(Guid customerId, [FromQuery] Currency currency)
        {
            if (!Orders.ContainsKey(customerId))
                return new OrderReadModel[] { };

            return Orders[customerId].Where(x => currency == 0 || x.Currency == currency);
        }

        [HttpPost("customers/{customerId}/orders")]
        [Consumes("application/json")]
        public Guid CreateOrder(Guid customerId, OrderCreateModel order)
        {
            return InnerCreateOrder(customerId, order);
        }

        [HttpPost("customers/{customerId}/orders-form")]
        [Consumes("multipart/form-data")]
        public Guid CreateOrderWithForm(Guid customerId, [FromForm] OrderCreateModel order)
        {
            return InnerCreateOrder(customerId, order);
        }

        private Guid InnerCreateOrder(Guid customerId, OrderCreateModel order)
        {
            var rnd = new Random(10000);
            var model = new OrderReadModel()
            {
                Goods = order.Goods,
                ArriveAt = DateTime.UtcNow.AddDays(2),
                OrderId = Guid.NewGuid(),
                Cost = (decimal) rnd.NextDouble(),
                Currency = order.Currency
            };
            if (!Orders.ContainsKey(customerId))
                Orders.Add(customerId, new List<OrderReadModel>() {model});
            else
                Orders[customerId].Add(model);

            return model.OrderId;
        }

        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [HttpGet("customers/{customerId}/orders/{orderId}")]
        public ActionResult<OrderReadModel> GetById(Guid customerId, Guid orderId)
        {
            if (!Orders.ContainsKey(customerId))
                return NotFound();

            var order = Orders[customerId].FirstOrDefault(x => x.OrderId == orderId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}