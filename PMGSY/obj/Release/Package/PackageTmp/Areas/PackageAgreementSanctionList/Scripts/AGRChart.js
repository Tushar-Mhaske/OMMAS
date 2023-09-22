var worksColumnChart = null;
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));
    $("#StateList_StateListRoadDetails").change(function () {
        loadDistrict($("#StateList_StateListRoadDetails").val());

    });

    $("#DistrictList_StateListRoadDetails").change(function () {
        loadBlock($("#StateList_StateListRoadDetails").val(), $("#DistrictList_StateListRoadDetails").val());

    });
    $("#btnViewStateListRoad").click(function () {
 //.  alert("Abc")
        // Added on 06 Sept 2019
        chartWorksColumnChart(worksColumnChart, "divWorksChartContainer");
    });




    closableNoteDiv("divStateListRoad", "spnStateListRoad");


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmStateListRoadLayout").toggle("slow");

    });
});

//State Change Fill District DropDown List
function loadDistrict(statCode) {
    $("#DistrictList_StateListRoadDetails").val(0);
    $("#DistrictList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").val(0);
    $("#BlockList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_StateListRoadDetails').find("option[value='0']").remove();
                    //$("#DistrictList_StateListRoadDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_StateListRoadDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_StateListRoadDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_StateListRoadDetails").attr("disabled", "disabled");
                        $("#DistrictList_StateListRoadDetails").trigger('change');
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#DistrictList_StateListRoadDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_StateListRoadDetails").empty();
        $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_StateListRoadDetails").val(0);
    $("#BlockList_StateListRoadDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_StateListRoadDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_StateListRoadDetails").attr("disabled", "disabled");
                        //$("#BlockList_StateListRoadDetails").trigger('change');
                    }
     


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");
    }
}









//Level 0 Declare Series
function chartWorksColumnChart(chart, containerID) {

    $.blockUI({ message: null });

    $.ajax({
        type: "POST",
        url: '/PackageAgreementSanctionList/PackageAgreement/ChartAGR/',
        data: { LevelCode: 0, StateCode: $("#StateList_StateListRoadDetails option:selected").val(), DistrictCode: $("#DistrictList_StateListRoadDetails option:selected").val(), Pmgsy: 0, WorkType: '%' },


        error: function (xhr, status, error) {
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null; var series2 = null; var series3 = null; var series4 = null;

            

            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                series1 = {
                    data: []
                };
                series2 = {
                    data: []
                };
                series3 = {
                    data: []
                };
                series4 = {
                    data: []
                };

                cat = [];
                $.each(data, function (item) {

                    console.log(data);

                    cat.push(data[item].Year);

                    series1.name = "Total Sactioned Works";
                    series1.type = "column";
                    //series1.data.push({ x: this.Year, y: parseFloat(this.SanctionWorks) });
                    series1.data.push(parseFloat(this.SanctionWorks));

                    series2.name = "Total Works Awarded";//
                    series2.type = "column";
                    series2.data.push(parseFloat(this.AwardedWorks));
                    // series2.data.push({ x: this.Year, y: parseFloat(this.ProgWorks) });

                    series3.name = "Total Unawarded Works";
                    series3.type = "column";
                    series3.data.push(parseFloat(this.UnawardedWorks));
                    //series3.data.push({ x: this.Year, y: parseFloat(this.CompletedWorks) });

                    series4.name = " Works Awarded & Terminated";
                    series4.type = "column";
                    series4.data.push(parseFloat(this.TerminatedWorks));

                });



                optionsColumn = CommonOptionsForWorksColumnChart(containerID);
                optionsColumn.series.push(series1);
                optionsColumn.series.push(series2);
                optionsColumn.series.push(series3);
                optionsColumn.series.push(series4);

                chart = new Highcharts.Chart(optionsColumn);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
                chart.series[3].setVisible(true, true);

                if (cat.splice(cat.length - 1, 1))
                    cat.push("Total")
                chart.xAxis[0].setCategories(cat);

                // console.log(cat)

                //chart.series[3].setVisible(true, true);
            }
            $.unblockUI();
            $('#' + containerID).addClass('div-border');
        }
    });
}


//Level 0 Plot Series 
function CommonOptionsForWorksColumnChart(containerDivID) {

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                height: 2000,
                width:1500,
                renderTo: containerDivID
            },
        title: {
            text: 'Works',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {
           
            gridLineWidth: 1,
            categories: {}

        },
        yAxis: [{
            title: {
                text: 'No. of Works'
            },
         //   tickInterval: 500,
            min: 0,
            labels: {
                format: '{value}',
                style: {
                    color: '#4572A7'
                }
            }
        }],
        tooltip:
            {
                headerFormat: '<span style="font-size:11px"> Year: All Years </span>' +
                              '<table>',

                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
        series: [],
        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248'],

      
        //Level 1 Declare Series
        plotOptions: {
            series: {
                point: {
                    events: {
                        click: function (e) {

                            
                            var seriesName = e.point.series.name;
                         //   alert(seriesName);
                            var id = e.point.x;
                            //alert(id);
                            var category = e.point.category;


                            var worksColumnChart = null;
                            var containerID1 = "divWorksChartContainer";

                            $.ajax({
                                type: "POST",
                                url: '/PackageAgreementSanctionList/PackageAgreement/ChartAGR/',
                                data: { LevelCode: 1, StateCode: $("#StateList_StateListRoadDetails option:selected").val(), DistrictCode: $("#DistrictList_StateListRoadDetails option:selected").val(), Pmgsy: 0, WorkType: '%' },

                                error: function (xhr, status, error) {
                                    $.unblockUI();
                                    $('#errorSpan').text(xhr.responseText);
                                    $('#divError').show('slow');
                                    return false;
                                },
                                success: function (data) {

                                    var series2 = null; var series3 = null; //var series4 = null; //  var series1 = null;

                                    if (data == "") {
                                        if ($('#' + containerID1).highcharts()) {
                                            $('#' + containerID1).highcharts().destroy();
                                        }
                                    }
                                    else {

                                 
                                        series2 = {
                                            data: []
                                        };
                                        series3 = {
                                            data: []
                                        };
                                   

                                        cat = [];
                                        $.each(data, function (item) {

                                            console.log(data);

                                            cat.push(data[item].stateName);


                                            series2.name = "Completed Works";//
                                            series2.type = "column";
                                            series2.data.push(parseFloat(this.CompletedWorks));

                                            series3.name = "Progress Works";
                                            series3.type = "column";
                                            series3.data.push(parseFloat(this.ProgressWorks));

                                        });

                                        optionsColumn = CommonOptionsForWorksColumnChart1(containerID1);
                                        optionsColumn.series.push(series2);
                                        optionsColumn.series.push(series3);
                                  


                                        chart = new Highcharts.Chart(optionsColumn);
                                        // code to display animation
                                        chart.series[0].setVisible(true, true);
                                        chart.series[1].setVisible(true, true);
                                     
                                      //  chart.xAxis[0].setCategories(cat);


                                        if (cat.splice(cat.length - 1, 1))
                                            cat.push("")
                                        chart.xAxis[0].setCategories(cat);


                                    }

                                    //  $.unblockUI();

                                    $('#' + containerID1).addClass('div-border');
                                }
                            });

                        }
                    }
                }
            }
        }








    };

    return options;
}


//Level 1 Plot Series
function CommonOptionsForWorksColumnChart1(containerDivID) {
   // alert("newly called")
    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID
            },
        title: {
            text: 'Sanctioned Works',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {

            gridLineWidth: 1,
            categories: {}

        },
        yAxis: [{
            title: {
                text: 'No. of Works'
            },
            min: 0,
           // tickInterval: 500,
            labels: {
                format: '{value}',
                style: {
                    color: '#4572A7'
                }
            }
        }],
        tooltip:
            {
                headerFormat: '<span style="font-size:11px"> Year: All Years </span>' +
                              '<table>',

                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
        series: [],
        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248'],

     
        //Level 2 Declare Series
        plotOptions: {
            series: {
                point: {
                    events: {
                        click: function (e) {


                            var seriesName = e.point.series.name;
                         //   alert(seriesName);
                            var id = e.point.x;
                            //alert(id);
                            var category = e.point.category;


                            var worksColumnChart = null;
                            var containerID2 = "divWorksChartContainer";

             
                            //
                            $.ajax({
                                type: "POST",
                                url: '/PackageAgreementSanctionList/PackageAgreement/ChartAGR/',
                                data: { LevelCode: 2, StateCode: $("#StateList_StateListRoadDetails option:selected").val(), DistrictCode: $("#DistrictList_StateListRoadDetails option:selected").val(), Pmgsy: 0, WorkType: '%' },


                                error: function (xhr, status, error) {
                                    $.unblockUI();
                                    $('#errorSpan').text(xhr.responseText);
                                    $('#divError').show('slow');
                                    return false;
                                },
                                success: function (data) {
                                    var series11 = null; var series12 = null; var series13 = null; var series14 = null;



                                    if (data == "") {
                                        if ($('#' + containerID2).highcharts()) {
                                            $('#' + containerID2).highcharts().destroy();
                                        }
                                    }
                                    else {

                                        series11 = {
                                            data: []
                                        };
                                        series12 = {
                                            data: []
                                        };
                                        series13 = {
                                            data: []
                                        };
                                        series14 = {
                                            data: []
                                        };

                                        cat = [];
                                        $.each(data, function (item) {

                                            console.log(data);

                                            cat.push(data[item].Year);

                                            series11.name = "Less Than Year 1";
                                            series11.type = "column";
                                            series11.data.push(parseFloat(this.LessThanYear_1));

                                            series12.name = "Between Year 1 and 2";//
                                            series12.type = "column";
                                            series12.data.push(parseFloat(this.Year_1_To_2));
                                           

                                            series13.name = "Between Year 2 and 3";
                                            series13.type = "column";
                                            series13.data.push(parseFloat(this.Year_2_To_3));
                                           

                                            series14.name = "Greater Than Year 3";
                                            series14.type = "column";
                                            series14.data.push(parseFloat(this.GreaterThanYear_3));

                                        });



                                        optionsColumn = CommonOptionsForWorksColumnChart2(containerID2);
                                        optionsColumn.series.push(series11);
                                        optionsColumn.series.push(series12);
                                        optionsColumn.series.push(series13);
                                        optionsColumn.series.push(series14);

                                        chart = new Highcharts.Chart(optionsColumn);
                                        // code to display animation
                                        chart.series[0].setVisible(true, true);
                                        chart.series[1].setVisible(true, true);
                                        chart.series[2].setVisible(true, true);
                                        chart.series[3].setVisible(true, true);

                                        if (cat.splice(cat.length - 1, 1))
                                            cat.push("")
                                        chart.xAxis[0].setCategories(cat);

                                        // console.log(cat)

                                        //chart.series[3].setVisible(true, true);
                                    }
                                    $.unblockUI();
                                    $('#' + containerID2).addClass('div-border');
                                }
                            });
                            //




                        }
                    }
                }
            }
        }








    };

    return options;
}


//Level 2 Plot Series
function CommonOptionsForWorksColumnChart2(containerDivID) {
    //alert("Level 2 Plot Series")
    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID
            },
        title: {
            text: 'Works',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {

            gridLineWidth: 1,
            categories: {}

        },
        yAxis: [{
            title: {
                text: 'No. of Works'
            },
            min: 0,
            // tickInterval: 500,
            labels: {
                format: '{value}',
                style: {
                    color: '#4572A7'
                }
            }
        }],
        tooltip:
            {
                headerFormat: '<span style="font-size:11px">  </span>' + //Year: All Years
                              '<table>',

                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
        series: [],
        colors: ['#00526F', '#594266', '#cb6828', '#DC143C', '#B2C248', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248'],

        // CALL Initail Series Level 1 is calling

        plotOptions: {
            series: {
                point: {
                    events: {
                        click: function (e) {


                            var seriesName = e.point.series.name;
                            //     alert(seriesName);
                            var id = e.point.x;
                            //alert(id);
                            var category = e.point.category;


                            var worksColumnChart = null;
                            var containerID1 = "divWorksChartContainer";


                            //
                            $.ajax({
                                type: "POST",
                                url: '/PackageAgreementSanctionList/PackageAgreement/ChartAGR/',
                                data: { LevelCode: 0, StateCode: $("#StateList_StateListRoadDetails option:selected").val(), DistrictCode: $("#DistrictList_StateListRoadDetails option:selected").val(), Pmgsy: 0, WorkType: '%' },


                                error: function (xhr, status, error) {
                                    $.unblockUI();
                                    $('#errorSpan').text(xhr.responseText);
                                    $('#divError').show('slow');
                                    return false;
                                },
                                success: function (data) {
                                    var series1 = null; var series2 = null; var series3 = null; var series4 = null;



                                    if (data == "") {
                                        if ($('#' + containerID1).highcharts()) {
                                            $('#' + containerID1).highcharts().destroy();
                                        }
                                    }
                                    else {

                                        series1 = {
                                            data: []
                                        };
                                        series2 = {
                                            data: []
                                        };
                                        series3 = {
                                            data: []
                                        };
                                        series4 = {
                                            data: []
                                        };

                                        cat = [];
                                        $.each(data, function (item) {

                                            console.log(data);

                                            cat.push(data[item].Year);

                                            series1.name = "Total Sactioned Works";
                                            series1.type = "column";
                                            //series1.data.push({ x: this.Year, y: parseFloat(this.SanctionWorks) });
                                            series1.data.push(parseFloat(this.SanctionWorks));

                                            series2.name = "Total Works Awarded";//
                                            series2.type = "column";
                                            series2.data.push(parseFloat(this.AwardedWorks));
                                            // series2.data.push({ x: this.Year, y: parseFloat(this.ProgWorks) });

                                            series3.name = "Total Unawarded Works";
                                            series3.type = "column";
                                            series3.data.push(parseFloat(this.UnawardedWorks));
                                            //series3.data.push({ x: this.Year, y: parseFloat(this.CompletedWorks) });

                                            series4.name = " Works Awarded & Terminated";
                                            series4.type = "column";
                                            series4.data.push(parseFloat(this.TerminatedWorks));

                                        });



                                        optionsColumn = CommonOptionsForWorksColumnChart(containerID1);
                                        optionsColumn.series.push(series1);
                                        optionsColumn.series.push(series2);
                                        optionsColumn.series.push(series3);
                                        optionsColumn.series.push(series4);

                                        chart = new Highcharts.Chart(optionsColumn);
                                        // code to display animation
                                        chart.series[0].setVisible(true, true);
                                        chart.series[1].setVisible(true, true);
                                        chart.series[2].setVisible(true, true);
                                        chart.series[3].setVisible(true, true);

                                        if (cat.splice(cat.length - 1, 1))
                                            cat.push("")
                                        chart.xAxis[0].setCategories(cat);

                                        // console.log(cat)

                                        //chart.series[3].setVisible(true, true);
                                    }
                                    $.unblockUI();
                                    $('#' + containerID1).addClass('div-border');
                                }
                            });
                            //




                        }
                    }
                }
            }
        }








    };

    return options;
}