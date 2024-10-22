using CTLLunch.Models;
using System;

namespace CTLLunch.Interface
{
    public interface IAuthen
    {
        AuthenModel ActiveDirectoryAuthenticate(string username, string password);
    }
}
