$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmCarriageDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddCarriageDetails").is(":visible")) {
            $('#dvAddCarriageDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchCarriageDetails").is(":visible")) {
            $("#divSearchCarriageDetails").show('slow');
        }

    });

    $("#btnSaveCarriageDetails").click(function (e) {

        if ($("#frmCarriageDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddCarriageDetails/',
                async: false,
                data: $("#frmCarriageDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        if ($("#dvAddCarriageDetails").is(":visible")) {
                            $('#dvAddCarriageDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchCarriageDetails").is(":visible")) {
                            $("#divSearchCarriageDetails").show('slow');
                        }
                        $('#tblCarriageDetails').trigger('reloadGrid');
                       // $("#dvAddCarriageDetails").load("/Master/AddEditCarriageDetails");
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
                        $("#dvAddCarriageDetails").html(data);
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

    $('#btnUpdateCarriageDetails').click(function (e) {

        if ($('#frmCarriageDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditCarriageDetails/",
                type: "POST",
                data: $("#frmCarriageDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblCarriageDetails').trigger('reloadGrid');
                        //$("#dvAddCarriageDetails").load("/Master/AddEditCarriageDetails");
                        if ($("#dvAddCarriageDetails").is(":visible")) {
                            $('#dvAddCarriageDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchCarriageDetails").is(":visible")) {
                            $("#divSearchCarriageDetails").show('slow');
                        }
                        $('#tblCarriageDetails').trigger('reloadGrid');

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddCarriageDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetCarriageDetails").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $('#btnCancelCarriageDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditCarriageDetails",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddCarriageDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //    }
        //});
        if ($("#dvAddCarriageDetails").is(":visible")) {
            $('#dvAddCarriageDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchCarriageDetails").is(":visible")) {
            $("#divSearchCarriageDetails").show('slow');
        }
        

    });
});