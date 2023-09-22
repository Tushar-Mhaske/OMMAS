$(document).ready(function () {
    $.validator.addMethod("comparevalidationwitfromyeartoyear", function (value, element, params) {

        if (parseInt($("#FromYearList_ConstCostPatternDetail").val()) > parseInt($("#ToYearList_ConstCostPatternDetail").val())) {
            return false;
        }
        else {
            return true;
        }
    });

    jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwitfromyeartoyear");


    $.validator.unobtrusive.parse($('#frmConstCostPattern'));

    $("#FromYearList_ConstCostPatternDetail").change(function () {
       
        $("#ToYearList_ConstCostPatternDetail").val($("#FromYearList_ConstCostPatternDetail").val())
    });
    $("#StateList_ConstCostPatternDetail").change(function () {
        loadDistrict($("#StateList_ConstCostPatternDetail").val());

    });

    $("#DistrictList_ConstCostPatternDetail").change(function () {
        loadBlock($("#StateList_ConstCostPatternDetail").val(), $("#DistrictList_ConstCostPatternDetail").val());

    });
    $("#btnViewConstCostPatternDetail").click(function () {

        if ($('#RoadWiseCheck_ConstCostPatternDetail').prop("checked") == true) {
            if ($("#StateList_ConstCostPatternDetail").is(":visible")) {
                if ($("#StateList_ConstCostPatternDetail").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmConstCostPattern').valid()) {
            $("#loadConstCostPattern").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_ConstCostPatternDetail option:selected").text());
            $("#StatusName").val($("#StatusList_ConstCostPatternDetail option:selected").text());
            $("#BatchName").val($("#BatchList_ConstCostPatternDetail option:selected").text());
            if ($("#StateList_ConstCostPatternDetail").is(":visible")) {

                $("#StateName").val($("#StateList_ConstCostPatternDetail option:selected").text());
            }

            if ($("#DistrictList_ConstCostPatternDetail").is(":visible")) {

                //$('#DistrictList_ConstCostPatternDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_ConstCostPatternDetail option:selected").text());
            }
            if ($("#BlockList_ConstCostPatternDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_ConstCostPatternDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/ConstructionCostPatternReport/',
                type: 'POST',
                catche: false,
                data: $("#frmConstCostPattern").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadConstCostPattern").html(response);

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

    $("#btnViewConstCostPatternDetail").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewConstCostPatternDetail").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_ConstCostPatternDetail").val(0);
    $("#DistrictList_ConstCostPatternDetail").empty();
    $("#BlockList_ConstCostPatternDetail").val(0);
    $("#BlockList_ConstCostPatternDetail").empty();
    $("#BlockList_ConstCostPatternDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_ConstCostPatternDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_ConstCostPatternDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_ConstCostPatternDetail').find("option[value='0']").remove();
                    //$("#DistrictList_ConstCostPatternDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_ConstCostPatternDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_ConstCostPatternDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_ConstCostPatternDetail").attr("disabled", "disabled");
                        $("#DistrictList_ConstCostPatternDetail").trigger('change');
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

        $("#DistrictList_ConstCostPatternDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_ConstCostPatternDetail").empty();
        $("#BlockList_ConstCostPatternDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_ConstCostPatternDetail").val(0);
    $("#BlockList_ConstCostPatternDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_ConstCostPatternDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_ConstCostPatternDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_ConstCostPatternDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_ConstCostPatternDetail").attr("disabled", "disabled");
                        //$("#BlockList_ConstCostPatternDetail").trigger('change');
                    }
                    //$('#BlockList_ConstCostPatternDetail').find("option[value='0']").remove();
                    //$("#BlockList_ConstCostPatternDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_ConstCostPatternDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_ConstCostPatternDetail").append("<option value='0'>All Blocks</option>");
    }
}