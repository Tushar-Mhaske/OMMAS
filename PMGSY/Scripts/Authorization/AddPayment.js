$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddPayment'));

    $("#VoucherNo").attr('readonly', true);
   


    $("#VoucherDate").blur(function () {
        
        try {
            var arraydate = $("#VoucherDate").val().split("/");
            if (arraydate[0].length != 2 || arraydate[1].length != 2 || arraydate[2].length != 4) {
                $("#VoucherDate").focus();
                return false;
            }
            else {

                $.ajax({
                    type: "POST",
                    url: "/payment/GenerateVoucherNo/V$" + arraydate[1] + '$' + arraydate[2],
                    async: false,
                    error: function (xhr, status, error) {
                    },
                    success: function (data) {
                        unblockPage();
                        if (data != "") {
                            //$("#VoucherNo").val("");
                            $("#VoucherNo").val(data.strVoucherNumber);
                            $("#VoucherNo").attr('readonly', true);
                            
                        }
                    }
                });
            }
        }
        catch (err) {

        }
    });


    $("#VoucherDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        buttonText: 'Voucher Date',
        maxDate: new Date(),
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $("#btnSavePayment").click(function (evt) {
        $("#PHDN_AUTH_ID").val(authId);
        evt.preventDefault();
        if ($('#frmAddPayment').valid()) {
            $.ajax({
                url: "/Authorization/AddPaymentDetails/" + authId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddPayment").serialize(),
                success: function (data) {

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadPaymentMaster").html(data);
                        }
                        else {
                            $("#divPaymentMasterError").show("slide");
                            $("#divPaymentMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divPaymentMasterError").hide("slide");
                        $("#divPaymentMasterError span:eq(1)").html('');
                        $("#btnResetPayment").trigger('click');
                        alert("Payment Details Added.");
                        $("#loadPaymentMaster").html("");
                        LoadPFAuthorizationGrid();
                        $("#DigReceiptPayment").dialog('close');
                        $("#DigReceiptPayment").hide();

                        $("#tblPFAuthList").trigger('reloadGrid');
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    });

    $("#btnResetPayment").click(function () {

    });

}); // Document.ready ends here