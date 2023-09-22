
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#MAST_CON_Reg_Bank_State").attr("disabled", "disabled");
    }
    $("#ContractorRegBankDetailsButton").click(function () {

        var stateCode = $("#MAST_CON_Reg_Bank_State").val();
        var contractRegFlag = $("#MAST_Contr_Reg_Flag").val();
        var contractStatus = $("#MAST_Contr_Reg_STATUS").val();
        if (stateCode > 0) {
            ContractorRegistrationBankReportsListing(stateCode, contractRegFlag, contractStatus);
        }
        else {
            alert("Please Select State");
        }
    });
    if ($("#MAST_CON_Reg_Bank_State").val() > 0) {
        $("#ContractorRegBankDetailsButton").trigger('click');
    }
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');

});

function ContractorRegistrationBankReportsListing(stateCode, contractRegFlag, contractStatus) {
    $("#ContractorRegistrationBankDetailsTable").jqGrid("GridUnload");
    $("#ContractorRegistrationBankDetailsTable").jqGrid({
        url: '/MasterReports/ContractorRegistrationBankDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Contract/Supplier', 'Company Name', 'Contractor Name', 'PAN', 'Contractor Status', 'Registration Number',
                    'Contractor Class', 'Contract Valid From', 'Contract Valid To', 'State Name', 'Registration Office', 'Registration Status', 'District Name',
                    'Account Number', 'Bank Name', 'IFSC Code', 'Account Status'],
        colModel: [
            { name: "MAST_CON_SUP_FLAG", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_CON_COMPANY_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_PAN", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_CON_STATUS", width: 70, align: 'left', height: 'auto' },
            { name: "MAST_CON_REG_NO", width: 100, align: 'left', height: 'auto',hidden:true },
            { name: "MAST_CON_CLASS", width: 70, align: 'left', height: 'auto', hidden: true },
            { name: "MAST_CON_VALID_FROM", width: 70, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' }, hidden: true },
            { name: "MAST_CON_VALID_TO", width: 70, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' }, hidden: true },
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto', hidden: true },
            { name: "MAST_REG_OFFICE", width: 150, align: 'left', height: 'auto', hidden: true },
            { name: "MAST_REG_STATUS", width: 70, align: 'left', height: 'auto', hidden: true },
            { name: "MAST_DISTRICT_NAME", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_ACCOUNT_NUMBER", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_BANK_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_IFSC_CODE", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_ACCOUNT_STATUS", width: 70, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "ContractRegFlag": contractRegFlag, "ContractStatus": contractStatus },
        pager: $("#ContractorRegistrationBankDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_CON_SUP_FLAG',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        recordtext: '{2} records found',
        autowidth: false,
        shrinkToFit: false,
        width: 1120,
        height: '580',
        viewrecords: true,
        caption: 'Contractor Bank Report',
        loadComplete: function () {
            $('#ContractorRegistrationBankDetailsTable_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}