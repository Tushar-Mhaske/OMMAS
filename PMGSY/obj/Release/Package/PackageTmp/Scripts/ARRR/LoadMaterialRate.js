$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadMaterialRate');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnAdd").click(function () {
        $.ajax({
            url: '/ARRR/MaterialRateLayout/',
            type: 'POST',
            //data: { useraction: "A" },
            success: function (jsonData) {

                $('#dvLoadMaterialRate').html('');
                $('#dvLoadMaterialRate').html(jsonData);
                $('#dvLoadMaterialRate').show('slow');
                $("#dvLR").hide('slow'); // $("#btnAdd").hide('slow');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });

    });

    $('#btnView').click(function () {
        LoadMaterialRateGrid();
    });
    // LoadMaterialRateGrid();
});

function LoadMaterialRateGrid() {
    if ($("#frmLoadMaterialRate").valid()) {
        $("#tblLoadMaterialRateList").jqGrid('GridUnload');

        jQuery("#tblLoadMaterialRateList").jqGrid({
            url: '/ARRR/GetMaterialRateList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Category', 'Type', 'Output Unit', 'Rate(Rs.)', 'Year', 'Is Finalize?', 'Active', 'Edit', 'Delete', 'View PDF File'], //26
            colModel: [
                        { name: 'Category', index: 'Category', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'MAST_LMM_DESC', index: 'MAST_LMM_DESC', height: 'auto', width: 200, align: "left", sortable: true },
                       { name: 'Output Unit', index: 'Output Unit', height: 'auto', width: 100, align: "left", sortable: true },
                        { name: 'Rate', index: 'Rate', height: 'auto', width: 120, align: "center", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 120, align: "center", sortable: true },
                        { name: 'Finalize', index: 'Finalize', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'Active', index: 'Active', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'Delete', index: 'Delete', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'View File', index: 'View File', height: 'auto', width: 120, align: "center", sortable: false, formatter: AnchorFormatter },

            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            postData: { year: $('#ddlYearWise option:selected').val() },
            pager: jQuery('#dvLoadMaterialRateListpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30, 50, 100],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ITEM_CODE',
            sortorder: "asc",
            caption: "Material Rates Master List",
            height: 'auto',
            autowidth: false,
            //width:'250',
            shrinkToFit: true,
            rownumbers: true,
            cmTemplate: { title: false },
            loadComplete: function () {
                var ids = jQuery("#tblLoadMaterialRateList").jqGrid('getDataIDs');
                if (ids.length > 0) {
                    $('#dvLoadMaterialRateListpager_left').html("<input type='button' style='margin-left:5px' id='btnFinalize' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeAllMaterialDetails(" + $('#ddlYearWise option:selected').val() + ");return false;' value='Finalize All'/>");
                }
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

function loadEditMaterialRateDetails(param) {
    //alert(param);
    //$('#tblConditionMasterList').jqGrid('setGridState', 'hidden');

    $("#dvLR").hide('slow'); //$("#btnAdd").hide('slow');
    $.ajax({
        url: '/ARRR/MaterialRateLayout/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadMaterialRate').html('');
            $('#dvLoadMaterialRate').html(jsonData);
            $('#dvLoadMaterialRate').show('slow');
            $("#dvLR").hide('fast');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteMaterialRate(urlparameter) {
    if (confirm("Are you sure you want to delete Material Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delMaterialRateDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);

                    LoadMaterialRateGrid();
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


function FinalizeMaterialRate(urlparameter) {
    if (confirm("Are you sure you want to Finalize Material Rate?")) {
        $.ajax({
            url: "/ARRR/MaterialRateFinalization/" + urlparameter,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {

                //alert(data.status);
                if (data.status = true) {
                    alert("Material Rates Finalized Successfully");

                    LoadMaterialRateGrid();
                }
                else {
                    alert("Error occured while Finalizing News.");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function changeMaterialRatestatus(urlparameter) {
    //alert(urlparameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/ARRR/changeMaterialRatestatus/' + urlparameter,
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $("#btnCancel").trigger('click');
                LoadMaterialRateGrid();
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


function FinalizeAllMaterialDetails(urlparameter) {

    if (confirm("Are you sure you want to finalize all Material Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/FinalizeAllMaterialRates/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadMaterialRateGrid();
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

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/ARRR/DownloadMaterialPdfFile/" + cellvalue;
    return "<center><table><tr><td style='border:none'><a href='#'  onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:13px;width:13px' height='13' width='13' border=0 src='../../Content/images/PDF.ico' /></a></td></tr></table></center>";
}