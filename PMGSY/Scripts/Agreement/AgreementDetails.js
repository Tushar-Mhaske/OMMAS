﻿jQuery.validator.addMethod("customrequired", function (value, element, param) {

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

jQuery.validator.unobtrusive.adapters.addBool("customrequired");

$.validator.unobtrusive.adapters.add('datecomparefieldvalidator', ['date'], function (options) {
    options.rules['datecomparefieldvalidator'] = options.params;
    options.messages['datecomparefieldvalidator'] = options.message;
});

//$.validator.addMethod("datecomparefieldvalidator", function (value, element, params) {

//    //var propertName = params.date;
//    //var propertValue = $('#' + params.date).val();

//    //alert(propertName + '--' + propertValue + '--' + value);


//    if (params.date == "TEND_DATE_OF_AGREEMENT") {
//        if (new Date($("#" + params.date).val()) > new Date(value))
//            return false;
//        else
//            return true;
//    }

//    if (params.date == "TEND_AGREEMENT_START_DATE") {

//        //alert(this.element);
//        element=this.validationTargetFor(this.clean(element));
//        //alert(element.name);

//        if ($('#TEND_AGREEMENT_END_DATE').val() != '' && element.name == 'TEND_AGREEMENT_END_DATE') {

//            if (new Date($("#" + params.date).val()) > new Date(value))
//                return false;
//            else
//                return true;
//        }

//        if ($('#TEND_DATE_OF_COMMENCEMENT').val() != '' && element.name == 'TEND_DATE_OF_COMMENCEMENT') {       
//            if (new Date($("#" + params.date).val()) > new Date(value))
//                return false;
//            else
//                return true;
//        }
//    }

//    if (params.date == "TEND_DATE_OF_WORK_ORDER") {
//        if ($('#TEND_DATE_OF_WORK_ORDER').val() != '') {
//            if (new Date($("#" + params.date).val()) >new Date(value))
//                return false;
//            else
//                return true;
//        }
//        else
//            return true;
//    }


//    return true;
//});


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

        //commented code if required uncomment
        element = this.validationTargetFor(this.clean(element));
        if ($('#TEND_DATE_OF_WORK_ORDER').val() != '' && element.name == 'TEND_DATE_OF_AGREEMENT' && compareDate != '') {

            if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {

                return false;
            }
            else {
                return true;
            }

        }


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



jQuery.validator.addMethod("comparefieldvalidator", function (value, element, param) {

    var startChainage = parseFloat($('#TEND_START_CHAINAGE').val());
    var endChainage = parseFloat($('#TEND_END_CHAINAGE').val());
    if (startChainage >= endChainage)
        return false;
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldvalidator");


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

    if (element.name == 'TEND_DATE_OF_AGREEMENT') {
        if ($('#SanctionedDate').val() != '' && $('#TEND_DATE_OF_AGREEMENT').val() != '') {


            //if (new Date($('#SanctionedDate').val().split('/')[2], $('#SanctionedDate').val().split('/')[1], $('#SanctionedDate').val().split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) { commented by Vikram as new Date function is converting wrong date
            if (new Date($('#SanctionedDate').val().split('/')[2] + "-" + $('#SanctionedDate').val().split('/')[1] + "-" + $('#SanctionedDate').val().split('/')[0]) > new Date(value.split('/')[2] + "-" + value.split('/')[1] + "-" + value.split('/')[0])) {
                return false;
            }
            else {

                return true;
            }
        }

    }

    return true;

});

//jQuery.validator.unobtrusive.adapters.addBool("maintenancedatevalidator");


//jQuery.validator.addMethod("customrequired", function (value, element, param) {

//    if ($('#TEND_START_CHAINAGE').val() == '' || $('#TEND_END_CHAINAGE').val() == '')
//        return false;
//    else
//        return true;
//});

//jQuery.validator.unobtrusive.adapters.addBool("customrequired");

$.validator.unobtrusive.adapters.add('chainagerequired', ['chainage'], function (options) {
    options.rules['chainagerequired'] = options.params;
    options.messages['chainagerequired'] = options.message;
});

jQuery.validator.addMethod("chainagerequired", function (value, element, params) {

    if (params.chainage == "TEND_START_CHAINAGE") {
        if ($('#TEND_START_CHAINAGE').val() != '' && $('#TEND_END_CHAINAGE').val() == '')
            return false;
        else
            return true;
    }

    if (params.chainage == "TEND_END_CHAINAGE") {
        if ($('#TEND_END_CHAINAGE').val() != '' && $('#TEND_START_CHAINAGE').val() == '')
            return false;
        else
            return true;
    }
});

//jQuery.validator.unobtrusive.adapters.addBool("endchainagerequired");

$(document).ready(function () {


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#divPanSearch").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Contractor Search'
    });

    var contractorID = 0;

    $.validator.unobtrusive.parse($('#frmAddAgreementDetails'));

    $('#EncryptedAgreementType_Add').val($('#EncryptedAgreementType').val());

    if ($('#ddlProposalWorks option').length > 1) {

        $('#tdtxtRemark').attr('colspan', '1');
        $("#ddlProposalWorks").find("option[value='0']").remove();
        $("#tdlblProposalWork").show('slow');
        $("#tdddlProposalWork").show('slow');
    }

    if ($('#AgreementType').val() == "O") {
        $('#trAgreementType').hide();
        $('#lblTenderAmount').hide();
        $('#tdTenderAmount').hide();
        $('#tdAgreementAmount').attr('colspan', '3');
        $('#trMainAmountYear1').hide();
        $('#trMainAmountYear2').hide();
        $('#trMainAmountYear3').hide();
        $('#trChainage').hide();

        $("#tdlblProposalWork").html('');
        $("#tdddlProposalWork").html('');
        $('#tdtxtRemark').attr('colspan', '3');

    }

    $("#TEND_AGREEMENT_AMOUNT").trigger('blur');

    //if ($("#rdoIsPartAgreementYes").is(":checked")) {

    //    IsPartAgreementYes();
    //}

    //if ($("#rdoIsPartAgreementNo").is(":checked")) {
    //    IsPartAgreementNo();
    //}

    if ($('#RoleCode').val() == 36 || $('#RoleCode').val() == 47 || $('#RoleCode').val() == 56) {
        LoadAgreementDetailsITNO();
    }
    else {
        LoadAgreementDetails();
    }

    //alert($('#dvAgreementDetailsList').width());
    //$('#tbMapMPConstituencyBlockList').setGridWidth($('#dvAgreementDetailsList').width());

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
            $("#TEND_DATE_OF_AWARD_WORK").datepicker("option", "maxDate", selectedDate); //new change done by Vikram -- as the date of work order must be greater than or equal to the date of award work but the date of work order has no maxdate set.
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
            //$("#TEND_DATE_OF_COMPLETION").datepicker("option", "maxDate", selectedDate);

        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    // changes by rohit borse on 01-07-2022
    $('#rdoAPSCollectedNo').click(function (e) {
        $('#txtAPSCollectedAmount').prop('disabled', true).val('');
    });

    $('#rdoAPSCollectedYes').click(function (e) {
        $('#txtAPSCollectedAmount').prop('disabled', false).val('');
    });

    APS_CollectedChecks();

    // changes by rohit borse on 01-07-2022
    $('#btnSaveAgreementDetails').click(function (e) {

        //Added by rohit borse on 01-07-2022
        var isAPSCollected = $("input[name='APS_COLLECTED']:checked").val();
        var tempAPSCollectedAmount = $('#txtAPSCollectedAmount').val();


        if ($('#frmAddAgreementDetails').valid()) {

            //Added by rohit borse on 01-07-2022
            if (isAPSCollected != "N" & tempAPSCollectedAmount == "") {

                alert("If APS collected, amount is required !");
            }
            else {
                var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
                var workCode = $('#ddlProposalWorks').val();

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: "/Agreement/AddAgreementDetails",
                    type: "POST",
                    //  dataType: "json",
                    data: $("#frmAddAgreementDetails").serialize(),
                    success: function (data) {


                        if (data.success == true) {
                            $("#ddlProposalWorks").find("option[value='" + workCode + "']").remove();
                            alert(data.message);
                            //ClearDetails();
                            $('#btnResetAgreementDetails').trigger('click');

                            if ($('#ddlProposalWorks option').length == 0) {

                                $('#tdtxtRemark').attr('colspan', '3');
                                $("#tdlblProposalWork").hide();
                                $("#tdddlProposalWork").hide();

                                $('#dvAgreementDetails').hide();

                            }
                            //$("#dvAgreementDetails").load("/Agreement/AgreementDetails/" + encryptedIMSPRCode, function () {                         
                            //    $('#trAgreementType').show('slow');
                            //    $('#trEmptyAgreementType').show('slow');
                            //    $('#dvNewAgreement').show('slow');
                            //    $('#dvAgreementDetails').show('slow');
                            //    $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                            //});

                            $('#tbAgreementDetailsList').jqGrid("setGridParam", { "postData": { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val(), AgreementType: $('#EncryptedAgreementType').val() } });
                            $('#tbAgreementDetailsList').trigger('reloadGrid', [{ page: 1 }]);

                            CheckforActiveAgreement();

                        }
                        else if (data.success == false) {
                            //alert(data.message);
                            if (data.message != "") {
                                $('#message').html(data.message);
                                //('#dvErrorMessage').html(data.message);
                                $('#dvErrorMessage').show('slow');
                            }
                            $("#TEND_STATE_SHARE,#TEND_MORD_SHARE,#TEND_HIGHER_SPEC_AMT").trigger('blur');
                        }
                        else {
                            $('#dvAgreementDetails').html(data);

                            if ($('#AgreementType').val() == "C") {
                                $('#trAgreementType').show('slow');
                                $('#trEmptyAgreementType').show('slow');
                            }
                            else {
                                $('#trMainAmountYear1').hide();
                                $('#trMainAmountYear2').hide();
                                $('#trMainAmountYear3').hide();
                                $('#trChainage').hide();
                            }
                            $('#dvNewAgreement').show('slow');
                            $('#tblContractor').show('slow');
                            $('#dvNewExistingAgreement').hide();
                            $('#dvAgreementDetails').show('slow');

                        }
                        $.unblockUI();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }
                });
            }
        }
    });


    $("#btnUpdateAgreementDetails").click(function (e) {


        if ($("#frmAddAgreementDetails").valid()) {

            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
            $("#ddlContractors").attr("disabled", false);


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Agreement/EditAgreementMasterDetails/',
                async: false,
                data: $("#frmAddAgreementDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        $('#dvAgreementDetails').hide();
                        $("#dvAgreementDetails").load("/Agreement/AgreementDetails/" + encryptedIMSPRCode, function () {
                            //$("#dvAgreementDetails").show('slow');
                            //$('#dvNewAgreement').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                            //  $("#tblContractor").hide();
                        });

                        $('#tbAgreementDetailsList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlContractors").attr("disabled", true);
                        }
                        $("#TEND_STATE_SHARE,#TEND_MORD_SHARE,#TEND_HIGHER_SPEC_AMT").trigger('blur');
                    }
                    else {
                        //$('#dvAgreementDetails').html(data);

                        $("#dvAgreementDetails").html(data);

                        //$("#tdlblPartAgreement").hide();
                        //$("#tdrdoPartAgreement").hide();
                        //$('#trAgreementType').show('slow');
                        //$('#trEmptyAgreementType').show('slow');
                        $("#dvAgreementDetails").show('slow');
                        $('#dvNewAgreement').show('slow');
                        $("#ddlContractors").attr("disabled", true);
                        $('#trChainage').hide();
                        $('#tdtxtRemark').attr('colspan', '3');

                        $("#tdlblProposalWork").hide();
                        $("#tdddlProposalWork").hide();
                        $("#TEND_AGREEMENT_NUMBER").focus();
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
        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
        $.ajax({
            url: "/Agreement/AgreementDetails/" + encryptedIMSPRCode,
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvAgreementDetails").hide();
                $("#dvAgreementDetails").html(data);
                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);


            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });

    })

    $('#btnAgreementDetailsCancel').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $('#btnResetAgreementDetails').click(function (e) {
        contractorID = $('#ddlContractors').val();
        //e.preventDefault();
        ClearDetails();
    });


    function ClearDetails() {

        //if ($("#rdoIsPartAgreementYes").is(":checked")) {

        //    $("#TEND_START_CHAINAGE").val('');
        //    $("#TEND_END_CHAINAGE").val('');
        //}

        //if ($("#rdoIsPartAgreementNo").is(":checked")) {
        //    IsPartAgreementNo();
        //}
        //$('#ddlYears').val('0');
        //$('#MAST_HAB_TOT_POP').val('');
        //$('#MAST_HAB_SCST_POP').val('');
        //$('#rdoHasHabConnectedNo').attr('checked', true);
        //$('#rdoISPanchayatHQNo').attr('checked', true);
        //$('#rdoISSchemeNo').attr('checked', true);
        //$('#rdoHasPrimarySchoolNo').attr('checked', true);
        //$('#rdoHasMiddleSchoolNo').attr('checked', true);
        //$('#rdoHasHighSchoolNo').attr('checked', true);
        //$('#rdoHasIntermediateSchoolNo').attr('checked', true);
        //$('#rdoHasDegreeCollegeNo').attr('checked', true);


        //$('#rdoHasHealthServiceNo').attr('checked', true);
        //$('#rdoHasDespensaryNo').attr('checked', true);
        //$('#rdoHasPHCSNo').attr('checked', true);
        //$('#rdoHasVetnaryHospitalNo').attr('checked', true);
        //$('#rdoHasMCWCentersNo').attr('checked', true);
        //$('#rdoHasTelegraphOfficeNo').attr('checked', true);

        //$('#rdoHasTelephoneConnectionNo').attr('checked', true);
        //$('#rdoHasBusServiceNo').attr('checked', true);
        //$('#rdoHasRailwayStationNo').attr('checked', true);
        //$('#rdoHasElectricityNo').attr('checked', true);
        //$('#rdoIsTouristPlaceNo').attr('checked', true);

        //$("#trChainage").hide();
        //$("#trEmpty").hide();
        //$("#tdlblProposalWork").hide();
        //$("#tdddlProposalWork").hide();
        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }


    }

    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    //$("#rdoIsPartAgreementYes").change(function () {
    //    IsPartAgreementYes();

    //});



    //$("#rdoIsPartAgreementNo").change(function () {
    //    IsPartAgreementNo();
    //});




    $("#rdoAgreementTypeNew").change(function () {
        IsNewAgreement();

    });



    $("#rdoAgreementTypeExisting").change(function () {
        IsExistingAgreement();
    });



    $('#ddlContractors').change(function () {
        if ($('#ddlContractors option:selected').val() > 0) {

            if ($('#AgreementType').val() == "C") {
                $('#trAgreementType').show('slow');
                $('#trEmptyAgreementType').show('slow')
            }
            else {
                $('#trMainAmountYear1').hide();
                $('#trMainAmountYear2').hide();
                $('#trMainAmountYear3').hide();
                $('#trChainage').hide();
            }
            //$('#rdoAgreementTypeNew').attr('checked', true);
            //$("#tdlblAgreementNumber").hide('slow');
            //$("#tdddlAgreementNumber").hide('slow');
            //$('#tdAgreementType').attr('colspan', '3');

            if ($("#rdoAgreementTypeNew").is(":checked")) {
                $('#dvNewExistingAgreement').hide('slow');
                $('#dvNewAgreement').show('slow');

            }
            else if ($("#rdoAgreementTypeExisting").is(":checked")) {

                $('#dvNewExistingAgreement').hide('slow');
                FillInCascadeDropdown({ userType: $("#ddlContractors").find(":selected").val() },
                    "#ddlAgreementNumbers", "/Agreement/GetAgreementNumbersByContractor?contractorID=" + $('#ddlContractors option:selected').val() + "&agreementType=" + $('#EncryptedAgreementType').val());
            }
        }
        else {
            //$('#trAgreementType').hide('slow');
            //$('#trEmptyAgreementType').hide('slow');
            //$('#dvNewAgreement').hide('slow');
            $('#dvNewExistingAgreement').hide('slow');

            if ($("#rdoAgreementTypeExisting").is(":checked")) {

                FillInCascadeDropdown({ userType: $("#ddlContractors").find(":selected").val() },
                    "#ddlAgreementNumbers", "/Agreement/GetAgreementNumbersByContractor?contractorID=" + $('#ddlContractors option:selected').val() + "&agreementType=" + $('#EncryptedAgreementType').val());
            }
        }

    });


    $('#ddlAgreementNumbers').change(function () {
        if ($('#ddlAgreementNumbers option:selected').val() > 0 && $("#rdoAgreementTypeExisting").is(":checked")) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#dvNewAgreement').hide();
            $('#dvNewExistingAgreement').empty();
            $('#dvNewExistingAgreement').load('/Agreement/ExistingAgreement?contractorID=' + $('#ddlContractors option:selected').val() + '&agreementCode=' + $('#ddlAgreementNumbers option:selected').val() + '&IMSPRRoadCode=' + $('#EncryptedIMSPRRoadCode').val(), function () {
                $('#dvNewExistingAgreement').show('slow');
                $.unblockUI();
            });
        }
        else {
            if ($('#dvNewExistingAgreement').is(':visible')) {
                $('#dvNewExistingAgreement').hide('slow');
            }
        }
    });

    //$("#rdoIsPartAgreementYes").change(function () {
    //    IsPartAgreementYes();

    //});



    //$("#rdoIsPartAgreementNo").change(function () {

    //    IsPartAgreementNo();
    //});


    //new change done by Vikram on 3 Feb 2014

    $('#imgCloseProgressDetails').click(function () {

        //$("#tblContractor").hide("slow");
        $("#dvNewAgreementDetails").hide("slow");
        $("#divError").hide("slow");

        $('#message').html('');
        $('#dvErrorMessage').hide('slow');
    });

    //end of change


    //01 June 2016


    //get the supplier name details
    $("#MAST_CON_ID_S").change(function () {

        if ($("#MAST_CON_ID_S").val() != 0 || $("#MAST_CON_ID_S").val() != "") {
            // FillInCascadeDropdown(null, "#IMS_AGREEMENT_CODE_S", "/Payment/PopulateAgreement/" + $("#MAST_CON_ID_S").val());
            setContractorSupplierName($("#MAST_CON_ID_S").val(), "S", true);

        } else {

            $("#PAYEE_NAME_S").val("");

        }
    });




    //get the contractor name details
    $("#ddlContractors").change(function () {
        if ($("#ddlContractors").val() != 0 && $("#ddlContractors").val() != "") {
           
            setContractorSupplierName($("#ddlContractors").val(), "C", true);

           
            getContratorBankDetails($("#ddlContractors").val());

        }

        else {

            $("#PAYEE_NAME_C").val("");

           
            $("#trContractorBankDetails").hide();
            $("#spnBankAccNumber").html("-");
            $("#spnIFSCCode").html("-");
            $("#spnBankName").html("-");

        }
    });

    // Get Bank Details
    function getContratorBankDetails(mastConID) {
        $.ajax({
            type: "POST",
            url: "/Agreement/GetContratorBankNameAccNoAndIFSCcode/" + mastConID,
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                $("#errorSpan").show('slow');
                return false;

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (data.Success == true) {
                    $("#spnBankAccNumber").html(data.BankAccNumber);
                    $("#spnIFSCCode").html(data.BankIFSCCode);
                    $("#spnBankName").html(data.BankName);
                    $("#trContractorBankDetails").show();
                }
                else if (data.Success == false) {
                    //alert("Contractor bank details not present.");
                    $("#spnBankAccNumber").html("-");
                    $("#spnIFSCCode").html("-");
                    $("#spnBankName").html("-");


                }
                else {
                    alert("An error ocured while proccessing your request.");
                    $("#spnBankAccNumber").html("-");
                    $("#spnIFSCCode").html("-");
                    $("#spnBankName").html("-");

                }
            }
        });

    }


    //function to get the contractor supplier name according to id
    function setContractorSupplierName(contractorId, contractorOrsupllier, disableName) {
        $.ajax({
            type: "POST",
            url: "/Agreement/GetContractorSupplierName/" + contractorId,
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                $("#errorSpan").show('slow');
                return false;

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (data != "") {
                    if (contractorOrsupllier == "C") {
                        $("#PAYEE_NAME_C").val(data);
                        if (disableName) {
                            $("#PAYEE_NAME_C").attr('readonly', 'readonly');
                        } else {
                            $("#PAYEE_NAME_C").removeAttr('readonly');
                        }
                    } else {

                        $("#PAYEE_NAME_S").val(data);
                        if (disableName) {
                            $("#PAYEE_NAME_S").attr('readonly', 'readonly');
                        } else {
                            $("#PAYEE_NAME_S").RemoveAttr('readonly');
                        }
                    }
                }
            }
        });

    }

    //



    $("#TEND_STATE_SHARE,#TEND_MORD_SHARE,#TEND_HIGHER_SPEC_AMT").blur(function () {

        var stateAmount = 0.0;
        var mordAmount = 0.0;
        var specAmount = 0.0;

        if (Number($("#TEND_STATE_SHARE").val()) != NaN) {

            stateAmount = Number($("#TEND_STATE_SHARE").val());
        }

        if (Number($("#TEND_MORD_SHARE").val()) != NaN) {

            mordAmount = Number($("#TEND_MORD_SHARE").val());
        }

        if (Number($("#TEND_HIGHER_SPEC_AMT").val()) != NaN) {

            specAmount = Number($("#TEND_HIGHER_SPEC_AMT").val());
        }

        if (Number(stateAmount) != NaN && Number(mordAmount) != NaN) {
            var totalPercetageAmount = stateAmount + mordAmount;
        }
        else {
            totalPercetageAmount = 0.0;
        }

        var totalAmount = stateAmount + mordAmount;

        //$("#TEND_AGREEMENT_AMOUNT").attr('readonly',true);

        //$("#TEND_AGREEMENT_AMOUNT").val(totalAmount);

        if (Number(stateAmount) != NaN && Number(totalPercetageAmount) != NaN && totalPercetageAmount != 0 && stateAmount != 0) {
            $("#lblStateSharePercentage").html(Number(stateAmount / totalPercetageAmount * 100).toFixed(2) + " %");
        }

        if (Number(mordAmount) != NaN && Number(totalPercetageAmount) != NaN && totalPercetageAmount != 0 && mordAmount != 0) {
            $("#lblMordSharePercentage").html(parseFloat(mordAmount / totalPercetageAmount * 100).toFixed(2) + " %");
        }

    });

    //$("#TEND_AGREEMENT_AMOUNT").blur(function () {

    //    totalAgreementAmount = (Number($("#ProposalStateCost").val()) + Number($("#ProposalMordCost").val())).toFixed(2);
    //    if (Number(totalAgreementAmount) >= parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2))
    //    {
    //        if (Number(totalAgreementAmount) != NaN && Number($("#TEND_AGREEMENT_AMOUNT").val()) != NaN) {
    //            if ($("#ProposalStateShare").val() == 10) {
    //                $("#TEND_STATE_SHARE").val(parseFloat(parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) * 0.10).toFixed(2));
    //                $("#TEND_MORD_SHARE").val(parseFloat((parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) * 0.90)).toFixed(2));
    //            }
    //            else {
    //                $("#TEND_STATE_SHARE").val(parseFloat(parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) * 0.25).toFixed(2));
    //                $("#TEND_MORD_SHARE").val(parseFloat(parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) * 0.75).toFixed(2));
    //            }
    //        }
    //    }
    //    else if (Number(totalAgreementAmount) < parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2))
    //    {
    //        if (Number(totalAgreementAmount) != NaN && Number($("#TEND_AGREEMENT_AMOUNT").val()) != NaN) {
    //            if ($("#ProposalStateShare").val() == 10) {
    //                $("#TEND_STATE_SHARE").val(parseFloat(parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) - parseFloat($("#ProposalMordCost").val()).toFixed(2)).toFixed(2));
    //                $("#TEND_MORD_SHARE").val(parseFloat($("#ProposalMordCost").val()).toFixed(2));
    //            }
    //            else {
    //                $("#TEND_STATE_SHARE").val(parseFloat(parseFloat($("#TEND_AGREEMENT_AMOUNT").val()).toFixed(2) - parseFloat($("#ProposalMordCost").val()).toFixed(2)).toFixed(2));
    //                $("#TEND_MORD_SHARE").val(parseFloat($("#ProposalMordCost").val()).toFixed(2));
    //            }
    //        }
    //    }

    //});

    $("#searchContractor").click(function () {

        $("#divPanSearch").load('/Agreement/SearchContractorByPan');
        $("#divPanSearch").dialog('open');
    });

});

// changes by rohit borse on 01-07-2022
function APS_CollectedChecks() {

    if ($("input[name='APS_COLLECTED']:checked").val() == 'N') {
        $('#txtAPSCollectedAmount').prop('disabled', true).val('');
    }

    if ($("input[name='APS_COLLECTED']:checked").val() == 'Y') {
        $('#txtAPSCollectedAmount').prop('disabled', false);
    }
}

function LoadAgreementDetails() {

    var gridCaption = "";
    var isHidden = true;

    var isHiddenWorkName = false;
    var isHiddenPartAgreement = false;
    var isHiddenStartChainage = false;
    var isHiddenEndChainage = false;
    var isHiddenMaintenenceCost = false;


    if ($('#AgreementType').val() == "O") {

        isHidden = false;

        isHiddenWorkName = true;
        isHiddenPartAgreement = true;
        isHiddenStartChainage = true;
        isHiddenEndChainage = true;
        isHiddenMaintenenceCost = true;

        gridCaption = 'Other Agreement for the Road / LSB / Building List';
        $('#aHeading').text('Add Agreement Details for Other Road');
    }
    else {
        gridCaption = 'Road / LSB / Building Agreement List';

        $('#aHeading').text('Add Agreement Details for Road / LSB / Building');

        //alert($('#isCompleted').val());
        ///Changed by SAMMED A. PATIL on 03 OCTOBER 2017 to display column changestatustocomplete for all agreement types
        if ($('#isCompleted').val() == "True") {
            isHidden = false;
        }
    }

    jQuery("#tbAgreementDetailsList").jqGrid({
        url: '/Agreement/GetAgreementMasterDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val(), AgreementType: $('#EncryptedAgreementType').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Type', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Finalize', 'View', 'Edit', 'Delete'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: 50, align: "left", sortable: false, hidden: true },
                          //  { name: 'RoadName', index: 'RoadName', height: 'auto', width: 170, align: "left", sortable: false },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: 120, sortable: true, resizable: false },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 200, sortable: true, resizable: false },
                             { name: 'AgreementType', index: 'AgreementType', width: 120, sortable: true, align: "left", resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: 110, sortable: true, resizable: false },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 110, sortable: false, align: "right", resizable: false },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 110, sortable: false, align: "right", resizable: false, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 120, sortable: false, align: "left", resizable: false },
                           // { name: 'Change Status', index: 'Change Status', width: 90, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },
                            { name: 'Finalize', index: 'Finalize', width: 50, sortable: false, resize: false, align: "center", resizable: false, hidden: true }, /* formatter: FormatColumnFinalize,*/
                             { name: 'View', index: 'View', width: 50, sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
                           { name: 'Edit', index: 'Edit', width: 50, sortable: false, formatter: FormatColumnEdit, align: "center", resizable: false },
                            { name: 'Delete', index: 'Edit', width: 50, sortable: false, align: "center", formatter: FormatColumnDelete, resizable: false, hidden: true }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvAgreementDetailsListPager'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        // caption: "Agreement Details List",
        caption: gridCaption,
        height: 'auto',
        width: '1135',
        // autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        loadComplete: function () {

            var AgreementCode = $('#TendAgreementCode').val();

            // alert(AgreementCode);
            if (AgreementCode != '') {
                $("#tbAgreementDetailsList").expandSubGridRow(AgreementCode);
            }

            var reccount = $('#tbAgreementDetailsList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvAgreementDetailsListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms. 3."NA"-Not Available  ]');
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        subGrid: true,
        subGridRowExpanded: function (subgrid_id, row_id) {
            // we pass two parameters
            // subgrid_id is a id of the div tag created whitin a table data
            // the id of this elemenet is a combination of the "sg_" + id of the row
            // the row_id is the id of the row
            // If we wan to pass additinal parameters to the url we can use
            // a method getRowData(row_id) - which returns associative array in type name-value
            // here we can easy construct the flowing

            //alert(subgrid_id);

            //alert(row_id);
            //var b = getRowData(row_id);
            /* var a=$('#adminCategory').getRowData(row_id);
             alert(a['ADMIN_ND_NAME']);*/


            CollapseAllOtherRowsSubGrid(row_id);

            //alert(subgrid_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;

            //alert($('#tbAgreementDetailsList').getCell(row_id, 'AgreementCode'));

            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/Agreement/GetAgreementDetailsList_ByAgreementCode',
                //postData: { AgreementCode: row_id, IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                postData: { AgreementCode: $('#tbAgreementDetailsList').getCell(row_id, 'AgreementCode'), IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                datatype: "json",
                mtype: "POST",
                colNames: ['Year', 'Package', 'Road/Bridge Name', 'Work Name', 'Part Agreement', 'Start Chainage', 'End Chainage', 'Road / Bridge Amount', 'Maintenance Amount', 'Agreement Status', 'Value of Work Done', 'Incomplete Reason', 'Change Status To Complete', 'Change Status To Incomplete', 'Edit', 'Delete'],
                colModel: [
                             { name: 'Year', index: 'Year', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Package', index: 'Package', height: 'auto', width: 110, align: "left", sortable: false },
                             { name: 'RoadName', index: 'RoadName', height: 'auto', width: 110, align: "left", sortable: true },
                             { name: 'WorkName', index: 'WorkName', height: 'auto', width: 80, align: "left", sortable: false, hidden: isHiddenWorkName },
                             { name: 'PartAgreement', index: 'PartAgreement', height: 'auto', width: 60, align: "left", sortable: true, hidden: isHiddenPartAgreement },
                             { name: 'StartChainage', index: 'StartChainage', height: 'auto', width: 60, align: "right", sortable: false, hidden: isHiddenStartChainage },
                             { name: 'EndChainage', index: 'EndChainage', height: 'auto', width: 60, align: "right", sortable: false, hidden: isHiddenEndChainage },
                             { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 100, sortable: false, align: "right" },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 100, sortable: false, align: "right", hidden: isHiddenMaintenenceCost },
                             { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 100, sortable: false, align: "left" },
                             { name: 'WorkDone', index: 'WorkDone', height: 'auto', width: 50, align: "left", sortable: false },
                             { name: 'IncompleteReason', index: 'IncompleteReason', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Change Status To Complete', index: 'Change Status To Complete', width: 70, sortable: false, formatter: FormatColumnChangeStatusToComplete, align: "center", hidden: isHidden },
                             { name: 'Change Status', index: 'Change Status', width: 70, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },

                             { name: 'Edit', index: 'Edit', width: 40, sortable: false, formatter: FormatColumnEdit_AgreementDetails, align: "center" },
                             { name: 'Delete', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnDelete_AgreementDetails }

                ],
                rowNum: 5,
                pager: pager_id,
                height: 'auto',
                autowidth: true,
                rownumbers: true,
                rowList: [5, 10],
                viewrecords: true,
                sortname: 'RoadName',
                sortorder: "asc",
                recordtext: '{2} records found',
                onSelectRow: function () {
                    $('#TendAgreementCode').val(row_id);
                }

            });

        },

        subGridOptions: {
            "plusicon": "ui-icon-triangle-1-s",
            "minusicon": "ui-icon-triangle-1-n",
            "openicon": "ui-icon-arrowreturn-1-e",
            //expand all rows on load
            "expandOnLoad": false
        },

        onSelectRow: function (id) {

            $('#TendAgreementCode').val(id);
            // var userCode = $(this).jqGrid('getCell', 'userCode');
            //alert(userCode);
        }


    }); //end of grid
}

function LoadAgreementDetailsITNO() {

    var gridCaption = "";
    var isHidden = true;

    var isHiddenWorkName = false;
    var isHiddenPartAgreement = false;
    var isHiddenStartChainage = false;
    var isHiddenEndChainage = false;
    var isHiddenMaintenenceCost = false;


    if ($('#AgreementType').val() == "O") {

        isHidden = false;

        isHiddenWorkName = true;
        isHiddenPartAgreement = true;
        isHiddenStartChainage = true;
        isHiddenEndChainage = true;
        isHiddenMaintenenceCost = true;

        gridCaption = 'Other Agreement for the Road List';
        $('#aHeading').text('Add Agreement Details for Other Road');
    }
    else {
        gridCaption = 'Road Agreement List';

        $('#aHeading').text('Add Agreement Details for Road');


    }

    jQuery("#tbAgreementDetailsList").jqGrid({
        url: '/Agreement/GetAgreementMasterDetailsListITNO',
        datatype: "json",
        mtype: "POST",
        postData: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val(), AgreementType: $('#EncryptedAgreementType').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Type', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Finalize', 'View', 'Edit', 'Delete'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: 50, align: "left", sortable: false, hidden: true },
                          //  { name: 'RoadName', index: 'RoadName', height: 'auto', width: 170, align: "left", sortable: false },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: 120, sortable: true, resizable: false },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 200, sortable: true, resizable: false },
                             { name: 'AgreementType', index: 'AgreementType', width: 120, sortable: true, align: "left", resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: 110, sortable: true, resizable: false },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 110, sortable: false, align: "right", resizable: false },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 110, sortable: false, align: "right", resizable: false, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 120, sortable: false, align: "left", resizable: false },
                           // { name: 'Change Status', index: 'Change Status', width: 90, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },
                            { name: 'Finalize', index: 'Finalize', width: 50, sortable: false, resize: false, align: "center", resizable: false, hidden: true }, /* formatter: FormatColumnFinalize,*/
                             { name: 'View', index: 'View', width: 50, sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
                           { name: 'Edit', index: 'Edit', width: 50, sortable: false, formatter: FormatColumnEdit, align: "center", resizable: false },
                            { name: 'Delete', index: 'Edit', width: 50, sortable: false, align: "center", formatter: FormatColumnDelete, resizable: false, hidden: true }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvAgreementDetailsListPager'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        // caption: "Agreement Details List",
        caption: gridCaption,
        height: 'auto',
        width: '1135',
        // autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        loadComplete: function () {

            var AgreementCode = $('#TendAgreementCode').val();

            // alert(AgreementCode);
            if (AgreementCode != '') {
                $("#tbAgreementDetailsList").expandSubGridRow(AgreementCode);
            }

            var reccount = $('#tbAgreementDetailsList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvAgreementDetailsListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms. 3."NA"-Not Available  ]');
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        subGrid: true,
        subGridRowExpanded: function (subgrid_id, row_id) {
            // we pass two parameters
            // subgrid_id is a id of the div tag created whitin a table data
            // the id of this elemenet is a combination of the "sg_" + id of the row
            // the row_id is the id of the row
            // If we wan to pass additinal parameters to the url we can use
            // a method getRowData(row_id) - which returns associative array in type name-value
            // here we can easy construct the flowing

            //alert(subgrid_id);

            //alert(row_id);
            //var b = getRowData(row_id);
            /* var a=$('#adminCategory').getRowData(row_id);
             alert(a['ADMIN_ND_NAME']);*/


            CollapseAllOtherRowsSubGrid(row_id);

            //alert(subgrid_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;

            //alert($('#tbAgreementDetailsList').getCell(row_id, 'AgreementCode'));

            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/Agreement/GetAgreementDetailsListITNO_ByAgreementCode',
                //postData: { AgreementCode: row_id, IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                postData: { AgreementCode: $('#tbAgreementDetailsList').getCell(row_id, 'AgreementCode'), IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                datatype: "json",
                mtype: "POST",
                colNames: ['Year', 'Package', 'Road/Bridge Name', 'Work Name', 'Part Agreement', 'Start Chainage', 'End Chainage', 'Road / Bridge Amount', 'Maintenance Amount', 'Agreement Status', 'Value of Work Done', 'Incomplete Reason', 'Change Status To Complete', 'Change Status To Incomplete', 'Edit', 'Delete'],
                colModel: [
                             { name: 'Year', index: 'Year', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Package', index: 'Package', height: 'auto', width: 110, align: "left", sortable: false },
                             { name: 'RoadName', index: 'RoadName', height: 'auto', width: 110, align: "left", sortable: true },
                             { name: 'WorkName', index: 'WorkName', height: 'auto', width: 80, align: "left", sortable: false, hidden: isHiddenWorkName },
                             { name: 'PartAgreement', index: 'PartAgreement', height: 'auto', width: 60, align: "left", sortable: true, hidden: isHiddenPartAgreement },
                             { name: 'StartChainage', index: 'StartChainage', height: 'auto', width: 60, align: "right", sortable: false, hidden: isHiddenStartChainage },
                             { name: 'EndChainage', index: 'EndChainage', height: 'auto', width: 60, align: "right", sortable: false, hidden: isHiddenEndChainage },
                             { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 100, sortable: false, align: "right" },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 100, sortable: false, align: "right", hidden: isHiddenMaintenenceCost },
                             { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 100, sortable: false, align: "left" },
                             { name: 'WorkDone', index: 'WorkDone', height: 'auto', width: 50, align: "left", sortable: false },
                             { name: 'IncompleteReason', index: 'IncompleteReason', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Change Status To Complete', index: 'Change Status To Complete', width: 70, sortable: false, formatter: FormatColumnChangeStatusToComplete, align: "center", hidden: isHidden },
                             { name: 'Change Status', index: 'Change Status', width: 70, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },

                             { name: 'Edit', index: 'Edit', width: 40, sortable: false, formatter: FormatColumnEdit_AgreementDetails, align: "center" },
                             { name: 'Delete', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnDelete_AgreementDetails }

                ],
                rowNum: 5,
                pager: pager_id,
                height: 'auto',
                autowidth: true,
                rownumbers: true,
                rowList: [5, 10],
                viewrecords: true,
                sortname: 'RoadName',
                sortorder: "asc",
                recordtext: '{2} records found',
                onSelectRow: function () {
                    $('#TendAgreementCode').val(row_id);
                }

            });

        },

        subGridOptions: {
            "plusicon": "ui-icon-triangle-1-s",
            "minusicon": "ui-icon-triangle-1-n",
            "openicon": "ui-icon-arrowreturn-1-e",
            //expand all rows on load
            "expandOnLoad": false
        },

        onSelectRow: function (id) {

            $('#TendAgreementCode').val(id);
            // var userCode = $(this).jqGrid('getCell', 'userCode');
            //alert(userCode);
        }


    }); //end of grid
}

function FormatColumnEdit(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        //Below Code is Modified on 27-12-2021 to Block Edit column for itno login
        //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Agreement Details' onClick ='EditAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
        if (cellvalue === '36') {
            //alert(cellvalue);
            jQuery("#tbAgreementDetailsList").jqGrid('hideCol', ["Edit"]);
        }
        else {
            return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Agreement Details' onClick ='EditAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
        }
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
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Agreement Details' onClick ='DeleteAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


function FormatColumnEdit_AgreementDetails(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Agreement Details' onClick ='EditAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnDelete_AgreementDetails(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Agreement Details' onClick ='DeleteAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
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

//function FormatColumnChangeTerminatedStatus(cellvalue, options, rowObject) {

//    if (cellvalue != '') {
//        return "<center><table><tr><td style='border-color:white'><a href='#' title='Agreement Terminated' onClick ='ChangeTerminatedAgreementStatus(\"" + cellvalue.toString() + "\");'>Agreement Terminated</a></td> </tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
//    }

//}

function ViewSearchDiv() {
    // blockPage();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
            $('#ddlPackages').val(data.packageID);
        }

        $("#dvSearchProposedRoad").show('slow');
        //unblockPage();
        $.unblockUI();
    }
    $.unblockUI();

}


function IsNewAgreement() {

    if ($("#rdoAgreementTypeNew").is(":checked")) {


        $("#tdlblAgreementNumber").hide('slow');
        $("#tdddlAgreementNumber").hide('slow');
        $('#tdAgreementType').attr('colspan', '3');

        $('#dvNewExistingAgreement').hide();
        $('#dvNewAgreement').show('slow');
        //$("#tblNewAgreement").show('slow');
        $('#dvNewExistingAgreement').empty();


        //FillInCascadeDropdown({ userType: $("#ddlContractors").find(":selected").val() },
        //          "#ddlAgreementNumbers", "/Agreement/GetAgreementNumbersByContractor?contractorID=" + $('#ddlContractors option:selected').val());
    }
}

function IsExistingAgreement() {

    if ($("#rdoAgreementTypeExisting").is(":checked")) {
        // $("#tblNewAgreement").hide('slow');
        $('#dvNewAgreement').hide('slow');
        $('#tdAgreementType').attr('colspan', '1');

        $('#ddlAgreementNumbers').empty();
        FillInCascadeDropdown({ userType: $("#ddlContractors").find(":selected").val() },
                  "#ddlAgreementNumbers", "/Agreement/GetAgreementNumbersByContractor?contractorID=" + $('#ddlContractors option:selected').val() + "&agreementType=" + $('#EncryptedAgreementType').val());

        $("#tdlblAgreementNumber").show('slow');
        $("#tdddlAgreementNumber").show('slow');

    }
}



function EditAgreementMasterDetails(urlparameter) {

    var EncryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/EditAgreementMasterDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAgreementDetails").html(data);

            //$("#tdlblPartAgreement").hide();
            //$("#tdrdoPartAgreement").hide();
            $("#dvAgreementDetails").show('slow');
            $('#dvNewAgreement').show('slow');

            $("#TEND_AGREEMENT_NUMBER").focus();
            $('#EncryptedIMSPRRoadCode').val(EncryptedIMSPRCode);
            $('#trChainage').hide();
            // $('#trAddNewSearch').show();
            $("#TEND_STATE_SHARE,#TEND_MORD_SHARE,#TEND_HIGHER_SPEC_AMT").trigger('blur');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}


function ViewAgreementMasterDetails(urlparameter) {

    var EncryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/ViewAgreementMasterDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvViewAgreementMaster").html(data);

            $("#dvViewAgreementMaster").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}


function DeleteAgreementMasterDetails(urlparameter) {
    if (confirm("Are you sure you want to delete agreement details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Agreement/DeleteAgreementMasterDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tbAgreementDetailsList').trigger('reloadGrid');

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}


function EditAgreementDetails(urlparameter) {


    //alert('a');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/EditAgreementDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvNewExistingAgreement").html(data);


            $("#tblExistingAgreement").hide();
            $("#tblContractor").hide();
            $('#dvNewAgreement').hide();
            $("#dvAgreementDetails").show('slow');
            $("#dvNewExistingAgreement").show('slow');


            $("#TEND_AGREEMENT_NUMBER").focus();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}


function DeleteAgreementDetails(urlparameter) {
    if (confirm("Are you sure you want to delete agreement details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Agreement/DeleteAgreementDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tbAgreementDetailsList').trigger('reloadGrid');

                    if ($('#dvNewAgreement').is(':visible')) {
                        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
                        $("#dvAgreementDetails").load("/Agreement/AgreementDetails/" + encryptedIMSPRCode, function () {
                            if ($('#AgreementType').val() == "C") {
                                $('#trAgreementType').show('slow');
                                $('#trEmptyAgreementType').show('slow');
                            }
                            else {
                                $('#trMainAmountYear1').hide();
                                $('#trMainAmountYear2').hide();
                                $('#trMainAmountYear3').hide();
                                $('#trChainage').hide();
                            }
                            $('#dvNewAgreement').show('slow');
                            $('#tblContractor').show('slow');
                            $('#dvNewExistingAgreement').hide();
                            $('#dvAgreementDetails').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                        });
                    }

                    if ($('#dvNewExistingAgreement').is(':visible')) {
                        $('#btnCancelAgreementDetails_Existing').trigger('click');
                    }

                    CheckforActiveAgreement();

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

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

function IsPartAgreementYes() {

    if ($("#rdoIsPartAgreementYes").is(":checked")) {

        // $("#trChainage").show('slow');
        //$("#trEmpty").show('slow');

        FillInCascadeDropdown({ userType: $("#ddlContractors").find(":selected").val() },
                   "#ddlProposalWorks", "/Agreement/GetProposalWorksByIMSRoadCode/" + $('#EncryptedIMSPRRoadCode').val());

        $('#tdrdoPartAgreement').attr('colspan', '1');
        $("#tdlblProposalWork").show('slow');
        $("#tdddlProposalWork").show('slow');
    }
}

function IsPartAgreementNo() {

    if ($("#rdoIsPartAgreementNo").is(":checked")) {

        // $("#trChainage").hide('slow');
        // $("#trEmpty").hide('slow');
        // $("#TEND_START_CHAINAGE").val('');
        // $("#TEND_END_CHAINAGE").val('');

        $("#tdlblProposalWork").hide();
        $("#tdddlProposalWork").hide();
        $('#tdrdoPartAgreement').attr('colspan', '3');


    }
}

function CollapseAllOtherRowsSubGrid(rowid) {
    var rowIds = $("#tbAgreementDetailsList").getDataIDs();
    $.each(rowIds, function (index, rowId) {
        $("#tbAgreementDetailsList").collapseSubGridRow(rowId);
    });
}

function FinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/FinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementDetailsList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }
                    else if ($('#btnUpdateAgreementDetails_Existing').is(':visible')) {

                        $("#btnCancelAgreementDetails_Existing").trigger('click');
                    }
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function ChangeAgreementStatusToInComplete(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvIncompleteReason').empty();
    $("#dvIncompleteReason").load("/Agreement/IncompleteReason/" + urlparameter, function () {

        $("#dvIncompleteReason").dialog('open');
        $.unblockUI();
    })

}


function ChangeAgreementStatusToComplete(urlparameter) {

    if (confirm("Are you sure you want to 'Complete' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/ChangeAgreementStatusToComplete/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementDetailsList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }

                    CheckforActiveAgreement();
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function ChangeTerminatedAgreementStatus(urlparameter) {

    if (confirm("Are you sure you want to convert the agreement from 'Terminated' to 'In Progress' ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/ChangeTerminatedAgreementStatus/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);

                    $("#tbAgreementDetailsList").trigger('reloadGrid');
                    $("#tbAgreementDetailsList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
                    }

                    //CheckforActiveAgreement();
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function ChangeTerminatedAgreementMasterStatus(urlparameter) {

    //if (confirm("Are you sure you want to convert the agreement from 'Terminated' to 'In Progress' ?")) {
    if (confirm("Are you sure you want to convert the agreement to 'In Progress' ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/ChangeTerminatedAgreementMasterStatus/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);

                    $("#tbAgreementDetailsList").trigger('reloadGrid');
                    //$("#tbAgreementDetailsList").trigger('reloadGrid');

                    //if ($('#btnUpdateAgreementDetails').is(':visible')) {

                    //    $("#btnCancelAgreementDetails").trigger('click');
                    //}

                    //CheckforActiveAgreement();
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}
