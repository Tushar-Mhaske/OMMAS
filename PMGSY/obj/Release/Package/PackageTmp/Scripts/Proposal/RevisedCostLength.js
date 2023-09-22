$(document).ready(function () {

    $.validator.unobtrusive.parse("frmAddRevisedCostLength");

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    $("#divAddRevisedDetails").load('/Proposal/AddRevisionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#divAddRevisedDetails'));
        unblockPage();
        $("#IMS_NEW_PAV_COST,#IMS_NEW_CD_COST,#IMS_NEW_PW_COST,#IMS_NEW_OW_COST,#IMS_NEW_FC_COST").trigger('blur');
    });

    
    $.ajax({

        type : 'POST',
        url: '/Proposal/GetProposalType/',
        data: { id: $("#IMS_PR_ROAD_CODE").val() },
        error: function () { },
        success: function (response) {
            if (response.data != "")
            {
                if (response.data == "P") {
                    LoadRevisionRoadList($("#IMS_PR_ROAD_CODE").val());
                }
                else if (response.data == "L") {
                    LoadRevisionBridgeList($("#IMS_PR_ROAD_CODE").val());
                }
            }
        }

    });

    

});
function LoadRevisionRoadList(IMS_PR_ROAD_CODE) {

    $("#tblstRevisedCost").jqGrid('GridUnload');

    jQuery("#tblstRevisedCost").jqGrid({
        url: '/Proposal/GetRevisedCostLengthList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode:IMS_PR_ROAD_CODE},
        //colNames: ['Road Length', 'CdWorks', 'Other Works', 'Pavement', 'Protection Works', 'State Share', 'Year1', 'Year2', 'Year3', 'Year4', 'Year5', 'Road Length', 'CdWorks', 'Other Works', 'Pavement', 'Protection Works', 'State Share', 'Year1', 'Year2', 'Year3', 'Year4', 'Year5', 'Edit'],
        colNames: ['Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Old', 'New', 'Edit'],
        colModel: [
                            
                            { name: 'IMS_OLD_CD_COST', index: 'IMS_OLD_CD_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_CD_COST', index: 'IMS_NEW_CD_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_OW_COST', index: 'IMS_OLD_OW_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_OW_COST', index: 'IMS_NEW_OW_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_PAV_COST', index: 'IMS_OLD_PAV_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_PAV_COST', index: 'IMS_NEW_PAV_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_PW_COST', index: 'IMS_OLD_PW_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_PW_COST', index: 'IMS_NEW_PW_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_RS_COST', index: 'IMS_OLD_RS_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_RS_COST', index: 'IMS_NEW_RS_COST', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_MAINT1', index: 'IMS_OLD_MAINT1', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_MAINT1', index: 'IMS_NEW_MAINT1', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_MAINT2', index: 'IMS_OLD_MAINT2', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_MAINT2', index: 'IMS_NEW_MAINT2', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_MAINT3', index: 'IMS_OLD_MAINT3', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_MAINT3', index: 'IMS_NEW_MAINT3', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_MAINT4', index: 'IMS_OLD_MAINT4', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_MAINT4', index: 'IMS_NEW_MAINT4', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_OLD_MAINT5', index: 'IMS_OLD_MAINT5', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_MAINT5', index: 'IMS_NEW_MAINT5', height: 'auto', width: 80, align: "center", search: false },
                            //{ name: 'IMS_OLD_BS_COST', index: 'IMS_OLD_BS_COST', height: 'auto', width: 50, align: "center", search: false },
                            //{ name: 'IMS_OLD_BW_COST', index: 'IMS_OLD_BW_COST', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'IMS_OLD_LENGTH', index: 'IMS_OLD_LENGTH', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NEW_LENGTH', index: 'IMS_NEW_LENGTH', height: 'auto', width: 80, align: "center", search: false },
                            //{ name: 'IMS_NEW_BS_COST', index: 'IMS_NEW_BS_COST', height: 'auto', width: 50, align: "center", search: false },
                            //{ name: 'IMS_NEW_BW_COST', index: 'IMS_NEW_BW_COST', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'a', width: 40, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlstRevisedCost'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_REVISION_CODE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Revised Cost Length List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth:true,
        shrinkToFit:false,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

    jQuery("#tblstRevisedCost").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'IMS_OLD_CD_COST', numberOfColumns: 2, titleText: 'CdWorks Cost' },
          { startColumnName: 'IMS_OLD_OW_COST', numberOfColumns: 2, titleText: 'Other Works Cost' },
          { startColumnName: 'IMS_OLD_PAV_COST', numberOfColumns: 2, titleText: 'Pavement Cost' },
          { startColumnName: 'IMS_OLD_PW_COST', numberOfColumns: 2, titleText: 'Protection Works Cost' },
          { startColumnName: 'IMS_OLD_RS_COST', numberOfColumns: 2, titleText: 'State Share Cost' },
          { startColumnName: 'IMS_OLD_MAINT1', numberOfColumns: 2, titleText: 'Maintenance Cost Year1' },
          { startColumnName: 'IMS_OLD_MAINT2', numberOfColumns: 2, titleText: 'Maintenance Cost Year2' },
          { startColumnName: 'IMS_OLD_MAINT3', numberOfColumns: 2, titleText: 'Maintenance Cost Year3' },
          { startColumnName: 'IMS_OLD_MAINT4', numberOfColumns: 2, titleText: 'Maintenance Cost Year4' },
          { startColumnName: 'IMS_OLD_MAINT5', numberOfColumns: 2, titleText: 'Maintenance Cost Year5' },
          { startColumnName: 'IMS_OLD_LENGTH', numberOfColumns: 2, titleText: 'Pavement Length' },
          { startColumnName: 'a', numberOfColumns: 1, titleText: 'Action' },
          { startColumnName: 'Year1', numberOfColumns: 5, titleText: 'Maintenance' },
          //{ startColumnName: 'IMS_OLD_MAINT1', numberOfColumns: 5, titleText: 'Maintenance Cost' },
          //{ startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: 'Payment Made(Rs. in Lakh)' }
        ]
    });
}
function EditRevisionDetails(urlparameter)
{
    $("#divAddRevisedDetails").load("/Proposal/EditRevisedDetails/" + urlparameter, function (data) {
        if ((data.success == false)) {
            alert(data.message);
        }
        $("#divAddRevisedDetails").show();
        $.validator.unobtrusive.parse($('#frmAddRevisedCostLength'));
        $("#IMS_NEW_PAV_COST,#IMS_NEW_CD_COST,#IMS_NEW_PW_COST,#IMS_NEW_OW_COST,#IMS_NEW_FC_COST").trigger('blur');
    });
}
function LoadRevisionBridgeList(IMS_PR_ROAD_CODE) {

    $("#tblstRevisedCost").jqGrid('GridUnload');

    jQuery("#tblstRevisedCost").jqGrid({
        url: '/Proposal/GetRevisionBridgeList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_PR_ROAD_CODE },
        colNames: ['Old', 'New', 'Old', 'New', 'Old', 'New', 'Edit'],
        colModel: [
                            { name: 'IMS_OLD_BS_COST', index: 'IMS_OLD_BS_COST', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_NEW_BS_COST', index: 'IMS_NEW_BS_COST', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_OLD_BW_COST', index: 'IMS_OLD_BW_COST', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_NEW_BW_COST', index: 'IMS_NEW_BW_COST', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_OLD_LENGTH', index: 'IMS_OLD_LENGTH', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'IMS_NEW_LENGTH', index: 'IMS_NEW_LENGTH', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'a', width: 100, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerlstRevisedCost'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_REVISION_CODE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Revised Cost Length List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

    jQuery("#tblstRevisedCost").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
            { startColumnName: 'IMS_OLD_BS_COST', numberOfColumns: 2, titleText: 'Bridge Works Cost' },
            { startColumnName: 'IMS_OLD_BW_COST', numberOfColumns: 2, titleText: 'State Share Cost' },
         //{ startColumnName: 'IMS_OLD_MAINT1', numberOfColumns: 2, titleText: 'Maintenance Year1 Cost' },
         // { startColumnName: 'IMS_OLD_MAINT2', numberOfColumns: 2, titleText: 'Maintenance Year2 Cost' },
         // { startColumnName: 'IMS_OLD_MAINT3', numberOfColumns: 2, titleText: 'Maintenance Year3 Cost' },
         // { startColumnName: 'IMS_OLD_MAINT4', numberOfColumns: 2, titleText: 'Maintenance Year4 Cost' },
         // { startColumnName: 'IMS_OLD_MAINT5', numberOfColumns: 2, titleText: 'Maintenance Year5 Cost' },
          { startColumnName: 'IMS_OLD_LENGTH', numberOfColumns: 2, titleText: 'Length' },
          { startColumnName: 'a', numberOfColumns: 1, titleText: 'Action' },
          //{ startColumnName: 'IMS_OLD_MAINT1', numberOfColumns: 5, titleText: 'Maintenance Cost' },
          //{ startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: 'Payment Made(Rs. in Lakh)' }
        ]
    });


}