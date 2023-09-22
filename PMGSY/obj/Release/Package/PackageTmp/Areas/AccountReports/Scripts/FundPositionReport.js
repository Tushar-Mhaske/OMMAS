$(document).ready(function () {
    $('#btnViewFundPostionReport').click(function () {

        if ($("#frmFundPosition").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            //Ajax Call
            $.ajax({
                url: "/AccountReports/Account/FundPostionReport/",
                type: "POST",
                data: $("#frmFundPosition").serialize(),
                success: function (data) {
                    $("#loadfundpostionReport").html(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
            $.unblockUI();
        }
    })

});