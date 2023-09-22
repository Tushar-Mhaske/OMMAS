$(document).ready(function () {

    // show/hide data table of Chequebook Abstract Details
    $("#spnChequeBookAbstractDetails").click(function () {
        $("#spnChequeBookAbstractDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblChequeIssueAbstractDetails_wrapper").slideToggle("slow");
    });

    $(function () {
        var oSingleCB = $('#tblChequeIssueAbstractDetails').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "320px",
            "bPaginate": false,
            "bScrollInfinite": true,
            "bScrollCollapse": true,
            "sDom": '<"H"Tfr>t<"F"ip>',
            "oTableTools": {
                "aButtons": []
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if ($("#chqIssueAbstractCount").val() > 0) {
                    $("td:first", nRow).html(iDisplayIndex + 1);
                    return nRow;
                }
            },
        });
    });
});