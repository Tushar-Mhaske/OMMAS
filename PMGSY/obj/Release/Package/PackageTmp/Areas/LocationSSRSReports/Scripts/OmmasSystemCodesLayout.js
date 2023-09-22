$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmOmmasSystemCodesLayout'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmOmmasSystemCodesLayout").toggle("slow");

    });

    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();
        //if ($('#frmOmmasSystemCodesLayout').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/LocationSSRSReports/LocationSSRSReports/DistrictDetails/',
            type: 'POST',
            catche: false,
            data: $("#frmOmmasSystemCodesLayout").serialize(),
            async: false,
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Error ocurred");
                return false;
            },
        });
        //}
        //else {
        //    alert($('#frmOmmasSystemCodesLayout').valid());
        //}
    });

    $("#btnView").click(function () {

        if ($("#ddlState option:selected").val() > 0) {
            $('#StateName').val($("#ddlState option:selected").text());
        }
        if ($("#ddlDistrict option:selected").val() > 0) {
            $('#DistrictName').val($("#ddlDistrict option:selected").text());
        }

        if ($('#frmOmmasSystemCodesLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/OmmasSystemCodesReport/',
                type: 'POST',
                catche: false,
                data: $("#frmOmmasSystemCodesLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadOmmasSystemCodesReport").html(response);
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