$(document).ready(function () {
    var status;
    $("#btnSendEmail").show();
    CheckSendMailStatus(EncryptedEauthID)
 
    //Send Mail button Click
    $("#btnSendEmail").click(function () {
        if (confirm("Are you sure you want to Send e-Authorization Mail ?")) {
            blockPage();
            var token = $('input[name=__RequestVerificationToken]').val();
            //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/EAuthorization/SendEAuthorizationMail/',
                type: 'POST',
                catche: false,
                data: { "EncryptedEAuthID": EncryptedEauthID.toString(), "__RequestVerificationToken": token },
                async: false,
                success: function (response) {
                    unblockPage();
                    if (response.success) {
                        alert("Email Sent SuccessFully");

                        //Hide Dialog after Sending Mail
                        var firstParent = $("#divEAuth").parent().attr('id');
                        var dialogID = $("#" + firstParent).parent().attr('id');
                        $('#tblEAuthSRRDARequestGrid1').trigger('reloadGrid');
                        $('#tblEAuthSRRDARequestGrid').trigger('reloadGrid');
                        $("#dvDetailGrid").hide();

                        $("#btnSendEmail").hide();
                        $('#' + dialogID).dialog('close');

                    }
                    else {
                        alert("Error in sending Email,Please try Again");
                    }


                },
                error: function () {
                    unblockPage();
                    alert("An Error Occur while Processing,please try Again");
                    return false;
                },
            });

        }

    });

});

//Method is Used to Hide/Show based on SendEmail status
function CheckSendMailStatus(EncryptedEauthID) {

    
        $.ajax({
            url: '/EAuthorization/CheckSendMailStatus/',
            type: 'POST',
            catche: false,
            data: { "EncryptedEAuthID": EncryptedEauthID.toString() },
            async: false,
            success: function (data) {
                
                if (data.Success) {
                    
                    if (data.status == "A") {

                        $("#btnSendEmail").hide();
                       
                    }

                    else if (data.status == "Y") {
                        
                        $("#btnSendEmail").show();
                       
                    }
                    else if (data.status == "P") {

                        $("#btnSendEmail").hide();

                    }


                    else if (data.status == "R") {
                        $("#btnSendEmail").hide();

                    }

                    else {

                        $("#btnSendEmail").hide();

                    }

                } else {
                    alert("Error Occur while Processing ,Please try Again ");
                    return;
                }
            },
            error: function () {
                //$.unblockUI();
                alert("An Error Occur while Processing,please try Again");
                return false;
            },
        });

    

}