/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />
$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmReplyticket");

    if ($(".tdfwdto").is(":hidden"))
    {
        $(".tdfwdto").show();
    }
    $("#CurrentStatus").change(function () {
        if ($("#CurrentStatus option:selected").val() == 4) {
            $(".tdfwdto").hide();
            $("#ForwardTo").val("0");
        }
        else{
            $(".tdfwdto").show();
        }
    });

    $("#btnReset").click(function() {
        $(".tdfwdto").show();
    });

    $("#btnSubmit").click(function () {

        if ($('#ReplyFile').val() != null && $('#ReplyFile').val() != undefined && $('#ReplyFile').val() != "")
        {
            var extension = $('#ReplyFile').val().split('.').pop().toLowerCase();

            if ($.inArray(extension, ['pdf', 'png', 'jpg', 'jpeg']) == -1) {
                alert('Only pdf,jpg,png files are allowed');
                $('#ReplyFile').val('');
                return false;
            }
            var FileSize = $('#ReplyFile').get(0).files[0].size / 1024 / 1024; // in MB
            if (FileSize > 4) {
                alert('File size should be upto 4 MB');
                $('#ReplyFile').val(''); //for clearing with Jquery
                return false;
            }
        }

        if ($('#frmReplyticket').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            // var formData = new FormData($(this).parents('form')[0]);  //to parse whole form
            var formData = new FormData();
            formData.append("ReplyFile", $('#ReplyFile').get(0).files[0])
            formData.append("ticketNo", jQuery("#TicketNo").val())
            formData.append("TicketReply", jQuery("#TicketReply").val());
            formData.append("CurrentStatus", jQuery("#CurrentStatus option:selected").val());
            formData.append("ForwardTo", jQuery("#ForwardTo option:selected").val());
            formData.append("__RequestVerificationToken", jQuery("#frmReplyticket input[name='__RequestVerificationToken']").val());


           
                $.ajax({
                    url: '/Ticket/SaveTicketReplyDetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    //contentType: "multipart/form-data",
                    contentType: false,
                    processData: false,
                    data: formData,
                    success: function (response) {
                        $("#divError span").text("");
                        $("#divError").hide();
                        if (response.success) {
                            alert(response.message);
                            CloseTicketDetails();
                            $("#tbTicketList").trigger("reloadGrid");
                            $("#tbAllTicketList").trigger("reloadGrid");
                        }
                        else {
                            $("#divError span").text("");
                            $("#divError span").text(response.message);
                            $("#divError").show('slow');
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
    });
});