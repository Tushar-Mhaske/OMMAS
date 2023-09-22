

$(document).ready(function () {

    if ($("#ddlState option:selected").val() == 0) {
        LoadStateAccMonitoringAllStateList();
    }
    else {
        LoadStateAccMonitoringStateList();
    }
});

function LoadStateAccMonitoringAllStateList()
{   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    State = $("#ddlState option:selected").val();
    Agency = $("#ddlAgency option:selected").val();
    FundType = $("#ddlFundType option:selected").val();
    
    jQuery("#tblStateAccMonitoringList").jqGrid({
        url: '/AccountsReports/ListStateAccountMonitoringDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Name of State', 'Status of Opening Balance', 'Last Month for which Entries Completed', 'Monthly Account', 'Balance Sheet (All PIUS)', 'Balance Sheet (SRRDA)', 'Balance Sheet (State)'],
        colModel: [
            { name: 'StateDpiuName', index: 'StateDpiuName', width: '250px', sortable: true, align: 'center' },
            { name: 'OBMonYear', index: 'OBMonYear', width: '150px', sortable: true, align: 'center' },
            { name: 'ClosedMonYear', index: 'ClosedMonYear', width: '150px', sortable: true, align: 'center' },
            { name: 'MonthlyAccount', index: 'MonthlyAccount', width: '80px', sortable: false, align: 'center' },
            { name: 'BalSheetAllPIU', index: 'BalSheetAllPIU', width: '80px', sortable: false, align: 'center' },
            { name: 'BalSheetSRRDA', index: 'BalSheetSRRDA', width: '80px', sortable: false, align: 'center' },
            { name: 'BalSheetState', index: 'BalSheetState', width: '80px', sortable: false, align: 'center' },
        ],
        pager: $("#dvStateAccMonitoringListPager"),
        postData: { State: State, Agency: Agency, FundType: FundType, value: Math.random() },
        sortOrder: 'asc',
        sortname: 'StateDpiuName',
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        rownumbers: true,
        hidegrid: true,
        viewrecords: true,
        recordText: '{2} records found',
        caption: 'State Account Monitoring Details',
        height: 'auto',
        shrinkToFit:true,
        autowidth:true,
        loaderror: function (xhr, status, error) {
            $.unblockUI();
            if (xhr.reponseText == "session expired")
            {
                window.location.href = "/Login/Login";
            }            
        },
        loadComplete: function (data) {
            var recordCount = $("#tblStateAccMonitoringList").jqGrid('getGridParam','reccount');
            if (recordCount > 0)
            {
                if (recordCount > 25) {
                    $("#tblStateAccMonitoringList").jqGrid('setGridHeight', '390');
                }
                else {
                    $("#tblStateAccMonitoringList").jqGrid('setGridHeight','auto');
                }
            }

            $("#tblStateAccMonitoringList_rn").html('Sr.<br/> No');

            $.unblockUI();
        },
    });
}

function LoadStateAccMonitoringStateList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    State = $("#ddlState option:selected").val();
    Agency = $("#ddlAgency option:selected").val();
    FundType = $("#ddlFundType option:selected").val();

    jQuery("#tblStateAccMonitoringList").jqGrid({
        url: '/AccountsReports/ListStateAccountMonitoringDetails',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Name of DPIU', 'Status of Opening Balance', 'Last Month for which Entries Completed', 'Monthly Account', 'Balance Sheet', 'Balance Sheet (SRRDA)', 'Balance Sheet (State)'],
        colModel: [
            { name: 'StateDpiuName', index: 'StateDpiuName', width: '170px', sortable: true, align: 'center' },
            { name: 'OBMonYear', index: 'OBMonYear', width: '170px', sortable: true, align: 'center' },
            { name: 'ClosedMonYear', index: 'ClosedMonYear', width: '170px', sortable: true, align: 'center' },
            { name: 'MonthlyAccount', index: 'MonthlyAccount', width: '170px', sortable: false, align: 'center' },
            { name: 'BalanceSheet', index: 'BalanceSheet', width: '170px', sortable: false, align: 'center' },
            { name: 'BalSheetSRRDA', index: 'BalSheetSRRDA', width: '170px', sortable: false, align: 'center' ,hidden:true},
            { name: 'BalSheetState', index: 'BalSheetState', width: '170px', sortable: false, align: 'center' ,hidden:true},
        ],
        pager: $("#dvStateAccMonitoringListPager"),
        postData: { State: State, Agency: Agency, FundType: FundType, value: Math.random() },
        sortOrder: 'asc',
        sortname: 'StateDpiuName',
        rowNum: 0,
        pginput: false,
        pgbuttons: false,
        rownumbers: true,
        hidegrid: true,
        viewrecords: true,
        recordText: '{2} records found',
        caption: 'State Account Monitoring Details',
        height: 'auto',
        autowidth: true,
        width: '100%',
        loaderror: function (xhr, status, error) {
            $.unblockUI();
            if (xhr.reponseText == "session expired") {
                window.location.href = "/Login/Login";
            }
        },
        loadComplete: function (data) {
            var recordCount = $("#tblStateAccMonitoringList").jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {
                if (recordCount > 25) {
                    $("#tblStateAccMonitoringList").jqGrid('setGridHeight', '390');
                }
                else {
                    $("#tblStateAccMonitoringList").jqGrid('setGridHeight', 'auto');
                }
            }

            $("#tblStateAccMonitoringList_rn").html('Sr.<br/> No');

            $.unblockUI();
        },
    });
}


function MonthlyAccount(param)
{
    var data = param.split('$');
    //data[0]=Month
    //data[1]=Year
    //data[2]=IsStateSelected T/F
    //data[3]=ADMIN_ND_CODE
    //data[4]=Parent_ADMIN_ND_CODE
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#dvHideShowStateAccMonitoringDetails").slideToggle();


    $("#dvMonthlyAccBalanceSheetDetails").show("slow");

    $.ajax({
        url: '/Reports/MonthlyAccount/',
        type:'GET',
        catche: false,
        async: false,
        data: { Month: data[0], Year: data[1], IsStateSelected: data[2], ADMIN_ND_CODE: data[3], PARENT_ND_CODE: data[4] },
        error: function (xhr, status, error) {
            $.unblockUI();

            alert("An error occured while processing your request.");

            return false;
        },
        success: function (response) {
            $("#dvMonthlyAccBalanceSheetDetails").html(response);
            $.unblockUI();
        },
        complete: function () {
            setTimeout(function () {
                $("#btnView").trigger("click");
            }, 500);
        }
    });
}
function BalanceSheetAllPiu(param) {
    var data = param.split('$');
    //data[0]=Month
    //data[1]=Year
    //data[2]=IsStateSelected T/F
    //data[3]=ADMIN_ND_CODE
    //data[4]=Parent_ADMIN_ND_CODE
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#dvHideShowStateAccMonitoringDetails").slideToggle();
    $("#dvMonthlyAccBalanceSheetDetails").show("slow");

    $.ajax({
        url: '/Reports/MaintenanceFundBalanceSheet/',
        type: 'GET',
        catche: false,
        async: false,
        data: { Month: data[0], Year: data[1], IsStateSelected: data[2], ADMIN_ND_CODE: data[3], PARENT_ND_CODE: data[4],BalSheetType:'A' },
        error: function (xhr, status, error) {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $("#dvMonthlyAccBalanceSheetDetails").html(response);
            $.unblockUI();
        }
    });
}
function BalanceSheetSRRDA(param) {
    var data = param.split('$');
    //data[0]=Month
    //data[1]=Year
    //data[2]=IsStateSelected T/F
    //data[3]=ADMIN_ND_CODE
    //data[4]=Parent_ADMIN_ND_CODE

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#dvHideShowStateAccMonitoringDetails").slideToggle();
    $("#dvMonthlyAccBalanceSheetDetails").show("slow");

    $.ajax({
        url: '/Reports/MaintenanceFundBalanceSheet/',
        type: 'GET',
        catche: false,
        async: false,
        data: { Month: data[0], Year: data[1], IsStateSelected: data[2], ADMIN_ND_CODE: data[3], PARENT_ND_CODE: data[4], BalSheetType: 'O' },
        error: function (xhr, status, error) {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $("#dvMonthlyAccBalanceSheetDetails").html(response);
            $.unblockUI();
        }
    });
}
function BalanceSheetState(param) {
    var data = param.split('$');
    //data[0]=Month
    //data[1]=Year
    //data[2]=IsStateSelected T/F
    //data[3]=ADMIN_ND_CODE
    //data[4]=Parent_ADMIN_ND_CODE

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#dvHideShowStateAccMonitoringDetails").slideToggle();
    $("#dvMonthlyAccBalanceSheetDetails").show("slow");

    $.ajax({
        url: '/Reports/MaintenanceFundBalanceSheet/',
        type: 'GET',
        catche: false,
        async: false,
        data: { Month: data[0], Year: data[1], IsStateSelected: data[2], ADMIN_ND_CODE: data[3], PARENT_ND_CODE: data[4], BalSheetType: 'S' },
        error: function (xhr, status, error) {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $("#dvMonthlyAccBalanceSheetDetails").html(response);
            $.unblockUI();
        }
    });
}