$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmRoutineMaintReport'));

    $("#StateList_RoutineMaintDetails").change(function () {
        loadDistrict($("#StateList_RoutineMaintDetails").val());

    });

    $("#DistrictList_RoutineMaintDetails").change(function () {
        loadBlock($("#StateList_RoutineMaintDetails").val(), $("#DistrictList_RoutineMaintDetails").val());

    });
    $("#btnViewRoutineMaintDetails").click(function () {

        if ($('#RoadWiseCheck_RoutineMaintDetails').prop("checked") == true) {
            if ($("#StateList_RoutineMaintDetails").is(":visible")) {
                if ($("#StateList_RoutineMaintDetails").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmRoutineMaintReport').valid()) {
            $("#loadRoutineMaintReport").html("");

            $("#TypeName").val($("#TypeList_RoutineMaintDetailss option:selected").text());
            if ($("#StateList_RoutineMaintDetails").is(":visible")) {

                $("#StateName").val($("#StateList_RoutineMaintDetails option:selected").text());
            }

            if ($("#DistrictList_RoutineMaintDetails").is(":visible")) {

                //$('#DistrictList_RoutineMaintDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_RoutineMaintDetails option:selected").text());
            }
            if ($("#BlockList_RoutineMaintDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_RoutineMaintDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/RoutineMaintPriorityReport/',
                type: 'POST',
                catche: false,
                data: $("#frmRoutineMaintReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadRoutineMaintReport").html(response);

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

    //$("#btnViewRoutineMaintDetails").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewRoutineMaintDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_RoutineMaintDetails").val(0);
    $("#DistrictList_RoutineMaintDetails").empty();
    $("#BlockList_RoutineMaintDetails").val(0);
    $("#BlockList_RoutineMaintDetails").empty();
    $("#BlockList_RoutineMaintDetails").append("<option value='0'>Select Block</option>");

    if (statCode > 0) {
        if ($("#DistrictList_RoutineMaintDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_RoutineMaintDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_RoutineMaintDetails').find("option[value='0']").remove();
                    //$("#DistrictList_RoutineMaintDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_RoutineMaintDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_RoutineMaintDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_RoutineMaintDetails").attr("disabled", "disabled");
                        $("#DistrictList_RoutineMaintDetails").trigger('change');
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

        $("#DistrictList_RoutineMaintDetails").append("<option value='0'>Select District</option>");
        $("#BlockList_RoutineMaintDetails").empty();
        $("#BlockList_RoutineMaintDetails").append("<option value='0'>Select Block</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_RoutineMaintDetails").val(0);
    $("#BlockList_RoutineMaintDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_RoutineMaintDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_RoutineMaintDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_RoutineMaintDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_RoutineMaintDetails").attr("disabled", "disabled");
                        //$("#BlockList_RoutineMaintDetails").trigger('change');
                    }
                    //$('#BlockList_RoutineMaintDetails').find("option[value='0']").remove();
                    //$("#BlockList_RoutineMaintDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_RoutineMaintDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_RoutineMaintDetails").append("<option value='0'>Select Block</option>");
    }
}