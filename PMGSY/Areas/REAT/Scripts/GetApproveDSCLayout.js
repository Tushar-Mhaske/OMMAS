var admOfcrCode = 0;
var admndcode = 0;
var fileid = 0;

$("#ddlPIU").hide();
$("#rdDPIU").click(function () {

    $("#ddlPIU").show('slow');
    $("#ddlSRRDA").val("");
});

$("#rdSRRDA").click(function () {

    $("#ddlPIU").hide('slow');
    $("#ddlPIU").val("")
});


$("#ddlPIU").click(function () {
    $("#ddlSRRDA").val("");

});

$("#ddlSRRDA").click(function () {
    $("#ddlPIU").val("");

});



$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmREATApproveDSCLayout');


    $('#ddlstate').change(function () {
        $("#ddlagency").empty();
        $("#ddlPIU").empty();
        $("#ddlSRRDA").empty();
        $("#ddlPIU").append("<option value=" + 0 + ">" +
            "---Select PIU---" + "</option>");
        $("#ddlSRRDA").append("<option value=" + 0 + ">" +
            "---Select SRRDA---" + "</option>");
        $.ajax({
            url: '/PFMS1/PopulateAgencybyStateCode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlstate").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlagency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlagency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $('#ddlagency').change(function () {
        $("#ddlPIU").empty();
        $("#ddlSRRDA").empty();

        $("#ddlPIU").append("<option value=" + 0 + ">" +
            "---Select PIU---" + "</option>");
        $("#ddlSRRDA").append("<option value=" + 0 + ">" +
            "---Select SRRDA---" + "</option>");

        $.ajax({
            url: '/Reat/Reat/PopulatePIUbystatecode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlstate").val(), agencyCode: $("#ddlagency").val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlPIU").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlPIU").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
        loadsrrda();
        $("#ddlSRRDA").empty();
        $("#ddlPIU").empty();

    });
});


function loadsrrda() {

    $.ajax({
        url: '/Reat/Reat/PopulateSRRDA',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlstate").val(), agencyCode: $("#ddlagency").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                if (jsonData[i].Value == 2) {
                    $("#ddlSRRDA").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                else {
                    $("#ddlSRRDA").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            }
            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });
    //var val = jsonData[i].Value
    //alert(val);

    //alert($("#ddlSRRDA").val());


}

$('#btnViewDetails').click(function () {
    if (!$("#frmREATApproveDSCLayout").valid()) {
        return false;
    }
    //debugger;
    GetAuthoriseSignatoryDetails();
});


function GetAuthoriseSignatoryDetails() {
    if ($("#ddlPIU").val() > 0) {
        AdminNDCode = $("#ddlPIU").val();
        //alert($("#ddlPIU").val());
    }
    else {
        AdminNDCode = $("#ddlSRRDA").val();
        //alert($("#ddlSRRDA").val());
    }
    $.ajax({
        url: '/Reat/Reat/GetAuthoriseSignatoryDetails',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },

        data: { AdminNDCode: AdminNDCode },

        success: function (jsonData) {
            //$("#mainDiv").load("/REAT/REAT/GetAuthoriseSignatoryDetails/", function () {
            //    unblockPage();
            //});
            // debugger;
            if (jsonData['Authorised_Signatory_Name'] != null) {
                $("#ASName").html(jsonData['Authorised_Signatory_Name']);
                $("#DSCStatus").html(jsonData['ACK_DSC_STATUS'] == null ? "Awaited Response" : jsonData['ACK_DSC_STATUS']);
                $("#RejNar").html(jsonData['REJECTION_NARRATION'] == null ? "-" : jsonData['REJECTION_NARRATION']);

                if (jsonData['ACK_DSC_STATUS'] == "RJCT" && jsonData['REJECTION_CODE'] == "CDE0029") {
                    $("#btnApproveDSC").show();
                    $("#btnApproveDSC").attr('disabled', false);
                    admOfcrCode = jsonData['ADMIN_NO_OFFICER_CODE'];
                    admndcode = jsonData['AdminNDCode'];
                    fileid = jsonData['FileID'];
                }
                else {
                    $("#btnApproveDSC").hide();
                }
            }
            else {
                $("#ASName").html(jsonData['Authorised_Signatory_Name'] = " ");
                $("#DSCStatus").html(jsonData['ACK_DSC_STATUS'] = " ");
                $("#RejNar").html(jsonData['REJECTION_NARRATION'] = " ");
                $("#btnApproveDSC").hide();

                alert("Authorised Signatory not found!");
            }
            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

    $('#btnApproveDSC').unbind().click(function () {
        if (!$("#AuthoriszedSignatoryDetails").valid()) {
            return false;
        }
        GetDSCApprove();
    });

    function GetDSCApprove() {
        $.ajax({
            url: '/Reat/Reat/GetApproveDSC',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { AdminOfficerCode: admOfcrCode, ADMINNDCODE: admndcode, FileID: fileid },
            success: function (result) {
                alert(result);
                GetAuthoriseSignatoryDetails();
                $("#btnApproveDSC").attr('disabled', true);
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    }






}