$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFundRelease'));

    $("#btnViewFundReleaseDetail").click(function () {


        if ($('#frmFundRelease').valid()) {
            $("#loadFundRelease").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_FundReleaseDetail option:selected").text());
            $("#FundTypeName").val($("#FundTypeList_FundReleaseDetail option:selected").text());
            $("#TypeName").val($("#TypeList_FundReleaseDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_FundReleaseDetail option:selected").text());
            $("#AgencyName").val($("#AgencyList_FundReleaseDetail option:selected").text());
            if ($("#StateList_FundReleaseDetail").is(":visible")) {

                $("#StateName").val($("#StateList_FundReleaseDetail option:selected").text());
            }           

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/FundReleaseReport/',
                type: 'POST',
                catche: false,
                data: $("#frmFundRelease").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadFundRelease").html(response);

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
        $("#btnViewFundReleaseDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
