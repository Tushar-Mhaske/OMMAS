$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAllotmentDutiesOne'));
    $("#btnAllotment").click(function () {
        if ($('#frmAllotmentDutiesOne').valid()) {
            $("#loadReportOne").html("");
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/MaintenanceWorkAndLengthReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAllotmentDutiesOne").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReportOne").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });

    $("#btnAllotment").trigger('click');

});