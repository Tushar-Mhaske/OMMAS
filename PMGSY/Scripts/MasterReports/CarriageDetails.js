
$(function () {
  
    $("#CarriageStatusDetailsButton").click(function () {

        var carriageStatus = $("#MAST_Carriage_Status").val();    
        CarriageReportsListing(carriageStatus);
    });
    $("#CarriageStatusDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});

function CarriageReportsListing(carriageStatus) {
    $("#CarriageDetailsTable").jqGrid("GridUnload");

    $("#CarriageDetailsTable").jqGrid({
        url: '/MasterReports/CarriageDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Carriage Width', 'Carriage Status'],
        colModel: [
            { name: "MAST_CARRIAGE_WIDTH", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_CARRIAGE_STATUS", width: '200', align: 'center', height: 'auto' }
        ],
        postData: { "CarriageStatus": carriageStatus },
        pager: $("#CarriageDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_CARRIAGE_WIDTH',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Carriage Report Details',
        loadComplete: function () {
            $('#CarriageDetailsTable_rn').html('Sr.<br/>No.');
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