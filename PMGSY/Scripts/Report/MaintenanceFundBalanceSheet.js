$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmMaintenanceBalanceSheet'));

    $('#btnViewDetails').click(function () {

        SearchMaintenanceFundBalacneFund();
    });

    $('#btnViewDetails').trigger('click');

});

function SearchMaintenanceFundBalacneFund() {

    if ($('#frmMaintenanceBalanceSheet').valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Reports/GetMaintenanceFundBalanceSheet",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmMaintenanceBalanceSheet").serialize(),
            success: function (data) {

                $('#rptMaintenanceBalanceSheet').html(data);

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
}