$(document).ready(function () {

    $("#tabs").tabs();
    
    
    $('#tblBankDetailsListView').jqGrid('GridUnload');
    LoadBankDetails();
    LoadRegistrationDetails();
    $("#tabs-1").load('/Master/ViewContractorDetails?id=' + $('#EncryptedContractor').val(), function ()
    {
        $.validator.unobtrusive.parse('#tabs-1');
    });

});
function LoadBankDetails() {
    $('#tblBankDetailsListView').jqGrid({
        url: '/Master/GetContractorBankDetailsView/',
        datatype: 'json',
        mtype: "POST",
        postData: { ContractorCode: $('#EncryptedContractor').val() },
        colNames: ['Contractor Name', 'District Name','State Name', 'Account Number', 'Bank Name', 'IFSC Code', 'Status', 'Action'],
        colModel: [
        { name: 'ContName', index: 'ContName', height: 'auto', width: 140, align: "left", sortable: true },
        { name: 'District', index: 'District', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'State', index: 'State', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'AccNumber', index: 'AccNumber', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'BankName', index: 'BankName', height: 'auto', width: 160, align: "left", sortable: true },
        { name: 'IfscCode', index: 'IfscCode', height: 'auto', width: 120, align: "left", sortable: true },
        { name: 'AccStatus', index: 'AccStatus', height: 'auto', width: 80, align: "center", sortable: true },
        { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false,hidden:true }
        ],
        pager: jQuery('#dvPagerBankDetailsView'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BankName',
        sortorder: "asc",
        caption: 'Bank List',
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again.");
            }
        }

    });

}

function LoadRegistrationDetails() {
    $('#tblContrRegDetailsListView').jqGrid({
        url: '/Master/GetViewContractorRegistrationDetails/',
        datatype: 'json',
        mtype: "POST",
        postData: { ConId: $('#MAST_CON_ID').val(), RegId: $('#MAST_REG_CODE').val() },
        colNames: ['Contractor/Supplier Name','Registration Number', 'Office Name','Valid From', 'Valid To','Class','Status'],
        colModel: [
        { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 170, align: "left", sortable: true },
        { name: 'MAST_CON_REG_NO', index: 'MAST_CON_REG_NO', height: 'auto', width: 140, align: "right", sortable: true },
        { name: 'MAST_REG_OFFICE', index: 'MAST_REG_OFFICE', height: 'auto', width: 180, align: "left", sortable: true },
        { name: 'MAST_CON_VALID_FROM', index: 'MAST_CON_VALID_FROM', height: 'auto', width: 100, align: "center", sortable: true },
        { name: 'MAST_CON_VALID_TO', index: 'MAST_CON_VALID_TO', height: 'auto', width: 100, align: "center", sortable: true },
        { name: 'MAST_CON_CLASS', index: 'MAST_CON_CLASS', height: 'auto', width: 120, align: "center", sortable: true,hidden:true },
        { name: 'MAST_REG_STATUS', index: 'MAST_REG_STATUS', height: 'auto', width: 100, align: "center", sortable: true }
           ],
        pager: jQuery('#dvPagerContrRegDetailsView'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        caption: 'Contractor Registration Detail',
        height: 'auto',
       // width:'100%',
        autowidth: true,
        shrinkToFit:true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again.");
            }
        }

    });

}


//Start Tab 4 Contractor Agreement Information
function LoadRegistrationAgreementDetailGrid() {
    $("#tbRegistrationAgreementDetailReport").jqGrid('GridUnload');
    jQuery("#tbRegistrationAgreementDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchAgreementList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": $('#EncryptedContractorCode').val() },
        colNames: ['Contractor Name', 'Company Name', 'State', 'District', 'Agreement Number', 'Start Date', 'End Date', 'Amount', 'Maintenance Amount', 'Agreement Status', 'Is Finalize'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'TEND_AGREEMENT_NUMBER', index: 'TEND_AGREEMENT_NUMBER', height: 'auto', width: 120, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_START_DATE', index: 'TEND_AGREEMENT_START_DATE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_END_DATE', index: 'TEND_AGREEMENT_END_DATE', height: 'auto', width: 80, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_AMOUNT', index: 'TEND_AGREEMENT_AMOUNT', height: 'auto', width: 80, align: "right", sortable: false },
                            { name: 'MAINT_Amount', index: 'MAINT_Amount', height: 'auto', width: 70, align: "right", sortable: false },
                            { name: 'TEND_AGREEMENT_STATUS', index: 'TEND_AGREEMENT_STATUS', height: 'auto', width: 100, align: "center", sortable: false },
                            { name: 'TEND_IS_AGREEMENT_FINALIZED', index: 'TEND_IS_AGREEMENT_FINALIZED', height: 'auto', width: 70, align: "center", sortable: true },

        ],
        pager: jQuery('#dvRegistrationAgreementDetailReportPager'),
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Agreement Details List",
        height: 'auto',
        //autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            //$("#tbRegistrationAgreementDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 40, true);
            $('#tbRegistrationAgreementDetailReport_rn').html('Sr.<br/>No.');

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")

            }
        }

    });
    jQuery("#tbRegistrationAgreementDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 4 Contractor Agreement Information

//Start Tab 5 Contractor Maintenance Information
function LoadRegistrationMaintenanceDetailGrid() {
    $("#tbRegistrationMaintenanceDetailReport").jqGrid('GridUnload');
    jQuery("#tbRegistrationMaintenanceDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchIMSMaintenanceList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": $('#EncryptedContractorCode').val() },
        colNames: ['Contractor Name', 'Company Name', 'State', 'Agreement Number', 'Contractor Number', 'Maintenance Agreement Date', 'Maintenance Amount', 'Agreement Status', 'Is Finalize'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MANE_AGREEMENT_NUMBER', index: 'MANE_AGREEMENT_NUMBER', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MANE_CONTRACT_NUMBER', index: 'MANE_CONTRACT_NUMBER', height: 'auto', width: 120, align: "center", sortable: true },
                            { name: 'MANE_AGREEMENT_DATE', index: 'MANE_AGREEMENT_DATE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'MAINT_Amount', index: 'MAINT_Amount', height: 'auto', width: 100, align: "right", sortable: false },
                            { name: 'MANE_CONTRACT_STATUS', index: 'MANE_CONTRACT_STATUS', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'MANE_CONTRACT_FINALIZED', index: 'MANE_CONTRACT_FINALIZED', height: 'auto', width: 100, align: "center", sortable: true },

        ],
        pager: jQuery('#dvRegistrationMaintenanceDetailReportPager'),
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Maintenance Details List",
        height: 'auto',
        //autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            //$("#tbRegistrationMaintenanceDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 20, true);
            $('#tbRegistrationMaintenanceDetailReport_rn').html('Sr.<br/>No.');

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")

            }
        }

    });
    jQuery("#tbRegistrationMaintenanceDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 5 Contractor Maintenance Information

//Start Tab 6 Contractor Payment Information
//function LoadRegistrationPaymentDetailGrid(urlparameter) {
//    $("#tbRegistrationPaymentDetailReport").jqGrid('GridUnload');
//    jQuery("#tbRegistrationPaymentDetailReport").jqGrid({
//        url: '/Master/GetContractorByIdPanSearchPaymentList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { "ContractorCode": $('#EncryptedContractorCode').val() },
//        colNames: ['Contractor Name', 'Company Name', 'Bill Month', 'Bill Year', 'Bill Number', 'Bill Date', 'Gross Amount', 'Transaction Description', 'Agreement Status', 'Is Finalize'],
//        colModel: [
//                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
//                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
//                            { name: 'BILL_MONTH', index: 'BILL_MONTH', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
//                            { name: 'BILL_YEAR', index: 'BILL_YEAR', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
//                            { name: 'BILL_NO', index: 'BILL_NO', height: 'auto', width: 100, align: "center", sortable: true },
//                            { name: 'BILL_DATE', index: 'BILL_DATE', height: 'auto', width: 100, align: "center", sortable: true },
//                            { name: 'GROSS_AMOUNT', index: 'GROSS_AMOUNT', height: 'auto', width: 100, align: "right", sortable: false },
//                            { name: 'TXN_DESC', index: 'TXN_DESC', height: 'auto', width: 100, align: "center", sortable: false },
//                            { name: 'BILL_TYPE', index: 'BILL_TYPE', height: 'auto', width: 100, align: "center", sortable: true },
//                            { name: 'BILL_FINALIZED', index: 'BILL_FINALIZED', height: 'auto', width: 100, align: "center", sortable: true },

//        ],
//        pager: jQuery('#dvRegistrationPaymentDetailReportPager'),
//        sortname: 'CONTRACTOR_NAME',
//        sortorder: "asc",
//        rowNum: 5,
//        rowList: [5, 10, 15, 20],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Payment Details List",
//        height: 'auto',
//        //autowidth: true,
//        rownumbers: true,
//        hidegrid: false,
//        loadComplete: function () {
//            //$("#tbRegistrationPaymentDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 20, true);
//            $('#tbRegistrationPaymentDetailReport_rn').html('Sr.<br/>No.');

//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")

//            }
//        }

//    });
//    jQuery("#tbRegistrationPaymentDetailReport").jqGrid('setFrozenColumns');
//}
//End Tab 6 Contractor Payment Information