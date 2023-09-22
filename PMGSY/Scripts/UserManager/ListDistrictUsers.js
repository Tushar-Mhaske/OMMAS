var oldPIU;
var newPIU;
$(document).ready(function () {

    $("#btnSubmit").click(function () {
        LoadUserList();
        LoadProposalList();
    });

    $("#ddlStates").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                   "#ddlDistricts", "/UserManager/GetDistrictsByState?state=" + $('#ddlStates option:selected').val());

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                   "#ddlAgencies", "/UserManager/GetAgenciesByState?state=" + $('#ddlStates option:selected').val());
    });


    $("#ddlDistricts").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlPIU", "/UserManager/GetPIUOfDistrict?district=" + $('#ddlDistricts option:selected').val() + "&agency=" + $('#ddlAgencies option:selected').val());
    });

    $("#ddlAgencies").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlAgencies").find(":selected").val() },
                   "#ddlPIU", "/UserManager/GetPIUOfDistrict?district=" + $('#ddlDistricts option:selected').val() + "&agency=" + $('#ddlAgencies option:selected').val());
    });

    
    

    $("#dvPIU").dialog({
        autoOpen: false,
        height: 'auto',
        width: '300',
        modal: true,
        title: 'PIU Details'
    });


    $("#btnChange").click(function () {

        $.ajax({
            url: "/UserManager/ChangeProposalPIUMapping/",
            type: "POST",
            cache: false,
            data: { OLD_PIU_CODE: oldPIU, NEW_PIU_CODE: $("#ddlPIU option:selected").val(), State: $("#ddlStates option:selected").val(), District: $("#ddlDistricts option:selected").val(), value: Math.random() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.success == true) {
                    $("#dvPIU").dialog('close');
                    $("#tbUserList").trigger('reloadGrid');
                    $("#tbUserProposalList").trigger('reloadGrid');
                    alert('Proposal Mapping changed successfully.');
                }
                else {
                    alert('Error occurred while processing your request.');
                }
            }
        });

    });

});
function LoadUserList()
{
    $("#tbUserList").jqGrid('GridUnload');

    $("#tbUserList").jqGrid({
        url: '/UserManager/GetDistrictUserList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        
        colNames: ["UserID", "Username", "PIU Code", "PIU Name", "Change"],
        colModel: [
                     { name: 'UserId', index: 'UserId', width: 70, align: "center", hidden: false },
                     { name: 'UserName', index: 'UserName', width: 160, align: "left" },
                     { name: 'AdminNdCode', index: 'AdminNdCode', width: 70, align: "center", hidden: false },
                     { name: 'AdminName', index: 'AdminName', width: 250, align: "left" },
                     { name: 'Change', index: 'Change', width: 50, align: "center", search: false, hidden: true}
        ],
        postData: { State: $("#ddlStates option:selected").val(), District: $("#ddlDistricts option:selected").val(), Agency: $("#ddlAgencies option:selected").val() },
        viewrecords: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5,10, 15],
        pager: '#dvpgUserList',
        sortname: 'UserName',
        sortorder: 'asc',
        loadComplete: function (rowid) {

        },
        caption: "Users of District"
    });
}

function LoadProposalList() {

    $("#tbUserProposalList").jqGrid('GridUnload');

    $("#tbUserProposalList").jqGrid({
        url: '/UserManager/GetProposalPIUList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        
        colNames: ["PIU Code", "PIU Name","Proposal Count","Change"],
        colModel: [
                     { name: 'AdminNdCode', index: 'AdminNdCode', width: 100, align: "center", hidden: false },
                     { name: 'AdminName', index: 'AdminName', width: 250, align: "left" },
                     { name: 'ProposalCount', index: 'ProposalCount', width: 100, align: "center", hidden: false },
                     { name: 'Change', index: 'Change', width: 110, align: "center", search: false, hidden: false }
        ],
        postData: { State: $("#ddlStates option:selected").val(), District: $("#ddlDistricts option:selected").val(), Agency: $("#ddlAgencies option:selected").val() },
        viewrecords: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5,10, 15],
        pager: '#dvpgUserProposalList',
        sortname: 'AdminName',
        sortorder: 'asc',
        loadComplete: function (rowid) {

        },
        caption: "Users of Proposal"
    });
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
function PIUMapping(PIU_CODE)
{
    oldPIU = PIU_CODE;
    $("#dvPIU").dialog('open');
}

