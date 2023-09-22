//Please import the following namespaces.
//================================================
//Please import the following namespaces.
//================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;


//================================================
/// <summary>
/// Summary description for VBComClass
/// </summary>
public class CSComClass
{


    // Please declare required variable of SQL Class.
    // ================================================
    SqlConnection conn;
    SqlCommand mCom;
    SqlDataAdapter mDa;
    // ================================================


    // Method to Open SQL DB Connection.
    // "connectionstring_UP" is a Connection String defined in "Web Config" File.
    private void OpenConnection()
    {
        string cs1 = "PMGSYConnection";

        conn = new SqlConnection(ConfigurationManager.ConnectionStrings[cs1].ToString());

        if (conn.State == ConnectionState.Closed)
            conn.Open();
        mCom = new SqlCommand();
        mCom.Connection = conn;
    }

    
        // Method to Close SQL DB Connection.
        private void CloseConnection()
        {
            if (conn == null == false)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // Method to Dispose SQL DB Connection.
        private void DisposeConnection()
        {
            if (conn == null == false)
                conn.Dispose();
        }

        // Method to Execute Stored Procedure on SQL DB.
        public DataSet getDatasetParametersp(string strSql, string strVTabName, Hashtable Param)
        {
            OpenConnection();
            mCom.CommandText = strSql;
            mCom.CommandType = CommandType.StoredProcedure;

            mDa = new SqlDataAdapter(mCom);
            foreach (DictionaryEntry de in Param)
                mCom.Parameters.AddWithValue(de.Key.ToString(), de.Value);
            DataSet ds = new DataSet();
            mDa.SelectCommand.CommandTimeout = 0;
            if (strVTabName == "")
                mDa.Fill(ds);
            else
                mDa.Fill(ds, strVTabName);
            mCom.Parameters.Clear();
            CloseConnection();
            DisposeConnection();
            return ds;
        }

        // Method to Convert JSON String to Dataset.
        public DataSet ConvertJsonToDatatable(string jsonString)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Dash");
            ds.Tables.Add(dt);
            string s = "{" + "\"" + "RetDMDashCaption" + "\"" + ":";
            jsonString = jsonString.Replace(s, "").Trim('}');
            string[] jsonParts = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> dtColumns = new List<string>();

            foreach (string jp in jsonParts)
            {
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");

                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1);
                        string v = rowData.Substring(idx + 1);

                        if (!dtColumns.Contains(n))
                            dtColumns.Add(n.Replace("\"", ""));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", rowData));
                    }
                }

                break;
            }

            foreach (string c in dtColumns)
                dt.Columns.Add(c);

            foreach (string jp in jsonParts)
            {
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();

                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string v = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[n] = v;
                }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                dt.Rows.Add(nr);
            }

            return ds;
        }
    
}