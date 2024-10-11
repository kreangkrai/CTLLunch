using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IReserve
    {
        List<ReserveModel> GetReserves();
        List<ReserveModel> GetReserveByDate(DateTime date);
        List<ReserveModel> GetReserveByDateEmployee(DateTime date,string employee_id);
        List<ReserveModel> GetReserveByShopDateEmployee(string shop_id,DateTime date, string employee_id);
        List<ReserveModel> GetReserveByShopDate(string shop_id, DateTime date);
        string Insert(ReserveModel reserve);
        string UpdateDelivery(ReserveModel reserve);
        string UpdateStatus(string reserve_id , string status );
        string UpdateReview(string reserve_id, int review);
        AmountDeliveryBalanceModel ComputeAmountDeliveryBalance(int delivery_service, int count_reserve, int current_balance);
    }
}
