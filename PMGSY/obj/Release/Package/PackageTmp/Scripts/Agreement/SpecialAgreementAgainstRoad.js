$(document).ready(function () {


    LoadProposedRoads();
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');


    $("#ddlFinancialYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());

    }); 

    $('#ddlDistricts').change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlBlocks", "/Agreement/GetBlocksByDistricts?districtCode=" + $('#ddlDistricts option:selected').val());
    });

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());

    }); 
});

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} 
function LoadProposedRoads() {

    if ($("#RoleCode").val() == 36 || $("#RoleCode").val() == 47) {
        if ($('#ddlDistricts').val() == 0) {
            alert('Please select District');
        }
        else {

            $('#tbProposedRoadList').setGridParam({
                url: '/Agreement/GetProposedRoadList',
                datatype: 'json'
            });
            $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { district: $('#ddlDistricts option:selected').val() } });
            $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);
        }
    }


    var gridCaption = "";

    if ($('#AgreementType').val() == "O") {

        gridCaption = 'Sanctioned Work List for Other Road';

    }
    else {
        gridCaption = 'Sanctioned Work List for Road';
    }

    jQuery("#tbProposedRoadList").jqGrid({
        url: '/Agreement/GetSpecialAgreementProposedRoadList',
        datatype: "local",
        mtype: "POST",
        colNames: ['Block', 'Year', 'Batch', 'Package', 'Work', 'Work Type', 'Road Length(In Kms)/ Bridge Length(In Mtrs)', 'Funding Agency', 'Sanctioned Cost', 'Maintenance Cost', 'View'],
        colModel: [
                            { name: 'Block', index: 'Block', width: 10, sortable: true, align: 'center' },
                            { name: 'SanctionedYear', index: 'SanctionedYear', height: 'auto', width: 10, sortable: true, align: 'center' },
                            { name: 'Batch', index: 'Batch', width: 10, sortable: true, align: 'center' },
                            { name: 'Package', index: 'Package', width: 10, sortable: true, align: 'center' },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 18, align: "left", sortable: true },
                            { name: 'WorkType', index: 'WorkType', height: 'auto', width: 10, align: "center", sortable: true },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 14, sortable: false, align: "left" },
                            { name: 'Collaboration', index: 'Collaboration', width: 15, sortable: true },
                            { name: 'SanctionedCost', index: 'SanctionedCost', width: 13, sortable: false, align: "right" },
                            //{ name: 'AgreementCost', index: 'AgreementCost', width: 13, sortable: false, align: "right" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 13, sortable: false, align: "right" },
                            { name: 'View', width: 7, sortable: false, resize: false, formatter: FormatColumnView, align: "center" }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvProposedRoadListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: gridCaption,
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'SanctionedYear,Package,RoadName,WorkType',
        sortorder: "asc",
        loadComplete: function () {

            var reccount = $('#tbProposedRoadList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvProposedRoadListPager_left').html('[<b> Note</b>: 1. All Amounts are in Lakhs. 2.All Lengths are in Kms ]');
            }


        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}

function SearchDetails() {

    $('#tbProposedRoadList').setGridParam({
        url: '/Agreement/GetSpecialAgreementProposedRoadList',
        datatype: 'json'
    });

    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), proposalType: $('#ddlProposalTypes option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val(), districtCode: $('#ddlDistricts option:selected').val() } });
    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

}

function FormatColumnView(cellvalue, options, rowObject) {
    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
}

function ViewAgreementDetails(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Agreement/CheckSplitWorkFinalized/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {
            if (data.isSplitWorkFinalized == true) {
                $.ajax({
                    url: "/Agreement/AddSpecialAgreementAgainstRoad/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,

                    success: function (data) {

                        $("#dvAddSpecialAgreementAgainstRoad").html(data);
                        $('#accordion').show('slow');
                        $('#dvAddSpecialAgreementAgainstRoad').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }


                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });

            }
            else if (data.isSplitWorkFinalized == false) {

                alert('You have not finalized split work details, so please finalize split work details and then make agreement against work.');
            }

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });



}