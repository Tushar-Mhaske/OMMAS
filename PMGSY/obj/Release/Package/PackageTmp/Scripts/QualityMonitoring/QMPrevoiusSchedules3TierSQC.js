/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPrevoiusSchedules3TierSQC.js
        * Description   :   Handles events, grids in Previous Schedules for NQMs in SQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $('#btnPrevSchDetails').click(function () {

        $("#dvPrevSchDistrictwiseDetails").hide();
        PrevSchListGrid($("#prevSchMonitorID").val(), $("#prevSchMonthID").val(), $("#prevSchYearID").val());
       

    });//btnPrevSchDetails ends here


    PrevSchListGrid($("#prevSchMonitorID").val(), $("#prevSchMonthID").val(), $("#prevSchYearID").val());

    $('#prevSchMonthID').change(function () {

        $("#prevSchMonitorID").empty();
        $("#prevSchMonitorID").val(0);

        if ($(this).val() == 0) {
            $("#prevSchMonitorID").append("<option value='0'>All Monitors</option>");
        }

        if ($("#prevSchMonthID").val() > 0) {

            if ($("#prevSchMonitorID").length > 0) {

                        getMonitors();
            }
        }
    });//month change ends here


    $('#prevSchYearID').change(function () {

        $("#prevSchMonitorID").empty();
        $("#prevSchMonitorID").val(0);

        if ($(this).val() == 0) {
            $("#prevSchMonitorID").append("<option value='0'>All Monitors</option>");
        }

        if ($("#prevSchYearID").val() > 0) {

            if ($("#prevSchMonitorID").length > 0) {

                getMonitors();
            }
        }
    });//year change ends here


});//doc.ready ends here



//------------------------------------------------------------------------------------------------------------------------
function getMonitors()
{
    $.ajax({
        url: '/QualityMonitoring/Populate3TierSQCMonitors',
        type: 'POST',
        data: { inspMonth: $("#prevSchMonthID").val(), inspYear: $("#prevSchYearID").val(), districtCode: 0, qmType: $("#QM_TYPE").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#prevSchMonitorID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


function PrevSchListGrid(prevSchMonitorId, prevSchMonthID, prevSchYearID) {

    $("#tbPrevScheduleList").jqGrid('GridUnload');

    jQuery("#tbPrevScheduleList").jqGrid({
        url: '/QualityMonitoring/GetPrevScheduleList3TierSQC?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District 1", "District 2", "District 3", "Inspection Status", "View"],
        colModel: [
                    { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
                    { name: 'State', index: 'State', width: 120, sortable: false, align: "center", search: false },
                    { name: 'District1', index: 'District1', width: 100, sortable: false, align: "left", search: false },
                    { name: 'District2', index: 'District2', width: 100, sortable: false, align: "left", search: false },
                    { name: 'District3', index: 'District3', width: 100, sortable: false, align: "left", search: false },
                    { name: 'InspStatus', index: 'InspStatus', width: 80, sortable: false, align: "center", search: false },
                    { name: 'View', index: 'View', width: 70, sortable: false, align: "center", search: false }
        ],
        postData: { "prevSchMonitorId": prevSchMonitorId, "prevSchMonthID": prevSchMonthID, "prevSchYearID": prevSchYearID, "is3TierPIU": $("#hdnTierId").val() },
        pager: jQuery('#dvPrevScheduleListPager'),
        rowNum: 1000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: '250',
        autowidth: true,
        sortname: 'Monitor',
        //rowList: [5, 10, 20, 30],
        //grouping: true,
        //groupingView: {
        //    groupField: ['Monitor'],
        //    groupText: ['<b>{0}</b>'],
        //    groupColumnShow: [false]
        //},
        loadComplete: function () {
            $("#gview_tbPrevScheduleList > .ui-jqgrid-titlebar").hide();

            if ($("#hdnRoleCodeOn3TierSQC").val() == 22) //for SQC hide these two columns
            {
                $('#tbPrevScheduleList').jqGrid('hideCol', 'State');
                $('#tbPrevScheduleList').jqGrid('hideCol', 'District1');
                $('#tbPrevScheduleList').jqGrid('hideCol', 'District2');
                $('#tbPrevScheduleList').jqGrid('hideCol', 'District3');

                $('#tbPrevScheduleList').setGridWidth(($('#divQualityLayoutPIU').width() - 100), true);
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
}



//------------------------------------------------------------------------------------------------------------------------