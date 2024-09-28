using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IGroup
    {
        List<GroupMenuModel> GetGroups();
        string GetLastID();
        string Insert(GroupMenuModel group);
        string Update(GroupMenuModel group);
        string Delete(string group_id);
    }
}
