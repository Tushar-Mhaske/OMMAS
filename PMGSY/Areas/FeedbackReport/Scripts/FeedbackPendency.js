$(document).ready(function () {
    LoadReport();
    $('#btn_fbreport_search').click(function () {
        LoadReport();
    });

    $('#spCollapseIconCN').click(function () {
        $('#frmFBPendency').toggle();
    });

});

function LoadReport() {

    $.blockUI({ message: null });
    $.ajax({
        url: '/FeedbackReport/Feedback/LoadFBPendencyReport/',
        type: 'POST',
        catche: false,
        data: $('#frmFBPendency').serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            if (response.success==false) {
                alert(response.message);
            }
            else {
                $("#FBPendencyReportDiv").html(response);
            }
         

        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });

}
function AddAntiForgeryToken(data) {
    data.__RequestVerificationToken = $('#frmFBPendency input[name=__RequestVerificationToken]').val();

    return data;
};