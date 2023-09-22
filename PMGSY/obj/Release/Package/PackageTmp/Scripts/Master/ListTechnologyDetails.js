$(document).ready(function () {
    if (!$("#divSearchTechnologyDetails").is(":visible")) {

        $('#divSearchTechnologyDetails').load('/Master/SearchTechnologyDetails');
        $('#divSearchTechnologyDetails').show('slow');

        $("#btnSearch").hide();
    }
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
 
    
    //LoadTechnologyDetailsList();
    
    $("#btnAdd").click(function () {
        if ($("#dvAddTechnologyDetails").is(':hidden')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddTechnologyDetails").load('/Master/AddEditTechDetails', function () {
                $.unblockUI();
            });
            $("#dvAddTechnologyDetails").show('slow');
            $("#btnAdd").hide('slow');
            $("#btnSearch").show();
        }
        if ($("#divSearchTechnologyDetails").is(":visible")) {
            $('#divSearchTechnologyDetails').hide('slow');
        }
    });

    $('#btnSearch').click(function (e) {

        if ($("#dvAddTechnologyDetails").is(":visible")) {
            $('#dvAddTechnologyDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#divSearchTechnologyDetails").is(":visible")) {

            $('#divSearchTechnologyDetails').load('/Master/SearchTechnologyDetails', function () {
                var data = $('#tblTechnologyDetails').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlTechnologyStatus').val(data.Status);
                }
                $('#divSearchTechnologyDetails').show('slow');
            });
        }
        $.unblockUI();
    });
});
function LoadTechnologyDetailsList()
{
    $('#tblTechnologyDetails').jqGrid({
        url: '/Master/TechnologyDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Name', 'Discription', 'Status', 'Type', 'Edit', 'Delete', 'Change Status'],
        colModel: [
                      { name: 'MAST_TECH_NAME', index: 'MAST_TECH_NAME', height: 'auto', width: 100, align: "left", sortable: true },
                      { name: 'MAST_TECH_DESC', index: 'MAST_TECH_DESC', height: 'auto', width: 100, align: "left", sortable: true },
                      { name: 'MAST_TECH_STATUS', index: 'MAST_TECH_STATUS', height: 'auto', width: 70, align: "left", sortable: true },
                      { name: 'MAST_TECH_TYPE', index: 'MAST_TECH_TYPE', height: 'auto', width: 200, align: "left", sortable: true },
                      { name: 'edit', width: 30, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 30, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false ,hidden:true},
                      { name: 'changeStatus', width: 30, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }

        ],
        postData: { Status: $('#ddlTechnologyStatus option:selected').val() },
        pager: jQuery('#dvPagerTechnologyDetails'),
        rowNum: 05,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_TECH_NAME',
        sortorder: "asc",
        caption: "Technology Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Technology Details' onClick ='EditTechnologyDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Technology Details' onClick ='DeleteTechnologyDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangeTechnologyStatus(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



function EditTechnologyDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetTechnologyDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddTechnologyDetails").html(data);
            $("#dvAddTechnologyDetails").show();
            $("#btnAdd").hide('slow');
            $('#btnSearch').show();
            if ($("#divSearchTechnologyDetails").is(":visible")) {
                $('#divSearchTechnologyDetails').hide('slow');
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

function DeleteTechnologyDetails(urlparameter)
{
    if (confirm("Are you sure you want to delete technology details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteTechnologyDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Technology details deleted successfully.');
                    // $("#dvAddTechnologyDetails").load('/Master/AddEditTechDetails');
                    if ($("#dvAddTechnologyDetails").is(":visible")) {
                        $('#dvAddTechnologyDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTechnologyDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTechnologyDetails").show('slow');
                        $('#tblTechnologyDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTechnologyDetails').trigger('reloadGrid');
                    }
                }
                else if(data.success == false) {
                    alert('Technology details is in use and can not be deleted.');
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



function ChangeTechnologyStatus(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeTechnologyStatus/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Technology status details changed successfully.');
                    //$('#tblTechnologyDetails').trigger('reloadGrid');
                    //$("#dvAddTechnologyDetails").load('/Master/AddEditTechDetails');
                    if ($("#dvAddTechnologyDetails").is(":visible")) {
                        $('#dvAddTechnologyDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTechnologyDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTechnologyDetails").show('slow');
                        $('#tblTechnologyDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTechnologyDetails').trigger('reloadGrid');
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

