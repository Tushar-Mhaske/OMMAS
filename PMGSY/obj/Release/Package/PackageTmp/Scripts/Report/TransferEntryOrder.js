

$(document).ready(function () {
    //alert("Ready");
    //$("#tblHeaderTransferEntryOrderList").hide();
    $("#Dpiu").hide();

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmTransferEntryOrder'));

    $('#btnViewDetails').click(function () {
        //alert($("#frmTransferEntryOrder").serialize());

        

        if ($("#tblTransferEntryOrderGrid") != undefined) {
            $("#tblTransferEntryOrderGrid").GridUnload();
        }
        if ($("#frmTransferEntryOrder").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#tblHeaderTransferEntryOrderList").show();
            SearchTransferEntryOrder();
        }
        //SearchTransferEntryOrder1();

    });

    $("#rdbSRRDA").click(function () {
        $("#Dpiu").hide();
    });
    $("#rdbDPIU").click(function () {
        $("#Dpiu").show();
    });
   

    $('#btnViewDetails').trigger('click');

    $("#Month").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month").val(), $("#Year").val());

    });

});



function SearchTransferEntryOrder() {
   


    //blockPage();
    jQuery("#tblTransferEntryOrderGrid").jqGrid({
        url: '/Reports/GetTransferEntryOrderJson',
        postData: $("#frmTransferEntryOrder").serialize(),
        colNames: ['Date', 'T.E. No.', 'Particulars of Transaction with reasons for adjustment', 'Debit Head', 'Debit Amount ', 'Credit Head ', 'Credit Amount', "Dated Initials of Accountant"],
        //colNames: [ '1', '2', '3', '4',  '5', '2P', '2', '3', '4', '5', '6', '7p', '7','8','9','10','11','12p','12','13','14','15','16','17p','17','18','19','20','21','22p','22','23','24','25','26','27p','27','28','29','30','31'],
        colModel: [

                  // { name: 'SrNo', index: 'SrNo', sortable: false, width: 50, align: 'center', hidden: false, editable: true, formatter: SerialNumberFarmatter },
   		            { name: 'bill_date', index: 'bill_date', sortable: false, width: 100, align: 'center', hidden: false },
                    { name: 'BILL_NO', index: 'BILL_NO', sortable: false, width: 100, align: 'center', hidden: false },
                    { name: 'NARRATION', index: 'NARRATION', sortable: false, width: 650, align: 'left', hidedlg: false, editable: true },
                    { name: 'Debit_Head', index: 'Debit_Head', sortable: false, width: 100, align: 'center', hidedlg: false },
                    { name: 'Debit_Amount', index: 'Debit_Amount', sortable: false, width: 100, align: 'right', hidden: false, editable: true, summaryType: 'sum', summaryTpl: '<b>Total: {0}</b>', formatter: Farmetter_CreditDebitAmount, formatoptions: {decimalPlaces:2}},
                    { name: 'Credit_Head', index: 'Credit_Head', sortable: false, width: 100, align: 'center', hidden: false },
                    { name: 'Credit_Amount', index: 'Credit_Amount', sortable: false, width: 100, align: 'right', hidden: false, editable: true, summaryType: 'sum', summaryTpl: '<b>Total: {0}</b>', formatter: Farmetter_CreditDebitAmount },
                    { name: 'DatedInitials', index: 'DatedInitials', sortable: false, width: 100, align: 'center', hidden: false }


        ],
        datatype: "json",
        mtype: 'post',
        viewrecords: true,
        rowNum: 10000,
       
        
        height: 'auto',
        
            
       
        width: 'auto',
        pginput: false,

        autowidth: true,
        shrinkToFit: false,
        pgbuttons: false,
        sortable: false,
        rownumbers: true,
        loadtext: 'Loading data,Please wait....',

        caption: "<B>Transfer Entry Book</B>",
        loadComplete: function (ReponseData) {

          
            // $("#tblTransferEntryOrderGrid").css({ "maxheight": 350 + "px" })
            //  alert(screen.height * 0.2);
            var gridMaxHeight = screen.height * 0.37;
            if (gridMaxHeight < 350)
                gridMaxHeight = 350;
            $("#tblTransferEntryOrderGrid").parents('div.ui-jqgrid-bdiv').css("max-height",gridMaxHeight+ "px");
            var dpiuName = ReponseData.header.piusrrdaName;
            var srrdaName = ReponseData.header.srrdaName;
            if (dpiuName == "")
            {
                dpiuName = "Name of DPIU : " + $("#Dpiu option:selected").text();
            }
            var strTEOHeader = ' <tr><td style="text-align:right;color:green"><b>' + ReponseData.header.formNumber + '</b></td> </tr>' +
                                       ' <tr><td style="text-align:center;color:green"><b>' + ReponseData.header.pageHeader1 + '</b></td> </tr>' +
                                        ' <tr><td style="text-align:center;color:green"><b>' + ReponseData.header.pageHeader2 + '</b></td> </tr>' +
                                        ' <tr><td style="text-align:center;color:green"><b>' + ReponseData.header.pageHeader3 + '</b></td> </tr>' +                                        
                                        ' <tr> <td style="text-align:left;color:green"><b>' + srrdaName + '</b></td> </tr>' +
                                        ' <tr> <td style="text-align:left;color:green"><b>' + dpiuName + '</b></td> </tr>' +                                        
                                        ' <tr> <td style="text-align:left;color:green"><b>' + ReponseData.header.monthYear + '</b></td> </tr>';
            // alert(strBalanceSheetHeader);
            $("#tblHeaderTransferEntryOrder tr").remove();
            $("#tblHeaderTransferEntryOrder").append(strTEOHeader);
            var recordCount = jQuery('#tblTransferEntryOrderGrid').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {
                $(this).jqGrid('footerData', 'set', { NARRATION: " Total", Debit_Amount: ReponseData.footer.debitAmt, Credit_Amount: ReponseData.footer.creditAmt });
            }
           // unblockPage();            
            $.unblockUI();
        },
        
        jsonReader: {
            repeatitems: false,

            total: "total",
            records: "records",
            page: "page",
            root: "rows",
            cell: "",
            id: "0"
        },

        grouping: true,
        groupingView: {
            groupField: ['bill_date', 'BILL_NO'],
            groupColumnShow: [false,false],
            groupSummary: [false,true],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupCollapse: false,
            groupOrder: ['asc', 'asc'],
            showSummaryOnHide: true
            

        },
        footerrow: true,
        userDataOnFooter: true
    });

    $.unblockUI();

}
function Farmetter_CreditDebitAmount(cellValue, option,rowObject)
{
    if(parseFloat(cellValue)==0.00 || cellValue == "")
        return "-";
    else
        // return parseFloat(cellValue).toFixed(2);
        //modified by abhishek kamble 10-dec-2013
        return parseFloat(cellValue).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Reports/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}