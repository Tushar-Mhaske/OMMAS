$(document).ready(function () {

    // show/hide data table of Chequebook Outstanding Details
    $("#spnChequeBookOutstandingDetails").click(function () {
        $("#spnChequeBookOutstandingDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblChequeOutstandingDetails_wrapper").slideToggle("slow");
    });

    $(function () {
        var oSingleCB = $('#tblChequeOutstandingDetails').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "130",
            "bPaginate": false,
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            //"sDom": '<"toolbar">frtip',
            "oTableTools": {
                "aButtons": []
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#chqOutstatndingCount").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            },
        });
    });
});