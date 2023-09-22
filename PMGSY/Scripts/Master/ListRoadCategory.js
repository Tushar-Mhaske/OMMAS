$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddRoadCategory');
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#roadDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#roadDetails").load("/Master/AddRoadCategory", function () {
                $("#btnCreateNew").hide('slow');
                $('#roadDetails').show('slow');
                $.unblockUI();
            });
        }
    });



    jQuery("#roadCategory").jqGrid({
        url: '/Master/GetRoadCategoryList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Road Category Name', 'Category Code', 'Action'],
        colModel: [
                            { name: 'MAST_ROAD_CAT_NAME', index: 'MAST_ROAD_CAT_NAME', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager').width(20),
        rowNum: 15,
     
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RaodCategoryName',
        sortorder: "asc",
        caption: "Road Category List",
        height: 'auto',
      
        autowidth:true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
               
            }
        }
    });

    $("#btnAddNew").click(function (e) {
        if (!$("#roadDetails").is(":visible")) {
            $('#roadDetails').show();
            $('#roadDetails').load("/Master/AddRoadCategory");
        }
    });


});

//function FormatColumn(cellvalue, options, rowObject) {


//    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Road Category Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Road Category Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        //return "<center><table><tr><td style='border:none;'><span>-</span></td><td style='border:none;'><span>-</span></td></tr></table></center>";
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked');'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked');'></span></td></tr></table></center>";
    }

    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Road Category Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Road Category Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function editData(urlparameter) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditRoadCategory/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#roadDetails").show();
            $("#roadDetails").html(data);
            $('#MAST_ROAD_CAT_NAME').focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete Road Category details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteRoadCategory/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#roadCategory").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#roadDetails').hide('slow');
                    $('#roadCategory').trigger('reloadGrid');
              
                }
                else {
                    alert(data.message);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}















