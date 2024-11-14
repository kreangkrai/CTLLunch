using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IGroup
    {
        Task<List<GroupMenuModel>> GetGroups();
        Task<string> GetLastID();
        Task<string> Insert(GroupMenuModel group);
        Task<string> Update(GroupMenuModel group);
        Task<string> Delete(string group_id);
    }
}
