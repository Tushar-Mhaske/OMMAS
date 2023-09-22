$(document).ready(function () {

    $.validator.unobtrusive.parse("#FilterForm");

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    if ($("#StateCode").val() > 0) {
        $("#ddlState").attr('disabled', true);

    }
    if ($("#DistrictCode").val() > 0) {
        $("#ddlDistrict").attr('disabled', true);
    }

    $("#ddlState").change(function () {
        loadDistrict($("#ddlState").val());
        setTimeout(function () {
            $("#ddlDistrict").trigger('blur');
        }, 1000);

    });

    $("#ddlDistrict").change(function () {
        loadBlock($("#ddlState").val(), $("#ddlDistrict").val());

    });

    $("#ddlBlock,#ddlImsYear,#ddlBatch").change(function () {

        FillInCascadeDropdown({ userType: $(this).find(":selected").val() },
                   "#ddlPackage", "/Proposal/PopulatePackagesForRepackaging?id=" + $('#ddlBlock option:selected').val() + "$" + 0 + "$" + $('#ddlBatch option:selected').val());

    });
    //population of DPR Proposals
    $('#btnListProposal').click(function () {
        if ($("#FilterForm").valid()) {
            LoadDPRProposalList();
        }
    });
    $('#btnListProposal').trigger('click');

});
//jqgrid code for population of DPR Proposals
function LoadDPRProposalList() {
    $("#tblstDPR").jqGrid('GridUnload');

    jQuery("#tblstDPR").jqGrid({
        url: '/Proposal/DPRProposalList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', 'Batch', 'Year', 'Package No.', 'Road Name / Bridge Name', 'Road Length (in Kms) / Bridge Length (in Mtrs.)', "Total Cost (Rs. in Lakhs)", 'View', 'Delete', 'Update Year'],
        colModel: [

                        { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 200, align: "center", search: false },
                        { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 130, align: "center", search: false, hidden: true },
                        { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 200, align: "center", search: false },
                        { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                        { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 150, align: "right", search: false },
                        { name: 'IMS_TOTAL_COST', index: 'IMS_TOTAL_COST', height: 'auto', width: 150, align: "right", search: false },
                        { name: 'View', index: 'View', height: 'auto', width: 60, align: "left", search: false },
                        { name: 'Delete', index: 'Delete', height: 'auto', width: 60, align: "left", search: false, hidden: false },
                        { name: 'UpdateYear', index: 'UpdateYear', height: 'auto', width: 60, align: "left", search: false, hidden: ((parseInt($('#RoleID').val()) == 25 || parseInt($('#RoleID').val()) == 2) ? false : true) },
        ],
        postData: { "IMS_YEAR": 0, "MAST_STATE_ID": $('#ddlState').val(), "MAST_DISTRICT_ID": $('#ddlDistrict').val(), 'MAST_BLOCK_ID': $('#ddlBlock').val(), "IMS_BATCH": $('#ddlBatch').val(), "IMS_STREAM": $('#ddlCollaboration').val(), "IMS_PROPOSAL_TYPE": $('#ddlImsProposalTypes').val(), "IMS_UPGRADE_COONECT": $('#ddlConnnectivityList').val(), "Package_Id": $('#ddlPackage').val(), "IMS_PROPOSAL_STATUS": "%" },

        pager: jQuery('#dvlstDPRPager'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_ROAD_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; DPR Proposal List",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        loadComplete: function () { },
        loadError: function () { }
    });
}
//display of DPR details view
function ViewDPRDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/Details?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tblstDPR").jqGrid('setGridState', 'hidden');

}
//closes the view and display the DPR Proposal list
function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tblstDPR").jqGrid('setGridState', 'visible');
}
//deletes the DPR Proposal
function DeleteDPRDetails(IMS_PR_ROAD_CODE) {
    if (confirm("Are you sure to Delete Road Proposal Details ? ")) {
        $.ajax({
            url: "/Proposal/DeleteDPRProposal?proposalCode=" + IMS_PR_ROAD_CODE,
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
                unblockPage();
                if (response.success == true) {
                    alert('Proposal deleted successfully.');
                    $("#tblstDPR").trigger('reloadGrid');
                }
                else {
                    alert('Error occurred while processing your request.');
                }
            },
        });
    }
    else {
        return false;
    }
}

function loadDistrictSRRDA(statCode) {

    $("#ddlDistrict").val(0);
    $("#ddlDistrict").empty();
    $("#ddlBlock").val(0);
    $("#ddlBlock").empty();
    $("#ddlBlock").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#ddlDistrict").length > 0) {
            $.ajax({
                url: '/Proposal/DistrictDetailsforDPRSRRDA',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#ddlDistrict').find("option[value='0']").remove();
                    //$("#ddlDistrict").append("<option value='0'>Select District</option>");
                    //$('#ddlDistrict').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#ddlDistrict").val($("#Mast_District_Code").val());
                        // $("#ddlDistrict").attr("disabled", "disabled");
                        $("#ddlDistrict").trigger('change');
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlDistrict").append("<option value='0'>All Districts</option>");
        $("#ddlBlock").empty();
        $("#ddlBlock").append("<option value='0'>All Blocks</option>");

    }
}


function loadDistrict(statCode) {

    $("#ddlDistrict").val(0);
    $("#ddlDistrict").empty();
    $("#ddlBlock").val(0);
    $("#ddlBlock").empty();
    $("#ddlBlock").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#ddlDistrict").length > 0) {
            $.ajax({
                url: '/Proposal/DistrictDetailsforDPR',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#ddlDistrict').find("option[value='0']").remove();
                    //$("#ddlDistrict").append("<option value='0'>Select District</option>");
                    //$('#ddlDistrict').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#ddlDistrict").val($("#Mast_District_Code").val());
                        // $("#ddlDistrict").attr("disabled", "disabled");
                        $("#ddlDistrict").trigger('change');
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlDistrict").append("<option value='0'>All Districts</option>");
        $("#ddlBlock").empty();
        $("#ddlBlock").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#ddlBlock").val(0);
    $("#ddlBlock").empty();

    if (districtCode > 0) {
        if ($("#ddlBlock").length > 0) {
            $.ajax({
                url: '/Proposal/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#ddlBlock").val($("#Mast_Block_Code").val());
                        // $("#ddlBlock").attr("disabled", "disabled");
                        //$("#ddlBlock").trigger('change');
                    }
                    //$('#ddlBlock').find("option[value='0']").remove();
                    //$("#ddlBlock").append("<option value='0'>Select Block</option>");
                    //$('#ddlBlock').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#ddlBlock").append("<option value='0'>All Blocks</option>");
    }
}

//Fill Pakage 
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

//deletes the DPR Proposal
function EditDPRYear(roadCode) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Update DPR Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/EditYearDPRProposalLayout?roadCode=' + roadCode, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tblstDPR").jqGrid('setGridState', 'hidden');

}