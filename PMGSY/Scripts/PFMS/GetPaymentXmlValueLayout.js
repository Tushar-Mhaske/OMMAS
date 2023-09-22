/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {

    
    $('#paymentackfile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation()
{
    var xmlfile = $("#paymentackfile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length==0) {
        alert("Please select file");
        return false;
    }
   // console.log();
    var ext = $("#paymentackfile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#paymentackfile").val('');
        return false;
    }
    //var fileSizeKb = $(this)[0].files[0].size; // file size in Kb
    return true;
}

function SaveFile()
{
    if (checkFilevalidation())// 
    {
        var formdata = new FormData(document.getElementById("frmReadFile"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/PFMS1/SavePaymentAcknowledgementXmlData',
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