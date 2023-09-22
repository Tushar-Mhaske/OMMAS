this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        //$img
		//	.css("top", (trc_y - $img.height()) + "px")
		//	.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 50; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};
var isRoad;
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

    //




    $("#tabs").tabs();
    var ProposalCode = $("#proposalCode").val();
   // CheckProposalType(ProposalCode);
    if (isRoad == true)
    {
        LoadPhysicalRoadDetails(ProposalCode);
    }
    else if(isRoad == false)
    {
        LoadPhysicalLSBDetails(ProposalCode);
    }
    var Progress = $("#progressType").val();
    var RsaCode = $("#EXEC_RSA_CODE").val();

    LoadInspDetailsList(RsaCode);

    $('#btnSave').click(function () {
       SaveDetails();
    });


    $('#btnCancelByPIU').click(function () {

        $('#accordion').hide();  //close the form
        $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open

    });

    // Saving Master Only
    $('#btnSaveByPIU').click(function () {
      //  alert("here")
        btnSaveByPIUDetails();

    });



});

function btnSaveByPIUDetails() {
    $("#dvAddMaintenanceAgreementAgainstRoad1").html();
    $('#accordion1').hide('slow');
    $('#dvAddMaintenanceAgreementAgainstRoad1').hide('slow');

    var token = $('#frmRoadSafetyLayout1 input[name=__RequestVerificationToken]').val();
  
    if ($('#frmRoadSafetyLayout1').valid()) {

        if (confirm("Do you want to save ATR Details ?")) {

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddATRByPIU',
                method: 'POST',
                cache: false,
                async: true,
                header: token,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr)
                {
                    alert(data.message)
                    document.getElementById("frmRoadSafetyLayout1").reset();
                    $("#divHabitationDetails").hide('slow');

                    $("#dvAddMaintenanceAgreementAgainstRoad1").html();
                    $('#accordion1').hide('slow');
                    $('#dvAddMaintenanceAgreementAgainstRoad1').hide('slow');

                  

                    if (data.success)
                    {


                        $("#dvAddMaintenanceAgreementAgainstRoad1").html();
                        $('#accordion1').hide('slow');
                        $('#dvAddMaintenanceAgreementAgainstRoad1').hide('slow');

                        document.getElementById("frmRoadSafetyLayout1").reset();
                        $('#tbCDWorksList').trigger('reloadGrid');
                        $('#tbExecutionList').trigger('reloadGrid');
                        $("#divHabitationDetails").hide('slow');
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


function SaveRoadSafety1() {
    if ($('#frmRoadSafetyLayout1').valid()) {

        if (confirm("Do you want to save Details ? RSA Stage & Inspection Date can not be modified again once the details are saved. ")) {

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSA',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr) {
                    alert(data.message)
                    if (data.success)
                    {
                     $('#accordion').hide();  //close the form
                      $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
                    // alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }
    else
    {
      return false;
    }
}




function SaveDetails() {
    var RsaCode = $("#EXEC_RSA_CODE").val();

    if ($("#frmRoadSafetyLayout1").valid()) {
    if (confirm("Do you want to save Details ?")) {

       
            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSAdetails',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr)
                {
                    alert(data.message)
                  
                    $('#tbCDWorksList').trigger('reloadGrid');

                    document.getElementById("frmRoadSafetyLayout1").reset();

                    if (data.success)
                    {
                        LoadInspDetailsList(RsaCode);
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
                    //   alert(xhr.responseText);
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
        colNames: ['Start Chainage', 'End Chainage', 'Safety Issue', 'RSA Recommendation', 'Probability of Occurrence', 'Severity', 'View Image Uploaded By Auditor', 'Delete', 'Add ATR', 'Acceptance', 'ATR', 'ATR Date', 'Upload PDF File', 'View PDF', 'Add ATR (SQC)', 'Acceptance (SQC) ', 'ATR Date (SQC)', 'Revised Grade (SQC)', 'Finalize (SQC)'],//, 'Add ATR', 'Acceptance By PIU', 'Action Taken Remarks By PIU','ATR Upload Date By PIU'
        colModel: [
    
          // Auditor
                            { name: 'EXEC_RSA_START_CHAINAGE', index: 'EXEC_RSA_START_CHAINAGE', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_RSA_END_CHAINAGE', index: 'EXEC_RSA_END_CHAINAGE', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_RSA_SAFETY_ISSUE', index: 'EXEC_RSA_SAFETY_ISSUE', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'EXEC_RSA_RECOMMENDATION', index: 'EXEC_RSA_RECOMMENDATION', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'IMS_ROAD_STATUS', index: 'IMS_ROAD_STATUS', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'EXEC_RSA_GRADE', index: 'EXEC_RSA_GRADE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'a', width: 60, sortable: false, resize: false, align: "center", search: false },// Upload Image By Auditor
                            { name: 'z', width: 150, sortable: false, resize: false, align: "center", search: false, hidden:true },// Delete Record By Auditor // DeleteDetailsByAuditor

                            // PIU
                           { name: 'b', width: 90, sortable: false, resize: false, align: "center", search: false }, // Add ATR By PIU
                           { name: 'EXEC_RSA_REC_ACCP', index: 'EXEC_RSA_REC_ACCP', height: 'auto', width: 90, align: "center", search: false },
                           { name: 'EXEC_RSA_ACTION_TAKEN', index: 'EXEC_RSA_ACTION_TAKEN', height: 'auto', width: 200, align: "center", search: false },
                           { name: 'EXEC_ATR_DATE', index: 'EXEC_ATR_DATE', height: 'auto', width: 70, align: "center", search: false },
                           { name: 'c', width: 200, sortable: false, resize: false, align: "center", search: false }, //  View / Upload PDF File=  UploadPhotoByPIU

                           // SQC
                             { name: 'z', width: 40, sortable: false, resize: false, align: "center", search: false, hidden: true }, // View ATR Uploaded By PIU = ViewPdfUploadedByPIU
                           { name: 'd', width: 200, sortable: false, resize: false, align: "center", search: false, hidden:true }, // Add SQC Remarks
                           { name: 'EXEC_ATR_ACCEPT_SQC', index: 'EXEC_ATR_ACCEPT_SQC', height: 'auto', width: 70, align: "center", search: false },
                           { name: 'EXEC_ATR_ACCEPT_SQC_DATE', index: 'EXEC_ATR_ACCEPT_SQC_DATE', height: 'auto', width: 70, align: "center", search: false },
                           { name: 'EXEC_RSA_GRADE_REVISED', index: 'EXEC_RSA_GRADE_REVISED', height: 'auto', width: 120, align: "center", search: false },
                           { name: 'e', width: 200, sortable: false, resize: false, align: "center", search: false, hidden:true }, // Finalize By Sqc



        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_RCD_CHAINAGE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Inspection Details List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            $("#tbCDWorksList #pagerCDWorksList").css({ height: '31px' });

            $("#pagerCDWorksList_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeATRRoad();return false;' value='Finalize ATR'/>");


            $("#pagerCDWorksList_right").html("<input type='button' style='margin-left:5px' id='idFinalizeExistingRoad1' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UploadPdfFileByAuditor();return false;' value='View PDF (Uploaded By Auditor)'/>");

            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured.");
                //     alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}

// RedirectFinalizeATRRoad


function RedirectFinalizeATRRoad() {
    var urlparameter = $('#EncryptedRoadCode').val();

    var token = $('input[name=__RequestVerificationToken]').val();

    if (confirm("Are you sure to finalize details ?")) {
        $.ajax({
            url: '/Execution/FinalizeByPIU?urlparameter=' + urlparameter,
            type: "POST",
            cache: false,
            async: false,
            data: { "__RequestVerificationToken": token },
            success: function (response) {
                if (response.Success) {
                    alert("Details Finalized successfully");


                    $('#tbCDWorksList').trigger('reloadGrid');

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                }
                else {
                    alert(response.ErrorMessage);
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



function UploadPdfFileByAuditor() {

    var token = $('input[name=__RequestVerificationToken]').val();
    var EncryptedURL = $("#EncryptedRoadCode").val();
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



// View Image Uploaded By Auditor
function UploadPhotoByAuditor(parameter) {
   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/ImageUploadByAuditor/" + parameter,
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
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


// View / Upload Image Uploaded By PIU
function UploadPhotoByPIU(parameter) {
   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/ImageUploadByPIU/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data)
        {


            $("#dvAddMaintenanceAgreementAgainstRoad1").html(data);
            $('#accordion1').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alert("Error Occured.");
            //     alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


//AddDetailsByPIU
function AddDetailsByPIUJavaScript(paramurl)
{


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/GetDetailsEnteredByAuditor/" + paramurl,
        type: "GET",
        async: false,
        cache: false,

        success: function (data)
        {

           

            document.getElementById('startChainage').innerHTML = data.startChainage;
            document.getElementById('endChainage').innerHTML = data.endChainage;


            document.getElementById('IssueID').innerHTML = data.Issue;
            document.getElementById('RecommondationID').innerHTML = data.Recommondation;

            document.getElementById('SeverityDetailsID').innerHTML = data.SeverityDetails;
       

            //$("#dvAddMaintenanceAgreementAgainstRoad1").html(data);
            //$('#accordion1').show('slow');
            //$('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alert("Error Occured.");
            //     alert(xhr.responseText);
            $.unblockUI();
        }

    });

    $("#dvAddMaintenanceAgreementAgainstRoad1").html();
    $('#accordion1').hide('slow');
    $('#dvAddMaintenanceAgreementAgainstRoad1').hide('slow');

    $("#EncryptedRSAId").val(paramurl);
    $("#divHabitationDetails").show('slow');
}


