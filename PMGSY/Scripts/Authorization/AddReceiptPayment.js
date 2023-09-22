
$(document).ready(function () {

   //alert(billId);
    if (currentStatus == "A") {
        $("#loadReceiptMaster").load("/Authorization/AddReceiptDetails/");
    }
    else if (currentStatus == "R") {
        $("#loadReceiptMaster").load("/Authorization/AddReceiptDetails/" + billId, function () {
            //$("#BILL_NO").attr("disabled", "disabled");
            //$("#BILL_DATE").attr("disabled", "disabled");
            //$("#BILL_DATE").datepicker('disabled').attr('readonly', 'readonly');

             //$("#BILL_DATE").parent('td').find(".ui-datepicker-trigger").css('display', 'none');
                  
            $('#BILL_DATE').attr('readonly', 'readonly');
            $('#BILL_NO').attr('readonly', 'readonly');
            //$('.ui-datepicker-trigger').hide();
            $("#BILL_DATE").datepicker("disable");
        });
        $("#loadPaymentMaster").load("/Authorization/AddPaymentDetails/");
    }

}); //Document.Ready end here

