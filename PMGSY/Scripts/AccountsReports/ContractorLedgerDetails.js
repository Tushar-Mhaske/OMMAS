
//$(document).ready(function () {

//    var oSingleCB = $('#tblLedger').dataTable({
//        "bJQueryUI": true,
//        "bFilter": false,
//        "bSort": false,
//        "bHeader": true,
//        "sScrollY": "320px",
//        "bPaginate": false,
//        "bScrollInfinite": true,
//        "bScrollCollapse": true,
//        "sDom": '<"H"Tfr>t<"F"ip>',
//        //"oTableTools": {
//        //    "aButtons": [
//        //		{
//        //		    "sExtends": "pdf",
//        //		    "sPdfOrientation": "landscape",
//        //		    "sTitle": $("#rptMAAnnex").find('td:eq(1)').text(),
//        //		    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
//        //		    //"sPdfMessage": pdfMessage,
//        //		    // "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
//        //		    "sFileName": $("#Year").val() + "," + $("#ddlBalance").val() + ".pdf"
//        //		},
//        //        {
//        //            "sExtends": "xls",
//        //            "bBomInc": true,
//        //            "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
//        //            "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
//        //        }

//        //    ]
//        //},
//        "oTableTools": {
//            "aButtons": []
//        },
//        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
//            $("td:first", nRow).html(iDisplayIndex + 1);
//            return nRow;
//        }

//    });


//});

$(document).ready(function () {

    var fundType = $("#FundType").val();
    if (fundType == "P") {
        LoadProgContLedger();
    }

    else if (fundType == "M") {
        LoadMainContLedger();
    }
    
});

function LoadProgContLedger() {

    $('#tblContLedgerDetails').jqGrid({

        url: '/AccountsReports/GetContLedgerDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "PIUCode": $("#PIUCode").val(), "AggrementCode": $("#AggrementId").val(), "ContrCode": $("#ContractorId").val() },
        colNames: ['Number', 'Date', 'Advance Payment (In Rs.) (+)Debit (-)Credit', 'Secured Advance (In Rs.) (+)Debit (-)Credit', 'Mobilisation Advance (In Rs.) (+)Debit (-)Credit', 'Machinery Advance (In Rs.) (+)Debit (-)Credit', 'Material Issued   (In Rs.) (+)Debit (-)Credit', 'Name Of Work', 'Particulars Of Transaction', 'Debits (In Rs.)', 'Credits (In Rs.)', 'Total Value Of Work Or Supplies (In Rs.)', 'Remark'],
        colModel: [
                            { name: 'Number', index: 'Number', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'Date', index: 'Date', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'Payment', index: 'Payment', height: 'auto', width: 60, align: "right", sortable: false },
                            { name: 'Secured', index: 'Secured', height: 'auto', width: 60, align: "right", sortable: false },
                            { name: 'Mobilisation', index: 'Mobilisation', height: 'auto', width: 65, align: "right", },
                            { name: 'Machinery', index: 'Mobilisation', height: 'auto', width: 65, align: "right", },
                            { name: 'Material', index: 'Material', height: 'auto', width: 50, align: "right", sortable: false },
                            { name: 'Work', index: 'Work', height: 'auto', width: 130, align: "left", sortable: false },
                            { name: 'Transaction', index: 'Transaction', height: 'auto', width: 130, align: "left", sortable: false },
                            { name: 'Debits', index: 'Debits', height: 'auto', width: 100, align: "right", sortable: false, summaryType: 'sum', summaryTpl: '<b> {0}</b>', formatter: Formatter_CreditDebitAmount },
                            { name: 'Credits', index: 'Credits', height: 'auto', width: 100, align: "right", sortable: false, summaryType: 'sum', summaryTpl: '<b> {0}</b>', formatter: Formatter_CreditDebitAmount },
                            { name: 'Value', index: 'Value', height: 'auto', width: 110, align: "right", sortable: false },
                            { name: 'Remark', index: 'Remark', height: 'auto', width: 120, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrContLedgerDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Number',
        sortorder: "asc",
        height: 'auto',
        width: "auto",
        rownumbers: true,      
        hidegrid: true, 
        caption: "Contractor Ledger Details",
        grouping: true,
        groupingView: {
            // groupSummary: true,
            groupSummary: [true],
            groupField: ['Number','Work'],
           // groupColumnShow: [false, false,false],
            groupCollapse: true,
            groupDataSorted: true,
            groupOrder: ['asc'],
            showSummaryOnHide: true

        },
        loadComplete: function (data) {

            var recordCount = jQuery('#tblContLedgerDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblContLedgerDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblContLedgerDetails').jqGrid('setGridHeight', 'auto');
            }
            //$("#tblTransactionDetails").footerData('set', { "Debit": "Total" }, true); //set footer data
            //var grid = $("#tblTransactionDetails"),
            //sum = grid.jqGrid('getCol', 'Amount', false, 'sum');
            //grid.jqGrid('footerData', 'set', { ID: 'Total:', Amount: sum });
        }
    });

    jQuery("#tblContLedgerDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Number', numberOfColumns: 2, titleText: 'Voucher/Transfer Entry No.' },
          { startColumnName: 'Debits', numberOfColumns: 2, titleText: 'Gross Transaction' }
        ]
    });

}

function LoadMainContLedger() {

    $('#tblContLedgerDetails').jqGrid({

        url: '/AccountsReports/GetContLedgerDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "AggrementCode": $("#AggrementId").val(), "ContrCode": $("#ContractorId").val() },
        colNames: ['Number', 'Date', 'Advance Payment (In Rs.) (+)Debit (-)Credit', 'Secured Advance (In Rs.) (+)Debit (-)Credit', 'Mobilisation Advance (In Rs.) (+)Debit (-)Credit', 'Machinery Advance (In Rs.) (+)Debit (-)Credit', 'Material Issued (In Rs.) (+)Debit (-)Credit', 'Name Of Work', 'Particulars Of Transaction', 'Debits (In Rs.)', 'Credits (In Rs.)', 'Total Value Of Work Or Supplies (In Rs.)', 'Remark'],
        colModel: [
                            { name: 'Number', index: 'Number', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Date', index: 'Date', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'Payment', index: 'Payment', height: 'auto', width: 60, align: "right", sortable: false },
                            { name: 'Secured', index: 'Secured', height: 'auto', width: 60, align: "right", sortable: false },
                            { name: 'Mobilisation', index: 'Mobilisation', height: 'auto', width: 65, align: "right", hidden: true },
                            { name: 'Machinery', index: 'Mobilisation', height: 'auto', width: 65, align: "right", hidden: true },
                            { name: 'Material', index: 'Material', height: 'auto', width: 50, align: "right", sortable: false },
                            { name: 'Work', index: 'Work', height: 'auto', width: 150, align: "right", sortable: false },
                            { name: 'Transaction', index: 'Transaction', height: 'auto', width: 150, align: "left", sortable: false },
                            { name: 'Debits', index: 'Debits', height: 'auto', width: 110, align: "right", sortable: false },
                            { name: 'Credits', index: 'Credits', height: 'auto', width: 110, align: "right", sortable: false },
                            { name: 'Value', index: 'Value', height: 'auto', width: 130, align: "right", sortable: false },
                            { name: 'Remark', index: 'Remark', height: 'auto', width: 150, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrContLedgerDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Number',
        sortorder: "asc",
        height: 'auto',
        width: "auto",
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        caption: "Contractor Ledger Details",
        grouping: true,
        groupingView: {
            // groupSummary: true,
            groupField: ['Number', 'Work'],
            groupCollapse: true,
            groupDataSorted: true,
            groupOrder: ['asc'],
            

        },
        loadComplete: function (data) {

            var recordCount = jQuery('#tblContLedgerDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblContLedgerDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblContLedgerDetails').jqGrid('setGridHeight', 'auto');
            }
            //$("#tblTransactionDetails").footerData('set', { "Debit": "Total" }, true); //set footer data
            //var grid = $("#tblTransactionDetails"),
            //sum = grid.jqGrid('getCol', 'Amount', false, 'sum');
            //grid.jqGrid('footerData', 'set', { ID: 'Total:', Amount: sum });
        }
    });

    jQuery("#tblContLedgerDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Number', numberOfColumns: 2, titleText: 'Voucher/Transfer Entry No.' },
          { startColumnName: 'Debits', numberOfColumns: 2, titleText: 'Gross Transaction' }
        ]
    });

}
function Formatter_CreditDebitAmount(cellValue, option, rowObject) {
    if (parseFloat(cellValue) == 0.00)
        return "-";
    else
        return parseFloat(cellValue).toFixed(2);
}

