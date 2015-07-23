using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace IssuerBankServiceServer
{
    class EncryptionController
    {
        public static X509Certificate2 LoadCertificate(StoreLocation storeLocation, string certificateName)
        {
            X509Store store = new X509Store(storeLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2 x509 = null;
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.Subject == certificateName)
                {
                    x509 = c;
                    break;
                }
            }

            if (x509 == null)
                Console.WriteLine("A x509 certificate for " + certificateName + " was not found");
            store.Close();
            return x509;
        }

        /*
         * Method for encrypt data
         * */
        public static string Encrypt(X509Certificate2 x509, string stringToEncrypt)
        {
            if (x509 == null || string.IsNullOrEmpty(stringToEncrypt))
                throw new Exception("A x509 certificate and string for encryption must be provided");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PublicKey.Key;
            byte[] bytestoEncrypt = ASCIIEncoding.ASCII.GetBytes(stringToEncrypt);
            byte[] encryptedBytes = rsa.Encrypt(bytestoEncrypt, false);

            string result = Convert.ToBase64String(encryptedBytes);
            return result;
        }

        /*
         * Method for decypt data
         * */
        public static string Decrypt(X509Certificate2 x509, string stringTodecrypt)
        {


            if (x509 == null || string.IsNullOrEmpty(stringTodecrypt))
                throw new Exception("A x509 certificate and string for decryption must be provided");

            if (!x509.HasPrivateKey)
                throw new Exception("x509 certicate does not contain a private key for decryption");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PrivateKey;
            byte[] bytestodecrypt = Convert.FromBase64String(stringTodecrypt);
            byte[] plainbytes = rsa.Decrypt(bytestodecrypt, false);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(plainbytes);
        }

        /*
         * Method for sign data 
         * */
        public static string SignData(string message, X509Certificate2 cert)
        {
            //// The array to store the signed message in bytes
            byte[] signedBytes;
            //// Write the message to a byte array using UTF8 as the encoding.
            var encoder = new UTF8Encoding();
            byte[] originalData = encoder.GetBytes(message);

            RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)cert.PrivateKey;

            try
            {
                //// Import the private key used for signing the message

                //// Sign the data, using SHA512 as the hashing algorithm 
                signedBytes = privateKey.SignData(originalData, CryptoConfig.MapNameToOID("SHA1"));
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                //// Set the keycontainer to be cleared when rsa is garbage collected.
                privateKey.PersistKeyInCsp = false;
            }
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }

        /*
         * Method to verify data sent
         * */
        public static bool VerifyData(string originalMessage, string signedMessage, X509Certificate2 cert)
        {
            bool success = false;
            var encoder = new UTF8Encoding();
            byte[] bytesToVerify = encoder.GetBytes(originalMessage);
            byte[] signedBytes = Convert.FromBase64String(signedMessage);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

            try
            {
                SHA1Managed Hash = new SHA1Managed();
                byte[] hashedData = Hash.ComputeHash(signedBytes);
                success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA1"), signedBytes);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
            return success;
        }

        public static string GetSHA1HashData(string data)
        {
            //create new instance of SHA1
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }

        public static bool ValidateSHA1HashData(string inputData, string storedHashData)
        {
            //hash input text and save it string variable
            string getHashInputData = GetSHA1HashData(inputData);

            if (string.Compare(getHashInputData, storedHashData) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string addDigitalSignature(string encMsg)
        {
            X509Certificate2 x509_2 = new X509Certificate2("E:/IssuerBank.pfx", "pidel123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            string result = "";
            result = SignData(encMsg, x509_2);
            return result;
        }
    }
}
