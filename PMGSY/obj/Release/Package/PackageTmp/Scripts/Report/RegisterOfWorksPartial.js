$(document).ready(function () {

    RegisterOfWorksGrid();

});


function RegisterOfWorksGrid()
{
    $("#tblRegisterOfWorksList").jqGrid('GridUnload');

    jQuery("#tblRegisterOfWorksList").jqGrid({
        url: '/Reports/RegisterOfWorksHeaderGrid?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Road Name", "Month & Year", "Bill No.", "Date", "Narration", "Amount (in Rs.)", "<b>Total</b>"],
        colModel: [
                            { name: 'RoadName', index: 'RoadName', width: 150, sortable: false, align: "left" },
                            { name: 'BillMonthYear', index: 'BillMonthYear', width: 100, sortable: false, align: "center", search: false },
                            { name: 'BillNo', index: 'BillNo', width: 70, sortable: false, align: "center", search: false },
                            { name: 'Date', index: 'Date', width: 70, sortable: false, align: "center", search: false },
                            { name: 'Narration', index: 'Narration', width: 150, sortable: false, align: "left", search: false },
                            { name: 'Amount', index: 'Amount', width: 70, sortable: false, align: "right", search: false, search: false },
                            { name: 'Total', index: 'Total', width: 70, sortable: false, align: "right", hidden:true }
        ],
        postData: { "ADMIN_ND_CODE": $("#ADMIN_ND_CODE").val(), "MAST_CON_ID": $("#MAST_CON_ID").val(), "TEND_AGREEMENT_CODE": $("#TEND_AGREEMENT_CODE").val() },
        pager: jQuery('#divRegisterOfWorksListPager'),
        rowNum: 30,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Register Of Works Details",
        height: 'auto',
        autowidth: true,
        sortname: 'RoadName',
        rowList: [30, 60, 90],
        grouping: true,
        groupingView: {
            groupField: ['RoadName', 'BillMonthYear'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false, false],
        },
        loadComplete: function () {
            //$("#gview_tblRegisterOfWorksList > .ui-jqgrid-titlebar").hide();
            $('#tblRegisterOfWorksList').setGridWidth(($('#dvRegisterOfWorksPartial').width() - 5), true);
            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}
