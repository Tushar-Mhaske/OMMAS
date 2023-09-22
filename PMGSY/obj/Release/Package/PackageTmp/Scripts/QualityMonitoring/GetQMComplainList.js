$(document).ready(function () {

    $("#btnView").click(function () {
        ListQMJointIspectionDetails();
    });

    $("#divCreateFormLoad").click(function () {
        //alert("Create Link Click");
        LoadDiv("/QualityMonitoring/QMComplainCreate");
    });

    ListQMJointIspectionDetails();

    $(function () {
        $("#dvQMComplainUploadFileAccordion").accordion({
            icons: true,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function ListQMJointIspectionDetails() {

    blockPage();
    $('#tbQMComplainList').jqGrid('GridUnload');
    jQuery("#tbQMComplainList").jqGrid({
        url: '/QualityMonitoring/GetQMComplainList',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Complainant", "Received Through", "Forwarded To", "Nature of Complaint", "Complaint Received at NRRDA", "Stage", "Upload (PDF)", "Detail View", "Delete"],
        colModel: [
                    { name: 'State', index: 'State', width: 100, sortable: false, align: "left" },
                    { name: 'Complainant', index: 'Complainant', width: 70, sortable: false, align: "center" },
                    { name: 'RecieveThrough', index: 'RecieveThrough', width: 70, sortable: false, align: "center" },
                    { name: 'ForwardedTo', index: 'ForwardedTo', width: 70, sortable: false, align: "center" },
                    { name: 'NatureComplaint', index: 'NatureComplaint', width: 150, sortable: false, align: "left" },
                    { name: 'ComplaintDate', index: 'ComplaintDate', width: 70, sortable: false, align: "center" },
                    { name: 'Status', index: 'Status', width: 120, sortable: false, align: "left" },
                    { name: 'Upload', index: 'Upload', width: 70, sortable: false, align: "center" },
                    { name: 'Detail', index: 'Detail', width: 70, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 70, sortable: false, align: "center" }

        ],
        postData: $("#frmQMComplainFilter").serialize(),
        pager: jQuery('#dvtbQMComplainListPager'),
        //   rowList: [100, 500, 2000],
        rowNum: 5000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Complaint List",
        height: '350',
        width: '1200',
        sortname: 'State',
        rownumbers: true,
        footerrow: false,
        loadComplete: function (data) {


        },
        loadError: function (xhr, status, error) {
            unblockPage();
        }
    }); //end of grid   
    unblockPage();

}


function UploadQMComplainDetail(ComplainId) {

    alert(ComplainId);
}



function ShowQMComplainDetail(ComplainId) {

    //alert(ComplainId);

    LoadDiv("/QualityMonitoring/QMComplainDetail/" + ComplainId);
}
/*
function CloseQMComplain() {
    LoadDiv("/QualityMonitoring/GetQMComplainList");
}
*/




function DownloadQMComplainDocument(ComplainFileId) {

    var url = "/QualityMonitoring/QMComplainDownloadPdf/" + ComplainFileId;
    window.location.href = url;
    return false;


}



function DeleteQMComplain(ComplainId) {

    var retVal = confirm("Are you sure you want to Delete?");
    if (retVal == true) {
        var url = "/QualityMonitoring/QMComplainDelete/" + ComplainId;
        $.post(url, function (responseData) {
            ListQMJointIspectionDetails();
        });
    }
}


function UploadQMComplainDocument(ComplainId) {


    //alert(JSON.stringify(url));
    // alert(url);
    $("#dvQMComplainUploadFile").load(url, function (responseData) {
        alert("Success");

    });
}
function UploadQMComplainDocument(ComplainId) {

    var url = "/QualityMonitoring/QMComplainUpload/" + ComplainId;
    // $("#dvQMComplainUploadFile").show();

    //

    $("#dvQMComplainUploadFileAccordion div").html("");
    $("#dvQMComplainUploadFileAccordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Upload Complaint Document</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseQMComplainUpload();" /></a>'
                    );

    $('#dvQMComplainUploadFileAccordion').show('fast', function () {


        // $("#divJIDetailsForm").load("/QualityMonitoring/FillMPVisitDetails/" + prRoadCode, function () {
        $("#dvQMComplainUploadFile").load(url, function () {
            //$.validator.unobtrusive.parse($('#divProposalForm'));
        });

        $("#dvQMComplainUploadFile").css('height', 'auto');
        $('#dvQMComplainUploadFile').show('slow');
    });

    //$("#tbMPVisitRoadList").jqGrid('setGridState', 'hidden');
}



function CloseQMComplainUpload() {
    $("#dvQMComplainUploadFileAccordion").hide('slow');
    $('#dvQMComplainUploadFile').hide('slow');
    ListQMJointIspectionDetails();
}