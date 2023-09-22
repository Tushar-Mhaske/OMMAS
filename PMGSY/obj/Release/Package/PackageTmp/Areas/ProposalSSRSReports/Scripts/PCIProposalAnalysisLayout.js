$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPCIPropAnalysisLayout'));

    $('#btnPCIPropAbstractAnalyisDetails').click(function () {


        $('#Route').val($('#ddRoute_PCIPropAbstractAnalyisDetails option:selected').val());
        $('#RouteName').val($('#ddRoute_PCIPropAbstractAnalyisDetails option:selected').text());

        if ($('#frmPCIPropAnalysisLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/PCIPropAnalysisReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPCIPropAnalysisLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadPCIPropAnalysisReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnPCIPropAbstractAnalyisDetails').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvPCIPropAnalysisLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});