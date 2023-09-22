var rowid;

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadAnalysisRates');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#ddlYear').change(function () {
        LoadAnalysisRatesGrid();
    });

    $('#ddlAnalysisYear').change(function () {
        LoadAnalysisRatesYearGrid();
    });

    $('#trbtn').click(function () {
        $('#trYear').show('slow');
        $("#ddlAnalysisYear").empty();
        $.ajax({
            url: '/ARRR/PopoulateAnalysisYear/',
            type: 'POST',
            //data: { useraction: "A" },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlAnalysisYear").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    $("#btnAdd").click(function () {
        $.ajax({
            url: '/ARRR/AnalysisRatesLayout/',
            type: 'POST',
            data: { year: $('#ddlYear option:selected').val() },
            success: function (jsonData) {

                $('#dvLoadAnalysisRates').html('');
                $('#dvLoadAnalysisRates').html(jsonData);
                $('#dvLoadAnalysisRates').show('slow');
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
    LoadAnalysisRatesGrid();

    $('#btnFinalize').click(function () {
        //alert($("#ddlYear").val());
        FinalizeAnalysisRates($('#ddlYear option:selected').val());
    });

    //$('#btnCopyAllYear').click(function () {
    //    alert($("#ddlYear").val());
    //    CopyAnalysisRates("" + $('#ddlYear option:selected').val() + "$" + $('#ddlAnalysisYear option:selected').val());
    //});

    //$('#btnResetYear').click(function () {
    //    $("#trYear").hide('slow');
    //    $("#trbtn").hide('slow');
    //});
});

function LoadAnalysisRatesGrid() {
    if ($("#frmLoadAnalysisRates").valid()) {
        $("#tblLoadAnalysisRatesList").jqGrid('GridUnload');
        $("#tblLoadAnalysisRatesYearList").jqGrid('GridUnload');

        jQuery("#tblLoadAnalysisRatesList").jqGrid({
            url: '/ARRR/GetAnalysisRatesList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Chapter', 'Item', 'Type', 'Quantity', 'Rate', 'Amount', 'Date', 'Save', 'Edit', 'Delete', 'Approved', 'Finalized'], //26
            colModel: [
                        { name: 'Chapter', index: 'Chapter', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'Item', index: 'Item', height: 'auto', width: 130, align: "center", sortable: true },
                        { name: 'Type', index: 'Type', height: 'auto', width: 130, align: "center", sortable: true },
                        { name: 'Quantity', index: 'Quantity', height: 'auto', width: 150, align: "center", sortable: true, },
                        { name: 'Rate', index: 'Rate', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Amount', index: 'Amount', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Date', index: 'Date', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", hidden: true },
                        { name: 'Edit', index: 'Add', height: 'auto', width: 150, align: "center", sortable: false },
                        { name: 'Delete', index: 'Add', height: 'auto', width: 140, align: "center", sortable: false },
                        { name: 'Approved', index: 'Approved', height: 'auto', width: 140, align: "center", sortable: true },
                        { name: 'Finalized', index: 'Finalized', height: 'auto', width: 140, align: "center", sortable: true },
            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            postData: { year: $("#ddlYear").val() },
            pager: jQuery('#dvLoadAnalysisRatespager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ARRR_CODE',
            sortorder: "asc",
            caption: "Analysis of Rates List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            grouping: true,
            groupingView: {
                groupField: ['Chapter', 'Item', 'Type'],
                groupColumnShow: [false, false, false],
                groupSummary: [false],
                groupText: ['<b>{0}</b>', '<b>{0}</b>', '<b>{0}</b>'],
                groupCollapse: false,
                groupOrder: ['asc', 'asc', 'asc'],
                //showSummaryOnHide: true
            },
            cmTemplate: { title: false },
            editurl: "/ARRR/UpdateAnalysisRatesDetails",
            loadComplete: function (data) {
                if (data.records == 0) {
                    $('#trbtn').show('slow');
                    $('#trYear').hide('slow');
                }
                    //alert($("#tblLoadAnalysisRatesYearList").getGridParam("reccount"));
                    //alert(jQuery("#tblLoadAnalysisRatesYearList").jqGrid('getGridParam', 'records'));

                else if (data.records > 0) {
                    //if (flg == "N") {
                    $('#trbtn').hide('slow');
                    $('#trYear').hide('slow');
                    //}
                    //$('#dvLoadAnalysisRatespager_left').html("<input type='button' style='margin-left:5px' id='btnFinalize' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeAnalysisRates(" + $('#ddlYear option:selected').val() + ");return false;' value='Finalize All'/>");

                    //$('#dvLoadAnalysisRatespager_left').html("<input type='button' style='margin-left:5px' id='btnCopyAll' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'CopyAnalysisRates();return false;' value='Copy All'/>");
                }
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

/*function LoadAnalysisRatesYearGrid() {
    if ($("#frmLoadAnalysisRates").valid()) {
        $("#tblLoadAnalysisRatesYearList").jqGrid('GridUnload');
        $("#tblLoadAnalysisRatesList").jqGrid('GridUnload');

        jQuery("#tblLoadAnalysisRatesYearList").jqGrid({
            url: '/ARRR/GetAnalysisRatesList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Chapter', 'Item', 'Type', 'Quantity', 'Rate', 'Amount', 'Date', 'Save', 'Edit', 'Delete', 'Approved', 'Finalized'], //26
            colModel: [
                        { name: 'Chapter', index: 'Chapter', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'Item', index: 'Item', height: 'auto', width: 130, align: "center", sortable: true },
                        { name: 'Type', index: 'Type', height: 'auto', width: 130, align: "center", sortable: true },
                        { name: 'Quantity', index: 'Quantity', height: 'auto', width: 150, align: "center", sortable: true, },
                        { name: 'Rate', index: 'Rate', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Amount', index: 'Amount', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Date', index: 'Date', height: 'auto', width: 150, align: "center", sortable: true },
                        { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", hidden: true },
                        { name: 'Edit', index: 'Add', height: 'auto', width: 150, align: "center", sortable: false, hidden: true },
                        { name: 'Delete', index: 'Add', height: 'auto', width: 140, align: "center", sortable: false, hidden: true },
                        { name: 'Approved', index: 'Approved', height: 'auto', width: 140, align: "center", sortable: true },
                        { name: 'Finalized', index: 'Finalized', height: 'auto', width: 140, align: "center", sortable: true },
            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            postData: { year: $("#ddlAnalysisYear").val() },
            pager: jQuery('#dvLoadAnalysisRatesYearpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ARRR_CODE',
            sortorder: "asc",
            caption: "Analysis of Rates Y List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            grouping: true,
            groupingView: {
                groupField: ['Chapter', 'Item', 'Type'],
                groupColumnShow: [false, false, false],
                groupSummary: [false],
                groupText: ['<b>{0}</b>', '<b>{0}</b>', '<b>{0}</b>'],
                groupCollapse: false,
                groupOrder: ['asc', 'asc', 'asc'],
                //showSummaryOnHide: true
            },
            cmTemplate: { title: false },
            editurl: "/ARRR/UpdateAnalysisRatesDetails",
            loadComplete: function (data) {
                if (data.records == 0) {
                    $('#trbtn').show('slow');
                }
                //else {
                //    if (flg == "N") {
                //        $('#trbtn').hide('slow');
                //        $('#trYear').hide('slow');
                //    }
                //}
                $('#dvLoadAnalysisRatesYearpager_left').html("<input type='button' style='margin-left:5px' id='btnCopyAllYear' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'CopyAnalysisRates(\"" + $('#ddlAnalysisYear option:selected').val() + "$" + $('#ddlYear option:selected').val() + "\");return false;' value='Copy All'/>");
                //$('#dvLoadAnalysisRatesYearpager_left').html("<input type='button' style='margin-left:5px' id='btnFinalizeYear' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeAnalysisRates();return false;' value='Finalize All'/>");
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
*/

function loadEditAnalysisRates(param) {
    //alert(param);
    //$('#tblConditionMasterList').jqGrid('setGridState', 'hidden');

    $("#btnAdd").hide('slow');
    $.ajax({
        url: '/ARRR/AnalysisRatesLayout/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadAnalysisRates').html('');
            $('#dvLoadAnalysisRates').html(jsonData);
            $('#dvLoadAnalysisRates').show('slow');
            if ($('#hdnItemType').val() == 'I') {
                $("#rdbItem").trigger('click')
            }

            if ($('#hdnItemType').val() == 'M') {
                $("#rdbMajorItem").trigger('click')
            }

            if ($('#hdnItemType').val() == 'N') {
                $("#rdbMinorItem").trigger('click')
            }

            $('#ddlChapter').attr("disabled", true);

            $('input[id=rdbItem]').attr("disabled", true);
            $('input[id=rdbMajorItem]').attr("disabled", true);
            $('input[id=rdbMinorItem]').attr("disabled", true);

            $('input[id=rdbLabour]').attr("disabled", true);
            $('input[id=rdbMachinery]').attr("disabled", true);
            $('input[id=rdbMaterial]').attr("disabled", true);

            if ($('#hdnlmmType').val() == "1") {
                $('#lbllmmType').text('Labour');
            }
            else if ($('#hdnlmmType').val() == "2") {
                $('#lbllmmType').text('Machinery');
            }
            else if ($('#hdnlmmType').val() == "3") {
                $('#lbllmmType').text('Material');
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteAnalysisRates(urlparameter) {
    if (confirm("Are you sure you want to delete Chapter Item details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delAnalysisRatesDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadAnalysisRatesGrid();
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

function ApproveAnalysisRates(urlparameter) {
    if (confirm("Are you sure you want to approve Analysis Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/AnalysisRatesApproval/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadAnalysisRatesGrid();
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

function FinalizeAnalysisRates(urlparameter) {
    alert(urlparameter);
    if (confirm("Are you sure you want to finalize Analysis Rate details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/AnalysisRatesFinalization/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadAnalysisRatesGrid();
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

function CopyAnalysisRates(urlparameter) {
    //alert(urlparameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/ARRR/copyAnalysisRatesDetails/' + urlparameter,
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $("#btnCancel").trigger('click');
                LoadAnalysisRatesGrid();
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
