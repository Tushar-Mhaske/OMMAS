$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmStateProfileLayout'));
    $("#ddlSPState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlSPAgency").empty();
        $("#ddlSPCollaboration").empty();
        $("#ddlSPDistrict").empty();


        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlSPState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlSPCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlSPCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            data: { stateCode: $("#ddlSPState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlSPAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlSPAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            data: { stateCode: $("#ddlSPState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlSPDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    $("#ddlSPDistrict").change(function () {
        $("#ddlSPBlock").empty();

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlSPDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlSPBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlSPBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

    $("#btnViewSP").click(function () {

        if ($('#frmStateProfileLayout').valid()) {
            $("#divStateProfileReport").html("");

            if ($("#ddlSPState").is(":visible")) {

                $("#StateName").val($("#ddlSPState option:selected").text());
            }

            if ($("#ddlSPDistrict").is(":visible")) {
                $("#DistName").val($("#ddlSPDistrict option:selected").text());
            }
            if ($("#ddlSPBlock").is(":visible")) {

                $("#BlockName").val($("#ddlSPBlock option:selected").text());
            }
            if ($("#ddlSPCollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlSPCollaboration option:selected").text());
            }
            if ($("#ddlSPAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlSPAgency option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ECBriefReport/ECBriefReport/StateProfileReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateProfileLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadStateProfileReport").html(response);

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
        $("#dvStateProfileParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
