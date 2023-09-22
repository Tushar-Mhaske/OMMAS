$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmRoadUpgradeMaintReport'));
    $("#StateList_RoadUpgradeMaintDetails").change(function () {
        loadDistrict($("#StateList_RoadUpgradeMaintDetails").val());

    });

    $("#DistrictList_RoadUpgradeMaintDetails").change(function () {
        loadBlock($("#StateList_RoadUpgradeMaintDetails").val(), $("#DistrictList_RoadUpgradeMaintDetails").val());

    });
    $("#btnViewRoadUpgradeMaintDetails").click(function () {

        //if ($('#RoadWiseCheck_RoadUpgradeMaintDetails').prop("checked") == true) {
        //    if ($("#StateList_RoadUpgradeMaintDetails").is(":visible")) {
        //        if ($("#StateList_RoadUpgradeMaintDetails").val() == 0) {
        //            alert("Please select State");
        //            return false;
        //        }
        //    }
            
        //}
        
        if ($('#frmRoadUpgradeMaintReport').valid()) {
            $("#loadRoadUpgradeMaintReport").html("");
           
            if ($("#StateList_RoadUpgradeMaintDetails").is(":visible")) {

                $("#StateName").val($("#StateList_RoadUpgradeMaintDetails option:selected").text());
            }

            if ($("#DistrictList_RoadUpgradeMaintDetails").is(":visible")) {

                //$('#DistrictList_RoadUpgradeMaintDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_RoadUpgradeMaintDetails option:selected").text());
            }
            if ($("#BlockList_RoadUpgradeMaintDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_RoadUpgradeMaintDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/RoadUpgradeMaintReport/',
                type: 'POST',
                catche: false,
                data: $("#frmRoadUpgradeMaintReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadRoadUpgradeMaintReport").html(response);

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

    //$("#btnViewRoadUpgradeMaintDetails").trigger('click');
    if ($('#Mast_Block_Code').val() > 0) {
        $("#btnViewRoadUpgradeMaintDetails").trigger('click');
    }
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewRoadUpgradeMaintDetails").trigger('click');
    //}
 

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_RoadUpgradeMaintDetails").val(0);
    $("#DistrictList_RoadUpgradeMaintDetails").empty();
    $("#BlockList_RoadUpgradeMaintDetails").val(0);
    $("#BlockList_RoadUpgradeMaintDetails").empty();
    $("#BlockList_RoadUpgradeMaintDetails").append("<option value='0'>Select Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_RoadUpgradeMaintDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_RoadUpgradeMaintDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_RoadUpgradeMaintDetails').find("option[value='0']").remove();
                    //$("#DistrictList_RoadUpgradeMaintDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_RoadUpgradeMaintDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_RoadUpgradeMaintDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_RoadUpgradeMaintDetails").attr("disabled", "disabled");
                        $("#DistrictList_RoadUpgradeMaintDetails").trigger('change');
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

        $("#DistrictList_RoadUpgradeMaintDetails").append("<option value='0'>Select Districts</option>");
        $("#BlockList_RoadUpgradeMaintDetails").empty();
        $("#BlockList_RoadUpgradeMaintDetails").append("<option value='0'>Select Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_RoadUpgradeMaintDetails").val(0);
    $("#BlockList_RoadUpgradeMaintDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_RoadUpgradeMaintDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_RoadUpgradeMaintDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_RoadUpgradeMaintDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_RoadUpgradeMaintDetails").attr("disabled", "disabled");
                        //$("#BlockList_RoadUpgradeMaintDetails").trigger('change');
                    }
                    //$('#BlockList_RoadUpgradeMaintDetails').find("option[value='0']").remove();
                    //$("#BlockList_RoadUpgradeMaintDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_RoadUpgradeMaintDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_RoadUpgradeMaintDetails").append("<option value='0'>Select Blocks</option>");
    }
}