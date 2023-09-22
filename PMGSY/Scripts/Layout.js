
//$.ajaxSetup({ cache: false });

var isAjaxCall = true;
var count = 0;
$(document).ready(function () {
    
    //Added by Pradip patil 23/03/2017  start

    $.ajax({
        url: '/Agreement/GetExpiredBankGuaranteeCount',
        type: 'POST',
        async: false,
        cache: false,
        success: function (jsonData) {
            if (jsonData.count > 0) {
                count = jsonData.count;
            }
        },
        error: function () {
        }
    });


    //Added by Pradip patil 31/03/2017  
    $('body').click(function (evt) {

        if (!$(evt.target).is('#bgnmessage') || $('#animate').width() != 30) {
            if ($('#animate').hasClass('expand')) {
                $('#animate').removeClass('expand');
                $('#animate').css('width', '30px');
                $('#animate').css('transition', 'width 0.5s ease-in-out');
                $('#notice').hide('slow');
            }
        }
    });

    $('#bgnmessage').click(function () {
        //  alert(count);
        $('#animate').addClass('expand');
        $('#animate').css('width', '350px');
        $('#animate').css('transition', 'width 0.5s ease-in-out');
        if (count > 0) {
            $('#notice').show();
        }
    });

    // Added by Pradip patil end here

    //$("#logout").click(function () {
    //    alert("Clicked");
    //    var backlen = history.length;
    //    history.go(-backlen);
    //    window.location.href = "/Login/logout";
    //});

    //Added By Abhishek kamble 6-May-2014 start
    $(function () {
        $.ajax({
            type: 'POST',
            url: '/Master/GetPMGSY2Status?id=' + $("#PMGSY2StateCode").val(),
            async: false,
            cache: false,
            success: function (data) {
                //alert(data.success);
                if (data.success == true) {
                    $("#spnPMGSY2").show();
                }
                else {
                    $("#spnPMGSY2").hide();
                }
            },
            error: function () {                
                alert("Request can not be processed at this time.");
            }
        });

    });
    //Added By Abhishek kamble 6-May-2014 end


    if (typeof history.pushState === "function") {
        history.pushState("jibberish", null, null);
        window.onpopstate = function () {
            history.pushState('newjibberish', null, null);
            // Handle the back (or forward) buttons here
            // Will NOT handle refresh, use onbeforeunload for this.
       
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

   

    //set selected fund type if exists
    if ($("#ddlFundChange") != null) {

        $("#ddlFundChange").val($("#fundType").val())
    }

    
    $("input[name=rdoScheme]:radio").change(function () {
       
        if ($("#rdoScheme1").attr("checked")) {
            setScheme(1);
        }
        else if ($("#rdoScheme2").attr("checked")) {
            setScheme(2);
        }
        else if ($("#rdoScheme3").attr("checked")) {
            setScheme(3);
        }
        else if ($("#rdoScheme4").attr("checked")) {
            setScheme(4);
        }
        else if ($("#rdoScheme5").attr("checked")) {
            setScheme(5);
        }
    });


    ///event to change the fund type
    $("#ddlFundChange").change(function () {
        blockPage();
        $.ajax({
            url: "/home/ChangeFundType/" + $(this).val(),
            type: "POST",
            cache: false,
            //async: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Status === "1") {
                    //location.reload();
                    window.location.replace("/Home/Index/" + response.encryptedurl)

                } else if (response.Status == "0") {
                    alert("invalid input");
                    return false;
                }
            }
        });

    });


    $("#switcher").themeswitcher({
        imgpath: "/Content/images/",
        loadTheme: "Humanity"
    });


   
    $("#UserLogin").qtip(
    {
        events: {
            show: function (event, api) {
                // $('#qtip-3-content').removeClass();
                $('.jquery-ui-switcher-link').css({
                    'position': ''
                });
            }
        },
        content: {
            // Set the text to an image HTML string with the correct src URL to the loading image you want to use
            text: $('#userProfile'),
            ajax: {
                //url: $(this).attr('rel') // Use the rel attribute of each element for the url to load
            },
            title: {
                // text: 'Settings', // Give the tooltip a title using each elements text
                // button: false
            }
        },
        position: {
            at: 'bottom center', // Position the tooltip above the link
            viewport: $(window), // Keep the tooltip on-screen at all times
            effect: false, // Disable positioning animation
            target: $('#UserLogin')
            //adjust: { y: 10, x: 10 }
        },
        show: {
            event: 'click',
            solo: true // Only show one tooltip at a time
        },
        hide: 'unfocus',
        style: {
            //width: 380,
            classes: 'qtip-wiki qtip-light',
            widget: true,
        }
    })
// Make sure it doesn't follow the link when we click it
.click(function (event) { event.preventDefault(); });

    

    $('#UserLogin').qtip('show');

    $('#UserLogin').qtip('hide');


    $(".menubar-icons").menubar({
        autoExpand: true,
        menuIcon: true,
        buttons: true,
        position: {
            within: $("#demo-frame").add(window).first()
        }

    });


    $("#bar2").show();


    //check for invalid session
    $(document).ajaxComplete(function (event, request, settings) {

        IsValidSession(request.responseText);

     });


    // ----------------------------------------------------------------------------------------------
    //event for back button

    /* $.address.externalChange(function (event) {     //Catching URL change in `event`
 
         Alert(event.value);
 
         if (isAjaxCall == false) {
 
             $.ajax({
                 url: event.value,
                 cache: false,
                 success: function (response) {
                     $("#mainDiv").html(response);    //loading page in div using AJAX
                 }
             });
         }
     });
     */

    //----------------------------------------------------------------------------------------------


});



/// Checks for invalid session [JQUERY]  
function IsValidSession(response_data) {
    if (response_data.match(/id="ajaxexpiry"/gi) != null)  // pattern to locate
    {
        window.location.replace("/Login/SessionExpire");
    }
    if (response_data.match(/id="ajaxErrorPage"/gi) != null)  // pattern to locate
    {
        window.location.replace("/Login/Error");
    }
    else if (response_data.match(/id="unauthorised"/gi) != null) {

        $("#mainDiv").html(response_data);

    }
}



//function to load the page in maindiv
function LoadPage(urlParam) {

    isAjaxCall = true;

    var arrparam = urlParam.split('$');
    url = arrparam[0];
    moduleName = arrparam[1];

    //get the data from server
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        async: false,
        beforeSend: function () {

            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            isAjaxCall = false;

            return false;
        },

        success: function (response) {
            isAjaxCall = false;
            IsValidSession(response);

            $('#mainDiv').html(response);
            $.validator.unobtrusive.parse($("#mainDiv"));
            unblockPage();


            //Set Module Name to Session
            $.ajax({
                url: "/Home/SetModuleName/" + moduleName,
                type: "POST",
                cache: false,
                async: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    isAjaxCall = false;

                    return false;
                },
                success: function (response) {
                    if (response.Success) {
                        $('#spnModuleName').html(response.Data);
                        unblockPage();
                    }
                    else {
                        $('#spnModuleName').html("");
                        unblockPage();
                    }

                }
            });//ajax call for Module Name ends here





        }
    });

    //VVIMP dont ever move return false in sucess method
    return false;


}

function setScheme(scheme)
{
    $.blockUI({ message: 'Wait...you will be redirected to Home Page' })
    $.ajax({
        url: "/Home/SetScheme/" + scheme,
        type: "POST",
        cache: false,
        async: false,
        beforeSend: function () {
           // $.blockUI({ message: 'Wait...you will be redirected to Home Page' })
        },
        error: function (xhr, status, error) {
            $.unblockUI();
            Alert("Request can not be processed at this time,please try after some time!!!");
            isAjaxCall = false;

            return false;
        },
        success: function (response) {
            if (response.Success) {
                scheme == 3 ? $('#spnScheme').html("&nbsp;&nbsp;RCPLWE") : scheme == 4 ? $('#spnScheme').html("&nbsp;&nbsp;PMGSY-3") : (scheme == 5 ? $('#spnScheme').html("&nbsp;&nbsp;RCPLWE") : $('#spnScheme').html("&nbsp;&nbsp;Vibrant Village Program-" + response.Data));  //Edited by Shreyas for VVP
                //alert($("#hdnRoleHomePage").val().replace('~', ''));
                //$('#mainDiv').load($("#hdnRoleHomePage").val().replace('~', ''));

                ///PMGSY3
                $.ajax({
                    url: "/Login/SetRedirectUrl/" + scheme,
                    type: "GET",
                    // dataType:"json",
                    success: function (data) {
                        if (data.status == true) {
                            if (data.url == "-") {
                                alert(data.message);
                                $("#rdoScheme1").attr("checked", "checked");
                                setScheme(1);
                                return false;
                            }
                            //alert($("#hdnRoleHomePage").val().replace('~', ''));
                            //$('#mainDiv').load($("#hdnRoleHomePage").val().replace('~', ''));
                            $('#mainDiv').load($("#hdnRoleHomePage").val(data.url));
                            //alert($("#hdnRoleHomePage").val());
                            //LoadPage($("#hdnRoleHomePage").val());
                            window.location.href = data.url;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                           alert(xhr.responseText);

                    }

                });

                setTimeout(function () {
                    $.unblockUI();
                }, 2000);
            }
            else {
                $('#spnScheme').html("");
                setTimeout(function () {
                    $.unblockUI();
                }, 2000);
            }

        }
    });//ajax call for Set Scheme ends here
}


/*Start Government Report*/
function closableNoteDiv(divId, spanId) {
    var i = $(this).index('.modalShow');
    $('#' + divId).fadeOut(1000);
    $('#' + divId).eq(i).fadeIn(1000);


    $('#' + divId).hover(
        function () {
            $(this).find('.close').delay(500).fadeIn(500);
        },
        function () {
            $(this).find('.close').delay(1000).fadeOut(500);
        });

    $('span.close').click(
        function () {
            $("#" + divId).fadeOut(1000);
        });
}

/*End Government Report*/









