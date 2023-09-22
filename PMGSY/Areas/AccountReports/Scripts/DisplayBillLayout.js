
//Added By Abhishek Kamble 12-Nov-2013 Start

jQuery.validator.addMethod("requiredmonthformonthly", function (value, element, param) {

    var IsMonthly = $('#rdoMonthly').val();
    var Month = $('#Month').val();
    if ($("#rdoMonthly").is(":checked")) {
        if (IsMonthly == "M" && Month == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requiredmonthformonthly");

jQuery.validator.addMethod("requiredyearformonthly", function (value, element, param) {

    var IsMonthly = $('#rdoMonthly').val();
    var Year = $('#Year').val();
    if ($("#rdoMonthly").is(":checked")) {
        if (IsMonthly == "M" && Year == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requiredyearformonthly");


jQuery.validator.addMethod("requiredyearforyearly", function (value, element, param) {

    var IsMonthly = $('#rdoYearly').val();
    var Year = $('#Year').val();
    if ($("#rdoYearly").is(":checked")) {




        if (IsMonthly == "Y" && Year == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requiredyearforyearly");

jQuery.validator.addMethod("requiredstartdateforperiodic", function (value, element, param) {

    var IsPeriodic = $('#rdoPeriodic').val();
    var StartDate = $('#StartDate').val();
    if ($("#rdoPeriodic").is(":checked")) {
        if (IsPeriodic == "P" && StartDate == '') {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requiredstartdateforperiodic");

jQuery.validator.addMethod("requiredenddateforperiodic", function (value, element, param) {

    var IsPeriodic = $('#rdoPeriodic').val();
    var EndDate = $('#EndDate').val();

    if ($("#rdoPeriodic").is(":checked")) {
        if (IsPeriodic == "P" && EndDate == '') {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requiredenddateforperiodic");

jQuery.validator.addMethod("requireddpiu", function (value, element, param) {

    var IsDPIU = $('#rdoDPIU').val();
    var DPIU = $('#DPIU').val();

    if ($("#rdoDPIU").is(":checked")) {
        if (IsDPIU == "D" && DPIU == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("requireddpiu");


//Added By Abhishek Kamble 12-Nov-2013 End


jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#StartDate').val();
    var toDate = $('#EndDate').val();

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    //var endDate = $('#ToDate').datepicker("getDate");
    //var currentDate = new Date();

    if (sDate > eDate) {
        return false;
    }
    else {
        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");


jQuery.validator.addMethod("currentdatefieldvalidator", function (value, element, param) {

    var fromDate = $('#StartDate').val();
    var toDate = $('#EndDate').val();

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    var endDate = $('#EndDate').datepicker("getDate");
    var currentDate = new Date();

    if (endDate > currentDate) {
        return false;
    }
    else {
        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("currentdatefieldvalidator");



$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmBillDetails");



    var date = new Date();
    var month = date.getMonth() + 1;


    //$("#Month option:nth(" + month + ")").attr("selected", "selected");
    //$("#Year option:nth(1)").attr("selected", "selected");
    $("#BillType option:nth(1)").attr("selected", "selected");

    $('#StartDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "Start Date",
        changeMonth: true,
        changeYear: true,
        buttonText: "Valid Date",
    });

    $('#EndDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "End Date",
        maxDate: new Date(),
        changeMonth: true,
        changeYear: true,
        buttonText: "End Date",
    });


    if ($("#rdoMonthly").is(":checked")) {
        $("#tdlblStDate").hide("slow");
        $("#tdtxtStDate").hide("slow");
        $("#tdlblEndDate").hide("slow");
        $("#tdtxtEndDate").hide("slow");
    }

    $("#rdoDPIU").click(function () {
        $("#divValidationMessages").empty();
        $("#DPIU").show("slow");
        $("#lblmandatory").show("slow");
    });


    $("#rdoSRRDA").click(function () {
        $("#divValidationMessages").empty();
        $("#DPIU").hide("slow");
        $("#lblmandatory").hide("slow");
        $("#DPIU").val(null);
    });


    $("#rdoPeriodic").click(function () {

        //Set month 0
        $("#Month").val(0);


        $("#divValidationMessages").empty();
        $("#trMonthYear").hide("slow");
        //$("#Year option:nth(0)").attr("selected", "selected");
        //$("#Month option:nth(0)").attr("selected", "selected");
        $("#lablMonth").hide("slow");
        $("#txtMonth").hide("slow");
        $("#lblYear").hide("slow");
        $("#txtYear").hide("slow");
        $("#tdlblStDate").show("slow");
        $("#tdtxtStDate").show("slow");
        $("#tdlblEndDate").show("slow");
        $("#tdtxtEndDate").show("slow");
        $("#EndDate").focus();
        $("#StartDate").focus();
    });

    $("#rdoYearly").click(function () {

        //Set month 0
        $("#Month").val(0);

        $("#divValidationMessages").empty();

        $("#Year option:last").attr("selected", "selected");
        //$("#Month option:nth(0)").attr("selected", "selected");
        //$("#Year option:nth(1)").attr("selected", "selected");
        //$("#trMonthYear").show("slow");
        $("#lablMonth").attr("disabled", true);
        $("#txtMonth").attr("disabled", true);
        $("#lablMonth").hide("slow");
        $("#txtMonth").hide("slow");
        $("#lblYear").show("slow");
        $("#txtYear").show("slow");
        $("#tdlblStDate").hide("slow");
        $("#tdtxtStDate").hide("slow");
        $("#tdlblEndDate").hide("slow");
        $("#tdtxtEndDate").hide("slow");
        $("#StartDate").val('');
        $("#EndDate").val('');

        FillInCascadeDropdown("#Year", "/AccountsReports/GetFullYears");
    });

    $("#rdoMonthly").click(function () {

        $("#divValidationMessages").empty();

        $("#lablMonth").show("slow");
        $("#txtMonth").show("slow");
        $("#lblYear").show("slow");
        $("#txtYear").show("slow");
        $("#tdlblStDate").hide("slow");
        $("#tdtxtStDate").hide("slow");
        $("#tdlblEndDate").hide("slow");
        $("#tdtxtEndDate").hide("slow");
        $("#StartDate").val('');
        $("#EndDate").val('');
        //$("#Month option:nth(" + month + ")").attr("selected", "selected");
        //$("#Year option:nth(" + year + ")").attr("selected", "selected");

        FillInCascadeDropdown("#Year", "/AccountsReports/GetYears");


    });

    $("#spnhdBillDetails").click(function () {
        // $("#spnhdAnnualAccount").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
        $("#spnhdBillDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvDetails").slideToggle("slow");
        //$("#dvBalance").slideToggle("slow");
    });


    $("#btnViewDetails").click(function () {

        //if ($("#BillType").val() == "O") {
        //    return false;
        //}

        $("#BilltypeName").val($("#BillType option:selected").text());
        $("#MonthName").val($("#Month option:selected").text());

        if ($("#DPIU").val() > 0) {
            if (!($("#rdoSRRDA").is(":checked"))) {
                $("#DPIUName").val($("#DPIU option:selected").text());
            } else {
                $("#DPIUName").val("-");

            }
        }
        if ($("#frmBillDetails").valid()) {
            //  if (ValidateForm() == true) {

            $("#spnhdBillDetails").trigger('click');

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/AccountReports/Account/ShowBillReport/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmBillDetails").serialize(),
                success: function (data) {
                    $("#dvBillDetails").html(data);
                    $("#ToolTables_tblMonthlyAccount_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#ToolTables_tblMonthlyAccount_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    $("#tblAnnualAccount_info").css('text-align', 'left');

                    $.unblockUI();
                    //if ($("#TotalRecords").val() > 0) {

                    //    $("#dvDetails").hide("slow");
                    //}
                }
            });
            //  }
        }

    });

    $('#btnViewDetails').trigger('click');

    $("#spCollapseIconS").click(function () {

        if ($("#dvDetails").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#dvDetails").slideToggle(300);
        }

    });

    //Added By Abhishek kamble 15-jan-2013

    $("#Month").change(function () {

        UpdateAccountSession($("#Month option:selected").val(), $("#Year option:selected").val());

    });

    $("#Year").change(function () {

        UpdateAccountSession($("#Month option:selected").val(), $("#Year option:selected").val());

    });


});

//function ValidateForm() {
//    //if ($("#BillType").val() == "O") {
//    //    alert("Please select Bill Type");
//    //    return false;
//    //}
//    //if ($("#rdoDPIU").is(":checked")) {
//    //    if ($("#DPIU").val() == 0) {
//    //        alert("Please select DPIU");
//    //        return false;
//    //    }       
//    //}

//    //if ($("#rdoMonthly").is(":checked")) {
//    //    if ($("#Month").val() == 0) {
//    //        alert("Please select Month");
//    //        return false;
//    //    }

//    //    else 
//    //    if ($("#Year").val() == 0) {
//    //        alert("Please select Year");
//    //        return false;
//    //    }
//    //}

//    //if ($("#rdoYearly").is(":checked")) {
//    //    if ($("#Year").val() == 0) {
//    //        alert("Please select Year");
//    //        return false;
//    //    }

//    //}


//    //if ($("#rdoPeriodic").is(":checked")) {
//    //    if ($("#StartDate").val() == "") {
//    //        alert("Please select Start Date");
//    //        return false;
//    //    }

//    //    else if ($("#EndDate").val() == "") {
//    //        alert("Please select End Date");
//    //        return false;
//    //    }
//    //}

//    return true;
//}


function FillInCascadeDropdown(dropdown, action) {

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var selectedYear;
    $.post(action, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            //Added By Abhishek kamble 15-jan-2014
            if (this.Selected == true) {
                selectedYear = this.Value;
                $("#Year").val(this.Value);
            }
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Reports/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}