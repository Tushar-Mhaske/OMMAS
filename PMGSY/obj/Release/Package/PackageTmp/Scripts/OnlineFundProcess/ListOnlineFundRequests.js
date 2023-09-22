$(document).ready(function () {

    $('#btnListRequest').click(function () {
        if ($('#frmFilterRequest').valid()) {
            LoadOnlineFundRequests();
        }
        else {
            return false;
        }
    });

    $('#btnAddRequestForm').click(function () {

        $('#divAddRequestDetails').load('/OnlineFund/AddOnlineFundRequest', function () {
            $('#dvFilterRequest').hide();
            $('#divAddRequestDetails').show('slow');
            $('#btnAddRequestForm').hide('slow');
            $.validator.unobtrusive.parse($('#frmAddRequest'));
        });

    });

    $('#imgCloseDetails').click(function () {

        $('#dvFilterRequest').show('slow');
        $('#divAddRequestDetails').hide('slow');
        $('#btnAddRequestForm').show('slow');
    });


    $('#ddlStates').change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlAgencies", "/OnlineFund/GetAgenciesByState?stateCode=" + $('#ddlStates option:selected').val());

    });

    //$("#draggable").draggable();
    //$("#draggable").draggable({ cursor: "move", cursorAt: { top: 225, left: 300 } });
    $("#draggable").load('/OnlineFund/ViewRequestStatusDetails');

    setTimeout(function () {

        //$("#draggable").show();

        $("#draggable").css({
            "position": "absolute",
            "width": "600px",
            "height": "300px",
            "top": "500px",
            "left": "650px",
            //"border": "1px solid black"
        });
    }, 500);

});
function LoadOnlineFundRequests()
{
    $("#tblstRequests").jqGrid('GridUnload');

    jQuery("#tblstRequests").jqGrid({
        url: '/OnlineFund/GetListOfOnlineFundRequests',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), CollaborationCode: $("#ddlCollaboration option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), PMGSYScheme: $("#ddlSchemes option:selected").val(), AgencyCode: $("#ddlAgencies option:selected").val() },
        colNames: ['State', 'Year', 'Batch', 'Agency', 'Scheme', 'Installment No.', 'Total No. of Road Works', 'Total No. of Bridge Works', 'Total Sanctioned Cost (Rs. in Cr.)', 'File No.', 'File Date', 'Request Amount', 'Upload', 'View Proposal Details', 'Finalize', 'View / Fill Observation', 'View', 'Reply on Condition', 'Delete', 'Regenerate Request'],
        colModel: [

                            { name: 'STATE', index: 'STATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'BATCH', index: 'BATCH', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'AGENCY', index: 'AGENCY', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'PMGSY_SCHEME', index: 'PMGSY_SCHEME', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'Installment', index: 'Installment', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'TOTAL_ROAD_PROPOSALS', index: 'TOTAL_ROAD_PROPOSALS', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_BRIDGE_PROPOSALS', index: 'TOTAL_BRIDGE_PROPOSALS', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_SANCTION_AMOUNT', index: 'TOTAL_SANCTION_AMOUNT', height: 'auto', width: 100, align: "center", search: false,formatter:'currency' },
                            { name: 'FILE_NO', index: 'FILE_NO', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'FILE_DATE', index: 'FILE_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RELEASE_AMOUNT', index: 'RELEASE_AMOUNT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'UploadDetails', index: 'UploadDetails', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'ProposalDetails', index: 'ProposalDetails', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'Finalize', index: 'Finalize', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'Observation', index: 'Observation', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'VIEW', index: 'VIEW', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'REPLY', index: 'REPLY', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'Delete', index: 'Delete', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'Regenerate', index: 'Regenerate', height: 'auto', width: 50, align: "center", search: false },
                            
        ],
        pager: jQuery('#dvlstPagerRequests'),
        rowNum: 20,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FILE_NO',
        sortorder: "desc",
        caption: 'Online Fund Requests',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        footerrow: true,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}
function UpdateOnlineFundRequest(id) {
    $('#divAddRequestDetails').load('/OnlineFund/AddOnlineFundRequest', function () {
        $('#dvFilterRequest').hide();
        $('#divAddRequestDetails').show('slow');
        $('#btnAddRequestForm').hide('slow');
        $.validator.unobtrusive.parse($('#frmAddRequest'));
    });
}
function ViewRequestDetails(id)
{
    //$('#divAddRequestDetails').load('/OnlineFund/ViewFundRequest/' + id, function () {
    //    $('#dvFilterRequest').hide();
    //    $('#divAddRequestDetails').show('slow');
    //    $('#btnAddRequestForm').hide('slow');
    //    $.validator.unobtrusive.parse($('#frmAddRequest'));
    //});

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Request Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/OnlineFund/CompleteRequestDetails/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#tblstRequests').jqGrid('setGridState', 'hidden');


}
function ViewProposalDetails(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/OnlineFund/ViewProposalDetails/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#tblstRequests').jqGrid('setGridState','hidden');
}
function ViewRequestObservation(requestId) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/OnlineFund/AddObservationDetails/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#tblstRequests').jqGrid('setGridState', 'hidden');
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $('#tblstRequests').jqGrid('setGridState', 'visible');
    $('#imgCloseDetails').trigger('click');
}
function FinalizeRequestDetails(requestId)
{
    if (confirm("Are you sure to finalize the request?")) {

        $.ajax({

            type: 'POST',
            url: '/OnlineFund/FinalizeRequestDetails/' + requestId,
            async: false,
            cache: false,
            success: function (data)
            {
                if (data.Success == true) {
                    alert('Request Finalized Successfully.');
                    $('#tblstRequests').trigger('reloadGrid');
                }
                else {
                    alert(data.ErrorMessage);
                }
            },
            error: function ()
            {
                alert('Error occurred while processing your request.');
            }

        });
    }
    else {
        return false;
    }
}
function RegenerateRequestDetails(requestId) {
    if (confirm("Are you sure to regenerate the request?")) {

        $.ajax({

            type: 'POST',
            url: '/OnlineFund/RegenerateRequestDetails/' + requestId,
            async: false,
            cache: false,
            success: function (data) {
                if (data.Success == true) {
                    alert('Request Regenerated Successfully.');
                    $('#tblstRequests').trigger('reloadGrid');
                }
                else {
                    alert(data.Message);
                }
            },
            error: function () {
                alert('Error occurred while processing your request.');
            }

        });
    }
    else {
        return false;
    }
}

function UploadDetails(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/OnlineFund/UploadDetails/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#tblstRequests').jqGrid('setGridState', 'hidden');
}
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}
function AddConditionReply(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/OnlineFund/AddConditionReply/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#tblstRequests').jqGrid('setGridState', 'hidden');
}
function DeleteRequestDetails(requestId)
{
    if (confirm("Are you sure to delete the request?")) {

        $.ajax({

            type: 'POST',
            url: '/OnlineFund/DeleteOnlineFundRequest/' + requestId,
            async: false,
            cache: false,
            success: function (data) {
                if (data.Success == true) {
                    alert('Request deleted Successfully.');
                    $('#tblstRequests').trigger('reloadGrid');
                }
                else {
                    alert(data.ErrorMessage);
                }
            },
            error: function () {
                alert('Error occurred while processing your request.');
            }

        });
    }
    else {
        return false;
    }
}
