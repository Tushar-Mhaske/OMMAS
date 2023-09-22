$(document).ready(function () {

    LoadPMGSY2DetailsList();
    
});
function LoadPMGSY2DetailsList()
{
    $('#tblPMGSY2Details').jqGrid({
        url: '/Master/PMGSY2DetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State',  'Status','Change Status'],
        colModel: [
                      { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                      { name: 'MAST_PMGSY2_ACTIVE', index: 'MAST_PMGSY2_ACTIVE', height: 'auto', width: 20, align: "center", sortable: false },
                      { name: 'changeStatus', width: 20, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }
        ],
        pager: jQuery('#dvPagerPMGSY2Details'),
        rowNum: 10,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: "PMGSY II List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#jqgh_tblPMGSY2Details_rn").html("Sr.<br/> No");
        }
    });

}
function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangePMGSY2Status(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function ChangePMGSY2Status(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangePMGSY2Status/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('PMGSY II status details changed successfully.');
                    $('#tblPMGSY2Details').trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('An error occued while proccessing your request.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

