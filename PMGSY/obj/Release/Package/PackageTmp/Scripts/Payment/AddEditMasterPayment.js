/*
This file is used for functionality of the master payment entry 

*/

var MasterTriggerWhenError = false;//var to keep track if validations is to be reapplied when server error occured while master payment add and edit
var availableCheques = []; //array to hold the available cheques for master payment
var balanceCashAmount = 0;//var to hold the payment Cash Amount balance
var balanceChequeAmount = 0;//var to hold the payment cheque Amount balance
var masterChqAmtToEdit = 0; //var to hold the master cheque amount which is to be edited
var masterCashAmount = 0; //var to hold the master cash amount which is to be edited
var isValid = undefined; // var to check whether data is valid or not
$(function () {

    $.validator.unobtrusive.adapters.add('isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });

    $.validator.addMethod("isdateafter", function (value, element, params) {
        //alert($('#'+params.propertytested).val());
        //var fullDate = new Date();
        var fullDate = process($('#' + params.propertytested).val());
        var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + fullDate.getFullYear();
        return (params.allowequaldates) ? process(currentDate) >= process(value) : process(currentDate) > process(value);
    });

    $.validator.unobtrusive.adapters.add('isvaliddate', ['month', 'year', 'chqepay'], function (options) {
        options.rules['isvaliddate'] = options.params;
        options.messages['isvaliddate'] = options.message;
    });

    $.validator.addMethod("isvaliddate", function (value, element, params) {
        //for chalana date
        if (value == "") {
            return true;
        }
        var month = $('#' + params.month).val();
        var year = $('#' + params.year).val();
        if (params.chqepay != "") {
            var IsCheque = $('input:radio[name="' + params.chqepay + '"]:checked').val();
            if (IsCheque == "Q" || IsCheque == "R" || IsCheque == "E") {
                return ($('#' + params.month).val() == value.split('/')[1].replace(/^0+/, '')) && ($('#' + params.year).val() == value.split('/')[2])
            }
            else {
                return true;
            }
        }
        else {
            return ($('#' + params.month).val() == value.split('/')[1].replace(/^0+/, '')) && ($('#' + params.year).val() == value.split('/')[2])
        }

    }, function (params, element) {
        return "Date must be in " + $('#BILL_MONTH option[value=' + $('#BILL_MONTH').val() + ']').text() + " month and " + $('#BILL_YEAR').val() + " year";
    });

    //Added By Abhishek kamble for chqissuedate validation 
    $.validator.unobtrusive.adapters.add('isvalidchqissuedate', ['cheissuedate', 'chqepay', 'ischqissuedatevalid'], function (options) {
        options.rules['isvalidchqissuedate'] = options.params;
        options.messages['isvalidchqissuedate'] = options.message;
    });

    $.validator.addMethod("isvalidchqissuedate", function (value, element, params) {

        //var month = $('#' + params.month).val();
        //var year = $('#' + params.year).val();
        if (params.chqepay != "") {
            var IsCheque = $('input:radio[name="' + params.chqepay + '"]:checked').val();

            if (IsCheque == "Q") {

                var bill_date = value;
                var Issue_date = $("#ChequeBookIssueDate").val();
                bill_date = bill_date.split('/');
                Issue_date = Issue_date.split('/');
                var new_bill_date = new Date(bill_date[2], parseInt(bill_date[1]) - 1, bill_date[0]);
                var new_Issue_date = new Date(Issue_date[2], parseInt(Issue_date[1]) - 1, Issue_date[0]);
                if (new_bill_date < new_Issue_date) {
                    return false;
                }
                else {
                    return true;
                }

            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    });

});

jQuery.validator.addMethod("CheckChequeNumber", function (value, element) {

    // do not check for State
    //alert(parseFloat($("#LevelID").val()));

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }

    if (value == "")
    { return false; }

    else if (jQuery.inArray(value, availableCheques) != -1) {
        return true;
    }
    else {
        return false;
    }

}, "Invalid Cheque Number");

//validation added for CHQ_NO at SRRDA 23-July-2014 start
//Uncommented by Abhishek for Advice no 7Apr2015
jQuery.validator.addMethod("SixDigitChequeNo", function (value, element) {
    if (value.length == 6) {
        return true;
    }
    else {
        return false;
    }

}, "");
//validation added for CHQ_NO at SRRDA 23-July-2014 end

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

function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

//client validation for cheque series 
jQuery.validator.addMethod("ChequeSeriesdrpRequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == "")
    { return false; }
    else if ((parseFloat(value) == 0)) {
        return false;
    }
    else {
        return true;
    }

}, "");

//client validation for department name 
jQuery.validator.addMethod("isdepartmentnamerequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == "")
    { return false; }
    else if ((parseFloat(value) == 0)) {
        return false;
    }
    else {
        return true;
    }

}, "");

jQuery.validator.addMethod("iscontractorrequired_C", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == "")
    { return false; }
    else if ((parseFloat(value) == 0)) {
        return false;
    }
    else {
        return true;
    }

}, "");

jQuery.validator.addMethod("iscontractorrequired_S", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == "")
    { return false; }
    else if ((parseFloat(value) == 0)) {
        return false;
    }
    else {
        return true;
    }

}, "");


//client side validation for cheque and Epayment amount 
jQuery.validator.addMethod("lessOrEqualToBankAuthBalance", function (value, element) {

    if (balanceChequeAmount < parseFloat(value)) {
        return false
    }
    else {
        return true;
    }

}, "Amount  must be less than or equal to Bank Authorization balance amount ");

//client side validation for cash amount
jQuery.validator.addMethod("lessOrEqualToCashAmountBalance", function (value, element) {

    if (balanceCashAmount < parseFloat(value)) {
        return false
    }
    else {
        return true;
    }

}, "Amount must be less than or equal to Cash Balance Amount");

//client side validation for bank auth while editing
jQuery.validator.addMethod("lessOrEqualToBankAuthBalanceEdit", function (value, element) {


    if (opeartion == "E") {


        var totalAmt = parseFloat(balanceChequeAmount) + parseFloat(masterChqAmtToEdit);

        // alert(totalAmt);
        // alert(parseFloat(value));

        //if new amount is less than or equal to current amount => valid new  amount 
        if (balanceChequeAmount >= parseFloat(value)) {
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
    }
    else {
        return true;
    }

}, "Amount must be less than or equal to Bank Authorization balance amount ");

//client side validation for cash while editing
jQuery.validator.addMethod("lessOrEqualToCashBalanceEdit", function (value, element) {

    if (opeartion == "E") {

        var totalAmt = parseFloat(balanceCashAmount) + parseFloat(masterCashAmount);

        // alert(totalAmt);
        // alert(parseFloat(value));

        //if new amount is less than or equal to current amount => valid new  amount 
        if (balanceCashAmount >= parseFloat(value)) {
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
    }
    else {

        return true;
    }

}, "Amount must be less than or equal to Cash balance amount");


jQuery.validator.addMethod("shouldBeLessThanChequeAmount", function (value, element) {

    if (value == "" || value == null) {
        return true;
    }

    if (value == "" && $("#CHQ_AMOUNT").val() == "") {
        return true;
    }

    var deductionAmount = parseFloat(value);
    var ChequeAmount = parseFloat($("#CHQ_AMOUNT").val())

    if (deductionAmount && ChequeAmount) {

        if (ChequeAmount < deductionAmount) {
            return false;
        }
        else {
            return true;
        }

    }
    else if (ChequeAmount == 0) {//Else if Condition Added By Abhishek kamble 21-Apr-2014
        if (ChequeAmount < deductionAmount) {
            return false;
        }
        else {
            return true;
        }
    }
    else {

        return true;
    }


}, "Deduction Amount should be less than  Amount.");


//jQuery.validator.addMethod("greaterThanZero", function (value, element) {

//    var amount = parseFloat(value);

//    if (amount) {

//        if (amount <= 0) {
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//    else {
//        return false;
//    }


//}, "Amount should be greater than zero");

//Added By Avinash on 28_08_2018
jQuery.validator.addMethod("greaterThanZero", function (value, element) {
    var amount = parseFloat(value);

    //TXN_ID:47$Q --->Contractor's Work Payment
    //TXN_ID:1484$Q --->Contractor's Work Payment - PMGSY II
    //TXN_ID:1788$Q --->Contractor's Work Payment - RCPLWE
    if (($("#TXN_ID option:selected").val() == "1788$Q") || ($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
        if ($("#Cheque").is(":checked")) {
            if (amount >= 0) {
                return true;
            }
            else {
                return false;
            }
            $("#spnBankName").html('');
            $("#spnIFSCCode").html('');
            $("#spnBankAccNumber").html('');
            $('#conAccountId').val('');
            $('#CONC_Account_ID').hide();
            $('#trContractorBankDetails').hide();
        }
        if ($("#Epay").is(":checked")) {
            if (amount == 0) {
                return false;
            }
            else {
                return true;
            }
            $("#spnBankName").html('');
            $("#spnIFSCCode").html('');
            $("#spnBankAccNumber").html('');
            $('#conAccountId').val('');
            $('#CONC_Account_ID').hide();
            $('#CONC_Account_ID').hide();
            $('#trContractorBankDetails').hide();
            // $('#CONC_Account_ID').hide();
            $('#trContractorBankDetails').hide();
        }


        if ($("#DeductionOnly").is(":checked")) {
            if (amount == 0) {
                return false;
            }
            else {
                return true;
            }
            $("#spnBankName").html('');
            $("#spnIFSCCode").html('');
            $("#spnBankAccNumber").html('');
            $('#conAccountId').val('');
            $('#CONC_Account_ID').hide();
            $('#trContractorBankDetails').hide();

            $('#trContractorBankDetails').hide();
        }


    }

    else {
        if (amount) {
            if (amount <= 0) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }
}, "Amount should be greater than zero");


var isValid = false;// var to hold the result of validation function(like Validating Epayment and ERemittence)




$(document).ready(function () {
    //Added By Abhishek kamble 11-Apr-2014 start    

    //if (($("#TXN_ID option:selected").val() == "47$Q")) {
    //    $("#CHQ_AMOUNT").val(0);
    //}

    //Added By Abhishek kamble 29-May-2014
    $('#CONC_Account_ID').hide();

    $('#trContractorBankDetails').hide();
    $(function () {

        $("#CHQ_AMOUNT").blur(function () {
            //Cont Work Payment

            //Added by Avinash on 28/08/2018
            if ($("#Cheque").is(":checked")) {
                if (($("#TXN_ID option:selected").val() == "1788$Q")) {
                    if ($("#CHQ_AMOUNT").val() == 0 && $("#CHQ_AMOUNT").val() != "") {
                        // $("#chqseriesTr").hide();
                        $(".chequeTr").hide();

                    }
                }
            }

            if ($("#Cheque").is(":checked")) {

                if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3

                    if ($("#CHQ_AMOUNT").val() == 0 && $("#CHQ_AMOUNT").val() != "") {
                        //$("#chqseriesTr").hide();
                        $(".chequeTr").hide();
                    } else {
                        //$("#chqseriesTr").show();
                        $(".chequeTr").show();
                    }
                }
            }
        });

    });

    //Added By Abhishek kamble 20-jan-2014 start    
    var currentDate = $("#CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
    //Added By Abhishek kamble 20-jan-2014 end

    //Added By Abhishek kamble 3-jan-2014 start change
    GetClosedMonthAndYear();

    $.validator.unobtrusive.parse($("#masterPaymentForm"));



    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD'))
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    })



    //get the Payment balances
    getPaymentClosingBalance($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

    //alert('IN ADD' + opeartion);

    if ((opeartion == 'A') || (opeartion == '')) {

        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (data != "") {
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);
                }
            }
        });
    }

    //for invalid trnasaction type
    $("#TXN_ID > option").each(function () {

        if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            $(this).addClass("X");
            $(this).css("color", "#b83400");

        }
    });

    // change the rule on checkbox and update displayed message dynamically
    $('#Cash').on('change', function () {
        if ($(this).is(':checked')) {

            clearValidation($("#masterPaymentForm"));

            $('#CASH_AMOUNT').rules('add', {
                // maxlength: 14,
                required: true,
                lessOrEqualToCashAmountBalance: true,
                lessOrEqualToCashBalanceEdit: true,
                greaterThanZero: true,
                messages: {
                    required: 'Amount is Required',
                    lessOrEqualToCashAmountBalance: 'Amount must be less than or equal to Cash Balance Amount',
                    lessOrEqualToCashBalanceEdit: 'Amount must be less than or equal to Cash balance amount',
                    greaterThanZero: "Amount Should be greater than zero"
                }
            });

            $('#CASH_AMOUNT').rules('add', {
                // maxlength: 14,
                required: true,
                messages: {
                    required: 'Amount is Required'
                }
            });


            $("#DEDUCTION_AMOUNT").rules("remove", "greaterThanZero");


        } else {
            $('#CHQ_Book_ID').rules("remove", "messages");
            $('#CHQ_NO').rules("remove", "messages");
            $('#CHQ_DATE').rules("remove", "messages");
            $('#CHQ_AMOUNT').rules("remove", "messages");
            $('#DEDUCTION_AMOUNT').rules("remove", "messages");
            $('#CASH_AMOUNT').rules("remove", "messages");
        }

    });

    //event for cheque change 
    $('#Cheque').on('change', function () {

        // alert('opeartion');


        if ((opeartion == 'A') || (opeartion == '')) {

            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                    }
                }
            });
        }

        if ($(this).is(':checked')) {

            clearValidation($("#masterPaymentForm"));


            //If Condition Added By Abhishek kamble 11-Apr-2014 start
            if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {
                $('#CHQ_NO').rules('add', {
                    // maxlength: 6,
                    required: true,
                    //SixDigitChequeNo:true,
                    CheckChequeNumber: function () { if (parseFloat($("#LevelID").val()) == 5) { return true; } else { return false; } },
                    messages: {
                        required: 'Cheque/Epayment Number is Required',
                        CheckChequeNumber: "Invalid Cheque Number",
                        //  SixDigitChequeNo: "Cheque Number must be 6 digit long"//Abhishek validation added for CHQ_NO at SRRDA 23-July-2014 
                    }
                });
            } else {
                if ($("#CHQ_NO").val() == "") {
                    $('#CHQ_NO').val(null);
                }
                $('#CHQ_NO').rules("remove", "messages");
                $('#CHQ_NO').rules("remove", "required");
                $('#CHQ_AMOUNT').rules("remove", "CheckChequeNumber");
            }

            //If Condition Added By Abhishek kamble 11-Apr-2014 start
            if (($("#TXN_ID option:selected").val() != "47$Q") && ($("#TXN_ID option:selected").val() != "737$Q") && ($("#TXN_ID option:selected").val() != "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                $('#CHQ_AMOUNT').rules('add', {
                    // maxlength: 10,
                    required: true,
                    lessOrEqualToBankAuthBalance: true,
                    lessOrEqualToBankAuthBalanceEdit: true,
                    greaterThanZero: true,
                    messages: {
                        required: 'Amount is Required',
                        lessOrEqualToBankAuthBalance: 'Amount  must be less than or equal to Bank Authorization balance amount',
                        lessOrEqualToBankAuthBalanceEdit: 'Amount  must be less than or equal to Bank Authorization balance amount',
                        greaterThanZero: "Amount Should be greater than zero"
                    }
                });
            }
            else {
                //Commented By Abhishek kamble 29-May-2014
                //if ($("#CHQ_AMOUNT").val() == "") {
                //    $('#CHQ_AMOUNT').val(0);
                //}

                //Added by Abhishek 29-May-2014
                $('#CHQ_AMOUNT').rules('add', {
                    required: true,
                    messages: {
                        required: 'Amount is Required',
                    }
                });

                //Commented by Abhishek 29-May-2014
                //$('#CHQ_AMOUNT').rules("remove", "messages");
                //$('#CHQ_AMOUNT').rules("remove", "required");
                //$('#CHQ_AMOUNT').rules("remove", "lessOrEqualToBankAuthBalance");
                //$('#CHQ_AMOUNT').rules("remove", "lessOrEqualToBankAuthBalanceEdit");
                //$('#CHQ_AMOUNT').rules("remove", "greaterThanZero");
            }
            //If Condition Added By Abhishek kamble 11-Apr-2014 end

            if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {
                $('#CHQ_DATE').rules('add', {
                    // maxlength: 10,
                    required: true,
                    messages: {
                        required: 'Cheque/Epayment date is Required'
                    }
                });
            }
            else {
                if ($("#CHQ_DATE").val() == "") {
                    $('#CHQ_DATE').val(null);
                }
                $('#CHQ_DATE').rules("remove", "messages");
                $('#CHQ_DATE').rules("remove", "required");
            }

            $('#CHQ_Book_ID').rules('add', {
                // maxlength: 10,
                ChequeSeriesdrpRequired: true,
                messages: {
                    ChequeSeriesdrpRequired: 'Cheque Series is Required'
                }
            });

            //$('#DEDUCTION_AMOUNT').rules('add', {
            //    shouldBeLessThanChequeAmount: true,
            //    messages: {
            //        //required: 'Deduction Amount Should be less than Amount'
            //        //change done by Vikram
            //        shouldBeLessThanChequeAmount: 'Deduction Amount Should be less than or equal to Amount'
            //    }
            //});

            $("#DEDUCTION_AMOUNT").rules("remove", "greaterThanZero");

            //added by koustubh nakate on 26/09/2013 to validate department name

            $('#DEPT_ID').rules('add', {

                isdepartmentnamerequired: true,
                messages: {
                    isdepartmentnamerequired: 'Department Name is Required'
                }
            });

            $('#MAST_CON_ID_C').rules('add', {

                iscontractorrequired_C: true,
                messages: {
                    iscontractorrequired_C: 'Company Name is Required'
                }
            });

            $('#MAST_CON_ID_S').rules('add', {

                iscontractorrequired_S: true,
                messages: {
                    iscontractorrequired_S: 'Company Name is Required'
                }
            });

        } else {
            $('#CHQ_Book_ID').rules("remove", "messages");
            $('#CHQ_NO').rules("remove", "messages");
            $('#CHQ_DATE').rules("remove", "messages");
            $('#CHQ_AMOUNT').rules("remove", "messages");
            $('#DEDUCTION_AMOUNT').rules("remove", "messages");
            $('#CASH_AMOUNT').rules("remove", "messages");

            $('#DEPT_ID').rules("remove", "messages");

            $('#MAST_CON_ID_C').rules("remove", "messages");
            $('#MAST_CON_ID_S').rules("remove", "messages");
        }

    });

    $('#Cheque').click(function () {

        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            error: function (xhr, status, error) {
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (data != "") {
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);
                }
            }
        });

    });
    // event for epay radio selection
    $('#Epay').on('change', function () {

        //alert('Epay');
        // alert(opeartion);

        if ((opeartion == 'A') || (opeartion == '')) {
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                    }
                }
            });
        }


        if ($(this).is(':checked')) {

            clearValidation($("#masterPaymentForm"));

            $('#EPAY_NO').rules('add', {
                // maxlength: 6,
                required: true,
                messages: {
                    required: 'Epayment Number is Required'
                }
            });

            $('#CHQ_DATE').rules('add', {
                // maxlength: 10,
                required: true,
                messages: {
                    required: 'Cheque/Epayment date is Required'
                }
            });

            $('#CHQ_AMOUNT').rules('add', {
                // maxlength: 10,
                required: true,
                lessOrEqualToBankAuthBalance: true,
                lessOrEqualToBankAuthBalanceEdit: true,
                greaterThanZero: true,
                messages: {
                    required: 'Amount is Required',
                    lessOrEqualToBankAuthBalance: 'Amount  must be less than or equal to Bank Authorization balance amount',
                    lessOrEqualToBankAuthBalanceEdit: 'Amount  must be less than or equal to Bank Authorization balance amount',
                    greaterThanZero: "Amount Should be greater than zero"
                }
            });

            //$('#DEDUCTION_AMOUNT').rules('add', {
            //    shouldBeLessThanChequeAmount: true,
            //    messages: {
            //        //required: 'Deduction Amount Should be less than Amount'
            //        //change done by Vikram
            //        shouldBeLessThanChequeAmount: 'Deduction Amount Should be less than or equal to Amount'
            //    }
            //});


            $("#DEDUCTION_AMOUNT").rules("remove", "greaterThanZero");

            $('#CHQ_NO').hide();
            $('#EPAY_NO').show();

            $('#MAST_CON_ID_C').rules('add', {

                iscontractorrequired_C: true,
                messages: {
                    iscontractorrequired_C: 'Company Name is Required'
                }
            });

            $('#BILL_DATE').val($('#currentDate').val());
            $('#CHQ_DATE').val($('#currentDate').val());

        } else {
            $('#CHQ_Book_ID').rules("remove", "messages");
            $('#CHQ_NO').rules("remove", "messages");
            $('#CHQ_DATE').rules("remove", "messages");
            $('#CHQ_AMOUNT').rules("remove", "messages");
            $('#DEDUCTION_AMOUNT').rules("remove", "messages");
            $('#CASH_AMOUNT').rules("remove", "messages");

            $('#MAST_CON_ID_C').rules("remove", "messages");
        }

    });

    // event for eremittance radio selection
    $('#ERem').on('change', function () {


        if ((opeartion == 'A') || (opeartion == '')) {
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                    }
                }
            });
        }


        if ($(this).is(':checked')) {

            clearValidation($("#masterPaymentForm"));

            $('#CHQ_NO').hide();
            $('#EPAY_NO').show();

            $('#EPAY_NO').rules('add', {
                // maxlength: 6,
                required: true,
                messages: {
                    required: 'Epayment Number is Required'
                }
            });

            $('#CHQ_DATE').rules('add', {
                // maxlength: 10,
                required: true,
                messages: {
                    required: 'Cheque/Epayment date is Required'
                }
            });

            $('#CHQ_AMOUNT').rules('add', {
                // maxlength: 10,
                required: true,
                lessOrEqualToBankAuthBalance: true,
                lessOrEqualToBankAuthBalanceEdit: true,
                greaterThanZero: true,
                messages: {
                    required: 'Amount is Required',
                    lessOrEqualToBankAuthBalance: 'Amount  must be less than or equal to Bank Authorization balance amount',
                    lessOrEqualToBankAuthBalanceEdit: 'Amount  must be less than or equal to Bank Authorization balance amount',
                    greaterThanZero: "Amount Should be greater than zero"
                }
            });

            //$('#DEDUCTION_AMOUNT').rules('add', {
            //    shouldBeLessThanChequeAmount: true,
            //    messages: {
            //        //required: 'Deduction Amount Should be less than Amount'
            //        //change done by Vikram
            //        shouldBeLessThanChequeAmount: 'Deduction Amount Should be less than or equal to Amount'
            //    }
            //});


            $("#DEDUCTION_AMOUNT").rules("remove", "greaterThanZero");



            //added by koustubh nakate on 26/09/2013 to validate department name

            $('#DEPT_ID').rules('add', {

                isdepartmentnamerequired: true,
                messages: {
                    isdepartmentnamerequired: 'Department Name is Required'
                }
            });


        } else {
            $('#CHQ_Book_ID').rules("remove", "messages");
            $('#CHQ_NO').rules("remove", "messages");
            $('#CHQ_DATE').rules("remove", "messages");
            $('#CHQ_AMOUNT').rules("remove", "messages");
            $('#DEDUCTION_AMOUNT').rules("remove", "messages");
            $('#CASH_AMOUNT').rules("remove", "messages");

            $('#DEPT_ID').rules("remove", "messages");
        }

    });

    //event for deduction radio selection
    $('#DeductionOnly').on('change', function () {


        if ((opeartion == 'A') || (opeartion == '')) {
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                    }
                }
            });
        }



        if ($(this).is(':checked')) {

            clearValidation($("#masterPaymentForm"));

            $('#DEDUCTION_AMOUNT').rules('add', {
                required: true,
                messages: {
                    required: 'Deduction Amount is Required',

                }
            });

            $('#DEDUCTION_AMOUNT').rules('add', {
                required: true,
                greaterThanZero: true,
                messages: {
                    required: 'Deduction Amount is Required',
                    greaterThanZero: "Deduction Amount Should be greater than zero"
                }
            });

            //Avinash
            $('#BILL_DATE').val($('#currentDate').val());

            $('#MAST_CON_ID_C').rules('add', {

                iscontractorrequired_C: true,
                messages: {
                    iscontractorrequired_C: 'Company Name is Required'
                }
            });

            //remove the amount validation when deduction only 
            $("#DEDUCTION_AMOUNT").rules("remove", "shouldBeLessThanChequeAmount");
            $('#CHQ_AMOUNT').rules("remove", "greaterThanZero");

        } else {
            $('#CHQ_Book_ID').rules("remove", "messages");
            $('#CHQ_NO').rules('remove');
            $('#CHQ_DATE').rules('remove');
            $('#CHQ_AMOUNT').rules('remove');
            $('#DEDUCTION_AMOUNT').rules('remove');
            $('#CASH_AMOUNT').rules('remove');

            $('#MAST_CON_ID_C').rules('remove');

        }

    });



    var isEpayChecked = $('#Epay').is(':checked');
    var isEremChecked = $('#ERem').is(':checked');
    //if operation is edit
    if (opeartion == "E") {

        $("#TXN_ID").attr('disabled', 'disabled');
        $("#BILL_NO").attr('disabled', 'disabled');

        if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {
            $('#btnSubmit').hide();
            $('#btnUpdate').show();
        }

        $("#masterListGrid").show('slow');
        $("#MasterDataEntryDiv").hide();
    }
    if (opeartion == "" || opeartion == "A" || opeartion == null) {

        $("#TXN_ID").removeAttr('disabled');
        $("#BILL_NO").removeAttr('disabled');

        // get the cheque book series
        if ($('#Cheque').is(':checked') && $("#CHQ_Book_ID").val() == 0) {
            $("#CHQ_Book_ID").empty();
            FillInCascadeDropdown(null, "#CHQ_Book_ID", "/Payment/GetchequebookSeries/");
        }

        if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {
            $('#btnSubmit').show();
            $('#btnUpdate').hide();
        }
    }

    //click event of view details in payment transaction page
    $('#btnViewDetails').click(function () {

        $("#PaymentMasterList").jqGrid().setGridParam
        ({ url: '/Payment/ListMasterPaymentDetailsForDataEntry/' }).trigger("reloadGrid");

        $("#trShowHideLinkTable").hide();
        $("#masterListGrid").show('slow');
        $("#TransactionForm").hide('slow');

        $("#trnsShowtable").hide('slow');


        $("#PaymentDeductionData").html('');

        $("#trShowHideLinkTable").hide();

    });

    //event to show new form to add master details
    $("#AddNewMasterDetails").live('click', function () {

        if (!$('#masterPaymentForm').is(':visible')) {

            $('#masterPaymentForm').show('slow');
        }
        opeartion = "A";

        $('#btnSubmit').show();

        $('#btnUpdate').hide();

        $("#masterListGrid").hide('slow');

        $("#TXN_ID").removeAttr('disabled');

        $("#BILL_NO").removeAttr('disabled');

        $("#MasterDataEntryDiv").show('slow');

        $("#tblTransaction").show('slow');

        $("#TransactionForm").hide('slow');

        $("#trnsShowtable").hide('slow');

        $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

        resetMasterForm();

        $("#PaymentDeductionData").html('');

        $("#trShowHideLinkTable").hide();

        availableCheques.length = 0;

        //$("#CHQ_Book_ID").empty();

        //FillInCascadeDropdown(null, "#CHQ_Book_ID", "/Payment/GetchequebookSeries/");

        $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");

        //Modified By Abhishek kamble 21-jan-2014
        //$("#BILL_DATE").val($("#CURRENT_DATE_Hidden").val());
        //$("#CHQ_DATE").val($("#CURRENT_DATE_Hidden").val());
        //$("#CHALAN_DATE").val($("#CURRENT_DATE_Hidden").val());
        $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
        $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
        $("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));

        $("#btnMasterReset").show();

        $("#formTable :input ").removeAttr("readonly");
        $("#formTable  select").prop("disabled", false);
        $("#formTable :radio").prop("disabled", false);

        //added by Koustubh Nakate on 26/09/2013
        if (!$("#tblOptions").is(":visible")) {

            $("#tblOptions").show();
        }



    });

    //get the supplier name details
    $("#MAST_CON_ID_S").change(function () {


        $("#PAYEE_NAME_S").val("");
        $('#PAYEE_NAME_S').attr('readOnly', true);
        $("#trContractorBankDetails").hide();
        $("#spnBankAccNumber").html("-");
        $("#spnIFSCCode").html("-");
        $("#spnBankName").html("-");



        if (!(($("#MAST_CON_ID_S").val() == 0) || ($("#MAST_CON_ID_S").val() == "") || ($("#MAST_CON_ID_S").val() == "-1"))) {

            // FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_S", "/Payment/PopulateAgreement/" + $("#MAST_CON_ID_S").val());
            setContractorSupplierName($("#MAST_CON_ID_S").val(), "S", true);
            getContratorBankDetails($("#MAST_CON_ID_S").val());
        } else {

            $('#PAYEE_NAME_S').attr('disabled', false);
            $('#PAYEE_NAME_S').attr('readOnly', false);
            $("#PAYEE_NAME_S").val("");
            $("#trContractorBankDetails").hide();
            $("#spnBankAccNumber").html("-");
            $("#spnIFSCCode").html("-");
            $("#spnBankName").html("-");

        }
    });

    //get the contractor name details
    $("#MAST_CON_ID_C").change(function () {


        //alert($("#MAST_CON_ID_C").val());
        if ($("#MAST_CON_ID_C").val() == '-1') {

            $("#PAYEE_NAME_C").attr("readonly", false);
            $("#PAYEE_NAME_C").prop('disabled', false);
            $("#trContractorBankDetails").hide();
            $("#spnBankAccNumber").html("-");
            $("#spnIFSCCode").html("-");
            $("#spnBankName").html("-");
            $("#PAYEE_NAME_C").val("");

        }
        else {
            if (($("#MAST_CON_ID_C").val() != 0 && $("#MAST_CON_ID_C").val() != "")) {
                // FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_C", "/Payment/PopulateAgreement/" + $("#MAST_CON_ID_C").val());
                setContractorSupplierName($("#MAST_CON_ID_C").val(), "C", true);

                //Added By Abhishek kamble 27-May-2014
                getContratorBankDetails($("#MAST_CON_ID_C").val());

            } else {

                $("#PAYEE_NAME_C").val("");

                //Added By Abhishek kamble 27-May-2014
                $("#trContractorBankDetails").hide();
                $("#spnBankAccNumber").html("-");
                $("#spnIFSCCode").html("-");
                $("#spnBankName").html("-");

            }
        }
    });

    $("#CHQ_Book_ID").change(function () {

        // $("#CHQ_NO").val('');

        var encryptedBillID = '';
        if ($("#CHQ_Book_ID").val() != 0) { //|| $("#CHQ_Book_ID").val() != ""




            encryptedBillID = $('#PaymentMasterList').getDataIDs()[0];


            var op = encryptedBillID === undefined ? "A" : "E";

            //alert("test : " +op);

            $.ajax({
                type: "POST",
                url: "/payment/GetAllAvailableCheques/" + $("#CHQ_Book_ID").val(),
                data: { opeartion: op, encryptedBillID: encryptedBillID },
                async: false,
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

                    availableCheques.length = 0;
                    if (data != "") {
                        if ($("#CHQ_NO").data('autocomplete')) {
                            $("#CHQ_NO").autocomplete("destroy");
                            $("#CHQ_NO").removeData('autocomplete');
                        }

                        availableCheques = data;

                        $("#CHQ_NO").autocomplete({
                            source: availableCheques,
                            minLength: 3
                        });

                        $.ajax({
                            type: "POST",
                            url: "/payment/GetChequeBookIssueDate?id=" + $("#CHQ_Book_ID").val(),
                            async: false,
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

                                $("#spnChqBookIssueDate").html(data.IssueDate);
                                $("#ChequeBookIssueDate").val(data.IssueDate);
                                $("#lblChqBookIssueDate").show();
                                $("#spnChqBookIssueDate").show();
                            }
                        });
                    }
                    else {
                        if ($("#CHQ_Book_ID").val() != 0) {
                            alert("No cheques available for selected series.");
                        }
                        if ($("#CHQ_NO").data('autocomplete')) {
                            $("#CHQ_NO").autocomplete("destroy");
                            $("#CHQ_NO").removeData('autocomplete');
                        }

                        $("#lblChqBookIssueDate").hide();
                        $("#spnChqBookIssueDate").hide();
                        return false;
                    }
                }
            });



        }
        else {
            $("#lblChqBookIssueDate").hide();
            $("#spnChqBookIssueDate").hide();

            $("#CHQ_NO").val('');
            $("#CHQ_NO").autocomplete({
                source: []
            });



        }

    });

    //change event of remitance department to get the name of the rem departmen
    $("#DEPT_ID").change(function () {
        if ($("#DEPT_ID").val() != 0 && $("#DEPT_ID").val() != "") {
            setRemDepartmentName($("#DEPT_ID").val(), true);
        }
        else {
            $("#PAYEE_NAME_R").val("");
        }

    });

    //Modified By Abhishek kamble 21-jan-2014 start
    if ($("#BILL_DATE").val() == "") {
        $("#BILL_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Voucher Date',
            onClose: function () {
                $(this).focus().blur();
            }
            //});
        }).datepicker('setDate', process(ModifiedCurrentDate));
    }
    else {
        $("#BILL_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Voucher Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });
    }

    $("#BILL_DATE").on('change focusout', function () {

        if ($("#BILL_DATE").val() != "") {
            $("#CHQ_DATE").val($("#BILL_DATE").val());
            //$("#CHALAN_DATE").val($("#BILL_DATE").val());
        }

    });

    if ($("#CHQ_DATE").val() == "") {
        $("#CHQ_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Cheque/Epayment Date',
            onClose: function () {
                $(this).focus().blur();
            }

            // });
        }).datepicker('setDate', process(ModifiedCurrentDate));
    } else {
        $("#CHQ_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Cheque/Epayment Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });
    }

    if ($("#CHALAN_DATE").val() == "") {
        $("#CHALAN_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Challan Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });//Commented by Abhishek kamble 6-June-2014
        // }).datepicker('setDate', process(ModifiedCurrentDate));
    } else {
        $("#CHALAN_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: new Date(),
            buttonText: 'Challan Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });
    }
    //Modified By Abhishek kamble 21-jan-2014 end

    $("#Cash").click(function () {

        $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");
        $("#DivCashAmount").show();
        $("#DivChequeAmount").hide();
        $(".chequeTr").hide();
        $("#CHQ_AMOUNT").removeAttr("readonly");
        // applyAltRowSyle($("form"));
    });

    $("#Cheque").click(function () {

        $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");

        $("#DivCashAmount").hide();
        $("#DivChequeAmount").show();
        $('#CHQ_NO').show();
        $('#EPAY_NO').hide();
        $(".chequeTr").show();

        $("#spnBankName").html('');
        $("#spnIFSCCode").html('');
        $("#spnBankAccNumber").html('');
        $('#conAccountId').val('');
        $('#CONC_Account_ID').hide();
        $('#trContractorBankDetails').hide();
        $('#MAST_CON_ID_C').val(0);
        $("#CHQ_AMOUNT").removeAttr("readonly");

        // FillInCascadeDropdown(null, "#CHQ_Book_ID", "/Payment/GetchequebookSeries/");

        //Added By Abhishek kamble 28-Apr-2014 Txn -Contrator Work payment
        //Modified By Abhishek kamble 29-May-2014 Txn -Contrator Work payment        
        if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3

            if ($("#CHQ_AMOUNT").val() == 0 && ($("#CHQ_AMOUNT").val() != "")) {
                $(".chequeTr").hide();
                $("#chqseriesTr").hide();
            } else {
                $(".chequeTr").show();
                $("#chqseriesTr").show();
            }
        }
        else {
            $(".chequeTr").show();
            $("#chqseriesTr").show();
        }
    });

    $("#BILL_MONTH").change(function () {

        //add the changed value of Year to the session
        //new change done by Vikram on 01-Jan-2014
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        if ((opeartion == 'A') || (opeartion == '')) {

            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                // data: $("#authSigForm").serialize(),
                error: function (xhr, status, error) {
                    //unblockPage();
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);

                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        // alert(data.strVoucherNumber);
                        // $("#BILL_NO").attr("disabled", true);
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                        // $("#VoucherCnt").val(data.strVoucherCnt);
                        //alert(data.strVoucherCnt);

                    }
                }
            });
        }

        //new change done by Abhishek kamble on 21-jan-2014 start
        if (!($("#BILL_DATE").is(":disabled")) || !($("#CHQ_DATE").is(":disabled"))) {
            if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {
                $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
                $("#CHQ_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
                //$("#CHALAN_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
            } else {
                //set Voucher Date
                if ($("#BILL_DATE").val() != '') {
                    var selectedDate = $("#BILL_DATE").val().split('/');
                    var day = selectedDate[0];
                    ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                    $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    //$("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                } else {
                    ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                    $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    //$("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                }
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end

        if ($("#BILL_MONTH").val() != 0 && $("#Epay").is(":checked") && $("#BILL_YEAR").val() != 0) {
            $.ajax({
                type: "POST",
                url: "/payment/GetEpayNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            // alert("Please  select month and year");
            return false;
        }

        if ($("#BILL_MONTH").val() != 0 && $("#ERem").is(":checked") && $("#BILL_YEAR").val() != 0) {
            //code to fetch the epay number
            $.ajax({
                type: "POST",
                url: "/payment/GetEremittanceNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            // alert("Please select month and year");
            return false;
        }



    });

    $("#BILL_YEAR").change(function () {

        //add the changed value of Year to the session
        //new change done by Vikram on 01-Jan-2014
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        if ((opeartion == 'A') || (opeartion == '')) {
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                // data: $("#authSigForm").serialize(),
                error: function (xhr, status, error) {
                    //unblockPage();
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);

                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        // alert(data.strVoucherNumber);
                        // $("#BILL_NO").attr("disabled", true);
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                        // $("#VoucherCnt").val(data.strVoucherCnt);
                        //alert(data.strVoucherCnt);

                    }
                }
            });
        }
        //new change done by Abhishek kamble on 21-jan-2014 start
        if (!($("#BILL_DATE").is(":disabled")) || !($("#CHQ_DATE").is(":disabled"))) {
            if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {
                $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
                $("#CHQ_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
                //$("#CHALAN_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
            } else {
                //set Voucher Date
                if ($("#BILL_DATE").val() != '') {
                    var selectedDate = $("#BILL_DATE").val().split('/');
                    var day = selectedDate[0];
                    ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                    $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    //$("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                } else {
                    ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                    $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                    //$("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));
                }
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end

        //on change of the year get epaynumber or eremittance number  if required

        if ($("#BILL_YEAR").val() != 0 && $("#Epay").is(":checked") && $("#BILL_YEAR").val() != 0) {

            $.ajax({
                type: "POST",
                url: "/payment/GetEpayNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            //alert("Please  select month and year");
            return false;
        }


        if ($("#BILL_MONTH").val() != 0 && $("#ERem").is(":checked") && $("#BILL_YEAR").val() != 0) {
            //code to fetch the epay number
            $.ajax({
                type: "POST",
                url: "/payment/GetEremittanceNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            // alert("Please select month and year");
            return false;
        }



    });

    //function for eapyamtn radibutton click event
    $("#Epay").click(function () {

        $("#spnBankName").html('');
        $("#spnIFSCCode").html('');
        $("#spnBankAccNumber").html('');
        $('#conAccountId').val('');
        $('#CONC_Account_ID').hide();
        $('#trContractorBankDetails').hide();
        $('#MAST_CON_ID_C').val(0);
        //Below Code Added on 01-01-2022
        $('#MAST_CON_ID_C').val(0);
        $('#MAST_CON_ID_S').val(0);
        $('#CHQ_AMOUNT').val(0);
        $('#CASH_AMOUNT').val(0);
        $('#DEDUCTION_AMOUNT').val(0);
        $('#txtTotalAmount').val(0);
        $('#PAYEE_NAME_C').val('');
        $('#PAYEE_NAME_S').val('');
        

        ValidateDPIUEpay();
        if (isValid == false) {
            return false;
        }

        if (($('#fundType').val() == 'P' || $('#fundType').val() == 'A')) {
            CheckAuthSignDsc();
            if (isValid == false) {
                return false;
            }

        }
        


        //disable dates
        $("#BILL_DATE").datepicker("disable").attr("readonly", "readonly");
        $("#CHQ_DATE").datepicker("disable").attr("readonly", "readonly");
        $("#CHALAN_DATE").datepicker("disable").attr("readonly", "readonly");

        if ($("#BILL_DATE").val() == "") {
            $("#BILL_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHQ_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHALAN_DATE").val($("#CURRENT_DATE_Hidden").val());
        }


        $("#CHQ_AMOUNT").removeAttr("readonly");
        $("#DivCashAmount").hide();
        $("#DivChequeAmount").show();

        $(".chequeTr").show();
        $("#chqseriesTr").hide('slow');

        if ($("#BILL_MONTH").val() != 0 && $("#BILL_YEAR").val() != 0) {
            //code to fetch the epay number
            $.ajax({
                type: "POST",
                url: "/payment/GetEpayNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            // alert("Please select month and year");
            return false;
        }

    });

    //event when clicks on the eremittance readiobuttons
    $("#ERem").click(function () {

        ValidateDPIUEremittence();
        if (isValid == false) {
            return false;
        }

        CheckAuthSignDsc();
        if (isValid == false) {
            return false;
        }
        //disable dates

        //commented by Vikram on 16-10-2013

        //$("#BILL_DATE").datepicker("disable").attr("readonly", "readonly");
        //$("#CHQ_DATE").datepicker("disable").attr("readonly", "readonly");


        //Modified By Abhishek Kamble 11-nov-2013
        $('#BILL_DATE').datepicker("disable").attr("readonly", "readonly");
        $('#CHQ_DATE').datepicker("disable").attr("readonly", "readonly");

        //end of change


        // $("#CHALAN_DATE").datepicker("disable").attr("readonly", "readonly");
        $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");
        if ($("#BILL_DATE").val() == "") {
            $("#BILL_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHQ_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHALAN_DATE").val($("#CURRENT_DATE_Hidden").val());
        } else {//else Added By Abhishek kamble 21-Apr-2014
            $("#BILL_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHQ_DATE").val($("#CURRENT_DATE_Hidden").val());
            $("#CHALAN_DATE").val($("#CURRENT_DATE_Hidden").val());
        }


        $("#CHQ_AMOUNT").removeAttr("readonly");
        $("#DivCashAmount").hide();
        $("#DivChequeAmount").show();

        $(".chequeTr").show();
        $("#chqseriesTr").hide('slow');

        if ($("#BILL_MONTH").val() != 0 && $("#BILL_YEAR").val() != 0) {
            //code to fetch the epay number
            $.ajax({
                type: "POST",
                url: "/payment/GetEremittanceNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
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
                    $('#CHQ_NO').hide();
                    $('#EPAY_NO').val(data).attr("readonly", "readonly");
                    $('#EPAY_NO').show();
                }
            });
        } else {

            // alert("Please select month and year");
            return false;
        }
    });

    //event for deduction only radiobutton 
    $("#DeductionOnly").click(function () {
        $("#DivCashAmount").hide();
        $(".chequeTr").hide();
        $("#CHQ_AMOUNT").attr("readonly", "readonly").val(0);
        $("#DEDUCTION_AMOUNT").removeAttr("readonly");
        $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
        $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");

        $("#spnBankName").html('');
        $("#spnIFSCCode").html('');
        $("#spnBankAccNumber").html('');
        $('#conAccountId').val('');
        $('#CONC_Account_ID').hide();
        $('#trContractorBankDetails').hide();
        $('#MAST_CON_ID_C').val(0);
    });

    //function for back button to master list of payment
    $("#lblBack").click(function () {
        blockPage();
        $("#mainDiv").load("/payment/GetPaymentList/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(), function () {
            unblockPage();
        });
    });

    //Added By Abhishek 1Apr2015
    $("#Advice").click(function () {

        //Below code added on 01-01-2022
        $('#MAST_CON_ID_C').val(0);
        $('#MAST_CON_ID_S').val(0);
        $('#CHQ_AMOUNT').val(0);
        $('#CASH_AMOUNT').val(0);
        $('#DEDUCTION_AMOUNT').val(0);
        $('#txtTotalAmount').val(0);
        $('#PAYEE_NAME_C').val('');
        $('#PAYEE_NAME_S').val('');

        if ((opeartion == 'A') || (opeartion == '')) {
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/V$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    if (data != "") {
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);
                    }
                }
            });
        }


        $("#chqseriesTr").hide('slow');


        $('#CHQ_NO').rules('add', {
            maxlength: 30,
            required: true,
            // SixDigitChequeNo:true,
            //CheckChequeNumber: function () { if (parseFloat($("#LevelID").val()) == 5) { return true; } else { return false; } },
            messages: {
                required: 'Advice Number is Required',
                maxlength: 'Maximum 30 digits are allowed in Advice Number'
                //  SixDigitChequeNo: "Cheque/Advice Number must be 6 digit long"
            }
        });

        // $('#CHQ_NO').rules("remove", "messages");
        //$('#CHQ_NO').rules("remove", "required");
        $('#CHQ_NO').rules("remove", "CheckChequeNumber");
        $('#CHQ_NO').rules("remove", "SixDigitChequeNo");
        $('#CHQ_NO').show();
        $('#EPAY_NO').hide();
    });

    //get the page design parameters based on the transaction type selected
    $("#TXN_ID").change(function () {

        // $("#trContractorBankDetails").html('');
        $("#spnBankName").html('');
        $("#spnIFSCCode").html('');
        $("#spnBankAccNumber").html('');
        $('#conAccountId').val('');
        $('#CONC_Account_ID').hide();
        $('#trContractorBankDetails').hide();

        $("#spnBankAccNumber").html('');
        $("#spnIFSCCode").html('');
        $("#spnBankName").html('');

        //New Change by Abhishek kamble 13-jan-2014 start

        if ((($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "469$Q") || ($("#TXN_ID option:selected").val() == "834$Q"))) {

            if (($("#PAYEE_NAME").val() == "")) {
                $("#PAYEE_NAME").val("SRRDA");
            }
        } else if ($("#TXN_ID").is(":enabled")) {
            $("#PAYEE_NAME").val("");
        }

        //New Change by Abhishek kamble 13-jan-2014 end

        //Added By Abhishek kamble 5-mar-2014 start 

        //alert($("#TXN_ID option:selected").val());
        //if ($("#TXN_ID option:selected").val() == "109$Q")
        //{
        //    $('#CHALAN_DATE').rules('add', {
        //        required: true,
        //        messages: {
        //            required: 'Challan Date is Required.'
        //        }
        //    });

        //    $('#CHALAN_NO').rules('add', {
        //        required: true,
        //        messages: {
        //            required: 'Challan number is Required.'
        //        }
        //    });
        //}
        //Added By Abhishek kamble 5-mar-2014 end


        if (!$("DeductionOnly").is(":checked")) {
            $("#CHQ_AMOUNT").removeAttr("readonly");
        }

        if ($("#TXN_ID").val() == 0) {
            resetMasterForm();
            return false;
        }
        blockPage();

        $('#Cash').attr('disabled', false);
        $('#Cheque').attr('disabled', false);
        $('#Epay').attr('disabled', false);
        $('#ERem').attr('disabled', false);
        $('#DeductionOnly').attr('disabled', false);
        $('#Advice').attr('disabled', false);

        $.ajax({
            type: "POST",
            url: "/payment/GetMasterDesignParams/" + $("#TXN_ID").val() + '$' + $('#BILL_MONTH').val() + '$' + $('#BILL_YEAR').val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                //alert(data.EpayRequired);
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");


                clearValidation($("#masterPaymentForm"));

                if (data != "") {


                    if (data.CashOrCheque == "Q") {
                          //if (($('#fundType').val() == 'P' || $('#fundType').val() == 'A') && ($("#TXN_ID option:selected").val() == "47$Q" || $("#TXN_ID option:selected").val() == "72$Q" || $("#TXN_ID option:selected").val() == "86$Q" //|| $("#TXN_ID option:selected").val() == "105$Q"
                          //    /*|| $("#TXN_ID option:selected").val() == "109$Q"*/ || $("#TXN_ID option:selected").val() == "1484$Q" || $("#TXN_ID option:selected").val() == "1788$Q") || ($("#TXN_ID option:selected").val() == "1974$Q"))

                        //Below Line Commented on 09-12-2021
                        //if (($('#fundType').val() == 'P' || $('#fundType').val() == 'A') && data.EpayRequired == 'Y')

                        //Below Line Added on 09-12-2021
                        if (($('#fundType').val() == 'P' || $('#fundType').val() == 'A' || $('#fundType').val() == 'M') && data.EpayRequired == 'Y')
                        {
                            //alert("Working");
                            //PMGSY3
                            //alert(data.IsEmailAvailable);
                            //alert(data.IsPaymentSuccess);
                            $('#spnAgency').hide();
                            $('#spnSRRDABank').hide();
                            $('#spnDSCEnroll').hide();
                            $('#spnEmail').hide();
                            $('#spnPaymentSuccess').hide();

                            if (!data.IsAgencyMapped) {
                                $('#FormParameters').hide();
                                $('#PFMSValidations').show();
                                $('#spnAgency').show();
                            }
                            if (!data.IsSRRDABankDetailsFinalized) {
                                $('#FormParameters').hide();
                                $('#PFMSValidations').show();
                                $('#spnSRRDABank').show();
                            }
                            if (!data.IsDSCEnrollmentFinalized) {
                                $('#FormParameters').hide();
                                $('#PFMSValidations').show();
                                $('#spnDSCEnroll').show();
                            }
                            if (!data.IsEmailAvailable) {
                                $('#FormParameters').hide();
                                $('#PFMSValidations').show();
                                $('#spnEmail').show();
                            }
                            if (!data.IsPaymentSuccess) {
                                $('#FormParameters').hide();
                                $('#PFMSValidations').show();
                                $('#spnPaymentSuccess').show();
                            }
                        }
                        else {
                            $('#FormParameters').show();
                            $('#PFMSValidations').hide();

                            $('#spnAgency').hide();
                            $('#spnSRRDABank').hide();
                            $('#spnDSCEnroll').hide();
                            $('#spnEmail').hide();
                            $('#spnPaymentSuccess').hide();
                        }
                        $('#Cheque').trigger('click');

                        //Added By Abhishek kamble 11-Apr-2014 start

                        //Added By Abhishek kamble 14-Apr-2014 -Auth Surrender
                        if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {

                            //Txn -Contrator Work payment
                            if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3

                                if (($("#CHQ_AMOUNT").val() == 0) && ($("#CHQ_AMOUNT").val() != "")) {
                                    $(".chequeTr").hide();
                                    //$("#chqseriesTr").hide();

                                }
                                else {
                                    $(".chequeTr").show();
                                    //$("#chqseriesTr").show();
                                }
                            }
                            else {
                                $(".chequeTr").show();
                                $("#chqseriesTr").show();
                            }

                            $("#trModeOfTransaction").show();
                        }
                        else {
                            $(".chequeTr").hide();
                            $("#chqseriesTr").hide();
                            $("#trModeOfTransaction").hide();
                        }//Txn -Contrator Work payment
                        if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3

                            //Commented By Abhishek kamble 29-May-2014 amount val(0)
                            //if ($("#CHQ_AMOUNT").val() == "") {

                            //    $('#CHQ_AMOUNT').val(0);
                            //}

                            //$('#chqseriesTr').hide();
                            //$('#chequeTr').hide();

                            //Added By Abhishek kamble 29-May-2014
                            //if ($('#CHQ_AMOUNT').val() == 0) {
                            //    $('#chqseriesTr').hide();
                            //    $('#chequeTr').hide();
                            //}
                            //else {
                            //    $('#chqseriesTr').show();
                            //    $('#chequeTr').show();
                            //}

                            $('#CHQ_AMOUNT').rules("remove", "messages");
                            $('#CHQ_AMOUNT').rules("remove", "required");
                            $('#CHQ_AMOUNT').rules("remove", "lessOrEqualToBankAuthBalance");
                            $('#CHQ_AMOUNT').rules("remove", "lessOrEqualToBankAuthBalanceEdit");
                            $('#CHQ_AMOUNT').rules("remove", "greaterThanZero");
                        }

                        if ($("#LevelID").val() == 5) {//DPIU
                            //Added By Abhishek kamble 11-Apr-2014 -Auth Surrender
                            if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                                $("#trModeOfTransaction").hide();

                                //$("#chqseriesTr").hide();

                                $(".chequeTr").hide();
                            }
                            else {
                                //Txn -Contrator Work payment //Modified By Abhishek kamble 29-May-2014 
                                if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                                    if ($("#CHQ_AMOUNT").val() == 0 && ($("#CHQ_AMOUNT").val() != "")) {
                                        $("#chqseriesTr").hide();
                                    }
                                    else {
                                        $("#chqseriesTr").show();
                                    }

                                }
                                else {
                                    $("#chqseriesTr").show();
                                }
                                //$("#chqseriesTr").show();                                
                                $("#trModeOfTransaction").show();

                            }
                        } else {//SRRDA
                            //alert("test");
                            if ($("#fundType").val() == "A") {//if Condition Added by Abhishek to show cheque book series at SRRDA level for ADMIN Fund. 25Feb2015
                                $("#chqseriesTr").show();
                            }
                            else {
                                $("#chqseriesTr").hide();
                            }
                        }
                        $("#divCash").hide();
                        $("#divCheque").show();
                        $("#divEpay").hide();





                        //change added by Koustubh Nakate on 04/10/2013 for hide and show cash and cheque amount div
                        $("#DivCashAmount").hide();
                        $("#DivChequeAmount").show();
                    }

                    //Added By Abhishek 1Apr2015 start
                    if (data.AdviceNoRequired == "Y") {
                        $("#divAdviceNo").show();
                        // alert($("#IsAdvicePayment").val());
                        //if ($("#IsAdvicePayment").val() == "A") {
                        //    $('#Advice').trigger('click');
                        //}
                    }
                    else {
                        $("#divAdviceNo").hide();
                    }
                    //Added By Abhishek 1Apr2015 end

                    if (data.CashOrCheque == "C") {
                        $('#Cash').attr('checked', true);
                        $(".chequeTr").hide();
                        $("#divCash").show();
                        $("#divCheque").hide();
                        $("#divEpay").hide();

                        //change added by Koustubh Nakate on 04/10/2013 for hide and show cash and cheque amount div
                        $("#DivChequeAmount").hide();
                        $("#DivCashAmount").show();

                        $('#Cash').trigger('change');

                        //Added By Abhishek kamble 28-Apr-2014 start
                        if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                            $("#trModeOfTransaction").hide();
                        } else {
                            $("#trModeOfTransaction").show();
                        }
                        //Added By Abhishek kamble 28-Apr-2014 end
                    }
                    if (data.CashOrCheque == "CQ") {
                        $('#Cheque').trigger('click');
                        if ($("#LevelID").val() == 5) {
                            $("#chqseriesTr").show('slow');
                        } else {
                            $("#chqseriesTr").hide();
                        }
                        $("#divCash").show();
                        $("#divCheque").show();
                    }

                    if (data.DedRequired == "N") {
                        $("#divDeductionOnly").hide();
                        $("#DEDUCTION_AMOUNT").val(0);
                        $("#DEDUCTION_AMOUNT").attr("disabled", "disabled");
                        $("#DEDUCTION_AMOUNT").attr("readonly", "readonly").val(0);
                        clearValidation($("#masterPaymentForm"));

                    }
                    else if (data.DedRequired == "Y") {
                        $("#divDeductionOnly").hide();
                        $("#DEDUCTION_AMOUNT").removeAttr("readonly");
                        $("#DEDUCTION_AMOUNT").removeAttr("disabled");
                    }

                    else if (data.DedRequired == "B") {
                        $("#DEDUCTION_AMOUNT").removeAttr("readonly")
                        $("#DEDUCTION_AMOUNT").removeAttr("disabled");
                        $("#divDeductionOnly").show();

                        /*$('#DEDUCTION_AMOUNT').rules('add', {
                            required: true,
                            greaterThanZero: true,
                            messages: {
                                required: 'Deduction Amount is Required',
                                greaterThanZero: "Deduction Amount Should be greater than zero"
                            }
                        });*/
                        // $(".chequeTr").hide();
                    }
                    else {

                    }
                    if (data.EpayRequired == "N") {
                        $("#divEpay").hide();
                    }
                    else {
                        $("#divEpay").show();
                    }
                    if (data.MultipleTransRequired == "Y") {

                    } else {

                    }

                    if (data.RemittanceRequired == "Y") {
                        $("#divEpay").hide();
                        $('.RemittanceTr').show('slow');
                        $("#divERemit").show('slow');
                        //if ($("#CHALAN_NO").val() == '') {
                        //    $("#CHALAN_DATE").val('');
                        //}
                        $('#DEPT_ID').rules('add', {
                            required: true,
                            messages: {
                                required: 'Department Name is Required'
                            }
                        });
                        //Below line added on 14-01-2022 to enable searching in dropdown
                        $('#DEPT_ID').chosen();

                        $('#PAYEE_NAME_R').rules('add', {
                            required: true,
                            messages: {
                                required: 'Payee Name(Remittance) is Required'
                            }
                        });

                        /*$('#CHALAN_NO').rules('add', {
                            required: true,
                            messages: {
                                required: 'Chalan Number is Required'
                            }
                        });

                        $('#CHALAN_DATE').rules('add', {
                            required: true,
                            messages: {
                                required: 'Chalan Date is Required'
                            }
                        });*/

                    }
                    else {
                        $("#divERemit").hide('slow');
                        $('.RemittanceTr').hide('slow');
                        if (data.EpayRequired == "Y") {
                            $("#divEpay").show('slow');
                        }
                    }
                    if (data.ContractorRequired == "Y") {

                        $('.ContracorTr').show('slow');
                        //Added By Abhishek kamble 27-May-2014
                        // $("#trContractorBankDetails").show();

                        if ($("#fundType").val() == "P" || $("#fundType").val() == "M") {
                            $('#MAST_CON_ID_C').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Company Name (Contractor) is Required'
                                }
                            });

                            //Below line added on 14-01-2022 to enable searching in dropdown
                            $('#MAST_CON_ID_C').chosen();

                            $('#PAYEE_NAME_C').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee Name(Contractor) is Required'
                                }
                            });
                        }
                        else if ($("#fundType").val() == "A") {
                            $('#MAST_CON_ID_C').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee Name is Required'
                                }
                            });
                            //Below line added on 14-01-2022 to enable searching in dropdown
                            $('#MAST_CON_ID_C').chosen();

                            $('#PAYEE_NAME_C').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee Name is Required'
                                }
                            });
                            $("#ContracorTrID").html("");
                            $("#ContracorTrID").html('Payee/Supplier Name');


                            //$("#MAST_CON_ID_C").empty();
                            //FillInCascadeDropdown(null, "#MAST_CON_ID_C", "/Payment/GetPayeeSupplierDetails/" + $("#TXN_ID").val());

                            //$("#trContractorBankDetails").html('');
                        }
                    }
                    else {
                        $('.ContracorTr').hide('slow');
                        //Added By Abhishek kamble 27-May-2014
                        $("#trContractorBankDetails").hide();

                    }
                    //if (data.AgreementRequired == "Y") {
                    //    $('.agreement').show();

                    //}
                    //else {
                    //    $('.agreement').hide();
                    //}
                    if (data.SupplierRequired == "Y") {
                        $('.supplierTr').show();

                        if ($("#fundType").val() == "P" || $("#fundType").val() == "M") {

                            $('#MAST_CON_ID_S').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Company Name (Supplier) is Required'
                                }
                            });

                            //Below line added on 14-01-2022 to enable searching in dropdown
                            $('#MAST_CON_ID_S').chosen();

                            $('#PAYEE_NAME_S').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee Name (Supplier)  is Required'
                                }
                            });
                        }


                        if ($("#fundType").val() == "A") {

                            $('#MAST_CON_ID_S').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee Name / Supplier Name  is Required'
                                }
                            });

                            //Below line added on 14-01-2022 to enable searching in dropdown
                            $('#MAST_CON_ID_S').chosen();

                            $('#PAYEE_NAME_S').rules('add', {
                                required: true,
                                messages: {
                                    required: 'Payee/Supplier Name is Required'
                                }
                            });
                            $("#tdSupplier").html('Payee/Supplier Name');
                            //$("#MAST_CON_ID_S").empty();
                            //FillInCascadeDropdown(null, "#MAST_CON_ID_S", "/Payment/GetPayeeSupplierDetails/" + $("#TXN_ID").val());

                        }

                    }
                    else {
                        $('.supplierTr').hide();

                    }

                    if (data.SupplierRequired == "Y" || data.AgreementRequired == "Y" || data.ContractorRequired == "Y" || data.RemittanceRequired == "Y") {

                        $('#normalPayeename').hide();
                    }
                    else {
                        $('#normalPayeename').show();

                        $('#PAYEE_NAME').rules('add', {
                            required: true,
                            messages: {
                                required: 'Payee Name is Required'
                            }
                        });
                    }

                    if (isEpayChecked && !isEremChecked) {
                        $('#Epay').attr('checked', true);
                        $('#CHQ_NO').hide();
                        $('#EPAY_NO').show();

                    }
                    if (isEremChecked) {
                        $('#ERem').attr('checked', true);
                        $('#CHQ_NO').hide();
                        $('#EPAY_NO').show();

                    }



                }
                else {
                    alert("design parameters not entered for this head... !!!");

                }

            }
        });

        //PFMS Validations
        //alert($("#TXN_ID option:selected").val());
        if ($('#fundType').val() == 'P'
               && ($("#TXN_ID option:selected").val() == "47$Q" || $("#TXN_ID option:selected").val() == "72$Q" || $("#TXN_ID option:selected").val() == "86$Q" //|| $("#TXN_ID option:selected").val() == "105$Q"
            /*|| $("#TXN_ID option:selected").val() == "109$Q"*/ || $("#TXN_ID option:selected").val() == "1484$Q" || $("#TXN_ID option:selected").val() == "1788$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
            //$('#divCheque').hide();
            $('#divERemit').hide();
            $('#divCash').hide();
            //$('#divDeductionOnly').hide();
            $('#divAdviceNo').hide();

            $('#divEpay').show();
            //$('#Epay').trigger('click');

        }
    });





    //reset button click event for master data entry    
    $("#btnMasterReset").click(function (e) {

        // $("#MasterDataEntryDiv").toggle('slow');
        $("#TXN_ID").removeAttr("disabled");
        $("#BILL_NO").removeAttr("disabled");
        $("#btnSubmit").show();
        $("#btnUpdate").hide();
        $(':input', '#masterPaymentForm').not("#EPAY_NO").not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

        resetMasterForm();

    });

    //cancel button click event for master data entry    
    $("#btnMasterCancel").click(function (e) {

        // $("#MasterDataEntryDiv").toggle('slow');
        $("#TXN_ID").removeAttr("disabled");
        $("#BILL_NO").removeAttr("disabled");
        $("#btnSubmit").show();
        $("#btnUpdate").hide();
        $(':input', '#masterPaymentForm').not("#EPAY_NO").not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
        resetMasterForm();

        $("#masterPaymentForm").toggle('slow');

        $("#MasterDataEntryDiv").toggle('slow');

        $('#HideShowTransaction').trigger('click');


    });

    //function add master details
    $("#btnSubmit").click(function (e) {
        //alert($('#conAccountId').val());
        e.preventDefault();

        if ($("#DEDUCTION_AMOUNT").val() == "") {
            $("#DEDUCTION_AMOUNT").val(0);

        }

        //if voucher is of type cheque then trigger the change event so that cheque related rules will be applied 
        if ($('#Cheque').is(":checked")) {
            $('#Cheque').trigger('change');
        }

        //validation for 
        if ($("#Epay").is(":checked")) {
            $("#BILL_DATE").removeAttr("disabled");
            $("#CHQ_DATE").removeAttr("disabled");
        }

        if ($("#masterPaymentForm").valid()) {

            //added by abhishek kamble 11-Nov-2013
            $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
            $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");

            $("#TXN_ID").removeAttr('disabled');
            //$("#BILL_DATE").datepicker("enable").removeAttr("readonly");
            //$("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
            $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");
            $("#DEDUCTION_AMOUNT").removeAttr("disabled");

            //Added By Abhishek kamble 15-Apr-2014 start pass null date for auth surrender , contractor work payment
            if ($("#Cheque").is(":checked")) {

                //|| ($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q")
                if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                    $('#CHQ_DATE').val(null);
                }



                //Added By Abhishek kamble 29-May-2014 pass null date for contractor work payment
                if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                    if ($("#CHQ_AMOUNT").val() == 0) {
                        $('#CHQ_DATE').val(null);
                        $('#CHQ_NO').val(null);
                    }
                }



            }
            //Added By Abhishek kamble 15-Apr-2014 end

            blockPage();

            $.ajax({
                type: "POST",
                url: "/payment/AddPaymentMasterDetails",
                async: false,
                data: $("#masterPaymentForm").serialize(),
                error: function (xhr, status, error) {
                    unblockPage();
                    //added by abhishek kamble 30-Sep-2014
                    if ($("#Epay").is(":checked")) {
                        $("#BILL_DATE").attr("disabled", true);
                        $("#CHQ_DATE").attr("disabled", true);
                    }
                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    //alert(data);
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();


                    if (data.Success === undefined) {

                        unblockPage();
                        ///PFMS Validations
                        if (data.message != undefined) {
                            alert(data.message);
                            return false;
                        }

                        $("#masterListGrid").hide();
                        $("#MasterData").html(data);
                        $.validator.unobtrusive.parse($("#MasterData"));
                        MasterTriggerWhenError = true;
                        $("#MasterDataEntryDiv").show();

                        $('#TXN_ID').trigger('change');

                        if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "C") {

                            if ($("#divDeductionOnly").is(":visible")) {
                                $("#DeductionOnly").trigger("click");
                            } else {
                                $("#Cash").trigger("click");
                            }

                        } else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "Q") {

                            $("#Cheque").trigger("click");
                            if ($("#LevelID").val() == 5) {
                                $("#CHQ_Book_ID").trigger("change");
                            }

                        } else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "E") {

                            $("#Epay").trigger("click");

                        } else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "R") {

                            $("#ERem").trigger("click");

                        }
                        else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "A") {//Added by Abhi 6Apr2015 for Advice No
                            $("#Advice").trigger("click");
                        }

                        if ($("#LevelID").val() == 5 && $("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "Q") {
                            $("#chqseriesTr").show('slow');
                        } else {
                            //if condition added by abhishek kamble to show chq book series at SRRDA level 25Feb2015
                            if ($("#fundType").val() == "A" && $("#LevelID").val() == 4) {
                                $("#chqseriesTr").show();
                            }
                            else {
                                $("#chqseriesTr").hide();
                            }
                        }

                        //Added By Abhishek kamble 14-Apr-2014 
                        //&& ($("#TXN_ID option:selected").val() != "47$Q") && ($("#TXN_ID option:selected").val() != "737$Q")
                        if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3

                            if ($("#CHQ_AMOUNT").val() == 0) {
                                $(".chequeTr").hide();
                                $("#chqseriesTr").hide();
                            }
                            else {
                                $(".chequeTr").show();
                                $("#chqseriesTr").show();
                            }
                        }
                        else {
                            if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                                $(".chequeTr").hide();
                            }
                        }
                    }

                    else if (data.Success) {


                        alert("Payment Master added.");

                        blockPage();

                        //show the master data grid with new entry 
                        // $('#PaymentMasterList').jqGrid('GridUnload');
                        // loadPaymentMasterGrid("view", data.Bill_ID);
                        $("#masterListGrid").show('slow');

                        // get the transaction form 

                        $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + data.Bill_ID + "?CHQ_AMOUNT=" + data.CHQ_AMOUNT, function () {



                            $("#PaymentMasterList").jqGrid().setGridParam
                                ({ url: '/Payment/ListMasterPaymentDetailsForDataEntry/' + data.Bill_ID }).trigger("reloadGrid");

                            $("#trShowHideLinkTable").show();

                            //get the amount table updates
                            GetAmountTableDetails(data.Bill_ID);


                            $('#PaymentGridDivList').jqGrid('GridUnload');
                            loadPaymentGrid(data.Bill_ID);

                            //clear & hide the master dataentry form 
                            $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
                            resetMasterForm()

                            $("#MasterDataEntryDiv").toggle('slow');

                            $("#trnsShowtable").show('slow');
                            //show details Table for dataentry
                            $("#TransactionForm").show('slow');

                            unblockPage();

                        });

                        //added by Koustubh Nakate on 26/09/2013 to show month and year selection div 
                        $("#tblOptions").hide();

                    }

                    else if (data.Success == false && data.Bill_ID == "-9") {
                        alert("Entry is prohibited since payments are  already acknowledeged by SRRDA. First definalize cheque acknowledgement entries for the selected month and year");
                        return false;
                    }
                    else if (data.Success == false && data.Bill_ID == "-2") {
                        alert("Epayment Number allready exist.please try again");
                        return false;
                    }
                    else if (data.Success == false && data.Bill_ID == "-3") {
                        alert("Bank details does not found.Please check the bank details");
                        return false;
                    } else if (data.Success == false && data.Bill_ID == "-4") {
                        alert("Opening Balance entry is not finalized.Please finalize it and try again");
                        return false;
                    }
                    else if (data.Success == false && data.Bill_ID == "-5") {
                        alert("Contractor bank details are not present.");
                        return false;
                    }
                    else if (data.Success == false && data.Bill_ID == "-111") {
                        alert("Epayment is not enabled for this user.");
                        return false;
                    }
                    else if (data.Success == false && data.Bill_ID == "-222") {
                        alert("Eremittence is not enabled for this user.");
                        return false;
                    }


                    //else if (data.Success == false && data.Bill_ID == "-999") {//Added By Abhishek kamble 9Mar2015 for to check chq book issue date validation
                    //    alert(data.Message);
                    //    return false;
                    //}
                }
            });
        } else {
            //added by abhishek kamble 11-Nov-2013
            //$('#BILL_DATE').datepicker("disable").attr("readonly", "readonly");
            //$('#CHQ_DATE').datepicker("disable").attr("readonly", "readonly");


            //added by abhishek kamble 30-Sep-2014
            if ($("#Epay").is(":checked")) {
                $("#BILL_DATE").attr("disabled", true);
                $("#CHQ_DATE").attr("disabled", true);
            }

        }
    });

    // button event to update the master details
    $("#btnUpdate").click(function (e) {


        if (parseFloat($("#TotalAmtEnteredChqAmount").text()) != 0 || parseFloat($("#TotalAmtEnteredDedAmount").text()) != 0 || parseFloat($("#TotalAmtEnteredGrossAmount").text()) != 0) {
            alert("Please delete all the transaction details entered for this voucher.");
            return false;
        }

        //if voucher is of type cheque then trigger the change event so that cheque related rules will be applied 
        if ($('#Cheque').is(":checked")) {
            $('#Cheque').trigger('change');
        }
        else if ($('#Cash').is(":checked")) {
            $('#Cash').trigger('change');
        }
        else if ($('#Epay').is(":checked")) {
            $('#Epay').trigger('change');
        }
        else if ($('#ERem').is(":checked")) {
            $('#ERem').trigger('change');
        }
        else if ($('#DeductionOnly').is(":checked")) {
            $('#DeductionOnly').trigger('change');
        }

        e.preventDefault();

        if ($("#masterPaymentForm").valid()) {

            if (confirm("Are you sure you want to update the details ?")) {

                //Added By Abhishek kamble 15-Apr-2014 start
                if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                    $('#CHQ_DATE').val(null);
                }

                //Added By Abhishek kamble 29-May-2014 start
                if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                    if ($("#CHQ_AMOUNT").val() == 0) {
                        $('#CHQ_DATE').val(null);
                        $('#CHQ_NO').val(null);
                    }
                }

                //Added By Abhishek kamble 15-Apr-2014 end
                $('#Cash').attr('disabled', false);
                $('#Cheque').attr('disabled', false);
                $('#Epay').attr('disabled', false);
                $('#ERem').attr('disabled', false);
                $('#DeductionOnly').attr('disabled', false);
                $('#Advice').attr('disabled', false);//Added By Abhi for Advice no 6Apr2015

                $("#TXN_ID").removeAttr('disabled');
                $("#BILL_NO").removeAttr('disabled');
                $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
                $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
                $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");
                $("#DEDUCTION_AMOUNT").removeAttr("disabled");
                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/payment/EditMasterPaymentDetails/" + Bill_ID,
                    async: false,
                    data: $("#masterPaymentForm").serialize(),
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
                            $("#MasterData").html(data);
                            $.validator.unobtrusive.parse($("#MasterData"));
                            MasterTriggerWhenError = true;
                            $('#TXN_ID').trigger('change');
                            $("#MasterDataEntryDiv").show();

                            if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "C") {

                                if ($("#divDeductionOnly").is(":visible")) {
                                    $("#DeductionOnly").trigger("click");
                                } else {
                                    $("#Cash").trigger("click");
                                }

                            } else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "Q") {

                                $("#Cheque").trigger("click");
                                //to get the all availble cheques
                                if ($("#LevelID").val() == 5) { $("#CHQ_Book_ID").trigger("change"); }

                            } else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "E") {
                                $("#Epay").trigger("click");
                            }
                            else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "R") {
                                $("#ERem").trigger("click");
                            }
                            else if ($("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "A") {//Added By Abhi for Advice no 6Apr2015
                                $("#Advice").trigger("click");
                            }

                            if ($("#LevelID").val() == 5 && $("#SELECTED_CHQ_EPAY_ON_ERROR").val() == "Q") {
                                $("#chqseriesTr").show('slow');
                            } else {
                                $("#chqseriesTr").hide();
                            }

                            //added by koustubh Nakate on 28/09/2013
                            $("#TXN_ID").attr('disabled', true);
                            $("#BILL_NO").attr('disabled', true);

                            $('#Cash').attr('disabled', true);
                            $('#Cheque').attr('disabled', true);
                            $('#Epay').attr('disabled', true);
                            $('#ERem').attr('disabled', true);
                            $('#DeductionOnly').attr('disabled', true);
                            $('#Advice').attr('disabled', true);//Added By Abhi for Advice no 6Apr2015

                            //Added By Abhishek kamble 14-Apr-2014 
                            //&& ($("#TXN_ID option:selected").val() != "47$Q") && ($("#TXN_ID option:selected").val() != "737$Q")
                            if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {
                                //$(".chequeTr").show();
                                //$("#chqseriesTr").show();
                                if ($("#CHQ_AMOUNT").val() == 0) {
                                    $(".chequeTr").hide();
                                    $("#chqseriesTr").hide();
                                } else {
                                    $(".chequeTr").show();
                                    $("#chqseriesTr").show();
                                    //$("#lblChqBookIssueDate").show();                                    
                                    //$("#spnChqBookIssueDate").show();
                                }

                            }
                        }

                        else if (data.Success) {


                            //show the master data grid with new entry 

                            $("#PaymentMasterList").jqGrid().setGridParam
                                      ({ url: '/Payment/ListMasterPaymentDetailsForDataEntry/' + data.Bill_ID }).trigger("reloadGrid");

                            // loadPaymentGrid("ShowAddedEntry", data.Bill_ID);


                            $("#masterListGrid").show('slow');

                            alert("Master Payment Details updated successfully.");

                            // get the transaction form 
                            blockPage();
                            $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + data.Bill_ID, function () {

                                //get the amount table updates
                                GetAmountTableDetails(data.Bill_ID);

                                loadPaymentGrid(data.Bill_ID);

                                //clear & hide the master dataentry form 
                                $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
                                resetMasterForm()

                                $("#MasterDataEntryDiv").toggle('slow');

                                //added by Koustubh Nakate on 26/09/2013 to hide month and year selection div 
                                $("#tblOptions").hide('slow');

                                //show details Table for dataentry
                                $("#TransactionForm").show('slow');

                                unblockPage();



                            });
                        } else if (data.Bill_ID == "-1") {
                            alert("This Payment cant be edited,its already Finalized");
                            $("#TXN_ID").attr('disabled', true);
                            $("#BILL_NO").attr('disabled', true);

                            return false;

                        } else if (data.Success == false && data.Bill_ID == "-2") {
                            alert("Epayment Number allready exist.please try again");
                            $("#TXN_ID").attr('disabled', true);
                            $("#BILL_NO").attr('disabled', true);

                            return false;
                        }
                        else if (data.Success == false && data.Bill_ID == "-3") {
                            alert("Bank details  does not found.Please check the bank details");
                            $("#TXN_ID").attr('disabled', true);
                            $("#BILL_NO").attr('disabled', true);

                            return false;
                        }
                        else if (data.Success == false && data.Bill_ID == "-4") {
                            alert("Opening Balance entry is not finalized.Please finalize it and try again");
                            $("#TXN_ID").attr('disabled', true);
                            $("#BILL_NO").attr('disabled', true);

                            return false;
                        }
                        //else if (data.Success == false && data.Bill_ID == "-999") {
                        //    $("#lblChqBookIssueDate").show();                            
                        //    //$("#spnChqBookIssueDate").html(data.ChequeIssueDate);
                        //    $("#spnChqBookIssueDate").show();
                        //    //alert("Opening Balance entry is not finalized.Please finalize it and try again");
                        //    alert("Voucher date : " + data.VoucherDate + " should be greater than or euqal to Cheque book issue date.");
                        //    $("#TXN_ID").attr('disabled', true);
                        //    $("#BILL_NO").attr('disabled', true);
                        //    return false;
                        //}
                    }
                });
            }

        }
    });

    //function for checking whether epay is enabled or not for this user(admin code)
    //$("#Epay").click(function () {


    //});

    //new change done by Vikram on 01-Jan-2014
    $("#Cheque").focus();
    //end of change

    //for populating the Challan date if the Challan No. is entered.
    //$("#CHALAN_NO").on('blur', function () {
    //    if ($("#CHALAN_NO").val() != '') {
    //        $("#CHALAN_DATE").datepicker('setDate', process(ModifiedCurrentDate));
    //    }
    //    else {
    //        $("#CHALAN_DATE").val('');
    //    }
    //});
    //Total Amount auto Calculation  -- added by Aditi Shree on 1 April 2021
    $(":text[class~=TAC]").blur(function () {
        var fltTPEC = 0.0;
        $(":text[class~=TAC]").each(function () {
            var tempValue = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempValue) != NaN) {
                fltTPEC += Number(tempValue);
                $("#txtTotalAmount").val(parseFloat(fltTPEC).toFixed(2));
            }
        });
    });


});//document ready


function resetMasterForm() {
    //$("#divCash").show();
    $("#divCheque").show();
    if ($("#divERemit").is(':visible')) {
        $("#divERemit").hide('slow');
    }
    if ($("#DEDUCTION_AMOUNT").is(':disabled')) {
        $("#DEDUCTION_AMOUNT").attr('disabled', false);
    }
    //$("#divEpay").show();
    $("#Cheque").trigger('click');
    $('.supplierTr').hide();
    $('.ContracorTr').hide();
    //Added By Abhishek kamble 27-May-2014
    $("#trContractorBankDetails").hide();
    // $('.agreement').hide();
    $('.RemittanceTr').hide();
    $('#normalPayeename').show();
    $('#Cheque').attr('checked', true);
    $("#divDeductionOnly").hide();
    MasterTriggerWhenError = false;
    $("#btnMasterCancel").hide();

    if ($("#CHQ_Book_ID").val() == 0) {
        $("#lblChqBookIssueDate").hide();
        $("#spnChqBookIssueDate").hide();
    }
}

//function to fill the dropdownbox  dynamically
function FillInCascadeDropdown(map, dropdown, action) {
    // alert('a' + dropdown);
    $(dropdown).empty();
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

//function to get the contractor supplier name according to id
function setContractorSupplierName(contractorId, contractorOrsupllier, disableName) {
    $.ajax({
        type: "POST",
        url: "/payment/GetContractorSupplierName/" + contractorId + '$' + $("#TXN_ID").val(),
        async: false,
        // data: $("#authSigForm").serialize(),
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
            if (data != "") {
                if (contractorOrsupllier == "C") {
                    $("#PAYEE_NAME_C").val(data);
                    if (disableName) {
                        $("#PAYEE_NAME_C").attr('readonly', 'readonly');
                    } else {
                        $("#PAYEE_NAME_C").removeAttr('readonly');
                    }
                } else {

                    $("#PAYEE_NAME_S").val(data);
                    if (disableName) {
                        $("#PAYEE_NAME_S").attr('readonly', 'readonly');
                    } else {
                        $("#PAYEE_NAME_S").RemoveAttr('readonly');
                    }
                }
            }
        }
    });

}

//function to get department name of rem department based on the department id provided
function setRemDepartmentName(deptId, disableName) {

    $.ajax({
        type: "POST",
        url: "/payment/GetRemDepartmentName/" + deptId,
        async: false,
        // data: $("#authSigForm").serialize(),
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
            if (data != "") {

                $("#PAYEE_NAME_R").val(data);

                if (disableName) {
                    $("#PAYEE_NAME_R").attr('readonly', 'readonly');
                } else {
                    $("#PAYEE_NAME_R").removeAttr('readonly');
                }

            }
        }
    });

}

//function to edit master payment details
function EditPaymentMaster(urlParam) {

    blockPage();

    //get the amount table updates
    GetAmountTableDetails(urlParam);

    $("#TransactionForm").hide('slow');

    $("#trnsShowtable").show('slow');

    $("#trShowHideLinkTable").show();

    // $('#masterPaymentForm').trigger("reset");
    $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

    resetMasterForm();

    $('#MasterData').load('/Payment/EditPaymentMasterDetails/' + urlParam, function () {

        //get the amounts for edit operation validations
        masterChqAmtToEdit = parseFloat($("#CHQ_AMOUNT").val());

        masterCashAmount = parseFloat($("#CASH_AMOUNT").val());

        //show cancel button
        $("#btnMasterCancel").show();

        //hide reset button
        $("#btnMasterReset").hide();

        // $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + urlParam, function () {

        $("#MasterDataEntryDiv").show();

        $('#TXN_ID').trigger('change');

        if ($("#CHQ_AMOUNT").val() == 0 && $("#DEDUCTION_AMOUNT").val() != 0) {
            $(".chequeTr").hide();

            $("#CHQ_AMOUNT").prop("readonly", 'readonly');

            $("#DeductionOnly").attr("checked", "checked");

            $('#DeductionOnly').trigger('change');

        }
        else {
            //if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {

            //modified by Abhishek kamble 29-May-2014
            if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                $(".chequeTr").hide();
            }
            else if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                if ($("#CHQ_AMOUNT").val() == 0) {
                    $(".chequeTr").hide();
                    $('#CONC_Account_ID').show();
                    $('#trContractorBankDetails').show();
                }
                else {

                    //  alert("test11");
                    $(".chequeTr").show();
                    $('#CONC_Account_ID').show();
                    $('#trContractorBankDetails').show();

                }

            } else {//default
                //  alert("test22");

                $(".chequeTr").show();
                $('#CONC_Account_ID').show();
                $('#trContractorBankDetails').show();

            }


            if ($("#LevelID").val() == 5) {

                //modified by Abhishek kamble 29-May-2014
                if (($("#TXN_ID option:selected").val() == "137$Q") || ($("#TXN_ID option:selected").val() == "834$Q") || ($("#TXN_ID option:selected").val() == "469$Q")) {
                    $(".chequeTr").hide();
                }
                else if (($("#TXN_ID option:selected").val() == "47$Q") || ($("#TXN_ID option:selected").val() == "737$Q") || ($("#TXN_ID option:selected").val() == "1484$Q") || ($("#TXN_ID option:selected").val() == "1974$Q")) {//PMGSY3
                    if ($("#CHQ_AMOUNT").val() == 0) {
                        $("#chqseriesTr").hide();
                    }
                    else {
                        $("#chqseriesTr").show('slow');
                    }

                } else {  //default                  
                    $("#chqseriesTr").show('slow');
                    $("#lblChqBookIssueDate").show();
                    $("#spnChqBookIssueDate").show();
                }

            } else {
                $("#chqseriesTr").hide();
            }
        }

        if (OnlyCash == "Y") {

            $("#Cash").trigger('click');

        }

        //Added By Abhishek kamble 6Apr2015 for Advice no  
        if ($("#IsAdvicePayment").val() == "A") {
            $('#Advice').trigger('click');
        }

        //if edit is of the cheque details triger the cheque book series 
        if ($('#Cheque').is(':checked')) {
            if ($("#LevelID").val() == 5) {
                //commented by Koustubh Nakate on 20/09/2013
                $("#CHQ_Book_ID").trigger('change');

                //added by Koustubh Nakate on 20/09/2013 for get cheque numbers
                /* if ($("#CHQ_Book_ID").val() !== 0 ) {

                     $.ajax({
                         type: "POST",
                         url: "/payment/GetAllAvailableCheques/" + $("#CHQ_Book_ID").val(),
                         async: false,
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

                             availableCheques.length = 0;
                             if (data != "") {
                                 if ($("#CHQ_NO").data('autocomplete')) {
                                     $("#CHQ_NO").autocomplete("destroy");
                                     $("#CHQ_NO").removeData('autocomplete');
                                 }

                                 availableCheques = data;

                                 $("#CHQ_NO").autocomplete({
                                     source: availableCheques
                                 });

                             } 
                         }
                     });
                 }*/




            }
            //add the cheque number in the available cheque array for validation
            availableCheques.push($("#CHQ_NO").val())

        }
        else {
            $("#chqseriesTr").hide();
        }

        //RELOAD load payment grid
        $("#PaymentGridDivList").jqGrid('GridUnload');
        loadPaymentGrid(urlParam);


        if (parseFloat($("#TotalAmtToEnterCachAmount").text()) == 0) {
            $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        }
        else {
            $("#AMOUNT_C").val(0).removeAttr("readonly");
        }

        if ($('#Epay').is(':checked')) {
            //disable dates
            $("#BILL_DATE").datepicker("disable").attr("readonly", "readonly");
            $("#CHQ_DATE").datepicker("disable").attr("readonly", "readonly");
            $("#CHALAN_DATE").datepicker("disable").attr("readonly", "readonly");

        } else {

            $("#BILL_DATE").datepicker("enable").removeAttr("readonly");
            $("#CHQ_DATE").datepicker("enable").removeAttr("readonly");
            $("#CHALAN_DATE").datepicker("enable").removeAttr("readonly");

        }

        unblockPage();

        //added by Koustubh Nakate on 04/10/2013 to make payment mode readonly 
        $('#Cash').attr('disabled', true);
        $('#Cheque').attr('disabled', true);
        $('#Epay').attr('disabled', true);
        $('#ERem').attr('disabled', true);
        $('#DeductionOnly').attr('disabled', true);
        $('#Advice').attr('disabled', true);
        //Added By Abhishek kamble 14-Apr-2014
        //&& ($("#TXN_ID option:selected").val() != "47$Q") && ($("#TXN_ID option:selected").val() != "737$Q")
        if (($("#TXN_ID option:selected").val() != "137$Q") && ($("#TXN_ID option:selected").val() != "834$Q") && ($("#TXN_ID option:selected").val() != "469$Q")) {
            //$(".chequeTr").show();
            //$("#chqseriesTr").show();

            if ($("#CHQ_AMOUNT").val() == 0) {
                $(".chequeTr").hide();
                $("#chqseriesTr").hide();
            }
            else {
                $(".chequeTr").show();
                $("#chqseriesTr").show();
                $("#lblChqBookIssueDate").show();
                $("#spnChqBookIssueDate").show();
            }

        }

        //Added By Abhishek kamble 27-May-2014            
        // $("#MAST_CON_ID_C").trigger("change");

    });
    //});

}

//function to view finalized master details payment commented as termed as not required during testing 25/06/2013 
function ViewPaymentMaster(urlParam) {
    return false;
    /*  blockPage();
      
      $("#TransactionForm").hide('slow');
  
      $("#trnsShowtable").show('slow');
  
      $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
        
  
      $('#MasterData').load('/Payment/EditPaymentMasterDetails/' + urlParam, null, function (data) {
  
         // $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + urlParam, function () {
          //get the amount table updates
          GetAmountTableDetails(urlParam);
  
          $("#MasterDataEntryDiv").show('slow');
  
          $('#TXN_ID').trigger('change');
  
          if (parseFloat($("#TotalAmtEnteredChqAmount").text()) == 0 && parseFloat($("#TotalAmtEnteredCachAmount").text()) == 0 && parseFloat($("#TotalAmtEnteredDedAmount").text()) == 0) {
              if ($("#TXN_ID") != null) {
                  $("#TXN_ID").removeAttr('disabled');
              }
          }
  
  
          loadPaymentGrid(urlParam);
  
          //reset the deduction form 
         // $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
  
         // resetDetailsForm();
  
         
          $("#BILL_DATE").datepicker("disable").attr("readonly", "readonly");
          $("#CHQ_DATE").datepicker("disable").attr("readonly", "readonly");
          $("#CHALAN_DATE").datepicker("disable").attr("readonly", "readonly");
  
          $("#btnSubmit").hide();
          $("#btnUpdate").hide();
          $("#btnMasterReset").hide();
          $("#formTable :input ").prop("readonly", 'readonly');
          $("#formTable  select").prop("disabled", true);
          $("#formTable :radio").prop("disabled", true);
  
  
              //if edit is of the cheque details triger the cheque book series 
          if (!$('#Cheque').is(':checked')) {
              $("#chqseriesTr").hide();
          }
          
          if ($('#Epay').is(':checked'))
          {
              $("#CHQ_NO").val("").hide();
              $("#EPAY_NO").show();
          }
         
        
  
          unblockPage();
  
          });
     // });
     */
}

//function to delete the payment master details on data entry page
function DeletePaymentMaster(urlParam) {

    var Delete = confirm("Are you sure you want to delete the master details?");

    if (Delete) {
        blockPage();
        $.ajax({
            type: "POST",
            url: "/payment/DeletetMasterPaymentDetails/" + urlParam,
            // async: false,
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

                if (data.result == 1) {

                    //do not reload as functionality is changed to show details of only one payment on data entry page
                    // $("#PaymentMasterList").jqGrid().setGridParam({ url: '/Payment/ListMasterPaymentDetailsForDataEntry/' }).trigger("reloadGrid");

                    $('#PaymentMasterList').jqGrid('clearGridData');

                    $("#TXN_ID").removeAttr('disabled');

                    $("#BILL_NO").removeAttr('disabled');

                    alert("Master Payment Deleted Successfuly.");

                    $(':input', '#masterPaymentForm').not("#BILL_MONTH").not("#BILL_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
                    resetMasterForm();

                    $("#btnSubmit").show();

                    $("#btnMasterReset").show();

                    $("#btnUpdate").hide();

                    $("#btnMasterCancel").hide();


                    $("#trnsShowtable").hide('slow');

                    $("#TransactionForm").hide('slow');

                    $(':input', '#TransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

                    $("#masterListGrid").hide('slow');

                    $("#MasterDataEntryDiv").show('slow');

                    availableCheques.length = 0;

                    //commented by Koustubh Nakate on 04/10/2013  
                    //  $("#CHQ_Book_ID").empty();  //new change done by Vikram

                    //FillInCascadeDropdown(null, "#CHQ_Book_ID", "/Payment/GetchequebookSeries/");

                    $("#BILL_DATE").val($("#CURRENT_DATE_Hidden").val());
                    $("#CHQ_DATE").val($("#CURRENT_DATE_Hidden").val());
                    $("#CHALAN_DATE").val($("#CURRENT_DATE_Hidden").val());

                    //added by Koustubh Nakate on 26/09/2013 to show month and year selection div 
                    $("#tblOptions").show('slow');

                    //added by Koustubh Nakate on 04/10/2013 
                    //if ($('#MasterDataEntryDiv').is(':visible')) {
                    //    alert('a');

                    //}

                    //   $("#AddNewMasterDetails").trigger('click');

                    if (!$('#masterPaymentForm').is(':visible')) {

                        $('#masterPaymentForm').show('slow');


                    }

                    return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else if (data.result == -2) {
                    alert("Asset Details has been entered for the selected  transaction type for this payment.Please delete the asset details first .");
                    return false;
                }
                else {

                    alert("Error while deleting Master Payment ");
                    return false;
                }
            }
        }); //end of ajax
    }

}

//function to get closing balances
function getPaymentClosingBalance(month, year) {

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

                $("#lblCash").text(parseFloat(data.Cash).toFixed(2));
                $("#lblBank").text(parseFloat(data.BankAuth).toFixed(2));
            }
        }
    });


}
//function for validation of ADMIN_ND_CODE of current DPIU whether the epay is enabled or not for this DPIU
function ValidateDPIUEpay() {
    $.ajax({
        type: "POST",
        url: "/payment/ValidateDPIUEpayment/",
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
            if (data.success == false) {
                isValid = false;
                alert('User is not authorized to make epayment.');
                $("#Epay").attr('checked', false);
                $("#btnMasterReset").trigger('click');
                $("#Cheque").attr('checked', true);
                $("#Cheque").trigger('click');
            }
            else if (data.success == true) {
                isValid = true;
            }
        }
    });

}

function CheckAuthSignDsc() {
    $.ajax({
        type: "POST",
        url: "/payment/ValidateAuthSignRegistration/",
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
            if (data.success == false) {
                isValid = false;
                alert('Digital signature of Authorise Signatory is not registered .To make an e-payment register digital signature of Authorise Signatory .');
                $("#Epay").attr('checked', false);
                $("#btnMasterReset").trigger('click');
                $("#Cheque").attr('checked', true);
                $("#Cheque").trigger('click');
            }
            else if (data.success == true) {
                isValid = true;
            }
        }
    });
}

//function for validation of ADMIN_ND_CODE of current DPIU whether the eremittence is enabled or not for this DPIU
function ValidateDPIUEremittence() {
    $.ajax({
        type: "POST",
        url: "/payment/ValidateDPIUEremittence/",
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
            if (data.success == false) {
                isValid = false;
                alert('User is not authorized to select Eremittence.Either DPIU is not configured to make Eremittence or DPIU TAN No. is not available.');
                $("#ERem").attr('checked', false);
                $("#btnMasterReset").trigger('click');
                $("#Cheque").attr('checked', true);
                $("#Cheque").trigger('click');
            }
            else if (data.success == true) {
                isValid = true;
            }
        }
    });

}

function ModifiedDate(day, month, year) {
    return day + "/" + month + "/" + year;
}



function getContratorBankDetails(mastConID) {
    //alert('1 : In contractor Bank Details id is : ', mastConID);

    $("#spnBankName").html('');
    $("#spnIFSCCode").html('');
    $("#spnBankAccNumber").html('');
    //PFMS Validations
    $('#conAccountId').val('');


    $('#CONC_Account_ID').show();
    $('#trContractorBankDetails').show();

    $('#CONC_Account_ID').rules('add', {
        required: true,
        messages: {
            required: 'Contractor bank Details is Required'
        }
    });
    // $('.CONC_Account_ID').show();
    //$('.trContractorBankDetails').show();
    if (mastConID == "-1") {
        if ($('#Cheque').is(':checked')) {
            $('#PAYEE_NAME_S').attr('readOnly', false);
        }
        else {
            alert("Contractor bank details not present.");

        }
    }
    else {

        if (mastConID != 0 || mastConID != "") {
            // alert("ABC")
            FillInCascadeDropdown(null, "#CONC_Account_ID", "/Payment/GetContratorBankNameAccNoAndIFSCcode/" + mastConID + '$' + $("#fundType").val() + '$' + $("#TXN_ID option:selected").val() + '$' + $('#Advice').is(':checked') + '$' + $('#Cheque').is(':checked'));
            //    $("#trContractorBankDetails").show();
        }
        else {

        }
        //Comment on 25-10-2021
        //$('#PAYEE_NAME_S').attr('disabled', true);

    }
}

$('#CONC_Account_ID').change(function () {

    // var arr = $('#CONC_Account_ID').text().split(':');

    var arr = $("#CONC_Account_ID option:selected").text().split(':');

    // alert("All = " + $("#CONC_Account_ID option:selected").text())



    $("#spnBankName").html(arr[0]);
    $("#spnIFSCCode").html(arr[1]);
    $("#spnBankAccNumber").html(arr[2]);


    //$("#spnBankName").html(arr[0].replace("---Select Account---", ""));
    //$("#spnIFSCCode").html(arr[1]);
    //$("#spnBankAccNumber").html(arr[2]);


    //PFMS Validations
    $('#conAccountId').val($('#CONC_Account_ID').val());
});

/*
function getContratorBankDetails(mastConID) {
    //   alert('aaa' + mastConID)


    if (mastConID == "-1") {
        if ($('#Cheque').is(':checked')) {
            $('#PAYEE_NAME_S').attr('readOnly', false);
        }
        else {
            alert("Contractor bank details not present.");

        }
    }
    else {
        $('#PAYEE_NAME_S').attr('disabled', true);

        $.ajax({
            type: "POST",
            url: "/payment/GetContratorBankNameAccNoAndIFSCcode/" + mastConID + '$' + $("#fundType").val() + '$' + $("#TXN_ID option:selected").val() + '$' + $('#Advice').is(':checked') + '$' + $('#Cheque').is(':checked')
                    + '$' + $('#BILL_DATE').val().replace(/\//g, '-'),//MF Advice Payment
            async: false,
            // data: $("#authSigForm").serialize(),
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
                if (data.Success == true) {
                    //alert('avc');
                    $("#trContractorBankDetails").show();

                    $("#spnBankAccNumber").html(data.BankAccNumber);
                    $("#spnIFSCCode").html(data.BankIFSCCode);
                    $("#spnBankName").html(data.BankName);
                    //PFMS Validations
                    $('#conAccountId').val(data.BankAccountId);
                    //alert(data.BankAccountId);

                }
                else if (data.Success == false) {
                    if (data.message != undefined) {
                        alert(data.message);
                    }
                    else {
                        alert("Contractor bank details not present.");
                    }
                    $("#spnBankAccNumber").html("-");
                    $("#spnIFSCCode").html("-");
                    $("#spnBankName").html("-");


                }
                else {
                    alert("An error ocured while proccessing your request.");
                    $("#spnBankAccNumber").html("-");
                    $("#spnIFSCCode").html("-");
                    $("#spnBankName").html("-");

                }
            }

        });
    }
}
*/