$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddSoilType');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#soilDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#soilDetails").load("/Master/AddSoilType", function () {
                $("#btnCreateNew").hide('slow');
                $('#soilDetails').show('slow');
                $.unblockUI();
            });
        }
    });


    jQuery("#soilCategory").jqGrid({
        url: '/Master/GetSoilTypeList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Soil Type Name', 'Action'],
        colModel: [
                            { name: 'MAST_SOIL_TYPE_NAME', index: 'MAST_SOIL_TYPE_NAME', height: 'auto', width: 250, align: "left", sortable: true },
                            { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager'),
        rowNum: 15,
       
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'SoilTypeName',
        sortorder: "asc",
        caption: "Soil Type List",
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
        if (!$("#soilDetails").is(":visible")) {
            $('#soilDetails').show();
            $('#soilDetails').load("/Master/AddSoilType");
        }
    });

});

function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit  Soil Type Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Soil Type Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(urlparameter) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditSoilType/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#soilDetails").show();
            $("#soilDetails").html(data);
            $('#MAST_SOIL_TYPE_NAME').focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete Soil Type details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteSoilType/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);                 
                    // $("#soilCategory").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#soilDetails').hide('slow');
                    $('#soilCategory').trigger('reloadGrid');
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

























