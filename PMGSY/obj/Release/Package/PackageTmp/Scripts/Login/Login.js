$(document).ready(function () {

    //$.ajaxSetup({ cache: false });

    $.validator.unobtrusive.parse($('loginFrm'));
    $.validator.unobtrusive.parse($('frmRecoverPassword'));
    $.validator.unobtrusive.parse($('frmChangePassword'));

    $("#UserName").focus();

    $("#Captcha").removeClass("pmgsy-textbox").addClass("border-radius-3 captcha-input");
    if (typeof history.pushState === "function") {
        history.pushState("jibberish", null, null);
        window.onpopstate = function () {
            history.pushState('newjibberish', null, null);
            // Handle the back (or forward) buttons here
            // Will NOT handle refresh, use onbeforeunload for this.
            //alert("Back");
        };
    }
    else {
        var ignoreHashChange = true;
        window.onhashchange = function () {
            if (!ignoreHashChange) {
                ignoreHashChange = true;
                window.location.hash = Math.random();
                // Detect and redirect change here
                // Works in older FF and IE9
                // * it does mess with your hash symbol (anchor?) pound sign
                // delimiter on the end of the URL
            }
            else {
                ignoreHashChange = false;
            }
        };
    }


    //Added By Abhishek kamble For Captch 24-Apr-2014 start
    $('.newCaptcha').click(function () {

        $.ajax({
            url: '/Login/GetCaptchaImages',
            type: 'POST',
            cache: false,
            error: function (xhr, status, error) {
                alert('Request can not be processed at this time,please try after some time!!!');
                return false;
            },
        });

    });

    $(function () {
        if ($("#IsShowCaptch").val() == "True") {
            $("#trCapcha").show();
        }
        else {
            $("#trCapcha").hide();
        }
    });

    //Commented By Abhishek kamble 26-Apr-2014 Captch Validation Code on Blur event
    //$("#UserName").blur(function () {
    //    //alert($("#UserName").val());
    //    $("#ValidateCaptcha").val(false);//Comment Later
    //    //if ($("#UserName").val() != '') {
    //    //    blockPage();

    //    //    $.ajax({
    //    //        url: "/Login/UserLoginAttemptStatus?UserName=" + $("#UserName").val(),
    //    //        type: "POST",
    //    //        cache: false,

    //    //        error: function (xhr, status, error) {
    //    //            unblockPage();

    //    //            alert("Request can not be processed at this time,please try after some time!!!");
    //    //            return false;
    //    //        },
    //    //        success: function (response) {
    //    //            unblockPage();

    //    //            if (response.ShowCaptch == true) {                        
    //    //                $("#trCapcha").show();
    //    //                $("#ValidateCaptcha").val(true);                       

    //    //            } else {                        
    //    //                $("#trCapcha").hide();
    //    //                $("#ValidateCaptcha").val(false);
    //    //            }
    //    //        }
    //    //    });
    //    //}
    //});

    
    $("#UserName").blur(function () {
        if ((($("#UserName").val()) == ($("#HiddenUserName").val())) && ($("#IsShowCaptch").val() == "True")) {
            $("#trCapcha").show();
            $("#ValidateCaptcha").val(true);
            $("#IsValidCaptcha").val(true);
        }
        else {
            $("#btnResetLogin").trigger("click");
        }

    });

    //Added By Abhishek kamble For Captch 24-Apr-2014 end


    //Reset Code Added By Abhishek kamble to reset capthca
    $('#btnResetLogin').click(function () {
        $("#trCapcha").hide();
        $("#ValidateCaptcha").val(false);
        $("#IsValidCaptcha").val(false);
        return false;
    });


    //$('#btnResetLogin').click(function () {
    //    $('#username').val('');
    //    $('#password').val('');

    //    $('.field-validation-error')
    //     .removeClass('field-validation-error')
    //     .addClass('field-validation-valid');

    //    $('.input-validation-error')
    //        .removeClass('input-validation-error')
    //        .addClass('valid');

    //    return false;
    //});


    //On Enter -- Default is form submit, so call to validate function
    //If Focus is on btnSubmitLogin, then unbind it, to avoid 2 calls
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            if (document.activeElement.id == "btnSubmitLogin") {
                $('#btnSubmitLogin').unbind('click');
            }
            validate();
        }
    });

    $('#btnSubmitLogin').click(function () {
        validate();
    });


    $('#btnSubmitRecoverPwdr').click(function () {

        if (validateChangePassword()) {
            if ($('#frmRecoverPassword').valid()) {
                $.ajax({
                    url: "/Login/RecoverPassword",
                    type: "POST",
                    data: $('#frmRecoverPassword').serialize(),
                    cache: false,
                    error: function (xhr, status, error) {

                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {
                        if (response.Success) {
                            alert("Password changed successfully.");
                            window.location.replace("/Login/Login");
                        }
                        else {
                            $("#divRecoverPassError").show("slow");
                            $("#divRecoverPassError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);

                            $("#oldpassword").focus();
                            $("#newpassword").val('');
                            $("#confirmpassword").val('');
                        }
                    }
                });
            }
        }

    });

    //changed by PP
    $("#btnResetChangePwdr").click(function () {
        $("#txtPwdrAnswer").prop("type", "text");
    });

    $('#btnSubmitChangePwdr').click(function () {

        if (validateChangePassword()) {
            if ($('#frmChangePassword').valid()) {

                //changed by PP
                var answer = $('#txtPwdrAnswer').val().trim();
                if ($('#txtPwdrAnswer').val() != undefined) {
                    var encAnswer = getEnryptedAnswer($('#txtPwdrAnswer').val().trim());
                    $('#txtPwdrAnswer').val('');
                    $("#txtPwdrAnswer").prop("type", "password");
                    $('#txtPwdrAnswer').val(encAnswer);
                }
                //end

                $.ajax({
                    url: "/Login/ChangePassword",
                    type: "POST",
                    data: $('#frmChangePassword').serialize(),
                    cache: false,
                    error: function (xhr, status, error) {

                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {
                        if (response.Success) {
                            alert("Password changed successfully.\nYou need to login again");
                            window.location.replace("/Login/Login");
                        }
                        else {
                            $("#divRecoverPassError").show("slow");
                            $("#divRecoverPassError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            $("#oldpassword").focus();
                            $("#oldpassword").val('');
                            $("#newpassword").val('');
                            $("#confirmpassword").val('');


                            //added by PP
                            $('#txtPwdrAnswer').val('');
                            $("#txtPwdrAnswer").prop("type", "text");
                            $('#txtPwdrAnswer').val(answer);
                            //

                        }
                    }
                });
            }
        }
    });


    // Validate User Name & Password
    function validate() {
        var browserMessage = 'Application best viewed in Firefox version 18.0 or Higher,\nGoogle Chrome version 20.0 or Higher & Internet Explorer version 9.0 or Higher in 1280 x 1024 screen resolution. Please upgrade your browser.';
        ;
        //If Browser is IE8/IE7/IE6 then Display message & Keep on same Page
        if ($.browser.msie && (parseInt($.browser.version, 10) === 8)) {
            alert("You are using Internet Explorer 8.\n" + browserMessage);
            $('#UserName').val('');
            $('#Password').val('');
            $('#UserName').focus();
            return false;
        }
        if ($.browser.msie && (parseInt($.browser.version, 10) === 7)) {
            alert("You are using Internet Explorer 7.\n" + browserMessage);
            $('#UserName').val('');
            $('#Password').val('');
            $('#UserName').focus();
            return false;
        }
        if ($.browser.msie && (parseInt($.browser.version, 10) === 6)) {
            alert("You are using Internet Explorer 6.\n" + browserMessage);
            $('#UserName').val('');
            $('#Password').val('');
            $('#UserName').focus();
            return false;
        }

        if ($.trim($('#Password').val()) != '') {

          //  alert("Password Enc")
            $.ajax({
                type: "POST",
                url: "/Login/GetSessionSalt/",
                async: false,
                beforeSend: function () {
                    blockPage();
                },
                data: { value: Math.random() },
                success: function (Data) {
                    blockPage();

                    var salt = Data.toString().replace(new RegExp('"', 'g'), '');
                    var singleEncryptedPWD = hex_md5($('#Password').val());
                    var doubleEncryptedPWD = hex_md5(singleEncryptedPWD.toUpperCase() + salt);

                    $('#Password').val('');
                    $('#Password').val(doubleEncryptedPWD);
                 //   alert("P= " + doubleEncryptedPWD)
                    //Added PBy Abhishek kamble 25-Apr-2014
                    $("#ValidateCaptcha").val($("#IsValidCaptcha").val());

                    //$('#btnPostLoginFrm').trigger('click');
                    $('#loginFrm').submit();

                    unblockPage();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    unblockPage();
                }
            });
        }

    }//validate() ends here


    // Validate old & new Password
    function validateChangePassword() {
        var singleEncryptedPWD = "";
        var doubleEncryptedPWD = "";

        //This checks if a string has atleast 8 length, has one special symbol( from @ # $ % ^ & + = ) or number, 
        //and has Atleast one lowerCase and one Uppercase character 
        var pattern = "^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=1234567890]).*$";

        if (!($('#newpassword').val().match(pattern))) {
            $("#divRecoverPassError").show("slow");
            $("#divRecoverPassError span:eq(1)").html('<strong> Password should contain at least 1 uppercase letter, 1 lowercase letter, 1 number or special character and 8 characters in length. </strong>');
            return false;
        }
        else {
            $("#divRecoverPassError span:eq(1)").html('');
            $("#divRecoverPassError").hide("slow");


            if ($('#oldpassword').val() != undefined) {


                singleEncryptedPWD = hex_md5($('#oldpassword').val());
                $('#oldpassword').val('');
                $('#oldpassword').val(singleEncryptedPWD);



            }

            if ($('#newpassword').val() != '') {


                //singleEncryptedPWD = hex_md5($('#newpassword').val());
                //$('#newpassword').val('');
                //$('#newpassword').val(singleEncryptedPWD);


                // Newly added on 06 Aug 2020
                singleEncryptedPWD = hex_md5($('#newpassword').val());
                var FinalNewPassword = singleEncryptedPWD.split("").reverse().join("");
                $('#newpassword').val('');
                $('#newpassword').val(FinalNewPassword);
               
                
            }

            if ($('#confirmpassword').val() != '') {


                //singleEncryptedPWD = hex_md5($('#confirmpassword').val());
                //$('#confirmpassword').val('');
                //$('#confirmpassword').val(singleEncryptedPWD);


                // Newly added on 06 Aug 2020
                singleEncryptedPWD = hex_md5($('#confirmpassword').val());
                var FinalConfirmPassword = singleEncryptedPWD.split("").reverse().join("");
              //  alert("Single " + singleEncryptedPWD + " Altered " + FinalConfirmPassword)

                $('#confirmpassword').val('');
                $('#confirmpassword').val(FinalConfirmPassword);
            }

            return true;

        }




    }//validateChangePassword() ends here


    $("#btnCancelChangePwdr").click(function () {
        window.location.replace("/Login/Login");
    });

    $("#btnCancelRecoverPwdr").click(function () {
        window.location.replace("/Login/Login");
    });

    $("#btnBackChangePwdr").click(function () {
        //history.go(-1);
        window.location.href = "/Login/Login";

    });

    $("#oldpassword").focus(function () {
        //Commented By Abhishek kamble 25-Apr-2014 to show error msg
        //$("#divRecoverPassError span:eq(1)").html('');
        //$("#divRecoverPassError").hide("slow");
    });

    $("#newpassword").focus(function () {
        $("#divRecoverPassError span:eq(1)").html('');
        $("#divRecoverPassError").hide("slow");
    });

    $("#confirmpassword").focus(function () {
        $("#divRecoverPassError span:eq(1)").html('');
        $("#divRecoverPassError").hide("slow");
    });

    //added by PP

    $("#btnSubmit1").click(function (event) {
        //event.preventDefault();
        var encAnswer = getEnryptedAnswer($("#tableMain #txtPwdrAnswer").val())
        $("#tableMain #txtPwdrAnswer").val(encAnswer);
        $("#frmChangePwd").submit();
    });

}); //doc.ready() ends here



//added by PP

function getEnryptedAnswer(answer) {
    var key = CryptoJS.enc.Utf8.parse('7061737323313233');
    var iv = CryptoJS.enc.Utf8.parse('7061737323313233');
    var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(answer), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

    var decrypted = CryptoJS.AES.decrypt(encrypted, key, {
        keySize: 128 / 8,
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    //alert('Encrypted :' + encrypted);
    //alert('Key :' + encrypted.key);
    //alert('Salt :' + encrypted.salt);
    //alert('iv :' + encrypted.iv);
    //alert('Decrypted : ' + decrypted);
    //alert('utf8 = ' + decrypted.toString(CryptoJS.enc.Utf8));
    return encrypted;
}

//end