using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankIssuer.Models;
using System.Web.Security;

namespace BankMerchant.Controllers
{
    public class AccountController : Controller
    {
        dbBankIssuerDataContext db = new dbBankIssuerDataContext();

        public ActionResult Index(int _idAccount = 0)
        {
            if (_idAccount != 0 && Session["id_account"] != null)
            {
                AccountModels model = db.accounts.Where(x => x.id_account.ToString() == Session["id_account"]).Select(x =>
                    new AccountModels()
                    {
                        name = x.name,
                        balance = x.balance,
                        account_number = x.account_number
                    }).SingleOrDefault();
                if (model != null && Session["id_account"].ToString() == _idAccount.ToString())
                {
                    return View(model);
                }
                else
                {
                    model = db.accounts.Where(x => x.id_account.ToString() == Session["id_account"]).Select(x =>
                    new AccountModels()
                    {
                        name = x.name,
                        balance = x.balance,
                        account_number = x.account_number
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

        [HttpGet]
        public ActionResult LogIn()
        {
            AccountModels model = new AccountModels();
            return View(model);
        }

        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            AccountModels account = new AccountModels();
            var login = (from log in db.accounts
                         where (username == log.username)
                         select log).First();

            if ((login != null && account.IsValid(login.password, password)))
            {
                account.username = login.username;
                account.id_account = login.id_account;
                Session["id_account"] = login.id_account;
                FormsAuthentication.SetAuthCookie(login.username, false);
                //return RedirectToAction("LogIn");
                return RedirectToAction("Index", "Account", new { _idAccount = Session["id_account"] });
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(account);
                //return RedirectToAction("Index", new { _idAccount = account.id_account });
            }
        }

        [HttpGet]
        public ActionResult Logout(string username)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}
