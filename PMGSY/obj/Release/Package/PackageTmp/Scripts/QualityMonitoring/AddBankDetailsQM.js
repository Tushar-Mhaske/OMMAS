$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmBankDetailsQM");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnCancelBankDetailsQM").click(function (e) {
        
        $('#btnResetBankDetailsQM').trigger('click');
        $("#dvBankDetailsQM").hide('slow');

        
        $('#tblBankDetailsListQM').jqGrid("setGridState", "visible");

        //$("#btnCreateNew").show();
        //$("#btnSearchView").hide();
        //$("#dvSearchNodalOfficer").show();
        ////$("#dvSearchContractorRegQM").show("slow");
        //$("#dvDetailsNodalOfficer").show("slow");

        $("#mainDiv").animate({
            scrollTop: 0
        });
    });

    $("#btnResetBankDetailsQM").click(function () {
        $("#dvErrorMessage").hide("slow");
    });
    $("#MAST_ACCOUNT_NUMBER").focus(function () {
        $("#dvErrorMessage").hide("slow");
    });
    $("#MAST_BANK_NAME").focus(function () {
        $("#dvErrorMessage").hide("slow");
    });
    $("#MAST_IFSC_CODE").focus(function () {
        $("#dvErrorMessage").hide("slow");
    });

    $("#btnCancelBankDetailsQM").click(function () {
        $("#dvContractorBankDetailsQM").show('slow');
        //$("#dvContractorBankDetails").load('/QualityMonitoring/AddBankDetails/' + $("#MAST_CON_ID").val() + "$" + $("#RegStateCode").val());
        $("#dvContractorBankDetailsQM").load('/QualityMonitoring/AddBankDetailsQM/' + $("#ADMIN_QM_CODE").val());
    });


    $("#dvhdCreateNewBankDetail").click(function () {
        if ($("#dvCreateNewBankDetail").is(":visible")) {
            $("#spCollapseIconCNBD").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvCreateNewBankDetail").slideToggle(300);
        }
        else {
            $("#spCollapseIconCNBD").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvCreateNewBankDetail").slideToggle(300);
        }
    });


    $("#btnSaveBankDetailsQM").click(function () {
        $("#ErrorMessage").show();
            {
            $.ajax({
                url: "/QualityMonitoring/CheckExistRecordQM",
                type: "POST",

                async: false,
                cache: false,
                data: $("#frmBankDetailsQM").serialize(),
                success: function (data) {

                    if (data.success == true) {

                        //Modified By Abhishek kamble 20-feb-2014 start

                        if (data.isBankDetailsExists == true) {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        } else {

                            //Modified By Abhishek kamble 20-feb-2014 end

                            if (data.alreadyExists == true) {
                                if (confirm("Already 'Active' Bank details exist for selected district,would you like to 'InActive' existing Bank details and enter new Bank details?")) {
                                    confirmAlreadyExists();
                                }
                                else {
                                    return false;
                                }
                            }
                            else {
                                addBankDetails();
                            }

                        }
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            alert("error");
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvBankDetailsQM").html(data);
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


    $("#btnUpdateBankDetailsQM").click(function () {

        if ($('#frmBankDetailsQM').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#MAST_DISTRICT_CODE").attr("disabled", false);
            $("#Mast_State_Code").attr("disabled", false);
            $.ajax({
                url: "/QualityMonitoring/EditBankDetailsQM",
                type: "POST",
                data: $("#frmBankDetailsQM").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tblBankDetailsListQM').trigger('reloadGrid');
                        $("#dvContractorBankDetailsQM").show('slow');
                        $("#dvContractorBankDetailsQM").load('/QualityMonitoring/AddBankDetailsQM/' + $("#ADMIN_QM_CODE").val());
                        $("#dvErrorMessage").hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#MAST_DISTRICT_CODE").attr("disabled", true);
                            $("#Mast_State_Code").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvContractorBankDetailsQM").html(data);
                        $("#MAST_DISTRICT_CODE").attr("disabled", true);
                        $("#Mast_State_Code").attr("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#MAST_DISTRICT_CODE").attr("disabled", true);
                    $("#Mast_State_Code").attr("disabled", true);
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }
    );
    $("#Mast_State_Code").change(function () {
        loadDistrictDropDopwnList($("#Mast_State_Code").val());
    });
});


function confirmAlreadyExists() {

    $.ajax({
        url: "/QualityMonitoring/AddBankDetailsQM",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetailsQM").serialize(),
        success: function (data) {
            if (data.success) {
                alert(data.message);
                $('#tblBankDetailsListQM').trigger('reloadGrid');
                $("#btnResetBankDetails").trigger('click');
            }
            else {
                if (data.message != "") {
                    $('#message').html(data.message);
                    $('#dvErrorMessage').show('slow');
                }
            }
        }
    });
}


function addBankDetails() {

    $.ajax({
        url: "/QualityMonitoring/AddBankDetailsQM",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetailsQM").serialize(),
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $('#tblBankDetailsListQM').trigger('reloadGrid');
                $("#btnResetBankDetailsQM").trigger('click');
                $('#dvBankDetailsQM').html('');
            }
            else {
                if (data.message != "") {
                    $('#message').html(data.message);
                    $('#dvErrorMessage').show('slow');
                }
            }
        }
    });
}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    message = '<h4><label style="font-weight:normal"> Loading Class... </label></h4>';

    $(dropdown).empty();


    $.blockUI({ message: message });

    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

function loadDistrictDropDopwnList(stateCode) {
    $("#MAST_DISTRICT_CODE").val(0);
    $("#MAST_DISTRICT_CODE").empty();


    if (stateCode > 0) {
        if ($("#MAST_DISTRICT_CODE").length > 0) {
            $.ajax({
                url: '/QualityMonitoring/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#MAST_DISTRICT_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

        $("#MAST_DISTRICT_CODE").append("<option value='0'>Select District</option>");
        //$("#BlockList_EstiMatedMaintDetail").empty();
        //$("#BlockList_EstiMatedMaintDetail").append("<option value='0'>All Blocks</option>");

    }
}