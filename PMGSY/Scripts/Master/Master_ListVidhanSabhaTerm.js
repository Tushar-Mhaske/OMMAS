$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    $.ajax({
        url: "/Master/SearchVidhanSabhaTerm/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchVidhanSabha").html(data);
            $('#btnSearch').trigger('click');

        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }
    });

    
  

    $('#btnCreateNew').click(function (e) {
           
        if ($("#dvSearchVidhanSabha").is(":visible")) {
            $('#dvSearchVidhanSabha').hide('slow');
        }

        if (!$("#dvVidhanSabhaDetails").is(":visible")) {
            $("#dvVidhanSabhaDetails").load("/Master/AddEditMasterVidhanSabhaTerm/");
            $('#dvVidhanSabhaDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
    });


    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvVidhanSabhaDetails").is(":visible")) {
            $('#dvVidhanSabhaDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchVidhanSabha").is(":visible")) {

            $('#dvSearchVidhanSabha').load('/Master/SearchVidhanSabhaTerm', function () {

                $('#tblVidhanSabhaList').trigger('reloadGrid');

                var data = $('#tblVidhanSabhaList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlSearchStates').val(data.stateCode);
                }
                $('#dvSearchVidhanSabha').show('slow');
            });
        }
        $.unblockUI();
    });
});


function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;'><span class='ui-icon ui-icon-pencil' title='Edit Vidhan Sabha Term Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;'><span class='ui-icon ui-icon-trash' title='Delete Vidhan Sabha Term Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function editData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterVidhanSabhaTerm/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {        
            if ($("#dvSearchVidhanSabha").is(":visible")) {
                $('#dvSearchVidhanSabha').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
          
            $("#dvVidhanSabhaDetails").html(data);
            $("#dvVidhanSabhaDetails").show();
            $("#MAST_VS_START_DATE").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError)
        {
            alert(xht.responseText);
            $.unblockUI();
        }
    });//ajax end
}

function deleteData(urlParam)
{  
    if (confirm("Are you sure you want to delete Vidhan Sabha Term details?")) {
        $.ajax({
            url: "/Master/DeleteMasterVidhanSabhaTerm/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //if ($("#dvSearchVidhanSabha").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $('#tblVidhanSabhaList').trigger('reloadGrid');
                    //}
                    if ($("#dvVidhanSabhaDetails").is(":visible")) {
                        $('#dvVidhanSabhaDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }

                    if (!$("#dvSearchVidhanSabha").is(":visible")) {
                        $("#dvSearchVidhanSabha").show('slow');
                        $('#tblVidhanSabhaList').trigger('reloadGrid');
                    }
                    else {
                        $('#tblVidhanSabhaList').trigger('reloadGrid');
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


function LoadGrid()
{
    $('#tblVidhanSabhaList').jqGrid('GridUnload');
    $('#tblVidhanSabhaList').jqGrid({
        url: '/Master/GetMasterVidhanSabhaTermList/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Vidhan Sabha Term', 'State Name', 'Start Date', 'End Date', 'Action'],
        colModel: [
        { name: 'MAST_VS_TERM', index: 'MAST_VS_TERM', height: 'auto', width: 120, align: "left", sortable: true },
         { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 180, align: "left", sortable: true },
        { name: 'MAST_VS_START_DATE', index: 'MAST_VS_START_DATE', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'MAST_VS_END_DATE', index: 'MAST_VS_END_DATE', height: 'auto', width: 100, align: "left", sortable: true },
        { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { stateCode: $('#ddlSearchStates option:selected').val() },
        pager: jQuery('#divPagerVidhanSabha'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME,MAST_VS_TERM',
        sortorder: "asc",
        caption: 'Vidhan Sabha Term List',
        height: 'auto',
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
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });

}