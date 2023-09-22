$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmBeneficiaryLayout'));

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

    $("#btnView").click(function () {

        if ($('#frmBeneficiaryLayout').valid()) {
            $("#divLoadBeneficiaryReport").html("");

            if ($("#ddlState").is(":visible")) {
                $("#StateName").val($("#ddlState option:selected").text());
            }
            if ($("#ddlDistrict").is(":visible")) {
                $("#DistrictName").val($("#ddlDistrict option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PFMSReports/PFMSReports/ReatBeneficiaryDetailsReport/',
                type: 'POST',
                catche: false,
                data: $("#frmBeneficiaryLayout").serialize(),
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


      ////Populate Agency
    $("#ddlState").change(function () {



        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Agency...</label></h4>' });

        $.ajax({
            url: '/PFMSReports/PFMSReports/PopulateAgency/' + $("#ddlState option:selected").val(),
            type: 'POST',
            catche: false,
            async: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while proccessing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlAgency").empty();
                $.each(response, function () {
                    $("#ddlAgency").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency






});