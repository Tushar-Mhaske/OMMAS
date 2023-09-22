$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadChapterItems');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnAdd").click(function () {
        $.ajax({
            url: '/ARRR/ChapterItemsLayout/',
            type: 'POST',
            //data: { useraction: "A" },
            success: function (jsonData) {

                $('#dvLoadChapterItems').html('');
                $('#dvLoadChapterItems').html(jsonData);
                $('#dvLoadChapterItems').show('slow');
                $("#btnAdd").hide('slow');
                $('input[id=rdbItem]').attr("enabled", true);
                $('input[id=rdbMajorItem]').attr("enabled", true);
                $('input[id=rdbMinorItem]').attr("enabled", true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });

    });

    LoadChapterGrid();
});

function LoadChapterGrid() {
    if ($("#frmLoadChapterItems").valid()) {
        $("#tblLoadChapterItemsList").jqGrid('GridUnload');
        var pageWidth = jQuery('#tblLoadChapterItemsList').parent().width() - 100;
        //alert(pageWidth);
        jQuery("#tblLoadChapterItemsList").jqGrid({
            url: '/ARRR/GetChapterItemsList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Chapter', 'Name', 'Description', 'Item Type', 'Item code', 'Major Item code', 'Minor Item code', 'User Code', 'Mord Reference', 'Active Flag', 'Edit', 'Delete'], //26
            colModel: [
                        { name: 'Chapter', index: 'Chapter', height: 'auto', width: (pageWidth * (22 / 100)), align: "left", sortable: true },
                        { name: 'Name', index: 'Name', height: 'auto', width: (pageWidth * (28 / 100)), align: "left", sortable: true },
                        { name: 'Desc', index: 'Desc', height: 'auto', width: (pageWidth * (28 / 100)), align: "left", sortable: true },
                        { name: 'ItemType', index: 'Item Type', height: 'auto', width: (pageWidth * (12 / 100)), align: "center", sortable: true },
                        { name: 'ItemCode', index: 'Item Code', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: true , hidden:true },
                        { name: 'MajorItemCode', index: 'Major Item code', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: true , hidden: true },
                        { name: 'MinorItemCode', index: 'Minor Item code', height: 'auto', width: (pageWidth * (7/ 100)), align: "center", sortable: true , hidden: true },
                        { name: 'UserCode', index: 'User Code', height: 'auto', width: (pageWidth * (12 / 100)), align: "center", sortable: false },
                        { name: 'MordReference', index: 'Mord Reference', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: false },
                        { name: 'Flag', index: 'Flag', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: true },
                        { name: 'Edit', index: 'Add', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: false },
                        { name: 'Delete', index: 'Add', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", sortable: false },
            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            pager: jQuery('#dvLoadChapterItemsListpager'),
            rowNum: 20,
            rowList: [10, 20, 30, 50,100],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ITEM_CODE',
            sortorder: "asc",
            caption: "Chapter Item List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: true,
            rownumbers: true,
            grouping: true,
            groupingView: {
                groupField: ['Chapter', 'ItemCode', 'MajorItemCode', 'MinorItemCode'],
                groupColumnShow: [false,false,false,false],
                groupSummary: [false],
                groupText: ['<b>Chapter : {0}</b>', '<b>Item : {0}</b>', '<b>Major Item : {0}</b>', '<b> Minor Item : {0}</b>'],
                groupCollapse: true,
                groupOrder: ['asc', 'asc', 'asc', 'asc'],
                //showSummaryOnHide: true
            },
            cmTemplate: { title: false },
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")
                }
            },
        });
    }
}

function loadEditChapterDetails(param) {
    //alert(param);
    //$('#tblConditionMasterList').jqGrid('setGridState', 'hidden');

    $("#btnAdd").hide('slow');
    $.ajax({
        url: '/ARRR/ChapterItemsLayout/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadChapterItems').html('');
            $('#dvLoadChapterItems').html(jsonData);
            $('#dvLoadChapterItems').show('slow');
            if ($('#hdnItemType').val() == 'I') {
                $("#rdbItem").trigger('click')
            }

            if ($('#hdnItemType').val() == 'M') {
                $("#rdbMajorItem").trigger('click')
            }

            if ($('#hdnItemType').val() == 'N') {
                $("#rdbMinorItem").trigger('click')
            }

            $('input[id=rdbItem]').attr("disabled", true);
            $('input[id=rdbMajorItem]').attr("disabled", true);
            $('input[id=rdbMinorItem]').attr("disabled", true);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteChapterDetails(urlparameter) {
    if (confirm("Are you sure you want to delete Chapter Item details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delChapterDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadChapterGrid();
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

function changeChapterstatus(urlparameter) {
    //alert(urlparameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/ARRR/changeChapterstatus/' + urlparameter,
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $("#btnCancel").trigger('click');
                LoadChapterGrid();
                $.unblockUI();
            }
            else {
                alert(data.message);
                $.unblockUI();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}
