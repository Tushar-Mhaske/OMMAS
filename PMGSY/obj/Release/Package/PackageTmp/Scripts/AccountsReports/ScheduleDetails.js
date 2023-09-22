
$(document).ready(function () {


    //alert($("#HeadCode").val());
    //$('#tblScheduleDetails').jqGrid({

    //    url: '/AccountsReports/GetScheduleDetails',
    //    datatype: "json",
    //    mtype: "POST",
    //    postData: { "AdminNdCode": $("#Piu").val(), "Month": $("#Month").val(), "Year": $("#Year").val(), "Head": $("#HeadCode").val(), "Agency": $("#FundingAgency").val() },
    //    colNames: ["FACode","Agency Name", "Duration","Flag", "Completion Status", "Phase", "Package Number","Road Code", "Road Name", "Amount Inner Column 1", "Inner Column 2 <br/>(sub-total of inner col.1)", "Outer Column <br/>(sub-total of inner col.2)"],
    //    //colNames: ['Name Of PIU', 'April', 'May', 'June', 'July', 'Augest', 'September', 'October', 'November', 'December', 'January' , 'February' , 'March' ],
    //    colModel: [
    //                        { name: 'FACode', index: 'FACode', height: 'auto', width: '120px', align: "left", sortable: false,hidden:true },
    //                        { name: 'Agency', index: 'Agency', height: 'auto', width: '120px', align: "left", sortable: false, summaryType: 'sum', summaryTpl: 'Phase Total: {0}' },
    //                        { name: 'Duration', index: 'Duration', height: 'auto', width: '90px', align: "left", sortable: false, hidden: true, summaryType: 'sum', summaryTpl: 'Phase Total: {0}' },
    //                        { name: 'Flag', index: 'Flag', height: 'auto', width: '90px', align: "left", sortable: false,hidden:true },
    //                        { name: 'Status', index: 'Status', height: 'auto', width: '120px', align: "right", sortable: false, hidden: true },
    //                        { name: 'Phase', index: 'Phase', height: 'auto', width: '120px', align: "right", sortable: false, summaryType: 'sum', summaryTpl: 'Phase Total: {0}' },
    //                        { name: 'Package', index: 'Package', height: 'auto', width: '120px', align: "right", sortable: false, },
    //                        { name: 'Code', index: 'Code', height: 'auto', width: '120px', align: "right", sortable: false,hidden:true },
    //                        { name: 'Road', index: 'Road', height: 'auto', width: '120px', align: "right", },
    //                        { name: 'AmountColf', index: 'AmountColf', height: 'auto', width: '120px', align: "right", formatter: 'number', summaryType: 'sum',summaryTpl: 'Package Total: {0}' },
    //                        { name: 'AmountColS', index: 'AmountColS', height: 'auto', width: '120px', align: "right", formatter: 'number', summaryType: 'sum', summaryTpl: 'Phase Total: {0}' },
    //                        { name: 'AmountColL', index: 'AmountColL', height: 'auto', width: '120px', align: "right"},

    //    ],

    //    pager: jQuery('#dvPgrScheduleDetails'),
    //    pginput: false,
    //    pgbuttons: false,
    //    rowNum: 999999999,
    //    viewrecords: true,
    //    recordtext: '{2} records found',       
    //    height: 'auto',
    //    autowidth: true,
    //    //width:'100%',
    //    sortname: 'Package',
    //    rownumbers: true,
    //    hidegrid: true,
    //    //footerrow: true,
    //    userDataOnFooter: true,
    //    caption: "Schedule Of Road",
    //    grouping: true,
    //    groupingView: {            
    //        groupField: ['Agency', 'Duration', 'Phase', 'Package'],
    //        groupSummary : [false, false,true,true],
    //        groupColumnShow: [false, false, false, false],
    //        groupCollapse:false

           
    //    },
    //    loadComplete: function (data) {

            


    //    }
    //});

    var fundTransfer = $('#tblSchedule').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
        "bSort": false,
        "bHeader": true,
        "sScrollY": "420px",
        "bPaginate": false,
        "bScrollInfinite": true,
        "bScrollCollapse": true,
        "sDom": '<"H"Tfr>t<"F"ip>',
        //"oTableTools": {
        //    "aButtons": [
        //		{
        //		    "sExtends": "pdf",
        //		    "sPdfOrientation": "landscape",
        //		    "sTitle": $("#rptMAAnnex").find('td:eq(1)').text(),
        //		    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
        //		    //"sPdfMessage": pdfMessage,
        //		    // "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
        //		    "sFileName": $("#Year").val() + "," + $("#ddlBalance").val() + ".pdf"
        //		},
        //        {
        //            "sExtends": "xls",
        //            "bBomInc": true,
        //            "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
        //            "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
        //        }

        //    ]
        //},
        "oTableTools": {
            "aButtons": []
        },
        //"fnRowCallback": function (nRow, aData, iDisplayIndex) {
        //    if ($("#TotalRecord").val() > 0) {
        //        $("td:first", nRow).html(iDisplayIndex + 1);
        //        return nRow;
        //    }
        //}


    });

    $("#tblSchedule_info").html('');
    $("#tblSchedule_info").html('Showing 1 of'+ $("#Count").val()+ '&nbsp;Records');

});