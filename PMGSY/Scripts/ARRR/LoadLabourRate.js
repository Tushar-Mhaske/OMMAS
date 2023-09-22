$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadLabourRate');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnAdd").click(function () {
        $.ajax({
            url: '/ARRR/LabourRateLayout/',
            type: 'POST',
            //data: { useraction: "A" },
            success: function (jsonData) {

                $('#dvLoadLabourRate').html('');
                $('#dvLoadLabourRate').html(jsonData);
                $('#dvLoadLabourRate').show('slow');
                $("#dvLR").hide('slow'); // $("#btnAdd").hide('slow');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });

    });

    $('#btnView').click(function () {
        LoadLabourRateGrid();
    });

    $("#btnUpload").click(function () {
        $.ajax({
            url: '/ARRR/GetFileUploadLayout/',
            type: 'GET',
            async: false,
            cache: false,
            dataType: "html",
            //data: { useraction: "A" },
            success: function (data) {
                $('#dvFileUpload').html('');
                $('#dvFileUpload').html(data);
                $('#dvhdFileUpload').show('slow');
                $('#dvFileUpload').show('slow');

                $("#dvLR").hide('slow');
                $("dvLoadLabourRateList").hide('slow');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    //LoadLabourRateGrid();
});

function LoadLabourRateGrid() {
    if ($("#frmLoadLabourRate").valid()) {
        $("#tblLoadLabourRateList").jqGrid('GridUnload');
        jQuery("#tblLoadLabourRateList").jqGrid({
            url: '/ARRR/GetLabourRateList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Category', 'Type', 'Unit', 'Rate(Rs.)', 'Year', 'Is Finalize?', 'Active', 'Edit', 'Delete', 'View PDF File'], //26
            colModel: [
                        { name: 'Category', index: 'Category', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'MAST_LMM_DESC', index: 'MAST_LMM_DESC', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'Unit', index: 'Unit', height: 'auto', width: 120, align: "left", sortable: true },
                        { name: 'Rate', index: 'Rate', height: 'auto', width: 120, align: "center", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 120, align: "center" },
                        //{ name: 'Date', index: 'Date', height: 'auto', width: 128, align: "center", sortable: true },
                        //{ name: 'Till Date', index: 'Till Date', height: 'auto', width: 120, align: "center", sortable: true },
                        { name: 'Finalize', index: 'Finalize', height: 'auto', width: 120, align: "center", sortable: false },
                        { name: 'Active', index: 'Active', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'Delete', index: 'Delete', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'View File', index: 'View File', height: 'auto', width: 120, align: "center", sortable: false, formatter: AnchorFormatter },
            ],
            //postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            postData: { year: $('#ddlYearWise option:selected').val() },
            pager: jQuery('#dvLoadLabourRateListpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30, 50],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ITEM_CODE',
            sortorder: "asc",
            caption: "Labour Rates Master List",
            height: 'auto',
            autowidth: false,
            //width:'250',
            shrinkToFit: true,
            rownumbers: true,
            cmTemplate: { title: false },
            loadComplete: function () {
                var ids = jQuery("#tblLoadLabourRateList").jqGrid('getDataIDs');
                if (ids.length > 0) {
                    $('#dvLoadLabourRateListpager_left').html("<input type='button' style='margin-left:5px' id='btnFinalize' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeAllLabourDetails(" + $('#ddlYearWise option:selected').val() + ");return false;' value='Finalize All'/>");
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

function loadEditLabourRateDetails(param) {
    //alert(param);
    //$('#tblConditionMasterList').jqGrid('setGridState', 'hidden');

    $("#dvLR").hide('slow'); //$("#btnAdd").hide('slow');
    $.ajax({
        url: '/ARRR/LabourRateLayout/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadLabourRate').html('');
            $('#dvLoadLabourRate').html(jsonData);
            $('#dvLoadLabourRate').show('slow');
            $("#dvLR").hide('fast');  //Added by aditi
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteLabourRate(urlparameter) {
    if (confirm("Are you sure you want to delete Labour Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delLabourRateDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);

                    LoadLabourRateGrid();
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

function FinalizeLabourRate(urlparameter) {
    if (confirm("Are you sure you want to Finalize Labour Rate?")) {
        $.ajax({
            url: "/ARRR/LabourRateFinalization/" + urlparameter,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {

                //alert(data.status);
                if (data.status = true) {
                    alert("Labour Rates Finalized Successfully");

                    LoadLabourRateGrid();
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

function changeLabourRatestatus(urlparameter) {
    //alert(urlparameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/ARRR/changeLabourRatestatus/' + urlparameter,
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $("#btnCancel").trigger('click');
                LoadLabourRateGrid();
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

function FinalizeAllLabourDetails(urlparameter) {

    if (confirm("Are you sure you want to finalize all Labour Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/FinalizeAllLabourRates/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadLabourRateGrid();
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
    var url = "/ARRR/DownloadARRRPdfFile/" + cellvalue;
    return "<center><table><tr><td style='border:none'><a href='#'  onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:13px;width:13px' height='13' width='13' border=0 src='../../Content/images/PDF.ico' /></a></td></tr></table></center>";
}
