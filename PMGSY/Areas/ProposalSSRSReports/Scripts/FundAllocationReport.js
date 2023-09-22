$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFundAllocation'));

    $("#btnViewFundAllocationDetail").click(function () {


        if ($('#frmFundAllocation').valid()) {
            $("#loadFundAllocation").html("");

            $("#FundingAgencyName").val($("#FundingAgencyList_FundAllocationDetail option:selected").text());
            $("#FundTypeName").val($("#FundTypeList_FundAllocationDetail option:selected").text());     
            $("#YearName").val($("#PhaseYearList_FundAllocationDetail option:selected").text());
            $("#AgencyName").val($("#AgencyList_FundAllocationDetail option:selected").text());
            if ($("#StateList_FundAllocationDetail").is(":visible")) {

                $("#StateName").val($("#StateList_FundAllocationDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/FundAllocationReport/',
                type: 'POST',
                catche: false,
                data: $("#frmFundAllocation").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadFundAllocation").html(response);

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
        $("#btnViewFundAllocationDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
