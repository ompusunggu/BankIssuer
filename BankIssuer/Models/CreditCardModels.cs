using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankIssuer.Models
{
    public class CreditCardModels
    {
        public int id_creditCard { set; get; }
        public int id_account { set; get; }
        public decimal limit { set; get; }
        public string cc_number { set; get; }
        public DateTime exp_date { set; get; }
    }
}