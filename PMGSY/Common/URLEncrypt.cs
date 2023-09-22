using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
//using  PMGSY.ExtensionMethods;

namespace  PMGSY.Common
{
    public class URLEncrypt 
    {
        public static string EncryptParameters(System.String[] strParametersToEncrypt)
        {
            // Holds the string to calculate digest which will be sent in url along with the parameteres.
            System.String parametersToEncrypt = string.Empty;
            // Random public key for the encryption which will be sent in url for decryption.
            System.String randomKeyForEncryption = Cryptography.createChallenge();
            // Concanated paramters encrypted.
            System.String encryptedParameterString = String.Empty;
            // SHA1 Hash
            System.String digest = String.Empty;
            for (int i = 0; i < strParametersToEncrypt.Length; i++)
            {
               // if (strParametersToEncrypt[i].Contains("/"))
                 //   throw new Exception("'/' Character is Not Allowed");
                if (i == 0)
                    parametersToEncrypt = strParametersToEncrypt[i];
                else
                    parametersToEncrypt = parametersToEncrypt + "#" + strParametersToEncrypt[i];
            }
            // parametersToEncrypt = "amol#1234#an kit#my&name#file+file#f&+ t";
            // Encrypt parameter string.
            randomKeyForEncryption = randomKeyForEncryption.Replace('/', '$');
            encryptedParameterString = Cryptography.Encrypt(parametersToEncrypt, randomKeyForEncryption);
            encryptedParameterString = encryptedParameterString.Replace('/', '$');
            // String temp = System.Web.HttpUtility.UrlPathEncode(encryptedParameterString);
            // String decryptedParameterString = NLMA.Common.Cryptography.Decrypt(encryptedParameterString, randomKeyForEncryption);
            // Calculate digest for stringToEncrypt + randomKeyForEncryption
            System.Byte[] byteDigest = Cryptography.GenerateHash(parametersToEncrypt + randomKeyForEncryption);
            digest = Cryptography.BytesToHexString(byteDigest);
            //digest = "";
            return System.Web.HttpUtility.UrlPathEncode(encryptedParameterString) + "/" + System.Web.HttpUtility.UrlPathEncode(randomKeyForEncryption) + "/" + System.Web.HttpUtility.UrlPathEncode(digest);
        }


        /// <summary>
        /// Encrypt parameters
        /// </summary>
        /// <param name="strParametersToEncrypt">Parameters to be encrypted</param>
        /// <returns></returns>
        /// 
        public static string EncryptParameters1(System.String[] strParametersToEncrypt)
        {
            // Holds the string to calculate digest which will be sent in url along with the parameteres.
            System.String parametersToEncrypt = string.Empty;
            // Random public key for the encryption which will be sent in url for decryption.
            System.String randomKeyForEncryption = GenerateSalt(16);
            // Concanated paramters encrypted.
            System.String encryptedParameterString = String.Empty;
            System.String digest = String.Empty;
            //"/" Characte is not allowed for encryption
            //Concat the parametrs with # value
            for (int i = 0; i < strParametersToEncrypt.Length; i++)
            {
                if (strParametersToEncrypt[i].Contains("/"))
                    throw new Exception("'/' Character is Not Allowed");
                if (i == 0)
                    parametersToEncrypt = strParametersToEncrypt[i];
                else
                    parametersToEncrypt = parametersToEncrypt + "#" + strParametersToEncrypt[i];
            }
            randomKeyForEncryption = randomKeyForEncryption.Replace('/', '$');
            // Encrypt parameter string.
            encryptedParameterString = Cryptography.Encrypt(parametersToEncrypt, randomKeyForEncryption);
            encryptedParameterString = encryptedParameterString.Replace('/', '$');
            // String temp = System.Web.HttpUtility.UrlPathEncode(encryptedParameterString);
            // Calculate digest for stringToEncrypt + randomKeyForEncryption
            System.Byte[] byteDigest = Cryptography.GenerateHash(parametersToEncrypt + randomKeyForEncryption);
            digest = BitConverter.ToString(byteDigest).Replace("-", "");
            //digest = "";
            //Returen Encrypted Encrypted parametrs,encrypted key and digest
            return System.Web.HttpUtility.UrlPathEncode(encryptedParameterString) + "/" + System.Web.HttpUtility.UrlPathEncode(randomKeyForEncryption) + "/" + System.Web.HttpUtility.UrlPathEncode(digest);
        }


        public static string[] DecryptParameters(string[] strParametersToDecrypt)
        {
            try
            {
                //Holds the string of Encrypted parameters separated by #
                System.String strEncryptedParameters = strParametersToDecrypt[0];
                //Public Key for the decryption
                System.String randomKeyForDecryption = strParametersToDecrypt[1];
                //Digest to check whether URL was tampered
                System.String strDigestFromQueryString = strParametersToDecrypt[2];
                strEncryptedParameters = strEncryptedParameters.Replace('$', '/');
                randomKeyForDecryption = randomKeyForDecryption.Replace('$', '/');
                //Parameters separated
                System.String[] strParameters = strEncryptedParameters.Split('#');
                System.String strComputedDigest = string.Empty;
                for (int i = 0; i < strParameters.Length; ++i)
                {
                    //Parameters Decrypted
                    strParameters[0] = Cryptography.Decrypt(strParameters[0], randomKeyForDecryption);
                    strComputedDigest += strParameters[0];
                }
                strComputedDigest += randomKeyForDecryption;
                //Digest again Computed
                System.Byte[] byteDigest = Cryptography.GenerateHash(strComputedDigest);
                strComputedDigest = Cryptography.BytesToHexString(byteDigest);
                //Check both Digest one Computed and other from URL
                if (!strComputedDigest.Equals(strDigestFromQueryString))
                {
                    throw new Exception("Url is tampered");
                   // throw new NLMA.CustomException.URLTamperedException("URL is Tampered");
                    //URL Tampered
                }
                return strParameters;
            }
            //catch (NLMA.CustomException.URLTamperedException objURLTampered)
            //{
            //    throw objURLTampered;
            //}
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Decrypt the parameters 
        /// </summary>
        /// <param name="strParametersToDecrypt">String to be decrypt</param>
        /// <returns>Parameters and value</returns>
        public static Dictionary<string, string> DecryptParameters1(string[] strParametersToDecrypt)
        {
            try
            {
                System.String strEncryptedParameters = null;
                System.String randomKeyForDecryption = null;
                System.String strDigestFromQueryString = null;
                if (strParametersToDecrypt.Length == 3)
                {
                    //string[] 
                    //Holds the string of Encrypted parameters separated by #
                    if (!(strParametersToDecrypt[0] == null || strParametersToDecrypt[1] == null || strParametersToDecrypt[2] == null))
                    {
                        strEncryptedParameters = strParametersToDecrypt[0];
                        //Public Key for the decryption
                        randomKeyForDecryption = strParametersToDecrypt[1];
                        //Digest to check whether URL was tampered
                        strDigestFromQueryString = strParametersToDecrypt[2];
                    }
                    else
                    {
                        throw new Exception("Url is tampered");
                    }
                }
                strEncryptedParameters = strEncryptedParameters.Replace('$', '/');
                randomKeyForDecryption = randomKeyForDecryption.Replace('$', '/');
                strEncryptedParameters = strEncryptedParameters.Replace(' ', '+');
                randomKeyForDecryption = randomKeyForDecryption.Replace(' ', '+');
                strEncryptedParameters = strEncryptedParameters.Replace("%20", "+");
                randomKeyForDecryption = randomKeyForDecryption.Replace("%20", "+");

                //Parameters separated
                System.String[] strParameters = strEncryptedParameters.Split('#');
                System.String strComputedDigest = string.Empty;
                for (int i = 0; i < strParameters.Length; ++i)
                {
                    //Parameters Decrypted
                    strParameters[0] = Cryptography.Decrypt(strParameters[0], randomKeyForDecryption);
                    strComputedDigest += strParameters[0];
                }
                strComputedDigest += randomKeyForDecryption;
                //Digest again Computed
                System.Byte[] byteDigest = Cryptography.GenerateHash(strComputedDigest);
                strComputedDigest = BitConverter.ToString(byteDigest).Replace("-", "");
                //Check both Digest one Computed and other from URL
                if (!strComputedDigest.Equals(strDigestFromQueryString))
                {
                    throw new Exception("Url is tampered");
                    //URL Tampered
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string[] str = (strParameters[0].ToString().Split('#'));
                for (int i = 0; i < str.Length; ++i)
                {
                    string[] splitParameter = str[i].Split('=');
                    parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                }
                return parameters;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Random number generation required for encryption
        /// </summary>
        /// <param name="length">Length of the key</param>
        /// <returns>Random Number</returns>
        private static string GenerateSalt(int length)
        {
            byte[] _randomArray = new byte[length];
            string _salt;
            //Create random salt and convert to string
            RNGCryptoServiceProvider _randomNumberGenerator = new RNGCryptoServiceProvider();
            _randomNumberGenerator.GetBytes(_randomArray);
            _salt = Convert.ToBase64String(_randomArray);
            //$ and / characters are used in URL encryption, so required to be removed from the key
            _salt = _salt.Replace('$', 'a');
            _salt = _salt.Replace('/', 'b');
            return _salt;
        }
    }
}
