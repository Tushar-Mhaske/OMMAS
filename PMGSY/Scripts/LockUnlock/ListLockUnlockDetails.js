$(document).ready(function () {

    $.validator.unobtrusive.parse('#formModuleFilter');


    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#filterMenuForm").toggle("slow");
    });

    if (!($("#optionDistrict").is(":visible"))) {
        $("#optionDistrict").show();
    }

    $("#ddlModule").change(function () {
        var option = $("#ddlModule option:selected").val();
        switch (option) {
            case "1":
                $("#divFilterForm").load('/LockUnlock/SearchProposal', function () {
                    $.validator.unobtrusive.parse($('#divFilterForm'));
                })
                HideAllPortions();
                break;
            case "2":
                $("#divFilterForm").load('/LockUnlock/SearchExistingRoad', function () {
                    $.validator.unobtrusive.parse($('#divFilterForm'));
                })
                HideAllPortions();
                break;
            case "3":
                $("#divFilterForm").load('/LockUnlock/SearchCoreNetwork', function () {
                    $.validator.unobtrusive.parse($('#divFilterForm'));
                })
                HideAllPortions();
                break;
            case "4":
                $("#divFilterForm").load('/LockUnlock/SearchAgreement', function () {
                    $.validator.unobtrusive.parse($('#searchAgreement'));
                })
                HideAllPortions();
                break;
            case "5":
                $("#divFilterForm").load('/LockUnlock/SearchTendering', function () {
                    $.validator.unobtrusive.parse($('#searchTendering'));
                })
                HideAllPortions();
                break;
            case "6":
                $("#divFilterForm").load('/LockUnlock/SearchIMSContract', function () {
                    $.validator.unobtrusive.parse($('#searchIMSContract'));
                })
                HideAllPortions();
                break;
            case "7":
                $("#divFilterForm").load('/LockUnlock/SearchCNContract', function () {
                    $.validator.unobtrusive.parse($('#searchCNContract'));
                })
                HideAllPortions();
                break;
            default:
                $("#divFilterForm").hide();
                break;
        }
    });

    $("#ddlModule").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlModule").find(":selected").val() },
                           "#ddlSubmodule", "/LockUnlock/GetSubmoduleByModuleCode?moduleCode=" + $('#ddlModule option:selected').val());

        FillInCascadeDropdown({ userType: $("#ddlModule").find(":selected").val() },
                           "#ddlLevel", "/LockUnlock/GetModuleLevelByModuleCode?moduleCode=" + $('#ddlModule option:selected').val());
    });
});

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center' title='Edit Core Network Details';'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Enter Habitation Details' onClick ='editHabitationDetails(\"" + cellvalue.toString() + "\");'>Habitation</span></center>";
    }
}

function FormatColumnProposal(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Core Network Details' onClick ='detailsProposal(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnCoreNetwork(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Core Network Details' onClick ='detailsCoreNetwork(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnExistingRoad(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Core Network Details' onClick ='detailsExistingRoad(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnCNContract(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Core Network Contract Details' onClick ='detailsCNContract(\"" + cellvalue.toString() + "\");'></span></center>";
}

function LoadGridData() {

    jQuery("#tbProposalList").jqGrid({
        url: '/LockUnlock/GetProposalList',
        datatype: "json",
        mtype: "POST",
        postData: { yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Block Name', 'Year', 'Package No.', 'Road Name','Road Length'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnProposal, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false,  align: "center", search: false },
                           
        ],
        pager: jQuery('#pagerProposal').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'IMS_ROAD_NAME',
        caption: "&nbsp;&nbsp; Locked Proposal List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect:true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                    $("#tbProposalList #pagerProposal").css({ height: '31px' });
                    $("#pagerProposal_left").html("<input type='button' style='margin-left:25px' id='idUnlockProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockModule();return false;' value='Unlock'/>")
            }

        },
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

function LoadProposalUnlockList() {

    jQuery("#tbProposalUnlockList").jqGrid({
        url: '/LockUnlock/GetProposalUnLockList',
        datatype: "json",
        mtype: "POST",
        postData: { yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Block Name', 'Year', 'Package No.', 'Road Name', 'Road Length'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnProposal, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false,  align: "center", search: false },

        ],
        pager: jQuery('#pagerUnlockProposal').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_BLOCK_NAME',
        caption: "&nbsp;&nbsp; Unlock Proposal List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] > 0)
            {
                $("#tbProposalUnlockList #pagerUnlockProposal").css({ height: '31px' });
                $("#pagerUnlockProposal_left").html("<input type='button' style='margin-left:25px' id='idLockProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockModule();return false;' value='Lock'/>")
            }

        },
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

function LoadNetworkGridData() {

    jQuery("#tbCoreNetworkList").jqGrid({
        url: '/LockUnlock/GetCoreNetworkList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val()},
        colNames: ['Block Name', 'Road Name', 'Road No.', 'Start Chainage(In Km)', 'End Chainage(In Km)'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'PLAN_RD_FROM_CHAINAGE', index: 'PLAN_RD_FROM_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnCoreNetwork, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerCoreNetwork').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'PLAN_RD_NAME',
        caption: "&nbsp;&nbsp; Locked CoreNetwork List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect:true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbCoreNetworkList #pagerCoreNetwork").css({ height: '31px' });
                $("#pagerCoreNetwork_left").html("<input type='button' style='margin-left:25px' id='idUnlockCoreNetwork' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockModule();return false;' value='Unlock'/>")
            }

        },
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

function LoadNetworkUnlockList()
{
    jQuery("#tbCoreNetworkUnlockList").jqGrid({
        url: '/LockUnlock/GetCoreNetworkUnlockList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Road No.', 'Start Chainage(In Km)', 'End Chainage(In Km)'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'PLAN_RD_FROM_CHAINAGE', index: 'PLAN_RD_FROM_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnCoreNetwork, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerUnlockCoreNetwork').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_BLOCK_NAME',
        caption: "&nbsp;&nbsp; Unlocked CoreNetwork List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbCoreNetworkUnlockList #pagerUnlockCoreNetwork").css({ height: '31px' });
                $("#pagerUnlockCoreNetwork_left").html("<input type='button' style='margin-left:27px' id='idLockCoreNetwork' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockModule();return false;' value='Lock'/>")
            }

        },
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

function LoadExistingRoadData() {

    jQuery("#tbExistingRoadList").jqGrid({
        url: '/LockUnlock/GetExistingRoadList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Road No.', 'Start Chainage(In Km)', 'End Chainage(In Km)'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_ER_ROAD_STR_CHAIN', index: 'MAST_ER_ROAD_STR_CHAIN', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_ER_ROAD_END_CHAIN', index: 'MAST_ER_ROAD_END_CHAIN', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnExistingRoad, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerExistingRoad').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_ER_ROAD_NAME',
        caption: "&nbsp;&nbsp;Locked Existing Road List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect:true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbExistingRoadList #pagerExistingRoad").css({ height: '31px' });
                $("#pagerExistingRoad_left").html("<input type='button' style='margin-left:25px' id='idUnlockExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockModule();return false;' value='Unlock'/>")
            }

        },
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

function LoadExistingRoadUnlockData()
{
    jQuery("#tbExistingRoadsUnlockList").jqGrid({
        url: '/LockUnlock/GetExistingRoadUnlockList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Road No.', 'Start Chainage(In Km)', 'End Chainage(In Km)'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_ER_ROAD_STR_CHAIN', index: 'MAST_ER_ROAD_STR_CHAIN', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_ER_ROAD_END_CHAIN', index: 'MAST_ER_ROAD_END_CHAIN', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnExistingRoad, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerUnlockExistingRoads').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_BLOCK_NAME',
        caption: "&nbsp;&nbsp;Unlocked Existing Road List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect:true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbExistingRoadsUnlockList #pagerUnlockExistingRoads").css({ height: '31px' });
                $("#pagerUnlockExistingRoads_left").html("<input type='button' style='margin-left:27px' id='idLockExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockModule();return false;' value='Lock'/>")
            }

        },
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

function LoadIMSContractList() {

    jQuery("#tbIMSContractList").jqGrid({
        url: '/LockUnlock/GetIMSContractList',
        datatype: "json",
        mtype: "POST",
        postData: { yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Year', 'View', 'Action'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: true },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnTendering, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerIMSContract').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Proposal Contract List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbIMSContractList #pagerIMSContract").css({ height: '31px' });
                $("#pagerIMSContract_left").html("<input type='button' style='margin-left:27px' id='idLockProposalContract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockProposalContract();return false;' value='Lock'/><input type='button' style='margin-left:2px' id='idUnlockProposalContract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockProposalContract();return false;' value='Unlock'/>")
            }

        },
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

function LoadCNContractData() {

    jQuery("#tbCNContractList").jqGrid({
        url: '/LockUnlock/GetManeCoreNetworkList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Year', 'View', 'Action'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 200, align: "left", search: true },
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnCNContract, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerCNContract').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Core Network Contract List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbCNContractList #pagerCNContract").css({ height: '31px' });
                $("#pagerCNContract_left").html("<input type='button' style='margin-left:27px' id='idLockContract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockCNContract();return false;' value='Lock'/><input type='button' style='margin-left:2px' id='idUnlockContract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockCNContract();return false;' value='Unlock'/>")
            }

        },
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

function LoadTenderingData() {

    jQuery("#tbTenderingList").jqGrid({
        url: '/LockUnlock/GetTenderingList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), yearCode: $("#ddlYearSearch option:selected").val() },
        colNames: ['District', 'NIT No.', 'Issue Start Date','Issue End Date', 'Year','View', 'Action'],
        colModel: [
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'TEND_NIT_NUMBER', index: 'TEND_NIT_NUMBER', height: 'auto', width: 150, align: "left", search: true },
                            { name: 'TEND_ISSUE_START_DATE', index: 'TEND_ISSUE_START_DATE', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'TEND_ISSUE_END_DATE', index: 'TEND_ISSUE_END_DATE', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnTendering, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerTendering').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_DISTRICT_NAME',
        caption: "&nbsp;&nbsp; NIT List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbTenderingList #pagerTendering").css({ height: '31px' });
                $("#pagerTendering_left").html("<input type='button' style='margin-left:27px' id='idLockTendering' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockModule();return false;' value='Lock'/><input type='button' style='margin-left:2px' id='idUnlockTendering' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockModule();return false;' value='Unlock'/>")
            }

        },
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

function LoadAgreementData() {

    jQuery("#tbAgreementList").jqGrid({
        url: '/LockUnlock/GetAgreementList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), yearCode: $("#ddlYearSearch option:selected").val() },
        colNames: ['Agreement No.', 'Contractor Name', 'Agreement Type', 'Agreement Date', 'Agreement Amount', 'View', 'Action'],
        colModel: [
                            { name: 'TEND_AGREEMENT_NUMBER', index: 'TEND_AGREEMENT_NUMBER', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_CON_NAME', index: 'MAST_CON_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'TEND_AGREEMENT_TYPE', index: 'TEND_AGREEMENT_TYPE', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'TEND_DATE_OF_AGREEMENT', index: 'TEND_DATE_OF_AGREEMENT', height: 'auto', width: 50, align: "center", search: true },
                            { name: 'TEND_AGREEMENT_AMOUNT', index: 'TEND_AGREEMENT_AMOUNT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'a', width: 40, sortable: false, resize: false, formatter: FormatColumnAgreement, align: "center", search: false },
                            { name: 'b', width: 40, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerAgreement').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'TEND_AGREEMENT_NUMBER',
        caption: "&nbsp;&nbsp; Agreement List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tbAgreementList #pagerAgreement").css({ height: '31px' });
                $("#pagerAgreement_left").html("<input type='button' style='margin-left:27px' id='idLockAgreement' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'LockModule();return false;' value='Lock'/><input type='button' style='margin-left:2px' id='idUnlockExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnlockAgreement();return false;' value='Unlock'/>")
            }

        },
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

function HideAllPortions() {

    $("#divFilterForm").hide();
    $("#divProposal").hide();
    $("#divCoreNetwork").hide();
    $("#divExistingRoad").hide();
    $("#divAgreement").hide();
    $("#divTendering").hide();
    $("#divCNContract").hide();
    $("#divIMSContract").hide();
    $("#divUnlockProposal").hide();
    $("#divUnlockCoreNetwork").hide();
    $("#divUnlockExistingRoads").hide();
}
  
        

function CloseLockDetails() {
    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');

    var module = $("#ddlModule option:selected").val();
    switch (module) {
        case '1':
            $("#tbProposalList").jqGrid('setGridState', 'visible');
            $("#tbProposalUnlockList").jqGrid('setGridState', 'visible');
            break;
        case '2':
            $("#tbExistingRoadList").jqGrid('setGridState', 'visible');
            $("#tbExistingRoadsUnlockList").jqGrid('setGridState', 'visible');
            break;
        case '3':
            $("#tbCoreNetworkList").jqGrid('setGridState', 'visible');
            $("#tbCoreNetworkUnlockList").jqGrid('setGridState', 'visible');
            break;
        case '4':
            $("#tbAgreementList").jqGrid('setGridState', 'visible');
            break;
        case '5':
            $("#tbTenderingList").jqGrid('setGridState', 'visible');
            break;
        case '6':
            $("#tbCNContractList").jqGrid('setGridState', 'visible');
            break;
        case '7':
            $("#tbIMSContractList").jqGrid('setGridState', 'visible');
            break;

    }
    showFilter();
}

function showFilter() {
    if ($('#filterMenuForm').is(":hidden")) {
        $("#filterMenuForm").show('slow');
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlFundingAgency') {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }

    $(dropdown).empty();
    blockPage();
    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
    unblockPage();
}

function LockModule() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var lockStatus = "L";

    var selectedModule = $("#ModuleCode").val();

    switch (selectedModule) {
        case '1':
            var unlockData = $("#tbProposalUnlockList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to lock.');
                return false;
            }
            break;
        case '2':
            var unlockData = $("#tbExistingRoadsUnlockList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to lock.');
                return false;
            }
            break;
        case '3':
            var unlockData = $("#tbCoreNetworkUnlockList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to lock.');
                return false;
            }
            break;
        default:
            break;
    }

    

    if (confirm("Are you sure to lock details? ")) {

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Lock Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
                );

            switch (selectedModule) {
                case '1':
                    //$('#accordion').show('fold', function () {
                    //    blockPage();
                        var data = $("#tbProposalList").jqGrid('getGridParam', 'postData');
                        var lockData = $("#tbProposalUnlockList").jqGrid('getGridParam', 'selarrrow');
                        //$("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, yearCode: data.yearCode, stateCode: data.stateCode, batchCode: data.batchCode, packageCode: data.packageCode, districtCode: data.districtCode,ids:unlockData }), function () {
                        //    $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                        //    unblockPage();
                        //});
                        ChangeLockStatus(lockData,"Proposal");
                    //    $('#divAddLockDetailsForm').show('slow');
                    //    $("#divAddLockDetailsForm").css('height', 'auto');
                    //});
                    //$("#tbProposalList").jqGrid('setGridState', 'hidden');
                    //$("#tbProposalUnlockList").jqGrid('setGridState', 'hidden');
                    //$('#idFilterDiv').trigger('click');
                    break;
                case '2':
                    //$('#accordion').show('fold', function () {
                    //    blockPage();
                        var data = $("#tbExistingRoadList").jqGrid('getGridParam', 'postData');
                        var lockData = $("#tbExistingRoadsUnlockList").jqGrid('getGridParam', 'selarrrow');
                        //$("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, blockCode: data.blockCode, districtCode: data.districtCode }), function () {
                        //    $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                        //    unblockPage();
                        //});
                        ChangeLockStatus(lockData, "Existing Roads");
                    //    $('#divAddLockDetailsForm').show('slow');
                    //    $("#divAddLockDetailsForm").css('height', 'auto');
                    //});
                    //$("#tbExistingRoadList").jqGrid('setGridState', 'hidden');
                    //$("#tbExistingRoadUnlockList").jqGrid('setGridState', 'hidden');
                    //$('#idFilterDiv').trigger('click');
                    break;
                case '3':
                    //$('#accordion').show('fold', function () {
                    //    blockPage();
                        var data = $("#tbCoreNetworkList").jqGrid('getGridParam', 'postData');
                        var lockData = $("#tbCoreNetworkUnlockList").jqGrid('getGridParam', 'selarrrow');
                        //$("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, blockCode: data.blockCode, districtCode: data.districtCode }), function () {
                        //    $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                        //    unblockPage();
                        //});
                        ChangeLockStatus(lockData, "Core Network");
                    //    $('#divAddLockDetailsForm').show('slow');
                    //    $("#divAddLockDetailsForm").css('height', 'auto');
                    //});
                    //$("#tbCoreNetworkList").jqGrid('setGridState', 'hidden');
                    //$("#tbCoreNetworkUnlockList").jqGrid('setGridState', 'hidden');
                    //$('#idFilterDiv').trigger('click');
                    break;
                case '4':
                    $('#accordion').show('fold', function () {
                        blockPage();
                        var data = $("#tbAgreementList").jqGrid('getGridParam', 'postData');
                        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                            $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                            unblockPage();
                        });
                        $('#divAddLockDetailsForm').show('slow');
                        $("#divAddLockDetailsForm").css('height', 'auto');
                    });
                    $("#tbAgreementList").jqGrid('setGridState', 'hidden');
                    $('#idFilterDiv').trigger('click');
                    break;
                case '5':
                    $('#accordion').show('fold', function () {
                        blockPage();
                        var data = $("#tbTenderingList").jqGrid('getGridParam', 'postData');
                        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                            $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                            unblockPage();
                        });
                        $('#divAddLockDetailsForm').show('slow');
                        $("#divAddLockDetailsForm").css('height', 'auto');
                    });
                    $("#tbTenderingList").jqGrid('setGridState', 'hidden');
                    $('#idFilterDiv').trigger('click');
                    break;
                case '6':
                    break;
                case '7':
                    break;
            
            }
    } else {
        return;
    }
}

function UnlockModule() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var unlockStatus = "U";
    var selectedModule = $("#ModuleCode").val();

    switch (selectedModule)
    {
        case '1':
            var unlockData = $("#tbProposalList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            break;
        case '2':
            var unlockData = $("#tbExistingRoadList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            break;
        case '3':
            var unlockData = $("#tbCoreNetworkList").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            break;
        default:
            break;
    }

    

    if (confirm("Are you sure to Unlock Details? ")) {

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Unlock Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
                );
    
                switch (selectedModule) {
                    case '1':
                        $('#accordion').show('fold', function () {
                            blockPage();
                            var data = $("#tbProposalList").jqGrid('getGridParam', 'postData');
                            var unlockData = $("#tbProposalList").jqGrid('getGridParam', 'selarrrow');
                           
                            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, yearCode: data.yearCode, stateCode: data.stateCode, batchCode: data.batchCode, packageCode: data.packageCode, districtCode: data.districtCode,ids:unlockData }), function () {
                                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                                unblockPage();
                            });
                            $('#divAddLockDetailsForm').show('slow');
                            $("#divAddLockDetailsForm").css('height', 'auto');
                        });
                        $("#tbProposalList").jqGrid('setGridState', 'hidden');
                        $("#tbProposalUnlockList").jqGrid('setGridState', 'hidden');
                        $('#idFilterDiv').trigger('click');
                        break;
                    case '2':
                        $('#accordion').show('fold', function () {
                            blockPage();
                            var data = $("#tbExistingRoadList").jqGrid('getGridParam', 'postData');
                            var unlockData = $("#tbExistingRoadList").jqGrid('getGridParam', 'selarrrow');
                            if (unlockData == "") {
                                alert('Please select records to unlock.');
                                return false;
                            }
                            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, blockCode: data.blockCode, districtCode: data.districtCode, ids: unlockData }), function () {
                                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                                unblockPage();
                            });
                            $('#divAddLockDetailsForm').show('slow');
                            $("#divAddLockDetailsForm").css('height', 'auto');
                        });
                        $("#tbExistingRoadList").jqGrid('setGridState', 'hidden');
                        $("#tbExistingRoadsUnlockList").jqGrid('setGridState', 'hidden');
                        $('#idFilterDiv').trigger('click');
                        break;
                    case '3':
                        $('#accordion').show('fold', function () {
                            blockPage();
                            var data = $("#tbCoreNetworkList").jqGrid('getGridParam', 'postData');
                            var unlockData = $("#tbCoreNetworkList").jqGrid('getGridParam', 'selarrrow');
                            if (unlockData == "") {
                                alert('Please select records to unlock.');
                                return false;
                            }
                            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, blockCode: data.blockCode, districtCode: data.districtCode, ids: unlockData }), function () {
                                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                                unblockPage();
                            });
                            $('#divAddLockDetailsForm').show('slow');
                            $("#divAddLockDetailsForm").css('height', 'auto');
                        });
                        $("#tbCoreNetworkList").jqGrid('setGridState', 'hidden');
                        $("#tbCoreNetworkUnlockList").jqGrid('setGridState', 'hidden');
                        $('#idFilterDiv').trigger('click');
                        break;
                    case '4':
                        $('#accordion').show('fold', function () {
                            blockPage();
                            var data = $("#tbAgreementList").jqGrid('getGridParam', 'postData');
                            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                                unblockPage();
                            });
                            $('#divAddLockDetailsForm').show('slow');
                            $("#divAddLockDetailsForm").css('height', 'auto');
                        });
                        $("#tbAgreementList").jqGrid('setGridState', 'hidden');
                        $('#idFilterDiv').trigger('click');
                        break;
                    case '5':
                        $('#accordion').show('fold', function () {
                            blockPage();
                            var data = $("#tbTenderingList").jqGrid('getGridParam', 'postData');
                            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                                unblockPage();
                            });

                            $('#divAddLockDetailsForm').show('slow');
                            $("#divAddLockDetailsForm").css('height', 'auto');
                        });
                        $("#tbTenderingList").jqGrid('setGridState', 'hidden');
                        $('#idFilterDiv').trigger('click');
                        break;
                }
    } else {
        return;
    }
}

function LockModuleUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock/Unlock Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
            );

    var module = $("#ddlModule option:selected").val();
    switch (module) {
        case '1':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbProposalList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '2':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbExistingRoadList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '3':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbCoreNetworkList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '4':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbAgreementList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '5':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbTenderingList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '6':
            break;
        case '7':
            break;
        default:

    }
}

function UnlockModuleUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock/Unlock Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
            );

    var module = $("#ddlModule option:selected").val();
 
    switch (module) {
        case '1':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbProposalList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '2':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbExistingRoadList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '3':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbCoreNetworkList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '4':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbAgreementList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        case '5':
            $('#accordion').show('fold', function () {
                blockPage();
                $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
                    $.validator.unobtrusive.parse($('#divAddForm'));
                    unblockPage();
                });
                $('#divAddLockDetailsForm').show('slow');
                $("#divAddLockDetailsForm").css('height', 'auto');
            });
            $("#tbTenderingList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            break;
        default:

    }
}

function detailsProposal(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/Proposal/Details?id=' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsCoreNetwork(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Core Network Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseLockDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/CoreNetwork/DetailsCoreNetwork/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbCoreNetworkList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsExistingRoad(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Existing Road Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseExistingDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/ExistingRoads/ViewExistingRoads/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbExistingRoadList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsAgreement(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Agreement Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseAgreementDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/ExistingRoads/ViewExistingRoads/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbAgreementList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsTendering(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Tendering Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseTenderingDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/ExistingRoads/ViewExistingRoads/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbTenderingList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsProposalContract(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Tendering Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddLockDetailsForm").load('/ExistingRoads/ViewExistingRoads/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbIMSContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function ChangeLockStatus(lockData,module)
{
    $.ajax({

        type: 'POST',
        url: '/LockUnlock/ChangeLockStatus?' +  $.param({ ids:lockData,module:module}),
        async: false,
        cache: false,
        beforeSend: function () {
            blockPage();
        },
        success: function (data) {
            unblockPage();
            if (data.success == true) {
                var module = $("#ddlModule").val();
                switch (module)
                {
                    case '1':
                        alert('Proposal details are locked successfully.');
                        $("#tbProposalList").trigger('reloadGrid');
                        $("#tbProposalUnlockList").trigger('reloadGrid');
                        break;
                    case '2':
                        alert('Existing Road details are locked successfully.');
                        $("#tbExistingRoadList").trigger('reloadGrid');
                        $("#tbExistingRoadsUnlockList").trigger('reloadGrid');
                        break;
                    case '3':
                        alert('Core Network details are locked successfully.');
                        $("#tbCoreNetworkList").trigger('reloadGrid');
                        $("#tbCoreNetworkUnlockList").trigger('reloadGrid');
                        break;
                    default:
                        break;
                }
            }
            else {
                alert('Error occurred while processing the request.');

            }
        },
        error: function () {
            unblockPage();
            alert('Error occurred while processing the request.');
        }


    });
}


function CloseExistingDetails() {

    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');
    $("#tbExistingRoadList").jqGrid('setGridState', 'visible');
    showFilter();

}




function FormatColumnAgreement(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Agreement Details' onClick ='detailsAgreement(\"" + cellvalue.toString() + "\");'></span></center>";
}

function CloseAgreementDetails() {

    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');
    $("#tbAgreementList").jqGrid('setGridState', 'visible');
    showFilter();

}

function FormatColumnTendering(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Agreement Details' onClick ='detailsTendering(\"" + cellvalue.toString() + "\");'></span></center>";
}
function CloseTenderingDetails() {

    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');
    $("#tbTenderingList").jqGrid('setGridState', 'visible');
    showFilter();

}

function LockCNContract() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var lockStatus = "L";

    if (confirm("Are you sure to lock proposal contract details? ")) {

        var data = $("#tbCNContractList").jqGrid('getGridParam', 'postData');

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseCNContractDetails();" /></a>'
                );

        $('#accordion').show('fold', function () {
            blockPage();

            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                unblockPage();
            });
            $('#divAddLockDetailsForm').show('slow');
            $("#divAddLockDetailsForm").css('height', 'auto');
        });

        $("#tbCNContractList").jqGrid('setGridState', 'hidden');

        $('#idFilterDiv').trigger('click');

    } else {
        return;
    }
}

function UnlockCNContract() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var unlockStatus = "U";
    if (confirm("Are you sure to Unlock Proposal Contract Details? ")) {

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Unlock Proposal Contract Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseCNContractDetails();" /></a>'
                );

        $('#accordion').show('fold', function () {
            blockPage();
            var data = $("#tbCNContractList").jqGrid('getGridParam', 'postData');
            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                unblockPage();
            });

            $('#divAddLockDetailsForm').show('slow');
            $("#divAddLockDetailsForm").css('height', 'auto');
        });

        $("#tbCNContractList").jqGrid('setGridState', 'hidden');

        $('#idFilterDiv').trigger('click');


    } else {
        return;
    }
}

function LockCNContractUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseCNContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbCNContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function UnlockCNContractUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseCNContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbCNContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function detailsCNContract(urlparameter) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Tendering Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseCNContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divAddForm").load('/CoreNetwork/DetailsCoreNetwork?id=' + urlparameter, function () {
        $("#divAddLockDetailsForm").load('/ExistingRoads/ViewExistingRoads/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbCNContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function CloseCNContractDetails() {

    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');
    $("#tbCNContractList").jqGrid('setGridState', 'visible');
    showFilter();
}


//----------------------------------------------------------IMS_CONTRACT----------------------------------------------------------------------



function LockProposalContract() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var lockStatus = "L";

    if (confirm("Are you sure to lock proposal contract details? ")) {

        var data = $("#tbIMSContractList").jqGrid('getGridParam', 'postData');

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalContractDetails();" /></a>'
                );

        $('#accordion').show('fold', function () {
            blockPage();

            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: lockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                unblockPage();
            });
            $('#divAddLockDetailsForm').show('slow');
            $("#divAddLockDetailsForm").css('height', 'auto');
        });

        $("#tbIMSContractList").jqGrid('setGridState', 'hidden');

        $('#idFilterDiv').trigger('click');

    } else {
        return;
    }
}

function UnlockProposalContract() {

    $("#ModuleCode").val($("#ddlModule option:selected").val());
    $("#SubModuleCode").val($("#ddlSubmodule option:selected").val());
    var unlockStatus = "U";
    if (confirm("Are you sure to Unlock Proposal Contract Details? ")) {

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Unlock Proposal Contract Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalContractDetails();" /></a>'
                );

        $('#accordion').show('fold', function () {
            blockPage();
            var data = $("#tbIMSContractList").jqGrid('getGridParam', 'postData');
            $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetailsBatch?' + $.param({ moduleCode: $("#ddlModule option:selected").val(), subModuleCode: $("#ddlSubmodule option:selected").val(), lockStatus: unlockStatus, stateCode: data.stateCode, yearCode: data.yearCode, districtCode: data.districtCode }), function () {
                $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
                unblockPage();
            });

            $('#divAddLockDetailsForm').show('slow');
            $("#divAddLockDetailsForm").css('height', 'auto');
        });

        $("#tbIMSContractList").jqGrid('setGridState', 'hidden');

        $('#idFilterDiv').trigger('click');


    } else {
        return;
    }
}
function LockIMSContractUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbIMSContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function UnlockIMSContractUnit(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lock Proposal Contract Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalContractDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddLockDetailsForm").load('/LockUnlock/AddLockDetails?urlparameter=' + urlparameter + "&moduleCode=" + $("#ddlModule option:selected").val() + "&subModuleCode=" + $("#ddlSubmodule option:selected").val(), function () {
            $.validator.unobtrusive.parse($('#divAddForm'));
            unblockPage();
        });
        $('#divAddLockDetailsForm').show('slow');
        $("#divAddLockDetailsForm").css('height', 'auto');
    });

    $("#tbIMSContractList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}
function CloseProposalContractDetails() {

    $('#accordion').hide('slow');
    $('#divAddLockDetailsForm').hide('slow');
    $("#tbIMSContractList").jqGrid('setGridState', 'visible');
    showFilter();

}
