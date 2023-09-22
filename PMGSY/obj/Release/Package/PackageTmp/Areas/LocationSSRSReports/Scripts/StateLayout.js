$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateLayout'));


    $("#StateDetailsButton").click(function () {

        $('#Territory').val($("#StateList_StateDetails option:selected").val());
        $('#StateType').val($("#StateType_StateDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_StateDetails option:selected").val());

        if ($('#frmStateLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/StateReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadStateReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#StateDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmStateLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
