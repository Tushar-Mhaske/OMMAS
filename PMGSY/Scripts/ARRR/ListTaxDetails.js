﻿$(document).ready(function ()
{
    $("input[type=text]").bind("keypress", function (e)
    {
        if (e.keyCode == 13) {
            return false;
        }
    });

    // Calling The Load List Function 
    LoadChapterDetailsList();

    $("#btnAdd").click(function () {
        if ($("#dvLayoutofAddChapterView").is(':hidden')) {
            $("#dvLayoutofAddChapterView").load('/ARRR/AddEditTaxDetails');
            $("#dvLayoutofAddChapterView").show('slow');
            $("#btnAdd").hide('slow');
        }


    });


});

// Format Columns Function

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Other Charges Details' onClick ='EditChapterDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Other Charges Details' onClick ='DeleteChapterDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


// Load List Function

function LoadChapterDetailsList() {
    $('#tblstChapterDetails').jqGrid({
        url: '/ARRR/GetTaxList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Other Charges Description', 'Is Active ?', 'Action'],
        colModel: [
                      { name: 'MAST_ARRR_TAX_NAME', index: 'MAST_ARRR_TAX_NAME', height: 'auto', width: 450, align: "left", sortable: true },

                      { name: 'MAST_ARRR_TAX_ISACTIVE', index: 'MAST_ARRR_TAX_ISACTIVE', height: 'auto', width: 450, align: "left", sortable: true },

                      { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pglstChapterDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'NoName',
        sortorder: "asc",
        caption: "Other Charges Description Details List",
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
        url: '/ARRR/EditTaxDetails/' + urlparameter,
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
    if (confirm("Are you sure you want to delete Other Charges details?")) {
        $.ajax({
            type: 'POST',
            url: '/ARRR/DeleteTaxDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Other Charges details deleted successfully.');
                    $('#tblstChapterDetails').trigger('reloadGrid');
                    $("#dvLayoutofAddChapterView").load('/ARRR/AddEditTaxDetails');
                }
                else if (data.success == false) {
                    alert('Other Charges details are in use and can not be deleted.');
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






