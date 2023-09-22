$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmSystemCodesExcelLayout'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmSystemCodesExcelLayout").toggle("slow");

    });

    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();
        //if ($('#frmSystemCodesExcelLayout').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/LocationSSRSReports/LocationSSRSReports/DistrictDetails/',
            type: 'POST',
            catche: false,
            data: $("#frmSystemCodesExcelLayout").serialize(),
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
    });

    $("#btnView").click(function () {

        if ($("#ddlState option:selected").val() > 0) {
            $('#StateName').val($("#ddlState option:selected").text());
        }
        if ($("#ddlDistrict option:selected").val() > 0) {
            $('#DistrictName').val($("#ddlDistrict option:selected").text());
        }

        //if ($('#frmSystemCodesExcelLayout').valid())
        {

            //*************New of SSRS Report PDF as Created*************//
            window.open('/LocationSSRSReports/LocationSSRSReports/SystemCodesExcelReport?rpt=' + $('#ddlReport').val() + "$" + $("#ddlState option:selected").val()
                        + "$" + $("#ddlState option:selected").text() + "$" + $("#ddlDistrict option:selected").val() + "$" + $("#ddlDistrict option:selected").text(), '_blank');

            //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            //$.ajax({
            //    url: '/LocationSSRSReports/LocationSSRSReports/SystemCodesExcelReport/',
            //    type: 'POST',
            //    catche: false,
            //    data: $("#frmSystemCodesExcelLayout").serialize(),
            //    async: false,
            //    success: function (response) {
            //        $.unblockUI();
            //        $("#dvSystemCodesExcelReport").html(response);
            //    },
            //    error: function () {
            //        $.unblockUI();
            //        alert("Error ocurred");
            //        return false;
            //    },
            //});
        }
    });
});