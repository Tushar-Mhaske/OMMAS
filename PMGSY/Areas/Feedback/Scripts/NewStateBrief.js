$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMMonthwiseInspectionsLayout'));

    $("#btnViewMonthwiseComparision").click(function () {
        if ($('#frmQMMonthwiseInspectionsLayout').valid()) {

            $('#StateName').val($('#ddlStateMonthwiseComparision option:selected').text());

            if ($("#hdnRole").val() == 8) {
                $("#ddlStateMonthwiseComparision").attr('disabled', false);
            }


            if ($("#ddlStateMonthwiseComparision").val() == 0 || $("#ddlStateMonthwiseComparision").val() == -1)
            {
                alert("Please select State");
                return false;
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Feedback/Feedback/NewStateReport/',
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
