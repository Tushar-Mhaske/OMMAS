
$(document).ready(function () {
    
    $("#StateDetailsButton").click(function () {
        var stateOrUnion = $("#StateList_StateDetails").val();
        var stateType = $("#StateType_StateDetails").val();
        var activeType = $("#ActiveType_StateDetails").val();
        StateMasterReportsListing(stateOrUnion, stateType,activeType);
    });
    $("#StateDetailsButton").trigger("click");
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
   
});



/*      State Master Reports        */
function StateMasterReportsListing(stateOrUnion, stateType, activeType)
{
    $("#stateDetailsTable").jqGrid("GridUnload");
    $("#stateDetailsTable").jqGrid({
        url: '/MasterReports/StateDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'State or Union Territory', 'Type', 'Short Code','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_STATE_UT', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_STATE_TYPE', width: 150, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_STATE_SHORT_CODE', width: 70, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_STATE_ACTIVE', width: 50, align: 'left', height: 'auto', sortable: false },

        ],
        postData: { "StateOrUnion": stateOrUnion, "StateType": stateType, "ActiveType": activeType },
        pager: $("#stateDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'State Details',
        loadComplete: function () {
            $('#stateDetailsTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}

