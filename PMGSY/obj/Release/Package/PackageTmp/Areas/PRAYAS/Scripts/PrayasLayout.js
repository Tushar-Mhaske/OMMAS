$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDarpanLayout'));

    $("#btnGenerateJSON1").click(function () {
        if ($('#frmDarpanLayout').valid()) {

            $('#StateName').val($('#ddlStateMonthwiseComparision option:selected').text());

            if ($("#hdnRole").val() == 8) {
                $("#ddlStateMonthwiseComparision").attr('disabled', false);
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PRAYAS/Prayas/generateJsonClick/',
                type: 'POST',
                catche: false,
                data: $("#frmDarpanLayout").serialize(),
                async: false,
                success: function (response) {
                    if (response.success) {
                        //$("#ddlStateMonthwiseComparision").attr('disabled', true);
                    }
                    $.unblockUI();
                    alert(response.message);
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

   // $("#btnGenerateJSON").trigger('click');
});
