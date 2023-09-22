$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    $("#divAddBankDetails").hide();

    LoadBankDetailsList();

    $("#DivIcoBank").click(function () {

        $("#tblOBMaster").hide('slow');

    });

    //LoadAddView();
    $("#btnCreateNew").click(function () {
        LoadAddView();
        $("#btnCreateNew").hide('slow');
    });


    if ($("#DPIURoleCode").val() == 5) {
        LoadAddView();
        $("#divAddBankDetails").show();
    }

});
function LoadBankDetailsList() {

    jQuery("#tbBankDetailsList").jqGrid({
        url: '/Bank/GetBankDetailsList',
        datatype: "json",
        mtype: "POST",
        //postData: { stateCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        colNames: ['Bank Name', 'Branch Name', 'Account Type', 'Account Holder Name', 'Account No.', 'Ifsc Code', 'Agreement Date', 'Address', 'Phone No.', 'Account Opening Date', 'Account Closing Date', 'Status', /*'View',*/ 'Edit/View'],
        colModel: [
            { name: 'BANK_NAME', index: 'BANK_NAME', height: 'auto', width: 180, align: "left", search: false },
            { name: 'BANK_BRANCH', index: 'BANK_BRANCH', height: 'auto', width: 120, align: "left", search: false },
            { name: 'BANK_ACC_TYPE', index: 'BANK_ACC_TYPE', height: 'auto', width: 120, align: "left", search: false },
            { name: 'ACC_HOLDER_NAME', index: 'ACC_HOLDER_NAME', height: 'auto', width: 120, align: "left", search: false },
            { name: 'BANK_ACC_NO', index: 'BANK_ACC_NO', height: 'auto', width: 100, align: "center", search: true },
            { name: 'IFSCCode', index: 'IFSCCode', height: 'auto', width: 100, align: "center", search: true },
            { name: 'BANK_AGREEMENT_DATE', index: 'BANK_AGREEMENT_DATE', height: 'auto', width: 100, align: "left", search: false },
            { name: 'ADDRESS', index: 'ADDRESS', height: 'auto', width: 200, align: "left", search: false },
            { name: 'PHONE', index: 'PHONE', height: 'auto', width: 100, align: "center", search: false },
            { name: 'BANK_ACC_OPEN_DATE', index: 'BANK_ACC_OPEN_DATE', height: 'auto', width: 100, align: "left", search: false },
            { name: 'BANK_ACC_CLOSE_DATE', index: 'BANK_ACC_CLOSE_DATE', height: 'auto', width: 100, align: "left", search: false },
            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
            //{ name: 'View', width: 50, sortable: false, resize: false, align: "center", search: false },
            { name: 'Edit', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerBankList'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "BANK_ACC_OPEN_DATE",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Bank Details",
        height: 'auto',
        // autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            $('#tbBankDetailsList_rn').html('Sr.<br/>No.');
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}
function LoadAddView() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divAddBankDetails").show();

    $("#divAddBankDetails").load('/Bank/AddBankDetails/', function (data) {
        if (data.success == false) {
            alert('Error occurred while processing your request.');
        }
        $.validator.unobtrusive.parse($('#frmAddBankDetails'));
        $.unblockUI();
    });
}
function ShowDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#divAddBankDetails").is(':hidden')) {
        $("#divAddBankDetails").show();
    }

    $("#divAddBankDetails").load('/Bank/ShowBankDetails/' + urlparameter, function (data) {
        if (data.success == false) {
            alert('Error occurred while processing your request.');
        }
        $.validator.unobtrusive.parse($('#frmAddBankDetails'));

        $.unblockUI();
    });

}
function EditDetails(bankCode) {
    //alert("in edit, bankCode : " + bankCode);
    // Changed by Srishti
    //LoadAddView();
    LoadEditView(bankCode);
    $("#btnCreateNew").hide('slow');
}

//Added by Srishti
function LoadEditView(bankCode) {
    //alert("in load edit, bankCode : " + bankCode);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divAddBankDetails").show();

    $("#divAddBankDetails").load('/Bank/EditBankDetails/?idtemp=' + bankCode, function (data) {
        if (data.success == false) {
            alert('Error occurred while processing your request.');
        }
        $.validator.unobtrusive.parse($('#frmAddBankDetails'));
        $.unblockUI();
    });
}