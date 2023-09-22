$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddCdWorks');


    $('#btnCreateNew').click(function () {

        if (!$('#cdWorksDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#cdWorksDetails").load("/Master/AddCdWorksType", function () {
                $("#btnCreateNew").hide('slow');
                $("#cdWorksDetails").show();
                $.unblockUI();
            });
        }
    });
$("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    jQuery("#cdWorksType").jqGrid({
        url: '/Master/GetCDWorksList',
        datatype: "json",
        mtype: "POST",
        colNames: ['CD Works Length','Action'],
        colModel: [
                            { name: 'MAST_CDWORKS_NAME', index: 'MAST_CDWORKS_NAME', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager'),
        rowNum: 15,
      
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_CDWORKS_NAME',
        sortorder: "asc",
        caption: "CD Works Length List",
        height: 'auto',
      
        autowidth:true,
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

    $("#btnAddNew").click(function (e) {
        if (!$("#cdWorksDetails").is(":visible")) {
            $('#cdWorksDetails').show();
            $('#cdWorksDetails').load("/Master/AddCdWorksType");
        }
    });


});

function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit CD Works Length Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete CD Works Length Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

}



function editData(urlparameter) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditCdWorksType/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#cdWorksDetails").show();
            $("#cdWorksDetails").html(data);
            $("#MAST_CDWORKS_NAME").focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete CD Works Length details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteCdWorksType/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //  $("#cdWorksType").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $("#cdWorksDetails").hide('slow');
                    $('#cdWorksType').trigger('reloadGrid');
             
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
















