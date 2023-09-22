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
            $("#dvLayoutofAddChapterView").load('/ARRR/AddEditCategoryDetails');
            $("#dvLayoutofAddChapterView").show('slow');
            $("#btnAdd").hide('slow');
        }


    });


});

// Format Columns Function

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Category Details' onClick ='EditChapterDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Category Details' onClick ='DeleteChapterDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


// Load List Function

function LoadChapterDetailsList() {
    $("#tblstChapterDetails").jqGrid('GridUnload');
    $('#tblstChapterDetails').jqGrid({
        url: '/ARRR/GetCategoryList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Category Description', 'Type', 'Action'],
        colModel: [
                      { name: 'MAST_LMM_CATEGORY', index: 'MAST_LMM_CATEGORY', height: 'auto', width: 800, align: "left", sortable: true },

                      { name: 'MAST_LMM_TYPE', index: 'MAST_LMM_TYPE', height: 'auto', width: 450, align: "left", sortable: true },

                      { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pglstChapterDetails'),
        rowNum: 15,
        rowList: [06, 12, 18],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'NoName',
        sortorder: "asc",
        caption: "Category Description Details List",
        height: 'auto',
        autowidth: '100%',
        rownumbers: true,
        hidegrid: false,
        grouping: true,
        //groupingView: {
        //    groupField: ['MAST_LMM_TYPE'],
        //    //groupDataSorted: true,
        //    //groupText: '',
        //    //groupText: ['<b> {0}  </b>  Total Amount: {Amount}'],
        //    //groupText: ['{0}'],
        //    groupText: ["<span style='font-size: 100%;'><b>{0}</b></span>", "<span style='font-size: 100%;'><b>{0}</b></span>"],
        //    groupSummary: [false, true],
        //    groupCollapse: false,
        //    groupColumnShow: false,
        //    showSummaryOnHide: true
        //},
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        },
    });

}


function EditChapterDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/ARRR/EditCategoryDetails1/' + urlparameter,
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
    if (confirm("Are you sure you want to delete Category details?")) {
        $.ajax({
            type: 'POST',
            url: '/ARRR/DeleteCategoryDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Category details deleted successfully.');
                    $('#tblstChapterDetails').trigger('reloadGrid');
                    $("#dvLayoutofAddChapterView").load('/ARRR/AddEditCategoryDetails');
                }
                else if (data.success == false) {
                    alert('Category details are in use and can not be deleted.');
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






