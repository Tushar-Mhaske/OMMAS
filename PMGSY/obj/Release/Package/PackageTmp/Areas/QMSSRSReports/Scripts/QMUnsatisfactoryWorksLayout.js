$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMUnsatisfactoryWorkLayout'));

    $("#btnViewUnsatisfactoryWorks").click(function () {

        if ($('#frmQMUnsatisfactoryWorkLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMUnsatisfactoryWorkReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMUnsatisfactoryWorkLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadQMUnsatisfactoryWorksReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnViewUnsatisfactoryWorks").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMUnsatisfactoryWorkLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
