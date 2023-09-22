$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMaintenanceIncentive'));

    $("#btnView").click(function () {
        if ($('#frmMaintenanceIncentive').valid()) {
            $("#loadReport").html("");

            $("#StateName").val($("#ddlState option:selected").text());

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/MaintenanceIncentiveReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintenanceIncentive").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });
        }
    });
});
