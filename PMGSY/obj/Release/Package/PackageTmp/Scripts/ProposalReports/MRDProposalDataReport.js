
$(document).ready(function () {
    $("#dvBridgeHeader").click(function () {

        $("#dvBridgeHeader").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvMRDPropBridge").toggle("slow");

    });

    //$("#tb1").find('tr:odd').css("background-color", "lightyellow");
    //alert($('#PMGSY').val());
    if ($('#PMGSY').val() == 1) {
        var oSingleCB = $('#tblBridgeReportData').dataTable({
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
    else {
        var oSingleCB = $('#tblBridgeReportData2').dataTable({
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
});



function GetBridgeTypeDetails(param) {
    
    //alert(param);
    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeTypeDetails?param="+param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeParent").html('');
            $("#dvBridgeParent").html(response);
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
    $("#dvBridgeParent").dialog("open");
}

function GetBridgeEstCostDetails(param) {

    //alert(param);
    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeEstCostDetails?param=" + param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeParent").html('');
            $("#dvBridgeParent").html(response);
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
    $("#dvBridgeParent").dialog("open");
}


function GetBridgeCostDetails(param) {

    //alert(param);
    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeCostDetails?param=" + param,
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmMRDProposal").serialize(),
        success: function (response) {

            $("#dvBridgeParent").html('');
            $("#dvBridgeParent").html(response);
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
    $("#dvBridgeParent").dialog("open");
}


function GetBridgeProposalFilesDetails(param) {

    //alert(p1 + "," + p2 + "," + p3);
    $.ajax({
        url: "/ProposalReports/MRDProposalBridgeFileDetails?param="+param,
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