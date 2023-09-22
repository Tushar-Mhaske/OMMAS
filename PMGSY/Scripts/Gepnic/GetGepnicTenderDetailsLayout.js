$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmGepnicTenderDetailsLayout");

    PopulateRefNoList();

    $('#txtRefNo').change(function () {
        PopulateRefNoList();
    });


    LoadTenderList();



});
//$("btnView").click(function () {

//    alert("clicked");
//});
function PopulateRefNoList() {
    //if ($("#ddlBankName option:selected").val() != "") {
    $.ajax({
        url: "/Gepnic/PopulatePackageRefNo",
        cache: false,
        type: "GET",
        async: false,
        //data: { bankName: $("#ddlBankName option:selected").text(), },
        success: function (data) {

            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#txtRefNo').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 14,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true,
                deferRequestBy: 200
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                //  alert(xhr.responseText);
                alert('An Error occurred while processing your request');
                window.location.href = "/Login/LogIn";
            }
        }
    })
    //}
}


                                       
function LoadTenderList() {
    $("#tblstDPR").jqGrid('GridUnload');
    jQuery("#tblstDPR").jqGrid({
        url: '/Gepnic/TenderList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Ref No.', 'Org Chain', 'T Ref', 'Tender Type.', 'T Contract', 'T Tender Category', 'W Title', 'W Description', 'W Pre Qual', 'W Prod Cat', 'W Prod Sub Cat', 'W Value', 'W Bidval', 'W Locaiton', 'W Pincode', 'W Meet Place', 'W Prebid Address', 'W Bid opening Place', 'Inviting Officer', 'W Inviting Off Address', 'W Tender fee', 'W Paybale To', 'W Paybale AT', 'W EMD Fee', 'EMD Payble To', 'EMD Payble AT', 'W PUB Date', 'W Doc Start Date', 'W Doc End Date', 'W Seel Clear Start Date', 'W Seel Clear End Date', 'W Prebid Date', 'W Bidsub Start Date', 'W Bidsub End Date', 'W Bid Open Date', 'W Fin Bid Open Date', 'W Bid Openers', 'No of Bids', 'W Return URL','View'],
        colModel: [
                
                        { name: 'WORKITEM_REF_NO', index: 'WORKITEM_REF_NO', height: 'auto', width: 140, align: "center", search: false },
                        { name: 'ORG_CHAIN', index: 'ORG_CHAIN', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_REF_NO', index: 'T_REF_NO', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_TENDER_TYPE', index: 'T_TENDER_TYPE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_FORM_OF_CONTRACT', index: 'T_FORM_OF_CONTRACT', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_TENDER_CATEGORY', index: 'T_TENDER_CATEGORY', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'W_TITLE', index: 'W_TITLE', height: 'auto', width: 200, align: "center", search: false },
                        { name: 'W_DESC', index: 'W_DESC', height: 'auto', width: 200, align: "center", search: false },
                        { name: 'W_PRE_QUAL', index: 'W_PRE_QUAL', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'W_PROD_CAT', index: 'W_PROD_CAT', height: 'auto', width: 90, align: "center", search: false },
                        { name: 'W_PROD_SUB_CAT', index: 'W_PROD_SUB_CAT', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'W_VALUE', index: 'W_VALUE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'W_BIDVALIDITY', index: 'W_BIDVALIDITY', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'W_LOCATION', index: 'W_LOCATION', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'W_PINCODE', index: 'W_PINCODE', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'W_PREBID_MEET_PLACE', index: 'W_PREBID_MEET_PLACE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_PREBID_ADDRESS', index: 'W_PREBID_ADDRESS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_BID_OPENING_PLACE', index: 'W_BID_OPENING_PLACE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_INVITING_OFFICER', index: 'W_INVITING_OFFICER', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_INVITING_OFF_ADDRESS', index: 'W_INVITING_OFF_ADDRESS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_TENDER_FEE', index: 'W_TENDER_FEE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_TF_PAYABLE_TO', index: 'W_TF_PAYABLE_TO', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_TF_PAYABLE_AT', index: 'W_TF_PAYABLE_AT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_EMD_FEE', index: 'W_EMD_FEE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_EMD_PAYABLE_TO', index: 'W_EMD_PAYABLE_TO', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_EMD_PAYABLE_AT', index: 'W_EMD_PAYABLE_AT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'W_PUB_DATE', index: 'W_PUB_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'W_DOC_START_DATE', index: 'W_DOC_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'W_DOC_END_DATE', index: 'W_DOC_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_SEEK_CLAR_START_DATE', index: 'W_SEEK_CLAR_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_SEEK_CLAR_END_DATE', index: 'W_SEEK_CLAR_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_PREBID_DATE', index: 'W_PREBID_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_BIDSUB_START_DATE', index: 'W_BIDSUB_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_BIDSUB_END_DATE', index: 'W_BIDSUB_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_BID_OPEN_DATE', index: 'W_BID_OPEN_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_FIN_BID_OPEN_DATE', index: 'W_FIN_BID_OPEN_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, hidden: true },
                        { name: 'W_BID_OPENERS', index: 'W_BID_OPENERS', height: 'auto', width: 120, align: "center", search: false, hidden: true },
                        { name: 'W_NO_OF_BIDS', index: 'W_NO_OF_BIDS', height: 'auto', width: 90, align: "center", search: false, hidden: true },
                        { name: 'W_RETURN_URL', index: 'W_RETURN_URL', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }                                                                 
     ],
        //postData: { "IMS_YEAR": 0, "MAST_STATE_ID": $('#ddlState').val(), "MAST_DISTRICT_ID": $('#ddlDistrict').val(), 'MAST_BLOCK_ID': $('#ddlBlock').val(), "IMS_BATCH": $('#ddlBatch').val(), "IMS_STREAM": $('#ddlCollaboration').val(), "IMS_PROPOSAL_TYPE": $('#ddlImsProposalTypes').val(), "IMS_UPGRADE_COONECT": $('#ddlConnnectivityList').val(), "Package_Id": $('#ddlPackage').val(), "IMS_PROPOSAL_STATUS": "%" },

        pager: jQuery('#dvlstDPRPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'WORKITEM_REF_NO',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; List Workitem Details",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}




function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '')
    {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click Here to View' onClick ='ViewDPRDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
    else
    {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function ViewDPRDetails(id) {

   LoadCorrigendumList(id);
   LoadBidderList(id);
   BidOpenEvalListDetails(id);
   ACOListDetails(id);


    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' ></a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function ()
    {
        blockPage();
        $("#divProposalForm").load('/Gepnic/ViewTenderDetails?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');

        // 30 July 2018
        $('#dvCorrList').show('slow');
        $('#dvBidderList').show('slow');
        $('#dvOpenEvalList').show('slow');
        $('#dvAOCList').show('slow');

    });

    $("#tblstDPR").jqGrid('setGridState', 'hidden');

}
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


function LoadCorrigendumList(id) {
    $("#tblstCorr").jqGrid('GridUnload');
    jQuery("#tblstCorr").jqGrid({
        url: '/Gepnic/CorrigendumList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Corrigendum Name', 'Corrigendum Type', 'Corrigendum Publish Date', 'Corrigendum Publish Type'],
        colModel: [

                        { name: 'CORR_NAME', index: 'CORR_NAME', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'CORR_TYPE', index: 'CORR_TYPE', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'CORR_PUB_DATE', index: 'CORR_PUB_DATE', height: 'auto', width: 155, align: "center", search: false },
                        { name: 'CORR_PUB_BY', index: 'CORR_PUB_BY', height: 'auto', width: 200, align: "center", search: false }

        ],
        postData: { "TENDER_ID": id },
        pager: jQuery('#dvlstCorrPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'CORR_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Corrigendum List",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}



function LoadBidderList(id) {
    $("#tblstBidder").jqGrid('GridUnload');

    jQuery("#tblstBidder").jqGrid({
        url: '/Gepnic/BidderList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Bid ID', 'Bidder Name', 'Bid Placed Date', 'Bid IP Address'],
        colModel: [

                        { name: 'GEPNIC_BID_ID', index: 'GEPNIC_BID_ID', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BIDDER_NAME', index: 'BIDDER_NAME', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BID_PLACED_DATE', index: 'BID_PLACED_DATE', height: 'auto', width: 155, align: "center", search: false },
                        { name: 'BID_IP_ADDRESS', index: 'BID_IP_ADDRESS', height: 'auto', width: 200, align: "center", search: false },
        ],
        postData: { "TENDER_ID": id },

        pager: jQuery('#dvlstBidderPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'GEPNIC_BID_ID',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Bidder List",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}


function BidOpenEvalListDetails(id) {
    $("#tblstOpenEval").jqGrid('GridUnload');
    jQuery("#tblstOpenEval").jqGrid({
        url: '/Gepnic/BidOpenEvalList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Bid ID', 'Bidder Name', 'Bid Opened Date', 'Bid Opened By','Type'],
        colModel: [

                        { name: 'GEPNIC_BID_ID', index: 'GEPNIC_BID_ID', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BIDDER_NAME', index: 'BIDDER_NAME', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'BID_OPENED_DATE', index: 'BID_OPENED_DATE', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BID_OPENED_BY', index: 'BID_OPENED_BY', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BID_OPEN_EVAL_TYPE', index: 'BID_OPEN_EVAL_TYPE', height: 'auto', width: 100, align: "center", search: false },
        ],
        postData: { "TENDER_ID": id },

        pager: jQuery('#dvlstOpenEvalPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'GEPNIC_BID_ID',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Technical and Financial Details",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}



function ACOListDetails(id) {
    $("#tblstAOC").jqGrid('GridUnload');
    jQuery("#tblstAOC").jqGrid({
        url: '/Gepnic/AOCList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Bid ID', 'Bidder Name', 'Contract Date', 'Contract Number', 'Awarded Value'],
        colModel: [

                        { name: 'GEPNIC_BID_ID', index: 'GEPNIC_BID_ID', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'BIDDER_NAME', index: 'BIDDER_NAME', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'CONTRACT_DATE', index: 'CONTRACT_DATE', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'CONTRACT_NUMBER', index: 'CONTRACT_NUMBER', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'AWARDED_VALUE', index: 'AWARDED_VALUE', height: 'auto', width: 100, align: "center", search: false },
        ],
        postData: { "TENDER_ID": id },

        pager: jQuery('#dvlstAOCPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'GEPNIC_BID_ID',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; AOC Details",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}


//This method is for save button click.
$("#btnView").click(function () {
    if ($("#frmGepnicTenderDetailsLayout").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Gepnic/GetDataFromGepenic",
            type: "POST",
            data: $("#frmGepnicTenderDetailsLayout").serialize(),
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