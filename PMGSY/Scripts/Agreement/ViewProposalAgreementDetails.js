$(document).ready(function () {

    LoadAgreementDetails();
    //LoadMaintenanceDetails();

});
function LoadAgreementDetails()
{
    $("#tbListAggDetails").jqGrid('GridUnload');
    jQuery("#tbListAggDetails").jqGrid({
        url: '/Agreement/GetProposalAgreementList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $('#ProposalCode').val(), AgreementType: $('#ProposalType').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Work','Contractor Name', 'Type', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status'/*'Change Status','Finalize', 'View', 'Edit', 'Delete'*/],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: 0, align: "left", sortable: false, hidden: true },
                          //  { name: 'RoadName', index: 'RoadName', height: 'auto', width: 170, align: "left", sortable: false },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: 150, sortable: true, resizable: false },
                            { name: 'WorkDesc', index: 'WorkDesc', width: 100, sortable: true, resizable: false,align:'center' },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 200, sortable: true, resizable: false },
                             { name: 'AgreementType', index: 'AgreementType', width: 120, sortable: true, align: "left", resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: 120, sortable: true, resizable: false },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 120, sortable: false, align: "right", resizable: false },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 50, sortable: false, align: "right", resizable: false},
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 150, sortable: false, align: "left", resizable: false },
        ],
        pager: jQuery('#pgAggDetails'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Agreement Details List",
        height: 'auto',
        //width: '1135',
        autowidth: false,
        rownumbers: true,
        hidegrid: false,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        loadComplete: function () {

            var reccount = $('#tbListAggDetails').getGridParam('reccount');
            if (reccount > 0) {
                $('#pgAggDetails_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms. 3."NA"-Not Available  ]');
            }
            else {
                $("#dvAgreementDetails").html('<center><b>Agreement Details are not present against this proposal.</b></center>');
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
        },
        onSelectRow: function (id) {
            $('#TendAgreementCode').val(id);
        }
    }); //end of grid
}
//function LoadMaintenanceDetails()
//{
//    jQuery("#tbListMaintenanceDetails").jqGrid({
//        url: '/MaintenanceAgreement/GetAgreementDetailsList_Proposal',
//        datatype: "json",
//        mtype: "POST",
//        postData: { ProposalCode: $('#ProposalCode').val() },
//        colNames: ['Agreement Number', 'Work', 'Contractor Name', 'Agreement Type', 'Agreement Date', 'Maintenance Start Date', 'AgreementStatus'],
//        colModel: [
//                           { name: 'AgreementNumber', index: 'AgreementNumber', width: 200, sortable: true },
//                           { name: 'Work', index: 'Work', height: 'auto', width: 80, sortable: true },
//                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 200, sortable: true, },
//                           { name: 'AgreementType', index: 'AgreementType', width: 100, sortable: true, align: "left" },
//                           { name: 'AgreementDate', index: 'AgreementDate', width: 90, sortable: true },
//                           { name: 'MaintenanceDate', index: 'MaintenanceDate', height: 'auto', width: 90, sortable: true, align: "left" },
//                           { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 90, sortable: false, align: "center" },
//                           //{ name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 90, sortable: false, align: "left" },
                           
//        ],
//        pager: jQuery('#pgMaintenanceDetails'),
//        rowNum: 5,
//        rowList: [5, 10],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Maintenance Agreement Details List",
//        height: 'auto',
//        //width: 1130,
//        //  autowidth: true,
//        rownumbers: true,
//        sortname: 'AgreementDate',
//        sortorder: "desc",
//        hidegrid: false,
//        loadComplete: function () {

//            var reccount = $('#tbListMaintenanceDetails').getGridParam('reccount');
//            if (reccount > 0) {
//                $('#pgMaintenanceDetails_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs.]'); //2.All Lengths are in Kms. 
//            }
//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                // alert(xhr.responseText);
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }


//    }); //end of grid
//}
