
$.validator.unobtrusive.adapters.add('checkrolewisevalidationattribute', ['previousval'], function (options) {
    options.rules['checkrolewisevalidationattribute'] = options.params;
    options.messages['checkrolewisevalidationattribute'] = options.message;
});

$.validator.addMethod("checkrolewisevalidationattribute", function (value, element, params) {
    if (value == '' || value == null) {
        return false;
    }
    else {
        return true;
    }
});

$(document).ready(function () {
    if ($('#IMS_EC_TYPE_HD').val() == "P") {
        $('#spandivhdAddEditEC').html("Pre EC Check List Details");
        $('#spnListAddEC').html("Add Pre EC Check List");
        $('#btnAdd').attr("title", "Add Pre EC Check List");
        $('#spnListSearchEC').html("Search Pre EC Check List");
        $('#btnSearch').attr("title", "Search Pre EC Check List");
        $('#btnImsEcCheckSearch').attr("title", "Search Pre EC Check List");


    } else {
        $('#spandivhdAddEditEC').html("EC Check List Details");
        $('#spnListAddEC').html("Add EC Check List");
        $('#btnAdd').attr("title", "Add EC Check List");
        $('#spnListSearchEC').html("Search EC Check List");
        $('#btnSearch').attr("title", "Search EC Check List");
        $('#btnImsEcCheckSearch').attr("title", "Search EC Check List");
    }
    $('#IMS_EC_TYPE').val($('#IMS_EC_TYPE_HD').val());
    $.validator.unobtrusive.parse('#frmAddImsEcCheck');

    $('#IMS_SSR_DATE, #IMS_CN_APPROVAL_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });
    $('#IMS_SSR_DATE_NRRDA').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
        $(this).focus().blur();
    }
    });

    $("#ddlState").change(function () {
        loadAgencyList($("#ddlState").find(":selected").val());
        ClearMessage();
    });


    $("#dvhdAddNewImsEcCheckDetails").click(function () {

        if ($("#dvAddNewImsEcCheckDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewImsEcCheckDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewImsEcCheckDetails").slideToggle(300);
        }
    });


    $("#btnSave").click(function (e) {
        $('#HdRoleTypeEntry').val("M");

        if ($("#frmAddImsEcCheck").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            $('#IMS_EC_TYPE').val($('#IMS_EC_TYPE_HD').val());
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddEditImsEcCheck/',
                async: false,
                data: $("#frmAddImsEcCheck").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        if ($("#ImsEcCheckAddDetails").is(":visible")) {
                            $('#ImsEcCheckAddDetails').hide('slow');

                            $('#btnSearch').hide();

                            $('#btnAdd').show();
                            if ($('#RoleCode').val() == 25) {
                                $('#btnAdd').hide();
                            }
                        }
                        if (!$("#ImsEcCheckSearchDetails").is(":visible")) {
                            $('#ImsEcCheckSearchDetails').show('slow');
                        }
                        SearchAddEcDetail(stateCode, agency, year, batch);
                        // $("#ddlState").attr("disabled", true);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                        if ($('#hdStatCode').val() > 0) {
                            $("#ddlState").val($('#hdStatCode').val());
                            $("#ddlState").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                        }
                    }
                    else {
                        $("#ImsEcCheckAddDetails").html(data);
                        //if ($('#stateCode').val() > 0) {
                        //    $("#ddlState").val($('#stateCode').val());
                        //    $("#ddlState").attr("disabled", true);
                        //}
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    if ($('#hdStatCode').val() > 0) {
                        $("#ddlState").val($('#hdStatCode').val());
                        $("#ddlState").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);
                    }
                }
            })
        }



    });


    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditImsEcCheck",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#ImsEcCheckAddDetails").html(data);
        //        $("#ImsEcCheckAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#ImsEcCheckAddDetails").is(":visible")) {
            $('#ImsEcCheckAddDetails').hide('slow');

            $('#btnSearch').hide();

            $('#btnAdd').show();
            if ($('#RoleCode').val() == 25) {
                $('#btnAdd').hide();
            }
        }
        if (!$("#ImsEcCheckSearchDetails").is(":visible")) {
            $('#ImsEcCheckSearchDetails').show('slow');
        }

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

    $("#btnSameAsSRRDA").click(function (e) {
        $('#HdRoleTypeEntry').val("S");
     
            $('#IMS_EC_TYPE').val($('#IMS_EC_TYPE_HD').val());
            $("#ddlState").attr("disabled", false);
            $("#ddlPhaseYear").attr("disabled", false);
            $("#ddlBatch").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditImsEcCheck/',
                async: false,
                data: $("#frmAddImsEcCheck").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#ImsEcCheckAddDetails").load("/Master/AddEditImsEcCheck");

                        //$('#tblImsEcCheck').trigger('reloadGrid');

                        ClearDetails();
                        if ($("#ImsEcCheckAddDetails").is(":visible")) {
                            $('#ImsEcCheckAddDetails').hide('slow');

                            $('#btnSearch').hide();

                            $('#btnAdd').show();
                            if ($('#RoleCode').val() == 25) {
                                $('#btnAdd').hide();
                            }
                        }
                        if (!$("#ImsEcCheckSearchDetails").is(":visible")) {
                            $('#ImsEcCheckSearchDetails').show('slow');
                        }
                        SearchAddEcDetail(stateCode, agency, year, batch);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);
                            $("#ddlBatch").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);

                        }
                    }
                    else {
                        $("#ImsEcCheckAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlPhaseYear").attr("disabled", true);
                        $("#ddlBatch").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);

                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })        
    });

    $("#btnUpdate").click(function (e) {
        $('#HdRoleTypeEntry').val("M");
        if ($("#frmAddImsEcCheck").valid()) {
            $('#IMS_EC_TYPE').val($('#IMS_EC_TYPE_HD').val());
            $("#ddlState").attr("disabled", false);
            $("#ddlPhaseYear").attr("disabled", false);
            $("#ddlBatch").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditImsEcCheck/',
                async: false,
                data: $("#frmAddImsEcCheck").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#ImsEcCheckAddDetails").load("/Master/AddEditImsEcCheck");

                        //$('#tblImsEcCheck').trigger('reloadGrid');

                        ClearDetails();
                        if ($("#ImsEcCheckAddDetails").is(":visible")) {
                            $('#ImsEcCheckAddDetails').hide('slow');

                            $('#btnSearch').hide();

                            $('#btnAdd').show();
                            if ($('#RoleCode').val() == 25) {
                                $('#btnAdd').hide();
                            }
                        }
                        if (!$("#ImsEcCheckSearchDetails").is(":visible")) {
                            $('#ImsEcCheckSearchDetails').show('slow');
                        }
                        SearchAddEcDetail(stateCode, agency, year, batch);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);
                            $("#ddlBatch").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);

                        }
                    }
                    else {
                        $("#ImsEcCheckAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlPhaseYear").attr("disabled", true);
                        $("#ddlBatch").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);

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

    $("#btnBack").click(function (e) {
        if ($("#ImsEcCheckAddDetails").is(":visible")) {
            $('#ImsEcCheckAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
            if ($('#RoleCode').val() == 25) {
                $('#btnAdd').hide();
            }
        }

        $('#ImsEcCheckSearchDetails').show('slow');
    });

});

function SearchAddEcDetail(stateCode, agency, year, batch) {
    $('#ddlStateSerach').val(stateCode);
    $('#ddlStateSerach').trigger('change');
    setTimeout(function () {
        $('#ddlAgencySerach').val(agency);
    }, 700);
    // alert("batch" + batch + "___Year" + year);

    $('#ddlPhaseYearSerach').val(year);
    $('#ddlBatchSerach').val(batch);
    setTimeout(function () {
        $('#tblImsEcCheck').setGridParam({
            url: '/Master/GetImsEcCheckList'
        });

        $('#tblImsEcCheck').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlStateSerach option:selected').val(), agency: $('#ddlAgencySerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val(), batch: $('#ddlBatchSerach option:selected').val() } });
        $('#tblImsEcCheck').trigger("reloadGrid", [{ page: 1 }]);
    }, 1200);
}

function ClearMessage() {

    if ($("#dvErrorMessage").is(":visible")) {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    }

}



function ClearDetails() {
    $('#ddlState').val('0');
    if ($('#hdStatCode').val() > 0) {
        $("#ddlState").val($('#hdStatCode').val());
        $("#ddlState").attr("disabled", true);
    }
    $('#ddlPhaseYear').val('0');
    $('#ddlBatch').val('0');
    $('#ddlAgency').val('0');

    $('#radioIMS_CN_READYNo').attr('checked', true);
    $('#radioIMS_DRRP_OMMASNo').attr('checked', true);
    $('#radioIMS_CNCUPL_READYNo').attr('checked', true);
    $('#radioIMS_DP_APPROVALNo').attr('checked', true);
    $('#radioIMS_IMS_SLSC_PROCEEDINGNo').attr('checked', true);
    $('#radioIMS_DPR_SCRUTINYNo').attr('checked', true);
    $('#radioIMS_PCI_REGISTERNo').attr('checked', true);
    $('#radioIMS_UNSEALEDNo').attr('checked', true);
    $('#radioIMS_MP_DATANo').attr('checked', true);
    $('#radioIMS_MAINT_YEARWISENo').attr('checked', true);
    $('#radioIMS_ESTIMATE_SSRNo').attr('checked', true);
    $('#radioIMS_DPR_STANo').attr('checked', true);
    $('#radioIMS_NIT_UPLOADEDNo').attr('checked', true);
    $('#radioIMS_WORK_CAPACITYNo').attr('checked', true);
    $('#radioIMS_IPAI_ACCOUNTSNo').attr('checked', true);
    $('#radioIMS_LWE_MHANo').attr('checked', true);
    $('#radioIMS_WB_BATCH_SIZENo').attr('checked', true);
    $('#radioIMS_WB_ECOPNo').attr('checked', true);
    $('#radioIMS_WB_STA_CLEAREDNo').attr('checked', true);


    $('#IMS_SSR_DATE').val('');
    $('#IMS_CN_APPROVAL_DATE').val('');
    $('#IMS_CN_APPROVAL_DATE').val('');
    $('#IMS_CE_REMARKS').val('');
    $('#IMS_NRRDA_REMARKS').val('');

    $('#dvErrorMessage').hide('slow');

    $('#message').html('');


}

function loadAgencyList(statCode) {
    $("#ddlAgency").val(0);
    $("#ddlAgency").empty();
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

    }
}







