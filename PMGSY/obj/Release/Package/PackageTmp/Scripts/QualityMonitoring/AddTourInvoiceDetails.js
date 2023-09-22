
//$.validator.addMethod("comparevalidationwithhonoriumamount", function (value, element, params) {

//    if (parseFloat($("#HONORARIUM_AMOUNT").val()) < parseFloat($("#PENALTY_AMOUNT").val())) {
//        return false;
//    }
//    else {
//        return true;
//    }
//});
//jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwithhonoriumamount");



$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmTourInvoice'));

    $("#btnSave").click(function (evt) {
        evt.preventDefault();
        //alert($("#MAST_TDS_ID").val());
        if ($('#frmTourInvoice').valid()) {
            if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined && parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
                CalculateDetails($("#PENALTY_AMOUNT").val());
            }
            $.ajax({
                url: "/QualityMonitoring/AddTourInvoiceDetails/",
                type: "POST",
                cache: false,
                data: $("#frmTourInvoice").serialize(),
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
                        $("#tbTourInvoiceList").trigger('reloadGrid');
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

    $("#PENALTY_AMOUNT").blur(function () {
        if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined && parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
            CalculateDetails($("#PENALTY_AMOUNT").val());
        }
    });

    $("#PENALTY_AMOUNT").focusout(function () {
        if ($("#PENALTY_AMOUNT").val() != "" && $("#PENALTY_AMOUNT").val() != NaN && $("#PENALTY_AMOUNT").val() != undefined && parseFloat($("#PENALTY_AMOUNT").val()) >= 0) {
            CalculateDetails($("#PENALTY_AMOUNT").val());
        }
    });

    $('#txtHonorariumAllowance').blur(function () {
        //txtDeduction
        if ($('#txtHonorariumAllowance').val() != NaN && $('#txtHonorariumAllowance').val() != "") {
            //alert($('#MAST_TDS').val());
            
            $('#txtDeduction').val(parseFloat($('#MAST_TDS').val()) * $('#txtHonorariumAllowance').val());
        }
    });

    $('#txtTravelClaim,#txtReportingAllowance,#txtMileageAllowance,#txtHoldingChargeAllowance,#txtDearnessAllowance,#txtHonorariumAllowance,#txtOtherAllowance,#txtDeduction,#txtOtherDeduction').blur(function () {
        
        var netPayable = 0;

        if ($('#txtTravelClaim').val() != NaN && $('#txtTravelClaim').val() != "" && $("#txtTravelClaim").val() != undefined && parseFloat($("#txtTravelClaim").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtTravelClaim').val());
        }
        if ($('#txtReportingAllowance').val() != NaN && $('#txtReportingAllowance').val() != "" && $("#txtReportingAllowance").val() != undefined && parseFloat($("#txtReportingAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtReportingAllowance').val());
        }
        if ($('#txtMileageAllowance').val() != NaN && $('#txtMileageAllowance').val() != "" && $("#txtMileageAllowance").val() != undefined && parseFloat($("#txtMileageAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtMileageAllowance').val());
        }
        if ($('#txtHoldingChargeAllowance').val() != NaN && $('#txtHoldingChargeAllowance').val() != "" && $("#txtHoldingChargeAllowance").val() != undefined && parseFloat($("#txtHoldingChargeAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtHoldingChargeAllowance').val());
        }
        if ($('#txtDearnessAllowance').val() != NaN && $('#txtDearnessAllowance').val() != "" && $("#txtDearnessAllowance").val() != undefined && parseFloat($("#txtDearnessAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtDearnessAllowance').val());
        }
        if ($('#txtHonorariumAllowance').val() != NaN && $('#txtHonorariumAllowance').val() != "" && $("#txtHonorariumAllowance").val() != undefined && parseFloat($("#txtHonorariumAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtHonorariumAllowance').val());
        }
        if ($('#txtOtherAllowance').val() != NaN && $('#txtOtherAllowance').val() != "" && $("#txtOtherAllowance").val() != undefined && parseFloat($("#txtOtherAllowance").val()) >= 0) {
            netPayable = netPayable + parseFloat($('#txtOtherAllowance').val());
        }
        if ($('#txtDeduction').val() != NaN && $('#txtDeduction').val() != "" && $("#txtDeduction").val() != undefined && parseFloat($("#txtDeduction").val()) >= 0) {
            netPayable = netPayable - parseFloat($('#txtDeduction').val());
        }
        if ($('#txtOtherDeduction').val() != NaN && $('#txtOtherDeduction').val() != "" && $("#txtOtherDeduction").val() != undefined && parseFloat($("#txtOtherDeduction").val()) >= 0) {
            netPayable = netPayable - parseFloat($('#txtOtherDeduction').val());
        }
        $('#txtNetPayable').val(netPayable);
    });

});

function CalculateDetails(paramPenaltyAmount) {

    //console.log('TOUR_EXPENDITURE= ' + parseFloat($("#TOUR_EXPENDITURE").val()));
    //console.log('PENALTY_AMOUNT= ' + parseFloat($("#PENALTY_AMOUNT").val()));

    var expenditureAmtAfterDeduction = parseFloat($("#TOUR_EXPENDITURE").val()) - parseFloat($("#PENALTY_AMOUNT").val());
    //console.log('expenditureAmtAfterDeduction= ' + expenditureAmtAfterDeduction);

    var tdsAmount = parseFloat((parseFloat(expenditureAmtAfterDeduction) * parseFloat($("#Per_Tds").val())) / 100);
    //console.log('tdsAmount= ' + tdsAmount);

    var scAmount = parseFloat((parseFloat(expenditureAmtAfterDeduction) * parseFloat($("#Per_Sc").val())) / 100);
    //console.log('scAmount= ' + scAmount);

    var serviceTaxAmount = parseFloat((parseFloat(expenditureAmtAfterDeduction) * parseFloat($("#Per_Service_Tax").val())) / 100);
    //console.log('serviceTaxAmount= ' + serviceTaxAmount);

    var amounttobePaid = parseFloat(expenditureAmtAfterDeduction) - (parseFloat(tdsAmount) + parseFloat(scAmount));
    //console.log('amounttobePaid= ' + amounttobePaid);


    if ($('#Per_Service_Tax').val() != 0) {
        amounttobePaid = parseFloat(amounttobePaid) + (parseFloat(amounttobePaid) * parseFloat($('#Per_Service_Tax').val()) / 100)
    }

    $("#TDS_AMOUNT").val(tdsAmount.toFixed(2));
    $("#SC_AMOUNT").val(scAmount.toFixed(2));

    if ($("#ServiceTaxNo").val() == 0) {//if added by rohit 8Apr2015
        $('#SERVICE_TAX_AMOUNT').val(serviceTaxAmount.toFixed(2));

    } else {
        $('#SERVICE_TAX_AMOUNT').val(serviceTaxAmount.toFixed(2));
    }
    $("#Amount_To_Be_Paid").val((expenditureAmtAfterDeduction).toFixed(2));
}