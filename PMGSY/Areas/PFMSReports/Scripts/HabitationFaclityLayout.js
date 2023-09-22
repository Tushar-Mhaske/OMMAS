$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmHabitationFaclityLayout'));

    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();
        $("#ddlDistrict").append("<option value='-1'>Select District</option>");

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 1; i < jsonData.length; i++) {
                    $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

    $("#ddlDistrict").change(function () {
        $("#ddlBlock").empty();
        $("#ddlBlock").append("<option value='-1'>Select Block</option>");

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 1; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

    });

    $("#btnView").click(function () {

        if ($('#frmHabitationFaclityLayout').valid()) {
            $("#divLoadBeneficiaryReport").html("");

            //if ($("#ddlState").is(":visible")) {
            //    $("#StateName").val($("#ddlState option:selected").text());
            //}
            //if ($("#ddlDistrict").is(":visible")) {
            //    $("#DistrictName").val($("#ddlDistrict option:selected").text());
            //}

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/HabitationFaclityReport/',
                type: 'POST',
                catche: false,
                data: $("#frmHabitationFaclityLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadBeneficiaryReport").html(response);
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