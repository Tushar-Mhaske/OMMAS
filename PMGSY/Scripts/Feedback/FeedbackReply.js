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

    $("#btnSubmitRep").click(function () {
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {

           // alert($("#rdbRepI").is(":checked").val());
            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?")))
            {
                $.ajax({
                    url: "/FeedBackDetails/saveFeedBackRep/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("Feedback Reply saved Successfully");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatus/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                //    unblockPage();
                                //});
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
                })
            }
            else {
                if (!($('input[name="Feed_Reply"]:checked').val() == "I") && !($('input[name="Feed_Reply"]:checked').val() == "F")) {
                    alert('Please select reply type');
                }
            }
        }
    });

    $("#btnUpdateRep").click(function () {
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {
            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?"))) {
                $.ajax({
                    url: "/FeedBackDetails/updateFeedBackRep/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("Feedback Reply updated Successfully");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatus/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                //    unblockPage();
                                //});
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
                })
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
    //$('#accordion').hide('slow');
    //$('#dvFeedbackDtls').hide('slow');
    $("#tbFBReplyStatusJqGrid").jqGrid('setGridState', 'visible');
    $("#tbFBReplyStatusJqGrid").trigger('reloadGrid');
    // For DPIU Login Reload the Jqgrid 
    //if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38') {

    //    LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val());
    //}
    $('#dvReplyStatus').hide('slow');
    $('#btnAdd').show('slow');
    //$("#divFBDetails").show("slow");
    //showFilter();

    //$('#tbFBDetailsForm').html('');
    //$('#tbFBApproval').html('');
    //$('#tbReplyStatus').html('');
}