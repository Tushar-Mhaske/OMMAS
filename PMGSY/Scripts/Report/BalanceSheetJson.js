$.ajaxSetup({

    // Disable caching of AJAX responses 
    cache: false
});

var GridData;

$(document).ready(function () {
   // alert("Ready");
   $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmMaintenanceBalanceSheet'));

    $('#btnViewDetails').click(function () {

         if ($("#tblList") != undefined) {
            $("#tblList").GridUnload();
        }
        ViewAbstractGrading();

    });

    $('#btnViewDetails').trigger('click'); 

    $('#btnPDFExport').click(function () {
        //GridData
        var rowdata = $("#tblList").jqGrid('getRowData');
        //alert(rowdata);
        var rowdataString = JSON.stringify(rowdata);
       // alert(rowdataString);
        //console.log(rowdata);
      //  $("#list").setRowData(1,{first_name:"HUHAHUA"});
       // $.post("demo_ajax_gethint.asp",{suggest:txt},function(result){
        //alert(GridData.header.ReportFormNumber);
        //  $.post('/Reports/pdfExport',{data:"jsond"}, function(data) {
        $.post('/Reports/pdfExport', { data: rowdataString }, function (dataResponse) {
        
            alert(dataResponse);
        });

    });
    $('#btnExcelFExport').click(function (e) {
        //GridData
       // var rowdata = $("#tblList").jqGrid('getRowData');
        //alert(rowdata);
       // var rowdataString = JSON.stringify(rowdata);
        
       // $.getJson('/Reports/ExportToExcel');
        //  exportToExcel();
        //window.open("/Reports/ExportToExcel");
       // click(function(e) {
       // window.open('data:application/vnd.ms-excel,' + "<data>");
        // e.preventDefault();
       // $.post('/Reports/ExportToExcel');
        window.open('/WebForm1.aspx');
        //$.getJson('/WebForm1.aspx', { data: rowdataString }, function (dataResponse) {

           // alert(dataResponse);
        //});
    });

});

function exportToExcel()
{
    /*alert('Hi');

    contentType = "application/vnd.ms-excel";
    var oExcel = new ActiveXObject("Excel.Application");
    var oBook = oExcel.Workbooks.Add;
    var oSheet = oBook.Worksheets(1);


    //Define criteria - start<br>
    oSheet.Cells(row, col)="Business Function";
    oSheet.Cells(row, col+1)="VESS";
    oSheet.Cells(row, col+2)=selectedVESSBusinessFunction;

    // oSheet.Cells(row, col+3)="Manufacturing";
    // if(selectedMFGBusinessFunction != "-Select-")
    // oSheet.Cells(row, col+4)=selectedMFGBusinessFunction;

    oSheet.columns.autofit;

    oExcel.Visible = true;
    oExcel.UserControl = true;*/
    window.open('data:application/vnd.ms-excel,'+"<table><tr>td>this data</td></tr></table>");
}

var jsondataforgrid =  [{ "GROUP_ID": "L", "ITEM_ID": -1, "ITEM_HEADING": "Maintenance Fund Liabilities", "CURRENT_AMT": null, "PREVIOUS_AMT": null, "LINK": "" }, { "GROUP_ID": "L", "ITEM_ID": 1, "ITEM_HEADING": "State Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 2, "ITEM_HEADING": "Central Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 3, "ITEM_HEADING": "Panchayat Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 4, "ITEM_HEADING": "Other Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 5, "ITEM_HEADING": "Surplus and Reserves", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 6, "ITEM_HEADING": "Deposits Repayable", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 7, "ITEM_HEADING": "Current Liabilities", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 17, "ITEM_HEADING": "Bank Authorization Outstanding with DPIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": -1, "ITEM_HEADING": "Total Funds and Liabilities", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": "" }, { "GROUP_ID": "A", "ITEM_ID": -1, "ITEM_HEADING": "Maintenance Fund Assets", "CURRENT_AMT": null, "PREVIOUS_AMT": null, "LINK": "" }, { "GROUP_ID": "A", "ITEM_ID": 8, "ITEM_HEADING": "State Maintenance Fund transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 9, "ITEM_HEADING": "Central Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 10, "ITEM_HEADING": "Panchayat Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 11, "ITEM_HEADING": "Other Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 12, "ITEM_HEADING": "Cash in Chest", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 13, "ITEM_HEADING": "Bank Balance", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 14, "ITEM_HEADING": "Investments \u0026 Deposits", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 15, "ITEM_HEADING": "Imprest with staff", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 16, "ITEM_HEADING": "Current Assets", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": -1, "ITEM_HEADING": "Total", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": "" }] ;
//{ "total": 1, "page": 1, "records": 21, "rows": [{ "GROUP_ID": "L", "ITEM_ID": -1, "ITEM_HEADING": "Maintenance Fund Liabilities", "CURRENT_AMT": null, "PREVIOUS_AMT": null, "LINK": "" }, { "GROUP_ID": "L", "ITEM_ID": 1, "ITEM_HEADING": "State Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 2, "ITEM_HEADING": "Central Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 3, "ITEM_HEADING": "Panchayat Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 4, "ITEM_HEADING": "Other Maintenance Fund", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 5, "ITEM_HEADING": "Surplus and Reserves", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 6, "ITEM_HEADING": "Deposits Repayable", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 7, "ITEM_HEADING": "Current Liabilities", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": 17, "ITEM_HEADING": "Bank Authorization Outstanding with DPIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "L", "ITEM_ID": -1, "ITEM_HEADING": "Total Funds and Liabilities", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": "" }, { "GROUP_ID": "A", "ITEM_ID": -1, "ITEM_HEADING": "Maintenance Fund Assets", "CURRENT_AMT": null, "PREVIOUS_AMT": null, "LINK": "" }, { "GROUP_ID": "A", "ITEM_ID": 8, "ITEM_HEADING": "State Maintenance Fund transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 9, "ITEM_HEADING": "Central Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 10, "ITEM_HEADING": "Panchayat Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 11, "ITEM_HEADING": "Other Maintenance fund Transferred to PIUs", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 12, "ITEM_HEADING": "Cash in Chest", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 13, "ITEM_HEADING": "Bank Balance", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 14, "ITEM_HEADING": "Investments \u0026 Deposits", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 15, "ITEM_HEADING": "Imprest with staff", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": 16, "ITEM_HEADING": "Current Assets", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": null }, { "GROUP_ID": "A", "ITEM_ID": -1, "ITEM_HEADING": "Total", "CURRENT_AMT": 0.00, "PREVIOUS_AMT": 0.00, "LINK": "" }] };


function ViewAbstractGrading() {
    //alert("OK");


    blockPage();
    jQuery("#tblList").jqGrid({
        url: '/Reports/GetBalanceSheetHeaderJSon',
        postData: $("#frmBalanceSheet").serialize(),
        colNames: ['Sr. No', '1', '2', 'Capital Fund and Liabilities', 'Schedule ', 'Current Year Amount ', 'Previous Year Amount '],
        //colNames: [ '1', '2', '3', '4',  '5', '2P', '2', '3', '4', '5', '6', '7p', '7','8','9','10','11','12p','12','13','14','15','16','17p','17','18','19','20','21','22p','22','23','24','25','26','27p','27','28','29','30','31'],
        colModel: [

                   { name: 'SrNo', index: 'SrNo', sortable: false, width: 50, align: 'center', hidden: false, editable: true, formatter: SerialNumberFarmatter },
   		            { name: 'GROUP_ID', index: 'GROUP_ID', sortable: false, width: 10, align: 'right', hidden: true },
                    { name: 'ITEM_ID', index: 'ITEM_ID', sortable: false, width: 10, align: 'left', hidden: true },
                    { name: 'ITEM_HEADING', index: 'ITEM_HEADING', sortable: false, width: 500, align: 'left', hidedlg: false, editable: true, formatter: ItemFarmatter },
                     { name: 'LINK', index: 'LINK', sortable: false, width: 300, align: 'left', hidedlg: false, editable: true, formatter: ScheduleFarmatter },
                    { name: 'CURRENT_AMT', index: 'CURRENT_AMT', sortable: false, width: 150, align: 'center', hidden: false },
                    { name: 'PREVIOUS_AMT', index: 'PREVIOUS_AMT', sortable: false, width: 150, align: 'center', hidden: false }
                   
        ],
        datatype: "json",
        mtype: 'post',
        viewrecords: true,
        rowNum: 100000,
        height: 'auto',
        width:'auto',
        pginput: false,
        
        //autowidth: true,
        shrinkToFit: true,
        pgbuttons: false,
        sortable: false,
        rownumbers: false,
        loadtext: 'Loading data,Please wait....',
        
        caption: "<B>Asset Value of PMGSY</B>",
        loadComplete: function (data) {
            
            GridData = data;
            var strBalanceSheetHeader =' <tr><td style="text-align:right" colspan="5">'+  data.header.ReportFormNumber +'</td> </tr>'+
                                       ' <tr><td style="text-align:center"  colspan="5">' + data.header.FundType + '</td> </tr>' +
                                        ' <tr><td style="text-align:center"  colspan="5">'+ data.header.ReportHeader  +'</td> </tr>'+
                                        ' <tr><td style="text-align:center"  colspan="5">'+ data.header.Section + '</td> </tr>'+
                                        ' <tr> <td style="text-align:left"  colspan="5">'+ data.header.SelectionHeader+'</td> </tr>';
                                      // alert(strBalanceSheetHeader);
            $("#tblHeaderBalanceSheet tr").remove();
            $("#tblHeaderBalanceSheet").append(strBalanceSheetHeader);
     
            unblockPage();
        },
        footerrow: false,
        jsonReader: {
            repeatitems: false,
           
            total: "total",
            records: "records",
            page: "page",
            root: "rows",
            cell: "",
            id: "0"
        }
    });

    jQuery("#tblList").jqGrid('navGrid', '#divBalanceSheet', { add: false, edit: false, del: false, search: false, refresh: false, view: false });

}

var counter = 0; 
function SerialNumberFarmatter(cellvalue, options, rowObject) {
   

    if (rowObject.ITEM_ID == "0" || rowObject.ITEM_ID == "-1") {
        counter=0;
        return "";
       
    }
    else {
           
        counter++;
        return counter;
    }
    
   
}
function ItemFarmatter(cellvalue, options, rowObject) {
    var str = cellvalue;
    
    if (rowObject.ITEM_ID == "0") {
        
        str = '<center><b>' + rowObject.ITEM_HEADING + '</b></center>';
       // alert(str);
    }
    else if (rowObject.ITEM_ID == "-1") {

        str = '<b>' + rowObject.ITEM_HEADING + '</b>';
        // alert(str);
    }
    
    return str;
}
function ScheduleFarmatter(cellvalue, options, rowObject) {


    return '<center>---</center>';

}
function ListPmgsyRoad() {

    blockPage();
   // alert($("#frmBalanceSheet").serialize());
    jQuery("#tblList").jqGrid({
        url: '/Reports/GetBalanceSheetJSon',
        //datatype: "json",
        datatype: "local",
        mtype: "GET",
        //postData: $("#frmBalanceSheet").serialize(),
        data:jsondataforgrid,
        colNames: ["Road Name", "Package Number", "Length of Road(in Km.)", "Last Entry Made For the Year", "PCI INDEX"],
        colModel: [
                    { name: 'GROUP_ID', index: 'GROUP_ID', width: 400, sortable: false, align: "left" },
                    { name: 'ITEM_ID', index: 'ITEM_ID', width: 200, sortable: false, align: "center" },
                    { name: 'ITEM_HEADING', index: 'ITEM_HEADING', width: 200, sortable: false, align: "center" },
                    { name: 'CURRENT_AMT', index: 'CURRENT_AMT', width: 200, sortable: false, align: "center" },
                    { name: 'PREVIOUS_AMT', index: 'PREVIOUS_AMT', width: 120, sortable: false, align: "center" }
        ],
        
       
        rowNum: 100,
        pager: jQuery('#divBalanceSheet'),
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PMGSY Roads",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            alert("Success");
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function SearchBalancesheetJQGrid() {

    
    if ($('#frmBalanceSheet').valid()) {

        //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Reports/GetBalanceSheetJSon",
            type: "GET",
            async: false,
            cache: false,
            data: $("#frmBalanceSheet").serialize(),
            success: function (data) {

                // $('#divBalanceSheet').html(data);
                alert(data.total);

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
        // $.unblockUI();
    }

}

