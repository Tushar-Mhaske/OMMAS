$(document).ready(function () {
   
    var oSingleCB = $('#tblBillDetails').dataTable({
        "bJQueryUI": true,
        "bFilter": false,
        "bSort": false,
        "bHeader": true,
        "sScrollY": "220px",
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

   
   // alert($.trim($('#dvBillDetails').html()).length);
    if ($.trim($('#dvBillDetails').html()).length == 0) {

        $("#btnViewDetails").trigger('click');
    }
    

    $("#dvTransactionDetails").dialog({
        autoOpen: false,
        //height: 'auto',
        width: 'auto',
        modal: true,
        title: 'Transaction Details'
    });

});

function transactionDetails(id,type) {
    //alert(id);
    //alert(type);

    //var billType = $("#BillType option:selected").val();

    //var code = billId + "," + billType;
    $.ajax({
        url: "/AccountsReports/TransactionDetailsView?param=" + id+","+type,
        type: "POST",
        dataType: "html",
        catche: false,
        async: false,
        success: function (data) {
            //$("#dvBillDetails").is(":visible")
            //{
            //    $("#dvBillDetails").hide("slow");
            //}


            $("#dvTransactionDetails").html('');
            $("#dvTransactionDetails").html(data);
            $("#dvTransactionDetails").dialog('open');

        },
        error: function (xht, ajaxOptions, throwError)
        { alert(xht.responseText); }

    });

}
