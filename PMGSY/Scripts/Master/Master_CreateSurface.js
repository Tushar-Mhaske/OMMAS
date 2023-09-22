
$(document).ready(function () {

    if ($("#frmMasterSurface") != null) {
        $.validator.unobtrusive.parse("#frmMasterSurface");
    }

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSave').click(function (e) {

        if ($('#frmMasterSurface').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterSurface/",
                type: "POST",
               // dataType: "json",
                data: $("#frmMasterSurface").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        // ClearGradeDetails();
                        //$('#btnReset').trigger('click');
                        //$('#tblMasterSurfaceList').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#dvSurfaceDetails').hide('slow');
                        $('#tblMasterSurfaceList').trigger('reloadGrid');
                        
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvSurfaceDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });



    $('#btnUpdate').click(function (e) {

        if ($('#frmMasterSurface').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterSurface/",
                type: "POST",
               // dataType: "json",
                data: $("#frmMasterSurface").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterSurfaceList').trigger('reloadGrid');
                        //$("#dvSurfaceDetails").load("/Master/AddEditMasterSurface");
                        $("#btnCreateNew").show();
                        $('#dvSurfaceDetails').hide('slow');
                        $('#tblMasterSurfaceList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvSurfaceDetails").html(data);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterSurface",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvSurfaceDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        $("#btnCreateNew").show();
        $('#dvSurfaceDetails').hide('slow');
    });

    $('#btnReset').click(function () {
     //   ClearGradeDetails();

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#spCollapseIconCN").click(function () {

        if ($("#dvSurfaceDetails").is(":visible")) {
            $("#dvSurfaceDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    //for expand and collpase Document Details 
    //$("#dvhdCreateNewSurfaceDetails").click(function () {

    //    if ($("#dvCreateNewSurfaceDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        //$("#dvDocumentDetails").css('margin-bottom','10px');

    //        $(this).next("#dvCreateNewSurfaceDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewSurfaceDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_SURFACE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});



//function ClearGradeDetails() {
//    $('#MAST_SURFACE_NAME').val('');
//    $('#dvErrorMessage').hide('slow');
//    $('#message').html('');
//}