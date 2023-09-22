$.validator.unobtrusive.adapters.add('compareprevvalue', ['oldroadclerancecount', 'oldroaddroppedcount'], function (options) {
    options.rules['compareprevvalue'] = options.params;
    options.messages['compareprevvalue'] = options.message;
});

$.validator.addMethod("compareprevvalue", function (value, element, params) {
    //alert(parseFloat($("#" + params.oldroadclerancecount).val()));
    //alert(parseFloat($("#" + params.oldroaddroppedcount).val()));
    //alert($("#" + params.oldroaddroppedcount).val());
    //if ($("#" + params.oldroadclerancecount).val() == "") {
    //    return true;
    //}

    if (parseFloat($("#" + params.oldroadclerancecount).val()) >= parseFloat($("#" + params.oldroaddroppedcount).val()) + parseFloat(value))
        return true;
    else
        return false;
});


$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddMrdDroppedLetter');

    setTimeout(function () {
        if ($('#hdUserAction').val() == "E") {
            if ($('#rdoNew').is(':checked')) {
                $('#rdoNew').trigger('click');
            }
        }
    }, 500);

    $('#MRD_CLEARANCE_DATE, #clrDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        changeMonth: true, //for month selection
        changeYear: true, //for year selection
        buttonText: "select date",

        minDate: $('#clrDate').val(),
        maxDate: new Date(year, 11, 31),
        onSelect: function (selectedDate) {

        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        // e.preventDefault();
        ClearDetails();

    });

    $("#btnCancel").click(function () {

        $("#dvhdSearch").trigger('click');
        $('#tblMrdDroppedLetterList').trigger('reloadGrid');
        $("#tblMrdDroppedLetterList").jqGrid('setGridState', 'visible');
        //LoadMrdDroppedLetterGrid();

        $('#btnAddMRDDroppedLetter').show('slow');

        $('#dvLoadSaveDroppedLetters').html('');
    })

    $('#rdoNew').click(function () {
        if ($('#trStageProposal').is(':hidden')) {
            $('#trStageProposal').show('slow');
            //$('#rdoStage1').is(':checked');
            //$('#rdoStage1').attr('checked', true);
        }
    });

    $('#rdoUpgrade').click(function () {
        if ($('#trStageProposal').is(':visible')) {
            $('#trStageProposal').hide();
        }
    });

    $('#imgDropPdf').click(function () {
        $('#ClerancePdfFile').val(null);
        $('#ClerancePdfFile').show('slow');
        $('#imgDropPdf').hide('slow');
        $('#lblDropPdf').text('');
        //$('#message').html('');
        //$('#dvErrorMessage').hide('slow');
    });
    $('#imgRoadPdf').click(function () {
        $('#RoadPdfFile').val(null);
        $('#RoadPdfFile').show('slow');
        $('#imgRoadPdf').hide('slow');
        $('#lblRoadPdf').text('');
    });
    $('#imgRoadExcel').click(function () {
        $('#RoadExcelFile').val(null);
        $('#RoadExcelFile').show('slow');
        $('#imgRoadExcel').hide('slow');
        $('#lblRoadExcel').text('');
    });

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmAddMrdDroppedLetter").toggle("slow");
        //$(this).next("#frmAddMrdDroppedLetter").slideToggle(300);

    });

    $('#MRD_ROAD_MORD_SHARE_AMT').blur(function () {
        CalaculateTotalAmount();
    });
    $('#MRD_ROAD_STATE_SHARE_AMT').blur(function () {
        CalaculateTotalAmount();
    });

    $('#MRD_LSB_MORD_SHARE_AMT').blur(function () {
        CalaculateTotalAmount();
    });
    $('#MRD_LSB_STATE_SHARE_AMT').blur(function () {
        CalaculateTotalAmount();
    });

    $("#frmAddMrdDroppedLetter").on('submit', function (event) {

        if ($("#frmAddMrdDroppedLetter").valid()) {
            //$('#User_Action').val($('#hdUserAction').val());
            //alert($('#User_Action').val());

            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();

            var objClearanceFile = $("input#ClerancePdfFile").prop("files");
            var objRoadPdfFile = $("input#RoadPdfFile").prop("files");
            var objRoadExcelFile = $("input#RoadExcelFile").prop("files");

            form_data.append("DroppedPdf", objClearanceFile[0]);
            form_data.append("DroppedRoadPdf", objRoadPdfFile[0]);
            form_data.append("DroppedRoadExcel", objRoadExcelFile[0]);

            //$("#User_Action").val("A");
            $("#MRD_ROAD_TOTAL_AMT").attr("disabled", false);
            $("#MRD_LSB_TOTAL_AMT").attr("disabled", false);
            $("#MRD_TOTAL_MORD_SHARE_AMT").attr("disabled", false);
            $("#MRD_TOTAL_STATE_SHARE_AMT").attr("disabled", false);
            $("#MRD_TOTAL_SANCTIONED_AMT").attr("disabled", false);
            $("#ddlState").attr("disabled", false);
            $("#ddlPhaseYear").attr("disabled", false);
            $("#ddlBatch").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            $("#ddlCollaboration").attr("disabled", false);

            var data = $("#frmAddMrdDroppedLetter").serializeArray();

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
                url: '/MRDProposal/AddDroppedLetters/',
                async: false,
                data: form_data,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        $("#btnCancel").trigger('click');
                        //if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
                        //    $('#MrdClearenceLetterAddDetails').hide('slow');
                        //    $('#btnSearch').hide();
                        //    $('#btnAdd').show();
                        //}

                        //if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                        //    $("#MrdClearenceLetterSearchDetails").show('slow');
                        //}
                        //SearchCleranceDetails(stateCode, agency, year, batch, collaboration);
                        // $("#ddlState").attr("disabled", true);
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                        if ($('#hdStatCode').val() > 0) {
                            $("#ddlState").val($('#hdStatCode').val());
                            $("#ddlState").attr("disabled", true);
                        }
                        //if ($('#User_Action').val() == "E") {
                        //    $("#ddlState").attr("disabled", true);
                        //    $("#ddlPhaseYear").attr("disabled", true);
                        //    $("#ddlBatch").attr("disabled", true);
                        //    $("#ddlAgency").attr("disabled", true);
                        //    $("#ddlCollaboration").attr("disabled", true);
                        //}
                        $("#MRD_ROAD_TOTAL_AMT").attr("disabled", true);
                        $("#MRD_LSB_TOTAL_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_MORD_SHARE_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_STATE_SHARE_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_SANCTIONED_AMT").attr("disabled", true);
                    }
                    else {
                        $("#MrdClearenceLetterAddDetails").html(data);
                        //if ($('#stateCode').val() > 0) {
                        //    $("#ddlState").val($('#stateCode').val());
                        //    $("#ddlState").attr("disabled", true);
                        //}
                        if ($('#hdStatCode').val() > 0) {
                            $("#ddlState").val($('#hdStatCode').val());
                            $("#ddlState").attr("disabled", true);
                        }
                        if ($('#User_Action').val() == "E") {
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);
                            $("#ddlBatch").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                            $("#ddlCollaboration").attr("disabled", true);
                        }
                        $("#MRD_ROAD_TOTAL_AMT").attr("disabled", true);
                        $("#MRD_LSB_TOTAL_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_MORD_SHARE_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_STATE_SHARE_AMT").attr("disabled", true);
                        $("#MRD_TOTAL_SANCTIONED_AMT").attr("disabled", true);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    if ($('#hdStatCode').val() > 0) {
                        $("#ddlState").val($('#hdStatCode').val());
                        $("#ddlState").attr("disabled", true);
                    }
                    if ($('#User_Action').val() == "E") {
                        $("#ddlPhaseYear").attr("disabled", true);
                        $("#ddlBatch").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);
                        $("#ddlCollaboration").attr("disabled", true);
                    }
                    $("#MRD_ROAD_TOTAL_AMT").attr("disabled", true);
                    $("#MRD_LSB_TOTAL_AMT").attr("disabled", true);
                    $("#MRD_TOTAL_MORD_SHARE_AMT").attr("disabled", true);
                    $("#MRD_TOTAL_STATE_SHARE_AMT").attr("disabled", true);
                    $("#MRD_TOTAL_SANCTIONED_AMT").attr("disabled", true);
                }
            })

        }

    });

    $('#ClerancePdfFile').on('change', clearanceUpload);

    $('#RoadPdfFile').on('change', roadUploadPdf);

    $('#RoadExcelFile').on('change', roadUploadExcel);

});

// Grab the files and set them to our variable
function clearanceUpload(event) {

    GlClearancePdfFiles = event.target.files;
    if (GlClearancePdfFiles != null) {

        /* IMP CLIENT-SIDE VALIDATIONS DO NOT DELETE 

        var index = this.files[0].name.lastIndexOf('.');
        var a = this.files[0].name.substring(index + 1);
        //alert(a);
        if (a != 'pdf') {
            $('#message').html('Dropped Letter Pdf - File type not allowed.');
            $('#dvErrorMessage').show('slow');
            
            $('#lblDropPdf').text($('#ClerancePdfFile').val());
            $('#imgDropPdf').show('slow');
            $('#ClerancePdfFile').hide('slow');
        }
        
        else if (this.files[0].size > 4194304) {
            $('#message').html('Dropped Letter Pdf - File size cannot be greater than 4 MB.');
            $('#dvErrorMessage').show('slow');
            $('#ClerancePdfFile').val(null);
        }
        else {
            $('#message').html('');
            $('#dvErrorMessage').hide('slow');
            $('#lblDropPdf').text($('#ClerancePdfFile').val());
            $('#imgDropPdf').show('slow');
            $('#ClerancePdfFile').hide('slow');
        }*/

        $('#lblDropPdf').text($('#ClerancePdfFile').val());
        $('#imgDropPdf').show('slow');
        $('#ClerancePdfFile').hide('slow');
    }
}

function roadUploadPdf(event) {

    GlRoadPdfFiles = event.target.files;

    if (GlRoadPdfFiles != null) {

        $('#lblRoadPdf').text($('#RoadPdfFile').val());
        $('#imgRoadPdf').show('slow');
        $('#RoadPdfFile').hide('slow');
    }
}

function roadUploadExcel(event) {

    GlRoadExcelFile = event.target.files;

    if (GlRoadExcelFile != null) {
        $('#lblRoadExcel').text($('#RoadExcelFile').val());
        $('#imgRoadExcel').show('slow');
        $('#RoadExcelFile').hide('slow');
    }

}

function CalaculateTotalAmount() {
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
    $('#MRD_ROAD_TOTAL_AMT').val(totalRoadAmount);

    if ($('#MRD_ROAD_TOTAL_AMT').val() >= 0) {
        grandtotalRoadAmount = $('#MRD_ROAD_TOTAL_AMT').val();
    }

    if ($('#MRD_LSB_MORD_SHARE_AMT').val() >= 0) {
        mordBridgeAmount = $('#MRD_LSB_MORD_SHARE_AMT').val();
    }
    if ($('#MRD_LSB_STATE_SHARE_AMT').val() >= 0) {
        stateBridgeAmount = $('#MRD_LSB_STATE_SHARE_AMT').val();
    }
    totalBridgeAmount = (parseFloat(stateBridgeAmount) + parseFloat(mordBridgeAmount)).toFixed(4);
    $('#MRD_LSB_TOTAL_AMT').val(totalBridgeAmount);

    if ($('#MRD_LSB_TOTAL_AMT').val() >= 0) {
        grandtotalBridgeAmount = $('#MRD_LSB_TOTAL_AMT').val();
    }

    grandTotalMordAmount = (parseFloat(mordRoadAmount) + parseFloat(mordBridgeAmount)).toFixed(4)
    $('#MRD_TOTAL_MORD_SHARE_AMT').val(grandTotalMordAmount);

    grandTotalStateAmount = (parseFloat(stateRoadAmount) + parseFloat(stateBridgeAmount)).toFixed(4)
    $('#MRD_TOTAL_STATE_SHARE_AMT').val(grandTotalStateAmount);
    grandTotalStateAmount = (parseFloat(grandTotalMordAmount) + parseFloat(grandTotalStateAmount)).toFixed(4)
    $('#MRD_TOTAL_SANCTIONED_AMT').val(grandTotalStateAmount);
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
        $('#tblMrdClearenceLetter').trigger("reloadGrid", [{ page: 1 }]);
    }, 1000);
}

function ClearMessage() {
    if ($("#dvErrorMessage").is(":visible")) {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    }

}

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


    $('#ClerancePdfFile').val(null);
    $('#lblRoadPdf').val(null);
    $('#lblRoadExcel').val(null);
}
