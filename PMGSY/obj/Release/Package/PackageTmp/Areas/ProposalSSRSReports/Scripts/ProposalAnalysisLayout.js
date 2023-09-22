$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmProposalAnalysisLayout'));

    if ($("#State").val() > 0) {

        $("#ddState_PropAnalysisDetails").attr("disabled", "disabled");
    }

    $('#btnGoPropAnalysis').click(function () {

        $('#State').val($('#ddState_PropAnalysisDetails option:selected').val()); //var year = $('#ddYear_PropListDetails').val();
        $('#Scrutiny').val($('#ddScrutiny_PropAnalysisDetails option:selected').val());
        $('#Sanctioned').val($('#ddSanctioned_PropAnalysisDetails option:selected').val());
        $('#Proposal').val($('#ddProposal_PropAnalysisDetails option:selected').val());

        $('#StateName').val($('#ddState_PropAnalysisDetails option:selected').text()); //var year = $('#ddYear_PropListDetails').val();
        $('#ScrutinyName').val($('#ddScrutiny_PropAnalysisDetails option:selected').text());
        $('#SanctionName').val($('#ddSanctioned_PropAnalysisDetails option:selected').text());
        $('#ProposalType').val($('#ddProposal_PropAnalysisDetails option:selected').text());

        if ($('#frmProposalAnalysisLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/ProposalAnalysisReport/',
                type: 'POST',
                catche: false,
                data: $("#frmProposalAnalysisLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadProposalAnalysisReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnGoPropAnalysis').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvProposalAnalysisLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});