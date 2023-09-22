$(document).ready(function () {

    MeetingReportCall();

     $('#MeetingDetailsButton').click(function () {
        $.blockUI({ message: null });
        MeetingReportCall();
    });
});


function MeetingReportCall() {
    $.ajax({
        url: '/SLSCGB/MeetingDetailsListing/',
        type: 'POST',
        catche: false,
        data: $("#MeetingForm").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#MeetingDetailsDiv").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}

function MeetingDownloadPDF(fileName) {
    var url = "/SLSCGB/MeetingFileDownloadPdf?FileName=" + fileName;
    window.location.href = url;
    return false;
    }