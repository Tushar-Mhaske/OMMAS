$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmMPMLACodesLayout'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMPMLACodesLayout").toggle("slow");

    });

    $("#btnView").click(function () {

        if ($("#ddlState option:selected").val() > 0) {
            $('#StateName').val($("#ddlState option:selected").text());
        }
        if ($("#ddlDistrict option:selected").val() > 0) {
            $('#DistrictName').val($("#ddlDistrict option:selected").text());
        }

        if ($('#frmMPMLACodesLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/MPMLAConCodesReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMPMLACodesLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvMPMLACodesReport").html(response);
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