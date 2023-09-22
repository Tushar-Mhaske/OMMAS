
$(function () {
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    OfficeCategoryReportsListing();
});

function OfficeCategoryReportsListing() {
    $("#OfficeCategoryDetailsTable").jqGrid({
        url: '/MasterReports/OfficerCategoryDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['MAST_OFFICER_CATEGORY_NAME'],
        colModel: [
            { name: "MAST_OFFICER_CATEGORY_NAME", width: '200', align: 'center', height: 'auto' }
             ],
        pager: $("#OfficeCategoryDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_OFFICER_CATEGORY_NAME',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'Officer Category Details',
        loadComplete: function () {
            $('#OfficeCategoryDetailsTable_rn').html('Sr.<br/>No.');
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