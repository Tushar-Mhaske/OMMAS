$(document).ready(function () {

    //for expand and collpase Document Details 
    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$("#dvDocumentDetails").css('margin-bottom','10px');

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });


    $('#btnSearch').click(function (e) {

        $('#tbPanchyatList').jqGrid("setGridState", "visible");
        // $('#tbMLAConstituencyList').setGridParam({ hidegrid: false });

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        SearchDetails();
    });


    $("#ddlSearchStates").change(function () {


        FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                    "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

        $('#ddlSearchBlocks').empty();
        $('#ddlSearchBlocks').append("<option value=0>All Blocks</option>");

    }); //end function state change


    $("#ddlSearchDistrict").change(function () {

        $('#ddlSearchBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                    "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());

    }); //end function District change

    if ($("#ddlSearchDistrict").val() == 0) {
        $("#ddlSearchDistrict").val($("#ddlSearchDistrict")[0].options[1].value);
        $("#ddlSearchDistrict").trigger('change');
        setTimeout(function () {
            $("#ddlSearchBlocks").val($("#ddlSearchBlocks")[0].options[1].value);
        }, 500);
    }


});

function SearchDetails() {

    $('#tbPanchyatList').setGridParam({
        url: '/LocationMasterDataEntry/GetPanchayatDetailsList', datatype: 'json'
    });
   /* var data = $('#tbPanchyatList').jqGrid("getGridParam", "postData");
    data._search = true;
    data.searchField = $("#frmSearchPanchayat").serialize();
    $('#tbPanchyatList').jqGrid("setGridParam", { "postData": data });*/

    $('#tbPanchyatList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), blockCode: $('#ddlSearchBlocks option:selected').val() } });
    $('#tbPanchyatList').trigger("reloadGrid", [{ page: 1 }]);

}

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlSearchDistrict') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else if (dropdown = '#ddlSearchBlocks') {
        message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
    }

    $(dropdown).empty();
 //   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });

    $.post(action, map, function (data) {
        $.each(data, function () {
            // if(this.Value!=0)
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()
