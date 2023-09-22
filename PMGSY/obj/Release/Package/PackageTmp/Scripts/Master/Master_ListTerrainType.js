$(document).ready(function () {
  $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
     $('#btnCreateNew').click(function () {

        if (!$('#dvDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvDetails").load("/Master/AddEditMasterTerrainType", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvDetails').show();
                $.unblockUI();
            });
        }
    });

    $('#tblList').jqGrid({
        url: '/Master/GetMasterTerrainTypeList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Terrain Name', 'Slope From','Slope To','Roadway Width','Action'],
        colModel: [
         { name: 'MAST_TERRAIN_TYPE_NAME', index: 'MAST_TERRAIN_TYPE_NAME', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'MAST_TERRAIN_SLOP_FROM', index: 'MAST_TERRAIN_SLOP_FROM', height: 'auto', width: 70, align: "center", sortable: true },
         { name: 'MAST_TERRAIN_SLOP_TO', index: 'MAST_TERRAIN_SLOP_TO', height: 'auto', width: 70, align: "center", sortable: true },
         { name: 'MAST_TERRAIN_ROADWAY_WIDTH', index: 'MAST_TERRAIN_ROADWAY_WIDTH', height: 'auto', width: 100, align: "center", sortable: true },
        { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_TERRAIN_TYPE_NAME',
        sortorder: "asc",
        caption: 'Terrain List',
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
                alert("Invalid Data. Please Check and Try Again!");
            }
        }
  
    });
});
function FormatColumn(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Terrain Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Terrain Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterTerrainType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvDetails").show();
            $("#dvDetails").html(data);
            $("#MAST_TERRAIN_TYPE_NAME").focus();
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
    if (confirm("Are you sure you want to delete Terrain details?")) {
        $.ajax({
            url: "/Master/DeleteMasterTerrainType/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {              

                if (data.success) {
                    alert(data.message);
                    // $("#tblList").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#dvDetails').hide('slow');
                    $('#tblList').trigger('reloadGrid');
                 
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
