$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFilterProposal'));

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#btnListProposal").click(function () {

        
        if ($("#frmFilterProposal").valid()) {
            LoadProposalList();
        }
    });

    $("#ddlBlocks,#ddlYears,#ddlBatchs").change(function () {

        FillInCascadeDropdown({ userType: $(this).find(":selected").val() },
                   "#ddlPackages", "/Proposal/PopulatePackagesForRepackaging?id=" + $('#ddlBlocks option:selected').val() + "$" + $('#ddlYears option:selected').val() + "$" + $('#ddlBatchs option:selected').val());

    });




});
function LoadProposalList()
{
    $("#tblstProposalRepackage").jqGrid('GridUnload');

    jQuery("#tblstProposalRepackage").jqGrid({
        url: '/Proposal/GetProposalListForRepackaging',
        datatype: "json",
        mtype: "POST",
        postData: { blockCode: $("#ddlBlocks option:selected").val(), YearCode: $("#ddlYears option:selected").val(), BatchCode: $("#ddlBatchs option:selected").val(), Package: $("#ddlPackages option:selected").val(), Collaboration: $("#ddlCollaborations option:selected").val(), ProposalType: $("#ddlProposalTypes option:selected").val(), UpgradationType: $("#ddlUpgradationTypes option:selected").val() },
        colNames: ['Block', 'Batch', 'Year', 'Package No.', 'Road Name / Bridge Name', 'Road Length (in Kms) / Bridge Length (in Mtrs.)', 'Change'],
        colModel: [

                        { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 120, align: "center", search: false },
                        { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 200, align: "center", search: false },
                        { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                        { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 150, align: "right", search: false },
                        { name: 'Repackage', index: 'Repackage', height: 'auto', width: 70, align: "left", search: false },
        ],
        pager: jQuery('#dvlstPagerProposalRepackage'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ROAD_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Proposal List",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}
function AddRepackagingDetails(IMS_PR_ROAD_CODE)
{
    
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Repackaging Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddPackage").load("/Proposal/AddRepackagingDetails?" + $.param({ ProposalCode: IMS_PR_ROAD_CODE }), function () {
            $.validator.unobtrusive.parse($('#divAddPackage'));
            unblockPage();
        });
    
        $('#divAddPackage').show('slow');
        $("#divAddPackage").css('height', 'auto');
        $("#tblstProposalRepackage").jqGrid('setGridState', 'hidden');
    });
}
function CloseDetails() {
    $("#tblstProposalRepackage").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}