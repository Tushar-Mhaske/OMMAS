/* 
     *  Name : ListMPProposedRoads.js
     *  Path : ~/Scripts/MPMLAProposal/ListMPProposedRoads.js
     *  Description : ListMPProposedRoads.js is used to show MP Proposed Road list, for Filter Search and show Add MP Proposed Road Form
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 04/07/2013
 */
$(document).ready(function () {

    blockPage();

    //set drop down year And constituency to first value
    $("#ddlYear option:nth(1)").attr('selected', 'selected');
    $("#ddlConstituency option:nth(1)").attr('selected', 'selected');

    var Year = $("#ddlYear option:selected").val();
    var Constituency = $("#ddlConstituency option:selected").val();

    //display MP Proposed Road List
    LoadMPPropposedRoadList(Year, Constituency);
    
    //show/hide filter search 
    $("#idFilterDiv").click(function () {           
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    //display Existing Road data entry form
    $('#btnAddMPProposedRoad').click(function () {

        $("#accordion div").html("");
      
        $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add MP Proposed Road Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMpProposedRoadDetails();" /></a>'
                    );

        $('#accordion').show('fold', function () {
            blockPage();
            $("#divMPProposedRoadForm").load('/MPMLAProposal/AddEditMPProposedRoadDetails/', function () {
                $.validator.unobtrusive.parse($('#divMPProposedRoadForm'));
                unblockPage();
            });

            $('#divMPProposedRoadForm').show('slow');
            $("#divMPProposedRoadForm").css('height', 'auto');
        });
             
        $("#tbMPProposedRoadList").jqGrid('setGridState', 'hidden');
        
    });//end AddForm
    
    
    //// display MP Proposed Road grid by filter search criteria
    $("#btnMPProposedRoadList").click(function () {
        blockPage();
        SearchMPProposedRoadList();
        CloseMpProposedRoadDetails();
        unblockPage();
    });//end Search

    //initialize Accordian
    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    
});

//show MP Proposed road filter search
function showFilter()
{    
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

//this function is used to close Data entry form and show the Grid
function CloseMpProposedRoadDetails() {
    $('#accordion').hide('slow');
    $('#divMPProposedRoadForm').hide('slow');
    $("#tbMPProposedRoadList").jqGrid('setGridState', 'visible');
    showFilter();
}

//display Existing Road list
function LoadMPPropposedRoadList(Year, Constituency) {
    jQuery("#tbMPProposedRoadList").jqGrid({
        url: '/MPMLAProposal/GetMPProposedRoadList/',
            datatype: "json",
            mtype: "POST",
            colNames: ['Road Details', 'Year', 'Constituency', 'Is Included in Core Network', 'Is Included in Proposal', "Inclusion Details", "Edit", "Delete"],
            colModel: [
                            { name: 'IMS_ROAD_DETAILS', index: 'IMS_ROAD_DETAILS', width: 150, sortable: true, align: "left", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 100, sortable: true, align: "center", search: false },
                            { name: 'MAST_MP_CONST_NAME', index: 'MAST_MP_CONST_NAME', width: 100, sortable: true, align: "center", search: false },
                            { name: 'IMS_INCLUDED_IN_CN', index: 'IMS_INCLUDED_IN_CN', width: 200, sortable: true, align: "center", search: false },
                            { name: 'IMS_INCLUDED_IN_PROPOSAL', index: 'IMS_INCLUDED_IN_PROPOSAL', width: 200, sortable: true, align: "center", search: false },
                            { name: 'Inclusion', width: 40, sortable: false, resize: false, formatter: FormatColumnInclusionDetails, align: "center", sortable: false, search: false },
                            { name: 'Edit', width: 40, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false, search: false },
                            { name: 'Delete', width: 40, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false, search: false },
            ],
            postData: { "Year": Year, "Constituency": Constituency },
            pager: jQuery('#dvMPProposedRoadListPager'),
            rowNum: 10,
            sortorder: "asc",
            sortname:"IMS_ROAD_DETAILS",
            rowList: [5, 10, 15, 20],
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;MP Proposal Status",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            loadComplete: function () {
                //$("#tbExistingRoadsList #dvExistingRoadsListPager").css({ height: '31px' });
                unblockPage();
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Session Timeout !!!");
                    window.location.href = "/Login/LogIn";
                }
            }

    }); //end of grid

}


//MP Proposed Road grid fromat column
function FormatColumnInclusionDetails(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to go to inclusion details' onClick ='InclusionDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}
//MP Proposed Road grid fromat column
function FormatColumnEdit(cellvalue, options, rowObject) {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' onClick ='EditMPProposedRoadDetails(\"" + cellvalue.toString() + "\");'></span></center>";    
}

//MP Proposed Road grid fromat column
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete details of road' onClick ='DeleteMPProposedRoadDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}

// diplay Existing Road Data entery form in Edit mode
function EditMPProposedRoadDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Edit MP Proposed Road Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMpProposedRoadDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divMPProposedRoadForm").load('/MPMLAProposal/EditMPProposedRoadDetails/' + id, function () {
            $.validator.unobtrusive.parse($('divMPProposedRoadForm'));
            //$("#MAST_ER_ROAD_NAME").focus();
            unblockPage();
        });
        $("#divMPProposedRoadForm").show("slow");
        $("#divMPProposedRoadForm").css('height', 'auto');
    });

    $("#tbMPProposedRoadList").jqGrid('setGridState', 'hidden');   
}

//show the Existing Road Detail information
function InclusionDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >MP Proposal Inclusion Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMpProposedRoadDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divMPProposedRoadForm").load('/MPMLAProposal/AddEditMPProposalInclusionDetails/' + id, function () {
            $.validator.unobtrusive.parse($('divMPProposedRoadForm'));
            unblockPage();
        });
        $("#divMPProposedRoadForm").show("slow");
        $("#divMPProposedRoadForm").css('height', 'auto');
    });

    $("#tbMPProposedRoadList").jqGrid('setGridState', 'hidden');
}

// diplay Existing Road Data entery form in Edit mode
function DeleteMPProposedRoadDetails(id) {
    if (confirm("Are you sure to delete MP Proposed Road details ? ")) {
        $.ajax({
            url: '/MPMLAProposal/DeleteMPProposedRoadDetails/' + id,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $("#tbMPProposedRoadList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }
                    
                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}

//MP Proposed Road List Search
function SearchMPProposedRoadList()
{   
    var Year = $("#ddlYear option:selected").val();
    var Constituency = $("#ddlConstituency option:selected").val();

    $('#tbMPProposedRoadList').setGridParam({
        url: '/MPMLAProposal/GetMPProposedRoadList', datatype: 'json'
    });
    $('#tbMPProposedRoadList').jqGrid("setGridParam", { "postData": { Year: Year, Constituency: Constituency } });
    $('#tbMPProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);
}

