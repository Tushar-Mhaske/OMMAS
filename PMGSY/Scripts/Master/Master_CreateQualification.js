
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmQualification");


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //This method is for reset functionality.
    $("#btnReset").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $("#MAST_QUALIFICATION_NAME").focus(function () {
        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

    //This method is for cancel functionality.
    $("#btnCancel").click(function () {
        // $("#dvDetails").load('/Master/AddMasterQual');
        $("#btnCreateNew").show();
        $('#dvDetails').hide('slow');
      
    });

    //This functionality is for maximmising/minimising list/form.
    //$("#dvhdCreateNewDetails").click(function () {

    //    if ($("#dvCreateNewDetails").is(":visible")) {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
    //        $(this).next("#dvCreateNewDetails").slideToggle(300);
    //    }
    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
    //        $(this).next("#dvCreateNewDetails").slideToggle(300);
    //    }
    //});

    $("#spCollapseIconCN").click(function () {

        if ($("#dvDetails").is(":visible")) {
            $("#dvDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    //This method is for save button.
    $("#btnSave").click(function () {

        $("#ErrorMessage").show();
        if ($("#frmQualification").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterQual",
                type: "POST",
                data: $("#frmQualification").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //$("#MAST_QUALIFICATION_NAME").val("");
                        //$('#tblList').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#dvDetails').hide('slow');
                        $('#tblList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);

                }
            });
        }
    });

    //This method is for update button.
    $("#btnUpdate").click(function () {
        if ($("#frmQualification").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterQual",
                type: "POST",
                data: $("#frmQualification").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblList').trigger('reloadGrid');
                        //$("#dvDetails").load('/Master/AddMasterQual');
                        //$("#dvErrorMessage").hide();
                        $("#btnCreateNew").show();
                        $('#dvDetails').hide('slow');
                        $('#tblList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    
    });

});