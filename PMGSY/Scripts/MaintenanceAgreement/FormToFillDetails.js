$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddAgreementDetails'));

    //$('#MANE_CONSTR_COMP_DATE').datepicker({

    //    dateFormat: 'dd/mm/yy',
    //    showOn: "button",
    //    buttonImage: "/Content/Images/calendar_2.png",
    //    buttonImageOnly: true,
    //    changeMonth: true,
    //    changeYear: true,
    //    buttonText: 'Agreement Date',
    //    onSelect: function (selectedDate) {  },
    //    onClose: function () {

    //        $(this).focus().blur();
    //    }
    //});

    $('#MANE_HANDOVER_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Handover Date',
        onSelect: function (selectedDate) {
            if ($('#tdAgreementDate .ui-datepicker-trigger').is(':visible')) {
                $("#MANE_AGREEMENT_DATE").datepicker("option", "maxDate", selectedDate);
            }
            $("#MANE_CONSTR_COMP_DATE").datepicker("option", "maxDate", selectedDate);
            $("#MANE_MAINTENANCE_START_DATE").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $('#btnSaveAgreementDetails').click(function (e) {

        if ($('#frmAddAgreementDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/MaintenanceAgreement/SaveContractorDetails",
                type: "POST", 
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {
               
                    if (data.success == true) {
                        
                        alert(data.message);

                        ReloadAddAgreemntDetails();

                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error occured while processing your request.");
                    if ($("#rdoIsNewContractorYes").is(":checked")) {
                        $('#ddlContractors').attr('disabled', true);
                    }
                    $.unblockUI();
                }

            });

        }
    });


    function ReloadAddAgreemntDetails() {

        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

        $.ajax({
            url: "/MaintenanceAgreement/MaintenanceContractorDetails/" + encryptedIMSPRCode,
            type: "GET",
            dataType: "html",
            async: false,
            success: function (data) {

                $('#dvRoadDetails').hide('slow');
                $('#frmAddAgreementDetails').hide('slow');
                $("#searchExecution").show('slow')
                $("#tbExecutionList").trigger('reloadGrid');
                $('#gview_tbExecutionList .ui-jqgrid-titlebar-close>span').trigger('click');

                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occured while processing your request.");
            }

        });

    }

    $('#btnResetAgreementDetails').click(function (e) {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#spmessage').html('');
        }

        $('#MANE_AGREEMENT_NUMBER').removeAttr('readonly');
        $('#MANE_AGREEMENT_DATE').removeAttr('readonly');
        $('#ddlContractors').attr('disabled', false);
        $('#MANE_YEAR1_AMOUNT').removeAttr('readonly');
        $('#MANE_YEAR2_AMOUNT').removeAttr('readonly');
        $('#MANE_YEAR3_AMOUNT').removeAttr('readonly');
        $('#MANE_YEAR4_AMOUNT').removeAttr('readonly');
        $('#MANE_YEAR5_AMOUNT').removeAttr('readonly');
        $('#tdAgreementDate .ui-datepicker-trigger').show();

        $("#rdoIsNewContractorYes").attr('disabled', false);
        $("#rdoIsNewContractorNo").attr('disabled', false);
     
        $("#rdoIsNewContractorNo").attr('checked', true);
    
    });

    $("#rdoIsNewContractorYes").change(function () {

        $('#ddlProposalWorks').val('0');

        $('#MANE_AGREEMENT_NUMBER').attr('readonly', 'readonly');
        $('#MANE_AGREEMENT_DATE').attr('readonly', 'readonly');
        $('#ddlContractors').attr('disabled', true);
        $('#MANE_YEAR1_AMOUNT').attr('readonly', 'readonly');
        $('#MANE_YEAR2_AMOUNT').attr('readonly', 'readonly');
        $('#MANE_YEAR3_AMOUNT').attr('readonly', 'readonly');
        $('#MANE_YEAR4_AMOUNT').attr('readonly', 'readonly');
        $('#MANE_YEAR5_AMOUNT').attr('readonly', 'readonly');
        $('#tdAgreementDate .ui-datepicker-trigger').hide();
    
        if ($('#ddlProposalWorks option').length == 1) {

            IsNewContractorYes();
        }

    });

    $('#ddlProposalWorks').change(function () {
    
        if ($('#ddlProposalWorks option:selected').val() > 0) {
            IsNewContractorYes();
        }
        else {
            ClearDetails();
        }

    });

    $("#divPanSearch").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Contractor Search'
    });

    $("#searchContractor").click(function () {

        $("#divPanSearch").load('/Agreement/SearchContractorByPan');
        $("#divPanSearch").dialog('open');
    });


});


function clearValidation(formElement) {

    alert('a');
    var id = $(formElement).attr('id');

    //Removes validation from input-fields
    $("#"+id + "input-validation-error").addClass('input-validation-valid');
    $("#" + id + "input-validation-error").removeClass('input-validation-error');
    //Removes validation message after input-fields
    $("#" + id + "field-validation-error").addClass('field-validation-valid');
    $("#" + id + "field-validation-error").removeClass('field-validation-error');
    //Removes validation summary
    $("#" + id + "validation-summary-errors").addClass('validation-summary-valid');
    $("#" + id + "validation-summary-errors").removeClass('validation-summary-errors');

}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    if (dropdown == '#ddlContractors') {
        message = '<h4><label style="font-weight:normal"> Loading Contractors... </label></h4>';
    }

    $(dropdown).empty(); 
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();

} 

function ClearDetails() {
    $('#MANE_YEAR1_AMOUNT').val('');
    $('#MANE_YEAR2_AMOUNT').val('');
    $('#MANE_YEAR3_AMOUNT').val('');
    $('#MANE_YEAR4_AMOUNT').val('');
    $('#MANE_YEAR5_AMOUNT').val('');
    $('#MANE_AGREEMENT_NUMBER').val('');
    $('#MANE_AGREEMENT_DATE').val('');
    $('#ddlContractors').val('0');
    $('#ddlProposalWorks').val('0');

    $('#MANE_AGREEMENT_NUMBER').removeAttr('readonly');
    $('#MANE_AGREEMENT_DATE').removeAttr('readonly');
    $('#ddlContractors').attr('disabled', false);
    $('#MANE_YEAR1_AMOUNT').removeAttr('readonly');
    $('#MANE_YEAR2_AMOUNT').removeAttr('readonly');
    $('#MANE_YEAR3_AMOUNT').removeAttr('readonly');
    $('#MANE_YEAR4_AMOUNT').removeAttr('readonly');
    $('#MANE_YEAR5_AMOUNT').removeAttr('readonly');
    $('#tdAgreementDate .ui-datepicker-trigger').show();
}
