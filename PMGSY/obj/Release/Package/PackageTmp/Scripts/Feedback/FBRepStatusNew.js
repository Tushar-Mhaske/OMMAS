$(document).ready(function () {
    $('#frmFBFilesDisplay').html('');
    $("#btnAdd").click(function () {
        ShowDetails1($("#hdnFeedId").val());
    });

    $("#btnAddSQC").click(function () {
        ShowDetails2($("#hdnFeedId").val());
    });

    loadFBRepStatus();
});

function loadFBRepStatus() {
    $("#tbFBReplyStatusJqGrid").jqGrid({
        url: '/FeedbackDetails/FBRepStatListNew',
        datatype: "json",
        mtype: "POST",
        //postData: { FOR_MONTH: $("#ddlMonths option:selected").val(), FOR_YEAR: $("#ddlYears option:selected").val(), FOR_STATES: $("#ddlStates option:selected").val(), FOR_CATEGORY: $("#ddlCategories option:selected").val(), APPR: $("#ddlApproved option:selected").val(), STATUS: $("#ddlStatus option:selected").val() },
        postData: { id: $("#hdnFeedId").val() },
        colNames: ["Reply Id", "Reply Date", "Comment", 'Reply By', "Status", "ETA (Tentative Timeline to resolve the complaint)", "Was any action taken to address the complaint?", 'Image Upload / View', 'PDF Upload / View', ($("#RoleCode").val() == 22) ? 'Forward to SQC' : 'Reply Stage', "Edit", "Delete", 'Add Reply ( SQC )', 'Delete', 'Send Reply to complainer'],
        colModel: [

            { name: 'Reply Id', index: 'ReplyId', width: "90", hidden: true, sortable: true, align: "center" },
            { name: 'Reply Date', index: 'ReplyDate', width: "80", sortable: true, align: "center", search: false },
            { name: 'Comment', index: 'Comment', width: "300", sortable: true, align: "center", search: false },
            { name: 'PIU_SQC', index: 'PIU_SQC', width: "55", sortable: true, align: "center", search: false },
            { name: 'Status', index: 'Status', width: "150", sortable: true, align: "center", search: false },

              { name: 'date', index: 'date', width: "150", sortable: true, align: "center", search: false }, // Tentitive date


                { name: 'yesNo', index: 'yesNo', width: "150", sortable: true, align: "center", search: false }, // Is Action Taken 



            { name: 'a', width: 120, sortable: false, resize: false, align: "center", search: false}, // Upload Image by PIU
            { name: 'b', width: 120, sortable: false, resize: false, align: "center", search: false }, // Upload PDF by PIU
            { name: 'c', width: 150, sortable: false, resize: false, align: "center", search: false, hidden: ($("#RoleCode").val() == 8 ? true : false) }, // Forward To SQC

            { name: 'Update', index: 'Update', width: "50", sortable: false, align: "center", hidden:true  }, // (($("#RoleCode").val() == 8) || ($("#RoleCode").val() == 25) ? true : false)
            { name: 'Delete', index: 'Delete', width: "50", sortable: false, align: "center", hidden: (($("#RoleCode").val() == 8) || ($("#RoleCode").val() == 25) ? true : false) },
            { name: 'd', width: 120, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22) || ($("#RoleCode").val() == 25) ? true : false) }, // Add Details by SQC - AddReplayBySQC     hidden: ($("#RoleCode").val() == 22 ? true : false)

            { name: 'f', width: 120, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22) || ($("#RoleCode").val() == 25) ? true : false) }, //  DelBySQC  -- Deletion of details by SQC

            { name: 'e', width: 120, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22) || ($("#RoleCode").val() == 25) ? true : false) } // Finalize by SQC. Will be visible only at SQC Login



            ],
        pager: jQuery("#divFBRepStatusReportPager"),
        rownum: 100,
        viewrecords: true,
        recordtext: '{2} Records Found',
        caption: "Feedback Reply Status",
        height: "auto",
        autowidth: true,
        sortname: 'SrNo.',
        sortorder: 'asc',
        rownumbers: true,
        grouping: false,
        shrinkToFit: false,
        width: '100%',
        loadComplete: function () {
        },
        loadError: function (xhr, status, error)
        {
            if (xhr.responseText == "session expired")
            {
                //window.location.href = "/Login/SessionExpire";
            }
            else
            {
                //window.loc.href = "/Login/SessionExpire";
            }
        }
    });//end of grid
}


function UploadPhotoByPIU(id) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/FeedbackDetails/ImageUploadByPIU/" + id,
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
            $.unblockUI();
        }

    });
}




function UploadPDFByPIU(id) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/FeedbackDetails/PDFFileUploadByPIU/" + id,
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
            $.unblockUI();
        }

    });
}

function ShowDetails1(id) {

    $("#dvReplyStatus").load('/FeedbackDetails/FeedbackReplyNew/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));
        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
        $('#btnAdd').hide();
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');
}

function UpdateFBRep(id) {
    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

    if ($("#accordion1").is(":visible")) {
        $('#accordion1').hide('slow');
    }

    $('#tbCDWorksList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });


    $('#btnAdd').hide();
    $("#dvReplyStatus").load('/FeedbackDetails/FeedbackReplyUpdateNew/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));

        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');
}

function DeleteFBRep(id) {
    //alert(paramater);
    if (confirm("Are you sure you want to Delete Feedback Reply?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FeedbackDetails/deleteFeedBackRepNew/" + id,
            type: "GET",
            dataType: "json",
            //data: $("frmFeedbackRep").serialize(),
            success: function (data) {
                if (data.status == true) {
                    alert("Feedback Reply Deleted Successfully");
                    $('#tbFBReplyStatusJqGrid').trigger('reloadGrid');


                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });

                }
                else {

                    alert("Image / PDF details are added for this feedback reply. Please delete those details first.");

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error occured on Delete");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

function ForwardToSQC(id) {
    //alert(paramater);
    if (confirm("Are you sure you want to forward this Feedback Reply to SQC?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FeedbackDetails/ForwardToSQC/" + id,
            type: "GET",
            dataType: "json",
            //data: $("frmFeedbackRep").serialize(),
            success: function (data) {
                if (data.status == true) {
                    alert("Feedback Reply forwared to SQC Successfully.");

                    $('#tbFBReplyStatusJqGrid').trigger('reloadGrid');



                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }
                else {
                    alert("Image or PDF Files are mandatory. Please upload these files and then forward to SQC.");

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occured on Delete");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}




//  SQC Method Starts Here 
function AddReplayBySQC(id) {
  //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    
    var id = $("#hdnFeedId").val();

    $("#dvReplyStatus").load('/FeedbackDetails/SQCFeedbackReplyNew/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));
        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
        $('#btnAdd').hide();
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');

}





function ShowDetails2(id) {

    $("#dvReplyStatus").load('/FeedbackDetails/SQCFeedbackReplyNew/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));
        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
        $('#btnAddSQC').hide();
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');
}



function UploadPhotoBySQC(id) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/FeedbackDetails/ImageUploadBySQC/" + id,
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
            $.unblockUI();
        }

    });
}


function UploadPDFBySQC(id) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/FeedbackDetails/PDFFileUploadBySQC/" + id,
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
            $.unblockUI();
        }

    });
}





function FinalizeBySQC(id) {
    //alert(paramater);
    if (confirm("Are you sure you want to send this Feedback Reply to complainer ? After sending these details, modifications can not be allowed.")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FeedbackDetails/SQCFinalize/" + id,
            type: "GET",
            dataType: "json",
            //data: $("frmFeedbackRep").serialize(),
            success: function (data) {
                if (data.status == true) {
                    alert("Feedback Reply sent to complainer Successfully.");

                    $('#tbFBReplyStatusJqGrid').trigger('reloadGrid');



                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }
                else {
                    alert("Image or PDF Files are mandatory. Please upload these files and then send it.");

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occured on Delete");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}





//DelBySQC




function DelBySQC(id) {
    //alert(paramater);
    if (confirm("Are you sure you want to delete this Feedback Reply ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FeedbackDetails/DelBySQC/" + id,
            type: "GET",
            dataType: "json",
            //data: $("frmFeedbackRep").serialize(),
            success: function (data) {
                if (data.status == true)
                {
                    alert("Feedback Reply details deleted Successfully.");

                    $('#tbFBReplyStatusJqGrid').trigger('reloadGrid');

                    $("#tbReplyStatus").load('/FeedbackDetails/FBRepStatusNew/' + $("#hdnFeedId").val(), function () {
                        $.validator.unobtrusive.parse($('#tbReplyStatus'));

                        $('#tbReplyStatus').show('slow');
                        $("#tbReplyStatus").css('height', 'auto');
                    });


                  //  $("#Closed").hide();

                  //  $("#Review").hide();

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }
                else
                {
                    alert("Please delete Image and PDF Files details first.");

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }

                    $('#tbCDWorksList').jqGrid("setGridState", "visible");

                    $("#dvAgreement").animate({
                        scrollTop: 0
                    });
                }

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occured on Delete");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

