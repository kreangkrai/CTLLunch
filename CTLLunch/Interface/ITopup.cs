using CTLLunch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface ITopup
    {
        Task<List<TopupModel>> GetTopups();
        Task<List<TopupModel>> GetTopupByEmployee(string employee_id);
        Task<string> Insert(TopupModel model);
        Task<string> UpdateStatus(TopupModel model);
    }
}
