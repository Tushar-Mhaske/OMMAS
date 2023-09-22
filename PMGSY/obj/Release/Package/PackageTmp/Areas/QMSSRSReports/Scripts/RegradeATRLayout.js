$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmRegradeATRLayout'));

    $("#btnViewRegradeATR").click(function () {

        $('#StateName').val($('#ddlRegradeATRState option:selected').text());

        if ($('#frmRegradeATRLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/RegradeATRReport/',
                type: 'POST',
                catche: false,
                data: $("#frmRegradeATRLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadRegradeATRReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnViewRegradeATR").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmRegradeATRLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});


$("#FROM_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: "dd/mm/yy",
    showOn: 'button',
    buttonImage: '../../Content/images/calendar_2.png',
    buttonImageOnly: true,
    onSelect: function (selectedDate) {
        $("#TO_DATE").datepicker("option", "minDate", selectedDate);
        $(function () {
            $('#FROM_DATE').focus();
            $('#TO_DATE').focus();
        })
    },
    onClose: function () {
        $(this).focus().blur();
    }
}).attr('readonly', 'readonly');


$("#TO_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: "dd/mm/yy",
    showOn: 'button',
    buttonImage: '../../Content/images/calendar_2.png',
    buttonImageOnly: true,
    onSelect: function (selectedDate) {
        $("#FROM_DATE").datepicker("option", "maxDate", selectedDate);
    },
    onClose: function () {
        $(this).focus().blur();
    }
}).attr('readonly', 'readonly');
