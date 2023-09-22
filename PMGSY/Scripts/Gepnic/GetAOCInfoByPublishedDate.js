$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmGepnicTenderDetailsLayout3");
    LoadTenderList();
});
                                       
function LoadTenderList() {
    $("#tblstDPR3").jqGrid('GridUnload');
    jQuery("#tblstDPR3").jqGrid({
        url: '/Gepnic/ListAOCinfoDetailsBYPublishedDate',
        datatype: "json",
        mtype: "POST",
        colNames: ['Tender ID', 'Ref No.', 'WorkItem ID', 'Tender Desc.', 'Tender Type', 'Publish Date', 'No. of Bids', 'No. of Bids Received', 'Bidder Name', 'Bidder Address', 'Contract Date', 'Contract Number', 'Currency', 'Contract Value', 'Completion Date', 'Valid From', 'Valid To', 'Party Qualifiued', 'Party Not Qualifiued', 'ORG ID', 'ORG Name', 'Remarks', 'Return URL', 'Generated Date'],
        colModel: [
                
                        { name: 'A_TENDERID', index: 'A_TENDERID', height: 'auto', width: 140, align: "center", search: false },
                        { name: 'A_TENDERREFNO', index: 'A_TENDERREFNO', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'A_WORKITEMID', index: 'A_WORKITEMID', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'A_TENDER_DESC', index: 'A_TENDER_DESC', height: 'auto', width: 350, align: "center", search: false },
                        { name: 'A_TENDER_TYPE', index: 'A_TENDER_TYPE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'A_PUBLISHED_DATE', index: 'A_PUBLISHED_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'A_NO_OF_BIDS', index: 'A_NO_OF_BIDS', height: 'auto', width: 50, align: "center", search: false },
                        { name: 'A_NO_BIDS_RECD', index: 'A_NO_BIDS_RECD', height: 'auto', width: 50, align: "center", search: false },
                        { name: 'A_L1BIDDER_NAME', index: 'A_L1BIDDER_NAME', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'A_L1BIDDER_ADDRESS', index: 'A_L1BIDDER_ADDRESS', height: 'auto', width: 90, align: "center", search: false },
                        { name: 'A_CONTRACT_DATE', index: 'A_CONTRACT_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'A_CONTRACT_NO', index: 'A_CONTRACT_NO', height: 'auto', width: 50, align: "center", search: false },
                        { name: 'A_CURRENCY', index: 'A_CURRENCY', height: 'auto', width: 50, align: "center", search: false },
                        { name: 'A_CONTRACT_VALUE', index: 'A_CONTRACT_VALUE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'A_DATE_COMPLETION', index: 'A_DATE_COMPLETION', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'A_CONT_VALID_FROM', index: 'A_CONT_VALID_FROM', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'A_CONT_VALID_TO', index: 'A_CONT_VALID_TO', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'A_PARTIES_QUALIFIED', index: 'A_PARTIES_QUALIFIED', height: 'auto', width: 70, align: "center", search: false, hidden: false },
                        { name: 'A_PARTIES_NOTQUALIFIED', index: 'A_PARTIES_NOTQUALIFIED', height: 'auto', width: 70, align: "center", search: false, hidden: false },
                        { name: 'A_ORGID', index: 'A_ORGID', height: 'auto', width: 50, align: "center", search: false, hidden: false },
                        { name: 'A_ORGNAME', index: 'A_ORGNAME', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'A_REMARKS', index: 'A_REMARKS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'A_RETURN_URL', index: 'A_RETURN_URL', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'GENERATED_DATE', index: 'GENERATED_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
        ],
        //postData: { "IMS_YEAR": 0, "MAST_STATE_ID": $('#ddlState').val(), "MAST_DISTRICT_ID": $('#ddlDistrict').val(), 'MAST_BLOCK_ID': $('#ddlBlock').val(), "IMS_BATCH": $('#ddlBatch').val(), "IMS_STREAM": $('#ddlCollaboration').val(), "IMS_PROPOSAL_TYPE": $('#ddlImsProposalTypes').val(), "IMS_UPGRADE_COONECT": $('#ddlConnnectivityList').val(), "Package_Id": $('#ddlPackage').val(), "IMS_PROPOSAL_STATUS": "%" },

        pager: jQuery('#dvlstDPRPager3'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'A_TENDERREFNO',
        sortorder: "asc",
        caption: "&nbsp;&nbsp;List AOC Information By Created Date",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        //added by nilesh on date 29-08-2018
        jsonReader: { repeatitems: false },
        loadComplete: function () {
            //added by nilesh on date 29-08-2018
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }
    });
}

//function FormatColumn(cellvalue, options, rowObject) {

//    if (cellvalue != '')
//    {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click Here to View' onClick ='ViewDPRDetails(\"" + cellvalue.toString() + "\");'></span></center>";
//    }
//    else
//    {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
//    }
//}


//closes the view and display the DPR Proposal list
function CloseProposalDetails()
{
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tblstDPR").jqGrid('setGridState', 'visible');
    //
    
    $('#dvCorrList').hide('slow');
    $('#dvBidderList').hide('slow');
    $('#dvOpenEvalList').hide('slow');
    $('#dvAOCList').hide('slow');
    //dvAOCList
    

}



//This method is for save button click.
$("#btnView3").click(function () {
    
    if ($("#frmGepnicTenderDetailsLayout3").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Gepnic/GetAOCinfoDetailsBYPublishedDate",
            type: "POST",
            data: $("#frmGepnicTenderDetailsLayout3").serialize(),
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    alert("Details Added Successfully.")
                    LoadTenderList();
                    $.unblockUI();
                }
                else if (data.success == false) {
                    if (data.message != "")
                    {
                        alert(data.message);


                        LoadTenderList();


                        $.unblockUI();
                    }
                }
                else
                {
                    
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }


        });
    }
});