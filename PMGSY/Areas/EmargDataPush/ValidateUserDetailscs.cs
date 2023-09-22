using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace PMGSY.Areas.EmargDataPush
{
    public class ValidateUserDetailscs : System.Web.Services.Protocols.SoapHeader
    {
        public string userName { get; set; }
        public string password { get; set; }


        //public bool IsValid()
        //{

        //    return this.userName == "omms"; //&& this.password == "Ommas@Nrida123";
        //}



        public static string ComputeSHA256Hash(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] textBytes = Encoding.ASCII.GetBytes(input);

            using (SHA256 sha1 = SHA256.Create())
            {
                byte[] computeHash = sha1.ComputeHash(textBytes);
                for (int i = 0; i < computeHash.Length; i++)
                {
                    stringBuilder.Append(computeHash[i].ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }


        public static bool validateSHA256Hash(string password, string actualPassword)
        {
            string temphashPassword = ComputeSHA256Hash(actualPassword);
            bool flag;
            if (string.Compare(temphashPassword, password) == 0)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }







    }
}