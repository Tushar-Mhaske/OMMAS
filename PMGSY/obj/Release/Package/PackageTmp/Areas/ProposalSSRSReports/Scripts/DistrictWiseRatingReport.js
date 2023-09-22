$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmDistRating'));
    $("#StateList_DistRatingDetail").change(function () {
        loadDistrict($("#StateList_DistRatingDetail").val());

    });

    $("#DistrictList_DistRatingDetail").change(function () {
        loadBlock($("#StateList_DistRatingDetail").val(), $("#DistrictList_DistRatingDetail").val());

    });
    $("#btnViewDistRatingDetail").click(function () {


        if ($('#frmDistRating').valid()) {
            $("#loadDistRating").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_DistRatingDetail option:selected").text());
            $("#StatusName").val($("#StatusList_DistRatingDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_DistRatingDetail option:selected").text());
            $("#BatchName").val($("#BatchList_DistRatingDetail option:selected").text());
            if ($("#StateList_DistRatingDetail").is(":visible")) {

                $("#StateName").val($("#StateList_DistRatingDetail option:selected").text());
            }

            if ($("#DistrictList_DistRatingDetail").is(":visible")) {

                //$('#DistrictList_DistRatingDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_DistRatingDetail option:selected").text());
            }
            if ($("#BlockList_DistRatingDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_DistRatingDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictWiseRatingReport/',
                type: 'POST',
                catche: false,
                data: $("#frmDistRating").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadDistRating").html(response);

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
        $("#btnViewDistRatingDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_DistRatingDetail").val(0);
    $("#DistrictList_DistRatingDetail").empty();
    $("#BlockList_DistRatingDetail").val(0);
    $("#BlockList_DistRatingDetail").empty();
    $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_DistRatingDetail").length > 0) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_DistRatingDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_DistRatingDetail').find("option[value='0']").remove();
                    //$("#DistrictList_DistRatingDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_DistRatingDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_DistRatingDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_DistRatingDetail").attr("disabled", "disabled");
                        $("#DistrictList_DistRatingDetail").trigger('change');
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

        $("#DistrictList_DistRatingDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_DistRatingDetail").empty();
        $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_DistRatingDetail").val(0);
    $("#BlockList_DistRatingDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_DistRatingDetail").length > 0) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_DistRatingDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_DistRatingDetail").val($("#Mast_Block_Code").val());
                        // $("#BlockList_DistRatingDetail").attr("disabled", "disabled");
                        //$("#BlockList_DistRatingDetail").trigger('change');
                    }
                    //$('#BlockList_DistRatingDetail').find("option[value='0']").remove();
                    //$("#BlockList_DistRatingDetail").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_DistRatingDetail').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");
    }
}