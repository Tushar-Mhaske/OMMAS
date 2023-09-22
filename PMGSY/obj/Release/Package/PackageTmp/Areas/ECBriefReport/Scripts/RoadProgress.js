$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFAPhaseProfileLayout'));
    $("#ddlFAState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlFAAgency").empty();
        $("#ddlFACollaboration").empty();
        $("#ddlFADistrict").empty();


        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlFAState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlFACollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlFACollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            url: '/ECBriefReport/ECBriefReport/PopulateAgenciesForRoadProgress',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlFAState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlFAAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlFAAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            data: { stateCode: $("#ddlFAState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlFADistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    $("#ddlFADistrict").change(function () {
        $("#ddlFABlock").empty();

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlFADistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlFABlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlFABlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

    $("#btnViewFA").click(function () {

        if ($('#frmFAPhaseProfileLayout').valid()) {
            $("#divFAPhaseProfileReport").html("");

            if ($("#ddlFAState").is(":visible")) {

                $("#StateName").val($("#ddlFAState option:selected").text());
            }

            if ($("#ddlFADistrict").is(":visible")) {
                $("#DistName").val($("#ddlFADistrict option:selected").text());
            }
            if ($("#ddlFABlock").is(":visible")) {

                $("#BlockName").val($("#ddlFABlock option:selected").text());
            }
            if ($("#ddlFACollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlFACollaboration option:selected").text());
            }
            if ($("#ddlFAAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlFAAgency option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ECBriefReport/ECBriefReport/RoadProgressReport/',
                type: 'POST',
                catche: false,
                data: $("#frmFAPhaseProfileLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divFAPhaseProfileReport").html(response);

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
        $("#dvFAPhaseProfileParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
