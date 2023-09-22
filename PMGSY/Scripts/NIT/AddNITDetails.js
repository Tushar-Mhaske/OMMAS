$.validator.unobtrusive.adapters.add('datecomparefieldvalidator', ['date'], function (options) {
    options.rules['datecomparefieldvalidator'] = options.params;
    options.messages['datecomparefieldvalidator'] = options.message;
});

$.validator.addMethod("datecomparefieldvalidator", function (value, element, params) {

    
    var compareDate = $("#" + params.date).val();

    if (params.date == "TEND_ISSUE_START_DATE") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "TEND_INSP_START_DATE") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "TEND_ISSUE_END_DATE") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "CurrentDate") {

     
        element = this.validationTargetFor(this.clean(element));
     
        compareDate = new Date();
        
        if ($('#TEND_PUBLICATION_DATE').val() != '' && element.name == 'TEND_PUBLICATION_DATE') {

            if (new Date(compareDate.getFullYear(), compareDate.getMonth() + 1, compareDate.getDate()) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))  
                return false;
            
            else      
                return true;
            
        }

        if ($('#TEND_ISSUE_START_DATE').val() != '' && element.name == 'TEND_ISSUE_START_DATE') {

            if (new Date(compareDate.getFullYear(), compareDate.getMonth() + 1, compareDate.getDate()) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;
            else
                return true;

        }
    }

   

    return true;
});


$.validator.unobtrusive.adapters.add('currentdatevalidator', ['date'], function (options) {
    options.rules['currentdatevalidator'] = options.params;
    options.messages['currentdatevalidator'] = options.message;
});

$.validator.addMethod("currentdatevalidator", function (value, element, params) {


    //var compareDate = $("#" + params.date).val();

    if (params.date == "CurrentDate") {

        compareDate = new Date();

        if ($('#CONT_REGN_VALIDITY_DATE').val() != '' && element.name == 'CONT_REGN_VALIDITY_DATE') {

            if (new Date(compareDate.getFullYear(), compareDate.getMonth() + 1, compareDate.getDate()) < new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;

            else
                return true;

        }
    }

    return true;
});


$(document).ready(function () {

    $('#aHeading').text('Add NIT Details');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmAddNITDetails'));

    $('#CONT_REGN_VALIDITY_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Contractor Registration Date',
        maxDate: new Date(),
        onSelect: function (selectedDate) {
            //$("#TEND_AGREEMENT_START_DATE").datepicker("option", "minDate", selectedDate);
            //$("#TEND_DATE_OF_WORK_ORDER").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_PUBLICATION_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        minDate:new Date(),
        buttonText: 'NIT Publication Date',
        onSelect: function (selectedDate) {
            //$("#TEND_AGREEMENT_START_DATE").datepicker("option", "minDate", selectedDate);
            //$("#TEND_DATE_OF_WORK_ORDER").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });



    $('#TEND_ISSUE_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        minDate: new Date(),
        buttonText: 'Tender Form Issue Start Date',
        onSelect: function (selectedDate) {
            $("#TEND_ISSUE_END_DATE").datepicker("option", "minDate", selectedDate);
            //$("#TEND_DATE_OF_AGREEMENT").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_COMMENCEMENT").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_ISSUE_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Tender Form Issue End Date',
        onSelect: function (selectedDate) {
            $("#TEND_ISSUE_START_DATE").datepicker("option", "maxDate", selectedDate);
            $("#TEND_INSP_START_DATE").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_INSP_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Inspection Start Date',
        onSelect: function (selectedDate) {
            $("#TEND_INSP_END_DATE").datepicker("option", "minDate", selectedDate);
            $("#TEND_ISSUE_END_DATE").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_AGREEMENT").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_COMMENCEMENT").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_INSP_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Inspection End Date',
        onSelect: function (selectedDate) {
            $("#TEND_INSP_START_DATE").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_ISSUE_START_TIME').timepicker({
        //showPeriod: true,
        showLeadingZero: false,
        onHourShow: tpStartOnHourShowCallback,
        onMinuteShow: tpStartOnMinuteShowCallback,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmTenderFormIssueStart",
       // showNowButton: true, 
       // nowButtonText: 'Now',
        showCloseButton: true,
        minutes: {
            starts: 0,              
            ends: 59,                
            interval: 1              
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });
    $('#TEND_ISSUE_END_TIME').timepicker({
       // showPeriod: true,
        showLeadingZero: false,
        onHourShow: tpEndOnHourShowCallback,
        onMinuteShow: tpEndOnMinuteShowCallback,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmTenderFormIssueEnd",
        //showNowButton: true, 
      //  nowButtonText: 'Now',
        showCloseButton: true,
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#CONT_REGN_VALIDITY_TIME').timepicker({
        //showPeriod: true,
        showLeadingZero: true,
        showOn: 'button',
        button: "#tmContractorRegistration",
        showDeselectButton: true,
       // showNowButton: true,        
      //  nowButtonText: 'Now',
        showCloseButton: true,
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

    
        $('#tbNITList').jqGrid("setGridState", "visible");

        $("#dvNIT").animate({
            scrollTop: 0
        });

    });

  

    $('#btnSaveNITDetails').click(function (e) {


        if ($('#frmAddNITDetails').valid()) {

       
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/NIT/AddNITDetails",
                type: "POST",            
                data: $("#frmAddNITDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                      
                        alert(data.message);
                    
                        $('#btnResetNITDetails').trigger('click');

                   
                        $('#tbNITList').trigger('reloadGrid');
                        $('#tbNITList').jqGrid("setGridState", "visible");

                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $('#dvAddNIT').html(data);
                        $('#dvAddNIT').show('slow');
                        $('#dvNITDetails').show('slow');
                       
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


    $('#btnResetNITDetails').click(function (e) {

        if ($("#dvErrorMessage").is(":visible")) {
            
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }
      
    });

    $("#btnUpdateNITDetails").click(function (e) {


        if ($("#frmAddNITDetails").valid()) {

            $('#ddlFundingAgencies').attr('disabled', false);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/NIT/EditNITDetails',
                async: false,
                data: $("#frmAddNITDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#btnCancelNITDetails').trigger('click');
                        $('#tbNITList').trigger('reloadGrid');
                        $('#tbNITList').jqGrid("setGridState", "visible");
                      
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        $('#ddlFundingAgencies').attr('disabled', true);
                    }
                    else {
                        $('#dvAddNIT').html(data);
                        $('#dvAddNIT').show('slow');
                        $('#dvNITDetails').show('slow'); 
                        $("#TEND_NIT_NUMBER").focus();
                        $('#ddlFundingAgencies').attr('disabled', true);
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


    $('#btnCancelNITDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }


        $('#tbNITList').jqGrid("setGridState", "visible");

        $("#dvNIT").animate({
            scrollTop: 0
        });

    });


});

function tpStartOnHourShowCallback(hour) {
    var tpEndHour = $('#TEND_ISSUE_END_TIME').timepicker('getHour');
    // all valid if no end time selected
    if ($('#TEND_ISSUE_END_TIME').val() == '') { return true; }
    // Check if proposed hour is prior or equal to selected end time hour
    if (hour <= tpEndHour) { return true; }
    // if hour did not match, it can not be selected
    return false;
}
function tpStartOnMinuteShowCallback(hour, minute) {
    var tpEndHour = $('#TEND_ISSUE_END_TIME').timepicker('getHour');
    var tpEndMinute = $('#TEND_ISSUE_END_TIME').timepicker('getMinute');
    // all valid if no end time selected
    if ($('#TEND_ISSUE_END_TIME').val() == '') { return true; }
    // Check if proposed hour is prior to selected end time hour
    if (hour < tpEndHour) { return true; }
    // Check if proposed hour is equal to selected end time hour and minutes is prior
    if ((hour == tpEndHour) && (minute < tpEndMinute)) { return true; }
    // if minute did not match, it can not be selected
    return false;
}

function tpEndOnHourShowCallback(hour) {
    var tpStartHour = $('#TEND_ISSUE_START_TIME').timepicker('getHour');
    // all valid if no start time selected
    if ($('#TEND_ISSUE_START_TIME').val() == '') { return true; }
    // Check if proposed hour is after or equal to selected start time hour
    var compareDate = $('#TEND_ISSUE_START_DATE').val();
    var value = $('#TEND_ISSUE_END_DATE').val();

    if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) >= new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {

        if (hour >= tpStartHour) { return true; }
    }
    else {
        return true;
    }
    // if hour did not match, it can not be selected
    return false;
}
function tpEndOnMinuteShowCallback(hour, minute) {
    var tpStartHour = $('#TEND_ISSUE_START_TIME').timepicker('getHour');
    var tpStartMinute = $('#TEND_ISSUE_START_TIME').timepicker('getMinute');
    // all valid if no start time selected
    if ($('#TEND_ISSUE_START_TIME').val() == '') { return true; }
    // Check if proposed hour is after selected start time hour
    var compareDate = $('#TEND_ISSUE_START_DATE').val();
    var value = $('#TEND_ISSUE_END_DATE').val();

    if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) >= new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {
        if (hour > tpStartHour) { return true; }
        // Check if proposed hour is equal to selected start time hour and minutes is after
        if ((hour == tpStartHour) && (minute > tpStartMinute)) { return true; }
    }
    else {
        return true;
    }
    // if minute did not match, it can not be selected
    return false;
}
