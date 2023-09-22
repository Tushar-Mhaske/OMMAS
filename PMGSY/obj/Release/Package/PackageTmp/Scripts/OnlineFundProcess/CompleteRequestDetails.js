$(document).ready(function () {

    
    $("#tabs").tabs();
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    LoadRequestDetails();
    LoadReleaseDetails();

});
function LoadProposalDetailsList() {
    blockPage();
    jQuery("#tbProposalList").jqGrid({
        url: '/OnlineFund/GetProposalList',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Proposal Type", "Package Number", "Year", "Road Name", "Pavement Length (in Kms.) / Bridge Name (in Mtrs.)", "Sanction Cost (in Lakhs)", "Maintenance Cost (in Lakhs)"],
        colModel: [
                    { name: 'District', index: 'District', width: 100, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                    { name: 'Type', index: 'Type', width: 100, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 100, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                    { name: 'PavementLength', index: 'PavementLength', width: 100, sortable: false, align: "right" },
                    { name: 'SanctionCost', index: 'SanctionCost', width: 100, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 100, sortable: false, align: "right" },
        ],
        postData: { "EncryptedId": $('#RequestId').val() },
        pager: jQuery('#pgProposalList'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road / Bridge Proposals",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: false,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            unblockPage();
            $("#tbProposalList").jqGrid('setGridWidth', $("#tbProposalList").width() - 100, true);
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        },
        beforeSelectRow: function (rowid, e) {
        }
    });
    unblockPage();
}
function LoadDocumentDetails() {
    $("#tbUploadList").jqGrid('GridUnload');

    jQuery("#tbUploadList").jqGrid({
        url: '/OnlineFund/GetListofDocumentsUploaded',
        datatype: "json",
        mtype: "POST",
        postData: { EncryptedRequestId: $("#RequestId").val() },
        colNames: ['Document Name', 'File Name', 'Upload Date', 'Remarks', 'Download'],
        colModel: [

                            { name: 'DocumentName', index: 'DocumentName', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'FileName', index: 'FileName', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'UploadDate', index: 'UploadDate', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'Download', index: 'Download', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgUploadList'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DocumentName',
        sortorder: "desc",
        caption: 'Document List',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
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
}
function Download(id) {
    window.location = '/OnlineFund/DownloadFile/' + id;
}
function LoadRequestDetails()
{
    $('#dvRequestDetails').load('/OnlineFund/ViewFundRequest/' + $("#RequestId").val());
}
function LoadReleaseDetails() {
    $('#dvReleaseDetails').load('/OnlineFund/AddUODetails/' + $("#RequestId").val());
}
function LoadObservationDetails() {
    $("#tbObservationList").jqGrid('GridUnload');

    jQuery("#tbObservationList").jqGrid({
        url: '/OnlineFund/GetListofObservationDetails',
        datatype: "json",
        mtype: "POST",
        postData: { RequestId: $("#RequestId").val() },
        colNames: ['Request Forwarded From', 'Request Forwarded To', 'File Number', 'Approval Date', 'Remarks', 'Approve / Approve & Forward', 'Reject', 'Forward', 'Reject Letter Name', 'Download'],
        colModel: [

                            { name: 'User', index: 'User', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'UserTo', index: 'UserTo', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'FileNo', index: 'FileNo', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ApprovalDate', index: 'ApprovalDate', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'Approve', index: 'Approve', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Reject', index: 'Reject', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Forward', index: 'Forward', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'LetterName', index: 'LetterName', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Download', index: 'Download', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgObservationList'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DocumentName',
        sortorder: "desc",
        caption: 'Observation List',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
            $("#tbObservationList").jqGrid('setGridWidth', $("#tbObservationList").width() - 100, true);
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
}