$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmSuperstructure');

    $('#btnSave').click(function () {

        if (!$("#frmSuperstructure").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/AddSuperstructureDetails/',
            data: $("#frmSuperstructure").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    LoadSuperstructureGrid();
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

        if (!$("#frmSuperstructure").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/EditSuperstructureDetails/',
            data: $("#frmSuperstructure").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    LoadSuperstructureGrid();
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
        $("#dvEditSuperstructure").html('');
        LoadSuperstructureGrid();
    });

    $('#ddlSubComponent').change(function () {
        if ($('#ddlSubComponent option:selected').val() == 12) {
            $('#courtyard').show('slow');
            $('#ncourtyard').hide('slow');
        }
        else {
            $('#courtyard').hide('slow');
            $('#ncourtyard').show('slow');
        }
    });

    $('#ddlSubComponent').trigger('change');
});

