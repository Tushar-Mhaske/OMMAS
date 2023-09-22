$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmFeedbackRep');
    
    if ($('#hdnRepStatus').val() == "I" || $('#hdnRepStatus').val() == "")
    {
        $('#rdbRepI').attr('checked', true);
    }
    else if ($('#hdnRepStatus').val() == "F") {
        $('#rdbRepF').attr('checked', true);
    }

    $("#btnCloseRep").click(function () {
        CloseFBReply();
    });

    $("#btnResetRep").click(function () {
        $("#txtFeedReply").empty();
    });


    if ($("#rdbRepI").is(":checked"))
    {// Intrim is chekced
        $('#lblPIUIntrim').show('slow');
        $('#lblPIUFinalYes').hide('slow');
        $('#lblPIUFinalNo').hide('slow');
        
        $('#lblIsActionTaken').hide('slow');
        $('#rdbYesDetails').hide('slow');
        $('#rdbNoDetails').hide('slow');

        $('#llYes').hide('slow');
        $('#llNo').hide('slow');

        $('#timeLineDateID').show('slow');
        // 

        $('#HereYes').hide('slow');

        $('#HereNo').hide('slow');

    }

    if ($("#rdbRepF").is(":checked"))
    {
        $('#lblPIUIntrim').hide('slow');


        $('#timeLineDateID').hide('slow');

        if ($(this).is(":checked")) {

            $('#llYes').show('slow');
            $('#llNo').show('slow');

            $('#lblIsActionTaken').show('slow');
            $('#rdbYesDetails').show('slow');
            $('#rdbNoDetails').show('slow');

            $('#HereYes').show('slow');

            $('#HereNo').show('slow');

          

           
        }



        if ($("#rdbYesDetails").is(":checked")) {

            $('#lblPIUFinalYes').show('slow');
            $('#lblPIUFinalNo').hide('slow');
        }


        if ($("#rdbNoDetails").is(":checked")) {
            $('#lblPIUFinalYes').hide('slow');
            $('#lblPIUFinalNo').show('slow');

        }

    }


    $("#rdbYesDetails").click(function () {

        $('#lblPIUFinalYes').show('slow');
        $('#lblPIUFinalNo').hide('slow');
    });

    $("#rdbNoDetails").click(function () {

        $('#lblPIUFinalYes').hide('slow');
        $('#lblPIUFinalNo').show('slow');
    });


    $("#rdbRepF").click(function () {
        // Final

        // 

        $('#timeLineDateID').hide('slow');

        if ($(this).is(":checked"))
        {

            $('#llYes').show('slow');
            $('#llNo').show('slow');

            $('#lblIsActionTaken').show('slow');
            $('#rdbYesDetails').show('slow');
            $('#rdbNoDetails').show('slow');

            $('#HereYes').show('slow');

            $('#HereNo').show('slow');

            $('#lblPIUIntrim').hide('slow');


            // For Below. Check FinalYes or Final No is checked or not
          





          

            if ($("#rdbYesDetails").is(":checked")) {

                $('#lblPIUFinalYes').show('slow');
                $('#lblPIUFinalNo').hide('slow');
            }


            if ($("#rdbNoDetails").is(":checked")) {
                $('#lblPIUFinalYes').hide('slow');
                $('#lblPIUFinalNo').show('slow');

            }


            // lblIsActionTaken
            //rdbYesDetails
            //rdbNoDetails
        }
       
    });



    $("#rdbRepI").click(function ()
    {
       // Intrim

        if ($(this).is(":checked")) {
            $('#lblPIUIntrim').show('slow');
            $('#lblPIUFinalYes').hide('slow');
            $('#lblPIUFinalNo').hide('slow');
            $('#timeLineDateID').show('slow');


            $('#lblIsActionTaken').hide('slow');
            $('#rdbYesDetails').hide('slow');
            $('#rdbNoDetails').hide('slow');



            $('#llYes').hide('slow');
            $('#llNo').hide('slow');

            $('#HereYes').hide('slow');

            $('#HereNo').hide('slow');
        }
        
    });







    $('#TIMELINE_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'ETA (Tentative Timeline to resolve the complaint)',
        onSelect: function (selectedDate) {
            //if ($('#tdAgreementDate .ui-datepicker-trigger').is(':visible')) {
            //    $("#MANE_AGREEMENT_DATE").datepicker("option", "maxDate", selectedDate);
            //}
            //$("#MANE_CONSTR_COMP_DATE").datepicker("option", "maxDate", selectedDate);
            //$("#MANE_HANDOVER_DATE").datepicker("option", "minDate", selectedDate);

         //   $("#TIMELINE_DATE").datepicker("option", "minDate", $("#TIMELINE_DATE").val());

        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    //$(document).on('click', 'input[name="rdbRepF"]', function () {
    //    alert("Final")
    //});


    $("#btnSubmitRep").click(function () {
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {


            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && $('input[name="Is_Action_Taken"]:checked').val() == "Y" && confirm("NOTE : You will also be required to upload an evidence (geotagged photo of site or pdf document) before you can forward the response to SQC. If the complaint was not genuine and no proof of resolution is available, you can select 'No'. In this case, you are not required to attach any proof.Do you want to continue with selecting 'Yes'? Once marked, you will not be able to change your response.")) || ($('input[name="Feed_Reply"]:checked').val() == "F" && $('input[name="Is_Action_Taken"]:checked').val() == "N") && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?")) {
                $.ajax({
                    url: "/FeedBackDetails/saveFeedBackRepNew/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("PIU Feedback Reply is saved Successfully.");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatusNew/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                $('#tbReplyStatus').show('slow');
                                $("#tbReplyStatus").css('height', 'auto');
                            });

                        }
                        else {
                            alert("Error occured while saving Feedback Reply");
                        }

                    },
                    error: function () {
                        alert("error");
                    }
                });
            }
            else {
                if (!($('input[name="Feed_Reply"]:checked').val() == "I") && !($('input[name="Feed_Reply"]:checked').val() == "F")) {
                    alert('Please select reply type');
                }
            }
        }
    });

    $("#btnUpdateRep").click(function () {


   //     alert("zc")
        var flag = false;
        if ($('#frmFeedbackRep').valid()) {
            if (($('input[name="Feed_Reply"]:checked').val() == "I") || ($('input[name="Feed_Reply"]:checked').val() == "F" && confirm("After Final Reply no modifications are allowed, are you sure you want to make Final Reply?"))) {
                $.ajax({
                    url: "/FeedBackDetails/updateFeedBackRepNew/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmFeedbackRep").serialize(),
                    success: function (data) {


                        if (data.status == true) {
                            alert("PIU Feedback Reply is updated Successfully.");

                            $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatusNew/' + $("#hdnFeedId").val(), function () {
                                $.validator.unobtrusive.parse($('#tbReplyStatus'));

                                $('#tbReplyStatus').show('slow');
                                $("#tbReplyStatus").css('height', 'auto');
                            });

                        }
                        else {
                            alert("Error occured while updating Feedback Reply");
                        }

                    },
                    error: function () {
                        alert("error");
                    }
                });
            }
            else {
                if (!($('input[name="Feed_Reply"]:checked').val() == "I") && !($('input[name="Feed_Reply"]:checked').val() == "F"))
                {
                    alert('Please select reply type');
                }
            }
        }
    });
});

function CloseFBReply() {
    
    $("#tbFBReplyStatusJqGrid").jqGrid('setGridState', 'visible');
    $("#tbFBReplyStatusJqGrid").trigger('reloadGrid');
    
    $('#dvReplyStatus').hide('slow');
    $('#btnAdd').show('slow');
   
}