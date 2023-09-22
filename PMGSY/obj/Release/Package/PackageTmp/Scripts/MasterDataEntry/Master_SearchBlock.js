$(document).ready(function () {

    //for expand and collpase Document Details 
    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
    if ($("#ddlSearchDistrict").val() == 0) {
        $("#ddlSearchDistrict").val($("#ddlSearchDistrict")[0].options[1].value);
    }

    $('#btnSearch').click(function () {
       // $("#tbBlockList").jqGrid('GridUnload');
        SearchBlockDetails();
    });

    $("#ddlSearchStates").change(function () {
       
        FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                    "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());


    }); //end function state change


});

function SearchBlockDetails() {
   
    //alert("SerachButton"+$('#ddlSearchDistrict option:selected').val());
    $('#tbBlockList').setGridParam({
        url: '/LocationMasterDataEntry/GetBlockDetailsList', datatype:'json'
    });
   /* var data = $('#tbBlockList').jqGrid("getGridParam", "postData");
    data._search = true;
    data.searchField = $("#frmSearchBlock").serialize();
    $('#tbBlockList').jqGrid("setGridParam", { "postData": data });*/
   
    $('#tbBlockList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val() } });

    $('#tbBlockList').trigger("reloadGrid", [{ page: 1 }]);

}

function FillInCascadeDropdown(map, dropdown, action) {

    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';

    if (dropdown == '#ddlSearchDistrict') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

