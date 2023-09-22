$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFilterMaintenanceProposal'));

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

 //   alert("Before List")
    LoadProposalList();

  //  $("#btnListProposal").trigger("click");

  //  $("#ddlSRRDA").trigger("change");

    $("#btnListProposal").click(function () {
       
        if ($("#frmFilterMaintenanceProposal").valid()) {
            LoadProposalList();
        }
    });

    $("#ddlBlocks,#ddlYears,#ddlBatchs").change(function () {

        FillInCascadeDropdown({ userType: $(this).find(":selected").val() },
                   "#ddlPackages", "/MaintenanceAgreement/PopulateEmargMaintenancePackagesForRepackaging?id=" + $('#ddlBlocks option:selected').val() + "$" + $('#ddlYears option:selected').val() + "$" + $('#ddlBatchs option:selected').val());

    });



    $("#ddlStates").change(function () {

        loadDistrictHere($("#ddlStates option:selected").val());
    });


});


function loadDistrictHere(statCode) {
    $("#ddlBlocks").val(0);
    $("#ddlBlocks").empty();

    if (statCode > 0) {
        if ($("#ddlBlocks").length > 0) {
            $.ajax({
                url: '/MaintenanceAgreement/DistrictDetailsHere',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBlocks").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //For Disable if District Login
                    if ($("#Block").val() > 0) {
                        $("#ddlBlocks").val($("#Block").val());
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

        $("#ddlDistricts").append("<option value='0'>All Districts</option>");

    }
}


function LoadProposalList() {
    if ($("#ddlBlocks option:selected").val() == 0 || $("#ddlBlocks option:selected").val() == -1) {
        alert("Please Select District")
        return null
    }
    $("#tblstProposalRepackage").jqGrid('GridUnload');
  
    jQuery("#tblstProposalRepackage").jqGrid({
        url: '/MaintenanceAgreement/GetEmargMaintenanceProposalListForRepackaging',
        datatype: "json",
        mtype: "POST",
        postData: { blockCode: $("#ddlBlocks option:selected").val(), StateCode: $("#ddlStates option:selected").val(), BatchCode: $("#ddlBatchs option:selected").val(), Package: $("#ddlPackages option:selected").val(), Collaboration: $("#ddlCollaborations option:selected").val(), ProposalType: $("#ddlProposalTypes option:selected").val(), UpgradationType: $("#ddlUpgradationTypes option:selected").val() },
        colNames: ['State Name', 'District Name','Block Name', 'Old Package No.', 'Current Package No.','Road Name',  'Repackage', 'Finalize'],
        colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 120, align: "center", search: false },
                         { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 120, align: "center", search: false },

                       { name: 'OLD_PACKAGE_NO', index: 'OLD_PACKAGE_NO', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'PACKAGE_NO', index: 'PACKAGE_NO', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                     //   { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },
                     
                        { name: 'a', index: 'a', height: 'auto', width: 120, align: "center", search: false },
                         { name: 'b', index: 'b', height: 'auto', width: 120, align: "center", search: false },
        ],
        pager: jQuery('#dvlstPagerProposalRepackage'),
        rowNum: 5000,
        //  rowNum: 15,
        rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
       // rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ROAD_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Emarg Works for Repackaging",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ["PACKAGE_NO"],
            groupColumnShow: [false],
            groupText: [

            
                "<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all Repackaged roads in this Package, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackageForRepackage(\'' + "{0}" + '\')">'


                //  + "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' 

            ],
            groupOrder: ["asc"],
            groupSummary: [true],
            groupSummaryPos: ['header'],
            groupCollapse: false
        },
        loadComplete: function () { },
        loadError: function () { }
    });
}


function FinalizeEmargPackageForRepackage(paramData) {
    if (confirm("Are you sure to finalize Package Details ?")) {
        $.ajax({
            url: '/MaintenanceAgreement/FinalizePackageEmargForRepackaing',
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
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
              //  LoadCompletedRoads();
                $("#tblstProposalRepackage").trigger('reloadGrid');
                if (response.success) {
                    unblockPage();
                    alert(response.message);
                }
                else {
                    alert(response.message);
                    unblockPage();
                }
            }
        });
    }
}


function FinalizeEmargRepackagingDetails(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' these details ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/FinalizeRepakageForEmarg/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblstProposalRepackage").trigger('reloadGrid');

                    
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function AddEmargRepackagingDetails(IMS_PR_ROAD_CODE) {
   

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Maintenance Repackaging Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddPackage").load("/MaintenanceAgreement/AddEmargMaintenanceRepackagingDetails?" + $.param({ ProposalCode: IMS_PR_ROAD_CODE }), function () {
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