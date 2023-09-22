
$(document).ready(function () {
$("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
$('#btnCreateNew').click(function () {

        if (!$('#dvComponentDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvComponentDetails").load("/Master/AddEditMasterComponentType", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvComponentDetails').show('slow');
                $.unblockUI();
            });
        }
    });
$('#tblMasterComponentTypeList').jqGrid({

    url: '/Master/GetMasterComponentTypeList/',
    datatype: 'json',
    mtype: "POST",
    colNames: ['Component Name', 'Action'],
    colModel: [
     { name: 'MAST_COMPONENT_NAME', index: 'MAST_COMPONENT_NAME', height: 'auto', width: 300, align: "left", sortable: true },
   
     { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
    ],
    pager: jQuery('#divPagerMasterComponentType'),
    rowNum: 15,
       
    rowList: [10, 15, 20, 30],
    viewrecords: true,
    recordtext: '{2} records found',
    sortname: 'MAST_COMPONENT_NAME',
    sortorder: "asc",
    caption: 'Component List',
    height: '100%',
    autowidth: true,
    rownumbers: true,
    hidegrid: false,
   
    loadComplete: function () { },

    loadError: function (xhr, status, error) {

        if (xhr.responseText == "session expired") {

            alert(xht.responseText);
            window.location.href = "Login/login";
        }
        else {
            alert("Invalid Data. Please Check and Try Again");
        }
    }

});
    
});


function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Component Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Component Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

}


function editData(id) {

    $("#btnCreateNew").hide();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterComponentType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
        
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvComponentDetails").show();
            $("#dvComponentDetails").html(data);
            $("#MAST_COMPONENT_NAME").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError)
        {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteData(urlParam)
{  
    if (confirm("Are you sure you want to delete Component Type details?")) {
        $.ajax({

            url: "/Master/DeleteMasterComponentType/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    // $("#tblMasterComponentTypeList").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#dvComponentDetails').hide('slow');
                    $("#tblMasterComponentTypeList").trigger('reloadGrid');
                  
                }
                else {
                    alert(data.message);
                }
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }

        });
    }
    else {
        return false;
    }


}
