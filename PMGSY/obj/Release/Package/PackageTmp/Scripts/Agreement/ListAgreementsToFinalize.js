$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFilterProposal'));

    $("#btnListAgreements").click(function () {

        
        if ($("#frmFilterAgreements").valid()) {
            LoadAgreementList();
        }
    });


    $("#ddlYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

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
function LoadAgreementList()
{
    $("#tblstAgreements").jqGrid('GridUnload');
    var pageWidth = jQuery("#tblstAgreements").parent().width() - 100;
   
    jQuery("#tblstAgreements").jqGrid({
        url: '/Agreement/GetAgreementDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { YearCode: $('#ddlYears option:selected').val(), BlockCode: $('#ddlBlocks option:selected').val(), Package: $('#ddlPackages option:selected').val(), ProposalType: $('#ddlProposalType option:selected').val(), AgreementStatus: $('#ddlAgreementStatus option:selected').val(), AgreementType: $('#ddlAgreementType option:selected').val(), Finalize: $('#ddlFinalize option:selected').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Type', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Finalize', 'DeFinalize', 'View','Change Status to Inprogress'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: (pageWidth * (5/100)), align: "left", sortable: false, hidden: true },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: (pageWidth * (8 / 100)), sortable: true, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: (pageWidth * (15 / 100)), sortable: true, resizable: false },
                            { name: 'AgreementType', index: 'AgreementType', width: (pageWidth * (12 / 100)), sortable: true, align: "left", resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: (pageWidth * (13 / 100)), sortable: true, resizable: false, align: 'center' },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: (pageWidth * (10 / 100)), sortable: false, align: "right", resizable: false },
                            { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: (pageWidth * (10 / 100)), sortable: false, align: "right", resizable: false },//, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: (pageWidth * (11 / 100)), sortable: false, align: "left", resizable: false },
                            { name: 'Finalize', index: 'Finalize', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false }, /* formatter: FormatColumnFinalize,*/
                            { name: 'DeFinalize', index: 'DeFinalize', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false },
                            { name: 'View', index: 'View', width: (pageWidth * (5 / 100)), sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
                            { name: 'Status', index: 'Status', width: (pageWidth * (5 / 100)), sortable: false, align: "center", resizable: false, hidden: true },
                            //{ name: 'Delete', index: 'Edit', width: 50, sortable: false, align: "center", formatter: FormatColumnDelete, resizable: false, hidden: true }
        ],
        pager: jQuery('#dvlstPagerAgreements'),
        rowNum: 15,
        rowList: [15, 20,25],
        viewrecords: true,
        recordtext: '{2} records found',
        // caption: "Agreement Details List",
        caption: "Agreement List",
        height: 'auto',
        width: 'auto',
        // autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        loadComplete: function () {

            var AgreementCode = $('#TendAgreementCode').val();

            // alert(AgreementCode);
            if (AgreementCode != '') {
                $("#tblstAgreements").expandSubGridRow(AgreementCode);
            }

            var reccount = $('#tblstAgreements').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerAgreements_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms. 3."NA"-Not Available  ]');
            }

        },
        loadError: function () { },
        subGrid: true,
        subGridRowExpanded: function (subgrid_id, row_id) {
            // we pass two parameters
            // subgrid_id is a id of the div tag created whitin a table data
            // the id of this elemenet is a combination of the "sg_" + id of the row
            // the row_id is the id of the row
            // If we wan to pass additinal parameters to the url we can use
            // a method getRowData(row_id) - which returns associative array in type name-value
            // here we can easy construct the flowing

            //alert(subgrid_id);

            //alert(row_id);
            //var b = getRowData(row_id);
            /* var a=$('#adminCategory').getRowData(row_id);
             alert(a['ADMIN_ND_NAME']);*/


            CollapseAllOtherRowsSubGrid(row_id);

            //alert(subgrid_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;

            //alert($('#tbAgreementDetailsList').getCell(row_id, 'AgreementCode'));

            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/Agreement/GetAgreementDetailsList_ByAgreementCode',
                //postData: { AgreementCode: row_id, IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                postData: { AgreementCode: $('#tblstAgreements').getCell(row_id, 'AgreementCode'), IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
                datatype: "json",
                mtype: "POST",
                colNames: ['Year','Package','Road/Bridge Name', 'Work Name', 'Part Agreement', 'Start Chainage', 'End Chainage', 'Road / Bridge Amount', 'Maintenance Amount', 'Agreement Status', 'Value of Work Done', 'Incomplete Reason'],
                colModel: [

                             { name: 'Year', index: 'Year', height: 'auto', width: 100, align: "left", sortable: false },
                             { name: 'Package', index: 'Package', height: 'auto', width: 110, align: "left", sortable: false },
                             { name: 'RoadName', index: 'RoadName', height: 'auto', width: 110, align: "left", sortable: true },
                             { name: 'WorkName', index: 'WorkName', height: 'auto', width: 80, align: "left", sortable: false},//, hidden: isHiddenWorkName },
                             { name: 'PartAgreement', index: 'PartAgreement', height: 'auto', width: 60, align: "left", sortable: true },// hidden: isHiddenPartAgreement },
                             { name: 'StartChainage', index: 'StartChainage', height: 'auto', width: 60, align: "right", sortable: false},// hidden: isHiddenStartChainage },
                             { name: 'EndChainage', index: 'EndChainage', height: 'auto', width: 60, align: "right", sortable: false },// hidden: isHiddenEndChainage },
                             { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 100, sortable: false, align: "right" },
                             { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 100, sortable: false, align: "right"},// hidden: isHiddenMaintenenceCost },
                             { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 100, sortable: false, align: "left" },
                             { name: 'WorkDone', index: 'WorkDone', height: 'auto', width: 50, align: "left", sortable: false },
                             { name: 'IncompleteReason', index: 'IncompleteReason', height: 'auto', width: 100, align: "left", sortable: false },
                             //{ name: 'Change Status To Complete', index: 'Change Status To Complete', width: 70, sortable: false, formatter: FormatColumnChangeStatusToComplete, align: "center"},// hidden: isHidden },
                             //{ name: 'Change Status', index: 'Change Status', width: 70, sortable: false, formatter: FormatColumnChangeStatus, align: "center" },

                             //{ name: 'Edit', index: 'Edit', width: 40, sortable: false, formatter: FormatColumnEdit_AgreementDetails, align: "center" },
                             //{ name: 'Delete', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnDelete_AgreementDetails }

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
            //expand all rows on load
            "expandOnLoad": false
        },

        onSelectRow: function (id) {

            $('#TendAgreementCode').val(id);
            // var userCode = $(this).jqGrid('getCell', 'userCode');
            //alert(userCode);
        }
    });
}
function CollapseAllOtherRowsSubGrid(rowid) {
    var rowIds = $("#tblstAgreements").getDataIDs();
    $.each(rowIds, function (index, rowId) {
        $("#tblstAgreements").collapseSubGridRow(rowId);
    });
}
function FinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/FinalizeAgreement/" + urlparameter,
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
