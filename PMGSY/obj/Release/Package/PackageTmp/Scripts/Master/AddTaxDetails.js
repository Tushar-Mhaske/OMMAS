$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmTaxDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddTaxDetails").is(':visible')) {
            $("#dvAddTaxDetails").hide('slow');
            $("#btnCreateNew").show('slow');
        }

    });

    $("#btnAddTaxDetails").click(function (e) {

        if ($("#frmTaxDetails").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddTaxDetails/',
                async: false,
                data: $("#frmTaxDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblstTaxDetails').trigger('reloadGrid');
                        //$("#dvAddTaxDetails").load("/Master/AddEditTaxDetails");
                        $("#dvAddTaxDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $('#tblstTaxDetails').trigger('reloadGrid');
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
                        $("#dvAddTaxDetails").html(data);
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

    $('#btnUpdateTaxDetails').click(function (e) {

        if ($('#frmTaxDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditTaxDetails/",
                type: "POST",
                data: $("#frmTaxDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblstTaxDetails').trigger('reloadGrid');
                        //$("#dvAddTaxDetails").load("/Master/AddEditTaxDetails");
                        $("#dvAddTaxDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $('#tblstTaxDetails').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddTaxDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });


    $("#btnResetTaxDetails").click(function () {
        $("#divError").hide('slow');
    });

    $('#btnCancelTaxDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditTaxDetails",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddTaxDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //    }
        //});
        $("#dvAddTaxDetails").hide('slow');
        $("#btnCreateNew").show();
     });


    $('#Effective_Date').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a Start Date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
        },
        onClose: function ()
        {
            $(this).focus().blur();
        }
    });

});