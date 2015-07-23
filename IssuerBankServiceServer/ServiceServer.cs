using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IssuerBankServiceLibrary;
namespace IssuerBankServiceServer
{
    public class ServiceServer : MarshalByRefObject, IIssuerService
    {
        dbBankIssuerDataContext dbAction = new dbBankIssuerDataContext();
        public string checkAndPay(string receivedData)
        {
            string returnFalse = "false";
            Console.WriteLine("Someone Is Consuming Your Service");
            try
            {
                /*split the mac and the data, with "--***--" delimiter*/
                string[] splitResult1 = receivedData.Split(new string[] { "--***--" }, StringSplitOptions.None);
                string allData = splitResult1[0];
                string mac = splitResult1[1];

                /*validate data integrity with MAC checker*/
                if (EncryptionController.ValidateSHA1HashData(allData, mac))
                {
                    Console.WriteLine("MAC Data is Valid");
                    /*if validated, then split the encryptedPaymentDetail and the signedData with "**---**" delimiter*/
                    string[] splitResult2 = allData.Split(new string[] { "**---**" }, StringSplitOptions.None);
                    string encryptedPaymentDetail = splitResult2[0];
                    string signedData = splitResult2[1];

                    /*verify the data sender by it's sign*/
                    /*load certificate of MerchantApp to get its public key*/
                    X509Certificate2 merchantAppCertificate = EncryptionController.LoadCertificate(StoreLocation.LocalMachine, "CN=MerchantApp");
                    if (EncryptionController.VerifyData(encryptedPaymentDetail, signedData, merchantAppCertificate))
                    {
                        Console.WriteLine("SIGNED Data is Valid");
                        /*Load my certificate to get my private key*/
                        X509Certificate2 myCertificate = new X509Certificate2("E:/IssuerBank.pfx", "pidel123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

                        /*if verified, decrypt the encryptedPaymentDetail*/
                        string decryptedPaymentDetail = EncryptionController.Decrypt(myCertificate, encryptedPaymentDetail);
                        Console.WriteLine(decryptedPaymentDetail);
                        string[] splitResult3 = decryptedPaymentDetail.Split(new string[] { "**~~~**" }, StringSplitOptions.None);
                        string cc_number = splitResult3[0];
                        string stringAmount = splitResult3[1];
                        Console.WriteLine(stringAmount);
                        string[] amountSplit = stringAmount.Split(',');
                        decimal amount = Convert.ToDecimal(amountSplit[0]);
                        Console.WriteLine("Add new debt : "+amount);
                        var cc = (from log in dbAction.credit_cards where (cc_number == log.cc_number) select log).FirstOrDefault();
                        if (cc_number == cc.cc_number)
                        {
                            Console.WriteLine("Account / Credit Card Data is Valid");
                            trade trd = new trade()
                            {
                                id_account = cc.id_account,
                                amount = amount,
                                time = DateTime.Now,
                                description = "From Merchant App",
                                issuer = "MerchantApp",
                                trade_type = "3"
                            };
                            dbAction.trades.InsertOnSubmit(trd);
                            dbAction.SubmitChanges();
                            Console.WriteLine("All Data is Valid");
                            Console.WriteLine("Now tell the Merchant Bank About The Amount");
                            X509Certificate2 merchantPUCertificate = EncryptionController.LoadCertificate(StoreLocation.LocalMachine, "CN=MerchantBank");
                            string encryptedAmountToMerchantbank = EncryptionController.Encrypt(merchantPUCertificate, amount.ToString());
                            string signedAmountToMerchantbank = EncryptionController.addDigitalSignature(encryptedAmountToMerchantbank);

                            string encryptedAndSignedToMerchantbank = encryptedAmountToMerchantbank + "**---**" + signedAmountToMerchantbank;
                            string macToMerchantbank = EncryptionController.GetSHA1HashData(encryptedAndSignedToMerchantbank);
                            string dataToSendToMerchantbank = encryptedAndSignedToMerchantbank + "--***--" + macToMerchantbank;
                            Console.WriteLine("Data Encrypted, Now SEND it");
                            return dataToSendToMerchantbank;
                        }
                        else
                        {
                            Console.WriteLine("Account is not existed.");
                            return returnFalse;
                        }
                    }
                    else 
                    {
                        Console.WriteLine("Data is not sent by Merchant App, do not proceed");
                        return returnFalse;
                    }

                }
                else 
                {
                    Console.WriteLine("Mac is not valid, data may changed");
                    return returnFalse;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                return returnFalse;
            }
        }
    }
}
