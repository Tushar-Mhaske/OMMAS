var isPFAuthGridLoaded = false;
var isAFAuthGridLoaded = false;
var isMFAuthGridLoaded = false;


var programAssetChart = null, programLiaChart = null;
var adminAssetChart = null, adminLiaChart = null;
var mainAssetChart = null, mainLiaChart = null;

//var ProgramColour =['#e47297','#5C9384', '#981A37', '#5485BC', '#AA8C30', '#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375','#9c1c6b']

var colourArray = [];

///generic function to return the options for pie chart
function CommonOptions(ContainerDivID) {


    if ($('#' + ContainerDivID).highcharts()) {
        $('#' + ContainerDivID).highcharts().destroy();
    }

    switch (ContainerDivID) {
        case "AssetChart": colourArray = ['#5485BC', '#FCB319', '#20B2AA', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37']; break;
        case "LiaChart": colourArray = ['#20B2AA', '#5485BC', '#AA8C30', '#cb6828', '#aaaaab', '#981A37']; break;
        case "adminAssetChart": colourArray = ['#CDAD00', '#00526F', '#594266', '#9ACD32', '#cb6828', '#1E90FF', '#aaaaab', '#a89375', '#9c1c6b', '#1E90FF']; break;
        case "adminLiaChart": colourArray = ['#5485BC', '#FCB319', '#5C9384', '#981A37', '#5485BC', '#CDCD00', '#AA8C30', '#e47297', '#7CCD7C']; break;
        case "mainAssetChart": colourArray = ['#594266', '#cb6828', '#89158', '#cb6828', '#aaaaab', '#a89375']; break;
        case "mainLiaChart": colourArray = ['#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#9c1c6b']; break;

        //case "AssetChart": colourArray = ['#981A37', '#5485BC', '#AA8C30', '#FCB319', '#86A033', '#614931']; break;
        //case "LiaChart": colourArray = ['#981A37', '#5485BC', '#AA8C30', '#cb6828', '#aaaaab']; break;
        //case "adminAssetChart": colourArray = ['#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#9c1c6b']; break;
        //case "adminLiaChart": colourArray = ['#e47297', '#5C9384', '#981A37', '#5485BC', '#AA8C30', '#FCB319']; break;
        //case "mainAssetChart": colourArray = ['#594266', '#cb6828', '#89158', '#cb6828', '#aaaaab', '#a89375']; break;
        //case "mainLiaChart": colourArray = ['#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#9c1c6b']; break;
    }

    var optionsPie =
    {
        chart: {
            type: 'pie',
            renderTo: ContainerDivID,
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            margin: [0, 0, 0, 0],
            spacingTop: 0,
            spacingBottom: 0,
            spacingLeft: 0,
            spacingRight: 0
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
            enabled: true,
            animation: true,
            formatter: function () {
                return ' ' +
                    'Head: ' + this.point.x + '<br />' +
                    'Percentage: ' + this.point.y + "%" + '<br />' +
                    'Amount: Rs.' + this.point.z + " Lacs";
            },
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
                    enabled: false,

                    style: {
                        width: '30px'
                    },
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function () {

                        if (this.point.z > 0) {

                            return '<b>' + this.point.x + ': </br> Rs.' + this.point.z + '' + " Lacs </b>";
                        }
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
            labelFormatter: function () {
                return '<div style="width:200px"><span style="float:left">' + this.x + '</span><span style="float:left"> Rs.' + this.z + ' Lacs </span></div>';
            },
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


//function to return the generic setting for column chart
function CommonColumnChartOptions(ContainerDivID) {


    var columnOptions =
    {
        chart: {
            renderTo: ContainerDivID,
            defaultSeriesType: 'column'
        },

        title: {
            text: ''
        },

        xAxis: {
            categories: [],
            tickmarkPlacement: 'on'
        },

        yAxis: {
            min: 0,
            title: {
                text: 'Amount(in Lacs.) '//amountLocalised.toString(),
            }
        },
        credits: {
            enabled: false
        },

        plotArea: {
            shadow: null,
            borderWidth: null,
            backgroundColor: null
        },

        plotOptions: {
            column: {
                allowPointSelect: true,
                cursor: 'pointer',
                pointPadding: 0.0,
                borderWidth: 0,
                point: {
                    events: {
                        click: function () {


                        }
                    }

                }
            }
        },


        tooltip: {
            formatter: function () {
                var pgm = this.series.name.split(",");

                return pgm[0] + ': ' + this.y;
            }
        },


        series: []
    }



    columnOptions.colors = Highcharts.map(colourArray, function (color) {
        return {
            radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
            stops: [
                [0, color],
                [1, Highcharts.Color(color).brighten(-0.1).get('rgb')] // darken
            ]
        };
    });
    return columnOptions;
}


$(function () {
    // $("#programmeTabs1").tabs();

    $('#programmeTabs1').tabs({
        activate: function (event, ui) {
            // console.log(event);
            // console.log(ui.newTab.index());

            if (ui.newTab.index() == 0) {

                $('#AssetGridDiv').hide();
                $("#divProgramChartIcon").hide();
                $("#divProgramGridIcon").show();

                $("#AssetChart").show();

                GetAssetLiabilityChart("P", "A", programAssetChart, "AssetChart");

            } else if (ui.newTab.index() == 1) {

                $('#LiabilityGridDiv').hide();
                $("#divProgramChartIcon").hide();
                $("#divProgramGridIcon").show();
                $("#LiaChart").show();

                GetAssetLiabilityChart("P", "L", programLiaChart, "LiaChart");

            }
        }
    });




    $('#programmeTabs2').tabs({
        activate: function (event, ui) {

            if (ui.newTab.index() == 0) {

                CreateAuthorizationGrid("P", "tblProgramAuthReceivedList", "divProgramAuthReceivedPager");

            } else if (ui.newTab.index() == 1) {

                CreateSummaryGrid("P", "tblProgramSummaryList", "divProgramSummaryPager");
            } else if (ui.newTab.index() == 2) {

                if (levelId == "5") {
                    //  LoadPFAuthorizationGrid();
                    $("#SRRDATD").hide();
                }

            }
        }
    });


    //$("#adminTabs1").tabs();

    $('#adminTabs1').tabs({
        activate: function (event, ui) {


            if (ui.newTab.index() == 0) {

                $('#adminAssetGridDiv').hide();
                $("#divAdminChartIcon").hide();
                $("#divAdminGridIcon").show();
                $("#adminAssetChart").show();

                GetAssetLiabilityChart("A", "A", adminAssetChart, "adminAssetChart");

            } else if (ui.newTab.index() == 1) {


                $('#adminLiabilityGridDiv').hide();
                $("#divAdminChartIcon").hide();
                $("#divAdminGridIcon").show();
                $("#adminLiaChart").show();

                GetAssetLiabilityChart("A", "L", adminLiaChart, "adminLiaChart");

            }
        }
    });


    $('#adminTabs2').tabs({
        activate: function (event, ui) {


            if (ui.newTab.index() == 0) {
                CreateAuthorizationGrid("A", "tblAdminAuthReceivedList", "divAdminAuthReceivedPager");
            } else if (ui.newTab.index() == 1) {

                CreateSummaryGrid("A", "tblAdminSummaryList", "divAdminSummaryPager");
            }
        }
    });



    $('#mainTabs1').tabs({
        activate: function (event, ui) {

            if (ui.newTab.index() == 0) {

                $('#mainAssetGridDiv').hide();
                $("#divMainChartIcon").hide();
                $("#divMainGridIcon").show();
                $("#mainLiaChart").show();

                GetAssetLiabilityChart("M", "A", mainAssetChart, "mainAssetChart");

            } else if (ui.newTab.index() == 1) {

                $('#adminLiabilityGridDiv').hide();
                $("#divAdminChartIcon").hide();
                $("#divAdminGridIcon").show();
                $("#adminLiaChart").show();

                GetAssetLiabilityChart("M", "L", mainLiaChart, "mainLiaChart");

            }
        }
    });


    $('#mainTabs2').tabs({
        activate: function (event, ui) {

            if (ui.newTab.index() == 0) {
                CreateAuthorizationGrid("M", "tblMainAuthReceivedList", "divMainAuthReceivedPager");
            } else if (ui.newTab.index() == 1) {
                CreateSummaryGrid("M", "tblMainSummaryList", "divMainSummaryPager");
            }
        }
    });





});




$(document).ready(function () {

    //Added By Abhishek kamble 6-May-2014 start
    $(function () {
        $.ajax({
            type: 'POST',
            url: '/Master/GetPMGSY2Status?id=' + $("#PMGSY2StateCode").val(),
            async: false,
            cache: false,
            success: function (data) {
                //alert(data.success);
                if (data.success == true) {
                    $("#spnPMGSY2").show();
                }
                else {
                    $("#spnPMGSY2").hide();
                }
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }
        });

    });
    //Added By Abhishek kamble 6-May-2014 end

    /////------------------- Log Off QTip -----------------------////

    //$("#switcher").themeswitcher({
    //    imgpath: "../../Content/images/",
    //    loadTheme: "UI-Lightness"
    //});

    //Added By Abhishek kamble 24-jan-2014 Start
    $("#MONTH").change(function () {
        UpdateAccountSession($("#MONTH").val(), $("#YEAR").val());
    });

    $("#YEAR").change(function () {
        UpdateAccountSession($("#MONTH").val(), $("#YEAR").val());
    });
    //Added By Abhishek kamble 24-jan-2014 End

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


    //$(".menubar-icons").menubar({
    //    autoExpand: true,
    //    menuIcon: true,
    //    buttons: true,
    //    position: {
    //        within: $("#demo-frame").add(window).first()
    //    }

    //});


    /////------------------- Log Off QTip -----------------------////




    if (levelId == "5") {
        LoadPFAuthorizationGrid();
        $("#SRRDATD").hide();
    }


    //function for handling event of click  of view details button
    $("#btnView").click(function () {


        var first_val = $("input[name=LEVEL]:checked").val();

        if (first_val == 4)  //SRRDA
        {
            $("#spnMaintenanceAuthTab").html("Authorization Received");
            $("#spnProgrammeAuthTab").html("Authorization Received");
            $("#spnAdministrativeAuthTab").html("Authorization Received");


        } else if (first_val == 5) //DPIU
        {
            $("#spnMaintenanceAuthTab").html("Fund Received");
            $("#spnProgrammeAuthTab").html("Fund Received");
            $("#spnAdministrativeAuthTab").html("Fund Received");

        }

        //========================program fund==================

        //program fund asset
        if ($("#programmeTabs1").tabs('option', 'active') == 0) {


            //if asset grid is visible 
            if (!$("#AssetChart").is(":visible")) {
                //reload the asset grid 
                GetAssetsGrid("P", "tblAssetList", "divAssetPager");
            }
            else {
                //reload chart
                GetAssetLiabilityChart("P", "A", programAssetChart, "AssetChart");
            }
        }
        else {

            //program fund liability
            //if asset grid is visible 
            if (!$("#LiaChart").is(":visible")) {
                //reload the liability  grid 
                getLiabilitiesGrid("P", "tblLiaList", "divLiaPager");
            }
            else {
                //reload laibility chart
                GetAssetLiabilityChart("P", "L", programLiaChart, "LiaChart");
            }

        }


        if ($("#programmeTabs2").tabs('option', 'active') == 0) {

            //reload Athorization grid
            CreateAuthorizationGrid("P", "tblProgramAuthReceivedList", "divProgramAuthReceivedPager");

        }
        else {
            //reload summary grid
            CreateSummaryGrid("P", "tblProgramSummaryList", "divProgramSummaryPager");
        }


        //============================================================================================

        //======================================admin fund Start ========================================= 


        //admin fund asset
        if ($("#adminTabs1").tabs('option', 'active') == 0) {


            //if asset grid is visible 
            if (!$("#adminAssetChart").is(":visible")) {
                //reload the asset grid 
                GetAssetsGrid("A", "admintblAssetList", "admindivAssetPager");
            }
            else {
                //reload chart
                GetAssetLiabilityChart("A", "A", adminAssetChart, "adminAssetChart");
            }
        }
        else {

            //admin fund liability
            //if asset grid is visible 
            if (!$("#adminLiaChart").is(":visible")) {
                //reload the liability  grid 
                getLiabilitiesGrid("A", "admintblLiaList", "admindivLiaPager");
            }
            else {
                //reload laibility chart
                GetAssetLiabilityChart("A", "L", adminLiaChart, "adminLiaChart");
            }

        }


        if ($("#adminTabs2").tabs('option', 'active') == 0) {

            //reload Athorization grid
            CreateAuthorizationGrid("A", "tblAdminAuthReceivedList", "divAdminAuthReceivedPager");

        }
        else {
            //reload summary grid
            CreateSummaryGrid("A", "tblAdminSummaryList", "divAdminSummaryPager");
        }


        //=========================================admin fund end===========================================

        //======================Mainatanance fund start======================================================


        //admin fund asset
        if ($("#mainTabs1").tabs('option', 'active') == 0) {


            //if asset grid is visible 
            if (!$("#mainAssetChart").is(":visible")) {
                //reload the asset grid 
                GetAssetsGrid("M", "maintblAssetList", "mainivAssetPager");
            }
            else {
                //reload chart
                GetAssetLiabilityChart("M", "A", mainAssetChart, "mainAssetChart");
            }
        }
        else {

            //admin fund liability
            //if asset grid is visible 
            if (!$("#mainLiaChart").is(":visible")) {
                //reload the liability  grid 
                getLiabilitiesGrid("M", "maintblLiaList", "maindivLiaPager");
            }
            else {
                //reload laibility chart
                GetAssetLiabilityChart("M", "L", mainLiaChart, "mainLiaChart");
            }

        }


        if ($("#mainTabs2").tabs('option', 'active') == 0) {

            //reload Athorization grid
            CreateAuthorizationGrid("M", "tblMainAuthReceivedList", "divMainAuthReceivedPager");

        }
        else {
            //reload summary grid
            CreateSummaryGrid("M", "tblMainSummaryList", "divMainSummaryPager");
        }






        //====================================================================================================

    });



    //create chart
    GetAssetLiabilityChart("P", "A", programAssetChart, "AssetChart");
    GetAssetLiabilityChart("A", "A", adminAssetChart, "adminAssetChart");
    GetAssetLiabilityChart("M", "A", mainAssetChart, "mainAssetChart");

    //create authorization grid 
    CreateAuthorizationGrid("P", "tblProgramAuthReceivedList", "divProgramAuthReceivedPager");
    CreateAuthorizationGrid("A", "tblAdminAuthReceivedList", "divAdminAuthReceivedPager");
    CreateAuthorizationGrid("M", "tblMainAuthReceivedList", "divMainAuthReceivedPager");



    //================================================================= program fund =======================================
    $("#divProgramPlus").click(function () {

        $("#divAdmin").hide();
        $("#divMain").hide();
        $("#divProgram").css('width', '99%');
        jQuery("#tblPFAuthList").setGridWidth($("#divPFAuth").width());
        jQuery("#tblAssetList").setGridWidth($("#AssetGridDiv").width());
        jQuery("#tblLiaList").setGridWidth($("#LiabilityGridDiv").width());

        jQuery("#tblProgramAuthReceivedList").setGridWidth($("#divProgramAuthDetails").width());
        jQuery("#tblProgramSummaryList").setGridWidth($("#divProgramSummaryDetails").width());

        $("#divProgramPlus").hide();
        $("#divProgramMinus").show();

        $(window).trigger('resize');

        /* var chart = $('#AssetChart').highcharts();
         var opt = chart.series[0].options;
         alert(opt.dataLabels.enabled);
         opt.dataLabels.enabled = !opt.dataLabels.enabled;
         chart.series[0].update(opt);*/
        /*  
          if ($('#AssetChart').highcharts())
          {
             $('#AssetChart').highcharts().legend.group.show();
          
          } else if ($('#LiaChart').highcharts()) 
          {
         
              $('#LiaChart').highcharts().legend.group.show();
          }*/

    });



    $("#divProgramMinus").click(function () {
        $("#divProgram").css('width', '98%');

        $('#divMain').toggle('slow', function () {
            $(window).trigger('resize');
        });
        $('#divAdmin').toggle('slow', function () {
            $(window).trigger('resize');
        });

        $("#divAdmin").show('slow');
        $("#divMain").show('slow');

        jQuery("#tblPFAuthList").setGridWidth($("#divPFAuth").width());
        jQuery("#tblAssetList").setGridWidth($("#AssetGridDiv").width());
        jQuery("#tblLiaList").setGridWidth($("#LiabilityGridDiv").width());
        jQuery("#tblProgramAuthReceivedList").setGridWidth($("#divProgramAuthDetails").width());
        jQuery("#tblProgramSummaryList").setGridWidth($("#divProgramSummaryDetails").width());

        $("#divProgramPlus").show();
        $("#divProgramMinus").hide();

        /*  if ($('#AssetChart').highcharts())
          {
              $('#AssetChart').highcharts().legend.group.hide();
          
          } else if ($('#LiaChart').highcharts()) {
         
              $('#LiaChart').highcharts().legend.group.hide();
          }
          */
    });


    //================================================================================

    //============================================admin fund=============================
    $("#divAdminPlus").click(function () {

        $("#divProgram").hide();
        $("#divMain").hide();
        $("#divAdmin").css('width', '99%');
        jQuery("#admintblAssetList").setGridWidth($("#adminAssetGridDiv").width());
        jQuery("#admintblLiaList").setGridWidth($("#adminLiabilityGridDiv").width());

        jQuery("#tblAdminAuthReceivedList").setGridWidth($("#divAdminAuthDetails").width());
        jQuery("#tblAdminSummaryList").setGridWidth($("#divAdminSummaryDetails").width());

        $("#divAdminPlus").hide();
        $("#divAdminMinus").show();

        $(window).trigger('resize');

        //$('#adminLiaChart').highcharts().legend.group.show();
        //$('#adminAssetChart').highcharts().legend.group.show();

    });


    $("#divAdminMinus").click(function () {
        $("#divAdmin").css('width', '98%');
        //$("#divProgram").show('slow');

        $('#divProgram').toggle('slow', function () {
            $(window).trigger('resize');
        });
        $('#divMain').toggle('slow', function () {
            $(window).trigger('resize');
        });

        $("#divAdminPlus").show();
        $("#divAdminMinus").hide();
        jQuery("#admintblAssetList").setGridWidth($("#adminAssetGridDiv").width());
        jQuery("#admintblLiaList").setGridWidth($("#adminLiabilityGridDiv").width());
        jQuery("#tblAdminAuthReceivedList").setGridWidth($("#divAdminAuthDetails").width());
        jQuery("#tblAdminSummaryList").setGridWidth($("#divAdminSummaryDetails").width());

        //$('#adminAssetChart').highcharts().legend.group.hide();
        //$('#adminLiaChart').highcharts().legend.group.hide();


    });
    //=======================================================admin fund===========================================


    //==================================================main anance fund ======================================
    $("#divMainPlus").click(function () {

        $("#divProgram").hide();
        $("#divAdmin").hide();
        $("#divMain").css('width', '99%');
        jQuery("#maintblAssetList").setGridWidth($("#mainAssetGridDiv").width());
        jQuery("#maintblLiaList").setGridWidth($("#mainLiabilityGridDiv").width());

        jQuery("#tblMainAuthReceivedList").setGridWidth($("#divMainAuthDetails").width());
        jQuery("#tblMainSummaryList").setGridWidth($("#divMainSummaryDetails").width());

        $("#divMainPlus").hide();
        $("#divMainMinus").show();
        $(window).trigger('resize');

        //$('#mainAssetChart').highcharts().legend.group.show();
        //$('#mainLiaChart').highcharts().legend.group.show();

    });


    $("#divMainMinus").click(function () {
        $("#divMain").css('width', '98%');

        // $("#divProgram").show('slow');
        // $("#divAdmin").show('slow');

        $('#divProgram').toggle('slow', function () {
            $(window).trigger('resize');
        });
        $('#divAdmin').toggle('slow', function () {
            $(window).trigger('resize');
        });

        $("#divMainPlus").show();
        $("#divMainMinus").hide();

        jQuery("#maintblAssetList").setGridWidth($("#mainAssetGridDiv").width());
        jQuery("#maintblLiaList").setGridWidth($("#mainLiabilityGridDiv").width());
        jQuery("#tblMainAuthReceivedList").setGridWidth($("#divMainAuthDetails").width());
        jQuery("#tblMainSummaryList").setGridWidth($("#divMainSummaryDetails").width());

        //$('#mainAssetChart').highcharts().legend.group.hide();
        //$('#mainLiaChart').highcharts().legend.group.hide();


    });

    //==========================================================maintainance  icon click events=======================================

    $("#divMainGridIcon").click(function () {

        $("#divMainChartIcon").show();
        $("#divMainGridIcon").hide();

        if ($("#mainTabs1").tabs('option', 'active') == 0) {

            $("#mainAssetChart").hide('slow');
            $('#mainAssetGridDiv').hide();
            $('#mainAssetGridDiv').toggle('slow', function () {
                GetAssetsGrid("M", "maintblAssetList", "mainivAssetPager");
            });

        }
        else {
            $("#mainLiaChart").hide('slow');
            $('#mainLiabilityGridDiv').hide();
            $('#mainLiabilityGridDiv').toggle('slow', function () {
                getLiabilitiesGrid("M", "maintblLiaList", "maindivLiaPager");
            });
        }

    });

    $("#divMainChartIcon").click(function () {

        $("#divMainChartIcon").hide();
        $("#divMainGridIcon").show();

        $(window).trigger('resize');

        if ($("#mainTabs1").tabs('option', 'active') == 0) {

            $("#mainAssetChart").show('slow');
            $('#mainAssetGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("M", "A", mainAssetChart, "mainAssetChart");
            });

        } else {
            $("#mainLiaChart").show('slow');
            $('#mainLiabilityGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("M", "L", mainLiaChart, "mainLiaChart");
            });
        }


    });


    //==========================================================end maintainance icon click events================================================================

    //=================================================program icon click envents============================================================


    $("#divProgramGridIcon").click(function () {

        $("#divProgramChartIcon").show();
        $("#divProgramGridIcon").hide();

        if ($("#programmeTabs1").tabs('option', 'active') == 0) {
            $("#AssetChart").hide('slow');

            $('#AssetGridDiv').hide();
            $('#AssetGridDiv').toggle('slow', function () {
                GetAssetsGrid("P", "tblAssetList", "divAssetPager");
            });
        } else {
            $("#LiaChart").hide('slow');

            $('#LiabilityGridDiv').hide();

            $('#LiabilityGridDiv').toggle('slow', function () {

                getLiabilitiesGrid("P", "tblLiaList", "divLiaPager");

            });

        }

    });
    $("#divProgramChartIcon").click(function () {

        $("#divProgramChartIcon").hide();
        $("#divProgramGridIcon").show();
        $(window).trigger('resize');

        if ($("#programmeTabs1").tabs('option', 'active') == 0) {
            $("#AssetChart").show('slow');

            $('#AssetGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("P", "A", programAssetChart, "AssetChart");
            });


        } else {
            $("#LiaChart").show('slow');

            $('#LiabilityGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("P", "L", programLiaChart, "LiaChart");
            });
        }
    });

    //=========================================================================================================================================

    //=================================================admin icon click events==================================================================

    $("#divAdminGridIcon").click(function () {

        $("#divAdminChartIcon").show();
        $("#divAdminGridIcon").hide();

        if ($("#adminTabs1").tabs('option', 'active') == 0) {
            $("#adminAssetChart").hide('slow');
            $('#adminAssetGridDiv').hide();
            $('#adminAssetGridDiv').toggle('slow', function () {
                GetAssetsGrid("A", "admintblAssetList", "admindivAssetPager");
            });

        }
        else {
            $("#adminLiaChart").hide('slow');
            $('#adminLiabilityGridDiv').hide();
            $('#adminLiabilityGridDiv').toggle('slow', function () {
                getLiabilitiesGrid("A", "admintblLiaList", "admindivLiaPager");
            });
        }
    });

    $("#divAdminChartIcon").click(function () {

        $("#divAdminChartIcon").hide();
        $("#divAdminGridIcon").show();
        $(window).trigger('resize');

        if ($("#adminTabs1").tabs('option', 'active') == 0) {

            $("#adminAssetChart").show('slow');
            $('#adminAssetGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("A", "A", adminAssetChart, "adminAssetChart");
            });

        } else {
            $("#adminLiaChart").show('slow');
            $('#adminLiabilityGridDiv').toggle('slow', function () {
                GetAssetLiabilityChart("A", "L", adminLiaChart, "adminLiaChart");
            });
        }

    });

    //=========================================================================================================================
    $("#rdDPIU").click(function () {

        $("#DPIU").show('slow');
    });

    $("#rdSRRDA").click(function () {

        $("#DPIU").hide('slow');
    });

    $("#DigReceiptPayment").dialog({
        resizable: false,
        closeOnEscape: true,
        title: "Receipt/ Payment Entry Screen",
        height: 'auto',
        width: '400',
        modal: true,
        autoOpen: false,
        open: function () {
            $(this).parent().appendTo($('#frmAddReceipt'));
        }
    });


    //Added By Abhishek kamble 28-Feb-2014

    $("#ddlSRRDA").change(function () {
        //alert();
        //$.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
        var val = $("#ddlSRRDA").val() + "$" + "S";
        $.ajax({
            type: 'POST',
            url: "/AccountsReports/PopulateDPIU?id=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#DPIU").empty();
                $.each(data, function () {
                    $("#DPIU").append("<option value=" + this.Value + ">" +
                        this.Text + "</option>");
                });

                //      $.unblockUI();
            }

        });


    });


    $(function () {
        if (levelId == 6) {
            $("#ddlSRRDA").trigger('change');
        }

        $("#spnMaintenanceAuthTab").html("Authorization Received");
        $("#spnProgrammeAuthTab").html("Authorization Received");
        $("#spnAdministrativeAuthTab").html("Authorization Received");
    });

});

function LoadPFAuthorizationGrid() {
    if (isPFAuthGridLoaded) {
        $("#tblPFAuthList").GridUnload();
        isPFAuthGridLoaded = false;
    }

    jQuery("#tblPFAuthList").jqGrid({
        url: '/Accounts/GetPFAuthorizationList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Authorization Number', 'Authorization Date', 'Amount', 'Action'],
        colModel: [
            { name: 'AuthNumber', index: 'AuthNumber', width: 100, align: 'center', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
            { name: 'AuthDate', index: 'AuthDate', width: 80, align: 'center', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
            { name: 'Amount', index: 'Amount', width: 90, align: 'right', sortable: true },
            { name: 'Action', index: 'Action', width: 50, align: 'center' },
        ],
        pager: jQuery('#divPFAuthPager'),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AuthDate',
        sortorder: "asc",
        caption: "Authorization Details",
        height: 'auto',
        autowidth: true,
        //rownumbers: true,
        loadComplete: function () {

            $("#gview_tblPFAuthList > .ui-jqgrid-titlebar").hide();

            isPFAuthGridLoaded = true;
            $('a.ui-qtp-dig').qtip({
                content: {
                    text: $('a.ui-qtp-dig').attr('title'),
                    title: {
                        text: 'Authorization request rejected', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top center',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });

            $('span.ui-qtp-dig').qtip({
                content: {
                    text: $('span.ui-qtp-dig').attr('title'),
                    title: {
                        text: 'Authorization Amount Details', // Give the tooltip a title using each elements text
                        button: true
                    }
                },
                position: {
                    at: 'bottom center', // Position the tooltip above the link
                    my: 'top center',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                show: {
                    event: 'click',
                    solo: true // Only show one tooltip at a time
                },
                hide: 'unfocus',
                style: {
                    classes: 'ui-state-default'
                }
            });


            //if ($('#tblOBList').jqGrid('getGridParam', 'reccount') > 0) {
            //    $("#AddOBMaster").hide();
            //}
            //$("#divOBListPager_right").html("<span style='float:right'>Status represents <font color='green'>OB Details Entered</font> and <font color='#b83400'>OB Details Remained</font> Amount</span><span style='float:right' class='ui-icon ui-icon-info'></span>");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });//end of documents grid
}

function GetAssetsGrid(fundtype, tableName, pagerName) {

    CreateAssetLiabilityGrid(fundtype, "A", tableName, pagerName);

}

function getLiabilitiesGrid(fundtype, tableName, pagerName) {
    CreateAssetLiabilityGrid(fundtype, "L", tableName, pagerName);
}


function GetAuthorizationGrid() {
    CreateAuthorizationGrid(fundtype, tableName, pagerName);
}


function getSummaryGrid() {


}






function CreateAssetLiabilityGrid(fundtype, assetOrliability, gridtableName, gridPagerName) {
    //alert(fundtype + "," + assetOrliability + "," + $("#MONTH").val() + "," + $("#YEAR").val() + "," + levelId)
    var title = assetOrliability == "A" ? " Asset" : " Liablity"

    $("#" + gridtableName).GridUnload();

    $("#" + gridtableName).jqGrid('GridDestroy');

    $("#" + gridtableName).jqGrid({
        url: '/Accounts/GetAssetliabilityList/',
        datatype: "json",
        mtype: "POST",
        postData: {
            'fundType': fundtype,
            'assetOrliability': assetOrliability,
            'month': $("#MONTH").val(),
            'year': $("#YEAR").val(),
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
                    if ($("#rdSRRDA").is(":checked")) {
                        return "0";
                    } else {
                        if ($("#DPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#DPIU").val();
                        }
                    }



                }


            },
            'ownLower': function () {
                if (levelId == "5") {
                    return "O";
                } else {

                    if ($("#rdSRRDA").is(":checked")) {
                        return "O";
                    } else {
                        return "L";
                    }
                }
            },
            'rptid': function () {

                if (fundtype == "A") {

                    if (levelId == "5") {
                        return 6;

                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 5;
                        } else {
                            return 6;

                        }

                    }
                }
                else if (fundtype == "P") {

                    if (levelId == "5") {
                        return 8;

                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 2;
                        } else {
                            return 8;

                        }

                    }
                }
                else if (fundtype == "M") {
                    if (levelId == "5") {
                        return 3;
                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 4;
                        } else {
                            return 3;

                        }
                    }
                }
            },
            'selectioncode': function () {
                if (levelId == "5") {
                    return "L";
                }
                if ($("#rdSRRDA").is(":checked")) {

                    return "A";
                }
                else {

                    if ($("#DPIU").val() == 0) {
                        return "A";
                    } else {
                        return "L";
                    }

                }
            },
            'adminNdCode': $("#ddlSRRDA option:selected").val()//added by Abhishek kamble 28-Feb-2014
        },
        colNames: ['id', 'Head', 'Amount (in Lacs)'],
        colModel: [
            { name: 'id', index: 'id', width: 0, align: 'center', hidden: true },
            { name: 'Head', index: 'Head', width: 140, align: 'left' },
            { name: 'Amount', index: 'Amount', width: 50, align: 'right' },

        ],
        pager: jQuery('#' + gridPagerName),
        rowNum: 5000,
        //rowList: [5],
        pginput: false,
        viewrecords: true,

        sortname: 'AuthDate',
        sortorder: "asc",
        caption: title,
        height: 'auto',
        autowidth: true,
        //rownumbers: true,
        loadComplete: function () {

            $("#gview_" + gridtableName + " > .ui-jqgrid-titlebar").hide();

            //jQuery('#' + gridPagerName + "_left").html("All Amount are in Lacs");

            $("#" + gridtableName).setGridWidth($("#gbox_" + gridtableName).parent().width());

            $("#" + gridtableName).parents('div.ui-jqgrid-bdiv').css("max-height", $("#gbox_" + gridtableName).parent().width());

        },
        loadError: function (xhr, ststus, error) {

            alert("Invalid data.Please check and Try again!")

        }

    });


}

function CreateAuthorizationGrid(fundtype, gridtableName, gridPagerName) {

    var title = "Authorization Received Details";

    $("#" + gridtableName).GridUnload();

    $("#" + gridtableName).jqGrid('GridDestroy');

    $("#" + gridtableName).jqGrid({
        url: '/Accounts/GetAuthorizationReceivedList/',
        datatype: "json",
        mtype: "POST",
        postData: {
            'fundType': fundtype,
            'month': $("#MONTH").val(),
            'year': $("#YEAR").val(),
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
                    if ($("#rdSRRDA").is(":checked")) {
                        return "0";
                    } else {
                        if ($("#DPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#DPIU").val();
                        }
                    }
                }
            },
            'ownLower': function () {
                if (levelId == "5") {
                    return "O";
                } else {

                    if ($("#rdSRRDA").is(":checked")) {
                        return "O";
                    } else {
                        return "L";
                    }
                }
            },
            'adminNdCode': $("#ddlSRRDA option:selected").val()//added by Abhishek kamble 28-Feb-2014
        },
        colNames: ['Receipt No.', 'Receipt Date', 'Reference Number', "Head", "Amount (in Rs.)"],
        colModel: [
            { name: 'RN', index: 'RN', width: 35, align: 'center', hidden: false },
            { name: 'Receipt_date', index: 'Receipt_date', width: 50, align: 'center' },
            { name: 'Reference_Number', index: 'Reference_Number', width: 0, align: 'center', hidden: true },
            { name: 'Head', index: 'Head', width: 140, align: 'left' },
            { name: 'Amount', index: 'Amount', width: 60, align: 'right' }

        ],
        pager: jQuery('#' + gridPagerName),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,

        sortname: 'Receipt_date',
        sortorder: "asc",
        caption: title,
        height: 'auto',
        autowidth: true,
        //rownumbers: true,
        loadComplete: function () {
            $("#gview_" + gridtableName + " > .ui-jqgrid-titlebar").hide();
            //jQuery('#' + gridPagerName + "_left").html("All Amount are in Lacs");
            $("#" + gridtableName).setGridWidth($("#gbox_" + gridtableName).parent().width());

        },
        loadError: function (xhr, ststus, error) {

            alert("Invalid data.Please check and Try again!")

        }

    });


}

///function to get the summary grid
function CreateSummaryGrid(fundtype, gridtableName, gridPagerName) {
    var title = "Summary";


    $("#" + gridtableName).GridUnload();

    $("#" + gridtableName).jqGrid('GridDestroy');

    $("#" + gridtableName).jqGrid({
        url: '/Accounts/GetSummaryList/',
        datatype: "json",
        mtype: "POST",
        postData: {
            'fundType': fundtype,
            'month': $("#MONTH").val(),
            'year': $("#YEAR").val(),
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
                    if ($("#rdSRRDA").is(":checked")) {
                        return "0";
                    } else {
                        if ($("#DPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#DPIU").val();
                        }
                    }
                }
            },
            'ownLower': function () {
                if (levelId == "5") {
                    return "O";
                } else {

                    if ($("#rdSRRDA").is(":checked")) {
                        return "O";
                    } else {
                        return "L";
                    }
                }
            },
            'adminNdCode': $("#ddlSRRDA option:selected").val()//added by Abhishek kamble 28-Feb-2014
        },
        colNames: ['Bill Type', 'Description', 'No.', "Amount (in Lacs) ", "No.", "Amount (in Lacs)"],
        colModel: [
            { name: 'BillType', index: 'BillType', width: 30, align: 'center', hidden: true },
            { name: 'Description', index: 'Description', width: 120, align: 'left' },
            { name: 'Upto_Count', index: 'Upto_Count', width: 30, align: 'center' },
            { name: 'Upto_Amount', index: 'Upto_Amount', width: 40, align: 'right' },
            { name: 'Month_Count', index: 'Month_Count', width: 30, align: 'center' },
            { name: 'Month_Amount', index: 'Month_Amount', width: 60, align: 'right' }

        ],
        pager: jQuery('#' + gridPagerName),
        rowNum: 5,
        //rowList: [5],
        pginput: false,
        viewrecords: true,

        sortname: 'BillType',
        sortorder: "asc",
        caption: title,
        height: 'auto',
        autowidth: true,
        //rownumbers: true,
        loadComplete: function () {
            //$('#' + gridtableName).jqGrid('setGridWidth', '1500');
            $("#gview_" + gridtableName + " > .ui-jqgrid-titlebar").hide();

            //jQuery('#' + gridPagerName + "_left").html("All Amount are in Lacs");

            // $("#" + gridtableName).setGridWidth($("#gbox_" + gridtableName).parent().width());

        },
        loadError: function (xhr, ststus, error) {

            alert("Invalid data.Please check and Try again!")

        }
    });


    jQuery("#" + gridtableName).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'Upto_Count', numberOfColumns: 2, titleText: 'Previous Month' },
        { startColumnName: 'Month_Count', numberOfColumns: 2, titleText: 'Current Month' }]
    });

}


function AddAuthDetails(urlParam) {
    $("#DigReceiptPayment").load("/Authorization/AddReceiptPaymentDetails/" + urlParam, function () {
        $("#DigReceiptPayment").show();
        $("#DigReceiptPayment").dialog('open');
    });
}

function ViewAuthDetails(urlParam) {
    $("#DigReceiptPayment").load("/Authorization/ViewReceiptPaymentDetails/" + urlParam, function () {
        $("#DigReceiptPayment").show();
        $("#DigReceiptPayment").dialog('open');
    });
}

//function to get the asset liability chart
function GetAssetLiabilityChart(fundtype, assetOrliability, chart, ContainerID) {
    $.ajax({
        type: "POST",
        url: '/Accounts/GetAssetliabilityChart/',
        data:
        {
            'fundType': fundtype,
            'assetOrliability': assetOrliability,
            'month': $("#MONTH").val(),
            'year': $("#YEAR").val(),
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
                    if ($("#rdSRRDA").is(":checked")) {
                        return "0";
                    } else {
                        if ($("#DPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#DPIU").val();
                        }
                    }
                }
            },
            'ownLower': function () {
                if (levelId == "5") {
                    return "O";
                } else {

                    if ($("#rdSRRDA").is(":checked")) {
                        return "O";
                    } else {
                        return "L";
                    }
                }
            },
            'rptid': function () {

                if (fundtype == "A") {

                    if (levelId == "5") {
                        return 6;

                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 5;
                        } else {
                            return 6;

                        }

                    }
                }
                else if (fundtype == "P") {

                    if (levelId == "5") {
                        return 8;

                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 2;
                        } else {
                            return 8;

                        }

                    }
                }
                else if (fundtype == "M") {
                    if (levelId == "5") {
                        return 3;
                    } else {

                        if ($("#rdSRRDA").is(":checked")) {
                            return 4;
                        } else {
                            return 3;

                        }
                    }
                }
            },
            'selectioncode': function () {
                if (levelId == "5") {
                    return "L";
                }
                if ($("#rdSRRDA").is(":checked")) {

                    return "A";
                }
                else {

                    if ($("#DPIU").val() == 0) {
                        return "A";
                    } else {
                        return "L";
                    }

                }
            },
            'adminNdCode': $("#ddlSRRDA option:selected").val()//added by Abhishek kamble 28-Feb-2014
        },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;

        },
        success: function (data) {


            var series1 = null;

            if (data == "") {

                if ($('#' + ContainerID).highcharts()) {
                    $('#' + ContainerID).highcharts().destroy();

                }

            }

            else {

                series1 = {
                    data: [], startAngle: 30,
                    point: {
                        events: {
                            /* click: function (event) {
                                var detailsDivId
                                
                                 switch (this.fundType) {
                                     case "P": {
                                         switch (this.AssetOrLia) {
                                             case "A": {
                                                 detailsDivId ="AssetMinorChart";
                                                 $("#AssetMinorChart").show();
                                                 $("#AssetChart").hide();
                                                 break;
                                             }
                                             case "L": {
                                                 detailsDivId ="LiaMinorChart";
                                                 $("#LiaMinorChart").show();
                                                 $("#LiaChart").hide();
                                                 break;
                                             }
                                         }
 
                                         break;
                                     }
 
                                     case "A": {
                                         switch (this.AssetOrLia) {
                                             case "A": { break; }
                                             case "L": { break; }
                                         }
 
                                         break;
                                     }
                                     case "M": {
                                         switch (this.AssetOrLia) {
                                             case "A": { break; }
                                             case "L": { break; }
                                         }
                                         break;
                                     }
 
                                 }
                                
                                 GetMinorAssetReliabilityDetails(this.id, this.fundType, this.AssetOrLia, detailsDivId);
 
                                
                             }*/
                        }
                    },
                };

                $.each(data, function (item) {

                    var amt = parseFloat(this.y).toFixed(2)
                    // required  when details chart is to be completed
                    //    series1.data.push({ x: this.x, y: parseFloat(amt), z: parseFloat(this.z), id: this.id, fundType: this.fundType, AssetOrLia: this.AssetOrLia, headCode: this.headCode });
                    series1.data.push({ x: this.x, y: parseFloat(amt), z: parseFloat(this.z) });

                });

                optionsPie = CommonOptions(ContainerID)
                optionsPie.series.push(series1);

                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.legend.group.hide();
                //chart.legend.box.hide();

            }


        }
    });


}

//currently unused
function GetMinorAssetReliabilityDetails(mainHeadId, fundType, assetOrliability, ContainerID) {

    alert(mainHeadId + " " + fundType + " " + assetOrliability + " " + ContainerID);

    $.ajax({
        type: "POST",
        url: '/Accounts/GetAssetliabilityDetailsChart/',
        data:
        {
            'fundType': fundType,
            'assetOrliability': assetOrliability,
            'month': $("#MONTH").val(),
            'year': $("#YEAR").val(),
            'level': function () {
                if (levelId == "5") {
                    return "5";
                }

                else {
                    return "4";
                }
            },
            'lowercode': function () {
                if (levelId == "4") {
                    return "0";
                }

                else {
                    if ($("#rdSRRDA").is(":checked")) {
                        return "0";
                    }
                    else {
                        if ($("#DPIU").val() == 0) {
                            return "0";
                        } else {
                            return $("#DPIU").val();
                        }
                    }
                }
            },
            'ownLower': function () {
                if (levelId == "4") {
                    return "O";
                }
                else {
                    if ($("#rdSRRDA").is(":checked")) {
                        return "O";
                    } else {
                        return "L";
                    }
                }
            },
            'rptid': "2",
            'selectioncode': function () {
                if (levelId == "4" && $("#rdSRRDA").is(":checked")) {
                    return "L";
                }
                else if ($("#DPIU").is(":checked") && levelId == "4" && $("#DPIU").val()) {
                    return "A";
                }
                else {

                    if ($("#DPIU").val() == 0) {
                        return "A";
                    }
                    else {
                        return "L";
                    }

                }
            },
            "mainHeadId": mainHeadId

        },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;

        },
        success: function (data) {


            var series1 = null;

            if (data == "") {

                if ($('#' + ContainerID).highcharts()) {
                    $('#' + ContainerID).highcharts().destroy();

                }
            }

            else {
                /* series1 = {
                    data: [], startAngle: 30,
                    
                };

               $.each(data, function (item) {

                    var amt = parseFloat(this.y).toFixed(2)
                    series1.data.push({ x: this.x, y: parseFloat(amt), z: parseFloat(this.z), id: this.id, fundType: this.fundType, AssetOrliability: this.AssetOrliability });
                });
                */

                optionsColumn = CommonColumnChartOptions(ContainerID)

                $.each(data, function () {

                    var series1 =
                    {
                        name: "",
                        point: {
                            events: {
                                click: function (event) {

                                    // find out the parent and display the pie chart

                                    switch (this.fundType) {
                                        case "P": {
                                            switch (this.AssetOrLia) {
                                                case "A": {
                                                    $("#AssetMinorChart").hide();
                                                    $("#AssetChart").show();
                                                    break;
                                                }
                                                case "L": {
                                                    $("#LiaMinorChart").hide();
                                                    $("#LiaChart").show();
                                                    break;
                                                }
                                            }

                                            break;
                                        }

                                        case "A": {
                                            switch (this.AssetOrLia) {
                                                case "A": { break; }
                                                case "L": { break; }
                                            }

                                            break;
                                        }
                                        case "M": {
                                            switch (this.AssetOrLia) {
                                                case "A": { break; }
                                                case "L": { break; }
                                            }
                                            break;
                                        }

                                    }



                                }
                            }
                        },
                        data: []

                    };

                    series1.name = this.x;
                    // var items = this.y.split(",");
                    series1.data.push(parseFloat(this.y))
                    // $.each(items, function (itemNo, item) {
                    //series1.data.push(parseFloat(item));
                    // });

                    optionsColumn.series.push(series1);

                });

                var chart = new Highcharts.Chart(optionsColumn);
                // code to display animation
                chart.series[0].setVisible(true, true);

            }
        }
    });





}


//Added By Abhishek kamble 24-jan-2014 Start
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Receipt/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
        {
            "Month": month,
            "Year": year
        },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}