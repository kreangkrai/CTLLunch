using CTLLunch.Interface;
using CTLLunch.Models;
using CTLLunch.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCTLLunch
{
    [TestClass]
    public class Reserve
    {
        private IReserve _Reserve;
        public Reserve()
        {
            _Reserve = new ReserveService();
        }
        [TestMethod]
        [DataRow(40, 7, 5, 5, 0)]
        [DataRow(40, 8, 5, 5, 5)]
        [DataRow(40, 6, 2, 7, 4)]
        [DataRow(30, 11, 2, 3, 5)]
        [DataRow(0, 11, 2, 0, 2)]
        [DataRow(0, 0, 0, 0, 0)]
        [DataRow(40, 6, 5, 6, 1)]

        [DataRow(0, 10, 0, 0, 0)]
        [DataRow(1, 10, 0, 1, 9)]
        [DataRow(2, 10, 0, 1, 8)]
        [DataRow(3, 10, 0, 1, 7)]
        [DataRow(4, 10, 0, 1, 6)]
        [DataRow(5, 10, 0, 1, 5)]
        [DataRow(6, 10, 0, 1, 4)]
        [DataRow(7, 10, 0, 1, 3)]
        [DataRow(8, 10, 0, 1, 2)]
        [DataRow(9, 10, 0, 1, 1)]
        [DataRow(10, 10, 0, 1, 0)]
        [DataRow(11, 10, 0, 2, 9)]

        [DataRow(40, 1, 0, 40, 0)]
        [DataRow(40, 2, 0, 20, 0)]
        [DataRow(40, 3, 0, 14, 2)]
        [DataRow(40, 4, 0, 10, 0)]
        [DataRow(40, 5, 0, 8, 0)]
        [DataRow(40, 6, 0, 7, 2)]
        [DataRow(40, 7, 0, 6, 2)]
        [DataRow(40, 8, 0, 5, 0)]
        [DataRow(40, 9, 0, 5, 5)]
        [DataRow(40, 10, 0, 4, 0)]
        [DataRow(40, 11, 0, 4, 4)]
        [DataRow(40, 12, 0, 4, 8)]
        [DataRow(40, 13, 0, 4, 12)]
        [DataRow(40, 14, 0, 3, 2)]
        [DataRow(40, 15, 0, 3, 5)]
        [DataRow(40, 16, 0, 3, 8)]
        [DataRow(40, 17, 0, 3, 11)]
        [DataRow(40, 18, 0, 3, 14)]

        [DataRow(40, 10, 10, 4, 10)]
        [DataRow(40, 11, 10, 3, 3)]
        [DataRow(40, 12, 10, 3, 6)]
        [DataRow(40, 13, 10, 3, 9)]
        [DataRow(40, 14, 10, 3, 12)]
        [DataRow(40, 15, 10, 2, 0)]
        [DataRow(40, 16, 10, 2, 2)]
        [DataRow(40, 17, 10, 2, 4)]
        [DataRow(40, 18, 10, 2, 6)]
        [DataRow(40, 19, 10, 2, 8)]
        [DataRow(40, 20, 10, 2, 10)]

        public void TestComputeAmountDeliveryBalance(int delivery_service,int count_reserve,int current_balance, int expect_delivery , int expect_balance)
        {
            AmountDeliveryBalanceModel actual = _Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, current_balance);
            AmountDeliveryBalanceModel expect = new AmountDeliveryBalanceModel() { delivery_service = expect_delivery, balance = expect_balance };
            Assert.AreEqual(expect.balance, actual.balance);
            Assert.AreEqual(expect.delivery_service, actual.delivery_service);
        }
    }
}
