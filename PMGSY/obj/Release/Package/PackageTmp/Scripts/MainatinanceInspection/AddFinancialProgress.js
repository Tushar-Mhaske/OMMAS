
var isValid;
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

    if ($("#Operation").val() == "E") {
        if ($("#radioYes").is(':checked')) {
            $("#radioYes").trigger('click');
        }
    }

    //save the financial progress details
    $("#btnAddFinancialDetails").click(function () {
        ///Added by SAMMED A. PATIL on 19JUNE2017 for new field Maintenance Type
        //if ($('#rdbTypeR').is(':checked') == true) {
        //    $('#maintenanceType').val("R");
        //}
        //else if ($('#rdbTypeP').is(':checked') == true) {
        //    $('#maintenanceType').val("P");
        //}

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

        $("#ProposalContractCode").val($("#ddlMaintenanceNo option:selected").val());

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

        ValidateAgreementCost($("#ProposalCode").val(), $("#ValueOfWorkThisMonth").val(), $("#PaymentThisMonth").val(),$("#ProposalContractCode").val());

        if (isValid == false) {

            alert('The value of work exceeding the overall agreement amount.');
            if (confirm("Are you sure you want to add Financial details?")) {



                $("#PaymentLastMonth").val($("#TotalPayment").val());
                $("#ValueOfWorkLastMonth").val($("#TotalValueofwork").val());

                if ($("#frmAddFinancialProgress").valid()) {
                    $.ajax({
                        type: 'POST',
                        url: '/MaintainanceInspection/AddFinancialProgress/',
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

            $("#PaymentLastMonth").val($("#TotalPayment").val());
            $("#ValueOfWorkLastMonth").val($("#TotalValueofwork").val());

            if ($("#frmAddFinancialProgress").valid()) {
                $.ajax({
                    type: 'POST',
                    url: '/MaintainanceInspection/AddFinancialProgress/',
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
        ///Added by SAMMED A. PATIL on 19JUNE2017 for new field Maintenance Type
        //if ($('#rdbTypeR').is(':checked') == true) {
        //    $('#maintenanceType').val("R");
        //}
        //else if ($('#rdbTypeP').is(':checked') == true) {
        //    $('#maintenanceType').val("P");
        //}

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

        $("#ProposalContractCode").val($("#ddlMaintenanceNo option:selected").val());


        ValidateAgreementCost($("#ProposalCode").val(), $("#ValueOfWorkThisMonth").val(), $("#PaymentThisMonth").val(), $("#ProposalContractCode").val());

        if (isValid == false) {

            alert('The value of work exceeding the overall agreement amount.');
            if (confirm("Are you sure you want to update Financial details?")) {

                if ($("#frmAddFinancialProgress").valid()) {


                    $("#PaymentLastMonth").val($("#TotalPayment").val());
                    $("#ValueOfWorkLastMonth").val($("#TotalValueofwork").val());
                    $.ajax({
                        type: 'POST',
                        url: '/MaintainanceInspection/EditFinancialProgress/',
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

            $("#PaymentLastMonth").val($("#TotalPayment").val());
            $("#ValueOfWorkLastMonth").val($("#TotalValueofwork").val());
            $.ajax({
                type: 'POST',
                url: '/MaintainanceInspection/EditFinancialProgress/',
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
    });



    $("#btnCancelFinancialDetails").click(function () {
        $("#divAddFinancialProgress").hide('slow');
    });


    $("#ValueOfWorkThisMonth").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#ValueOfWorkThisMonth").blur(function () {

        
        if ($("#ValueOfWorkThisMonth").val() == '') {
            if ($("#Operation").val() == "A") {
                $("#lblValueofWorkLastMonth").html($("#ValueOfWorkLastMonth").val());
            }
            var totalNewValue = $("#TotalValueofwork").val();
            $("#totalValue").html(totalNewValue);
        }
        else {
            if ($("#Operation").val() == "A") {
                $("#lblValueofWorkLastMonth").html($("#TotalValueofwork").val());
            }

            var totalNewValue = parseFloat($("#TotalValueofwork").val()) + parseFloat($("#ValueOfWorkThisMonth").val());
            $("#totalValue").html(parseFloat(totalNewValue).toFixed(2));
            //if (parseFloat(totalNewValue).toFixed(2) == "NaN") {
            //    $("#totalValue").html("0");
            //}
            //else
            //{
            //    $("#totalValue").html(parseFloat(totalNewValue).toFixed(2));
            //}

        }

        if ($("#Operation").val() == "E") {
            var value = parseFloat($("#ValueOfWorkThisMonth").val()) + parseFloat($("#ValueOfWorkLastMonth").val());
            $("#totalValue").html(parseFloat(value).toFixed(2));
        }
        //if (parseFloat(value).toFixed(2) == "NaN") {
        //    $("#totalValue").html("0");
        //}
        //else {
        //    $("#totalValue").html(parseFloat(totalNewValue).toFixed(2));
        //}

    });

    $("#PaymentThisMonth").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#PaymentThisMonth").blur(function () {

        if ($("#PaymentThisMonth").val() == '') {
            if ($("#Operation").val() == "A") {
                $("#lblPaymentLastMonth").html($("#PaymentLastMonth").val());
            }
            var totalNewPayment = parseFloat($("#TotalPayment").val()).toFixed(2);
            $("#totalPayment").html(totalNewPayment);
        }
        else {
            if ($("#Operation").val() == "A") {
                $("#lblPaymentLastMonth").html($("#TotalPayment").val());
            }
            var totalNewPayment = parseFloat($("#TotalPayment").val()) + parseFloat($("#PaymentThisMonth").val());
            $("#totalPayment").html(parseFloat(totalNewPayment).toFixed(2));
            //if (parseFloat(totalNewPayment).toFixed(2) == "NaN") {
            //    $("#totalPayment").html("0");
            //}
        }

        if ($("#Operation").val() == "E") {
            var value = parseFloat($("#PaymentThisMonth").val()) + parseFloat($("#PaymentLastMonth").val());
            $("#totalPayment").html(parseFloat(value).toFixed(2));
        }
        //if (parseFloat(value).toFixed(2) == "NaN") {
        //    $("#totalPayment").html("0");
        //}
    });

    $('#datePayment').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            //$(this).datepicker('options', 'setMonth', $("#ddlMonth").val())
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
        $("#divError").hide('slow');
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
function ValidateAgreementCost(proposalCode, valueofWork, valueofPayment,contractCode) {
    var code = proposalCode + '$' + valueofWork + '$' + valueofPayment+'$'+$("#Operation").val()+"$"+contractCode;

    $.ajax({

        type: 'POST',
        url: '/MaintainanceInspection/CheckSanctionCost?id=' + code,
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == false) {
                //alert('The value of work exceeding the total maintenance amount.');
                isValid = false;
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