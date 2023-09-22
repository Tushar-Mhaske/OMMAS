q$(document).ready(function () {

    $("#spCollapseIconQMJILayout").click(function () {
        $("#spCollapseIconQMJILayout").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMJIFilterForm").toggle("slow");
    });

    $("#btnViewWorks").click(function () {
        ListQMJointIspectionDetails();
    });

    $(function () {
        $("#fillJIDetailsAccordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});




function ListQMJointIspectionDetails() {
    blockPage();
    $('#tbQMJointInspectionsList').jqGrid('GridUnload');
    jQuery("#tbQMJointInspectionsList").jqGrid({
        url: '/QualityMonitoring/QMJIList',
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Sanction Year", "Work Name", "Progress Status", "Proposal Type", "Download Format", /*"Overall Quality Grading", "Inspection Date", "View PDF", "Action Taken", "Details",*/ "Add", /*"Edit", "Delete"*/],
        colModel: [
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, sortable: false, align: "left" },
                    { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 70, sortable: false, align: "left" },
                    { name: 'SANCTION_YEAR', index: 'SANCTION_YEAR', width: 70, sortable: false, align: "center" },
                    { name: 'WORK_NAME', index: 'WORK_NAME', width: 200, sortable: false, align: "left" },
                    { name: 'PROG_STATUS', index: 'PROG_STATUS', width: 100, sortable: false, align: "center" },
                    { name: 'PROPOSAL_TYPE', index: 'PROPOSAL_TYPE', width: 70, sortable: false, align: "center" },
                    { name: 'Download', index: 'Download', width: 70, sortable: false, align: "center" },
                    //{ name: 'QM_JNT_INSP_OVERALL', index: 'QM_JNT_INSP_OVERALL', width: 100, sortable: false, align: "center" },
                    //{ name: 'IMS_JNT_INSP_DATE', index: 'IMS_JNT_INSP_DATE', width: 100, sortable: false, align: "center" },
                    //{ name: 'FileName', index: 'FileName', width: 70, sortable: false, align: "center", hidden:true },
                    //{ name: 'FALLOWUP', index: 'FALLOWUP', width: 70, sortable: false, align: "center", hidden: true },
                    //{ name: 'details', index: 'details', width: 70, sortable: false, align: "center", hidden: true },
                    { name: 'add', index: 'add', width: 70, sortable: false, align: "center" },
                    //{ name: 'Edite', index: 'Edite', width: 70, sortable: false, align: "center", hidden: true },
                    //{ name: 'Delete', index: 'Delete', width: 70, sortable: false, align: "center", hidden: true }
        ],
        postData: { "block": $("#BlockCode").val(), "ptype": $("#ProposalType").val(), "inspstatus": $("#InspectionStatus").val() },
        pager: jQuery('#dvQMJointInspectionsPager'),
        //   rowList: [100, 500, 2000],
        rowNum: 5000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Joint Inspection Details",
        height: '350',
        //width: '1200',
        autowidth: true,
        sortname: 'Block',
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


function AddJIInspection(workId) {

    //QMJIAccordionDetails("/QualityMonitoring/AddJIDetails/" + workId);
    QMJIAccordionDetails("/QualityMonitoring/QMJIDetailsLayout/" + workId);
}
//function DeleteJIDetail(jicode) {

//    $.post("/QualityMonitoring/QMJIDelete/" + jicode, function (data) {
//        alert(data.message);
//        ListQMJointIspectionDetails();
//    });

//}
//function EditJIDetail(jicode) {
//    QMJIAccordionDetails("/QualityMonitoring/EditJIDetails/" + jicode);

//}
//function ShowJIDetail(jicode) {


//    QMJIAccordionDetails("/QualityMonitoring/GetQMJIDetail/" + jicode);
//}

//function ATRJIInspection(jicode) {


//    QMJIAccordionDetails("/QualityMonitoring/QMJIATRLayout/" + jicode);
//}


function QMJIAccordionDetails(url) {
    // alert(url);
    // $('#divList').show();
    //
    $("#divJIDetailsForm").show();

    //

    $("#fillJIDetailsAccordion div").html("");
    $("#fillJIDetailsAccordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add Joint Inspection Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseJIDetails();" /></a>'
                    );

    $('#fillJIDetailsAccordion').show('fast', function () {


        // $("#divJIDetailsForm").load("/QualityMonitoring/FillMPVisitDetails/" + prRoadCode, function () {
        $("#divJIDetailsForm").load(url, function () {
            //$.validator.unobtrusive.parse($('#divProposalForm'));
        });

        $("#divJIDetailsForm").css('height', 'auto');
        $('#divJIDetailsForm').show('slow');
    });
    $("#divQMJIFilter").hide('slow');
    //$("#tbMPVisitRoadList").jqGrid('setGridState', 'hidden');
}



function CloseJIDetails() {

    $("#fillJIDetailsAccordion").hide('slow');
    $("#divQMJIFilter").show('slow');
    //ListQMJointIspectionDetails();
    //$("#tbQMJointInspectionsList").trigger('reload');
    $("#tbQMJointInspectionsList").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
}