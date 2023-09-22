/// <reference path="../jquery-1.9.1-vsdoc.js" />

$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmticketreport");

    //accordian congiguration
    $("#accordion").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });

    if ($("input[name='Level']:checked").val() == "0") {
        $(".state").hide();
    }
    else {
        $(".state").show();
    }

    $("input[name='Level']").change(function () {
        var level =$("input[name='Level']:checked").val();
        if (level == "0") {             //all
            $(".state").hide();
            $("#ddlDesignation").empty();
            $("#ddlDesignation").append("<option value='0'>All</option>");
        } else if (level == "2")   //national
        {
            getDesignation(level);
            $(".state").hide();
        }
        else{                     // statewise
            getDesignation(level);
            $(".state").show();
        }
    });

    $('#spCollapseIconTktRpt').click(function () {
        $("#spCollapseIconTktRpt").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
       
        $("#dvTktReportingMain").slideToggle('slow');
    });

    $('#imgCloseRptDetails').click(function () {
       // $('#dvTktRptNote').hide('slow');
    });
 
    $("#btnViewTktRpt").click(function () {
        getStatisticDetails();
        getTicketReportList();
    });
    getTicketReportList();
});

function getDesignation(level)
{

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/Ticket/GetDesignation',
        type: 'POST',
        cache: false,
        async: true,
        data: {Level : level},
        success: function (data) {
            
            $("#ddlDesignation").empty();
            $.each(data, function (index, value) {
                $("#ddlDesignation").append("<option value=" + value.Value + ">" + value.Text + "</option>");
            });
        },
        complete: function () {
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            return;
        } 
    });

}


function getStatisticDetails()
{

    if (jQuery("#frmticketreport").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: '/Ticket/GetStatisticDetails',
            type: 'POST',
            cache: false,
            async: true,
            data: $("#frmticketreport").serialize(),
            success: function (data) {
                $("#totalticket").html(data.TotalTicket == null ? 0 : data.TotalTicket);
                $("#totalapprovedtkt").html(data.TotalApprovedTicket == null ? 0 : data.TotalApprovedTicket);
                $("#totalclosedtkt").html(data.TotalClosedTicket == null ? 0 : data.TotalClosedTicket);
                $("#totalnotclosedtkt").html(data.TotalNotClosedTicket == null ? 0 : data.TotalNotClosedTicket);
                $("#partialclosed").html(data.PartialClosedTicket == null ? 0 : data.PartialClosedTicket);
                $("#inprogress").html(data.InProgressTicket == null ? 0 : data.InProgressTicket);
                $("#openedtkt").html(data.OpenedTicket == null ? 0 : data.OpenedTicket);
                $("#notopened").html(data.NotOpenedTicket == null ? 0 : data.NotOpenedTicket);
            },
            complete: function () {
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert("Error occured while processing your request.");
            }
        });

    }
}

function getTicketReportList() {


    var level = $("input[name='Level']:checked").val();
    var designation = $("#ddlDesignation option:selected").val();
    var category = $("#ddlCategory option:selected").val();
    var module = $("#ddlModule option:selected").val();
    var state = $("#ddlState option:selected").val();

    $("#tbTktReportList").jqGrid('GridUnload');
    $("#tbTktReportList").jqGrid({
        url: '/Ticket/GetTicketReportList',
        mtype: "POST",
        datatype: "json",
        colNames: ['Ticket No.', 'State', 'Category', 'Module', 'Subject', 'Reported By', 'Reported Date', 'Approval Date', 'First Forwarded To', 'Status', 'Currently Pending At', 'Close Date', 'Pending since(days)', 'View'],
        colModel: [
            { name: 'TicketNo', index: "TicketNo", key: true, width: 80, hidden: false, search: true, align: 'center' },
            { name: 'State', index: "State", width: 130, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Category', index: "Category", width: 100, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Module', index: "Module", width: 100, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Subject', index: "Subject", width: 190, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'ReportedBy', index: "ReportedBy", width: 160, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'ReportedDate', index: "ReportedDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'ApprovalDate', index: "ApprovalDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'ForwardTo', index: "ForwardTo", width: 120, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'Status', index: "Status", width: 120, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'PendingAt', index: "PendingAt", width: 110, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'CloseDate', index: "CloseDate", width: 110, sortable: false, resizable: false, search: false, align: 'center' },
            { name: 'Pendingdays', index: "Pendingdays", width: 90, sortable: false, resizable: false, search: true, align: 'center' },
            { name: 'View', index: "View", width: 70, sortable: false, resizable: false, search: false, align: 'center' },
        ],
        //  loadonce: true,
        postData: { Level: level, Designation: designation, Category: category, Module: module, State: state },
        pager: "#pgTktReportList",
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
        }
    });
    $("#tbTktReportList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true, beforeSearch: function () {

            var level = $("input[name='Level']:checked").val();
            var designation = $("#ddlDesignation option:selected").val();
            var category = $("#ddlCategory option:selected").val();
            var module = $("#ddlModule option:selected").val();
            var state = $("#ddlState option:selected").val();
            jQuery("#tbTktReportList").jqGrid("setGridParam", { postData: { Level: level, Designation: designation, Category: category, Module: module, State: state } });
        }
    });
};


function ViewTicket(urlparameter) {
    $("#tbTktReportList").jqGrid("setGridState", "hidden");

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" + 'View Ticket' +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseTicketDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        var scrolled = 0;
        $(this).css("width", "72%")
        $("#dvticketlayout").load("/Ticket/ViewTicketDetails/" + urlparameter, function () {
            $.validator.unobtrusive.parse("#frmAcceptticket");
            $(this).show('slow');
          
            scrolled = $("#dvticketlayout").height();// scrolled + 300;
                $("#mainDiv").animate({
                    scrollTop: scrolled
                });
        });
    });
   
}


function CloseTicketDetails() {
    $('#accordion').hide('slow');
    $("#tbTktReportList").jqGrid("setGridState", "visible");
}