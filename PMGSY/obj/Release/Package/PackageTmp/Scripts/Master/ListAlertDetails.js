$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/Master/SearchAlertDetails',
        type: "Get",
        dataType: "html",
        success: function (data) {
            $("#dvSearchAlertDetails").html(data);
            LoadAlertDetailsList();
            $.unblockUI();
        },
        error: function (xhr, opt, error) {
            $.unblockUI();

            alert("An Error occured while proccessing your request.");
            return false;
        }

    });
    
    $("#btnCreateNew").click(function () {
        if ($("#dvAddAlertDetails").is(':hidden')) {

            $("#dvSearchAlertDetails").hide();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddAlertDetails").load('/Master/AddEditAlertDetails', function () {
                $.unblockUI();
            });
            $("#dvAddAlertDetails").show('slow');
            $("#btnCreateNew").hide('slow');
            $("#trSearchDetails").show('slow');

        }
    });

    $("#btnSearchAlerts").click(function () {
        if ($("#dvSearchAlertDetails").is(':hidden')) {

            $("#dvSearchAlertDetails").show();                        
            $("#dvAddAlertDetails").hide('slow');
            $("#btnCreateNew").show();
            $("#trSearchDetails").hide();

        }
    });
    

});
function LoadAlertDetailsList()
{
    $('#tblAlertDetails').jqGrid({
        url: '/Master/ListAlertsDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Heading', 'Subject', 'Start Date','End Date','Status','Edit','Delete','Change Status'],
        colModel: [
                      { name: 'ALERT_HEADING', index: 'ALERT_HEADING', height: 'auto', width: 100, align: "left", sortable: true },
                      { name: 'ALERT_TEXT', index: 'ALERT_TEXT', height: 'auto', width: 300, align: "left", sortable: true },
                      { name: 'DISPLAY_START_DATE', index: 'DISPLAY_START_DATE', height: 'auto', width: 70, align: "center", sortable: true },
                      { name: 'DISPLAY_END_DATE', index: 'DISPLAY_END_DATE', height: 'auto', width: 70, align: "center", sortable: true },
                      { name: 'ALERT_STATUS', index: 'ALERT_STATUS', height: 'auto', width: 70, align: "left", sortable: true },
                      { name: 'edit', width: 30, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 30, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false},
                      { name: 'changeStatus', width: 30, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }
        ],
        postData:{Status:$("#ddlStatus").val()},
        pager: jQuery('#dvPagerAlertDetails'),
        rowNum: 05,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ALERT_HEADING',
        sortorder: "asc",
        caption: "Alert Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Alert Details' onClick ='EditAlertDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Alert Details' onClick ='DeleteAlertDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangeAlertStatus(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function EditAlertDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/ViewAlertDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddAlertDetails").html(data);
            $("#dvAddAlertDetails").show();
            $("#btnCreateNew").hide('slow');
            $("#dvSearchAlertDetails").hide();
            $("#trSearchDetails").show();

            
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


function DeleteAlertDetails(urlparameter) {
    if (confirm("Are you sure you want to delete alert details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteAlertDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Alert details deleted successfully.');
                    //$('#tblAlertDetails').trigger('reloadGrid');
                    //$("#dvAddAlertDetails").load('/Master/AddEditAlertDetails');
                    if ($("#dvSearchAlertDetails").is(':hidden')) {
                        $("#dvSearchAlertDetails").show();
                        $("#dvAddAlertDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $("#trSearchDetails").hide();
                    }
                    $('#tblAlertDetails').trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('An error occured while processing your request.');
                    return false;
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



function ChangeAlertStatus(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeAlertStatus/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Alert status details changed successfully.');
                    //$('#tblAlertDetails').trigger('reloadGrid');
                    //$("#dvAddAlertDetails").load('/Master/AddEditAlertDetails');
                    if ($("#dvSearchAlertDetails").is(':hidden')) {
                        $("#dvSearchAlertDetails").show();
                        $("#dvAddAlertDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $("#trSearchDetails").hide();
                    }
                    $('#tblAlertDetails').trigger('reloadGrid');
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

