$(document).ready(function () {

    if (!$("#divSearchCarriageDetails").is(":visible")) {

        $('#divSearchCarriageDetails').load('/Master/SearchCarriageDetail');
        $('#divSearchCarriageDetails').show('slow');

        $("#btnSearch").hide();
    }
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    
    
    $("#btnAdd").click(function () {
        if ($("#dvAddCarriageDetails").is(':hidden')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddCarriageDetails").load('/Master/AddEditCarriageDetails', function () {
                $.unblockUI();
            });
            $("#dvAddCarriageDetails").show('slow');
            $("#btnAdd").hide('slow');
            $("#btnSearch").show();
        }
        if ($("#divSearchCarriageDetails").is(":visible")) {
            $('#divSearchCarriageDetails').hide('slow');
        }
    });

    $('#btnSearch').click(function (e) {

        if ($("#dvAddCarriageDetails").is(":visible")) {
            $('#dvAddCarriageDetails').hide('slow');
          
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#divSearchCarriageDetails").is(":visible")) {
           
            $('#divSearchCarriageDetails').load('/Master/SearchCarriageDetail', function () {
                var data = $('#tblCarriageDetails').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlCarriageStatus').val(data.Status);
                }
                $('#divSearchCarriageDetails').show('slow');
            });
        }
        $.unblockUI();
    });



    //$("#dvhdSearch").click(function () {

    //    if ($("#dvSearchCarriageParameter").is(":visible")) {

    //        $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvSearchCarriageParameter").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvSearchCarriageParameter").slideToggle(300);
    //    }
    //});

});
function LoadCarriageDetailsList()
{
    $('#tblCarriageDetails').jqGrid({
        url: '/Master/CarriageDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Width','Status','Edit','Delete','Change Status'],
        colModel: [
                      { name: 'MAST_CARRIAGE_WIDTH', index: 'MAST_CARRIAGE_WIDTH', height: 'auto', width: 150, align: "center", sortable: true },
                      { name: 'MAST_CARRIAGE_STATUS', index: 'MAST_CARRIAGE_STATUS', height: 'auto', width: 20, align: "center", sortable: true },
                      { name: 'edit', width: 20, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 20, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false},
                      { name: 'changeStatus', width: 20, sortable: false, resize: false, formatter: FormatColumnChangeStatus, align: "center", sortable: false }
        ],
        postData: { Status: $('#ddlCarriageStatus option:selected').val()},
        pager: jQuery('#dvPagerCarriageDetails'),
        rowNum: 10,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_CARRIAGE_WIDTH',
        sortorder: "asc",
        caption: "Carriage Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#jqgh_tblCarriageDetails_rn").html("Sr.<br/> No");
        }
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Carriage Details' onClick ='EditCarriageDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Carriage Details' onClick ='DeleteCarriageDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnChangeStatus(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-refresh' title='Change Status' onClick ='ChangeCarriageStatus(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function EditCarriageDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetCarriageDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddCarriageDetails").html(data);
            $("#dvAddCarriageDetails").show();           
            $('#btnAdd').hide();
            $('#btnSearch').show();
            if ($("#divSearchCarriageDetails").is(":visible")) {
                $('#divSearchCarriageDetails').hide('slow');
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


function DeleteCarriageDetails(urlparameter) {
    if (confirm("Are you sure you want to delete carriage details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteCarriageDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Test details deleted successfully.');
                   
                    //$("#dvAddCarriageDetails").load('/Master/AddEditCarriageDetails');
                    if ($("#dvAddCarriageDetails").is(":visible")) {
                        $('#dvAddCarriageDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();                       
                    }
                    if (!$("#divSearchCarriageDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchCarriageDetails").show('slow');
                        $('#tblCarriageDetails').trigger('reloadGrid');
                    } else {
                        $('#tblCarriageDetails').trigger('reloadGrid');
                    }
                }
                else if (data.success == false) {
                    alert('Carriage details is in use and can not be deleted.');
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



function ChangeCarriageStatus(urlparameter) {
    if (confirm("Are you sure you want to change status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeCarriageStatus/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Carriage status details changed successfully.');
                    //$('#tblCarriageDetails').trigger('reloadGrid');
                    // $("#dvAddCarriageDetails").load('/Master/AddEditCarriageDetails');
                    if ($("#dvAddCarriageDetails").is(":visible")) {
                        $('#dvAddCarriageDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchCarriageDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchCarriageDetails").show('slow');
                        $('#tblCarriageDetails').trigger('reloadGrid');
                    } else {
                        $('#tblCarriageDetails').trigger('reloadGrid');
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

