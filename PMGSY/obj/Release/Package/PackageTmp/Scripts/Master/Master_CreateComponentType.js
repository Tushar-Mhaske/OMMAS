
$(document).ready(function () {

    if ($("#frmMasterComponentType") != null) {
        $.validator.unobtrusive.parse("#frmMasterComponentType");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSave').click(function (e) {

        if ($('#frmMasterComponentType').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterComponentType/",
                type: "POST",
               
                data: $("#frmMasterComponentType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                    

                        //$('#btnReset').trigger('click');
                        //$('#tblMasterComponentTypeList').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#dvComponentDetails').hide('slow');
                        $('#tblMasterComponentTypeList').trigger('reloadGrid');
                        
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvComponentDetails").html(data);
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

        if ($('#frmMasterComponentType').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterComponentType/",
                type: "POST",
              
                data: $("#frmMasterComponentType").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterComponentTypeList').trigger('reloadGrid');
                        //$("#dvComponentDetails").load("/Master/AddEditMasterComponentType");
                        $("#btnCreateNew").show();
                        $('#dvComponentDetails').hide('slow');
                        $('#tblMasterComponentTypeList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvComponentDetails").html(data);
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

        if ($("#dvComponentDetails").is(":visible")) {
            $("#dvComponentDetails").hide("slow");
           
            $("#btnCreateNew").show();
        }
    });
    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterComponentType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
              
        //        $("#dvComponentDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show();
        $('#dvComponentDetails').hide('slow');
      
    });

    $('#btnReset').click(function () {
        
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');


    });
    

    
    //$("#dvhdCreateNewComponentDetails").click(function () {

    //    if ($("#dvCreateNewComponentDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

        

    //        $(this).next("#dvCreateNewComponentDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewComponentDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_COMPONENT_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


});
