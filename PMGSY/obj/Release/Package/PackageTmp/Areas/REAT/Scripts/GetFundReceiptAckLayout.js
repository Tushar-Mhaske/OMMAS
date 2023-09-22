$(document).ready(function () {


    $('#fundReceiptAckFile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnSaveFundReceiptAckXML").click(function () {
        SaveFile();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#fundReceiptAckFile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#fundReceiptAckFile").val().split('.').pop();
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#fundReceiptAckFile").val('');
        return false;
    }
    return true;
}

function SaveFile() {
    if (checkFilevalidation()) {
        var formdata = new FormData(document.getElementById("frmFundReceiptAck"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/Reat/Reat/SaveFundReceiptAck',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                alert(jsonData.message);
                $("#fundReceiptAckFile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#fundReceiptAckFile").val('');
                $.unblockUI();
            }
        });
    }

}