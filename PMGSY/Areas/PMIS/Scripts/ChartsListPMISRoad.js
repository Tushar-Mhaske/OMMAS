var today = new Date(),
    day = 1000 * 60 * 60 * 24;// 1 day
// month=1000 * 60 * 60 * 24 // 1 month

// Set to 00:00:00:000 today
today.setUTCHours(0);
today.setUTCMinutes(0);
today.setUTCSeconds(0);
today.setUTCMilliseconds(0);


$(document).ready(function () {
    LoadPMISRoadList();


    //  DrawGanttChart();


    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $("#ddlDistrict").change(function () {
        $("#ddlBlock").empty();

        $.ajax({
            url: '/PMIS/PMIS/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $("#btnRoadList").click(function () {
        if ((parseInt($("#ddlState option:selected").val()) > 0) && (parseInt($("#ddlDistrict option:selected").val()) > 0)) {
            LoadPMISRoadList();
        }


    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

});

//function FormatPlanColumn(cellvalue, options, rowObject) {

//    if (cellvalue.endsWith("$")) {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick ='AddProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else if (cellvalue.endsWith("&")) {
//        "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick ='EditProjectPlan(\"" + cellvalue.toString() + "\");' ></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }


//}

//function FormatActualsColumn(cellvalue, options, rowObject) {

//    if (cellvalue != '') {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick ='AddActuals(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border:none;'><span>-</span></td></tr></table></center>";
//    }

//}

function LoadPMISRoadList() {

    jQuery("#tbPMISRoadList").jqGrid('GridUnload');

    jQuery("#tbPMISRoadList").jqGrid({
        url: '/PMIS/PMIS/PMISRoadListCharts',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'District', 'Block', 'Package Name', 'Sanction Year', 'Sanction Date', 'Batch', 'Length', 'Agreement Number', 'Agreement Cost', 'MoRD Share', 'State Share', 'Total Sanctioned Cost', 'Road Name', 'View', 'Add/Update Project Plan', 'Finalize', 'Revise Plan Details', 'Add/Update Actuals', 'Add Chainage wise Details'], //, 'Revise Plan Details' 
        colModel: [

                         { name: 'STATE', index: 'STATE', width: 100, align: "left" },
                         { name: 'DISTRICT', index: 'DISTRICT', width: 100, align: "left" },
                         { name: 'BLOCK', index: 'BLOCK', width: 100, align: "left" },
                         { name: 'PACKAGE_NAME', index: 'PACKAGE_NAME', width: 120, align: "left" },
                         { name: 'SANCTION_YEAR', index: 'SANCTION_YEAR', width: 70, align: "left" },
                         { name: 'SANCTION_DATE', index: 'SANCTION_Date', width: 70, align: "left" },
                         { name: 'BATCH', index: 'BATCH', width: 70, align: "left" },
                         { name: 'LENGHT', index: 'LENGHT', width: 60, align: "left" },
                         { name: 'AGREEMENT_NUMBER', index: 'AGREEMENT_NUMBER', width: 100, align: "left" },
                         { name: 'AGREEMENT_COST', index: 'AGREEMENT_COST', width: 80, align: "left" },
                         { name: 'MoRD_SHARE', index: 'MoRD_SHARE', width: 80, align: "left" },
                         { name: 'STATE_SHARE', index: 'STATE_SHARE', width: 80, align: "left" },
                         { name: 'TOTAL_SANCTIONED_COST', index: 'TOTAL_SANCTIONED_COST', width: 80, align: "left" },
                         { name: 'ROAD_NAME', index: 'ROAD_NAME', width: 120, align: "left" },

                         { name: 'view', index: 'view', width: 120, align: "left" },





                         { name: 'PROJECT_PLAN', width: 120, resize: false, align: "center", hidden: true }, //formatter: FormatPlanColumn,
                         { name: 'FINALIZE', index: 'FINALIZE', width: 120, align: "center", hidden: true },
                         { name: 'REVISE_PLAN', index: 'REVISE_PLAN', width: 120, align: "center", hidden: true },
                         { name: 'ACTUALS', width: 120, resize: false, align: "center", hidden: true },
                         { name: 'CHAINAGE', width: 120, resize: false, align: "center", hidden: true }// formatter: FormatActualsColumn,
        ],
        postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val() },
        pager: jQuery('#dvPMISRoadListPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PMIS Road List for Report",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#tbPMISRoadList #dvPMISRoadListPager").css({ height: '31px' });
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!");

            }
        }

    }); //end of grid



}


// ViewChart

function ViewChart(urlparameter) {



    $.ajax({
        url: '/PMIS/GetDataForGanttChart/' + urlparameter,
        type: "POST",
        // data: { 'RoadCode': 231053, 'BaseLine': 1 },
        success: function (response) {
            //  alert("console.log(Highcharts.chart) =="+Highcharts.chart);
            //     console.log("***** console.log(Highcharts.chart) ******");
            // console.log(Highcharts.chart)

            //      alert("ajda.ListForChart ==> " + response.ListForChart.length);
            //   console.log(response);
            DrawGanttChart(response, response.ListForChart[0].State, response.ListForChart[0].District, response.ListForChart[0].BlockName, response.ListForChart[0].PiuName, response.ListForChart[0].RoadName, response.ListForChart[0].PackageNo, response.ListForChart[0].SanctionedLength, response.ListForChart[0].SanctionYear, response.ListForChart[0].SanctionDate, response.ListForChart[0].ImsBatch, response.ListForChart[0].AgreementCost);

            $.unblockUI();
        },
        error: function (xhr, status, err) {
            $.unblockUI();
            alert("Error occured while proccessing your request : " + err);
        }
    });
}



function DrawGanttChart(response, state, district, blockName, piuName, roadName, packageNo, length, year, date, batch, agreeCost) {

    //   alert("$('#container').length ==> " + $('#container').length);
    //  alert("DrawGanttChart")


    // THE CHART
    Highcharts.ganttChart('container', {
        title: {
            text: 'Target Schedule Vs Progress made (In %)'
        },
        subtitle: {
            text: '<span> <b>STATE : </b>' + state + '   |   <b>DISTRICT : </b>' + district + '   |   <b>BLOCK : </b>' + blockName + '   |  <b>PIU NAME : </b>' + piuName + '   |   <b>ROAD NAME : </b>' + roadName + '<br>' + '   |  <b>PACKAGE NO : </b>' + packageNo + '   |  <b>Sanction Length (Km) : </b>' + length + '   |  <b>SANCTION YEAR : </b>' + year + '   |   <b>SANCTION DATE : </b>' + date + '   |  <b>IMS BATCH : </b> ' + batch + '   |   <b>AGREEMENT COST (Lakhs) : </b> ' + agreeCost + '</span>',
            useHTML: true
        },
        credits: {
            enabled: false
        },
        xAxis: [
            { // first x-axis
                tickInterval: 1000 * 60 * 60 * 24,// 1 month
                visible: false,
                currentDateIndicator: {
                    width: 2,
                    dashStyle: 'dash',
                    color: 'red',
                    label: {
                        //format: '%Y-%M-%d'
                        format: '%d-%m-%Y'
                    }
                },
                type: 'datetime',
                labels: {
                    format: '{value:%d}'
                },
            },

            { // second x-axis
                tickInterval: 1000 * 60 * 60 * 24 * 30, // 1 day,
                // id: 'bottom-datetime-axis',
                currentDateIndicator: {
                    width: 2,
                    dashStyle: 'line',
                    color: 'red',
                    label: {
                        format: '%d-%m-%Y'
                    }
                },
                type: 'datetime',
                labels: {
                    format: '{value:%b-%Y}'
                },
                gridLineWidth: 1

            }


            //{ // second x-axis
            //    tickInterval: 1000 * 60 * 60 * 24 * 30, // 1 day,
            //    // id: 'bottom-datetime-axis',
            //    currentDateIndicator: {
            //        width: 2,
            //        dashStyle: 'line',
            //        color: 'red',
            //        label: {
            //            //format: '%Y-%M-%d'
            //            format: '%d-%m-%Y'
            //        }
            //    },

            //    //visible: false,
            //    type: 'datetime',
            //    //tickInterval: day,
            //    labels: {
            //        // format: '{value:%d-%m-%Y}'
            //    },
            //    //min: today.getTime() - (3 * day),
            //    //max: today.getTime() + (3 * day)
            //}



        ],
        //xAxis: {
        //    id: 'bottom-datetime-axis',
        //    currentDateIndicator: {
        //        width: 2,
        //        dashStyle: 'dot',
        //        color: 'red',
        //        label: {
        //            //format: '%Y-%M-%d'
        //            format: '%d-%M-%Y'
        //        }
        //    },
        //    //visible: false,
        //    type: 'datetime',
        //    tickInterval: day,
        //    labels: {
        //        format: '{value:%a}'
        //    },
        //    //min: today.getTime() - (3 * day),
        //    //max: today.getTime() + (3 * day)
        //},
        plotOptions: {
            series: {
                dataLabels: {
                    align: 'left',
                    enabled: true,
                    inside: true,
                    color: 'white',
                    style: {
                        //   fontSize: '16px',

                        textShadow: false,
                        textOutline: false
                    }
                },
                //color: 'black',
                //grouping: false
            }
        },
        series: [{
            name: 'Target Schedule',
            data: response.ListForChart
            //data: [{
            //    name: 'Test prototype',
            //    start: Date.UTC(2020, 12, 27),
            //    end: Date.UTC(2021, 8, 21),
            //    completed: {
            //        amount: 0.2,
            //         // fill: '#A9FF96'
            //    }, 
            //   // color:'red'

            //}, {
            //    name: 'Test prototype',
            //        start: Date.UTC(2020, 12, 27),
            //        end: Date.UTC(2021, 3, 21),
            //       // color: 'black'
            //}, {
            //    name: 'Develop',
            //        start: Date.UTC(2020, 12, 27),
            //        end: Date.UTC(2021, 5, 21),
            //        completed: {
            //        amount: 0.12,
            //      //  fill: '#A9FF96'
            //        },
            //   // color: 'red'
            //}, {
            //    name: 'Run acceptance tests',
            //        start: Date.UTC(2021, 1, 27),
            //        end: Date.UTC(2021, 9, 21),
            //    }]

        }]
    });

}

function BlockUI() {
    $.blockUI({
        message: '<span style="color:white;font-weight:bold;font-size:160%;">Please Wait...</span>',
        css: {
            border: 'none',
            padding: '10px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}



// By Akash
//$.ajax({
//    url: "/Home/GetDataForGanttChart",
//    type: "POST",
//    //data: { 'RoadCode': StateID, 'BaseLine': DistrictID },
//    data: { 'RoadCode': 231053, 'BaseLine': 1 },
//    success: function (response) {

//        //     alert("ajda.ListForChart ==> " + response.ListForChart.length);
//        console.log(response);
//        DrawGanttChart(response);

//        $.unblockUI();
//    },
//    error: function (xhr, status, err) {
//        $.unblockUI();
//        alert("Error occured while proccessing your request : " + err);
//    }
//});


function ClosePMISRoadDetails() {
    $('#accordion').hide('slow');
    $('#divPMISRoadListForm').hide('slow');
    $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
    $('#divFilterForm').show('slow');
}


function ViewChart1(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >View PMIS Project Plan</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISRoadDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divPMISRoadListForm").load('/PMIS/ViewPMISRoadProjectPlanLayout/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));
            unblockPage();
        });
        $('#divPMISRoadListForm').show('slow');
        $("#divPMISRoadListForm").css('height', 'auto');
    });
    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//function AddProjectPlan(cellvalue) {
//    debugger;
//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Add PMIS Project Plan</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISRoadDetails();" /></a>' +
//            '<span style="float: right;"></span>'
//            );

//    $('#accordion').show('slow', function () {

//        $("#divPMISRoadListForm").load('/PMIS/AddPMISRoadProjectPlan/' + cellvalue, function () {
//            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));

//        });

//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });

//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');

//    $('#idFilterDiv').trigger('click');
//}

//function DeleteProjectPlan(urlparameter) {

//    //var token = $('input[name=__RequestVerificationToken]').val();
//    debugger;
//    if (confirm("Are you sure to delete Plan ?")) {
//        $.ajax({
//            url: '/PMIS/DeletePmisRoadProjectPlan/' + urlparameter,
//            type: "POST",
//            cache: false,
//            async: false,
//            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
//            success: function (response) {
//                if (response.success) {
//                    alert("Plan deleted successfully.");
//                    LoadPMISRoadList();
//                    $('#tbPMISRoadList').trigger('reloadGrid');

//                }
//                else {
//                    alert(response.ErrorMessage)
//                    $('#tbPMISRoadList').trigger('reloadGrid');
//                }
//                $.unblockUI();
//            },
//            error: function () {

//                $.unblockUI();
//                alert("Error : " + error);
//                return false;
//            }
//        });

//    }


//}

//function RevisePlanDetails(RoadCode) {

//    //var token = $('input[name=__RequestVerificationToken]').val();
//    debugger;
//    if (confirm("Are you sure to revise Plan ?")) {
//        $.ajax({
//            url: '/PMIS/RevisePmisRoadProjectPlan/' + RoadCode,
//            type: "POST",
//            cache: false,
//            async: false,
//            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
//            success: function (response) {
//                if (response.success) {
//                    EditProjectPlan(RoadCode);
//                    //alert("Plan revised successfully.");
//                    //LoadPMISRoadList();
//                    //$('#tbPMISRoadList').trigger('reloadGrid');

//                }
//                else {
//                    alert(response.ErrorMessage)
//                    $('#tbPMISRoadList').trigger('reloadGrid');
//                }
//                $.unblockUI();
//            },
//            error: function () {

//                $.unblockUI();
//                alert("Error : " + error);
//                return false;
//            }
//        });

//    }


//}

//function FinalizeProjectPlan(RoadCode) {

//    //var token = $('input[name=__RequestVerificationToken]').val();
//    debugger;
//    if (confirm("Are you sure to finalize Plan ?")) {
//        $.ajax({
//            url: '/PMIS/FinalizePmisRoadProjectPlan/' + RoadCode,
//            type: "POST",
//            cache: false,
//            async: false,
//            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
//            success: function (response) {
//                if (response.success) {
//                    alert("Plan finalized successfully.");
//                    LoadPMISRoadList();
//                    $('#tbPMISRoadList').trigger('reloadGrid');

//                }
//                else {
//                    alert(response.ErrorMessage)
//                    $('#tbPMISRoadList').trigger('reloadGrid');
//                }
//                $.unblockUI();
//            },
//            error: function () {

//                $.unblockUI();
//                alert("Error : " + error);
//                return false;
//            }
//        });

//    }


//}

//function EditProjectPlan(id) {

//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Edit PMIS Project Plan</a>" +
//            '<a href="#" style="float: right;">' +
//            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISRoadDetails();" /></a>'
//            );

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#divPMISRoadListForm").load('/PMIS/UpdatePMISRoadProjectPlanLayout/' + id, function (response) {
//            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));
//            unblockPage();
//        });
//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });

//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');

//    $('#idFilterDiv').trigger('click');

//}

//function ViewProjectPlan(id) {

//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >View PMIS Project Plan</a>" +
//            '<a href="#" style="float: right;">' +
//            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISRoadDetails();" /></a>'
//            );

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#divPMISRoadListForm").load('/PMIS/ViewPMISRoadProjectPlanLayout/'+ id, function (response) {
//           $.validator.unobtrusive.parse($('#divPMISRoadListForm'));
//            unblockPage();
//        });
//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });
//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');
//    $('#idFilterDiv').trigger('click');

//}

//function ViewPartialProjectPlan(RoadNameParam, baselineParam) {
//    debugger;
//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >View PMIS Project Plan</a>" +
//            '<a href="#" style="float: right;">' +
//            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISRoadDetails();" /></a>'
//            );

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#divPMISRoadListForm").load('/PMIS/PMIS/ViewPMISRoadProjectPlan/?RoadName=' + encodeURIComponent(RoadNameParam) + '&baseline=' + baselineParam, function (response) {
//            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));
//            unblockPage();
//        });
//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });
//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');
//    $('#idFilterDiv').trigger('click');

//}

//function AddActuals(cellvalue) {
//    debugger;
//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Add Actuals</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISRoadDetails();" /></a>' +
//            '<span style="float: right;"></span>'
//            );

//    $('#accordion').show('slow', function () {

//        $("#divPMISRoadListForm").load('/PMIS/AddActuals/' + cellvalue, function () {
//            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));

//        });

//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });

//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');

//    $('#idFilterDiv').trigger('click');
//}

//function AddChainage(cellvalue) {
//    debugger;
//    $("#accordion div").html("");
//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Add Chainage wise Details</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISRoadDetails();" /></a>' +
//            '<span style="float: right;"></span>'
//            );

//    $('#accordion').show('slow', function () {

//        $("#divPMISRoadListForm").load('/PMIS/AddChainage/' + cellvalue, function () {
//            $.validator.unobtrusive.parse($('#divPMISRoadListForm'));

//        });

//        $('#divPMISRoadListForm').show('slow');
//        $("#divPMISRoadListForm").css('height', 'auto');
//    });

//    $("#tbPMISRoadList").jqGrid('setGridState', 'hidden');

//    $('#idFilterDiv').trigger('click');
//}
