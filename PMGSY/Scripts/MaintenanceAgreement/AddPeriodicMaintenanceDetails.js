/// <reference path="../jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {
    var selectedYear = $('#ddlMaintanenaceYeaelst option:selected').val();
    var selectedmonth = $('#ddlMaintanenaceMonthlst option:selected').val();

    $.validator.unobtrusive.parse($('#frmCreatePeriodicMaintenance'));

    $('#imgCloseAgreementDetails').click(function () {
        $('#spCollapseIconS').trigger('click');
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");
        $('#accordion').hide('slow');
    })

    /////Changes by SAMMED A. PATIL on 18JULY2017 
    //$('#ddlTechnology').blur(function () {
    //    if ($('#rdoIsPerIncentiveNo').prop('checked')) {
    //        $('#ddlTechnology').rules('remove');
    //    }
    //});

    $('#btnCancel').click(function () {
        $('#accordion').hide('slow');
        $('#tbProposedRoadList').jqGrid('setGridState', 'visible');
    });

    $('#ddlMaintanenaceYeaelst').change(function () {
        selectedYear = $('#ddlMaintanenaceYeaelst option:selected').val();

        if (parseInt(selectedYear) == new Date().getFullYear()) {
            disableMonths();
        }
        else {
            enableMonths();
        }
        var selectedDate = new Date(selectedYear, 0, 1);
        var Cur = new Date();
        var diff = new Date(Cur - selectedDate);
        var days = diff / 1000 / 60 / 60 / 24;
        if ($('#Operation').val() == "A") {
            $('#maintananceDate').val('');
        }
        $("#maintananceDate").datepicker('option', { minDate: "-" + Math.floor(days) + "D" });
        if (!($('#rdoIsCompletedYes').prop('checked'))) {
            $('.ui-datepicker-trigger').hide();
        }
    });

    $('#ddlMaintanenaceMonthlst').change(function () {
        selectedmonth = $('#ddlMaintanenaceMonthlst option:selected').val();
        var selectedDate = new Date(selectedYear, parseInt(selectedmonth) - 1, 1);
        var Cur = new Date();
        var diff = new Date(Cur - selectedDate);
        var days = diff / 1000 / 60 / 60 / 24;
        if ($('#Operation').val() == "A") {
            $('#maintananceDate').val('');
        }
        $("#maintananceDate").datepicker('option', { minDate: "-" + Math.floor(days) + "D" });
        if (!($('#rdoIsCompletedYes').prop('checked'))) {
            $('.ui-datepicker-trigger').hide();
        }
    });


    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    var cur = date.getDate();
    var difference = (lastDay.getDate() - cur);

    $("#maintananceDate").datepicker({
        changeMonth: true,
        changeYear: true,
        maxDate: "+" + 0 + "D",
        dateFormat: "dd/mm/yy",
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        buttonImageOnly: true,
        buttonText: "Select date",
        onSelect: function (dateText, inst) {
            //  $("span").find("[for=txtIssueDate]").text(' ');
        }
    });

    $('#profileCorrCost').focusout(function (event) {
        var profileCorrCost = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var maintenCost = $('#ddlMaintanenaceCost').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCost').val()).toFixed(2);
        var otherCost = $('#ddlOtherCost').val() == "" ? 0 : parseFloat($('#ddlOtherCost').val()).toFixed(2);

        var Sum = (parseFloat(otherCost) + parseFloat(maintenCost) + parseFloat(profileCorrCost));
        $('#TotalMaintenanceCost').val(parseFloat(Sum).toFixed(2));
    });

    $('#ddlOtherCost').focusout(function (event) {
        var otherCost = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var maintenCost = $('#ddlMaintanenaceCost').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCost').val()).toFixed(2);
        var profileCorrCost = $('#profileCorrCost').val() == "" ? 0 : parseFloat($('#profileCorrCost').val()).toFixed(2);

        var Sum = (parseFloat(otherCost) + parseFloat(maintenCost) + parseFloat(profileCorrCost));
        $('#TotalMaintenanceCost').val(parseFloat(Sum).toFixed(2));
    });

    $('#ddlMaintanenaceCost').focusout(function (event) {
        var maintenCost = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var otherCost = $('#ddlOtherCost').val() == "" ? 0 : parseFloat($('#ddlOtherCost').val()).toFixed(2);
        var profileCorrCost = $('#profileCorrCost').val() == "" ? 0 : parseFloat($('#profileCorrCost').val()).toFixed(2);

        var Sum = (parseFloat(otherCost) + parseFloat(maintenCost) + parseFloat(profileCorrCost));

        $('#TotalMaintenanceCost').val(parseFloat(Sum).toFixed(2));
    });

    /*
    $('#ddlMaintanenaceCostYr1').focusout(function (event) {
        var mt1 = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var mt2 = $('#ddlMaintanenaceCostYr2').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr2').val()).toFixed(2);
        var mt3 = $('#ddlMaintanenaceCostYr3').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr3').val()).toFixed(2);
        var mt4 = $('#ddlMaintanenaceCostYr4').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr4').val()).toFixed(2);
        var mt5 = $('#ddlMaintanenaceCostYr5').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr5').val()).toFixed(2);
        var Sum = (parseFloat(mt1) + parseFloat(mt2) + parseFloat(mt3) + parseFloat(mt4) + parseFloat(mt5));
        $('#ddlMaintanenaceTotalYr6').val(parseFloat(Sum).toFixed(2));
    });

    $('#ddlMaintanenaceCostYr2').focusout(function (event) {
        var mt1 = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var mt2 = $('#ddlMaintanenaceCostYr1').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr1').val()).toFixed(2);
        var mt3 = $('#ddlMaintanenaceCostYr3').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr3').val()).toFixed(2);
        var mt4 = $('#ddlMaintanenaceCostYr4').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr4').val()).toFixed(2);
        var mt5 = $('#ddlMaintanenaceCostYr5').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr5').val()).toFixed(2);
        var Sum = (parseFloat(mt1) + parseFloat(mt2) + parseFloat(mt3) + parseFloat(mt4) + parseFloat(mt5));

        $('#ddlMaintanenaceTotalYr6').val('');
        $('#ddlMaintanenaceTotalYr6').val(parseFloat(Sum).toFixed(2));
    });

    $('#ddlMaintanenaceCostYr3').focusout(function (event) {
        var mt1 = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var mt2 = $('#ddlMaintanenaceCostYr2').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr2').val()).toFixed(2);
        var mt3 = $('#ddlMaintanenaceCostYr1').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr1').val()).toFixed(2);
        var mt4 = $('#ddlMaintanenaceCostYr4').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr4').val()).toFixed(2);
        var mt5 = $('#ddlMaintanenaceCostYr5').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr5').val()).toFixed(2);
        var Sum = (parseFloat(mt1) + parseFloat(mt2) + parseFloat(mt3) + parseFloat(mt4) + parseFloat(mt5));
        $('#ddlMaintanenaceTotalYr6').val('');
        $('#ddlMaintanenaceTotalYr6').val(parseFloat(Sum).toFixed(2));
    });
    $('#ddlMaintanenaceCostYr4').focusout(function (event) {
        var mt1 = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var mt2 = $('#ddlMaintanenaceCostYr2').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr2').val()).toFixed(2);
        var mt3 = $('#ddlMaintanenaceCostYr3').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr3').val()).toFixed(2);
        var mt4 = $('#ddlMaintanenaceCostYr1').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr1').val()).toFixed(2);
        var mt5 = $('#ddlMaintanenaceCostYr5').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr5').val()).toFixed(2);
        var Sum = (parseFloat(mt1) + parseFloat(mt2) + parseFloat(mt3) + parseFloat(mt4) + parseFloat(mt5));

        $('#ddlMaintanenaceTotalYr6').val('');
        $('#ddlMaintanenaceTotalYr6').val(parseFloat(Sum).toFixed(2));
    });
    $('#ddlMaintanenaceCostYr5').focusout(function (event) {
        var mt1 = $(this).val() == "" ? 0 : parseFloat($(this).val()).toFixed(2);
        var mt2 = $('#ddlMaintanenaceCostYr2').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr2').val()).toFixed(2);
        var mt3 = $('#ddlMaintanenaceCostYr3').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr3').val()).toFixed(2);
        var mt4 = $('#ddlMaintanenaceCostYr4').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr4').val()).toFixed(2);
        var mt5 = $('#ddlMaintanenaceCostYr1').val() == "" ? 0 : parseFloat($('#ddlMaintanenaceCostYr1').val()).toFixed(2);
        var Sum = (parseFloat(mt1) + parseFloat(mt2) + parseFloat(mt3) + parseFloat(mt4) + parseFloat(mt5));

        $('#ddlMaintanenaceTotalYr6').val('');
        $('#ddlMaintanenaceTotalYr6').val(parseFloat(Sum).toFixed(2));
    });
    
    */
    $('#StartChainage').focusout(function () {
        var StartChainge = parseFloat(jQuery('#StartChainage').val()).toFixed(3);
        var EndChainge = parseFloat(jQuery('#EndChainage').val()).toFixed(3);

        if (EndChainge > 0) {
            if (parseFloat(StartChainge) > parseFloat(EndChainge)) {
                alert('End chainage must be greater than Start chainage.');
                return;
            }
        }

    });


    $('#EndChainage').focusout(function () {
        debugger;
        //check length diff(End chainage -Start chinage)
        var StartChainge = parseFloat(jQuery('#StartChainage').val()).toFixed(3);
        var EndChainge = parseFloat(jQuery('#EndChainage').val()).toFixed(3);

        var Length = parseFloat($('#ddlLength').text().trim()).toFixed(3);
        var diff = (EndChainge - StartChainge).toFixed(3);
        //  alert("Length" + Length + " Start " + StartChainge + " End " + EndChainge);
        if (parseFloat(StartChainge) > parseFloat(EndChainge)) {
            alert('End chainage must be greater than Start chainage.');
            return;
        }
        else if (parseFloat(diff) > parseFloat(Length)) {
            alert('Difference between end chainage and start chainage should be less than or equal to Length');
            return;
        }

    })

    $('#btnReset').click(function () {
        $('#performanceIntYear').hide();
        $('#performanceIntYearList').hide();
        $('#errspnPerformanceyear').text('');  //error msg
        $('#lblmaintenanceDate').hide('slow');
        $('#maintananceDate').hide('slow');
        $('.ui-datepicker-trigger').hide();
        $('#lblrequired').hide('slow');
    })

    ///Changes by SAMMED A. PATIL on 18JULY2017 
    if ($('#rdoIsPerIncentiveNo').prop('checked')) {
        $('#ddlTechnology').rules('remove', "required");
        $('#ddlTechnology').rules('remove', "range");
    }
    $('#btnSave').click(function () {
        ///Changes by SAMMED A. PATIL on 18JULY2017 
        if ($('#rdoIsPerIncentiveNo').prop('checked')) {
            $('#ddlTechnology').rules('remove');
        }

        //check length diff(End chainage -Start chinage)
        var StartChainge = parseFloat(jQuery('#StartChainage').val()).toFixed(3);
        var EndChainge = parseFloat(jQuery('#EndChainage').val()).toFixed(3);

        var Length = parseFloat($('#ddlLength').text().trim()).toFixed(3);
        var diff = (EndChainge - StartChainge).toFixed(3);
        //  alert("Length" + Length + " Start " + StartChainge + " End " + EndChainge);
        if (parseFloat(StartChainge) > parseFloat(EndChainge)) {
            alert('End Chainage must be greater than Start chainage.');
            return;
        }
        else if (parseFloat(diff) > parseFloat(Length)) {
            alert('Difference between end chainage and start chainage should be less than or equal to Length');
            return;
        }
        else {
            //$('#frmCreatePeriodicMaintenance').validate({ // initialize the plugin
            //    rules: {
            //        "screentext_184": {
            //            required: "input[name='screen_184'][value='Yes']:checked"
            //        }
            //    },
            //    submitHandler: function (form) { // for demo
            //        alert('valid form submitted'); // for demo
            //        return false; // for demo
            //    }
            //});

            if ($('#frmCreatePeriodicMaintenance').valid()) {
                savePeriodicMaintenance();
            }
        }
    });

    $('#btnUpdate').click(function () {
        ///Changes by SAMMED A. PATIL on 18JULY2017 
        if ($('#rdoIsPerIncentiveNo').prop('checked')) {
            $('#ddlTechnology').rules('remove');
        }

        //check length diff(End chainage -Start chinage)
        var StartChainge = parseFloat(jQuery('#StartChainage').val()).toFixed(3);
        var EndChainge = parseFloat(jQuery('#EndChainage').val()).toFixed(3);

        var Length = parseFloat($('#ddlLength').text().trim()).toFixed(3);
        var diff = (EndChainge - StartChainge).toFixed(3);
        // alert("Length" + Length + " Start " + StartChainge + " End " + EndChainge);

        if (parseFloat(StartChainge) > parseFloat(EndChainge)) {
            alert('End Chainage must be greater than Start chainage.');
            return;
        }
        else if (parseFloat(diff) > parseFloat(Length)) {
            alert('Difference between end chainage and start chainage should be less than or equal to Length');
            return;
        }
        else {
            if ($('#frmCreatePeriodicMaintenance').valid()) {
                EditPeriodicMaintenance();
            }
        }
    });
    //radio button => Dropdown of performance year toggle
    if ($('#rdoIsPerIncentiveYes').prop('checked')) {
        $('#performanceIntYear').show('slow');
        $('#performanceIntYearList').show('slow');
        $('#lbTechnologyMand').show('slow');
        $('#ddlTechnology').rules('add', "required");
    }
    else {

        $('#performanceIntYear').hide('slow');
        $('#performanceIntYearList').hide('slow');
        $('#performanceIntYearList').val('0');
        $('#errspnPerformanceyear').text('');  //error msg
        $('#lbTechnologyMand').hide('slow');
    }

    $('#rdoIsPerIncentiveYes').click(function () {
        $('#performanceIntYear').show('slow');
        $('#performanceIntYearList').show('slow');
        $('#lbTechnologyMand').show('slow');

        $('#ddlTechnology').rules('add', "required");
        //$('#ddlTechnology').rules('add', "range"); 
        $('#ddlTechnology').rules('add', { range: [1, $('#intMaxValue').val()] });
    });

    $('#rdoIsPerIncentiveNo').click(function () {
        $('#performanceIntYear').hide('slow');
        $('#performanceIntYearList').hide('slow');
        $('#performanceIntYearList').val('0');
        $('#errspnPerformanceyear').text('');  //error msg
        $('#lbTechnologyMand').hide('slow');

        $('#ddlTechnology').rules('remove', "required");
        $('#ddlTechnology').rules('remove', "range");
        $('#ddlTechnology').trigger('blur');
    });

    //radio button => Dropdown of maintenance date
    if ($('#rdoIsCompletedYes').prop('checked')) {
        $('#lblmaintenanceDate').show('slow');
        $('#maintananceDate').show('slow');
        $('.ui-datepicker-trigger').show('slow');
        $('#lblrequired').show('slow');
    }

    $('#rdoIsCompletedYes').click(function () {
        $('#lblmaintenanceDate').show('slow');
        $('#maintananceDate').show('slow');
        $('.ui-datepicker-trigger').show('slow');
        $('#lblrequired').show('slow');

    });
    $('#rdoIsCompletedNo').click(function () {
        $('#lblmaintenanceDate').hide('slow');
        $('#maintananceDate').hide('slow');
        $('.ui-datepicker-trigger').hide('slow');
        $('#lblrequired').hide('slow');
        $('#maintananceDate').val('');
        $('#errspnmaintananceDate').text(''); //error msg
    });

});

function disableMonths() {
    var month = new Date().getMonth();
    var selectedmonth = $('#ddlMaintanenaceMonthlst option:selected').val();

    if (selectedmonth > (month + 1))
        $('#ddlMaintanenaceMonthlst').val('0');
    for (var i = (month + 2) ; i <= 12; i++) {
        $('#ddlMaintanenaceMonthlst option[value=' + i + ']').prop('disabled', true);
    }

}

function enableMonths() {

    var month = new Date().getMonth();

    for (var i = (month + 2) ; i <= 12; i++) {
        // alert($('#ddlMaintanenaceMonthlst option[value=' + i + ']').prop('disabled'));
        if ($('#ddlMaintanenaceMonthlst option[value=' + i + ']').prop('disabled'))
            $('#ddlMaintanenaceMonthlst option[value=' + i + ']').removeAttr('disabled');
    }

}

function savePeriodicMaintenance() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({

        url: '/MaintenanceAgreement/AddPeriodicMaintenance',
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        data: $('#frmCreatePeriodicMaintenance').serialize(),
        dataType: 'json',
        success: function (jsonData, status, xhr) {
            alert(jsonData.message);
            if (jsonData.success) {
                //$('#imgCloseAgreementDetails').trigger('click');
                $('#dvViewMaintenanceAgreementAgainstRoad').hide();
                $('#tbMaintenanceList').trigger('reloadGrid');
                $('#tbMaintenanceList').jqGrid('setGridState', 'visible');
            }
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

}

function EditPeriodicMaintenance() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({

        url: '/MaintenanceAgreement/EditPeriodicMaintenance',
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        data: $('#frmCreatePeriodicMaintenance').serialize(),
        dataType: 'json',
        success: function (jsonData, status, xhr) {
            alert(jsonData.message);
            if (jsonData.success) {
                //$('#imgCloseAgreementDetails').trigger('click');
                $('#dvViewMaintenanceAgreementAgainstRoad').hide();
                $('#tbMaintenanceList').trigger('reloadGrid');
                $('#tbMaintenanceList').jqGrid('setGridState', 'visible');
            }
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

}