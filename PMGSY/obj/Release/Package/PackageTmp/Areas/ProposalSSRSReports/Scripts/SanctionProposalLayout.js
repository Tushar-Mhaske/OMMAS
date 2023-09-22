$(function () {

    $.validator.unobtrusive.parse($('#frmSanctionProposalLayout'));

    $('#btnPropSanctionDetails').click(function () {

        $('#Year').val($('#ddYear_PropSanctionDetails').val());
        $('#Batch').val($('#ddBatch_PropSanctionDetails').val());
        $('#Collaboration').val($('#ddAgency_PropSanctionDetails').val());

        $('#CollabName').val($('#ddAgency_PropSanctionDetails option:selected').text());
        $('#StatusName').val("All");

        if ($('#frmSanctionProposalLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/SanctionProposalReport/',
                type: 'POST',
                catche: false,
                data: $("#frmSanctionProposalLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadSanctionProposalReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnPropSanctionDetails').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSanctionProposalLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
