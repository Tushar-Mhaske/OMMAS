$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmAnaAvgLength'));
    $("#StateList_AnaAvgLengthDetail").change(function () {
        loadDistrict($("#StateList_AnaAvgLengthDetail").val());

    });

    $("#DistrictList_AnaAvgLengthDetail").change(function () {
        loadBlock($("#StateList_AnaAvgLengthDetail").val(), $("#DistrictList_AnaAvgLengthDetail").val());

    });
    $("#btnViewAnaAvgLengthDetail").click(function () {
        if ($('#RoadWiseCheck_AnaAvgLengthDetail').prop("checked") == true) {
            if ($("#StateList_AnaAvgLengthDetail").is(":visible")) {
                if ($("#StateList_AnaAvgLengthDetail").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }

        if ($('#frmAnaAvgLength').valid()) {
            $("#loadAnaAvgLength").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AnaAvgLengthDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AnaAvgLengthDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AnaAvgLengthDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_AnaAvgLengthDetail option:selected").text());

            if ($("#StateList_AnaAvgLengthDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AnaAvgLengthDetail option:selected").text());
            }

            if ($("#DistrictList_AnaAvgLengthDetail").is(":visible")) {

                //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AnaAvgLengthDetail option:selected").text());
            }
            if ($("#BlockList_AnaAvgLengthDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AnaAvgLengthDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/AnalysisAverageLengthReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAnaAvgLength").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAnaAvgLength").html(response);

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

    $("#btnViewAnaAvgLengthDetail").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewAnaAvgLengthDetail").trigger('click');
    //}
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_AnaAvgLengthDetail").val(0);
    $("#DistrictList_AnaAvgLengthDetail").empty();
    $("#BlockList_AnaAvgLengthDetail").val(0);
    $("#BlockList_AnaAvgLengthDetail").empty();
    $("#BlockList_AnaAvgLengthDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AnaAvgLengthDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AnaAvgLengthDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AnaAvgLengthDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AnaAvgLengthDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AnaAvgLengthDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AnaAvgLengthDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AnaAvgLengthDetail").attr("disabled", "disabled");
                        $("#DistrictList_AnaAvgLengthDetail").trigger('change');
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

        $("#DistrictList_AnaAvgLengthDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AnaAvgLengthDetail").empty();
        $("#BlockList_AnaAvgLengthDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AnaAvgLengthDetail").val(0);
    $("#BlockList_AnaAvgLengthDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AnaAvgLengthDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AnaAvgLengthDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AnaAvgLengthDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AnaAvgLengthDetail").attr("disabled", "disabled");
                        //$("#BlockList_AnaAvgLengthDetail").trigger('change');
                    }
                    //$('#BlockList_AnaAvgLengthDetail').find("option[value='0']").remove();
                    //$("#BlockList_AnaAvgLengthDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AnaAvgLengthDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AnaAvgLengthDetail").append("<option value='0'>All Blocks</option>");
    }
}