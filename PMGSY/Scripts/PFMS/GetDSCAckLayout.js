/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {


    $('#dscackfile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveDSCAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#dscackfile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#dscackfile").val().split('.').pop();
    //  alert("File extension :" + ext.toLowerCase());
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#dscackfile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation())// 
    {
        var formdata = new FormData(document.getElementById("frmDSCAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/PFMS1/SaveDSCAcknowledgementXmlData',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // if (jsonData.success)
                alert(jsonData.message);
                $("#dscackfile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#dscackfile").val('');
                $.unblockUI();
            }
        });
    }

}