
$(document).ready(function () {

    if ($("#frmUnit") != null) {
        $.validator.unobtrusive.parse("#frmUnit");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {
        if ($('#frmUnit').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterUnit/",
                type: "POST",
            
                data: $("#frmUnit").serialize(),
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        //$('#btnReset').trigger('click');                      
                        //$('#tblUnitList').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#dvUnitDetails').hide('slow');
                        $('#tblUnitList').trigger('reloadGrid');

                    }
                    else if (data.success==false) {
                        if (data.message != "") { 
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvUnitDetails").html(data);
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

        if ($('#frmUnit').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterUnit/",
                type: "POST",
             
                data: $("#frmUnit").serialize(),
                success: function (data) {

                    if (data.success) {
                        alert(data.message);
                        //$('#tblUnitList').trigger('reloadGrid');
                        //$("#dvUnitDetails").load("/Master/AddEditMasterUnit");
                        $("#btnCreateNew").show();
                        $('#dvUnitDetails').hide('slow');
                        $('#tblUnitList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvUnitDetails").html(data);
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
        //    url: "/Master/AddEditMasterUnit",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvUnitDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        $("#btnCreateNew").show();
        $('#dvUnitDetails').hide('slow');
       
    });
    $("#spCollapseIconCN").click(function () {

        if ($("#dvUnitDetails").is(":visible")) {
            $("#dvUnitDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });
    $('#btnReset').click(function () {       
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    

    //for expand and collpase Document Details 
    //$("#dvhdCreateNewUnitDetails").click(function () {

    //    if ($("#dvCreateNewUnitDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");


    //        $(this).next("#dvCreateNewUnitDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewUnitDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_UNIT_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_UNIT_SHORT_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_UNIT_DIMENSION").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});
