$.validator.unobtrusive.adapters.add('compareagreementyear', ['year'], function (options) {
    options.rules['compareagreementyear'] = options.params;
    options.messages['compareagreementyear'] = options.message;
});
var isValid;
$.validator.addMethod("compareagreementyear", function (value, element, params) {
    var year = params.year;
       if (year < parseInt($("#ddlYear").val())) {
        return false;
    }
    return true;
});

//$.validator.addMethod("comparefinancialvalidationwork", function (value, element, params) {

//    if (parseFloat($("#EXEC_VALUEOFWORK_THISMONTH").val()) < parseFloat($("#EXEC_PAYMENT_THISMONTH").val())) {
//        return false;
//    }
//    else
//    {
//        return true;
//    }
//});
//jQuery.validator.unobtrusive.adapters.addBool("comparefinancialvalidationwork");


$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    $("#lblPaymentDate").hide();
    $("#lblRequired").hide();
    $("#regdatePicker").hide();

    //on selection of yes radio button
    $("#radioYes").click(function () {

        var selValue = $("#radioYes").val();
        if (selValue == "Y") {
            $("#lblPaymentDate").show('slow');
            $("#lblRequired").show('slow');
            $("#datePayment").show('slow');
            $("#regdatePicker").show('slow');
            $(".ui-datepicker-trigger").show('slow');
        }
        else {
            $("#lblPaymentDate").hide();
            $("#lblRequired").hide();
            $("#datePayment").hide();
            $("#regdatePicker").hide();
            $(".ui-datepicker-trigger").hide();
        }

    });

    $('#imgCloseProgressDetails').click(function () {

        $("#divAddFinancialProgress").hide("slow");
        $("#divError").hide("slow");

    });

    if ($("#IsFinalPaymentBefore").val() == "Y")
    {
        $("#radioYes").trigger('click');
    }


    //on selection of no radio button
    $("#radioNo").click(function () {
        $("#lblPaymentDate").hide();
        $("#lblRequired").hide();
        $("#datePayment").hide();
        $("#regdatePicker").hide();
    });

    if ($("#Operation").val() == "E")
    {
        if ($("#radioYes").is(':checked')) {
            $("#radioYes").trigger('click');
        }
    }

    //save the financial progress details
    $("#btnAddFinancialDetails").click(function () {
        //for validation of payment date


        if (($("#radioYes").is(':checked'))) {
            if ($("#datePayment").val() == "") {
                $("#msgDateValidation").show();
                $("#msgDateValidation").html("<span style='color:red'>Please enter Final Payment Date.</span>");
                return false;
            }
        }

        if ($("#radioYes").is(':checked')) {
            if (validateDate() == false) {
                return false;
            }
        }


        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }



        ValidateAgreementCost($("#IMS_PR_ROAD_CODE").val(), $("#EXEC_VALUEOFWORK_THISMONTH").val(), $("#EXEC_PAYMENT_THISMONTH").val());

        if (isValid == false) {

            alert('The value of work exceeding the overall agreement amount.');
            if (confirm("Are you sure you want to add Financial details?")) {



                $("#EXEC_PAYMENT_LASTMONTH").val($("#TotalPayment").val());
                $("#EXEC_VALUEOFWORK_LASTMONTH").val($("#TotalValueofwork").val());

                if ($("#frmAddFinancialProgress").valid()) {
                    $.ajax({
                        type: 'POST',
                        url: '/Execution/AddFinancialProgress/',
                        data: $("#frmAddFinancialProgress").serialize(),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data.success == true) {
                                alert(data.message);
                                $("#tbFinancialList").trigger('reloadGrid');
                                $("#btnResetRoadDetails").trigger('click');
                                $("#divAddFinancialProgress").html('');
                            }
                            else if (data.success == false) {
                                $("#divError").show();
                                $("#divError").html('<strong>Alert : </strong>' + data.message);
                            }
                        },
                        error: function () {
                            alert("Request can not be processed at this time.");
                        }

                    })
                }
            }
        }
        else {


            $("#EXEC_PAYMENT_LASTMONTH").val($("#TotalPayment").val());
            $("#EXEC_VALUEOFWORK_LASTMONTH").val($("#TotalValueofwork").val());

            if ($("#frmAddFinancialProgress").valid()) {
                $.ajax({
                    type: 'POST',
                    url: '/Execution/AddFinancialProgress/',
                    data: $("#frmAddFinancialProgress").serialize(),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data.success == true) {
                            alert(data.message);
                            $("#tbFinancialList").trigger('reloadGrid');
                            $("#btnResetRoadDetails").trigger('click');
                            $("#divAddFinancialProgress").html('');
                        }
                        else if (data.success == false) {
                            $("#divError").show();
                            $("#divError").html('<strong>Alert : </strong>' + data.message);
                        }
                    },
                    error: function () {
                        alert("Request can not be processed at this time.");
                    }

                })
            }
        }
    });

    //update road details button click
    $("#btnUpdateFinancialDetails").click(function () {


        //for validation of payment date
        if ($("#radioYes").is(':checked')) {


            if (validateDate() == false) {
                return false;
            }
        }

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        ValidateAgreementCost($("#IMS_PR_ROAD_CODE").val(), $("#EXEC_VALUEOFWORK_THISMONTH").val(), $("#EXEC_PAYMENT_THISMONTH").val());
        if (isValid == false) {

            alert('The value of work exceeding the overall agreement amount.');
            if (confirm("Are you sure you want to update Financial details?")) {


                if ($("#frmAddFinancialProgress").valid()) {


                    $("#EXEC_PAYMENT_LASTMONTH").val($("#TotalPayment").val());
                    $("#EXEC_VALUEOFWORK_LASTMONTH").val($("#TotalValueofwork").val());
                    $.ajax({
                        type: 'POST',
                        url: '/Execution/EditFinancialProgress/',
                        data: $("#frmAddFinancialProgress").serialize(),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data.success == true) {
                                alert(data.message);
                                $("#tbFinancialList").trigger('reloadGrid');
                                $("#divAddFinancialProgress").html('');
                            }
                            else if (data.success == false) {
                                $("#divError").show();
                                $("#divError").html('<strong>Alert : </strong>' + data.message);
                            }
                            if (data.success === undefined) {
                                alert("Request can not be processed at this time.Please Try Again.");
                            }
                        },
                        error: function () {
                            alert("Request can not be processed at this time.");
                        }
                    })
                }
            }
        }
        else {

            if ($("#frmAddFinancialProgress").valid()) {


                //$("#EXEC_PAYMENT_LASTMONTH").val($("#TotalPayment").val());
                //$("#EXEC_VALUEOFWORK_LASTMONTH").val($("#TotalValueofwork").val());
                $.ajax({
                    type: 'POST',
                    url: '/Execution/EditFinancialProgress/',
                    data: $("#frmAddFinancialProgress").serialize(),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data.success == true) {
                            alert(data.message);
                            $("#tbFinancialList").trigger('reloadGrid');
                            $("#divAddFinancialProgress").html('');
                        }
                        else if (data.success == false) {
                            $("#divError").show();
                            $("#divError").html('<strong>Alert : </strong>' + data.message);
                        }
                        if (data.success === undefined) {
                            alert("Request can not be processed at this time.Please Try Again.");
                        }
                    },
                    error: function () {
                        alert("Request can not be processed at this time.");
                    }
                })
            }

        }
    });



    $("#btnCancelFinancialDetails").click(function ()
    {
        $("#divAddFinancialProgress").hide('slow');
    });

    $("#EXEC_VALUEOFWORK_THISMONTH").blur(function () {

        if ($("#EXEC_VALUEOFWORK_THISMONTH").val() == '') {
            if ($("#Operation").val() == "A") {
                $("#lblValueOfWorkLastMonth").html($("#EXEC_VALUEOFWORK_LASTMONTH").val());
            }
                var totalNewValue = parseFloat($("#TotalValueofwork").val()).toFixed(2);
                $("#totalValue").html(totalNewValue);
            }
        else {
            if ($("#Operation").val() == "A") {
                $("#lblValueOfWorkLastMonth").html($("#TotalValueofwork").val());
            }
                var totalNewValue = parseFloat($("#TotalValueofwork").val()) + parseFloat($("#EXEC_VALUEOFWORK_THISMONTH").val());
                $("#totalValue").html(parseFloat(totalNewValue).toFixed(2));
            }

            if ($("#Operation").val() == "E") {
                var value = parseFloat($("#EXEC_VALUEOFWORK_THISMONTH").val())+ parseFloat($("#EXEC_VALUEOFWORK_LASTMONTH").val());
                $("#totalValue").html(parseFloat(value).toFixed(2));
            }

    });

    $("#EXEC_PAYMENT_THISMONTH").blur(function () {

        if ($("#EXEC_PAYMENT_THISMONTH").val() == '')
        {
            if ($("#Operation").val() == "A") {
                $("#lblPaymentLastMonth").html($("#EXEC_PAYMENT_LASTMONTH").val());
            }
                var totalNewPayment = parseFloat($("#TotalPayment").val()).toFixed(2);
                $("#totalPayment").html(totalNewPayment);
        }
        else
        {
            if ($("#Operation").val() == "A") {
                $("#lblPaymentLastMonth").html($("#TotalPayment").val());
            }
                var totalNewPayment = parseFloat($("#TotalPayment").val()) + parseFloat($("#EXEC_PAYMENT_THISMONTH").val());
                $("#totalPayment").html(parseFloat(totalNewPayment).toFixed(2));
        }

        if ($("#Operation").val() == "E") {
            //alert('EXEC PAYMENT LAST MONTH' + $("#EXEC_PAYMENT_LASTMONTH").val());
            //alert('EXEC PAYMENT THIS MONTH' + $("#EXEC_PAYMENT_THISMONTH").val());
            var value = parseFloat($("#EXEC_PAYMENT_THISMONTH").val()) + parseFloat($("#EXEC_PAYMENT_LASTMONTH").val());
            $("#totalPayment").html(parseFloat(value).toFixed(2));
        }
    });

    
    $('#datePayment').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear:true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {

        }
    });

    $('#imgCloseFinancetDetails').click(function () {


        if ($("#accordionFinance").is(":visible")) {
            $('#accordionFinance').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $("#btnResetFinancialDetails").click(function () {

        $("#lblPaymentDate").hide();
        $("#lblRequired").hide();
        $("#datePayment").hide();
        $("#regdatePicker").hide();
        $("#divError").hide();

        if ($("#radioYes").is(':checked'))
        {
            $("#lblPaymentDate").show();
            $("#datePayment").show();
            $("#regdatePicker").show();
        }


    });

});
function validateDate() {

    var paymentDate = $("#datePayment").val();
    var dateParts = paymentDate.split('/');
    var month = parseInt(dateParts[1]);
    var year = parseInt(dateParts[2]);

    if (parseInt(month) != parseInt($("#ddlMonth").val())) {
        alert('Month must match the month of payment date.');
        return false;
    }

    if (parseInt(year) != parseInt($("#ddlYear").val())) {
        alert('Year must match the year of payment date.');
        return false;
    }

}

function ValidateAgreementCost(proposalCode,valueofWork,valueofPayment) {
    var code = proposalCode +','+valueofWork +','+valueofPayment+','+$("#Operation").val();

    $.ajax({
        type: 'POST',
        url: '/Execution/CheckSanctionCost?id=' + code,
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == false) {
                isValid = false;
                //  alert('The value of work exceeding the first agreement amount.');
            }
            else {
                isValid = true;
            }

        },
        error: function (data) {
            //alert('Error occurred while processing the request.');
        },
    });

}