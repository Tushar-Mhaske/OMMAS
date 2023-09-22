$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchFilter');

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#ddlStateSearch").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlDistrictSearch", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val());
    });

    $("#ddlDistrictSearch").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistrictSearch").find(":selected").val() },
                           "#ddlBlockSearch", "/LockUnlock/GetAllBlocksByDistrictCode?districtCode=" + $('#ddlDistrictSearch option:selected').val());

    });

    $("#ddlBlockSearch").change(function () {
        if ($("#ddlBlockSearch").find(":selected").val() <= 0) {
            $("#ddlVillageSearch").empty();
            $("#ddlVillageSearch").append("<option value='-1'>Select Village</option>");
        }
        else {
            FillInCascadeDropdown({ userType: $("#ddlBlockSearch").find(":selected").val() },
                               "#ddlVillageSearch", "/LockUnlock/GetAllVillagesByBlockCode?blockCode=" + $('#ddlBlockSearch option:selected').val());
        }
    });

    $("#ddlVillageSearch").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlVillageSearch").find(":selected").val() },
                           "#ddlHabSearch", "/LockUnlock/GetAllHabsByVillageCode?villageCode=" + $('#ddlVillageSearch option:selected').val());
    });

    //$("#ddlYearSearch").change(function () {

    //    FillInCascadeDropdown({ userType: $("#ddlYearSearch").find(":selected").val() },
    //                      "#ddlPackageSearch", "/LockUnlock/GetAllPackageByDistrictCode?stateCode=" + $('#ddlStateSearch option:selected').val() + "&districtCode=" + $('#ddlDistrictSearch option:selected').val() + "&yearCode=" + $('#ddlYearSearch option:selected').val());

    //});

    $("#ddlBatchSearch").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBatchSearch").find(":selected").val() },
                          "#ddlPackageSearch", "/LockUnlock/GetAllPackageByDistrictCode?stateCode=" + $('#ddlStateSearch option:selected').val() + "&districtCode=" + $('#ddlDistrictSearch option:selected').val() + "&yearCode=" + $('#ddlYearSearch option:selected').val() + "&batchCode=" + $('#ddlBatchSearch option:selected').val() + "&blockCode=" + $('#ddlBlockSearch option:selected').val() + "&type=" + $('#ddlTypeSearch option:selected').val());

    });

    $("#ddlTypeSearch").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBatchSearch").find(":selected").val() },
                          "#ddlPackageSearch", "/LockUnlock/GetAllPackageByDistrictCode?stateCode=" + $('#ddlStateSearch option:selected').val() + "&districtCode=" + $('#ddlDistrictSearch option:selected').val() + "&yearCode=" + $('#ddlYearSearch option:selected').val() + "&batchCode=" + $('#ddlBatchSearch option:selected').val() + "&blockCode=" + $('#ddlBlockSearch option:selected').val() + "&type=" + $('#ddlTypeSearch option:selected').val());

    });



    ///Changes for RCPLWE
    if ($("#RoleCode").val() == '36' || $("#RoleCode").val() == '47' || $("#RoleCode").val() == '56') {
        setTimeout($("#ddlStateSearch").trigger('change'), 500);
    }



    $("#btnListDetails").click(function () {

        $("#dvProposalDetails").hide();

        if ($("#searchFilter").valid()) {

            var levelCode = $("#ddlLevel").val();
            switch (levelCode) {
                case 'S':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    $("#divAddState").show();
                    PopulateState();
                    break;
                case 'D':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    var stateCode = $("#ddlStateSearch").val();
                    $("#divAddDistrict").show();
                    PopulateDistrict(stateCode);
                    break;
                case 'B':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    var districtCode = $("#ddlDistrictSearch").val();
                    $("#divAddBlock").show();
                    PopulateBlocks(districtCode);
                    break;
                case 'V':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    var blockCode = $("#ddlBlockSearch").val();
                    $("#divAddVillage").show();
                    PopulateVillages(blockCode);
                    break;
                case 'H':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    var villageCode = $("#ddlVillageSearch").val();
                    $("#divAddHabitation").show();
                    PopulateHabitations(villageCode);
                    break;
                case 'Y':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    $("#divAddYears").show();
                    PopulateYears($("#ddlStateSearch option:selected").val(), $("#ddlDistrictSearch option:selected").val(), $("#ddlBlockSearch option:selected").val());
                    break;
                case 'T':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    HideAllRegions();
                    $("#divAddBatches").show();
                    PopulateBatches($("#ddlStateSearch option:selected").val(), $("#ddlDistrictSearch option:selected").val(), $("#ddlBlockSearch option:selected").val(), $("#ddlYearSearch").val());
                    break;
                case 'R':
                    if (CheckFilters() == false) {
                        return false;
                    }
                    var module = $("#ddlModule option:selected").val();
                    HideAllRegions();
                    switch (module) {
                        case '2':
                            ///Changes for RCPLWE
                            if ($("#RoleCode").val() == '36' || $("#RoleCode").val() == '47' || $("#RoleCode").val() == '56') {
                                $("#divAddProposalITNO").show();
                                LoadITNOProposal();
                            }
                            else {
                                $("#divAddProposal").show();
                                LoadProposal();
                            }
                            break;
                        case '3':
                            $("#divAddExistingRoads").show();
                            LoadExistingRoadData();
                            break;
                        case '4':
                            $("#divAddCoreNetwork").show();
                            LoadNetworkGridData();
                            break;
                        case '8':
                            ///Changes for RCPLWE
                            if ($("#RoleCode").val() == '36' || $("#RoleCode").val() == '47' || $("#RoleCode").val() == '56') {
                                $("#divAddProposalITNO").show();
                                LoadITNOProposal();
                            }
                            else {
                                $("#divAddProposal").show();
                                LoadProposal();
                            }
                            break;
                        // Added to unlock Proposal Technology Details
                        case '10':
                            ///Changes for RCPLWE
                            if ($("#RoleCode").val() == '36' || $("#RoleCode").val() == '47' || $("#RoleCode").val() == '56') {
                                $("#divAddProposalITNO").show();
                                LoadITNOProposal();
                            }
                            else {
                                $("#divAddProposal").show();
                                LoadProposal();
                            }
                            break;

                        //Added by Shreyas on 20-07-2023 to unlock C-Proforma PDFs
                        case '11':
                            ///Changes for RCPLWE
                            if ($("#RoleCode").val() == '36' || $("#RoleCode").val() == '47' || $("#RoleCode").val() == '56') {
                                $("#divAddProposalITNO").show();
                                LoadITNOProposal();
                            }
                            else {
                                $("#divAddProposal").show();
                                LoadProposal();
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    });

});
function PopulateDistrict(stateCode) {

    $("#tblstDistricts").jqGrid('GridUnload');
    jQuery("#tblstDistricts").jqGrid({
        url: '/LockUnlock/GetDistrictList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: stateCode, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Districts', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistDistricts').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_DISTRICT_NAME',
        caption: "&nbsp;&nbsp; Districts List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstDistricts")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstDistricts")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstDistricts")[0].p.selarrrow = $("#tblstDistricts").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstDistricts_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstDistricts #pagerlistDistricts").css({ height: '31px' });
                $("#pagerlistDistricts_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

function PopulateYears(stateCode, districtCode, blockCode) {

    $("#tbPropYears").jqGrid('GridUnload');
    jQuery("#tbPropYears").jqGrid({
        url: '/LockUnlock/GetYearsList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: stateCode, districtCode: districtCode, blockCode: blockCode, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Years', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_YEAR_NAME', index: 'MAST_YEAR_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerPropYears').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_YEAR_NAME',
        caption: "&nbsp;&nbsp; Years List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tbPropYears")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tbPropYears")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tbPropYears")[0].p.selarrrow = $("#tbPropYears").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tbPropYears_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tbPropYears #pagerPropYears").css({ height: '31px' });
                $("#pagerPropYears_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

function PopulateBatches(stateCode, districtCode, blockCode, year) {

    $("#tbPropBatches").jqGrid('GridUnload');
    jQuery("#tbPropBatches").jqGrid({
        url: '/LockUnlock/GetBatchesList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: stateCode, districtCode: districtCode, blockCode: blockCode, yearCode: year, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Batch', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_BATCH_NAME', index: 'MAST_BATCH_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerPropBatches').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_BATCH_NAME',
        caption: "&nbsp;&nbsp; Batches List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tbPropBatches")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tbPropBatches")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tbPropBatches")[0].p.selarrrow = $("#tbPropBatches").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tbPropBatches_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tbPropBatches #pagerlistBatches").css({ height: '31px' });
                $("#pagerPropBatches_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

function PopulateBlocks(districtCode) {

    $("#tblstBlocks").jqGrid('GridUnload');
    jQuery("#tblstBlocks").jqGrid({
        url: '/LockUnlock/GetBlockList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: districtCode, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Blocks', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistBlocks').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_BLOCK_NAME',
        caption: "&nbsp;&nbsp; Block List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstBlocks")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstBlocks")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstBlocks")[0].p.selarrrow = $("#tblstBlocks").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstBlocks_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstBlocks #pagerlistBlocks").css({ height: '31px' });
                $("#pagerlistBlocks_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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
function PopulateVillages(blockCode) {

    $("#tblstVillages").jqGrid('GridUnload');
    jQuery("#tblstVillages").jqGrid({
        url: '/LockUnlock/GetVillageList',
        datatype: "json",
        mtype: "POST",
        postData: { blockCode: blockCode, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Village', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistVillages').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_VILLAGE_NAME',
        caption: "&nbsp;&nbsp; Village List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstVillages")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstBlocks")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstVillages")[0].p.selarrrow = $("#tblstVillages").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstVillages_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstVillages #pagerlistVillages").css({ height: '31px' });
                $("#pagerlistVillages_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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
function PopulateHabitations(villageCode) {

    $("#tblstHabs").jqGrid('GridUnload');
    jQuery("#tblstHabs").jqGrid({
        url: '/LockUnlock/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { villageCode: villageCode, moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },   //, stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() },
        colNames: ['Habitations', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', height: 'auto', width: 200, align: "left", search: false },
                             { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistHabs').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_HAB_NAME',
        caption: "&nbsp;&nbsp; Habitation List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstHabs")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstHabs")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstHabs")[0].p.selarrrow = $("#tblstHabs").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstHabs_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstHabs #pagerlistHabs").css({ height: '31px' });
                $("#pagerlistHabs_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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
function HideAllRegions() {
    $("#divAddState").hide();
    $("#divAddDistrict").hide();
    $("#divAddBlock").hide();
    $("#divAddVillage").hide();
    $("#divAddHabitation").hide();
    $("#divAddProposal").hide();
    $("#divAddExistingRoads").hide();
    $("#divAddCoreNetwork").hide();
    $("#divAddProposalITNO").hide();
    $("#divAddYears").hide();
    $("#divAddBatches").hide();
}
function LoadProposal() {

    $("#tblstProposal").jqGrid('GridUnload');
    jQuery("#tblstProposal").jqGrid({
        url: '/LockUnlock/GetProposalList',
        datatype: "json",
        mtype: "POST",
        postData: { proposalType: $('#ddlModule option:selected').val(), yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), type: $("#ddlTypeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },
        colNames: ['Road Name', 'Year', 'Package No.', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'LEVELNAME', index: 'LEVELNAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'YEARCODE', index: 'YEARCODE', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'LEVELNUMBER', index: 'LEVELNUMBER', height: 'auto', width: 50, align: "center", search: true },
                            { name: 'STARTDATE', index: 'STARTDATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ENDDATE', index: 'ENDDATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnProposal, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistProposal').width(20),
        rowNum: 100,
        rowList: [100, 200, 500, 1000, 2000, 5000],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'LEVELNAME',
        caption: "&nbsp;&nbsp;Proposal List",
        height: '400px',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstProposal")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstProposal")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstProposal")[0].p.selarrrow = $("#tblstProposal").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[3] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstProposal_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstProposal #pagerlistProposal").css({ height: '31px' });
                $("#pagerlistProposal_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

function LoadITNOProposal() {

    $("#tblstProposalITNO").jqGrid('GridUnload');
    jQuery("#tblstProposalITNO").jqGrid({
        url: '/LockUnlock/GetITNOProposalList',
        datatype: "json",
        mtype: "POST",
        postData: { yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), type: $("#ddlTypeSearch option:selected").val() },
        colNames: ['Block Name', 'Road Name', 'Year', 'Package No.', 'Road Length'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnProposal, align: "center", search: false },
                            //{ name: 'b', width: 50, sortable: false, resize: false,  align: "center", search: false },

        ],
        pager: jQuery('#pagerlistProposalITNO').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'IMS_ROAD_NAME',
        caption: "&nbsp;&nbsp;Proposal List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] > 0) {

                $("#tblstProposalITNO #pagerlistProposalITNO").css({ height: '31px' });
                $("#pagerlistProposalITNO_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

    $("#tblstCoreNetwork").jqGrid('GridUnload');
    jQuery("#tblstCoreNetwork").jqGrid({
        url: '/LockUnlock/GetCoreNetworkList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },
        colNames: ['Road Name', 'Road Number', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'LEVELNAME', index: 'LEVELNAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'LEVELNUMBER', index: 'LEVELNUMBER', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'STARTDATE', index: 'STARTDATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ENDDATE', index: 'ENDDATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnCoreNetwork, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistCoreNetwork').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'LEVELNAME',
        caption: "&nbsp;&nbsp;CoreNetwork List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstCoreNetwork")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstCoreNetwork")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstCoreNetwork")[0].p.selarrrow = $("#tblstCoreNetwork").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[2] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstCoreNetwork_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }


            if (data["records"] > 0) {

                $("#tblstCoreNetwork #pagerlistCoreNetwork").css({ height: '31px' });
                $("#pagerlistCoreNetwork_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

    $("#tblstExistingRoads").jqGrid('GridUnload');
    jQuery("#tblstExistingRoads").jqGrid({
        url: '/LockUnlock/GetExistingRoadList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), blockCode: $("#ddlBlockSearch option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },
        colNames: ['Road Name', 'Road Number', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'LEVELNAME', index: 'LEVELNAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'LEVELNUMBER', index: 'LEVELNUMBER', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'STARTDATE', index: 'STARTDATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ENDDATE', index: 'ENDDATE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumnCoreNetwork, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistExistingRoads').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'LEVELNAME',
        caption: "&nbsp;&nbsp;Existing Road List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstExistingRoads")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstExistingRoads")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstExistingRoads")[0].p.selarrrow = $("#tblstExistingRoads").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[2] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstExistingRoads_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstExistingRoads #pagerlistExistingRoads").css({ height: '31px' });
                $("#pagerlistExistingRoads_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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
function CheckFilters() {
    var levelCode = $("#ddlLevel option:selected").val();
    switch (levelCode) {
        case 'S':
            break;
        case 'D':
            if ($("#ddlStateSearch option:selected").val() == "0") {
                alert('Please Select State');
                return false;
            }
            break;
        case 'B':
            if ($("#ddlStateSearch option:selected").val() == "0") {
                alert('Please Select State and District');
                return false;
            }
            if ($("#ddlDistrictSearch option:selected").val() == "0") {
                alert('Please Select State and District');
                return false;
            }
            break;
        case 'V':
            if ($("#ddlStateSearch option:selected").val() <= "0") {
                alert('Please Select State, District and Block');
                return false;
            }
            if ($("#ddlDistrictSearch option:selected").val() <= "0") {
                alert('Please Select District and Block');
                return false;
            }
            if ($("#ddlBlockSearch option:selected").val() <= "0") {
                alert('Please Select Block');
                return false;
            }
            break;
        case 'H':
            if ($("#ddlStateSearch option:selected").val() <= "0") {
                alert('Please Select State, District, Block and Village');
                return false;
            }
            if ($("#ddlDistrictSearch option:selected").val() <= "0") {
                alert('Please Select District, Block and Village');
                return false;
            }
            if ($("#ddlBlockSearch option:selected").val() <= "0") {
                alert('Please Select Block and Village');
                HideAllRegions();
                return false;
            }
            if ($("#ddlVillageSearch option:selected").val() <= "0") {
                alert('Please Select Village');
                return false;
            }
            break;
        case 'R':
            if ($("#ddlStateSearch option:selected").val() == "0") {
                alert('Please Select State ,District,Block and Year');
                return false;
            }
            //if ($("#ddlDistrictSearch option:selected").val() == "0") {
            //    alert('Please Select State ,District,Block and Year');
            //    return false;
            //}
            //if ($("#ddlBlockSearch option:selected").val() == "0") {
            //    alert('Please Select State ,District,Block and Year');
            //    return false;
            //}
            var module = $("#ddlModule option:selected").val();
            switch (module) {
                case '2':
                    //if ($("#ddlYearSearch option:selected").val() == "0") {
                    //    alert('Please Select State ,District,Block and Year');
                    //    return false;
                    //}
                    break;
                default:
                    break;
            }

            break;
        default:
            break;
    }


}
