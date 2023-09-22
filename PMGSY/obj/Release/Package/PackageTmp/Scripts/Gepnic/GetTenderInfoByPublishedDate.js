$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmGepnicTenderDetailsLayout1");

  


    LoadTenderListDetails();



});

//This method is for save button click.
$("#btnView1").click(function () {
    if ($("#frmGepnicTenderDetailsLayout1").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Gepnic/GetTenderXMLFromGepenic",
            type: "POST",
            data: $("#frmGepnicTenderDetailsLayout1").serialize(),
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    alert("Details Added Successfully.")
                    LoadTenderList();
                    $.unblockUI();
                }
                else if (data.success == false) {
                    if (data.message != "") {
                        alert(data.message);


                        LoadTenderListDetails();


                        $.unblockUI();
                    }
                }
                else {

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




function LoadTenderListDetails() {
    $("#tblstDPR1").jqGrid('GridUnload');
    jQuery("#tblstDPR1").jqGrid({
        url: '/Gepnic/TenderListByPublishedDate',
        datatype: "json",
        mtype: "POST",
        colNames: ['Tender ID', 'Ref No.', 'Title', 'Pre Qual.', 'Location', 'Pincode', 'Currency', 'Fee', 'Value', 'EMD', 'Pub Date', 'Prebid Date', 'Doc Start', 'Doc End', 'BidSub Start', 'BidSub End', 'Bid Open', 'Inviting Officer', 'Inv Off Address', 'Prod Cat', 'Prodsub Cat', 'Tender Type', 'Tender Cat', 'Form Contract', 'Return URL', 'Remark', 'Org Name', 'Generated Date'],
        colModel: [
                        { name: 'T_ID', index: 'T_ID', height: 'auto', width: 140, align: "center", search: false },
                        { name: 'T_REF_NO', index: 'T_REF_NO', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'T_TITLE', index: 'T_TITLE', height: 'auto', width: 400, align: "center", search: false },
                        { name: 'T_PRE_QUAL', index: 'T_PRE_QUAL', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'T_LOCATION', index: 'T_LOCATION', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'T_PINCODE', index: 'T_PINCODE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_CURRENCY', index: 'T_CURRENCY', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'T_FEE', index: 'T_FEE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_VALUE', index: 'T_VALUE', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_EMD', index: 'T_EMD', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'T_PUB_DATE', index: 'T_PUB_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_PREBID_DATE', index: 'T_PREBID_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_DOC_START_DATE', index: 'T_DOC_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_DOC_END_DATE', index: 'T_DOC_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_BIDSUB_START_DATE', index: 'T_BIDSUB_START_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_BIDSUB_END_DATE', index: 'T_BIDSUB_END_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_BID_OPEN_DATE', index: 'T_BID_OPEN_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                        { name: 'T_INVITING_OFFICER', index: 'T_INVITING_OFFICER', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_INVITING_OFF_ADDRESS', index: 'T_INVITING_OFF_ADDRESS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_PROD_CAT', index: 'T_PROD_CAT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_PROD_SUB_CAT', index: 'T_PROD_SUB_CAT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_TENDER_TYPE', index: 'T_TENDER_TYPE', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_TENDER_CATEGORY', index: 'T_TENDER_CATEGORY', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_FORM_CONTRACT', index: 'T_FORM_CONTRACT', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_RETURN_URL', index: 'T_RETURN_URL', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_REMARKS', index: 'T_REMARKS', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'T_ORGNAME', index: 'T_ORGNAME', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                        { name: 'GENERATED_DATE', index: 'GENERATED_DATE', width: 60, align: 'left', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },         

        ],
        
        pager: jQuery('#dvlstDPRPager1'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'T_ID',
        sortorder: "asc",
        caption: "&nbsp;&nbsp;List Tender Information By Published Date",
	
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