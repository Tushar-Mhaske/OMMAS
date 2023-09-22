$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    

    LoadProposalDetails($('#RequestId').val());

});

function LoadProposalDetails(requestId)
{
    blockPage();
    jQuery("#tbProposalList").jqGrid({
        url: '/OnlineFund/GetProposalList',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Proposal Type","Package Number", "Year", "Road Name", "Pavement Length (in Kms.) / Bridge Name (in Mtrs.)", "Sanction Cost (in Lakhs)", "Maintenance Cost (in Lakhs)"],
        colModel: [
                    { name: 'District', index: 'District', width: 100, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                    { name: 'Type', index: 'Type', width: 100, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 100, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                    { name: 'PavementLength', index: 'PavementLength', width: 100, sortable: false, align: "right" },
                    { name: 'SanctionCost', index: 'SanctionCost', width: 100, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 100, sortable: false, align: "right" },
        ],
        postData: { "EncryptedId": requestId},
        pager: jQuery('#dvProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road / Bridge Proposals",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: false,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
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
        },
        beforeSelectRow: function (rowid, e) {
        }
    }); 
}