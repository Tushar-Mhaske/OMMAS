$(document).ready(function () {
    LoadExecutionGrid("A");
});

$('#btnViewScore').click(function () {
    LoadExecutionGrid($('#monitorTypeList').val());
});

$('#searchMonitor').click(function () {
    $('#searchMonitor').hide('slow');
    $('#formAddScore').show('slow');
});

$('#spCollapseIconCQC').click(function () {
    $('#searchMonitor').show('slow');
    $('#formAddScore').hide('slow');
    LoadExecutionGrid("A");
});

function LoadExecutionGrid(type) {

    jQuery("#tbScoreList").jqGrid('GridUnload');
    jQuery("#tbScoreList").jqGrid({
        url: '/QualityMonitoring/GetProficiencyTestScoreListCQC',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { monitorType: type },
        colNames: ['TEST', 'ID', 'Monitor Name', 'Monitor Type', 'Marks', 'Remark', 'Edit'/*, 'Finalize'*/],
        colModel: [
            { name: 'TEST', index: 'TEST', height: 'auto', width: 80, align: "center", search: false, hidden: true },
            { name: 'ID', index: 'ID', height: 'auto', width: 80, align: "center", search: false, hidden: true },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 100, align: "center", search: true },
            { name: 'TYPE', index: 'TYPE', height: 'auto', width: 100, align: "center", search: false },
            { name: 'PERCENTAGE', index: 'PERCENTAGE', height: 'auto', width: 80, align: "center", search: false, sortable: true },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 100, align: "center", search: false },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 50, align: "center", search: false },
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
        grouping: true,
        groupingView: {
            groupField: ['TEST'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
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
    });
    $("#tbScoreList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
}

function EditScore(idtemp) {

    if (confirm("Do you want to edit details ?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/EditProficiencyScoreCQC/?idtemp=' + idtemp,
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            success: function (data) {
                $('#formScore').html(data);
                $('#searchMonitor').hide();
                $('#divListScore').hide();
                $('#btnViewScore').hide();
                $('#formAddScore').hide();
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

    if (confirm("Do you want to finalize records ?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/FinalizeProficiencyScoreCQC/?idtemp=' + idtemp,
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
