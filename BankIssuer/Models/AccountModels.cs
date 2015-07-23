using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Threading.Tasks;
using Helpers;


namespace BankIssuer.Models
{
    public class AccountModels
    {
        dbBankIssuerDataContext db = new dbBankIssuerDataContext();

        public int id_account { set; get; }
        public string username { set; get; }
        public string password { set; get; }
        public string name { set; get; }
        public decimal balance { set; get; }
        public int account_number { set; get; }
        public Boolean RememberMe { set; get; }

        public bool IsValid(string _password, string _insertedPassword)
        {

            SHA1 hashSHA = new SHA1();
            //TempData["data"] = Helpers.SHA1.Encode(_insertedPassword);
            return _password.Equals(Helpers.SHA1.Encode(_insertedPassword));
        }
    }
}