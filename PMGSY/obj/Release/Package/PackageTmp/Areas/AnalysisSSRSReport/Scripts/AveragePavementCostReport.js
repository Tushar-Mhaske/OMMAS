$(document).ready(function () {
    $.validator.addMethod("comparevalidationwitfromyeartoyear", function (value, element, params) {

        if (parseInt($("#FromYearList_AveragePavCostDetail").val()) > parseInt($("#ToYearList_AveragePavCostDetail").val())) {
            return false;
        }
        else {
            return true;
        }
    });

    jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwitfromyeartoyear");

    $.validator.unobtrusive.parse($('#frmAveragePavCost'));
    $("#FromYearList_AveragePavCostDetail").change(function () {
        $("#ToYearList_AveragePavCostDetail").val($("#FromYearList_AveragePavCostDetail").val())
    });
    $("#StateList_AveragePavCostDetail").change(function () {
        loadDistrict($("#StateList_AveragePavCostDetail").val());

    });

    $("#DistrictList_AveragePavCostDetail").change(function () {
        loadBlock($("#StateList_AveragePavCostDetail").val(), $("#DistrictList_AveragePavCostDetail").val());

    });
    $("#btnViewAveragePavCostDetail").click(function () {

        if ($('#RoadWiseCheck_AveragePavCostDetail').prop("checked") == true) {
            if ($("#StateList_AveragePavCostDetail").is(":visible")) {
                if ($("#StateList_AveragePavCostDetail").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmAveragePavCost').valid()) {
            $("#loadAveragePavCost").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AveragePavCostDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AveragePavCostDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AveragePavCostDetail option:selected").text());
            if ($("#StateList_AveragePavCostDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AveragePavCostDetail option:selected").text());
            }

            if ($("#DistrictList_AveragePavCostDetail").is(":visible")) {

                //$('#DistrictList_AveragePavCostDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AveragePavCostDetail option:selected").text());
            }
            if ($("#BlockList_AveragePavCostDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AveragePavCostDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/AveragePavementCostReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAveragePavCost").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAveragePavCost").html(response);

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

    $("#btnViewAveragePavCostDetail").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewAveragePavCostDetail").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_AveragePavCostDetail").val(0);
    $("#DistrictList_AveragePavCostDetail").empty();
    $("#BlockList_AveragePavCostDetail").val(0);
    $("#BlockList_AveragePavCostDetail").empty();
    $("#BlockList_AveragePavCostDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AveragePavCostDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AveragePavCostDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AveragePavCostDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AveragePavCostDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AveragePavCostDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AveragePavCostDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AveragePavCostDetail").attr("disabled", "disabled");
                        $("#DistrictList_AveragePavCostDetail").trigger('change');
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

        $("#DistrictList_AveragePavCostDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AveragePavCostDetail").empty();
        $("#BlockList_AveragePavCostDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AveragePavCostDetail").val(0);
    $("#BlockList_AveragePavCostDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AveragePavCostDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AveragePavCostDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AveragePavCostDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AveragePavCostDetail").attr("disabled", "disabled");
                        //$("#BlockList_AveragePavCostDetail").trigger('change');
                    }
                    //$('#BlockList_AveragePavCostDetail').find("option[value='0']").remove();
                    //$("#BlockList_AveragePavCostDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AveragePavCostDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AveragePavCostDetail").append("<option value='0'>All Blocks</option>");
    }
}