jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();

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

    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    var endDate = $('#ToDate').datepicker("getDate");
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

    $.validator.unobtrusive.parse("#frmAssetRegisterDetails");

    var date = new Date();
    //var month = date.getMonth() + 1;   
    var month = $("#Month").val();
    var year = $("#Year").val();
    $("#ddlMonth option:nth(" + month + ")").attr("selected", "selected");


    //$("#ddlYear option:eq(1)").attr("selected", "selected");
    $("#ddlYear").val(year);
    //$("#ddlYear option:last").attr("selected", "selected");

    $('#FromDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a From Date',
        buttonImageOnly: true,
        title: "From Date",
        changeMonth: true,
        changeYear: true,
        buttonText: "From Date",
    });

    $('#ToDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a To Date',
        buttonImageOnly: true,
        title: "To Date",
        changeMonth: true,
        changeYear: true,
        buttonText: "To Date",
        maxDate: '0'
    });

    $("#rdoDPIU").click(function () {
        $("#ddlDPIU").show("slow");
        $("#lblDPIU").show("slow");
    });

    $("#rdoSRRDA").click(function () {
        $("#ddlDPIU").hide("slow");
        $("#lblDPIU").hide("slow");
        $("#ddlDPIU option:nth(0)").attr("selected", "selected");
    });


    $("#rdoMonthly").click(function () {

        $(".tdMonthly").show();
        $(".tdPriodic").hide();

        //reset dates
        $("#FromDate").val(null);
        $("#ToDate").val(null);
    });


    $("#rdoPeriodic").click(function () {

        $(".tdPriodic").show();
        $(".tdMonthly").hide();

        $("#ddlMonth option:nth(0)").attr("selected", "selected");
        $("#ddlYear option:nth(0)").attr("selected", "selected");

        $(function () {
            $('#ToDate').focus();
            $('#FromDate').focus();
        });

    });

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

    $("#btnViewDetails").click(function () {
        $("#spCollapseIconS").trigger('click');
        $("#MonthName").val($("#ddlMonth option:selected").text());

        $("#FundStateCentralName").val($("#FundCentralState option:selected").text());

        if ($("#DPIU").val() > 0) {
            $("#DPIUName").val($("#DPIU option:selected").text());
        }

        if ($("#FundCentralState").val() == "0") {
            $("#AssetPurchaseDetails").val("Assets purchased from Central and State Administrative Fund");
        } else if ($("#FundCentralState").val() == "C") {
            $("#AssetPurchaseDetails").val("Assets purchased from Central Administrative Fund");
        }
        else if ($("#FundCentralState").val() == "S") {
            $("#AssetPurchaseDetails").val("Assets purchased from State Administrative Fund");
        }


        if ($("#frmAssetRegisterDetails").valid()) {
            if (ValidateForm() == true) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    url: "/AccountReports/Account/AssetRegisterReport/",
                    type: "POST",
                    async: false,
                    cache: false,
                    data: $("#frmAssetRegisterDetails").serialize(),
                    success: function (data) {
                        $.unblockUI();

                        $("#dvAssetRegisterDetails").html("");
                        $("#dvAssetRegisterDetails").html(data);
                    }
                });
                $.unblockUI();
            }
        }
    });

    $('#btnViewDetails').trigger('click');

   

    $("#ddlMonth").change(function () {

        //$('#ddlMonth option:selected').removeAttr('selected');

        //$("#ddlMonth option:nth(" + $("#ddlMonth").val() + ")").attr("selected", "selected");


        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

        //Added By Abhishek kamble 17-feb-2014
        $("#Month").val($("#ddlMonth option:selected").val());
    });

    $("#ddlYear").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

        $("#Year").val($("#ddlYear option:selected").val());
    });

});

function ValidateForm() {

    if ($("#rdoDPIU").is(":checked")) {
        if ($("#ddlDPIU").val() == 0) {
            alert("Please select DPIU");
            return false;
        }
    }

    if ($("#rdoMonthly").is(":checked")) {
        if ($("#ddlMonth").val() == 0) {
            alert("Please select Month");
            return false;
        }
        if ($("#ddlYear").val() == 0) {
            alert("Please select Year");
            return false;
        }
    }

    if ($("#rdoPeriodic").is(":checked")) {
        if ($("#FromDate").val() == "") {
            alert("Please enter From Date");
            return false;
        }

        else if ($("#ToDate").val() == "") {
            alert("Please enter To Date");
            return false;
        }
    }
    return true;
}
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