namespace CreditSwiss.Orders.Service
{
    public interface IEquityOrder: IOrderPlaced, IOrderErrored
    {       
        void ReceiveTick(string equityCode, decimal price);
    }
}
