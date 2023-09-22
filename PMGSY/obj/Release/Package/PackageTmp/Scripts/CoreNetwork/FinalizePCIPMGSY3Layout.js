$(document).ready(function () {

    $('#btnView').click(function () {
        if ($("#frmFinalizeFacilityLayout").valid()) {
            LoadBlocks();
        }
    });
});

function LoadBlocks() {

    jQuery("#tbBlockList").jqGrid('GridUnload');

    jQuery("#tbBlockList").jqGrid({
        url: '/CoreNetwork/GetBlockListPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Block', 'Total TR/MRL Records', 'Finalized PCI Road Count', 'Unfinalized PCI Road Count', 'Finalize Block'],
        colModel: [
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'TotalRecords', index: 'TotalRecords', width: 60, sortable: true, align: "left", search: false },
                        { name: 'FinalizedRecords', index: 'FinalizedRecords', width: 60, sortable: true, align: "left", search: false },
                        { name: 'NON_FINALIZED_COUNT', index: 'NON_FINALIZED_COUNT', width: 60, sortable: true, align: "left", search: false, hidden:true },
                        { name: 'Finalize', index: 'Block', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
        ],
        postData: { districtCode: $("#ddlDistrict option:selected").val() },
        pager: jQuery('#dvBlockListPager'),
        rowNum: 10,
        sortorder: "asc",
        sortname: 'Block',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function (data) {

            $("#tbBlockList #dvBlockListPager").css({ height: '31px' });
            //if (data.isAllBlockFinalized == true) {
            //    $("#dvBlockListPager_left").html("<input type='button' title = 'Click here to finalize District' style='margin-left:27px' id='idFinalizeFacility' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeFacilityDistrict();return false;' value='Finalize District for PCI'/>");
            //}
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

    $("#tbFacilityList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}


function FinalizeFacilityBlock(urlparameter) {
    if (confirm("Are you sure to finalize the Block ?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetwork/FinalizePCIBlock/' + urlparameter,
            dataType: 'json',
            //data: $("#frmFinalizeFacilityLayout").serialize(),
            data: { __RequestVerificationToken: $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    //$("#tbBlockList").trigger('reloadGrid');
                    LoadBlocks();
                }
                else {
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }
}

function FinalizeFacilityDistrict() {
    if (confirm("Are you sure to finalize the District ?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetwork/FinalizePCIDistrict/' + $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val(),
            dataType: 'json',
            data: $("#frmFinalizeFacilityLayout").serialize(),
            //data: { __RequestVerificationToken: $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    //$("#tbBlockList").trigger('reloadGrid');
                    LoadBlocks();
                }
                else {
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }
}