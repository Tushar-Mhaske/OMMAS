/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {


    $('#openingBalanceAckFile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveOBAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#openingBalanceAckFile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#openingBalanceAckFile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#openingBalanceAckFile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation()) {
        var formdata = new FormData(document.getElementById("frmOpeningBalanceAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/Reat/Reat/SaveOpeningBalanceXmlData',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // if (jsonData.success)
                alert(jsonData.message);
                $("#openingBalanceAckFile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#openingBalanceAckFile").val('');
                $.unblockUI();
            }
        });
    }

}