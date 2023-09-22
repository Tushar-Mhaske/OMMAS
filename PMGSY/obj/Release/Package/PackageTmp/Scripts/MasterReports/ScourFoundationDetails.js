
$(function () {
    $("#ScourFoundationDetailsButton").click(function () {
        var scourFoundationType = $("#IMS_SC_FD_TYPE").val();
        ScourFoundationReportsListing(scourFoundationType);
    });
    $("#ScourFoundationDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function ScourFoundationReportsListing(scourFoundationType) {
    $("#ScourFoundationDetailsTable").jqGrid("GridUnload");
    $("#ScourFoundationDetailsTable").jqGrid({
        url: '/MasterReports/ScourFoundationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Scour/Foundation Name', 'Type'],
        colModel:[
         {name: "IMS_SC_FD_NAME", width: 150, align: 'left', height: 'auto'},
         {name: "IMS_SC_FD_TYPE", width: 150, align: 'left', height: 'auto'},
        ],
        postData: { "IMS_SC_FD_TYPE": scourFoundationType },
        pager: $("#ScourFoundationDetailsPager"),
        pgbuttons: true,
        sortname: 'IMS_SC_FD_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '100%',
        viewrecords: true,
        caption: 'Scour Foundation Details',
        loadComplete: function () {
            $('#ScourFoundationDetailsTable_rn').html('Sr.<br/>No.');
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