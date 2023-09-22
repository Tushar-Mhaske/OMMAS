var isGridLoaded = false;
var isDetailsGridLoaded = false;

$(document).ready(function () {
    //Added By Abhishek kamble 6-jan-2014 start change
    GetAccountStartMonthYear();
    GetClosedMonthAndYear();

    //LoadAssetLibChart();
    if (AccountEntry == "N") {
        LoadGrid();
    }

    $("#AddOBMaster").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#loadOBMaster").load("/OB/AddOBMaster/", function () {
            $("#loadOBMaster").show();
            $.unblockUI();

        });
        return false;
    });

    $("#btnView").click(function () {
        //$("#DivIcoOBTrans").trigger('click');
        LoadGrid();        
    });

    $("#lblBackToList").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#mainDiv").load("/OB/OpeningBalance/");
        $.unblockUI();

        return false;
        
        //$("#loadOBDetails").hide('slow');
        //$("#btnlblBackToList").hide('slow');
        //$("#loadOBMaster").hide('slow');
        //$("#tblOBList").jqGrid("setGridState", 'visible');
        //$("#tblOBDetailsGrid").jqGrid("setGridState", 'hidden');
    });

    $("#DivIcoOBTrans").click(function () {
        if ($(this).hasClass('ui-icon-circle-triangle-n')) {
            $("#tblOBChart").hide('slide');
            $(this).removeClass('ui-icon-circle-triangle-n');
            $(this).addClass('ui-icon-circle-triangle-s');
        }
        else {
            $("#tblOBChart").show('slide');
            $(this).removeClass('ui-icon-circle-triangle-s');
            $(this).addClass('ui-icon-circle-triangle-n');
        }
    });
    

}); // Document.ready ends here
function arrtSetting(rowId, val, rawObject, cm) {
    var result;
    //alert(val.toString().toLowerCase() + " " + rawObject[0].toLowerCase());
    if (cm.name == "Finalize") {
        if (val.toString().toLowerCase().contains("lockob") && rawObject[0].toLowerCase() == "assets") {
            result = ' rowspan=2';
        }
        else if (val.toString().toLowerCase().contains("lockob") && rawObject[0].toLowerCase() == "liabilities") {
            result = ' style="display: none';
        }
    }
    else {
        if (rawObject[0].toLowerCase() == "assets") {
            result = ' rowspan=2';
        } else if (rawObject[0].toLowerCase() == "liabilities") {
            result = ' style="display: none';
        }
    }
    return result;
};

function LoadGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (isGridLoaded) {
        $("#tblOBList").jqGrid('GridUnload');
      isGridLoaded = false;
    }

    jQuery("#tblOBList").jqGrid({
        url: '/OB/GetOBMasterList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Type', 'Transaction Name','OB Date', 'Gross Amount(In Rs.)', 'Details Amount(In Rs.)', 'Status','Add Transactions', 'Edit', 'Delete', 'Status'],
        colModel: [
                            { name: 'OBType', index: 'OBType', width: 80, align: 'center', sortable: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 0, align: 'center', sortable: true, hidden:true },                            
                            { name: 'OBDate', index: 'OBDate', width: 80, align: 'center', sortable: true },
                            { name: 'MasterAmount', index: 'MasterAmount', width: 80, align: 'right', sortable: true },
                            { name: 'DetailsAmount', index: 'DetailsAmount', width: 80, align: 'right', sortable: true },
                            {
                                name: 'Add', index: 'Add', width: 50, align: 'center', sortable: false/*, cellattr: arrtSetting*/  },
                            {
                                name: 'Edit', index: 'Edit', width: 50, align: 'center', sortable: false/*, cellattr: arrtSetting*/
                            },
                            {
                                name: 'Delete', index: 'Delete', width: 50, align: 'center', sortable: false/*, cellattr: arrtSetting*/
                            },
                            { name: 'Finalize', index: 'Finalize', width: 70, align: 'center', sortable: false/*, cellattr: arrtSetting*/ },
                            { name: 'Action', index: 'Action', width: 50, align: 'center'/*, cellattr: arrtSetting, hidden:false*/ },
        ],
        pager: jQuery('#divOBListPager'),
        rowNum: 2,
        //rowList: [10, 20, 50],
        pginput:false,
        viewrecords: true,
        //recordtext: '{2} records found',
        sortname: 'OBDate',
        sortorder: "desc",
        caption: "Opening Balances Details",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        //toppager:true,
        loadComplete: function () {
            isGridLoaded = true;
            if ($('#tblOBList').jqGrid('getGridParam', 'reccount') > 0) {
                $("#AddOBMaster").hide();
                //$("#tblOBList #divOBListPager").css({ height: 'auto' });
                $("#divOBListPager_right").html("<span style='float:right'>Status represents <font color='green'>OB Details Entered</font> and <font color='#b83400'>OB Details Remained</font> Amount</span><span style='float:right' class='ui-icon ui-icon-info'></span> </span></span>");
                //$("#divOBListPager_left").html("<span style='float:right'><span class='ui-icon ui-icon-check'></span> represents <font color='green'>Correct Entry</font> and <span class='ui-icon ui-icon-check'></span><font color='#b83400'>Wrong Entry</font></span>");
            }           
            else {
                $("#AddOBMaster").show();
            }

            $('#tblOBList_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();
        }

    }).navGrid('#divOBListPager', { edit: false, add: false, del: false, search: false, refresh: false })
        .navButtonAdd('#divOBListPager', {
            caption: "Chart",
            buttonicon: "ui-icon-image",
            onClickButton: function () {
                if ($('#tblOBList').jqGrid('getGridParam', 'reccount') > 0) {
                    $("#divOBChart").show();
                    $("#tblOBChart").show();
                    if ($('#tblOBList').jqGrid('getGridParam', 'reccount') > 0)
                    {
                        LoadAssetLibChart();
                    }
                    else {
                        $("#divOBListPager_left").Append("<span style='float:right'>Status represents <font color='green'>OB Details Entered</font> and <font color='#b83400'>OB Details Remained</font> Amount</span><span style='float:right' class='ui-icon ui-icon-info'></span>");
                    }
                }
                else {
                    alert('OB Details not present to show Chart');
                    return false;
                }
            },
            position: "first"
        }); //end of documents grid

    //var topPagerDiv = $("#pg_tblOBList_toppager")[0];
    //$("#tblOBList_toppager_center", topPagerDiv).remove();
    //$("#tblOBList_toppager_center", topPagerDiv).remove();

    /************** For Future Use **********************
    var topPagerDiv = $("#list_toppager")[0];
    $("#edit_list_top", topPagerDiv).remove();
    $("#del_list_top", topPagerDiv).remove();
    $("#search_list_top", topPagerDiv).remove();
    $("#refresh_list_top", topPagerDiv).remove();
    $("#list_toppager_center", topPagerDiv).remove();
    $(".ui-paging-info", topPagerDiv).remove();

    var bottomPagerDiv = $("div#pager")[0];
    $("#add_list", bottomPagerDiv).remove();

    ****************************************************/

}

function LoadOBDetailsGrid(MasterId) {  

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (isDetailsGridLoaded) {
        $("#tblOBDetailsGrid").GridUnload();
        isDetailsGridLoaded = false;
    }
    jQuery("#tblOBDetailsGrid").jqGrid({
        url: '/OB/GetOBDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Type', 'Head Name', 'Contractor Name', 'Agreement','Road / LSB Name', 'DPIU', 'Asset Amount', 'Liability Amount', 'Narration', 'Edit', 'Delete','Status'],
        colModel: [
                            { name: 'CreditDebit', index: 'CreditDebit', width: 0, align: 'center', hidden: true },
                            { name: 'HeadName', index: 'HeadName', width: 250, align: 'left', sortable:false , cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Contractor', index: 'Contractor', width: 125, align: 'left', sortable: false,hidden:fundType == 'A'?true:false  },
                            { name: 'Agreement', index: 'Agreement', width: 125, align: 'left', sortable: true, hidden: fundType == 'A' ? true : false },//, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } 
                            { name: 'RoadName', index: 'RoadName', width: 125, align: 'left', sortable: false },
                            { name: 'DPIU', index: 'DPIU', width: 90, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'AssetAmount', index: 'AssetAmount', width: 80, align: 'right', sortable: true ,cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' }},
                            { name: 'LibAmount', index: 'LibAmount', width: 80, align: 'right', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Narration', index: 'Narration', width: 120, align: 'left', sortable: false, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Edit', index: 'Edit', width: 30, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 30, align: 'center', sortable: false },
                            { name: 'Status', index: 'Status', width: 50, align: 'center', sortable: false }
        ],
        pager: jQuery('#divOBDetailsPager'),
       
        postData: {
            'masterId': MasterId
        },
        pginput: false,
        pgbuttons: false,
        rowNum: 99999999,
        viewrecords: true,
        recordtext: '{2} records found',
        emptyrecords: 'No records to view',
        sortname: 'CreditDebit',
        sortorder: "desc",
        caption: "Opening Balance Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
       // rowNum:0,
        loadComplete: function () {           
            isDetailsGridLoaded = true;
            var userdata = $("#tblOBDetailsGrid").getGridParam('userData');
            //$("#divOBDetailsPager").html("<span  style='float:left;' class='ui-icon ui-icon-info'></span><span style='align:left'> Rs. <font color='#4eb305'>" + userdata.grossAmount + "</font> Gross Amount Entered</span>" + "<span style='float:left;' class='ui-icon ui-icon-info'></span> <span style='align:left'>Rs. <font color='#4eb305'>" + userdata.AssetRemainingAmount + "</font> Asset Amount Remaining</span>" + "<span style='float:left;' class='ui-icon ui-icon-info'></span><span style='align:left'>Rs. <font color='#4eb305'>" + userdata.LibRemainingAmount + "</font> Liability Amount Remaining</span>");
            
            $("#divOBDetailsPager_left").html("<table><tr><td style='width:20%'><span  style='float:left;' class='ui-icon ui-icon-info'></span>Rs. <font color='#4eb305'>" + userdata.grossAmount + "</font> Gross Amount Entered</td><td style='width:20%'><span style='float:left;display:inline' class='ui-icon ui-icon-info'></span>Rs. <font color='#4eb305'>" + userdata.AssetRemainingAmount + "</font> Asset Amount Remaining</td></tr></table>");
            $("#divOBDetailsPager_center").html("<table><tr><td style='width:20%'><span style='float:left;display:inline' class='ui-icon ui-icon-info'></span>Rs. <font color='#4eb305'>" + userdata.LibRemainingAmount + "</font> Liability Amount Remaining</td></tr></table>");

            if (userdata.isFinalize == "Y")
            {
                $("#AddOBMaster").hide();
                $("#lblBackToList").hide();
                $("#loadOBMaster").hide();
                $("#loadOBDetails").hide();
                $("#divFinalizeOB").show();
            }
            // new change done by Vikram 
            else if (userdata.isFinalize == "N")
            {
                if ($("#divFinalizeOB").is(':visible'))
                {
                    $("#divFinalizeOB").hide();
                }
            }
            //end of change

            $("#lblBackToList").show('slow');

            var recordCount = jQuery("#tblOBDetailsGrid").jqGrid('getGridParam', 'reccount');          
           
            if (recordCount > 10) {

                $('#tblOBDetailsGrid').jqGrid('setGridHeight', '320');
            }
            else {

                $('#tblOBDetailsGrid').jqGrid('setGridHeight', 'auto');
            }
            $.unblockUI();

        },
        loadError: function (xhr, ststus, error) {
            $("#lblBackToList").show('slow');
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();

        }

    }); //end of documents grid
}

function ViewOBDetails(urlParam)
{
   // $("#divOBChart").html("").hide();
  //  $("#tblOBChart").html("").hide();
    LoadOBDetailsGrid(urlParam);
}

function EditOB(urlParam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#loadOBMaster").hide();
    $("#divOBChart").html("").hide();
    $("#tblOBChart").html("").hide();
    $("#divOBMasterWrapper").hide();
    $("#loadOBDetails").html("");
    $("#divOBDetailsWrapper").hide();
    $("#loadOBMaster").load("/OB/AddOBMaster/" + urlParam, function () {
        //$("#DivIcoOBTrans").trigger('click');
        //$("#tblOBList").jqGrid("setGridState", 'hidden');
        $("#btnlblBackToList").show('slow');
        $("#loadOBMaster").show('slow');
        $.unblockUI();
        return false;
    });
}

function DeleteOB(urlParam, AssetLibAmt) {

    var OBAmt = AssetLibAmt.split('$');
    var varMsg = null;
    if (parseFloat(OBAmt[0]) > 0 || parseFloat(OBAmt[1] > 0)) {
        varMsg = "Details present. Are you sure you want to delete all details?";
    }
    else {
        varMsg = "Are you sure you want to Delete OB Master?";
    }
    if (confirm(varMsg)) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/OB/DeleteOBMaster/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    $("#divOBChart").html("").hide();
                    $("#tblOBChart").html("").hide();
                    alert("OB Entry Deleted");
                    $("#divOBList").show();
                    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), null, null, 0, 'view');
                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {

        return false;
    }

}

function AddOBDetails(urlParam, transNo)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#loadOBDetails").load("/OB/AddOBDetails/" + urlParam+"/"+transNo, function () {
        //$("#DivIcoOBTrans").trigger('click');
        $("#divOBChart").html("").hide();
        $("#tblOBChart").html("").hide();
        $("#divOBMasterWrapper").hide();
        $("#loadOBDetails").show('slow');
        $("#loadOBMaster").hide('slow');
        $("#btnlblBackToList").show('slow');
        $("#divOBDetailsWrapper").show();
        LoadOBDetailsGrid(urlParam);
        //$("#tblOBList").jqGrid("setGridState", 'hidden');
        $("#tblOBDetailsGrid").jqGrid("setGridState", 'visible');

        $('#EncryptedParam').val(urlParam);
        $('#TransNo').val(transNo);
        $("#btnlblBackToList").show('slow');
        $.unblockUI();

        return false;
    });

}

function LoadAssetLibChart()
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#lblDescChart").html("Outer chart represents <font color='#b83400'>ASSETS</font> and Inner chart represents <font color='green'>LIABILITIES</font>");
    var desc = $("#lblDescChart").html();
    $.ajax({
        url: "/OB/GetAssetLiabilityDetails/",
        type: "POST",
        async: false,
        cache: false,
        success: function (data) {
            $.unblockUI();

            if (data.success) {
                
                var dataAdapter_asset = data.asset;
                
                var dataAdapter_liability = data.liability;
                
                // prepare jqxChart settings
                var settings = {
                    title: "Asset and Liabilities Transaction Description",
                    description: "Outer chart represents ASSETS and Inner chart represents LIABILITIES",
                    enableAnimations: true,
                    showLegend: true,
                    legendLayout: { left: 350, top: 120, width: 700, height: 200, flow: 'vertical' },
                    padding: { left: 5, top: 5, right: 5, bottom: 5 },
                    titlePadding: { left: 0, top: 0, right: 0, bottom: 10 },
                    seriesGroups:
                        [
                            {
                                type: 'donut',
                                offsetX: 200,
                                source: dataAdapter_asset,
                                //showLabels: true,
                                categoryAxis:
                                {
                                    formatSettings: { prefix: 'Asset: ' }
                                },
                                //click: AssetEventHandler,
                                series:
                                    [
                                        {
                                            dataField: 'amount',
                                            displayText: 'transname',
                                            labelRadius: 120,
                                            initialAngle: 10,
                                            radius: 130,
                                            innerRadius: 90,
                                            centerOffset: 0,
                                            formatSettings: { prefix: 'Rs', decimalPlaces: 2 }
                                        }
                                    ]
                            },
                            {
                                type: 'donut',
                                offsetX: 200,
                                source: dataAdapter_liability,
                                colorScheme: 'scheme05',
                                //showLabels: true,
                                categoryAxis:
                                {
                                    formatSettings: { prefix: 'Liability: ' }
                                },
                                //click: LibEventHandler,
                                series:
                                    [
                                        {
                                            dataField: 'amount',
                                            displayText: 'transname',
                                            labelRadius: 120,
                                            initialAngle: 10,
                                            radius: 70,
                                            innerRadius: 30,
                                            centerOffset: 0,
                                            formatSettings: { prefix: 'Rs', decimalPlaces: 2 }
                                        }
                                    ]
                            }
                        ]
                };

                //function AssetEventHandler(e) {
                //    var eventData = '<b>Last Event: </b>' + e.event + '<b>, DataField: </b>' + e.serie.dataField + '<b>, Value: </b>' + e.elementValue;
                //    alert(eventData);
                //    $('#eventText').html(eventData);
                //};

                //function LibEventHandler(e) {
                //    var eventData = '<b>Last Event: </b>' + e.event + '<b>, DataField: </b>' + e.serie.dataField + '<b>, Value: </b>' + e.elementValue;
                //    alert(eventData);
                //    $('#eventText').html(eventData);
                //};

                // setup the chart
                $('#chartContainer').jqxChart(settings);

                $('rect[width="898"]').attr('width', '895px').attr('fill', '#ffffff').attr('stroke', '#ffffff');

                // get the series groups of an existing chart
                var groups = $('#chartContainer').jqxChart('seriesGroups');

                // add a click event handler function to the 1st group    
                if (groups.length > 0) {
                    groups[0].click = function (e) {
                        //alert('<b>Last Event: </b>' + e.event + '<b>, DataField: </b>' + e.serie.dataField + '<b>, Value: </b>' + e.elementValue);
                    }
                    groups[1].click = function (e) {
                        //alert('event = ' + e.event + ' index = ' + e.elementIndex);
                    }
                    // update the group
                    //$('#jqxChart').jqxChart({ seriesGroups: groups });
                }
            }
            else {
                alert(data.message);
                return false;
            }
        }
    });

}

function LockOB(urlParam)
{

    if (confirm("Are you sure you want to finalize OB Entry?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/OB/FinalizeOB/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("OB Entry Finalize");
                    $("#mainDiv").load("/OB/OpeningBalance/");
                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {

        return false;
    }
}

//Added By Abhishek kamble 6-jan-2014
//function to get the account  start month and year
function GetAccountStartMonthYear() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetAccountStartMonthandYear/",
        //async: false,
        error: function (xhr, status, error) {
            //unblockPage();
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            $.unblockUI();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.accountStarted) {
                $("#lblAccMonth").text(data.month);
                $("#lblAccYear").text(data.year);

                $("#TrAccountStatus").show('Slow');
                $("#accountMonthYearTr").hide('Slow');
                return false;
            }
            else if (data.accountStarted == false) {
                $("#accountMonthYearTr").show('Slow');
                $("#TrAccountStatus").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Account Start month and year");
                return false;
            }

        }
    });


}

//function to get the account  Close month and year
function GetClosedMonthAndYear() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,

        error: function (xhr, status, error) {
            $.unblockUI();

            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');

            return false;

        },
        success: function (data) {
            $.unblockUI();

            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);

                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });


}
