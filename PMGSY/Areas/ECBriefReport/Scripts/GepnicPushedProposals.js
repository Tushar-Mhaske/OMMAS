$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmPMGSYAchLayout'));
    $("#ddlPMGSYAchState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlPMGSYAchAgency").empty();
        $("#ddlPMGSYAchCollaboration").empty();
        $("#ddlPMGSYAchDistrict").empty();
        $("#ddlPMGSYAchBlock").empty();
        $("#ddlPMGSYAchBlock").append("<option value='0'>All Blocks</option>");

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlPMGSYAchState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlPMGSYAchCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlPMGSYAchCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            url: '/ECBriefReport/ECBriefReport/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlPMGSYAchState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlPMGSYAchAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlPMGSYAchAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            data: { stateCode: $("#ddlPMGSYAchState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlPMGSYAchDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    $("#ddlPMGSYAchDistrict").change(function () {
        $("#ddlPMGSYAchBlock").empty();

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlPMGSYAchDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlPMGSYAchBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlPMGSYAchBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

    $("#btnViewPMGSYAch").click(function () {
        
        if ($('#frmPMGSYAchLayout').valid()) {
            $("#divLoadPMGSYAchReport").html("");

            //$("#StateName").val($("#ddlECState option:selected").text());
            //$("#State_Name").val($("#ddlECState option:selected").text());
            //$("#StatusName").val($("#StatusList_AnaAvgLengthDetail option:selected").text());
            //$("#BatchName").val($("#BatchList_AnaAvgLengthDetail option:selected").text());
            //$("#YearName").val($("#PhaseYearList_AnaAvgLengthDetail option:selected").text());

            if ($("#ddlPMGSYAchState").is(":visible")) {
                //alert("1");
                $("#StateName").val($("#ddlPMGSYAchState option:selected").text());
            }

            if ($("#ddlPMGSYAchDistrict").is(":visible")) {

                //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
                $("#DistName").val($("#ddlPMGSYAchDistrict option:selected").text());
            }
            if ($("#ddlPMGSYAchBlock").is(":visible")) {

                $("#BlockName").val($("#ddlPMGSYAchBlock option:selected").text());
            }
            if ($("#ddlPMGSYAchCollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlPMGSYAchCollaboration option:selected").text());
            }
            if ($("#ddlPMGSYAchAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlPMGSYAchAgency option:selected").text());
            }
            if ($("#ddlPMGSYAchBatch").is(":visible")) {

                $("#BatchName").val($("#ddlPMGSYAchBatch option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ECBriefReport/ECBriefReport/GepnicPushedReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPMGSYAchLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadPMGSYAchReport").html(response);

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

    $("#btnViewPMGSYAch").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmPMGSYAchLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
