﻿$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAbstractBankAuthDetails'));

    $(function () {
        if ($("#LevelID").val() == 4) {
            $("#ddlStateSRRDA").attr("disabled", true);
            $("#ddlStateSRRDA").val($("#AdminNDCode").val());
            $("#ddlStateSRRDA").trigger("change");
        } else {

        }
    });

    //populate DPIU
    $("#ddlStateSRRDA").change(function () {

        $.blockUI({ message: '<h4><label style="font-weight:normal">Loadding DPIU...</label></h4>' });

        //var id = $("#ddlStateSRRDA").val() + "$" + "A";

        $.ajax({
            url: "/AccountReports/Account/PopulateDPIU?id=" + $("#ddlStateSRRDA").val(),
            type: 'POST',
            catche: false,
            asysnc: false,
            error: function (xhr, staus, error) {
                $.unblockUI();
                alert("An error occured while processing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlDPIU").empty();
                $.each(response, function () {
                    $("#ddlDPIU").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });

                $.unblockUI();
            },

        });

    });//end of Populate DPIU

    $("#btnView").click(function () {

        if ($('#frmAbstractBankAuthDetails').valid()) {

            if ($("#LevelID").val() == 4) {
                $("#ddlStateSRRDA").attr("disabled", false);
            }

            $("#DisplayYear").val($("#ddlYear option:selected").text());
            $('#DisplayState').val($('#ddlStateSRRDA option:selected').text());
            $('#DisplayDPIU').val($('#ddlDPIU option:selected').text());

            $.ajax({
                url: '/AccountReports/Account/AbstractBankAuthReport/',
                type: 'POST',
                catche: false,
                data: $('#frmAbstractBankAuthDetails').serialize(),
                error: function (xhr, status, error) {
                    alert("An error occured while processing your request.");
                    return false;
                },
                success: function (response) {
                    $("#dvAbstractBankAuthDetails").html(response);

                    if ($("#LevelID").val() == 4) {
                        $("#ddlStateSRRDA").attr("disabled", true);
                    }
                }
            });
        }
    });
});