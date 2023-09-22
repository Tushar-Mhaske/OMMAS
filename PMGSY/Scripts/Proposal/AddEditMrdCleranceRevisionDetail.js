var GlClearanceRevisedPdfFile;
var GlRoadRevisedPdfFiles;
var GlRoadRevisedExcelFile;
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddMrdCleranceRevisionDetail');

    $('#MRD_REVISION_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        changeMonth: true, //for month selection
        changeYear: true, //for year selection
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    if ($('#tblFileDownloadClearancedRevisionDetail').is(':visible'))
    {
        LoadMrdClearenceRevisionLetterGrid();
    }
    $("#ddlState").change(function () {
        loadAgencyList($("#ddlState").find(":selected").val());
        ClearMessage();
    });

    $("#dvhdAddNewMrdCleranceRevisionDetail").click(function () {

        if ($("#dvAddNewMrdCleranceRevisionDetail").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewMrdCleranceRevisionDetail").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewMrdCleranceRevisionDetail").slideToggle(300);
        }
    });

    $("#frmAddMrdCleranceRevisionDetail").on('submit', function (event) {
       
        if ($("#frmAddMrdCleranceRevisionDetail").valid()) {
           
            event.stopPropagation(); // Stop stuff happening
            event.preventDefault();

            $('#MrdClearenceLetterAddDetails').html('');


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
           
            var data = $("#frmAddMrdCleranceRevisionDetail").serializeArray();

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
                url: '/Proposal/AddEditMrdClerenceRevisionDetail/',
                async: false,
                data: form_data,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                       // $('#divMrdRevisionClearance').load('/Proposal/AddEditMrdClearenceRevsionDetail?id=' + $('#EncryptedClearanceCode').val());
                        //if ($("#divMrdRevisionClearance").is(":visible")) {
                        //    $('#divMrdRevisionClearance').hide('slow');
                        //    $('#btnSearch').hide();
                        //    $('#btnAdd').show();
                        //}

                        //if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                        //    $("#MrdClearenceLetterSearchDetails").show('slow');
                        //}

                       // CloseClearanceRevisionDetails();
                        //SearchMrdRevsionDetails(stateCode, agency, year, batch, collaboration);
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
                        CloseClearanceRevsionAddEditDetails();
                        $('#tblFileDownloadClearancedRevisionDetail').trigger('reloadGrid');
                     
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

    $("#btnCancelRevision").click(function () {
        
        CloseClearanceRevisionDetails();
    })

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();

    });

    setTimeout(function () {
        if ($('#User_Action').val() == "E") {
            if ($('#rdoNewRevision').is(':checked')) {
                $('#rdoNewRevision').trigger('click');
            }
        }
    }, 500);


    $('#ClearanceRevisedPdfFile').on('change', clearanceRevisonUpload);

    $('#RoadRevisedPdfFile').on('change', roadRevisionUploadPdf);

    $('#RoadRevisedExcelFile').on('change', roadRevisionUploadExcel);

    $('#MRD_ROAD_MORD_SHARE_AMT').blur(function () {
        CalaculateTotalRevisionAmount();
    });
    $('#MRD_ROAD_STATE_SHARE_AMT').blur(function () {
        CalaculateTotalRevisionAmount();
    });

    $('#MRD_LSB_MORD_SHARE_AMT').blur(function () {
        CalaculateTotalRevisionAmount();
    });
    $('#MRD_LSB_STATE_SHARE_AMT').blur(function () {
        CalaculateTotalRevisionAmount();
    });

    $('#rdoNewRevision').click(function () {
        if ($('#trStageProposalRevision').is(':hidden')) {
            $('#trStageProposalRevision').show('slow');
        }
    });

    $('#rdoUpgradeRevision').click(function () {
        if ($('#trStageProposalRevision').is(':visible')) {
            $('#trStageProposalRevision').hide();
        }
    });

});
// Grab the files and set them to our variable
function clearanceRevisonUpload(event) {
    ///$(this).parent().next().find("label").text("TESTTTT");
    //alert($('#Temp_MRD_CLEARANCE_REVISED_PDF_FILE').text());
    //$('#Temp_MRD_CLEARANCE_REVISED_PDF_FILE').text('');

    $("label[for = Revised_File_1]").text("");
    $('#Temp_MRD_CLEARANCE_REVISED_PDF_FILE').val('');
    GlClearanceRevisedPdfFile = event.target.files;
}

function roadRevisionUploadPdf(event) {
    $("label[for = Revised_File_2]").text("");
    $('#Temp_MRD_ROAD_REVISED_PDF_FILE').val('');
    GlRoadRevisedPdfFiles = event.target.files;
}
function roadRevisionUploadExcel(event) {
    $("label[for = Revised_File_3]").text("");
    $('#Temp_MRD_ROAD_REVISED_EXCEL_FILE').val('');
    GlRoadRevisedExcelFile = event.target.files;

}


function SearchMrdRevsionDetails(stateCode, agency, year, batch, collaboration) {
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
        $('#tblMrdClearenceLetter').trigger("reloadGrid", [{ page: 1 }]);
    }, 1000);
}

function ClearMessage() {
    if ($("#dvErrorMessageRevision").is(":visible")) {
        $('#dvErrorMessageRevision').hide('slow');
        $('#messageRevision').html('');
    }

}

function ClearDetails() {  
    $('#ClearanceRevisedPdfFile').val('');
    $('#RoadRevisedPdfFile').val('');
    $('#RoadRevisedExcelFile').val('');
    $('#dvErrorMessageRevision').hide('slow');
    $('#messageRevision').html('');
}


function FileUploadSuccess(data) {
    alert(data.success);
    if (data.success == false) {
        $("#dvErrorMessageRevision").show('fade');
        $("#messageRevision").html(data.message);
        return false;
    }
}



function CalaculateTotalRevisionAmount() {
    var grandtotalRoadAmount = 0;
    var totalRoadAmount = 0
    var stateRoadAmount = 0;
    var mordRoadAmount = 0;


    var grandtotalBridgeAmount = 0;
    var totalBridgeAmount = 0
    var stateBridgeAmount = 0;
    var mordBridgeAmount = 0;

    var grandTotalMordAmount = 0;
    var grandTotalStateAmount = 0;

    if ($('#MRD_ROAD_MORD_SHARE_AMT').val() >= 0) {
        mordRoadAmount = $('#MRD_ROAD_MORD_SHARE_AMT').val();
    }
    if ($('#MRD_ROAD_STATE_SHARE_AMT').val() >= 0) {
        stateRoadAmount = $('#MRD_ROAD_STATE_SHARE_AMT').val();
    }
    totalRoadAmount = (parseFloat(stateRoadAmount) + parseFloat(mordRoadAmount)).toFixed(4);
    $('#MRD_ROAD_TOTAL_AMTRevision').val(totalRoadAmount);

    if ($('#MRD_ROAD_TOTAL_AMTRevision').val() >= 0) {
        grandtotalRoadAmount = $('#MRD_ROAD_TOTAL_AMTRevision').val();
    }

    if ($('#MRD_LSB_MORD_SHARE_AMT').val() >= 0) {
        mordBridgeAmount = $('#MRD_LSB_MORD_SHARE_AMT').val();
    }
    if ($('#MRD_LSB_STATE_SHARE_AMT').val() >= 0) {
        stateBridgeAmount = $('#MRD_LSB_STATE_SHARE_AMT').val();
    }
    totalBridgeAmount = (parseFloat(stateBridgeAmount) + parseFloat(mordBridgeAmount)).toFixed(4);
    $('#MRD_LSB_TOTAL_AMTRevision').val(totalBridgeAmount);

    if ($('#MRD_LSB_TOTAL_AMTRevision').val() >= 0) {
        grandtotalBridgeAmount = $('#MRD_LSB_TOTAL_AMTRevision').val();
    }

    grandTotalMordAmount = (parseFloat(mordRoadAmount) + parseFloat(mordBridgeAmount)).toFixed(4)
    $('#MRD_TOTAL_MORD_SHARE_AMTRevision').val(grandTotalMordAmount);

    grandTotalStateAmount = (parseFloat(stateRoadAmount) + parseFloat(stateBridgeAmount)).toFixed(4)
    $('#MRD_TOTAL_STATE_SHARE_AMTRevision').val(grandTotalStateAmount);
    grandTotalStateAmount = (parseFloat(grandTotalMordAmount) + parseFloat(grandTotalStateAmount)).toFixed(4)
    $('#MRD_TOTAL_SANCTIONED_AMTRevision').val(grandTotalStateAmount);
}





