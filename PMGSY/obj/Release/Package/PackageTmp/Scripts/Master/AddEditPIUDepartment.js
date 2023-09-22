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
    $.validator.unobtrusive.parse('#frmAddPIU');
   
    if ($('#MAST_PARENT_ND_CODE').val() > 0) {
        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                        "#ddlDistrict", "/Master/GetDistrictsListByAdminNDCode?stateCode=" + $('#MAST_STATE_CODE').val() + "&" + "AdminNdCode=" + $('#MAST_PARENT_ND_CODE').val());
        if ($('#hdDistrictCode').val() > 0) {
            setTimeout(function () {
                //alert($('#hdDistrictCode').val());
                $('#ddlDistrict').val($('#hdDistrictCode').val());
            }, 1300);
        }

    }
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


   

    $("#ddlState").change(function () {


        //FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
        //            "#ddlDistrict", "/Master/GetDistrictsListByAdminNDCode?stateCode=" + $('#ddlState option:selected').val() + "&" + $('#ddlPARENT_ND_CODE_List option:selected').val());
        //
        loadAgencyList($("#ddlState").find(":selected").val());

        $('#ddlDistrict').val(0);
        $('#ddlDistrict').empty();
        $("#ddlDistrict").append("<option value='0'>--Select--</option>");

        ClearMessage();
    });
    
    $('#ddlPARENT_ND_CODE_List').change(function () {
        if ($('#ddlPARENT_ND_CODE_List').val() > 0 && +$('#ddlState').val() > 0) {
            FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                        "#ddlDistrict", "/Master/GetDistrictsListByAdminNDCode?stateCode=" + $('#ddlState option:selected').val() + "&" + "AdminNdCode=" + $('#ddlPARENT_ND_CODE_List option:selected').val());
            if ($('#hdDistrictCode').val() > 0) {
                setTimeout(function () {
                    //alert($('#hdDistrictCode').val());
                    $('#ddlDistrict').val($('#hdDistrictCode').val());
                }, 1000);
            }
        } else {
            $('#ddlDistrict').val(0);
            $('#ddlDistrict').empty();
            $("#ddlDistrict").append("<option value='0'>--Select--</option>");
        }
    });

    if ($('#stateCode').val() > 0 && $('#btnSave').is(':visible')) {

        $("#ddlState").val($('#stateCode').val());
        $("#ddlState").attr("disabled", true);
        $("#ddlState").trigger('change');
    }

    $('#ddlAgency').change(function () {
        loadSSRDADropDopwnList($('#ddlAgency').val());
    });


    $("#dvhdAddNewPIUDetails").click(function () {

        if ($("#dvAddNewPIUDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewPIUDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewPIUDetails").slideToggle(300);
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
    $("#btnSave").click(function (e) {


        if ($("#frmAddPIU").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            var stateCode = 0;
            var ParentNDCode = 0;
            if ($("#ddlState").is(":visible")) {
                 stateCode = $("#ddlState option:selected").val();
            } else {
                stateCode = $('#MAST_STATE_CODE').val();
            }
            if ($("#ddlPARENT_ND_CODE_List").is(":visible")) {
                 ParentNDCode = $("#ddlPARENT_ND_CODE_List option:selected").val();
            }
            else {
                ParentNDCode = $('#MAST_PARENT_ND_CODE').val();
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddEditPIUDepartment/',
                async: false,
                data: $("#frmAddPIU").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);

                        if ($("#PIUAddDetails").is(":visible")) {
                            $('#PIUAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#PIUSearchDetails").is(":visible")) {
                            $("#PIUSearchDetails").show('slow');
                        }
                        ClearDetails();
                        SearchDetails(stateCode, ParentNDCode);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                       
                    }
                    else {
                        $("#PIUAddDetails").html(data);
                       
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

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditPIUDepartment",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#PIUAddDetails").html(data);
        //        $("#PIUAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#PIUAddDetails").is(":visible")) {
            $('#PIUAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#PIUSearchDetails").is(":visible")) {
            $("#PIUSearchDetails").show('slow');
        }
        ClearDetails();


    })

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();
        //if ($('#stateCode').val() > 0) {

        //    $("#ddlState").val($('#stateCode').val());
        //    $("#ddlState").attr("disabled", true);
        //    $("#ddlState").trigger('change');

        //}
    });

    $("#btnUpdate").click(function (e) {


        if ($("#frmAddPIU").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlDistrict").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            $("#ddlPARENT_ND_CODE_List").attr("disabled", false);
            var stateCode = 0;
            var ParentNDCode = 0;
            if ($("#ddlState").is(":visible")) {
                stateCode = $("#ddlState option:selected").val();
            } else {
                stateCode = $('#MAST_STATE_CODE').val();
            }
            if ($("#ddlPARENT_ND_CODE_List").is(":visible")) {
                ParentNDCode = $("#ddlPARENT_ND_CODE_List option:selected").val();
            }
            else {
                ParentNDCode = $('#MAST_PARENT_ND_CODE').val();
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditPIUDepartment/',
                async: false,
                data: $("#frmAddPIU").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#PIUAddDetails").load("/Master/AddEditPIUDepartment");

                        //$('#PIUCategory').trigger('reloadGrid');
                        if ($("#PIUAddDetails").is(":visible")) {
                            $('#PIUAddDetails').hide('slow');

                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#PIUSearchDetails").is(":visible")) {
                            $("#PIUSearchDetails").show('slow');
                        }
                        ClearDetails();
                        SearchDetails(stateCode, ParentNDCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                            $("#ddlPARENT_ND_CODE_List").attr("disabled", true);
                            
                        }
                    }
                    else {
                        $("#PIUAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);
                        $("#ddlPARENT_ND_CODE_List").attr("disabled", true);

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

function SearchDetails(stateCode, ParentNDCode) {
    $('#State').val(stateCode);
    $('#State').trigger('change');
    setTimeout(function () {
        $('#ddlSSRDA').val(ParentNDCode);
    }, 1000);
    setTimeout(function () {
        $('#PIUCategory').setGridParam({
            url: '/Master/GetPIUDepartmentList'
        });

        $('#PIUCategory').jqGrid("setGridParam", { "postData": { stateCode: $('#State option:selected').val(), agency: "", adminNDCode: $('#ddlSSRDA option:selected').val(), active: "" } });
        $('#PIUCategory').trigger("reloadGrid", [{ page: 1 }]);
    }, 1500);
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
    $('#ddlState').val('0');
    $('#ddlAgency').val('0');
    $('#ddlPARENT_ND_CODE_List').val('0');
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

function loadAgencyList(statCode) {
    $("#ddlAgency").val(0);
    $("#ddlAgency").empty();
    $("#ddlPARENT_ND_CODE_List").val(0);
    $("#ddlPARENT_ND_CODE_List").empty();
    $("#ddlPARENT_ND_CODE_List").append("<option value='0'>--Select--</option>");

    if (statCode > 0) {
        if ($("#ddlAgency").length > 0) {
            $.ajax({
                url: '/Master/GetAgencyListByState',
                type: 'POST',
                data: { "stateCode": statCode, "IsAllSelected": false },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }



                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlAgency").append("<option value='0'>--Select--</option>");
        $("#ddlPARENT_ND_CODE_List").empty();
        $("#ddlPARENT_ND_CODE_List").append("<option value='0'>--Select--</option>");

    }
}

function loadSSRDADropDopwnList(agencyCode) {
    $("#ddlPARENT_ND_CODE_List").val(0);
    $("#ddlPARENT_ND_CODE_List").empty();


    if (agencyCode > 0) {
        if ($("#ddlPARENT_ND_CODE_List").length > 0) {
            $.ajax({
                url: '/Master/GetSSRDAList',
                type: 'POST',
                data: { "stateCode": $('#ddlState').val(), "agencyCode": agencyCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlPARENT_ND_CODE_List").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#ddlPARENT_ND_CODE_List').find("option[value='0']").remove();
                    //$("#ddlPARENT_ND_CODE_List").append("<option value='0'>Select District</option>");
                    //$('#ddlPARENT_ND_CODE_List').val(0);





                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlPARENT_ND_CODE_List").append("<option value='0'>All Districts</option>");
        //$("#BlockList_EstiMatedMaintDetail").empty();
        //$("#BlockList_EstiMatedMaintDetail").append("<option value='0'>All Blocks</option>");

    }
}





