$(document).ready(function () {


    $.validator.unobtrusive.parse('#frmAdd');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#iconClose").click(function () {
        if ($("#dvAddTrafficDetails").is(':visible')) {
            $("#dvAddTrafficDetails").hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTrafficDetails").is(":visible")) {
            $("#divSearchTrafficDetails").show('slow');
        }
    });

    $("#btnSave").click(function (e) {
        if ($("#frmAdd").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddTrafficType/',
                async: false,
                data: $("#frmAdd").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        ClearDetails();
                        //$('#tblTrafficDetails').trigger('reloadGrid');
                        if ($("#dvAddTrafficDetails").is(":visible")) {
                            $('#dvAddTrafficDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTrafficDetails").is(":visible")) {
                            $("#divSearchTrafficDetails").show('slow');
                        }
                        $('#tblTrafficDetails').trigger('reloadGrid');
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
                        $("#dvAddTrafficDetails").html(data);
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

    $("#MAST_TRAFFIC_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_TRAFFIC_STATUS").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
  
 
    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnCancel").click(function (e) {
        //$.ajax({
        //    url: "/Master/AddTrafficType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
             
        //        $("#dvAddTrafficDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#dvAddTrafficDetails").is(":visible")) {
            $('#dvAddTrafficDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#divSearchTrafficDetails").is(":visible")) {
            $("#divSearchTrafficDetails").show('slow');
        }
       
    })

    $("#spCollapseIconCN").click(function () {

        if ($("#dvCreateNewDetails").is(":visible")) {
            $("#dvCreateNewDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnUpdate").click(function (e) {

        if ($("#frmAdd").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditTrafficType/',
                async: false,
                data: $("#frmAdd").serialize(),
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                   
                        //$('#dvAddTrafficDetails').load("/Master/AddTrafficType");
                        //$('#tblTrafficDetails').trigger('reloadGrid');
                        if ($("#dvAddTrafficDetails").is(":visible")) {
                            $('#dvAddTrafficDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#divSearchTrafficDetails").is(":visible")) {
                            $("#divSearchTrafficDetails").show('slow');
                        }
                        $('#tblTrafficDetails').trigger('reloadGrid');
                    }
                    else if (!data.success) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddTrafficDetails").html(data);
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
    $('#MAST_TRAFFIC_NAME').val('');
    $('#MAST_TRAFFIC_CODE').val('');
    $('#MAST_TRAFFIC_STATUS').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}