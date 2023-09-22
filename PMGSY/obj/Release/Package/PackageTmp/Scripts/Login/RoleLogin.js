$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmLoginRole'));

    
    if ($("#RoleId option").length == 1) {
        //blockPage();
        //$('#btnGoLogin').trigger('click', function () { unblockPage(); });
    }

    $('#btnGoLogin').click(function () {
        
        if ($("#DefaultRoleId").val() == "") {
            alert("Please select Role");
            return false;
        }

        blockPage();
        $("#dialog-form-login").load('/Login/SwitchRole', function () {
            $("#dialog-form").dialog("open");
            unblockPage();
        });

    });


    
 
});