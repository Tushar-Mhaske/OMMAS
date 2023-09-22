
jQuery.validator.addMethod("drpRequired", function (value, element) {
    if (value == "")
    { return false; }
    else if ((parseFloat(value) == 0)) {
        return false;
    }
    else {
        return true;
    }

}, "");

jQuery.validator.addMethod("greaterThanZero", function (value, element) {
    return this.optional(element) || (parseFloat(value) > 0);
}, "Amount must be greater than zero");

//#region client side validation for add operation

jQuery.validator.addMethod("lessOrEqualToChequeAmountRemaining", function (value, element) {
    if (diffChqAmount < parseFloat(value)) {

        return false
    }
    else {

        return true;
    }

}, "amount  must be less than or equal to Cheque amount diffrence.");

jQuery.validator.addMethod("lessOrEqualToCashAmountRemaining", function (value, element) {


    if (DiffCachAmount < parseFloat(value)) {
        return false
    }
    else {
        return true;
    }

}, "amount must be less than or equal to Cash  amount diffrence.");

jQuery.validator.addMethod("lessOrEqualToChequeAmount", function (value, element) {


    if (parseFloat($("#AMOUNT_Q").val()) < parseFloat(value)) {
        return false
    }
    else {
        return true;
    }

}, "cash amount must be less than or equal to Cheque amount.");

jQuery.validator.addMethod("lessOrEqualToDedAmountRemaining", function (value, element) {


    if (DiffDedAmount < parseFloat(value)) {
        return false
    }
    else {
        return true;
    }

}, "amount must be less than or equal to deduction amount diffrence.");

//#endregion

//#region client side validation while edit operation
var valueAmt = 0;

jQuery.validator.addMethod("lessOrEqualToChequeAmountRemainingEdit", function (value, element) {

    //  alert(parseFloat(diffChqAmount) +" "+ parseFloat(ChqAmountToEdit));

    var totalAmt = parseFloat(diffChqAmount) + parseFloat(ChqAmountToEdit);

    //if new amount is less than or equal to current amount => valid new  amount 
    if (ChqAmountToEdit >= parseFloat(value)) {
        return true;
    }
    else {
        //if amount remaining + old amount is less than new amount => invalid new amount 
        if (totalAmt < parseFloat(value)) {
            return false;
        }
        else if (totalAmt >= parseFloat(value)) {
            //total amount equal to new amount valid amount
            return true;
        }
        else {
            return false;
        }
    }

}, "Cheque amount must be less than or equal to " + valueAmt);


jQuery.validator.addMethod("lessOrEqualToCashAmountRemainingEditForCashOnly", function (value, element) {

    var totalAmt = parseFloat(DiffCachAmount) + parseFloat(ChqAmountToEdit) //why becouse we are putting  cash amount in cheque amount textbox when transaction is of only cash


    if (CashAmountToEdit >= parseFloat(value)) {
        return true;
    }
    else {
        if (totalAmt < parseFloat(value)) {
            return false;
        }
        else if (totalAmt >= parseFloat(value)) {
            return true;

        }
        else {
            return false;
        }
    }

}, "Cash amount must be less than or equal to " + valueAmt);


jQuery.validator.addMethod("lessOrEqualToCashAmountRemainingEdit", function (value, element) {

    var totalAmt = parseFloat(DiffCachAmount) + parseFloat(CashAmountToEdit)

    //alert(totalAmt + "totalAmt");
    //alert(parseFloat(value));

    if (CashAmountToEdit >= parseFloat(value)) {

        return true;
    }
    else {
        if (totalAmt < parseFloat(value)) {
            return false;
        }
        else if (totalAmt >= parseFloat(value)) {

            return true;

        }
        else {

            return false;
        }
    }

}, "Cash amount must be less than or equal to " + valueAmt);


jQuery.validator.addMethod("lessOrEqualToDedAmountRemainingEdit", function (value, element) {

    var totalAmt = parseFloat(DiffDedAmount) + parseFloat(dedAmountToEdit)
    //alert(DiffDedAmount);
    //alert(totalAmt);
    //alert(dedAmountToEdit);
    //alert(value);
    if (dedAmountToEdit >= parseFloat(value)) {
        return true;
    }
    else {
        if (totalAmt < parseFloat(value)) {
            return false;
        }
        else if (totalAmt >= parseFloat(value)) {
            return true;

        }
        else {
            return false;
        }
    }

}, "Deduction amount must be less than or equal to " + valueAmt);


//added by Koustubh Nakate on 27/09/2013 for agreement and contractor required validation

jQuery.validator.addMethod("iscontractorequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == 0) {
        return false;
    }
    else {
        return true;
    }

}, "");




jQuery.validator.addMethod("isagreementrequired", function (value, element) {
    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == 0) {
        return false;
    }
    else {
        return true;
    }

}, "");

jQuery.validator.addMethod("isagreementrequired_s", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == 0) {
        return false;
    }
    else {
        return true;
    }

}, "");



jQuery.validator.addMethod("issanctionedyearrequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == 0) {
        return false;
    }
    else {
        return true;
    }

}, "");

jQuery.validator.addMethod("ispackagerequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == 0) {
        return false;
    }
    else {
        return true;
    }

}, "");

//#endregion


var TriggerWhenError = false
var changeNarrationPay = false; //variable to keep track of whther to populate narration of payment head
var changeNarrationDed = false;//variable to keep track of whther to populate narration of deduction head
var showContractorNarration = false; //variable to keep track if narration is to be set true when contractor suplier required 
var getContractorNameFromMasterEntry = true;//for narration if contractor name is to be taken fm master payment entry
var isValid = false;
var voucherPayment;

var _MasterAgreementRequired;//Added By Abhishek kamble 25Nov2014 to use hide / show Agreement on deduction side 


$(document).ready(function () {

    //alert("a" +  parseFloat(TotalAmtToEnterDedAmount));

   
    

    //added by Abhishek kamble 2-jan-2013    
    if (($("#AMOUNT_Q").val() == "") && IsMultiTranAllowed == "N") {


        if (DeductionRequired == "True") {
            $("#AMOUNT_Q").val(ChequeAmount);
        }
        else if (DeductionRequired == "False") {
            $("#AMOUNT_Q").val(ReceiptGrossAmount);
        }
    }

    var isAgreementRequired = false;
    var isContractorRequired = false;
    var isSanctionedYearRequired = false;
    var isPackageRequired = false;

    var isAgreementRequired_s = false;

    //to close the epayment dialog
    $('#PaymentEpaydialog').live("dialogclose", function () {


        $('#PaymentEpaydialog').dialog('destroy');
        blockPage();
        $("#mainDiv").load("/payment/GetPaymentList/", function () {
            unblockPage();
            return false;
        });

    });

    //to close the eremittance dialog
    $('#PaymentEremDialog').live("dialogclose", function () {

        $('#PaymentEremDialog').dialog('destroy');
        blockPage();
        $("#mainDiv").load("/payment/GetPaymentList/", function () {
            unblockPage();
            return false;
        });

    });



    //to keep track of whether remove all validation error on payment transaction type change
    var roadRequired = false; //variable to keep track if road is required ;so as to get the final payment status

    $.validator.unobtrusive.parse($("#PaymentTransactionForm"));

    $.validator.unobtrusive.parse($("#DeductionTransactionForm"));

    //event to reset the payment form
    $("#btnPaymentReset").click(function () {

        TriggerWhenError = false;

        //$("#HEAD_ID_P").trigger('change');

        $(':input', '#PaymentTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

        clearValidation($("#PaymentTransactionForm"));

        $("#btnPaymentUpdate").hide();
        $("#btnPaymentSubmit").show();

        //new change done by Vikram 
        //$("#HeadDescTr").hide('slow');
        if (!$("#HEAD_ID_P").is(':disabled')) {
            $("#HeadDescTr").hide('slow');
        }
        //end of change


        if ($('#IMS_AGREEMENT_CODE_C').is(':disabled')) {

            $('#IMS_AGREEMENT_CODE_C').attr('disabled', false);

            $('#IMS_AGREEMENT_CODE_C').val('0');
        }

        setTimeout(function () {
            if ($("#HEAD_ID_P").is(':disabled')) {
                $("#HEAD_ID_P").change();
            }

        }, 100);

        if ($("#divDetailsError").is(':visible')) {
            $("#divDetailsError").html('');
            $("#divDetailsError").hide('slow');
        }

    });

    //event to cancel the payment form
    $("#btnPaymentCancel").click(function () {

        //commeneted by Koustubh Nakate on 23/09/2013 
        /*  TriggerWhenError = false;
  
          //$("#HEAD_ID_P").trigger('change');
  
          $(':input', '#PaymentTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
  
          clearValidation($("#PaymentTransactionForm"));
  
          $("#btnPaymentUpdate").hide();
          $("#btnPaymentCancel").hide();
          $("#btnPaymentSubmit").show();
          $("#btnPaymentReset").show();
          $("#HeadDescTr").hide('slow');
  
          $("#HEAD_ID_P > option").each(function () {
  
              if ($(this).attr('class') == "X") {
                  $(this).remove();
              }
          });*/

        //added by Koustubh Nakate on 23/09/2013

        var urlParam = $('#PaymentMasterList').getDataIDs()[0];

        //added by Koustubh Nakate on 28/10/2013 to avoid async ajax call


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/Payment/PartialTransaction/' + urlParam,
            async: false,
            cache: false,
            success: function (data) {


                $('#PaymentDeductionData').html(data);

                $("#TransactionForm").show('slow');

                //show transaction form
                $("#trnsShowtable").show('slow');

                if ($("#HEAD_ID_P").is(':disabled')) {

                    $("#HEAD_ID_P").trigger('change');
                }

                if (parseFloat(TotalAmtToEnterDedAmount) == 0) {

                    $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                    $("#cashAmtTr").hide();
                }
                else {
                    $("#AMOUNT_C").val(0).removeAttr("readonly");
                    $("#cashAmtTr").show();
                }


                // $("#tblOptions").hide('slow');
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                // $('#PaymentDeductionData').html('');
                alert('Error while loading transaction details.');
                $.unblockUI();
            }


        });
        //end added by Koustubh Nakate on 28/10/2013 to avoid async ajax call


        //load payment and deduction form
        /* $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + urlParam, function () {
 
 
             $("#TransactionForm").show('slow');
 
             //show transaction form
             $("#trnsShowtable").show('slow');
 
 
 
             if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                 $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                 $("#cashAmtTr").hide();
             }
             else {
                 $("#AMOUNT_C").val(0).removeAttr("readonly");
                 $("#cashAmtTr").show();
             }
 
 
             //new change done by Vikram 
             // when details contain only one head and after insertion of first data,the head and agreement get disabled but on click on cancel the agreement dropdown is not shown
             if ($("#HEAD_ID_P").is(":disabled"))
             {
                 $("#HEAD_ID_P").trigger('change');
             }
 
         });*/

    });

    //event to reset the deduction form
    $("#btnDeductionReset").click(function () {

        clearValidation($("#DeductionTransactionForm"));

        $(':input', '#DeductionTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

        $("#btnDeductionUpdate").hide();
        $("#btnDeductionSubmit").show();
        $("#headDescDedTR").hide('slow');

        if ($('#IMS_AGREEMENT_CODE_DED').is(':disabled')) {

            $('#IMS_AGREEMENT_CODE_DED').attr('disabled', false);

            $('#IMS_AGREEMENT_CODE_DED').val('0');

        }

        setTimeout(function () {
            if ($("#HEAD_ID_D").is(':disabled')) {
                $("#HEAD_ID_D").change();
            }

        }, 100);

    });

    //event to reset the deduction form
    $("#btnDeductionCancel").click(function () {

        //commeneted by Koustubh Nakate on 23/09/2013 

        /*  clearValidation($("#DeductionTransactionForm"));
  
          $(':input', '#DeductionTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
  
          $("#btnDeductionUpdate").hide();
          $("#btnDeductionCancel").hide();
          $("#btnDeductionSubmit").show();
          $("#btnDeductionReset").show();
          $("#headDescDedTR").hide('slow');
          //for invalid trnasaction type
          $("#HEAD_ID_D > option").each(function () {
  
              if ($(this).attr('class') == "X") {
                  $(this).remove();
              }
          });*/


        //added by Koustubh Nakate on 28/10/2013 to avoid async ajax call

        var urlParam = $('#PaymentMasterList').getDataIDs()[0];
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/Payment/PartialTransaction/' + urlParam,
            async: false,
            cache: false,
            success: function (data) {

                $('#PaymentDeductionData').html(data);

                $("#TransactionForm").show('slow');

                //show transaction form
                $("#trnsShowtable").show('slow');

                if ($("#HEAD_ID_D").is(':disabled')) {

                    $("#HEAD_ID_D").trigger('change');
                }

                if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                    $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                    $("#cashAmtTr").hide();
                }
                else {
                    $("#AMOUNT_C").val(0).removeAttr("readonly");
                    $("#cashAmtTr").show();
                }


                // $("#tblOptions").hide('slow');
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                // $('#PaymentDeductionData').html('');
                alert('Error while loading transaction details.');
                $.unblockUI();
            }


        });

        //added by Koustubh Nakate on 23/09/2013

        /*   var urlParam = $('#PaymentMasterList').getDataIDs()[0];
           //load payment and deduction form
           $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + urlParam, function () {
   
   
               $("#TransactionForm").show('slow');
   
               //show transaction form
               $("#trnsShowtable").show('slow');
   
   
   
               if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                   $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                   $("#cashAmtTr").hide();
               }
               else {
                   $("#AMOUNT_C").val(0).removeAttr("readonly");
                   $("#cashAmtTr").show();
               }
           });
           */
    });

    //to hide shoe transaction form
    $('#HideShowTransForm').click(function () {

        $("#PaymentTransactionForm").toggle('slow', function () { });
        $("#DeductionTransactionForm").toggle('slow', function () { });

        $('#iconSpan2').toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");

    });

    //populate roads based on the contractor aggrement
    $('#IMS_AGREEMENT_CODE_C').change(function () {
        //alert($('#IMS_AGREEMENT_CODE_C').val());
        if ($('#IMS_AGREEMENT_CODE_C').val() != 0 && $('#IMS_AGREEMENT_CODE_C').val() != "" && $('#IMS_AGREEMENT_CODE_C').val() != null) {//&& $('#IMS_AGREEMENT_CODE_C').val() != null) {

            //alert('a');
            $("#IMS_PR_ROAD_CODE").empty(); //change done by Vikram

            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoad/" + $("#IMS_AGREEMENT_CODE_C").val() + "$" + $("#HEAD_ID_P").val() + "$" + $("#CONTRACTOR_ID").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#IMS_AGREEMENT_CODE_C option:selected").text() }));

            $('#IMS_AGREEMENT_CODE_DED').attr("disabled", "disabled").val($('#IMS_AGREEMENT_CODE_C').val())

            $("#agreementSpan").text($('#IMS_AGREEMENT_CODE_C :selected').text().trim());
        }
        else {
            $("#agreementSpan").text("");
        }

        SetNarration();
    });


    ////New code - to add bank Gurantee validations 
    //$('#IMS_AGREEMENT_CODE_C').change(function () {

    //    if ($('#IMS_AGREEMENT_CODE_C').val() != 0 && $('#IMS_AGREEMENT_CODE_C').val() != "" && $('#IMS_AGREEMENT_CODE_C').val() != null) {//&& $('#IMS_AGREEMENT_CODE_C').val() != null) {
    //        //GetContractorBankGuranteeDetails($('#IMS_AGREEMENT_CODE_C').val(), $("#fundType").val(), 'A');
    //        //Added By Bhushan
    //        //  GetContractorBankGuranteeDetails($('#IMS_AGREEMENT_CODE_C').val(), $("#fundType").val(), 'A', $('#chq_amt').data("value"));

    //    //    GetContractorBankGuranteeDetails($('#IMS_AGREEMENT_CODE_C').val(), $("#fundType").val(), 'A' );
    //     }
    //    else {
    //        $("#agreementSpan").text("");
    //    }

    //    SetNarration();
    //});

    
    function VerifyDPRAgreement(contractorCode, agreementCode, fundType) {
        //alert($("#TXNId").val());
        var FlagSuccess;
        blockPage();
        $.ajax({
            type: "POST",
            url: "/payment/VerifyDPRAgreement/" + contractorCode + '$' + agreementCode + '$' + fundType + '$' + $("#TXNId").val(),
            async: false,
            error: function (xhr, status, error) {
                unblockPage();
                return false;
            },
            success: function (data) {
                unblockPage();
                if (data.Success == true) {
                    FlagSuccess = true;
                    return true;
                }
                else if (data.Success == false) {
                    unblockPage();
                    FlagSuccess = false; 
                }
                else {
                        unblockPage();
                        FlagSuccess = false;
                        alert('An error occured while processing your request');
                }
            }
        });
        return FlagSuccess;
    }

    function GetContractorBankGuranteeDetails(agreementCode, fundType, callType) {

        var FlagSuccess;
  
        
        if (parseFloat(ChequeAmount) == 0) {
            return true;

        }

             
        if ((parseFloat($("#LevelID").val()) == 4) || ((fundType == 'P') && (MasterTxnId == 105 || MasterTxnId == 109 || MasterTxnId == 120 || MasterTxnId == 86 || MasterTxnId == 118 || MasterTxnId == 134 || MasterTxnId == 137 || MasterTxnId == 1488 || MasterTxnId == 1546 || MasterTxnId == 1553 || MasterTxnId == 1661 || MasterTxnId == 2000 || MasterTxnId == 42 || MasterTxnId == 113 || MasterTxnId == 1485 || MasterTxnId == 1814 || MasterTxnId == 1997))) {
            return true;
        }
        else {

            $.ajax({
                type: "POST",
                url: "/payment/GetContractorBankGuranteeDetails/" + agreementCode + '$' + fundType + '$' + MasterTxnId,

                async: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    // alert(data.Success);
                    if (data.Success == true) {
                        //  alert('in success');
                        FlagSuccess = true;
                        if (callType == 'A') {
                            $("#IMS_PR_ROAD_CODE").empty(); //change done by Vikram
                            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoad/" + $("#IMS_AGREEMENT_CODE_C").val() + "$" + $("#HEAD_ID_P").val() + "$" + $("#CONTRACTOR_ID").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#IMS_AGREEMENT_CODE_C option:selected").text() }));
                            $('#IMS_AGREEMENT_CODE_DED').attr("disabled", "disabled").val($('#IMS_AGREEMENT_CODE_C').val())
                            $("#agreementSpan").text($('#IMS_AGREEMENT_CODE_C :selected').text().trim());
                        }
                        return true;
                    }
                    else if (data.Success == false) {
                        unblockPage();
                        FlagSuccess = false;
                        alert(data.ErrorMessage);

                    }
                    else {
                        unblockPage();
                        FlagSuccess = false;
                        alert('An error occured while processing your request');

                    }
                }
            });
            return FlagSuccess;
        }
     
    }


    //populate roads based on the supplier aggrement
    $('#IMS_AGREEMENT_CODE_S').change(function () {
        if ($('#IMS_AGREEMENT_CODE_S').val() != 0 || $('#IMS_AGREEMENT_CODE_S').val() != "") {
            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoad/" + $("#IMS_AGREEMENT_CODE_S").val());
            $('#IMS_AGREEMENT_CODE_DED').attr("disabled", "disabled").val($('#IMS_AGREEMENT_CODE_S').val())
            $("#agreementSpan").text($('#IMS_AGREEMENT_CODE_S :selected').text().trim());
        }
        else {
            $("#agreementSpan").text("");
        }
        SetNarration();

    });

    //chage event of road code
    $('#IMS_PR_ROAD_CODE').change(function () {
        // debugger;
        //alert("line No-762: " + Bill_ID);

        $.ajax({
            type: "POST",
            //url: "/payment/GetQCRRecordStatus/?roadCode=" + $('#IMS_PR_ROAD_CODE').val() ,
            url: "/payment/GetQCRRecordStatus/" + Bill_ID +"/?roadCode="+$('#IMS_PR_ROAD_CODE').val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
            },
            success: function (result1) {
                unblockPage();
                //alert(result1.status);
                if (result1.status == false) {
                    $("#divDetailsError").show("slide");
                    $("#divDetailsError span:eq(1)").html('<strong>Alert:QCR details not uploaded against selected road .Hence Payment cannot be done. </strong>');
                    $('#IMS_PR_ROAD_CODE').val(0);

                    alert("QCR details not uploaded against selected road . Hence Payment cannot be done.");

                    //$('#divError').show('slow');
                    //$('#errorSpan').show();
                    //alert("QCR details not uploaded against selected road . Hence Payment cannot be done.");
                    //$('#errorSpan').text("Alert : QCR details not uploaded against selected road .Hence Payment cannot be done.");
                    //$('#IMS_PR_ROAD_CODE').val(0);
                }
                else
                {
                    //Add Validation For VTS start

                    $.ajax({

                        type: "POST",

                        url: "/payment/GetVTSValidationStatus/" + Bill_ID + "/?roadCode=" + $('#IMS_PR_ROAD_CODE').val(),
                        async: false,

                        error: function (xhr, status, error) {
                            unblockPage();
                            $('#divError').show('slow');
                            $('#errorSpan').text(xhr.responseText);
                        },
                        success: function (result1) {
                            if (result1.status == false) {
                                $("#divDetailsError").show("slide");
                                $("#divDetailsError span:eq(1)").html('<strong>Alert:Work freeze due to GPS/VTS Analysis not uploaded. </strong>');
                                $('#IMS_PR_ROAD_CODE').val(0);

                                Alert("Work freeze due to GPS/VTS Analysis not uploaded.");
                                return false;
                            } else {
                                $('#divDetailsError').hide('slow');

                                if (roadRequired && $('#IMS_PR_ROAD_CODE').val() != 0) {

                                    if (Bill_ID == 0 || Bill_ID == null) {

                                        Bill_ID = $("#Bill_ID").val();
                                    }

                                    FillInCascadeDropdown(null, "#FINAL_PAYMENT", "/Payment/GetFinalPaymentDetails/" + Bill_ID + "/" + $("#IMS_PR_ROAD_CODE").val());

                                    var subtran = $("#HEAD_ID_P option:selected").val();
                                    if ($("#HEAD_ID_P option:selected").val() == "1975$Q" || $("#HEAD_ID_P option:selected").val() == "1976$Q" || $("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q" || $("#HEAD_ID_P option:selected").val() == "1543$Q" || $("#HEAD_ID_P option:selected").val() == "1464$Q" || $("#HEAD_ID_P option:selected").val() == "1465$Q") {
                                        $(".final").show('slow');
                                    }
                                    else {
                                        $(".final").hide();
                                    }
                                    //Avinash
                                    if (TXNId == 1788 || TXNId == 1484 || TXNId == 1974 || TXNId == 47) {
                                        $(".final").show();
                                    }


                                    $("#roadSelectedSpan").text($('#IMS_PR_ROAD_CODE :selected').text().trim());


                                }
                                else {
                                    $(".final").hide();
                                    $("#roadSelectedSpan").text("");
                                }
                                SetNarration();
                                altRowStyle();
                            }
                        }
                    });

                    //end
                    //$('#divDetailsError').hide('slow');

                    //if (roadRequired && $('#IMS_PR_ROAD_CODE').val() != 0) {

                    //    if (Bill_ID == 0 || Bill_ID == null) {

                    //        Bill_ID = $("#Bill_ID").val();
                    //    }

                    //    FillInCascadeDropdown(null, "#FINAL_PAYMENT", "/Payment/GetFinalPaymentDetails/" + Bill_ID + "/" + $("#IMS_PR_ROAD_CODE").val());

                    //    var subtran = $("#HEAD_ID_P option:selected").val();
                    //    if ($("#HEAD_ID_P option:selected").val() == "1975$Q" || $("#HEAD_ID_P option:selected").val() == "1976$Q" || $("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q" || $("#HEAD_ID_P option:selected").val() == "1543$Q" || $("#HEAD_ID_P option:selected").val() == "1464$Q" || $("#HEAD_ID_P option:selected").val() == "1465$Q") {
                    //        $(".final").show('slow');
                    //    }
                    //    else {
                    //        $(".final").hide();
                    //    }
                    //    //Avinash
                    //    if (TXNId == 1788 || TXNId == 1484 || TXNId == 1974 || TXNId == 47) {
                    //        $(".final").show();
                    //    }


                    //    $("#roadSelectedSpan").text($('#IMS_PR_ROAD_CODE :selected').text().trim());


                    //}
                    //else {
                    //    $(".final").hide();
                    //    $("#roadSelectedSpan").text("");
                    //}
                    //SetNarration();
                    //altRowStyle();
                }
                
            }
        });


        //if (roadRequired && $('#IMS_PR_ROAD_CODE').val() != 0) {

        //    if (Bill_ID == 0 || Bill_ID == null) {

        //        Bill_ID = $("#Bill_ID").val();
        //    }

        //    FillInCascadeDropdown(null, "#FINAL_PAYMENT", "/Payment/GetFinalPaymentDetails/" + Bill_ID + "/" + $("#IMS_PR_ROAD_CODE").val());

        //    var subtran = $("#HEAD_ID_P option:selected").val();
        //    if ($("#HEAD_ID_P option:selected").val() == "1975$Q" || $("#HEAD_ID_P option:selected").val() == "1976$Q" || $("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q" || $("#HEAD_ID_P option:selected").val() == "1543$Q" || $("#HEAD_ID_P option:selected").val() == "1464$Q" || $("#HEAD_ID_P option:selected").val() == "1465$Q") {
        //        $(".final").show('slow');
        //    }
        //   else {
        //        $(".final").hide();
        //    }
        //    //Avinash
        //    if (TXNId == 1788 || TXNId == 1484 || TXNId == 1974 || TXNId == 47) {
        //       $(".final").show();
        //    }


        //    $("#roadSelectedSpan").text($('#IMS_PR_ROAD_CODE :selected').text().trim());


        //}
        //else {
        //    $(".final").hide();
        //    $("#roadSelectedSpan").text("");
        //}
        //SetNarration();
        //altRowStyle();

    });

    //if cash amount is 0 disable it
    if (parseFloat(TotalAmtToEnterChqAmount) == 0) {

        //Commented by Abhishek kamble 21-June-2014 for disp cash amount for deduction only txn 
        //$("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        $("#cashAmtTr").hide();
    }
    else {
        $("#AMOUNT_C").removeAttr("readonly");
        $("#cashAmtTr").show();
    }

    //deduction head chage event
    $("#HEAD_ID_D").change(function () {

        //alert($('#BILL_DATE').val());

        if ($("#HEAD_ID_D").val() == 0) {
            $("#headDescDedTR").hide('slow');
            return false;
        }

        if (Bill_ID == 0 || Bill_ID == null || Bill_ID == "") {
            Bill_ID = $("#Bill_ID").val();
        }


        $.ajax({
            type: "POST",
            url: "/payment/GetHeadDetails/" + Bill_ID + "/" + $("#HEAD_ID_D").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
            },
            success: function (result1) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (result1 != "") {
                    var resultData = result1.split('$');
                    $("#descDedTd").text(resultData[1]).parent().show('slow').show('slow').css("font-weight", "bold").css("text-align", "center");

                    if (changeNarrationDed == true || (!transEdit)) {
                        $("#NARRATION_D").text(resultData[0]);
                    }

                }
            }
        });

        //get head transaction details        
        if ($("#LevelID").val() == 5) {

            //Added By Abhishek kamble to hide agreement Details for txn Expenditure on RRTRC/RRNMU start
            //If condition added by Abhishek 24Nov2014
            //if ((parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1491) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1492) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1493)) {

            //this ajax call added by Abhishek to hide / show agreement ddl 25Nov2014            
            $.ajax({
                type: "POST",
                url: "/payment/GetMasterDesignParams/" + MasterTxnId,
                data: { billDate: $('#BILL_DATE').val() },//Added on 27-07-2022
                async: false,
                // data: $("#authSigForm").serialize(),
                error: function (xhr, status, error) {
                    // unblockPage();
                    $.unblockUI();
                    $('#divError').show('slow');
                    $('#errorSpan').text("Error While Getting transaction design parameters");
                },
                success: function (data) {

                    if (data.AgreementRequired == "Y" && Bill_ID != null && Bill_ID != "") {
                        _MasterAgreementRequired = "Y";
                        //Original ajax call
                        $.ajax({
                            type: "POST",
                            url: "/payment/GetSelectedAgreementForTransaction/" + Bill_ID,
                            async: false,
                            // data: $("#authSigForm").serialize(),
                            error: function (xhr, status, error) {
                                unblockPage();
                                $('#divError').show('slow');
                                $('#errorSpan').text("Error While Getting agreement details");

                            },
                            success: function (data1) {
                                unblockPage();
                                $('#divError').hide('slow');
                                $('#errorSpan').html("");

                                if (data1 != "") {
                                    if ($('#FundType').val() != 'A') { //added by Koustubh Nakate on 07/10/2013 for admin fund 
                                        $(".AgreementDed").show();
                                    }
                                    $('#IMS_AGREEMENT_CODE_DED').val(data1);
                                    if ($("#IMS_AGREEMENT_CODE_DED") != null) {
                                        $("#IMS_AGREEMENT_CODE_DED").attr('disabled', 'disabled');

                                        if (!$('#IMS_AGREEMENT_CODE_DED').is("readonly")) {
                                            $('#IMS_AGREEMENT_CODE_DED').rules('add', {
                                                required: true,
                                                messages: {
                                                    required: 'Agreement Name (Deduction) is required'
                                                }
                                            });
                                        }
                                    }

                                }
                                else {
                                    if ($('#FundType').val() != 'A') { //added by Koustubh Nakate on 07/10/2013 for admin fund 
                                        $(".AgreementDed").show();
                                    }
                                    $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');
                                    if (!$('#IMS_AGREEMENT_CODE_DED').is("readonly")) {
                                        $('#IMS_AGREEMENT_CODE_DED').rules('add', {
                                            required: true,
                                            messages: {
                                                required: 'Agreement Name (Deduction) is required'
                                            }
                                        });
                                    }
                                }
                            }
                        });


                    }
                    else {
                        $(".AgreementDed").hide("slow");
                        _MasterAgreementRequired = "N";
                    }
                }
            });



            // } 
            //Added By Abhishek kamble to hide agreement Details for txn Expenditure on RRTRC/RRNMU end



        }
        $("#deductionTable").removeClass('rowstyle').addClass('rowstyle');

    });

    //populate the sanction packagge based on sanction year
    $("#SANCTION_YEAR").change(function () {

        if ($("#SANCTION_YEAR").val() != 0) {
            //FillInCascadeDropdown(null, "#SANCTION_PACKAGE", "/Payment/PopulateSactionPackage/" + $("#SANCTION_YEAR").val());
            FillInCascadeDropdown(null, "#SANCTION_PACKAGE", "/Payment/PopulateSactionPackage/" + $("#SANCTION_YEAR").val() + "$" + $("#HEAD_ID_P").val());//new change done by Vikram

            $(".sanctionPackage").show();
            altRowStyle()
        } else {//modified by abhishek kamble 11-nov-2013
            $('#SANCTION_PACKAGE').empty();
            $('#SANCTION_PACKAGE').append("<option value=0>--Select--</option>");
        }

    });

    //populate the road based on sanction package
    $("#SANCTION_PACKAGE").change(function () {

        if ($("#SANCTION_PACKAGE").val() != 0) {
            //FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoadbyPackageYear/" + $("#SANCTION_PACKAGE").val());

            $("#IMS_PR_ROAD_CODE").empty();//change done by Vikram

            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoadbyPackageYear/" + $("#SANCTION_PACKAGE").val() + "$" + $("#HEAD_ID_P").val()); //new change done by Vikram

            $(".road").show();
            altRowStyle()
        } else {//modified by abhishek kamble 11-nov-2013
            $('#IMS_PR_ROAD_CODE').empty();
            $('#IMS_PR_ROAD_CODE').append("<option value=0>--Select--</option>");
        }


    });

    //for invalid trnasaction type
    $("#HEAD_ID_P > option").each(function () {

        if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            //$(this).addClass("X");
            //$(this).css("color", "#b83400");

        }
    });

    //for invalid trnasaction type
    $("#HEAD_ID_D > option").each(function () {

        if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            //$(this).addClass("X");
            //$(this).css("color", "#b83400");

        }
    });

    //change event of contractor which is applicable for Statutory Deductions/State Government populate the contractor
    $("#MAST_CON_ID_CON").change(function () {

        if ($("#MAST_CON_ID_CON").val() != 0 && $("#MAST_CON_ID_CON").val() != null) {


            if (!($("#HEAD_ID_P option:selected").val() == "3042$Q" || $("#HEAD_ID_P option:selected").val() == "3045$Q" || $("#HEAD_ID_P option:selected").val() == "3046$Q" || $("#HEAD_ID_P option:selected").val() == "3047$Q" || $("#HEAD_ID_P option:selected").val() == "3048$Q")) {
                FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_C", "/Payment/GetAgreementForContractor/" + $("#MAST_CON_ID_CON").val());

                $('.conAgreement').show('slow');

                $('#IMS_AGREEMENT_CODE_C').rules('add', {
                    required: true,
                    messages: {
                        required: 'Agreement Name is Required'
                    }
                });

                $("#CompanyNameSpan").text($("#MAST_CON_ID_CON :selected").text().trim());
            }

        }
        else {
            $("#IMS_AGREEMENT_CODE_C").empty();
            $("#IMS_AGREEMENT_CODE_C").append("<option value='0'>--Select--</option>");
            $("#CompanyNameSpan").text("");
        }

        SetNarration();

    });

    //payment transaction change event
    $("#HEAD_ID_P").change(function () {


        //change by Koustubh Nakate on 23/09/2013 for reset all details in edit mode  
        if ($("#HEAD_ID_P").val() == 0 && $('#btnPaymentSubmit').is(':visible')) {
            $("#HeadDescTr").hide('slow');
            $('.conAgreement').hide('slow');
            $('.piu').hide('slow');
            $('.road').hide('slow');
            $('.supAgreement').hide('slow');
            // resetDetailsForm();
            return false;
        }
        else if ($("#HEAD_ID_P").val() == 0 && $('#btnPaymentUpdate').is(':visible')) {
            return false;
        }

        //change don by Vikram on 26 March 2014 -- in edit mode all (new / upgrade roads were populated)
        $("#IMS_AGREEMENT_CODE_C").val('0');

        // for showing final payment option to only Construction of New Works and Upgradation of New Works

        //alert("Working1");
        if ($("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q" || $("#HEAD_ID_P option:selected").val() == "1543$Q" ) {
            //alert("Working");
            $(".final").show('slow');
        }
        else {

            $(".final").hide();
        }

        //Avinash
        if (TXNId == 1788 || TXNId == 1484 || TXNId == 1974 || TXNId == 47) {
            $(".final").show();
        }
        //alert(Bill_ID);
        blockPage();
        $.ajax({
            type: "POST",
            url: "/payment/getDetailsDesignParam/" + $("#HEAD_ID_P").val(),
            async: false,
            data: { billId: Bill_ID },
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                // unblockPage();
                $.unblockUI();
                $('#divError').show('slow');
                $('#errorSpan').text("Error While Getting transaction design parameters");

            },
            success: function (data) {


                //unblockPage();
                $.unblockUI();
                $('#divError').hide('slow');
                $('#errorSpan').html("");

                if (TriggerWhenError == false) {
                    clearValidation($("#PaymentTransactionForm"));
                }
                if (data != "") {

                    //for Remittance Of Statutory Deductions/State Government populate the contractor
                    if (data.ShowContractor) {
                        if ($("#MAST_CON_ID_CON").val() == 0 || $("#MAST_CON_ID_CON").val() == "") {
                            if ($('#MAST_CON_ID_CON  option').length == 1) {
                                FillInCascadeDropdown(null, "#MAST_CON_ID_CON", "/Payment/GetContractor/");
                            }

                        }
                        $('.conAgreement').show('slow');
                        $("#conTractorTR").show();
                        showContractorNarration = true;
                        getContractorNameFromMasterEntry = false;

                        isContractorRequired = true;
                    }
                    else {
                        $("#conTractorTR").hide();
                        showContractorNarration = false;
                    }

                    if (!($("#HEAD_ID_P option:selected").val() == "3042$Q" || $("#HEAD_ID_P option:selected").val() == "3045$Q" || $("#HEAD_ID_P option:selected").val() == "3046$Q" || $("#HEAD_ID_P option:selected").val() == "3047$Q" || $("#HEAD_ID_P option:selected").val() == "3048$Q")) {
                      


                        if (data.SupplierRequired == "Y") {

                            showContractorNarration = true;

                            if ($("#IMS_AGREEMENT_CODE_S").val() == 0 || $("#IMS_AGREEMENT_CODE_S").val() == "") {
                                if ($('#IMS_AGREEMENT_CODE_S  option').length == 1) {
                                    FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_S", "/Payment/GetSupplierContractorAgreement/" + Bill_ID);
                                }
                            }

                            $('.supAgreement').show('slow');

                            $('#IMS_AGREEMENT_CODE_S').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Agreement Name is Required'
                                }
                            });
                        }
                        else {
                            $('.supAgreement').hide('slow');
                            showContractorNarration = false;
                        }
                    }

                   

                    if (data.RoadRequired == "Y") {

                        var arr = $("#HEAD_ID_P").val().split('$');
                        $('.road').show('slow');
                        roadRequired = true;

                        if (arr[0] == 90) {
                            //alert('aac');
                            roadRequired = true;
                            //$('.road').hide('slow');
                        }
                        else {
                            //$('#IMS_PR_ROAD_CODE').rules('add', {
                            //    drpRequired: true,
                            //    messages: {
                            //        drpRequired: 'Road is Required'
                            //    }
                            //});
                        }
                    }
                    else {
                        roadRequired = false;
                        $('.road').hide('slow');
                        //get if final payment drodown details 

                    }

                    if (data.PiuRequired == "Y") {


                        if ($("#MAST_DPIU_CODE").val() == 0 || $("#MAST_DPIU_CODE").val() == "") {

                            if ($('#MAST_DPIU_CODE  option').length == 1) {//|| $('#MAST_DPIU_CODE  option').length == 0) {
                                if (urlparamsForTransEdit == "") {
                                    FillInCascadeDropdown(null, "#MAST_DPIU_CODE", "/Payment/GetPIU/" + Bill_ID);
                                }
                                else {
                                    FillInCascadeDropdown(null, "#MAST_DPIU_CODE", "/Payment/GetPIU/" + urlparamsForTransEdit);
                                }
                            }
                        }

                        $('.piu').show('slow');

                        $('#MAST_DPIU_CODE').rules('add', {
                            required: true,
                            messages: {
                                required: 'DPIU is Required'
                            }
                        });

                    }
                    else {
                        $('.piu').hide('slow');
                    }

                   

                    //if contractor required at master payment and transaction payment also
                    if (data.ContractorRequired == "Y" && data.ShowContractor == false) {

                        showContractorNarration = true;

                        if (!($("#HEAD_ID_P option:selected").val() == "3042$Q" || $("#HEAD_ID_P option:selected").val() == "3045$Q" || $("#HEAD_ID_P option:selected").val() == "3046$Q" || $("#HEAD_ID_P option:selected").val() == "3047$Q" || $("#HEAD_ID_P option:selected").val() == "3048$Q")) {
                            if ($("#IMS_AGREEMENT_CODE_C").val() == 0 || $("#IMS_AGREEMENT_CODE_C").val() == "") {

                                if ($('#IMS_AGREEMENT_CODE_C  option').length == 1) {
                                    FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_C", "/Payment/GetSupplierContractorAgreement/" + Bill_ID);
                                }
                            }

                            $('.conAgreement').show('slow');

                            $('#IMS_AGREEMENT_CODE_C').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Agreement Name is Required'
                                }
                            });
                        }
                      
                    }
                    else {
                        showContractorNarration = true;

                        if (data.ShowContractor == false) {
                            $('.conAgreement').hide();
                            showContractorNarration = false;
                        }
                    }
                    //if road required but agreement is not required
                    if (data.RoadRequired == "Y" && data.AgreementRequired == "N") {
                        $(".road").show();
                        $("#supAgreement").hide();
                    }
                    else {
                        
                        if ($("#HEAD_ID_P option:selected").val() == "3042$Q" || $("#HEAD_ID_P option:selected").val() == "3045$Q" || $("#HEAD_ID_P option:selected").val() == "3046$Q" || $("#HEAD_ID_P option:selected").val() == "3047$Q" || $("#HEAD_ID_P option:selected").val() == "3048$Q" ) {
                        
                            $('.conAgreement').hide('slow');
                            showContractorNarration = false;
                        }
                    }

                    if (data.YearRequired == "Y") {

                        if ($("#SANCTION_YEAR").val() == 0 || $("#SANCTION_YEAR").val() == "") {

                            if ($('#SANCTION_YEAR  option').length == 1) {

                                FillInCascadeDropdown(null, "#SANCTION_YEAR", "/Payment/populateSactionYear/" + $("#HEAD_ID_P").val()); //new change done by Vikram on 30-08-2013
                            }
                        }
                        $(".sanctionYear").show('slow');
                        isSanctionedYearRequired = true;

                    }
                    else {
                        $(".sanctionYear").hide('slow');

                    }

                    if (data.PackageRequired == "Y") {

                        isPackageRequired = true;
                        $(".sanctionPackage").show('slow');
                    }
                    else {
                        $(".sanctionPackage").hide('slow');
                    }


                    if (Bill_ID == 0 || Bill_ID == null || Bill_ID == "") {
                        Bill_ID = $("#Bill_ID").val();
                    }

                    if (data.AgreementRequired == "Y" && Bill_ID != null && Bill_ID != "") {

                        isAgreementRequired = true;

                        isAgreementRequired_s = true;

                        //setTimeout(function () {

                        $.ajax({
                            type: "POST",
                            url: "/payment/GetSelectedAgreementForTransaction/" + Bill_ID,
                            async: false,
                            cache: false,
                            // data: $("#authSigForm").serialize(),
                            error: function (xhr, status, error) {
                                unblockPage();
                                $('#divError').show('slow');
                                $('#errorSpan').text(xhr.responseText);

                            },
                            success: function (data1) {
                                unblockPage();
                                $('#divError').hide('slow');
                                $('#errorSpan').html("");

                                if (data1 != "") {
                                    if (data.SupplierRequired == "Y") {

                                        $('#IMS_AGREEMENT_CODE_S').val(data1);

                                        if ($("#IMS_AGREEMENT_CODE_S").val() != null) {
                                            $("#IMS_AGREEMENT_CODE_S").attr('disabled', 'disabled');
                                        }

                                        if (TriggerWhenError == false)
                                            $('#IMS_AGREEMENT_CODE_S').trigger('change');

                                    }
                                        //if contractor required at master payment and transaction payment also
                                    else if (data.ContractorRequired == "Y" && data.ShowContractor == false) {




                                        if ($("#IMS_AGREEMENT_CODE_C").val() != null) {
                                            $("#IMS_AGREEMENT_CODE_C").attr('disabled', 'disabled');

                                            //added by Koustubh Nakate on 27/09/2013 to populate roads when agreement dropdown disabled

                                            //$('#IMS_AGREEMENT_CODE_C').trigger('change');

                                            //   alert('B');

                                            //setTimeout(function () {
                                            //    $('#IMS_AGREEMENT_CODE_C').trigger('change');
                                            //}, 1000);

                                        }

                                        $('#IMS_AGREEMENT_CODE_C').val(data1);
                                        //commented by Koustubh Nakate on 14/10/2013 to avoid repeated change trigger  
                                        if (TriggerWhenError == false)
                                            setTimeout(function () {
                                                $('#IMS_AGREEMENT_CODE_C').val(data1).trigger('change');
                                            }, 200);


                                    }
                                }
                                else {
                                    $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
                                    $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');
                                    // $('#IMS_PR_ROAD_CODE').children('option:not(:first)').remove();
                                    // $('#IMS_PR_ROAD_CODE').children('option:not(:first)').remove();

                                }
                            }
                        });
                        //}, 1000);

                    } else {

                        $(".AgreementDed").hide("slow");
                    }

                    altRowStyle();

                }
                else {
                    alert("Design parameter not added for this head !!!");

                }

            }
        });

        $.ajax({
            type: "POST",
            url: "/payment/GetHeadDetails/" + Bill_ID + "/" + $("#HEAD_ID_P").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (result1) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (result1 != "") {
                    var resultData = result1.split('$');

                    $("#descPayTd").text(resultData[1]).parent().show('slow').css("font-weight", "bold").css("text-align", "center");

                    if (changeNarrationPay == true || (!transEdit)) {
                        $("#HeadNarrationSpan").text(resultData[0]);

                        if (!showContractorNarration) {
                            //Old Code
                            //$("#NARRATION_P").val(resultData[0]);

                            //modified By Abhishek Kamble 11-Nov-2013
                            if (resultData[0] != "") {
                                $("#NARRATION_P").val(resultData[0]);
                            }
                        } else {
                            SetNarration();
                        }
                    }

                }
            }
        });

    });


    //function to submit transaction details
    $("#btnPaymentSubmit").click(function (e) {
        var isDprAgreement = false;
        //alert(MasterTxnId);
        if ((isAgreementRequired == true) && (roadRequired == true) && (MasterTxnId ==86)) {
            if (VerifyDPRAgreement($("#CONTRACTOR_ID").val(), $("#IMS_AGREEMENT_CODE_C").val(), $("#fundType").val())) { 
                isDprAgreement = true;
            }
            else {
                $('#IMS_PR_ROAD_CODE').rules('add', {
                    drpRequired: true,
                    messages: {
                        drpRequired: 'Road is Required'
                    }
                });
            }

        }
        //alert("alert1");
        if ((isSanctionedYearRequired == true) && (isPackageRequired == true) && (roadRequired == true)) {
            $('#IMS_PR_ROAD_CODE').rules('add', {
                drpRequired: true,
                messages: {
                    drpRequired: 'Road is Required'
                }
            });
        }
        //alert("alert2");
        if (parseFloat(TotalAmtToEnterDedAmount) != 0) {
            clearValidation($("#DeductionTransactionForm"));
        }


        //alert("alert3");

        $('#HEAD_ID_P').rules('add', {
            required: true,
            messages: {
                required: 'Sub Transaction Type (Payment) is Required'
            }
        });

        $('#NARRATION_P').rules('add', {
            required: true,
            messages: {
                required: 'Narration is Required'
            }
        });
        //alert("alert4");
        //added by Koustubh Nakate on 27/09/2013 for contractor and agreement
        if (isContractorRequired == true) {
            $('#MAST_CON_ID_CON').rules('add', {
                iscontractorequired: true,
                messages: {
                    iscontractorequired: 'Contractor Name is Required'
                }
            });
        }
        else {
            $('#MAST_CON_ID_CON').rules("remove", "messages");
        }

        if (isAgreementRequired == true) {
            $('#IMS_AGREEMENT_CODE_C').rules('add', {
                isagreementrequired: true,
                messages: {
                    isagreementrequired: 'Agreement Name is Required'
                }
            });
        }
        else {
            $('#IMS_AGREEMENT_CODE_C').rules("remove", "messages");
        }
        //alert("alert5");
        if (isSanctionedYearRequired == true) {
            $('#SANCTION_YEAR').rules('add', {
                issanctionedyearrequired: true,
                messages: {
                    issanctionedyearrequired: 'Sanction Year is Required'
                }
            });
        }
        else {
            $('#SANCTION_YEAR').rules("remove", "messages");
        }
        //alert("alert6");
        if (isPackageRequired == true) {
            $('#SANCTION_PACKAGE').rules('add', {
                ispackagerequired: true,
                messages: {
                    ispackagerequired: 'Sanction Package is Required'
                }
            });
        }
        else {
            $('#SANCTION_PACKAGE').rules("remove", "messages");
        }

        //alert("alert7");
        if (isAgreementRequired_s == true) {
            $('#IMS_AGREEMENT_CODE_S').rules('add', {
                isagreementrequired_s: true,
                messages: {
                    isagreementrequired_s: 'Agreement Name is Required'
                }
            });
        }
        else {
            $('#IMS_AGREEMENT_CODE_S').rules("remove", "messages");
        }


       
        //alert("alert8");

        //  alert(OnlyCash +" "+ TotalAmtToEnterDedAmount)
        //only cash amount with no deduction
        if (OnlyCash == "Y" && TotalAmtToEnterDedAmount == 0) {
            $('#AMOUNT_Q').rules('add', {
                required: true,
                greaterThanZero: true,
                lessOrEqualToCashAmountRemaining: true,
                messages: {
                    required: 'Amount is Required',
                    greaterThanZero: 'Amount must be greater than 0',
                    lessOrEqualToCashAmountRemaining: 'Amount must be less than or equal to Diffrence To Be Entered for Cash Amount'
                }
            });
        }
        else {


            if (TotalAmtToEnterDedAmount != 0) {

                $('#AMOUNT_Q').rules('add', {
                    required: true,
                    //greaterThanZero: true,
                    lessOrEqualToChequeAmountRemaining: true,
                    messages: {
                        required: 'Amount is Required',
                        //           greaterThanZero: 'Amount must be greater than 0',
                        lessOrEqualToChequeAmountRemaining: 'Amount must be less than or equal to Diffrence To Be Entered for cheque Amount'
                    }
                });
            } else {
                $('#AMOUNT_Q').rules('add', {
                    required: true,
                    lessOrEqualToChequeAmountRemainingEdit: true,
                    messages: {
                        required: 'Cheque Amount is Required',
                        //greaterThanZero: 'Cheque Amount must be greater than 0'
                        lessOrEqualToChequeAmountRemainingEdit: 'Invalid Cheque Amount.its greater than remaining cheque amount to enter'
                    }
                });
            }
        }
        //alert("alert9");

        //Cash Amount Should be less than or equal to Cheque Amount Validation is Commented Deduction only transaction
        if (!$('#AMOUNT_C').is("readonly")) {
            $('#AMOUNT_C').rules('add', {
                //required: true,
                lessOrEqualToCashAmountRemaining: true,
                //lessOrEqualToChequeAmount:true,
                messages: {
                    //required: 'Cash Amount is Required',
                    lessOrEqualToCashAmountRemaining: "Cash Amount must be less than or equal to Diffrence To Be Entered for Cash Amount"
                    //lessOrEqualToChequeAmount:"Cash Amount Should be less than or equal to Cheque Amount"
                }
            });
        }

        if (Bill_ID == 0 || Bill_ID == null || Bill_ID == "") {
            Bill_ID = $("#Bill_ID").val();
        }

       
        if (!(isDprAgreement)) {
            if ($("#IMS_PR_ROAD_CODE").is(":visible")) {
                if ($("#IMS_PR_ROAD_CODE").val() == 0) {
                    alert('Please select Road');
                }
            }
        }

        e.preventDefault();
        //var flag = GetContractorBankGuranteeDetails($('#IMS_AGREEMENT_CODE_C').val(), $("#fundType").val(), 'S');
        //Added By Bhushan

       // alert($("#AMOUNT_Q").val());
       // if ($("#AMOUNT_Q").val() > 0) {
        var flag=true;
        //alert("alert10");
        if ($('#fundType').val() == 'P' ) {
             flag = GetContractorBankGuranteeDetails($('#IMS_AGREEMENT_CODE_C').val(), $("#fundType").val(), 'S');
        }
        //}

        //alert("alert11");
        if (flag) {
            if ($("#PaymentTransactionForm").valid()) {
                //alert("alert12");
               blockPage();

                $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
                $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');
                $("#HEAD_ID_P").removeAttr('disabled');
                $("#MAST_CON_ID_CON").removeAttr('disabled');

                $.ajax({
                    type: "POST",
                    url: "/payment/AddPaymentTransactionDetails/" + Bill_ID,
                    //async: false,
                    data: $("#PaymentTransactionForm").serialize(),
                    error: function (xhr, status, error) {
                        unblockPage();
                        $('#errorSpan').text(xhr.responseText);
                        $('#divError').show('slow');
                        $("#errorSpan").show('slow');
                        return false;
                    },
                    success: function (data) {
                        unblockPage();
                        $('#divError').hide('slow');
                        $('#errorSpan').html("");
                        $('#errorSpan').hide();

                        $('#divDetailsError').hide('slow');

                        if (data.Success === undefined) {
                            unblockPage();
                            $("#PaymentDeductionData").html(data);
                            $.validator.unobtrusive.parse($("#PaymentDeductionData"));
                            TriggerWhenError = true;
                            $("#HEAD_ID_P").trigger('change');

                        }
                        else if (data.Success) {

                            //reload the payment transaction grid

                            $("#PaymentGridDivList").jqGrid().setGridParam
                                ({ url: '/Payment/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                            GetAmountTableDetails(Bill_ID);

                            $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                            resetDetailsForm();

                            //if master transaction doesent have multiple transaction allowed disable the transaction payment head and set the value
                            if (data.disblehead) {

                                $("#HEAD_ID_P").attr('disabled', 'disabled');
                                $("#HEAD_ID_P").val(data.head).trigger("change");
                                //$("#HEAD_ID_P").trigger("change");

                            }
                            else {
                                $("#HEAD_ID_P").removeAttr('disabled');
                            }

                            //hide the is final payment row
                            $(".final").hide('slow');

                            if (!$("#HEAD_ID_P").is(":disabled")) {
                                $("#HeadDescTr").hide('slow');
                            }

                            alert("Payment Details Added successfully.");

                            return false;


                        }
                        else if (data.Success == false && data.status == "-1") {
                            alert("This transaction cant be added ,master payment is already finalized");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-990") {
                            alert("Expenditure amount can not be greater than sanctioned cost");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-555") {
                            $("#divDetailsError").show("slide");
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            // $("#AddPayment").hide();
                        }
                        else if (data.Success == false && data.status == "-55") { //Validation to check Head 55 added by Abhishek kamble 26-Aug-2014 start
                            alert("Entry is not allowed since Head 55 is valid upto March-2014.");
                            //  $("#AddPayment").hide();
                            return false;
                        }
                        else if (data.Success == false && data.status == "-999") { //Validation to check PMGSY scheme for Selected Road added by Abhishek kamble 5-Sep-2014 start
                            $("#divDetailsError").show("slide");
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            // $("#AddPayment").hide();
                        }
                        else if (data.Success == false && data.status == "-9") { //Validation to check completion date less than 6 months
                            $("#divDetailsError").show("slide");
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            ////  $("#AddPayment").hide();
                        }

                        else if (data.Success == false && data.status == "-777") { //Validation to check if the lab establishment details are available or not
                            $("#divDetailsError").show("slide");
                            //  $("#AddPayment").hide();
                            // 
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        else if (data.Success == false && data.status == "-111")//Added on 14-09-2022
                        {
                            $("#divDetailsError").show("slide");
                            //  $("#AddPayment").hide();
                            // 
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        else if (data.Success == false && data.status == "-222")//Added on 26-08-2023
                        {
                            $("#divDetailsError").show("slide");
                            //  $("#AddPayment").hide();
                            // 
                            $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }






                    }
                });
            }

        }

    });

    // button event to update the transaction details
    $("#btnPaymentUpdate").click(function (e) {


        if (Bill_ID == 0 || Bill_ID == null) {

            Bill_ID = $("#Bill_ID").val();
        }
        e.preventDefault();

        if (parseFloat(TotalAmtToEnterDedAmount) != 0) {
            clearValidation($("#DeductionTransactionForm"));
        }

        $('#HEAD_ID_P').rules('add', {
            required: true,
            messages: {
                required: 'Sub Transaction Type (Payment) is Required'
            }
        });

        $('#NARRATION_P').rules('add', {
            required: true,
            messages: {
                required: 'Narration is Required'
            }
        });


        //added by Koustubh Nakate on 27/09/2013 for contractor and agreement
        if (isContractorRequired == true) {
            $('#MAST_CON_ID_CON').rules('add', {
                iscontractorequired: true,
                messages: {
                    iscontractorequired: 'Contractor Name is Required'
                }
            });
        }
        else {
            $('#MAST_CON_ID_CON').rules("remove", "messages");
        }

        if (isAgreementRequired == true) {
            $('#IMS_AGREEMENT_CODE_C').rules('add', {
                isagreementrequired: true,
                messages: {
                    isagreementrequired: 'Agreement Name is Required'
                }
            });
        }
        else {
            $('#IMS_AGREEMENT_CODE_C').rules("remove", "messages");
        }

        if (isSanctionedYearRequired == true) {
            $('#SANCTION_YEAR').rules('add', {
                issanctionedyearrequired: true,
                messages: {
                    issanctionedyearrequired: 'Sanction Year is Required'
                }
            });
        }
        else {
            $('#SANCTION_YEAR').rules("remove", "messages");
        }

        if (isPackageRequired == true) {
            $('#SANCTION_PACKAGE').rules('add', {
                ispackagerequired: true,
                messages: {
                    ispackagerequired: 'Sanction Package is Required'
                }
            });
        }
        else {
            $('#SANCTION_PACKAGE').rules("remove", "messages");
        }


        if (isAgreementRequired_s == true) {
            $('#IMS_AGREEMENT_CODE_S').rules('add', {
                isagreementrequired_s: true,
                messages: {
                    isagreementrequired_s: 'Agreement Name is Required'
                }
            });
        }
        else {
            $('#IMS_AGREEMENT_CODE_S').rules("remove", "messages");
        }

        //id only cash amouunt check with cash amount 
        //alert(OnlyCash +" "+ TotalAmtToEnterDedAmount)

        if (OnlyCash == "Y" && TotalAmtToEnterDedAmount == 0) {

            $('#AMOUNT_Q').rules('add', {
                required: true,
                greaterThanZero: true,
                lessOrEqualToCashAmountRemainingEditForCashOnly: true,
                messages: {
                    required: 'Amount is Required',
                    greaterThanZero: 'Amount must be greater than 0',
                    lessOrEqualToCashAmountRemainingEditForCashOnly: 'Amount must be less than or equal to Diffrence To Be Entered for Cash Amount'
                }

            });

        }
        else {
            //If Condition Added By Abhishek kamble 16-Apr-2014
            if (TotalAmtToEnterDedAmount != 0) {
                $('#AMOUNT_Q').rules('add', {
                    required: true,
                    //greaterThanZero: true,
                    lessOrEqualToChequeAmountRemainingEdit: true,
                    messages: {
                        required: 'Cheque Amount is Required'
                        //greaterThanZero: 'Cheque Amount must be greater than 0'
                       , lessOrEqualToChequeAmountRemainingEdit: 'Invalid Cheque Amount.its greater than remaining cheque amount to enter'
                    }
                });
            }
            else {
                $('#AMOUNT_Q').rules('add', {
                    required: true,
                    lessOrEqualToChequeAmountRemainingEdit: true,
                    messages: {
                        required: 'Cheque Amount is Required',
                        //greaterThanZero: 'Cheque Amount must be greater than 0'
                        lessOrEqualToChequeAmountRemainingEdit: 'Invalid Cheque Amount.its greater than remaining cheque amount to enter'
                    }
                });

            }
        }

        //Cash Amount Should be less than or equal to Cheque Amount Validation is Commented Deduction only transaction
        if (!$('#AMOUNT_C').is("readonly")) {
            $('#AMOUNT_C').rules('add', {
                //required: true,
                lessOrEqualToCashAmountRemainingEdit: true,
                //lessOrEqualToChequeAmount: true,
                messages: {
                    required: 'Cash Amount is Required'
                   , lessOrEqualToCashAmountRemainingEdit: "Invalid Cash Amount,its greater than remaining cheque amount to enter"
                    //lessOrEqualToChequeAmount: "Cash Amount Should be less than or equal to Cheque Amount"
                }
            });
        }

     //   var boolResult= VerifyDPRAgreement($("#CONTRACTOR_ID").val(), $("#IMS_AGREEMENT_CODE_C").val(), $("#fundType").val());

     

    
        if ($("#PaymentTransactionForm").valid()) {
            blockPage();
            $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
            $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');
            $("#MAST_CON_ID_CON").removeAttr('disabled');
            $("#HEAD_ID_P").removeAttr('disabled');

            $.ajax({
                type: "POST",
                url: "/payment/PostEditTransactionDetails/" + urlparamsForTransEdit,
                //async: false,
                data: $("#PaymentTransactionForm").serialize(),
                error: function (xhr, status, error) {
                    unblockPage();
                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();

                    if (data.Success === undefined) {
                        unblockPage();
                        $("#PaymentDeductionData").html(data);
                        $.validator.unobtrusive.parse($("#PaymentDeductionData"));
                        TriggerWhenError = true;
                        $("#HEAD_ID_P").trigger('change');
                        $("#btnPaymentUpdate").show();
                        $("#btnPaymentSubmit").hide();

                    }
                    else if (data.Success) {

                        $("#PaymentGridDivList").jqGrid().setGridParam
                       ({ url: '/Payment/GetPaymentDetailList/' + data.Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");


                        GetAmountTableDetails(data.Bill_ID);

                        $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();


                        //if master transaction doesent have multiple transaction allowed disable the transaction payment head and set the value
                        if (data.disblehead) {
                            $("#HEAD_ID_P").val(data.head);
                            $("#HEAD_ID_P").attr('disabled', 'disabled');
                            $("#HEAD_ID_P").trigger("change");
                        }
                        else {
                            $("#HEAD_ID_P").removeAttr('disabled');
                            $("#HeadDescTr").hide('slow');
                        }



                        $("#HEAD_ID_P > option").each(function () {

                            if ($(this).attr('class') == "X") {
                                $(this).remove();
                            }
                        });
                        if ($("#divDetailsError").is(":visible")) {
                            $("#divDetailsError").html('');
                            $("#divDetailsError").hide('slow');
                        }

                        alert("Payment Details updated successfully.");

                        return false;

                    }
                    else if (data.Success == false && data.Bill_ID == "-1") {
                        alert("This transaction cant be edited ,master payment is already finalized");

                        $("#HEAD_ID_P").attr('disabled', true);
                        return false;
                    }
                    else if (data.Success == false && data.status == "-555") {
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);

                    } else if (data.Success == false && data.status == "-999") { //Validation to check PMGSY scheme for Selected Road added by Abhishek kamble 5-Sep-2014 start
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                    else if (data.Success == false && data.status == "-9") { //Validation to check completion date less than 6 months
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }


                    else if (data.Success == false && data.status == "-777") { //Validation to check if the lab establishment details are available or not
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }




                }
            });
        }


    });

    //event to save the deduction details
    $("#btnDeductionSubmit").click(function (e) {

        if (parseFloat(TotalAmtToEnterChqAmount) != 0) {
            clearValidation($("#PaymentTransactionForm"));
        }


        $('#HEAD_ID_D').rules('add', {
            required: true,
            messages: {
                required: 'Sub Transaction Type (Deduction) is Required'
            }
        });

        $('#NARRATION_D').rules('add', {
            required: true,
            messages: {
                required: 'Narration is Required'
            }
        });


        $('#AMOUNT_D').rules('add', {
            required: true,
            lessOrEqualToDedAmountRemaining: true,
            greaterThanZero: true,
            messages: {
                required: 'Deduction Amount is Required',
                lessOrEqualToDedAmountRemaining: "Deduction Amount must be less than or equal to Diffrence To Be Entered for Deduction Amount",
                greaterThanZero: "Deduction Amount Should be greater than 0"
            }
        });



        if (parseFloat($("#TotalAmtEnteredChqAmount").text()) == 0 && parseFloat($("#TotalAmtToEnterChqAmount").text()) != 0) {
            alert("Please enter payment transactions first");

            //added by Abhishek kamble 25Nov2014 to reset ded sub txn
            $("#btnDeductionReset").trigger("click");

            return false;
        }

        if (Bill_ID == 0 || Bill_ID == null) {

            Bill_ID = $("#Bill_ID").val();
        }
        e.preventDefault();



        if ($("#DeductionTransactionForm").valid()) {

            //Added By Abhishek kamble to hide agreement Details for txn Expenditure on RRTRC/RRNMU start
            //If condition added by Abhishek 24Nov2014
            // if ((parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1491) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1492) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1493)) {

            if (_MasterAgreementRequired == "Y") {
                $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');
            } else {
                //$("#IMS_AGREEMENT_CODE_DED").val(null);
                //Below line is modified on 21-01-2022
                $("#IMS_AGREEMENT_CODE_DED").html("");
            }
            // }


            blockPage();
            $.ajax({
                type: "POST",
                url: "/payment/AddDeductionTransactionDetails/" + Bill_ID,
                //async: false,
                data: $("#DeductionTransactionForm").serialize(),
                error: function (xhr, status, error) {

                    unblockPage();
                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();

                    if (data.Success === undefined) {

                        unblockPage();
                        $("#PaymentDeductionData").html(data);
                        $.validator.unobtrusive.parse($("#PaymentDeductionData"));

                    }
                    else if (data.Success) {
                        //reload the payment deduction grid
                        $("#PaymentGridDivList").jqGrid().setGridParam
                       ({ url: '/Payment/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAmountTableDetails(Bill_ID);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();

                        $("#headDescDedTR").hide('slow');

                        alert("Deduction Details added.");
                        return false;

                    } else if (data.Success == false && data.status == "-1") {
                        alert("This transaction cant be added ,master payment is already finalized");
                        return false;
                    }
                    else {



                    }

                }
            });
        }


    });

    //function to update the deduction transaction
    $("#btnDeductionUpdate").click(function (e) {

        if (Bill_ID == 0 || Bill_ID == null) {

            Bill_ID = $("#Bill_ID").val();
        }


        if (parseFloat(TotalAmtToEnterChqAmount) != 0) {
            clearValidation($("#PaymentTransactionForm"));
        }


        $('#HEAD_ID_D').rules('add', {
            required: true,
            messages: {
                required: 'Sub Transaction Type (Deduction) is Required'
            }
        });

        $('#NARRATION_D').rules('add', {
            required: true,
            messages: {
                required: 'Narration is Required'
            }
        });


        $('#AMOUNT_D').rules('add', {
            required: true,
            lessOrEqualToDedAmountRemainingEdit: true,
            greaterThanZero: true,
            messages: {
                required: 'Deduction Amount is Required',
                lessOrEqualToDedAmountRemainingEdit: "invalid Deduction Amount, its greater than remaining deduction amount to enter",
                greaterThanZero: "Deduction Amount Should be greater than 0"
            }
        });

        //return false;
        e.preventDefault();

        if ($("#DeductionTransactionForm").valid()) {

            //Added By Abhishek kamble to hide agreement Details for txn Expenditure on RRTRC/RRNMU start
            //If condition added by Abhishek 24Nov2014
            // if ((parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1491) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1492) && (parseInt($("#HEAD_ID_D").val().split('$')[0]) != 1493)) {

            if (_MasterAgreementRequired == "Y") {
                $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');
            }
            // }

            $("#HEAD_ID_D").removeAttr('disabled');

            blockPage();
            $.ajax({
                type: "POST",
                url: "/payment/PostEditTransactionDetails/" + urlparamsForTransEdit,
                //async: false,
                data: $("#DeductionTransactionForm").serialize(),
                error: function (xhr, status, error) {

                    unblockPage();
                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();

                    if (data.Success === undefined) {
                        unblockPage();
                        $("#PaymentDeductionData").html(data);
                        $.validator.unobtrusive.parse($("#PaymentDeductionData"));
                        $("#btnDeductionUpdate").show();
                        $("#btnDeductionSubmit").hide();
                    }
                    else if (data.Success) {

                        //reload the payment deduction grid
                        $("#PaymentGridDivList").jqGrid().setGridParam
                       ({ url: '/Payment/GetPaymentDetailList/' + data.Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAmountTableDetails(data.Bill_ID);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

                        resetDetailsForm();

                        $("#headDescDedTR").hide('slow');

                        //remove invalid trnasaction type as its only for add opeartion
                        $("#HEAD_ID_D > option").each(function () {

                            if ($(this).attr('class') == "X") {
                                $(this).remove();
                            }
                        });



                        alert("Deduction Details Updated successfully.");

                        if (data.disblehead) {
                            $("#HEAD_ID_P").trigger("change");

                        }



                        return false;

                    }
                    else if (data.Success == false && data.Bill_ID == "-1") {
                        alert("This transaction cant be edited ,master payment is already finalized");
                        $("#HEAD_ID_D").attr('disabled', true);
                        return false;
                    }
                    else {



                    }

                }
            });
        }


    });

    //function to finalize the voucher
    $("#finalize").unbind().click(function () {
        //added by vikram 28-8-2013 start
        getPaymentClosingBalanceOnFinalize($("#BILL_MONTH").val(), $("#BILL_YEAR").val());


        ValidateVoucher(Bill_ID);

        if (isValid == false) {
            alert('Amount  must be less than or equal to Bank Authorization balance amount');
            return false;
        }
        //added by vikram 28-8-2013 end
        var final = confirm("Are you sure to Finalize the Payment?");
        if (final) {

            $.ajax({
                type: "POST",
                url: "/payment/FinalizeVoucher/" + Bill_ID,
                //async: false,
                cache: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();

                    if (data.Success == 1) {

                        alert("Voucher Finalized.");


                        //if voucher is of epay show epay order
                        if (data.isEpayVoucher) {
                            ViewEpayOrder(data.voucherCodeAndStatus);
                        }
                            //if voucher is of eremittance show eremittance order
                        else if (data.isEremVoucher) {

                            ViewEremOrder(data.voucherCodeAndStatus);
                        }
                        else {
                            blockPage();

                            $("#mainDiv").load("/payment/GetPaymentList/", function () {
                                unblockPage();
                                // return false;
                            });

                        }

                    }
                    else if (data.Success == -1) {

                        alert("Voucher cant be Finalized as all transaction details are not entered.");
                        return false;
                    } else if (data.Success == -2) {

                        alert("Voucher cant be Finalized as all transaction are not correct.");
                        return false;
                    }
                    else {
                        alert("Error while finalizing the voucher.");
                        return false;
                    }
                }
            });


        }
    });

    //on change event of the cheque amount find out the the deduction amount
    $("#AMOUNT_Q").bind('change', function () {

        if (TotalAmtToEnterDedAmount != 0 && TotalAmtToEnterChqAmount != 0) {
            //get what % of the cheque amount is cash amount  in master payment entry
            var deductionAmtPErcentage = (TotalAmtToEnterDedAmount * 100 / TotalAmtToEnterChqAmount)

            var deductionAmt = deductionAmtPErcentage * $("#AMOUNT_Q").val() / 100;
            if (deductionAmt) {
                //find out that % of the amount of current cheque amount and set it as deduction amount 
                $("#AMOUNT_C").val(parseFloat(deductionAmt).toFixed(2));
            }
            else {

                $("#AMOUNT_C").val(0);
            }
        }

    });

    //Added By Abhishek kamble 16-jan-2014
    $("#AMOUNT_C").click(function () {

        if ($("#AMOUNT_C").val() == 0) {
            if (TotalAmtToEnterDedAmount != 0 && TotalAmtToEnterChqAmount != 0) {
                //get what % of the cheque amount is cash amount  in master payment entry
                var deductionAmtPErcentage = (TotalAmtToEnterDedAmount * 100 / TotalAmtToEnterChqAmount)

                var deductionAmt = deductionAmtPErcentage * $("#AMOUNT_Q").val() / 100;
                if (deductionAmt) {
                    //find out that % of the amount of current cheque amount and set it as deduction amount 
                    $("#AMOUNT_C").val(parseFloat(deductionAmt).toFixed(2));
                }
                else {
                    $("#AMOUNT_C").val(0);
                }
            }
        }
    });


    if (parseFloat(TotalAmtToEnterDedAmount) == 0) {

        $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        $("#cashAmtTr").hide();
        // alert("c");
    }
});//document ready

function altRowStyle() {

    $("#PaymentTable").removeClass('rowstyle')//.addClass('rowstyle');
    $("#PaymentTable tr").removeClass('ui-state-hover');
    $('#PaymentTable tr:visible').filter(':even').addClass('ui-state-hover');

}

//function to reset the details form
function resetDetailsForm() {
    //$('#TransactionForm').trigger('reset');
    $('.conAgreement').hide('slow');
    $('.piu').hide('slow');
    $('.road').hide('slow');
    $('.supAgreement').hide('slow');
    $("#btnDeductionUpdate").hide();
    $("#btnDeductionSubmit").show();
    $("#btnDeductionReset").show();
    $("#btnDeductionCancel").hide();
    $("#btnPaymentUpdate").hide();
    $("#btnPaymentCancel").hide();
    $("#btnPaymentReset").show();
    $("#btnPaymentSubmit").show();
    // after save/edit operation hide the final payment row
    if ($(".final").is(':visible')) // new change done by Vikram 
    {
        $(".final").hide();

    }
    // after save/update operation hide the agreement deduction dropdown
    if ($(".AgreementDed").is(':visible')) //new change done by Vikram
    {
        $(".AgreementDed").hide();
    }
}

//function to show epayment order in pop up 
function ViewEpayOrder(urlParam) {


    //$.ajax({
    //    type: "POST",
    //    url: "/payment/GetEpaymentOrderDetails/" + urlParam,
    //    //async: false,
    //    error: function (xhr, status, error) {
    //        unblockPage();

    //        $('#errorSpan').text(xhr.responseText);
    //        $('#divError').show('slow');
    //        $('#errorSpan').show();
    //        return false;
    //    },
    //    success: function (data) {
    //        unblockPage();
    //        $('#divError').hide('slow');
    //        $('#errorSpan').html("");
    //        $('#errorSpan').hide();

    //        if (data.Success) {


    //            $("#EmailRecepient1").text(data.EmailRecepient).css("font-weight", "bold");

    //            $("#DPIUName1").text(data.DPIUName).css("font-weight", "bold");
    //            $("#STATEName1").text(data.STATEName).css("font-weight", "bold");
    //            $("#EmailDate1").text(data.EmailDate).css("font-weight", "bold");
    //            $("#Bankaddress1").text(data.Bankaddress).css("font-weight", "bold");
    //            $("#BankAcNumber1").text(data.BankAcNumber).css("font-weight", "bold");
    //            $("#EpayNumber1").text(data.EpayNumber).css("font-weight", "bold");
    //            $("#EpayDate1").text(data.EpayDate).css("font-weight", "bold");
    //            $("#EpayState1").text(data.EpayState).css("font-weight", "bold");
    //            $("#EpayDPIU1").text(data.EpayDPIU).css("font-weight", "bold");
    //            $("#EpayVNumber1").text(data.EpayVNumber).css("font-weight", "bold");
    //            $("#EpayVDate1").text(data.EpayVDate).css("font-weight", "bold");
    //            $("#EpayVPackages1").text(data.EpayVPackages).css("font-weight", "bold");
    //            $("#EpayConName1").text(data.EpayConName).css("font-weight", "bold");
    //            $("#EpayConAcNum1").text(data.EpayConAcNum).css("font-weight", "bold");
    //            $("#EpayConBankName1").text(data.EpayConBankName).css("font-weight", "bold");
    //            $("#EpayConBankIFSCCode1").text(data.EpayConBankIFSCCode).css("font-weight", "bold");
    //            $("#EpayAmount1").text(data.EpayAmount).css("font-weight", "bold");
    //            $("#EpayNo1").text(data.EpayNumber).css("font-weight", "bold");
    //            $("#EpayAmountInWord1").text(data.EpayAmountInWord).css("font-weight", "bold");


    //            //Added by Abhishek kamble 29-May-2014              
    //            if (data.EpayContLegalHeirName == "" || data.EpayContLegalHeirName == null) {
    //                $("#trContLegalHeirDetails1").hide();
    //            }
    //            else {
    //                $("#trContLegalHeirDetails1").show();
    //            }
    //            $("#EpayConLegalHeirName1").text(data.EpayContLegalHeirName).css("font-weight", "bold");


    //            $("#PaymentEpaydialog").dialog("open");

    //            return false;
    //        }
    //    }
    //});

    $.ajax({
        type: "POST",
        url: "/payment/GetEpaymentDetailsForSigning/" + urlParam + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan2').text(xhr.responseText);
            $('#divSignError2').show('slow');
            $('#errorSignSpan2').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError2').hide('slow');
            $('#errorSignSpan2').html("");
            $('#errorSignSpan2').hide();

            if (data != "") {
                //  alert(data);
                $("#SignEpaymentDiv").html(data);

                $("#SignEpaymentDialog").dialog("open");

                return false;
            }
        }
    });
}

//function to show eremittnace order in pop up
function ViewEremOrder(urlParam) {


    //$.ajax({
    //    type: "POST",
    //    url: "/payment/GetERemOrderDetails/" + urlParam,
    //    //async: false,
    //    error: function (xhr, status, error) {
    //        unblockPage();


    //        $('#errorSpan2').text(xhr.responseText);
    //        $('#divError2').show('slow');
    //        $('#errorSpan2').show();
    //        return false;
    //    },
    //    success: function (data) {
    //        unblockPage();
    //        $('#divError2').hide('slow');
    //        $('#errorSpan2').html("");
    //        $('#errorSpan2').hide();

    //        if (data != "") {

    //            //$("#ERemOrderDiv").html(data);

    //            //$("#PaymentEremDialog").dialog("open");

    //            // alert(data);
    //            $("#PaymentEpaydialog").html(data);

    //            $("#PaymentEpaydialog").dialog("open");

    //            return false;
    //        }
    //    }
    //});

    $.ajax({
        type: "POST",
        url: "/payment/GetERemDetailsForSigning/" + urlParam + '#' + Math.random(),
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#errorSignSpan3').text(xhr.responseText);
            $('#divSignError3').show('slow');
            $('#errorSignSpan3').show();
            return false;
        },
        success: function (data) {
            unblockPage();

            $('#divSignError3').hide('slow');
            $('#errorSignSpan3').html("");
            $('#errorSignSpan3').hide();

            if (data != "") {
                //  alert(data);
                $("#SignERemDiv").html(data);

                $("#SignERemDialog").dialog("open");

                return false;
            }
        }
    });

}



//function to set the narration when contractor payment
function SetNarration() {
    if (showContractorNarration) {
        //get the contractor name from master grid
        if (getContractorNameFromMasterEntry) {
            $("#CompanyNameSpan").text($("#PaymentMasterList > tbody > tr:nth-child(" + 2 + ") td:nth-child(" + 8 + ")").text().trim());
        }
        $("#NARRATION_P").val($("#HiddenNarrationSpan").text().trim());
    }
}

//added by vikram 28-8-2013 start
function getPaymentClosingBalanceOnFinalize(month, year) {

    $.ajax({
        type: "POST",
        url: "/payment/GetClosingBalanceForPayment/" + month + "$" + year,
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);
            return false;
        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data != "") {
                balanceCashAmount = data.Cash;
                balanceChequeAmount = data.BankAuth;
                if (data.BankAuth == '0') {
                    isValid = false;
                }
                else {
                    isValid = true;
                }

                $("#lblCash").text(parseFloat(data.Cash).toFixed(2));
                $("#lblBank").text(parseFloat(data.BankAuth).toFixed(2));
            }
        }
    });


}
function ValidateVoucher(billId) {

    $.ajax({
        type: "POST",
        url: "/payment/GetVoucherPayment/" + billId,
        async: false,
        error: function (xhr, status, error) {
            unblockPage();

            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);
            return false;
        },
        success: function (data) {

            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");

            if (data != '') {
                voucherPayment = data.Payment;
                paymentType = data.PaymentType;
                if (paymentType === "C") {
                    if (voucherPayment <= balanceCashAmount) {
                        isValid = true;
                    }
                    else {
                        isValid = false;
                    }
                }
                else if (paymentType === "Q") {
                    if (voucherPayment <= balanceChequeAmount) {
                        isValid = true;
                    }
                    else {
                        isValid = false;
                    }
                }

            }
        }
    });
}

//added by vikram 28-8-2013 end

function clearValidation(formElement) {

    if (MasterTriggerWhenError == false) {

        var id = $(formElement).attr('id');

        //Removes validation from input-fields
        $("#" + id + " .input-validation-error").addClass('input-validation-valid');
        $("#" + id + " .input-validation-error").removeClass('input-validation-error');
        //Removes validation message after input-fields
        $("#" + id + " .field-validation-error").addClass('field-validation-valid');
        $("#" + id + " .field-validation-error").removeClass('field-validation-error');
        //Removes validation summary 
        $("#" + id + " .validation-summary-errors").addClass('validation-summary-valid');
        $("#" + id + " .validation-summary-errors").removeClass('validation-summary-errors');
    }


}