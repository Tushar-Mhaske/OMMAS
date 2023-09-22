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



    $("#divFilterForm").load('/LockUnlock/LoadFilterDetails', function () {
        $.validator.unobtrusive.parse($('#divFilterForm'));
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#filterMenuForm").toggle("slow");
    });

    if (!($("#optionDistrict").is(":visible"))) {
        $("#optionDistrict").show();
    }
    
    $('#ddlRole').change(function () {
        if (parseInt($('#ddlRole option:selected').val()) == 54 || parseInt($('#ddlRole option:selected').val()) == 55 || parseInt($('#ddlRole option:selected').val()) == 0)
        {
            

            $("#ddlSchemeSearch").empty();
            $("#ddlSchemeSearch").append("<option value='0'>--Select--</option>");
            $("#ddlSchemeSearch").append("<option value='1'>PMGSY-1</option>");
            $("#ddlSchemeSearch").append("<option value='2'>PMGSY-2</option>");
            $("#ddlSchemeSearch").append("<option value='3'>RCPLWE</option>");



        }
        else
        {
            $('#ddlSchemeSearch').find("option[value='3']").remove();
        }
    });

    $("#ddlLevel").change(function () {


        if ($("#ddlModule option:selected").val() == 9) {


            $('#ddlRole').hide('slow');
            $('#roleName').hide('slow');

        }
        else {
            $('#roleName').show('slow');
            $('#ddlRole').show('slow');
        }


        var option = $("#ddlLevel option:selected").val();
        HideAllDetails();
        $('#lblCollaboration').hide('slow');
        $('#ddlCollaboration').hide('slow');
        switch (option) {
            case "S":
                //PopulateState();
                HideAllPortions();
                HideAllRegions();
                $("#divFilterForm").show();
                $("#divAddState").show();
                $("#divFilterForm").show('slow');
                if ($("#ddlModule option:selected").val() == 2 || $("#ddlModule option:selected").val() == 8) {
                    $("#lblType").show('slow');
                    $("#ddlTypeSearch").show('slow');
                }
                break;
            case "D":
                $("#divAddState").hide('slow');
                $("#divFilterForm").show();
                HideAllPortions();
                HideAllRegions();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                if ($("#ddlModule option:selected").val() == 2 || $("#ddlModule option:selected").val() == 8) {
                    $("#lblType").show('slow');
                    $("#ddlTypeSearch").show('slow');
                }
                break;
            case "B":
                $("#divAddState").hide('slow');
                $("#divFilterForm").show();
                HideAllPortions();
                HideAllRegions();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                if ($("#ddlModule option:selected").val() == 2 || $("#ddlModule option:selected").val() == 8) {
                    $("#lblType").show('slow');
                    $("#ddlTypeSearch").show('slow');
                }
                break;
            case "V":
                $("#divAddState").hide('slow');
                HideAllPortions();
                HideAllRegions();
                $("#divFilterForm").show();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblBlock").show('slow');
                $("#ddlBlockSearch").show('slow');
                break;
            case "R":
                HideAllRegions();
                HideAllPortions();
                var module = $("#ddlModule option:selected").val();
                switch (module) {
                    case '2':
                        $("#divFilterForm").show();
                        $("#lblState").show('slow');
                        $("#ddlStateSearch").show('slow');
                        $("#lblDistrict").show('slow');
                        $("#ddlDistrictSearch").show('slow');
                        $("#lblBlock").show('slow');
                        $("#ddlBlockSearch").show('slow');
                        $("#lblYear").show('slow');
                        $("#ddlYearSearch").show('slow');
                        $("#lblPackage").show('slow');
                        $("#ddlPackageSearch").show('slow');
                        $("#lblBatch").show('slow');
                        $("#ddlBatchSearch").show('slow');
                        $("#lblType").show('slow');
                        $("#ddlTypeSearch").show('slow');
                        $("#lblCollaboration").show('slow');
                        $("#ddlCollaboration").show('slow');
                        break;
                    case '3':
                        $("#divFilterForm").show();
                        $("#lblState").show('slow');
                        $("#ddlStateSearch").show('slow');
                        $("#lblDistrict").show('slow');
                        $("#ddlDistrictSearch").show('slow');
                        $("#lblBlock").show('slow');
                        $("#ddlBlockSearch").show('slow');
                        break;
                    case '4':
                        $("#divFilterForm").show();
                        $("#lblState").show('slow');
                        $("#ddlStateSearch").show('slow');
                        $("#lblDistrict").show('slow');
                        $("#ddlDistrictSearch").show('slow');
                        $("#lblBlock").show('slow');
                        $("#ddlBlockSearch").show('slow');
                        break;
                    case '8':
                        $("#divFilterForm").show();
                        $("#lblState").show('slow');
                        $("#ddlStateSearch").show('slow');
                        $("#lblDistrict").show('slow');
                        $("#ddlDistrictSearch").show('slow');
                        $("#lblBlock").show('slow');
                        $("#ddlBlockSearch").show('slow');
                        $("#lblYear").show('slow');
                        $("#ddlYearSearch").show('slow');
                        $("#lblPackage").show('slow');
                        $("#ddlPackageSearch").show('slow');
                        $("#lblBatch").show('slow');
                        $("#ddlBatchSearch").show('slow');
                        $("#lblType").show('slow');
                        $("#ddlTypeSearch").show('slow');
                        $("#lblCollaboration").show('slow');
                        $("#ddlCollaboration").show('slow');
                        break;
                    default:
                        break;
                }
                break;
            case "H":
                $("#divAddState").hide('slow');
                HideAllPortions();
                HideAllRegions();
                $("#divFilterForm").show();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblBlock").show('slow');
                $("#ddlBlockSearch").show('slow');
                $("#lblVillage").show('slow');
                $("#ddlVillageSearch").show('slow');
                break;
            case "T":
                $("#divAddState").hide('slow');
                HideAllPortions();
                HideAllRegions();
                $("#divFilterForm").show();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblBlock").show('slow');
                $("#ddlBlockSearch").show('slow');
                $("#lblYear").show('slow');
                $("#ddlYearSearch").show('slow');
                $("#lblType").show('slow');
                $("#ddlTypeSearch").show('slow');
                if ($("#ddlModule option:selected").val() == 2 || $("#ddlModule option:selected").val() == 8) {
                    $("#lblCollaboration").show('slow');
                    $("#ddlCollaboration").show('slow');
                }
                break;
            case "Y":
                $("#divAddState").hide('slow');
                HideAllPortions();
                HideAllRegions();
                $("#divFilterForm").show();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblBlock").show('slow');
                $("#ddlBlockSearch").show('slow');
                $("#lblType").show('slow');
                $("#ddlTypeSearch").show('slow');
                if ($("#ddlModule option:selected").val() == 2 || $("#ddlModule option:selected").val() == 8) {
                    $("#lblCollaboration").show('slow');
                    $("#ddlCollaboration").show('slow');
                }
                break;
            default:
                break;
        }

        if (($("#ddlModule option:selected").val() == 6 || $("#ddlModule option:selected").val() == 7) && ($("#ddlLevel option:selected").val() == "S")) {
            $("#lblScheme").hide();
            $("#ddlSchemeSearch").val(1);
            $("#ddlSchemeSearch").hide();
        }
        else {
            if ($("#ddlSchemeSearch").is(':hidden')) {
                $("#lblScheme").show();
                $("#ddlSchemeSearch").show();
            }
        }

    });

    $("#ddlModule").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlModule").find(":selected").val() },
                           "#ddlSubmodule", "/LockUnlock/GetSubmoduleByModuleCode?moduleCode=" + $('#ddlModule option:selected').val());

        FillInCascadeDropdown({ userType: $("#ddlModule").find(":selected").val() },
                           "#ddlLevel", "/LockUnlock/GetModuleLevelByModuleCode?moduleCode=" + $('#ddlModule option:selected').val());
    });




    $("#MAST_STATE_CODE").multiselect({
        nonSelectedText: "Select States",
        minWidth: 300,
        height: 500
    });
    $("#MAST_STATE_CODE").multiselect("uncheckAll");

    $("#MAST_DISTRICT_CODE").multiselect({
        nonSelectedText: "Select Districts",
        minWidth: 300,
        height: 500
    });
    $("#MAST_DISTRICT_CODE").multiselect("uncheckAll");

    $("#MAST_BLOCK_CODE").multiselect({
        nonSelectedText: "Select Blocks",
        minWidth: 300,
        height: 500
    });
    $("#MAST_BLOCK_CODE").multiselect("uncheckAll");

    $("#MAST_HAB_CODE").multiselect({
        nonSelectedText: "Select Habitations",
        minWidth: 300,
        height: 500
    });
    $("#MAST_HAB_CODE").multiselect("uncheckAll");

    $("#lblViewRecords").click(function () {
        $("#filterMenuForm").hide();
        $("#dvUnlockRecords").show('slow');
        $("#lblViewRecords").hide();
        $("#lblUnlockRecords").show('slow');
        $("#dvProposalDetails").hide();
        $("#accordion").hide();
    });

    $("#lblUnlockRecords").click(function () {
        $("#dvUnlockRecords").hide();
        $("#filterMenuForm").show('slow');
        $("#lblUnlockRecords").hide();
        $("#lblViewRecords").show('slow');
        $("#dvProposalDetails").hide();
        $("#accordion").hide();
    });

    $("#btnViewData").click(function () {

        if ($("#ddlState option:selected").val() == 0) {
            alert("Please select State.");
            return false;
        }

        if ($("#ddlFilterModule option:selected").val() == 0) {
            alert("Please select Module.");
            return false;
        }

        LoadUnlockedRecords();
    });

});
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

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
function HideAllPortions() {
    $("#lblState").hide();
    $("#lblYear").hide();
    $("#lblPackage").hide();
    $("#lblBatch").hide();
    $("#lblDistrict").hide();
    $("#lblBlock").hide();
    $("#lblVillage").hide();
    $("#lblHabitation").hide();
    $("#lblType").hide();
    $("#ddlStateSearch").hide();
    $("#ddlDistrictSearch").hide();
    $("#ddlBlockSearch").hide();
    $("#ddlVillageSearch").hide();
    $("#ddlHabSearch").hide();
    $("#ddlYearSearch").hide();
    $("#ddlPackageSearch").hide();
    $("#ddlBatchSearch").hide();
    $("#ddlTypeSearch").hide();
    $("#lblProposalType").hide();
    $("#ddlPropTypeSearch").hide();
}
function AddUnlockDetails() {
    var level = $("#ddlLevel option:selected").val();

    HideAllDetails();

    switch (level) {
        case 'S':

            var unlockData = $("#tblstState").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tblstState").jqGrid('setGridState', 'hidden');
            break;
        case 'D':
            var unlockData = $("#tblstDistricts").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tblstDistricts").jqGrid('setGridState', 'hidden');
            break;
        case 'B':
            var unlockData = $("#tblstBlocks").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tblstBlocks").jqGrid('setGridState', 'hidden');
            break;
        case 'V':
            $("#tblstVillages").jqGrid('setGridState', 'hidden');
            var unlockData = $("#tblstVillages").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            break;
        case 'H':

            var unlockData = $("#tblstHabs").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tblstHabs").jqGrid('setGridState', 'hidden');
            break;
        case 'T':

            var unlockData = $("#tbPropBatches").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tbPropBatches").jqGrid('setGridState', 'hidden');
            break;
        case 'Y':

            var unlockData = $("#tbPropYears").jqGrid('getGridParam', 'selarrrow');
            if (unlockData == "") {
                alert('Please select records to unlock.');
                return false;
            }
            $("#tbPropYears").jqGrid('setGridState', 'hidden');
            break;
        case 'R':
            var module = $("#ddlModule option:selected").val();
            switch (module) {
                case '2':

                    if ($("#RoleCode").val() == 36 || $("#RoleCode").val() == 47 || $("#RoleCode").val() == 56) {
                        var unlockData = $("#tblstProposalITNO").jqGrid('getGridParam', 'selarrrow');
                    }
                    else {
                        var unlockData = $("#tblstProposal").jqGrid('getGridParam', 'selarrrow');
                    }

                    if (unlockData == "") {
                        alert('Please select records to unlock.');
                        return false;
                    }
                    $("#tblstProposal").jqGrid('setGridState', 'hidden');
                    $("#tblstProposalITNO").jqGrid('setGridState', 'hidden');
                    break;
                case '4':

                    var unlockData = $("#tblstCoreNetwork").jqGrid('getGridParam', 'selarrrow');
                    if (unlockData == "") {
                        alert('Please select records to unlock.');
                        return false;
                    }
                    $("#tblstCoreNetwork").jqGrid('setGridState', 'hidden');
                    break;
                case '3':

                    var unlockData = $("#tblstExistingRoads").jqGrid('getGridParam', 'selarrrow');
                    if (unlockData == "") {
                        alert('Please select records to unlock.');
                        return false;
                    }
                    $("#tblstExistingRoads").jqGrid('setGridState', 'hidden');
                    break;
                case '8':

                    if ($("#RoleCode").val() == 36 || $("#RoleCode").val() == 47 || $("#RoleCode").val() == 56) {
                        var unlockData = $("#tblstProposalITNO").jqGrid('getGridParam', 'selarrrow');
                    }
                    else {
                        var unlockData = $("#tblstProposal").jqGrid('getGridParam', 'selarrrow');
                    }

                    if (unlockData == "") {
                        alert('Please select records to unlock.');
                        return false;
                    }
                    $("#tblstProposal").jqGrid('setGridState', 'hidden');
                    $("#tblstProposalITNO").jqGrid('setGridState', 'hidden');
                    break;
                default:
                    break;
            }
            break;
        default:
            break;
    }

    var data = $("#formModuleFilter").serialize();
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Unlock Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseUnlockDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {

        blockPage();

        //$("#divAddUnlockDetails").load('/LockUnlock/AddEditUnlockDetails/' + $.post({ moduleCode: $("#ddlModule option:selected").val(), levelCode: level, unlockData: unlockData, scheme: $("#ddlSchemeSearch option:selected").val() }), function () {
        //    $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
        //    unblockPage();
        //});

        $.post('/LockUnlock/AddEditUnlockDetails/', { moduleCode: $("#ddlModule option:selected").val(), levelCode: level, unlockData: unlockData, scheme: $("#ddlSchemeSearch option:selected").val(), yearbatch: $('#ddlStateSearch option:selected').val() + '$' + $('#ddlDistrictSearch option:selected').val() + '$' + $('#ddlBlockSearch option:selected').val() + '$' + $('#ddlYearSearch option:selected').val(), roleCode: $('#ddlRole option:selected').val() }, function (data) {
            $("#divAddUnlockDetails").html(data);
            $.validator.unobtrusive.parse($('#divAddLockDetailsForm'));
            unblockPage();
        });

        $('#divAddUnlockDetails').show('slow');
        $("#divAddUnlockDetails").css('height', 'auto');
    });
}
function CloseUnlockDetails() {
    var level = $("#ddlLevel").val();

    switch (level) {
        case 'S':
            $("#tblstState").jqGrid('setGridState', 'visible');
            break;
        case 'D':
            $("#tblstDistricts").jqGrid('setGridState', 'visible');
            break;
        case 'B':
            $("#tblstBlocks").jqGrid('setGridState', 'visible');
            break;
        case 'V':
            $("#tblstVillages").jqGrid('setGridState', 'visible');
            break;
        case 'H':
            $("#tblstHabs").jqGrid('setGridState', 'visible');
            break;
        case 'R':
            //$("#tbProposalList").jqGrid('setGridState', 'visible');
            var module = $("#ddlModule option:selected").val();
            switch (module) {
                case '2':
                    $("#tblstProposal").jqGrid('setGridState', 'visible');
                    $("#tblstProposalITNO").jqGrid('setGridState', 'visible');
                    break;
                case '4':
                    $("#tblstCoreNetwork").jqGrid('setGridState', 'visible');
                    break;
                case '3':
                    $("#tblstExistingRoads").jqGrid('setGridState', 'visible');
                    break;
                default:
                    break;
            }
    }
    $("#accordion").hide('slow');
}
function PopulateState() {


    $("#tblstState").jqGrid('GridUnload');

    jQuery("#tblstState").jqGrid({
        url: '/LockUnlock/GetStateList',
        datatype: "json",
        mtype: "POST",
        postData: { moduleCode: $("#ddlModule option:selected").val(), scheme: $("#ddlSchemeSearch option:selected").val(), RoleCode: $("#ddlRole option:selected").val(), collaboration: $("#ddlCollaboration option:selected").val() },
        colNames: ['States', 'Unlock Start Date', 'Unlock End Date', 'View'],
        colModel: [
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlistState').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_STATE_NAME',
        caption: "&nbsp;&nbsp; State List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tblstState")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tblstState")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tblstState")[0].p.selarrrow = $("#tblstState").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[1] != '-') {//update this to have your own check
                    var checkbox = $("#jqg_tblstState_" + rowData['id']);//update this with your own grid name
                    checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);
                }
            }

            if (data["records"] > 0) {

                $("#tblstState #pagerlistState").css({ height: '31px' });
                $("#pagerlistState_left").html("<input type='button' style='margin-left:25px' id='btnAdd' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddUnlockDetails();return false;' value='Unlock'/>")
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

function LoadUnlockedRecords() {
    $("#tbUnlockRecordList").jqGrid('GridUnload');

    jQuery("#tbUnlockRecordList").jqGrid({
        url: '/LockUnlock/GetUnlockRecordList',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlState option:selected").val(), ModuleCode: $("#ddlFilterModule option:selected").val() },
        colNames: ['Module Name', 'PMGSY Scheme', 'Unlock Level', 'Unlock Data', 'Unlock Start Date', 'Unlock End Date', 'Remarks'],
        colModel: [
                            { name: 'IMS_UNLOCK_TABLE', index: 'IMS_UNLOCK_TABLE', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_PMGSY_SCHEME', index: 'MAST_PMGSY_SCHEME', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'IMS_UNLOCK_LEVEL', index: 'IMS_UNLOCK_LEVEL', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'LEVEL_NAME', index: 'LEVEL_NAME', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_UNLOCK_REMARKS', index: 'IMS_UNLOCK_REMARKS', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgUnlockRecordList').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'IMS_UNLOCK_TABLE',
        caption: "&nbsp;&nbsp; Unlocked Record List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}

function ViewDetails(urlParam) {
    ViewDetailsToUnlock(urlParam);
}
function ViewDetailsToUnlock(urlparam) {
    var level = $("#ddlLevel option:selected").val();
    var module = $("#ddlModule option:selected").val();
    switch (module) {
        case "1":
            ViewHabDetails(urlparam, "HM");
            HideAllDetails();
            $("#dvHabDetails").show();
            break;
        case "2":

            if ($("#RoleCode").val() == 36 || $("#RoleCode").val() == 47 || $("#RoleCode").val() == 56) {
                ViewProposalDetailsForITNO(urlparam, "PR");
                HideAllDetails();
                $("#dvitnoProposalDetails").show();
            }
            else {
                ViewProposalDetails(urlparam, "PR");
                HideAllDetails();
                $("#dvProposalDetails").show();
            }
            break;
        case "3":
            ViewDRRPDetails(urlparam, "ER");
            HideAllDetails();
            $("#dvDRRPDetails").show();
            break;
        case "4":
            ViewCNDetails(urlparam, "CN");
            HideAllDetails();
            $("#dvCNDetails").show();
            break;
        case "5":
            ViewVillageDetails(urlparam, "VM");
            HideAllDetails();
            $("#dvVillageDetails").show();
            break;
        case "8":
            ViewProposalDetails(urlparam, "PR");
            HideAllDetails();
            $("#dvProposalDetails").show();
            break;
    }

}
function HideAllDetails() {
    $("#dvProposalDetails").hide();
    $("#dvCNDetails").hide();
    $("#dvDRRPDetails").hide();
    $("#dvVillageDetails").hide();
    $("#dvHabDetails").hide();
    $("#dvitnoProposalDetails").hide();
}
function ViewProposalDetails(urlparam, module) {
    $("#tblstPropView").jqGrid('GridUnload');

    jQuery("#tblstPropView").jqGrid({
        url: '/LockUnlock/GetProposalDetails',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val(), type: $("#ddlTypeSearch option:selected").val(), yearbatch: $('#ddlStateSearch option:selected').val() + '$' + $('#ddlDistrictSearch option:selected').val() + '$' + $('#ddlBlockSearch option:selected').val() + '$' + $('#ddlYearSearch option:selected').val() },
        colNames: ['Road/Bridge Name', 'Block', 'District', 'Package', 'Year', 'Batch', 'Road Length'],
        colModel: [
                            { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 150, align: "center", search: false },
        ],
        pager: jQuery('#dvpgDetails').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'ROAD_NAME',
        caption: "&nbsp;&nbsp; Proposal Details List",
        height: 'auto',
        autowidth: false,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}
function ViewProposalDetailsForITNO(urlparam, module) {
    $("#tbItnoProposalList").jqGrid('GridUnload');

    jQuery("#tbItnoProposalList").jqGrid({
        url: '/LockUnlock/GetProposalDetailsForITNO',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val() },
        colNames: ['Road/Bridge Name', 'Block', 'District', 'Package', 'Batch', 'Road Length'],
        colModel: [
                            { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 150, align: "center", search: false },
        ],
        pager: jQuery('#pgItnoProposalList').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'ROAD_NAME',
        caption: "&nbsp;&nbsp; Proposal Details List",
        height: 'auto',
        autowidth: false,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}


function ViewCNDetails(urlparam, module) {
    $("#tblstCNView").jqGrid('GridUnload');

    jQuery("#tblstCNView").jqGrid({
        url: '/LockUnlock/GetCoreNetworkDetails',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val() },
        colNames: ['Road Name', 'Road Number', 'Block', 'District', 'Road Type', 'Road Length'],
        colModel: [
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: false },
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'ROAD_TYPE', index: 'ROAD_TYPE', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#dvpgCNDetails').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'IMS_UNLOCK_TABLE',
        caption: "&nbsp;&nbsp; Core Network List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}
function ViewDRRPDetails(urlparam, module) {
    $("#tblstDRRPView").jqGrid('GridUnload');

    jQuery("#tblstDRRPView").jqGrid({
        url: '/LockUnlock/GetDRRPDetails',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val() },
        colNames: ['Road Name', 'Road Number', 'Block', 'District', 'Road Category', 'Road Owner'],
        colModel: [
                            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_ROAD_CAT_NAME', index: 'MAST_ROAD_CAT_NAME', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'MAST_AGENCY_NAME', index: 'MAST_AGENCY_NAME', height: 'auto', width: 250, align: "center", search: false },

        ],
        pager: jQuery('#dvpgDRRPDetails').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'IMS_UNLOCK_TABLE',
        caption: "&nbsp;&nbsp; Existing Road List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}
function ViewVillageDetails(urlparam, module) {
    $("#tblstVillageView").jqGrid('GridUnload');

    jQuery("#tblstVillageView").jqGrid({
        url: '/LockUnlock/GetVillageDetails',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val() },
        colNames: ['Village Name', 'Block', 'District', 'Is Schedule5', 'Total Population', 'Total SC/ST Population'],
        colModel: [
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IS_SCHEDULE5', index: 'IS_SCHEDULE5', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_VILLAGE_TOT_POP', index: 'MAST_VILLAGE_TOT_POP', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_VILLAGE_SCST_POP', index: 'MAST_VILLAGE_SCST_POP', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#dvpgVillageDetails').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_VILLAGE_NAME',
        caption: "&nbsp;&nbsp; Village Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}
function ViewHabDetails(urlparam, module) {
    $("#tblstHabView").jqGrid('GridUnload');

    jQuery("#tblstHabView").jqGrid({
        url: '/LockUnlock/GetHabitationDetails',
        datatype: "json",
        mtype: "POST",
        postData: { param: urlparam, module: module, scheme: $("#ddlSchemeSearch option:selected").val() },
        colNames: ['Habitation Name', 'MLA Constituency', 'MP Constituency', 'Is Schedule5', 'Connected/Unconnected', 'Total Population', 'SC/ST Population'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'MAST_MLA_CONST_NAME', index: 'MAST_MLA_CONST_NAME', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'MAST_MP_CONST_NAME', index: 'MAST_MP_CONST_NAME', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IS_SCHEDULE5', index: 'IS_SCHEDULE5', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'HAB_STATUS', index: 'HAB_STATUS', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', height: 'auto', width: 120, align: "center", search: false },
                            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#dvpgHabDetails').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'MAST_HAB_NAME',
        caption: "&nbsp;&nbsp; Habitation Details List",
        height: 'auto',
        autowidth: false,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {

        }
    });
}