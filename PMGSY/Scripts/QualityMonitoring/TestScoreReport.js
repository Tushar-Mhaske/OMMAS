$(document).ready(function () {
    LoadReportView("A", "A", 0);

    $("#nqmSqmDropdownListId").change(function () {
        if ($("#nqmSqmDropdownListId").val() == "S") {
            $(".stateRow").show();
        }
        else {
            $(".stateRow").hide();
            $("#stateListId").val('0');
        }
    });
});

$('#btnViewReport').click(function () {
    var institutionName = $('#instName').val();
    var monitorType = $('#nqmSqmDropdownListId').val();
    var stateCode = $('#stateListId').val();

    if (monitorType == "A" || monitorType == "I")
        stateCode = 0;

    LoadReportView(institutionName, monitorType, stateCode);
});

function LoadReportView(institutionName, monitorType, stateCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    var combinedParam = institutionName + "$" + monitorType + "$" + stateCode;

    $.ajax({
        url: '/QualityMonitoring/ViewTestScoreReport/?combinedParam=' + combinedParam,
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        beforeSend: function () { },
        success: function (data) {
            $('#viewReport').html(data);
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}


