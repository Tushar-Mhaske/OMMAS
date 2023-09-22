$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmAgreement'));
    $("#StateList_AgreementDetail").change(function () {
        loadDistrict($("#StateList_AgreementDetail").val());

    });

    $("#DistrictList_AgreementDetail").change(function () {
        loadBlock($("#StateList_AgreementDetail").val(), $("#DistrictList_AgreementDetail").val());

    });
    $("#btnViewAgreementDetail").click(function () {


        if ($('#frmAgreement').valid()) {
            $("#loadAgreement").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_AgreementDetail option:selected").text());
            $("#StatusName").val($("#StatusList_AgreementDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_AgreementDetail option:selected").text());
            $("#BatchName").val($("#BatchList_AgreementDetail option:selected").text());
            if ($("#StateList_AgreementDetail").is(":visible")) {

                $("#StateName").val($("#StateList_AgreementDetail option:selected").text());
            }

            if ($("#DistrictList_AgreementDetail").is(":visible")) {

                //$('#DistrictList_AgreementDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_AgreementDetail option:selected").text());
            }
            if ($("#BlockList_AgreementDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_AgreementDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictWiseRatingReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAgreement").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadAgreement").html(response);

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
        $("#btnViewAgreementDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_AgreementDetail").val(0);
    $("#DistrictList_AgreementDetail").empty();
    $("#BlockList_AgreementDetail").val(0);
    $("#BlockList_AgreementDetail").empty();
    $("#BlockList_AgreementDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_AgreementDetail").length > 0) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_AgreementDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_AgreementDetail').find("option[value='0']").remove();
                    //$("#DistrictList_AgreementDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_AgreementDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_AgreementDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_AgreementDetail").attr("disabled", "disabled");
                        $("#DistrictList_AgreementDetail").trigger('change');
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

        $("#DistrictList_AgreementDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_AgreementDetail").empty();
        $("#BlockList_AgreementDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_AgreementDetail").val(0);
    $("#BlockList_AgreementDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_AgreementDetail").length > 0) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_AgreementDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_AgreementDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_AgreementDetail").attr("disabled", "disabled");
                        //$("#BlockList_AgreementDetail").trigger('change');
                    }
                    //$('#BlockList_AgreementDetail').find("option[value='0']").remove();
                    //$("#BlockList_AgreementDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_AgreementDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_AgreementDetail").append("<option value='0'>All Blocks</option>");
    }
}