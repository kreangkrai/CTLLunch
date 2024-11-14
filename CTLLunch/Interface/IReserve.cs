using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IReserve
    {
        Task<List<ReserveModel>> GetReserves();
        Task<List<ReserveModel>> GetReserveByDate(DateTime date);
        Task<List<ReserveModel>> GetReserveByDateEmployee(DateTime date,string employee_id);
        Task<List<ReserveModel>> GetReserveByShopDateEmployee(string shop_id,DateTime date, string employee_id);
        Task<List<ReserveModel>> GetReserveByShopDate(string shop_id, DateTime date);
        Task<string> Insert(ReserveModel reserve);
        Task<string> UpdateDelivery(ReserveModel reserve);
        Task<string> UpdateStatus(string reserve_id , string status );
        Task<string> UpdateReview(string reserve_id, int review);
        Task<AmountDeliveryBalanceModel> ComputeAmountDeliveryBalance(int delivery_service, int count_reserve, int current_balance);
    }
}
