$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddLokSabhaTerm');
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
$('#btnCreateNew').click(function () {

        if (!$('#loksabhaDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#loksabhaDetails").load("/Master/AddLokSabhaTerm", function () {
                $("#btnCreateNew").hide('slow');
                $('#loksabhaDetails').show();
                $.unblockUI();
            });
        }
    });


    jQuery("#loksabhaCategory").jqGrid({
        url: '/Master/GetLokSabhaTermList',
        datatype: "json",
        mtype: "POST",
        postData: { desigCode: ($("#MAST_DESIG_TYPE").val()) },
        colNames: ['Lok Sabha Term','Start Date', 'End Date', 'Action'],
        colModel: [
                            { name: 'MAST_LS_TERM', index: 'MAST_LS_TERM', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_LS_START_DATE', index: 'MAST_LS_START_DATE', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'MAST_LS_END_DATE', index: 'MAST_LS_END_DATE', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager'),
        rowNum: 15,
        sortname: 'MAST_LS_TERM',
        sortorder: "asc",
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Lok Sabha Term List",
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


});

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Lok Sabha Term Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Lok Sabha Term Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}
function editData(urlparameter) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditLokSabhaTerm/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            
            $("#loksabhaDetails").html(data);
            $("#loksabhaDetails").show();
            $("#MAST_LS_START_DATE").focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete Lok Sabha Term details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteLokSabhaTerm/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    // $("#loksabhaCategory").trigger('reloadGrid');
                    $("#btnCreateNew").show();
                    $('#loksabhaDetails').hide();
                    $('#loksabhaCategory').trigger('reloadGrid');
               
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