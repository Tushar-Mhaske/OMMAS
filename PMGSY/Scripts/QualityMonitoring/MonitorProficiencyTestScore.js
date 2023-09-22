$(document).ready(function () {

    if ($('#isUserEnableProficiencyTest').val() == "Y") {
        LoadExecutionGrid();
    }

});

$("#monitorStatus").change(function (e) {
    if ($("#monitorStatus").val() == "P") {

        $('.marks').show();
    }
    else {
        $('.marks').hide();
    }
})

$("#monitorStatusEdit").change(function (e) {
    if ($("#monitorStatusEdit").val() == "P") {

        $('.marks').show();
    }
    else {
        $('.marks').hide();
    }
})


$("#spCollapseIconDownload").click(function () {
    $('#formDownloadTemplate').hide();
    $('#AddScore').show();
    $('#divListScore').show();
    $('#DownloadTemplate').show();

    $('#tbScoreList').trigger('reloadGrid');
});

$('#iconEditClose').click(function () {
    $('#formEditScore').hide();
    $('#AddScore').show();
    $('#divListScore').show();
    $('#DownloadTemplate').show();
    $('#pagerScore').show();

    $('#tbScoreList').trigger('reloadGrid');
});

$('#iconAddClose').click(function () {
    $('#formAddProficiencyScore').hide();
    $('#AddScore').show();
    $('#divListScore').show();
    $('#DownloadTemplate').show();
    $('#pagerScore').show();

    $('#tbScoreList').trigger('reloadGrid');
});

$('#AddScore').click(function () {
    $('#formScore').show();
    $.ajax({
        url: '/QualityMonitoring/ProficiencyTestScoreForm',
        type: 'GET',
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        success: function (data) {
            $('#formScore').html(data);
            $('#divListScore').hide();
            $('#AddScore').hide();
            $('#DownloadTemplate').hide();
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
});

$('#DownloadTemplate').click(function () {

    $('#formScore').show();

    $.ajax({
        url: '/QualityMonitoring/DownloadTemplateForm',
        type: 'GET',
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        success: function (data) {
            $('#formScore').html(data);
            $('#divListScore').hide();
            $('#AddScore').hide();
            $('#DownloadTemplate').hide();
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
});

$('#monitorType').change(function () {
    if ($('#monitorType').val() == "S")
        $('.stateDropdown').show();
    else
        $('.stateDropdown').hide();
});

$('#btnDownloadTemplate').click(function () {

    var monitorType = $('#monitorType').val();
    var stateCode = $('#stateCode').val();
    var mergeValue = monitorType + "$" + stateCode;

    if (monitorType == "")
        alert("Please select Monitor Type.");
    else if (monitorType == "S" && stateCode == "0")
        alert("Please select a state.");
    else
        window.location.href = "/QualityMonitoring/DownloadTemplateSheet/?mergeValue=" + mergeValue;
});

function LoadExecutionGrid() {

    jQuery("#tbScoreList").jqGrid('GridUnload');
    jQuery("#tbScoreList").jqGrid({
        url: '/QualityMonitoring/GetProficiencyTestScoreList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        colNames: ['ID', 'Date of Exam', 'Institution Name', 'Monitor Type', 'State', 'Data Entry Date', 'View Uploaded Data', 'Delete', 'Finalize'],
        colModel: [
            { name: 'ID', index: 'ID', height: 'auto', width: 80, align: "center", search: false, hidden: true },
            { name: 'DATE', index: 'DATE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'INSTITUTION', index: 'INSTITUTION', height: 'auto', width: 120, align: "center", sortable: false },
            { name: 'TYPE', index: 'TYPE', height: 'auto', width: 80, align: "center", search: false, sortable: true },
            { name: 'STATE', index: 'STATE', height: 'auto', width: 80, align: "center", search: false, sortable: true },
            { name: 'ENTRY_DATE', index: 'ENTRY_DATE', height: 'auto', width: 80, align: "center", search: false, sortable: true },
            { name: 'VIEW', index: 'VIEW', height: 'auto', width: 50, align: "center", search: false },
            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 50, align: "center", search: false },
            { name: 'FINALIZE', index: 'FINALIZE', height: 'auto', width: 50, align: "center", search: false }
        ],
        pager: jQuery('#pagerScore').width(20),
        rowNum: 30,
        rowList: [30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ID",
        sortorder: "asc",
        caption: "Monitors Score List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbScoreList #pagerScore").css({ height: '40px' });
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
        onHeaderClick: function () {
            $('#AddScore').show();
            $('#DownloadTemplate').show();

            $('#divListScoreDetails').hide('slow');
        }
    });

}

function ViewUploadedDataDetails(idtemp) {

    $("#tbScoreList").jqGrid('setGridState', 'hidden');
    $('#divListScoreDetails').show();

    $('#AddScore').hide();
    $('#DownloadTemplate').hide();

    jQuery("#tbScoreListDetails").jqGrid('GridUnload');
    jQuery("#tbScoreListDetails").jqGrid({
        url: '/QualityMonitoring/GetProficiencyTestScoreListDetails/?examId=' + idtemp,
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        colNames: ['ID', 'Monitor Name', 'Marks', 'Edit', 'FinalizeFlag'],
        colModel: [
            { name: 'ID', index: 'ID', height: 'auto', width: 80, align: "center", search: false, hidden: true },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 100, align: "left", sortable: false, search: true },
            { name: 'PERCENTAGE', index: 'PERCENTAGE', height: 'auto', width: 80, align: "center", search: false, sortable: true },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 50, align: "center", search: false },
            { name: 'IS_FINALIZE', index: 'IS_FINALIZE', height: 'auto', width: 50, align: "center", search: false, hidden: true },
        ],
        pager: jQuery('#pagerScoreDetails').width(20),
        rowNum: 50,
        rowList: [50, 70, 100],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ID",
        sortorder: "asc",
        caption: "Monitors Score List Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbScoreListDetails #pagerScoreDetails").css({ height: '40px' });

            var ids = jQuery("#tbScoreListDetails").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var curretRowId = ids[i];
                var rowData = jQuery("#tbScoreListDetails").getRowData(curretRowId);
                var finalizeFlag = rowData['IS_FINALIZE'];

                if (finalizeFlag == "N") {
                    $("#pagerScoreDetails_left").html("<input type='button' style='margin-left:1px; margin-top:1px' id='addNewScore' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddNewScore(" + idtemp + ");return false;' value='Add New Score'/>");
                }
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
        onHeaderClick: function () {
            $('#AddScore').show();
            $('#DownloadTemplate').show();

            $('#divListScoreDetails').hide('slow');
            LoadExecutionGrid();
            $('#pagerScore').show('slow');
        }
    });
    $("#tbScoreListDetails").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
}

function AddNewScore(idtemp) {
    $('#divListScoreDetails').hide();
    $.ajax({
        url: '/QualityMonitoring/AddProficiencyScore/?examId=' + idtemp,
        type: 'GET',
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        success: function (data) {
            $('#formScore').html(data);
            $('#divListScore').hide();
            $('#AddScore').hide();
            $('#DownloadTemplate').hide();
            $('#divListScoreDetails').hide();
            $('#monitorList').chosen();
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}

$('#btnSaveNewScore').click(function () {
    //if ($("#formAddProficiencyScore").valid()) {
    if (confirm("Are you sure to add proficiency test score details ?")) {
        //if ($('#btnSaveNewScore').valid()) {
        var form = $('#formAddProficiencyScore');
        var formadata = new FormData(form.get(0));

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/AddProficiencyTestScore',
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            data: formadata,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#formAddProficiencyScore")[0].reset();
                    $('#formAddProficiencyScore').hide();
                    $('#divListScore').hide();
                    $('#AddScore').show();
                    $('#DownloadTemplate').show();
                    //$('#divListScoreDetails').show();

                    $('#tbScoreListDetails').trigger('reloadGrid');


                    //$("#formEditScore")[0].reset();
                    //$("#formEditScore").hide();
                    /*$('#formScore').hide();*/
                    $('#AddScore').show();
                    $('#DownloadTemplate').show();
                    $('#divListScore').show();
                    $('#pagerScore').show();
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
        //}
    }
    //}

    //else
    //return false;

});



function EditScore(idtemp) {
    $.ajax({
        url: '/QualityMonitoring/EditProficiencyScore/?scoreId=' + idtemp,
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        beforeSend: function () { },
        success: function (data) {
            $('#formScore').html(data);
            $('#formScore').show();
            $('#divListScore').hide();
            $('#divListScoreDetails').hide();
            $('#AddScore').hide();
            $('#DownloadTemplate').hide();
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}

$('#btnEditScore').click(function () {

    //if ($("#formEditScore").valid()) {
    if (confirm("Are you sure to update proficiency test score details ?")) {
        //if ($('#btnEditScore').valid()) {
        var form = $('#formEditScore');
        var formadata = new FormData(form.get(0));

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/EditProficiencyTestScore',
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            data: formadata,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#formEditScore")[0].reset();
                    $("#formEditScore").hide();
                    /*$('#formScore').hide();*/
                    $('#AddScore').show();
                    $('#DownloadTemplate').show();
                    $('#divListScore').show();
                    $('#pagerScore').show();
                    //$('#divListScore').hide();
                    //$('#AddScore').show();
                    //$('#DownloadTemplate').show();
                    //$('#divListScoreDetails').show();

                    $('#tbScoreListDetails').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
        //}
    }
    //}

    else
        return false;

});


//function EditScore(idtemp) {
//    $('#formScore').show();
//    if (confirm("Do you want to edit details ?")) {
//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//        $.ajax({
//            url: '/QualityMonitoring/EditProficiencyScore/?idtemp=' + idtemp,
//            cache: false,
//            async: false,
//            contentType: false,
//            processData: false,
//            beforeSend: function () { },
//            success: function (data) {
//                $('#formScore').html(data);
//                $('#divListScore').hide();
//                $('#AddScore').hide();
//                $('#DownloadTemplate').hide();
//                $.unblockUI();
//            },
//            error: function () {
//                $.unblockUI();
//                alert("An Error");
//                return false;
//            },
//        });
//    }
//}

function DeleteScore(idtemp) {

    if (confirm("Do you want to delete details ? This will delete the entire data of proficiency test for OMMAS.")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/DeleteProficiencyScore/?idtemp=' + idtemp,
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $('#tbScoreList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
}

function FinalizeScore(idtemp) {

    if (confirm("Do you want to finalize record ?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/FinalizeProficiencyScore/?idtemp=' + idtemp,
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            success: function (response) {
                alert(response.message);
                if (response.success) {

                    $('#tbScoreList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
}
