
$(document).ready(function(){
    
    $.validator.unobtrusive.parse("#frmTenderCostDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });  

    $('#btnAddTenderCostDetails').click(function (e) {

        if ($('#frmTenderCostDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/FortyPointChecklist/AddTenderCostInformationDetails/",
                type: "POST",
                data: $("#frmTenderCostDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        
                        alert(data.message);

                        $('#btnResetTenderCostDetails').trigger('click');
                        $('#tblListGridDetails').trigger('reloadGrid');

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
    
    $('#btnUpdateTenderCostDetails').click(function (e) {

        if ($('#frmTenderCostDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/FortyPointChecklist/UpdateTenderCostDetails/",
                type: "POST",
                data: $("#frmTenderCostDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#tblListGridDetails').trigger('reloadGrid');
                        $('#btnCancelTenderCostDetails').trigger('click');
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

    $('#btnCancelTenderCostDetails').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FortyPointChecklist/AddEditTenderCostInformationDetails",
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

    $('#btnResetTenderCostDetails').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $('#spCollapseIconCN').click(function () {
        $('#dvFrmAddDetails').hide('slow');
    });
});