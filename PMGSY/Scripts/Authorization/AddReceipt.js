$(document).ready(function () {

   
  
    $("#BILL_NO").attr('readonly', true);
    $.validator.unobtrusive.parse($('#frmAddReceipt'));


    $("#BILL_DATE").blur(function () {
         $("#BILL_NO").attr('readonly', true);
        try
        {
            var arraydate = $("#BILL_DATE").val().split("/");
            if (arraydate[0].length != 2 || arraydate[1].length != 2 || arraydate[2].length != 4) {
                $("#BILL_DATE").focus();
                return false;
            }
            else {
                
                $.ajax({
                    type: "POST",
                    url: "/payment/GenerateVoucherNo/R$" + arraydate[1] + '$' + arraydate[2],
                    async: false,
                    error: function (xhr, status, error) {
                    },
                    success: function (data) {
                        unblockPage();
                        if (data != "") {
                            $("#BILL_NO").val("");
                            $("#BILL_NO").val(data.strVoucherNumber);
                            $("#BILL_NO").attr('readonly', true);
                        }
                    }
                });
            }
        }
        catch(err)
        {

        }
    });


    $("#BILL_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        buttonText: 'Receipt Date',
        maxDate:new Date(),
        onClose: function () {

            $(this).focus().blur();
        }
    });
    
    $("#btnSaveReceipt").click(function (evt) {
        $("#RHDN_AUTH_ID").val(authId);
        evt.preventDefault();
        if ($('#frmAddReceipt').valid()) {
            $.ajax({
                url: "/Authorization/AddReceiptDetails/" + authId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddReceipt").serialize(),
                success: function (data) {

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadReceiptMaster").html(data);
                        }
                        else {
                            $("#divReceiptMasterError").show("slide");
                            $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divReceiptMasterError").hide("slide");
                        $("#divReceiptMasterError span:eq(1)").html('');
                        $("#btnResetReceipt").trigger('click');
                        // alert("Receipt Details Added.");
                        alert(data.message);
                        $("#loadReceiptMaster").html("");
                        $("#loadReceiptMaster").load("/Authorization/AddReceiptDetails/" + data.billid, function () {
                            //$("#BILL_NO").attr("disabled", "disabled");
                            //$("#BILL_DATE").attr("disabled", "disabled");
                            //$("#BILL_DATE").datepicker('disabled').attr('readonly', 'readonly');

                            $('#BILL_DATE').attr('readonly', 'readonly');
                            $('#BILL_NO').attr('readonly', 'readonly'); 
                            $('#BILL_DATE .ui-datepicker-trigger').hide();
                        });
                        $("#loadPaymentMaster").load("/Authorization/AddPaymentDetails/");
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    });

    $("#btnResetReceipt").click(function(){
        
    });
});