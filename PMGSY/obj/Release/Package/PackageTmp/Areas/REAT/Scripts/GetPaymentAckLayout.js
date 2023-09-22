/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {


    $('#paymentackfile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSavePaymentAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#paymentackfile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#paymentackfile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#paymentackfile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation()) 
    {
        var formdata = new FormData(document.getElementById("frmPaymentAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/Reat/Reat/SavePaymentAcknowledgementXmlData',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // if (jsonData.success)
                alert(jsonData.message);
                $("#paymentackfile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#paymentackfile").val('');
                $.unblockUI();
            }
        });
    }

}