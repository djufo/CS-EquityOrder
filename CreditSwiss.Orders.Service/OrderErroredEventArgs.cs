using System;
using System.IO;

namespace CreditSwiss.Orders.Service
{
    public delegate void OrderErroredEventHandler(OrderErroredEventArgs e);

    public class OrderErroredEventArgs : ErrorEventArgs
    {
        public OrderErroredEventArgs(string equityCode, decimal price, Exception ex) : base(ex)
        {
            EquityCode = equityCode;
            Price = price;
        }
        public string EquityCode { get; }
        public decimal Price { get; }
    }
}
