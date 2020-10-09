using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Data
{
   
    public struct MinecraftAccount
    {
        public string Email, Password;

        public MinecraftAccount(string Email,string Password)
        {
            this.Email = Email;
            this.Password = Password;
        }
    }

    public static class Accounts
    {
        public static List<MinecraftAccount> LoadedAccounts = new List<MinecraftAccount>();

        public static void LoadAccounts()
        {
            CheckForDataDirectory();
            CheckForAccountsDirectory();

            
            foreach(string FileName in Directory.GetFiles("Data/Accounts"))
            {
                if (FileName.Contains(".json"))
                {
                    MinecraftAccount Account = JsonConvert.DeserializeObject<MinecraftAccount>(FileName);
                    LoadedAccounts.Add(Account);
                }
            }
        }

        public static void SaveAccount(MinecraftAccount Account)
        {
            CheckForDataDirectory();

            string Json = JsonConvert.SerializeObject(Account);

            CheckForAccountsDirectory();

            File.WriteAllText("Data/Accounts/" + Account.Email + ".json", Json);
        }

        public static void SaveAccounts()
        {
            foreach(MinecraftAccount Account in LoadedAccounts)
            {
                SaveAccount(Account);
            }
        }

        private static void CheckForDataDirectory()
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
        }

        private static void CheckForAccountsDirectory()
        {
            if (!Directory.Exists("Data/Accounts"))
            {
                Directory.CreateDirectory("Data/Accounts");
            }
        }
    }
}
