
$(document).ready(function () {



    $.validator.unobtrusive.parse("#frmEndDateState");
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnResetEndDateState").click(function () {
        $("#dvErrorMessageEndDteState").hide('slow');

    });

    if ($('#dvErrorMessageEndDteState').is(':visible')) {
        $('#dvErrorMessageEndDteState').hide();
    }

    $('#txtEndDateState').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../../Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function () {
            $('.input-validation-error').addClass('input-validation-valid');
            $('.input-validation-error').removeClass('input-validation-error');
            //Removes validation message after input-fields
            $('.field-validation-error').addClass('field-validation-valid');
            $('.field-validation-error').removeClass('field-validation-error');
            //Removes validation summary 
            $('.validation-summary-errors').addClass('validation-summary-valid');
            $('.validation-summary-errors').removeClass('validation-summary-errors');
        }
    });


    $("#btnSaveEndDateState").click(function () {

        if (confirm("Are you sure you want to update the End Date. You won't be allowed to change it again.")) {
            if ($("#frmEndDateState").valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    url: "/Master/UpdateStateEndDatePTA/",
                    type: "POST",
                    data: $("#frmEndDateState").serialize(),
                    async: false,
                    cache: false,
                    dataType: 'json',
                    success: function (data) {

                        if (data.success == true) {

                            alert("End Date Saved successfully");
                            $("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');
                            $.unblockUI();
                            $("#dvEndDateDialogState").dialog("close");
                        }
                        else if (data.success == false) {
                            if (data.message != "") {
                                $('#messageEndDateState').html(data.message);
                                $('#dvErrorMessageEndDteState').show('slow');
                                window.location.hash = '#dvErrorMessageEndDteState';
                                $.unblockUI();
                            }
                        }
                        else if (data.success == false) {

                            alert("End Date can not be Blank.");
                            $.unblockUI();
                        }

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });
            }
        }
        else {

            return false;
        }

    });


});