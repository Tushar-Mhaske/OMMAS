$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#ddlSearchState").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
                    "#ddlSearchOffice", "/Master/PopulateAdminNd_ByStateCode?stateCode=" + $('#ddlSearchState option:selected').val());

        //$("#ddlSearchOffice").append("<option value='0'>All Office</option>");
    });
    var hdStateCode = $("#hdnStateCode").val();
    if (hdStateCode > 0) {

        $('#ddlSearchState').attr("disabled", true);
        $("#ddlSearchState").trigger('change');
    }
    else {
        if ($("#ddlSearchState").val() == 0) {
            $("#ddlSearchState").val($("#ddlSearchState")[0].options[1].value);
            $("#ddlSearchState").trigger('change');
        }
    }

    if ($('#adminCode').val() > 0) {
       
        $("#ddlSearchOffice").val($('#adminCode').val());
    }

   
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

    $('#btnSearch').click(function (e) {
        SearchDetails();
    });
  
    setTimeout(function () {
        if ($("#RoleCode").val() == 22) {
            $("#ddlSearchOffice").attr('disabled', true);
        }
    }, 100);

    setTimeout(function () {
        $('#btnSearch').trigger('click')
        {
            LoadGrid();
        }
    }, 300);

    
});

$("#dvhdSearch").click(function () {
   
    if ($("#dvSearchParameter").is(":visible")) {
        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
        $(this).next("#dvSearchParameter").slideToggle(300);
    }
    else {
        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $(this).next("#dvSearchParameter").slideToggle(300);
    }
});

function SearchDetails() {

    $('#tblList').setGridParam({
        url: '/Master/GetNodalOfficerDetails', datatype: 'json'
    });

    $('#tblList').jqGrid("setGridParam", { "postData": { StateCode: $('#ddlSearchState option:selected').val(), officeCode: $('#ddlSearchOffice option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), designationCode: $('#ddlSearchDesignation option:selected').val(), NoTypeCode: $('#ddlSearchNOType option:selected').val(), ModuleType: $('#ddlModuleType option:selected').val(), Active: $('#ddlSearchActive option:selected').val() } });

    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);
}
function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#Constituency') {
        message = '<h4><label style="font-weight:normal"> Loading Constituencies... </label></h4>';
    }
  
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            if (this.Selected == true)
            {
                $(dropdown).append("<option value=" + this.Value + " selected = 'true'>" + this.Text + "</option>");
            }
            else
            {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });
    }, "json");
    $.unblockUI();
}