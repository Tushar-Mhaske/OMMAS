$(document).ready(function () {


    $.validator.unobtrusive.parse("#frm");

    $("#dvSearchHeadType").show();
    $("#dvSearchHeadType").load("/Master/SearchHeadType/");

    
    //Method for jqgrid.
    $('#tblList').jqGrid({
        url: '/Master/GetMasterExecutionDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Item Description','Item Short Description','Item Type','Action'],
        colModel: [
                            { name: 'HeadDesc', index: 'HeadDesc', height: 'auto', width: 100, align: "left", sortable: true },
                            { name: 'HeadShDesc', index: 'HeadShDesc', height: 'auto', width: 70, align: "left", sortable: true },
                            { name: 'HeadType', index: 'HeadType', height: 'auto', width: 70, align: "left", sortable: true },
                            { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'HeadType',
        sortorder: "asc",
        caption: "Execution Item List", 
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        
    });

    //function for loading Add form.
    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchHeadType").is(":visible")) {
            $('#dvSearchHeadType').hide('slow');
        }
        if (!$("#dvDetails").is(":visible")) {
            $("#dvDetails").load("/Master/AddMasterExecution/");
            $('#dvDetails').show('slow');
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
    });

    //function for loading search form.  
    $('#btnSearchView').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvDetails").is(":visible")) {
            $('#dvDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchHeadType").is(":visible")) {
            $('#dvSearchHeadType').load('/Master/SearchHeadType', function () {
                $('#tblList').trigger('reloadGrid');
                var data = $('#tblList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {
                    $('#ddlSearchType').val(data.typeCode);
                }
                $('#dvSearchHeadType').show('slow');
            });
        }
        $.unblockUI();
    });


});

//function for edit functionality.
function editMasterExecution(urlParam) {
    $.ajax({
        url: "/Master/EditMasterExecution/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchHeadType").is(":visible")) {
                $('#dvSearchHeadType').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $("#dvDetails").html(data);
            $("#dvDetails").show();
            $("#MAST_HEAD_DESC").focus();

        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
        }

    });

}

//Function for delete functionality.
function deleteMasterExecution(urlParam) {
    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete Execution details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/DeleteMasterExecution/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //if ($("#dvSearchHeadType").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //    $.unblockUI();
                    //}
                    //else {
                    //    $("#dvDetails").load("/Master/AddMasterExecution/");
                    //    $('#tblList').trigger('reloadGrid');
                    //    $.unblockUI();
                    //}
                    if ($("#dvDetails").is(":visible")) {
                        $('#dvDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }

                    if (!$("#dvSearchHeadType").is(":visible")) {
                        $("#dvSearchHeadType").show('slow');
                    }
                    $('#tblList').trigger('reloadGrid');
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); $.unblockUI(); }
        });
    }
    else {
        return false;
        $.unblockUI();
    }
}


function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Execution Details' onClick ='editMasterExecution(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Execution Details' onClick =deleteMasterExecution(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";

}