$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmWronglyAwardedLayout'));

    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlState").val(), value: Math.random() },
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
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });


    $("#btnView").click(function () {

        if ($('#frmWronglyAwardedLayout').valid()) {
            $("#dvloadReport").html("");

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/WronglyAwardedWorksReport/',
                type: 'POST',
                catche: false,
                data: $("#frmWronglyAwardedLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvloadReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });

        }
        else {

        }
    });

});