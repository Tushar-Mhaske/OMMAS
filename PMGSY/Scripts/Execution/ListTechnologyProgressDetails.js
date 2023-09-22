$(document).ready(function () {

    loadTechnologyProgress();

    //if ($('#Operation').val() != 'E') {
    LoadExecTechnologyList($('#IMS_PR_ROAD_CODE').val());
    //}
});

// to load execution technology progress details
function LoadExecTechnologyList(roadCode) {
    //alert($('#hdnpreviousStatus').val());
    jQuery("#tbExecTechnologyList").jqGrid('GridUnload');
    jQuery("#tbExecTechnologyList").jqGrid({
        url: '/Execution/GetExecTechnologyProgressDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: roadCode, TechnologyCode: $('#EncryptedTechCode').val(), LayerCode: $('#EncryptedLayerCode').val() },
        colNames: ['Month', 'Year', 'Sanction Length', 'Layer', 'Technology Name', 'Completed/In-Progress', 'Completed/In-Progress Length', 'Date of Completion', 'Edit', 'Delete'],
        //colNames: ['Month', 'Year', 'Completed/In-Progress', 'Completed Length', 'Date of Completion', 'Edit', 'Delete'],
        colModel: [
                    { name: 'Month', index: 'Month', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                    { name: 'Year', index: 'Year', width: 40, sortable: false, align: "center", },
                    { name: 'TechnologyLength', index: 'TechnologyLength', width: 50, sortable: false, align: "center", },
                    { name: 'LayerName', index: 'LayerName', width: 100, sortable: false, align: "center", },
                    { name: 'TechnologyName', index: 'TechnologyName', width: 90, sortable: false, align: "center", },
                    { name: 'CompletedInProgress', index: 'CompletedInProgress', height: 'auto', width: 50, align: "center", search: true, sortable: false },
                    { name: 'CompletedLength', index: 'CompletedLength', height: 'auto', width: 50, align: "center", search: true, sortable: false },
                    { name: 'DateofCompletion', index: 'DateofCompletion', height: 'auto', width: 50, align: "center", search: true, sortable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", sortable: false },
                    //{ name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Delete', width: 40, resize: false, align: "center", sortable: false }
        ],
        pager: jQuery('#pgExecTechnologyList').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "Month",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Execution Technology Progress Details List",
        height: 'auto',
        //width: '98%',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            if ($('#hdnpreviousStatus').val() != 'C') {
                $("#tbExecTechnologyList #pgExecTechnologyList_left").css({ height: '40px' });
                $("#pgExecTechnologyList_left").html("<input type='button' style='margin-left:27px' id='idAddTechnology' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddTechnologyProgress();return false;' value='Add Technology Progress'/>")
            }

        },
        editData: {
            ProposalCode: roadCode
        },
        editurl: "/Execution/EditTechnologyDetails",
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}


function loadTechnologyProgress() {
    //alert($('#hdnEncryptedRoadCode').val());
    //jQuery("#tbExecTechnologyList").jqGrid('GridUnload');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/AddEditTechnologyProgressDetails/" + $('#hdnEncryptedRoadCode').val(),
        type: "GET",
        dataType: "html",
        async: false,
        cache: false,
        data: { Operation: 'A', EncrTechCode: $('#EncryptedTechCode').val(), EncrLayerCode: $('#EncryptedLayerCode').val() },
        success: function (data) {
            $('#divAddTechnologyProgressDetails').html('');
            $('#divAddTechnologyProgressDetails').html(data);
            $('#divAddTechnologyProgressDetails').show('slow');
            //$('#tbExecTechnologyList').trigger('reloadGrid');
            //jQuery("#tbExecTechnologyList").setGridWidth('100%');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}

function AddTechnologyProgress() {
    loadTechnologyProgress();
    $('#divAddEditTechnologyProgressDetails').show('slow');
}