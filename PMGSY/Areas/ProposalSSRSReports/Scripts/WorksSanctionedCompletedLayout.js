$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmWorksSanctionedCompletedLayout'));

    $('#btnView').click(function () {
        ReportCall();
    });
});

function ReportCall() {
    $.ajax({
        url: '/ProposalSSRSReports/ProposalSSRSReports/WorksSanctionedCompletedReport/',
        type: 'POST',
        cache: false,
        data: $("#frmWorksSanctionedCompletedLayout").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#dvloadReport").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
    });
}