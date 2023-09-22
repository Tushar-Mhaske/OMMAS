
$(document).ready(function () {
    // var json = JSON.stringify($("#frmSearchQualityMonitor").serialize());
    // alert(json);
    LoadMonitorReport();
    $("#btnSearch").click(function () {
        LoadMonitorReport();
    });
});

function LoadMonitorReport() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    $.ajax({
        url: '/QMSSRSReports/QMSSRSReports/MonitorReport/',
        type: 'POST',
        catche: false,
        data: $("#frmSearchQualityMonitor").serialize(),
        async: false,
        success: function (response) {
            $("#dvQualityMonitorList").html(response);
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            alert("Error ocurred");
            return false;
        },
    });

}
