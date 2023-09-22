$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmGenerateSRRDAPdfKeyDetails'));

    $("#btnSubmitDetails").click(function () {
        if ($("#frmGenerateSRRDAPdfKeyDetails").valid()) {
            $.ajax({
                url: '/Bank/SaveSRRDAPdfKeyDetails/',
                async: false,
                catche: false,
                method: 'POST',
                data: $("#frmGenerateSRRDAPdfKeyDetails").serialize(),
                success: function (response) {
                    if (response.success === undefined) {
                        $("#mainDiv").html(response);
                    }
                    if (response.success == true) {
                        alert(response.message);
                    }
                    else if (response.success == false) {                        
                        $("#divSrrdaPdfKeyError").show("slow");
                        $("#spnError").html(response.message);
                        return false;
                    }
                },
                error: function (xhr, status, code) {
                    alert("An error occured while processing your request.");
                    return false;
                }
            });
        }
    });
});
