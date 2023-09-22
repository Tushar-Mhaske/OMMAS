
$(document).ready(function(){
    
    $.validator.unobtrusive.parse("#frmEmploymentDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });  

    $('#btnAddEmploymentDetails').click(function (e) {

        if ($('#frmEmploymentDetails').valid()) {            
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/FortyPointChecklist/AddEmploymentInformationDetails/",
                type: "POST",
                data: $("#frmEmploymentDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {

                        alert(data.message);

                        $('#btnResetEmploymentDetails').trigger('click');
                        $('#tableEmploymentDetails').trigger('reloadGrid');

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
    
    $('#btnUpdateEmploymentDetails').click(function (e) {

        if ($('#frmEmploymentDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/FortyPointChecklist/UpdateEmploymentDetails/",
                type: "POST",
                data: $("#frmEmploymentDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#tableEmploymentDetails').trigger('reloadGrid');                        
                        $('#btnCancelEmploymentDetails').trigger('click');
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

    $('#btnCancelEmploymentDetails').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FortyPointChecklist/AddEmploymentInformation",
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

    $('#btnResetEmploymentDetails').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $('#spCollapseIconCN').click(function () {
        $('#dvFrmAddDetails').hide('slow');
        //$('#btnCreateNew').show('slow');

    });
    
});