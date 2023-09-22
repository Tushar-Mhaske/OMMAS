

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#AccountMasterTransactionForm'));


    //Save Details 
    $("#btnSave").click(function () {
        
        if ($("#AccountMasterTransactionForm").valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


            
            $("#ddlLevel").attr("disabled", false);
            //$("#ddlCashCheque").attr("disabled", false);
            //$("#ddlBillType").attr("disabled", false);

            $.ajax({
                type: 'POST',
                url: '/AccountMaster/AddMasterTransactionDetails/',
                data: $("#AccountMasterTransactionForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#dvMasterTransactionAddEdit").html(data);

                        $("#ddlLevel").attr("disabled", true);
                        $("#ddlCashCheque").attr("disabled", true);
                        $("#ddlBillType").attr("disabled", true);
                    }
                    else if (data.success) {
                        alert(data.message);

                        //$("#btnReset").trigger('click');
                        ResetForm();

                        //$("#tbTransactionDetailsList").trigger('reloadGrid');
                        $('#tbTransactionDetailsList').setGridParam({
                            url: '/AccountMaster/MasterTransactionList', datatype: 'json'
                        });
                        $('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": { IsSearch: false } });
                        $('#tbTransactionDetailsList').trigger("reloadGrid");

                    }
                    else {
                        $("#divError").show();
                        $("#errorSpan").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();

                    $("#ddlLevel").attr("disabled", true);
                    //$("#ddlCashCheque").attr("disabled", true);
                    //$("#ddlBillType").attr("disabled", true);
                    alert("Request can not be processed at this time.");
                }
            })

        }

    });

    //Update Details 
    $("#btnUpdate").click(function () {

        if ($("#AccountMasterTransactionForm").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlParentTxn").attr("disabled", false);
            $("#ddlLevel").attr("disabled", false);
            //$("#ddlCashCheque").attr("disabled", false);
            //$("#ddlBillType").attr("disabled", false);


            $.ajax({
                type: 'POST',
                url: '/AccountMaster/EditMasterTransactionDetails/',
                data: $("#AccountMasterTransactionForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#dvMasterTransactionAddEdit").html(data);
                    }
                    else if (data.success) {
                        alert(data.message);
                        //$("#tbTransactionDetailsList").trigger('reloadGrid');
                        
                        //$('#tbTransactionDetailsList').setGridParam({
                        //    url: '/AccountMaster/MasterTransactionList', datatype: 'json'
                        //});
                        //$('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": {  IsSearch: false } });
                        //$('#tbTransactionDetailsList').trigger("reloadGrid");

                        if ($("btnSearch").is(":visible")) {
                            $('#tbTransactionDetailsList').setGridParam({
                                url: '/AccountMaster/MasterTransactionList', datatype: 'json'
                            });
                            $('#tbTransactionDetailsList').jqGrid("setGridParam", { "postData": { IsSearch: false } });
                            $('#tbTransactionDetailsList').trigger("reloadGrid");
                           // ResetForm();
                            $("#btnCancel").trigger('click');
                        }
                        else {
                            var IsOperational;

                            if ($("#rdoOperationalYes").is(":checked")) {
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
                            $("#btnCancel").trigger('click');

                            // ResetForm();

                        }

                    }
                    else {
                        $("#divError").show();
                        $("#errorSpan").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();

                    $("#ddlParentTxn").attr("disabled", true);
                    $("#ddlLevel").attr("disabled", true);
                   // $("#ddlCashCheque").attr("disabled", true);
                  //  $("#ddlBillType").attr("disabled", true);

                    alert("Request can not be processed at this time.");
                }
            })

        }

    });

    //Cancel
    $("#btnCancel").click(function () {
        ResetForm();           
    });

    $("#rdoParentTransaction").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterTransactionDetails/' + "P",
            //data: $("#AccountMasterHeadForm").serialize(),
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

    $("#rdoSubTransaction").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterTransactionDetails/' + "S",
            //data: $("#AccountMasterHeadForm").serialize(),
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

    $("#btnReset").click(function () {       
    });

    $("#ddlParentTxn").change(function () {

        if ($("#EncryptedHeadID").val()==null || ($("#EncryptedHeadID").val()==""))
        {
            $.ajax({
                type: 'POST',
                url: '/AccountMaster/GetParentTransactionDetails/' + $("#ddlParentTxn").val(),                
                async: false,
                cache: false,
                success: function (data) {
                    
                    $("#ddlLevel").val(data.Level);
                    //$("#ddlCashCheque").val(data.CashCheque);
                    //$("#ddlBillType").val(data.BillType);

                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });



});

function ResetForm()
{
    $("#btnSearch").show();
    $("#btnCreateNew").hide();

    var ParentSubTransaction;

    if ($("#rdoParentTransaction").is(":checked")) {
        ParentSubTransaction = "P";
    }
    else {
        ParentSubTransaction = "S";
    }

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
}