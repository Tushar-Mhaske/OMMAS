$(document).ready(function () {
    $.validator.unobtrusive.parse('#phaseSanctionForm');
    PhaseSanctionReport();
    $('#PhasedetailButton').click(function () {
        PhaseSanctionReport();
    })
});

function PhaseSanctionReport() {
    if ($('#phaseSanctionForm').valid()) {
        $.blockUI({ message: null });
        $.ajax({
            url: '/OtherReports/OtherReports/PhaseSactionReport',
            type: 'POST',
            cache: false,
            async: false,
            data: $('#phaseSanctionForm').serialize(),
            success: function (response) {
                $.unblockUI();
                $("#PhaseSanctionedreportDiv").html(response);
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
}