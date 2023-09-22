$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFinancial'));
    $("#btnView").click(function () {
        if ($('#frmFinancial').valid()) {
            $("#loadReport").html("");
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Feedback/Feedback/LengthClearedFourReport/',
                type: 'POST',
                catche: false,
                data: $("#frmFinancial").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });

    $("#btnView").trigger('click');

});