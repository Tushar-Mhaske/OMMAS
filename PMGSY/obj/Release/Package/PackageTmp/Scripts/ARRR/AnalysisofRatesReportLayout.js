$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadScheduleRates');

    $("#idFilterDivI").click(function () {
        $("#idFilterDivI").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmLoadScheduleRates").toggle("slow");
    });

    $("#btnView").click(function () {
        debugger;
        //var data = $("#frmAnalysisofRatesReportLayout").serialize();
        $.ajax({
            url: '/ARRR/AnalysisofRatesReport/',
            async: false,
            cache: false,
            type: "GET",
            data: $("#frmAnalysisofRatesReportLayout").serialize(),
            //data: { typeCode: $('#ItemTypeCode').val() },
            dataType: "html",
            success: function (data) {
                $('#divLoadAnalysisRatesReport').html(data);
                //$('#divLoadAnalysisRatesReport').show('fade');
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    });
});