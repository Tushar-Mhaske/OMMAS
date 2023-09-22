$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmReconciliation'));

    $("#btnView").click(function () {
        if ($('#frmReconciliation').valid()) {
            $("#loadReport").html("");

            $("#StateName").val($('#ddlState option:selected').text());

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AccountReports/Account/ReconciliationReport/',
                type: 'POST',
                catche: false,
                data: $("#frmReconciliation").serialize(),
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

   // $("#btnView").trigger('click');

});