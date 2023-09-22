$(document).ready(function ()
{
   ListPmgsyRoad();
    
   $("#ddlRoadType").change(function () {
       if ($("#ddlRoadType").val() == "C") {
           $("#tdlblYear").hide('slow');
           $("#tdddlYear").hide('slow');
       }
       else if ($("#ddlRoadType").val() == "P") {
           $("#tdlblYear").show('slow');
           $("#tdddlYear").show('slow');
       }
   });

   $("#btnGo").click(function () {      
       CloseDetails();
       if ($("#ddlRoadType").val() == "P") {
           $("#tbPmgsyRoadList").jqGrid("GridUnload");
           $("#tbCNRoadList").jqGrid("GridUnload");           
           ListPmgsyRoad();
       }
       else {
           $("#tbPmgsyRoadList").jqGrid("GridUnload");
           $("#tbCNRoadList").jqGrid("GridUnload");
           ListCNRoads();
       }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

});

function ListPmgsyRoad(){

    blockPage();
  
    jQuery("#tbPmgsyRoadList").jqGrid({
        url: '/Maintenance/GetPmgsyRoadList',
        datatype: "json",
        mtype: "POST",
        colNames: ["Road Name", "Package Number", "Length of Road(in Km.)", "Last Entry Made For the Year", "PCI INDEX"],
        colModel: [
                    { name: 'RoadName', index: 'RoadName', width: 400, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 200, sortable: false, align: "center" },
                    { name: 'length', index: 'length', width: 200, sortable: false, align: "center" },
                    { name: 'lastentry', index: 'lastentry', width: 200, sortable: false, align: "center" },
                    { name: 'pci', index: 'pci', width: 120, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": $("#ddlYear").val(), "Block": $("#ddlBlock").val(), "Road": $("#ddlRoadType").val() , value: Math.random },
        pager: jQuery('#tbPmgsyRoadListPager'),
        rowNum: 5,
        rowList: [5, 10, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PMGSY Roads",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        sortname: 'RoadName',
        rownumbers: true,
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function ListCNRoads() {
    //$("#tbCNRoadList").jqGrid("GridUnload");

    blockPage();

    jQuery("#tbCNRoadList").jqGrid({
        url: '/Maintenance/GetCNRoadList',
        datatype: "json",
        mtype: "POST",
        colNames: ["Road Name", "Road Code", "Road Catagory", "Start Chainage", "End Chainage", "Length of Road(in Km.)", "Last Entry Made For the Year", "PCI INDEX"],
        colModel: [
                    { name: 'RoadName', index: 'RoadName', width: 300, sortable: false, align: "left" },
                    { name: 'RoadCode', index: 'RoadCode', width: 100, sortable: false, align: "center" },
                    { name: 'RoadCatagory', index: 'RoadCatagory', width: 200, sortable: false, align: "center" },
                    
                    { name: 'StartChainage', index: 'StartChainage', width: 100, sortable: false, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 100, sortable: false, align: "center" },

                    { name: 'length', index: 'length', width: 100, sortable: false, align: "center" },
                    { name: 'lastentry', index: 'lastentry', width: 100, sortable: false, align: "center" },
                    { name: 'pci', index: 'pci', width: 120, sortable: false, align: "center" }
        ],
        postData: { "Block": $("#ddlBlock").val(),  value: Math.random },
        pager: jQuery('#tbCNRoadListPager'),
        rowNum: 5,
        rowList: [5, 10, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Core Network Roads",
        height: 'auto',
        width: 'auto',
        sortname:'RoadName',
        //autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function CloseDetails() {
    $('#accordion').hide('slow');
    $('#divPciForm').hide('slow');
    $("#tbPmgsyRoadList").jqGrid('setGridState', 'visible');
    $("#tbCNRoadList").jqGrid('setGridState', 'visible');    
}

function AddPCIIndexForPmgsyRoad(paramRoadCode) {
    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;'>Add PCI Index For PMGSY Road</a>" +
            '<a href="#" style="float: right;" onclick="CloseDetails();">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {
        blockPage();

        $("#divPciForm").load('/Maintenance/AddPciForPmgsyRoad/' + paramRoadCode, function () {
            $.validator.unobtrusive.parse($('#divPciForm'));
            unblockPage();
        });
        
        $('#divPciForm').show('slow');
    });

    $("#tbPmgsyRoadList").jqGrid('setGridState', 'hidden');
    $("#tbCnRoadList").jqGrid('setGridState', 'hidden');
}

function AddPCIIndexForCNRoad(paramRoadCode) {
    //$("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;'>Add PCI Index For Core Network Road</a>" +
            '<a href="#" style="float: right;" onclick="CloseDetails();">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {
        blockPage();

        $("#divPciForm").load('/Maintenance/AddPciForCNRoad/' + paramRoadCode, function () {
            $.validator.unobtrusive.parse($('#frmPciForCNRoad'));
            unblockPage();
        });

        $('#divPciForm').show('slow');
    });

    $("#tbPmgsyRoadList").jqGrid('setGridState', 'hidden');
    $("#tbCNRoadList").jqGrid('setGridState', 'hidden');
}

