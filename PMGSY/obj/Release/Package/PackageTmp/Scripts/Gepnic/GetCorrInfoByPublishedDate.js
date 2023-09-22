$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmGepnicTenderDetailsLayout2");
    LoadTenderList();
});
                                       
function LoadTenderList() {
    $("#tblstDPR2").jqGrid('GridUnload');
    jQuery("#tblstDPR2").jqGrid({
        url: '/Gepnic/ListCorrinfoDetailsBYPublishedDate',
        datatype: "json",
        mtype: "POST",
        colNames: ['C ID', 'C Date', 'C Type', 'C Status', 'T Ref No.', 'T ID', 'C Title', 'C Desc', 'T Pre Qual', 'T Location', 'T Pincode', 'T Currency', 'T Fee', 'T Value', 'T EMD', 'T Pub Date', 'T Prebid Date', 'Doc Start', 'Doc End', 'Bid Sub Start', 'Bid Sub End', 'Bid Open Date', 'Inviting Officer', 'Inviting Off Address', 'Prod Cat', 'Prod Sub Cat', 'Tender Type', 'Tender Category', 'T Frorm Contract', 'Return URL', 'Remarks', 'Tender Corr', 'Generated Date'],
        colModel: [
                        { name: 'C_ID', index: 'C_ID', height: 'auto', width: 50, align: "center", search: false },
                        { name: 'C_DATE', index: 'C_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'C_TYPE', index: 'C_TYPE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'C_STATUS', index: 'C_STATUS', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_REF_NO', index: 'T_REF_NO', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_ID', index: 'T_ID', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'C_TITLE', index: 'C_TITLE', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'C_DESC', index: 'C_DESC', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'T_PRE_QUAL', index: 'T_PRE_QUAL', height: 'auto', width: 120, align: "center", search: false },
                        { name: 'T_LOCATION', index: 'T_LOCATION', height: 'auto', width: 120, align: "center", search: false },
                        { name: 'T_PINCODE', index: 'T_PINCODE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_CURRENCY', index: 'T_CURRENCY', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_FEE', index: 'T_FEE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_VALUE', index: 'T_VALUE', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'T_EMD', index: 'T_EMD', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'T_PUB_DATE', index: 'T_PUB_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_PREBID_DATE', index: 'T_PREBID_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: false },
                        { name: 'T_DOC_START_DATE', index: 'T_DOC_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'T_DOC_END_DATE', index: 'T_DOC_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'T_BIDSUB_START_DATE', index: 'T_BIDSUB_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'T_BIDSUB_END_DATE', index: 'T_BIDSUB_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'T_BID_OPEN_DATE', index: 'T_BID_OPEN_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'T_INVITING_OFFICER', index: 'T_INVITING_OFFICER', height: 'auto', width: 120, align: "center", search: false, hidden: false },
                        { name: 'T_INVITING_OFF_ADDRESS', index: 'T_INVITING_OFF_ADDRESS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_PROD_CAT', index: 'T_PROD_CAT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_PROD_SUB_CAT', index: 'T_PROD_SUB_CAT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_TENDER_TYPE', index: 'T_TENDER_TYPE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_TENDER_CATEGORY', index: 'T_TENDER_CATEGORY', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_FORM_CONTRACT', index: 'T_FORM_CONTRACT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_RETURN_URL', index: 'T_RETURN_URL', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_REMARKS', index: 'T_REMARKS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'TENDER_CORRIGENDUM', index: 'TENDER_CORRIGENDUM', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'GENERATED_DATE', index: 'GENERATED_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } }                                           
        ],
        pager: jQuery('#dvlstDPRPager2'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'T_REF_NO',
        sortorder: "asc",
        caption: "&nbsp;&nbsp;List Corrigendum Information By Published Date",
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
$("#btnView2").click(function () {
    
    if ($("#frmGepnicTenderDetailsLayout2").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Gepnic/GetCorrinfoDetailsBYPublishedDate",
            type: "POST",
            data: $("#frmGepnicTenderDetailsLayout2").serialize(),
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


                      //  LoadTenderList();


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