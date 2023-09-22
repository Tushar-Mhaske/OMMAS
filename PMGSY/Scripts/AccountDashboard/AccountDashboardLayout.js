var myLayout;
var mainDivLayout;
var mainDivCenterLayout;
var mainDivWestLayout;
var OldmainDivCenterLayout;
var OldmainDivWestLayout;
var finProgLineChart;
var pkgWiseAgreementColumnChart;
var phyProgColumnChart;
var avgTotalColumnChart;

//Pie Chart
var optionsExpenditureDetailsPieChart;
var ExpenditureDetailsPieChart;
var togglePieChartsDataLabels = true;

//Ded Vs Rem
var options;
var chart;
var toggleDedvsRemLegend = false;
var pointWidthDedVsRem = 30;
var toggleDedVsRemChartsDataLabels = true;

//Fund Rec Vs Exp
var optionsFundExpenditureChart;
var toggleFundRecVsExpChartsDataLabels = true
var chartFundExpenditureChart;

$(document).ready(function () {

    //$.validator.unobtrusive.parse($('#frmAccountDashboard'));

    //Start

    myLayout = $('body').layout({
        closable: false
  , resizable: true
  , slidable: false
  , livePaneResizing: true
  , north__slidable: false
  , north__spacing_closed: 0
  , north__resizable: false
  , north__spacing_open: 0
  , south__slidable: false
  , south__spacing_closed: 0
  , south__resizable: false
  , south__spacing_open: 0
  , north__minSize: 110
  , north__maxSize: 110
  , south__minSize: 10
  , south__maxSize: 10
  , showDebugMessages: true
        //, north_minSize: '05%'
        //, north_maxSize:'05%'
        , south__resizable: false,
    });


    mainDivLayout = $('body .ui-layout-center').layout({
        closable: false
    , resizable: false
    , slidable: true
    , livePaneResizing: true
    , south__resizable: false
    , south__spacing_open: 0
    , south__spacing_closed: 0
    , west__spacing_open: 0
    , west__spacing_closed: 0
    , west__minSize: '50%'
    , west__maxSize: '100%'
    , center__minWidth: '50%'
    , center__maxWidth: '100%'
    , showDebugMessages: true
    , south__minSize: '02%'
    , south__maxSize: '03%'
    });


    mainDivCenterLayout = $('body .ui-layout-center .ui-layout-center').layout({

        closable: false
    , resizable: false
    , slidable: false
    , livePaneResizing: true
   , center__minSize: '50%'
    , center__maxSize: '100%'
    , south__minSize: '50%'
    , south__maxSize: '100%'
    , center__minWidth: '50%'
    , center__maxWidth: '100%'
    , south__minWidth: '50%'
    , south__maxWidth: '100%'
    , south__minHeight: '50%'
    , south__maxHeight: '100%'
    , south__spacing_open: 0
    , south__spacing_closed: 0
    , center__spacing_open: 0
    , center__spacing_closed: 0
    , south__onresize: function () { $(window).resize(); }
    , center__onresize: function () { $(window).resize(); }
    });

    mainDivWestLayout = $('body .ui-layout-center .ui-layout-west').layout({

        closable: false
    , resizable: false
    , slidable: false
    , livePaneResizing: true
    , center__minSize: '50%'
    , center__maxSize: '100%'
    , north__minSize: '50%'
    , north__maxSize: '100%'
    , center__minWidth: '50%'
    , center__maxWidth: '100%'
    , south__minWidth: '50%'
    , south__maxWidth: '100%'

    , north__minHeight: '50%'
    , north__maxHeight: '100%'
    , north__spacing_open: 0
    , north__spacing_closed: 0
    , center__spacing_open: 0
    , center__spacing_closed: 0
    , north__onresize: function () {
        $(window).resize();
    }
    , center__onresize: function () {
        $(window).resize();
    }
    });


    $("#btnMaxCenter").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMinCenter").show('slow');
        $("#btnMaxCenter").hide();
        $("#btnMinMaxCenter").show();
        $("#btnMinMaxWest").show();
        toggleDedvsRemLegend = true;
        options.legend.enabled = toggleDedvsRemLegend;
        //ajax call to dynamically load column chart.
        chart = new Highcharts.Chart(options);
        $.unblockUI();
    });

    $("#btnMinCenter").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMaxCenter").show('slow');
        $("#btnMinCenter").hide();
        $("#btnMinMaxCenter").show();
        $("#btnMinMaxWest").show();

        toggleDedvsRemLegend = false;
        options.legend.enabled = toggleDedvsRemLegend;
        //ajax call to dynamically load column chart.
        chart = new Highcharts.Chart(options);
        $.unblockUI();
    });

    $("#btnMaxWestCenter").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMinWestCenter").show('slow');
        $("#btnMaxWestCenter").hide();
        $("#btnMinMaxWestCenter").hide();
        $("#btnMinMaxWest").show();
        $.unblockUI();
    });

    $("#btnMinWestCenter").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMaxWestCenter").show('slow');
        $("#btnMinWestCenter").hide();
        $("#btnMinMaxWestCenter").show();
        $("#btnMinMaxWest").show();
        $.unblockUI();
    });

    $("#btnMaxCenterSouth").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMaxCenterSouth").hide();
        $("#btnMinCenterSouth").show();
        $("#btnMinMaxCenter").hide();
        $.unblockUI();
    });

    $("#btnMinCenterSouth").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMinCenterSouth").hide();
        $("#btnMaxCenterSouth").show();
        $("#btnMinMaxCenter").show();
        $("#btnMinMaxWest").show();
        $.unblockUI();
    });

    $("#btnMaxWestSouth").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMaxWestSouth").hide();
        $("#btnMinWestSouth").show('slow');
        $("#btnMinMaxCenter").show();
        $("#btnMinMaxWest").hide();
        $("#dvCenter").hide();
        $.unblockUI();
    });

    $("#btnMinWestSouth").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $("#btnMinWestSouth").hide();
        $("#btnMaxWestSouth").show('slow');
        $("#btnMinMaxCenter").show();
        $("#btnMinMaxWest").show();

        $("#dvCenter").show();

        $.unblockUI();
    });


    //End

    $(function () {

        // Radialize the colors for Pie chart
        Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
            return {
                radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                stops: [
                    [0, color],
                    [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
                ]
            };
        });

        LoadFundExpenditureChart();
        LoadExpenditureDetailsPieChart('');
        LoadDeductionVsRemittenceColumnChart('', null);
        CreateAccountSummaryGrid($("#currentFinYear").val());
        UserLogin();

        //Reload Pie Chart and Grid
        $("#spnReloadPieChart").click(function () {
            $("#spnShowExpenditureDetailsChart").trigger('click');
            LoadExpenditureDetailsPieChart('');
            $("#tblExpenditureDetails").GridUnload();
            LoadExpenditureDetailsGrid('');
        });

        //Reload Deduction Vs Remittance Chart
        $("#spnReloadDedVsRemittanceChart").click(function () {
            $("#spnShowDedVsRemittanceChart").trigger('click');
            LoadDeductionVsRemittenceColumnChart('', null);
            $("#tblDeductionVsRemittence").GridUnload();
            LoadDeductionVsRemittenceGrid('');
        });


        //ShowFundReceivedVsExpenditureChart
        $("#spnShowFundReceivedVsExpenditureChart").click(function () {
            //Set FundReceivedVsExpenditure Grid Width     
            $("#tblFundReceivedVsExpenditure").setGridWidth($("#dvWestSouth").width());

            $("#dvFundReceivedVsExpenditureGrid").hide("slow");
            $("#dvFundReceivedVsExpenditureColumnChart").show("slow");

            $("#spnShowFundReceivedVsExpenditureChart").hide("slow");
            $("#spnShowFundReceivedVsExpenditureGrid").show("slow");

            $("#dvHideShowFundRecVsExpDataLabels").show("slow");

        });

        //ShowFundReceivedVsExpenditureGrid
        $("#spnShowFundReceivedVsExpenditureGrid").click(function () {

            //FundReceived Vs Expenditure Grid
            LoadFundReceivedVsExpenditureGrid();

            //Set FundReceivedVsExpenditure Grid Width            
            $("#tblFundReceivedVsExpenditure").setGridWidth($("#dvWestSouth").width());

            $("#dvFundReceivedVsExpenditureColumnChart").hide("slow");
            $("#dvFundReceivedVsExpenditureGrid").show("slow");

            $("#spnShowFundReceivedVsExpenditureGrid").hide("slow");
            $("#spnShowFundReceivedVsExpenditureChart").show("slow");

            $("#dvHideShowFundRecVsExpDataLabels").hide("slow");


        });


        //ShowExpenditureDetailsChart
        $("#spnShowExpenditureDetailsChart").click(function () {

            //Set FundReceivedVsExpenditure Grid Width     
            $("#tblExpenditureDetails").setGridWidth($("#dvWestCenter").width());
            $("#dvExpenditureDetailsGrid").hide("slow");

            $("#dvPieChart").show("slow");

            $("#spnShowExpenditureDetailsChart").hide("slow");
            $("#spnShowExpenditureDetailsGrid").show("slow");

            $("#dvHideShowDataLabels").show("slow");

        });

        $("#spnShowExpenditureDetailsGrid").click(function () {

            LoadExpenditureDetailsGrid('');

            //Set FundReceivedVsExpenditure Grid Width     
            $("#tblExpenditureDetails").setGridWidth($("#dvWestCenter").width());
            $("#dvPieChart").hide("slow");

            $("#dvExpenditureDetailsGrid").show("slow");

            $("#spnShowExpenditureDetailsGrid").hide("slow");
            $("#spnShowExpenditureDetailsChart").show("slow");
            $("#dvHideShowDataLabels").hide("slow");

        });

        //ShowDedVsExpenditureChart
        $("#spnShowDedVsRemittanceChart").click(function () {
            //Set FundReceivedVsExpenditure Grid Width     
            $("#tblDeductionVsRemittence").setGridWidth($("#dvCenter").width());

            $("#dvDeductionVsRemittenceGrid").hide("slow");
            $("#dvDeductionVsRemittenceColumnChart").show("slow");

            $("#spnShowDedVsRemittanceChart").hide("slow");
            $("#spnShowDedVsRemittanceGrid").show("slow");

            $("#dvHideShowDedRemDataLabels").show("slow");

        });

        //ShowDedVsExpenditureGrid
        $("#spnShowDedVsRemittanceGrid").click(function () {

            //Deduction Vs Remitance Grid
            LoadDeductionVsRemittenceGrid('');

            //Set Deduction Vs Remitance Grid Width            
            $("#tblDeductionVsRemittence").setGridWidth($("#dvCenter").width());

            $("#dvDeductionVsRemittenceColumnChart").hide("slow");
            $("#dvDeductionVsRemittenceGrid").show("slow");

            $("#spnShowDedVsRemittanceGrid").hide("slow");
            $("#spnShowDedVsRemittanceChart").show("slow");

            $("#dvHideShowDedRemDataLabels").hide("slow");
        });

        //Populate DPIU
        $("#ddlStates").change(function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $.ajax({
                type: 'POST',
                url: '/AccountDashboard/PopulateDPIU?id=' + $("#ddlStates option:selected").val(),
                error: function (xhr, status, error) {
                    alert("An Error occured while proccessing your request.");
                    return false;
                },
                success: function (responce) {
                    $("#ddlDPIU").empty();
                    $.each(responce, function (data) {
                        $("#ddlDPIU").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    });
                },
                complete: function () {
                    $.unblockUI();
                }
            });
        });

        if ($("#LevelId").val() == 4) {
            $("#ddlStates").trigger("change");
        }

        //Hide/ show ExpenditureDetailsPieChart Data labels
        $("#dvHideShowDataLabels").click(function () {
            optionsExpenditureDetailsPieChart.plotOptions.pie.dataLabels.enabled = togglePieChartsDataLabels;
            ExpenditureDetailsPieChart = new Highcharts.Chart(optionsExpenditureDetailsPieChart);
            if (togglePieChartsDataLabels) {
                togglePieChartsDataLabels = false;
                $("#dvHideShowDataLabels").attr('title', 'Hide Labels');
            }
            else {
                togglePieChartsDataLabels = true;
                $("#dvHideShowDataLabels").attr('title', 'Show Labels');
            }
        });

        //Hide/ show Ded Rem chart Data labels
        $("#dvHideShowDedRemDataLabels").click(function () {
            showHideDedRemChartsDataLabels(toggleDedVsRemChartsDataLabels);
        });

        //Hide/ show Fund Rec Vs Exp chart Data labels
        $("#dvHideShowFundRecVsExpDataLabels").click(function () {
            showHideFundRecVsExpChartsDataLabels(toggleFundRecVsExpChartsDataLabels);
        });


        //Btn View Details
        $("#btnView").click(function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            if ($("#ddlFundType").val() == 0) {
                alert("Please select Fund Type.");
                return false;
            }
            else {
                if ($("#dvExpenditureDetailsGrid").is(":visible")) {
                    $("#dvExpenditureDetailsGrid").hide("slow");
                    $("#dvPieChart").show("slow");

                    $("#spnShowExpenditureDetailsChart").hide("slow");
                    $("#spnShowExpenditureDetailsGrid").show("slow");

                    $("#dvHideShowDataLabels").show("slow");
                    $("#dvHideShowDedRemDataLabels").show("slow");
                }

                if ($("#dvFundReceivedVsExpenditureGrid").is(":visible")) {
                    $("#dvFundReceivedVsExpenditureGrid").hide("slow");
                    $("#dvFundReceivedVsExpenditureColumnChart").show("slow");

                    $("#spnShowFundReceivedVsExpenditureChart").hide("slow");
                    $("#spnShowFundReceivedVsExpenditureGrid").show("slow");
                }

                if ($("#dvDeductionVsRemittenceGrid").is(":visible")) {
                    $("#dvDeductionVsRemittenceGrid").hide("slow");
                    $("#dvDeductionVsRemittenceColumnChart").show("slow");

                    $("#spnShowDedVsRemittanceChart").hide("slow");
                    $("#spnShowDedVsRemittanceGrid").show("slow");
                }

                LoadFundExpenditureChart();
                LoadExpenditureDetailsPieChart('');
                LoadDeductionVsRemittenceColumnChart('', null);
                CreateAccountSummaryGrid($("#currentFinYear").val());

                $("#tblExpenditureDetails").GridUnload();
                $("#tblFundReceivedVsExpenditure").GridUnload();
                $("#tblDeductionVsRemittence").GridUnload();

                $("#btnMinWestSouth").trigger('click');
                $("#btnMinWestCenter").trigger('click');
                $("#btnMinCenter").trigger('click');
            }
        });



    });//end of function


});//eof




function UserLogin() {


    $("#UserLogin").qtip(
    {
        events: {
            show: function (event, api) {
                // $('#qtip-3-content').removeClass();
                $('.jquery-ui-switcher-link').css({
                    'position': ''
                });
            }
        },
        content: {
            // Set the text to an image HTML string with the correct src URL to the loading image you want to use
            text: $('#userProfile'),//+'<a href="/login/logout" title="Click to log Off" >Log Off</a> </br> ' ,
            ajax: {
                //url: $(this).attr('rel') // Use the rel attribute of each element for the url to load
            },
            title: {
                // text: 'Settings', // Give the tooltip a title using each elements text
                // button: false
            }
        },
        position: {
            at: 'bottom center', // Position the tooltip above the link
            // my: 'bottom center',
            viewport: $(window), // Keep the tooltip on-screen at all times
            effect: false, // Disable positioning animation
            target: $('#UserLogin')
            , adjust: { y: 10, x: 10 },
        },
        show: {
            event: 'click',
            solo: true // Only show one tooltip at a time
        },
        hide: 'unfocus',
        style: {
            // height: 200,
            width: 250,
            classes: 'qtip-wiki qtip-light ',
            widget: true
        }
    })
// Make sure it doesn't follow the link when we click it
.click(function (event) { event.preventDefault(); });
    $('#UserLogin').qtip('show');
    $('#UserLogin').qtip('hide');
}

function MaximizeCenter() {
    mainDivCenterLayout.hide('south');
    mainDivLayout.hide('west');

    $("#tblDeductionVsRemittence").parents('div.ui-jqgrid-bdiv').css("max-height", (parseInt($("#dvCenter").height()) - 200), true);
    $("#tblDeductionVsRemittence").setGridWidth($("#dvCenter").width());
}
function MinimizeCenter() {
    mainDivCenterLayout.show('south');
    mainDivLayout.show('west');
    $("#tblDeductionVsRemittence").parents('div.ui-jqgrid-bdiv').css("max-height", 250);
    $("#tblDeductionVsRemittence").setGridWidth(600);
}
function MaximizeCenterSouth() {
    mainDivCenterLayout.sizePane('south', '100%');
    mainDivLayout.hide('west');

    $("#dvCenter").hide();

    //Set Other Details Grid Width    
    $("#tblOtherDetails").setGridWidth($("#dvCenterSouth").width());

}
function MinimizeCenterSouth() {
    mainDivCenterLayout.sizePane('south', '50%');
    mainDivCenterLayout.show('south');
    mainDivLayout.show('west');

    $("#dvCenter").show();

    //Set Other Details Grid Width        
    $("#tblOtherDetails").setGridWidth($("#dvCenterSouth").width());
}
function MinimizeWest() {
    mainDivLayout.sizePane('west', '50%');
    mainDivCenterLayout.sizePane('south', '50%');
    mainDivCenterLayout.show('south');
    mainDivWestLayout.show('north');

    showHidePieChartsDataLabels(false);
    $("#tblExpenditureDetails").setGridWidth($("#dvWestCenter").width());
}
function MaximizeWest() {
    mainDivWestLayout.hide('north');
    mainDivCenterLayout.sizePane('south', '100%');
    mainDivCenterLayout.hide('south');
    mainDivLayout.sizePane('west', '100%');

    showHidePieChartsDataLabels(true);
    $("#tblExpenditureDetails").setGridWidth($("#dvWestCenter").width());
}
function MinimizeWestSouth() {
    mainDivLayout.sizePane('west', '50%');
    mainDivWestLayout.sizePane('north', '50%');
    mainDivCenterLayout.sizePane('south', '50%');
    mainDivCenterLayout.show('south');
    $("#dvWestCenter").show();
    $("#dvCenter").show("slow");

    //Set FundReceivedVsExpenditure Grid Width            
    $("#tblFundReceivedVsExpenditure").setGridWidth($("#gbox_tblFundReceivedVsExpenditure").parent().width());
    $('#tblFundReceivedVsExpenditure').jqGrid('setGridHeight', (parseInt($("#dvWestSouth").height()) - 200));

}
function MaximizeWestSouth() {
    mainDivCenterLayout.sizePane('south', '100%')
    mainDivCenterLayout.hide('south');
    mainDivWestLayout.sizePane('north', '100%');
    mainDivLayout.sizePane('west', '100%');

    $("#dvWestCenter").hide("slow");
    $("#dvCenter").hide("slow");

    //Set FundReceivedVsExpenditure Grid Width                
    $("#tblFundReceivedVsExpenditure").setGridWidth($("#gbox_tblFundReceivedVsExpenditure").parent().width());
    $('#tblFundReceivedVsExpenditure').jqGrid('setGridHeight', 'auto');
}
function showHidePieChartsDataLabels(flag) {
    optionsExpenditureDetailsPieChart.plotOptions.pie.dataLabels.enabled = flag;
    ExpenditureDetailsPieChart = new Highcharts.Chart(optionsExpenditureDetailsPieChart);
    if (flag == true) {
        $("#dvHideShowDataLabels").attr('title', 'Hide Labels');
    }
    else {
        $("#dvHideShowDataLabels").attr('title', 'Show Labels');
    }
}
function showHideDedRemChartsDataLabels(flag) {

    options.yAxis.stackLabels.enabled = flag;
    options.plotOptions.column.dataLabels.enabled = flag;

    chart = new Highcharts.Chart(options);
    if (flag == true) {
        toggleDedVsRemChartsDataLabels = false;
        $("#dvHideShowDedRemDataLabels").attr('title', 'Hide Labels');
    }
    else {
        toggleDedVsRemChartsDataLabels = true;
        $("#dvHideShowDedRemDataLabels").attr('title', 'Show Labels');
    }
}
function showHideFundRecVsExpChartsDataLabels(flag) {

    optionsFundExpenditureChart.plotOptions.column.dataLabels.enabled = flag;
    chartFundExpenditureChart = new Highcharts.Chart(optionsFundExpenditureChart);

    if (flag == true) {
        toggleFundRecVsExpChartsDataLabels = false;
        $("#dvHideShowFundRecVsExpDataLabels").attr('title', 'Hide Labels');
    }
    else {
        toggleFundRecVsExpChartsDataLabels = true;
        $("#dvHideShowFundRecVsExpDataLabels").attr('title', 'Show Labels');
    }
}

///function to get the summary grid
function CreateAccountSummaryGrid(finYear) {
    //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    var levelId = $("#LevelId").val();

    //var title = "Account Summary";

    $("#tblOtherDetails").GridUnload();
    $("#tblOtherDetails").jqGrid('GridDestroy');

    $("#tblOtherDetails").jqGrid({
        url: '/AccountDashboard/GetAccountSummaryList/',
        datatype: "json",
        mtype: "POST",
        postData: {
            'fundType': $("#ddlFundType option:selected").val(),
            //'month': $("#ddlMonth option:selected").val(),
            'year': finYear,
            'level': function () {
                if (levelId == "5") {
                    return "5";
                }
                else {
                    return "4";
                }
            },
            'lowercode': function () {
                if (levelId == "5") {
                    return "0";
                }
                else {

                    if ($("#ddlStates option:selected").val() == 0) {
                        return "0";
                    } else {
                        if ($("#ddlDPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#ddlDPIU option:selected").val();
                        }
                    }

                    //if ($("#rdSRRDA").is(":checked")) {
                    //    return "0";
                    //} else {
                    //    if ($("#DPIU").val() == 0) {
                    //        return "0";
                    //    } else {
                    //        return $("#DPIU").val();
                    //    }
                    //}
                }
            },
            'ownLower': function () {
                return "L";

                //if (levelId == "5") {
                //    return "O";
                //} else {

                //    //SRRDA
                //    if (($("#ddlDPIU").val() == 0)) {
                //        return "O";
                //    }
                //    else {//DPIU
                //        return "L";
                //    }

                //    //if ($("#rdSRRDA").is(":checked")) {
                //    //    return "O";
                //    //} else {
                //    //    return "L";
                //    //}
                //}
            },
            'adminNdCode': $("#ddlStates option:selected").val()
        },
        colNames: ['Bill Type', 'Description', 'No.', "Amount", "No.", "Amount"],
        colModel: [
                            { name: 'BillType', index: 'BillType', width: 30, align: 'center', hidden: true },
                            { name: 'Description', index: 'Description', width: '60%', align: 'left' },
                            { name: 'Upto_Count', index: 'Upto_Count', width: '17%', align: 'center' },
                            {
                                name: 'Upto_Amount', index: 'Upto_Amount', width: '35%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 }
                            },
                            { name: 'Month_Count', index: 'Month_Count', width: '10%', align: 'center' },
                            {
                                name: 'Month_Amount', index: 'Month_Amount', width: '35%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 }
                            }
        ],
        pager: jQuery('#divOtherDetailsPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: false,
        sortname: 'BillType',
        sortorder: "asc",
        caption: "Account Summary " + finYear.toString().split('-')[0],
        height: 'auto',
        autowidth: true,
        footerrow: false,
        userDataOnFooter: true,
        loadComplete: function () {
            //$("#gview_tblOtherDetails > .ui-jqgrid-titlebar").hide();
            //jQuery('#divOtherDetailsPager_left').html("<span id='spnRefreshAccountSummary' class='ui-icon	ui-icon-arrowrefresh-1-s' title='Reload' style='float:left' onClick='ReloadAccountSummary();'></span>  &nbsp&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            jQuery('#divOtherDetailsPager_left').html("&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            $("#divOtherDetailsPager_center").html('');
            $("#tblOtherDetails").setGridWidth($("#gbox_tblOtherDetails").parent().width());

            //$("#tblOtherDetails").footerData('set', { "Upto_Count": "Total : " }, true); //set footer data

            $("#tblOtherDetails").footerData('set', { "Description": "<span style='margin-left:80%; font-size: 75%;'> Total : </span>" }, true); //set footer data

            var parseUptoAmountTotal = $(this).jqGrid('getCol', 'Upto_Amount', false, 'sum');
            //$(this).jqGrid('footerData', 'set', { Upto_Amount: parseUptoAmountTotal });
            $(this).jqGrid("footerData", "set",
               {
                   //HEAD_NAME: "Total:",
                   Upto_Amount: "<span style='font-size: 75%;'>" + numberWithCommas(parseUptoAmountTotal) + "</span>"
               },
               false
               );

            var parseMonthAmountTotal = $(this).jqGrid('getCol', 'Month_Amount', false, 'sum');
            //$(this).jqGrid('footerData', 'set', { Month_Amount: parseMonthAmountTotal });

            $(this).jqGrid("footerData", "set",
               {
                   //HEAD_NAME: "Total:",
                   Month_Amount: "<span style='font-size: 75%;'>" + numberWithCommas(parseMonthAmountTotal) + "</span>"
               },
               false
               );
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            alert("Invalid data.Please check and Try again!")

        }
    });


    jQuery("#tblOtherDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'Upto_Count', numberOfColumns: 2, titleText: 'Previous Month' },
                       { startColumnName: 'Month_Count', numberOfColumns: 2, titleText: 'Current Month' }]
    });

}

///function to get the Fund Received Vs Expenditure chart
function LoadFundExpenditureChart() {
    //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    //chart options
    optionsFundExpenditureChart = {
        chart: {
            renderTo: 'dvFundReceivedVsExpenditureColumnChart',
            type: 'column',
            //zoomType:'y',
            //margin: 75,
            //marginTop: 50,
            //marginRight: 40,
            marginTop: 30,
            marginRight: 15,
            marginLeft: 80,
            marginBottom: 75,
            options3d: {
                enabled: true,
                //alpha: 10,//15
                alpha: 11,//15
                beta: 02,//15
                //depth: 70,
                depth: 60,

                frame: {
                    side: {
                        size: 5,
                        //color: '#FCB319',
                        color: '#d3d3d3',
                    },
                    bottom: {
                        size: 5,
                        //color: '#FCB319',
                        color: '#d3d3d3',
                    }
                }
                //viewDistance:75
            }
        },
        credits: {
            enabled: false
        },
        legend: {
            //enabled:false,
            //title: {
            //    text: '<span style="font-size:9px;color:#666;font-weight:normal">(Click to hide)</span>'
            //},
            align: 'right',
            verticalAlign: 'top',
            //x: -70,
            //y: 60,
            x: -200,
            y: 20,
            floating: true,
            //layout: 'vertical',
            itemHoverStyle: {
                color: 'red'
            }
        },
        title: {
            text: ""
            // text: "Authorization Vs Expenditure",
            //style: {
            //    //color: '#FF00FF',
            //    fontWeight: 'bold',
            //    fontSize: '10px',
            //},
            //y:320
        },
        //subtitle: {
        //    text: "Fund Received Vs Expenditure"
        //},
        plotOptions: {
            column: {
                depth: 40,
                stacking: false,
                grouping: false,
                groupZPadding: 15,
                //dataLabels: {
                //    enabled: false,
                //    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'black',
                //    style: {
                //        textShadow: '0 0 1px black'
                //    }
                //}
            },
            series: {
                animation: {
                    duration: 2000,
                    //easing: 'easeOutBounce'
                },
                cursor: 'pointer',
                //innerSize: '50%',
                //dataLabels: {
                //    enabled: true,
                //    borderRadius: 5,
                //    backgroundColor: 'rgba(252, 255, 197, 0.7)',
                //    borderWidth: 1,
                //    borderColor: '#AAA',
                //    y: -6,                        
                //}
                point: {
                    events: {
                        click: function () {
                            //alert('Category: ' + this.category + ', value: ' + this.y);

                            DrillDownChart(this.category);


                        }
                    }
                },
                dataLabels: {
                    enabled: false,
                    // rotation: -90,
                    // color: '#FFFFFF',
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'black',
                    align: 'left',
                    format: '{point.y:.2f}', // one decimal
                    y: 30, // 10 pixels down from the top
                    zIndex: 10,
                    //borderRadius: 5,
                    //backgroundColor: 'rgba(252, 255, 197, 0.7)',
                    //borderWidth: 1,
                    style: {
                        fontSize: '10px',
                        fontFamily: 'Verdana, sans-serif'
                    }
                }

            },
            //startAngle: -90,
            //endAngle: 90,
            //center: ['50%', '75%']
        },
        xAxis: {
            categories: [],
            title: {
                //rotation: -45,
                text: "Financial Year"
            },
            labels: {
                //format: '{value}',
                //rotation: -20,
                style: {
                    fontSize: '11px',
                    fontFamily: 'Verdana',
                    //fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                }
            },
            //alternateGridColor: '#FDFFD5'
        },
        tooltip: {
            valueDecimals: 2,
            headerFormat: '<span style="font-size:10px"><b>{point.key} </b></span><table>',
            //pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            //    '<td style="padding:0"><b>{point.y:.1f} Rs. </b></td></tr>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.2f} Lacs. </b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            //useHTML: true
        },

        yAxis: {
            //min:100,
            title: {
                text: "Amount in Lacs."
            },
            labels: {
                format: '{value}'
            },
            alternateGridColor: '#FDFFD5'
        },
        series: [],
    }

    //ajax call to dynamically load column chart.

    //options.yAxis = [];
    chartFundExpenditureChart = new Highcharts.Chart(optionsFundExpenditureChart);

    chartFundExpenditureChart.showLoading();

    $.ajax({
        type: 'POST',
        url: '/AccountDashboard/FundExpenditureColumnChart',
        error: function (xhr, status, error) {
            alert("An Error occured while processing your request.");
            return false;
        },
        data: $("#frmAccountDashboard").serialize(),
        success: function (responce) {
            var incomeArray = [];
            var expenditureArray = [];
            var categories = [];

            var seriesDataArray = [];

            $.each(responce, function (item) {
                incomeArray.push(this.YEARLY_INCOME);
                expenditureArray.push(this.YEARLY_EXPN);
                categories.push(this.YEAR_ID);
            });



            var colors = Highcharts.getOptions().colors;

            seriesDataArray.push({
                name: "Expenditure",
                data: expenditureArray,
                //stack: this.StatckName                
                //color: colors[6]
                color: colors[3]
            });


            seriesDataArray.push({
                name: "Authorization",
                data: incomeArray,
                //stack: this.StatckName
                color: "#a6c96a"//old
                //color: colors[9]
                //color: colors[6]
                // color: colors[2]
            });
            optionsFundExpenditureChart.series = seriesDataArray;
            optionsFundExpenditureChart.xAxis.categories = categories;

            chartFundExpenditureChart = new Highcharts.Chart(optionsFundExpenditureChart);
        }
    });//end of ajax call

}

///function to get the Fund Received Vs Expenditure Grid
function LoadFundReceivedVsExpenditureGrid() {

    $("#tblFundReceivedVsExpenditure").jqGrid({
        url: '/AccountDashboard/FundExpenditureGrid/',
        datatype: "json",
        mtype: "POST",
        //postData: $("#frmAccountDashboard").serialize(),
        postData: {
            AgencyCode: $("#ddlStates").val(),
            DPIU: $("#ddlDPIU").val(),
            FundType: $("#ddlFundType").val()
        },

        colNames: ['Year', 'Income', 'Expenditure', ''],
        colModel: [
                            { name: 'YEAR_ID', index: 'YEAR_ID', width: '30%', align: 'center' },
                            {
                                name: 'YEARLY_INCOME', index: 'YEARLY_INCOME', width: '30%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 }
                            },
                            {
                                name: 'YEARLY_EXPN', index: 'YEARLY_EXPN', width: '30%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 }
                            },
                            { name: '', index: '', width: '2%', align: 'center' },

        ],
        pager: jQuery('#divFundReceivedVsExpenditurePager'),
        rowNum: 0,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        sortname: 'YEAR_ID',
        sortorder: "asc",
        caption: "Fund Received and Expenditure Details",
        //height: 'auto',
        autowidth: true,
        footerrow: true,
        userDataOnFooter: true,
        onSelectRow: function (rowid) {
            var grid = $('#tblFundReceivedVsExpenditure');
            var sel_id = grid.jqGrid('getGridParam', 'selrow');
            var myCellData = grid.jqGrid('getCell', sel_id, 'YEAR_ID');
            DrillDownChart(myCellData);
        },
        rownumbers: true,
        loadComplete: function () {
            jQuery('#divFundReceivedVsExpenditurePager_left').html("&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            $("#divFundReceivedVsExpenditurePager_center").html('');
            $("#jqgh_tblFundReceivedVsExpenditure_rn").html('S.No');
            $("#tblFundReceivedVsExpenditure").setGridWidth($("#dvWestSouth").width());
            var recordCount = jQuery('#tblFundReceivedVsExpenditure').jqGrid('getGridParam', 'reccount');
            if (recordCount > 5) {
                $("#tblFundReceivedVsExpenditure").parents('div.ui-jqgrid-bdiv').css("max-height", (parseInt($("#dvWestSouth").height()) - 100), true);
                $('#tblFundReceivedVsExpenditure').jqGrid('setGridWidth', 'auto');
            }
            else {
                $('#tblFundReceivedVsExpenditure').jqGrid('setGridHeight', 'auto');
            }
            $("#tblFundReceivedVsExpenditure").footerData('set', { "YEAR_ID": "<span style='margin-left:80%; font-size: 70%;'> Total : </span>" }, true); //set footer data
            var parseYearlyIncomeTotal = $(this).jqGrid('getCol', 'YEARLY_INCOME', false, 'sum');
            //$(this).jqGrid('footerData', 'set', { YEARLY_INCOME: parseYearlyIncomeTotal });

            $(this).jqGrid("footerData", "set",
              {
                  //HEAD_NAME: "Total:",
                  YEARLY_INCOME: "<span style='font-size: 75%;'>" + numberWithCommas(parseYearlyIncomeTotal) + "</span>"
              },
              false
              );

            var parseYearlyExpnTotal = $(this).jqGrid('getCol', 'YEARLY_EXPN', false, 'sum');
            //$(this).jqGrid('footerData', 'set', { YEARLY_EXPN: parseYearlyExpnTotal });

            $(this).jqGrid("footerData", "set",
              {
                  //HEAD_NAME: "Total:",
                  YEARLY_EXPN: "<span style='font-size: 75%;'>" + numberWithCommas(parseYearlyExpnTotal) + "</span>"
              },
              false
              );
        },
        loadError: function (xhr, ststus, error) {

            alert("Invalid data.Please check and Try again!")

        }
    });
}

///function to get the Expenditure Details Chart
function LoadExpenditureDetailsPieChart(finYear) {
    //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    // Radialize the colors for Pie chart
    //Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
    //    return {
    //        radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
    //        stops: [
    //            [0, color],
    //            [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
    //        ]
    //    };
    //});

    //Pie chart
    optionsExpenditureDetailsPieChart = {
        chart: {
            renderTo: 'dvPieChart',
            type: 'pie',
            options3d: {
                enabled: true,
                //alpha: 30,
                //beta: 20,
                alpha: 45,
                beta: 0,
            },
            //plotShadow:true,
            marginTop: 30,
            marginRight: 5,
            marginLeft: 65,
            marginBottom: 65,
        },
        credits: {
            enabled: false
        },
        title: {
            text: "Expenditure Details " + finYear,

            style: {
                //color: '#FF00FF',
                fontWeight: 'bold',
                fontSize: '10px',
            }

            // text: ""
        },
        //subtitle: {
        //    text: "Expenditure Details"
        //},
        plotOptions: {
            pie: {
                depth: 30,
                innerSize: 35,
                //center:[500,100],
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    format: '<b> {point.name} </b> : {point.percentage:.1f}%',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    },
                    //distance: -50,
                    //color:'white',
                },
                startAngle: -30,
                showInLegend: true,
                //connectorColor:'silver'
                connectorColor: 'silver'
            },
            //        startAngle: -90,
            //endAngle: 90,
            //center: ['50%', '75%']
        },
        series: [
            {
                name: '<b>Expenditure</b>',
                //  innerSize: '50%',               
                data: [],
            },
        ],

        tooltip: {
            pointFormat: '{series.name}  <b>{point.y:.2f} Lacs. ( {point.percentage:.2f}% )</b>',
            percentageDecimals: 2

        }
        //    tooltip: {
        //    enabled: true,
        //    animation: true,
        //    formatter: function () {
        //    return ' ' +
        //                  'Head: ' + this.point.HEAD_CODE + '<br />' +
        //                  //'Percentage: ' + this.point.y + "%" + '<br />' +
        //                  'Amount: Rs.' + this.point.Expn + " Lacs";
        //},
        //// pointFormat: '{series.name}: <b>{point.percentage}%</b>',
        //    percentageDecimals: 2

        //},
    }//end of options

    ExpenditureDetailsPieChart = new Highcharts.Chart(optionsExpenditureDetailsPieChart);
    ExpenditureDetailsPieChart.showLoading();

    //Ajax Call to retrive data

    $.ajax({
        type: 'POST',
        url: '/AccountDashboard/ExpenditureDetailsPieChart?id=' + finYear,
        error: function (xhr, status, error) {
            alert("An Error occured while processing your request.");
            return false;
        },
        data: $("#frmAccountDashboard").serialize(),
        success: function (responce) {

            var colors = Highcharts.getOptions().colors;
            var i = 1;
            var dataItem = new Array();
            $.each(responce, function (item) {

                if (i == 1) {
                    dataItem.push({
                        name: this.HEAD_CODE,
                        y: this.Expn,
                        color: colors[i + 1],
                        sliced: true,
                        selected: true
                    });
                } else {
                    dataItem.push({
                        name: this.HEAD_CODE,
                        y: this.Expn,
                        color: colors[i + 1],
                        sliced: false,
                        selected: false
                    });
                }
                i++;
            });

            optionsExpenditureDetailsPieChart.series[0].data = dataItem;

            ExpenditureDetailsPieChart = new Highcharts.Chart(optionsExpenditureDetailsPieChart);
        }
    });//end of ajax call
}

///function to get the Expenditure Details Grid
function LoadExpenditureDetailsGrid(FinYear) {

    $("#tblExpenditureDetails").jqGrid({
        url: '/AccountDashboard/ExpenditureDetailsGrid/',
        datatype: "json",
        mtype: "POST",
        //postData: $("#frmAccountDashboard").serialize(),
        postData: {
            AgencyCode: $("#ddlStates").val(),
            DPIU: $("#ddlDPIU").val(),
            FundType: $("#ddlFundType").val(),
            Year: FinYear
        },

        colNames: ['Head', 'Expenditure'],
        colModel: [
                            { name: 'HEAD_CODE', index: 'HEAD_CODE', width: '30%', align: 'center' },
                            {
                                name: 'Expn', index: 'Expn', width: '30%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 }
                            },
        ],
        pager: jQuery('#divExpenditureDetailsPager'),
        rowNum: 0,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        sortname: 'HEAD_CODE',
        sortorder: "asc",
        caption: "Expenditure Details " + FinYear,
        height: 'auto',
        autowidth: true,
        //shrinkToFit:true,
        footerrow: true,
        userDataOnFooter: true,
        rownumbers: true,
        loadComplete: function () {
            jQuery('#divExpenditureDetailsPager_left').html("&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            $("#divExpenditureDetailsPager_center").html('');
            $("#jqgh_tblExpenditureDetails_rn").html('S.No');

            $("#tblExpenditureDetails").setGridWidth($("#gbox_tblFundReceivedVsExpenditure").parent().width());
            //$("#tblExpenditureDetails").parents('div.ui-jqgrid-bdiv').css("max-height", "320px");

            //$("#tblOtherDetails").setGridWidth(1200);
            $("#tblExpenditureDetails").footerData('set', { "HEAD_CODE": "<span style='margin-left:85%; font-size: 75%;'> Total : </span>" }, true); //set footer data
            var parseExpnTotal = $(this).jqGrid('getCol', 'Expn', false, 'sum');
            // $(this).jqGrid('footerData', 'set', { Expn: parseExpnTotal });

            $(this).jqGrid("footerData", "set",
              {
                  //HEAD_NAME: "Total:",
                  Expn: "<span style='font-size: 75%;'>" + numberWithCommas(parseExpnTotal) + "</span>"
              },
              false
              );
        },
        loadError: function (xhr, ststus, error) {
            alert("Invalid data.Please check and Try again!")
        }
    });



}

//function to get the Deduction Vs Remittence Chart
function LoadDeductionVsRemittenceColumnChart(finYear, pointWidthDedVsRem) {
    //   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    //chart options
    options = {
        chart: {
            renderTo: 'dvDeductionVsRemittenceColumnChart',
            type: 'column',
            //zoomType:'y',
            //margin:30,
            marginTop: 25,
            marginRight: 25,
            marginLeft: 65,
            marginBottom: 75,
            //width:300,
            options3d: {
                enabled: true,
                //alpha: 10,
                //beta: 10,
                //depth: 70,
                alpha: 7,
                beta: 6,
                depth: 70,
                //viewDistance: 25,                
                frame: {
                    side: {
                        size: 5,
                        // color: '#FCB319',
                        color: '#d3d3d3',
                        //color: '#C0C0C0',
                    },
                    bottom: {
                        size: 5,
                        //color: '#FCB319',
                        color: '#d3d3d3',
                    }
                }
                // ,viewDistance:75
            }

        },
        credits: {
            enabled: false
        },
        legend: {
            enabled: false,
            //title: {
            //    text: '<span style="font-size:9px;color:#666;font-weight:normal">(Click to hide)</span>'
            //},
            align: 'center',
            verticalAlign: 'top',
            x: 40,
            y: 35,
            //x: 10,
            //y: 20,
            floating: true,
            //layout: 'vertical',
            itemHoverStyle: {
                color: 'red'
            }
        },
        title: {
            text: "Deduction Vs Remittance ",
            style: {
                //color: '#FF00FF',
                fontWeight: 'bold',
                fontSize: '10px',
            }
            //text: ""
        },
        //subtitle: {
        //    text: "Deduction Vs Remittence"
        //},
        plotOptions: {
            column: {
                depth: 40,
                //stacking: true,
                //grouping: false,
                stacking: 'normal',
                //groupZPadding: 15    

                dataLabels: {
                    enabled: false,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                    style: {
                        textShadow: '0 0 1px black'
                    }
                }
            },
            series: {
                animation: {
                    duration: 1500,
                    //easing: 'easeOutBounce'
                },
                //colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']

                colors: ['#058DC7']

                //colorByPoint: true,
                , pointWidth: pointWidthDedVsRem
                //,pointPadding: 0.2,
                //groupPadding: 0,
                //borderWidth: 0,
                //shadow: false
                //dataLabels: {
                //    enabled: true,
                //    borderRadius: 5,
                //    backgroundColor: 'rgba(252, 255, 197, 0.7)',
                //    borderWidth: 1,
                //    borderColor: '#AAA',
                //    y: -6,                        
                //}
            },

        },
        //colors: ['#2f7ed8', '#0d233a', '#910000', '#1aadce', '#492970', '#f28f43', '#77a1e5', '#c42525', '#a6c96a', '#4572A7', '#AA4643', '#89A54E', '#80699B', '#3D96AE'],
        xAxis: {
            categories: [],
            title: {
                //rotation: -45,
                text: "Financial Year",
                //align: 'low',
                //y: -10
            },
            labels: {
                //rotation: -20,
                // style: {
                //fontSize: '9px',
                //fontFamily: 'Verdana, sans-serif'
                //fontFace:'bold'
                //}
                style: {
                    fontSize: '11px',
                    fontFamily: 'Verdana',
                    //fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                }
            },
            //alternateGridColor: '#FDFFD5'
        },
        tooltip: {
            valueDecimals: 2,
            headerFormat: '<span style="font-size:10px"><b> {point.key} </b></span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.2f} Lacs.</b> </td></tr>' +
                '<tr><td style="color:{series.color};padding:0"></br>Total :</td>' +
                '<td style="padding:0"><b>{point.stackTotal} Lacs.</b></td></tr>',
            footerFormat: '</table>',
            shared: false,
            //useHTML: true
        },

        yAxis: {
            //min:100,
            title: {
                text: "Amount in Lacs."
            },
            labels: {
                format: '{value}'
            },
            alternateGridColor: '#FDFFD5',
            stackLabels: {
                enabled: false,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                }
            }
        },
    }

    var seriesData = [];
    options.series = seriesData;

    options.legend.enabled = toggleDedvsRemLegend;
    //toggleDedvsRemDataLabels = true;
    //ajax call to dynamically load column chart.
    chart = new Highcharts.Chart(options);
    chart.showLoading();

    $.ajax({
        type: 'POST',
        url: '/AccountDashboard/DeductionRemttancesColumnStackChart?id=' + finYear,
        data: $("#frmAccountDashboard").serialize(),
        error: function (xhr, status, error) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {
            var colors = Highcharts.getOptions().colors;
            var i = 2;
          
            $.each(responce.DeductionRemittanceData, function (item) {

                if (i == 1) {
                    seriesData.push({
                        name: this.HeadName,
                        data: this.HeadArrayDeductionsRemiAmount,
                        stack: this.StatckName,
                        color: '#339999'
                    });
                }
                if (i == 7) {
                    seriesData.push({
                        name: this.HeadName,
                        data: this.HeadArrayDeductionsRemiAmount,
                        stack: this.StatckName,
                        color: '#058DC7'
                    });
                }
                else {
                    seriesData.push({
                        name: this.HeadName,
                        data: this.HeadArrayDeductionsRemiAmount,
                        stack: this.StatckName,
                        color: colors[i]
                    });
                }
                i++;

                if (i >= 11) {
                    i = 0;
                }
            });

            options.xAxis.categories = responce.lstYearText;
            options.series = seriesData;
            chart = new Highcharts.Chart(options);
        }
    });//end of ajax call

}
///function to get the Deduction Vs Remittence
function LoadDeductionVsRemittenceGrid(FinYear) {

    $("#tblDeductionVsRemittence").jqGrid({
        url: '/AccountDashboard/DeductionRemittancesGrid/',
        datatype: "json",
        mtype: "POST",
        postData: {
            AgencyCode: $("#ddlStates").val(),
            DPIU: $("#ddlDPIU").val(),
            FundType: $("#ddlFundType").val(),
            Year: FinYear
        },
        colNames: ['Year', 'Ded Rem', 'Fin Year', 'Head', 'Amount', ''],
        colModel: [
                            { name: 'YEAR_ID', index: 'YEAR_ID', width: '30%', align: 'center', hidden: true },
                            { name: 'DRType', index: 'DRType', width: '30%', align: 'center' },
                            { name: 'YEAR_TEXT', index: 'YEAR_TEXT', width: '30%', align: 'center' },
                            { name: 'HEAD_NAME', index: 'HEAD_NAME', width: '30%', align: 'left' },
                            {
                                name: 'DED_AMOUNT', index: 'Expn', width: '30%', align: 'right', formatter: 'number',
                                formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 },
                                summaryTpl: "<span style='font-size: 90%;'>Total : {0}</span>",
                                summaryType: "sum"//summary sum
                            },
                            { name: '', index: '', width: '1%', align: 'center' },

        ],
        pager: jQuery('#divFDeductionVsRemittencePager'),
        rowNum: 0,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        sortname: 'YEAR_ID',
        sortorder: "asc",
        caption: "Deduction and Remittence Details " + FinYear,
        height: 'auto',
        autowidth: true,
        //width:'auto',
        //shrinkToFit:true,
        footerrow: false,
        userDataOnFooter: true,
        grouping: true,
        groupingView: {
            groupField: ['YEAR_TEXT', 'DRType'],
            //groupDataSorted: true,
            //groupText: '',
            //groupText: ['<b> {0}  </b>  Total Amount: {Amount}'],
            //groupText: ['{0}'],
            groupText: ["<span style='font-size: 100%;'><b>{0}</b></span>", "<span style='font-size: 100%;'><b>{0}</b></span>"],
            groupSummary: [false, true],
            groupCollapse: true,
            groupColumnShow: false,
            showSummaryOnHide: true
        },
        rownumbers: true,
        loadComplete: function () {
            //$("#gview_tblOtherDetails > .ui-jqgrid-titlebar").hide();
            //jQuery('#divOtherDetailsPager_left').html("<span id='spnRefreshAccountSummary' class='ui-icon	ui-icon-arrowrefresh-1-s' title='Reload' style='float:left' onClick='ReloadAccountSummary();'></span>  &nbsp&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            jQuery('#divFDeductionVsRemittencePager_left').html("&nbsp<b>  Note :   </b> All Amount are in Lacs ");
            $("#divFDeductionVsRemittencePager_center").html('');
            $("#jqgh_tblDeductionVsRemittence_rn").html('S.No');
            //dvWestSouth            
            $("#tblDeductionVsRemittence").setGridWidth($("#dvCenter").width());

            $("#tblDeductionVsRemittence").parents('div.ui-jqgrid-bdiv').css("max-height", (parseInt($("#dvCenter").height()) - 120), true);

            $("#tblDeductionVsRemittence").footerData('set', { "HEAD_NAME": "<span style='margin-left:72%; font-size: 75%;'>Grand Total : </span>" }, true); //set footer data

            var parseDedAmtTotal = $(this).jqGrid('getCol', 'DED_AMOUNT', false, 'sum');

            $(this).jqGrid("footerData", "set",
                {
                    //HEAD_NAME: "Total:",
                    DED_AMOUNT: "<span style='font-size: 75%;'>" + numberWithCommas(parseDedAmtTotal) + "</span>"
                },
                false
                );
        },
        loadError: function (xhr, ststus, error) {
            alert("Invalid data.Please check and Try again!");
        }
    });
}


function ReloadAccountSummary() {
    CreateAccountSummaryGrid($("#currentFinYear").val());
}

function DrillDownChart(finYear) {
    $("#spnShowDedVsRemittanceChart").trigger('click');
    $("#spnShowExpenditureDetailsChart").trigger('click');
    LoadDeductionVsRemittenceColumnChart(finYear, 30);
    $("#tblDeductionVsRemittence").GridUnload();
    LoadDeductionVsRemittenceGrid(finYear);
    LoadExpenditureDetailsPieChart(finYear);
    $("#tblExpenditureDetails").GridUnload();
    LoadExpenditureDetailsGrid(finYear);
    CreateAccountSummaryGrid(finYear);
}

function numberWithCommas(x) {
    return x.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}