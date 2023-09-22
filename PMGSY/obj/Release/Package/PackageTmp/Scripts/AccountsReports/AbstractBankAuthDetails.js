

$(document).ready(function () {

    LoadAbstractBankAuthList();
        
});


function LoadAbstractBankAuthList()
{
    //blockPage();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    YEAR = $("#ddlYear").val();
    STATE_SRRDA = $("#ddlStateSRRDA").val();
    DPIU = $("#ddlDPIU").val();
    NewYear = parseInt(YEAR) + 1;
    

    jQuery("#tblAbstractBankAuthList").jqGrid({
        url: '/AccountsReports/ListAbstractBankAuthDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Name of DPIU', 'Ledger Folio', 'April &nbsp;' + YEAR + '', 'May &nbsp;' + YEAR + '', 'June &nbsp;' + YEAR + '', 'July &nbsp;' + YEAR + '', 'Augest &nbsp;' + YEAR + '', 'September &nbsp;' + YEAR + '', 'Octomber &nbsp;' + YEAR + '', 'November &nbsp;' + YEAR + '', 'December &nbsp;' + YEAR + '', 'January &nbsp;' + NewYear + '', 'February &nbsp;' + NewYear + '', 'March &nbsp;' + NewYear + ''],
        colModel: [
            { name: 'NameOfDPIU', index: 'NameOfDPIU', width: '170px', sortable: false, align: 'left' },
            { name: 'LedgerFolio', index: 'LedgerFolio', width: '50px', sortable: false, align: 'left' },
            { name: 'AprilAmt', index: 'AprilAmt', width: '110px', sortable: false, align: 'right',formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'MayAmt', index: 'MayAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'JunAmt', index: 'JunAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'JulyAmt', index: 'JulyAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'AugAmt', index: 'AugAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'SeptAmt', index: 'SeptAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'OctAmt', index: 'OctAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'NovAmt', index: 'NovAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'DecAmt', index: 'DecAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'JanAmt', index: 'JanAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'FebAmt', index: 'FebAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } },
            { name: 'MarchAmt', index: 'MarchAmt', width: '110px', sortable: false, align: 'right', formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        pager: $("#dvAbstractBankAuthPager"),
        postData: {YEAR:YEAR,STATE_SRRDA:STATE_SRRDA,DPIU:DPIU,value:Math.random()},
        sortOrder: 'asc',
        sortname: 'NameOfDPIU',
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        rownumbers: true,
        hidegrid: true,
        footerrow: true,
        userDataOnFooter:true,
        viewrecords: true,
        recordText: '{2} records found',
        caption: 'Abstract of Outstanding Bank Authorization Details',
        height: 'auto',
        shrinkToFit:false,
        autowidth:true,
        width: '100%',
        loaderror: function (xhr, status, error) {

            if (xhr.reponseText == "session expired")
            {
                window.location.href = "/Login/Login";
            }
            unblockPage();
        },
        loadComplete: function (data) {

            var recordCount = $("#tblAbstractBankAuthList").jqGrid('getGridParam', 'reccount');

            if (recordCount > 0) {

                if (recordCount > 25) {
                    $("#tblAbstractBankAuthList").jqGrid('setGridHeight', '320');
                }
                else {
                    $("#tblAbstractBankAuthList").jqGrid('setGridHeight', 'auto');
                }

                $("#tblAbstractBankAuthList").footerData('set', { "NameOfDPIU": "Total" }, true);

                var grid = $("#tblAbstractBankAuthList");

                aprTotal = grid.jqGrid('getCol', 'AprilAmt', false, 'sum');
                mayTotal = grid.jqGrid('getCol', 'MayAmt', false, 'sum');
                junTotal = grid.jqGrid('getCol', 'JunAmt', false, 'sum');
                julyTotal = grid.jqGrid('getCol', 'JulyAmt', false, 'sum');
                augTotal = grid.jqGrid('getCol', 'AugAmt', false, 'sum');
                sepTotal = grid.jqGrid('getCol', 'SeptAmt', false, 'sum');
                octTotal = grid.jqGrid('getCol', 'OctAmt', false, 'sum');
                novTotal = grid.jqGrid('getCol', 'NovAmt', false, 'sum');
                decTotal = grid.jqGrid('getCol', 'DecAmt', false, 'sum');
                janTotal = grid.jqGrid('getCol', 'JanAmt', false, 'sum');
                febTotal = grid.jqGrid('getCol', 'FebAmt', false, 'sum');
                marchTotal = grid.jqGrid('getCol', 'MarchAmt', false, 'sum');

                //if (aprTotal % 1 == 0)
                //{
                //    aprTotal += ".00";
                //}
                //if (mayTotal % 1 == 0) {
                //    mayTotal += ".00";
                //}
                //if (junTotal % 1 == 0) {
                //    junTotal += ".00";
                //}
                //if (julyTotal % 1 == 0) {
                //    julyTotal += ".00";
                //}
                //if (augTotal % 1 == 0) {
                //    augTotal += ".00";
                //}
                //if (sepTotal % 1 == 0) {
                //    sepTotal += ".00";
                //}
                //if (octTotal % 1 == 0) {
                //    octTotal += ".00";
                //}
                //if (novTotal % 1 == 0) {
                //    novTotal += ".00";
                //}
                //if (decTotal % 1 == 0) {
                //    decTotal += ".00";
                //}
                //if (janTotal % 1 == 0) {
                //    janTotal += ".00";
                //}
                //if (febTotal % 1 == 0) {
                //    febTotal += ".00";
                //}
                //if (marchTotal % 1 == 0) {
                //    marchTotal += ".00";
                //}

                grid.jqGrid('footerData', 'set', { ID: 'ftrAprTotal', AprilAmt: aprTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrMayTotal', MayAmt: mayTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrJunTotal', JunAmt: junTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrJulyTotal', JulyAmt: julyTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrAugTotal', AugAmt: augTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrSepTotal', SeptAmt: sepTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrOctTotal', OctAmt: octTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrNovTotal', NovAmt: novTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrDecTotal', DecAmt: decTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrJanTotal', JanAmt: janTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrFebTotal', FebAmt: febTotal });
                grid.jqGrid('footerData', 'set', { ID: 'ftrMarchTotal', MarchAmt: marchTotal });
                $("#dvAbstractBankAuthPager_left").html('<b>( Note: All amounts are in Rs. )</b>');

            }
            //unblockPage();
            $.unblockUI();
        },
    });

    $("#tblAbstractBankAuthList").jqGrid('setGroupHeaders', {

        useColSpanStyle: false,
        groupHeaders: [
            { startColumnName: 'AprilAmt', numberOfColumns: 12, titleText: $("#FundTypeName").val() + ' Outstanding Authorization at the end of ' + $("#ddlYear option:selected").text() }
        ]
    });
    $.unblockUI();
}