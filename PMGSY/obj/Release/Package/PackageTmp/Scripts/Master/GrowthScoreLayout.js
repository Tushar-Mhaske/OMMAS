$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    LoadGrid();
    $.unblockUI();

    $('#btnAdd').click(function (e) {

        if (!$("#dvGrowthScoreDetails").is(":visible")) {
            $("#dvGrowthScoreDetails").load("/Master/AddEditMasterGrowthScoreType/");

            //$.ajax({
            //    url: "/Master/AddEditMasterGrowthScoreType/",
            //    type: "GET",
            //    async: false,
            //    dataType: "html",
            //    catche: false,
            //    contentType: "application/json; charset=utf-8",
            //    success: function (data) {

            //        $("#dvGrowthScoreDetails").show();
            //        $("#dvGrowthScoreDetails").html(data);

            //        alert($('#txtScoreVal').text());

            //        //$.unblockUI();

            //    },
            //});


            $('#dvGrowthScoreDetails').show('slow');
            //alert($('#txtScoreVal').val());
            $('#txtScoreVal').val("");
            $('#btnAdd').hide();
            $('#btnView').show();
        }

    });
    $('#btnView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvGrowthScoreDetails").is(":visible")) {
            $('#dvGrowthScoreDetails').hide('slow');
        }

        $("#GrowthScoreSubItems").html('');
        $('#tblMasterGrowthScoreList').trigger('reloadGrid');
        $("#tblMasterGrowthScoreList").jqGrid('setGridState', 'visible');
        $('#btnView').hide();
        $('#btnAdd').show();

        $.unblockUI();
    });


});


function FormatColumn(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;'  class='ui-icon ui-icon-pencil' title='Edit Score Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash' title='Delete Score Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function editData(id) {

    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterGrowthScoreType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $("#dvGrowthScoreDetails").show();
            $("#dvGrowthScoreDetails").html(data);

            //$.unblockUI();

            $("#GrowthScoreSubItems").html('');
            $("#dvItemBody").show('slow');
            //$("#tblMasterGrowthScoreList").jqGrid('setGridState', 'visible');
            $('#btnView').hide();
            $('#btnAdd').show();
            $('#tblGrowthScoreSubItemList').trigger('reloadGrid');
        },
        error: function (xht, ajaxOptions, throwError) {
            if ($("#dvGrowthScoreDetails").is(":visible")) {
                $('#dvGrowthScoreDetails').hide('slow');
            }
            alert(xht.responseText);
            //$.unblockUI();
        }
    });
}
function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete Score details and Sub Item details?")) {
        $.ajax({
            url: "/Master/DeleteMasterGrowthScore/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    $('#tblMasterGrowthScoreList').trigger('reloadGrid');
                    $("#dvGrowthScoreDetails").hide('slow');
                    $('#btnView').hide();
                    $('#btnAdd').show();

                    $("#GrowthScoreSubItems").html('');
                    $("#dvItemBody").show('slow');
                    //$("#tblMasterGrowthScoreList").jqGrid('setGridState', 'visible');
                    $('#btnView').hide();
                    $('#btnAdd').show();
                    $('#tblGrowthScoreSubItemList').trigger('reloadGrid');
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
function LoadGrid() {

    $("#tblMasterGrowthScoreList").jqGrid('GridUnload');
    $('#tblMasterGrowthScoreList').jqGrid({
        url: '/Master/GetMasterGrowthScoreList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Item Description', 'Max Score', 'Score Type', 'Action', 'Sub Item', 'Parent Id'],
        colModel: [
             { name: 'MAST_SCORE_NAME', index: 'MAST_SCORE_NAME', height: 'auto', width: 200, align: "left", sortable: true },
             { name: 'MAST_SCORE_VALUE', index: 'MAST_SCORE_VALUE', height: 'auto', width: 180, align: "left", sortable: true },
             { name: 'MAST_SCORE_TYPE', index: 'MAST_SCORE_TYPE', height: 'auto', width: 180, align: "left", sortable: true },
             { name: 'a', width: 85, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
             { name: 'a', width: 85, sortable: false, resize: false, align: "center", sortable: false },
             { name: 'MAST_PARENT_ID', index: 'MAST_PARENT_ID', height: 'auto', hidden: "true", align: "left", sortable: true }
        ],
        pager: jQuery('#divPagerMasterScoreType'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_SCORE_NAME',
        sortorder: "asc",
        caption: 'Score Item List',
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


function DisplayScoreDetails(id) {
    $("#dvGrowthScoreDetails").hide('slow');
    $("#dvItemBody").hide('slow');
    $('#btnAdd').hide();
    $('#btnView').show();



    $("#GrowthScoreSubItems").load('/Master/GrowthScoreSubItemLayout/' + id);
    //$("#dvGrowthScoreSubItemInfo").load('/Master/GrowthScoreSubItemInfo/' + id);
    $.ajax({
        url: "/Master/GrowthScoreSubItemInfo/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            //alert(1);
            $("#dvGrowthScoreSubItemInfo").html('');
            $("#dvGrowthScoreSubItemInfo").show();
            $("#dvGrowthScoreSubItemInfo").html(data);

            //$.unblockUI();

        },
    });

    $("#tblMasterGrowthScoreList").jqGrid('setGridState', 'hidden');

};

$("#spnCollapseIcon").click(function () {

    $("#spnCollapseIcon").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    $("#dvItemBody").toggle('slow');
    $("#tblMasterGrowthScoreList").jqGrid('setGridState', 'visible');

});