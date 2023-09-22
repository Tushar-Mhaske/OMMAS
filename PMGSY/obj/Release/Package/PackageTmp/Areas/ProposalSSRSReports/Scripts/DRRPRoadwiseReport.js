$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDRRPRoadwise'));

    $("#btnViewPhysicalProgress").click(function () {

        if ($("#ddlStates").is(':visible')) {
            $("#StateName").val($("#ddlStates option:selected").text());
        }
        if ($("#ddlDistricts").is(':visible')) {
            $("#DistrictName").val($("#ddlDistricts option:selected").text());
        }
        $("#BlockName").val($("#ddlBlocks option:selected").text());
        $("#CategoryName").val($("#ddlRoadCategory option:selected").text());
        $("#SoilTypeName").val($("#ddlSoilTypes option:selected").text());
        $("#TerrainTypeName").val($("#ddlTerrainTypes option:selected").text());
        
        if ($("#frmDRRPRoadwise").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DRRPRoadwiseReport/',
                type: 'POST',
                catche: false,
                data: $("#frmDRRPRoadwise").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadDRRPRoadwise").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error");
                    return false;
                },
            });

        }
        else {
            return false;
        }

    });

    closableNoteDiv("divCommonReport", "spnCommonReport");

    $("#ddlStates").change(function () {
        loadDistrict($("#ddlStates option:selected").val());
    });

    $("#ddlDistricts").change(function () {
        loadBlock($("#ddlStates option:selected").val(), $("#ddlDistricts option:selected").val());
    });


});
function loadDistrict(statCode) {
    $("#ddlDistricts").val(0);
    $("#ddlDistricts").empty();
    $("#ddlBlocks").val(0);
    $("#ddlBlocks").empty();
    $("#ddlBlocks").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#ddlDistricts").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistricts").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //For Disable if District Login
                    if ($("#DistrictCode").val() > 0) {
                        $("#ddlDistricts").val($("#DistrictCode").val());
                        $("#ddlDistricts").trigger('change');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlDistricts").append("<option value='0'>All Districts</option>");
        $("#ddlBlocks").empty();
        $("#ddlBlocks").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#ddlBlocks").val(0);
    $("#ddlBlocks").empty();

    if (districtCode > 0) {
        if ($("#ddlBlocks").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBlocks").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#BlockCode").val() > 0) {
                        $("#ddlBlocks").val($("#BlockCode").val());
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#ddlBlocks").append("<option value='0'>All Blocks</option>");
    }
}