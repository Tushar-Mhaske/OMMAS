$(document).ready(function () {   

    $.validator.unobtrusive.parse($('#frmCommitmentReport'));
 
    $("#StateList_CommitmentDetails").change(function () {
        loadDistrict($("#StateList_CommitmentDetails").val());

    });

    $("#DistrictList_CommitmentDetails").change(function () {
        loadBlock($("#StateList_CommitmentDetails").val(), $("#DistrictList_CommitmentDetails").val());

    });
    $("#btnViewCommitmentDetails").click(function () {

        if ($('#RoadWiseCheck_CommitmentDetails').prop("checked") == true) {
            if ($("#StateList_CommitmentDetails").is(":visible")) {
                if ($("#StateList_CommitmentDetails").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmCommitmentReport').valid()) {
            $("#loadCommitmentReport").html("");

            $("#TypeName").val($("#TypeList_CommitmentDetailss option:selected").text());
            if ($("#StateList_CommitmentDetails").is(":visible")) {

                $("#StateName").val($("#StateList_CommitmentDetails option:selected").text());
            }

            if ($("#DistrictList_CommitmentDetails").is(":visible")) {

                //$('#DistrictList_CommitmentDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_CommitmentDetails option:selected").text());
            }
            if ($("#BlockList_CommitmentDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_CommitmentDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/CommitmentMaintenanceReport/',
                type: 'POST',
                catche: false,
                data: $("#frmCommitmentReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadCommitmentReport").html(response);

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

    $("#btnViewCommitmentDetails").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewCommitmentDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_CommitmentDetails").val(0);
    $("#DistrictList_CommitmentDetails").empty();
    $("#BlockList_CommitmentDetails").val(0);
    $("#BlockList_CommitmentDetails").empty();
    $("#BlockList_CommitmentDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_CommitmentDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_CommitmentDetails').find("option[value='0']").remove();
                    //$("#DistrictList_CommitmentDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_CommitmentDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_CommitmentDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_CommitmentDetails").attr("disabled", "disabled");
                        $("#DistrictList_CommitmentDetails").trigger('change');
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

        $("#DistrictList_CommitmentDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_CommitmentDetails").empty();
        $("#BlockList_CommitmentDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_CommitmentDetails").val(0);
    $("#BlockList_CommitmentDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_CommitmentDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_CommitmentDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_CommitmentDetails").attr("disabled", "disabled");
                        //$("#BlockList_CommitmentDetails").trigger('change');
                    }
                    //$('#BlockList_CommitmentDetails').find("option[value='0']").remove();
                    //$("#BlockList_CommitmentDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_CommitmentDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_CommitmentDetails").append("<option value='0'>All Blocks</option>");
    }
}