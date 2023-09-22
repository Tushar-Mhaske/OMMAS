
$(document).ready(function () {    
        var fundTransfer = $('#tblFundTransfer').dataTable({
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

  

    
});