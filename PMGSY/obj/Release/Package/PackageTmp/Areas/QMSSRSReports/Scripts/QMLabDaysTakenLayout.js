$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmLabDaysTakenLayout'));

    $("#btnLabDaysTaken").click(function () {

        $('#StateName').val($('#ddlStateLabDaysTaken option:selected').text());

        if ($('#frmLabDaysTakenLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/DaysTakenReport/',
                type: 'POST',
                catche: false,
                data: $("#frmLabDaysTakenLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadLabDaysTakenReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnLabDaysTaken").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmLabDaysTakenLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
