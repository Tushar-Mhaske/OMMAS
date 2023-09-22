$(document).ready(function () {
    
    $("#btnCancelLogin").click(function () {
        $("#dialog-form").dialog("close");
    });

    $("#DefaultRoleId").val($("#RoleId").val());

    $("#dialog-form").dialog({
        autoOpen: false,
        height: 'auto',
        width: 300,
        modal: true
    });


    $("#btnSubmitLogin").click(function () {
        //alert($.trim($('#password').val().length));
        if ($.trim($('#password').val().length) == 0)
        {
            $("#showError").html("Please enter password");
            $("#showError").addClass("field-validation-error");
            alert("Please enter password");
            return false;
        }

        $("#showError").html("");
        $("#showError").removeClass("field-validation-error");

        validate();
    });

});



// Validate User Name & Password
function validate() {
    if ($('#password').val() != '') {
        
        var singleEncryptedPWD = hex_md5($('#password').val());
        //alert(singleEncryptedPWD);
        //alert($('#sessionSalt').val());
        
        var doubleEncryptedPWD = hex_md5(singleEncryptedPWD.toUpperCase() + $('#sessionSalt').val());
        //alert(doubleEncryptedPWD);
        $('#password').val('');
        $('#password').val(doubleEncryptedPWD);
        return true;
    }

}//validate() ends here