
$(function () {

    QualificationReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});

function QualificationReportsListing() {
    $("#QualificationDetailsTable").jqGrid({
        url: '/MasterReports/QualificationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Qualification Name'],
        colModel:[
            {name: "MAST_QUALIFICATION_NAME", width: 150, align: 'left', height: 'auto'},
        ],
        pager: $("#QualificationDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_QUALIFICATION_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Qualification Details',
        loadComplete: function () {
            $('#QualificationDetailsTable_rn').html('Sr.<br/>No.');
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