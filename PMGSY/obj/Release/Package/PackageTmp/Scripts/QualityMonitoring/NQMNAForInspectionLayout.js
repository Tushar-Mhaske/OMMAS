$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmNQMNAForInspectionLayout');

    $('#btnAdd').click(function () {
        if (!$("#NQMNADetails").is(":visible")) {

        }
        $('#NQMNADetails').html('');
        $("#NQMNADetails").load("/QualityMonitoring/AddNQMNAForInspectionLayout/");

        //$('#NQMNADetails').show('slow');

        $('#btnAdd').hide('slow');
        $('#btnSearch').show('slow');
    });

    $('#btnSearch').click(function () {

        $('#NQMNADetails').html('');

        $("#NQMNADetails").load("/QualityMonitoring/SearchNQMNAForInspectionLayout/");

        $('#btnAdd').show('slow');
        $('#btnSearch').hide('slow');
    });

    $('#btnSearch').trigger('click');
});