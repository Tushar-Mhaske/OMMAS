/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />

$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmAddticket");

    $("#btnSubmit").click(function () {
        SaveTicketDetails();
    });
});


function SaveTicketDetails()
{
    if ($('#frmAddticket').valid()) {
        if (confirm("Are you sure to save ticket details?")) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: '/Ticket/AddTicketDetails',
                type: 'POST',
                cache: false,
                async: false,
                //contentType: "multipart/form-data",
                data: $('#frmAddticket').serialize(),
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                    }
                    else {
                        $("#divError span").text("error occured while processing your request.");
                        $("#divError").show();
                    }
                },
                complete: function () {
                    $.unblockUI();
                    CloseTicketDetails();
                    $("#tbTicketList").trigger("reloadGrid");
                    $("#tbAllTicketList").trigger("reloadGrid");


                },
                error: function () {
                    $("#divError span").text("error occured while processing your request.");
                    $("#divError").show();
                    // alert("Error occured while processing your request");
                },
                statusCode: function () {
                }
            });
        }
    }
}
