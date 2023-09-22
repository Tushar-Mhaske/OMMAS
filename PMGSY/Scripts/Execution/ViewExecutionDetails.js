$(document).ready(function () {

    LoadFinancialDetails();
    if ($("#ddlImsProposalTypes").val() == "P") {
        LoadRoadPhysicalDetails();
    }
    else if ($("#ddlImsProposalTypes").val() == "L") {
        LoadLSBPhysicalDetails();
    }
    else 
    {
        if ($("#RoadType").val() == "P")
        {
            LoadRoadPhysicalDetails();
        }
        else if ($("#RoadType").val() == "L")
        {
            LoadLSBPhysicalDetails();
        }
    }

});
function LoadRoadPhysicalDetails()
{
    
    $("#tbListRoadDetails").jqGrid('GridUnload');
    jQuery("#tbListRoadDetails").jqGrid({
        url: '/execution/getRoadProposalExecutionList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $("#ProposalCode").val() },
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
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 80, align: "center", search: false },
                            //{ name: 'a', width: 40, align: "center", search: false, sortable: false, hidden: true },
                            //{ name: 'b', width: 40, align: "center", search: false, sortable: false, hidden: true },

        ],
        pager: jQuery('#pgRoadDetails').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Physical Road Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            var reccount = $("#tbListRoadDetails").getGridParam('reccount');
            var reccountLSB = $("#tbListLSBDetails").getGridParam('reccount');
                
                if ($("#ddlImsProposalTypes").val() == "P")
                {
                    if (reccount == 0) {
                        $("#dvPhysicalRoadDetails").html('<center><b>Physical progress details are not present for this proposal.</b></center>');
                        $("#dvFinancialDetails").html('');
                    }
                }

                

                if ($("#ddlImsProposalTypes").val() == "A") {

                    if ($("#RoadType").val() == "P") {
                        if (reccount == 0) {
                            $("#dvPhysicalRoadDetails").html('<center><b>Physical progress details are not present for this proposal.</b></center>');
                            $("#dvFinancialDetails").html('');
                        }
                    }
                }
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
function LoadLSBPhysicalDetails()
{
    $("#tbListLSBDetails").jqGrid('GridUnload');
    jQuery("#tbListLSBDetails").jqGrid({
        url: '/Execution/GetLSBProposalExecutionList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $("#ProposalCode").val() },
        colNames: ['Month', 'Year', 'Work Status', 'Cutoff/raft/Individual footing', 'Floor Protection', 'Sinking', 'Bottom Pluggings', 'Top Pluggings', 'Well Caps', 'Pier/Abutment Shaft', 'Pier/Abutment Caps', 'Bearings', 'Deck Slab', 'Wearing Coat', 'Posts & Railing', 'Road Work', 'CD Work', 'Bridge Length Completed', 'Approach Work Completed'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 40, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 60, align: "left", search: true },
                            { name: 'EXEC_RAFT', index: 'EXEC_RAFT', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_FLOOR_PROTECTION', index: 'EXEC_FLOOR_PROTECTION', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_SINKING', index: 'EXEC_SINKING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BOTTOM_PLUGGING', index: 'EXEC_BOTTOM_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_TOP_PLUGGING', index: 'EXEC_TOP_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_WELL_CAP', index: 'EXEC_WELL_CAP', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PIER_SHAFT', index: 'EXEC_PIER_SHAFT', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_PIER_CAP', index: 'EXEC_PIER_CAP', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BEARINGS', index: 'EXEC_BEARINGS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_DECK_SLAB', index: 'EXEC_DECK_SLAB', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_WEARING_COAT', index: 'EXEC_WEARING_COAT', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_POSTS_RAILING', index: 'EXEC_POSTS_RAILING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_APP_ROAD_WORK', index: 'EXEC_APP_ROAD_WORK', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_APP_CD_WORKS', index: 'EXEC_APP_CD_WORKS', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_COMPLETED', index: 'EXEC_APP_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BRIDGE_COMPLETED', index: 'EXEC_BRIDGE_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            //{ name: 'a', width: 30, align: "center", search: false, hidden: true },
                            //{ name: 'b', width: 40, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pgLSBDetails'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_YEAR,EXEC_PROG_MONTH',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Physical LSB Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            var reccountLSB = $("#tbListLSBDetails").getGridParam('reccount');
            if ($("#ddlImsProposalTypes").val() == "L") {
                if (reccountLSB == 0) {
                    $("#dvPhysicalLSBDetails").html('<center><b>Physical progress details are not present for this proposal.</b></center>');
                    $("#dvFinancialDetails").html('');
                }
            }

            if ($("#ddlImsProposalTypes").val() == "A") {

                if ($("#RoadType").val() == "L") {
                    if (reccountLSB == 0) {
                        $("#dvPhysicalLSBDetails").html('<center><b>Physical progress details are not present for this proposal.</b></center>');
                        $("#dvFinancialDetails").html('');
                    }
                }


            }

        },
        loadError: function (xhr, status, error) {

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
function LoadFinancialDetails()
{
    $("#tbListFinancialDetails").jqGrid('GridUnload');
    jQuery("#tbListFinancialDetails").jqGrid({
        url: '/Execution/GetProposalFinancialList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: $("#ProposalCode").val() },
        colNames: ['Year', 'Month', 'Upto Last Month', 'During This Month', 'Total', 'Upto Last Month', 'During This Month', 'Total', 'Is Final Payment Made', 'Date'],
        colModel: [
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 65, align: "left", search: false },
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_VALUEOFWORK_LASTMONTH', index: 'EXEC_VALUEOFWORK_LASTMONTH', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_VALUEOFWORK_THISMONTH', index: 'EXEC_VALUEOFWORK_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL', index: 'TOTAL', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_LASTMONTH', index: 'EXEC_PAYMENT_LASTMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_PAYMENT_THISMONTH', index: 'EXEC_PAYMENT_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_PAYMENT', index: 'TOTAL_PAYMENT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_FLAG', index: 'EXEC_FINAL_PAYMENT_FLAG', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'EXEC_FINAL_PAYMENT_DATE', index: 'EXEC_FINAL_PAYMENT_DATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, align: "center", search: false, hidden: true },
                            //{ name: 'b', width: 50, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pgFinancialDetails'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "EXEC_PROG_YEAR,EXEC_PROG_MONTH",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Financial Progress List",
        height: 'auto',
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

    jQuery("#tbListFinancialDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: '<center>Value of Work Done(Rs. in Lakh)</center>' },
          { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: '<center>Payment Made(Rs. in Lakh)</center>' }
        ]
    });
}