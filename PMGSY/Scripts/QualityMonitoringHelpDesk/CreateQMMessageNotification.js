$(document).ready(function () {
    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateNotification'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    if ($("#qmTypeNotList").val() === "S") {
        $("#ddlMonitor").hide();
        $('#lblMonitor').hide();
        $('#lblStarMonitor').hide();
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');
    }
    else {
        $("#ddlMonitor").show();
        $('#lblMonitor').show();
        $('#lblStarMonitor').show();
    }

    $('#lblStarMonitor').hide();
    $('#ddlState').change(function () {
        if ($('#ddlState').val() > 0) {

            loadMonitorList();
        }
        else {
            $("input,select").removeClass("input-validation-error");
            $('.field-validation-error').html('');
            $("#ddlMonitor").val(0);
            $("#ddlMonitor").empty();
            $("#ddlMonitor").hide();
            $('#lblMonitor').hide();
            $('#lblStarMonitor').hide();

        }
    });
    if ($('#Message_Id').val() == 0) {
        if ($('#qmTypeStateList').val() > 0) {
            $('#ddlState').val($('#qmTypeStateList').val());
            $('#ddlState').trigger('change');
        }
    }
    $('#btnSave').click(function (e) {
        //  e.preventDefault();

        if ($('#frmCreateNotification').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/QualityMonitoringHelpDesk/SaveQMMessageNotification",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateNotification").serialize(),
                success: function (data) {

                    //  $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        $('#tblNotificationDetails').trigger('reloadGrid');
                        $('#dvErrorMessage').hide();
                    }
                    else if (data.success == false) {

                        if (data.message != "") {

                            $('#dvErrorMessage').show('slow');
                            $('#spnErrMessage').html(data.message);

                        }

                    }
                    else {
                        $("#divNotificationForm").html(data);
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

    $('#btnUpdate').click(function (e) {
        //  e.preventDefault();

        if ($('#frmCreateNotification').valid()) {
            $("#ddlMonitor").attr("disabled", false);
            $("#ddlMessageType").attr("disabled", false);
            if ($("#ddlState").is(":visible")) {
                $('#ddlState').attr("disabled", false);
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/QualityMonitoringHelpDesk/UpdateQMMessageNotification",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateNotification").serialize(),
                success: function (data) {

                    //  $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        $('#tblNotificationDetails').trigger('reloadGrid');
                        $('#btnNotificationAdd').trigger('click');
                        $('#dvErrorMessage').hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            $('#dvErrorMessage').show('slow');
                            $('#spnErrMessage').html(data.message);

                        }

                    }
                    else {
                        $("#divNotificationForm").html(data);
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

    $('#btnCancel').click(function (e) {

        $.ajax({
            url: '/QualityMonitoringHelpDesk/CreateQMMessageNotification/' + $("#qmTypeNotList").val(),
            type: "GET",
            dataType: "html",
            success: function (data) {

                $("#divNotificationForm").html(data);
                $("#ddlMonitor").attr("disabled", false);
                $("#accordion h3").html(
                        "<a href='#' style= 'font-size:.9em;' >Add Notification Details</a>" +
                        '<a href="#" style="float: right;">' +
                        '<img  class="ui-icon ui-icon-closethick" onclick="CloseNotificationDetails();" /></a>'
                        );
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });



    $('#btnReset').click(function () {

        ClearDetails(); ///it will automatically reset all field if  button type is "Reset"
        $('#dvErrorMessage').hide('slow');

    });
});
function loadMonitorList() {

    $("#ddlMonitor").show();
    $('#lblMonitor').show();
    $('#lblStarMonitor').show();
    $("#ddlMonitor").val(0);
    $("#ddlMonitor").empty();
    //  $('#ddlBlocks').append("<option value=0>Select Monitor</option>");
    var stateCode = $("#ddlState").val();
    if (stateCode === undefined) {
        stateCode = 0;
    }
    $.ajax({
        url: '/QualityMonitoringHelpDesk/PopulateMonitorsDetails?qm=' + $("#qmTypeNotList").val() + '&' + 'stateCode=' + stateCode + '&' + 'messageType=' + $("#ddlMessageType").val(),
        type: 'POST',
        // data: { "Type": $("#ddStaPta_PropScruitinyDetails").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlMonitor").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
function ClearDetails() {
    $('#ddlMessageType').val(0);

    $('#txtMessageDesc').val('');


    //if ($("#qmTypeNotList").val() === "S") {
    //    $("#ddlMonitor").hide();
    //    $('#lblMonitor').hide();
    //    $('#lblStarMonitor').hide();
    //    $('#ddlMonitor').empty();
    //}
    //else {
    //    $("#ddlMonitor").show();
    //    $('#lblMonitor').show();
    //    $('#lblStarMonitor').show();
    //    $('#ddlMonitor').val(0);
    //    $("#qmTypeStateList").val(0);
    //}


    // $('#ddlMonitor').append("<option value=0>Select Monitors<option>"); //old
    $("#ddlMessageType").attr("disabled", false);
    $('#dvErrorMessage').hide('slow');
    $('#spnErrMessage').html('');
    // $("#ddlMonitor").attr("disabled", false);
    if ($("#ddlState").is(":visible")) {
        $('#ddlState').attr("disabled", false);
    }

    if ($('#qmTypeStateList').val() > 0) {
        $('#ddlState').val($('#qmTypeStateList').val());
        $('#ddlState').trigger('change');
    }
    //else {
    //    $('#ddlState').val(0);
    //    $("#ddlMonitor").hide();
    //    $('#lblMonitor').hide();
    //    $('#lblStarMonitor').hide();
    //    $('#ddlMonitor').empty();
    //    $("input,select").removeClass("input-validation-error");
    //    $('.field-validation-error').html('');
    //}
    if ($("#qmTypeNotList").val() === "I") {
        $("#ddlMonitor").show();
        $('#lblMonitor').show();
        $('#lblStarMonitor').show();
        $('#ddlMonitor').val(0);
        $("#qmTypeStateList").val(0);

    }
}
