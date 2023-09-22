
$(function () {
    GradeReportsListing();

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function GradeReportsListing() {
    $("#GradeDetailsTable").jqGrid({
        url: '/MasterReports/GradeDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Grade Name','Grade Short Name'],
        colModel: [
            {name: "MAST_GRADE_NAME", width: 600, align: 'left', height: 'auto'},
            {name: "MAST_GRADE_SHORT_NAME", width: 150, align: 'left', height: 'auto'},
        ],
        pager: $("#GradeDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_GRADE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Grade Details',
        loadComplete: function () {
            $('#GradeDetailsTable_rn').html('Sr.<br/>No.');
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