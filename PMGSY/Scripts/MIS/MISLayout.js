var tblstring;
//Global Variable For Chart1
var StateNamestr11;
var ChequeAmountstr11;
var TotalPaymentMadestr11;
var sumChqAmount11;
var sumPaymentMade11;



//Global Variable For Chart2
var FinancialYearstr22;
var ChequeAmountstr22;
var TotalPaymentMadestr22;
var sumChqAmount22;
var sumPaymentMade22;

//Global Variable For Chart3
var Monthstr33;
var TotalPaymentMadestr33;
var objPieChartdata = new Array();
var sumPaymentMade33;

//Global Variable For Chart4
var Monthstr44;
var ChequeAmountstr44;
var sumChqAmount44;



var DSCStateNamestr1;
var DSCFinalizedstr1;
var DSCVerifiedstr1;
var sumDSCFinalized1;
var sumDSCVerified1;

var BeneficiaryStateNamestr1;
var BeneficiaryFinalizedstr1;
var BeneficiaryTotalstr1;
var BeneficiaryVerifiedstr1;
var sumTotalBeneficiary1;
var sumBeneficiaryFinalized1;
var sumBeneficiaryVerified1;
var customers = new Array();

$(document).ready(function () {


    //Chart1 view Close
    $("#viewchart1id").click(function () {
        var tableString;
        for (var i = 0; i < 3; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:8%;'><span style='color:orange;'>State Name</span></th>";
                for (var j = 0; j < StateNamestr11.length; j++) {

                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + StateNamestr11[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                tableString += "/<tr>";

            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'> Cheque Amount(In Lacs.) </span></th> ";
                for (var j = 0; j < ChequeAmountstr11.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr11[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount11 + "</td>";
                tableString += "/<tr>";

            }
            if (i == 2) {
                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'> Total Payment Made </span> </th>";
                for (var j = 0; j < TotalPaymentMadestr11.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr11[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade11 + "</td>";
                tableString += "/<tr>";
            }
        }


        $("#dataTable1").html(tableString);

        tableString = "";

        var tblstring = tableString;
        $("#dataTable1").show();


        $("#viewchart1id").hide();
        $("#closechart1id").show();



    });

    $("#closechart1id").click(function () {
        $("#dataTable1").hide();
        $("#closechart1id").hide();
        $("#viewchart1id").show();

    });

    //Chart2 View Close
    
    $("#viewchart2id").click(function () {
        var tableString;
        for (var i = 0; i < 3; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Financial Year </span></th>";
                for (var j = 0; j < FinancialYearstr22.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + FinancialYearstr22[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                tableString += "/<tr>";

            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Cheque Amount(In Lacs.) </span> </th> ";
                for (var j = 0; j < ChequeAmountstr22.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr22[j] + "</td>";
                }

                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount22 + "</td>";
                tableString += "/<tr>";

            }

            if (i == 2) {
                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'>  Total Payment Made </span></th> ";
                for (var j = 0; j < TotalPaymentMadestr22.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr22[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade22 + "</td>";
                tableString += "/<tr>";

            }



        }

        $("#dataTable2").html(tableString);

        tableString = "";
        $("#dataTable2").show();



            $("#viewchart2id").hide();
            $("#closechart2id").show();



        });

        $("#closechart2id").click(function () {
            $("#dataTable2").hide();
            $("#closechart2id").hide();
            $("#viewchart2id").show();

        });


    //Chart3 View Close
    $("#viewchart3id").click(function () {
        var tableString;
        for (var i = 0; i < 2; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Month <span></th>";
                for (var j = 0; j < Monthstr33.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + Monthstr33[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";

                tableString += "/<tr>";

            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Total Payment Made </span></th> ";
                for (var j = 0; j < TotalPaymentMadestr33.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr33[j] + "</td>";
                }

                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade33 + "</td>";
                tableString += "/<tr>";

            }
        }

        $("#dataTable3").html(tableString);

        tableString = "";
        $("#dataTable3").show();


        $("#viewchart3id").hide();
        $("#closechart3id").show();
    });

    $("#closechart3id").click(function () {
        $("#dataTable3").hide();
        $("#closechart3id").hide();
        $("#viewchart3id").show();

    });








    //Chart4 View Close

    $("#viewchart4id").click(function () {
        var tableString;
        for (var i = 0; i < 2; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:4%;'> <span style='color:orange;'> Months </span></th>";
                for (var j = 0; j < Monthstr44.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + Monthstr44[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";

                tableString += "/<tr>";

            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:4%;'>  <span style='color:orange;'> Cheque Amount(In Lacs.) </span></th> ";
                for (var j = 0; j < ChequeAmountstr44.length; j++) {
                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr44[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount44 + "</td>";
                tableString += "/<tr>";

            }

        }

        $("#dataTable4").html(tableString);

        tableString = "";
        $("#dataTable4").show();


        $("#viewchart4id").hide();
        $("#closechart4id").show();
    });

    $("#closechart4id").click(function () {
        $("#dataTable4").hide();
        $("#closechart4id").hide();
        $("#viewchart4id").show();

    });




    //ChartDSC View Close

    $("#viewdscid").click(function () {
        var tableString;
        for (var i = 0; i < 3; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:6%;'> <span style='color:orange;'> State Name </span></th>";
                for (var j = 0; j < DSCStateNamestr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + DSCStateNamestr1[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                tableString += "/<tr>";
            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:6%;'>  <span style='color:orange;'>  DSC Finalized </span></th> ";
                for (var j = 0; j < DSCFinalizedstr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + DSCFinalizedstr1[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + sumDSCFinalized1 + "</td>";
                tableString += "/<tr>";
            }
            if (i == 2) {
                tableString += "<tr><th style='text-align:center;width:6%;'>  <span style='color:orange;'>  DSC Verified </span></th>";
                for (var j = 0; j < DSCVerifiedstr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + DSCVerifiedstr1[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + sumDSCVerified1 + "</td>";
                tableString += "/<tr>";
            }
        }
        $("#dataTableDSC").html(tableString);
        tableString = "";
        $("#dataTableDSC").show();
        $("#viewdscid").hide();
        $("#closedscid").show();
    });

    $("#closedscid").click(function () {
        $("#dataTableDSC").hide();
        $("#closedscid").hide();
        $("#viewdscid").show();

    });



    //ChartBeneficiary View Close

    $("#viewbeneficiaryid").click(function () {
        var tableString;

        for (var i = 0; i < 4; i++) {
            if (i == 0) {
                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'>  State Name </span></th>";
                for (var j = 0; j < BeneficiaryStateNamestr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + BeneficiaryStateNamestr1[j] + '</b>' + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";

                tableString += "/<tr>";

            }
            if (i == 1) {
                tableString += "<tr><th style='text-align:center;width:8%;'>  <span style='color:orange;'> Total Beneficiary in OMMAS</span></th> ";
                for (var j = 0; j < BeneficiaryTotalstr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryTotalstr1[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + sumTotalBeneficiary1 + "</td>";

                tableString += "/<tr>";

            }
            if (i == 2) {
                tableString += "<tr><th style='text-align:center;width:8%;'>  <span style='color:orange;'> Beneficiary Finalized in OMMAS </span></th>";
                for (var j = 0; j < BeneficiaryFinalizedstr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryFinalizedstr1[j] + "</td>";

                }
                tableString += "<td style='text-align:center;width:3%;'>" + sumBeneficiaryFinalized1 + "</td>";
                tableString += "/<tr>";
            }
            if (i == 3) {
                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'>Beneficiary Verified by PFMS</span></th>";
                for (var j = 0; j < BeneficiaryVerifiedstr1.length; j++) {
                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryVerifiedstr1[j] + "</td>";
                }
                tableString += "<td style='text-align:center;width:3%;'>" + sumBeneficiaryVerified1 + "</td>";
                tableString += "/<tr>";
            }

        }

        $("#dataTableBeneficiary").html(tableString);

        tableString = "";
        $("#dataTableBeneficiary").show();

        $("#viewbeneficiaryid").hide();
        $("#closebeneficiaryid").show();
    });

    $("#closebeneficiaryid").click(function () {
        $("#dataTableBeneficiary").hide();

        $("#closebeneficiaryid").hide();
        $("#viewbeneficiaryid").show();

    });


    $("#tabOne1").click(function () {
        $("#viewchart1id").show();
        $("#closechart1id").hide();

        $("#viewchart2id").show();
        $("#closechart2id").hide();

        $("#viewchart3id").show();
        $("#closechart3id").hide();

        $("#viewchart4id").show();
        $("#closechart4id").hide();

        $("#viewdscid").show();
        $("#closedscid").hide();

        $("#viewbeneficiaryid").show();
        $("#closebeneficiaryid").hide();

    });

    $("#tabOne2").click(function () {

        $("#viewchart1id").show();
        $("#closechart1id").hide();

        $("#viewchart2id").show();
        $("#closechart2id").hide();

        $("#viewchart3id").show();
        $("#closechart3id").hide();

        $("#viewchart4id").show();
        $("#closechart4id").hide();

        $("#viewdscid").show();
        $("#closedscid").hide();

        $("#viewbeneficiaryid").show();
        $("#closebeneficiaryid").hide();



    });

   








   
    $("#tabs-3TierDetails").show();

    $("#tabs-3TierDetails").tabs();
    $('#tabs-3TierDetails ul').removeClass('ui-widget-header');

    $("#tbl1").show();
    $("#tbl2").show();
    $("#tbl3").show();
    $("#tblDSC").show();
    $("#tblBeneficiary").show();


    //$("#tblDSC").show();
    //$("#tblBeneficiary").show();


    //Chart 1 DATA
    StateNamestr11 = StateNamestr1.split(',');
    ChequeAmountstr11 = ChequeAmountstr1.split(',');
    TotalPaymentMadestr11 = TotalPaymentMadestr1.split(',');
    sumChqAmount11 = sumChqAmount1;
    sumPaymentMade11 = sumPaymentMade1;

    //Chart 2 DATA
    FinancialYearstr22 = FinancialYearstr2.split(',');
    ChequeAmountstr22 = ChequeAmountstr2.split(',');
    TotalPaymentMadestr22 = TotalPaymentMadestr2.split(',');
    sumChqAmount22 = sumChqAmount2;
    sumPaymentMade22 = sumPaymentMade2;

    //Chart 3 DATA
    Monthstr33 = Monthstr3.split(',');
    TotalPaymentMadestr33 = TotalPaymentMadestr3.split(',');
    sumPaymentMade33 = sumPaymentMade3;

    //Chart 4 DATA
    Monthstr44 = Monthstr4.split(',');
    ChequeAmountstr44 = ChequeAmountstr4.split(',');
    sumChqAmount44 = sumChqAmount4;

    //DSC
    DSCStateNamestr1 = DSCStateNamestr.split(',');
    DSCFinalizedstr1 = DSCFinalizedstr.split(',');
    DSCVerifiedstr1 = DSCVerifiedstr.split(',');
    sumDSCFinalized1 = sumDSCFinalized;
    sumDSCVerified1 = sumDSCVerified;

    //Beneficiary
    BeneficiaryStateNamestr1 = BeneficiaryStateNamestr.split(',');
    BeneficiaryTotalstr1 = BeneficiaryTotalstr.split(',');
    BeneficiaryFinalizedstr1 = BeneficiaryFinalizedstr.split(',');
    BeneficiaryVerifiedstr1 = BeneficiaryVerifiedstr.split(',');
    sumTotalBeneficiary1 = sumTotalBeneficiary;
    sumBeneficiaryFinalized1 = sumBeneficiaryFinalized;
    sumBeneficiaryVerified1 = sumBeneficiaryVerified;

    //Call to Chart 1 on Load
    LoadChart1()

    //Call to Chart 2 on Load
    LoadChart2('');

    //Call to Chart 3 on Load
    LoadChart3('');

    //Call to Chart 4 on Load
    LoadChart4('');

    LoadChartDSC();

    LoadChartBeneficiary();

    //$("#dialog").dialog({
    //    open: function (event, ui) {
    //        console.log('dialog open');
    //        //$("#divloadReport").html(htmlRes);
    //        $(event.target).parent().css('background-color', 'white');
    //        $(event.target).parent().css('color', 'white');
    //    },
    //    width: 1830,
    //    height: 500,
    //    autoOpen: false,
    //    position: {
    //        my: "left top",
    //        at: "left top",
    //        of: "#mainDiv"
    //    },
    //});


});


function LoadChart1() {
    
    blockPage();

    $('#container1').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'State wise total Payment made and cheque Amount'
        },
        /*
        subtitle: {
            text: 'Source:Payment Status'
        },
        */
        xAxis: [{
            categories: StateNamestr11,
            crosshair: true,
            title: {
                text: 'State Names',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        }],
        yAxis: [{ // Primary yAxis
            labels: {
                ///format: '{value} lakhs',
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            title: {
                text: 'Total Payment made',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        }, { // Secondary yAxis
            title: {
                text: 'Cheque Amount (In Lacs.)',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: true
        }],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 120,
            verticalAlign: 'top',
            y: 100,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || 'rgba(255,255,255,0.25)'
        },
        series: [{
            name: 'Cheque Amount (In Lacs)',
            type: 'column',
            yAxis: 1,
            data: JSON.parse("[" + ChequeAmountstr11 + "]"),
            tooltip: {
                valueSuffix: ' Lacs'
            },
            color: '#ffc081',
        }, {
            name: 'Total payment made',
            type: 'spline',
            data: JSON.parse("[" + TotalPaymentMadestr11 + "]"),
            tooltip: {
                valueSuffix: ''
            },
            color: '#000000',
        }],
        shadow: true,

       /*


        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',
                    //text:'<b class=ui-icon ui-icon-search>View</b>',
                    onclick: function () {
                        var tableString;
                        for (var i = 0; i < 3; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:8%;'><span style='color:orange;'>State Name</span></th>";
                                for (var j = 0; j < StateNamestr11.length; j++)
                                {
                                    
                                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + StateNamestr11[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'> Cheque Amount(In Lakhs.) </span></th> ";
                                for (var j = 0; j < ChequeAmountstr11.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr11[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount11 + "</td>";
                                tableString += "/<tr>";

                            }
                            if (i == 2) {
                                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'> Total Payment Made </span> </th>";
                                for (var j = 0; j < TotalPaymentMadestr11.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr11[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade11 + "</td>";
                                tableString += "/<tr>";
                            }
                        }


                        
                        
                        $("#dataTable1").html(tableString);
                        
                        tableString = "";

                        var tblstring = tableString;
                        $("#dataTable1").show();

                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',

                    //enabled: false,

                    onclick: function () {
                        $("#dataTable1").hide();
                    }
                }
                
            }
        },

        */

        plotOptions: {
            series: {
                point: {
                    events: {
                        click: function (e) {

                            //blockPage();
                            //setTimeout(function () { unblockPage(); }, 1500);

                            //BlockUI();
                            //$.unblockUI();
                            
                            blockPage();

                            $("#dataTable1").hide();
                            $("#dataTable2").hide();
                            $("#dataTable3").hide();
                            $("#dataTable4").hide();
                            
                            var seriesName = e.point.series.name;
                            var id = e.point.x;
                            var category = e.point.category;
                            var stateID = category + "_" + id;

                            

                            
                            if (seriesName == "Cheque Amount (In Lacs)" || seriesName == "Total payment made") {
                                var StateShortName = category;
                                $.ajax({
                                    url: '/MIS/ReteriveStateWiseDataForMISPayment/',
                                    type: "POST",
                                    catche: false,
                                    data: { "StateShortName": StateShortName.toString() },
                                    //async: false,
                                    async: true,
                                    success: function (data) {
                                        if (data.Success) {


                                            $("#viewchart2id").show();
                                            $("#closechart2id").hide();

                                            $("#viewchart3id").show();
                                            $("#closechart3id").hide();

                                            $("#viewchart4id").show();
                                            $("#closechart4id").hide();

                                            //Call to Reload Chart2
                                            $("#container2").highcharts().destroy();
                                            FinancialYearstr22 = data.FinancialYearstr2.split(',');
                                            ChequeAmountstr22 = data.ChequeAmountstr2.split(',');;
                                            TotalPaymentMadestr22 = data.TotalPaymentMadestr2.split(',');
                                            sumPaymentMade22 = data.sumPaymentMade2;
                                            sumChqAmount22 = data.sumChqAmount2;
                                            

                                            LoadChart2('<br/>State:' + StateShortName);

                                            //Call to Reload Chart3
                                            objPieChartdata.length = 0;
                                            Monthstr33 = data.Monthstr3.split(',');
                                            TotalPaymentMadestr33 = data.TotalPaymentMadestr3.split(',');
                                            sumPaymentMade33 = data.sumPaymentMade3;
                                            LoadChart3('<br/>State:' + StateShortName);

                                            //Call to Reload Chart4
                                            $("#container4").highcharts().destroy();
                                            Monthstr44 = data.Monthstr4.split(',');
                                            ChequeAmountstr44 = data.ChequeAmountstr4.split(',');
                                            sumChqAmount44 = data.sumChqAmount4;
                                            LoadChart4('<br/>State:' + StateShortName);
                                            unblockPage();

                                            //setTimeout(function () { unblockPage(); }, 1500);
                                        } else {
                                            unblockPage();
                                            alert("An Error Occur while Processing,please try Again");
                                        }

                                    },
                                    error: function () {
                                        unblockPage();
                                        alert("An Error Occur while Processing,please try Again");
                                        return false;
                                    },
                                });

                            }
                        }
                    }
                }
            }
        }
    });

    setTimeout(function () { unblockPage(); }, 1500);
}


function LoadChart2(StateShortName) {

    
    var StateShortName1;
    var tagValue;
    var l = StateShortName.length;
    
    if (l > 0) {
        var str = StateShortName.split(':');
        var splitStateName = str[1];
        StateShortName1 = GetStateNameFromStateCode(splitStateName);
        tagValue = '<br/>State:' + StateShortName1 + '</b>';
    } else {
        tagValue = '';
    }

    //alert(StateShortName);
    //var StateShortName1 = GetStateNameFromStateCode(StateShortName);

    $('#container2').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'Financial year wise payment status' + tagValue
        },

        /*
        subtitle: {
            text: 'Source:Payment Status'
        },
        */
        xAxis: [{
            //categories: ['2018-19', '2019-20', '2020-21', '2021-22', '2022-23', '2023-24',
            //  '2024-25', '2025-26', '2026-27', '2027-28', '2028-29', '2029-2030'],
            categories: FinancialYearstr22,
            crosshair: true,
            title: {
                text: 'Financial Year',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        }],
        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            title: {
                text: 'Total Payment made',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        }, { // Secondary yAxis
            title: {
                text: 'Cheque Amount (In Lacs.)',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: true
        }],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 120,
            verticalAlign: 'top',
            y: 100,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || 'rgba(255,255,255,0.25)'
        },
        series: [{
            name: 'Cheque Amount (In Lacs)',
            type: 'column',
            yAxis: 1,
            //data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4],
            data: JSON.parse("[" + ChequeAmountstr22 + "]"),
            tooltip: {
                valueSuffix: ' Lacs'
            },
            color: '#ffc081',
            pointWidth: 28
        }, {
            name: 'Total payment made',
            type: 'spline',
            //data: [7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6],
            data: JSON.parse("[" + TotalPaymentMadestr22 + "]"),
            tooltip: {
                valueSuffix: ''
            },
            color: '#000000',
        }],

        //exporting: {
        //    buttons: {
        //        customButton: {
        //            text: 'View Data Table',
        //            onclick: function () {


        //                alert(FinancialYearstr22);
        //                alert(ChequeAmountstr22);
        //                alert(TotalPaymentMadestr22);




        //            }
        //        }
        //    }
        //},


        /*
        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',

                    onclick: function () {

                        var tableString;
                        for (var i = 0; i < 3; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Financial Year </span></th>";
                                for (var j = 0; j < FinancialYearstr22.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + FinancialYearstr22[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Cheque Amount(In Lakhs.) </span> </th> ";
                                for (var j = 0; j < ChequeAmountstr22.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr22[j] + "</td>";
                                }

                                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount22 + "</td>";
                                tableString += "/<tr>";

                            }

                            if (i == 2) {
                                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'>  Total Payment Made </span></th> ";
                                for (var j = 0; j < TotalPaymentMadestr22.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr22[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade22 + "</td>";
                                tableString += "/<tr>";

                            }



                        }

                        $("#dataTable2").html(tableString);

                        tableString = "";
                        $("#dataTable2").show();


                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',
                    onclick: function () {
                        $("#dataTable2").hide();
                    }
                }
            }
        },

        */


        plotOptions: {
            series: {
                point: {
                    events: {
                        click: function (e) {
                             
                            //First Time
                            if (StateShortName == "undefined" || StateShortName == null || StateShortName=="") {
                                    StateShortName = "s";
                            } else {

                                var Statename = StateShortName.split(':');
                                StateShortname = Statename[1];
                                StateShortName = StateShortname;
                            }

                            blockPage();
                            $("#dataTable1").hide();
                            $("#dataTable2").hide();
                            $("#dataTable3").hide();
                            $("#dataTable4").hide();

                            var seriesName = e.point.series.name;
                            var id = e.point.x;
                            var category = e.point.category;

                            
                            var finYear = category + "_" + id;
                            
                            if (seriesName == "Cheque Amount (In Lacs)") {
                                $.ajax({
                                    url: '/MIS/ReteriveYearWiseDataForMISPayment/',
                                    type: "POST",
                                    catche: false,
                                    data: { "Year": finYear.toString(),"State":StateShortName.toString() },
                                    async: true,
                                    //beforeSend: function () {

                                    //    setTimeout(function () { blockPage(); }, -3000);
                                    //},
                                    success: function (data) {
                                        if (data.Success) {
                                            //Call to Reload Chart3
                                            objPieChartdata.length = 0;
                                            Monthstr33 = data.Monthstr3.split(',');
                                            TotalPaymentMadestr33 = data.TotalPaymentMadestr3.split(',');
                                            sumPaymentMade3 = data.sumPaymentMade3;

                                            var array = finYear.split("_");
                                            LoadChart3('<br/>Financial Year:' + array[0]);

                                            //Call to Reload Chart4
                                            $("#container4").highcharts().destroy();
                                            Monthstr44 = data.Monthstr4.split(',');
                                            ChequeAmountstr44 = data.ChequeAmountstr4.split(',');
                                            sumChqAmount4 = data.sumChqAmount4;
                                            LoadChart4('<br/>Financial Year:' + array[0]);

                                            unblockPage();
                                            //setTimeout(function () { unblockPage(); }, 1500);
                                        } else {

                                            unblockPage();
                                            alert("An Error Occur while Processing,please try Again");
                                            return false;
                                        }
                                    },
                                    error: function () {
                                        unblockPage();

                                        alert("An Error Occur while Processing,please try Again");
                                        return false;
                                    },
                                });
                            }
                        }
                    }
                }
            }
        }
    });

}

function LoadChart3(StateShortName) {

    var StateShortName1;
    var tagValue;

    if (StateShortName.includes("Financial"))
    {
        var l = StateShortName.length;
        if (l > 0) {

            var str = StateShortName.split(':');
            var splitYear = str[1];
            tagValue = '<br/>Financial Year:' + splitYear + '</b>';

        } else {
            tagValue = '';
        }
    }
    else
    {

        var l = StateShortName.length;

        if (l > 0) {
            var str = StateShortName.split(':');
            var splitStateName = str[1];
            StateShortName1 = GetStateNameFromStateCode(splitStateName);
            tagValue = '<br/>State:' + StateShortName1 + '</b>';
        } else {
            tagValue = '';
        }
    }

    var txt;
    if (StateShortName.length>0) {
        txt = 'Month wise payment quantity' + tagValue;
    } else {
        txt = 'Average Month wise payment quantity' + tagValue;
    }
    
    //alert(Monthstr33);
    //alert(TotalPaymentMadestr33);
    for (var i = 0; i < Monthstr33.length; i++) {
        var obj = {
            name: "",
            y: 0
        };
        obj.name = Monthstr33[i];
        obj.y = parseInt(TotalPaymentMadestr33[i]);
        objPieChartdata.push(obj);
    }
    $('#container3').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: true,
            type: 'pie',
            enabled: true,
            zoomType: 'xy'

        },
        title: {
            text: txt
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.y}',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total Payment Made',
            colorByPoint: true,
            data: objPieChartdata
        }]

        /*
        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',

                    onclick: function () {

                        var tableString;
                        for (var i = 0; i < 2; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Month <span></th>";
                                for (var j = 0; j < Monthstr33.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + Monthstr33[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";

                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:2%;'> <span style='color:orange;'> Total Payment Made </span></th> ";
                                for (var j = 0; j < TotalPaymentMadestr33.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + TotalPaymentMadestr33[j] + "</td>";
                                }

                                tableString += "<td style='text-align:center;width:2%;'>" + sumPaymentMade33 + "</td>";
                                tableString += "/<tr>";

                            }
                        }

                        $("#dataTable3").html(tableString);

                        tableString = "";
                        $("#dataTable3").show();


                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',
                    onclick: function () {
                        $("#dataTable3").hide();
                    }
                }
            }
        },
        */

    });
}


function LoadChart4(StateShortName) {
    var StateShortName1;
    var tagValue;

    if (StateShortName.includes("Financial")) {
        var l = StateShortName.length;
        if (l > 0) {

            var str = StateShortName.split(':');
            var splitYear = str[1];
            tagValue = '<br/>Financial Year:' + splitYear + '</b>';

        } else {
            tagValue = '';
        }
    }
    else {

        var l = StateShortName.length;

        if (l > 0) {
            var str = StateShortName.split(':');
            var splitStateName = str[1];
            StateShortName1 = GetStateNameFromStateCode(splitStateName);
            tagValue = '<br/>State:' + StateShortName1 + '</b>';
        } else {
            tagValue = '';
        }
    }





  

    var txt;
    if (StateShortName.length > 0) {
        txt = 'Month wise cheque Amount' + tagValue;
    } else {
        txt = 'Average Month wise payment Amount' + tagValue;
    }


    $('#container4').highcharts({
        chart: {
            zoomType: 'xy'
        },

        title: {
            text:txt
        },
        subtitle: {
            text: ''
        },
        xAxis: {
            // categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            categories: Monthstr44,
            title: {
                text: 'Months',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        },
        yAxis: {
            title: {
                text: 'Cheque Amount (In Lacs.)',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        },
        series: [{
            type: 'column',
            name: 'Cheque Amount (In Lacs.)',
            colorByPoint: true,
            //data: [29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4],
            data: JSON.parse("[" + ChequeAmountstr44 + "]"),
            showInLegend: false,
            tooltip: {
                valueSuffix: ' Lacs'
            },
        }]
        /*

        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',

                    onclick: function () {

                        var tableString;
                        for (var i = 0; i < 2; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:4%;'> <span style='color:orange;'> Months </span></th>";
                                for (var j = 0; j < Monthstr44.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + Monthstr44[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                                
                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:4%;'>  <span style='color:orange;'> Cheque Amount(In Lakhs.) </span></th> ";
                                for (var j = 0; j < ChequeAmountstr44.length; j++) {
                                    tableString += "<td style='text-align:center;width:2%;'>" + ChequeAmountstr44[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:2%;'>" + sumChqAmount44 + "</td>";
                                tableString += "/<tr>";

                            }
                            
                        }

                        $("#dataTable4").html(tableString);

                        tableString = "";
                        $("#dataTable4").show();


                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',
                    onclick: function () {
                        $("#dataTable4").hide();
                    }
                }
            }
        },
        */


    });
}


function LoadChartDSC() {

    $('#container5').highcharts({
        chart: {
            type: 'line',
            zoomType: 'xy'
        },
        title: {
            text: 'State Wise DSC Status'
        },
        subtitle: {
            text: ''
        },
        xAxis: {
            // categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            categories: DSCStateNamestr1
        },
        yAxis: {
            title: {
                text: 'DSC Finalized/Verified',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                },
                enableMouseTracking: false
            }
        },
        series: [{
            name: 'DSC Finalized',
            //data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
            data: JSON.parse("[" + DSCFinalizedstr1 + "]"),

        }, {
            name: 'DSC Verified',
            //data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
            data: JSON.parse("[" + DSCVerifiedstr1 + "]"),
            color: '#008000',
        }]

        /*
        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',
                    onclick: function () {
                        var tableString;
                        for (var i = 0; i < 3; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:6%;'> <span style='color:orange;'> State Name </span></th>";
                                for (var j = 0; j < DSCStateNamestr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + DSCStateNamestr1[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";
                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:6%;'>  <span style='color:orange;'>  DSC Finalized </span></th> ";
                                for (var j = 0; j < DSCFinalizedstr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + DSCFinalizedstr1[j] + "</td>";
                                }

                                tableString += "<td style='text-align:center;width:3%;'>" + sumDSCFinalized1 + "</td>";
                                tableString += "/<tr>";

                            }
                            if (i == 2) {
                                tableString += "<tr><th style='text-align:center;width:6%;'>  <span style='color:orange;'>  DSC Verified </span></th>";
                                for (var j = 0; j < DSCVerifiedstr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + DSCVerifiedstr1[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + sumDSCVerified1 + "</td>";

                                tableString += "/<tr>";
                            }
                        }

                        $("#dataTableDSC").html(tableString);

                        tableString = "";
                        $("#dataTableDSC").show();


                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',
                    onclick: function () {
                        $("#dataTableDSC").hide();
                    }
                }
            }
        },

        */



    });
}

function LoadChartBeneficiary() {

    $('#container6').highcharts({
        chart: {
            type: 'column',
            zoomType: 'xy'
        },

        title: {
            text: 'State Wise Beneficiary Status',

        },
        subtitle: {
            text: ''
        },
        xAxis: {

            categories: BeneficiaryStateNamestr1,
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Beneficiary Finalize/Verified',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },

        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
              '<td style="padding:0"><b>{point.y:.1f} </b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [{
            name: 'Total Beneficiary in OMMAS',

            data: JSON.parse("[" + BeneficiaryTotalstr1 + "]"),
            //data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4]
            pointWidth: 14

        }, {
            name: 'Beneficiary Finalized in OMMAS ',

            data: JSON.parse("[" + BeneficiaryFinalizedstr1 + "]"),
            pointWidth: 14,
            color:'rgb(144,237,125)'
            //data: [83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3]

        }, {
            name: 'Beneficiary Verified by PFMS',

            data: JSON.parse("[" + BeneficiaryVerifiedstr1 + "]"),
            color: '#ffc081',
            pointWidth: 14
            //data: [48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2]

        }
        ]

        /*
        exporting: {
            buttons: {
                customButton: {
                    text: '<b>View Data Table<b>',

                    onclick: function () {

                        var tableString;

                        for (var i = 0; i < 4; i++) {
                            if (i == 0) {
                                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'>  State Name </span></th>";
                                for (var j = 0; j < BeneficiaryStateNamestr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + BeneficiaryStateNamestr1[j] + '</b>' + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + '<b style=color:orange;>' + 'Total' + '</b>' + "</td>";

                                tableString += "/<tr>";

                            }
                            if (i == 1) {
                                tableString += "<tr><th style='text-align:center;width:8%;'>  <span style='color:orange;'> Total Beneficiary in OMMAS</span></th> ";
                                for (var j = 0; j < BeneficiaryTotalstr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryTotalstr1[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + sumTotalBeneficiary1 + "</td>";

                                tableString += "/<tr>";

                            }
                            if (i == 2) {
                                tableString += "<tr><th style='text-align:center;width:8%;'>  <span style='color:orange;'> Beneficiary Finalized in OMMAS </span></th>";
                                for (var j = 0; j < BeneficiaryFinalizedstr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryFinalizedstr1[j] + "</td>";

                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + sumBeneficiaryFinalized1 + "</td>";
                                tableString += "/<tr>";
                            }
                            if (i == 3) {
                                tableString += "<tr><th style='text-align:center;width:8%;'> <span style='color:orange;'>Beneficiary Verified by PFMS</span></th>";
                                for (var j = 0; j < BeneficiaryVerifiedstr1.length; j++) {
                                    tableString += "<td style='text-align:center;width:3%;'>" + BeneficiaryVerifiedstr1[j] + "</td>";
                                }
                                tableString += "<td style='text-align:center;width:3%;'>" + sumBeneficiaryVerified1 + "</td>";
                                tableString += "/<tr>";
                            }

                        }

                        $("#dataTableBeneficiary").html(tableString);

                        tableString = "";
                        $("#dataTableBeneficiary").show();


                    }
                },
                anotherButton: {
                    text: '<b>Close Data Table<b>',
                    onclick: function () {
                        $("#dataTableBeneficiary").hide();
                    }
                }
                
            }
        },
        */


    });
}


function BlockUI() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}

//BlockUI();
//$.unblockUI();

function ShowPaymentChart() {
    $("#dataTable1").hide();
    $("#dataTable2").hide();
    $("#dataTable3").hide();
    $("#dataTable4").hide();
    $("#dataTableDSC").hide();
    $("#dataTableBeneficiary").hide();


}

function ShowDSCBenefiicaryChart() {

    $("#dataTable1").hide();
    $("#dataTable2").hide();
    $("#dataTable3").hide();
    $("#dataTable4").hide();
    $("#dataTableDSC").hide();
    $("#dataTableBeneficiary").hide();

}


function GetStateNameFromStateCode(ShortCode) {
    var StateName;
    switch (ShortCode.trim()) {

        
        case 'AN':
            StateName= 'Andaman And Nicobar Islands';
            break;
        case 'AP':
            StateName= 'Andhra Pradesh';
            break;

        case 'AR':
            StateName = 'Arunachal Pradesh';
            break;

        case 'AS':
            StateName = 'Assam';
            break;

        case 'BR':
            StateName = 'Bihar';
            break;

        case 'CH':
            StateName = 'Chandigarh';
            break;


        case 'CG':
            StateName = 'Chhattisgarh';
            break;

        case 'DN':
            StateName = 'Dadra And Nagar Haveli';
            break;

        case 'DD':
            StateName = 'Daman And Diu';
            break;

        case 'DL':
            StateName = 'Delhi';
            break;

        case 'GA':
            StateName = 'Goa';
            break;


        case 'GJ':
            StateName = 'Gujarat';
            break;

        case 'HR':
            StateName = 'Haryana';
            break;

        case 'HP':
            StateName = 'Himachal Pradesh';
            break;

        case 'JK':
            StateName = 'Jammu And Kashmir';
            break;

        case 'JH':
            StateName = 'Jharkhand';
            break;

        case 'KN':
            StateName = 'Karnataka';
            break;

        case 'KR':
            StateName = 'Kerala';
            break;

        case 'LK':
            StateName = 'Lakshadweep';
            break;

        case 'MP':
            StateName = 'Madhya Pradesh';
            break;

        case 'MH':
            StateName = 'Maharashtra';
            break;

        case 'MN':
            StateName = 'Manipur';
            break;

        case 'MG':
            StateName = 'Meghalaya';
            break;

        case 'MZ':
            StateName = 'Mizoram';
            break;

        case 'NG':
            StateName = 'Nagaland';
            break;

        case 'OR':
            StateName = 'Odisha';
            break;

        case 'PD':
            StateName = 'Pondicherry';
            break;

        case 'PB':
            StateName = 'Punjab';
            break;

        case 'RJ':
            StateName = 'Rajasthan';
            break;


        case 'SK':
            StateName = 'Sikkim';
            break;

        case 'TN':
            StateName = 'Tamilnadu';
            break;

        case 'TS':
            StateName = 'Telangana';
            break;

        case 'TR':
            StateName = 'Tripura';
            break;

        case 'UP':
            StateName = 'Uttar Pradesh';
            break;

        case 'UT':
            StateName = 'Uttarakhand';
            break;

        case 'WB':
            StateName = 'West Bengal';
            break;

    }
    return StateName;
    
    
        
            
}