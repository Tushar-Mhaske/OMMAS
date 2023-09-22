$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmHabitationScoreReport'));

    $("#StateList_HabitationScoreDetails").change(function () {
        loadDistrict($("#StateList_HabitationScoreDetails").val());

    });

    $("#DistrictList_HabitationScoreDetails").change(function () {
        loadBlock($("#StateList_HabitationScoreDetails").val(), $("#DistrictList_HabitationScoreDetails").val());

    });
    $("#btnViewHabitationScoreDetails").click(function () {

        if ($('#RoadWiseCheck_HabitationScoreDetails').prop("checked") == true) {
            if ($("#StateList_HabitationScoreDetails").is(":visible")) {
                if ($("#StateList_HabitationScoreDetails").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmHabitationScoreReport').valid()) {
            $("#loadHabitationScoreReport").html("");

            $("#TypeName").val($("#TypeList_HabitationScoreDetailss option:selected").text());
            if ($("#StateList_HabitationScoreDetails").is(":visible")) {

                $("#StateName").val($("#StateList_HabitationScoreDetails option:selected").text());
            }

            if ($("#DistrictList_HabitationScoreDetails").is(":visible")) {

                //$('#DistrictList_HabitationScoreDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_HabitationScoreDetails option:selected").text());
            }
            if ($("#BlockList_HabitationScoreDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_HabitationScoreDetails option:selected").text());
            }        
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/HabitationScoreReport/',
                type: 'POST',
                catche: false,
                data: $("#frmHabitationScoreReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadHabitationScoreReport").html(response);

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

    //$("#btnViewHabitationScoreDetails").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewHabitationScoreDetails").trigger('click');
    //}
    $("#btnViewHabitationScoreDetails").trigger('click');
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });


});

function loadDistrict(statCode) {
    $("#DistrictList_HabitationScoreDetails").val(0);
    $("#DistrictList_HabitationScoreDetails").empty();
    $("#BlockList_HabitationScoreDetails").val(0);
    $("#BlockList_HabitationScoreDetails").empty();
    $("#BlockList_HabitationScoreDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_HabitationScoreDetails").length > 0) {
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_HabitationScoreDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_HabitationScoreDetails').find("option[value='0']").remove();
                    //$("#DistrictList_HabitationScoreDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_HabitationScoreDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_HabitationScoreDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_HabitationScoreDetails").attr("disabled", "disabled");
                        $("#DistrictList_HabitationScoreDetails").trigger('change');
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

        $("#DistrictList_HabitationScoreDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_HabitationScoreDetails").empty();
        $("#BlockList_HabitationScoreDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_HabitationScoreDetails").val(0);
    $("#BlockList_HabitationScoreDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_HabitationScoreDetails").length > 0) {
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_HabitationScoreDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_HabitationScoreDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_HabitationScoreDetails").attr("disabled", "disabled");
                        //$("#BlockList_HabitationScoreDetails").trigger('change');
                    }
                    //$('#BlockList_HabitationScoreDetails').find("option[value='0']").remove();
                    //$("#BlockList_HabitationScoreDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_HabitationScoreDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_HabitationScoreDetails").append("<option value='0'>All Blocks</option>");
    }
}