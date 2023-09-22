/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />

$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmAddticket");

    $("#btnSubmit").click(function () {
        SaveTicketDetails();
    });

    // function added by rohit borse on 14-07-2022
    
    $('#ddlModule').change(function () {
       
        var id = $("#ddlModule option:selected").val();
        if (id > 0) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: '/Ticket/GetForwardToDetails/' + id,
                type: 'GET',
                dataType: 'json',
                //cache: false,
                //async: false,           
                // data: $("#ddlModule option:selected").val(),
                success: function (data) {
                    if (data != null) {
                        $('#forwardToAuthority').html("Ticket will be forwarded to " + data);
                        $('#forwardToAuthority').show();
                    }
                    else {
                        $("#divError span").text("error occured while processing your request.");
                        $("#divError").show();
                    }
                    $.unblockUI();
                },
                error: function () {
                    $("#divError span").text("error occured while processing your request.");
                    $("#divError").show();
                    $.unblockUI();
                }
            });
        }
        $('#forwardToAuthority').hide();

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
