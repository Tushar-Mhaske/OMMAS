$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmAdminBalanceSheet'));

    $('#btnViewDetails').click(function () {

        SearchAdminFundBalacneFund();
    });

    $('#btnViewDetails').trigger('click');

});

function SearchAdminFundBalacneFund() {

    if ($('#frmAdminBalanceSheet').valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Reports/GetAdminFundBalanceSheet",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmAdminBalanceSheet").serialize(),
            success: function (data) {

                $('#rptAdminBalanceSheet').html(data);

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
}