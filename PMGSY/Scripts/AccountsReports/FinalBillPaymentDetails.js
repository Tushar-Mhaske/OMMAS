

$(document).ready(function () {

    LoadFinalBillPaymentCompletedList();



    var $grid = $("#tblFinalBillPaymentCompletedList"); // your grid

    $($grid[0].grid.cDiv).click(function () {
        var $mygrid = $(this).closest(".ui-jqgrid-view")
                          .find(">.ui-jqgrid-bdiv>div>.ui-jqgrid-btable"),
            gridstate = $mygrid.jqGrid("getGridParam", "gridstate");
    });

    $grid.bind("jqGridHeaderClick", function (e, gridstate) {
        if (gridstate == "visible") {
            $("#tblFinalBillPaymentPendingList").jqGrid('setGridState', 'hidden');
        }
        else {
            $("#tblFinalBillPaymentPendingList").jqGrid('setGridState', 'visible');
        }
    });
    
        //if ($("#tblFinalBillPaymentCompletedList").jqGrid('getGridParam', 'gridstate') == "hidden") {
        //    alert("hiden");
        //    $("#tblFinalBillPaymentPendingList").jqGrid('setGridState', 'visible');

        //}
        //else {
        //    alert("show");
        //}

        //$("#mySearchResultsGrid").trigger("reloadGrid");
    
});

function LoadFinalBillPaymentCompletedList()
{   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    State = $("#ddlState option:selected").val();
    Agency = $("#ddlAgency option:selected").val();
    FundingAgency = $("#ddlFundingAgency option:selected").val();
    
    jQuery("#tblFinalBillPaymentCompletedList").jqGrid({
        url: '/AccountsReports/ListFinalBillPaymentCompletedDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Sanction Year', 'Sanctioned Works', 'Physically Completed Works', 'Financial Completion Of Physically Completed Works', 'Pending Financial Completion Of Physically Completed Works'],
        colModel: [
            { name: 'SanctionedYear', index: 'SanctionedYear', width: '170px', sortable:true, align: 'center' },
            { name: 'SanctionedWorks', index: 'SanctionedWorks', width: '170px', sortable: true, align: 'center' },
            { name: 'PhyCompletedWorks', index: 'PhyCompletedWorks', width: '170px', sortable: true, align: 'center' },
            { name: 'FinCompOfPhyCompletedWorks', index: 'FinCompOfPhyCompletedWorks', width: '170px', sortable: true, align: 'center' },
            { name: 'PendingFinCompOfPhyCompletedWorks', index: 'PendingFinCompOfPhyCompletedWorks', width: '170px', sortable: false, align: 'center' },
        ],
        pager: $("#dvFinalBillPaymentCompletedPager"),
        postData: { State: State, Agency: Agency, FundingAgency: FundingAgency, value: Math.random() },
        sortOrder: 'desc',
        sortname: 'SanctionedYear',
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        rownumbers: true,
        hidegrid: true,
        footerrow: true,
        userDataOnFooter:true,
        viewrecords: true,
        recordText: '{2} records found',
        caption: 'Financial Completion of Works As Per Accounts',
        height: 'auto',
        //shrinkToFit:false,
        autowidth:true,
        width: '100%',
        loaderror: function (xhr, status, error) {
            $.unblockUI();
            if (xhr.reponseText == "session expired")
            {
                window.location.href = "/Login/Login";
            }            
        },
        loadComplete: function (data) {

            var recordCount = $("#tblFinalBillPaymentCompletedList").jqGrid('getGridParam', 'reccount');

            if (recordCount > 0) {

                if (recordCount > 15) {
                    $("#tblFinalBillPaymentCompletedList").jqGrid('setGridHeight', '320');
                }
                else {
                    $("#tblFinalBillPaymentCompletedList").jqGrid('setGridHeight', 'auto');
                }
                $("#tblFinalBillPaymentCompletedList").footerData('set', { "SanctionedYear": "Total" }, true);

                var grid = $("#tblFinalBillPaymentCompletedList");

                TotalSanctionedWorks = grid.jqGrid('getCol', 'SanctionedWorks', false, 'sum')
                TotalPhyCompletedWorks = grid.jqGrid('getCol', 'PhyCompletedWorks', false, 'sum')
                TotalFinCompOfPhyCompletedWorks = grid.jqGrid('getCol', 'FinCompOfPhyCompletedWorks', false, 'sum')
                
                grid.jqGrid('footerData', 'set', { ID: 'SanctionedWorks', SanctionedWorks: TotalSanctionedWorks });
                grid.jqGrid('footerData', 'set', { ID: 'PhyCompletedWorks', PhyCompletedWorks: TotalPhyCompletedWorks });
                grid.jqGrid('footerData', 'set', { ID: 'FinCompOfPhyCompletedWorks', FinCompOfPhyCompletedWorks: TotalFinCompOfPhyCompletedWorks });
                
                //$("#dvAbstractBankAuthPager_left").html('<b>( Note: All amounts are in Rs. )</b>');

            }            
            $.unblockUI();
        },
    });
}


function ShowDetails(parameters)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tblFinalBillPaymentCompletedList").jqGrid('setGridState', 'hidden');
    
    jQuery("#tblFinalBillPaymentPendingList").jqGrid('GridUnload');
    
    jQuery("#tblFinalBillPaymentPendingList").jqGrid({
        url: '/AccountsReports/ListFinalBillPaymentPenddingDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District', 'Package Number', 'Road Name', 'Financial Road Category', 'Pavement Length ( Kms ) ','Sanction Cost (Rs. Lacs)','Expenditure (Rs. Lacs)'],
        colModel: [
            { name: 'DistrictName', index: 'DistrictName', width: '120px', sortable: false, align: 'center'},
            { name: 'PackageNumber', index: 'PackageNumber', width: '120px', sortable: false, align: 'center'},
            { name: 'RoadName', index: 'RoadName', width: '270px', sortable: false, align: 'center' },
            { name: 'FinancialRoadCategory', index: 'FinancialRoadCategory', width: '140px', sortable: false, align: 'center' },
            { name: 'PavementLength', index: 'PavementLength', width: '140px', sortable: false, align: 'center', formatter: 'number', formatOptions: { decimalPlaccess: 2 } },
            { name: 'SanctionCost', index: 'SanctionCost', width: '140px', sortable: false, align: 'center', formatter: 'number', formatOptions: { decimalPlaccess: 2 } },
            { name: 'Expenditure', index: 'Expenditure', width: '140px', sortable: false, align: 'center', formatter: 'number', formatOptions: { decimalPlaccess: 2 } },
        ],
        pager: $("#dvFinalBillPaymentPendingPager"),
        postData: { Parameters: parameters, value: Math.random() },
        sortOrder: 'asc',
        sortname: 'DistrictName',
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        rownumbers: true,
        hidegrid: true,
        footerrow: true,
        userDataOnFooter: true,
        viewrecords: true,
        recordText: '{2} records found',
        caption: 'Pending Financial Completion of Physically Completed Works',
        height: 'auto',
        //shrinkToFit:false,
        autowidth: true,
        width: '100%',
        loaderror: function (xhr, status, error) {
            $.unblockUI();
            if (xhr.reponseText == "session expired") {
                window.location.href = "/Login/Login";
            }
        },
        loadComplete: function (data) {
         
            jQuery("#tblFinalBillPaymentPendingList").jqGrid('setGridState', 'visible');

            var recordCount = $("#tblFinalBillPaymentPendingList").jqGrid('getGridParam', 'reccount');

            if (recordCount > 0) {

                if (recordCount > 10) {
                    $("#tblFinalBillPaymentPendingList").jqGrid('setGridHeight', '320');
                }
                else {
                    $("#tblFinalBillPaymentPendingList").jqGrid('setGridHeight', 'auto');
                }
                $("#tblFinalBillPaymentPendingList").footerData('set', { "FinancialRoadCategory": "Total" }, true);

                var grid = $("#tblFinalBillPaymentPendingList");

                TotalPavementLength = grid.jqGrid('getCol', 'PavementLength', false, 'sum')
                TotalSanctionedCost = grid.jqGrid('getCol', 'SanctionCost', false, 'sum')
                TotalExpenditure = grid.jqGrid('getCol', 'Expenditure', false, 'sum')
                
                grid.jqGrid('footerData', 'set', { ID: 'PavementLength', PavementLength: TotalPavementLength });
                grid.jqGrid('footerData', 'set', { ID: 'SanctionCost', SanctionCost: TotalSanctionedCost });
                grid.jqGrid('footerData', 'set', { ID: 'Expenditure', Expenditure: TotalExpenditure });
                
                //$("#dvAbstractBankAuthPager_left").html('<b>( Note: All amounts are in Rs. )</b>');
            }
            $.unblockUI();
        },
        grouping: true,
        groupingView: {
            groupField: ['DistrictName', 'PackageNumber'],
            groupColumnShow:[false,false]
        }
        //hoverrows: false,
        //autoencode: true,
        //ignoreCase: true,
     //   cmTemplate: { title: false },
        //gridComplete: function () {
        //    var grid = this;

        //    $('td[rowspan="1"]', grid).each(function () {

        //        var spans = $('td[rowspanid="' + this.id + '"]', grid).length + 1;
        //        if (spans > 1) {
        //            $(this).attr('rowspan', spans).attr('vertical-align', 'central');
        //        }
        //    });
        //},
        //resizeStop: function () {
        //    var $self = $(this),
        //        shrinkToFit = $self.jqGrid("getGridParam", "shrinkToFit");

        //    $self.jqGrid("setGridWidth", this.grid.newWidth, shrinkToFit);
        //    setHeaderWidth.call(this);
        //}        
    });
}
var prevCellVal0 = { cellId: undefined, value: undefined };
arrtSetting0 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal0.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal0.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal0 = { cellId: cellId, value: val };
    }

    return result;
}


var prevCellVal1 = { cellId: undefined, value: undefined };

arrtSetting1 = function (rowId, val, rawObject, cm, rdata) {
    var result;

    if (prevCellVal1.value == val) {
        result = ' style="display: none" rowspanid="' + prevCellVal1.cellId + '"';
    }
    else {
        var cellId = this.id + '_row_' + rowId + '_' + cm.name;

        result = ' rowspan="1" id="' + cellId + '"';
        prevCellVal1 = { cellId: cellId, value: val };
    }

    return result;
}
