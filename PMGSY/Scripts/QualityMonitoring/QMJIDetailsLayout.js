$(document).ready(function () {
    ListQMJIDetails();

    $('#btnAddQMJIDetails').click(function (e) {
        $('#spnQMJIHeader').text('Add Joint Inspection');
        $("#dvJointInspectionDetails").load('/QualityMonitoring/AddJIDetails/' + $("#hdnRoadCode").val());
        $('#dvhdQMJILayout').show('slow');
        $("#dvJointInspectionDetails").show('slow');
    });

    $('#btnCancelQMJIDetails').click(function () {

        $("#fillJIDetailsAccordion").hide('slow');
        $("#divQMJIFilter").show('slow');
        //ListQMJointIspectionDetails();
        $("#tbQMJointInspectionsList").trigger('reload');
    });

});


/**/
function ListQMJIDetails() {
    blockPage();
    $('#tbQMJIList').jqGrid('GridUnload');
    jQuery("#tbQMJIList").jqGrid({
        url: '/QualityMonitoring/ListJIDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "Block", "Work Type", "Inspection Date", "MP Name", "MLA Name", "GP Name", "Other Representative Name", "SE Name", "PIU Name", "AE Name", "District Officer Name", "Contractor Representative",
                    "Contractor Name", "Server Connectivity", "Work Progress Satisfactory", "CD Work Sufficient", "Variation Exec Length Reason", "Quality Grading", "Remarks", "View PDF", "Action Taken", "Details", "Edit", "Delete"],
        colModel: [
                    { name: 'stateName', index: 'stateName', width: 100, sortable: false, align: "left" },
                    { name: 'districtName', index: 'districtName', width: 70, sortable: false, align: "left" },
                    { name: 'blockName', index: 'blockName', width: 70, sortable: false, align: "center" },
                    { name: 'workType', index: 'workType', width: 70, sortable: false, align: "center" },
                    { name: 'inspectionDate', index: 'inspectionDate', width: 70, sortable: false, align: "center" },
                    { name: 'mpName', index: 'mpName', width: 70, sortable: false, align: "center" },
                    { name: 'mlaName', index: 'mlaName', width: 70, sortable: false, align: "center" },
                    { name: 'gpName', index: 'gpName', width: 70, sortable: false, align: "center" },
                    { name: 'otherRepresentativeName', index: 'otherRepresentativeName', width: 70, sortable: false, align: "center" },
                    { name: 'seName', index: 'seName', width: 70, sortable: false, align: "center" },
                    { name: 'piuName', index: 'piuName', width: 70, sortable: false, align: "center" },
                    { name: 'aeName', index: 'aeName', width: 70, sortable: false, align: "center" },
                    { name: 'districtOfficerName', index: 'districtOfficerName', width: 70, sortable: false, align: "center" },
                    { name: 'contractorRepresentative', index: 'contractorRepresentative', width: 70, sortable: false, align: "center" },
                    { name: 'contractorName', index: 'contractorName', width: 70, sortable: false, align: "center" },
                    { name: 'serveConnectivity', index: 'serveConnectivity', width: 70, sortable: false, align: "center" },
                    { name: 'workProgressSatisfactory', index: 'workProgressSatisfactory', width: 70, sortable: false, align: "center" },
                    { name: 'cdWorkSufficient', index: 'cdWorkSufficient', width: 70, sortable: false, align: "center" },
                    { name: 'variationExecLengthReason', index: 'variationExecLengthReason', width: 70, sortable: false, align: "center" },
                    { name: 'qualityGrading', index: 'qualityGrading', width: 70, sortable: false, align: "center" },
                    { name: 'remarks', index: 'remarks', width: 70, sortable: false, align: "center" },
                    { name: 'FileName', index: 'FileName', width: 70, sortable: false, align: "center" },
                    { name: 'FALLOWUP', index: 'FALLOWUP', width: 70, sortable: false, align: "center" },
                    { name: 'details', index: 'details', width: 70, sortable: false, align: "center" },
                    { name: 'Edite', index: 'Edite', width: 70, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 70, sortable: false, align: "center" }
        ],
        postData: { "prRoadCode": $("#hdnRoadCode").val() },
        pager: jQuery('#dvQMJIPager'),
        //   rowList: [100, 500, 2000],
        rowNum: 5000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Joint Inspection Details List",
        height: 'auto',
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

function EditJIDetail(jicode) {
    //QMJIAccordionDetails("/QualityMonitoring/EditJIDetails/" + jicode);
    $('#spnQMJIHeader').text('Edit Joint Inspection');
    $("#dvJointInspectionDetails").load('/QualityMonitoring/EditJIDetails/' + jicode);
    $('#dvhdQMJILayout').show('slow');
    $("#dvJointInspectionDetails").show('slow');
}

function DeleteJIDetail(jicode) {
    $.post("/QualityMonitoring/QMJIDelete/" + jicode, function (data) {
        alert(data.message);
        ListQMJIDetails();
    });
}

function ShowJIDetail(jicode) {
    //QMJIAccordionDetails("/QualityMonitoring/GetQMJIDetail/" + jicode);
    $('#spnQMJIHeader').text('View Joint Inspection Details');
    $("#dvJointInspectionDetails").load('/QualityMonitoring/GetQMJIDetail/' + jicode);
    $('#dvhdQMJILayout').show('slow');
    $("#dvJointInspectionDetails").show('slow');
}

function ATRJIInspection(jicode) {
    //QMJIAccordionDetails("/QualityMonitoring/QMJIATRLayout/" + jicode);
    $('#spnQMJIHeader').text('Action Taken for Joint Inspection');
    $("#dvJointInspectionDetails").load('/QualityMonitoring/QMJIATRLayout/' + jicode);
    $('#dvhdQMJILayout').show('slow');
    $("#dvJointInspectionDetails").show('slow');
}

function CloseJIDetailsLayout1() {
    $('#dvhdQMJILayout').hide('slow');
    $("#dvJointInspectionDetails").hide('slow');
    $("#dvJointInspectionDetails").html('');
    //ListQMJIDetails();
    //$("#tbQMJointInspectionsList").trigger('reload');
    $("#tbQMJIList").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

}