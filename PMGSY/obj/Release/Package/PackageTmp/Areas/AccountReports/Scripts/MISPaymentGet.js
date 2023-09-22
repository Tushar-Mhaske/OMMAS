﻿
jQuery.validator.addMethod("isdpiurequired", function (value, element) {
    if ($("#ddlDPIU option:selected").val() == 0) {
        return false;
    }
    else {
        return true;
    }
}, "");

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



//jQuery.validator.addMethod("requireddpiu", function (value, element, param) {

//    var IsDPIU = $('#rdoDPIU').val();
//    var DPIU = $('#ddlDPIU').val();

//    if ($("#rdoDPIU").is(":checked")) {
//        if (IsDPIU == "D" && DPIU == 0) {
//            return false;
//        }
//        else {
//            return true;
//        }
//    } else {
//        return true;
//    }
//});
//jQuery.validator.unobtrusive.adapters.addBool("requireddpiu");


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

    if ($("#isRedirection").val())
    {
        $("#rdoMonthly").attr("checked", false);
        $("#rdoPeriodic").attr('checked', true);
        
        //$(".tdlblStDate").show();
        //$(".tdtxtStDate").show();
        //$(".tdlblEndDate").show();
        //$(".tdtxtEndDate").show();
    }

    if ($("#levelId").val() == 4) {
        $("#ddlSRRDA").val($("#AdminNdCode").val());
    }


    $.validator.unobtrusive.parse("#frmBillDetails");


    //Added by abhinav pathak on 06-DEC-2018
    //
    var adminNdCode = $('#ddlSRRDA option:selected').val();
    if ($("#levelId").val() != 5) {

        $.ajax({
            url: '/AccountReports/Account/PopulateDPIUForStates/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlDPIU').empty();
                $.each(data, function () {
                    $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
               
            }
        });
    }

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
        $(".tdlblStDate").hide();
        $(".tdtxtStDate").hide();
        $(".tdlblEndDate").hide();
        $(".tdtxtEndDate").hide();
    }

    $("#rdoDPIU").click(function () {
        $("#divValidationMessages").empty();
        $("#ddlDPIU").show("slow");
        $("#lblmandatory").show("slow");
    });


    $("#rdoSRRDA").click(function () {
        $("#divValidationMessages").empty();
        $("#ddlDPIU").hide("slow");
        $("#lblmandatory").hide("slow");
        $("#ddlDPIU").val(null);
        $("#ddlDPIU").blur();
    });


    $("#rdoPeriodic").click(function () {

        //Set month 0
        $("#Month").val(0);


        $("#divValidationMessages").empty();
        $("#trMonthYear").hide();
        //$("#Year option:nth(0)").attr("selected", "selected");
        //$("#Month option:nth(0)").attr("selected", "selected");
        $(".lablMonth").hide();
        $("#txtMonth").hide();
        $(".lblYear").hide();
        $("#txtYear").hide();
        $(".tdlblStDate").show();
        //$(".tdtxtStDate").show("slow");
        // $(".tdtxtStDate").val(null);
        //  $(".tdtxtStDate").blur();
        $(".tdlblEndDate").show();
        //  $(".tdtxtEndDate").show("slow");
        // $("#EndDate").val(null);
        // $("#EndDate").focus();
        //$("#EndDate").blur()
        // $("#StartDate").focus();
    });

    $("#rdoPeriodic").trigger('click');

    $("#rdoYearly").click(function () {

        //Set month 0
        $("#Month").val(0);

        $("#divValidationMessages").empty();

        $("#Year option:last").attr("selected", "selected");
        //$("#Month option:nth(0)").attr("selected", "selected");
        //$("#Year option:nth(1)").attr("selected", "selected");
        //$("#trMonthYear").show("slow");
        $(".lablMonth").attr("disabled", true);
        $("#txtMonth").attr("disabled", true);
        $(".lablMonth").hide();
        $("#txtMonth").hide();
        $(".lblYear").show();
        $("#txtYear").show();
        $(".tdlblStDate").hide();
        $(".tdtxtStDate").hide();
        $(".tdlblEndDate").hide();
        $(".tdtxtEndDate").hide();
        $("#StartDate").val('');
        $("#EndDate").val('');

        FillInCascadeDropdown("#Year", "/AccountsReports/GetFullYears");
    });


    $("#ddlSRRDA").change(function () {
        var adminNdCode = $('#ddlSRRDA option:selected').val();
        $.ajax({
            url: '/AccountReports/Account/PopulateDPIUForStates/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlDPIU').empty();
                $.each(data, function () {
                    $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });

    });

    $("#rdoMonthly").click(function () {

        $("#divValidationMessages").empty();

        $(".lablMonth").show();
        $("#txtMonth").show();
        $(".lblYear").show();
        $("#txtYear").show();
        $(".tdlblStDate").hide();
        $(".tdtxtStDate").hide();
        $(".tdlblEndDate").hide();
        $(".tdtxtEndDate").hide();
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

            //$("#spnhdBillDetails").trigger('click');

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/AccountReports/Account/ShowMISPaymentReport/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmBillDetails").serialize(),
                success: function (response) {
                    $.unblockUI();
                    $("#dvBillDetails").html(response);
                    //$("#ToolTables_tblMonthlyAccount_0").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    //$("#ToolTables_tblMonthlyAccount_1").removeClass('DTTT_button').removeClass('ui-state-default').removeClass('DTTT_button_collection');
                    //$("#tblAnnualAccount_info").css('text-align', 'left');

                    // $.unblockUI();
                    //if ($("#TotalRecords").val() > 0) {

                    //    $("#dvBillDetails").hide("slow");
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

