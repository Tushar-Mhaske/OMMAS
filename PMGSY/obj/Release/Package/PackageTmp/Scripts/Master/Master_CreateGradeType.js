
$(document).ready(function () {

    if ($("#frmMasterGradeType") != null) {
        $.validator.unobtrusive.parse("#frmMasterGradeType");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSave').click(function (e) {

        if ($('#frmMasterGradeType').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterGradeType/",
                type: "POST",
              
                data: $("#frmMasterGradeType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        
                        //$('#btnReset').trigger('click');
                        //$('#tblMasterGradeTypeList').trigger('reloadGrid');

                        $("#btnCreateNew").show();
                        $('#dvGradeDetails').hide('slow');
                        $('#tblMasterGradeTypeList').trigger('reloadGrid');                        
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvGradeDetails").html(data);
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

        if ($('#frmMasterGradeType').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterGradeType/",
                type: "POST",
           
                data: $("#frmMasterGradeType").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterGradeTypeList').trigger('reloadGrid');
                        //$("#dvGradeDetails").load("/Master/AddEditMasterGradeType");

                        $("#btnCreateNew").show();
                        $('#dvGradeDetails').hide('slow');
                        $('#tblMasterGradeTypeList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvGradeDetails").html(data);
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
        //    url: "/Master/AddEditMasterGradeType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvGradeDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});

        $("#btnCreateNew").show();
        $('#dvGradeDetails').hide('slow');
     
    });

    $('#btnReset').click(function () {       
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');        
    });
    

    $("#spCollapseIconCN").click(function () {

        if ($("#dvGradeDetails").is(":visible")) {
            $("#dvGradeDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });
   
    //$("#dvhdCreateNewGradeDetails").click(function () {

    //    if ($("#dvCreateNewGradeDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

         

    //        $(this).next("#dvCreateNewGradeDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewGradeDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_GRADE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_GRADE_SHORT_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});



