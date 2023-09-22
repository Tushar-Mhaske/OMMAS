$(document).ready(function () {


    $('#DSCFile').on('change', function () {
        checkFilevalidation();
    });

    $("#btnVerifyDSCFile").click(function () {
        CheckFileForTempering();
    });


});


function checkFilevalidation() {
    var xmlfile = $("#DSCFile").val();

    if (xmlfile = "" || xmlfile == undefined || xmlfile.length == 0) {
        alert("Please select file");
        return false;
    }

    var ext = $("#DSCFile").val().split('.').pop();
    if (ext.toLowerCase() != "xml") {
        alert("only xml file is allowed.");
        $("#DSCFile").val('');
        return false;
    }
    return true;
}

function CheckFileForTempering() {
    if (checkFilevalidation()) {
        var formdata = new FormData(document.getElementById("frmDSCFileVerify"));
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/Reat/Reat/DSCTemperVerification',
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                alert(jsonData.message);
                $("#DSCFile").val('');
                $.unblockUI();
            },
            error: function (err) {
                alert("Error occureed while processing your request.");
                $("#DSCFile").val('');
                $.unblockUI();
            }
        });
    }

}