$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmEarthworkExcavation');

    $('#btnSave').click(function () {

        if (!$("#frmEarthworkExcavation").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/AddEarthworkExcavationDetails/',
            data: $("#frmEarthworkExcavation").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    $('#btnCancel').trigger('click');
                    LoadEarthworksGrid();
                    $("#btnResetCDWorksDetails").trigger('click');
                    $("#divAddCDWorks").html('');
                    $("#tbExecutionList").trigger('reloadGrid');
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

        if (!$("#frmEarthworkExcavation").valid()) {
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Execution/EditEarthworkExcavationDetails/',
            data: $("#frmEarthworkExcavation").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tbCDWorksList").trigger('reloadGrid');
                    $('#btnCancel').trigger('click');
                    LoadEarthworksGrid();
                    $("#btnResetCDWorksDetails").trigger('click');
                    $("#divAddCDWorks").html('');
                    $("#tbExecutionList").trigger('reloadGrid');
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
        $("#AdEditEarthwork").html('');
        LoadEarthworksGrid();
        $("#tbExecutionList").trigger('reloadGrid');
    });
});

