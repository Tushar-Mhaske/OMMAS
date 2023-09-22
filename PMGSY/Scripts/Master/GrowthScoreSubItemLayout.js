$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    LoadGrid(parseInt($('#hdnScoreCode').val()));
    $.unblockUI();

    $('#btnAddSubItem').click(function (e) {

        $("#dvGrowthScoreSubItemDetails").html('');

        $("#dvGrowthScoreSubItemDetails").load("/Master/GrowthScoreSubItemDetails?scoreId=" + parseInt($('#hdnScoreCode').val()));
        $('#dvGrowthScoreSubItemDetails').show('slow');

        $('#btnAddSubItem').hide();
        $('#btnViewSubItem').show();

    });
    $('#btnViewSubItem').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvGrowthScoreSubItemDetails").is(":visible")) {
            $('#dvGrowthScoreSubItemDetails').hide('slow');

            $('#btnViewSubItem').hide();
            $('#btnAddSubItem').show();
        }

        $.unblockUI();
    });


});


function FormatColumn(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;'  class='ui-icon ui-icon-pencil' title='Edit Score Sub Item Details' onClick ='editSubItemData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash' title='Delete Score Sub Item Details' onClick ='deleteSubItemData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function editSubItemData(id) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditScoreSubItem/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $("#dvGrowthScoreSubItemDetails").show();
            $("#dvGrowthScoreSubItemDetails").html(data);

            //$.unblockUI();

        },
        error: function (xht, ajaxOptions, throwError) {
            if ($("#dvGrowthScoreSubItemDetails").is(":visible")) {
                $('#dvGrowthScoreSubItemDetails').hide('slow');
            }
            alert(xht.responseText);
            //$.unblockUI();
        }
    });
}
function deleteSubItemData(urlParam) {
    if (confirm("Are you sure you want to delete Score Sub Item details?")) {
        $.ajax({
            url: "/Master/DeleteScoreSubItemDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    $('#tblGrowthScoreSubItemList').trigger('reloadGrid');
                    $("#dvGrowthScoreSubItemDetails").hide('slow');
                    $('#btnViewSubItem').hide();
                    $('#btnAddSubItem').show();
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
function LoadGrid(scoreId) {
    $('#tblGrowthScoreSubItemList').jqGrid({
        url: '/Master/GetMasterGrowthScoreSubItemList?scoreId=' + scoreId,
        datatype: 'json',
        mtype: "POST",
        colNames: ['Item Description', 'Score', 'Action', 'Parent Id'],
        colModel: [
            { name: 'MAST_SCORE_NAME', index: 'MAST_SCORE_NAME', height: 'auto', width: 180, align: "left", sortable: true },
            { name: 'MAST_SCORE_VALUE', index: 'MAST_SCORE_VALUE', height: 'auto', width: 180, align: "left", sortable: true },
         { name: 'a', width: 180, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
         { name: 'MAST_PARENT_ID', index: 'MAST_PARENT_ID', height: 'auto', hidden: "true", align: "left", sortable: true }
        ],
        pager: jQuery('#divPagerGrowthScoreSubItemList'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_SCORE_NAME',
        sortorder: "asc",
        caption: 'Score Sub Item List',
        height: '100%',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
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

function closeSubItemDetails() {
    $("#GrowthScoreSubItems").html('');
    $("#dvItemBody").show('slow');
    $('#tblMasterGrowthScoreList').trigger('reloadGrid');
    $("#tblMasterGrowthScoreList").jqGrid('setGridState', 'visible');
    $('#btnView').hide();
    $('#btnAdd').show();
}