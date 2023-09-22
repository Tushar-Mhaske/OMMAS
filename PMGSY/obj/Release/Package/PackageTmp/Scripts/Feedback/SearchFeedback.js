jQuery.validator.addMethod("customvalidator", function (value, element, param) {

    var a = $("#txtToken").val();
    var b = $("#txtcontactDetails").val();
    if (a == "" && b == "") {
        $("#lblErrMsg").show();
        $("#lblErrMsg").text("Please enter/select Token or Search By Details");
        return false;
    }
    else if (a == "" && b != "") {
        if ($("#ddlsrDetails").val() == 0 && $("#txtcontactDetails").val() != "") {
            $("#lblErrMsg").show();
            $("#lblErrMsg").text("Please select Search By Details Type");
        }
        else if ($("#ddlsrDetails").val() == 1) {
            var mob = "^[0-9]+$";
            var filter = /^[0-9]+$/;
            if (filter.test(b) && b.length == 10) {
                $("#lblErrMsg").text('');
                return true;
            }
            else {
                $("#lblErrMsg").show();
                $("#lblErrMsg").text("Mobile number should be a 10 digit number");
                return false;
            }

        }
        else if ($("#ddlsrDetails").val() == 2) {
            var tel = /^[0-9-+]+$/;
            if (tel.test(b) && (b.length >= 9 && b.length <= 13)) {
                $("#lblErrMsg").text('');
                return true;
            }
            else {
                $("#lblErrMsg").show();
                $("#lblErrMsg").text("Telephone number should be a 9-13 digit number");
                return false;
            }
        }
        else if ($("#ddlsrDetails").val() == 3) {
            //var email = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
            var email = /^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$/;
            //var email = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/; && (b.length <= 15)
            if (email.test(b)) {
                $("#lblErrMsg").text('');
                return true;
            }
            else {
                $("#lblErrMsg").show();
                $("#lblErrMsg").text("Email id is invalid");
                return false;
            }
        }
            ///Added for Name and Feedback options
        else if ($("#ddlsrDetails").val() == 4) {
            var name = /^[a-zA-Z'. ]+$/;
            if (name.test(b) && (b.length <= 50)) {
                $("#lblErrMsg").text('');
                return true;
            }
            else {
                $("#lblErrMsg").show();
                $("#lblErrMsg").text("Name contains invalid characters");
                return false;
            }
        }
        else if ($("#ddlsrDetails").val() == 5) {
            var name = /^[-0-9a-zA-Z'.,:\n\r ]+$/;
            if (name.test(b) && (b.length <= 100)) {
                $("#lblErrMsg").text('');
                return true;
            }
            else {
                $("#lblErrMsg").show();
                $("#lblErrMsg").text("Feedback contain invalid characters");
                return false;
            }
        }
    }
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("customvalidator"/*, function (options) {
    options.rules["customvalidator"] = "#" + options.params.requiredif;
    options.messages["customvalidator"] = options.message;
}*/);

$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmsearchFB');

    $("#spnHeaderSummarysrchFB2").click(function () {

        $("#spnHeaderSummarysrchFB2").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmsearchFB").toggle("slow");

    });

    //$("#dvTabs").tabs();

    //$("#linkQuery").click(function () {
    //    alert();
    //    $.ajax({
    //        url: '/feedback/Queryfeedback/QueryFB',
    //        async: false,
    //        cache: false,
    //        type: "GET",
    //        dataType: "html",
    //        success: function (data) {
    //            $('#dvFBQuery').html(data);
    //            $('#dvFBQuery').show('fade');
    //            $.unblockUI();
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert(xhr.responseText);
    //            $.unblockUI();
    //        }
    //    });
    //});

    $("#lblcontactDetails").show();

    $("#lblMobileNo").hide();
    $("#lblTelNo").hide();
    $("#lblEmailId").hide();
    $("#lblName").hide();
    $("#lblSubject").hide();

    $("#btnView").click(function () {

        
        $('#dvSearchFBDetails').html('');
        $('#dvSearchFBDetails').hide();
        
        if (($("#txtToken").val() != "") || ($("#ddlsrDetails").val() != 0)) {
            if ($("#frmsearchFB").valid()) {
                getFeedbackDetails();
            }
        }
        else {

            if ($("#ddlsrDetails").val() == 0) {
                $("#vMsgsearchDetails").show();
                $("#vMsgsearchDetails").text("Please select contact Details Type");
            }
        }
    });

    $("#txtToken").change(function () {
        $("#vMsgsearchDetails").empty();
        $("#vMsgsearchDetails").hide();

        $("#ddlsrDetails").val("0");
        $("#txtcontactDetails").val('');

        $("#lblErrMsg").hide();
        $("#lblErrMsg").text('');
    });

    $("#txtcontactDetails").change(function () {
        $("#vMsgsearchDetails").empty();
        $("#vMsgsearchDetails").hide();

        if ($("#frmsearchFB").valid()) {
            $("#lblErrMsg").hide();
            $("#lblErrMsg").text('');
        }
    });

    $("#ddlsrDetails").change(function () {
        $("#txtToken").val('');
        $("#vMsgsearchDetails").empty();
        $("#vMsgsearchDetails").hide();
        if ($("#ddlsrDetails").val() == "1") {
            //$("#lblcontactDetails").text("Enter Mobile Number:")
            $("#lblcontactDetails").hide();

            $("#lblMobileNo").show();
            $("#lblTelNo").hide();
            $("#lblEmailId").hide();
            $("#lblName").hide();
            $("#lblSubject").hide();
        }
        else if ($("#ddlsrDetails").val() == "2") {
            //$("#lblcontactDetails").text("Enter Telephone Number:")
            $("#lblcontactDetails").hide();

            $("#lblMobileNo").hide();
            $("#lblTelNo").show();
            $("#lblEmailId").hide();
            $("#lblName").hide();
            $("#lblSubject").hide();
        }
        else if ($("#ddlsrDetails").val() == "3") {
            //$("#lblcontactDetails").text("Enter Email Id:")
            $("#lblcontactDetails").hide();

            $("#lblMobileNo").hide();
            $("#lblTelNo").hide();
            $("#lblEmailId").show();
            $("#lblName").hide();
            $("#lblSubject").hide();
        }
        else if ($("#ddlsrDetails").val() == "4") {
            //$("#lblcontactDetails").text("Enter Email Id:")
            $("#lblcontactDetails").hide();

            $("#lblMobileNo").hide();
            $("#lblTelNo").hide();
            $("#lblEmailId").hide();
            $("#lblName").show();
            $("#lblSubject").hide();
        }
        else if ($("#ddlsrDetails").val() == "5") {
            //$("#lblcontactDetails").text("Enter Email Id:")
            $("#lblcontactDetails").hide();

            $("#lblMobileNo").hide();
            $("#lblTelNo").hide();
            $("#lblEmailId").hide();
            $("#lblName").hide();
            $("#lblSubject").show();
        }
        else if ($("#ddlsrDetails").val() == "0") {
            //$("#lblcontactDetails").text("Enter Contact Details:")
            $("#lblcontactDetails").show();

            $("#lblMobileNo").hide();
            $("#lblTelNo").hide();
            $("#lblEmailId").hide();
            $("#lblName").hide();
            $("#lblSubject").hide();
        }
    });
});

function getFeedbackDetails() {
    //$("#dvTabs").show();

    //$("#dvFBQuery").load('/feedback/Queryfeedback/QueryFB');

    $('#dvSearchFBDetails').html('');
    $('#dvSearchFBDetails').hide('slow');

    $("#hdnsrdtls").val($("#ddlsrDetails option:selected").text());
    $.ajax({
        url: "/FeedbackDetails/FBListSearch",
        cache: false,
        type: "POST",
        async: false,
        data: $("#frmsearchFB").serialize(),
        success: function (data) {
            $("#dvFBList").html(data);
            $('#dvFBList').show('slow');
        },
        error: function () {
            alert("error");
        }
    })
}

$('#hrefHeadersrchFB').click(function () {
    collapseExpandTbl('hrefHeadersrchFB', 'dvearchFbMain', 'spnHeaderSummarysrchFB1', 'spnHeaderSummarysrchFB2');
});

$('#hrefdispToken').click(function () {
    collapseExpandTbl('hrefdispToken', 'dvearchFbMain', 'spnHeaderSummarysrchFB1', 'spnHeaderSummarysrchFB2');
});


function loadFBDesc(feedId) {
    $('#feedbackId').val(feedId);

    $('#dvSearchFBDetails').html('');
    $('#dvSearchFBDetails').hide('slow');

    $.ajax({
        url: "/FeedbackDetails/searchFBDetails",
        cache: false,
        type: "POST",
        async: false,
        data: $("#frmFBList").serialize(),
        success: function (data) {
            $("#dvSearchFBDetails").html('');
            $("#dvSearchFBDetails").html(data);
            $('#dvSearchFBDetails').show('slow');
            //$("#divNewsDataList").hide('slow');
        },
        error: function () {
            alert("error");
        }
    })
}
