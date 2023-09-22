$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmAnaProposal'));
    $("#StateList_AnaProposalDetail").change(function () {
        loadDistrict($("#StateList_AnaProposalDetail").val());

    });

    $("#DistrictList_AnaProposalDetail").change(function () {
        loadBlock($("#StateList_AnaProposalDetail").val(), $("#DistrictList_AnaProposalDetail").val());

    });
    $("#btnViewAnaProposalDetail").click(function () {


        if ($('#frmAnaProposal').valid()) {
            $("#loadAnaProposal").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AnaProposalDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AnaProposalDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_AnaProposalDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AnaProposalDetail option:selected").text());
            if ($("#StateList_AnaProposalDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AnaProposalDetail option:selected").text());
            }

            if ($("#DistrictList_AnaProposalDetail").is(":visible")) {

                //$('#DistrictList_AnaProposalDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AnaProposalDetail option:selected").text());
            }
            if ($("#BlockList_AnaProposalDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AnaProposalDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/AnalysisProposalReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAnaProposal").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAnaProposal").html(response);

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

   
    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewAnaProposalDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_AnaProposalDetail").val(0);
    $("#DistrictList_AnaProposalDetail").empty();
    $("#BlockList_AnaProposalDetail").val(0);
    $("#BlockList_AnaProposalDetail").empty();
    $("#BlockList_AnaProposalDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AnaProposalDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AnaProposalDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AnaProposalDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AnaProposalDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AnaProposalDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AnaProposalDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AnaProposalDetail").attr("disabled", "disabled");
                        $("#DistrictList_AnaProposalDetail").trigger('change');
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

        $("#DistrictList_AnaProposalDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AnaProposalDetail").empty();
        $("#BlockList_AnaProposalDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AnaProposalDetail").val(0);
    $("#BlockList_AnaProposalDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AnaProposalDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AnaProposalDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AnaProposalDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AnaProposalDetail").attr("disabled", "disabled");
                        //$("#BlockList_AnaProposalDetail").trigger('change');
                    }
                    //$('#BlockList_AnaProposalDetail').find("option[value='0']").remove();
                    //$("#BlockList_AnaProposalDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AnaProposalDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AnaProposalDetail").append("<option value='0'>All Blocks</option>");
    }
}