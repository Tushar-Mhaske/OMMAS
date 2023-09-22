

$(document).ready(function () {

    //var fundTransfer = $('#tblAbstractFund').dataTable({
    //    "bJQueryUI": true,
    //    "bFilter": false,
    //    "bSort": false,
    //    "bHeader": true,
    //    "sScrollY": "220px",
    //    "bPaginate": false,
    //    "bScrollInfinite": true,
    //    "bScrollCollapse": true,
    //    "sDom": '<"H"Tfr>t<"F"ip>',
    //    "oTableTools": {
    //        "aButtons": [
    //    		{
    //    		    "sExtends": "pdf",
    //    		    "sPdfOrientation": "landscape",
    //    		    "sTitle": $("#rptMAAnnex").find('td:eq(1)').text(),
    //    		    "sButtonText": "<img src='../../content/images/Reader.gif' alt='pdf' style='margin-right:20px'></img>",
    //    		    //"sPdfMessage": pdfMessage,
    //    		    // "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".pdf"
    //    		    "sFileName": $("#Year").val() + "," + $("#ddlBalance").val() + ".pdf"
    //    		},
    //            {
    //                "sExtends": "xls",
    //                "bBomInc": true,
    //                "sButtonText": "<img src='../../content/images/Excel.gif' alt='Excel'></img>",
    //                "sFileName": "MonthlyAccount" + $('#Month').children('option').filter(':selected').text() + "-" + $("#Year").val() + ".xls"
    //            }

    //        ]
    //    },
    //    "oTableTools": {
    //        "aButtons": []
    //    },
    //    "fnRowCallback": function (nRow, aData, iDisplayIndex) {
    //        if ($("#TotalRecord").val() > 0) {
    //            $("td:first", nRow).html(iDisplayIndex + 1);
    //            return nRow;
    //        }
    //    }


    // });

    $("#spCollapseIconS").click(function () {
        $("#spCollapseIconS").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvFundTransfer").slideToggle("slow");
    });

    var Year = $("#Year").val();
    var NextYear =parseInt($("#Year").val()) + 1;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tblAbsFundTransfer').jqGrid({

        url: '/AccountsReports/GetAbstractFundDetails',
        datatype: "json",
        mtype: "POST",
        postData: { "Year": Year,"Head":$("#Head").val(), "State": $("#State").val(),"DPIU":$("#DPIU").val() },
        colNames: ['Name Of PIU', 'Ledger Folio', 'April &nbsp;' + Year + '', 'May &nbsp;' + Year + '', 'June &nbsp;' + Year + '', 'July &nbsp;' + Year + '', 'Augest &nbsp;' + Year + '', 'September &nbsp;' + Year + '', 'October &nbsp;' + Year + '', 'November &nbsp;' + Year + '', 'December &nbsp;' + Year + '', 'January &nbsp;' + NextYear + '', 'February &nbsp;' + NextYear + '', 'March &nbsp;' + NextYear + ''],
        //colNames: ['Name Of PIU', 'April', 'May', 'June', 'July', 'Augest', 'September', 'October', 'November', 'December', 'January' , 'February' , 'March' ],
        colModel: [ 
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: '120px', align: "left", sortable: false },
                            { name: 'Ledger', index: 'Ledger', height: 'auto', width:'90px', align: "left", sortable: false },
                            { name: 'AprAmount', index: 'AprAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: {decimalPlaces:2} },
                            { name: 'MayAmount', index: 'MayAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'JuneAmount', index: 'JuneAmount', height: 'auto', width:'120px', align: "right",  formatter: "number", formatoptions: {decimalPlaces:2}},
                            { name: 'JulyAmount', index: 'JulyAmount', height: 'auto', width: '120px', align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'AugAmount', index: 'AugAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'SeptAmount', index: 'SeptAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'OctAmount', index: 'OctAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'NovAmount', index: 'NovAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'DecAmount', index: 'DecAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'JanAmount', index: 'JanAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'FebAmount', index: 'FebAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },
                            { name: 'MarAmount', index: 'MarAmount', height: 'auto', width: '120px', align: "right", sortable: false, formatter: "number", formatoptions: { decimalPlaces: 2 } },

        ],

        pager: jQuery('#dvpgrAbsFundTransfer'),
        pginput: false,
        pgbuttons: false,
        rowNum: 999999999,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AprAmount',
        sortorder: "asc",
        height: 'auto',
        autowidth:true,
        //width:'100%',      
        rownumbers: true,
        hidegrid: true,
        footerrow: true,
        userDataOnFooter: true,
        caption: "Abstract of Fund Transferred",       
        loadComplete: function (data) {

            var recordCount = jQuery('#tblAbsFundTransfer').jqGrid('getGridParam', 'reccount');

            if (recordCount > 25) {
                $('#tblAbsFundTransfer').jqGrid('setGridHeight', '320');
            }
            else {
                $('#tblAbsFundTransfer').jqGrid('setGridHeight', 'auto');
            }
            $("#tblAbsFundTransfer").footerData('set', { "ADMIN_ND_NAME": "Total" }, true); //set footer data
            var grid = $("#tblAbsFundTransfer"),
            aprSum = grid.jqGrid('getCol', 'AprAmount', false, 'sum');
            maySum = grid.jqGrid('getCol', 'MayAmount', false, 'sum');
            junSum = grid.jqGrid('getCol', 'JuneAmount', false, 'sum');
            julySum = grid.jqGrid('getCol', 'JulyAmount', false, 'sum');
            augSum = grid.jqGrid('getCol', 'AugAmount', false, 'sum');
            sepSum = grid.jqGrid('getCol', 'SeptAmount', false, 'sum');
            octSum = grid.jqGrid('getCol', 'OctAmount', false, 'sum');
            novSum = grid.jqGrid('getCol', 'NovAmount', false, 'sum');
            decSum = grid.jqGrid('getCol', 'DecAmount', false, 'sum');
            janSum = grid.jqGrid('getCol', 'JanAmount', false, 'sum');
            febSum = grid.jqGrid('getCol', 'FebAmount', false, 'sum');
            marSum = grid.jqGrid('getCol', 'MarAmount', false, 'sum');



            

            //if (aprSum % 1 == 0) {                
            //    aprSum = aprSum + ".00";
            //}
            //if (maySum % 1 == 0) {
            //    maySum = maySum + ".00";
            //}
            //if (junSum % 1 == 0) {
            //    junSum = junSum + ".00";
            //}
            //if (julySum % 1 == 0) {
            //    julySum = julySum + ".00";
            //}
            //if (augSum % 1 == 0) {
            //    augSum = augSum + ".00";
            //}
            //if (sepSum % 1 == 0) {
            //    sepSum = sepSum + ".00";
            //}
            //if (octSum % 1 == 0) {
            //    octSum = octSum + ".00";
            //}
            //if (novSum % 1 == 0) {
            //    novSum = novSum + ".00";
            //}
            //if (decSum % 1 == 0) {
            //    decSum = decSum + ".00";
            //}
            //if (janSum % 1 == 0) {
            //    janSum = janSum + ".00";
            //}
            //if (febSum % 1 == 0) {
            //    febSum = febSum + ".00";
            //}
            //if (marSum % 1 == 0) {
            //    marSum = marSum + ".00";
            //}               
            
            grid.jqGrid('footerData', 'set', { ID: 'AprTotal:', AprAmount: aprSum });
            grid.jqGrid('footerData', 'set', { ID: 'MayTotal:', MayAmount: maySum });
            grid.jqGrid('footerData', 'set', { ID: 'JunTotal:', JuneAmount: junSum });
            grid.jqGrid('footerData', 'set', { ID: 'JulyTotal:', JulyAmount: julySum });
            grid.jqGrid('footerData', 'set', { ID: 'AugTotal:', AugAmount: augSum });
            grid.jqGrid('footerData', 'set', { ID: 'SepTotal:', SeptAmount: sepSum });
            grid.jqGrid('footerData', 'set', { ID: 'OctTotal:', OctAmount: octSum });
            grid.jqGrid('footerData', 'set', { ID: 'NovTotal:', NovAmount: novSum });
            grid.jqGrid('footerData', 'set', { ID: 'DecTotal:', DecAmount: decSum });
            grid.jqGrid('footerData', 'set', { ID: 'JanTotal:', JanAmount: janSum });
            grid.jqGrid('footerData', 'set', { ID: 'FebTotal:', FebAmount: febSum });
            grid.jqGrid('footerData', 'set', { ID: 'MarTotal:', MarAmount: marSum });

            var reccount = $('#tblAbsFundTransfer').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvpgrAbsFundTransfer_left').html('[<b> Note: All Amounts are in Rs.</b> ]');
            }

            $('#tblAbsFundTransfer_rn').html('Sr. <br/>No.');
          
            $.unblockUI();
        }
    });

    jQuery("#tblAbsFundTransfer").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'AprAmount', numberOfColumns: 12, titleText: ''+$("#FundType").val()+' Outstanding At The End Of '+Year+'-'+NextYear+'' },
          //{ startColumnName: 'Debits', numberOfColumns: 2, titleText: 'Gross Transaction' }
        ]
    });
    $.unblockUI();

});