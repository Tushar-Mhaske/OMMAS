$(document).ready(function () {


    setTimeout(function () {
        if ($('#User_Action').val() == "E") {
            if ($('#rdoNew').is(':checked')) {
                $('#rdoNew').trigger('click');
            }
        }
    }, 500);


    $("#btnCancelRevisionPartial").click(function () {

        if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
            $('#MrdClearenceLetterAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
            $("#MrdClearenceLetterSearchDetails").show('slow');
        }
    })


    $("#frmAddMrdCleranceRevisionDetailPartial").on('submit', function (event) {

        if ($("#frmAddMrdCleranceRevisionDetailPartial").valid()) {

            event.stopPropagation(); 
            event.preventDefault();

            

            var form_data = new FormData();

            var objClearanceRevisedPdfFile = $("input#ClearanceRevisedPdfFile").prop("files");
            var objRoadRevisedPdfFile = $("input#RoadRevisedPdfFile").prop("files");
            var objRoadRevisedExcelFile = $("input#RoadRevisedExcelFile").prop("files");

            form_data.append("ClearanceRevisedPdfFile", objClearanceRevisedPdfFile[0]);
            form_data.append("RoadRevisedPdfFile", objRoadRevisedPdfFile[0]);
            form_data.append("RoadRevisedExcelFile", objRoadRevisedExcelFile[0]);

            $("#MRD_ROAD_TOTAL_AMTRevision").attr("disabled", false);
            $("#MRD_LSB_TOTAL_AMTRevision").attr("disabled", false);
            $("#MRD_TOTAL_MORD_SHARE_AMTRevision").attr("disabled", false);
            $("#MRD_TOTAL_STATE_SHARE_AMTRevision").attr("disabled", false);
            $("#MRD_TOTAL_SANCTIONED_AMTRevision").attr("disabled", false);
            $("#ddlStateRevision").attr("disabled", false);
            $("#ddlPhaseYearRevision").attr("disabled", false);
            $("#ddlBatchRevision").attr("disabled", false);
            $("#ddlAgencyRevision").attr("disabled", false);
            $("#ddlCollaborationRevision").attr("disabled", false);

            var data = $("#frmAddMrdCleranceRevisionDetailPartial").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();
            var collaboration = $("#ddlCollaboration option:selected").val();

            $.ajax({
                type: 'POST',
                url: '/Proposal/AddEditMrdClerenceRevisionDetailPartial/',
                async: false,
                data: form_data,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        $("#ddlStateRevision").attr("disabled", true);
                        $("#ddlPhaseYearRevision").attr("disabled", true);
                        $("#ddlBatchRevision").attr("disabled", true);
                        $("#ddlAgencyRevision").attr("disabled", true);
                        $("#ddlCollaborationRevision").attr("disabled", true);
                        $("#MRD_ROAD_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_LSB_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_MORD_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_STATE_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_SANCTIONED_AMTRevision").attr("disabled", true);
                        
                        $('#tblFileDownloadClearancedRevisionDetail').trigger('reloadGrid');

                        if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
                            $('#MrdClearenceLetterAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                            $("#MrdClearenceLetterSearchDetails").show('slow');
                        }

                        SearchCleranceDetails(stateCode, agency, year, batch, collaboration);

                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#messageRevision').html(data.message);
                            $('#dvErrorMessageRevision').show('slow');

                        }
                        $("#ddlStateRevision").attr("disabled", true);
                        $("#ddlPhaseYearRevision").attr("disabled", true);
                        $("#ddlBatchRevision").attr("disabled", true);
                        $("#ddlAgencyRevision").attr("disabled", true);
                        $("#ddlCollaborationRevision").attr("disabled", true);
                        $("#MRD_ROAD_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_LSB_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_MORD_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_STATE_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_SANCTIONED_AMTRevision").attr("disabled", true);
                    }
                    else {
                        $("#ddlStateRevision").attr("disabled", true);
                        $("#ddlPhaseYearRevision").attr("disabled", true);
                        $("#ddlBatchRevision").attr("disabled", true);
                        $("#ddlAgencyRevision").attr("disabled", true);
                        $("#ddlCollaborationRevision").attr("disabled", true);
                        $("#MRD_ROAD_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_LSB_TOTAL_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_MORD_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_STATE_SHARE_AMTRevision").attr("disabled", true);
                        $("#MRD_TOTAL_SANCTIONED_AMTRevision").attr("disabled", true);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })

        }

    });

    $('#rdoNew').click(function () {
        if ($('#trStageProposal').is(':hidden')) {
            $('#trStageProposal').show('slow');
        }
    });

    $('#rdoUpgrade').click(function () {
        if ($('#trStageProposal').is(':visible')) {
            $('#trStageProposal').hide();
        }
    });

});
function ClearDetails() {
    $('#ddlState').val('0');
    if ($('#hdStatCode').val() > 0) {
        $("#ddlState").val($('#hdStatCode').val());
        $("#ddlState").attr("disabled", true);
    }
    $('#ddlPhaseYear').val('0');
    $('#ddlBatch').val('0');
    $('#ddlAgency').val('0');
    $('#ddlImsFileType').val('%');
    $('#file').val('');
    $('#ClerancePdfFile').val('');
    $('#RoadPdfFile').val('');
    $('#RoadExcelFile').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');


}
function SearchCleranceDetails(stateCode, agency, year, batch, collaboration) {
    $('#ddlStateSerach').val(stateCode);
    $('#ddlStateSerach').trigger('change');
    setTimeout(function () {
        $('#ddlAgencySerach').val(agency);
        $('#ddlPhaseYearSerach').val(year);
        $('#ddlBatchSerach').val(batch);
        $('#ddlCollaborationSerach').val(collaboration);

        $('#tblMrdClearenceLetter').setGridParam({
            url: '/Proposal/GetMrdClearenceLetterList'
        });
        $('#tblMrdClearenceLetter').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlStateSerach').val(), agency: $('#ddlAgencySerach').val(), year: $('#ddlPhaseYearSerach').val(), batch: $('#ddlBatchSerach').val(), collaboration: $('#ddlCollaborationSerach').val() } });
        $('#tblMrdClearenceLetter').trigger("reloadGrid");
    }, 1000);
}