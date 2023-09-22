jQuery.validator.addMethod("custommanerequired", function (value, element, param) {

    if ($("#PMGSYScheme").val() == 2) {
        if ($('#' + element.id).val() == "") {
            return false;
        }
    }
    else {

        return true;
    }

    return true;

});

jQuery.validator.unobtrusive.adapters.addBool("custommanerequired");


jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {


    if (new Date($('#MANE_MAINTENANCE_START_DATE').val().split('/')[2], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[1], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[0]) < new Date($('#MANE_AGREEMENT_DATE').val().split('/')[2], $('#MANE_AGREEMENT_DATE').val().split('/')[1], $('#MANE_AGREEMENT_DATE').val().split('/')[0])) {

        return false;
    }
    else {

        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");


jQuery.validator.addMethod("checkdatevalidator", function (value, element, param) {


    if (new Date($('#MANE_HANDOVER_DATE').val().split('/')[2], $('#MANE_HANDOVER_DATE').val().split('/')[1], $('#MANE_HANDOVER_DATE').val().split('/')[0]) < new Date($('#MANE_MAINTENANCE_START_DATE').val().split('/')[2], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[1], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[0]) || new Date($('#MANE_HANDOVER_DATE').val().split('/')[2], $('#MANE_HANDOVER_DATE').val().split('/')[1], $('#MANE_HANDOVER_DATE').val().split('/')[0]) < new Date($('#MANE_CONSTR_COMP_DATE').val().split('/')[2], $('#MANE_CONSTR_COMP_DATE').val().split('/')[1], $('#MANE_CONSTR_COMP_DATE').val().split('/')[0])) {

        return false;
    }
    else {

        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("checkdatevalidator");

jQuery.validator.addMethod("maintenancedatevalidator", function (value, element, param) {


    if (new Date($('#MANE_MAINTENANCE_START_DATE').val().split('/')[2], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[1], $('#MANE_MAINTENANCE_START_DATE').val().split('/')[0]) < new Date($('#MANE_CONSTR_COMP_DATE').val().split('/')[2], $('#MANE_CONSTR_COMP_DATE').val().split('/')[1], $('#MANE_CONSTR_COMP_DATE').val().split('/')[0])) {

        return false;
    }
    else {

        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("maintenancedatevalidator");

$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmAddAgreementDetails'));

    if ($('#ddlProposalWorks option').length > 1) {
        //$("#tdlblProposalWork").show('slow');
        //$("#tdddlProposalWork").show('slow');
    }


    if (isNewEntry == 'False') {
        $('#tdIsNewContractor').hide();
    }

    LoadAgreementDetails();

    $('#MANE_AGREEMENT_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Agreement Date',
        onSelect: function (selectedDate) {
            $("#MANE_MAINTENANCE_START_DATE").datepicker("option", "minDate", selectedDate);
            $("#MANE_CONSTR_COMP_DATE").datepicker("option", "maxDate", selectedDate);
            $("#MANE_HANDOVER_DATE").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    if ($('#ExecutionCompleted').val() == "Y") {
        //$('#MANE_CONSTR_COMP_DATE').datepicker({

        //    dateFormat: 'dd/mm/yy',
        //    showOn: "button",
        //    buttonImage: "/Content/Images/calendar_2.png",
        //    buttonImageOnly: true,
        //    changeMonth: true,
        //    changeYear: true,
        //    buttonText: 'Agreement Date',
        //    onSelect: function (selectedDate) {
        //    },
        //    onClose: function () {

        //        $(this).focus().blur();
        //    }
        //});
    }


    $('#MANE_MAINTENANCE_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Maintenance Start Date',
        onSelect: function (selectedDate) {
            if ($('#tdAgreementDate .ui-datepicker-trigger').is(':visible')) {
                $("#MANE_AGREEMENT_DATE").datepicker("option", "maxDate", selectedDate);
            }
            $("#MANE_CONSTR_COMP_DATE").datepicker("option", "maxDate", selectedDate);
            $("#MANE_HANDOVER_DATE").datepicker("option", "minDate", selectedDate);
            var startDate = selectedDate.split('/');
            var day = startDate[0];
            var month = startDate[1];
            var year = startDate[2];
            var nextYear = parseInt(year) + 5;
            var nextDate = day + "/" + month + "/" + nextYear;
            $('#MANE_MAINTENANCE_END_DATE').val(nextDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#MANE_MAINTENANCE_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Maintenance Start Date',
        onSelect: function (selectedDate) {

        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

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
            var workCode = $('#ddlProposalWorks').val();

            if ($("#rdoIsNewContractorYes").is(":checked")) {
                $('#ddlContractors').attr('disabled', false);
            }
            $.ajax({
                url: "/MaintenanceAgreement/AddRenewalAgreement",
                type: "POST",
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        $("#ddlProposalWorks").find("option[value='" + workCode + "']").remove();
                        alert(data.message);
                        ReloadAddAgreemntDetails();
                        $('#tbAgreementList').jqGrid("setGridParam", { "postData": { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() } });
                        $('#tbAgreementList').trigger('reloadGrid', [{ page: 1 }]);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        if ($("#rdoIsNewContractorYes").is(":checked")) {
                            $('#ddlContractors').attr('disabled', true);
                        }

                    }
                    else {
                        $('#dvAgreementDetails').html(data);
                        if ($("#rdoIsNewContractorYes").is(":checked")) {
                            $('#ddlContractors').attr('disabled', true);
                            $('#MANE_AGREEMENT_NUMBER').attr('readonly', 'readonly');
                            $('#MANE_AGREEMENT_DATE').attr('readonly', 'readonly');
                            $('#MANE_YEAR1_AMOUNT').attr('readonly', 'readonly');
                            $('#tdAgreementDate .ui-datepicker-trigger').hide();
                        }
                        $('#dvNewAgreement').show('slow');
                        $('#dvAgreementDetails').show('slow');
                    }


                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert("Error occured while processing your request.");
                    if ($("#rdoIsNewContractorYes").is(":checked")) {
                        $('#ddlContractors').attr('disabled', true);
                    }
                    $.unblockUI();
                }

            });

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



    $("#btnUpdateAgreementDetails").click(function (e) {


        if ($("#frmAddAgreementDetails").valid()) {

            $("#ddlContractors").attr("disabled", false);
            $("#ddlProposalWorks").attr("disabled", false);
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/MaintenanceAgreement/EditRenewalAgreementDetails',
                async: false,
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#dvAgreementDetails').load('/MaintenanceAgreement/RenewalAgreementDetails/' + encryptedIMSPRCode, function () {
                            $('#dvNewAgreement').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                            $('#dvAgreementDetails').hide();
                            $.unblockUI();
                        });

                        $('#tbAgreementList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlContractors").attr("disabled", true);
                            $("#ddlProposalWorks").attr("disabled", true);
                        }
                    }
                    else {

                        $("#dvAgreementDetails").html(data);
                        $('#dvNewAgreement').show('slow');
                        $("#dvAgreementDetails").show('slow');
                        $("#MANE_AGREEMENT_NUMBER").focus();
                        $("#ddlContractors").attr("disabled", true);
                        $("#ddlProposalWorks").attr("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert("Error occured while processing your request.");
                    $.unblockUI();
                }
            })
        }
    });

    $('#btnResetAgreementDetails').click(function (e) {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $('#MANE_AGREEMENT_NUMBER').removeAttr('readonly');
        $('#MANE_AGREEMENT_DATE').removeAttr('readonly');
        $('#ddlContractors').attr('disabled', false);
        $('#MANE_YEAR1_AMOUNT').removeAttr('readonly');
        $('#tdAgreementDate .ui-datepicker-trigger').show();
        $("#rdoIsNewContractorYes").attr('disabled', false);
        $("#rdoIsNewContractorNo").attr('disabled', false);
        $("#rdoIsNewContractorNo").attr('checked', true);

    });


    $("#btnCancelAgreementDetails").click(function (e) {

        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
        $.ajax({
            url: "/MaintenanceAgreement/SpecialAgreementDetails/" + encryptedIMSPRCode,
            type: "GET",
            dataType: "html",
            success: function (data) {

                $('#dvAgreementDetails').html(data);
                $('#dvNewAgreement').show('slow');
                $('#dvAgreementDetails').hide();
                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert("Error occured while processing your request.");
            }
        });
    })

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

    $("#rdoIsNewContractorNo").change(function () {
        IsNewContractorNo();
    });

    $('#ddlProposalWorks').change(function () {


        if ($('#ddlProposalWorks option:selected').val() > 0) {
            IsNewContractorYes();
        }
        else {
            ClearDetails();
        }
    });

});
function clearValidation(formElement) {

    var id = $(formElement).attr('id');
    //Removes validation from input-fields
    $("#" + id + "input-validation-error").addClass('input-validation-valid');
    $("#" + id + "input-validation-error").removeClass('input-validation-error');
    //Removes validation message after input-fields
    $("#" + id + "field-validation-error").addClass('field-validation-valid');
    $("#" + id + "field-validation-error").removeClass('field-validation-error');
    //Removes validation summary
    $("#" + id + "validation-summary-errors").addClass('validation-summary-valid');
    $("#" + id + "validation-summary-errors").removeClass('validation-summary-errors');
}
function IsNewContractorYes() {

    if ($("#rdoIsNewContractorYes").is(":checked")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/GetExistingAgreementDetails",
            cache: false,
            type: "GET",
            data: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val(), IMSWorkCode: $('#ddlProposalWorks option:selected').val() },
            success: function (data) {

                if (data.success == true) {

                    $('#MANE_AGREEMENT_NUMBER').val(data.agreementNumber);
                    $('#MANE_AGREEMENT_DATE').val(data.agreementDate);
                    $('#ddlContractors').val(data.contractorID);
                    $('#MANE_YEAR1_AMOUNT').val(data.year1);

                    $('#MANE_AGREEMENT_NUMBER').attr('readonly', 'readonly');
                    $('#MANE_AGREEMENT_DATE').attr('readonly', 'readonly');
                    $('#ddlContractors').attr('disabled', true);
                    $('#MANE_YEAR1_AMOUNT').attr('readonly', 'readonly');
                    $('#tdAgreementDate .ui-datepicker-trigger').hide();

                }
                else if (data.success == false) {
                    if (data.message != "") {
                        alert(data.message);
                        $("#rdoIsNewContractorNo").attr('checked', true);
                        $("#rdoIsNewContractorYes").attr('disabled', false);
                        $("#rdoIsNewContractorNo").attr('disabled', false);

                        $('#MANE_AGREEMENT_NUMBER').removeAttr('readonly');
                        $('#MANE_AGREEMENT_DATE').removeAttr('readonly');
                        $('#ddlContractors').attr('disabled', false);
                        $('#MANE_YEAR1_AMOUNT').removeAttr('readonly');
                        $('#tdAgreementDate .ui-datepicker-trigger').show();
                    }
                }
                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occured while processing your request.");
                $.unblockUI();
            }

        });
    }
}

function IsNewContractorNo() {

    if ($("#rdoIsNewContractorNo").is(":checked")) {

        ClearDetails();
    }
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

    $('#MANE_AGREEMENT_NUMBER').val('');
    $('#MANE_AGREEMENT_DATE').val('');
    $('#ddlContractors').val('0');
    $('#ddlProposalWorks').val('0');

    $('#MANE_AGREEMENT_NUMBER').removeAttr('readonly');
    $('#MANE_AGREEMENT_DATE').removeAttr('readonly');
    $('#ddlContractors').attr('disabled', false);
    $('#MANE_YEAR1_AMOUNT').removeAttr('readonly');

    $('#tdAgreementDate .ui-datepicker-trigger').show();
}

function LoadAgreementDetails() {


    jQuery("#tbAgreementList").jqGrid({
        url: '/MaintenanceAgreement/GetRenewalAgreementDetailsList_Proposal',
        datatype: "json",
        mtype: "GET",
        postData: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
        colNames: ['Agreement Number', 'Work', 'Contractor Name', 'Agreement Date', 'Maintenance Start Date', 'Maintenance Amount', 'Agreement Status', 'Change Status To Complete', 'Change Status To Incomplete', 'Finalize', 'DeFinalize', 'View', 'Edit', 'Delete'],
       // colNames: ['Agreement Number', 'Work', 'Contractor Name', 'Agreement Date', 'Maintenance Start Date', 'Maintenance Amount', 'Agreement Status', 'Finalize', 'DeFinalize', 'View', 'Edit', 'Delete'],

        colModel: [
                           { name: 'AgreementNumber', index: 'AgreementNumber', width: 100, sortable: true },
                           { name: 'Work', index: 'Work', height: 'auto', width: 80, sortable: true },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 100, sortable: true, },
                           { name: 'AgreementDate', index: 'AgreementDate', width: 90, sortable: true },
                           { name: 'MaintenanceDate', index: 'MaintenanceDate', height: 'auto', width: 90, sortable: true, align: "left" },
                           { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 90, sortable: false, align: "right" },
                           { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 90, sortable: false, align: "left" },

                           { name: 'Change Status To Complete', index: 'Change Status To Complete', width: 80, sortable: false, formatter: FormatColumnChangeStatusToComplete, align: "center" },
                           { name: 'Change Status', index: 'Change Status', width: 80, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },

                           { name: 'Finalize', index: 'Finalize', width: 50, sortable: false, resize: false, align: "center" }, /* formatter: FormatColumnFinalize,*/
                           { name: 'DeFinalize', index: 'DeFinalize', width: 50, sortable: false, resize: false, align: "center" },
                           { name: 'View', index: 'View', width: 40, sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
                           { name: 'Edit', index: 'Edit', width: 40, sortable: false, formatter: FormatColumnEdit, align: "center" },
                           { name: 'Delete', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnDelete }
        ],
        pager: jQuery('#dvAgreementListPager'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Renewal Maintenance Agreement Details List",
        height: 'auto',
        width: 1130,
        rownumbers: true,
        sortname: 'AgreementDate',
        sortorder: "desc",
        hidegrid: false,
        loadComplete: function () {

            var reccount = $('#tbAgreementList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvAgreementListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs.]'); //2.All Lengths are in Kms. 
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}


function FormatColumnEdit(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Agreement Details' onClick ='EditAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function FormatColumnView(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Agreement Details' onClick ='ViewAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}



function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Agreement Details' onClick ='DeleteAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function EditAgreementDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/MaintenanceAgreement/EditRenewalAgreementDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAgreementDetails").html(data);
            $('#dvNewAgreement').show('slow');
            $('#dvAgreementDetails').show('slow');
            $('#MANE_AGREEMENT_NUMBER').attr('readonly', 'readonly');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error occured while processing your request.");
            $.unblockUI();
        }
    })
}



function ViewAgreementMasterDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/MaintenanceAgreement/ViewRenewalAgreementDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvViewAgreementMaster").html(data);

            $("#dvViewAgreementMaster").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error occured while processing your request.");
            $.unblockUI();
        }
    })
}

function DeleteAgreementDetails(urlparameter) {
    if (confirm("Are you sure you want to delete agreement details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/MaintenanceAgreement/DeleteMaintenanceAgreementDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            data: { __RequestVerificationToken: $('#frmAddAgreementDetails input[name=__RequestVerificationToken]').val() },
            success: function (data) {
                if (data.success) {
                    alert(data.message);


                    $('#tbAgreementList').trigger('reloadGrid');

                    if ($('#dvNewAgreement').is(':visible')) {
                        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
                        $("#dvAgreementDetails").load("/MaintenanceAgreement/SpecialAgreementDetails/" + encryptedIMSPRCode, function () {
                            $('#dvNewAgreement').show('slow');
                            $('#dvAgreementDetails').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                        });
                    }

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert("Error occured while processing your request.");
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

function FinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/FinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            data: { __RequestVerificationToken: $('#frmAddAgreementDetails input[name=__RequestVerificationToken]').val() },
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }

                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                //alert(xht.responseText);
                alert("Error occured while processing your request.");
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}
function DeFinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'DeFinalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/DeFinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            data: { __RequestVerificationToken: $('#frmAddAgreementDetails input[name=__RequestVerificationToken]').val() },
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }

                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                //alert(xht.responseText);
                alert("Error occured while processing your request.");
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function FormatColumnChangeStatusToComplete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><a href='#' title='Complete Agreement' onClick ='ChangeAgreementStatusToComplete(\"" + cellvalue.toString() + "\");'>Complete</a></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><a href='#' title='Incomplete Agreement' onClick ='ChangeAgreementStatusToInComplete(\"" + cellvalue.toString() + "\");'>Incomplete</a></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function ChangeAgreementStatusToInComplete(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvIncompleteReason').empty();
    $("#dvIncompleteReason").load("/MaintenanceAgreement/IncompleteReason/" + urlparameter, function () {

        $("#dvIncompleteReason").dialog('open');
        $.unblockUI();
    })
}
function ChangeAgreementStatusToComplete(urlparameter) {

    if (confirm("Are you sure you want to 'Complete' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/ChangeAgreementStatusToComplete/" + urlparameter,
            type: "POST",
            dataType: "json",
            data: { __RequestVerificationToken: $('#frmAddAgreementDetails input[name=__RequestVerificationToken]').val() },
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }
                    else if ($('#btnSaveAgreementDetails').is(':visible')) {
                        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

                        $.ajax({
                            url: "/MaintenanceAgreement/SpecialAgreementDetails/" + encryptedIMSPRCode,
                            type: "GET",
                            dataType: "html",
                            async: false,
                            success: function (data) {

                                $("#dvAgreementDetails").html(data);
                                $('#trAgreementType').show('slow');
                                $('#dvNewAgreement').show('slow');
                                $('#dvNewExistingAgreement').hide();
                                $('#dvAgreementDetails').show('slow');
                                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                            }
                        });
                    }
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                //alert(xht.responseText);
                alert("Error occured while processing your request.");
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function ReloadAddAgreemntDetails() {

    var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

    $.ajax({
        url: "/MaintenanceAgreement/RenewalAgreementDetails/" + encryptedIMSPRCode,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {

            $("#dvAgreementDetails").html(data);
            $('#trAgreementType').show('slow');
            $('#dvNewAgreement').show('slow');
            $('#dvNewExistingAgreement').hide();
            $('#dvAgreementDetails').show('slow');
            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.responseText);
            alert("Error occured while processing your request.");
        }

    });

}
