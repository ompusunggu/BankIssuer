using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankIssuer.Models;

namespace BankIssuer.Controllers
{
    public class CreditCardController : Controller
    {
        dbBankIssuerDataContext db = new dbBankIssuerDataContext();

        public ActionResult Index(int _idAccount)
        {
            if (_idAccount != 0 && Session["id_account"] != null)
            {
                CreditCardModels model = db.credit_cards.Where(x => x.id_account.ToString() == Session["id_account"]).Select(x =>
                    new CreditCardModels()
                    {
                        id_creditCard = x.id_creditCard,
                        id_account = x.id_account,
                        limit = x.limit,
                        cc_number = x.cc_number,
                        exp_date = x.expiry_date
                    }).SingleOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    model = db.credit_cards.Where(x => x.id_account.ToString() == Session["id_account"]).Select(x =>
                    new CreditCardModels()
                    {
                        id_creditCard = x.id_creditCard,
                        id_account = x.id_account,
                        limit = x.limit,
                        cc_number = x.cc_number,
                        exp_date = x.expiry_date
                    }).SingleOrDefault();

                    return View(model);
                    //return RedirectToAction("LogIn", "Account");
                }
            }
            else
            {
                return RedirectToAction("LogIn", "Account");
            }
        }
    }
}
