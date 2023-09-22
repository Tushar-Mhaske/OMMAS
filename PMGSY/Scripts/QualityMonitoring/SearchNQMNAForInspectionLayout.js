$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmSearchNQMNAForInspectionLayout');
    //GetNQMNAInspectionList();
    $('#btnView').click(function () {

        if (!$('#frmSearchNQMNAForInspectionLayout').valid()) {
            return false;
        }
        GetNQMNAInspectionList();
    });

    $('#btnView').trigger('click');
});

function GetNQMNAInspectionList() {
    $("#tblNQMNAForInspectionLayoutGrid").jqGrid('GridUnload');
    $('#tblNQMNAForInspectionLayoutGrid').jqGrid({
        url: '/QualityMonitoring/ListNQMNotAvailableInspections',
        datatype: "json",
        mtype: "POST",
        postData: { month: $('#ddlMonth').val(), year: $('#ddlYear').val() },
        colNames: ['NQM', 'Month', 'Year', 'Delete'],
        colModel: [
                      { name: 'State', index: 'State', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'Month', index: 'Month', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'Year', index: 'Year', height: 'auto', width: 10, align: "center", sortable: false, hidden: false },
                      { name: 'Delete', index: 'Delete', height: 'auto', width: 10, align: "center", sortable: false },
        ],
        pager: jQuery('#divNQMNAForInspectionLayoutPager'),
        rowNum: 200,
        //rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ID',
        sortorder: "asc",
        caption: "NQM Not Available for Inspection List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {

        },
    });
}

function DeleteNQMNADetails(urlParam) {
    if (confirm("Are you sure to Delete NQM Details ? ")) {
        $.ajax({
            type: 'POST',
            url: '/QualityMonitoring/DeleteNQMNADetails/' + urlParam,
            async: false,
            cache: false,
            data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            //traditional: true,
            success: function (data) {
                //arr.splice(0, arr.length); //Clear the preveious value
                //alert(data.success);
                if (data.success) {
                    GetNQMNAInspectionList();
                }
                if (data.success == false) {
                    /**/
                }
                alert(data.message);
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                arr.splice(0, arr.length);//Clear the preveious value
                alert(data.message);
                $.unblockUI();
            }
        });
    } else {
        return false;
    }
}