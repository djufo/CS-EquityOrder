namespace CreditSwiss.Orders.Service
{
    public class EquityOrderFactory
    {
        private readonly IOrderService _orderService;
        public EquityOrderFactory(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        public EquityOrder Create(string equityCode, int quantity, decimal price)
        {
            return  new EquityOrder(this._orderService, equityCode, quantity, price);
        }
    }
}
