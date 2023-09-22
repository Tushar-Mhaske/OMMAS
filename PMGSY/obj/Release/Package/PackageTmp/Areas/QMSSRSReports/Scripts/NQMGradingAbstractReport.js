jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = parseInt($('#ddlFrmYearNQMGradingAbstract').val());
    var toDate = parseInt($('#ddlToYearNQMGradingAbstract').val());

    var fromMonth = parseInt($('#ddlFrmMonthNQMGradingAbstract').val());
    var toMonth = parseInt($('#ddlToMonthNQMGradingAbstract').val());

    if (fromDate == toDate) {
        if (fromMonth > toMonth) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        if (fromDate > toDate) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");

$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmNQMGrAbs');
    $.unblockUI();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    //$("#loadReport").load("/QualityMonitoringArea/QualityMonitoring/NQMGradingAbstractReport/" + $("#ddlFrmMonthNQMGradingAbstract").val() + "$" + $("#ddlFrmYearNQMGradingAbstract").val() + "$" + $("#ddlToMonthNQMGradingAbstract").val() + "$" + $("#ddlToYearNQMGradingAbstract").val(), $.unblockUI());

    $("#btnViewNQMGradingAbstractReport").click(function () {
        

        //validation start
        /*if ($("#ddlFrmMonthNQMGradingAbstract").val() == 0)
        {
            alert("Please select From Month.");
            return false;
        }
        if ($("#ddlFrmYearNQMGradingAbstract").val() == 0) {
            alert("Please select From Year.");
            return false;
        }
        if ($("#ddlToMonthNQMGradingAbstract").val() == 0) {
            alert("Please select To Month.");
            return false;
        }
        if ($("#ddlToYearNQMGradingAbstract").val() == 0) {
            alert("Please select To Year.");
            return false;
        }*/
        //validation end

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

        //$("#loadReport").html("");
        
        //$("#loadReport").load("/QualityMonitoringArea/QualityMonitoring/NQMGradingAbstractReport/" + $("#ddlFrmMonthNQMGradingAbstract").val() + "$" + $("#ddlFrmYearNQMGradingAbstract").val() + "$" + $("#ddlToMonthNQMGradingAbstract").val() + "$" + $("#ddlToYearNQMGradingAbstract").val(),$.unblockUI());
        if ($("#frmNQMGrAbs").valid()) {

            $("#FromMonthName").val($("#ddlFrmMonthNQMGradingAbstract option:selected").text());
            $("#FromYearName").val($("#ddlFrmYearNQMGradingAbstract option:selected").text());
            $("#ToMonthName").val($("#ddlToMonthNQMGradingAbstract option:selected").text());
            $("#ToYearName").val($("#ddlToYearNQMGradingAbstract option:selected").text());

            $.ajax({
                url: "/QMSSRSReports/QMSSRSReports/NQMGradingAbstractReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmNQMGrAbs").serialize(),
                success: function (data) {
                    $("#loadReport").html('');
                    $("#loadReport").html(data);
                    closableNoteDiv("divCompRoads", "spnCompRoads");
                },
                error: function () {
                    alert("error");
                }
            })
        }
        $.unblockUI();
    });

    $("#btnViewNQMGradingAbstractReport").trigger('click');
});