$(document).ready(function () {
$.validator.unobtrusive.parse('#frmAdd');
if (!$("#divSearchTrafficDetails").is(":visible")) {

    $('#divSearchTrafficDetails').load('/Master/SearchTrafficDetails');
    $('#divSearchTrafficDetails').show('slow');

    $("#btnSearch").hide();
}
$("input[type=text]").bind("keypress", function (e) {
    if (e.keyCode == 13) {
        return false;
    }
});
$('#btnAdd').click(function () {

        if (!$('#dvAddTrafficDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvAddTrafficDetails").load("/Master/AddTrafficType", function () {               
                $.unblockUI();
            });
            $("#dvAddTrafficDetails").show('slow');
            $("#btnAdd").hide('slow');
            $("#btnSearch").show();
        }
        if ($("#divSearchTrafficDetails").is(":visible")) {
            $('#divSearchTrafficDetails').hide('slow');
        }
        
    });

  
    $('#btnSearch').click(function (e) {

        if ($("#dvAddTrafficDetails").is(":visible")) {
            $('#dvAddTrafficDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#divSearchTrafficDetails").is(":visible")) {

            $('#divSearchTrafficDetails').load('/Master/SearchTrafficDetails', function () {
                var data = $('#tblTrafficDetails').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlTrafficStatus').val(data.Status);
                }
                $('#divSearchTrafficDetails').show('slow');
            });
        }
        $.unblockUI();
    });
});

function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Traffic Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Traffic Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(urlparameter) {
    $("#btnAdd").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditTrafficType/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvAddTrafficDetails").show();
            $("#dvAddTrafficDetails").html(data);
            $("#MAST_TRAFFIC_NAME").focus();
            $("#btnAdd").hide('slow');
            $('#btnSearch').show();
            if ($("#divSearchTrafficDetails").is(":visible")) {
                $('#divSearchTrafficDetails').hide('slow');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete Traffic Type details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteTrafficType/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                   // $("#tblTrafficDetails").trigger('reloadGrid');
                    if ($("#dvAddTrafficDetails").is(":visible")) {
                        $('#dvAddTrafficDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTrafficDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTrafficDetails").show('slow');
                        $('#tblTrafficDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTrafficDetails').trigger('reloadGrid');
                    }

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
function changeType(urlparameter) {

    if (confirm("Are you sure you want to change Traffic Type status?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/ChangeTrafficType/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    
                    $("#tblTrafficDetails").trigger('reloadGrid', [{
                        page: $('#tblTrafficDetails').getGridParam('page'),
                        sortname: 'MAST_TRAFFIC_NAME',
                    }]);
                 
                  //  $("#dvAddTrafficDetails").show();
                   // $("#dvAddTrafficDetails").load("/Master/AddTrafficType");
                    //  $("#btnReset").trigger('click');

                    if ($("#dvAddTrafficDetails").is(":visible")) {
                        $('#dvAddTrafficDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#divSearchTrafficDetails").is(":visible")) {
                        $('#btnAdd').show();
                        $("#divSearchTrafficDetails").show('slow');
                        $('#tblTrafficDetails').trigger('reloadGrid');
                    } else {
                        $('#tblTrafficDetails').trigger('reloadGrid');
                    }
                }
                else {
                    alert("You can not change this Traffic Type details.");
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

function LoadTrafficDetailsList() {
    jQuery("#tblTrafficDetails").jqGrid({
        url: '/Master/GetTrafficTypeList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Traffic Type', 'Traffic Status', 'Change Status', 'Action'],
        colModel: [
                            { name: 'MAST_TRAFFIC_NAME', index: 'MAST_TRAFFIC_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_TRAFFIC_STATUS', index: 'MAST_TRAFFIC_STATUS', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'Change To', index: 'Change To', height: 'auto', width: 100, sortable: false, align: "center" },
                            { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { Status: $('#ddlTrafficStatus option:selected').val() },
        pager: jQuery('#pager'),
        rowNum: 15,
        // altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        //sortname: 'MAST_TRAFFIC_STATUS,MAST_TRAFFIC_NAME',
        sortname: 'MAST_TRAFFIC_NAME',
        sortorder: "asc",
        caption: "Traffic Type List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
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
}