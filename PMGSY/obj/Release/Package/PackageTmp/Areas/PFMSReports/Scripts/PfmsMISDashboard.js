
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

    $("#divDashboard").hide();
    $("#divDashBoardTitle").hide();
    $("#divDashBoardBottom").hide();
    
    var date = new Date();
    var month = date.getMonth() + 1;

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
        buttonText: "Click To Select Start Date",
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
        buttonText: "Click To Select End Date",
       
    });

    $('#StartDate').datepicker().datepicker("setDate", new Date("2019/04/01"));
    $('#EndDate').datepicker().datepicker("setDate" , new Date());

    $("#btnViewDetails").click(function () {
    if ($("#frmFilterDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/PFMSReports/PFMSReports/PfmsMisDashboardView/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmFilterDetails").serialize(), 
                success: function (response) {
                    $.unblockUI();
                    $("#divDashboard").html(response);
                    $("#divDashBoardTitle").show();
                    $("#divDashBoardBottom").show();
                    $("#divDashboard").show();
                }
            });
        }

    });

    $("#btnViewDetails").trigger("click");

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


});

