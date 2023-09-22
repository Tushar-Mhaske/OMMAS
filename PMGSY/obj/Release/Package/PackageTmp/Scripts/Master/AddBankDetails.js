$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmBankDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadBankList();
    LoadIfscList();

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
        $("#dvContractorBankDetails").show('slow');
        $("#dvContractorBankDetails").load('/Master/AddBankDetails/' + $("#MAST_CON_ID").val() + "$" + $("#RegStateCode").val());
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


    $("#btnSaveBankDetails").click(function () {
        $("#ErrorMessage").show();
        if ($("#frmBankDetails").valid()) {
            $.ajax({
                url: "/Master/CheckExistRecord",
                type: "POST",

                async: false,
                cache: false,
                data: $("#frmBankDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {

                        //Modified By Abhishek kamble 20-feb-2014 start

                        if (data.isBankDetailsExists == true) {
                            $('#errmessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        } else {

                            //Modified By Abhishek kamble 20-feb-2014 end

                            //if (data.alreadyExists == true) {
                            //    if (confirm("Already 'Active' Bank details exist for selected district,would you like to 'InActive' existing Bank details and enter new Bank details?")) {
                            //        confirmAlreadyExists();
                            //    }
                            //    else {
                            //        return false;
                            //    }
                            //}
                            //else {
                            //    addBankDetails();
                            //}
                            addBankDetails();
                        }
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            alert("error");
                            $('#errmessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvContractorBankDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    $.unblockUI();

                }
            });
        }
    });


    $("#btnUpdateBankDetails").click(function () {

        if ($('#frmBankDetails').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#MAST_DISTRICT_CODE").attr("disabled", false);
            $("#Mast_State_Code").attr("disabled", false);
            $.ajax({
                url: "/Master/EditBankDetails",
                type: "POST",
                data: $("#frmBankDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tblBankDetailsList').trigger('reloadGrid');
                        $("#dvContractorBankDetails").show('slow');
                        $("#dvContractorBankDetails").load('/Master/AddBankDetails/' + $("#MAST_CON_ID").val() + "$" + $("#RegStateCode").val());
                        $("#dvErrorMessage").hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#MAST_DISTRICT_CODE").attr("disabled", true);
                            $("#Mast_State_Code").attr("disabled", true);
                            $('#errmessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvContractorBankDetails").html(data);
                        $("#MAST_DISTRICT_CODE").attr("disabled", true);
                        $("#Mast_State_Code").attr("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#MAST_DISTRICT_CODE").attr("disabled", true);
                    $("#Mast_State_Code").attr("disabled", true);
                    //alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    $.unblockUI();
                }
            });
        }
    }
    );
    $("#Mast_State_Code").change(function () {
        loadDistrictDropDopwnList($("#Mast_State_Code").val());
    });

    $('#ddlBankName').change(function () {
        //$("#ddlIfscCode").empty();
        //$.ajax({
        //    url: '/Master/PopulateIfscByBankName',
        //    type: 'GET',
        //    beforeSend: function () {
        //        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        //    },
        //    data: { bankName: $("#ddlBankName option:selected").text(), },
        //    success: function (jsonData) {
        //        for (var i = 0; i < jsonData.length; i++) {
        //            if (jsonData[i].Value == 2) {
        //                $("#ddlIfscCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
        //            }
        //            else {
        //                $("#ddlIfscCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
        //            }
        //        }
        //        $.unblockUI();
        //    },
        //    error: function (err) {
        //        $.unblockUI();
        //    }
        //});
        LoadIfscList();
    });
});


function confirmAlreadyExists() {

    $.ajax({
        url: "/Master/AddBankDetails",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetails").serialize(),
        success: function (data) {
            if (data.success) {
                alert(data.message);
                $('#tblBankDetailsList').trigger('reloadGrid');
                $("#btnResetBankDetails").trigger('click');
            }
            else {
                if (data.message != "") {
                    $('#errmessage').html(data.message);
                    $('#dvErrorMessage').show('slow');
                }
            }
        }
    });
}


function addBankDetails() {

    $.ajax({
        url: "/Master/AddBankDetails",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmBankDetails").serialize(),
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $('#tblBankDetailsList').trigger('reloadGrid');
                $("#btnResetBankDetails").trigger('click');

            }
            else {
                if (data.message != "") {
                    $('#errmessage').html(data.message);
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
                    //alert(xhr.status);
                    //alert(thrownError);
                    alert('An Error occurred while processing your request');
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

function LoadBankList() {

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
                //  alert(xhr.responseText);
                alert('An Error occurred while processing your request');
                window.location.href = "/Login/LogIn";
            }
        }
    })
}

function LoadIfscList() {
    if ($("#ddlBankName option:selected").val() != "") {
        $.ajax({
            url: "/Master/PopulateIfscByBankName",
            cache: false,
            type: "GET",
            async: false,
            data: { bankName: $("#ddlBankName option:selected").text(), },
            success: function (data) {

                var rows = new Array();
                for (var i = 0; i < data.length; i++) {
                    rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
                }

                $('#MAST_IFSC_CODE').autocomplete({
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
                    //  alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    window.location.href = "/Login/LogIn";
                }
            }
        })
    }
}