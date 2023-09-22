$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddRoadCategory');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#btnSave").click(function (e) {

        if ($("#frmAddRoadCategory").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddRoadCategory/',
                async: false,
                data: $("#frmAddRoadCategory").serialize(),
                success: function (data) {
                    if (data.success==true) {
                     
                        alert(data.message);
                        //ClearDetails();
                       // $('#roadCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#roadDetails').hide('slow');
                        $('#roadCategory').trigger('reloadGrid');


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
                        $("#roadDetails").html(data);
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


    $("#MAST_ROAD_CAT_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_ROAD_CAT_DESC").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    //$("#dvhdAddNewRoadDetails").click(function () {

    //    if ($("#dvAddNewRoadDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewRoadDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewRoadDetails").slideToggle(300);
    //    }
    //});
    $("#spCollapseIconCN").click(function () {

        if ($("#roadDetails").is(":visible")) {
            $("#roadDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });


    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddRoadCategory",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
             
        //        $("#roadDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show();
        $('#roadDetails').hide('slow');
    
    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {
        if ($("#frmAddRoadCategory").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditRoadCategory/',
                async: false,
                data: $("#frmAddRoadCategory").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //$('#roadDetails').load("/Master/AddRoadCategory");
                        //$('#roadCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $('#roadDetails').hide('slow');
                        $('#roadCategory').trigger('reloadGrid');
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#roadDetails").html(data);
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
    $('#MAST_ROAD_CAT_NAME').val('');
    $('#MAST_ROAD_SHORT_DESC').val('');
    $('#MAST_ROAD_CAT_CODE').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}