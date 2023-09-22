$(document).ready(function () {
    
    $.validator.addMethod("comparevalidationwitfromyeartoyear", function (value, element, params) {

        if (parseInt($("#FromYearList_AnaSubGrdSoilDetail").val()) > parseInt($("#ToYearList_AnaSubGrdSoilDetail").val())) {
            return false;
        }
        else {
            return true;
        }
    });
  
    jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwitfromyeartoyear");
    $.validator.unobtrusive.parse($('#frmAnaSubGrdSoil'));

    $("#FromYearList_AnaSubGrdSoilDetail").change(function () {
        $("#ToYearList_AnaSubGrdSoilDetail").val($("#FromYearList_AnaSubGrdSoilDetail").val())
    });

    $("#StateList_AnaSubGrdSoilDetail").change(function () {
       
        loadDistrict($("#StateList_AnaSubGrdSoilDetail").val());

    });

    $("#DistrictList_AnaSubGrdSoilDetail").change(function () {
        loadBlock($("#StateList_AnaSubGrdSoilDetail").val(), $("#DistrictList_AnaSubGrdSoilDetail").val());

    });
    $("#btnViewAnaSubGrdSoilDetail").click(function () {
        //if ($('#RoadWiseCheck_AnaSubGrdSoilDetail').prop("checked") == true) {
        //    if ($("#StateList_AnaSubGrdSoilDetail").is(":visible")) {
        //        if ($("#StateList_AnaSubGrdSoilDetail").val() == 0) {
        //            alert("Please select State");
        //            return false;
        //        }
        //    }
        //}

        if ($('#frmAnaSubGrdSoil').valid()) {
            $("#loadAnaSubGrdSoil").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AnaSubGrdSoilDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AnaSubGrdSoilDetail option:selected").text());
            $("#CBRName").val($("#CBRList_AnaSubGrdSoilDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AnaSubGrdSoilDetail option:selected").text());
           
            if ($("#StateList_AnaSubGrdSoilDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AnaSubGrdSoilDetail option:selected").text());
            }

            if ($("#DistrictList_AnaSubGrdSoilDetail").is(":visible")) {

                //$('#DistrictList_AnaSubGrdSoilDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AnaSubGrdSoilDetail option:selected").text());
            }
            if ($("#BlockList_AnaSubGrdSoilDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AnaSubGrdSoilDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/AnalysisSubgradeSoilPatternReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAnaSubGrdSoil").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAnaSubGrdSoil").html(response);

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

    $("#btnViewAnaSubGrdSoilDetail").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewAnaSubGrdSoilDetail").trigger('click');
    //}
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {

   
    $("#DistrictList_AnaSubGrdSoilDetail").val(0);
    $("#DistrictList_AnaSubGrdSoilDetail").empty();
    $("#BlockList_AnaSubGrdSoilDetail").val(0);
    $("#BlockList_AnaSubGrdSoilDetail").empty();
    $("#BlockList_AnaSubGrdSoilDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AnaSubGrdSoilDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AnaSubGrdSoilDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AnaSubGrdSoilDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AnaSubGrdSoilDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AnaSubGrdSoilDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AnaSubGrdSoilDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AnaSubGrdSoilDetail").attr("disabled", "disabled");
                        $("#DistrictList_AnaSubGrdSoilDetail").trigger('change');
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

        $("#DistrictList_AnaSubGrdSoilDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AnaSubGrdSoilDetail").empty();
        $("#BlockList_AnaSubGrdSoilDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AnaSubGrdSoilDetail").val(0);
    $("#BlockList_AnaSubGrdSoilDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AnaSubGrdSoilDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AnaSubGrdSoilDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AnaSubGrdSoilDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AnaSubGrdSoilDetail").attr("disabled", "disabled");
                        //$("#BlockList_AnaSubGrdSoilDetail").trigger('change');
                    }
                    //$('#BlockList_AnaSubGrdSoilDetail').find("option[value='0']").remove();
                    //$("#BlockList_AnaSubGrdSoilDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AnaSubGrdSoilDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AnaSubGrdSoilDetail").append("<option value='0'>All Blocks</option>");
    }
}