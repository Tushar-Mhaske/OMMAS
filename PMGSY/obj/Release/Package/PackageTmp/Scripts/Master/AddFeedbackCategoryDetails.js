$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmFeedbackDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddFeedbackDetails").is(':visible')) {
            $("#dvAddFeedbackDetails").hide('slow');
            $("#btnCreateNew").show('slow');
        }
    });

    $("#btnSaveFeedbackDetails").click(function (e) {

        if ($("#frmFeedbackDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddFeedbackDetails/',
                async: false,
                data: $("#frmFeedbackDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblFeedbackDetails').trigger('reloadGrid');
                        //$("#dvAddFeedbackDetails").load("/Master/AddEditFeedbackDetails");
                        $("#dvAddFeedbackDetails").hide('slow');
                        $("#btnCreateNew").show('slow');
                        $('#tblFeedbackDetails').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvAddFeedbackDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $('#btnUpdateFeedbackDetails').click(function (e) {

        if ($('#frmFeedbackDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditFeedbackDetails/",
                type: "POST",
                data: $("#frmFeedbackDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblFeedbackDetails').trigger('reloadGrid');
                        //$("#dvAddFeedbackDetails").load("/Master/AddEditFeedbackDetails");
                        $("#dvAddFeedbackDetails").hide('slow');
                        $("#btnCreateNew").show('slow');
                        $('#tblFeedbackDetails').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddFeedbackDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetFeedbackDetails").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $('#btnCancelFeedbackDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditFeedbackDetails",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddFeedbackDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //    }
        //});
        $("#dvAddFeedbackDetails").hide('slow');
        $("#btnCreateNew").show('slow');
        
    });
});