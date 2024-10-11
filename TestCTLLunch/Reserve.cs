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
        public void TestComputeAmountDeliveryBalance(int delivery_service,int count_reserve,int current_balance, int expect_delivery , int expect_balance)
        {
            AmountDeliveryBalanceModel actual = _Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, current_balance);
            AmountDeliveryBalanceModel expect = new AmountDeliveryBalanceModel() { delivery_service = expect_delivery, balance = expect_balance };
            Assert.AreEqual(expect.balance, actual.balance);
            Assert.AreEqual(expect.delivery_service, actual.delivery_service);
        }
    }
}
