$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    ModulewiseFileCountList();

});

function ModulewiseFileCountList() {

    $("#tbFileCountList").jqGrid({

        url: '/UserManager/GetProposalDataGapList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["Report No.", "Report Name", "Proposal Count"],
        colModel: [
                    { name: 'RptNo', index: 'RptNo', width: 50, align: "center" },
                    { name: 'RptName', index: 'RptName', width: 500, align: "center" },
                    { name: 'Proposals', index: 'Proposals', width: 200, align: "center" }

        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 20,
        rowList: [15, 20, 25, 30],
        pager: '#pgPropDataList',
        sortname: 'S.No.',
        sortorder: 'asc',
        autoWidth: true,
        shrinkToFit: false,
        loadComplete: function (rowid) {
        },
        onSelectRow: function (rowid) {
            ProposalDataGapDetailsListGrid(rowid);
        },
        caption: "Proposal Data Gap List"
    });
}
function ProposalDataGapDetailsListGrid(rowId) {

    $("#tbPropDataDetailsList").jqGrid('GridUnload');

    $("#tbPropDataDetailsList").jqGrid({

        url: '/UserManager/GetProposalDataGapDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { RptNo: rowId },
        loadError: function (r, st, error) {
        },
        height: '200px',
        rowNum: 30,
        colNames: ["State Code", "State Name", "District Code", 'District Name', 'Block Code', 'Block Name', 'Road Code', 'DPIU Code', 'DPIU Name', 'Year', 'Batch',
                    'Package Status', 'Package Id', 'Proposal Type', 'Connnectivity Type',
                    'Collaboration', 'Road Name', 'Road From', 'Road To', 'Pavement Length', 'Partial/Full', 'BT', 'CC', 'BridgeWorks',
                    'Bridge Name', 'Bridge Length', 'CN Code', 'CN Name', 'Surface Name', 'Proposed Surface Status',
                    'Carriage Width', 'Traffic Type', 'Stream Name', 'ZP Reso', 'CD WOrks', 'Higher Spec Status',
                    'Share %', 'Stage Status', 'Stage Phase', 'Stage Year', 'Stage Package', 'Stage Road', 'Old Block Code', 'Old Block Name', 'Old Package',
                    'Old Road Id', 'MP Const Name', 'MLA Const Name', 'Is Benefitted Hab', 'Hab Reason', 'STA Status',
                    'Sta Sanctioned By', 'STA Sanction Date', 'STA Remarks', 'PTA Status', 'PTA Name', 'PTA Sanction Date', 'PTA Remarks', 'MRD Status', 'Reason',
                    'Sanction By', 'Sanction Date', 'Pav Amt', 'CD Amt', 'PW Amt', 'OW Amt', 'HS Amt', 'FC Amt', 'BW Amt', 'RS Amt', 'BS Amt', 'Year1', 'Year2',
                    'Year3', 'Year4', 'Year5', 'Renewal Amt', 'Remarks', 'Exec Remarks', 'Prog Remarks', 'Value Work Done', 'Payment Made', 'Final Payment',
                    'Payment Date', 'Fin Date', 'Phy Date', 'Lock Status', 'Freeze Status', 'Shift Status', 'DPR Status', 'IS Completed', 'Scheme'],
        colModel: [
                    { name: 'StateCode', index: 'StateCode', align: "center", width: '50px' },
                    { name: 'StateName', index: 'StateName', align: "center", width: '100px', search: true },
                    { name: 'DistrictCode', index: 'DistrictCode', align: "center", width: '50px' },
                    { name: 'DistrictName', index: 'DistrictName', align: "center", width: '100px', search: true },
                    { name: 'BlockCode', index: 'BlockCode', align: "center", width: '50px' },
                    { name: 'BlockName', index: 'BlockName', align: "center", width: '100px', search: true },
                    { name: 'RoadCode', index: 'RoadCode', align: "center", width: '50px' },
                    { name: 'DPIUCode', index: 'DPIUCode', align: "center", width: '50px' },
                    { name: 'DPIUName', index: 'DPIUName', align: "center", width: '100px', search: true },
                    { name: 'Year', index: 'Year', align: "center", width: '50px', search: true },
                    { name: 'Batch', index: 'Batch', align: "center", width: '50px', search: true },
                    { name: 'PackageStatus', index: 'PackageStatus', align: "center", width: '80px' },
                    { name: 'PackageId', index: 'PackageId', align: "center", width: '80px', search: true },
                    { name: 'Proposal Type', index: 'Proposal Type', align: "center", width: '80px' },
                    { name: 'ConnectivityType', index: 'ConnectivityType', align: "center", width: '80px' },
                    { name: 'Collaboration', index: 'Collaboration', align: "center", width: '50px' },
                    { name: 'RoadName', index: 'RoadName', align: "center", width: '50px', search: true },
                    { name: 'RoadFrom', index: 'RoadFrom', align: "center", width: '50px', search: true },
                    { name: 'RoadTo', index: 'RoadTo', align: "center", width: '50px', search: true },
                    { name: 'PavLength', index: 'PavLength', align: "center", width: '50px' },
                    { name: 'PartialLength', index: 'PartialLength', align: "center", width: '50px' },
                    { name: 'BT', index: 'BT', align: "center", width: '50px' },
                    { name: 'CC', index: 'CC', align: "center", width: '50px' },
                    { name: 'LSB', index: 'LSB', align: "center", width: '50px' },
                    { name: 'LSBName', index: 'LSBName', align: "center", width: '50px', search: true },
                    { name: 'LSBLength', index: 'LSBLength', align: "center", width: '50px', search: true },
                    { name: 'CNCode', index: 'CNCode', align: "center", width: '50px' },
                    { name: 'CNName', index: 'CNName', align: "center", width: '50px', search: true },
                    { name: 'SurfaceName', index: 'SurfaceName', align: "center", width: '50px' },
                    { name: 'ProposedSurfaceName', index: 'ProposedSurfaceName', align: "center", width: '50px' },
                    { name: 'CarriageWidth', index: 'CarriageWidth', align: "center", width: '50px' },
                    { name: 'TrafficType', index: 'TrafficType', align: "center", width: '50px' },
                    { name: 'StreamName', index: 'StreamName', align: "center", width: '50px' },
                    { name: 'ZPReso', index: 'ZPReso', align: "center", width: '50px' },
                    { name: 'CDWorks', index: 'CDWorks', align: "center", width: '50px' },
                    { name: 'HSStatus', index: 'HSStatus', align: "center", width: '50px' },
                    { name: 'Share%', index: 'Share%', align: "center", width: '50px' },
                    { name: 'StageStatus', index: 'StageStatus', align: "center", width: '50px' },
                    { name: 'StagePhase', index: 'StagePhase', align: "center", width: '50px' },
                    { name: 'StageYear', index: 'StageYear', align: "center", width: '50px' },
                    { name: 'StagePackage', index: 'StagePackage', align: "center", width: '50px' },
                    { name: 'StageRoad', index: 'StageRoad', align: "center", width: '50px' },
                    { name: 'OldBlockCode', index: 'OldBlockCode', align: "center", width: '50px' },
                    { name: 'OldBlockName', index: 'OldBlockName', align: "center", width: '50px' },
                    { name: 'OldPackage', index: 'OldPackage', align: "center", width: '50px' },
                    { name: 'OldRoadId', index: 'OldRoadId', align: "center", width: '50px' },
                    { name: 'MPConstName', index: 'MPConstName', align: "center", width: '50px' },
                    { name: 'MLAConstName', index: 'MLAConstName', align: "center", width: '50px' },
                    { name: 'IsHabBen', index: 'IsHabBen', align: "center", width: '50px' },
                    { name: 'HabReason', index: 'HabReason', align: "center", width: '50px' },
                    { name: 'StaStatus', index: 'StaStatus', align: "center", width: '50px' },
                    { name: 'StaName', index: 'StaName', align: "center", width: '50px' },
                    { name: 'StaDate', index: 'StaDate', align: "center", width: '50px' },
                    { name: 'StaRemarks', index: 'StaRemarks', align: "center", width: '50px' },
                    { name: 'PtaStatus', index: 'PtaStatus', align: "center", width: '50px' },
                    { name: 'PtaName', index: 'PtaName', align: "center", width: '50px' },
                    { name: 'PtaDate', index: 'PtaDate', align: "center", width: '50px' },
                    { name: 'PtaRemarks', index: 'PtaRemarks', align: "center", width: '50px' },
                    { name: 'MRDStatus', index: 'MRDStatus', align: "center", width: '50px' },
                    { name: 'Reason', index: 'Reason', align: "center", width: '50px' },
                    { name: 'MRDName', index: 'MRDName', align: "center", width: '50px' },
                    { name: 'MRDDate', index: 'MRDDate', align: "center", width: '50px' },
                    { name: 'PavAmt', index: 'Pav Amt', align: "center", width: '50px' },
                    { name: 'CDAmt', index: 'CDAmt', align: "center", width: '50px' },
                    { name: 'PWAmt', index: 'PWAmt', align: "center", width: '50px' },
                    { name: 'OWAmt', index: 'OWAmt', align: "center", width: '50px' },
                    { name: 'HSAmt', index: 'HSAmt', align: "center", width: '50px' },
                    { name: 'FCAmt', index: 'FCAmt', align: "center", width: '50px' },
                    { name: 'BWAmt', index: 'BWAmt', align: "center", width: '50px' },
                    { name: 'RSAmt', index: 'RSAmt', align: "center", width: '50px' },
                    { name: 'BSAmt', index: 'BSAmt', align: "center", width: '50px' },
                    { name: 'Year1', index: 'Year1', align: "center", width: '50px' },
                    { name: 'Year2', index: 'Year2', align: "center", width: '50px' },
                    { name: 'Year3', index: 'Year3', align: "center", width: '50px' },
                    { name: 'Year4', index: 'Year4', align: "center", width: '50px' },
                    { name: 'Year5', index: 'Year5', align: "center", width: '50px' },
                    { name: 'Renewal', index: 'Renewal', align: "center", width: '50px' },
                    { name: 'Remarks', index: 'Remarks', align: "center", width: '50px' },
                    { name: 'ExecRemarks', index: 'ExecRemarks', align: "center", width: '50px' },
                    { name: 'ProgRemarks', index: 'ProgRemarks', align: "center", width: '50px' },
                    { name: 'ValueWorkDone', index: 'ValueWorkDone', align: "center", width: '50px' },
                    { name: 'PaymentMade', index: 'PaymentMade', align: "center", width: '50px' },
                    { name: 'FinalPayment', index: 'FinalPayment', align: "center", width: '50px' },
                    { name: 'PaymentDate', index: 'PaymentDate', align: "center", width: '50px' },
                    { name: 'FinDate', index: 'FinDate', align: "center", width: '50px' },
                    { name: 'PhyDate', index: 'PhyDate', align: "center", width: '50px' },
                    { name: 'LockStatus', index: 'LockStatus', align: "center", width: '50px' },
                    { name: 'FreezeStatus', index: 'FreezeStatus', align: "center", width: '50px' },
                    { name: 'ShiftStatus', index: 'ShiftStatus', align: "center", width: '50px' },
                    { name: 'DPRStatus', index: 'DPRStatus', align: "center", width: '50px' },
                    { name: 'IsCompleted', index: 'IsCompleted', align: "center", width: '50px' },
                    { name: 'Scheme', index: 'Scheme', align: "center", width: '50px' },

        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 200,
        rowList: [200, 300, 400, 500],
        pager: '#pgPropDataDetailsList',
        sortname: 'S.No.',
        sortorder: 'asc',
        //width:'200px',
        autowidth: true,
        shrinkToFit: false,
        loadComplete: function (rowid) {
        },
        onSelectRow: function (rowid) {
            ViewProposalDetails(rowid);
        },
        caption: "Proposal Data Gap Details List"
    });

    jQuery("#tbPropDataDetailsList").jqGrid('filterToolbar', {
        autosearch: true,
        searchOnEnter: true,
        defaultSearch: false
    });
}
function ViewProposalDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fast', function () {

        blockPage();

        $('#dvPropDetails').load('/Proposal/Details/' + id, function () {
            $('#dvGapDetails').hide('slow');
            unblockPage();
        });
    });

}
function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#dvGapDetails').show('slow');
}

