$(document).ready(function () {

    
    $.validator.unobtrusive.parse($('#frmMaintInspection'));

   

    $("#StateList_MaintInspectionDetail").change(function () {

        loadDistrict($("#StateList_MaintInspectionDetail").val());

    });

    $("#DistrictList_MaintInspectionDetail").change(function () {
        loadBlock($("#StateList_MaintInspectionDetail").val(), $("#DistrictList_MaintInspectionDetail").val());

    });
    $("#btnViewMaintInspectionDetail").click(function () {       

        if ($('#frmMaintInspection').valid()) {
            $("#loadMaintInspection").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_MaintInspectionDetail option:selected").text());
            $("#TypeName").val($("#TypeList_MaintInspectionDetail option:selected").text());
            $("#BatchName").val($("#BatchList_MaintInspectionDetail option:selected").text());

            if ($("#StateList_MaintInspectionDetail").is(":visible")) {

                $("#StateName").val($("#StateList_MaintInspectionDetail option:selected").text());
            }

            if ($("#DistrictList_MaintInspectionDetail").is(":visible")) {

                //$('#DistrictList_MaintInspectionDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_MaintInspectionDetail option:selected").text());
            }
            if ($("#BlockList_MaintInspectionDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_MaintInspectionDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/MaintenanceInspectionReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMaintInspection").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadMaintInspection").html(response);

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

    //$("#btnViewMaintInspectionDetail").trigger('click');
    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewMaintInspectionDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {


    $("#DistrictList_MaintInspectionDetail").val(0);
    $("#DistrictList_MaintInspectionDetail").empty();
    $("#BlockList_MaintInspectionDetail").val(0);
    $("#BlockList_MaintInspectionDetail").empty();
    $("#BlockList_MaintInspectionDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_MaintInspectionDetail").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_MaintInspectionDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_MaintInspectionDetail').find("option[value='0']").remove();
                    //$("#DistrictList_MaintInspectionDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_MaintInspectionDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_MaintInspectionDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_MaintInspectionDetail").attr("disabled", "disabled");
                        $("#DistrictList_MaintInspectionDetail").trigger('change');
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

        $("#DistrictList_MaintInspectionDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_MaintInspectionDetail").empty();
        $("#BlockList_MaintInspectionDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_MaintInspectionDetail").val(0);
    $("#BlockList_MaintInspectionDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_MaintInspectionDetail").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_MaintInspectionDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_MaintInspectionDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_MaintInspectionDetail").attr("disabled", "disabled");
                        //$("#BlockList_MaintInspectionDetail").trigger('change');
                    }
                    //$('#BlockList_MaintInspectionDetail').find("option[value='0']").remove();
                    //$("#BlockList_MaintInspectionDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_MaintInspectionDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_MaintInspectionDetail").append("<option value='0'>All Blocks</option>");
    }
}