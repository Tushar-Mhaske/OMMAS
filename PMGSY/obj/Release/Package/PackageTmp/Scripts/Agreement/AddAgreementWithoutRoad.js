
/*jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {


    if (new Date($('#TEND_AGREEMENT_END_DATE').val()) < new Date($('#TEND_AGREEMENT_START_DATE').val())) {

        return false;
    }
    else {

        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");*/

//jQuery.validator.addMethod("maintenancedatevalidator", function (value, element, param) {

//    if ($('#TEND_DATE_OF_COMPLETION').val() != '' && $('#TEND_DATE_OF_COMMENCEMENT').val() != '') {

//        if (new Date($('#TEND_DATE_OF_COMPLETION').val().split('/')[2], $('#TEND_DATE_OF_COMPLETION').val().split('/')[1], $('#TEND_DATE_OF_COMPLETION').val().split('/')[0]) > new Date($('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[2], $('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[1], $('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[0])) {

//            return false;
//        }
//        else {

//            return true;
//        }
//    }
//    else {
//        return true;
//    }
//});

//jQuery.validator.unobtrusive.adapters.addBool("maintenancedatevalidator");

$.validator.unobtrusive.adapters.add('maintenancedatevalidator', ['date'], function (options) {
    options.rules['maintenancedatevalidator'] = options.params;
    options.messages['maintenancedatevalidator'] = options.message;
});


jQuery.validator.addMethod("maintenancedatevalidator", function (value, element, param) {


    element = this.validationTargetFor(this.clean(element));

    if (element.name == 'TEND_DATE_OF_COMPLETION') {
        if ($('#TEND_DATE_OF_COMPLETION').val() != '' && $('#TEND_DATE_OF_COMMENCEMENT').val() != '') {

            if (new Date($('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[2], $('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[1], $('#TEND_DATE_OF_COMMENCEMENT').val().split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {

                return false;
            }
            else {

                return true;
            }
        }

    }

    if (element.name == 'TEND_AGREEMENT_END_DATE') {
        if ($('#TEND_DATE_OF_COMPLETION').val() != '' && $('#TEND_AGREEMENT_END_DATE').val() != '') {


            if (new Date($('#TEND_DATE_OF_COMPLETION').val().split('/')[2], $('#TEND_DATE_OF_COMPLETION').val().split('/')[1], $('#TEND_DATE_OF_COMPLETION').val().split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {

                return false;
            }
            else {

                return true;
            }
        }

    }
    return true;

});

$.validator.unobtrusive.adapters.add('datecomparefieldvalidator', ['date'], function (options) {
    options.rules['datecomparefieldvalidator'] = options.params;
    options.messages['datecomparefieldvalidator'] = options.message;
});

$.validator.addMethod("datecomparefieldvalidator", function (value, element, params) {

    //var propertName = params.date;
    //var propertValue = $('#' + params.date).val();

    //alert(propertName + '--' + propertValue + '--' + value);
    var compareDate = $("#" + params.date).val();

    if (params.date == "TEND_DATE_OF_AGREEMENT") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }

    if (params.date == "TEND_AGREEMENT_START_DATE") {

        //alert(this.element);
        element = this.validationTargetFor(this.clean(element));
        //alert(element.name);

        if ($('#TEND_AGREEMENT_END_DATE').val() != '' && element.name == 'TEND_AGREEMENT_END_DATE') {

            if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;
            else
                return true;
        }

        if ($('#TEND_DATE_OF_COMMENCEMENT').val() != '' && element.name == 'TEND_DATE_OF_COMMENCEMENT') {

            if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;
            else
                return true;
        }
    }

    if (params.date == "TEND_DATE_OF_WORK_ORDER") {
        //if ($('#TEND_DATE_OF_WORK_ORDER').val() != '') {
        //    if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
        //        return false;
        //    else
        //        return true;
        //}
        //else
        //    return true;


        //alert(this.element);
        element = this.validationTargetFor(this.clean(element));
        //alert(element.name);

        if ($('#TEND_DATE_OF_WORK_ORDER').val() != '' && element.name == 'TEND_DATE_OF_AGREEMENT') {

            if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;
            else
                return true;
        }



        //if ($('#TEND_DATE_OF_WORK_ORDER').val() != '' && element.name == 'TEND_DATE_OF_AGREEMENT') {

        //    if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
        //        return false;
        //    else
        //        return true;
        //}

        //if ($('#TEND_DATE_OF_WORK_ORDER').val() != '' && element.name == 'TEND_DATE_OF_AWARD_WORK') {

        //    if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
        //        return false;
        //    else
        //        return true;
        //}
    }

    if (params.date == "TEND_DATE_OF_AWARD_WORK") {

        if ($('#TEND_DATE_OF_AWARD_WORK').val() != '' && $('#TEND_DATE_OF_WORK_ORDER').val() != '') {

            if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;
            else
                return true;
        }
    }

    return true;
});

$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmAddAgreementDetails'));

    $('#EncryptedAgreementType_Add').val($('#EncryptedAgreementType').val());

    $("#divPanSearch").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Contractor Search'
    });


    $('#TEND_DATE_OF_AWARD_WORK' + ',' + '#TEND_DATE_OF_WORK_ORDER' + ',' + '#TEND_DATE_OF_COMMENCEMENT' + ',' + '#TEND_DATE_OF_COMPLETION').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date',
        onSelect: function (selectedDate) {
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_DATE_OF_AGREEMENT').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Agreement Date',
        onSelect: function (selectedDate) {
            $("#TEND_AGREEMENT_START_DATE").datepicker("option", "minDate", selectedDate);
            $("#TEND_DATE_OF_WORK_ORDER").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_AGREEMENT_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Start Date',
        onSelect: function (selectedDate) {
            $("#TEND_AGREEMENT_END_DATE").datepicker("option", "minDate", selectedDate);
            $("#TEND_DATE_OF_AGREEMENT").datepicker("option", "maxDate", selectedDate);
            $("#TEND_DATE_OF_COMMENCEMENT").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_AGREEMENT_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'End Date',
        onSelect: function (selectedDate) {
            $("#TEND_AGREEMENT_START_DATE").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#btnSaveAgreementDetails').click(function (e) {


        if ($('#frmAddAgreementDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Agreement/AddAgreementDetails_WithoutRoad",
                type: "POST",
                //dataType: "json",
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {


                    if (data.success == true) {
                        alert(data.message);
                        $('#btnResetAgreementDetails').trigger('click');

                        $('#tbAgreementList').jqGrid("setGridParam", { "postData": { AgreementType: $('#EncryptedAgreementType').val(), agreementYear: "0", status: "0" } });
//                        $('#tbAgreementList').jqGrid("setGridParam", { "postData": { AgreementType: $('#EncryptedAgreementType').val() } });
                        $('#tbAgreementList').trigger('reloadGrid', [{ page: 1 }]);

                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $('#dvAgreementDetails').html(data);
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

    $('#btnResetAgreementDetails').click(function (e) {
        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }
    });


    $("#btnUpdateAgreementDetails").click(function (e) {


        if ($("#frmAddAgreementDetails").valid()) {

            var conSupFlag = $('#Mast_Con_Sup_Flag').val();
            $("#ddlContractors").attr("disabled", false);


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Agreement/EditAgreementMasterDetails_WithoutRoad',
                async: false,
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);


                        if (conSupFlag == "S") {
                            $('#dvAgreementDetails').load('/Agreement/AddAgreementWithoutRoad/', function () {
                                $('#Mast_Con_Sup_Flag').val(conSupFlag);
                                $.unblockUI();
                            });
                        }
                        else if (conSupFlag == "C") {
                            $('#dvAgreementDetails').load('/Agreement/AddAgreementWithoutRoad_DPR/', function () {
                                $('#Mast_Con_Sup_Flag').val(conSupFlag);
                                $.unblockUI();
                            });
                        }


                        //$('#dvAgreementDetails').load('/Agreement/AddAgreementWithoutRoad/', function () {
                        //    $('#Mast_Con_Sup_Flag').val(conSupFlag);
                        //    $.unblockUI();
                        //});


                        $('#tbAgreementList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlContractors").attr("disabled", true);
                        }
                    }
                    else {
                        //$('#dvAgreementDetails').html(data);
                        $("#dvAgreementDetails").html(data);
                        $("#dvAgreementDetails").show('slow');
                        $("#TEND_AGREEMENT_NUMBER").focus();
                        $('#Mast_Con_Sup_Flag').val(conSupFlag);
                        $("#ddlContractors").attr("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });


    $("#btnCancelAgreementDetails").click(function (e) {

        var conSupFlag = $('#Mast_Con_Sup_Flag').val();
        var URL = '';
        if (Mast_Con_Sup_Flag == "S") {
            URL = 'AddAgreementWithoutRoad';
        }
        else if (Mast_Con_Sup_Flag == "D") {
            URL = 'AddAgreementWithoutRoad_DPR';
        }

        $.ajax({
            url: "/Agreement/"+URL,
            type: "GET",
            dataType: "html",
            success: function (data) {

                $("#dvAgreementDetails").html(data);
                $("#dvAgreementDetails").show();
                $('#Mast_Con_Sup_Flag').val(conSupFlag);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });

    })

    $("#searchContractor").click(function () {

        $("#divPanSearch").load('/Agreement/SearchContractorByPan');
        $("#divPanSearch").dialog('open');
    });


});

function FillInCascadeDropdown(map, dropdown, action) {

    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';
    if (dropdown == '#ddlProposalWorks') {
        message = '<h4><label style="font-weight:normal"> Loading Proposal Works... </label></h4>';
    }
    else {
        message = '<h4><label style="font-weight:normal"> Loading Agreements... </label></h4>';
    }

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()