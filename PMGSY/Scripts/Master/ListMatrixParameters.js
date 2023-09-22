$(document).ready(function () {
   
    LoadmatrixParamDetailsList();

    $("#btnAdd").click(function () {
        if ($("#dvAddMatrixParam").is(':hidden')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddMatrixParam").load('/Master/AddEditMatrixDetails', function () {
                $('#Weight').val(''); //value of weight 0
                $.unblockUI();
            });
      
            $("#dvAddMatrixParam").show('slow');
            $("#btnAdd").hide('slow');
            $("#btnSearch").show();
        }
        
    });

   

    $('#btnSearch').click(function (e) {

        if ($("#dvAddMatrixParam").is(":visible")) {
            $('#dvAddMatrixParam').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
    })
  
    

});
function LoadmatrixParamDetailsList() {
    $('#tblMatrixParamDetails').jqGrid({
        url: '/Master/MatrixParamDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Parameter Code', 'Parameter', 'Weightage', 'Edit', 'Delete'],
        colModel: [
                      //{ name: 'MAST_MATRIX_ID', index: 'MAST_MATRIX_ID', height: 'auto', width: 20, align: "center", sortable: true,hidden:true },
                      { name: 'MAST_MATRIX_NO', index: 'MAST_MATRIX_NO', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'MAST_MATRIX_PARAMETER', index: 'MAST_MATRIX_PARAMETER', height: 'auto', width: 90, align: "left", sortable: false },
                      { name: 'MAST_MATRIX_WEIGHT', index: 'MAST_MATRIX_WEIGHT', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'edit', width: 10, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 10, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
        ],
        //postData: {   },
        pager: jQuery('#dvPagerMatrixDetails'),
        rowNum: 10,
        rowList: [5, 10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        //grouping: true,
        //groupingView: {
        //    groupField: ['name', 'MAST_MATRIX_NO'],
        //    groupDataSorted: true,
        //    groupText: ['<b>{0}</b>'],
        //},
        loadComplete: function () {
            $("#jqgh_tblMatrixParamDetails_rn").html("Sr.<br/> No");
        }
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit matrix parameters Details' onClick ='EditMatrixParamDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete matrix parameters Details' onClick ='DeleteMatrixParamDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function EditMatrixParamDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetMatrixParamDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
             
            $("#dvAddMatrixParam").html(data);
            $("#dvAddMatrixParam").show('slow');
            $('#btnAdd').hide();
            $('#btnSearch').show();
            if (data.success == false) {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    });
}


function DeleteMatrixParamDetails(urlparameter) {
    if (confirm("Are you sure you want to delete parameter details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMatrixParamDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Parameters details deleted successfully.');
                    $('#tblMatrixParamDetails').trigger('reloadGrid');
 
                }
                else if (data.success == false) {
                    alert('Parameters details is not deleted.');
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