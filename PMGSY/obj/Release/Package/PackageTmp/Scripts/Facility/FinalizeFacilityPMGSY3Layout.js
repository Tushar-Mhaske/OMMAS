$(document).ready(function () {

    $('#btnView').click(function () {
        if ($("#frmFinalizeFacilityLayout").valid()) {
            LoadBlocks();
        }
    });
    $('#btnViewForMoRD').click(function () {
        if ($("#frmFinalizeFacilityLayout").valid()) {
            LoadBlocksForMoRD();
        }
    });
    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();
        $.ajax({
            // url: '/PFMS1/PopulateDistrictsbyStateCode',
            url: '/CoreNetwork/PopulateDistrictListMORD',
            type: 'GET',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlState").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });
});

function LoadBlocks() {

    jQuery("#tbBlockList").jqGrid('GridUnload');

    jQuery("#tbBlockList").jqGrid({
        url: '/Facility/GetBlockListPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Block', 'Finalize'],
        colModel: [
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
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
            if (data.isAllBlockFinalized == true) {
                $("#dvBlockListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeFacility' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeFacilityDistrict();return false;' value='Finalize District for Facility'/>");
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

    $("#tbFacilityList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}


function FinalizeFacilityBlock(urlparameter) {
    if (confirm("Are you sure you want to finalize ?")) {
    $.ajax({
        type: 'POST',
        url: '/Facility/FinalizeFacilityBlock/' + urlparameter,
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
    //else {
    //    return false;
    //}
}

function FinalizeFacilityDistrict() {
    if (confirm("Are you sure you want to finalize ?")) {
    $.ajax({
        type: 'POST',
        url: '/Facility/FinalizeFacilityDistrict/' + $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val(),
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
    //else {
    //    return false;
    //}
}

function LoadBlocksForMoRD() {

    jQuery("#tbBlockList").jqGrid('GridUnload');

    jQuery("#tbBlockList").jqGrid({
        url: '/Facility/GetBlockListPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Block', 'Finalize / Definalize'],
        colModel: [
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'Finalize / Definalize', index: 'Block', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
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
                $("#dvBlockListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeFacility' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeFacilityDistrict();return false;' value='Finalize District for Facility'/>");
            }
            else if (data.isDistrictDefinalized == false) {
                $("#dvBlockListPager_left").html("<input type='button' style='margin-left:27px' id='idDefinalizeFacility' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DefinalizeFacilityDistrict();return false;' value='Definalize District for Facility'/>");
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

    $("#tbFacilityList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function DefinalizeFacilityDistrict() {
    if (confirm("Are you sure you want to definalize ?")) {
        $.ajax({
            type: 'POST',
            url: '/Facility/DefinalizeFacilityDistrict/' + $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val(),
            dataType: 'json',
            data: $("#frmFinalizeFacilityLayout").serialize(),

            async: false,
            cache: false,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    LoadBlocksForMoRD();
                }
                else {
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
}

function DefinalizeFacilityBlock(urlparameter) {
    if (confirm("Are you sure you want to definalize ?")) {
        $.ajax({
            type: 'POST',
            url: '/Facility/DefinalizeFacilityBlock/' + urlparameter,
            dataType: 'json',
            data: { __RequestVerificationToken: $("#frmFinalizeFacilityLayout input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    LoadBlocksForMoRD();
                }
                else {
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
}