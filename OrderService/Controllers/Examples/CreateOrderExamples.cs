using System.Collections.Generic;
using OrderService.Models;
using Swashbuckle.AspNetCore.Filters;

namespace OrderService.Controllers.Examples
{
    public class CreateOrderExamples: IMultipleExamplesProvider<OrderCreateModel>
    {
        public IEnumerable<SwaggerExample<OrderCreateModel>> GetExamples()
        {
            yield return SwaggerExample.Create("Пример создания",
                new OrderCreateModel()
                {
                    Currency = Currency.Ru,
                    Goods = new[]
                    {
                        "Cheese",
                        "Bread"
                    },
                    Number = "786251",
                    TotalAmount = (decimal) 103.12,
                    InnerNumber = new InnerNumber()
                    {
                        Number = "1234"
                    }
                });
        }
    }
}