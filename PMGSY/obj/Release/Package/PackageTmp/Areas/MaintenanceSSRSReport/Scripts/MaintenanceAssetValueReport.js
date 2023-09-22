$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAssetValueReport'));



    if ($('#frmAssetValueReport').valid()) {
        $("#loadAssetValueReport").html("");

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/MaintenanceAssetValueReport/',
            type: 'POST',
            catche: false,
            data: $("#frmAssetValueReport").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadAssetValueReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    }





    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
