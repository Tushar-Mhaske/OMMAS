
var arrFinalizeAndForward = [];

$(document).ready(function () {

    $("#monitorCode3TierSQCInsp").chosen();
    $('#dropdownTechExpert').hide('fast');
    $(function () {
        $("#accordion3TierPIUInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#id3TierSQCInspFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");

    selectedNameVal = 0;
    $("#stateCode3TierSQCInsp").change(function () {

        $("#monitorCode3TierSQCInsp").empty();

        if ($(this).val() == 0) {
            $("#monitorCode3TierSQCInsp").append("<option value='0'>All Monitors</option>");
        }

        if ($("#stateCode3TierSQCInsp").val() > 0) {

            if ($("#monitorCode3TierSQCInsp").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCode3TierSQCInsp").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#monitorCode3TierSQCInsp").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //Change Hrishikesh for apply chosen
                        $("#monitorCode3TierSQCInsp").val('').trigger('chosen:updated');
                        $("#monitorCode3TierSQCInsp").chosen();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });

    $('#btn3TierSQCInspDetails').click(function () {
        InspectionListGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());
    });

    $('#btn3TierCQCAdminInspDetails').click(function () {
        InspectionListGridCQCAdmin($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());
    });

    $('#btnQMInspDetailsTEReviewList').click(function () {
        QMInspectionListTEReviewGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val());
    });

    if ($('#RoleCode').val() == 81) {
        QMInspectionListTEReviewGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), 1, 2000, $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), 0, "A", 0, 0);
    }

    $('#btnInspDetailsTEList').click(function () {
        QMInspectionListTEReviewGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), 1, 2000, $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), 0, "A", 0, 0);
    });

});

$("#id3TierSQCInspFilterDiv").click(function () {
    $("#div3TierSQCInspFilterForm").toggle("slow");
});

//Changed By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins" -- 22-06-2023
function InspectionListGridCQCAdmin(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatusType) {
    $("#tbInspList").jqGrid('GridUnload');

    jQuery("#tbInspList").jqGrid({
        url: '/QualityMonitoring/QMInspectionDetailsAllocateTechExpert?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        multiselect: true,
        colNames: ["Monitor", "State", "District", "Block", "Road/Bridge Name", "Package No", "Road/Bridge", "Scheme", "Work Status (As inspected)", "Sanctioned Length(km) / Bridge Length(m)", "From(km)", "To(km)", "Inspection Date",
            "Overall Grade", "View Report (pdf)", "View abstract/Images", "Images Uploaded", "Technical Expert Name", "Remove Technical Expert", "Finalize and forward to Technical Expert", "Forward to NQM", "View TE/QM Remark", "View E-Form", "View Test Report"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
            { name: 'SancLength', index: 'SancLength', width: 50, sortable: false, align: "center", search: false },
            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
            { name: 'viewreport', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
            { name: 'viewImages', index: 'UploadReport', width: 50, sortable: false, align: "center", search: false },
            { name: 'Images uploaded', index: 'View', width: 30, sortable: false, align: "center", search: false },  //
            { name: 'TEName', index: 'TEName', width: 100, sortable: false, align: "left", search: false },
            { name: 'RemoveTE', index: 'RemoveTE', width: 50, sortable: false, align: "left", search: false },
            { name: 'Finalize', index: 'Finalize', width: 50, sortable: false, align: "center", search: false },
            { name: 'ForwardToNQM', index: 'ForwardToNQM', width: 50, sortable: false, align: "center", search: false },
            { name: 'ViewRemark', index: 'ViewRemark', width: 30, sortable: false, align: "center", search: false },

            //added By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins" -- 22-06-2023
            { name: 'Eform', index: 'eform', width: 30, sortable: false, align: "center", search: false },
            { name: 'TestReport', index: 'TestReport', width: 30, sortable: false, align: "center", search: false },

        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType": "I", "schemeType": schemeType, "roadStatus": roadStatus, "roadOrBridge": roadOrBridge, "gradeType": gradeType, "eFormStatus": eFormStatusType },
        pager: jQuery('#dvInspListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        height: '300',
        autowidth: true,
        autoResizing: { adjustGridWidth: false },
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            var userdata = jQuery("#tbInspList").getGridParam('userData');
            var rocordCount = jQuery("#tbInspList").jqGrid('getGridParam', 'records');
            var count = 0;

            for (var i = 0; i < userdata.ids.length; i++) {

                $('.finalizeCheckBox').attr('checked', false);

                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tbInspList_" + userdata.ids[i]).attr("disabled", true);
                }
            }

            for (var i = 0; i < userdata.ids.length; i++) {
                if (($("#jqg_tbInspList_" + userdata.ids[i]).attr("disabled"))) {
                    count = count + 1;
                }
            }

            if (count == rocordCount) {
                $("#cb_tbInspList").attr('checked', true);
                $("#cb_tbInspList").attr('disabled', true);
            }

            if (count != rocordCount) {
                $('#dropdownTechExpert').show('slow');
            }
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
    jQuery("#tbInspList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: '<center>Inspection Chainage</center>' },
            { startColumnName: 'viewreport', numberOfColumns: 3, titleText: '<center>Report</center>' },
        ]
    });

}

$("#tbInspList").click(function (e) {

    var el = e.target; // DOM of the HTML element which was clicked

    if (el.nodeName == "INPUT") {
        // in case of the usage of the custom formatter we should go to the next
        // parent TD element
        el = $(el, this.rows).closest("td");
        var iCol = $(el).index();
        var row = $(el, this.rows).closest("tr.jqgrow");
        var rowId = row[0].id;

        // now you can do what you need. You have iCol additionally to rowId
        if ($(el, this.rows).closest("td").attr('aria-describedby') === "tbInspList_Finalize")//column for IsEnquiry
        {

            if ($(el, this.rows).closest("td").find('input[type=checkbox]').prop('checked')) {
                arrFinalizeAndForward.push(rowId);
            }
            else {
                arrFinalizeAndForward = jQuery.grep(arrFinalizeAndForward, function (value) {
                    return value != rowId;
                });
            }
        }

    }
});//Grid Click ends here

function ShowObservationDetails(obsId) {

    jQuery('#tbInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/QMObservationDetails3TierSQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');

    $("#tbInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


function ShowInspReportFile(obsId) {

    jQuery('#tbInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);
    $('#accordion3TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');

    $("#tbInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


function Close3TierPIUInspectionDetails() {
    $("#div3TierSQCInspFilterForm").toggle("slow");
    $('#accordion3TierPIUInspection').hide('slow');
    $('#div3TierPIUInspDetails').hide('slow');
    $("#tbInspList").jqGrid('setGridState', 'visible');
    $('#div3TierPIUInspectionDetails').show('slow');

    $('#dropdownTechExpert').show('slow');
}

function GetDataInArray() {
    var submitArray = [];

    submitArray.push($('#TechnicalExpertID').val());

    var selRowIds = jQuery('#tbInspList').jqGrid('getGridParam', 'selarrrow');

    if (selRowIds.length > 0) {

        if ($('#TechnicalExpertID').val() > 0) {
            for (var i = 0; i < selRowIds.length; i++) {
                submitArray.push(selRowIds[i]);
            }
            AssignTechExpert(submitArray);
        }
        else
            alert("Please select technical expert");

    }
    else
        alert("No records to assign");

}

function AssignTechExpert(submitArray) {
    if (confirm("Do you want to assign the selected works to the technical expert ? ")) {

        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/AssignTechExpert/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'submitarray': submitArray }),

            success: function (response) {
                unblockPage();
                alert(response.message);
                if (response.success) {
                    $("#tbInspList").trigger('reloadGrid');
                }
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            }

        });
    }
    else {
        return;
    }

}

// Added by Srishti on 10-04-2023
function RemoveTechnicalExpert(observationId) {

    if (confirm("Do you want to remove assigned technical expert ? ")) {

        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/RemoveTechnicalExpert/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'observationId': observationId }),

            success: function (response) {
                unblockPage();
                alert(response.message);
                if (response.success) {
                    $("#tbInspList").trigger('reloadGrid');
                }
            },

            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            }
        });
    }
    else {
        return;
    }
}

function addWorks() {

    if (arrFinalizeAndForward.length == 0)
        alert("Please select atleast one of the works.");

    else if (confirm('Do you want to Finalize selected works ?')) {

        $.ajax({
            url: '/QualityMonitoring/FinalizeTechnicalExpert',
            type: 'POST',
            data: { arrFinalizeAndForward: arrFinalizeAndForward },
            success: function (response) {
                unblockPage();
                alert(response.message);
                if (response.success) {
                    $("#tbInspList").trigger('reloadGrid');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    //else {
    //    return false;
    //}
}

//function FinalizeTechnicalExpert(observationId) {

//    if (confirm("Do you want to Finalize ? ")) {

//        $.ajax({
//            type: "POST",
//            url: "/QualityMonitoring/FinalizeTechnicalExpert/",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            data: JSON.stringify({ 'observationId': observationId }),

//            success: function (response) {
//                unblockPage();
//                alert(response.message);
//                if (response.success) {
//                    $("#tbInspList").trigger('reloadGrid');
//                }

//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            }

//        });
//    }
//    else {
//        return;
//    }
//}

function ForwardToNQM(observationId) {

    if (confirm("Do you want to forward the remarks to the NQM ? ")) {
        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/ForwardToNQM/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'observationId': observationId }),

            success: function (response) {
                unblockPage();
                alert(response.message);
                if (response.success == true) {
                    $("#tbInspList").trigger('reloadGrid');
                }

            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            }

        });
    }
    else {
        return;
    }
}

//Srishti
//function QMInspectionListTEGrid(stateCode, monitorCode, toInspMonth, toInspYear) {

//    $('#div3TierPIUInspectionDetails').show('slow');
//    $("#tbInspList").jqGrid('GridUnload');

//    jQuery("#tbInspList").jqGrid({
//        url: '/QualityMonitoring/QMInspectionListForTechExpert?' + Math.random(),
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Monitor", "State", "District", "Block", "Road/Bridge Name", "Package No", "Road/Bridge", "Scheme", "Work Status (As inspected)", "Sanctioned Length(km) / Bridge Length(m)", "From(km)", "To(km)", "Inspection Date",
//            "Overall Grade", "View Report (pdf)", "View abstract/Images", "Images Uploaded", "Add/Edit Remarks", "View Remarks", "Forward To CQC"],
//        colModel: [
//            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
//            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
//            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
//            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
//            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
//            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
//            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
//            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
//            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
//            { name: 'SancLength', index: 'SancLength', width: 50, sortable: false, align: "center", search: false },
//            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
//            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
//            { name: 'viewreport', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
//            { name: 'viewImages', index: 'UploadReport', width: 50, sortable: false, align: "center", search: false },
//            { name: 'Images uploaded', index: 'View', width: 30, sortable: false, align: "center", search: false },  //
//            { name: 'AddRemarks', index: 'AddRemarks', width: 30, sortable: false, align: "center", search: false },
//            { name: 'ViewRemarks', index: 'ViewRemarks', width: 30, sortable: false, align: "center", search: false },
//            { name: 'ForwardToCQC', index: 'ForwardToCQC', width: 30, sortable: false, align: "center", search: false }
//        ],
//        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "toInspMonth": toInspMonth, "toInspYear": toInspYear },
//        pager: jQuery('#dvInspListPager'),
//        rowNum: 20000,
//        viewrecords: true,
//        pgbuttons: false,
//        pgtext: null,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Inspection List",
//        height: '300',
//        autowidth: true,
//        autoResizing: { adjustGridWidth: false },
//        grouping: true,
//        groupingView: {
//            groupField: ['State', 'District'],
//            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
//            groupColumnShow: [false, false],
//            groupCollapse: false
//        },
//        loadError: function (xhr, status, error) {
//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//        }
//    }); //end of grid
//    jQuery("#tbInspList").jqGrid('setGroupHeaders', {
//        useColSpanStyle: true,
//        groupHeaders: [
//            { startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: '<center>Inspection Chainage</center>' },
//            { startColumnName: 'viewreport', numberOfColumns: 3, titleText: '<center>Report</center>' },

//        ]
//    });

//}


// Bhushan

//Add for NQM Inspection Review list by Technical Expert on 29-03-2023 by Bhushan

//Moditfy By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins" -- 22-06-2023
function QMInspectionListTEReviewGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, schemeType, roadStatus, roadOrBridge, gradeType) {

    /*alert("state : " + stateCode + ", monitor : " + monitorCode + ", from month : " + fromInspMonth + ", from year : " +
        fromInspYear + ", to month : " + toInspMonth + ", to year : " + toInspYear + ", scheme : " + schemeType + 
        ", road status : " +  roadStatus + ", road or lsb : " + roadOrBridge + ", grade : " + gradeType)*/

    //alert("QMInspectionListTEReviewGridList Grid.");
    // commented by srishti on 3-05-2023 
    //$('#div3TierPIUInspectionDetails').show('slow');
    $("#tbInspList").jqGrid('GridUnload');

    jQuery("#tbInspList").jqGrid({
        url: '/QualityMonitoring/QMInspectionDetailsTechExpertReviewList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //multiselect: true,
        colNames: ["Monitor", "State", "District", "Block", "Road/Bridge Name", "Package No", "Road/Bridge", "Scheme", "Work Status (As inspected)", "Sanctioned Length(km) / Bridge Length(m)", "From(km)", "To(km)", "Inspection Date",
            "Overall Grade", "View Report (pdf)", "View abstract/Images", "Images Uploaded", "Add Remarks", /*"View Remarks" ,*/ "Finalize and Forward To CQC", "View E-Form", "View Test Report"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
            { name: 'SancLength', index: 'SancLength', width: 50, sortable: false, align: "center", search: false },
            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
            { name: 'viewreport', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
            { name: 'viewImages', index: 'UploadReport', width: 50, sortable: false, align: "center", search: false },
            { name: 'Images uploaded', index: 'View', width: 30, sortable: false, align: "center", search: false },  //
            { name: 'AddEditViewRemarks', index: 'AddEditViewRemarks', width: 30, sortable: false, align: "center", search: false },
            //{ name: 'ViewRemarks', index: 'ViewRemarks', width: 30, sortable: false, align: "center", search: false },
            { name: 'ForwardToCQC', index: 'ForwardToCQC', width: 30, sortable: false, align: "center", search: false },

            //added By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins" -- 22-06-2023
            { name: 'Eform', index: 'eform', width: 30, sortable: false, align: "center", search: false },
            { name: 'TestReport', index: 'TestReport', width: 30, sortable: false, align: "center", search: false }

        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType": "I", "schemeType": schemeType, "roadStatus": roadStatus, "roadOrBridge": roadOrBridge, "gradeType": gradeType },
        pager: jQuery('#dvInspListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        height: '300',
        autowidth: true,
        autoResizing: { adjustGridWidth: false },  //
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            //$("#tbInspList #dvInspListPager").css({ height: '40px' });
            $("#tbInspList").jqGrid("setLabel", "colName", { height: 50 });

            var windowWidth = window.innerWidth;
            var grid = $("#tbInspList");
            grid.setGridWidth(windowWidth - 100);
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
    jQuery("#tbInspList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: '<center>Inspection Chainage</center>' },
            { startColumnName: 'viewreport', numberOfColumns: 3, titleText: '<center>Report</center>' },

        ]
    });

}


function ForwardTEQMReplyToCQC(obsId) {
    if (confirm("Do you want to forward the remarks to the CQC ? ")) {
        $.ajax({
            url: '/QualityMonitoring/ForwardTEQMReplyToCQC/' + obsId,
            type: 'POST',
            //data: { selectedState: $("#stateCode3TierSQCInsp").val(), value: Math.random() },
            success: function (data) {
                alert(data.message)
                $('#tbInspList').trigger('reloadGrid');
                Close3TierPIUInspectionDetails();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    } else {
        return;
    }
}


//--------------------------------------------- 


function AddEditViewReplyBy_TE_QM(obsId) {

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUInspection').show('slow', function () {
        $("#div3TierSQCInspFilterForm").toggle("slow");
        $('#div3TierPIUInspectionDetails').toggle('slow');
        //$("#tbInspList").jqGrid('setGridState', 'hidden');

        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/TEQMObservationDetails/' + obsId, function () {
            unblockPage();

        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    //$('#div3TierPIUInspectionDetails').toggle('slow');
    $('#div3TierPIUInspDetails').show('slow');

    //$('#div3TierPIUInspectionDetails').hide('slow');
    //$('#div3TierSQCInspFilterForm').hide('slow');
    //$("#tbInspList").jqGrid('setGridState', 'hidden');
}


$('#btnsubmitform').click(function () {
    var jsonData = $("#frmGradeCorrection").serialize();

    //alert(JSON.stringify(jsonData));
    //console.log(JSON.stringify(jsonData));
    if (confirm("Do you want to Save the remarks ? ")) {
        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/SaveTechExpertRemarks",
            data: $("#frmGradeCorrection").serialize(),
            async: false,

            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                alert("Error");

            },
            success: function (data) {
                unblockPage();
                //alert(data.Success);
                //alert(data.Success)
                if (data.Success) {
                    alert(data.status)
                    Close3TierPIUInspectionDetails();
                    $('#tbInspList').trigger('reloadGrid');
                    // $("#btnListDetails").trigger('click');
                } else {
                    alert(data.status);
                    return;
                }

            }
        });
    } else {
        return;
    }
});


function SaveFinalizeAndForwardTEQMRemarksToCQC(obsId) {
    var jsonData = $("#frmGradeCorrection").serialize();

    //alert(JSON.stringify(jsonData));
    //console.log(JSON.stringify(jsonData));
    if (confirm("Do you want to Save and Forward the remarks to CQC ? ")) {
        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/SaveTechExpertRemarks",
            data: $("#frmGradeCorrection").serialize(),
            async: false,

            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                alert("Error");

            },
            success: function (data) {
                unblockPage();

                if (data.Success) {
                    $.ajax({
                        url: '/QualityMonitoring/ForwardTEQMReplyToCQC/' + obsId,
                        type: 'POST',
                        //data: { selectedState: $("#stateCode3TierSQCInsp").val(), value: Math.random() },
                        success: function (data) {
                            if (data.success) {
                                alert("Remarks saved and forwarded to CQC successfully !!");
                            } else {
                                alert(data.message)
                            }

                            //$('#tbInspList').trigger('reloadGrid');
                            //Close3TierPIUInspectionDetails();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status);
                            alert(thrownError);
                        }
                    });
                    Close3TierPIUInspectionDetails();
                    $('#tbInspList').trigger('reloadGrid');
                    $('#div3TierPIUInspectionDetails').show('slow');
                    // $("#btnListDetails").trigger('click');
                } else {
                    alert(data.status)
                    return;
                }

            }
        });
    } else {
        return;
    }
}

$('#btnUpdateform').click(function () {
    var jsonData = $("#frmGradeCorrection").serialize();

    //alert(JSON.stringify(jsonData));
    //console.log(JSON.stringify(jsonData));
    if (confirm("Do you want to Update the remarks ? ")) {
        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/UpdateTechExpertRemarks",
            data: $("#frmGradeCorrection").serialize(),
            async: false,

            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                alert("Error");

            },
            success: function (data) {
                unblockPage();
                //alert(data.Success)
                if (data.Success) {
                    alert(data.status)
                    Close3TierPIUInspectionDetails();
                    $('#tbInspList').trigger('reloadGrid');
                    // $("#btnListDetails").trigger('click');
                } else {
                    alert(data.status);
                    return;
                }

            }
        });
    } else {
        return;
    }
});

function EditFinalizeAndForwardTEQMRemarksToCQC(obsId1) {
    var jsonData = $("#frmGradeCorrection").serialize();

    //alert(JSON.stringify(jsonData));
    //console.log(JSON.stringify(jsonData));
    if (confirm("Do you want to Update and Forward the remarks to CQC ? ")) {
        $.ajax({
            type: "POST",
            url: "/QualityMonitoring/UpdateTechExpertRemarks",
            data: $("#frmGradeCorrection").serialize(),
            async: false,

            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                alert("Error");

            },
            success: function (data) {
                unblockPage();
                //alert(data.status)
                //alert(data.Success)
                if (data.Success) {
                    $.ajax({
                        url: '/QualityMonitoring/ForwardTEQMReplyToCQC/' + obsId1,
                        type: 'POST',
                        //data: { selectedState: $("#stateCode3TierSQCInsp").val(), value: Math.random() },
                        success: function (data) {
                            if (data.success) {
                                alert("Remarks Updated and forwarded to CQC successfully !!");
                            } else {
                                alert(data.message)
                            }

                            //$('#tbInspList').trigger('reloadGrid');
                            //Close3TierPIUInspectionDetails();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status);
                            alert(thrownError);
                        }
                    });
                    Close3TierPIUInspectionDetails();
                    $('#tbInspList').trigger('reloadGrid');
                    // $("#btnListDetails").trigger('click');
                } else {
                    alert(data.status);
                    return;
                }

            }
        });
    } else {
        return;
    }
}

//-------------------------------------------
// Srishti 

//function AddReplyByNQM(obsId) {

//    $("#accordion3TierPIUInspection div").html("");
//    $("#accordion3TierPIUInspection h3").html(
//        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

//        '<a href="#" style="float: right;">' +
//        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
//        '<span style="float: right;"></span>'
//    );

//    $('#accordion3TierPIUInspection').show('slow', function () {
//        blockPage();
//        $("#div3TierPIUInspDetails").load('/QualityMonitoring/AddReplyByNQM/' + obsId, function () {
//            unblockPage();
//        });
//    });

//    $("#div3TierPIUInspDetails").css('height', 'auto');
//    $('#div3TierPIUInspDetails').show('slow');
//    //$('#id3TierSQCInspFilterDiv').trigger('click');
//    $('#idTEFilterDiv').hide('slow');
//    $("#tblAllocatedWorksDetails").jqGrid('setGridState', 'hidden');
//    $('#div3TierPIUInspectionDetails').hide('slow');
//    $('#div3TierSQCInspFilterForm').hide('slow');
//    //$('#id3TierSQCInspFilterDiv').trigger('click');

//}


function ViewTEQMRemark(obsId) {

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUInspection').show('slow', function () {
        $("#div3TierSQCInspFilterForm").toggle("slow");
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/ViewTEQMRemarkForCQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');
    $("#tbInspList").jqGrid('setGridState', 'hidden');
}




//added By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins and Allocate Technical Expert menu in cqc login" --start-- 01-06-2023
function viewCombinedPart_1_2_Pdf(id) {
    //$("#adobedwnldDiv").hide();

    //alert(id);
    $.ajax({
        type: "POST",
        url: '/EFORM/isPart1Part2PdfAvail/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open("/EFORM/viewCombinedPdf12/" + id);
            }
            else {
                alert(response.Message);
            }


        },
        error: function (error) {
            alert(error);
            //$("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }

    });
}

function viewTRPdfVirtualDir(id) {
    //$("#adobedwnldDiv").hide();

    $.ajax({
        type: "POST",
        url: '/EFORM/viewTRPdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            alert(error);
            //$("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }

    });
}
//added By Hrishikesh To "download eform and Test Report For TechnicalExperts Logins and Allocate Technical Expert menu in cqc login" --end-- 01-06-2023
