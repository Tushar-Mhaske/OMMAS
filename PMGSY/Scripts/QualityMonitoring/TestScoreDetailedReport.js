$(document).ready(function () {

    $('#monitorListD').chosen();

    $("#monitorTypeD").change(function () {

        if ($("#monitorTypeD").val() == "S") {
            $(".stateRow").show();
        }
        else {
            $(".stateRow").hide();
            $("#stateListId").val('0');
        }

        // Populate Monitors List
        var combineTypeAndState = $('#monitorTypeD').val() + "$" + $('#stateListId').val();

        $('#monitorListD').chosen('destroy');
        $("#monitorListD").empty();

        $.ajax({
            url: '/QualityMonitoring/PopulateMonitorsList',
            type: 'POST',
            async: false,
            cache: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { typeAndState: combineTypeAndState },
            success: function (jsonData) {

                for (var i = 0; i < jsonData.length; i++) {

                    $("#monitorListD").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $('#monitorListD').chosen();

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });

    });

    $("#stateListId").change(function () {

        // Populate Monitors List
        var combineTypeAndState = $('#monitorTypeD').val() + "$" + $('#stateListId').val();

        $('#monitorListD').chosen('destroy');
        $("#monitorListD").empty();

        $.ajax({
            url: '/QualityMonitoring/PopulateMonitorsList',
            type: 'POST',
            async: false,
            cache: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { typeAndState: combineTypeAndState },
            success: function (jsonData) {

                for (var i = 0; i < jsonData.length; i++) {

                    $("#monitorListD").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $('#monitorListD').chosen();

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });

    });

    if ($('#RoleCode').val() == 8 || $('#RoleCode').val() == 48 || $('#RoleCode').val() == 69) {

        var monitorType = $('#monitorTypeD').val();
        var stateCode = $('#stateListId').val();

        LoadReportView("0$" + monitorType + "$" + stateCode);
    }
    else {
        LoadReportView("0$A$0");
    }

});



$('#btnViewReport').click(function () {
    var monitorName = $('#monitorListD').val();
    var monitorType = $('#monitorTypeD').val();
    var stateCode = $('#stateListId').val();

    LoadReportView(monitorName + "$" + monitorType + "$" + stateCode);
});

function LoadReportView(mNameMTypeState) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/QualityMonitoring/ViewTestScoreDetailedReport/?mNameMTypeState=' + mNameMTypeState,
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        //data: { monitorType: monitorType, institutionName: institutionName},
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


