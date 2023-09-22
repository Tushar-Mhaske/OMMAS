 $(document).ready(function () {

     $.validator.unobtrusive.parse('#frmAddCdWorksConstruction');

     
     $('#btnCreateNew').click(function () {

         if (!$('#constructionDetails').is(':visible')) {

             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

             $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

             $("#constructionDetails").load("/Master/AddConstructionType", function () {
                 $("#btnCreateNew").hide('slow');
                 $('#constructionDetails').show();
                 $.unblockUI();
             });
         }
     });
   jQuery("#constructionCategory").jqGrid({
        url: '/Master/GetConstructionTypeList',
        datatype: "json",
        mtype: "POST",
        colNames: ['CD Works Type', 'Action'],
        colModel: [
                            { name: 'MAST_CDWORKS_NAME', index: 'MAST_CDWORKS_NAME', height: 'auto', width: 250, align: "left", sortable: true },
                            { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager'),
        rowNum: 15,
     
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_CDWORKS_NAME',
        sortorder: "asc",
        caption: "CD Works Type List",
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
        if (!$("#constructionDetails").is(":visible")) {
            $('#constructionDetails').show();
            $('#constructionDetails').load("/Master/AddConstructionType");
        }
    }); 
});
 function FormatColumn(cellvalue, options, rowObject) {
     return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit CD Works Type Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete CD Works Type Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
 }




 function editData(urlparameter) {
     $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditConstructionType/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#constructionDetails").html(data);
            $("#constructionDetails").show(); 
            $('#MAST_CDWORKS_NAME').focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete CD Works Type details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteConstructionType/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#constructionCategory").trigger('reloadGrid');
                    $("#btnCreateNew").show('slow');
                    $('#constructionDetails').hide('slow');
                    $('#constructionCategory').trigger('reloadGrid');
                  
                    
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


































