$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMaintenanceIncentiveLenExp'));

    $("#btnView").click(function () {

        if ($('#frmMaintenanceIncentiveLenExp').valid()) {
            $("#loadReport").html("");

            if ($("#ddlState").val() > 0) {
                $("#Level").val(2);
                $("#LocationName").val($("#ddlState option:selected").text());
            }
            else {
                $("#Level").val(1);
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/MaintenanceIncentiveLengthExpenditureReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintenanceIncentiveLenExp").serialize(),
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