$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFilterProposal'));

    $("#btnListAgreements").click(function () {


        if ($("#frmFilterAgreements").valid()) {

            if ($('#ddlStates option:selected').val() <= 0)
            {
                alert('Please select State and District');
                return false;
            }

            if ($('#ddlAgreementType option:selected').val() == '0')
            {
                alert('Please select Agreement Type');
                return false;
            }

            LoadAgreementList();
        }
    });

    $('#ddlStates').change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/Agreement/GetDistrictsByState?stateCode=" + $('#ddlStates option:selected').val());
    });


    $("#ddlYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#ddlDistricts").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlBlocks", "/Agreement/GetBlocksByDistricts?districtCode=" + $('#ddlDistricts option:selected').val());



    }); //end function district change

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#dvViewAgreementMaster").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Agreement Details'
    });

});
function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()
function LoadAgreementList() {
    $("#tblstAgreements").jqGrid('GridUnload');
    jQuery("#tblstAgreements").jqGrid({
        url: '/Agreement/GetAgreementDetailsListForUpdation',
        datatype: "json",
        mtype: "POST",
        postData: { YearCode: $('#ddlYears option:selected').val(), BlockCode: $('#ddlBlocks option:selected').val(), Package: $('#ddlPackages option:selected').val(), ProposalType: $('#ddlProposalType option:selected').val(), AgreementStatus: $('#ddlAgreementStatus option:selected').val(), AgreementType: $('#ddlAgreementType option:selected').val(), Finalize: $('#ddlFinalize option:selected').val(), State: $('#ddlStates option:selected').val(), District: $('#ddlDistricts option:selected').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Type', 'Agreement Amount','Proposal Name','Proposal Length','Batch','Year','Package', 'Maintenance Amount', 'Agreement Status', 'Change Status to Inprogress'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: 50, align: "left", sortable: false, hidden: true },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: 200, sortable: true, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 120, sortable: true, resizable: false },
                            { name: 'AgreementType', index: 'AgreementType', width: 60, sortable: true, align: "left", resizable: false },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 60, sortable: false, align: "right", resizable: false },
                            { name: 'ProposalName', index: 'ProposalName', width: 250, sortable: true, align: "left", resizable: false },
                            { name: 'ProposalLength', index: 'ProposalLength', width: 60, sortable: true, align: "left", resizable: false },
                            { name: 'Batch', index: 'Batch', width: 60, sortable: false, align: "center", resizable: false },
                            { name: 'Year', index: 'Year', width: 60, sortable: false, align: "center", resizable: false },
                            { name: 'Package', index: 'Package', width: 50, sortable: false, align: "center", resizable: false },
                            { name: 'MaintenanceAmount', index: 'MaintenanceAmount', width: 60, sortable: false, align: "center", resizable: false },
                            { name: 'AgreementStatus', index: 'AgreementStatus', width: 60, sortable: false, align: "center", resizable: false },
                            { name: 'View', index: 'View', width: 50, sortable: false, align: "center", resizable: false },
        ],
        pager: jQuery('#dvlstPagerAgreements'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Agreement List",
        height: 'auto',
        width: 'auto',
        rownumbers: true,
        hidegrid: false,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        loadComplete: function () {

            var AgreementCode = $('#TendAgreementCode').val();

            if (AgreementCode != '') {
                $("#tblstAgreements").expandSubGridRow(AgreementCode);
            }

            var reccount = $('#tblstAgreements').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerAgreements_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms. 3."NA"-Not Available  ]');
            }

        },
        loadError: function () { },
        subGrid: false,
        subGridRowExpanded: function (subgrid_id, row_id) {
            
            CollapseAllOtherRowsSubGrid(row_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;

            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/Agreement/GetAgreementDetailsList_ByAgreementCode',
                postData: { AgreementCode: $('#tblstAgreements').getCell(row_id, 'AgreementCode'), IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                datatype: "json",
                mtype: "POST",
                colNames: ['Year', 'Package', 'Road/Bridge Name', 'Work Name', 'Part Agreement', 'Start Chainage', 'End Chainage', 'Road / Bridge Amount', 'Maintenance Amount', 'Agreement Status', 'Value of Work Done', 'Incomplete Reason'],
                colModel: [

                             { name: 'Year', index: 'Year', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Package', index: 'Package', height: 'auto', width: 110, align: "left", sortable: false },
                             { name: 'RoadName', index: 'RoadName', height: 'auto', width: 110, align: "left", sortable: true },
                             { name: 'WorkName', index: 'WorkName', height: 'auto', width: 80, align: "left", sortable: false },
                             { name: 'PartAgreement', index: 'PartAgreement', height: 'auto', width: 60, align: "left", sortable: true },
                             { name: 'StartChainage', index: 'StartChainage', height: 'auto', width: 60, align: "right", sortable: false },
                             { name: 'EndChainage', index: 'EndChainage', height: 'auto', width: 60, align: "right", sortable: false },
                             { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 100, sortable: false, align: "right" },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 100, sortable: false, align: "right" },
                             { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 100, sortable: false, align: "left" },
                             { name: 'WorkDone', index: 'WorkDone', height: 'auto', width: 50, align: "left", sortable: false },
                             { name: 'IncompleteReason', index: 'IncompleteReason', height: 'auto', width: 100, align: "left", sortable: false },
                ],
                rowNum: 5,
                pager: pager_id,
                height: 'auto',
                autowidth: true,
                rownumbers: true,
                rowList: [5, 10],
                viewrecords: true,
                sortname: 'RoadName',
                sortorder: "asc",
                recordtext: '{2} records found',
                onSelectRow: function () {
                    $('#TendAgreementCode').val(row_id);
                }
            });
        },
        subGridOptions: {
            "plusicon": "ui-icon-triangle-1-s",
            "minusicon": "ui-icon-triangle-1-n",
            "openicon": "ui-icon-arrowreturn-1-e",
            "expandOnLoad": false
        },
        onSelectRow: function (id) {
            $('#TendAgreementCode').val(id);
        }
    });
}
function CollapseAllOtherRowsSubGrid(rowid) {
    var rowIds = $("#tblstAgreements").getDataIDs();
    $.each(rowIds, function (index, rowId) {
        $("#tblstAgreements").collapseSubGridRow(rowId);
    });
}
function ChangeAgreementStatus(urlparameter) {

    if (confirm("Are you sure you want to change agreement status?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/ChangeAgreementStatus/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblstAgreements").trigger('reloadGrid');
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function DeFinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'DeFinalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/DeFinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblstAgreements").trigger('reloadGrid');
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}
function FormatColumnView(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Agreement Details' onClick ='ViewAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}
function ViewAgreementMasterDetails(urlparameter) {

    var EncryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/ViewAgreementMasterDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvViewAgreementMaster").html(data);

            $("#dvViewAgreementMaster").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}