$(document).ready(function () {
    $.validator.unobtrusive.parse("#frm");
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
 $('#btnCreateNew').click(function () {
        if (!$('#dvDetails').is(':visible')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvDetails").load("/Master/AddMasterQual", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvDetails').show('slow');
                $.unblockUI();
            });
        }
    });
    $('#tblList').jqGrid({

        url: '/Master/GetMasterQualDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Qualification', 'Action'],
        colModel: [
                            { name: 'QualificationName', index: 'QualificationName', height: 'auto', width: '200', align: "left", sortable: true },
                            { name: 'a', width: '80', sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'QualificationName',
        sortorder: "asc",
        caption: "Qualification List",
        height: '100%',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: true,
        hidegrid: false,
    });


});

function editMasterQual(urlParam) {
    $("#btnCreateNew").hide();
    $.ajax({
        url: "/Master/EditMasterQual/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvDetails").html(data);
            $("#dvDetails").show();
            $("#MAST_QUALIFICATION_NAME").focus();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
        }
    });
}
function deleteMasterQual(urlParam) {

    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete Qualification details?")) {
        $.ajax({
            url: "/Master/DeleteMasterQual/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                   // $('#tblList').trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#dvDetails').hide('slow');
                    $('#tblList').trigger('reloadGrid');
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
function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Qualification Details' onClick ='editMasterQual(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Qualification Details' onClick =deleteMasterQual(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";

}