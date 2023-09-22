$(document).ready(function () {

    // show/hide data table of Chequebook Details
    $("#spnAuthSignatoryDetails").click(function () {
        $("#spnAuthSignatoryDetails").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#tblAuthSignatoryDetails_wrapper").slideToggle("slow");
    });

    var oSingleCB = $('#tblAuthSignatoryDetails').dataTable({
            "bJQueryUI": true,
            "bFilter": false,
            "bSort": false,
            "bHeader": true,
            "sScrollY": "420px",
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
});