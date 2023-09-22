$(document).ready(function () {
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    // Calling The Load List Function 
    LoadChapterDetailsList();

    $("#btnAdd").click(function () {
        if ($("#dvLayoutofAddChapterView").is(':hidden')) {
            $("#dvLayoutofAddChapterView").load('/ARRR/AddEditLabourDetails');
            $("#dvLayoutofAddChapterView").show('slow');
            $("#btnAdd").hide('slow');
        }


    });


});

// Format Columns Function

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Labour Details' onClick ='EditChapterDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Labour Details' onClick ='DeleteChapterDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


// Load List Function

function LoadChapterDetailsList() {
    $('#tblstChapterDetails').jqGrid({
        url: '/ARRR/GetLabourList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Category','Labour Description', 'Unit', 'Code','Action'],
        colModel: [
                      { name: 'MAST_LMM_CATEGORY', index: 'MAST_LMM_CATEGORY', height: 'auto', width: 300, align: "left", sortable: true },

                      { name: 'MAST_HEAD_NAME', index: 'MAST_HEAD_NAME', height: 'auto', width: 200, align: "left", sortable: true },

                      { name: 'MAST_UNIT_NAME', index: 'MAST_UNIT_NAME', height: 'auto', width: 200, align: "left", sortable: true },

                      { name: 'MAST_LMM_CODE', index: 'MAST_LMM_CODE', height: 'auto', width: 200, align: "left", sortable: true },

                      { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pglstChapterDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'NoName',
        sortorder: "asc",
        caption: "Labour Description Details List",
        height: 'auto',
        autowidth: '100%',
        rownumbers: true,
        hidegrid: false,
    });

}


function EditChapterDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/ARRR/EditLabourDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvLayoutofAddChapterView").html(data);
            $("#dvLayoutofAddChapterView").show();
            $("#btnAdd").hide('slow');

            if (data.success == false) {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}


function DeleteChapterDetails(urlparameter) {
    if (confirm("Are you sure you want to delete Labour details?")) {
        $.ajax({
            type: 'POST',
            url: '/ARRR/DeleteLabDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Labour details deleted successfully.');
                    $('#tblstChapterDetails').trigger('reloadGrid');
                    $("#dvLayoutofAddChapterView").load('/ARRR/AddEditLabourDetails');
                }
                else if (data.success == false) {
                    alert('Labour details are in use and can not be deleted.');
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






