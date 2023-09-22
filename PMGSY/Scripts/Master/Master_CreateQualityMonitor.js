

jQuery.validator.addMethod("empanelledyearvalidator", function (value, element, param) {

    var IsEmpanelled = $('#rdoEMPANELLEDYes').val();
    var EmpanelledYear = $('#ADMIN_QM_EMPANELLED_YEAR').val();

    if ($("#rdoEMPANELLEDYes").is(":checked")) {
        if (IsEmpanelled == "Y" && EmpanelledYear == 0) {
            return false;
        }
        else {
            return true;
        }
    } else {
        return true;
    }



});

jQuery.validator.unobtrusive.adapters.addBool("empanelledyearvalidator");


var district;

$(document).ready(function () {
    if ($("#FILE_NAME").val() != null) {
        $("#FileValidation").hide();
    }

    if ($("#rdoEMPANELLEDYes").is(":checked")) {
        $("#listEMPANELLEDNoo").hide();
        $("#lblEmpanelledNoo").hide();
        $("#lblDeEmpanelledDOD").hide();
        $("#txtDeEmpanelledDOD").hide();
        $("#deEmpanelledDateRow").hide();

    }
    //added by abhinav on 18-july-2019.
    if ($("#rdoEMPANELLEDNo").is(":checked")) {
        $("#TxtDeEmpanelledRemark").show();
        $("#LblDeEmpanelledRemark").show();
        $("#listEMPANELLEDNoo").show();
        $("#lblEmpanelledNoo").show();
        $("#lblDeEmpanelledDOD").show();
        $("#txtDeEmpanelledDOD").show();
        $("#deEmpanelledDateRow").show();
    }
    else {
        $("#TxtDeEmpanelledRemark").hide();
        $("#LblDeEmpanelledRemark").hide();
    }

    $("#rdoEMPANELLEDYes").click(function () {
        $("#TxtDeEmpanelledRemark").hide();
        $("#LblDeEmpanelledRemark").hide();

        $("#listEMPANELLEDNoo").hide();
        $("#lblEmpanelledNoo").hide();
        $("#lblDeEmpanelledDOD").hide();
        $("#txtDeEmpanelledDOD").hide();
        $("#deEmpanelledDateRow").hide();
    });



    $("#rdoEMPANELLEDNo").click(function () {
        $("#TxtDeEmpanelledRemark").show();
        $("#LblDeEmpanelledRemark").show();
    });

    if ($("#isOpEdit").val() == "") {
        $("#lblEmpanelled").css('display', 'none');
        $("#tdEMPANELLED").css('display', 'none');
    }
    else {
        $("#lblEmpanelled").css('display', 'normal');
        $("#tdEMPANELLED").css('display', 'normal');
    }




    $.validator.unobtrusive.parse("#frmMasterQualityMonitor");

    $('#ADMIN_QM_DEMPANELLED_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        defaultDate: new Date(),
        //mniDate: new Date("01/01/" + startYear),
        //maxDate : new Date("31/12/" + endYear),
        yearRange: setYear,
        onSelect: function (selectedDate) {
            $(this).focus().blur();
        }
    });

    //Added by DEENDAYAL on 20JUNE2017 Allow SQC to modify SQM details
    //if ($('#EncryptedQmCode').val() != "" && $('#monitor_type').val() == 8) {
    //if ($('#EncryptedQmCode').val() != "" && $('#monitor_type').val() != 9) {  //Edit option for PAN will only be available in CQC login    --Modified on 02-01-2023 by Shreyas
    if ($('#EncryptedQmCode').val() != "" && $('#monitor_type').val() == 8) {
        //added to authorize sqc to modify sqm details
        $('#ADMIN_QM_BIRTH_DATE').datepicker({
            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            buttonImageOnly: true,
            changeMonth: false,
            changeYear: false,
            defaultDate: new Date("01/01/" + startYear),
            //mniDate: new Date("01/01/" + startYear),
            //maxDate : new Date("31/12/" + endYear),
            yearRange: setYear,
            onSelect: function (selectedDate) {
                $(this).focus().blur();
            }
        });
        $("#ADMIN_QM_BIRTH_DATE").siblings('img').hide();

        $('#ADMIN_QM_PAN').prop("readonly", true);
        $('#ADMIN_QM_FNAME').prop("readonly", true);
        $('#ADMIN_QM_MNAME').prop("readonly", true);
        $('#ADMIN_QM_LNAME').prop("readonly", true);

        if ($('#ADMIN_QM_TYPE').val() == "I") {   //Change in Empaneled status for NQM will be available only in CQC login --Changes on 03-01-2023
            $('#rdoEMPANELLEDYes').attr('disabled', true);
            $('#rdoEMPANELLEDNo').attr('disabled', true);
            $('#ADMIN_QM_EMPANELLED_MONTH').attr('disabled', true);
            $('#ADMIN_QM_EMPANELLED_YEAR').attr('disabled', true);

        }
    }



    $("#MAST_CADRE_STATE_CODE").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#MAST_CADRE_STATE_CODE").multiselect("uncheckAll");

    var IsEdit = $("#EncryptedQmCode").val().length == 0 ? false : true;

    if (IsEdit) {
        var serviceType = $("#ADMIN_SERVICE_TYPE").val();
        if (serviceType == "S") {
            $("#divlblCadreState").show();
            $("#divState").show();

        }
        else if (serviceType == "A") {
            $("#divlblCadreState").show();
            $("#divAgency").show();

            var dataarray = $("#cadreStatesMapp").val().split("$");
            $("#MAST_CADRE_STATE_CODE").val(dataarray);
            $("#MAST_CADRE_STATE_CODE").multiselect("refresh");
        }

        district = $("#MAST_DISTRICT_CODE").val();
        //alert(district);
        FillInCascadeDropdown({ userType: $("#stateCode").find(":selected").val() },
            "#MAST_DISTRICT_CODE", "/Master/GetDistrictByStateCode?stateCode=" + $('#stateCode option:selected').val() + "&value=" + Math.random());


    }

    var endYear = $("#currentYear").val();

    var startYear = endYear - 71;
    var setYear = startYear + ':' + (endYear - 30);

    //minDate = new Date(endYear - 71,);

    $('#ADMIN_QM_BIRTH_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        defaultDate: new Date("01/01/" + startYear),
        //mniDate: new Date("01/01/" + startYear),
        //maxDate : new Date("31/12/" + endYear),
        yearRange: setYear,
        onSelect: function (selectedDate) {
            $(this).focus().blur();
        }
    });


    $("#ADMIN_SERVICE_TYPE").change(function () {
        var serviceCode = $('#ADMIN_SERVICE_TYPE option:selected').val()
        // alert("OK: " + serviceCode);
        if (serviceCode == "S") {
            $("#divState").show();
            $("#divAgency").hide();
            $("#divlblCadreState").show();
        } else if (serviceCode == "A") {
            $("#divState").hide();
            $("#divAgency").show();
            $("#divlblCadreState").show();
        }
        else if (serviceCode == "C") {
            $("#divState").hide();
            $("#divAgency").hide();
            $("#divlblCadreState").hide();

        }
    });

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($("#rdoEMPANELLEDYes").is(":checked")) {

        IsEMPANELLEDYes();
    }

    if ($("#rdoEMPANELLEDNo").is(":checked")) {

        IsEMPANELLEDNo();
    }

    $("#rdoEMPANELLEDYes").change(function () {
        IsEMPANELLEDYes();

    });

    $("#rdoEMPANELLEDNo").change(function () {
        IsEMPANELLEDNo();
    });


    if ($('#HiddenstateCode').val() > 0) {
        $("#stateCode").val($('#HiddenstateCode').val());
        $("#stateCode").attr("disabled", true);
        $(function () {
            $("#stateCode").trigger('change');
        });
    }



    $("#rdoSQM").change(function () {
        //   NqmSqm = $("#rdoSQM").val();
        $('#NQMRowID').hide();

        $('#MAST_STATE_CODE').val(0);
        $('#ADMIN_SERVICE_TYPE').val(0);
        $("#divState").hide();
        $("#divAgency").hide();
        $("#divlblCadreState").hide();
    });
    $("#rdoNQM").change(function () {
        //   NqmSqm = $("#rdoSQM").val();
        $('#NQMRowID').show();

    }); //Added by deendayal on 28/7/2017



    $('#btnSave').click(function (e) {
        $("#stateCode").attr("disabled", false);
        $("#stateCode").attr("readonly", true);

        var form = $('#frmMasterQualityMonitor');
        var formadata = new FormData(form.get(0));
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]
        formadata.append("BGFile", FileBG);
        //formadata.append("BirthDate", $("#ADMIN_QM_BIRTH_DATE").val());
        //alert(JSON.stringify(formadata))

        if ($('#frmMasterQualityMonitor').valid()) {

            $("#stateCode").attr("disabled", false);
            var stateCode = $('#stateCode option:selected').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterQualityMonitor",
                type: "POST",

                //data: $("#frmMasterQualityMonitor").serialize(),
                cache: false,
                async: false,
                contentType: false,
                processData: false,
                beforeSend: function () { },
                data: formadata,

                success: function (data) {
                    if (data.success == true) {
                        //alert(data.message);
                        //$('#btnReset').trigger('click');
                        //if ($('#HiddenstateCode').val() > 0) {
                        //    $("#stateCode").attr("disabled", true);
                        //}

                        //ClearDetails();
                        if ($("#dvQualityMonitorDetails").is(":visible")) {
                            $('#dvQualityMonitorDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchQualityMonitor").is(":visible")) {
                            $("#dvSearchQualityMonitor").show('slow');
                        }
                        $("#dvhdFileUpload").hide("slow");
                        searchCreateQMDetails(stateCode);
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                       // alert("in elseif")
                       // alert(data)
                       // alert(JSON.stringify(data))
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if ($('#HiddenstateCode').val() > 0) {
                                $("#stateCode").attr("disabled", true);
                            }
                        }
                    }
                    else {
                        $("#dvQualityMonitorDetails").html(data);

                        if ($('#HiddenstateCode').val() > 0) {
                            $("#stateCode").attr("disabled", true);
                        }
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

    $('#btnUpdate').click(function (e) {
        //alert($('#BGFile').val())
        //alert(JSON.stringify($('#BGFile').val()))
        $('#stateCode').attr('disabled', false);
        $('#stateCode').attr('readonly', true);

        if (!($('#BGFile').val() == "" || $('#BGFile').val() == null)) {
            //alert("in if")
            $('#FILE_NAME').val('')
        }
        else {
            $('#BGFile').val('')
            $("#File").val('')
        }
        var form = $('#frmMasterQualityMonitor');//Shreyas
        var formadata = new FormData(form.get(0));//Shreyas

        if ($('#frmMasterQualityMonitor').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            var stateCode = $('#stateCode option:selected').val();
            $('#stateCode').attr('disabled', false);
            $('#MAST_DISTRICT_CODE').attr('disabled', false);

            $.ajax({
                url: "/Master/EditMasterQualityMonitor/",
                type: "POST",

                //data: $("#frmMasterQualityMonitor").serialize(),
                cache: false,
                async: false,
                contentType: false,
                processData: false,
                beforeSend: function () { },
                data: formadata,
                success: function (data) {


                    if (data.success == true) {
                        //alert(data.message);

                        //$('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { stateCode: $("#MAST_STATE_CODE option:selected").val(), districtCode: "", designationCode: $("#ADMIN_QM_DESG option:selected").val() } });

                        //$('#tblQualityMonitorListDetails').trigger("reloadGrid");                  
                        //$("#dvQualityMonitorDetails").load("/Master/AddEditMasterQualityMonitor");

                        if ($("#dvQualityMonitorDetails").is(":visible")) {
                            $('#dvQualityMonitorDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchQualityMonitor").is(":visible")) {
                            $("#dvSearchQualityMonitor").show('slow');
                        }
                        $("#dvhdFileUpload").hide("slow");

                        searchCreateQMDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            //  $('#stateCode').attr('disabled', true);
                            // $('#MAST_DISTRICT_CODE').attr('disabled', true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvQualityMonitorDetails").html(data);
                        $('#stateCode').attr('disabled', true);
                        $('#MAST_DISTRICT_CODE').attr('disabled', true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $('#stateCode').attr('disabled', true);
                    $('#MAST_DISTRICT_CODE').attr('disabled', false);
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    });

    $('#btnCancel').click(function (e) {



        //$.ajax({
        //    url: "/Master/AddEditMasterQualityMonitor",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvQualityMonitorDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});

        if ($("#dvQualityMonitorDetails").is(":visible")) {
            $('#dvQualityMonitorDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchQualityMonitor").is(":visible")) {
            $("#dvSearchQualityMonitor").show('slow');
        }
        $("#dvhdFileUpload").hide("slow");

    });


    $('#btnReset').click(function (e) {

        //Added By Abhishek kamble 20-Feb-2014 start
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        //if ($("#rdoSQM").is(":checked")) {
        //    $('input[value="I"]').attr('checked', 'checked');
        //}
        if ($("#rdoEMPANELLEDNo").is(":checked")) {
            $('input[value="Y"]').attr('checked', 'checked');
            $("#rdoEMPANELLEDYes").trigger('change');

        }
        //Added By Abhishek kamble 20-Feb-2014 end

        e.preventDefault();

        ClearDetails();



    });


    $("#stateCode").change(function () {



        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }


        FillInCascadeDropdown({ userType: $("#stateCode").find(":selected").val() },
            "#MAST_DISTRICT_CODE", "/Master/GetDistrictByStateCode?stateCode=" + $('#stateCode option:selected').val() + "&value=" + Math.random());

    });
    $("#dvhdCreateNewQualityMonitorDetails").click(function () {

        if ($("#dvCreateNewQualityMonitorDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewQualityMonitorDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewQualityMonitorDetails").slideToggle(300);
        }
    });
    ClearErrorAlertMessage();

    //$("#dvhdFileUpload").click(function () {

    //    if ($("#dvhdFileUpload").is(":visible")) {

    //        $("#spCollapseIconQM").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#divQualityMonitorForm").slideToggle(300);
    //    }
    //    else {
    //        $("#spCollapseIconQM").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#divQualityMonitorForm").slideToggle(300);
    //    }
    //});


});



function FillInCascadeDropdown(map, dropdown, action) {

    var message = "";

    if (dropdown == '#MAST_DISTRICT_CODE') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });

    $('#MAST_DISTRICT_CODE').append("<option value=0>--select--</option>");

    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
        $("#MAST_DISTRICT_CODE").val(district);
    }, "json");
    $.unblockUI();

}

function ClearErrorAlertMessage() {
    $("#stateCode").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_FNAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_MNAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_LNAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_DESG").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_ADDRESS1").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_ADDRESS2").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_DISTRICT_CODE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $("#ADMIN_QM_PIN").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_STD1").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_STD2").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_PHONE1").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_PHONE2").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_STD_FAX").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_FAX").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $("#ADMIN_QM_MOBILE1").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_MOBILE2").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_EMAIL").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_PAN").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_DEG").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_EMPANELLED").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#ADMIN_QM_EMPANELLED_YEAR").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_IMAGE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_DOCPATH").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#ADMIN_QM_REMARKS").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

}

function searchCreateQMDetails(stateCode) {
    // alert("Add s1");
    $('#tblQualityMonitorListDetails').setGridParam({
        url: '/Master/GetMasterQualityMonitorList/'
    });
    //alert($("#rdoNQM").is(":checked"));
    //alert($('#ADMIN_QM_TYPE').val());
    var NqmSqm;
    var Emppaneled;


    if ($("#rdoNQM").is(":checked")) {
        NqmSqm = $("#rdoNQM").val();
    }
    if ($("#rdoSQM").is(":checked")) {
        NqmSqm = $("#rdoSQM").val();
    }
    if (NqmSqm == undefined) {
        NqmSqm = $('#ADMIN_QM_TYPE').val();
    }
    if ($("#rdoEMPANELLEDYes").is(":checked")) {
        Emppaneled = $("#rdoEMPANELLEDYes").val();
    }
    if ($("#rdoEMPANELLEDNo").is(":checked")) {
        Emppaneled = $("#rdoEMPANELLEDNo").val();
    }
    $('#ddlSearchEmpanelled ').val(Emppaneled);
    $('#ddlSearchQmTypes').val(NqmSqm);
    $('#ddlSearchStates').val(stateCode);
    $('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates').val(), QmTypeName: $('#ddlSearchQmTypes').val(), isEmpanelled: $('#ddlSearchEmpanelled option:selected').val(), districtCode: "", designationCode: "", firstName: "" } });

    $('#tblQualityMonitorListDetails').trigger("reloadGrid", [{ page: 1 }]);

}

function IsEMPANELLEDYes() {

    if ($("#rdoEMPANELLEDYes").is(":checked")) {

        $('#empanelledMonthYearRow').show();//Added by deendayal on 06/21/2017 to show Empanelled month as well

        $("#tdEMPANELLED").attr('colspan', '1');
        $("#tdlblEMPANELLEDYear").show('slow');
        $("#tdddlEMPANELLEDYear").show('slow');

        $('#ADMIN_QM_PAN').rules('add', 'required');
        $('#ADMIN_QM_BIRTH_DATE').rules('add', 'required');
        $('#ADMIN_QM_PAN').blur();
        $('#ADMIN_QM_BIRTH_DATE').blur();
        $('#strPAN').show('slow');
        $('#strDOB').show('slow');

        $("#listEMPANELLEDNoo").blur();
        $("#lblEmpanelledNoo").blur();
        $("#lblDeEmpanelledDOD").blur();
        $("#txtDeEmpanelledDOD").blur();
        $("#deEmpanelledDateRow").blur();
    }
}

function IsEMPANELLEDNo() {

    if ($("#rdoEMPANELLEDNo").is(":checked")) {

        $('#empanelledMonthYearRow').hide();
        $("#ADMIN_QM_EMPANELLED_MONTH").val('0');//Added by deendayal on 06/21/2017 to hide Empanelled month as well

        $("#tdlblEMPANELLEDYear").hide();
        $("#tdddlEMPANELLEDYear").hide();
        $("#ADMIN_QM_EMPANELLED_YEAR").val('0');
        $("#tdEMPANELLED").attr('colspan', '');

        //$("#ADMIN_QM_PAN").removeAttr("data-val-required");
        //$('#ADMIN_QM_PAN').removeAttr('required');
        $('#ADMIN_QM_PAN').rules('remove', 'required');
        $('#ADMIN_QM_BIRTH_DATE').rules('remove', 'required');
        $('#ADMIN_QM_PAN').blur();
        $('#ADMIN_QM_BIRTH_DATE').blur();
        $('#strPAN').hide('slow');
        $('#strDOB').hide('slow');

        $("#lblEmpanelledNoo").show('slow');
        $("#listEMPANELLEDNoo").show('slow');
        $("#lblDeEmpanelledDOD").show('slow');
        $("#txtDeEmpanelledDOD").show('slow');
        $("#deEmpanelledDateRow").show('slow');
    }
}

$('#frmMasterQualityMonitor table tr:nth-last-child(3)').removeClass('ui-state-hover');
$('#frmMasterQualityMonitor table tr:nth-last-child(4)').addClass('ui-state-hover');
$('#frmMasterQualityMonitor table tr:nth-last-child(2)').addClass('ui-state-hover');
$('#frmMasterQualityMonitor table tr:nth-last-child(1)').removeClass('ui-state-hover');


function ClearDetails() {

    $('#dvErrorMessage').hide('slow');

    $('#message').html('');

    if (!$('#stateCode').is(':disabled')) {
        $('#stateCode').val('0');
    }

    $("#ADMIN_QM_FNAME").val('');
    $("#ADMIN_QM_MNAME").val('');
    $("#ADMIN_QM_LNAME").val('');
    $("#ADMIN_QM_ADDRESS1").val('');
    $("#ADMIN_QM_ADDRESS2").val('');
    $("#ADMIN_QM_DESG").val('');
    $("#MAST_DISTRICT_CODE").val('');

    $("#ADMIN_QM_PIN").val('');
    $("#ADMIN_QM_STD1").val('');
    $("#ADMIN_QM_PHONE1").val('');

    $("#ADMIN_QM_STD2").val('');
    $("#ADMIN_QM_PHONE2").val('');

    $("#ADMIN_QM_MOBILE1").val('');
    $("#ADMIN_QM_MOBILE2").val('');

    $("#ADMIN_QM_STD_FAX").val('');
    $("#ADMIN_QM_FAX").val('');

    $("#ADMIN_QM_EMAIL").val('');
    $("#ADMIN_QM_PAN").val('');

    $("#ADMIN_QM_EMPANELLED_YEAR").val('');

    $("#ADMIN_QM_REMARKS").val('');
}

function downloadPDF() {
    //alert("in downloadPDF")
    userPANData = $('#PANDownload').val();
    var url = "/Master/DownloadPANFile/" + userPANData;
    window.location = url;
}


//function DeletePANFileDetails() {
//    alert($('#PANDelete').val())
//    AdminQmCode = $('#PANDelete').val();
//    if (confirm("Are you sure to delete the PAN File and it's details ? "))
//    {
//        $.ajax({
//            url: "/Master/DeleteMonitorPANFile/" + AdminQmCode,
//            type: "POST",
//            cache: false,
//            //data: {
//            //    AdminQmCode
//            //},
//            beforeSend: function () {
//                blockPage();
//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            },
//            success: function (response) {
//                unblockPage();
//                if (response.Success) {
//                    $("#showFileEdit").hide();
//                    $("#browsefile").show();
//                    //var url = "/Master/EditMasterQualityMonitor/" + AdminQmCode;
//                    //window.location = url;
//                    //$("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
//                    //alert("PAN and it's Details Deleted Succesfully.");
//                    //$("#tbPANFileList").trigger('reloadGrid');
//                    //$("#tblQualityMonitorListDetails").trigger('reloadGrid');
//                }
//                else {
//                    alert(response.ErrorMessage);
//                }
//            }
//        });

//    }
//    else {
//        return;
//    }
//}

