
$(function () {
 

    $("#QMItemsDetailsButton").click(function () {

        var qmType = $("#MAST_QMItems_QMType").val();
        var qmItemActive = $("#MAST_QMItems_QMItemActive").val();

        QMItemsReportsListing(qmType, qmItemActive);
    });
    $("#QMItemsDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function QMItemsReportsListing(qmType, qmItemActive) {
    $("#QMItemsDetailsTable").jqGrid("GridUnload");
    $("#QMItemsDetailsTable").jqGrid({
        url: '/MasterReports/QMItemsDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Monitor Type', 'Item Name', 'Item Active', 'Item Activation Date', 'Item Dactivation Date', 'Item Status', 'Grade Name'],
        colModel: [
            { name: "MAST_QM_TYPE", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_ITEM_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_ITEM_ACTIVE", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_ITEM_ACTIVATION_DATE", width: '200', align: 'center', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_ITEM_DEACTIVATION_DATE", width: '200', align: 'center', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_ITEM_STATUS", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_GRADE_NAME", width: '200', align: 'center', height: 'auto' }
            
        ],
        postData: { "QMType": qmType, "QMItemActive": qmItemActive },
        pager: $("#QMItemsDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_QM_TYPE',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'QM Items Report Details',
        loadComplete: function () {
            $('#QMItemsDetailsTable_rn').html('Sr.<br/>No.');
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