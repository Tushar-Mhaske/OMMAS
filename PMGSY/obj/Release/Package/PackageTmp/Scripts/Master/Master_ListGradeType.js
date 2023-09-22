
$(document).ready(function () {
$("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#dvGradeDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvGradeDetails").load("/Master/AddEditMasterGradeType", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvGradeDetails').show('slow');
                $.unblockUI();
            });
        }
    });


    $('#tblMasterGradeTypeList').jqGrid({
        url: '/Master/GetMasterGradeTypeList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Grade Name', 'Grade Short Name', 'Action'],
        colModel: [
         { name: 'MAST_GRADE_NAME', index: 'MAST_GRADE_NAME', height: 'auto', width: 180, align: "left", sortable: true },
         { name: 'MAST_GRADE_SHORT_NAME', index: 'MAST_GRADE_SHORT_NAME', height: 'auto', width: 120, align: "left", sortable: true },
         { name: 'a', width: 90, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#divPagerMasterGradeType'),
        rowNum: 15,
      
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_GRADE_NAME',
        sortorder: "asc",
        caption: 'Grade List',
        height: '100%',
     
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
       
        emptyrecords: 'No Records Found',
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
   
});


function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Grade Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Grade Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

  
}


function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterGradeType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#dvGradeDetails").show();
            $("#dvGradeDetails").html(data);
            $("#MAST_GRADE_NAME").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError)
        {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteData(urlParam)
{  
    if (confirm("Are you sure you want to delete Grade details?")) {
        $.ajax({
            url: "/Master/DeleteMasterGradeType/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    $("#btnCreateNew").show();
                    $('#dvGradeDetails').hide('slow');
                    $("#tblMasterGradeTypeList").trigger('reloadGrid');
               
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
