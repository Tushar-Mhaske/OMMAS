
$(document).ready(function () {
   
    LoadPhysicalRoadDetails();
    
});
//Load the Physical details of road
function LoadPhysicalRoadDetails() {
    $("#tbPhysicalRoadList").jqGrid('GridUnload');
    jQuery("#tbPhysicalRoadList").jqGrid({
        url: '/QualityMonitoring/GetRoadPhysicalProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { prRoadCode: $("#IMS_PR_ROAD_CODE").val() },
        colNames: ['Month', 'Year', 'Work Status', 'Preparatory Work (Length in Km.)', 'Subgrade Stage (Length in Km.)', 'Subbase (Length in Km.)', 'Base Course (Length in Km.)', 'Surface Course (Length in Km.)', 'Road Signs Stones (in Nos.)', 'CDWorks (in Nos.)', 'LS Bridges (in Nos.)', 'Miscellaneous (Length in Km.)', 'Completed (Length in Km.)'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_PREPARATORY_WORK', index: 'EXEC_PREPARATORY_WORK', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_EARTHWORK_SUBGRADE', index: 'EXEC_EARTHWORK_SUBGRADE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SUBBASE_PREPRATION', index: 'EXEC_SUBBASE_PREPRATION', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BASE_COURSE', index: 'EXEC_BASE_COURSE', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_SURFACE_COURSE', index: 'EXEC_SURFACE_COURSE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SIGNS_STONES', index: 'EXEC_SIGNS_STONES', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_CD_WORKS', index: 'EXEC_CD_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_LSB_WORKS', index: 'EXEC_LSB_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_MISCELANEOUS', index: 'EXEC_MISCELANEOUS', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 80, align: "center", search: false }

        ],
        pager: jQuery('#pagerPhysicalRoadList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Physical Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

