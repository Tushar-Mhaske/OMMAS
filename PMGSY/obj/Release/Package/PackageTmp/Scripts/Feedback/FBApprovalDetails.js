$.validator.unobtrusive.adapters.addBool("booleanrequired", "required");

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmFBApproval');
    $('#frmFBFilesDisplay').html('');

    if ($('#hdnRepstat').val() != "O" && $('#hdnRepstat').val() != "N") {
        $('#ddlState').attr("disabled", true);
        $('#ddlDistrict').attr("disabled", true);
        $('#txtApprFeedReply').attr("disabled", true);
    }
    else {
        $('#ddlState').attr("disabled", false);
        $('#ddlDistrict').attr("disabled", false);
        $('#txtApprFeedReply').attr("disabled", false);
    }

    if ($('#hdnApproval').val() == "Y") {
        $('#txtApprFeedReply').val('');
        $('#trComments').hide('slow');
    }
    else {
        $('#trComments').show('slow');
    }

    $(function () {
        $("#rdbApprY").click(function () {
            $('#txtApprFeedReply').val('');
            $('#trComments').hide('slow');
        });

        $("#rdbApprN").click(function () {
            $('#trComments').show('slow');
        });
    });

    $("#ddlState").change(function () {
        $.ajax({
            url: "/FeedbackDetails/fillDDLDistricts?Code=" + $("#ddlState option:selected").val(),
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {
                $("#ddlDistrict").empty();
                $.each(data, function () {
                    $("#ddlDistrict").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            },
            error: function () {
                alert("error");
            }
        })
    });

    $("#btnSubmitApr").click(function () {
        var flag = false;

        if ($('#frmFBApproval').valid()) {
            if ($('#hdnCitizenId').val() != null) {
                //alert($('#ddlState option:selected').val());
                if ($('#ddlState option:selected').val() == "-1" && $('#rdbApprY').is(':checked')) {
                    $('#message').html('Please select State');
                    $('#dvErrMessage').show('slow');
                    //alert(0);
                }
                else if ($('#txtApprFeedReply').val() == "" && $('#rdbApprN').is(':checked')) {
                    $('#message').html('Please enter reason for not acceptance');
                    $('#dvErrMessage').show('slow');
                }
                else {
                    //alert(1);
                    submit();
                }
            }
            else {
                //alert(2);
                $('#message').html('');
                $('#dvErrMessage').hide('slow');
                submit();
                //alert($('#CitizenId').val());
            }
        }
    });
});

function submit() {
    if ($('#rdbApprN').is(':checked') == true) {
        if ($.trim($('#txtApprFeedReply').val()).length == 0)
        {
            alert("Please enter valid reason");
            return;
        }
        if (confirm("Once submitted status cannot be changed. Do you want to proceed?")) {
            fbsubmit();
        }
    }
    else {
        fbsubmit();
    }
}

function fbsubmit() {
    $.ajax({
        url: "/FeedBackDetails/updateFBApproval/",
        cache: false,
        type: "POST",
        async: false,
        data: $("#frmFBApproval").serialize(),
        success: function (data) {
            //alert(data.message);
            if (data.message == 1) {
                alert("Feedback Successfully - 'Accepted'");

                $("#tbFBApproval").load('/FeedbackDetails/FBApprovalDetails/' + $("#hdnFeedId").val(), function () {
                    $.validator.unobtrusive.parse($('#tbFBApproval'));

                    //    unblockPage();
                    //});
                    $('#tbFBApproval').show('slow');
                    $("#tbFBApproval").css('height', 'auto');
                    $('#tbFBDetailsJqGrid').trigger('reloadGrid');
                });
            }
            else if (data.message == 2) {
                alert("Feedback Successfully - 'Not Accepted'");

                $("#tbFBApproval").load('/FeedbackDetails/FBApprovalDetails/' + $("#hdnFeedId").val(), function () {
                    $.validator.unobtrusive.parse($('#tbFBApproval'));

                    //    unblockPage();
                    //});
                    $('#tbFBApproval').show('slow');
                    $("#tbFBApproval").css('height', 'auto');
                    $('#tbFBDetailsJqGrid').trigger('reloadGrid');
                })
            }
            else {
                alert("Error Occured on Feedback Acceptance");
            }
        },
        error: function () {
            alert("error");
        }
    })
}