using System;
using Moq;
using NUnit.Framework;

namespace CreditSwiss.Orders.Service.Tests
{
    [TestFixture]
    public class EquityOrderTests
    {
        private EquityOrder _sut;

        private Mock<IOrderService> _orderServiceMock;
        private EquityOrderFactory _equityOrderFactory;
        const string EQUITY_CODE = "ecxxxx";
        const int TRESHOLD = 10;

        [SetUp]
        public void SetUp()
        {
            this._orderServiceMock = new Mock<IOrderService>();

            this._equityOrderFactory = new EquityOrderFactory(this._orderServiceMock.Object);

            this._sut = _equityOrderFactory.Create(EQUITY_CODE, 10, TRESHOLD);
        }


        [Test]
        public void TickBelowTreshhold_SuccessEventReceived()
        {
            // arrange
            OrderPlacedEventArgs args = null;
            OrderErroredEventArgs errorArgs = null;

            _sut.OrderPlaced += evtArgs => { args = evtArgs; };
            _sut.OrderErrored += evtArgs => { errorArgs = evtArgs; };

            // act
            _sut.ReceiveTick(EQUITY_CODE, TRESHOLD - 1);

            // assert
            Assert.IsNull(errorArgs);
            Assert.IsTrue(_sut.Completed);
            Assert.AreSame(EQUITY_CODE, args.EquityCode);
            Assert.AreEqual(TRESHOLD - 1, args.Price);
            _orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
            _orderServiceMock.Verify(x => x.Sell(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Test]
        public void TickAboveOrSameTreshhold_BuyIsNotExecuted()
        {
            // arrange
            OrderPlacedEventArgs args = null;
            OrderErroredEventArgs errorArgs = null;

            _sut.OrderPlaced += evtArgs => { args = evtArgs; };
            _sut.OrderErrored += evtArgs => { errorArgs = evtArgs; };

            // act
            _sut.ReceiveTick(EQUITY_CODE, TRESHOLD);
            _sut.ReceiveTick(EQUITY_CODE, TRESHOLD + 1);

            // assert
            Assert.IsNull(args);
            Assert.IsNull(errorArgs);
            Assert.IsFalse(_sut.Completed);
            _orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
            _orderServiceMock.Verify(x => x.Sell(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Test]
        public void TickBelowTreshhold_OrderFailed_ErrorEventReceived()
        {
            // arrange
            OrderPlacedEventArgs args = null;
            OrderErroredEventArgs errorArgs = null;

            _sut.OrderPlaced += evtArgs => { args = evtArgs; };
            _sut.OrderErrored += evtArgs => { errorArgs = evtArgs; };

            _orderServiceMock.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Throws<Exception>();

            // act
            _sut.ReceiveTick(EQUITY_CODE, TRESHOLD - 1);

            // assert
            Assert.IsNull(args);
            Assert.IsTrue(_sut.Completed);
            Assert.AreSame(EQUITY_CODE, errorArgs.EquityCode);
            Assert.AreEqual(TRESHOLD - 1, errorArgs.Price);
            _orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
            _orderServiceMock.Verify(x => x.Sell(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NoEventsAssigned_AppRemainsStable(bool orderPass)
        {
            // arrange
            if (!orderPass)
            {
                _orderServiceMock.Setup(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
                    .Throws<Exception>();
            }

            // act
            _sut.ReceiveTick(EQUITY_CODE, TRESHOLD - 1);

            // assert
            Assert.IsTrue(_sut.Completed);
            _orderServiceMock.Verify(x => x.Buy(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Once);
            _orderServiceMock.Verify(x => x.Sell(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()),
                Times.Never);
        }

    }
}