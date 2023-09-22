/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />

$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmAcceptticket");
    
    $("#AcceptReject").change(function () {

        if ($("#AcceptReject option:selected").val() == 2) {
            $(".tdfwdto").hide();
            $("#ForwardTo").val("0");
        }
        else {
            $(".tdfwdto").show();
        }
    });

    $("#btnSubmit").click(function () {
        SaveTicketApproval();
    });
});

function SaveTicketApproval()
{
    if ($('#frmAcceptticket').valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: '/Ticket/SaveApproveDetails',
            type: 'POST',
            cache: false,
            async: false,
            //contentType: "multipart/form-data",
            data: $('#frmAcceptticket').serialize(),
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    CloseTicketDetails();
                    $("#tbTicketList").trigger("reloadGrid");
                    $("#tbAllTicketList").trigger("reloadGrid");

                }
                else {
                    alert(response.message);
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function () {
                $("#divError span").text("");
                $("#divError span").text("error occured while processing your request.");
                $("#divError").show();
                $.unblockUI();
                // alert("Error occured while processing your request");
            },
            statusCode: function () {
            }
        });
    }

}