$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmRoadSafetyLayout1');

    // Date Inspection date
    var agrdate = $('#Agreementdate').text().split('/');;
    var Agdate = new Date(agrdate[2], (parseInt(agrdate[1]) - 1), agrdate[0]);
    //  alert(Agdate)
    var end = new Date();
    //var diff = new Date(end - Agdate);
    var start = new Date(2000, 0, 1);

    var diff = new Date(end - start);
    var days = diff / 1000 / 60 / 60 / 24;
    // alert(Math.floor(days))
    $('#txtAuditDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a Audit date',
        maxDate: "0D",
        minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Audit Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtAuditDate').trigger('blur');
        }
    });

    var RsaCode = $("#EXEC_RSA_CODE").val();

    LoadInspDetailsList(RsaCode);

    $('#btnSave').click(function ()
    {
       SaveDetails();
    });


    $('#btnCancel').click(function () {

         $('#accordion').hide('slow');  //close the form
         $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
    });

    // Saving Master Only
    $('#btnSave1').click(function () {
        SaveRoadSafety1();
   });


    

    $('#btnCancel1').click(function () {

        $('#accordion').hide('slow');  //close the form
        $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
    });


    $("#HighlyDesirable").hide();
    $("#Desirable").hide();
    $("#Essential").hide();
});


// ddlIssueCodeDetails

$("#ddlGrade").change(function () {
    
    $("#HighlyDesirable").hide();
    $("#Desirable").hide();
    $("#Essential").hide();


    var likelihood = $('#ddlLikelihood').val();
    var GradeValue = $('#ddlGrade').val();

    if (likelihood != -1 && GradeValue!=-1) {
        var ProbabilityOfOccurance;

        if (GradeValue=="U" && likelihood=="H") {
            ProbabilityOfOccurance = "E"; //Essential
        }
        else if (GradeValue == "U" && likelihood=="M") {
            ProbabilityOfOccurance = "E";
        }
        else if (GradeValue == "U" && likelihood=="L") {
            ProbabilityOfOccurance = "H"; //Highly Desirable
        }
        else if (GradeValue == "R" && likelihood=="H") {
            ProbabilityOfOccurance = "E";
        }
        else if (GradeValue == "R" && likelihood=="M") {
            ProbabilityOfOccurance = "H";
        }
        else if (GradeValue == "R" && likelihood=="L") {
            ProbabilityOfOccurance = "D";
        }
        else if (GradeValue == "S" && likelihood=="H") {
            ProbabilityOfOccurance = "H";
        }
        else if (GradeValue == "S" && likelihood=="M") {
            ProbabilityOfOccurance = "D"; //Desirable
        }
        else if (GradeValue == "S" && likelihood=="L") {
            ProbabilityOfOccurance = "D";
        }
        else
        {
            ProbabilityOfOccurance = "E";
        }

        if (ProbabilityOfOccurance == 'E')
        {
            $("#Essential").show();

            $("#Desirable").hide();
            $("#HighlyDesirable").hide();
          
        }
        else if (ProbabilityOfOccurance == 'D')
        {
            $("#Desirable").show();

            $("#Essential").hide();
            $("#HighlyDesirable").hide();

        }
        else if (ProbabilityOfOccurance == 'H')
        {
            $("#HighlyDesirable").show();

            $("#Desirable").hide();
            $("#Essential").hide();
        }
      


    }

});


$("#ddlLikelihood").change(function () {

    $("#HighlyDesirable").hide();
    $("#Desirable").hide();
    $("#Essential").hide();

    var likelihood = $('#ddlLikelihood').val();
    var GradeValue = $('#ddlGrade').val();


    if (likelihood != -1 && GradeValue != -1) {
        var ProbabilityOfOccurance;

        if (GradeValue == "U" && likelihood == "H") {
            ProbabilityOfOccurance = "E"; //Essential
        }
        else if (GradeValue == "U" && likelihood == "M") {
            ProbabilityOfOccurance = "E";
        }
        else if (GradeValue == "U" && likelihood == "L") {
            ProbabilityOfOccurance = "H"; //Highly Desirable
        }
        else if (GradeValue == "R" && likelihood == "H") {
            ProbabilityOfOccurance = "E";
        }
        else if (GradeValue == "R" && likelihood == "M") {
            ProbabilityOfOccurance = "H";
        }
        else if (GradeValue == "R" && likelihood == "L") {
            ProbabilityOfOccurance = "D";
        }
        else if (GradeValue == "S" && likelihood == "H") {
            ProbabilityOfOccurance = "H";
        }
        else if (GradeValue == "S" && likelihood == "M") {
            ProbabilityOfOccurance = "D"; //Desirable
        }
        else if (GradeValue == "S" && likelihood == "L") {
            ProbabilityOfOccurance = "D";
        }
        else {
            ProbabilityOfOccurance = "E";
        }

        if (ProbabilityOfOccurance == 'E') {
            $("#Essential").show();

            $("#Desirable").hide();
            $("#HighlyDesirable").hide();

        }
        else if (ProbabilityOfOccurance == 'D') {
            $("#Desirable").show();

            $("#Essential").hide();
            $("#HighlyDesirable").hide();

        }
        else if (ProbabilityOfOccurance == 'H') {
            $("#HighlyDesirable").show();

            $("#Desirable").hide();
            $("#Essential").hide();
        }



    }

});


$("#ddlIssueCodeDetails").change(function () {
    //   NqmSqm = $("#rdoSQM").val();
   var IssueCodeID= $('#ddlIssueCodeDetails').val();

  

   $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
   $.ajax({
       url: '/Execution/GetIssueSubDetailsForTextBox?id=' + IssueCodeID,
       method: 'GET',
       cache: false,
       async: true,
    //   header: token,
      // data: $('#frmRoadSafetyLayout1').serialize(),
       dataType: 'json',
       success: function (data, status, xhr) {
         //  alert(data.message)
           if (data.success)
           {
               // alert("Start Chainage = " + data.StartChainage)
               //$('#accordion').hide();  //close the form
               //$("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
               //  $("#divHabitationDetails").unload();

              
               $('#txtSafetyIssue').val('');
               $('#txtRecommondation').val('');


               var Desc = data.IssueDescDetails;
               var Recommendation = data.IssueReccomendationDetails;

               $('#txtSafetyIssue').val(Desc);
               $('#txtRecommondation').val(Recommendation);

              


               



           }
           $.unblockUI();
       },
       error: function (xhr, status, err)
       {
           $('#txtSafetyIssue').val('');
           $('#txtRecommondation').val('');

           alert("Error Occured.");
           $.unblockUI();
       }
   });

});


function SaveRoadSafety1() {
    var token = $('#frmRoadSafetyLayout1 input[name=__RequestVerificationToken]').val();
    if ($('#frmRoadSafetyLayout1').valid()) {

        if (confirm("Do you want to save Details ?  RSA Stage & Inspection Date can not be modified again once the details are saved. ")) {

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSA',
                method: 'POST',
                cache: false,
                async: true,
                header: token,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr) {
                    alert(data.message)
                    if (data.success)
                    {
                       // alert("Start Chainage = " + data.StartChainage)
                         //$('#accordion').hide();  //close the form
                         //$("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                        //  $("#divHabitationDetails").unload();
                        ViewAfterSave(data.encryptedURLID);



                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
                    $.unblockUI();
                }
            });
        }
    }



    else {

        return false;
    }

}


function ViewAfterSave(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Road Safety Auditor Inspection Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );
    debugger;
    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        debugger;
        $("#divAddExecution").load('/Execution/GetDetailsForATR/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#divAddExecution'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}



function SaveDetails()
{
    var RsaCode = $("#EXEC_RSA_CODE").val();
    var token = $('#frmRoadSafetyLayout1 input[name=__RequestVerificationToken]').val();
    if ($("#frmRoadSafetyLayout1").valid()) {
    if (confirm("Do you want to save Details ?")) {

       
            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSAdetails',
                method: 'POST',
                cache: false,
                async: true,
                header: token,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr)
                {
                    alert(data.message)
                  
                    $('#tbCDWorksList').trigger('reloadGrid');
                 //   document.getElementById("frmRoadSafetyLayout1").reset();

                    $("#dvAddMaintenanceAgreementAgainstRoad1").html();
                    $('#accordion1').hide('slow');
                    $('#dvAddMaintenanceAgreementAgainstRoad1').hide('slow');


                 // alert("Start Chainage = "+data.StartChainage)

                    if (data.success)
                    {
                        // $('#accordion').hide();  //close the form
                        // $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                        LoadInspDetailsList(RsaCode);

                        ViewAfterSave(data.encryptedURLID);

                      //  document.getElementById("#StartChainage").value = data.StartChainage;

                        //alert(data.StartChainage);
                        //$("#db").hide('slow');
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err)
                {
                    alert("Error Occured.");
                    // alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }

    else {

        return false;
    }

}



function LoadInspDetailsList(RSACode) {
  //  alert("RSACode = " + RSACode)

    jQuery("#tbCDWorksList").jqGrid({
        url: '/Execution/GetInspectionDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { RSACode: RSACode },
        colNames: ['Start Chainage', 'End Chainage', 'Safety Issue', 'RSA Recommendation','Probability of Occurrence', 'Severity', 'Image Upload', 'Delete', 'Add ATR (PIU)', 'Acceptance (PIU)', 'ATR (PIU)', 'ATR Date (PIU)', 'Image Upload (PIU)', 'View PDF by PIU', 'Add ATR (SQC)', 'Acceptance (SQC) ', 'ATR Date (SQC)', 'Revised Grade (SQC)', 'Finalize (SQC)'],//, 'Add ATR', 'Acceptance By PIU', 'Action Taken Remarks By PIU','ATR Upload Date By PIU'
        colModel: [
                    { name: 'EXEC_RSA_START_CHAINAGE', index: 'EXEC_RSA_START_CHAINAGE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'EXEC_RSA_END_CHAINAGE', index: 'EXEC_RSA_END_CHAINAGE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'EXEC_RSA_SAFETY_ISSUE', index: 'EXEC_RSA_SAFETY_ISSUE', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'EXEC_RSA_RECOMMENDATION', index: 'EXEC_RSA_RECOMMENDATION', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'IMS_ROAD_STATUS', index: 'IMS_ROAD_STATUS', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'EXEC_RSA_GRADE', index: 'EXEC_RSA_GRADE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 150, sortable: false, resize: false, align: "center", search: false },// Upload Image By Auditor // UploadPhotoByAuditor
                            { name: 'z', width: 150, sortable: false, resize: false, align: "center", search: false },// Delete Record By Auditor // DeleteDetailsByAuditor

                            // PIU
                           { name: 'b', width: 60, sortable: false, resize: false, align: "center", search: false, hidden: true, hidden: true }, // Add ATR By PIU
                           { name: 'EXEC_RSA_REC_ACCP', index: 'EXEC_RSA_REC_ACCP', height: 'auto', width: 40, align: "center", search: false, hidden: true },
                           { name: 'EXEC_RSA_ACTION_TAKEN', index: 'EXEC_RSA_ACTION_TAKEN', height: 'auto', width: 200, align: "center", search: false, hidden: true },
                           { name: 'EXEC_ATR_DATE', index: 'EXEC_ATR_DATE', height: 'auto', width: 70, align: "center", search: false, hidden: true },
                           { name: 'c', width: 200, sortable: false, resize: false, align: "center", search: false, hidden: true },

                           // SQC
                           { name: 'z', width: 40, sortable: false, resize: false, align: "center", search: false, hidden: true }, // View ATR Uploaded By PIU = ViewPdfUploadedByPIU
                           { name: 'd', width: 200, sortable: false, resize: false, align: "center", search: false, hidden: true, hidden: true }, // Add SQC Remarks
                           { name: 'EXEC_ATR_ACCEPT_SQC', index: 'EXEC_ATR_ACCEPT_SQC', height: 'auto', width: 70, align: "center", search: false, hidden: true },
                           { name: 'EXEC_ATR_ACCEPT_SQC_DATE', index: 'EXEC_ATR_ACCEPT_SQC_DATE', height: 'auto', width: 70, align: "center", search: false, hidden: true },
                           { name: 'EXEC_RSA_GRADE_REVISED', index: 'EXEC_RSA_GRADE_REVISED', height: 'auto', width: 70, align: "center", search: false, hidden: true },
                           { name: 'e', width: 200, sortable: false, resize: false, align: "center", search: false, hidden: true }, // Finalize By Sqc

            

        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_RCD_CHAINAGE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Road Safety Auditor Inspection Details List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function ()
        {
            $("#tbCDWorksList #pagerCDWorksList").css({ height: '31px' });
           
            $("#pagerCDWorksList_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeRoad();return false;' value='Finalize Audit'/>");
            

            $("#pagerCDWorksList_right").html("<input type='button' style='margin-left:5px' id='idFinalizeExistingRoad1' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UploadPdfFileByAuditor();return false;' value='Upload / View PDF File'/>");
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured.");
                //alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}

function UploadPdfFileByAuditor() {

    var token = $('input[name=__RequestVerificationToken]').val();
    var EncryptedURL = $("#encryptedURLID").val();
    var id = EncryptedURL;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/PDFUploadByAuditorLayout/" + id,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {


            $("#dvAddMaintenanceAgreementAgainstRoad1").html(data);
            $('#accordion1').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error Occured.");
            //     alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


function RedirectFinalizeRoad() {
    var token = $('input[name=__RequestVerificationToken]').val();
    var EncryptedURL = $("#encryptedURLID").val();
    var id = EncryptedURL;
    //alert("ID  = " + id)
    if (confirm("Are you sure to finalize details ? Once Audit is finalized, chainage details can not be added against this Road")) {
        $.ajax({
            url: '/Execution/FinalizeByAuditor/' + id,
            type: "POST",
            cache: false,
            async: false,
            data: { "__RequestVerificationToken": token },
            success: function (response)
            {
                if (response.Success)
                {
                    alert("Details Finalized successfully");
                //    alert("data.Code = " + response.Code)
                    ViewAfterSave(response.Code)
                   
                }
                else
                {
                    alert(response.ErrorMessage)
                    //ViewAfterSave(data.Code)
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}


// UploadPhotoByAuditor

//Photo Upload 
function UploadPhotoByAuditor(id) {
 //   alert("Image Upload Auditor RSA ID = "+id)
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/ImageUploadByAuditor/" + id,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddMaintenanceAgreementAgainstRoad1").html(data);
            $('#accordion1').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');
            $.unblockUI();
         },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alert("Error Occured.");
         //   alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

//DeleteDetailsByAuditor

function DeleteDetailsByAuditor(id) {
    var token = $('input[name=__RequestVerificationToken]').val();

    if (confirm("Are you sure to delete details ?")) {
            $.ajax({
                url: '/Execution/DeleteByAuditor/' + id,
                type: "POST",
                cache: false,
                async: false,
                data: { "__RequestVerificationToken": token },
                success: function (response)
                {
                    if (response.Success)
                    {
                        alert("Details deleted successfully.");
                        //ViewAfterSave(response.Code)
                        $('#tbCDWorksList').trigger('reloadGrid');


                        $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");


                        if ($("#accordion1").is(":visible"))
                        {
                            $('#accordion1').hide('slow');
                        }

                        //alert("response.Code " + response.Code)
                       // ViewAfterSave(response.Code);
                    }
                    else
                    {
                         alert(response.ErrorMessage)
                         $('#tbCDWorksList').trigger('reloadGrid');


                         $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                         if ($("#accordion1").is(":visible")) {
                             $('#accordion1').hide('slow');
                         }
                    }
                    $.unblockUI();
                },
                error: function () {

                    $.unblockUI();
                    alert("Error : " + error);
                    return false;
                }
            });

        }
    }