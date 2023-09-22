$(document).ready(function () {

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    //save the CDWorks details
    $("#btnAddCDWorksDetails").click(function () {
        if ($("#frmAddCDWorks").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/AddCDWorksDetails/',
                data: $("#frmAddCDWorks").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        $("#tbCDWorksList").trigger('reloadGrid');
                        $("#btnResetCDWorksDetails").trigger('click');
                        $("#divAddCDWorks").html('');
                    }
                    else {
                        //alert(data.message);
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' +data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }

            })
        }
    });

    //update cdworks details button click
    $("#btnUpdateCDWorksDetails").click(function () {
        if ($("#frmAddCDWorks").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/EditCDWorksDetails/',
                data: $("#frmAddCDWorks").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        $("#tbCDWorksList").trigger('reloadGrid');
                        $("#divAddCDWorks").html('');
                    }
                    else {
                        $("#divError").show();
                        $("#divError").html(data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnCancelCDWorksDetails").click(function () {

        $("#divAddCDWorks").html('');
    });

    $('#imgCloseProgressDetails').click(function () {

        $("#divAddCDWorks").hide("slow");
        $("#divError").hide("slow");

    });


    $("#btnResetCDWorksDetails").click(function () {

        $("#divError").hide("slow");
    });

});