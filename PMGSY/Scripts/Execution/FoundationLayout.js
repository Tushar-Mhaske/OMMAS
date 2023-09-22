$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmFoundation');

    $('#btnSave').click(function () {

        if (!$("#frmFoundation").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/AddFoundationDetails/',
            data: $("#frmFoundation").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    LoadFoundationGrid();
                    $("#btnCancel").trigger('click');
                    $("#divAddCDWorks").html('');
                }
                else {
                    alert(data.message);
                    $("#divError").show();
                    $("#divError").html('<strong>Alert : </strong>' + data.message);
                }
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }

        })
    });

    $('#btnUpdate').click(function () {

        if (!$("#frmFoundation").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/EditFoundationDetails/',
            data: $("#frmFoundation").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    LoadFoundationGrid();
                    $("#btnCancel").trigger('click');
                    $("#divAddCDWorks").html('');
                }
                else {
                    alert(data.message);
                    $("#divError").show();
                    $("#divError").html('<strong>Alert : </strong>' + data.message);
                }
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }

        })
    });

    $('#btnCancel').click(function () {
        $("#dvEditFoundation").html('');
        LoadFoundationGrid();
    });
});

