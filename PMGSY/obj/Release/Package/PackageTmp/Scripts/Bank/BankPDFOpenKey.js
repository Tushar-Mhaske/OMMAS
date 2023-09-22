$(document).ready(function () {

    $.validator.unobtrusive.parse($("#frmGenerateBankPdfKeyDetails"));

    ListBankDetailsList();

    $("#btnViewDetails").click(function () {
        if ($("#frmGenerateBankPdfKeyDetails").valid()) {
            ListBankDetailsList();
        }
    });

    $("#btnSaveKeyDetails").click(function () {
        $.ajax({
            url: '/Bank/SaveBankPDFOpenKey/',
            async: false,
            catche: false,
            method: 'POST',
            data: $("#frmGenerateKey").serialize(),
            success: function (response) {
                if (response.success == true) {
                    alert(response.message);
                    $("#hdnGeneratedKey").val('');
                    $("#dvDialogGenerateKey").dialog("close");
                    $("#tblBankDetails").trigger("reloadGrid");
                }
                else if (response.success == false) {
                    alert(response.message);
                    return false;
                }
            },
            error: function (xhr, status, code) {
                alert("An error occured while processing your request.");
                return false;
            }
        });
    });
});
function ListBankDetailsList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tblBankDetails").jqGrid("GridUnload");

    jQuery("#tblBankDetails").jqGrid({
        url: '/Bank/DisplayBankDetailList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 0,
        rownumbers: true,
        postData: { AgencyCode: $("#ddlAgency").val(), FundType: $("#ddlFundType").val() },
        autowidth: true,
        //width: 900,
        //shrinkToFit: true,
        pginput: false,
        pgbuttons: false,
        loadComplete: function () {

            if ($("#ddlAgency option:selected").text() != "--All--") {
                $("#spnSRRDAName").html($("#ddlAgency option:selected").text());
            }

            $.unblockUI();
            var recordCount = jQuery("#tblBankDetails").jqGrid('getGridParam', 'reccount');
            if (recordCount > 25) {
                $("#tblBankDetails").jqGrid('setGridHeight', '230');
            } else {
                $("#tblBankDetails").jqGrid('setGridHeight', 'auto');
            }
            $('#tblBankDetails_rn').html('Sr.<br/>No.');
        },
        colNames: ['State Name', 'Agency Name', 'Bank Name', 'Branch Name', 'Account No.', 'Address', 'Phone', 'Email', 'Acc Open Date', 'Acc Close Date', 'Is Key Present', 'Generate Key'],
        colModel: [
            { name: 'STATE_NAME', index: 'STATE_NAME', width: 120, align: "left", sortable: true },
            { name: 'AGENCY_NAME', index: 'AGENCY_NAME', width: 120, align: "left", sortable: true },
            { name: 'BANK_NAME', index: 'BANK_NAME', width: 120, align: "left", sortable: true },
            { name: 'BANK_BRANCH', index: 'BANK_BRANCH', width: 120, align: "left", sortable: true },
            { name: 'BANK_ACC_NO', index: 'BANK_ACC_NO', width: 150, align: "left", sortable: true },
            { name: 'ADDRESS', index: 'ADDRESS', width: 120, align: "left", sortable: true },
            { name: 'PHONE', index: 'PHONE', width: 80, align: "Center", sortable: true },
            { name: 'BANK_EMAIL', index: 'BANK_EMAIL', width: 150, align: "left", sortable: true },
            { name: 'BANK_ACC_OPEN_DATE', index: 'BANK_ACC_OPEN_DATE', width: 80, align: "left", sortable: true },
            { name: 'BANK_ACC_CLOSE_DATE', index: 'BANK_ACC_CLOSE_DATE', width: 80, align: "left", hidden: true, sortable: true },
            { name: 'Bank_SEC_CODE', index: 'Bank_SEC_CODE', width: 60, align: "center", sortable: true },
            { name: 'GENERATE_KEY', index: 'GENERATE_KEY', width: 70, align: "left", sortable: true },
        ],
        pager: "#dvPagerBankDetails",
        viewrecords: true,
        sortname: 'STATE_NAME',
        sortorder: "asc",
        caption: "Bank Details",
        hidegrid: false
    });

}
function GeneratePDFOpenKey(param) {
    $(function () {
        $("#dvKeyLabel").hide();
        $("#dvGeneratedKey").html('');
        $("#hdnGeneratedKey").val('');

        $("#dvDialogGenerateKey").dialog({
            modal: true,
            //closeText:"Hide",
            //draggable:true,
            //autoOpen: false,
            //show: {
            //    effect:"blind",
            //    duration:1000,
            //},
            //hide: {
            //    effect: "explode",
            //    duration: 1000
            //}
        });

        $("#hdnBankCode").val(param);
        $("#StateAgencyName").val($("#ddlAgency option:selected").text());

        $(function () {
            GenerateKey();
        });


    });
}

function GenerateKey() {

    $.ajax({
        url: '/Bank/GenerateKey/',
        async: false,
        catche: false,
        method: 'POST',
        success: function (response) {
            if (response.success == true) {
                $("#dvKeyLabel").show();
                $("#dvGeneratedKey").html(response.key);
                $("#hdnGeneratedKey").val(response.key);
                $("#dvGeneratedKey").show();
            }
            else if (response.success == false) {
                alert(response.message);
                return false;
            }
        },
        error: function (xhr, status, code) {
            alert("An error occured while processing your request.");
            return false;
        }
    });

}


function BankEmailNotPresent() {
    alert("Please update bank email address.");
    return false;
}

function BankEmailNotValid() {
    alert("Bank Email address is invalid, Please update the bank email address.");
    return false;
}