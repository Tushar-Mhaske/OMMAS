$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmContractorInspectionLayout'));

    $("#btnView").click(function () {

        if ($('#frmContractorInspectionLayout').valid()) {
            $("#divLoadReport").html("");

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/ContractorInspectionModReport/',
                type: 'POST',
                catche: false,
                data: $("#frmContractorInspectionLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });
});