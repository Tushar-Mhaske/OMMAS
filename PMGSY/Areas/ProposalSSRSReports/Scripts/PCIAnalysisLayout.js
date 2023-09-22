$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPCIAnalysisLayout'));

    $('#btnPCIAbstractAnalyisDetails').click(function () {


        $('#Route').val($('#ddRoute_PCIAbstractAnalyisDetails option:selected').val());
        $('#RouteName').val($('#ddRoute_PCIAbstractAnalyisDetails option:selected').text());

        if ($('#frmPCIAnalysisLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/PCIAnalysisReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPCIAnalysisLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadPCIAnalysisReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnPCIAbstractAnalyisDetails').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvPCIAnalysisLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});