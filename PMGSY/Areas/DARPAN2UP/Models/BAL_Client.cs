using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for BAL_Client
/// </summary>
public class BAL_Client
{
    public BAL_Client()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DataTable Bal_GetDataForPorting(int instance_Code, int project_Code, int clientGroupId, string client_DataDate, string Proc_name)
    {
        DAL_Client obj = new DAL_Client();
        return obj.Dal_GetDataForPorting(instance_Code, project_Code, clientGroupId, client_DataDate, Proc_name);
    }

    public void Bal_MsgLog( DateTime now, int status,string msg, int group_id = 0, string Datadate = "")
    {
        DAL_Client obj = new DAL_Client();
        obj.Dal_MsgLog( now, status, msg, group_id, Datadate);
    }
}