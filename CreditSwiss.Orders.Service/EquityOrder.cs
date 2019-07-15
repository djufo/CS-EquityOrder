using System;

namespace CreditSwiss.Orders.Service
{
    public class EquityOrder: IEquityOrder
    {
        private readonly object _lock = new object();
        private readonly IOrderService _orderService;

        public bool Completed { get; private set; } = false;
        public string EquityCode { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        
        public event OrderPlacedEventHandler OrderPlaced;
        public event OrderErroredEventHandler OrderErrored;
        
        public EquityOrder(IOrderService orderService, string equityCode, int quantity, decimal price)
        {
            _orderService = orderService;
            this.EquityCode = equityCode;
            this.Quantity = quantity;
            this.Price = price;
        }
        
        public void ReceiveTick(string equityCode, decimal price)
        {
            if (Completed || price >= this.Price) return;
            lock (this._lock)
            {
                if (Completed) return;

                try
                {
                    this._orderService.Buy(this.EquityCode, this.Quantity, price);
                    OrderPlaced?.Invoke(new OrderPlacedEventArgs(this.EquityCode, price));
                }
                catch (Exception ex)
                {
                    OrderErrored?.Invoke(new OrderErroredEventArgs(this.EquityCode, price, ex));
                }
                finally
                {
                    this.Completed = true;
                }
            }
        }
    }
}
