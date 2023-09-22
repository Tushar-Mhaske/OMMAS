var itemtypeCode;
var curTabIndex = 0;
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadScheduleRates');

    //$('#dvTabs a').click(function (e) {
    //    var curTab = $('.ui-tabs-active');
    //    curTabIndex = curTab.index();
    //    //document.tabTest.currentTab.value = curTabIndex;
    //});

    loadScheduleItems1();
    LoadLMMScheduleGrid();

    $("#idFilterDivI").click(function () {
        $("#idFilterDivI").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmLoadScheduleRates").toggle("slow");
    });


    $("#ddlMajorItem").change(function () {
        itemtypeCode = $("#ddlMajorItem").val();
    });

    $("#ddlMinorItem").change(function () {
        itemtypeCode = $("#ddlMinorItem").val();
    });
    $("#ddlItem").change(function () {

        $("#ddlMajorItem").empty();

        $.ajax({
            url: '/ARRR/MajorItemDetails/',
            type: 'POST',
            data: { "ItemCode": $("#ddlItem").val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlMajorItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    $("#ddlMajorItem").change(function () {

        $("#ddlMinorItem").empty();

        $.ajax({
            url: '/ARRR/PopulateMinorItemsList/',
            type: 'POST',
            data: { "ItemCode": $("#ddlMajorItem").val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlMinorItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

});

function loadScheduleItems1() {
    //alert($("#ItemCode").val());
    //alert($('#ItemTypeCode').val());
    $("#accordion").show('slow');
    $("#dvTabs").tabs();
    $("#dvTabs").tabs({ active: 0 });
    $("#dvTabs").show();

    $.ajax({
        url: '/ARRR/LabourScheduleLayout/' + $("#ItemTypeCode").val(),
        async: false,
        cache: false,
        type: "GET",
        data: { typeCode: $('#ItemTypeCode').val() },
        dataType: "html",
        success: function (data) {
            $('#tbLabour').html(data);
            $('#tbLabour').show('fade');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}



$(function () {

    $("#dvTabs").tabs({
        active: 0,
        beforeActivate: function (event, ui) {
            switch (ui.newTab.index()) {
                case 0:
                    curTabIndex = 0;
                    //alert("1");
                    //event.preventDefault();
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/ARRR/LabourScheduleLayout/' + $("#ItemTypeCode").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        data: { typeCode: $('#ItemTypeCode').val() },
                        dataType: "html",
                        success: function (data) {
                            $('#tbLabour').html(data);
                            $('#tbLabour').show('fade');
                            LoadLMMScheduleGrid();
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 1:
                    curTabIndex = 1;
                    //alert("3");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/ARRR/MachineryScheduleLayout/' + $("#ItemTypeCode").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        data: { typeCode: $('#ItemTypeCode').val() },
                        dataType: "html",
                        success: function (data) {
                            //$('#frmFBFilesDisplay').html('');
                            $('#tbMachinery').html(data);
                            $('#tbMachinery').show('fade');
                            LoadLMMScheduleGrid();
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 2:
                    curTabIndex = 2;
                    //alert("2");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/ARRR/MaterialScheduleLayout/' + $("#ItemTypeCode").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        data: { typeCode: $('#ItemTypeCode').val() },
                        dataType: "html",
                        success: function (data) {
                            $('#tbMaterial').html(data);
                            $('#tbMaterial').show('fade');
                            LoadLMMScheduleGrid();
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 3:
                    curTabIndex = 3;
                    //default:
                    //alert("default");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/ARRR/TaxScheduleLayout/' + $("#ItemTypeCode").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        data: { typeCode: $('#ItemTypeCode').val() },
                        dataType: "html",
                        success: function (data) {
                            $('#tbOther').html(data);
                            $('#tbOther').show('fade');
                            LoadTaxScheduleGrid();
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 4:
                    curTabIndex = 4;
                    //alert("4");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/ARRR/OutputScheduleLayout/' + $("#ItemTypeCode").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        data: { typeCode: $('#ItemTypeCode').val() },
                        dataType: "html",
                        success: function (data) {
                            $('#tbOutput').html(data);
                            $('#tbOutput').show('fade');
                            LoadTaxScheduleGrid();
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
            }
        },
        create: function (event, ui) {
            $('#dvTabs .ui-helper-reset').css("line-height", "1");
        },
    });
    $("#dvTabs").tabs().removeClass("ui-widget");
});

function LoadLMMScheduleGrid() {
    //if ($("#frmLabourSchedule").valid()) {
    $("#tblLoadScheduleLMMItemsList").jqGrid('GridUnload');
    $("#tblLoadScheduleTaxItemsList").jqGrid('GridUnload');

    var chapter;
    if (curTabIndex == 2) {
        chapter = $('#ddlCategoryMat').val();
    }
    else if (curTabIndex == 1) {
        chapter = $('#ddlCategoryMac').val();
    }
    else if (curTabIndex == 0) {
        chapter = $('#ddlCategory').val();
    }
    else {
        chapter = "-1";
    }

    //alert(chapter);
    //alert($("#dvTabs").tabs('option', 'selected'));

    jQuery("#tblLoadScheduleLMMItemsList").jqGrid({
        url: '/ARRR/GetScheduleLMMList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Type', 'Item', 'Quantity', 'LMM', 'Save', 'Edit', 'Delete'], //26
        colModel: [
                    { name: 'Type', index: 'Type', height: 'auto', width: 300, align: "left", sortable: true, editable: false },
                    { name: 'Item', index: 'Item', height: 'auto', width: 300, align: "center", sortable: true, editable: false },
                    { name: 'Quantity', index: 'Quantity', height: 'auto', width: 300, align: "center", sortable: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateScheduleLabour } },
                    { name: 'LMMType', index: 'LMM Type', height: 'auto', width: 300, align: "left", sortable: true, editable: false },
                    { name: 'Save', index: 'Save', width: 300, sortable: false, align: "center", hidden: true, editable: false },
                    { name: 'Edit', index: 'Edit', width: 300, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 270, sortable: false, align: "center" },
                    //{ name: 'Action', index: 'Action', height: 'auto', width: 130, align: "center", sortable: true, formatter: FormatColumn, editable: false },
        ],
        //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
        postData: { itemCode: $('#ItemTypeCode').val(), chapter: chapter },
        pager: jQuery('#dvLoadScheduleLMMItemspager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Type',
        sortorder: "asc",
        caption: "Schedule List",
        height: 'auto',
        autowidth: true,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ['Item', 'LMMType'],
            groupColumnShow: [false, false],
            groupSummary: [false],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupCollapse: false,
            groupOrder: ['asc', 'asc'],
            //showSummaryOnHide: true
        },
        //cmTemplate: { title: false },
        editurl: "/ARRR/UpdateScheduleLMM",
        loadComplete: function () {
            //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
            //unblockPage();
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
    //}
}

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit' onClick ='EditScheduleLMM(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete' onClick ='delScheduleLMM(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked'></span></td></tr></table></center>";
    }
}


function EditScheduleLMM(paramFileID) {
    //alert(paramFileID);

    jQuery("#tblLoadScheduleLMMItemsList").editRow(paramFileID);
    $('#tblLoadScheduleLMMItemsList').jqGrid('showCol', 'Save');
}

function SaveScheduleLMM(paramFileID) {
    //alert("Save");
    jQuery("#tblLoadScheduleLMMItemsList").saveRow(paramFileID, checksave);
    LoadLMMScheduleGrid();
}

function CancelScheduleLMM(paramFileID) {
    //alert("Cancel Save");
    $('#tblLoadScheduleLMMItemsList').jqGrid('hideCol', 'Save');
    jQuery("#tblLoadScheduleLMMItemsList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tblLoadScheduleLMMItemsList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateScheduleLabour(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter Quantity."];
    }
    else if ((/^[0-9.]+$/).test(value)) {
        return ["Invalid Quantity,Only decimal Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function delScheduleLMM(urlparameter) {
    if (confirm("Are you sure you want to delete Schedule details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delScheduleLMM/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //$("#btnCancel").trigger('click');
                    LoadLMMScheduleGrid();
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



function LoadTaxScheduleGrid() {
    //if ($("#frmLabourSchedule").valid()) {
    $("#tblLoadScheduleLMMItemsList").jqGrid('GridUnload');
    $("#tblLoadScheduleTaxItemsList").jqGrid('GridUnload');

    jQuery("#tblLoadScheduleTaxItemsList").jqGrid({
        url: '/ARRR/GetScheduleTaxList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Tax', 'Item', 'Rate', 'Flag', 'Save', 'Edit', 'Delete'], //26
        colModel: [
                    { name: 'Tax', index: 'Tax', height: 'auto', width: 250, align: "center", sortable: true, editable: false },
                    { name: 'Item', index: 'Item', height: 'auto', width: 250, align: "center", sortable: true, editable: false },
                    { name: 'Rate', index: 'Rate', height: 'auto', width: 250, align: "center", sortable: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateScheduleLabour } },
                    { name: 'Flag', index: 'Flag', height: 'auto', width: 250, align: "left", sortable: true, editable: false },
                    { name: 'Save', index: 'Save', width: 200, sortable: false, align: "center", hidden: true, editable: false },
                    { name: 'Edit', index: 'Edit', width: 200, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 200, sortable: false, align: "center" },
                    //{ name: 'Action', index: 'Action', height: 'auto', width: 130, align: "center", sortable: true, formatter: FormatColumn, editable: false },
        ],
        //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
        postData: { itemCode: $('#ItemTypeCode').val() },
        pager: jQuery('#dvLoadScheduleTaxItemspager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Type',
        sortorder: "asc",
        caption: "Tax List",
        height: 'auto',
        autowidth: true,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        //cmTemplate: { title: false },
        editurl: "/ARRR/UpdateScheduleTax",
        loadComplete: function () {
            //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
            //unblockPage();
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
    //}
}



function EditScheduleTax(paramFileID) {
    //alert(paramFileID);

    jQuery("#tblLoadScheduleTaxItemsList").editRow(paramFileID);
    $('#tblLoadScheduleTaxItemsList').jqGrid('showCol', 'Save');
}

function SaveScheduleTax(paramFileID) {
    //alert("Save");
    jQuery("#tblLoadScheduleTaxItemsList").saveRow(paramFileID, checksaveTax);
    LoadTaxScheduleGrid();
}

function CancelScheduleTax(paramFileID) {
    //alert("Cancel Save");
    $('#tblLoadScheduleTaxItemsList').jqGrid('hideCol', 'Save');
    jQuery("#tblLoadScheduleTaxItemsList").restoreRow(paramFileID);
}

function checksaveTax(result) {
    $('#tblLoadScheduleTaxItemsList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateScheduleLabour(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter Rate."];
    }
    else if ((/^[0-9.]+$/).test(value)) {
        return ["Invalid Rate,Only decimal Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}


function delScheduleTax(urlparameter) {
    if (confirm("Are you sure you want to delete Schedule Tax details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delScheduleTax/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //$("#btnCancel").trigger('click');
                    LoadTaxScheduleGrid();
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