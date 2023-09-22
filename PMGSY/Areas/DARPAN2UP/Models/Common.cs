using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace DARPAN2UP
{

    /// <summary>
    /// Summary description for Common
    /// </summary>
    /// 

    public class GlobalItem
    {
        //Key Inforamtion
        public static string APIKEY = "2oi/ELI3+NiSZambOVQ6ALiWupqUHs8FocSDP7PEXbI=";  // API KEY PROVIDED IN KEY RECIPIENT EMAIL
        public static string APIHMAC_KEY = "yxA65It1WptUka5jKOplBsrGTLuUv3EoyYg0ul9rnYM="; // HMACAPI KEY PROVIDED IN KEY RECIPIENT EMAIL

        //public static string APIKEY = "v13AotfLMJG+UgVrKckvMmhlCiG0TXU0edi6aXSsePI=";    // for testing purpose
        //public static string APIHMAC_KEY = "04nq+n9w4/t7rb2Ww9x+x2WzFu4b/9xLTm42V5oSUQ0=";


        //Project Paramter
        public static int project_Code = 2340; // PROJECT CODE PROVIDED IN KEY RECIPIENT EMAIL
        public static int instance_Code = 2;// INSATNCE CODE PROVIDED IN KEY RECIPIENT EMAIL
                                            //Database Procedures
        public static string SQLDBLogStoredProc = "omms.UP-DARPAN2-Maintain_Log";
        public static string SQLDBDataStoredProc = "omms.UP-DARPAN2_Retrieve_Data";
        //API Endpoints
        public static string GetDateRangeAPIUrl = "https://stateapi.darpan.nic.in/getdate";
        public static string PushDataAPIUrl = "https://stateapi.darpan.nic.in/pushdata";
    }
    public class DarpanCommon
    {
        #region Encrypt,Decrypt,Hashing and VI Methods-----
        public object CreateInputWithHash(object response)
        {
            string tjson = JsonConvert.SerializeObject(response);//tesing
            string jsonData = EncryptData(JsonConvert.SerializeObject(response), GlobalItem.APIKEY);
            IDictionary<string, object> numberNames = new Dictionary<string, object>();
            numberNames.Add("data", jsonData);
            numberNames.Add("hash", ComputeSha512Hash(jsonData + GlobalItem.APIKEY));
            numberNames.Add("iv", HttpContext.Current.Session["MIS_ClientIV"].ToString());
            numberNames.Add("project_code", GlobalItem.project_Code);
            numberNames.Add("instance_code", GlobalItem.instance_Code);
            return numberNames;
        }
        public string ComputeSha512Hash(string rawData)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i <= bytes.Length - 1; i++)
                    builder.Append(bytes[i].ToString("x2"));

                return builder.ToString();
            }
        }
        public string EncryptData(string iplaintText, string AppKey)
        {
            GenerateIV();  // THIS FUNCTION WILL GENERATE RANDOM IV
                           // HttpContext.Current.Session["MIS_ClientIV"] = "ZY4hPbe125vdVIr5QPDpQw==";
            AesCryptoServiceProvider aesEncryption = new AesCryptoServiceProvider();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = Convert.FromBase64String(HttpContext.Current.Session["MIS_ClientIV"].ToString());
            aesEncryption.Key = Convert.FromBase64String(AppKey);
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(iplaintText);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText);
        }
        public string DecryptData(string iEncryptedText, string AppKey)
        {
            AesCryptoServiceProvider aesEncryption = new AesCryptoServiceProvider();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = Convert.FromBase64String(HttpContext.Current.Session["MIS_ClientIV"].ToString());
            aesEncryption.Key = Convert.FromBase64String(AppKey);
            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64CharArray(iEncryptedText.ToCharArray(), 0, iEncryptedText.Length);
            return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }
        public string GenerateIV()
        {
            AesCryptoServiceProvider aesEncryption = new AesCryptoServiceProvider();
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            //aesEncryption.GenerateIV();
            string ivStr = Convert.ToBase64String(aesEncryption.IV);
            HttpContext.Current.Session["MIS_ClientIV"] = ivStr;  // ZY4hPbe125vdVIr5QPDpQw==
           // HttpContext.Current.Session["MIS_ClientIV"] = "ZY4hPbe125vdVIr5QPDpQw==" ;     // For Testing Purpose
            return ivStr;

        }
        public string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            byte[] compressed = new byte[ms.Length - 1 + 1];
            ms.Read(compressed, 0, compressed.Length);
            byte[] gzBuffer = new byte[compressed.Length + 4 - 1 + 1];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }
        public string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
                byte[] buffer = new byte[msgLength - 1 + 1];
                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }

        #endregion

        public bool CheckValidateDateFormat(string dateRange_Data)
        {
            //Check and validate valid date format    
            if (dateRange_Data == "0")
                return false;
            DateTime parsed;
            return DateTime.TryParseExact(dateRange_Data, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }

        public string ValidateOutPutData(HttpResponseMessage response)
        {
            string ResOutPut_Client = null, OutPutHash_Client = null;

            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            ResOutPut_Client = values["data"].ToString();
            OutPutHash_Client = values["hash"].ToString();
            HttpContext.Current.Session["MIS_ClientIV"] = values["iv"].ToString();
            if (!string.IsNullOrEmpty(ResOutPut_Client) && !string.IsNullOrEmpty(OutPutHash_Client))
            {
                if (!String.Equals(OutPutHash_Client, this.ComputeSha512Hash(ResOutPut_Client + GlobalItem.APIKEY)))
                    ResOutPut_Client = null; // Hash Mismatched  
            }
            else
                ResOutPut_Client = null; // Respose Not found        
            return ResOutPut_Client;
        }

        public List<Masterdata> ConvertToListnew(DataTable dt)
        {
            var datalist = DataTableToListnew(dt);
            var PojectKPIList = new List<Masterdata>();
            var DistinctData = datalist.Select(m => new { m.instance_code, m.project_code, m.frequency_id, m.seq_no, m.datadate, m.group_id, m.totalrecordcount, m.optional1, m.optional2, }).Distinct().ToList();


            foreach (var item in DistinctData)
            {
                DateTime dateAndTime = DateTime.Parse(item.datadate);
                DateTime dateOnly = dateAndTime.Date;

                var KPIDetailsInternal = new List<KPIDetails>();
                foreach (var item1 in datalist)
                {
                    KPIDetailsInternal.Add(new KPIDetails
                    {
                        kvalue = item1.kvalue,
                        lvalue = item1.lvalue
                    });
                }
                PojectKPIList.Add(new Masterdata
                {
                    instance_code = item.instance_code,
                    project_code = item.project_code,
                    frequency_id = item.frequency_id,
                    seq_no = item.seq_no,
                    totalrecordcount = dt.Rows.Count,
                    group_id = item.group_id,
                    optional1 = item.optional1,
                    optional2 = item.optional2,
                    datadate = dateOnly.ToString("MM/dd/yyyy"),
                  //  datadate = "12/31/2022",
                    listkpidata = KPIDetailsInternal
                });
            }

            return PojectKPIList;

        }

        public static List<KPIData> DataTableToListnew(DataTable dt)
        {
            List<KPIData> DataList = new List<KPIData>();
            DataList = (from DataRow dr in dt.Rows
                        select new KPIData()
                        {
                            instance_code = Convert.ToInt32(dr["instance_code"]),
                            project_code = Convert.ToInt64(dr["project_code"]),
                            frequency_id = Convert.ToInt32(dr["frequency_id"]),
                            seq_no = Convert.ToInt32(dr["seq_no"]),
                            kvalue = dr["kvalue"].ToString(),
                            lvalue = dr["lvalue"].ToString(),
                            group_id = Convert.ToInt32(dr["group_id"]),
                            datadate = dr["datadate"].ToString()
                        }).ToList();
            return DataList;

        }


    }
    #region  Classes-----------------------------
    public class Bopayload
    {
        public string output { get; set; }
        public string outputHasg { get; set; }
        public string iv { get; set; }
        public int instance_code { get; set; }
        public string project_code { get; set; }
    }
    public class BoInput
    {
        public int instance_code { get; set; }
        public long project_code { get; set; }

    }
    public class Masterdata
    {
        public int instance_code { get; set; }
        public long project_code { get; set; }
        public int frequency_id { get; set; }
        public int group_id { get; set; }
        public string datadate { get; set; }
        public int seq_no { get; set; }
        public IList<KPIDetails> listkpidata { get; set; }
        public int totalrecordcount { get; set; }
        public string optional1 { get; set; }
        public string optional2 { get; set; }


    }
    public class KPIDetails
    {

        public string kvalue { get; set; }
        public string lvalue { get; set; }


    }
    public class KPIData
    {
        public int instance_code { get; set; }
        public long project_code { get; set; }
        public int frequency_id { get; set; }
        public int seq_no { get; set; }
        public string kvalue { get; set; }
        public string lvalue { get; set; }
        public int group_id { get; set; }
        public string datadate { get; set; }
        public int totalrecordcount { get; set; }
        public string optional1 { get; set; }
        public string optional2 { get; set; }

    }
    public class ProjectKpiDetailsCompressData
    {
        public BoInput projpara { get; set; }
        public string compresseddata { get; set; }
    }
    public class ProjectKpiDetails
    {
        public BoInput projpara { get; set; }
        public string rawdata { get; set; }
    }

    #endregion


}

