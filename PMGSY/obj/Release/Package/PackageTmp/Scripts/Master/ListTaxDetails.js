$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    LoadTaxDetailsList();
    
    $("#btnCreateNew").click(function () {
        if ($("#dvAddTaxDetails").is(':hidden')) {
            $("#dvAddTaxDetails").load('/Master/AddEditTaxDetails');
            $("#dvAddTaxDetails").show('slow');
            $("#btnCreateNew").hide('slow');
        }
    });


});
function LoadTaxDetailsList()
{
    $('#tblstTaxDetails').jqGrid({
        url: '/Master/GetTaxDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['TDS', 'Service Charge','Service Tax', 'Effective Date','Action'],
        colModel: [
                      { name: 'MAST_TDS', index: 'MAST_TDS', height: 'auto', width: 150, align: "left", sortable: true },
                      { name: 'MAST_TDS_SC', index: 'MAST_TDS_SC', height: 'auto', width: 200, align: "left", sortable: true },
                      { name: 'MAST_EFFECTIVE_DATE', index: 'MAST_EFFECTIVE_DATE', height: 'auto', width: 150, align: "left", sortable: true },
                      { name: 'SERVICE_TAX', index: 'SERVICE_TAX', height: 'auto', width: 200, align: "left", sortable: true },
                      { name: 'a', width: 65, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pglstTaxDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'NoName',
        sortorder: "asc",
        caption: "Tax Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
    });

}
function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Tax Details' onClick ='EditTaxDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Tax Details' onClick ='DeleteTaxDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}
function EditTaxDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditTaxDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddTaxDetails").html(data);
            $("#dvAddTaxDetails").show();
            $("#btnCreateNew").hide('slow');

            if (data.success == false)
            {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}
function DeleteTaxDetails(urlparameter)
{
    if (confirm("Are you sure you want to delete tax details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteTaxDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Tax details deleted successfully.');
                    $('#tblstTaxDetails').trigger('reloadGrid');
                    $("#dvAddTaxDetails").load('/Master/AddEditTaxDetails');
                }
                else if(data.success == false) {
                    alert('Tax details is in use and can not be deleted.');
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