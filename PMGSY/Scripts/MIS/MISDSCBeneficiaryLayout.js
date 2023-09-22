
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

    $("#tblDSC").show();
    $("#tblBeneficiary").show();

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
    LoadChartDSC();
    LoadChartBeneficiary();




});

function LoadChartDSC() {

    $('#container5').highcharts({
        chart: {
            type: 'line'
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
                text: 'Dsc Finalized/Verified',
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
            name: 'Dsc Finalized',
            //data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
            data: JSON.parse("[" + DSCFinalizedstr1 + "]"),

        }, {
            name: 'Dsc Verified',
            //data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
            data: JSON.parse("[" + DSCVerifiedstr1 + "]"),
            color: '#008000',
        }],

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
                }
            }
        },



    });
}

function LoadChartBeneficiary() {

    $('#container6').highcharts({
        chart: {
            type: 'column'
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
            pointWidth: 12

        }, {
            name: 'Beneficiary Finalized in OMMAS ',

            data: JSON.parse("[" + BeneficiaryFinalizedstr1 + "]"),
            pointWidth: 12
            //data: [83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3]

        }, {
            name: 'Beneficiary Verified by PFMS',

            data: JSON.parse("[" + BeneficiaryVerifiedstr1 + "]"),
            color: '#ffc081',
            pointWidth: 12
            //data: [48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2]

        }
        ],


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
                }
            }
        },


    });
}


