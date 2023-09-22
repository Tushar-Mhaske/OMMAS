/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />
var rowsToColor = [];
var rowsToColorAll = [];
$(document).ready(function () {

    $("#accordion").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });
    $("#btnAdd").click(function () {
        LoadAddTicket();
    });

    DisplayPendingLicketList();

    DisplayAllLicketList();
});



function CloseTicketDetails() {
    $('#accordion').hide('slow');
    $('#dvticketlayout').hide('slow');
    $("#tbTicketList").jqGrid("setGridState", "visible");
    $("#tbAllTicketList").jqGrid("setGridState", "visible");
}


function DisplayPendingLicketList()
{
   
    $("#tbTicketList").jqGrid('GridUnload');
    $("#tbTicketList").jqGrid({
        url: '/Ticket/GetTicketList',
        mtype: "POST",
        datatype: "json",
        colNames: ['Ticket No.', 'NQM/SQM/State', 'Category', 'Module', 'Subject', 'Reported By', 'Reported Date', 'Approval Date', 'First Forwarded To','Status', 'Currently Pending At', 'Upload File', 'View', 'Reply','Delete'],
        colModel: [
            { name: 'TicketNo', index: "TicketNo", key: true, width: 70, hidden: false,search:false,align:'center' },
            { name: 'State', index: "State", width: 130, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Category', index: "Category", width: 100, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'Module', index: "Module", width: 100, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'Subject', index: "Subject", width: 190, sortable: false, resizable: false, search:false,align: 'center' },
            { name: 'ReportedBy', index: "ReportedBy", width: 160, sortable: false, resizable: false,search:false, align: 'center' },
            { name: 'ReportedDate', index: "ReportedDate", width: 110, sortable: false, resizable: false,search:false, align: 'center' },
            { name: 'ApprovalDate', index: "ApprovalDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'ForwardTo', index: "ForwardTo", width: 140, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'Status', index: "Status", width: 120, sortable: false, resizable: false, formatter: rowColorFormatter, search: false, align: 'center' },
            { name: 'PendingAt', index: "PendingAt", width: 140, sortable: false, resizable: false,search:false, align: 'center' },
            { name: 'Upload', index: "Upload", width: 70, sortable: false, resizable: false,search:false, align: 'center' },
            { name: 'View', index: "View", width: 70, sortable: false, resizable: false,search:false, align: 'center' },
            { name: 'Reply', index: "Reply", width: 70, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'Delete', index: "Delete", width: 70, sortable: false, resizable: false, search: false, align: 'center' },
        ],
        //  loadonce: true,
        postData: {},
        pager: "#dvTicketPager",
        width: 'auto',
        height: 'auto',
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;Pending Ticket List",
        sortname: "TicketNo",
        rownumbers: true,
        //  autowidth: true,
        shrinkToFit: true,
        //forceFit: true,
        loadComplete: function (data) {
            $("#gs_State").attr("placeholder", "  search here...")

           // var userRole = jQuery("#role").val().toLowerCase();

            //if (data.userTyepe != "R") {
            //    $("#tbTicketList").jqGrid("hideCol", ["Reply"]);
            //}
           
            if (data.userTyepe=="R")
            {
                $("#tbTicketList").jqGrid("hideCol", ["Upload"]);
            }
            
            if (data.userTyepe == "R")
            {
                $("#tbTicketList").jqGrid("hideCol", ["View"]);
            }

            //if (userRole == "srrda" || userRole == "nrrda")
            //{
            //    $("#tbTicketList").jqGrid("showCol", ["Reply"]);
            //}

            for (var i = 0; i < rowsToColor.length; i++) {
              
                var status = $("#" + rowsToColor[i]).find("td").eq(10).html();
               
                 if (status == "Opened") {
                     $("#" + rowsToColor[i]).find("td").eq(10).css("background-color", "#ffebcc");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("color", "black");
                }
                if (status == "in-progress") {
                    $("#" + rowsToColor[i]).find("td").eq(10).css("background-color", "orange");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("color", "white");
                }
                if (status == "Partially Closed") {
                    $("#" + rowsToColor[i]).find("td").eq(10).css("background-color", "lightgreen ");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("color", "white");
                }
                if (status == "Closed") {
                    $("#" + rowsToColor[i]).find("td").eq(10).css("background-color", "#d9d9d9");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#" + rowsToColor[i]).find("td").eq(10).css("color", "black");
                }
            }
            rowsToColor.splice(0, rowsToColor.length);
        }
    });
   /// $("#tbTicketList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
};



function DisplayAllLicketList() {

    $("#tbAllTicketList").jqGrid('GridUnload');
    $("#tbAllTicketList").jqGrid({
        url: '/Ticket/GetAllTicketList',
        mtype: "POST",
        datatype: "json",
        colNames: ['Ticket No.', 'NQM/SQM/State', 'Category', 'Module', 'Subject', 'Reported By', 'Reported Date', 'Approval Date', 'First Forwarded To', 'Status', 'Currently Pending At', 'Closing Date', 'View'],
        colModel: [
            { name: 'TicketNo', index: "TicketNo", key: true, width: 70, hidden: false, search: false, align: 'center' },
            { name: 'State', index: "State", width: 130, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Category', index: "Category", width: 100, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Module', index: "Module", width: 130, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Subject', index: "Subject", width: 190, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'ReportedBy', index: "ReportedBy", width: 160, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'ReportedDate', index: "ReportedDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'ApprovalDate', index: "ApprovalDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'ForwardTo', index: "ForwardTo", width: 110, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Status', index: "Status", width: 120, sortable: false, resizable: false, formatter: AllTktrowColorFormatter, search: true, align: 'center' },
            { name: 'PendingAt', index: "PendingAt", width: 110, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'CloseDate', index: "CloseDate", width: 110, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'View', index: "View", width: 70, sortable: false, resizable: false, search: false, align: 'center' },
        ],
        //  loadonce: true,
        postData: {},
        pager: "#dvAllTicketPager",
        width: 'auto',
        height: 'auto',
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;All Ticket List",
        sortname: "TicketNo",
        rownumbers: true,
        //  autowidth: true,
        shrinkToFit: true,
        //forceFit: true,
        loadComplete: function (data) {
            $("#gs_State").attr("placeholder", "  search here...")

            var userRole = jQuery("#role").val().toLowerCase();
            /*
            if (data.userTyepe != "R") {
                $("#tbTicketList").jqGrid("hideCol", ["Reply"]);
            }

            if (data.userTyepe == "R") {
                $("#tbTicketList").jqGrid("hideCol", ["Upload"]);
            }

            if (data.userTyepe == "R") {
                $("#tbTicketList").jqGrid("hideCol", ["View"]);
            }

            if (userRole == "srrda") {
                $("#tbTicketList").jqGrid("showCol", ["Reply"]);
            }
            */
            console.log("total" + rowsToColorAll)

            for (var i = 0; i < rowsToColorAll.length; i++) {

                var status = $("#" + rowsToColorAll[i]).find("td").eq(10).html();
                if (status == "Opened") {

                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("background-color", "#ffebcc");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("color", "black");
                }
                if (status == "in-progress") {
                 
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("background-color", "orange");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("color", "white");
                }
                if (status == "Partially Closed") {
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("background-color", "lightgreen ");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("color", "white");
                }
                if (status == "Closed") {
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("background-color", "#d9d9d9");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("font-weight", "bold");
                    $("#tbAllTicketList #" + rowsToColorAll[i]).find("td").eq(10).css("color", "black");
                }
            }
            rowsToColorAll.splice(0, rowsToColorAll.length);
        }
    });
    $("#tbAllTicketList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
};
function LoadAddTicket()
{
    $("#tbTicketList").jqGrid("setGridState", "hidden");
    $("#tbAllTicketList").jqGrid("setGridState", "hidden");

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" +'Add Ticket'+
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseTicketDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        $("#dvticketlayout").load("/Ticket/AddTicket", function () {
            $(this).show('slow');
        });
    });
}

function rowColorFormatter(cellValue, options, rowObject) {
   
    if (cellValue != "-")
        rowsToColor[rowsToColor.length] = options.rowId;
    return cellValue;
}


function AllTktrowColorFormatter(cellValue, options, rowObject) {

    if (cellValue != "-")
        rowsToColorAll[rowsToColorAll.length] = options.rowId;
     
    return cellValue;
}

function UploadFile(urlparameter)
{
    $("#tbTicketList").jqGrid("setGridState", "hidden");
    $("#tbAllTicketList").jqGrid("setGridState", "hidden");

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" + 'Upload File' +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseTicketDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        $(this).css("width", "60%")
        $("#dvticketlayout").load("/Ticket/TicketFileUpload/" + urlparameter, function () {
            $(this).show('slow');
        });
    });
}

function ViewAndAcceptTicket(urlparameter)
{
    $("#tbTicketList").jqGrid("setGridState", "hidden");
    $("#tbAllTicketList").jqGrid("setGridState", "hidden");

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" + 'View Ticket' +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseTicketDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        $(this).css("width", "72%")
        $("#dvticketlayout").load("/Ticket/ViewTicketDetails/" + urlparameter, function () {
            $.validator.unobtrusive.parse("#frmAcceptticket");
            $(this).show('slow');
        });
    });


}


function ReplyDetails(urlparameter)
{
    $("#tbTicketList").jqGrid("setGridState", "hidden");
    $("#tbAllTicketList").jqGrid("setGridState", "hidden");

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" + 'Ticket Reply' +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseTicketDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        $(this).css("width", "72%")
        $("#dvticketlayout").load("/Ticket/ReplyTicketDetails/" + urlparameter, function () {
            $.validator.unobtrusive.parse("#frmReplyticket");
            $(this).show('slow');
        });
    });

}


function DeleteTicketMaster(urlparameter) {
    
    if (confirm("Do you want to delete ticket ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/Ticket/DeleteTicketById/' + urlparameter,
            datatype: "json",
            type: 'GET',
            success: function (response) {
                if (response) {
                    alert(response.message);
                    DisplayPendingLicketList();
                    DisplayAllLicketList();
                }
                $.unblockUI();
            },
            error: function () {
                alert('Error occured while processing your request');
                DisplayPendingLicketList();
                DisplayAllLicketList();
                $.unblockUI();
                return false;
            },
        });
    }


}