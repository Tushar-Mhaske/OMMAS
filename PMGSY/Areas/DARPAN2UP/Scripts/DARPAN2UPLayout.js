$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDarpanLayout'));
   /* alert("1");*/
    $("#btnGenDARPAN2UPJSON1").click(function () {
       /* alert("2");*/
        if ($('#frmDarpanLayout').valid()) {

            $('#StateName').val($('#ddlStateMonthwiseComparision option:selected').text());

            if ($("#hdnRole").val() == 8) {
                $("#ddlStateMonthwiseComparision").attr('disabled', false);
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/DARPAN2UP/DARPAN2UP/Index/',
                type: 'POST',
                catche: false,
                data: $("#frmDarpanLayout").serialize(),
                async: false,
                success: function (response) {
                    if (response.success) {
                        
                    }
                    $.unblockUI();
                    alert(response.message);
                    /*alert("Data Ported Successfully");*/
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