$(document).ready(function () {
    //ListPmgsyRoad();

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

        if ($("#BlockID option:selected").val() == 0)
        {
            alert("Please select Block");
            return;
        }


        if ($("#Year option:selected").val() == 0) {
            alert("Please select Year");
            return;
        }

        CloseDetails();
        if ($("#ddlRoadType").val() == "P") {
            
            $("#tbCNRoadList").jqGrid("GridUnload");
            ListPmgsyRoad();
        }
        else {
            //$("#tbPmgsyRoadList").jqGrid("GridUnload");
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

function ListPmgsyRoad() {

    blockPage();
    $("#tbPmgsyRoadList").jqGrid("GridUnload");
    jQuery("#tbPmgsyRoadList").jqGrid({
        url: '/CoreNetwork/GetPmgsyRoadList',
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
        postData: { "IMS_YEAR": $("#ddlYear").val(), "Block": $("#ddlBlock").val(), "Road": $("#ddlRoadType").val(), value: Math.random },
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
        url: '/CoreNetwork/GetCNRoadList',
        datatype: "json",
        mtype: "POST",
        colNames: ["TR/MRL System Id", "TR/MRL Road Name", "Road Number", "DRRP Road Name", "Road Catagory", "Start Chainage", "End Chainage", /*"Length of Road(in Km.)",*/ "Last Entry Made For the Year", "PCI INDEX" , "Status"],
        colModel: [
                    { name: 'CNRoadCode', index: 'RoadCode', width: 100, sortable: false, align: "center", },
                    { name: 'RoadName', index: 'RoadName', width: 300, sortable: false, align: "left" },
                    { name: 'RoadCode', index: 'RoadCode', width: 100, sortable: false, align: "center" },
                    { name: 'DRRP', index: 'DRRP', width: 300, sortable: false, align: "left" },
                    { name: 'RoadCatagory', index: 'RoadCatagory', width: 200, sortable: false, align: "center" },
                    { name: 'StartChainage', index: 'StartChainage', width: 100, sortable: false, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 100, sortable: false, align: "center" },
                    //{ name: 'length', index: 'length', width: 100, sortable: false, align: "center" },
                    { name: 'lastentry', index: 'lastentry', width: 100, sortable: false, align: "center" },
                    { name: 'pci', index: 'pci', width: 120, sortable: false, align: "center" },
                    { name: 'isfinalized', index: 'isfinalized', width: 120, sortable: false, align: "center"  }
        ],
        postData: { "Block": $("#BlockID option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val() },
        pager: jQuery('#tbCNRoadListPager'),
        rowNum: 15,
        rowList: [15, 30, 45],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;TR/MRL Roads",
        height: 'auto',
        width: 'auto',
        sortname: 'RoadName',
        //autowidth: true,
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ["CNRoadCode"],
            groupColumnShow: [false],
            groupText: [
                "<b>TR/MRL System ID : {0}</b>" + "  <input type = 'button'  title='Click here to finalize PCI details' value = 'Finalize' style = 'width:80px; hieght:10px; color:ButtonColor({9})' onclick = 'FinalizePCIRoad({0})' />"//class='ui-icon-unlocked'
            ],
            groupOrder: ["asc"],
            groupSummary: [true],
            groupSummaryPos: ['header'],
            groupCollapse: false
        },
        loadComplete: function (data) {

            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                console.log(data.rows.length);
                if (rowData.cell[8] == 'True') {
                    var checkbox = $("#jqg_tbCNRoadList_" + rowData['id']);
                    checkbox.attr("disabled", false);
                }
                $("#cb_tbCNRoadList").attr("disabled", false);
            }

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
            "<a href='#' style= 'font-size:.9em;'>Add PCI Index For TR/MRL Road</a>" +
            '<a href="#" style="float: right;" onclick="CloseDetails();">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {
        blockPage();

        $("#divPciForm").load('/CoreNetwork/AddPciForCNRoad/' + paramRoadCode, function () {
            $.validator.unobtrusive.parse($('#frmPciForCNRoad'));
            unblockPage();
        });

        $('#divPciForm').show('slow');
    });

    $("#tbPmgsyRoadList").jqGrid('setGridState', 'hidden');
    $("#tbCNRoadList").jqGrid('setGridState', 'hidden');
    $("#tbPciForCNRoad").jqGrid("GridUnload");
    $("#tbPciForCNRoad").trigger("reloadGrid");
}

function FinalizePCIRoad(paramData) {


    var Year = $("#Year option:selected").val();

    if (confirm("Are you sure to finalize PCI Details ?")) {
        $.ajax({
            url: '/CoreNetwork/FinalizePCIRoadDetails?SelectedYear=' + Year,
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.success)
                {
                    $("#tbCNRoadList").trigger("reloadGrid");
                    unblockPage();
                    alert(response.ErrorMessage);
                }
                else {
                    alert(response.ErrorMessage);
                    unblockPage();
                }
            }
        });
    }
}

function ButtonColor(param)
{
    console.log(param);
    if (param == "Y")
        return "green";
    else
        return "red";
}