
$(document).ready(function () {
        

    //$('#btnCreateNew').click(function () {

    //    if (!$('#dvContClassDetails').is(':visible')) {

    //        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //        $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
    //        $("#dvContClassDetails").load("/Master/AddEditMasterContractorClassType", function () {
    //            $("#btnCreateNew").hide('slow');
    //            $('#dvContClassDetails').show('slow');
    //            $.unblockUI();
    //        });

    //    }
    //});
    
    $.ajax({
        url: "/Master/SearchContractorClassType/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchContrClass").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }

    });
    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchContrClass").is(":visible")) {
            $('#dvSearchContrClass').hide('slow');
        }

        if (!$("#dvContClassDetails").is(":visible")) {

            $("#dvContClassDetails").load("/Master/AddEditMasterContractorClassType/");

            $('#dvContClassDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();

        }

    });

    $('#btnSearchView').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvContClassDetails").is(":visible")) {
            $('#dvContClassDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchContrClass").is(":visible")) {

            $('#dvSearchContrClass').load('/Master/SearchContractorClassType/', function () {

                $('#tblMasterContClassTypeList').trigger('reloadGrid');

                var data = $('#tblMasterContClassTypeList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {

                    $('#StateList').val(data.StateCode);
                }
                $('#dvSearchContrClass').show('slow');

            });
        }
        $.unblockUI();
    });

  

});
function LoadContractorClassTypeList() {
    $('#tblMasterContClassTypeList').jqGrid({
        url: '/Master/GetMasterContractorClassTypeList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Class Name', 'State', 'Action'],
        colModel: [
         { name: 'MAST_CON_CLASS_TYPE_NAME', index: 'MAST_CON_CLASS_TYPE_NAME', height: 'auto', width: 200, align: "left", sortable: true },
          { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: true, align: "left" },
         { name: 'e', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }

        ],
        postData: { StateCode: $('#StateList option:selected').val() },
        pager: jQuery('#divPagerMasterContClassType'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_CON_CLASS_TYPE_NAME',
        sortorder: "asc",
        caption: 'Contractor Class List',
        height: 'auto',
        rownumbers: true,
        hidegrid: false,
        emptyrecords: 'No Records Found',
        autowidth: true,
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
function FormatColumn(cellvalue, options, rowObject) {   
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Contractor Class Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Contractor Class Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(id) {

    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterContractorClassType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            //$('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            //$("#dvContClassDetails").show();
            //$("#dvContClassDetails").html(data);
            //$('#MAST_CON_CLASS_TYPE_NAME').focus();
            if ($("#dvSearchContrClass").is(":visible")) {
                $('#dvSearchContrClass').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();

            $("#dvContClassDetails").html(data);
            $("#dvContClassDetails").show();
            $("#MAST_CON_CLASS_TYPE_NAME").focus();

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
    if (confirm("Are you sure you want to delete Contractor Class details?")) {
        $.ajax({
            url: "/Master/DeleteMasterContractorClassType/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#tblMasterContClassTypeList").trigger('reloadGrid');
                    if ($("#dvContClassDetails").is(":visible")) {
                        $('#dvContClassDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }

                    if (!$("#dvSearchContrClass").is(":visible")) {
                        $("#dvSearchContrClass").show('slow');
                        $("#tblMasterContClassTypeList").trigger('reloadGrid');
                    }
                    else {
                        $("#tblMasterContClassTypeList").trigger('reloadGrid');
                    }
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
