$(document).ready(function () {


    $.validator.unobtrusive.parse($('#frmEstiMatedMaint'));



    $("#StateList_EstiMatedMaintDetail").change(function () {

        loadDistrict($("#StateList_EstiMatedMaintDetail").val());

    });

    $("#DistrictList_EstiMatedMaintDetail").change(function () {
        loadBlock($("#StateList_EstiMatedMaintDetail").val(), $("#DistrictList_EstiMatedMaintDetail").val());

    });
    $("#btnViewEstiMatedMaintDetail").click(function () {

        if ($('#frmEstiMatedMaint').valid()) {
            $("#loadEstiMatedMaint").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_EstiMatedMaintDetail option:selected").text());
            $("#BatchName").val($("#BatchList_EstiMatedMaintDetail option:selected").text());

            if ($("#StateList_EstiMatedMaintDetail").is(":visible")) {

                $("#StateName").val($("#StateList_EstiMatedMaintDetail option:selected").text());
            }

            if ($("#DistrictList_EstiMatedMaintDetail").is(":visible")) {

                //$('#DistrictList_EstiMatedMaintDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_EstiMatedMaintDetail option:selected").text());
            }
            if ($("#BlockList_EstiMatedMaintDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_EstiMatedMaintDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/EstimatedMaintenanceReport/',
                type: 'POST',
                catche: false,
                data: $("#frmEstiMatedMaint").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadEstiMatedMaint").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });

    //$("#btnViewEstiMatedMaintDetail").trigger('click');
    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewEstiMatedMaintDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {


    $("#DistrictList_EstiMatedMaintDetail").val(0);
    $("#DistrictList_EstiMatedMaintDetail").empty();
    $("#BlockList_EstiMatedMaintDetail").val(0);
    $("#BlockList_EstiMatedMaintDetail").empty();
    $("#BlockList_EstiMatedMaintDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_EstiMatedMaintDetail").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_EstiMatedMaintDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_EstiMatedMaintDetail').find("option[value='0']").remove();
                    //$("#DistrictList_EstiMatedMaintDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_EstiMatedMaintDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_EstiMatedMaintDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_EstiMatedMaintDetail").attr("disabled", "disabled");
                        $("#DistrictList_EstiMatedMaintDetail").trigger('change');
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

        $("#DistrictList_EstiMatedMaintDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_EstiMatedMaintDetail").empty();
        $("#BlockList_EstiMatedMaintDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_EstiMatedMaintDetail").val(0);
    $("#BlockList_EstiMatedMaintDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_EstiMatedMaintDetail").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_EstiMatedMaintDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_EstiMatedMaintDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_EstiMatedMaintDetail").attr("disabled", "disabled");
                        //$("#BlockList_EstiMatedMaintDetail").trigger('change');
                    }
                    //$('#BlockList_EstiMatedMaintDetail').find("option[value='0']").remove();
                    //$("#BlockList_EstiMatedMaintDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_EstiMatedMaintDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_EstiMatedMaintDetail").append("<option value='0'>All Blocks</option>");
    }
}