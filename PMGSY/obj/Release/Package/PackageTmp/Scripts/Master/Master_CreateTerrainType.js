
jQuery.validator.addMethod("comparefieldvalidator", function (value, element, param) {

    if (parseInt($('#MAST_TERRAIN_SLOP_TO').val()) <= parseInt($('#MAST_TERRAIN_SLOP_FROM').val()))
        return false;
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldvalidator");


$(document).ready(function () {

    $.validator.unobtrusive.parse("#frm");
   
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#MAST_TERRAIN_SLOP_TO").blur(function () {
       
       // alert(parseInt($("#MAST_TERRAIN_SLOP_TO")));

       
    });


    $('#btnSave').click(function (e) {
      
     

        if ($('#frm').valid()) {
            var slopTo = $("#MAST_TERRAIN_SLOP_TO").val();
            var slopFrom = $("#MAST_TERRAIN_SLOP_FROM").val();

            if (slopTo != "" && slopFrom == "") {
                alert("Please enter Slope From.");
                return false;
            }
            if (slopFrom != "" && slopTo == "") {
                alert("Please enter Slope To.");
                return false;
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterTerrainType/",
                type: "POST",
             
                data: $("#frm").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);                      
                        //$('#btnReset').trigger('click');
                        //$('#tblList').trigger('reloadGrid');

                        $("#btnCreateNew").show();
                        $('#dvDetails').hide('slow');
                        $('#tblList').trigger('reloadGrid');

                        
                    }
                    else if (data.success==false) {
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
                    $.unblockUI();
                }

            });

        }
    });



    $('#btnUpdate').click(function (e) {

        if ($('#frm').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterTerrainType/",
                type: "POST",
           
                data: $("#frm").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblList').trigger('reloadGrid');
                        //$("#dvDetails").load("/Master/AddEditMasterTerrainType");
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
                    $.unblockUI();
                }

            });
        }
    });
    $("#spCollapseIconCN").click(function () {

        if ($("#dvDetails").is(":visible")) {
            $("#dvDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterTerrainType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        $("#btnCreateNew").show();
        $('#dvDetails').hide('slow');
         
    });

    $('#btnReset').click(function () {
        
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    

    //for expand and collpase Document Details 
    //$("#dvhdCreateNewDetails").click(function () {

    //    if ($("#dvCreateNewDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        //$("#dvDocumentDetails").css('margin-bottom','10px');

    //        $(this).next("#dvCreateNewDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_TERRAIN_TYPE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_TERRAIN_SLOP_FROM").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_TERRAIN_SLOP_TO").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_TERRAIN_ROADWAY_WIDTH").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});
