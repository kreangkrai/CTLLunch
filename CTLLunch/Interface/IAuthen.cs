using CTLLunch.Models;
using System;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IAuthen
    {
        Task<AuthenModel> ActiveDirectoryAuthenticate(string username, string password);
    }
}
