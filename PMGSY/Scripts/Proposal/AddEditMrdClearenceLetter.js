var GlClearancePdfFiles;
var GlRoadPdfFiles;
var GlRoadExcelFile;
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddMrdClearenceLetter');

    $('#MRD_CLEARANCE_DATE, #MRD_REVISION_DATE').datepicker({

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

    $("#ddlState").change(function () {
        loadAgencyList($("#ddlState").find(":selected").val());
        ClearMessage();
    });


    setTimeout(function () {
        if ($('#User_Action').val() == "E") {
            if ($('#rdoNew').is(':checked')) {
                $('#rdoNew').trigger('click');
            }
        }
    }, 500);

    $("#dvhdAddNewPIUDetails").click(function () {

        if ($("#dvAddNewMrdClearenceLetterDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewMrdClearenceLetterDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewMrdClearenceLetterDetails").slideToggle(300);
        }
    });
    //Save File Details on Button Submit type click
    $("#frmAddMrdClearenceLetter").on('submit', function (event) {

        if ($("#frmAddMrdClearenceLetter").valid()) {
            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();

            var objClearanceFile = $("input#ClerancePdfFile").prop("files");
            var objRoadPdfFile = $("input#RoadPdfFile").prop("files");
            var objRoadExcelFile = $("input#RoadExcelFile").prop("files");          
           
            form_data.append("ClearancePdfFile", objClearanceFile[0]);
            form_data.append("RoadPdfFile", objRoadPdfFile[0]);
            form_data.append("RoadExcelFile", objRoadExcelFile[0]);

            //$("#fileDetails").val(form_data);

            //alert("objClearanceFile" + objClearanceFile[0]);
            //alert("objRoadPdfFile" + objRoadPdfFile[0]);
            //alert("objRoadExcelFile" + objRoadExcelFile[0]);
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
            var data = $("#frmAddMrdClearenceLetter").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }
          
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();
            var collaboration = $("#ddlCollaboration option:selected").val();
          
            // console.log(form_data);

            $.ajax({
                type: 'POST',
                url: '/Proposal/AddEditMrdClearenceLetter/',
                async: false,
                data: form_data,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
                            $('#MrdClearenceLetterAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                            $("#MrdClearenceLetterSearchDetails").show('slow');
                        }
                        SearchCleranceDetails(stateCode, agency, year, batch, collaboration);
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

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Proposal/AddEditMrdClearenceLetter",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#MrdClearenceLetterAddDetails").html(data);
        //        $("#MrdClearenceLetterAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
            $('#MrdClearenceLetterAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
            $("#MrdClearenceLetterSearchDetails").show('slow');
        }
       
    })

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

       // e.preventDefault();
        ClearDetails();

    });

  

    //$("#frmAddMrdClearenceLetter").bind('submitdone', function () {

    //});

    // Add events
    //  $('input[type=file]').on('change', prepareUpload);

    $('#ClerancePdfFile').on('change', clearanceUpload);

    $('#RoadPdfFile').on('change', roadUploadPdf);

    $('#RoadExcelFile').on('change', roadUploadExcel);


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

    $('#rdoNew').click(function () {
        if ($('#trStageProposal').is(':hidden'))
        {
            $('#trStageProposal').show('slow');
        }
    });

    $('#rdoUpgrade').click(function () {
        if ($('#trStageProposal').is(':visible')) {
            $('#trStageProposal').hide();
        }
    });


});
// Grab the files and set them to our variable
function clearanceUpload(event) {
    ///$(this).parent().next().find("label").text("TESTTTT");
    //alert($('#Temp_MRD_CLEARANCE_PDF_FILE').text());
    //$('#Temp_MRD_CLEARANCE_PDF_FILE').text('');

    $("label[for = File_1]").text("");
    $('#Temp_MRD_CLEARANCE_PDF_FILE').val('');
    GlClearancePdfFiles = event.target.files;
}

function roadUploadPdf(event) {
    $("label[for = File_2]").text("");
    $('#Temp_MRD_ROAD_PDF_FILE').val('');
    GlRoadPdfFiles = event.target.files;
}
function roadUploadExcel(event) { 
    $("label[for = File_3]").text("");
    $('#Temp_MRD_ROAD_EXCEL_FILE').val('');
    GlRoadExcelFile = event.target.files;

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


}

function loadAgencyList(statCode) {
    $("#ddlAgency").val(0);
    $("#ddlAgency").empty();
    if (statCode > 0) {
        if ($("#ddlAgency").length > 0) {
            $.ajax({
                url: '/Proposal/PopulateAgenciesByStateAndDepartmentwise',
                type: 'POST',
                data: { "StateCode": statCode, "IsAllSelected": false },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }



                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlAgency").append("<option value='0'>--Select--</option>");

    }
}

function FileUploadSuccess(data) {
    alert(data.success);
    if (data.success == false) {
        $("#dvErrorMessage").show('fade');
        $("#message").html(data.message);
        return false;
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






