/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPreviousSchedules.js
        * Description   :   Handles events, grids in Previous Schedules Details
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    // #Added For Searchable Dropdown on 24-01-2023 
    $("#prevSchMonitorID").chosen();

    $('#btnPrevSchDetails').click(function () {

        $("#dvPrevSchDistrictwiseDetails").hide();
        PrevSchListGrid($("#prevSchMonitorID").val(), $("#prevSchMonthID").val(), $("#prevSchYearID").val());

    });//btnPrevSchDetails ends here


    PrevSchListGrid($("#prevSchMonitorID").val(), $("#prevSchMonthID").val(), $("#prevSchYearID").val());

});


function PrevSchListGrid(prevSchMonitorId, prevSchMonthID, prevSchYearID) {

    $("#tbPrevScheduleList").jqGrid('GridUnload');

    jQuery("#tbPrevScheduleList").jqGrid({
        url: '/QualityMonitoring/GetPrevScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District 1", "District 2", "District 3", "Inspection Status", "View" ],
        colModel: [
                    { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
                    { name: 'State', index: 'State', width: 120, sortable: false, align: "center", search: false },
                    { name: 'District1', index: 'District1', width: 100, sortable: false, align: "left", search: false },
                    { name: 'District2', index: 'District2', width: 100, sortable: false, align: "left", search: false },
                    { name: 'District3', index: 'District3', width: 100, sortable: false, align: "left", search: false },
                    { name: 'InspStatus', index: 'InspStatus', width: 80, sortable: false, align: "center", search: false },
                    { name: 'View', index: 'View', width: 70, sortable: false, align: "center", search: false }
        ],
        postData: { "prevSchMonitorId": prevSchMonitorId, "prevSchMonthID": prevSchMonthID, "prevSchYearID": prevSchYearID },
        pager: jQuery('#dvPrevScheduleListPager'),
        rowNum: 1000,
        pgbuttons: false,
        pgtext: null,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: '250',
        autowidth: true,
        sortname: 'Monitor',
        //rowList: [5, 10, 20, 30],
        grouping: true,
        groupingView: {
            groupField: ['Monitor'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            $("#gview_tbPrevScheduleList > .ui-jqgrid-titlebar").hide();
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


}