
$(document).ready(function () {


    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $("#btnListProposal").click(function () {
        LoadITNOProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val());
    });

    if ($("#ddlDistrict").val() > 0) {
        $("#btnListProposal").trigger("click");
    }
    $("#idFilterDiv").click(function () {

        $("#divFilterForm").slideToggle("slow");

    });

    //divFilterForm

});


function LoadITNOProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM) {

    //alert("load");

        //$("#tbItnoProposalList").show();
        //$("#dvItnoProposalListPager").show();

    $('#tbItnoProposalList').jqGrid('GridUnload');

    //STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);

    blockPage();
    var pageWidth = $('#tbItnoProposalList').parent().width() - 100;
  
    jQuery("#tbItnoProposalList").jqGrid({
        url: '/Proposal/GetItnoRoadProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['District','Block', "Package Number", "Road Name", "Pavement Length", "Pavement Cost", "Add Test Result Details"],
        colModel: [
                    { name: 'District', index: 'District', width: (pageWidth * (11/100)), sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: (pageWidth * (11 / 100)), sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: (pageWidth * (11 / 100)), sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: (pageWidth * (20 / 100)), sortable: false, align: "left" },
                    { name: 'PavementLength', index: 'PavementLength', width: (pageWidth * (11 / 100)), sortable: false, align: "center" },
                    { name: 'PavementCost', index: 'PavementCost', width: (pageWidth * (11 / 100)), sortable: false, align: "center" },
                    { name: 'AddTestResultDetails', index: 'AddTestResultDetails', width: (pageWidth * (10 / 100)), sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM},
        pager: jQuery('#dvItnoProposalListPager'),
        rowList: [10, 15, 20],
        rowNum: 10,
        viewrecords: true,
        sortname: 'Block',  
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            //$("#tbStaProposalList #dvStaProposalListPager").css({ height: '31px' });

            //$("#dvStaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid
}


function AddTestResultDetails(IMS_PR_ROAD_CODE)
{ 
    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add Test Result Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseTestResultDetails();" /></a>'
                    );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divTestResultAddForm").load("/Proposal/TestResultDetails/id?",IMS_PR_ROAD_CODE, function () {
            
        //    //$.validator.unobtrusive.parse($('#divProposalForm'));
        //    unblockPage();
        //});
        
        $.ajax({
            url: '/Proposal/TestResultDetails/' + IMS_PR_ROAD_CODE,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {

                alert("Error occured while processing your request.");
                return false;
            },
            success: function (response) {
                $("#dvTestResultForm").html(response);
                unblockPage();
            }
        });

        $("#dvTestResultForm").css('height', 'auto');
        $('#dvTestResultForm').show('slow');
        $("#divFilterForm").hide("slow");


    });
    
    $("#IMS_PR_ROAD_CODE").val(IMS_PR_ROAD_CODE);

    $("#tbItnoProposalList").jqGrid('setGridState', 'hidden');
    
}

function CloseTestResultDetails()
{
    $('#accordion').hide('slow');
    $('#dvTestResultForm').hide('slow');
    $("#tbItnoProposalList").jqGrid('setGridState', 'visible');
    $("#divFilterForm").show("slow");
    
}