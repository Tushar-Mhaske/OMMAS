$(document).ready(function () {
    //$.validator.unobtrusive.parse("#frmViewAgencyMapping");



    if ($('#distAgencyroleCode').val() == 36) {
        $('#ddlAgency').prop("disabled", true);
        $('#ddlState').prop("disabled", true);
    }
    else {
        $('#ddlAgency').prop("disabled", false);
        $('#ddlState').prop("disabled", false);
    }

    AgencySelection();
    setTimeout(function () {
        $("#btnViewDistrictAgency").trigger('click');
    }, 200);


    $('#ddlAgency').change(function () {
        AgencySelection();
    });


    $('#btnViewDistrictAgency').click(function () {
        //if ($('#distAgencyroleCode').val() == 25) {
        //    $("#btnAddAgency").show('slow');
        //}
        $('#btnViewAgency').hide('slow');
        $("#btnAddAgency").show('slow');
        LoadDistrictAgencyGrid();
        //$('#tblList').hide('slow');
        //$('#divPager').hide('slow');
    });


    $('#ddlState').change(function () {
        $('#ddlDistrict').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                    "#ddlDistrict", "/Master/GetDistrictbyState?stateCode=" + $('#ddlState option:selected').val());

    }); //end function District change

    function FillInCascadeDropdown(map, dropdown, action) {

        var message = '';

        if (dropdown == '#ddlDistrict') {

            message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
        }
        //else if (dropdown == '#ddlBlocks') {
        //    message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
        //}

        $(dropdown).empty();
        //$(dropdown).append("<option value=0>--Select--</option>");

        //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //$.blockUI({ message: message });

        $.post(action, map, function (data) {
            $.each(data, function () {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }, "json");
        //$.unblockUI();

    } //end FillInCascadeDropdown()
});

function LoadDistrictAgencyGrid() {
    //alert($('#ddlAgency').val());
    //alert($('#ddlState').val());
    //alert($('#ddlDistrict').val());
    $("#tblDistrictAgencyList").jqGrid('GridUnload');
    $("#tblList").jqGrid('GridUnload');
    $('#tblDistrictAgencyList').jqGrid({

        url: '/Master/GetDistrictTechnicalAgencyDetails',
        datatype: "json",
        mtype: "POST",
        postData: { Agency: $('#ddlAgency').val(), StateCode: $('#ddlState').val(), DistrictCode: $('#ddlDistrict').val() },
        colNames: ['State', 'District', 'Start Date', 'End Date', 'Agency Type', 'Technical Agency Name', 'Contact Name', 'Is Finalized', 'Is Enabled', 'Is Active'], //, 'Address', 'State Name', 'District Name', 'Contact No 1', 'Contact No 2', 'FAX', 'Mobile Number', 'Email', 'Website', 'Remark'
        colModel: [
                           { name: 'State', index: 'State', height: 'auto', width: 100, align: "left", sortable: true },
                           { name: 'District', index: 'District', height: 'auto', width: 104, align: "left", sortable: true },
                           { name: 'StartDate', index: 'StartDate', height: 'auto', width: 100, align: "center", sortable: true },
                           { name: 'EndDate', index: 'EndDate', height: 'auto', width: 100, align: "center", sortable: true },
                           { name: 'AgencyType', index: 'AgencyType', height: 'auto', width: 80, align: "center", sortable: true },
                           { name: 'TAName', index: 'TAName', height: 'auto', width: 200, align: "left", sortable: true },
                           { name: 'ContactName', index: 'ContactName', height: 'auto', width: 120, align: "left", sortable: true },
                           { name: 'IsFinalized', index: 'IsActive', height: 'auto', width: 50, align: "center", sortable: true },
                           { name: 'IsEnabled', index: 'IsActive', height: 'auto', width: 50, align: "center", sortable: true },
                           { name: 'IsActive', index: 'IsActive', height: 'auto', width: 50, align: "center", sortable: true },
        ],

        pager: jQuery('#divDistrictAgencyPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'State',
        sortorder: "asc",
        caption: "District Agency List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $('#divPager_left').html("<span class='ui-icon ui-icon-info ui-align-center'>Mapped District facility is only available for Other Agencies.</span>");
        },
    });
}

function AgencySelection() {
    if ($('#ddlAgency option:selected').val() == 'P') {
        $('#lblDistrict').hide('slow');
        $('#ddlDistrict').hide('slow');
    }
    else {
        $('#lblDistrict').show('slow');
        $('#ddlDistrict').show('slow');
    }
}