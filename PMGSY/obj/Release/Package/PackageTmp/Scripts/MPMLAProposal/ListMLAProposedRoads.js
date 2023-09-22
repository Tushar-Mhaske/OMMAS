/* 
     *  Name : ListMLAProposedRoads.js
     *  Path : ~/Scripts/MPMLAProposal/ListMLAProposedRoads.js
     *  Description : ListMLAProposedRoads.js is used to show MLA Proposed Road list, for Filter Search and show Add MLA Proposed Road Form
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 08/07/2013
 */
$(document).ready(function () {

    blockPage();

    //set drop down year And constituency to first value
    $("#ddlYear option:nth(1)").attr('selected', 'selected');
    $("#ddlConstituency option:nth(1)").attr('selected', 'selected');

    var Year = $("#ddlYear option:selected").val();
    var Constituency = $("#ddlConstituency option:selected").val();

    //display MLA Proposed Road List
    LoadMLAPropposedRoadList(Year, Constituency);
    
    //show/hide filter search 
    $("#idFilterDiv").click(function () {           
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    //display 
    $('#btnAddMLAProposedRoad').click(function () {

        $("#accordion div").html("");
      
        $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add MLA Proposed Road Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMLAProposedRoadDetails();" /></a>'
                    );

        $('#accordion').show('fold', function () {
            blockPage();
            $("#divMLAProposedRoadForm").load('/MPMLAProposal/AddEditMLAProposedRoadDetails/', function () {
                $.validator.unobtrusive.parse($('#divMLAProposedRoadForm'));
                unblockPage();
            });

            $('#divMLAProposedRoadForm').show('slow');
            $("#divMLAProposedRoadForm").css('height', 'auto');
        });
             
        $("#tbMLAProposedRoadList").jqGrid('setGridState', 'hidden');
        
    });//end AddForm
    
    
    //// display MLA Proposed Road grid by filter search criteria
    $("#btnMLAProposedRoadList").click(function () {
        blockPage();
        SearchMLAProposedRoadList();
        CloseMLAProposedRoadDetails();
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

//show MLA Proposed road filter search
function showFilter()
{    
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

//this function is used to close Data entry form and show the Grid
function CloseMLAProposedRoadDetails() {
    $('#accordion').hide('slow');
    $('#divMLAProposedRoadForm').hide('slow');
    $("#tbMLAProposedRoadList").jqGrid('setGridState', 'visible');
    showFilter();
}

//display MLA Proposed Road list
function LoadMLAPropposedRoadList(Year, Constituency) {
    jQuery("#tbMLAProposedRoadList").jqGrid({
        url: '/MPMLAProposal/GetMLAProposedRoadList/',
            datatype: "json",
            mtype: "POST",
            colNames: ['Road Details', 'Year', 'Constituency', 'Is Included in Core Network', 'Is Included in Proposal', "Inclusion Details", "Edit", "Delete"],
            colModel: [
                            { name: 'IMS_ROAD_DETAILS', index: 'IMS_ROAD_DETAILS', width: 200, sortable: true, align: "left", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 100, sortable: true, align: "center", search: false },
                            { name: 'MAST_MP_CONST_NAME', index: 'MAST_MP_CONST_NAME', width: 100, sortable: true, align: "center", search: false },
                            { name: 'IMS_INCLUDED_IN_CN', index: 'IMS_INCLUDED_IN_CN', width: 200, sortable: true, align: "center", search: false },
                            { name: 'IMS_INCLUDED_IN_PROPOSAL', index: 'IMS_INCLUDED_IN_PROPOSAL', width: 200, sortable: true, align: "center", search: false },
                            { name: 'Inclusion', width: 40, sortable: false, resize: false, formatter: FormatColumnInclusionDetails, align: "center", sortable: false, search: false },
                            { name: 'Edit', width: 40, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false, search: false },
                            { name: 'Delete', width: 40, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false, search: false },
            ],
            postData: { "Year": Year, "Constituency": Constituency },
            pager: jQuery('#dvMLAProposedRoadListPager'),
            rowNum: 10,
            sortorder: "asc",
            sortname:"IMS_ROAD_DETAILS",
            rowList: [5, 10, 15, 20],
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;MLA Proposal Status",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            loadComplete: function () {
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


//MLA Proposed Road grid fromat column
function FormatColumnInclusionDetails(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to go to inclusion details' onClick ='InclusionDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}
//MLA Proposed Road grid fromat column
function FormatColumnEdit(cellvalue, options, rowObject) {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' onClick ='EditMLAProposedRoadDetails(\"" + cellvalue.toString() + "\");'></span></center>";    
}

//MLA Proposed Road grid fromat column
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete details of road' onClick ='DeleteMLAProposedRoadDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}

// diplay MLA Proposed road data entery form in Edit mode
function EditMLAProposedRoadDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Edit MLA Proposed Road Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMLAProposedRoadDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divMLAProposedRoadForm").load('/MPMLAProposal/EditMLAProposedRoadDetails/' + id, function () {
            $.validator.unobtrusive.parse($('divMLAProposedRoadForm'));
            //$("#MAST_ER_ROAD_NAME").focus();
            unblockPage();
        });
        $("#divMLAProposedRoadForm").show("slow");
        $("#divMLAProposedRoadForm").css('height', 'auto');
    });

    $("#tbMLAProposedRoadList").jqGrid('setGridState', 'hidden');   
}

//show the Inclusion Detail information
function InclusionDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >MLA Proposal Inclusion Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseMLAProposedRoadDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divMLAProposedRoadForm").load('/MPMLAProposal/AddEditMLAProposalInclusionDetails/' + id, function () {
            $.validator.unobtrusive.parse($('divMLAProposedRoadForm'));
            unblockPage();
        });
        $("#divMLAProposedRoadForm").show("slow");
        $("#divMLAProposedRoadForm").css('height', 'auto');
    });

    $("#tbMLAProposedRoadList").jqGrid('setGridState', 'hidden');
}

// delete MLA Proposal road Details
function DeleteMLAProposedRoadDetails(id) {
    if (confirm("Are you sure to delete MLA Proposed Road details ? ")) {
        $.ajax({
            url: '/MPMLAProposal/DeleteMLAProposedRoadDetails/' + id,
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
                    $("#tbMLAProposedRoadList").trigger('reloadGrid');
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

//MLA Proposed Road List Search
function SearchMLAProposedRoadList()
{   
    var Year = $("#ddlYear option:selected").val();
    var Constituency = $("#ddlConstituency option:selected").val();

    $('#tbMLAProposedRoadList').setGridParam({
        url: '/MPMLAProposal/GetMLAProposedRoadList', datatype: 'json'
    });
    $('#tbMLAProposedRoadList').jqGrid("setGridParam", { "postData": { Year: Year, Constituency: Constituency } });
    $('#tbMLAProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);
}

