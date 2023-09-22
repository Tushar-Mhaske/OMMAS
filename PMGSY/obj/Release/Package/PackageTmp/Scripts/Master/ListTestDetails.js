$(document).ready(function () {

    if (!$("#divSearchTestDetails").is(":visible")) {

        $('#divSearchTestDetails').load('/Master/SearchTestDetails');
        $('#divSearchTestDetails').show('slow');

        $("#btnSearch").hide();
    }
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
  
    //LoadTestDetailsList();
    
    $("#btnAdd").click(function () {
        if ($("#dvAddTestDetails").is(':hidden')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddTestDetails").load('/Master/AddEditTestDetails', function () {
                $.unblockUI();
            });
            $("#dvAddTestDetails").show('slow');
            $("#btnAdd").hide('slow');
            $("#btnSearch").show();
        }
        if ($("#divSearchTestDetails").is(":visible")) {
            $('#divSearchTestDetails').hide('slow');
        }
    });
    $('#btnSearch').click(function (e) {

        if ($("#dvAddTestDetails").is(":visible")) {
            $('#dvAddTestDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#divSearchTestDetails").is(":visible")) {

            $('#divSearchTestDetails').load('/Master/SearchTestDetails', function () {
                var data = $('#tblTestDetails').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlTestStatus').val(data.Status);
                }
                $('#divSearchTestDetails').show('slow');
            });
        }
        $.unblockUI();
    });

});
function LoadTestDetailsList()
{
    $('#tblTestDetails').jqGrid({
        url: '/Master/TestDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Name', 'Discription', 'Status','Edit','Delete','Change Status'],
        colModel: [
                      { name: 'MAST_TEST_NAME', index: 'MAST_TEST_NAME', height: 'auto', width: 100, align: "left", sortable: true },
                      { name: 'MAST_TEST_DESC', index: 'MAST_TEST_DESC', height: 'auto', width: 300, align: "left", sortable: true },
                      { name: 'MAST_TEST_STATUS', index: 'MAST_TEST_STATUS', height: 'auto', width: 70, align: "left", sortable: true },
                      { name: 'edit', width: 30, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 30, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false ,hidden:true},
                      { name: 'changeStatus', width: 30, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }
        ],
        postData: { Status: $('#ddlTestStatus option:selected').val() },
        pager: jQuery('#dvPagerTestDetails'),
        rowNum: 05,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_TEST_NAME',
        sortorder: "asc",
        caption: "Test Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Test Details' onClick ='EditTestDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Test Details' onClick ='DeleteTestDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangeTestStatus(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function EditTestDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetTestDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddTestDetails").html(data);
            $("#dvAddTestDetails").show();
            $("#btnAdd").hide('slow');
            $('#btnSearch').show();
            if ($("#divSearchTestDetails").is(":visible")) {
                $('#divSearchTestDetails').hide('slow');
            }

            if (data.success == false)
            {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}


function DeleteTestDetails(urlparameter) {
    if (confirm("Are you sure you want to delete test details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteTestDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Test details deleted successfully.');
                    //$('#tblTestDetails').trigger('reloadGrid');
                    //$("#dvAddTestDetails").load('/Master/AddEditTestDetails');
                    if ($("#dvAddTestDetails").is(":visible")) {
                        $('#dvAddTestDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTestDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTestDetails").show('slow');
                        $('#tblTestDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTestDetails').trigger('reloadGrid');
                    }
                }
                else if (data.success == false) {
                    alert('Test details is in use and can not be deleted.');
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



function ChangeTestStatus(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeTestStatus/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Test status details changed successfully.');
                    //$('#tblTestDetails').trigger('reloadGrid');
                    //$("#dvAddTestDetails").load('/Master/AddEditTechDetails');
                    if ($("#dvAddTestDetails").is(":visible")) {
                        $('#dvAddTestDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTestDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTestDetails").show('slow');
                        $('#tblTestDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTestDetails').trigger('reloadGrid');
                    }
                }
                else if (data.success == false) {
                    alert('An error occued while proccessing your request.');
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

