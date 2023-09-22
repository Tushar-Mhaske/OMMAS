$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmContractorWorksNotInspectedLayout'));

    $("#btnView").click(function () {

        if ($('#frmContractorWorksNotInspectedLayout').valid()) {
            $("#dvLoadReport").html("");

            if ($("#ddlState").is(":visible")) {
                $("#StateName").val($("#ddlState option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/ContractorWorksNotInspectedReport/',
                type: 'POST',
                catche: false,
                data: $("#frmContractorWorksNotInspectedLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadReport").html(response);
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