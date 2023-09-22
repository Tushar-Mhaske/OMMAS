$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    $.ajax({
        url: "/Master/SearchScourFoundation/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchScourFoundation").html(data);
            $('#btnSearch').trigger('click');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });

    LoadGrid();
    $.unblockUI();

    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchScourFoundation").is(":visible")) {
            $('#dvSearchScourFoundation').hide('slow');
        }

        if (!$("#dvScourFoundationDetails").is(":visible")) {
            $("#dvScourFoundationDetails").load("/Master/AddEditMasterScourFoundationType/");
            $('#dvScourFoundationDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }

    });
    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvScourFoundationDetails").is(":visible")) {
            $('#dvScourFoundationDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }


        if (!$("#dvSearchScourFoundation").is(":visible")) {

            $('#dvSearchScourFoundation').load('/Master/SearchScourFoundation', function () {

                $('#tblMasterScourFoundationTypeList').trigger('reloadGrid');

                var data = $('#tblMasterScourFoundationTypeList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlScourFoundationType').val(data.SfTypeCode);
                }
                $('#dvSearchScourFoundation').show('slow');
            });
        }
        $.unblockUI();
    });
   
  
});


function FormatColumn(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;'  class='ui-icon ui-icon-pencil' title='Edit Scour/Foundation Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash' title='Delete Scour/Foundation Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function editData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterScourFoundationType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {        

            if ($("#dvSearchScourFoundation").is(":visible")) {
                $('#dvSearchScourFoundation').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();

            $("#dvScourFoundationDetails").show();
            $("#dvScourFoundationDetails").html(data);

            $("#IMS_SC_FD_NAME").focus();

            $.unblockUI();

        },
        error: function (xht, ajaxOptions, throwError)
        {
            if ($("#dvSearchScourFoundation").is(":visible")) {
                $('#dvSearchScourFoundation').hide('slow');
            }
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}
function deleteData(urlParam)
{  
    if (confirm("Are you sure you want to delete Scour/Foundation details?")) {
        $.ajax({
            url: "/Master/DeleteMasterScourFoundationType/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchScourFoundation").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $('#tblMasterScourFoundationTypeList').trigger('reloadGrid');
                    //}
                    if ($("#dvScourFoundationDetails").is(":visible")) {
                        $('#dvScourFoundationDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }

                    if (!$("#dvSearchScourFoundation").is(":visible")) {
                        $("#dvSearchScourFoundation").show('slow');
                    }
                    $('#tblMasterScourFoundationTypeList').trigger('reloadGrid');
                }
                else {
                    alert(data.message);
                }
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }
        });
    }
    else {
        return false;
    }
}
function LoadGrid()
{
    $('#tblMasterScourFoundationTypeList').jqGrid({
        url: '/Master/GetMasterScourFoundationTypeList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Scour / Foundation Name', 'Scour / Foundation Type', 'Action'],
        colModel: [
         { name: 'IMS_SC_FD_NAME', index: 'IMS_SC_FD_NAME', height: 'auto', width: 200, align: "left", sortable: true },
         { name: 'IMS_SC_FD_TYPE', index: 'IMS_SC_FD_TYPE', height: 'auto', width: 130, align: "left", sortable: true },
         { name: 'a', width: 85, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerMasterScourFoundationType'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_SC_FD_NAME',
        sortorder: "asc",
        caption: 'Scour/Foundation List',
        height: '100%',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again!");
            }
        }
    });
}
