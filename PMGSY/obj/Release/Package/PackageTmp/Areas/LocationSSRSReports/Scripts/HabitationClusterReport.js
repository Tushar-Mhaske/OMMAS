$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmHabitationClusterReport'));

    $("#StateList_HabitationClusterDetails").change(function () {
        loadDistrict($("#StateList_HabitationClusterDetails").val());

    });

    $("#DistrictList_HabitationClusterDetails").change(function () {
        loadBlock($("#StateList_HabitationClusterDetails").val(), $("#DistrictList_HabitationClusterDetails").val());

    });
    $("#btnViewHabitationClusterDetails").click(function () {

        if ($('#RoadWiseCheck_HabitationClusterDetails').prop("checked") == true) {
            if ($("#StateList_HabitationClusterDetails").is(":visible")) {
                if ($("#StateList_HabitationClusterDetails").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmHabitationClusterReport').valid()) {
            $("#loadHabitationClusterReport").html("");

            $("#TypeName").val($("#TypeList_HabitationClusterDetailss option:selected").text());
            if ($("#StateList_HabitationClusterDetails").is(":visible")) {

                $("#StateName").val($("#StateList_HabitationClusterDetails option:selected").text());
            }

            if ($("#DistrictList_HabitationClusterDetails").is(":visible")) {

                //$('#DistrictList_HabitationClusterDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_HabitationClusterDetails option:selected").text());
            }
            if ($("#BlockList_HabitationClusterDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_HabitationClusterDetails option:selected").text());
            }
            $("#StatusName").val($("#StatusList_HabitationClusterDetails option:selected").text());

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/HabitationClusterReport/',
                type: 'POST',
                catche: false,
                data: $("#frmHabitationClusterReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadHabitationClusterReport").html(response);

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

    //$("#btnViewHabitationClusterDetails").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewHabitationClusterDetails").trigger('click');
    //}
    $("#btnViewHabitationClusterDetails").trigger('click');
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

  
});

function loadDistrict(statCode) {
    $("#DistrictList_HabitationClusterDetails").val(0);
    $("#DistrictList_HabitationClusterDetails").empty();
    $("#BlockList_HabitationClusterDetails").val(0);
    $("#BlockList_HabitationClusterDetails").empty();
    $("#BlockList_HabitationClusterDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_HabitationClusterDetails").length > 0) {
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_HabitationClusterDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_HabitationClusterDetails').find("option[value='0']").remove();
                    //$("#DistrictList_HabitationClusterDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_HabitationClusterDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_HabitationClusterDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_HabitationClusterDetails").attr("disabled", "disabled");
                        $("#DistrictList_HabitationClusterDetails").trigger('change');
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

        $("#DistrictList_HabitationClusterDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_HabitationClusterDetails").empty();
        $("#BlockList_HabitationClusterDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_HabitationClusterDetails").val(0);
    $("#BlockList_HabitationClusterDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_HabitationClusterDetails").length > 0) {
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_HabitationClusterDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_HabitationClusterDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_HabitationClusterDetails").attr("disabled", "disabled");
                        //$("#BlockList_HabitationClusterDetails").trigger('change');
                    }
                    //$('#BlockList_HabitationClusterDetails').find("option[value='0']").remove();
                    //$("#BlockList_HabitationClusterDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_HabitationClusterDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_HabitationClusterDetails").append("<option value='0'>All Blocks</option>");
    }
}