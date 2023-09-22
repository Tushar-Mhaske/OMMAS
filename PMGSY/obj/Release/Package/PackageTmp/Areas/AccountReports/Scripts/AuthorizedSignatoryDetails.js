$(document).ready(function () {

    // show/hide data table of Authorised Signatory Details
    $("#SpnListAuthSignatoryDetails").click(function () {
        $("#SpnListAuthSignatoryDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvAuthSignatoryDetails").slideToggle("slow");
    });

    $("#btnViewDetails").click(function () {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'POST',
            url: '/AccountReports/Account/AuthorizedSignatoryReport',
            data: $("#frmAuthSignatoryDetails").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#dvShowAuthSignatoryDetails").html('');
                $("#dvShowAuthSignatoryDetails").html(data);

                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Request can not be processed at this time.");
            }
        })
    });


    $(function () {
        $("#btnViewDetails").trigger("click");
    });

});


