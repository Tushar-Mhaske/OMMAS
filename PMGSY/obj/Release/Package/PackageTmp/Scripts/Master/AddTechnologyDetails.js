$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmTechnologyDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddTechnologyDetails").is(':visible')) {
            $("#dvAddTechnologyDetails").hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTechnologyDetails").is(":visible")) {
            $("#divSearchTechnologyDetails").show('slow');
        }
    });

    $("#btnSaveTechnologyDetails").click(function (e) {

        if ($("#frmTechnologyDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddTechnologyDetails/',
                async: false,
                data: $("#frmTechnologyDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblTechnologyDetails').trigger('reloadGrid');
                        //$("#dvAddTechnologyDetails").load("/Master/AddEditTechDetails");
                        if ($("#dvAddTechnologyDetails").is(":visible")) {
                            $('#dvAddTechnologyDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTechnologyDetails").is(":visible")) {
                            $("#divSearchTechnologyDetails").show('slow');
                        }
                        $('#tblTechnologyDetails').trigger('reloadGrid');
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
                        $("#dvAddTechnologyDetails").html(data);
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

    $('#btnUpdateTechnologyDetails').click(function (e) {

        if ($('#frmTechnologyDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditTechnologyDetails/",
                type: "POST",
                data: $("#frmTechnologyDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblTechnologyDetails').trigger('reloadGrid');                        
                        //$("#dvAddTechnologyDetails").load("/Master/AddEditTechDetails");
                        if ($("#dvAddTechnologyDetails").is(":visible")) {
                            $('#dvAddTechnologyDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTechnologyDetails").is(":visible")) {
                            $("#divSearchTechnologyDetails").show('slow');
                        }
                        $('#tblTechnologyDetails').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddTechnologyDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetTechnologyDetails").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $('#btnCancelTechnologyDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditTechDetails",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddTechnologyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //    }
        //});
        if ($("#dvAddTechnologyDetails").is(":visible")) {
            $('#dvAddTechnologyDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTechnologyDetails").is(":visible")) {
            $("#divSearchTechnologyDetails").show('slow');
        }
        
    });
});