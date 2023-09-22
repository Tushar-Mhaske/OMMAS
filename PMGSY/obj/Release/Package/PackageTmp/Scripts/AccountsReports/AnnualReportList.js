
$(document).ready(function () {

    $("#spnSRRDABalances").click(function () {
        $("#spnSRRDABalances").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblAnnualAccountSRRDA_wrapper").slideToggle("slow");
    });

    $("#spnStateBalances").click(function () {
        $("#spnStateBalances").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblAnnualAccount_wrapper").slideToggle("slow");
    });

    $("#spnDPIUBalances").click(function () {
        $("#spnDPIUBalances").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblAnnualAccountDPIU_wrapper").slideToggle("slow");
        $("#dvBalance").slideToggle("slow");
    });

   


   

    if ($("#Selection").val() == "S") {
        var oSingleCB = $('#tblAnnualAccount').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
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
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#TotalRecord").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            }

        });

        var oSingleCB = $('#tblAnnualAccountDPIU').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
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
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#TotalRecord").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            }

        });

    }

    if (($("#Selection").val() == "R") || ($("#LevelId").val() == 4)) {
       // alert("here");
        var oSingleCB = $('#tblAnnualAccountSRRDA').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
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
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#TotalRecord").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            }

        });

    }

    if ($("#Selection").val() == "D"|| $("#LevelId").val()==5) {       

        var oSingleCB = $('#tblAnnualAccountDPIU').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
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
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#TotalRecord").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            }

        });

   }


});