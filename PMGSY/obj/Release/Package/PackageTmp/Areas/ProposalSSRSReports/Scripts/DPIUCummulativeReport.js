$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmDPIUCummlative'));
    $("#StateList_DPIUCummlativeDetail").change(function () {
        loadDistrict($("#StateList_DPIUCummlativeDetail").val());

    });

 
    $("#btnViewDPIUCummlativeDetail").click(function () {


        if ($('#frmDPIUCummlative').valid()) {
            $("#loadDPIUCummlative").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_DPIUCummlativeDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_DPIUCummlativeDetail option:selected").text());
            $("#BatchName").val($("#BatchList_DPIUCummlativeDetail option:selected").text());
            if ($("#StateList_DPIUCummlativeDetail").is(":visible")) {

                $("#StateName").val($("#StateList_DPIUCummlativeDetail option:selected").text());
            }

            if ($("#DistrictList_DPIUCummlativeDetail").is(":visible")) {

                //$('#DistrictList_DPIUCummlativeDetail').attr("disabled", false);
                $("#DistName").val($("#DistrictList_DPIUCummlativeDetail option:selected").text());
            }
            //if ($("#BlockList_DPIUCummlativeDetail").is(":visible")) {

            //    $("#BlockName").val($("#BlockList_DPIUCummlativeDetail option:selected").text());
            //}

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DPIUWiseCumulativePositionReport/',
                type: 'POST',
                catche: false,
                data: $("#frmDPIUCummlative").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadDPIUCummlative").html(response);

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
        $("#btnViewDPIUCummlativeDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_DPIUCummlativeDetail").val(0);
    $("#DistrictList_DPIUCummlativeDetail").empty();
    //$("#BlockList_DPIUCummlativeDetail").val(0);
    //$("#BlockList_DPIUCummlativeDetail").empty();
    //$("#BlockList_DPIUCummlativeDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_DPIUCummlativeDetail").length > 0) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_DPIUCummlativeDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_DPIUCummlativeDetail').find("option[value='0']").remove();
                    //$("#DistrictList_DPIUCummlativeDetail").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_DPIUCummlativeDetail').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_DPIUCummlativeDetail").val($("#Mast_District_Code").val());
                        // $("#DistrictList_DPIUCummlativeDetail").attr("disabled", "disabled");
                        $("#DistrictList_DPIUCummlativeDetail").trigger('change');
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

        $("#DistrictList_DPIUCummlativeDetail").append("<option value='0'>All Districts</option>");
        //$("#BlockList_DPIUCummlativeDetail").empty();
        //$("#BlockList_DPIUCummlativeDetail").append("<option value='0'>All Blocks</option>");

    }
}

