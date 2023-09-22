
$(document).ready(function () {
 $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#dvUnitDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvUnitDetails").load("/Master/AddEditMasterUnit", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvUnitDetails').show('slow');
                $.unblockUI();
            });
        }
    });

    $('#tblUnitList').jqGrid({
        url: '/Master/GetMasterUnitList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Unit Name', 'Unit Short Name', 'Unit Dimension','Action'],
        colModel: [
         { name: 'MAST_UNIT_NAME', index: 'MAST_UNIT_NAME', height: 'auto', width: 130, align: "left", sortable: true },
         { name: 'MAST_UNIT_SHORT_NAME', index: 'MAST_UNIT_SHORT_NAME', height: 'auto', width: 130, align: "left", sortable: true },
         { name: 'MAST_UNIT_DIMENSION', index: 'MAST_UNIT_DIMENSION', height: 'auto', width: 100, align: "center", sortable: true },
        { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerUnit'),
        rowNum: 15,
    
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_UNIT_NAME',
        sortorder: "asc",
        caption: 'Unit List',
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

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Unit Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Unit Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

}


function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterUnit/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvUnitDetails").show();
            $("#dvUnitDetails").html(data);
            $("#MAST_UNIT_NAME").focus();
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
    if (confirm("Are you sure you want to delete Unit details?")) {
        $.ajax({
            url: "/Master/DeleteMasterUnit/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //  $("#tblUnitList").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#dvUnitDetails').hide('slow');
                    $('#tblUnitList').trigger('reloadGrid');
                
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
