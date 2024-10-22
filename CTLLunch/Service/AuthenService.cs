using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.DirectoryServices;
namespace CTLLunch.Service
{
    public class AuthenService : IAuthen
    {
        public AuthenModel ActiveDirectoryAuthenticate(string username, string password)
        {
            bool userOk = false;
            byte[] image = new byte[0];
            string user = "";
            string dep = "";
            AuthenModel authen = new AuthenModel();
            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://192.168.15.1", username, password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = "(samaccountname=" + username + ")";
                        searcher.PropertiesToLoad.Add("displayname");
                        searcher.PropertiesToLoad.Add("thumbnailPhoto");
                        searcher.PropertiesToLoad.Add("department");

                        SearchResult adsSearchResult = searcher.FindOne();

                        if (adsSearchResult != null)
                        {

                            var prop = adsSearchResult.Properties["thumbnailPhoto"];
                            if (adsSearchResult.Properties["displayname"].Count == 1)
                            {
                                user = (string)adsSearchResult.Properties["displayname"][0];
                                dep = (string)adsSearchResult.Properties["department"][0];
                                var img = adsSearchResult.Properties["thumbnailPhoto"].Count;
                                if (img > 0)
                                {
                                    image = adsSearchResult.Properties["thumbnailPhoto"][0] as byte[];
                                }
                            }
                            userOk = true;
                        }

                        authen = new AuthenModel()
                        {
                            authen = userOk,
                            user = user,
                            department = dep,
                            image = image
                        };
                        return authen;
                    }
                }
            }
            catch
            {
                return authen;
            }
        }
    }
}
