/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {


    $('#bankpaymentackackfile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveBankAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#bankpaymentackackfile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }
   
    var ext = $("#bankpaymentackackfile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#bankpaymentackackfile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation())// 
    {
        var formdata = new FormData(document.getElementById("frmBankAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/PFMS1/SaveBankAcknowledgementXmlData',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // if (jsonData.success)
                alert(jsonData.message);
                $("#bankpaymentackackfile").val('');
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