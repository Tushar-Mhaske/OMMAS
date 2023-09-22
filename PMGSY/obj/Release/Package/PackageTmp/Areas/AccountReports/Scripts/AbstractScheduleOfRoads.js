
$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmSheduleView");

    $("#rdoMonthly").click(function () {
        $(".tdMonth").show();
        $(".tdYear").show();
        $(".tdFinYear").hide();
    });

    $("#rdoYearly").click(function () {
        $(".tdMonth").hide();
        $(".tdYear").hide();
        $(".tdFinYear").show();
    });

    $("#btnViewDetails").click(function () {

        if ($("#frmSheduleView").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/AccountReports/Account/AbstractScheduleRoadDetails/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmSheduleView").serialize(),
                success: function (data) {
                    $("#ScheduleDetails").html(data);
                    $(function () {
                        $("#spnhdSchedule").trigger('click');
                    });
                    $.unblockUI();
                },
                error: function (data) {
                    $.unblockUI();
                }
            });
        }
        //$.unblockUI();
    });

    $("#spnhdSchedule").click(function () {
        $("#spnhdSchedule").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvScheduleView").slideToggle("slow");
    });

    $("#ddlMonth").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

    $("#ddlYear").change(function () {

        UpdateAccountSession($("#ddlMonth").val(), $("#ddlYear").val());

    });

});
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/AccountReports/Account/UpdateAccountSession",
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