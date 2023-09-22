
$(document).ready(function () {
        $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
        });

 $('#btnCreateNew').click(function () {

        if (!$('#dvChecklistDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvChecklistDetails").load("/Master/AddEditMasterChecklist", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvChecklistDetails').show('slow');
                $.unblockUI();
            });
        }
 });

$('#tblMasterChecklistList').jqGrid({
        url: '/Master/GetMasterChecklistList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Checklist Point', 'Action'],
        colModel: [
         { name: 'MAST_CHECKLIST_ISSUES', index: 'MAST_CHECKLIST_ISSUES', height: 'auto', width: 400, align: "left", sortable: true },
         { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerMasterChecklist'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_CHECKLIST_ISSUES',
        sortorder: "asc",
        caption: 'Checklist Point List',
        height: 'auto',
        autowidth:true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again");
            }
        }

    });

});

function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Checklist Point Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Checklist Point Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterChecklist/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;'); 
            $("#dvChecklistDetails").html(data);
            $("#dvChecklistDetails").show();
            $("#MAST_CHECKLIST_ISSUES").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete  Checklist Point details?")) {
        $.ajax({
            url: "/Master/DeleteMasterChecklist/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#tblMasterChecklistList").trigger('reloadGrid');
                    //$("#dvChecklistDetails").load("/Master/AddEditMasterChecklist");

                    $("#dvChecklistDetails").hide('slow');
                    $("#btnCreateNew").show('slow');
                    $("#tblMasterChecklistList").trigger('reloadGrid');
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
