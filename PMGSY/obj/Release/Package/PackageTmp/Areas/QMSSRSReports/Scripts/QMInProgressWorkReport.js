$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMMonthwiseInspectionsLayout'));

    $("#btnInProgressWorkView").click(function () {
        if ($('#InProgressWorkViewModel').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMInProgressWorkReportPost',
                type: 'POST',
                catche: false,
                data: $("#InProgressWorkViewModel").serialize(),
                async: false,
                success: function (response) {

                    $.unblockUI();
                    $("#dvLoadQMMonthwiseInspectionsReport").html(response);
                },
                error: function () {

                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });


});