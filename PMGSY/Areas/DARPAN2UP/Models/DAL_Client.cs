using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DARPAN2UP;

/// <summary>
/// Summary description for DAL_Client
/// </summary>
public class DAL_Client
{
    SqlConnection conn = new SqlConnection();
    SqlCommand cmd = new SqlCommand();
    public DAL_Client()
    {

        conn.ConnectionString = ConfigurationManager.ConnectionStrings["PMGSYConnection"].ConnectionString;
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
    }

    public DataTable Dal_GetDataForPorting(int instance_Code, int project_Code, int clientGroupId, string client_DataDate, string Proc_name)
    {

            DataTable dtData = new DataTable();
            cmd.CommandText = Proc_name;
            cmd.Parameters.AddWithValue("@instance_Code", instance_Code);
            cmd.Parameters.AddWithValue("@project_Code", project_Code);
            cmd.Parameters.AddWithValue("@clientGroupId", clientGroupId);
            cmd.Parameters.AddWithValue("@client_DataDate", client_DataDate);
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                sda.Fill(dtData);
            return dtData;
    }

  
    internal void Dal_MsgLog(DateTime now, int status,string msg, int group_id=0,string Datadate="")
    {
       // cmd.CommandText = "Maintain_Log";
        cmd.CommandText = "omms.UP-DARPAN2-Maintain_Log";  // change by saurabh
        cmd.Parameters.AddWithValue("@instance_Code", GlobalItem.instance_Code);
        cmd.Parameters.AddWithValue("@project_Code", GlobalItem.project_Code);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@group_id", group_id);
        cmd.Parameters.AddWithValue("@Req_Start_dt", now);        
        cmd.Parameters.AddWithValue("@msg", msg);
        cmd.Parameters.AddWithValue("@Datadate", Datadate);
        conn.Open();
        int rowsAffected = cmd.ExecuteNonQuery();
        conn.Close();

    }
}