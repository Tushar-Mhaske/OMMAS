
$(document).ready(function () {
    
    //$("#b1").find('tr:even').css("background-color", "lightyellow");
    //$("div").removeClass("ui-state-default");
    //$("table").not('#tblRoadReportData1').removeClass(".ui-state-default");
    //$(".ui-state-default").removeClass();
    //$("#tblRoadReportData1").find('th').addClass("CBHeader");
    //$(".ui-state-default.dataTable.DTFC_Cloned").width("585px");
    //$('table[class^=ui-state-default dataTable]').css("margin-left", "20px");
    
    //$("ui-state-default.dataTable").css("margin-left", "20px");
    
    //$("table").find("ui-state-default.dataTable").css("background-color", "red");
    //$("div.dataTables_scroll").find('table.ui-state-default,table.dataTable').css("background-color", "red"); //({ marginLeft: "-20px" });
    //$("div.dataTables_scroll table:nth-child(3)").css("background-color","red");
    //$(".dataTables_scrollHeadInner").children('table.ui-state-default,table.dataTable :first').css("background-color", "red");
    
    
    $.ajax({
        url: "/ProposalReports/MRDProposalHabCoverage/",
        cache: false,
        type: "POST",
        async: false,
        data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvHabCoverage").html('');
            $("#dvHabCoverage").html(response);
        },
        error: function () {
            alert("error");
        }
    })

    //$(".dataentry").find('td:even').css('text-align', 'right');
    //$(".dataentry").find('td:odd').css('text-align', 'left');
    $("#idheaderDiv").click(function () {

        $("#idheaderDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvMRDPropRoad").toggle("slow");
        $("#dvHabCoverage").toggle("slow");
    });

    //alert($('#PMGSY').val());
    if ($('#PMGSY').val() == 1) {
        var oSingleCB = $('#tblRoadReportData1').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
            "sScrollX": "100%",
            
            "bPaginate": false,
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            "oTableTools": {
                "aButtons": []
            },
            "bAutoWidth": false, // Disable the auto width calculation 
            //"aoColumns": [
            //  { "sWidth": "34px" }, // 1st column width 
            //  { "sWidth": "68px" }, // 2nd column width 
            //  { "sWidth": "75px" }, // 3rd column width and so on 
            //  { "sWidth": "45px" }, // 4th column width and so on 
            //  { "sWidth": "106px" },// 5th column width and so on 
            //  { "sWidth": "62px" }, // 6th column width and so on 
            //  { "sWidth": "31px" }, // 7th column width and so on 
            //  { "sWidth": "45px" }, // 8th column width and so on 
            //  { "sWidth": "50px" }, // 9th column width and so on 
            //  { "sWidth": "71px" }, // 10th column width and so on 
            //  { "sWidth": "64px" }, // 11th column width and so on 
            //  { "sWidth": "80px" }, // 12th column width and so on 
            //  { "sWidth": "61px" }, // 13th column width and so on 
            //  { "sWidth": "61px" }, // 14th column width and so on 
            //  { "sWidth": "39px" }, // 15th column width and so on 
            //  { "sWidth": "39px" }, // 16th column width and so on 
            //  { "sWidth": "39px" }, // 17th column width and so on 
            //  { "sWidth": "56px" }, // 18th column width and so on 
            //  { "sWidth": "45px" }, // 19th column width and so on 
            //  { "sWidth": "44px" }, // 20th column width and so on 
            //  { "sWidth": "56px" }, // 21st column width and so on 
            //  { "sWidth": "45px" }, // 22nd column width and so on 
            //  { "sWidth": "60px" }, // 23rd column width and so on 
            //  { "sWidth": "56px" }, // 24th column width and so on 
            //  { "sWidth": "49px" }, // 25th column width and so on 
            //  { "sWidth": "46px" }, // 26th column width and so on 
            //  { "sWidth": "49px" }, // 27th column width and so on 
            //  { "sWidth": "45px" }, // 28th column width and so on 
            //  { "sWidth": "72px" }, // 29th column width and so on 
            //  { "sWidth": "66px" }, // 30th column width and so on 
            //  { "sWidth": "66px" }, // 31st column width and so on 
            //  { "sWidth": "60px" }, // 32nd column width and so on 
            //  { "sWidth": "46px" }, // 33rd column width and so on 
            //  { "sWidth": "52px" }, // 34th column width and so on 
            //  { "sWidth": "69px" }, // 35th column width and so on 
            //  { "sWidth": "72px" }, // 36th column width and so on 
            //  { "sWidth": "26px" }, // 37th column width and so on 
            //  { "sWidth": "31px" }, // 38th column width and so on 
            //  { "sWidth": "35px" }, // 39th column width and so on 
            //  { "sWidth": "67px" }, // 40th column width and so on 
            //  { "sWidth": "62px" }, // 41st column width and so on 
            //  { "sWidth": "48px" }, // 42nd column width and so on 
            //  { "sWidth": "48px" }, // 43rd column width and so on 
            //  { "sWidth": "56px" }, // 44th column width and so on 
            //  { "sWidth": "47px" }, // 45th column width and so on 
            //  { "sWidth": "47px" }, // 46th column width and so on 
            //  { "sWidth": "59px" }, // 47th column width and so on 
            //  { "sWidth": "51px" }, // 48th column width and so on 
            //]
        });
        //new $.fn.dataTable.FixedColumns(oSingleCB);
        var oFC = new $.fn.dataTable.FixedColumns(oSingleCB, {
            "iLeftColumns": 11,
            //leftColumns: 2
            //"sHeightMatch": "auto",
            //"sWidthMatch": "auto",
            //"iLeftWidth": 405,
            //"sScrollX": "100%"
        });
        //oSingleCB.fnAdjustColumnSizing();
    }
    else {
        var oSingleCB = $('#tblRoadReportData2').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
            "sScrollX": "100%",
            "bPaginate": false,
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            "oTableTools": {
                "aButtons": []
            },
        });
    }

    $("#dvBridgeParent").dialog({
        autoOpen: false,

        width: 920,
        modal: true,
        show: {
            effect: "blind",
            duration: 900
        },
        hide: {
            effect: "explode",
            duration: 1000
        }

    });

    $("#dvBridgeFile").dialog({
        autoOpen: false,
        height: 550,
        width: 920,
        modal: true,
        show: {
            effect: "blind",
            duration: 900
        },
        hide: {
            effect: "explode",
            duration: 1000
        }

    });
    
    $('.ui-state-default.dataTable').css("marginLeft", "-20px");
    $('#tblRoadReportData1').css("marginLeft", "-20px");
    $('.ui-state-default.dataTable.DTFC_Cloned').css("marginLeft", "0px");
    $('.ui-state-default.dataTable.ui-corner-all').css("marginLeft", "0px");
    //$("table.ui-state-default, table.dataTable").css({ marginLeft: "-20px" });
});

function GetRoadCBRDetails(param) {

    $.ajax({
        url: "/ProposalReports/MRDRoadCBRDetails?param=" + param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeParent").html('');
            $("#dvBridgeParent").html(response);
        },
        error: function () {
            alert("error");
        }
    })
    $("#dvBridgeParent").dialog("open");
}

function GetBridgeCostDetails(param) {

    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeCostDetails?param=" + param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeParent").html('');
            $("#dvBridgeParent").html(response);
        },
        error: function () {
            alert("error");
        }
    })
    $("#dvBridgeParent").dialog("open");
}

function GetBridgeProposalFilesDetails(param) {

    //alert(p1 + "," + p2 + "," + p3);
    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeFileDetails?param=" + param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeFile").html('');
            $("#dvBridgeFile").html(response);
            //alert($("#loadReport").html());
            //if (data.message == true) {

            //}
            //else {
            //    if (data.message != false) {

            //    }
            //    else {
            //        alert("Error occured");
            //    }
            //}
        },
        error: function () {
            alert("error");
        }
    })
    $("#dvBridgeFile").dialog("open");
}

