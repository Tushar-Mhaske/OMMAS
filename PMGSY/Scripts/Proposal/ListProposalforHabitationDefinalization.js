$(document).ready(function () {

    
    $("#btnListProposal").click(function () {

        if ($("#ddlDistrict").val() == '0') {
            alert('Please select district.');
            return false;
        }
        LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val());
    });

    $("#ddlState").change(function () {
        if ($("#ddlState").val() > 0) {

            $("#ddlDistrict").empty();

            $.ajax({
                url: '/Proposal/GetDistricts',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { MAST_STATE_CODE: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

    

});
function LoadMordProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM) {

        $("#tbMORDLSBProposalList").hide();
        $("#dvMORDLSBProposalListPager").hide();
        $("#tbMORDProposalList").show();
        $("#dvMORDProposalListPager").show();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');

        MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM);
}
function MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM) {

    blockPage();
    jQuery("#tbMORDProposalList").jqGrid({
        url: '/Proposal/GetMordProposalsforHabitationDefinalization',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "Pavement Length (in Kms.)", "Sanction Cost (in Lakhs)",  "Definalization"],
        colModel: [
                    { name: 'District', index: 'District', width: 150, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 150, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 120, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 220, sortable: false, align: "left" },
                    { name: 'PavementLength', index: 'PavementLength', width: 100, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 100, sortable: false, align: "right" },
                    { name: 'ShowDetailsHabitationFinalize', index: 'ShowDetailsHabitationFinalize', width: 100, sortable: false, align: "center", search: false }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM },
        pager: jQuery('#dvMORDProposalListPager'),
        rowList: [10, 20, 30],
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        autowidth: false,
        width: 1180,
        sortname: 'District',
        rownumbers: true,
        shrinkToFit: false,
        loadComplete: function (data) {
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
function DefinalizeHabitation(id)
{
    $.ajax({
        url: '/Proposal/DefinalizeHabitation/'+id,
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        
        success: function (jsonData) {
            if (jsonData.success == true) {
                alert("Habitations Definalized Successfully.");
                jQuery("#tbMORDProposalList").trigger('reloadGrid');
            } else {

                alert("Error occurred while processing your request.");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });
}
