using System;
using System.Data.Entity;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web.Mvc;

namespace  PMGSY.Common
{
    public class Cryptography
    {
        #region Fields

        private static byte[] key = { };
        private static byte[] IV = { 38, 55, 206, 48, 28, 64, 20, 16 };
        // private static string stringKey = "!5663a#KN";

        #endregion

        #region Public Methods
        public static string Encrypt(string text, string stringKey)
        {
            try
            {
                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
            
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
              
                Byte[] byteArray = Encoding.UTF8.GetBytes(text);

                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    des.CreateEncryptor(key, IV), CryptoStreamMode.Write);


                cryptoStream.Write(byteArray, 0, byteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());


                
            }
            catch (Exception ex)
            {
                // Handle Exception Here
            }

            return string.Empty;
        }

        public static string AESEncrypt(string text)
        {
            string encStr = string.Empty;
            try
            {

                //text = "Hello,Bye";
                //////////  string strKey =  "Cdac*()$Egov321";

              ////////  string strKey = "4D455858";

              ////////  byte[] aesKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
              //////// // ey = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
              ////////  byte[] aeskVector = Encoding.UTF8.GetBytes("Cdac*()$Egov321");

              ////////  AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

              ////////  Byte[] byteArray = Encoding.UTF8.GetBytes(text);

              ////////  MemoryStream memoryStream = new MemoryStream();
              ////////  CryptoStream cryptoStream = new CryptoStream(memoryStream,
              ////////      aes.CreateEncryptor(aesKey, IV), CryptoStreamMode.Write);


              ////////  cryptoStream.Write(byteArray, 0, byteArray.Length);
              ////////  cryptoStream.FlushFinalBlock();

              ////////  return Convert.ToBase64String(memoryStream.ToArray());

              //  string strKey = "4D455858";

                //var bytes = Encoding.UTF8.GetBytes("00000Cdac*()$Egov3210000");
                //var base64 = Convert.ToBase64String(bytes);


                //AesManaged myAes = new AesManaged();
                Aes myAes = Aes.Create();
                
                myAes.Padding = PaddingMode.PKCS7;
                myAes.Mode = CipherMode.ECB;
               
                
              //  myAes.Mode = CipherMode.ECB;
                myAes.KeySize = 128;
                myAes.BlockSize = 128;

               
                myAes.IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // CRB mode uses an empty IV
                myAes.Key = new byte[16] { 0x74, 0x68, 0x69, 0x73, 0x49, 0x73, 0x41, 0x53, 0x65, 0x63, 0x72, 0x65, 0x74, 0x4b, 0x65, 0x79 };
                
                // myAes.Key = UTF8Encoding.UTF8.GetBytes(strKey);
                //myAes.Key = System.Text.ASCIIEncoding.Default.GetBytes(strKey);
                


                // Create a encryption object to perform the stream transform.
                ICryptoTransform encryptor = myAes.CreateEncryptor();
                //MemoryStream memoryStream = new MemoryStream();

                //CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                
                //cryptoStream.Write(byteArray, 0, byteArray.Length);
                //cryptoStream.FlushFinalBlock();

                //return Convert.ToBase64String(memoryStream.ToArray());

                byte[] encrypted;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(text);
                            //if (text.Length < 16)
                            //{
                            //    for (int i = text.Length; i < 16; i++)
                            //    {
                            //        swEncrypt.Write((byte)0x0);
                            //    }
                            //}
                        }
                        encrypted = msEncrypt.ToArray();
                        encStr = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle Exception Here
            }

            return encStr;
        }




        public static string AESDecrypt(string text)
        {
            try
            {
          
                byte[] base64 = new byte[16] { 0x74, 0x68, 0x69, 0x73, 0x49, 0x73, 0x41, 0x53, 0x65, 0x63, 0x72, 0x65, 0x74, 0x4b, 0x65, 0x79 };
                byte[] aeskVector = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } ;
               
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.ECB;

                Byte[] byteArray = Convert.FromBase64String(text);
               // Byte[] byteArray = Convert.FromBase64String(text.Replace("", "+"));

                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    aes.CreateDecryptor(base64, aeskVector), CryptoStreamMode.Write);

                cryptoStream.Write(byteArray, 0, byteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

           
        }

        public static string Decrypt(string text, string stringKey)
        {
            try
            {
                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] byteArray = Convert.FromBase64String(text);

                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    des.CreateDecryptor(key, IV), CryptoStreamMode.Write);

                cryptoStream.Write(byteArray, 0, byteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                // Handle Exception Here
            }

            return string.Empty;
        }

        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return result;
        }



        public static string createChallenge()
        {
            System.Random rng = new Random(DateTime.Now.Millisecond);
            // Create random string
            byte[] salt = new byte[64];
            for (int i = 0; i < 64; )
            {
                salt[i++] = (byte)rng.Next(65, 90); // a-z
             //  salt[i++] = (byte)rng.Next(97, 122); // A-Z
            }
            string challenge = string.Empty;
            challenge = BytesToHexString(salt);
            return challenge.Substring(0,8);
        }

        public static string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);
            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }

        public static Byte[] GenerateHash(string inStr)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            return md5Hasher.ComputeHash(encoder.GetBytes(inStr.ToString().Trim()));
        }
        #endregion
    }
}
