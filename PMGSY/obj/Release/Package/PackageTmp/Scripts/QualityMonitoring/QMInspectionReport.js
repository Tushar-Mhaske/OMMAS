/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMInspectionReport.js
        * Description   :   Handles events, grids in Inspection Report
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    loadInspectionReportGrid();

    loadDistrictwiseInspectionReportGrid("tbDistwiseInspDetailsReport", "dvDistwiseInspDetailsReportPager",  0 , 0, '0');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function loadInspectionReportGrid()
{
    $("#tbInspectionCountReport").jqGrid('GridUnload');

    jQuery("#tbInspectionCountReport").jqGrid({
        url: '/QualityMonitoring/QMInspectionReportList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "No. Of Roads", "Inspected By NQMs", "Inspected By SQMs"],
        colModel: [
                            //{ name: 'StateName', index: 'StateName', width: 120, sortable: false, align: "center" },
                            //{ name: 'DistrictName', index: 'DistrictName', width: 120, sortable: false, align: "center", search: false, summaryType: 'count', summaryTpl: 'Total ({0})' },
                            //{ name: 'TotalRdCnt', index: 'TotalRdCnt', width: 70, sortable: false, align: "center", search: false,sorttype:'number',formatter:'integer', summaryType:'sum' },
                            //{ name: 'NqmInspCnt', index: 'NqmInspCnt', width: 70, sortable: false, align: "center", search: false, sorttype: 'number', formatter: 'integer', summaryType: 'sum' },
                            //{ name: 'SqmInspCnt', index: 'SqmInspCnt', width: 70, sortable: false, align: "center", search: false, sorttype: 'number', formatter: 'integer', summaryType: 'sum' }

                            { name: 'StateName', index: 'StateName', width: 120, sortable: false, align: "center" },
                            { name: 'DistrictName', index: 'DistrictName', width: 120, sortable: false, align: "center", search: false },
                            { name: 'TotalRdCnt', index: 'TotalRdCnt', width: 70, sortable: false, align: "center", search: false },
                            { name: 'NqmInspCnt', index: 'NqmInspCnt', width: 70, sortable: false, align: "center", search: false },
                            { name: 'SqmInspCnt', index: 'SqmInspCnt', width: 70, sortable: false, align: "center", search: false }
        ],
        pager: jQuery('#dvInspectionCountReportPager'),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Districtwise Inspection Count",
        height: 220,
        autowidth: true,
        sortname: 'StateName',
        //rowList: [5, 10, 15],
        grouping: true,
        groupingView: {
            groupField: ['StateName'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false],
            groupCollapse: true,
            groupOrder: ['asc']
            //groupSummary: [true],
            //showSummaryOnHide: true,
            //groupDataSorted: true
        },
        loadComplete: function () {
               //$("#gview_tbInspectionCountReport > .ui-jqgrid-titlebar").hide();
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
}



function loadDistrictwiseInspectionReportGrid(gridId, divPager, stateCode, districtCode, qmType) {

    $("#"+ gridId).jqGrid('GridUnload');

    $("#" + gridId).jqGrid({
        url: '/QualityMonitoring/QMOverallDistrictwiseInspDetailsReport?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "Block", "Monitor", "Package", "Sanction Year", "Road Name", "Type", "Length (Km.)", "Start Chainage (Km.)", "End Chainage (Km.)", "Road Status", "Inspection Date", "Overall Grade"],
        colModel: [
                        { name: 'StateName', index: 'StateName', width: 140, sortable: false, align: "center" },
                        { name: 'DistrictName', index: 'DistrictName', width: 140, sortable: false, align: "center", search: false },
                        { name: 'BlockName', index: 'BlockName', width: 140, sortable: false, align: "center", search: false },
                        { name: 'MonitorName', index: 'MonitorName', width: 100, sortable: false, align: "left", search: false },
                        { name: 'Package', index: 'Package', width: 100, sortable: false, align: "center", search: false },
                        { name: 'SanctionYear', index: 'SanctionYear', width: 100, sortable: false, align: "center", search: false },
                        { name: 'RoadName', index: 'RoadName', width: 180, sortable: false, align: "left", search: false },
                        { name: 'PropType', index: 'PropType', width: 50, sortable: false, align: "left", search: false },
                        { name: 'RdLength', index: 'RdLength', width: 70, sortable: false, align: "center", search: false },
                        { name: 'StartChainage', index: 'StartChainage', width: 70, sortable: false, align: "center", search: false },
                        { name: 'EndChainage', index: 'EndChainage', width: 70, sortable: false, align: "center", search: false },
                        { name: 'RdStatus', index: 'RdStatus', width: 90, sortable: false, align: "center", search: false },
                        { name: 'InspDate', index: 'InspDate', width: 100, sortable: false, align: "center", search: false },
                        { name: 'OverallGrade', index: 'OverallGrade', width: 100, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "qmType" : qmType},
        pager: $("#"+ divPager),
        rowNum: 1000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Districtwise Inspected Road Details",
        height: 220,
        autowidth: true,
        sortname: 'StateName',
        //rowList: [5000, 7500, 10000],
        grouping: true,
        groupingView: {
            groupField: ['StateName', 'DistrictName'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            //$("#gview_" + gridId + " > .ui-jqgrid-titlebar").hide();
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
}


function viewDistrictwiseInspDetails(stateCode, districtCode, qmType)
{
    loadDistrictwiseInspectionReportGrid("tbDistwiseInspDetailsReport", "dvDistwiseInspDetailsReportPager", stateCode, districtCode, qmType);
}