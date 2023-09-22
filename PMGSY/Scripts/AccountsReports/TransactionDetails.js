
$(document).ready(function () {
   // var billType = $("#BillType").val();

    var billType = $("#hdnBillType").val();
        
    var fundType = $("#FundType").val();

    if ((fundType == "M" || fundType == "P") && billType == "P") {       
        loadProgramGrid();
    }

    else if (fundType == "A" && billType == "P") {       
        loadAdminGrid();
    }

    else if (billType == "R") {        
        loadRecieptGrid();
    }
    else if ((fundType == "M" || fundType == "P") && billType == "J") {        
        loadTEOGrid();
    }
    else if (fundType == "A" && billType == "J") {      
        loadAdminTEOGrid();
    }
});

function loadProgramGrid() {

    $('#tblTransactionDetails').jqGrid({

        url: '/AccountsReports/GetTransactionDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "BillId": $("#BillId").val() },
        colNames: ['Head Description', 'Type', 'Cash/Cheque', 'Credit/Debit', 'Amount', 'Company Name', 'Agreement Number', 'Work Name', 'Department Name', 'Narration'],
        colModel: [
                            { name: 'HeadDescription', index: 'Desc', height: 'auto', width: 200, align: "left", sortable: false },
                            { name: 'Type', index: 'Type', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'Cash/Cheque', index: 'Cash/Cheque', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'Debit', index: 'Debit', height: 'auto', width: 50, align: "left", sortable: true },
                            { name: 'Amount', index: 'Amount', height: 'auto', width: 80, align: "right", sortable: false, formatter: 'number' },
                            { name: 'Name', index: 'Name', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'AggNumber', index: 'AggNumber', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'WorkName', index: 'WorkName', height: 'auto', width: 150, align: "left", sortable: false },
                            { name: 'DeptName', index: 'DeptName', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Narration', index: 'Narration', height: 'auto', width: 100, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrTransactionDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Debit',
        sortorder: "asc",
        height: 'auto',
        width: 'auto',
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function (data) {

            var recordCount = jQuery('#tblTransactionDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblTransactionDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblTransactionDetails').jqGrid('setGridHeight', 'auto');
            }
            $("#tblTransactionDetails").footerData('set', { "Debit": "Total" }, true); //set footer data
            var grid = $("#tblTransactionDetails"),
            sum = grid.jqGrid('getCol', 'Amount', false, 'sum');
            grid.jqGrid('footerData', 'set', { ID: 'Total:', Amount: sum });
        }
    });
}

function loadAdminGrid() {

    $('#tblTransactionDetails').jqGrid({

        url: '/AccountsReports/GetTransactionDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "BillId": $("#BillId").val() },
        colNames: ['Head Description', 'Type', 'Cash/Cheque', 'Debit', 'Amount', 'Department Name', 'Narration'],
        colModel: [
                            { name: 'HeadDescription', index: 'Desc', height: 'auto', width: 200, align: "left", sortable: false },
                            { name: 'Type', index: 'Type', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'Cash/Cheque', index: 'Cash/Cheque', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'Debit', index: 'Debit', height: 'auto', width: 50, align: "left", sortable: true },
                            { name: 'Amount', index: 'Amount', height: 'auto', width: 80, align: "right", sortable: false, formatter: 'number' },
                            { name: 'DeptName', index: 'DeptName', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Narration', index: 'Narration', height: 'auto', width: 100, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrTransactionDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Debit',
        sortorder: "asc",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {
            var recordCount = jQuery('#tblTransactionDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblTransactionDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblTransactionDetails').jqGrid('setGridHeight', 'auto');
            }

            $("#tblTransactionDetails").footerData('set', { "Debit": "Total" }, true);  //set footer data
            var grid = $("#tblTransactionDetails"),
             sum = grid.jqGrid('getCol', 'Amount', false, 'sum');

            grid.jqGrid('footerData', 'set', { ID: 'Total:', Amount: sum });


        }
    });


}

function loadRecieptGrid() {

    $('#tblTransactionDetails').jqGrid({

        url: '/AccountsReports/GetTransactionDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "BillId": $("#BillId").val() },
        colNames: ['Head Description', 'Type', 'Cash/Cheque', 'Credit', 'Amount', 'Company Name', 'Agreement Number', 'Work Name', 'Department Name', 'Narration'],
        colModel: [
                            { name: 'HeadDescription', index: 'Desc', height: 'auto', width: 200, align: "left", sortable: false },
                            { name: 'Type', index: 'Type', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'CashorCheque', index: 'CashorCheque', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'Credit', index: 'Credit', height: 'auto', width: 70, align: "left", sortable: true },
                            { name: 'Amount', index: 'Amount', height: 'auto', width: 80, align: "right", sortable: false, formatter: 'number' },
                            { name: 'Name', index: 'Name', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'AggNumber', index: 'AggNumber', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'WorkName', index: 'WorkName', height: 'auto', width: 150, align: "left", sortable: false },
                            { name: 'DeptName', index: 'DeptName', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Narration', index: 'Narration', height: 'auto', width: 150, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrTransactionDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Credit',
        sortorder: "asc",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {

            var recordCount = jQuery('#tblTransactionDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblTransactionDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblTransactionDetails').jqGrid('setGridHeight', 'auto');
            }

            $("#tblTransactionDetails").footerData('set', { "Credit": "Total" }, true);  //set footer data
            var grid = $("#tblTransactionDetails"),
            sum = grid.jqGrid('getCol', 'Amount', false, 'sum');
            grid.jqGrid('footerData', 'set', { ID: 'Total:', Amount: sum });


        }
    });
}

function loadTEOGrid() {

    $('#tblTransactionDetails').jqGrid({

        url: '/AccountsReports/GetTransactionDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "BillId": $("#BillId").val() },
        colNames: ['Head Description', 'Type', 'Cash/Cheque', 'Credit/Debit', 'Amount', 'Company Name', 'Agreement Number', 'Work Name', 'Department Name', 'Narration'],
        colModel: [
                            { name: 'HeadDescription', index: 'Desc', height: 'auto', width: 200, align: "left", sortable: false },
                            { name: 'Type', index: 'Type', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'CashorCheque', index: 'CashorCheque', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'CreditorDebit', index: 'CreditorDebit', height: 'auto', width: 90, align: "left", sortable: true },
                            { name: 'Amount', index: 'Amount', height: 'auto', width: 100, align: "right", sortable: false, formatter: 'number', summaryType: 'sum', summaryTpl: 'Total:{0}' },
                            { name: 'Name', index: 'Name', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'AggNumber', index: 'AggNumber', height: 'auto', width: 100, align: "left", sortable: false },
                            { name: 'WorkName', index: 'WorkName', height: 'auto', width: 150, align: "left", sortable: false },
                            { name: 'DeptName', index: 'DeptName', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Narration', index: 'Narration', height: 'auto', width: 200, align: "left", sortable: false },

        ],

        pager: jQuery('#pagrTransactionDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        sortorder:"CreditorDebit",
        viewrecords: true,
        recordtext: '{2} records found',
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        footerrow: true,
        userDataOnFooter: true,
        grouping: true,
        groupingView: {
            groupSummary: true,
            groupField: ['CreditorDebit']

        },
        loadComplete: function () {

            var recordCount = jQuery('#tblTransactionDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblTransactionDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblTransactionDetails').jqGrid('setGridHeight', 'auto');
            }

        }
    });



}

function loadAdminTEOGrid() {

    $('#tblTransactionDetails').jqGrid({

        url: '/AccountsReports/GetTransactionDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "BillId": $("#BillId").val() },
        colNames: ['Head Description', 'Type', 'Cash/Cheque', 'Credit/Debit', 'Amount', 'Department Name', 'Narration'],
        colModel: [
                            { name: 'HeadDescription', index: 'Desc', height: 'auto', width: 200, align: "left", sortable: false },
                            { name: 'Type', index: 'Type', height: 'auto', width: 50, align: "left", sortable: false },
                            { name: 'CashorCheque', index: 'CashorCheque', height: 'auto', width: 90, align: "left", sortable: false },
                            { name: 'CreditorDebit', index: 'CreditorDebit', height: 'auto', width: 50, align: "left", sortable: true },
                            { name: 'Amount', index: 'Amount', height: 'auto', width: 80, align: "right", sortable: false, formatter: 'number', summaryType: 'sum', summaryTpl: 'Total:{0}' },
                            { name: 'DeptName', index: 'DeptName', height: 'auto', width: 80, align: "left", sortable: false },
                            { name: 'Narration', index: 'Narration', height: 'auto', width: 100, align: "left", sortable: false },

        ],
        pager: jQuery('#pagrTransactionDetails'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        sortorder: "CreditorDebit",
        recordtext: '{2} records found',
        caption: "Transaction Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        footerrow: true,
        userDataOnFooter: true,
        grouping: true,
        groupingView: {
            groupSummary: true,
            groupField: ['CreditorDebit']
        },
        loadComplete: function () {

            var recordCount = jQuery('#tblTransactionDetails').jqGrid('getGridParam', 'reccount');

            if (recordCount > 10) {
                $('#tblTransactionDetails').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblTransactionDetails').jqGrid('setGridHeight', 'auto');
            }
        }
    });
}

