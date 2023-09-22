
$(document).ready(function(){
    
    $.validator.unobtrusive.parse("#frmTenderEquipmentDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });  

    $('#btnAddTenderEquipmentDetails').click(function (e) {

        if ($('#frmTenderEquipmentDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/FortyPointChecklist/AddTenderEquipmentDetails/",
                type: "POST",
                data: $("#frmTenderEquipmentDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#btnResetTenderEquipmentDetails').trigger('click');
                        $('#tableListEquipmentDetails').trigger('reloadGrid');

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvFrmAddDetails").html(data);
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
    
    $('#btnUpdateTenderEquipmentDetails').click(function (e) {

        if ($('#frmTenderEquipmentDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/FortyPointChecklist/UpdateTenderEquipmentDetails/",
                type: "POST",
                data: $("#frmTenderEquipmentDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#tableListEquipmentDetails').trigger('reloadGrid');
                        $('#btnCancelTenderEquipmentDetails').trigger('click');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvFrmAddDetails").html(data);
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

    $('#btnCancelTenderEquipmentDetails').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FortyPointChecklist/AddEditTenderEquipmentDetails",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvFrmAddDetails").html(data);
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
                alert(xhr.responseText);               
            }
        });
    });

    $('#btnResetTenderEquipmentDetails').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $('#spCollapseIconCN').click(function () {
        $('#dvFrmAddDetails').hide('slow');
    });
});