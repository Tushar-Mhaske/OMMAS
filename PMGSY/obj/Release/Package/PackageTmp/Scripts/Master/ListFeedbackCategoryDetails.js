$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    LoadFeedbackDetailsList();
    
    $("#btnCreateNew").click(function () {
        if ($("#dvAddFeedbackDetails").is(':hidden')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#dvAddFeedbackDetails").load('/Master/AddEditFeedbackDetails', function () {
                $.unblockUI();
            });
            $("#dvAddFeedbackDetails").show('slow');
            $("#btnCreateNew").hide('slow');
        }
    });


});
function LoadFeedbackDetailsList()
{
    $('#tblFeedbackDetails').jqGrid({
        url: '/Master/FeedbackDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Name','Edit','Delete'],
        colModel: [
                      { name: 'MAST_FEED_NAME', index: 'MAST_FEED_NAME', height: 'auto', width: 300, align: "left", sortable: true },
                      { name: 'edit', width: 20, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                      { name: 'delete', width: 20, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false},                      
        ],
        pager: jQuery('#dvPagerFeedbackDetails'),
        rowNum: 05,
        rowList: [5,10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_FEED_NAME',
        sortorder: "asc",
        caption: "Feedback category Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#jqgh_tblFeedbackDetails_rn").html("Sr.<br/> No");
        }
    });

}
function FormatColumnEdit(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Feedback Details' onClick ='EditFeedbackDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Feedback Details' onClick ='DeleteFeedbackDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function EditFeedbackDetails(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/GetFeedbackDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddFeedbackDetails").html(data);
            $("#dvAddFeedbackDetails").show();
            $("#btnCreateNew").hide('slow');

            if (data.success == false)
            {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}


function DeleteFeedbackDetails(urlparameter) {
    if (confirm("Are you sure you want to delete feedback details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteFeedbackDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Feedback details deleted successfully.');
                    //$('#tblFeedbackDetails').trigger('reloadGrid');
                    //$("#dvAddFeedbackDetails").load('/Master/AddEditFeedbackDetails');
                    $("#dvAddFeedbackDetails").hide('slow');
                    $("#btnCreateNew").show('slow');
                    $('#tblFeedbackDetails').trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('Feedback details is in use and can not be deleted.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}