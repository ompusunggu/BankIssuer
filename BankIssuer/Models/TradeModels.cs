using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankIssuer.Models
{

    public class TradeModels
    {
        dbBankIssuerDataContext db = new dbBankIssuerDataContext();

        public int id_trade { set; get; }
        public int id_account { set; get; }
        public decimal amount { set; get; }
        public DateTime time { set; get; }
        public string description { set; get; }
        public string issuer { set; get; }
        public string trade_type { set; get; }
    }
}