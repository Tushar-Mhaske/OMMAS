$(document).ready(function () {
    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateBroadCastNotification'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });   
 

    $('#btnBSave').click(function (e) {
        //  e.preventDefault();

        if ($('#frmCreateBroadCastNotification').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/QualityMonitoringHelpDesk/SaveQMBroadCastMessageNotification",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateBroadCastNotification").serialize(),
                success: function (data) {

                    //  $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearBrodCastDetails();
                        // $('#tblNotificationDetails').trigger('reloadGrid');
                        LoadBroadCastNotificationDetails();
                        $('#dvErrorMessage').hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#dvErrorMessage').show('slow');
                            $('#spnErrMessage').html(data.message);
                        }

                    }
                    else {
                        $("#divbrodcastNotificationForm").html(data);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }

    });

    $('#btnBUpdate').click(function (e) {
        //  e.preventDefault();

        if ($('#frmCreateBroadCastNotification').valid()) {
            if ($("#ddlStateB").is(":visible")) {
                $('#ddlStateB').attr("disabled", false);
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/QualityMonitoringHelpDesk/UpdateQMBroadCastMessageNotification",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateBroadCastNotification").serialize(),
                success: function (data) {

                    //  $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearBrodCastDetails();
                        // $('#tblNotificationDetails').trigger('reloadGrid');
                        LoadBroadCastNotificationDetails();
                        $('#btnBroadCastNotificationAdd').trigger('click');
                        $('#dvErrorMessage').hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            $('#dvErrorMessage').show('slow');
                            $('#spnErrMessage').html(data.message);

                        }

                    }
                    else {
                        $("#divbrodcastNotificationForm").html(data);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }

    });

    $('#btnBCancel').click(function (e) {

        $.ajax({
            url: '/QualityMonitoringHelpDesk/CreateQMBroadCastMessageNotification/' + $("#qmTypeBroadNotList").val(),
            type: "GET",
            dataType: "html",
            success: function (data) {

                $("#divbrodcastNotificationForm").html(data);
                $("#accordionBroadcast h3").html(
                        "<a href='#' style= 'font-size:.9em;' >Add BroadCast Notification Details</a>" +
                        '<a href="#" style="float: right;">' +
                        '<img  class="ui-icon ui-icon-closethick" onclick="CloseBroadCastNotificationDetails();" /></a>'
                        );
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });

    $('#btnBReset').click(function () {
        // ClearBrodCastDetails();
        $('#dvErrorMessage').hide('slow');
    });
});

function ClearBrodCastDetails() {
   
    $('#ddlStateB').val(0);
    $('#txtMessageDescB').val('');
    $('#spnErrMessage').html('');  
    if ($("#ddlStateB").is(":visible")) {
        $('#ddlStateB').attr("disabled", false);
    }
}
