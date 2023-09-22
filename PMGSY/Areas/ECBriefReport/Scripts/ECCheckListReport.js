$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmECBriefLayout'));
    $("#ddlECState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlECAgency").empty();
        $("#ddlECCollaboration").empty();
        $("#ddlECDistrict").empty();


        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateCollaborations',
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
                //alert("error " + err);
                $.unblockUI();
            }
        });




        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateAgenciesChkList',
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
                //alert("error " + err);
                $.unblockUI();
            }
        });

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
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
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    $("#ddlECDistrict").change(function () {
        $("#ddlECBlock").empty();

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
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
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

    $("#btnViewEC").click(function () {

        if ($('#frmECCheckListLayout').valid()) {
            $("#divLoadECCheckListReport").html("");

            if ($("#ddlECState").is(":visible")) {

                $("#StateName").val($("#ddlECState option:selected").text());
            }

            if ($("#ddlECDistrict").is(":visible")) {
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
                url: '/ECBriefReport/ECBriefReport/ECCheckListReport/',
                type: 'POST',
                catche: false,
                data: $("#frmECCheckListLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadECCheckListReport").html(response);

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

    //$("#btnViewEC").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvECCheckListParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
