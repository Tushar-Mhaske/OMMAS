$(document).ready(function () {

    // show/hide data table of Chequebook Details
    $("#spnChequeBookDetails").click(function () {
        $("#spnChequeBookDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblCheckbookDetails_wrapper").slideToggle("slow");
    });

    if ($("#rdoSRRDA").is(":checked")) {
        $("#lblNameForAgency_PIU").html("Nodal Agency");
    }

    $("#rdoSRRDA").click(function () {
        $("#lblNameForAgency_PIU").html("Nodal Agency");
    });

    $("#rdoDPIU").click(function () {
        $("#lblNameForAgency_PIU").html("Name of PIU");
    });

    if ($("#rdoDPIU").is(":checked")) {
        $("#lblNameForAgency_PIU").html("Name of PIU");
    }
    
    if ($("#rdoCheckbookWise").is(":checked"))
    {
        var oSingleCB = $('#tblCheckbookDetails').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "620px",
            "bPaginate": false,
            //"sPaginationType": "full_numbers",
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            "oTableTools": {
                "aButtons": []
            },
            

            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#totalRecords").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
        },
        });
}
else{

        var oSingleCB = $('#tblCheckbookDetails').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "200px",
            "bPaginate": false,
            //"sPaginationType": "full_numbers",
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            "oTableTools": {
                "aButtons": []
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                
                if ($("#totalRecords").val()>0)
                {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            },
        });
}


});