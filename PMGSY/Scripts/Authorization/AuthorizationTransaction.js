//#region file header

   /*  Name : AuthorizationTransaction.js
    
    *  Path : ~Script\Authorization\AuthorizationTransaction.js
    
    *  Description : AuthorizationTransaction.js is javascript file used for adding ,editing ,finalizing ,deleting the authorization details request.
                    
    *  Functions : 

    *  Author : Amol Jadhav (PE, e-gov)
    *  Company : C-DAC,E-GOV
    *  Dates of creation : 06/06/2013  

   */                                      

//#endregion  file header

var changeNarrationPayAuth = false; //variable to keep track of whther to populate narration of payment head
var changeNarrationDedAuth = false;
var showContractorNarration = false; //variable to keep track if narration is to be set true when contractor suplier required 
var getContractorNameFromMasterEntry = true;//for narration if contractor name is to be taken fm master payment entry

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

//added by Koustubh Nakate on 03/10/2013 for client side agreement validation
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

    // alert(totalAmt);
    // alert(parseFloat(value));

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

    if (dedAmountToEdit >= parseFloat(value)) {
        return true;
    }
    else {
        if (totalAmt < parseFloat(value)) {
            return false;
        }
        else if (totalAmt >= parseFloat(value)) {
            return true

        }
        else {
            return false;
        }
    }

}, "Deduction amount must be less than or equal to " + valueAmt);

//#endregion


var TriggerWhenError = false

$(document).ready(function () {

    var isAgreementRequired = false;
   
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

        if ($("#HEAD_ID_P").val() == 0) {
            $("#HeadDescTr").hide('slow');
        }

        if ($("#divDetailsError").is(":visible")) {
            $("#divDetailsError").html('');
            $("#divDetailsError").hide('slow');
        }

    });

    $("#btnPaymentCancel").click(function () {

        TriggerWhenError = false;

        //$("#HEAD_ID_P").trigger('change');

        $(':input', '#PaymentTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

        clearValidation($("#PaymentTransactionForm"));

        $("#btnPaymentUpdate").hide();
        $("#btnPaymentSubmit").show();
        $("#btnPaymentReset").show();
        $("#btnPaymentCancel").hide();

        //added by Koustubh Nakate on 03/10/2013
        if ($("#HEAD_ID_P").val() == 0) {
            $("#HeadDescTr").hide('slow');
        }

        //if ($('#IMS_AGREEMENT_CODE_C').is(':disabled')) {

        //    $('#IMS_AGREEMENT_CODE_C').attr('disabled', false);
        //    $('#IMS_AGREEMENT_CODE_C').val(0);
        //}

    });

    
    //event to reset the deduction form
    $("#btnDeductionReset").click(function () {

        clearValidation($("#DeductionTransactionForm"));

        $(':input', '#DeductionTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

        $("#btnDeductionUpdate").hide();

        $("#btnDeductionSubmit").show();

        //added by Koustubh Nakate on 03/10/2013
        if ($("#HEAD_ID_D").val() == 0) {
            $("#headDescDedTR").hide('slow');
        }


    });


    //event to reset the deduction form
    $("#btnDeductionCancel").click(function () {

        clearValidation($("#DeductionTransactionForm"));

        $(':input', '#DeductionTransactionForm').not(':disabled').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

        $("#btnDeductionUpdate").hide();
        $("#btnDeductionSubmit").show();
        $("#btnDeductionReset").show();
        $("#btnDeductionCancel").hide();
       

        //added by Koustubh Nakate on 03/10/2013
        if ($("#HEAD_ID_D").val() == 0) {
            $("#headDescDedTR").hide('slow');
        }

        //if ($('#IMS_AGREEMENT_CODE_DED').is(':disabled')) {

        //    $('#IMS_AGREEMENT_CODE_DED').attr('disabled', false);
        //    $('#IMS_AGREEMENT_CODE_DED').val(0);
        //}

    });


    $('#HideShowTransForm').click(function () {

        $("#PaymentTransactionForm").toggle('slow', function () { });
        $("#DeductionTransactionForm").toggle('slow', function () { });

        $('#iconSpan2').toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");

    });

    //populate roads based on the contractor aggrement
    $('#IMS_AGREEMENT_CODE_C').change(function () {

        if ($('#IMS_AGREEMENT_CODE_C').val() !== 0 && $('#IMS_AGREEMENT_CODE_C').val() != "") {

            $("#IMS_PR_ROAD_CODE").empty();

            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoad/" + $("#IMS_AGREEMENT_CODE_C option:selected").val() + "$" + $("#HEAD_ID_P option:selected").val() + "$" + $("#CONTRACTOR_ID").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#IMS_AGREEMENT_CODE_C option:selected").text() }));//new change done by Vikram on 16-10-2013

            //FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoad/" + $("#IMS_AGREEMENT_CODE_C").val() + "$" + $("#HEAD_ID_P").val() + "$" + $("#CONTRACTOR_ID").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#IMS_AGREEMENT_CODE_C option:selected").text() }));

            $('#IMS_AGREEMENT_CODE_DED').attr("disabled", "disabled").val($('#IMS_AGREEMENT_CODE_C').val())

            $("#agreementSpan").text($('#IMS_AGREEMENT_CODE_C :selected').text().trim());
        }
        else {
            $("#agreementSpan").text("");
        }

        SetNarration();

    });

    //chage event of road code
    $('#IMS_PR_ROAD_CODE').change(function () {

        if (roadRequired && $('#IMS_PR_ROAD_CODE').val() != 0) {

            if (Bill_ID == 0 || Bill_ID == null) {

                Bill_ID = $("#Bill_ID").val();
            }

            FillInCascadeDropdown(null, "#FINAL_PAYMENT", "/Authorization/GetFinalAuthorizationDetails/" + Bill_ID + "/" + $("#IMS_PR_ROAD_CODE").val());

            // for showing final payment option to only Construction of New Works and Upgradation of New Works
            if ($("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q") {
                $(".final").show('slow');
            }
            else {
                $(".final").hide();
            }

            $("#roadSelectedSpan").text($('#IMS_PR_ROAD_CODE :selected').text().trim());
        }
        else {
            $(".final").hide();
        }
        SetNarration();
    });

    //if cash amount is 0 disable it
    if (parseFloat(TotalAmtToEnterChqAmount) == 0) {

        $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        $("#cashAmtTr").hide();
    }
    else {
        $("#AMOUNT_C").removeAttr("readonly");
        $("#cashAmtTr").show();
    }

    //change event of deduction transaction change to get the agrrement details entered in payment deatails
    $("#HEAD_ID_D").change(function () {

        if (Bill_ID == 0 || Bill_ID == null || Bill_ID == "") {
            Bill_ID = $("#Bill_ID").val();
        }

        if ($("#HEAD_ID_D").val() == 0) {
          
            $("#NARRATION_D").text('');
            return false;
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

                    if (changeNarrationDedAuth == true || (!authTransEdit)) {
                        $("#NARRATION_D").text(resultData[0]);
                    }

                }
            }
        });


        $.ajax({
            type: "POST",
            url: "/Authorization/GetSelectedAgreementForTransaction/" + Bill_ID,
            async: false,
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
                    $("#AgreementDed").show();
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
                    $("#AgreementDed").show();
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


    });

    //populate the sanction packagge based on sanction year
    $("#IMS_SANCTION_YEAR").change(function () {

        if ($("#IMS_SANCTION_YEAR").val() != 0) {
            FillInCascadeDropdown(null, "#IMS_SANCTION_PACKAGE", "/Payment/PopulateSactionPackage/" + $("#IMS_SANCTION_YEAR").val());

            $(".sanctionPackage").show();
        }

    });

    //populate the road based on sanction package
    $("#IMS_SANCTION_PACKAGE").change(function () {

        if ($("#IMS_SANCTION_PACKAGE").val() != 0) {
            FillInCascadeDropdown(null, "#IMS_PR_ROAD_CODE", "/Payment/PopulateRoadbyPackageYear/" + $("#IMS_SANCTION_PACKAGE").val());

            $(".road").show();
        }

    });

    //for invalid trnasaction type
    $("#HEAD_ID_P > option").each(function () {

        if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            $(this).addClass("X");
            $(this).css("color", "#b83400");

        }
    });

    //for invalid trnasaction type
    $("#HEAD_ID_D > option").each(function () {

        if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            $(this).addClass("X");
            $(this).css("color", "#b83400");

        }
    });

    //payment transaction change event
    $("#HEAD_ID_P").change(function () {

        if (Bill_ID == 0 || Bill_ID == null || Bill_ID == "") {
            Bill_ID = $("#Bill_ID").val();
        }


        if ($("#HEAD_ID_P").val() == 0) {
            resetDetailsForm();
            return false;
        }

        //change don by Vikram on 26 March 2014 -- in edit mode all (new / upgrade roads were populated)
        $("#IMS_AGREEMENT_CODE_C").val('0');

        // for showing final payment option to only Construction of New Works and Upgradation of New Works
        if ($("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q") {
            $(".final").show('slow');
        }
        else {
            $(".final").hide();
        }

        //alert('a');

        blockPage();
        $.ajax({
            type: "POST",
            url: "/payment/getDetailsDesignParam/" + $("#HEAD_ID_P").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");


                if (TriggerWhenError == false) {
                    clearValidation($("#PaymentTransactionForm"));
                }
                if (data != "") {

                    if (data.ShowContractor) {
                        showContractorNarration = true;
                        getContractorNameFromMasterEntry = false;

                    }
                    else {

                        showContractorNarration = false;
                    }

                    if (data.SupplierRequired == "Y") {

                        showContractorNarration = true;
                        if ($("#IMS_AGREEMENT_CODE_S").val() == 0 || $("#IMS_AGREEMENT_CODE_S").val() == "") {
                            if ($('#IMS_AGREEMENT_CODE_S  option').length == 1)
                            {
                                FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_S", "/Authorization/GetSupplierContractorAgreement/" + Bill_ID);
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
                    else
                    {
                        $('.supAgreement').hide('slow');
                        showContractorNarration = false;
                    }

                    if (data.RoadRequired == "Y") {
                        $('.road').show('slow');

                        roadRequired = true;

                        $('#IMS_PR_ROAD_CODE').rules('add', {
                            drpRequired: true,
                            messages: {
                                drpRequired: 'Road is Required'
                            }
                        });
                    }
                    else {
                        roadRequired = false;
                        $('.road').hide('slow');
                        //get if final payment drodown details 

                    }

                    if (data.PiuRequired == "Y") {

                        if ($("#MAST_DPIU_CODE").val() == 0 || $("#MAST_DPIU_CODE").val() == "") {

                            if ($('#MAST_DPIU_CODE  option').length == 1)
                            {
                                FillInCascadeDropdown(null, "#MAST_DPIU_CODE", "/Payment/GetPIU/" + Bill_ID);
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
                    if (data.ContractorRequired == "Y") {

                        showContractorNarration = true;

                        if ($("#IMS_AGREEMENT_CODE_C").val() == 0 || $("#IMS_AGREEMENT_CODE_C").val() == "") {

                            if ($('#IMS_AGREEMENT_CODE_C  option').length == 1)
                            {
                                FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_C", "/Authorization/GetSupplierContractorAgreement/" + Bill_ID);
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
                    else {
                        $('.conAgreement').hide('slow');
                        showContractorNarration = false;
                    }
                    //if road required but agreement is not required
                    if (data.RoadRequired == "Y" && data.AgreementRequired == "N") {
                        $(".road").show();
                        $("#supAgreement").hide();
                    }

                    if (data.YearRequired == "Y") {

                        if ($("#IMS_SANCTION_YEAR").val() == 0 || $("#IMS_SANCTION_YEAR").val() == "") {

                            if ($('#IMS_SANCTION_YEAR  option').length == 1) {
                                FillInCascadeDropdown(null, "#IMS_SANCTION_YEAR", "/Payment/populateSactionYear/");
                            }
                        }
                        $(".sanctionYear").show('slow');

                    }
                    else {
                        $(".sanctionYear").hide('slow');

                    }

                    if (data.PackageRequired == "Y") {
                        $(".sanctionPackage").show('slow');
                    }
                    else {
                        $(".sanctionPackage").hide('slow');
                    }

                             

                    if (data.AgreementRequired == "Y" && Bill_ID != null && Bill_ID != "") {

                        isAgreementRequired = true;

                        $.ajax({
                            type: "POST",
                            url: "/Authorization/GetSelectedAgreementForTransaction/" + Bill_ID,
                            async: false,
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

                                        if ($("#IMS_AGREEMENT_CODE_S") != null) {
                                            $("#IMS_AGREEMENT_CODE_S").attr('disabled', 'disabled');
                                        }

                                        if (TriggerWhenError == false)
                                            $('#IMS_AGREEMENT_CODE_S').trigger('change');

                                    }
                                    else if (data.ContractorRequired == "Y" && data.ShowContractor == false) {


                                        $('#IMS_AGREEMENT_CODE_C').val(data1);
                                        //$('#IMS_AGREEMENT_CODE_C').attr('selected','selected');
                                        
                                        if ($("#IMS_AGREEMENT_CODE_C") != null && $("#IMS_AGREEMENT_CODE_C").val() != 0) {
                                            $("#IMS_AGREEMENT_CODE_C").attr('disabled', 'disabled');

                                            //  $('#IMS_AGREEMENT_CODE_C').trigger('change');
                                        }
                                        
                                        if (TriggerWhenError == false)
                                                $('#IMS_AGREEMENT_CODE_C').trigger('change');
                                            
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

                    } else {
                        $(".AgreementDed").hide("slow");
                    }
                    // applyAltRowSyle($("form"));
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

                   // alert("changeNarrationPayAuth " + changeNarrationPayAuth + " opeartion " + opeartion + " opeartion  " + opeartion)

                    if (changeNarrationPayAuth == true || (!authTransEdit)) {

                        $("#HeadNarrationSpan").text(resultData[0]);

                        if (!showContractorNarration)
                        {
                            $("#NARRATION_P").val(resultData[0]);

                        } else
                        {
                            SetNarration();
                        }
                    }

                }
            }
        });
    });


    //$("#HEAD_ID_P").trigger('change'); commented by Vikram

    //function to submit transaction details
    $("#btnPaymentSubmit").click(function (e) {

        if (Bill_ID == 0 || Bill_ID == null) {

            Bill_ID = $("#Bill_ID").val();
        }

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

        $('#AMOUNT_Q').rules('add', {
            required: true,
            greaterThanZero: true,
            lessOrEqualToChequeAmountRemaining: true,
            messages: {
                required: 'Amount is Required',
                greaterThanZero: 'Amount must be greater than 0',
                lessOrEqualToChequeAmountRemaining: 'Amount must be less than or equal to Diffrence To Be Entered for cheque Amount',
            }
        });
       

        if (!$('#AMOUNT_C').is("readonly")) {
            $('#AMOUNT_C').rules('add', {
                required: true,
                lessOrEqualToCashAmountRemaining: true,
                lessOrEqualToChequeAmount: true,
                messages: {
                    required: 'Cash Amount is Required',
                    lessOrEqualToCashAmountRemaining: "Cash Amount must be less than or equal to Diffrence To Be Entered for Cash Amount",
                    lessOrEqualToChequeAmount: "Cash Amount Should be less than or equal to Cheque Amount"
                }
            });
        }


        //added by Koustubh Nakate on 03/10/2013

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

       
        e.preventDefault();


        if ($("#PaymentTransactionForm").valid()) {
            blockPage();

            $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
            $("#HEAD_ID_P").removeAttr('disabled');

            $.ajax({
                type: "POST",
                url: "/Authorization/AddPaymentTransactionDetails/" + Bill_ID,
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
                        $("#AuthDetailsEntryDiv").html(data);
                        $.validator.unobtrusive.parse($("#AuthDetailsEntryDiv"));
                        TriggerWhenError = true;
                      //  alert('b');
                        $("#HEAD_ID_P").trigger('change');

                    }
                    else if (data.Success) {

                        //reload the payment transaction grid
                        
                        $("#PaymentGridDivList").jqGrid().setGridParam
                        ({ url: '/Authorization/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(Bill_ID);

                        $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        
                        resetDetailsForm();
                        //if master transaction doesent have multiple transaction allowed disable the transaction payment head and set the value
                        if (data.disblehead) {
                            $("#HEAD_ID_P").attr('disabled', 'disabled');
                            $("#HEAD_ID_P").val(data.head);
                        }
                        else {
                            $("#HEAD_ID_P").removeAttr('disabled');
                        }

                        $("#HEAD_ID_P").trigger('change');
                        //hide the is final payment row
                        $(".final").hide('slow');

                        if ($("#HEAD_ID_P").val() == 0) {
                            $("#HeadDescTr").hide('slow');
                        }

                        alert("Payment Details Added successfully.");

                        if ($("#divDetailsError").is(":visible")) {
                            $("#divDetailsError").html('');
                            $("#divDetailsError").hide('slow');
                        }

                        return false;


                    }
                    else if (data.Success == false && data.status == "-1") {
                        alert("This transaction cant be added ,master payment is already finalized");
                        return false;
                    }
                    else if (data.Success == false && data.status == "-555")
                    {
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                        //Added By Abhishek 4 Sep 2014 for PMGSY Scheme selected road validation
                    else if (data.Success == false && data.status == "-999") {
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                    else {


                    }

                }
            });
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

        $('#AMOUNT_Q').rules('add', {
                required: true,
                greaterThanZero: true,
                lessOrEqualToChequeAmountRemainingEdit: true,
                messages: {
                    required: 'Cheque Amount is Required',
                    greaterThanZero: 'Cheque Amount must be greater than 0'
                   , lessOrEqualToChequeAmountRemainingEdit: 'Invalid Cheque Amount.its greater than remaining cheque amount to enter)'
                }
            });
      
        if (!$('#AMOUNT_C').is("readonly")) {
            $('#AMOUNT_C').rules('add', {
                required: true,
                lessOrEqualToCashAmountRemainingEdit: true,
                lessOrEqualToChequeAmount: true,
                messages: {
                    required: 'Cash Amount is Required'
                   , lessOrEqualToCashAmountRemainingEdit: "Invalid Cash Amount,its greater than remaining cheque amount to enter",
                    lessOrEqualToChequeAmount: "Cash Amount Should be less than or equal to Cheque Amount"
                }
            });
        }

        //added by Koustubh Nakate on 03/10/2013
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

        if ($("#PaymentTransactionForm").valid()) {

            blockPage();

            $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
            $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');

            $("#HEAD_ID_P").removeAttr('disabled');

            $.ajax({
                type: "POST",
                url: "/Authorization/PostEditTransactionDetails/" + urlparamsForTransEdit,
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
                        $("#AuthDetailsEntryDiv").html(data);
                        $.validator.unobtrusive.parse($("#AuthDetailsEntryDiv"));
                        TriggerWhenError = true;
                        $("#HEAD_ID_P").trigger('change');
                        $("#btnPaymentUpdate").show();
                        $("#btnPaymentSubmit").hide();

                    }
                    else if (data.Success) {

                        $("#PaymentGridDivList").jqGrid().setGridParam
                        ({ url: '/Authorization/GetPaymentDetailList/' + data.Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(data.Bill_ID);

                     
                        $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();


                        //if master transaction doesent have multiple transaction allowed disable the transaction payment head and set the value
                        if (data.disblehead) {
                            $("#HEAD_ID_P").val(data.head);
                            $("#HEAD_ID_P").attr('disabled', 'disabled');
                        }
                        else {
                            $("#HEAD_ID_P").removeAttr('disabled');
                        }

                        $("#HeadDescTr").hide('slow');

                        alert("Authorization Details updated successfully.");

                        if ($("#divDetailsError").is(":visible")) {
                            $("#divDetailsError").html('');
                            $("#divDetailsError").hide('slow');
                        }

                        return false;

                    }
                    else if (data.Success == false && data.Bill_ID == "-1") {
                        alert("This transaction cant be edited ,Authorization Request  is already finalized");
                        return false;
                    }
                    else if (data.Success == false && data.status == "-555") {
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                        //Added By Abhishek 4 Sep 2014 for PMGSY Scheme selected road validation
                    else if (data.Success == false && data.status == "-999") {
                        $("#divDetailsError").show("slide");
                        $("#divDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                    else {


                    }

                }
            });
        }


    });

    //event to save the deduction details
    $("#btnDeductionSubmit").click(function (e) {
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
            return false;
        }

       
        e.preventDefault();

        if ($("#DeductionTransactionForm").valid()) {

            $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');

            blockPage();
            $.ajax({
                type: "POST",
                url: "/Authorization/AddDeductionTransactionDetails/" + Bill_ID,
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
                        $("#AuthDetailsEntryDiv").html(data);
                        $.validator.unobtrusive.parse($("#AuthDetailsEntryDiv"));

                    }
                    else if (data.Success) {

                        //reload the payment transaction grid

                        $("#PaymentGridDivList").jqGrid().setGridParam
                        ({ url: '/Authorization/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(Bill_ID);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();

                        if ($("#HEAD_ID_D").val() == 0) {
                            $("#headDescDedTR").hide('slow');
                        }

                        alert("Deduction Details Added successfully.");

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


        e.preventDefault();

        if ($("#DeductionTransactionForm").valid()) {

            $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');

            blockPage();
            $.ajax({
                type: "POST",
                url: "/Authorization/PostEditTransactionDetails/" + urlparamsForTransEdit,
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
                        $("#AuthDetailsEntryDiv").html(data);
                        $.validator.unobtrusive.parse($("#AuthDetailsEntryDiv"));
                        $("#btnDeductionUpdate").show();
                        $("#btnDeductionSubmit").hide();
                    }
                    else if (data.Success) {

                        $("#PaymentGridDivList").jqGrid().setGridParam
                       ({ url: '/Authorization/GetPaymentDetailList/' + data.Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(data.Bill_ID);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();

                        $("#headDescDedTR").hide('slow');

                        alert("Deduction Details Updated successfully.");

                        return false;

                    }
                    else if (data.Success == false && data.Bill_ID == "-1") {
                        alert("This transaction cant be edited ,Authorization request is already finalized");
                        return false;
                    }
                    else {



                    }

                }
            });
        }


    });

    //function to finalize the voucher
    $("#finalize").unbind().click(function() { 

      
        if (confirm("Are you sure you want to finalize the Authorization  Request? ")) {
      
            if (Bill_ID == 0 || Bill_ID == null)
            {
                Bill_ID = $("#Bill_ID").val();
            }

        $.ajax({
            type: "POST",
            url: "/Authorization/FinalizeAuthorization/" + Bill_ID,
            //async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                return false;
            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();
               
                if (data.result == "1")
                {
                   
                    alert("Authorization Finalized Successfully.");
                    blockPage();
                    $("#mainDiv").load("/Authorization/GetAuthorizationRequest/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(), function () {
                        unblockPage();
                    });
                    return false;
                }
                else if (data.result == "-1") {
                    alert("Authorization cant be Finalized as all transaction details are not entered.");
                    return false;
                }
                else {
                    alert("Error while finalizing the Authorization.");
                    return false;
                }
            }
        });
    }
      
        return false;
    });


    //on change event of the cheque amount find out the the deduction amount
    $("#AMOUNT_Q").bind('change', function () {

        if (TotalAmtToEnterDedAmount != 0 && TotalAmtToEnterChqAmount != 0)
        {
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

});//document ready



function resetDetailsForm() {
    //$('#TransactionForm').trigger('reset');
    $('.conAgreement').hide('slow');
    $('.piu').hide('slow');
    $('.road').hide('slow');
    $('.supAgreement').hide('slow');
    $("#btnDeductionUpdate").hide();
    $("#btnDeductionSubmit").show();
    $("#btnPaymentUpdate").hide();
    $("#btnPaymentSubmit").show();
    // new change done by Vikram on 27-09-2013
    $("#descPayTd").html('');
    $("#btnDeductionReset").show();
    $("#btnDeductionCancel").hide();
    $("#btnPaymentCancel").hide();
    $("#btnPaymentReset").show();
    //end of change

}

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()
    $.post(action, map, function (data) {

        $.each(data, function () {
            // alert('TEST' + this.Selected);
            // alert("fillCascaded =" + this.Value);
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}

//function to set the narration when contractor payment
function SetNarration() {
    if (showContractorNarration) {
        //get the contractor name from master grid
        if (getContractorNameFromMasterEntry) {
            $("#CompanyNameSpan").text($("#AuthorizationList > tbody > tr:nth-child(" + 2 + ") td:nth-child(" + 5 + ")").text().trim());
        }
        $("#NARRATION_P").val($("#HiddenNarrationSpan").text().trim());
    }
}




