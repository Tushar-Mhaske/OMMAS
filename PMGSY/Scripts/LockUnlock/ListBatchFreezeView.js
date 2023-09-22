$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmFilterDetails');

    $("#btnViewDetails").click(function () {

        if ($("#frmFilterDetails").valid())
        {
            LoadFreezeUnfreezeDetails();
        }

    });


});
function LoadFreezeUnfreezeDetails()
{
   
    $("#tbrptDetails").jqGrid('GridUnload');
    jQuery("#tbrptDetails").jqGrid({
        url: '/LockUnlock/GetFreezeUnfreezeReportDetails',
        datatype: "json",
        mtype: "POST",
        postData: { BatchCode: $("#ddlBatch option:selected").val(), PMGSYScheme: $("#ddlSchemes option:selected").val(), StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val() },
        colNames: ['State Name', 'Year', 'PMGSY Scheme', 'Batch', 'Freeze/Unfreeze Date', "Freeze/Unfreeze"],
        colModel: [
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 180, align: "left", sortable: true,align:'center' },
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 150, sortable: true, align: "left", align: 'center' },
                            { name: 'MAST_PMGSY_SCHEME', index: 'MAST_PMGSY_SCHEME', width: 110, sortable: true, align: 'center' },
                            { name: 'MAST_BATCH_NAME', index: 'MAST_BATCH_NAME', height: 'auto', width: 180, sortable: true, align: "left", align: 'center' },
                            //{ name: 'IMS_TRANSACTION_NO', index: 'IMS_TRANSACTION_NO', height: 'auto', width: 150, sortable: true, align: "left", align: 'center' },
                            { name: 'IMS_FREEZE_DATE', index: 'IMS_FREEZE_DATE', width: 110, sortable: true, align: 'center' },
                            { name: 'FREEZE_STATUS', index: 'FREEZE_STATUS', width: 110, sortable: true, align: 'center' },
                            
        ],
        pager: jQuery('#dvpgrptDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: "Freeze/Unfreeze List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of grid
}