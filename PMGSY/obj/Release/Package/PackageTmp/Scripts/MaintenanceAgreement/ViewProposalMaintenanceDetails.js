$(document).ready(function () {

    LoadMaintenanceAgreementDetails();
    LoadMaintenanceFinancialDetails();

});
function LoadMaintenanceAgreementDetails() {
    $("#tbListMaintenanceDetails").jqGrid('GridUnload');
    jQuery("#tbListMaintenanceDetails").jqGrid({
        url: '/MaintenanceAgreement/GetAgreementDetailsList_Proposal',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $('#ProposalCode').val() },
        colNames: ['Agreement Number', 'Work', 'Contractor Name',  'Agreement Date', 'Maintenance Start Date', 'Maintenance Amount','Agreement Status'],
        colModel: [
                           { name: 'AgreementNumber', index: 'AgreementNumber', width: 200, sortable: true },
                           { name: 'Work', index: 'Work', height: 'auto', width: 120, sortable: false },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 200, sortable: true, },
                           { name: 'AgreementType', index: 'AgreementType', width: 100, sortable: false, align: "left" },
                           { name: 'AgreementDate', index: 'AgreementDate', width: 100, sortable: false},
                           { name: 'MaintenanceDate', index: 'MaintenanceDate', height: 'auto', width: 120, sortable: false, align: "left" },
                           { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 100, sortable: false, align: "center" },
                           //{ name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 90, sortable: false, align: "left" },

        ],
        pager: jQuery('#pgMaintenanceDetails'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Maintenance Agreement Details List",
        height: 'auto',
        //width: 1130,
        //  autowidth: true,
        rownumbers: true,
        sortname: 'AgreementDate',
        sortorder: "desc",
        hidegrid: false,
        loadComplete: function () {

            var reccount = $('#tbListMaintenanceDetails').getGridParam('reccount');
            if (reccount > 0) {
                $('#pgMaintenanceDetails_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs.]'); //2.All Lengths are in Kms. 
            }
            else {
                $("#dvMaintenanceDetails").html('<center><b>Maintenance Agreement details are not present against this proposal.</b></center>');
                $("#dvFinanceDetails").html('');
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }


    }); //end of grid
}
function LoadMaintenanceFinancialDetails()
{
    $("#tbListFinanceDetails").jqGrid('GridUnload');
    jQuery("#tbListFinanceDetails").jqGrid({
        url: '/MaintenanceAgreement/GetProposalFinancialList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $("#ProposalCode").val()},
        colNames: ['Year', 'Month', 'Upto Last Month', 'During This Month', 'Total', 'Upto Last Month', 'During This Month', 'Total', 'Is Final Payment Made', 'Date'],
        colModel: [
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 55, align: "left", search: false },
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 65, align: "center", search: false },
                            { name: 'EXEC_VALUEOFWORK_LASTMONTH', index: 'EXEC_VALUEOFWORK_LASTMONTH', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_VALUEOFWORK_THISMONTH', index: 'EXEC_VALUEOFWORK_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL', index: 'TOTAL', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_LASTMONTH', index: 'EXEC_PAYMENT_LASTMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_THISMONTH', index: 'EXEC_PAYMENT_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_PAYMENT', index: 'TOTAL_PAYMENT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_FLAG', index: 'EXEC_FINAL_PAYMENT_FLAG', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_DATE', index: 'EXEC_FINAL_PAYMENT_DATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 55, align: "center", search: false },
                            //{ name: 'b', width: 55, align: "center", search: false },

        ],
        pager: jQuery('#pgFinanceDetails'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "EXEC_PROG_YEAR,EXEC_PROG_MONTH",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Financial Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

           
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

    jQuery("#tbListFinanceDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: '<center>Value of Work Done(Rs. in Lakh)</center>' },
          { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: '<center>Payment Made(Rs. in Lakh)</center>' }
        ]
    });
}
