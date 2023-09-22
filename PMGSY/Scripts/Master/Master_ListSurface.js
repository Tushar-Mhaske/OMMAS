
$(document).ready(function () {
$("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#dvSurfaceDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvSurfaceDetails").load("/Master/AddEditMasterSurface", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvSurfaceDetails').show('slow');
                $.unblockUI();
            });
        }
    });

    $('#tblMasterSurfaceList').jqGrid({
        url: '/Master/GetMasterSurfaceList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Surface Name','Action'],
        colModel: [
         { name: 'MAST_SURFACE_NAME', index: 'MAST_SURFACE_NAME', height: 'auto', width: 300, align: "left", sortable: true },
         { name: 'a', width: 200, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerMasterSurface'),
        rowNum: 15,
     
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_SURFACE_NAME',
        sortorder: "asc",
        caption: 'Surface List',
        height: '100%',
      
        autowidth:true,
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

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Surface Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Surface Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

}


function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterSurface/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvSurfaceDetails").show();
            $("#dvSurfaceDetails").html(data);
            $("#MAST_SURFACE_NAME").focus();
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
    if (confirm("Are you sure you want to delete Surface details?")) {
        $.ajax({
            url: "/Master/DeleteMasterSurface/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#tblMasterSurfaceList").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#dvSurfaceDetails').hide('slow');
                    $('#tblMasterSurfaceList').trigger('reloadGrid');
               
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
