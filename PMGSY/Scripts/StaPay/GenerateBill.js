
$.validator.addMethod("comparevalidationwithhonoriumamount", function (value, element, params) {
       
    if (parseFloat($("#HONORARIUM_AMOUNT").val()) < parseFloat($("#PENALTY_AMOUNT").val())) {
        return false;
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwithhonoriumamount");



$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStaInvoice'));

    $("#btnSave").click(function (evt) {
        evt.preventDefault();
        if ($('#frmStaInvoice').valid()) {
            if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined && parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
                CalculateDetails($("#PENALTY_AMOUNT").val());
            }
            $.ajax({
                url: "/StaPay/AddStaInvoiceDetails/",
                type: "POST",
                cache: false,
                data: $("#frmStaInvoice").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    if (response.Success) {
                        $("#divError").html("");
                        $("#divError").hide();
                        $("#tbStaInvoiceList").trigger('reloadGrid');
                        alert('Invoice Details added Successfully.');
                    }
                    else {
                        //alert('There is an error occured while processing your request.');
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + response.ErrorMessage);
                        return false;
                    }
                    CloseDetails();
                }
            });
       }
    });

    $("#PENALTY_AMOUNT,#INVOICE_FILE_NO").blur(function () {
        if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined &&  parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
            CalculateDetails($("#PENALTY_AMOUNT").val());
        }
    });

    $("#PENALTY_AMOUNT").focusout(function () {
        if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined && parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
            CalculateDetails($("#PENALTY_AMOUNT").val());
        }
    });

});

function CalculateDetails(paramPenaltyAmount) {

    //$("#Honorarium_Amount_After_Deduction").val(parseFloat($("#Balance_Amount").val()) - parseFloat($("#PENALTY_AMOUNT").val()));
    //$("#TDS_AMOUNT").val(parseFloat((parseFloat($("#Honorarium_Amount_After_Deduction").val()) * parseFloat($("#Per_Tds").val())) / 100).toFixed(2));
    //$("#SC_AMOUNT").val(parseFloat((parseFloat($("#Honorarium_Amount_After_Deduction").val()) * parseFloat($("#Per_Sc").val())) / 100).toFixed(2));
    //$("#Amount_To_Be_Paid").val(parseFloat($("#Honorarium_Amount_After_Deduction").val()) - (parseFloat($("#TDS_AMOUNT").val()) + parseFloat($("#SC_AMOUNT").val()))).toFixed(2);

    var honoriumAmountAfterDeduction = parseFloat($("#Balance_Amount").val()) - parseFloat($("#PENALTY_AMOUNT").val());
    var tdsAmount = parseFloat((parseFloat(honoriumAmountAfterDeduction) * parseFloat($("#Per_Tds").val())) / 100);
    var scAmount = parseFloat((parseFloat(honoriumAmountAfterDeduction) * parseFloat($("#Per_Sc").val())) / 100);
    var serviceTaxAmount = parseFloat((parseFloat(honoriumAmountAfterDeduction) * parseFloat($("#Per_Service_Tax").val())) / 100);

    //alert(serviceTaxAmount);
    //alert(parseFloat(honoriumAmountAfterDeduction));
    //alert($("#Per_Service_Tax").val());

    var amounttobePaid = parseFloat(honoriumAmountAfterDeduction) - (parseFloat(tdsAmount) + parseFloat(scAmount));

    if ($('#Per_Service_Tax').val() != 0)
    {
        amounttobePaid = parseFloat(amounttobePaid) + (parseFloat(amounttobePaid) * parseFloat($('#Per_Service_Tax').val()) / 100)
    }

    $("#Honorarium_Amount_After_Deduction").val(honoriumAmountAfterDeduction.toFixed(2));
    $("#TDS_AMOUNT").val(tdsAmount.toFixed(2));
    $("#SC_AMOUNT").val(scAmount.toFixed(2));



    if ($("#ServiceTaxNo").val() == 0)
    {//if added by rohit 8Apr2015
       //var serviceTaxAmount = 0;
        $('#SERVICE_TAX_AMOUNT').val(serviceTaxAmount.toFixed(2));

    } else
    {

        $('#SERVICE_TAX_AMOUNT').val(serviceTaxAmount.toFixed(2));

    }

    // $("#Amount_To_Be_Paid").val(amounttobePaid.toFixed(2));


    //$("#Amount_To_Be_Paid").val((honoriumAmountAfterDeduction + serviceTaxAmount).toFixed(2));
    $("#Amount_To_Be_Paid").val((honoriumAmountAfterDeduction).toFixed(2));

    
}