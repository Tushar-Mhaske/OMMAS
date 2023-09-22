$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmsearchRoadwiseQualityDetails');

    // Populate District
    $("#ddlStates").change(function () {

        // on state change district,block,road list will be empty
        $('#ddlDistricts').empty();
        $('#ddlSearchBlocks').empty();
        $('#ddlSearchBlocks').append("<option value=" + 0 + "> Select Block </option>");
        $('#ddlSearchRoad').empty();
        $('#ddlSearchRoad').append("<option value=" + 0 + "> Select Road </option>");

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val(),
            type: 'POST',
            cache: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while processing your request.");
                return false;
            },
            success: function (response) {
                $.each(response, function () {
                    $('#ddlDistricts').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            },
        });

    });//end of Populate District

    // Populate Blocks
    $("#ddlDistricts").change(function () {

        // on state change district,block,road list will be empty
        $('#ddlSearchBlocks').empty();
        $('#ddlSearchRoad').empty();
        $('#ddlSearchRoad').append("<option value=" + 0 + "> Select Road </option>");

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val(),
            type: 'POST',
            cache: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while processing your request.");
                return false;
            },
            success: function (response) {                
                $.each(response, function () {
                    $('#ddlSearchBlocks').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            },
        });

    }); //end of Populate Blocks

    // Populate Roads
    $("#ddlSearchBlocks").change(function () {
        $('#ddlSearchRoad').empty();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetAllRoadsByBlockCode?blockCode=" + $('#ddlSearchBlocks option:selected').val(),
            type: 'POST',
            cache: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while processing your request...");
                return false;
            },
            success: function (response) {               
                $.each(response, function () {
                    $('#ddlSearchRoad').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            },
        });

    });//end of Populate Blocks

});


// on road change hide 
// Proposal, Quality, Inspection Detail Div to reload properly
$('#ddlSearchRoad').change(function () {
    $("#dvProposalDetail").hide();
    $("#dvQualityDetail").hide();
    $("#dvInspectionDetail").hide();
});



$("#btnGetDetails").click(function () {
    if ($("#frmsearchRoadwiseQualityDetails").valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetProposalDetails?planRoadCode=" + $('#ddlSearchRoad option:selected').val(),
            type: 'GET',
            cache: false,
            error: function (xhr, status, error) {
                alert("An error occured while processing your request...");
                $.unblockUI();
                return false;
            },
            success: function (response) {                
                $("#dvProposalDetail").html(response);
                $("#dvProposalDetail").show();
                $.unblockUI();
            },
        });
       
    }
});

//======================= GetProposalDetails.cshtml Page ================================

// To Show Nested table details on click plus
// icon of GetProposalDetails page
function ViewSubTableDetails(proposalCode) {

    // to get element by id
    var el = document.getElementById('subTable+' + proposalCode);
    if (el && el.style.display == 'inline-table')
        el.style.display = 'none';
    else
        el.style.display = 'inline-table';

};

function ViewQualityDetails(RoadCode) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetQualityDetails?roadCode=" + RoadCode,
        type: 'GET',
        cache: false,
        error: function (xhr, status, error) {
            alert("An error occured while processing your request...");
            $.unblockUI();
            return false;
        },
        success: function (response) {
            $("#dvQualityDetail").html(response);
            $("#dvQualityDetail").show();
            $.unblockUI();
        },
    });

}


// to close current div
function closeCurrentDiv(currentDivId, currentDivName) {

    if (confirm("Do You Want To Close " + currentDivName + "?")) {

        // to get element by id
        var el = document.getElementById(currentDivId);
        el.style.display = 'none';
    }   
}


function ViewQualityInspectionDetails(observationId, qmType) {       //observationId,qmType
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/RoadwiseQualityDetails/RoadwiseQualityInformation/GetInspectionGradingDetails?obsidqmtype=" + observationId + "$" + qmType,
        type: 'GET',
        cache: false,
        error: function (xhr, status, error) {
            alert("An error occured while processing your request...");
            $.unblockUI();
            return false;
        },
        success: function (response) {
            $("#dvInspectionDetail").html(response);
            $("#dvInspectionDetail").show();
            $.unblockUI();
        },
    });

}
