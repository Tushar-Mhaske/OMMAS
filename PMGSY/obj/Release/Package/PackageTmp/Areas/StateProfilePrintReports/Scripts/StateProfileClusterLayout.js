$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmECBriefLayout'));
    $("#ddlECState").change(function () {

        $("#ddlECAgency").empty();
        $("#ddlECCollaboration").empty();
        $("#ddlECDistrict").empty();
        //$("#ddlECBlock").empty();
        // $("#ddlECBlock").append("<option value='0'>All Blocks</option>");

        $.ajax({
            url: '/StateProfilePrintReports/StateProfilePrint/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlECCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlECCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {

                $.unblockUI();
            }
        });




        $.ajax({
            url: '/StateProfilePrintReports/StateProfilePrint/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlECAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlECAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {

                $.unblockUI();
            }
        });

        $.ajax({
            url: '/StateProfilePrintReports/StateProfilePrint/PopulateDistricts',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlECDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {

                $.unblockUI();
            }
        });


    });

    $("#ddlECDistrict").change(function () {
        $("#ddlECBlock").empty();

        $.ajax({
            url: '/StateProfilePrintReports/StateProfilePrint/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlECDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlECBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlECBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {

                $.unblockUI();
            }
        });
    });

    $("#btnViewEC").click(function () {


        if ($('#frmECBriefLayout').valid()) {
            $("#divLoadECReport").html("");



            if ($("#ddlECState").is(":visible")) {
                //alert("1");
                $("#StateName").val($("#ddlECState option:selected").text());
            }

            if ($("#ddlECDistrict").is(":visible")) {

                //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
                $("#DistName").val($("#ddlECDistrict option:selected").text());
            }
            if ($("#ddlECBlock").is(":visible")) {

                $("#BlockName").val($("#ddlECBlock option:selected").text());
            }
            if ($("#ddlECCollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlECCollaboration option:selected").text());
            }
            if ($("#ddlECAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlECAgency option:selected").text());
            }
            if ($("#ddlECBatch").is(":visible")) {

                $("#BatchName").val($("#ddlECBatch option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/StateProfilePrintReports/StateProfilePrint/StateProfileClusterReport/',
                type: 'POST',
                catche: false,
                data: $("#frmECBriefLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadECReport").html(response);

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


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvECBriefParameter").toggle("slow");

    });


    closableNoteDiv("divCommonReport", "spnCommonReport");
});

