$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmAgreementAbstract'));

    $("#btnView").click(function () {

        if ($('#frmAgreementAbstract').valid()) {
            $("#divLoadReport").html("");

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
                url: '/ProposalSSRSReports/ProposalSSRSReports/AgreementAbstractReport/',
                type: 'POST',
                catche: false,
                data: $("#frmAgreementAbstract").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("An Error ocurred while proessing your request..");
                    return false;
                },
            });
        }
        else {

        }
    });

});