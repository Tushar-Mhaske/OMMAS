$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmTechnologyLayout'));

    hideShowTech();
    $("#btnViewTech").trigger('click');
    

    if ($("#Mast_State_Code").val() > 0) {

        $("#ddlTechState").attr("disabled", "disabled");
    }

    if ($("#Mast_District_Code").val() > 0) {

        $("#ddlTechDistrict").attr("disabled", "disabled");
    }

    if ($("#Mast_Block_Code").val() > 0) {

        $("#ddlTechBlock").attr("disabled", "disabled");
    }

    $("#ddlTechState").change(function () {
        
        //loadDistrict($("#ddlTechState").val());
        //$("#ddlECAgency").empty();
        $("#ddlTechCollaboration").empty();
        $("#ddlTechDistrict").empty();
        $("#ddlTechBlock").empty();
        $("#ddlTechBlock").append("<option value='0'>All Blocks</option>");
        
        $.ajax({
            url: '/ProposalSSRSReports/ProposalSSRSReports/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlTechState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlTechCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlTechCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictDetails',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { StateCode: $("#ddlTechState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlTechDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    $("#ddlTechDistrict").change(function () {
        $("#ddlTechBlock").empty();

        $.ajax({
            url: '/ProposalSSRSReports/ProposalSSRSReports/BlockDetails',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlTechDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlTechBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlTechBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $("#ddlTechReportType").change(function () {
        hideShowTech();
    });

    $("#btnViewTech").click(function () {

        if ($('#frmTechnologyLayout').valid()) {

            $("#spCollapseIconCN").trigger('click');

            $("#dvLoadTechnologyReport").html("");

            if ($("#ddlTechState").is(":visible")) {
                //alert("1");
                $("#StateName").val($("#ddlTechState option:selected").text());
            }

            if ($("#ddlTechDistrict").is(":visible")) {

                //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
                $("#DistName").val($("#ddlTechDistrict option:selected").text());
            }
            if ($("#ddlTechBlock").is(":visible")) {

                $("#BlockName").val($("#ddlTechBlock option:selected").text());
            }
            if ($("#ddlTechCollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlTechCollaboration option:selected").text());
            }
            if ($("#ddlTechAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlTechAgency option:selected").text());
            }
            if ($("#ddlTechBatch").is(":visible")) {

                $("#BatchName").val($("#ddlTechBatch option:selected").text());
            }

            if ($("#ddlTechnology").is(":visible")) {

                $("#TechName").val($("#ddlTechnology option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/TechnologyReport/',
                type: 'POST',
                catche: false,
                data: $("#frmTechnologyLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadTechnologyReport").html(response);

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
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewAnaAvgLengthDetail").trigger('click');
    //}
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmTechnologyLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function hideShowTech()
{
    if ($('#ddlTechReportType option:selected').val() == 'T') {
        $('#td1').show();
        $('#td2').show();
        $('#td3').show();
    }
    else {
        $('#td1').hide();
        $('#td2').hide();
        $('#td3').hide();
    }
}