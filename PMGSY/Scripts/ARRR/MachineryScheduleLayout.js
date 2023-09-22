$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmMachinerySchedule');

    $("#idFilterDivMac").click(function () {
        $("#idFilterDivMac").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmMachinerySchedule").toggle("slow");
    });

    //$('#ddlCategoryMac').change(function () {
    //    //alert(1);
    //    LoadLMMScheduleGrid();
    //});

    $('#ddlCategoryMac').change(function () {
        $("#ddlItemMac").empty();
        $.ajax({
            url: '/ARRR/LMMScheduleDetails/',
            type: 'POST',
            data: { lmmCode: 2, category: $('#ddlCategoryMac').val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlItemMac").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        //LoadLMMScheduleGrid();
        //jQuery("#tblLoadScheduleLMMItemsList").trigger("reloadGrid")
    });

    //alert($('#ddlCategoryMac').val());
    LoadMachineryScheduleGrid();

    $("#btnSaveMachinery").click(function () {
        //alert($('#ItemCode').val());
        if ($('#ddlMinorItem option:selected').val() > 0) {
            //alert('Mn');
            $('#hdnItemCodeMachinery').val($('#ddlMinorItem').val());
        }
        else if ($('#ddlMajorItem option:selected').val() > 0) {
            //alert('Mj');
            $('#hdnItemCodeMachinery').val($('#ddlMajorItem').val());
        }
        else {
            //alert('I');
            $('#hdnItemCodeMachinery').val($('#hdnItemCodeMc').val());
        }
        if ($('#frmMachinerySchedule').valid()) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddScheduleMachineryDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmMachinerySchedule").serialize(),
                //contentType: false,
                //processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnResetMachinery").trigger('click');
                        LoadLMMScheduleGrid();
                        //$('#dvLoadMaterialRate').hide('slow');
                        //$("#btnAdd").show('slow');
                    }
                }
            })
        }
    });

    $("#btnResetMachinery").click(function () {
        //LoadMachineryScheduleGrid();
    });

});

function LoadMachineryScheduleGrid() {
    //if ($("#frmMachinerySchedule").valid()) {
    $("#tblLoadScheduleMachineryList").jqGrid('GridUnload');

    jQuery("#tblLoadScheduleMachineryList").jqGrid({
        url: '/ARRR/GetScheduleMachineryList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Type', 'Item', 'Quantity', 'Save', 'Edit', 'Delete'], //26
        colModel: [
                    { name: 'Type', index: 'Type', height: 'auto', width: 150, align: "left", sortable: true, editable: false },
                    { name: 'Item', index: 'Item', height: 'auto', width: 150, align: "center", sortable: true, editable: false },
                    { name: 'Quantity', index: 'Quantity', height: 'auto', width: 150, align: "center", sortable: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateScheduleMachinery } },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", hidden: true, editable: false },
                    { name: 'Edit', index: 'Edit', width: 80, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 80, sortable: false, align: "center" },
        ],
        //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
        pager: jQuery('#dvLoadScheduleMachineryListpager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Type',
        sortorder: "asc",
        caption: "Schedule Machinery List",
        height: 'auto',
        autowidth: true,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        //cmTemplate: { title: false },
        editurl: "/ARRR/UpdateScheduleMachinery",
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
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit' onClick ='EditScheduleMachinery(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete' onClick ='delScheduleLMM(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked'></span></td></tr></table></center>";
    }
}


function EditScheduleMachinery(paramFileID) {
    //alert(paramFileID);

    jQuery("#tblLoadScheduleMachineryList").editRow(paramFileID);
    $('#tblLoadScheduleMachineryList').jqGrid('showCol', 'Save');
}

function SaveScheduleMachinery(paramFileID) {
    //alert("Save");
    jQuery("#tblLoadScheduleMachineryList").saveRow(paramFileID, checksave);
    LoadMachineryScheduleGrid();
}

function CancelScheduleMachinery(paramFileID) {
    //alert("Cancel Save");
    $('#tblLoadScheduleMachineryList').jqGrid('hideCol', 'Save');
    jQuery("#tblLoadScheduleMachineryList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tblLoadScheduleMachineryList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateScheduleMachinery(value, colname) {

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

function delScheduleMachinery(urlparameter) {
    if (confirm("Are you sure you want to delete Schedule Machinery details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delScheduleMachinery/' + urlparameter,
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