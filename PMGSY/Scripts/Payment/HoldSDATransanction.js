$(document).ready(function () {

    $.validator.unobtrusive.parse($("#frmSDAholding"));

    ViewHoldingSDATransactionDetails();

    $("#btnView").click(function () {

        if ($("#frmSDAholding").valid()) {

            $.validator.unobtrusive.parse($("#frmSDAholding"));

            ViewHoldingSDATransactionDetails();
        }

    });

    function ViewHoldingSDATransactionDetails() {

        jQuery("#tbHoldingList").jqGrid('GridUnload');
        var totalAmt;
        var CheqTotal = 0;
        var CashTotal = 0;
        var selectRow = true;
        totalAmt = 0;


        $("#tbHoldingList").jqGrid({
            url: "/Payment/GetSDAandHoldingList",
            datatype: 'json',
            mtype: 'POST',
            multiselect: true,  // change
            cache: false,
            postData: { month: $("#ddlmonth option:selected").val(), year: $("#ddlyear option:selected").val(), DPIUCode: $("#ddlDPIU option:selected").val(), TxnID: $("#ddlTxn option:selected").val() },  // , TxnID: $("#  option:selected").val()
            colNames: ['Bill ID', "Voucher Number", 'Voucher Date', 'Transaction Type', 'Transaction Desc', 'Cheque/ Epayment/ Advice Number', 'Cheque Amount (in Rs.)', 'Cash Amount (in Rs.)', "Gross Amount (in Rs.)", "Bank Acknowledgment Status", "Finalize", "TXN NO"],  //
            colModel: [
                { key: true, hidden: true, name: 'BILL_ID', index: 'BILL_ID', editable: true },
                { key: false, name: 'BILL_NO', index: 'BILL_NO', editable: true, width: 18, align: "center", resizable: true },
                { name: 'BILL_DATE', index: 'BILL_DATE', width: 16, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'TXN_ID', index: 'TXN_ID', width: 15, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'TXN_DESC', index: 'TXN_DESC', width: 45, resize: false, align: "left", sortable: true, resizable: true },
                { name: 'CHQ_NO', index: 'CHQ_NO', width: 30, align: "center", resize: false, sortable: true, resizable: true },
                { key: false, name: 'CHQ_AMOUNT', index: 'CHQ_AMOUNT', editable: true, width: 20, align: "left", resizable: true },
                { key: false, name: 'CASH_AMOUNT', index: 'CASH_AMOUNT', editable: true, width: 10, align: "left", resizable: true },
                { key: false, name: 'GROSS_AMOUNT', index: 'GROSS_AMOUNT', editable: true, width: 20, align: "left", resizable: true },
                { key: false, name: 'BANK_ACK_BILL_STATUS', index: 'BANK_ACK_BILL_STATUS', editable: true, width: 20, align: "left", resizable: true },

                { key: false, name: 'IS_FINALIZE', index: 'IS_FINALIZE', editable: true, width: 20, align: "center", resizable: true, hidden: true },
                { key: false, name: 'TXN_NO', index: 'TXN_NO', width: 20, align: "center", resize: false, sortable: true, resizable: true, hidden: true }
            ],

            pager: "#pager", // change
            rowNum: 20,
            rowList: [20, 40, 60],
            rownumbers: true,
            //loadonce: true,
            viewrecords: true,
            footerrow: true, // change
            userDataOnFooter: true, // change
            recordtext: '{2} records found',
            sortname: "BILL_ID",
            sortorder: "asc",
            caption: "&nbsp;&nbsp; Holding & SDA Transaction List",
            height: 'auto',
            autowidth: true,
            hidegrid: true,
            rownumbers: true,
            cmTemplate: { title: false },
            onSelectAll: function (aRowids, status) {
                var amtTotal = 0;
                var CheqAmt = 0;
                var CashAmt = 0;
                var grid = $("#tbHoldingList");
                var rowKey = grid.getGridParam("selrow");

                if (!rowKey) {
                    $("#tbHoldingList").footerData('set', { "CHQ_AMOUNT": CheqAmt.toFixed(2) }, true);
                    $("#tbHoldingList").footerData('set', { "CASH_AMOUNT": CashAmt.toFixed(2) }, true);
                    $("#tbHoldingList").footerData('set', { "GROSS_AMOUNT": amtTotal.toFixed(2) }, true);
                }
                else {
                    var selectedIDs = grid.getGridParam("selarrrow");
                    for (var i = 0; i < selectedIDs.length; i++) {
                        if ($("#jqg_tbHoldingList_" + selectedIDs[i]).attr("disabled")) {
                            $("#jqg_tbHoldingList_" + selectedIDs[i]).attr("checked", false);
                        }
                        else {

                            var rowData = $("#tbHoldingList").getRowData(selectedIDs[i]);
                            amtTotal = parseFloat(amtTotal) + parseFloat(rowData.GROSS_AMOUNT);
                            CheqAmt = parseFloat(CheqAmt) + parseFloat(rowData.CHQ_AMOUNT);
                            CashAmt = parseFloat(CashAmt) + parseFloat(rowData.CASH_AMOUNT);
                        }
                    }
                }  // else end
                $("#tbHoldingList").footerData('set', { "GROSS_AMOUNT": amtTotal.toFixed(2) }, true);
                $("#tbHoldingList").footerData('set', { "CASH_AMOUNT": CashAmt.toFixed(2) }, true);
                $("#tbHoldingList").footerData('set', { "CHQ_AMOUNT": CheqAmt.toFixed(2) }, true);
                totalAmt = amtTotal;
                CheqTotal = CheqAmt;
                CashTotal = CashAmt;
                var userdata = jQuery("#tbHoldingList").getGridParam('userData');

                idsOfSelectedRows = userdata.ids;
                if (userdata.ids.length == $("#tbHoldingList").jqGrid('getGridParam', 'records')) {
                    $('.cbox').attr('checked', true);

                    var CheqAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CHQ_AMOUNT', false, 'sum');
                    var CashAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CASH_AMOUNT', false, 'sum');
                    var amtTotal = jQuery("#tbHoldingList").jqGrid('getCol', 'GROSS_AMOUNT', false, 'sum');

                    jQuery("#tbHoldingList").jqGrid('footerData', 'set',
                        {
                            CHQ_AMOUNT: parseFloat(CheqAmt).toFixed(2),
                            CASH_AMOUNT: parseFloat(CashAmt).toFixed(2),
                            GROSS_AMOUNT: parseFloat(amtTotal).toFixed(2)
                        });
                }
            },
            onSelectRow: function (id, isSelected) {

                var rowData = $("#tbHoldingList").getRowData(id);
                if (isSelected) {
                    var selectedRowIds = jQuery('#tbHoldingList').jqGrid('getGridParam', 'selarrrow');

                    if (selectedRowIds.length == $("#tbHoldingList").jqGrid('getGridParam', 'records')) {
                        $('.cbox').attr('checked', true);
                    }
                    if (rowData.GROSS_AMOUNT != undefined) {
                        totalAmt = parseFloat(totalAmt) + parseFloat(rowData.GROSS_AMOUNT);
                    }
                    if (rowData.CHQ_AMOUNT != undefined) {
                        CheqTotal = parseFloat(CheqTotal) + parseFloat(rowData.CHQ_AMOUNT);
                    }
                    if (rowData.CASH_AMOUNT != undefined) {
                        CashTotal = parseFloat(CashTotal) + parseFloat(rowData.CASH_AMOUNT);
                    }
                }
                else {
                    var summaryRow = $("#tbHoldingList").footerData('get', { name: "GROSS_AMOUNT" }, false);
                    var CheqsummaryRow = $("#tbHoldingList").footerData('get', { name: "CHQ_AMOUNT" }, false);
                    var CashsummaryRow = $("#tbHoldingList").footerData('get', { name: "CASH_AMOUNT" }, false);
                    totalAmt = parseFloat(summaryRow.GROSS_AMOUNT);
                    CheqTotal = parseFloat(CheqsummaryRow.CHQ_AMOUNT);
                    totalAmt = parseFloat(totalAmt) - parseFloat(rowData.GROSS_AMOUNT);
                    CheqTotal = parseFloat(CheqTotal) - parseFloat(rowData.CHQ_AMOUNT);
                    CashTotal = parseFloat(CashsummaryRow.CASH_AMOUNT);
                    CashTotal = parseFloat(CashTotal) - parseFloat(rowData.CASH_AMOUNT);
                }
                //set footer data
                $("#tbHoldingList").footerData('set', { "GROSS_AMOUNT": totalAmt.toFixed(2) }, true);
                $("#tbHoldingList").footerData('set', { "CHQ_AMOUNT": CheqTotal.toFixed(2) }, true);
                $("#tbHoldingList").footerData('set', { "CASH_AMOUNT": CashTotal.toFixed(2) }, true);
            },
            loadComplete: function (xhr, st, err, id) {

                $("#tbHoldingList #pager").css({ height: '31px' });

                $("#pager_left").html("<input type='button' style='margin-left:27px' id='idAutoGenerateVoucher' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'getSelectedRows();return false;' value='Generate Voucher'/>")

                var amtTotal = 0;
                var CheqAmt = 0;
                var CashAmt = 0;

                var userdata = jQuery("#tbHoldingList").getGridParam('userData');
                idsOfSelectedRows = userdata.ids;
                var CheqAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CHQ_AMOUNT', false, 'sum');
                var CashAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CASH_AMOUNT', false, 'sum');
                var amtTotal = jQuery("#tbHoldingList").jqGrid('getCol', 'GROSS_AMOUNT', false, 'sum');

                var grid = $("#tbHoldingList");

                $("#cb_tbHoldingList").trigger('click');
                $('.cbox').attr('checked', true);

                for (var i = 0; i < userdata.ids.length; i++) {
                    if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', false)) {
                        jQuery("#jqg_tbHoldingList_" + userdata.ids[i]).attr("disabled", true);
                    }
                    var rowData = $("#tbHoldingList").getRowData(userdata.ids[i]);

                    if (rowData.GROSS_AMOUNT != undefined) {
                        amtTotal = parseFloat(amtTotal) - parseFloat(rowData.GROSS_AMOUNT);
                    }
                    if (rowData.CHQ_AMOUNT != undefined) {
                        CheqAmt = parseFloat(CheqAmt) - parseFloat(rowData.CHQ_AMOUNT);
                    }
                    if (rowData.CASH_AMOUNT != undefined) {
                        CashAmt = parseFloat(CashAmt) - parseFloat(rowData.CASH_AMOUNT);
                    }
                }

                jQuery("#tbHoldingList").jqGrid('footerData', 'set',
                    {
                        CHQ_AMOUNT: parseFloat(CheqAmt).toFixed(2),
                        CASH_AMOUNT: parseFloat(CashAmt).toFixed(2),
                        GROSS_AMOUNT: parseFloat(amtTotal).toFixed(2)
                    });

                //if all rows selected  check header checkbox              
                if (userdata.ids.length == $("#tbHoldingList").jqGrid('getGridParam', 'records')) {
                    $('.cbox').attr('checked', true);

                    var CheqAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CHQ_AMOUNT', false, 'sum');
                    var CashAmt = jQuery("#tbHoldingList").jqGrid('getCol', 'CASH_AMOUNT', false, 'sum');
                    var amtTotal = jQuery("#tbHoldingList").jqGrid('getCol', 'GROSS_AMOUNT', false, 'sum');

                    jQuery("#tbHoldingList").jqGrid('footerData', 'set',
                        {
                            CHQ_AMOUNT: parseFloat(CheqAmt).toFixed(2),
                            CASH_AMOUNT: parseFloat(CashAmt).toFixed(2),
                            GROSS_AMOUNT: parseFloat(amtTotal).toFixed(2)
                        });
                }

                var reccount = $('#tbHoldingList').getGridParam('reccount');
                if (reccount > 0) {
                    $('#pager_right').html('[Note: Select only those voucher that you wish to Generate.]');
                }
                if (($("#tbHoldingList").getGridParam("reccount") > 14)) {
                    $('#tbHoldingList').jqGrid('setGridHeight', "360px");
                }

            },
            beforeSelectRow: function (rowId, e) {

                if ($("#jqg_tbHoldingList_" + rowId).attr("disabled")) {
                    //selectRow = false;
                    return false;
                }
                else {
                    //selectRow = true;
                    return true;
                }
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")
                }
            }
        })
    }

});//document ready

function getSelectedRows() {
    var TxnId = 0;
    var grid = $("#tbHoldingList");
    var rowKey = grid.getGridParam("selrow");
    /*  alert("rowKey " + rowKey)*/
    if (!rowKey)
        alert("Please Select Voucher.");
    else {
        var selectedIDs = grid.getGridParam("selarrrow");
        var result = "";
        var result1 = "";
        var submitArray = [];
        if (selectedIDs.length > 0) {
            for (var i = 0; i < selectedIDs.length; i++) {

                if ($("#jqg_tbHoldingList_" + selectedIDs[i]).attr("disabled")) {


                }
                else {
                    var rowData = $("#tbHoldingList").getRowData(selectedIDs[i]);
                    // TxnId = parseFloat(rowData.TXN_NO);
                    submitArray.push(selectedIDs[i]); //  + "$" + TxnId
                }
            }

        }
        else {
            alert("Please select the row for Generation of auto entries at SRRDA Level");
        }

        AutoHoldingTransaction(submitArray);
    }
}




function AutoHoldingTransaction(submitArray) {

    /*alert("ok" + submitArray)*/
    var month = $("#ddlmonth option:selected").val();
    var year = $("#ddlyear option:selected").val();
    var DPIUCode = $("#ddlDPIU option:selected").val();
    var aCCtyPE = $("#ddlTxn option:selected").val();

    if (confirm('Do you want Generate Auto entries at SRRDA Level') == true) {

        $.ajax({
            type: "POST",
            url: '/Payment/AutomateHoldingSecurity/?id=' + month + "$" + year + "$" + DPIUCode + "$" + aCCtyPE,
            dataType: 'json',
            data: JSON.stringify({ 'submitarray': submitArray }),
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (data) {
                if (data.Success) {

                    alert(data.message);

                    $('#tbHoldingList').trigger('reloadGrid');
                }
                else {
                    alert(data.message);

                    $('#tbHoldingList').trigger('reloadGrid');

                }
            },
            error: function (error) {
                alert("Error occured while Processing Request.")
                $('#tbWorkList').trigger('reloadGrid');
            }
        });

    }
}