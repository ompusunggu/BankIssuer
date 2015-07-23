using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankIssuer.Models;

namespace BankIssuer.Controllers
{
    public class TradeController : Controller
    {
        dbBankIssuerDataContext db = new dbBankIssuerDataContext();

        public ActionResult Index()
        {
            IList<TradeModels> ListTrade = new List<TradeModels>();
            var qryTrade = from trade in db.trades
                           select new TradeModels
                           {
                               amount = trade.amount,
                               time = trade.time,
                               description = trade.description,
                               issuer = trade.issuer,
                               trade_type = trade.trade_type
                           };
            ListTrade = qryTrade.ToList();
            return View(ListTrade);
        }

    }
}
