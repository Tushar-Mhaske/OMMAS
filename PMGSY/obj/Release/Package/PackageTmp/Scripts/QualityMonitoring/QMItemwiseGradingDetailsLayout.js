$(document).ready(function () {

    $('#loadItemwiseReport').html("");

    $('#btnGetItemwiseReport').click(function () {

        loadItemwiseReport();
        //if (checkMonthandYear()) {
        //    loadItemwiseReport();
        //}
        //else {
        //    alert("Selected Duration is invalid");
        //}

    });


});

function loadItemwiseReport() {
    //alert("function called and " + $('#frmloadreport').valid());
    if ($('#frmloadreport').valid()) {

        $.ajax({
            url: '/QualityMonitoring/QMItemwiseGradingDetailsReport/',
            type: 'POST',
            data: $('#frmloadreport').serialize(),
            success: function (response, status) {
                $("#loadItemwiseReport").html(response);
             //   alert("status " + status);
            },
            error: function () {
                alert('Something went wrong. Please try again');
            }
        });
    }
}