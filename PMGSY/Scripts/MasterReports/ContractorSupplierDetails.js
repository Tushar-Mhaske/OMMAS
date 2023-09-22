
$(document).ready(function () {
 
    $('#MAST_CON_SUP_FLAG option[value="C"]').attr("selected", true);
    $('#MAST_CON_STATUS option[value="A"]').attr("selected", true);
    $("#ContractorSupplierDetailsButton").click(function () {
        var contractorSupplierFlag = $("#MAST_CON_SUP_FLAG").val();
        var contractStatus = $("#MAST_CON_STATUS").val();
        var stateCode = $("#ContractorSupState").val();
        ContractorSupplierReportsListing(contractorSupplierFlag, contractStatus,stateCode);
    });
    $("#ContractorSupplierDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    
});




function ContractorSupplierReportsListing(contractorSupplierFlag, contractStatus,stateCode) {
    $("#ContractorSupplierDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ContractorSupplierDetailsTable").jqGrid({
        url: '/MasterReports/ContractorSupplierDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Contractor/Supplier','Company Name','Contact Person Name','Address','Contact','PAN','Legal Heir Name',
            'Expiry Date','Remarks','Status'],
        colModel:[
            {name: "MAST_CON_SUP_FLAG", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_CON_COMPANY_NAME", width: 150, align: 'left', height: 'auto'},
            { name: "MAST_CON_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_ADDR", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_CONTACT", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_PAN", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_LEGAL_HEIR_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_EXPIRY_DATE", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_REMARKS", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_STATUS", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "MAST_CON_SUP_FLAG": contractorSupplierFlag, "MAST_CON_STATUS": contractStatus,"STATE":stateCode},
        pager: $("#ContractorSupplierDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_CON_COMPANY_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Contractor Supplier Details',
        loadComplete: function () {
            $('#ContractorSupplierDetailsTable_rn').html('Sr.<br/>No.');

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