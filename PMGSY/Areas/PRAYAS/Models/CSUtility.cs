// Please import the following namespaces.
// ================================================
using System.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Data.Entity;
using System.Web.Script.Serialization;
using CSModels;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

public class CSUtility
{
    // Please import the following Classes of VB Code.
    // ================================================
    private CSIntegration objintgrate = new CSIntegration();

    // Set the values of Key File Path along with File Name.
    //private string keyPath = "~/mdck/SymKey_2.key";

    private string keyPath = "~/mdck/Key_1_10046_20201008.key";

    

    // Method to enforce Security.
    public string GetCheckSum(string str1)
    {
        return objintgrate.GetMD5Hasher(str1);
    }
    // Method to Compress String Input.
    public string Compress(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        MemoryStream ms = new MemoryStream();

        using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            zip.Write(buffer, 0, buffer.Length);
        }

        ms.Position = 0;
        // Dim outStream As MemoryStream = New MemoryStream()
        byte[] compressed = new byte[ms.Length - 1 + 1];
        ms.Read(compressed, 0, compressed.Length);
        byte[] gzBuffer = new byte[compressed.Length + 4 - 1 + 1];
        System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
        System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
        return Convert.ToBase64String(gzBuffer);
    }
    // Method to Decompress Compressed String Input.
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
    // Function to Read Key File saved in Website Folder and Encrypt your Data with this Key.
    public string Encrypt(string value)
    {
        string fl1 = HttpContext.Current.Server.MapPath(keyPath);

        return objintgrate.Encrypt(value, fl1);
    }

    // Function to Encrypt your Data with passed Key Bytes.
    private string EncryptB(string value, byte[] key)
    {
        return objintgrate.EncryptB(value, key);
    }



  

    //public List<Masterdata> ConvertToList(DataSet ds)
    //{


    //    var datalist = ds.Tables[0].DataTableToList<KPIData>();

    //    //var datalist = ds.Tables[0].Rows.Cast<System.Data.DataRow>().ToList();
    //    //List<KPIData> resultlist = new List<KPIData>();
    //    //foreach (var item in datalist)
    //    //{
    //    //    KPIData result = new KPIData();
    //    //    result.Instance_Code = item.Field<int>("Instance_Code");
    //    //    result.Sec_Code = item.Field<int>("Sec_Code");
    //    //    result.Ministry_Code = item.Field<int>("Ministry_Code");
    //    //    result.Dept_Code = item.Field<int>("Dept_Code");
    //    //    result.Project_Code = item.Field<long>("Project_Code");
    //    //    result.Frequency_Id = item.Field<int>("Frequency_Id");
    //    //    result.atmpt = item.Field<int>("atmpt");
    //    //    resultlist.Add(result);
    //    //}



       
    //    var PojectKPIList = new List<Masterdata>();
    //    var DistinctData = datalist.Select(m => new {m.Instance_Code, m.Sec_Code, m.Ministry_Code, m.Dept_Code, m.Project_Code, m.Frequency_Id, m.atmpt }).Distinct().ToList();

    //  //  var DistinctData=resultlist.Distinct();
        
    //    foreach (var item in DistinctData)
    //    {

    //        var KPIDetailsInternal = new List<KPIDetails>();
    //        foreach (var item1 in datalist) //   resultlist
    //        {
    //            KPIDetailsInternal.Add(new KPIDetails
    //            {
    //                Group_Id = item1.Group_Id,
    //                datadate = item1.datadate,
    //                KValue = item1.KValue,
    //                LValue = item1.LValue

    //            });
    //        }
    //        PojectKPIList.Add(new Masterdata { Instance_Code = item.Instance_Code, Sec_Code = item.Sec_Code, Ministry_Code = item.Ministry_Code, Dept_Code = item.Dept_Code, Project_Code = item.Project_Code, Frequency_Id = item.Frequency_Id, ListKpidata = KPIDetailsInternal });
    //    }
    //    return PojectKPIList;
    //}


    public List<Masterdata> ConvertToList(DataSet ds)
    {
        var datalist = ds.Tables[0].DataTableToList<KPIData>();

        //var datalist = ds.Tables[0].Rows.Cast<System.Data.DataRow>().ToList();
        //List<KPIData> resultlist = new List<KPIData>();
        //foreach (var item in datalist)
        //{
        //    KPIData result = new KPIData();
        //    result.Instance_Code = item.Field<int>("Instance_Code");
        //    result.Sec_Code = item.Field<int>("Sec_Code");
        //    result.Ministry_Code = item.Field<int>("Ministry_Code");
        //    result.Dept_Code = item.Field<int>("Dept_Code");
        //    result.Project_Code = item.Field<long>("Project_Code");
        //    result.Frequency_Id = item.Field<int>("Frequency_Id");
        //    result.atmpt = item.Field<int>("atmpt");
        //    resultlist.Add(result);
        //}


        var PojectKPIList = new List<Masterdata>();
        var DistinctData = datalist.Select(m => new { m.Instance_Code, m.Sec_Code, m.Ministry_Code, m.Dept_Code, m.Project_Code, m.Frequency_Id, m.atmpt }).Distinct().ToList();
       // var DistinctData=resultlist.Select(m => new { m.Instance_Code, m.Sec_Code, m.Ministry_Code, m.Dept_Code, m.Project_Code, m.Frequency_Id, m.atmpt }).Distinct().ToList();
        foreach (var item in DistinctData)
        {

            var KPIDetailsInternal = new List<KPIDetails>();
            foreach (var item1 in datalist)
            {
                KPIDetailsInternal.Add(new KPIDetails
                {
                    Group_Id = item1.Group_Id,
                    datadate = item1.datadate,
                    KValue = item1.KValue,
                    LValue = item1.LValue

                });
            }
            PojectKPIList.Add(new Masterdata { Instance_Code = item.Instance_Code, Sec_Code = item.Sec_Code, Ministry_Code = item.Ministry_Code, Dept_Code = item.Dept_Code, Project_Code = item.Project_Code, Frequency_Id = item.Frequency_Id, ListKpidata = KPIDetailsInternal });
        }
        return PojectKPIList;
    }




}
public static class Helper
{
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }

}
