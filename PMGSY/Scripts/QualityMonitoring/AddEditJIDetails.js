$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmQMJointInspections");

    $("#spCollapseIconQMJointInspections").click(function () {
        $("#spCollapseIconQMJointInspections").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMJointInspections").toggle("slow");
    });

    //alert($('#dbOperation').val());
    if ($('#dbOperation').val() != 'E') {
        NotPostBack();
    }
    else {
        PostBackForEdit();
    }

    ///Check Box click code
    $('#chkMP').click(function () {
        if ($('#chkMP').is(':checked')) {
            $('#mpName').attr('disabled', false);
        }
        else {
            $('#mpName').val('');
            $('#mpName').attr('disabled', true);
        }
        $('#mpName').trigger('blur');
    });
    $('#mpName').blur(function () {
        if (!($('#chkMP').is(':checked'))) {
            $('#mpName').val('');
            $('#mpName').attr('disabled', true);
        }
    });

    $('#chkMLA').click(function () {
        if ($('#chkMLA').is(':checked')) {
            $('#mlaName').attr('disabled', false);
        }
        else {
            $('#mlaName').val('');
            $('#mlaName').attr('disabled', true);
        }
        $('#mlaName').trigger('blur');
    });
    $('#mlaName').blur(function () {
        if (!($('#chkMLA').is(':checked'))) {
            $('#mlaName').val('');
            $('#mlaName').attr('disabled', true);
        }
    });


    $('#chkGP').click(function () {
        if ($('#chkGP').is(':checked')) {
            $('#gpName').attr('disabled', false);
        }
        else {
            $('#gpName').val('');
            $('#gpName').attr('disabled', true);
        }
        $('#gpName').trigger('blur');
    });
    $('#gpName').blur(function () {
        if (!($('#chkGP').is(':checked'))) {
            $('#gpName').val('');
            $('#gpName').attr('disabled', true);
        }
    });

    $('#chkOther').click(function () {
        if ($('#chkOther').is(':checked')) {
            $('#otherRepresentativeName').attr('disabled', false);
        }
        else {
            $('#otherRepresentativeName').val('');
            $('#otherRepresentativeName').attr('disabled', true);
        }
        $('#otherRepresentativeName').trigger('blur');
    });
    $('#otherRepresentativeName').blur(function () {
        if (!($('#chkOther').is(':checked'))) {
            $('#otherRepresentativeName').val('');
            $('#otherRepresentativeName').attr('disabled', true);
        }
    });

    $('#chkSE').click(function () {
        if ($('#chkSE').is(':checked')) {
            $('#seName').attr('disabled', false);
        }
        else {
            $('#seName').val('');
            $('#seName').attr('disabled', true);
        }
        $('#seName').trigger('blur');
    });
    $('#seName').blur(function () {
        if (!($('#chkSE').is(':checked'))) {
            $('#seName').val('');
            $('#seName').attr('disabled', true);
        }
    });

    $('#chkPIU').click(function () {
        if ($('#chkPIU').is(':checked')) {
            $('#piuName').attr('disabled', false);
        }
        else {
            $('#piuName').val('');
            $('#piuName').attr('disabled', true);
        }
        $('#piuName').trigger('blur');
    });
    $('#piuName').blur(function () {
        if (!($('#chkPIU').is(':checked'))) {
            $('#piuName').val('');
            $('#piuName').attr('disabled', true);
        }
    });

    $('#chkAE').click(function () {
        if ($('#chkAE').is(':checked')) {
            $('#aeName').attr('disabled', false);
        }
        else {
            $('#aeName').val('');
            $('#aeName').attr('disabled', true);
        }
        $('#aeName').trigger('blur');
    });
    $('#aeName').blur(function () {
        if (!($('#chkAE').is(':checked'))) {
            $('#aeName').val('');
            $('#aeName').attr('disabled', true);
        }
    });

    $('#chkDO').click(function () {
        if ($('#chkDO').is(':checked')) {
            $('#districtOfficerName').attr('disabled', false);
        }
        else {
            $('#districtOfficerName').val('');
            $('#districtOfficerName').attr('disabled', true);
        }
        $('#districtOfficerName').trigger('blur');
    });
    $('#districtOfficerName').blur(function () {
        if (!($('#chkDO').is(':checked'))) {
            $('#districtOfficerName').val('');
            $('#districtOfficerName').attr('disabled', true);
        }
    });
    ///Check Box click code ENDS...

    ///Contractor represntative available code
    $('#rdbContrReprY').click(function () {
        $('#rdbContrReprY').attr('checked', true);
        $('#txtContractorName').attr('disabled', false);
        $('#contractorRepresentative').val('Y');
    });
    $('#rdbContrReprN').click(function () {
        $('#rdbContrReprN').attr('checked', true);
        $('#txtContractorName').attr('disabled', true);
        $('#contractorRepresentative').val('N');
    });
    $('#contractorRepresentative').blur(function () {
        if (($('#rdbContrReprN').is(':checked'))) {
            $('#contractorRepresentative').val('');
            $('#contractorRepresentative').attr('disabled', true);
        }
    });
    ///Contractor represntative available code ENDS...

    ///Contractor represntative available code
    $('#rdbserveConnectivityY').click(function () {
        $('#rdbserveConnectivityY').attr('checked', true);
        $('#serveConnectivity').val('Y');
    });
    $('#rdbserveConnectivityN').click(function () {
        $('#rdbserveConnectivityN').attr('checked', true);
        $('#serveConnectivity').val('N');
    });
    ///Contractor represntative available code ENDS...

    ///Work Progress Satisfactory  code
    $('#rdbworkProgressSatisfactoryY').click(function () {
        $('#rdbworkProgressSatisfactoryY').attr('checked', true);
        $('#workProgressSatisfactory').val('Y');
    });
    $('#rdbworkProgressSatisfactoryN').click(function () {
        $('#rdbworkProgressSatisfactoryN').attr('checked', true);
        $('#workProgressSatisfactory').val('N');
    });
    ///Work Progress Satisfactory code ENDS...

    ///CD Work Sufficient code
    $('#rdbcdWorkSufficientY').click(function () {
        $('#rdbcdWorkSufficientY').prop('checked', true);
        $('#cdWorkSufficient').val('Y');
    });
    $('#rdbcdWorkSufficientN').click(function () {
        $('#rdbcdWorkSufficientN').attr('checked', true);
        $('#cdWorkSufficient').val('N');
    });
    ///CD Work Sufficient code ENDS...

    ///Quality Grading available code
    $('#rdbqualityGradingG').click(function () {
        $('#qualityGrading').val('G');
    });
    $('#rdbqualityGradingI').click(function () {
        $('#qualityGrading').val('I');
    });
    $('#rdbqualityGradingU').click(function () {
        $('#qualityGrading').val('U');
    });
    ///Quality Grading available code ENDS

    ///Inspection Date
    $('#txtInspectionDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: 0,
        maxDate: 0,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            //$("#txtInspectionDate").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#txtInspectionDate').focus();
                //$('#txtNewsPublishEnd').focus();
            })
        }
    });
    ///Inspection Date ENDS...

    $('#btnResetJI').click(function () {
        NotPostBack();
        //$('#districtOfficerName').val('');
    });

    $('#btnUpdateJI').click(function (event) {
        //checkFormValid();
        var form_data = new FormData();

        var objQMJIFile = $("input#QMJIFileUpload").prop("files");
        console.log(objQMJIFile[0]);

        form_data.append("QMJIFileUploadfile", objQMJIFile[0]);

        var data = $("#frmQMJointInspections").serializeArray();

        for (var i = 0; i < data.length; i++) {
            form_data.append(data[i].name, data[i].value);
        }
        //alert($("#frmQMJointInspections").valid());
        //alert(checkFormValid());
        if (checkFormValid() && $("#frmQMJointInspections").valid()) {
            $.ajax({
                url: "/QualityMonitoring/UpdateQMJointInspectionDetails",
                type: "POST",
                //async: false,
                //cache: false,
                data: form_data,//$("#frmQMJointInspections").serialize(),
                contentType: false,
                processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        // $("#spCollapseIconQMJointInspections").trigger('click');
                        $('#btnResetJI').trigger('click');
                        //CloseJIDetails();
                        CloseJIDetailsLayout();
                    }
                }
            });
        }

    });
    ///Code to Save JI Details
    $('#btnSubmitJI').click(function (event) {

        //checkFormValid();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        var form_data = new FormData();

        var objQMJIFile = $("input#QMJIFileUpload").prop("files");
        console.log(objQMJIFile[0]);

        form_data.append("QMJIFileUploadfile", objQMJIFile[0]);

        var data = $("#frmQMJointInspections").serializeArray();
        // alert(JSON.stringify(data));
        for (var i = 0; i < data.length; i++) {
            form_data.append(data[i].name, data[i].value);
        }
        //alert($("#frmQMJointInspections").valid());
        //alert(checkFormValid());
        if ($("#frmQMJointInspections").valid() && checkFormValid()) {
            $.ajax({
                url: "/QualityMonitoring/AddEditJIDetails",
                type: "POST",
                data: form_data,//$("#frmQMJointInspections").serialize(),
                contentType: false,
                processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $('#btnResetJI').trigger('click');
                        //CloseJIDetails();
                        CloseJIDetailsLayout();
                    }
                }
            });
        }
        $.unblockUI();
    });


});
///Code to Save JI Details ENDS...

///Code for first time load
function NotPostBack() {
    $('#lbConnectivityType').text('-');

    $('#rdbContrReprN').trigger('click');

    //alert($('#imsUpgradeConnect').val());
    ///Serve Connectivity to target habitation is applicable only for New Connectivity Road
    if ($('#workType').val() == 'P' && $('#imsUpgradeConnect').val() == 'N') {
        $('#rdbserveConnectivityY').attr('disabled', false);
        $('#rdbserveConnectivityN').attr('disabled', false);
        $("#rdbserveConnectivityY").prop("checked", true)
        $('#serveConnectivity').val('Y');
        $('#rdbserveConnectivityY').trigger('click');
    }
    else {
        $('#rdbserveConnectivityY').attr('disabled', true);
        $('#rdbserveConnectivityN').attr('disabled', true);
        $("#rdbserveConnectivityN").prop("checked", true)
        $('#serveConnectivity').val('N');
        $('#rdbserveConnectivityN').trigger('click');
    }

    //alert($('#workType').val());
    ///CD Works Sufficient is applicable for Road Works only
    if ($('#workType').val() == 'P') {
        $('#rdbcdWorkSufficientY').attr('disabled', false);
        $('#rdbcdWorkSufficientN').attr('disabled', false);
        $("#rdbcdWorskSufficientY").prop("checked", true)
        $('#cdWorkSufficient').val('Y');
        $('#rdbcdWorkSufficientY').trigger('click');
    }
    else {
        $('#rdbcdWorkSufficientY').attr('disabled', 'disabled');
        $('#rdbcdWorkSufficientN').attr('disabled', 'disabled');
        $("#rdbcdWorkSufficientN").prop("checked", true)
        $('#cdWorkSufficient').val('N');
        $('#rdbcdWorkSufficientN').trigger('click');
    }

    //alert($('#imsProgress').val());
    ///Work Progress satisfactory is applicable for on-going work only
    if ($('#imsProgress').val() != 'C' || $('#imsProgress').val() != 'X') {
        $('#rdbworkProgressSatisfactoryY').trigger('click');
        $('#rdbworkProgressSatisfactoryY').attr('disabled', true);
        $('#rdbworkProgressSatisfactoryN').attr('disabled', true);
        $("#rdbworkProgressSatisfactoryY").prop("checked", true)
        $('#workProgressSatisfactory').val('Y');
    }
    else {
        $('#rdbworkProgressSatisfactoryN').trigger('click');
        $('#rdbworkProgressSatisfactoryY').attr('disabled', true);
        $('#rdbworkProgressSatisfactoryN').attr('disabled', true);
        $("#rdbworkProgressSatisfactoryN").prop("checked", true)
        $('#workProgressSatisfactory').val('N');
    }

    $('#txtContractorName').attr('disabled', true);

    $('#mpName').attr('disabled', 'disabled');
    $('#mlaName').attr('disabled', 'disabled');
    $('#gpName').attr('disabled', 'disabled');
    $('#otherRepresentativeName').attr('disabled', 'disabled');

    $('#seName').attr('disabled', 'disabled');
    $('#piuName').attr('disabled', 'disabled');
    $('#aeName').attr('disabled', 'disabled');
    $('#districtOfficerName').attr('disabled', 'disabled');
}
///Code for first time load ENDS...

function PostBackForEdit() {

    //if ($('#chkMP').val()) {
    if ($('#mpName').val() != '') {
        $('#mpName').attr('disabled', false);
    }
    else {
        $('#mpName').attr('disabled', true);
    }

    //if ($('#chkMLA').val()) {
    if ($('#mlaName').val() != '') {
        $('#mlaName').attr('disabled', false);
    }
    else {
        $('#mlaName').attr('disabled', true);
    }

    //if ($('#chkGP').val()) {
    if ($('#gpName').val() != '') {
        $('#gpName').attr('disabled', false);
    }
    else {
        $('#gpName').attr('disabled', true);
    }

    //if ($('#chkOther').val()) {
    if ($('#otherRepresentativeName').val() != '') {
        $('#otherRepresentativeName').attr('disabled', false);
    }
    else {
        $('#otherRepresentativeName').attr('disabled', true);
    }

    //if ($('#chkSE').val()) {
    if ($('#seName').val() != '') {
        $('#seName').attr('disabled', false);
    }
    else {
        $('#seName').attr('disabled', true);
    }

    //if ($('#chkPIU').val()) {
    if ($('#piuName').val() != '') {
        $('#piuName').attr('disabled', false);
    }
    else {
        $('#piuName').attr('disabled', true);
    }

    //if ($('#chkAE').val()) {
    if ($('#aeName').val() != '') {
        $('#aeName').attr('disabled', false);
    }
    else {
        $('#aeName').attr('disabled', true);
    }

    //if ($('#chkDO').val()){
    if ($('#districtOfficerName').val() != '') {
        $('#districtOfficerName').attr('disabled', false);
    }
    else {
        $('#districtOfficerName').attr('disabled', true);
    }



    if ($('#contractorRepresentative').val() == "Y") {
        $('#rdbContrReprY').trigger('click');
    }
    else {
        $('#rdbContrReprN').trigger('click');
    }

    if ($('#serveConnectivity').val() == "Y") {
        $('#rdbserveConnectivityY').trigger('click');
    }
    else {
        $('#rdbserveConnectivityN').trigger('click');
    }

    if ($('#workProgressSatisfactory').val() == "Y") {
        $('#rdbworkProgressSatisfactoryY').trigger('click');
    }
    else {
        $('#rdbworkProgressSatisfactoryN').trigger('click');
    }

    if ($('#cdWorkSufficient').val() == "Y") {
        $('#rdbcdWorkSufficientY').trigger('click');
    }
    else {
        $('#rdbcdWorkSufficientN').trigger('click');
    }
}

function checkFormValid() {
    if (!($('#rdbqualityGradingG').is(':checked') || $('#rdbqualityGradingI').is(':checked') || $('#rdbqualityGradingU').is(':checked'))) {
        alert('Please select Quality Grading');

        $('#rdbqualityGradingG').focus();
        return false;
    }

    if (!($('#chkMP').is(':checked') || $('#chkMLA').is(':checked') || $('#chkGP').is(':checked') || $('#chkOther').is(':checked')
          || $('#chkSE').is(':checked') || $('#chkPIU').is(':checked') || $('#chkAE').is(':checked') || $('#chkDO').is(':checked')
          )
       ) {
        alert('Please select atleast one of Public Representative and Accompanying Officer details');
        ($('#chkMP').focus());
        return false;
    }

    if ($('#chkMP').is(':checked') && $('#mpName').val() == '') {
        alert('Please enter MP Name');
        ($('#chkMP').focus());
        return false;
    }
    if ($('#chkMLA').is(':checked') && $('#mlaName').val() == '') {
        alert('Please enter MLA Name');
        ($('#chkMLA').focus());
        return false;
    }
    if ($('#chkGP').is(':checked') && $('#gpName').val() == '') {
        alert('Please enter GP Name');
        ($('#chkGP').focus());
        return false;
    }
    if ($('#chkOther').is(':checked') && $('#otherRepresentativeName').val() == '') {
        alert('Please enter Other representative Name');
        ($('#chkOther').focus());
        return false;
    }
    if ($('#chkSE').is(':checked') && $('#seName').val() == '') {
        alert('Please enter SE Name');
        ($('#chkSE').focus());
        return false;
    }
    if ($('#chkPIU').is(':checked') && $('#piuName').val() == '') {
        alert('Please enter PIU Name');
        ($('#chkPIU').focus());
        return false;
    }
    if ($('#chkAE').is(':checked') && $('#aeName').val() == '') {
        alert('Please enter AE Name');
        ($('#chkAE').focus());
        return false;
    }
    if ($('#chkDO').is(':checked') && $('#districtOfficerName').val() == '') {
        alert('Please enter DO Name');
        ($('#chkDO').focus());
        return false;
    }
    if ($('#rdbContrReprY').is(':checked') && $('#txtContractorName').val() == '') {
        alert('Please enter Contractor/contractor representative Name');
        ($('#txtContractorName').focus());
        return false;
    }

    if ($('#rdbworkProgressSatisfactoryN').is(':checked')) {
        $('#workProgressSatisfactory').val('N');
    }
    if ($('#rdbserveConnectivityN').is(':checked')) {
        $('#serveConnectivity').val('N');
    }
    if ($('#rdbcdWorkSufficientN').is(':checked')) {
        $('#cdWorkSufficient').val('N');
    }
    return true;
}

function CloseJIDetailsLayout() {
    $('#dvhdQMJILayout').hide('slow');
    $("#dvJointInspectionDetails").hide('slow');
    $("#dvJointInspectionDetails").html('');
    ListQMJIDetails();
}