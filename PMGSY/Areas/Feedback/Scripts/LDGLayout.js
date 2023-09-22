$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmCategoryFeedbackReportLayout'));

    $("#btnViewCategorywisefeedbackreport").click(function () {
        //viewCompletedWorks();

        if ($('#frmCategoryFeedbackReportLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Feedback/Feedback/LDGreport/',
                type: 'POST',
                cache: false,
                data: $("#frmCategoryFeedbackReportLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadCategorywisefeedbackreport").html(response);
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

//$("#btnViewCategorywisefeedbackreport").trigger('click');

$("#spCollapseIconCN").click(function () {

    $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#frmCategoryFeedbackReportLayout").toggle("slow");

});

$("#FROM_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    maxDate:'-1',
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





