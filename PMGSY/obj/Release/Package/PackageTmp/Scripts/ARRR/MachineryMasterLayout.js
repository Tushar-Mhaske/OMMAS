$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmMachineryMaster');

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmMachineryMaster").toggle("slow");
    });

    $("#btnSave").click(function () {
        if ($('#frmMachineryMaster').valid()) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddEditMachineryMasterDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmMachineryMaster").serialize(),
                //contentType: false,
                //processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadMachineryMasterGrid();
                        $('#dvLoadMachineryMaster').hide('slow');
                        $("#btnAdd").show('slow');
                    }
                }
            })
        }
    });

    $('#btnUpdate').click(function () {
        if ($('#frmMachineryMaster').valid()) {
            $('#User_Action').val('E');
            $.ajax({
                url: '/ARRR/AddEditMachineryMasterDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmMachineryMaster").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadMachineryMasterGrid();
                        $('#dvLoadMachineryMaster').hide('slow');
                        $("#btnAdd").show('slow');
                    }
                }
            })
        }
    })

    $("#btnCancel").click(function () {
        $('#dvLoadMachineryMaster').hide('slow');
        $("#btnAdd").show('slow');
    });

});