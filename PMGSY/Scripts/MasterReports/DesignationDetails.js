
$(document).ready(function () {
    $("#DesignationDetailsButton").click(function () {
        var designationType = $("#MAST_DESIG_TYPE").val();
        DesignationReportsListing(designationType);
    });
    $("#DesignationDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function DesignationReportsListing(designationType) {
    $("#DesignationDetailsTable").jqGrid("GridUnload");
    $("#DesignationDetailsTable").jqGrid({
        url: '/MasterReports/DesignationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Designation Name','Type'],
        colModel: [
            { name: "MAST_DESIG_NAME", width: 400, align: 'left', height: 'auto' },
            { name: "MAST_DESIG_TYPE", width: 300, align: 'left', height: 'auto' }
        ],
        postData: { "MAST_DESIG_TYPE": designationType},
        pager: $("#DesignationDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_DESIG_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,       
        height: '580',
        viewrecords: true,
        caption: 'Designation Details',
        loadComplete: function () {
            $('#DesignationDetailsTable_rn').html('Sr.<br/>No.');

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