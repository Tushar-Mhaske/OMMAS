

$(document).ready(function () {
       
    $(function () {

        $("#rdoOperationalNo").attr("checked", false);

    });

    $("#btnSearchTxnDetails").click(function () {
       
        var IsOperational;

        if ($("#rdoOperationalYes").is(":checked"))
        {
            IsOperational = true;
        }
        else if ($("#rdoOperationalNo").is(":checked")) {
            IsOperational = false;
        }

        $('#tbTransactionDetailsList').setGridParam({
            url: '/AccountMaster/MasterTransactionList', datatype: 'json'
        });

        $('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": { ParentTxn: $('#ddlParentTxn option:selected').val(), Level: $('#ddlLevel option:selected').val(), CashCheque: $('#ddlCashCheque option:selected').val(), BillType: $('#ddlBillType option:selected').val(), IsOperational: IsOperational, IsSearch: true } });
        $('#tbTransactionDetailsList').trigger("reloadGrid");

    });

    //Cancel
    $("#btnCancel").click(function () {

        $("#btnSearch").show();
        $("#btnCreateNew").hide();

        var ParentSubTransaction="P";

        //if ($("#rdoParentTransaction").is(":checked")) {
        //    ParentSubTransaction = "P";
        //}
        //else {
        //    ParentSubTransaction = "S";
        //}

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterTransactionDetails/' + ParentSubTransaction,
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterTransactionAddEdit").html(data);
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Request can not be processed at this time.");
            }
        })
    });


});




