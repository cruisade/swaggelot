namespace OrderService.Models
{
    public class CreateOrderFromFormModel
    {
        public Currency Currency { get; set; }
        public string[] Goods { get; set; }
    }
}