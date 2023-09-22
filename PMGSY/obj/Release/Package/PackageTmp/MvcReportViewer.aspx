<%@ Page Language="C#" AutoEventWireup="true" Inherits="MvcReportViewer.MvcReportViewer, MvcReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div style="width:auto;">
        <form id="reportForm" runat="server" style="width: 100%; height: 550px;">
            <asp:ScriptManager AsyncPostBackTimeout="0" ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer" runat="server" Width="100%" Height="550px" AsyncRendering="true" ShowRefreshButton="false" ShowBackButton="true" 
                ShowPageNavigationControls="true" WaitMessageFont-Size="14px" ShowPrintButton="true" ></rsweb:ReportViewer>
        </form>
    </div>
</body>
</html>
