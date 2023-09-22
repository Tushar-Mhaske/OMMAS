$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmBankDetailsNO");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadBankListNO();

    $("#btnReset").click(function () {
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

    $("#btnCancelBankDetails").click(function () {
        $("#dvContractorBankDetailsNO").show('slow');
        //$("#dvContractorBankDetails").load('/Master/AddBankDetails/' + $("#MAST_CON_ID").val() + "$" + $("#RegStateCode").val());
        $("#dvContractorBankDetailsNO").load('/Master/AddBankDetailsNO/' + $("#NodalOfficerCode").val());
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


    $("#btnSaveBankDetailsNO").click(function () {
        $("#ErrorMessage").show();
        if ($("#frmBankDetailsNO").valid()) {
            $.ajax({
                url: "/Master/CheckExistRecordNO",
                type: "POST",

                async: false,
                cache: false,
                data: $("#frmBankDetailsNO").serialize(),
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
                        $("#dvContractorBankDetailsNO").html(data);
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


    $("#btnUpdateBankDetailsNO").click(function () {

        if ($('#frmBankDetailsNO').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#MAST_DISTRICT_CODE").attr("disabled", false);
            $("#Mast_State_Code").attr("disabled", false);
            $.ajax({
                url: "/Master/EditBankDetailsNO",
                type: "POST",
                data: $("#frmBankDetailsNO").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tblBankDetailsListNO').trigger('reloadGrid');
                        $("#dvContractorBankDetailsNO").show('slow');
                        $("#dvContractorBankDetailsNO").load('/Master/AddBankDetailsNO/' + $("#NodalOfficerCode").val());
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
                        $("#dvContractorBankDetailsNO").html(data);
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
        url: "/Master/AddBankDetailsNO",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetailsNO").serialize(),
        success: function (data) {
            if (data.success) {
                alert(data.message);
                $('#tblBankDetailsListNO').trigger('reloadGrid');
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
        url: "/Master/AddBankDetailsNO",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetailsNO").serialize(),
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $('#tblBankDetailsListNO').trigger('reloadGrid');
                $("#btnResetBankDetailsNO").trigger('click');

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
                url: '/Master/DistrictSelectDetails',
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

function LoadBankListNO() {

    $.ajax({
        url: "/Master/GetBankList",
        cache: false,
        type: "GET",
        async: false,
        success: function (data) {


            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#MAST_BANK_NAME').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })
}