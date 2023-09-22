$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmFeedbackRep');
    
    if ($('#hdnRepStatus').val() == "I" || $('#hdnRepStatus').val() == "")
    {
        $('#rdbRepI').attr('checked', true);
    }
    else if ($('#hdnRepStatus').val() == "F") {
        $('#rdbRepF').attr('checked', true);
    }

    $("#btnCloseRep").click(function () {
        CloseFBReply();
    });

    $("#btnResetRep").click(function () {
        $("#txtFeedReply").empty();
    });

    $("#SQCbtnSubmitRep").click(function () {
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {

   
            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?")))
            {
                $.ajax({
                    url: "/FeedBackDetails/SQCsaveFeedBackRepNew/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("SQC Feedback Reply is saved Successfully.");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatusNew/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                $('#tbReplyStatus').show('slow');
                                $("#tbReplyStatus").css('height', 'auto');
                            });

                        }
                        else {
                            alert("Error occured while saving Feedback Reply");
                        }

                    },
                    error: function () {
                        alert("error");
                    }
                });
            }
            else {
                if (!($('input[name="Feed_Reply"]:checked').val() == "I") && !($('input[name="Feed_Reply"]:checked').val() == "F")) {
                    alert('Please select reply type');
                }
            }
        }
    });

    $("#SQCbtnUpdateRep").click(function () {


   //     alert("zc")
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {
            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?"))) {
                $.ajax({
                    url: "/FeedBackDetails/SQCupdateFeedBackRepNew/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("SQC Feedback Reply is updated Successfully.");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatusNew/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                $('#tbReplyStatus').show('slow');
                                $("#tbReplyStatus").css('height', 'auto');
                            });

                        }
                        else {
                            alert("Error occured while updating Feedback Reply");
                        }

                    },
                    error: function () {
                        alert("error");
                    }
                });
            }
            else {
                if (!($('input[name="Feed_Reply"]:checked').val() == "I") && !($('input[name="Feed_Reply"]:checked').val() == "F"))
                {
                    alert('Please select reply type');
                }
            }
        }
    });
});

function CloseFBReply() {
    
    $("#tbFBReplyStatusJqGrid").jqGrid('setGridState', 'visible');
    $("#tbFBReplyStatusJqGrid").trigger('reloadGrid');
    
    $('#dvReplyStatus').hide('slow');
    $('#btnAdd').show('slow');
   
}