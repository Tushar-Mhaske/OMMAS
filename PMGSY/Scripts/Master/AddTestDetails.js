$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmTestDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddTestDetails").is(':visible')) {
            $("#dvAddTestDetails").hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTestDetails").is(":visible")) {
            $("#divSearchTestDetails").show('slow');
        }

    });

    $("#btnSaveTestDetails").click(function (e) {

        if ($("#frmTestDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddTestDetails/',
                async: false,
                data: $("#frmTestDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblTestDetails').trigger('reloadGrid');
                        //$("#dvAddTestDetails").load("/Master/AddEditTestDetails");
                        if ($("#dvAddTestDetails").is(":visible")) {
                            $('#dvAddTestDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTestDetails").is(":visible")) {
                            $("#divSearchTestDetails").show('slow');
                        }
                        $('#tblTestDetails').trigger('reloadGrid');
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
                        $("#dvAddTestDetails").html(data);
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

    $('#btnUpdateTestDetails').click(function (e) {

        if ($('#frmTestDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditTestDetails/",
                type: "POST",
                data: $("#frmTestDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblTestDetails').trigger('reloadGrid');
                        //$("#dvAddTestDetails").load("/Master/AddEditTestDetails");
                        if ($("#dvAddTestDetails").is(":visible")) {
                            $('#dvAddTestDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTestDetails").is(":visible")) {
                            $("#divSearchTestDetails").show('slow');
                        }
                        $('#tblTestDetails').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddTestDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetTestDetails").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $('#btnCancelTestDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditTestDetails",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddTestDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //    }
        //});
        if ($("#dvAddTestDetails").is(":visible")) {
            $('#dvAddTestDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTestDetails").is(":visible")) {
            $("#divSearchTestDetails").show('slow');
        }
      
    });
});