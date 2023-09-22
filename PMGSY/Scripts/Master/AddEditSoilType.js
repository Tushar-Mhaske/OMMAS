$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddSoilType');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#btnSave").click(function (e) {

        if ($("#frmAddSoilType").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddSoilType/',
                async: false,
                data: $("#frmAddSoilType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                    
                        alert(data.message);
                        //ClearDetails();
                        //$('#soilCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#soilDetails').hide('slow');
                        $('#soilCategory').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }                        
                    }
                    else {
                        $("#soilDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $("#MAST_SOIL_TYPE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    //$("#dvhdAddNewSoilDetails").click(function () {

    //    if ($("#dvAddNewSoilDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewSoilDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewSoilDetails").slideToggle(300);
    //    }
    //});


    $("#spCollapseIconCN").click(function () {

        if ($("#soilDetails").is(":visible")) {
            $("#soilDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddSoilType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
            
        //        $("#soilDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show();
        $('#soilDetails').hide('slow');
    
    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {
     
        if ($("#frmAddSoilType").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditSoilType/',
                async: false,
                data: $("#frmAddSoilType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                     
                        //$("#soilDetails").load('/Master/AddSoilType');
                        //$('#soilCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#soilDetails').hide('slow');
                        $('#soilCategory').trigger('reloadGrid');
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#soilDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });
});

function ClearDetails() {
    $('#MAST_SOIL_TYPE_CODE').val('');
    $('#MAST_SOIL_TYPE_NAME').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}