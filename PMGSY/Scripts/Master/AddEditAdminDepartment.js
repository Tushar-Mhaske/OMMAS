jQuery.validator.addMethod("customvalidation", function (value, element, param) {

    if ($("#radioBankAuthYes").is(":checked") && $('#ADMIN_BA_ENABLE_DATE').val() == '')
        return false;
    else
        return true;

    if ($("#radioRemitYes").is(":checked") && $('#ADMIN_EREMIT_ENABLED_DATE').val() == '')
        return false;
    else
        return true;

    if ($("#radioEpayYes").is(":checked") && $('#ADMIN_EPAY_ENABLE_DATE').val() == '')
        return false;
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("customvalidation");

//PIU Close Date 
jQuery.validator.addMethod("customvalidationpiuclosedate", function (value, element, param) {
    if ($("#radioPiuIsActiveNo").is(":checked") && $('#PIU_Close_DATE').val() == '')
        return false;
    else
        return true;
});
jQuery.validator.unobtrusive.adapters.addBool("customvalidationpiuclosedate");

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddAdmin');

   
  

    if ($("#radioBankAuthYes").is(":checked")) {

        AuthenticationYes();
    }

    if ($("#radioRemitYes").is(":checked")) {
        RemittanceYes();
    }

    if ($("#radioEpayYes").is(":checked")) {
        EpayYes();
    }
    if ($("#radioPiuIsActiveNo").is(":checked")) {
        PIUACtiveNo();
    }

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#ADMIN_SBD_DATE, #ADMIN_BA_ENABLE_DATE, #ADMIN_EPAY_ENABLE_DATE, #ADMIN_EREMIT_ENABLED_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        }
    });
    $('#PIU_Close_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        }
    });


    $("#btnSave").click(function (e) {

        if ($("#frmAddAdmin").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);

            var stateCode = $("#ddlState option:selected").val();
            var agencyCode = $("#ddlAgency option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddEditAdminDepartment/',
                async: false,
                data: $("#frmAddAdmin").serialize(),
                success: function (data) {



                    if (data.success == true) {
                        alert(data.message);
                      
                        
                        if ($('#MAST_ND_TYPE').val() == 'D') {
                            $("#ddlState").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);

                        }
                        if ($('#stateCode').val() > 0) {
                            $("#ddlState").val($('#stateCode').val());
                            $("#ddlState").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                        }
                        ////////////////////////////////////////////
                        if ($("#adminAddDetails").is(":visible")) {
                            $('#adminAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#adminSearchDetails").is(":visible")) {
                            $("#adminSearchDetails").show('slow');
                        }
                        SearchAdminDetails(stateCode, agencyCode);
                        ClearDetails();
                    }
                    else if (data.success == false) {


                        if ($('#MAST_ND_TYPE').val() == 'D') {
                            $("#ddlState").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                        }

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                        if ($('#stateCode').val() > 0) {
                            $("#ddlState").val($('#stateCode').val());
                            $("#ddlState").attr("disabled", true);
                            if ($('#RoleCode').val() == 47 || $('#RoleCode').val() == 36)//RoleCode ITNOOA-47 && ITNO-36
                            {
                                $("#ddlAgency").attr("disabled", true);
                            }

                        }
                    }
                    else {
                        $("#adminAddDetails").html(data);
                        if ($('#stateCode').val() > 0) {
                            $("#ddlState").val($('#stateCode').val());
                            $("#ddlState").attr("disabled", true);
                            if ($('#RoleCode').val() == 47 || $('#RoleCode').val() == 36)//RoleCode ITNOOA-47 && ITNO-36
                            {
                                $("#ddlAgency").attr("disabled", true);
                            }
                        }
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    if ($('#stateCode').val() > 0) {
                        $("#ddlState").val($('#stateCode').val());
                        $("#ddlState").attr("disabled", true);
                    }
                }
            })
        }



    });

    $("#ddlState").change(function () {


        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                    "#ddlDistrict", "/Master/GetDistrictsList?stateCode=" + $('#ddlState option:selected').val());
        ClearMessage();
    });

    if ($('#stateCode').val() > 0 && $('#btnSave').is(':visible')) {

        $("#ddlState").val($('#stateCode').val());
        $("#ddlState").attr("disabled", true);
        $("#ddlState").trigger('change');
    }
    if ($('#stateCode').val() > 0) {
        $('#MAST_AGENCY_CODE').attr("disabled", true);
    }


    $("#dvhdAddNewAdminDetails").click(function () {

        if ($("#dvAddNewAdminDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewAdminDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewAdminDetails").slideToggle(300);
        }
    });

    $("#radioBankAuthYes").change(function () {
        AuthenticationYes();

    });



    $("#radioBankAuthNo").change(function () {
        AuthenticationNo();
    });


    $("#radioRemitYes").change(function () {
        RemittanceYes();



    });

    $("#radioRemitNo").change(function () {
        RemittanceNo();

    });


    $("#radioEpayYes").change(function () {
        EpayYes();


    });

    $("#radioEpayNo").change(function () {
        EpayNo();
    });

    //New PIU
    $("#radioPiuIsActiveYes").change(function () {
        PIUActiveYes();
    });

    $("#radioPiuIsActiveNo").change(function () {
        PIUACtiveNo();
    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditAdminDepartment",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#adminAddDetails").html(data);
        //        $("#adminAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        ////////////////////////////////////////////
        if ($("#adminAddDetails").is(":visible")) {
            $('#adminAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#adminSearchDetails").is(":visible")) {
            $("#adminSearchDetails").show('slow');
        }      
        ClearDetails();

    })

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();
        if ($('#stateCode').val() > 0) {

            $("#ddlState").val($('#stateCode').val());
            $("#ddlState").attr("disabled", true);
            $("#ddlState").trigger('change');

        }
    });

    $("#btnUpdate").click(function (e) {


        if ($("#frmAddAdmin").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlDistrict").attr("disabled", false);
            if ($('#RoleCode').val() != 23) {
                $("#ddlAgency").attr("disabled", false);
            }           
            var stateCode = $("#ddlState").val();
            var agencyCode = $("#ddlAgency option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditAdminDepartment/',
                async: false,
                data: $("#frmAddAdmin").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#adminAddDetails").load("/Master/AddEditAdminDepartment");

                        //$('#adminCategory').trigger('reloadGrid');

                        ////////////////////////////////////////////
                        if ($("#adminAddDetails").is(":visible")) {
                            $('#adminAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#adminSearchDetails").is(":visible")) {
                            $("#adminSearchDetails").show('slow');
                        }
                        SearchAdminDetails(stateCode,agencyCode);
                        ClearDetails();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            if ($('#RoleCode').val() != 23) {
                                $("#ddlAgency").attr("disabled", true);
                            }
                        }
                    }
                    else {
                        $("#adminAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        if ($('#RoleCode').val() != 23) {
                            $("#ddlAgency").attr("disabled", true);
                        }
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


});

function SearchAdminDetails(stateCode,agencyCode) {
    $('#State').val(stateCode);
    $('#Agency').val(agencyCode);
    $('#adminCategory').setGridParam({
        url: '/Master/GetDepartmentList'
    });


    $('#adminCategory').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), agency: $('#Agency option:selected').val() } });
    $('#adminCategory').trigger("reloadGrid", [{ page: 1 }]);
}

function ClearMessage() {

    if ($("#dvErrorMessage").is(":visible")) {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    }

}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#ddlDistrict') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
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
    
    if ($('#MAST_ND_TYPE').val() == 'S') {

        $('#ddlState').val('0');       
        $('#ddlAgency').val('0');
    }


    $('#ddlDistrict').val('0');

    $('#ADMIN_ND_NAME').val('');
    $('#ADMIN_ND_ADDRESS1').val('');
    $('#ADMIN_ND_ADDRESS2').val('');
    $('#ADMIN_ND_PIN').val('');
    $('#ADMIN_ND_STD1').val('');
    $('#ADMIN_ND_STD2').val('');
    $('#ADMIN_ND_PHONE1').val('');
    $('#ADMIN_ND_PHONE2').val('');
    $('#ADMIN_ND_STD_FAX').val('');
    $('#ADMIN_ND_FAX').val('');
    $('#ADMIN_ND_MOBILE_NO').val('');
    $('#ADMIN_ND_EMAIL').val('');
    $('#ADMIN_ND_REMARKS').val('');
    $('#ADMIN_SBD_DATE').val('');
    $('#dataPdf').val('');
    $('#dataEmail').val('');
    $('#ADMIN_ND_TAN_NO').val('');

    $('#ADMIN_BA_ENABLE_DATE').val('');

    $('#ADMIN_EPAY_ENABLE_DATE').val('');

    $("ADMIN_EREMIT_ENABLED_DATE").val('');
    $('#PIU_Close_DATE').val('');
    $('#radioBankAuthNo').attr('checked', true);
    $('#radioRemitNo').attr('checked', true);
    $('#radioEpayNo').attr('checked', true);
    $('#radioPiuIsActiveYes').attr('checked', true);


    AuthenticationNo();
    RemittanceNo();
    EpayNo();
    PIUActiveYes();


    $('#dvErrorMessage').hide('slow');
    $('#message').html('');


}



function AuthenticationYes() {

    if ($("#radioBankAuthYes").is(":checked")) {

        $("#tdBankAuthEnabled").attr('colspan', '1');
        $("#tdlblBank").show('slow');
        $("#dataBank").show('slow');

    }
}

function EpayYes() {

    if ($("#radioEpayYes").is(":checked")) {

        $("#tdEPayEnabled").attr('colspan', '1');
        $("#tdlblEpay").show('slow');
        $("#dataEpay").show('slow');
        $("#rowEpay").show('slow');
    }
}



function RemittanceYes() {

    if ($("#radioRemitYes").is(":checked")) {

        $("#tdERemittanceEnabled").attr('colspan', '1');
        $("#tdlblRemit").show('slow');
        $("#dataRemit").show('slow');
    }
}



function AuthenticationNo() {

    if ($("#radioBankAuthNo").is(":checked")) {
        $("#tdlblBank").hide();
        $("#dataBank").hide();
        $("#ADMIN_BA_ENABLE_DATE").val('');
        $("#tdBankAuthEnabled").attr('colspan', '3');
    }
}

function RemittanceNo() {
    if ($("#radioRemitNo").is(":checked")) {
        $("#tdlblRemit").hide();
        $("#dataRemit").hide();
        $("#ADMIN_EREMIT_ENABLED_DATE").val('');
        $("#tdERemittanceEnabled").attr('colspan', '3');
    }

}

function EpayNo() {

    if ($("#radioEpayNo").is(":checked")) {
        $("#tdlblEpay").hide();
        $("#dataEpay").hide();
        $("#tdEPayEnabled").attr('colspan', '3');
        $("#rowEpay").hide();

        $("#ADMIN_EPAY_ENABLE_DATE").val('');
        $("#dataEmail").val('');
        $("#dataPdf").val('');

    }
}

function PIUACtiveNo() {

    if ($("#radioPiuIsActiveNo").is(":checked")) {

        $("#tdPiuIsActive").attr('colspan', '1');
        $("#tdlblPiuIsActive").show('slow');
        $("#dataPiuIsActive").show('slow');
    }
}

function PIUActiveYes() {
    if ($("#radioPiuIsActiveYes").is(":checked")) {
        $("#tdlblPiuIsActive").hide();
        $("#dataPiuIsActive").hide();
        $("#PIU_Close_DATE").val('');
        $("#tdPiuIsActive").attr('colspan', '3');
    }

}




