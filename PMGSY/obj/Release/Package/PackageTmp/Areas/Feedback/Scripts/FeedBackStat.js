$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMMonthwiseInspectionsLayout'));

    $("#btnViewMonthwiseComparision").click(function () {
        if ($('#frmQMMonthwiseInspectionsLayout').valid()) {

            $('#StateName').val($('#ddlStateMonthwiseComparision option:selected').text());

            if ($("#hdnRole").val() == 8) {
                $("#ddlStateMonthwiseComparision").attr('disabled', false);
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Feedback/Feedback/FeedbackStatReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMMonthwiseInspectionsLayout").serialize(),
                async: false,
                success: function (response) {
                    if ($("#hdnRole").val() == 8) {
                        $("#ddlStateMonthwiseComparision").attr('disabled', true);
                    }
                    $.unblockUI();
                    $("#dvLoadQMMonthwiseInspectionsReport").html(response);
                },
                error: function () {
                    if ($("#hdnRole").val() == 8) {
                        $("#ddlStateMonthwiseComparision").attr('disabled', true);
                    }
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });


    $("#btnViewMonthwiseComparision").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMMonthwiseInspectionsLayout").toggle("slow");

    });
});
