$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAddChapterDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {

        if ($("#dvLayoutofAddChapterView").is(':visible')) {
            $("#dvLayoutofAddChapterView").hide('slow');
            $("#btnAdd").show('slow');
        }
    });

    $("#btnSave").click(function (e) {



        if ($("#frmAddChapterDetails").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({

                type: 'POST',
                url: '/ARRR/AddMaterialDetails',
                async: false,
                data: $("#frmAddChapterDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //1.Grid Will be triggered.
                        $("#tblstChapterDetails").trigger('reloadGrid');
                        //2.Add Edit view will be reloaded.
                        $("#dvLayoutofAddChapterView").load('/ARRR/AddEditMaterialDetails');
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            $("#message").html(data.html);
                            $("#dvErrorMessage").show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvLayoutofAddChapterView").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }

            })

        }


    });
    //Cancel Button
    $('#btnCancel').click(function (e) {
        $.ajax({
            url: "/ARRR/AddEditMaterialDetails",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvLayoutofAddChapterView").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    });

    // Update
    $('#btnUpdate').click(function (e) {

        if ($('#frmAddChapterDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/ARRR/EditMaterialDetails/",
                type: "POST",
                data: $("#frmAddChapterDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tblstChapterDetails').trigger('reloadGrid');
                        $("#dvLayoutofAddChapterView").load("/ARRR/AddEditMaterialDetails");
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvLayoutofAddChapterView").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            });
        }
    });



});