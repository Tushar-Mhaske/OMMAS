$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmMonitorScheduleLayout');

    $("#btnMonitorSchedule").click(function () {
        $('#StateName').val($('#ddlStateMonitorSchedule option:selected').text());
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/MonitorScheduleReport/',
            type: 'POST',
            catche: false,
            data: $("#frmMonitorScheduleLayout").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#dvLoadMonitorScheduleReport").html(response);
            },
            error: function () {
                $.unblockUI();
                alert("Error ocurred");
                return false;
            },
        });
    });

    $("#btnMonitorSchedule").trigger('click');
    //$("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s");

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMonitorScheduleLayout").toggle("slow");

    });
})