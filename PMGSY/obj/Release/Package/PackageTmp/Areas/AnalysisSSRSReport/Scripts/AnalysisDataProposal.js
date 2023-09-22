$(document).ready(function () {
    $.validator.addMethod("comparevalidationwitfromyeartoyear", function (value, element, params) {

        if (parseInt($("#FromYearList_AnaDataProposalDetail").val()) > parseInt($("#ToYearList_AnaDataProposalDetail").val())) {
            return false;
        }
        else {
            return true;
        }
    });

    jQuery.validator.unobtrusive.adapters.addBool("comparevalidationwitfromyeartoyear");

    $.validator.unobtrusive.parse($('#frmAnaDataProposal'));
    $("#FromYearList_AnaDataProposalDetail").change(function () {
        $("#ToYearList_AnaDataProposalDetail").val($("#FromYearList_AnaDataProposalDetail").val())
    });
    $("#StateList_AnaDataProposalDetail").change(function () {
        loadDistrict($("#StateList_AnaDataProposalDetail").val());

    });

    $("#DistrictList_AnaDataProposalDetail").change(function () {
        loadBlock($("#StateList_AnaDataProposalDetail").val(), $("#DistrictList_AnaDataProposalDetail").val());

    });
    $("#btnViewAnaDataProposalDetail").click(function () {

       // alert("attribut "+$('#RoadWiseCheck_AnaDataProposalDetail').attr("checked"));
        //alert("propoperty"+$('#RoadWiseCheck_AnaDataProposalDetail').prop("checked"));

        if ($('#RoadWiseCheck_AnaDataProposalDetail').prop("checked")==true)
        {
            if ($("#StateList_AnaDataProposalDetail").is(":visible")) {
                if ($("#StateList_AnaDataProposalDetail").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }

        if ($('#frmAnaDataProposal').valid()) {
            
            $("#loadAnaDataProposal").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AnaDataProposalDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AnaDataProposalDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AnaDataProposalDetail option:selected").text());
            if ($("#StateList_AnaDataProposalDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AnaDataProposalDetail option:selected").text());
            }

            if ($("#DistrictList_AnaDataProposalDetail").is(":visible")) {

                //$('#DistrictList_AnaDataProposalDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AnaDataProposalDetail option:selected").text());
            }
            if ($("#BlockList_AnaDataProposalDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AnaDataProposalDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/AnalysisDataProposalReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAnaDataProposal").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAnaDataProposal").html(response);
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

    //$("#btnViewAnaDataProposalDetail").trigger('click');
    //if ($('#Mast_Block_Code').val() > 0) {
    //    $("#btnViewAnaDataProposalDetail").trigger('click');
    //}
    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewAnaDataProposalDetail").trigger('click');
    }
    //this function call  on layout.js
    //closableNoteDiv("divCNR1Report", "spnCNR1Report");
    //$("#spCollapseIconCN").click(function () {

    //    if ($("#dvSearchParameter").is(":visible")) {
    //        $("#dvSearchParameter").hide("slow");
    //    }
    //    if ($("#dvSearchParameter").is(":hidden")) {
    //        $("#dvSearchParameter").show("slow");
    //    }
    //});


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    
    $("#DistrictList_AnaDataProposalDetail").val(0);
    $("#DistrictList_AnaDataProposalDetail").empty();
    $("#BlockList_AnaDataProposalDetail").val(0);
    $("#BlockList_AnaDataProposalDetail").empty();
    $("#BlockList_AnaDataProposalDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AnaDataProposalDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AnaDataProposalDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AnaDataProposalDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AnaDataProposalDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AnaDataProposalDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AnaDataProposalDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AnaDataProposalDetail").attr("disabled", "disabled");
                        $("#DistrictList_AnaDataProposalDetail").trigger('change');
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

        $("#DistrictList_AnaDataProposalDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AnaDataProposalDetail").empty();
        $("#BlockList_AnaDataProposalDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AnaDataProposalDetail").val(0);
    $("#BlockList_AnaDataProposalDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AnaDataProposalDetail").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AnaDataProposalDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AnaDataProposalDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AnaDataProposalDetail").attr("disabled", "disabled");
                        //$("#BlockList_AnaDataProposalDetail").trigger('change');
                    }
                    //$('#BlockList_AnaDataProposalDetail').find("option[value='0']").remove();
                    //$("#BlockList_AnaDataProposalDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AnaDataProposalDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AnaDataProposalDetail").append("<option value='0'>All Blocks</option>");
    }
}
