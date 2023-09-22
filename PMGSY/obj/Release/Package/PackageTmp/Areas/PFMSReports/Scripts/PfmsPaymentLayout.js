$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPfmsPaymentLayout'));

    $("#btnView").click(function () {

        if ($('#frmPfmsPaymentLayout').valid()) {
            $("#divLoadPfmsPaymentReport").html("");

            alert($('#rdoSRRDA').is(':checked'));

            if ($("#SRRDA_DPIU").val() == "S") {
                $("#AdminName").val($("#ddlSRRDA option:selected").text());
            }
            else {
                $("#AdminName").val($("#ddlDPIU option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PFMSReports/PFMSReports/PfmsPaymentReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPfmsPaymentLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadPfmsPaymentReport").html(response);
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