$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLabourRate');

    $("#idFilterDivL").click(function () {
        $("#idFilterDivL").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmLabourRate").toggle("slow");
    });

    $('#ddlCategory').change(function () {
        $("#ddlItem").empty();
        $.ajax({
            url: '/ARRR/LabourRateDetails/',
            type: 'POST',
            data: { category: $('#ddlCategory').val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    $('#ArrrDate, #Date').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        changeMonth: true, //for month selection
        changeYear: true, //for year selection
        buttonText: "select date",

        //minDate: $('#Date').val(),
        //maxDate: new Date(year, 11, 31),
        onSelect: function (selectedDate) {

        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $('#ArrrTillDate, #Date').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        changeMonth: true, //for month selection
        changeYear: true, //for year selection
        buttonText: "select date",

        minDate: $('#ArrrDate').val(),
        //maxDate: new Date(year, 11, 31),
        onSelect: function (selectedDate) {

        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmLabourRate").toggle("slow");
    });

    $("#btnSave").click(function () {

        debugger;
        var allData = [];
        var ids = jQuery("#tblLoadLabourFormList").jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var rowId = ids[i];
            var txt = getTextBoxValue(rowId);

            if (!!txt) {
                if (txt > 0) {
                    if (checkFileValidation(rowId)) {
                        if (checkFileExtension(rowId)) {
                            var file = getBrowsedFileValue(rowId);
                            SaveFile(rowId);
                        }
                        else {
                            return false;
                        }
                    }
                    var rowData = jQuery('#tblLoadLabourFormList').jqGrid('getRowData', rowId);
                    allData.push({
                        File: file,
                        Category: rowData.Category,
                        Labour: rowData.MAST_LMM_DESC,
                        Rate: txt,
                        Year: $("#ddlYear option:selected").val(),
                        Form: $('#frmLabourRate').serializeArray(),
                        Mast_Lmm_Code: rowData.ItemCode
                    });
                }
                else {
                    alert("Rate should be greater than 0");
                    return false;
                }
            }

        }
        //var recCount = $("#tblLoadLabourFormList").jqGrid("getGridParam", "reccount");
        //var arrCount = allData.length;
        if ($("#ddlYear option:selected").val() > 0/*$('#frmLabourRate').valid()*/) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddMultipleLabour/',
                async: false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(allData),
                //processData: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#btnCancel").trigger('click');
                        LoadLabourRateGrid();
                        $('#dvLoadLabourRate').hide('slow');
                        $("#dvLR").show('fast');  //$("#btnAdd").show('slow');
                    }
                    else {
                        alert(data.message);
                    }
                }
            })
        }
        else {
            alert("Select a valid year");
            return false;
        }
    });

    $('#btnUpdate').click(function () {
        if ($('#frmLabourRate').valid()) {
            $('#User_Action').val('E');
            $.ajax({
                url: '/ARRR/AddEditLabourRateDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmLabourRate").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadLabourRateGrid();
                        $('#dvLoadLabourRate').hide('slow');
                        $("#dvLR").show('fast'); //$("#btnAdd").show('slow');
                    }
                }
            })
        }
    })

    $("#btnCancel").click(function () {
        $('#dvLoadLabourRate').hide('slow');
        $("#dvLR").show('fast');//$("#btnAdd").show('slow');
    });

    LoadLabourFormGrid();
});

function LoadLabourFormGrid() {
    //if ($("#frmLoadLabourRate").valid()) {
    $("#tblLoadLabourFormList").jqGrid('GridUnload');
    jQuery("#tblLoadLabourFormList").jqGrid({
        url: '/ARRR/GetLabourFormList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Item Code', 'Category', 'Labour Type', 'Unit', 'Upload File', 'Rate(Rs.)'],
        colModel: [
                    { name: 'ItemCode', index: 'ItemCode', height: 'auto', width: 100, align: "left", sortable: true },
                    { name: 'Category', index: 'Category', height: 'auto', width: 150, align: "left", sortable: true },
                    { name: 'MAST_LMM_DESC', index: 'MAST_LMM_DESC', height: 'auto', width: 150, align: "left", sortable: true },
                    { name: 'Unit', index: 'Unit', height: 'auto', width: 100, align: "left", sortable: true },
                    { name: 'Upload', index: 'Upload', height: 'auto', width: 200, align: "center", sortable: false, edittype: "file", editable: true, editoptions: { enctype: "multipart/form-data" }, formatter: generateBrowseFile },
                    { name: 'Rate', index: 'Rate', height: 'auto', width: 150, align: "center", sortable: true, editable: true, edittype: "text", formatter: generateTextBox },

        ],
        //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
        pager: jQuery('#dvLoadLabourFormListpager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30, 40, 50, 100, 200],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_ITEM_CODE',
        sortorder: "asc",
        caption: "Enter Rates Data",
        height: 'auto',
        autowidth: false,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        cmTemplate: { title: false },
        //editurl: "/ARRR/AddMultipleLabour",
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
    //}
}

function generateTextBox(cellvalue, options, rowObject) {
    return "<input type=\"text\" name =\"Orate\" size=\"10\" onkeypress = \"return isNumericKeyStroke(event)\" placeholder=\"Enter Rate\" maxlength=\"15\" id=\"textbox" + options.rowId + "\"/ required >";
}

function getTextBoxValue(SelectedId) {
    var txt = $("#textbox" + SelectedId).val();
    if (jQuery.trim(txt).length > 0) {
        return (txt);
    }
}

function isNumericKeyStroke(event) {
    var returnValue = false;
    var keyCode = (event.which) ? event.which : event.keyCode;
    //var rates = $("input[name='Orate']").val();
    //var val = parseFloat(rates);
    //if (parseFloat(val.toFixed(2)) != val) {
    //    alert(val);
    //    returnValue = false;
    //}
    if (((keyCode >= 48) && (keyCode <= 57)) || (keyCode == 46) || (keyCode == 8) || (keyCode == 9) || (keyCode == 37) || (keyCode == 39))// All numerics
    {
        returnValue = true;
    }
    if (event.returnValue)
        event.returnValue = returnValue;
    return returnValue;
}

function generateBrowseFile(cellvalue, options, rowObject) {
    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Upload File' ></span></td></tr></table></center>";
    return "<center><form id=\"frmFileUpload" + options.rowId + "\"/ enctype='multipart/form-data'><table><tr><td  style='border-color:white'><input type='file' title='Upload File' name='Upload File' id=\"browseFile" + options.rowId + "\"/ class='jqueryButton' multiple ></input></td></tr></table></form></center>";
}

function getBrowsedFileValue(SelectedId) {
    var file = $("#browseFile" + SelectedId).val().split('\\').pop();
    return (file);
}

function checkFileExtension(SelectedId) {

    var ext = $("#browseFile" + SelectedId).val().split('.').pop();
    if (ext.toLowerCase() != "pdf") {
        alert("only pdf file is allowed.");
        $("#browseFile").val('');
        return false;
    }
    return true;
}

function checkFileValidation(SelectedId) {
    var pdffile = $("#browseFile" + SelectedId).val();

    if (pdffile = "" || pdffile == undefined || pdffile.length == 0) {
        return false;
    }
    return true;
}

function SaveFile(SelectedId) {
    debugger;
    var formdata = new FormData($('#frmFileUpload' + SelectedId)[0]);
    //formdata.append('year', $("#ddlYear option:selected").val());
    //formdata.append('myfile', $('input[type=file]')[0].files[0]);
    $.ajax({
        url: '/ARRR/SavePDFfile',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (jsonData) {
            //if (jsonData.success)
            //alert(jsonData.message);
            //$("#browseFile" + SelectedId).val('');
        },
        error: function (err) {
            alert("Error occureed while processing your request.");
            $("#browseFile" + SelectedId).val('');
        }
    });
}

