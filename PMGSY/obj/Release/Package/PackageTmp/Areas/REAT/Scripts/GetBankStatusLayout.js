/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {


    $('#bankpaymentackfile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveBankPaymentAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#bankpaymentackfile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#bankpaymentackfile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#bankpaymentackfile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation()) {
        var formdata = new FormData(document.getElementById("frmBankPaymentAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/Reat/Reat/SaveBankPaymentStatusXmlData',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // if (jsonData.success)
                alert(jsonData.message);
                $("#bankpaymentackfile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#bankpaymentackfile").val('');
                $.unblockUI();
            }
        });
    }

}