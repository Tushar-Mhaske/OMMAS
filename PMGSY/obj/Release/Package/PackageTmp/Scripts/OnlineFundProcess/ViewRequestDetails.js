$(document).ready(function () {

    $("#requesttabs").tabs();
    LoadRequestDetails();

});
function LoadDocumentDetails() {
    $("#tbDocumentList").jqGrid('GridUnload');

    jQuery("#tbDocumentList").jqGrid({
        url: '/OnlineFund/GetListofDocumentsUploaded',
        datatype: "json",
        mtype: "POST",
        postData: { EncryptedRequestId: $("#EncryptedRequestId").val() },
        colNames: ['Document Name', 'File Name', 'Upload Date', 'Remarks', 'Download'],
        colModel: [

                            { name: 'DocumentName', index: 'DocumentName', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'FileName', index: 'FileName', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'UploadDate', index: 'UploadDate', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'Download', index: 'Download', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgDocumentList'),
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
function LoadObservationDetails() {
    $("#tbObservationList").jqGrid('GridUnload');

    jQuery("#tbObservationList").jqGrid({
        url: '/OnlineFund/GetListofObservationDetails',
        datatype: "json",
        mtype: "POST",
        postData: { RequestId: $("#EncryptedRequestId").val() },
        colNames: ['Request Forwarded From', 'Request Forwarded To', 'File Number', 'Approval Date', 'Remarks', 'Approve / Approve & Forward', 'Reject', 'Forward'],
        colModel: [

                            { name: 'User', index: 'User', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'UserTo', index: 'UserTo', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'FileNo', index: 'FileNo', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'ObservationDate', index: 'ObservationDate', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'Approve', index: 'Approve', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Reject', index: 'Reject', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Forward', index: 'Forward', height: 'auto', width: 100, align: "center", search: false },

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
function LoadRequestDetails()
{
    $('#dvRequestDetails').load('/OnlineFund/ViewObservationDetails/' + $("#EncryptedRequestId").val());
}
function LoadDocumentDetails()
{
    blockPage();
    $("#dvDocumentDetails").load('/OnlineFund/UploadDetails/' + $("#EncryptedRequestId").val(), function (response) {
        $.validator.unobtrusive.parse($('#dvRequestDetails'));
        unblockPage();
    });
}
function LoadObservationDetails()
{
    blockPage();
    $("#dvObservationDetails").load('/OnlineFund/AddObservationDetails/' + $("#EncryptedRequestId").val(), function (response) {
        $.validator.unobtrusive.parse($('#dvRequestDetails'));
        unblockPage();
    });
}