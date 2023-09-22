$(document).ready(function () {

    $('#btnView').click(function () {
        if ($("#frmFinalizeMRLLayout").valid()) {
            LoadBlocks();
        }
    });

});

function LoadBlocks() {

    jQuery("#tbBlockList").jqGrid('GridUnload');

    jQuery("#tbBlockList").jqGrid({
        url: '/CoreNetwork/GetBlockListMRLPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Block', 'Finalize','Definalize'],
        colModel: [
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'Finalize', index: 'Block', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'DeFinalize', index: 'Block', width: 10, sortable: false, resize: false, align: "center", sortable: false, search: false , hidden:true},
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
            if (data.isAllBlockFinalized == true) {
                $("#dvBlockListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeDRRP' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeMRLDistrict();return false;' value='Finalize District for DRRP'/>");
            }
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

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}


function FinalizeMRLBlock(urlparameter) {
    //if (confirm("Are you sure you want to delete Core Network Details?")) {
    $.ajax({
        type: 'POST',
        url: '/CoreNetwork/FinalizeMRLBlock/' + urlparameter,
        dataType: 'json',
        //data: $("#frmFinalizeMRLLayout").serialize(),
        data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
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
    //}
    //else {
    //    return false;
    //}
}

function FinalizeMRLDistrict() {
    if (confirm("Are you sure you want to finalize TR/MRL Details?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetwork/FinalizeMRLDistrict/' + $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val(),
            dataType: 'json',
            data: $("#frmFinalizeMRLLayout").serialize(),
            //data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
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
        //}
        //else {
        //    return false;
        //}
    }
}

function DeFinalizeMRLBlock(urlparameter) {
    //if (confirm("Are you sure you want to delete Core Network Details?")) {
    $.ajax({
        type: 'POST',
        url: '/CoreNetwork/DeFinalizeMRLBlock/' + urlparameter,
        dataType: 'json',
        //data: $("#frmFinalizeMRLLayout").serialize(),
        data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
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
    //}
    //else {
    //    return false;
    //}
}