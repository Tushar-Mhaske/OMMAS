var colourArray = [];
var roadWidth = 0;
$(document).ready(function () {


    $('#btnPCIAbstractAnalyisDetails').click(function () {
        $("#dvPropPCIChart").show();
        var routeType = $('#ddRoute_PCIAbstractAnalyisDetails').val();
        var flag;
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#hdnLevelId").val() == 6) //mord
        {
            flag = 'S';
            PCIAnalysisStateReportGrid(0, "", 0, "", 0, flag, routeType);

            GetPCIChart(0, "", 0, 0, "", 0, flag, routeType, "dvPropPCIChart");

        }
        else if ($("#hdnLevelId").val() == 4) //state
        {

            flag = 'D';
            PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", 0, flag, routeType);


            GetPCIChart($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, 0, "", 0, flag, routeType, "dvPropPCIChart");

        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            flag = 'B';
            PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType);
            GetPCIChart($("#MAST_STATE_CODE").val(), "", 0, $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType, "dvPropPCIChart")

        }
        $.unblockUI();
    });

    $('#btnPCIAbstractAnalyisDetails').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
    // $('#btnPCIAbstractAnalyisDetails').trigger('click');

    LoadGrid();
});

function LoadGrid() {

    var routeType = $('#ddRoute_PCIAbstractAnalyisDetails').val();
    var flag;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#hdnLevelId").val() == 6) //mord
    {
        flag = 'S';
        PCIAnalysisStateReportGrid(0, "", 0, "", 0, flag, routeType);

        // GetPCIChart(0, "", 0, 0, "", 0, flag, routeType, "dvPCIChart");

    }
    else if ($("#hdnLevelId").val() == 4) //state
    {

        flag = 'D';
        PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", 0, flag, routeType);


        //GetPCIChart($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, 0, "", 0, flag, routeType, "dvPCIChart");

    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        flag = 'B';
        PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType);
        // GetPCIChart($("#MAST_STATE_CODE").val(), "", 0, $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType, "dvPCIChart")

    }
    $.unblockUI();
}
function LoadChart() {
    $("#dvPropPCIChart").show();
    var routeType = $('#ddRoute_PCIAbstractAnalyisDetails').val();
    var flag;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#hdnLevelId").val() == 6) //mord
    {
        flag = 'S';
        // PCIAnalysisStateReportGrid(0, "", 0, "", 0, flag, routeType);

        GetPCIChart(0, "", 0, 0, "", 0, flag, routeType, "dvPropPCIChart");

    }
    else if ($("#hdnLevelId").val() == 4) //state
    {

        flag = 'D';
        //PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", 0, flag, routeType);


        GetPCIChart($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, 0, "", 0, flag, routeType, "dvPropPCIChart");

    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        flag = 'B';
        //PCIAnalysisStateReportGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType);
        GetPCIChart($("#MAST_STATE_CODE").val(), "", 0, $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, flag, routeType, "dvPropPCIChart")

    }
    $.unblockUI();
}
/*       STATE REPORT LISTING       */
function PCIAnalysisStateReportGrid(stateCode, stateName, districtCode, districtName, blockCode, flag, routeType) {
    var lblhead;
    if (stateCode > 0 && districtCode == 0) {
        lblhead = "District Name";
    }
    else if (stateCode > 0 && districtCode > 0) {
        lblhead = "Block Name";
    }
    else {
        lblhead = "State Name";
    }
    var dateVar = new Date();
    var month = dateVar.getMonth();
    var year = dateVar.getFullYear();

    if (month > 3) {
        year = year + 1;
    }
    var firstFinYear = parseInt(year - 3) + "-" + parseInt(year - 2);
    var secondFinYear = parseInt(year - 2) + "-" + parseInt(year - 1);
    var thirdFinYear = parseInt(year - 1) + "-" + parseInt(year);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPCIPropAnalysisReport").jqGrid('GridUnload');
    $("#tbPCIPropAnalysisReport").jqGrid({
        url: '/ProposalSSRSReports/ProposalSSRSReports/PCIProposalAnalysisListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: [lblhead, 'Number of Rural Route', 'Total Length (in KM)', 'Number of Roads', 'Total Length (in KM)', '1', '2', '3', '4', '5', firstFinYear, secondFinYear, thirdFinYear],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 180, align: 'left', height: 'auto', sortable: true },
            { name: 'TOTAL_CN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN4', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LEN5', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY1', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY2', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_PCI_LY3', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        postData: { "Flag": flag, "RouteType": routeType, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode },
        pager: $("#dvPCIPropAnalysisReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '200',
        viewrecords: true,
        caption: districtName == '' ? (stateName == '' ? "PCI-Abstract Details for All State " + stateName : "PCI-Abstract Details for  " + stateName) : ' PCI-Abstract Details for  ' + districtName,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_CN_T = $(this).jqGrid('getCol', 'TOTAL_CN', false, 'sum');
            TOTAL_CN_T = parseFloat(TOTAL_CN_T).toFixed(3);
            var TOTAL_LEN_T = $(this).jqGrid('getCol', 'TOTAL_LEN', false, 'sum');
            TOTAL_LEN_T = parseFloat(TOTAL_LEN_T).toFixed(3);
            var TOTAL_PCI_T = $(this).jqGrid('getCol', 'TOTAL_PCI', false, 'sum');
            TOTAL_PCI_T = parseFloat(TOTAL_PCI_T).toFixed(3);
            var TOTAL_PCI_LEN_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN', false, 'sum');
            TOTAL_PCI_LEN_T = parseFloat(TOTAL_PCI_LEN_T).toFixed(3);
            var TOTAL_PCI_LEN1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN1', false, 'sum');
            TOTAL_PCI_LEN1_T = parseFloat(TOTAL_PCI_LEN1_T).toFixed(3);
            var TOTAL_PCI_LEN2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN2', false, 'sum');
            TOTAL_PCI_LEN2_T = parseFloat(TOTAL_PCI_LEN2_T).toFixed(3);
            var TOTAL_PCI_LEN3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN3', false, 'sum');
            TOTAL_PCI_LEN3_T = parseFloat(TOTAL_PCI_LEN3_T).toFixed(3);
            var TOTAL_PCI_LEN4_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN4', false, 'sum');
            TOTAL_PCI_LEN4_T = parseFloat(TOTAL_PCI_LEN4_T).toFixed(3);
            var TOTAL_PCI_LEN5_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LEN5', false, 'sum');
            TOTAL_PCI_LEN5_T = parseFloat(TOTAL_PCI_LEN5_T).toFixed(3);
            var TOTAL_PCI_LY1_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY1', false, 'sum');
            TOTAL_PCI_LY1_T = parseFloat(TOTAL_PCI_LY1_T).toFixed(3);
            var TOTAL_PCI_LY2_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY2', false, 'sum');
            TOTAL_PCI_LY2_T = parseFloat(TOTAL_PCI_LY2_T).toFixed(3);
            var TOTAL_PCI_LY3_T = $(this).jqGrid('getCol', 'TOTAL_PCI_LY3', false, 'sum');
            TOTAL_PCI_LY3_T = parseFloat(TOTAL_PCI_LY3_T).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_CN: TOTAL_CN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LEN: TOTAL_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI: TOTAL_PCI_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN: TOTAL_PCI_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN1: TOTAL_PCI_LEN1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN2: TOTAL_PCI_LEN2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN3: TOTAL_PCI_LEN3_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN4: TOTAL_PCI_LEN4_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LEN5: TOTAL_PCI_LEN5_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY1: TOTAL_PCI_LY1_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY2: TOTAL_PCI_LY2_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_PCI_LY3: TOTAL_PCI_LY3_T }, true);
            //Set Span Text
            if ($('#spanChartFor').html("")) {
                if ($("#hdnLevelId").val() == 6) //mord
                {
                    $('#spanChartFor').html("All States" + "    [" + TOTAL_PCI_LEN_T + "Kms.]");
                }
                else if ($("#hdnLevelId").val() == 4)//state
                {
                    $('#spanChartFor').html($("#STATE_NAME").val() + "  [" + TOTAL_PCI_LEN_T + "Kms.]");

                }
                else if ($("#hdnLevelId").val() == 5)//district
                {
                    $('#spanChartFor').html($("#DISTRICT_NAME").val() + "   [" + TOTAL_PCI_LEN_T + "Kms.]");

                }
            }
            $('#tbPCIAbstractAnalysisReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            //var total = $(this).jqGrid('footerData', 'get', TOTAL_CN, true);
            //alert(total);
            var params = id.split('$');
            var flagstype;
            roadWidth = parseFloat(params[2]).toFixed(3);
            if ($("#hdnLevelId").val() == 6) //mord
            {
                // flags = 'S';
                flagstype = 'D';
                GetPCIChart(params[0], params[1], roadWidth, districtCode, districtName, blockCode, flagstype, routeType, "dvPropPCIChart");
                $('#spanChartFor').html(params[1] + "  [" + roadWidth + "Kms.]");

            }
            else if ($("#hdnLevelId").val() == 4)//state
            {
                //flags = 'D';
                flagstype = 'B';
                GetPCIChart(stateCode, '', roadWidth, params[0], params[1], blockCode, flagstype, routeType, "dvPropPCIChart");
                $('#spanChartFor').html(params[1] + "   [" + roadWidth + "Kms.]");

            }
            else if ($("#hdnLevelId").val() == 5)//district
            {
                //flagstype = 'B';
                //GetPCIChart(stateCode, '', roadWidth, districtCode, districtName, params[0], flagstype, routeType, "dvPCIChart");
                //$('#spanChartFor').html(params[1] + "   [" + roadWidth + "Kms.]");

            }

        },

        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });

    $("#tbPCIPropAnalysisReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TOTAL_PCI', numberOfColumns: 2, titleText: '<em>PCI Data Entered For </em>' },
          { startColumnName: 'TOTAL_PCI_LEN1', numberOfColumns: 5, titleText: '<em>Length (in KM) with PCI Value </em>' },
          { startColumnName: 'TOTAL_PCI_LY1', numberOfColumns: 3, titleText: '<em>Year Wise Length (in KM) </em>' }

        ]
    });

}


//function to get the asset liability chart
function GetPCIChart(stateCode, stateName, rodeWidth, districtCode, districtName, blockCode, flag, routeType, ContainerDivID) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: "POST",
        url: '/ProposalSSRSReports/ProposalSSRSReports/PCIProposalAnalysisChartListing/',
        data:
            {
                'StateCode': stateCode,
                'Flag': flag,
                'RouteType': routeType,
                "DistrictCode": districtCode,
                "BlockCode": blockCode,

            },
        error: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        },
        success: function (data) {
            var series1 = null;

            if (data == "") {
                //  alert('in empty');
                if ($('#' + ContainerDivID).highcharts()) {
                    $('#' + ContainerDivID).highcharts().destroy();
                }

            }

            else {


                series1 = {
                    data: [], startAngle: 30,
                    point: {
                        events: {

                        }
                    },
                };

                $.each(data, function (item) {

                    //alert(this.y);
                    var len = parseFloat(this.y).toFixed(2)
                    var per = parseFloat(this.x.split(':')[1]).toFixed(2);
                    var charTOAppend = this.x.split(':')[0];

                    // required  when details chart is to be completed
                    //    series1.data.push({ x: this.x, y: parseFloat(len), z: parseFloat(this.z), id: this.id, fundType: this.fundType, AssetOrLia: this.AssetOrLia, headCode: this.headCode });
                    series1.data.push({ x: charTOAppend + " :" + per + "%", y: parseFloat(len), z: parseFloat(this.z) });


                });

                optionsPie = CommonOptions(ContainerDivID)
                optionsPie.series.push(series1);

                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.legend.group.hide();
                //chart.legend.box.hide();

            }

            $.unblockUI();
        }

    });


}
function CommonOptions(ContainerDivID) {


    if ($('#' + ContainerDivID).highcharts()) {
        $('#' + ContainerDivID).highcharts().destroy();
    }

    switch (ContainerDivID) {
        case "dvPropPCIChart": colourArray = ['#DF0101', '#FE2E2E', '#FF9900', '#80E675', '#16BA04', '#86A033', '#614931', '#981A37']; break;
            //Taken From orignal'#DF0101', '#FE2E2E', '#FF9900', '#80E675', '#16BA04','#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab','#a89375'] //Taken From original ,
            // case "dvPCIChart": colourArray = ['#5485BC', '#FCB319', '#20B2AA', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37']; break;
            //    case "LiaChart": colourArray = ['#20B2AA', '#5485BC', '#AA8C30', '#cb6828', '#aaaaab', '#981A37']; break;
            //    case "adminAssetChart": colourArray = ['#CDAD00', '#00526F', '#594266', '#9ACD32', '#cb6828', '#1E90FF', '#aaaaab', '#a89375', '#9c1c6b', '#1E90FF']; break;
            //    case "adminLiaChart": colourArray = ['#5485BC', '#FCB319', '#5C9384', '#981A37', '#5485BC', '#CDCD00', '#AA8C30', '#e47297', '#7CCD7C']; break;
            //    case "mainAssetChart": colourArray = ['#594266', '#cb6828', '#89158', '#cb6828', '#aaaaab', '#a89375']; break;
            //    case "mainLiaChart": colourArray = ['#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#9c1c6b']; break;

    }
    // colourArray = ['#5485BC', '#FCB319', '#20B2AA', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37'];
    var optionsPie =
        {
            chart: {
                type: 'pie',
                renderTo: ContainerDivID,
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                margin: [20, 20, 30, 20],
                spacingTop: 20,
                spacingBottom: 20,
                spacingLeft: 20,
                spacingRight: 20
            },
            credits: {
                enabled: false
            },

            title: {
                text: 'Title'
            },
            credits: {
                enabled: false
            },
            // colors: ['#5C9384', '#981A37', '#5485BC', '#AA8C30', '#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375'],
            title: {
                text: null
            },
            tooltip: {
                //enabled: true,
                // animation: true,
                formatter: function () {
                    return ' ' +
                                  this.point.x + '<br />' +
                                  'Length: ' + this.point.y + "Kms." + '<br />' +
                                  'PCI:' + this.point.z;
                },
                // categories: ['Poor', 'Very Poor', 'Good', 'Fair', 'Very Good'],
                // pointFormat: '{series.name}: <b>{point.percentage}%</b>',
                percentageDecimals: 2

            },
            plotOptions: {
                pie: {
                    size: "90%",
                    allowPointSelect: true,
                    series: {
                        animation: {
                            duration: 1000
                        }
                    },

                    animation: true,
                    cursor: 'pointer',
                    showInLegend: true,
                    dataLabels: {
                        enabled: true,

                        style: {
                            width: '150px'
                        },
                        color: '#000000',
                        connectorColor: '#000000',
                        formatter: function () {
                            return '<b>' + this.point.x + '</b>' + '<br/>' + 'PCI :' + this.point.z;
                        }
                    }
                }
            },
            legend: {
                enabled: true,
                layout: 'vertical',
                align: 'left',
                width: 220,
                verticalAlign: 'middle',
                borderWidth: 0,
                useHTML: true,
                //labelFormatter: function () {
                //    return '<div style="width:200px"><span style="float:left">' + this.x + '</span><span style="float:left"> Rs.' + this.z + ' Lacs </span></div>';
                //},
                title: {
                    text: '',
                    style: {
                        fontWeight: 'bold'
                    }
                }
            },
            series: [
                {
                    type: 'pie',
                    data: []
                }
            ]
        }



    optionsPie.colors = Highcharts.map(colourArray, function (color) {
        return {
            radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
            stops: [
                [0, color],
                [1, Highcharts.Color(color).brighten(-0.1).get('rgb')] // darken
            ]
        };
    });




    return optionsPie;
}