$(document).ready(function () {

    //Added By Abhishek kamble 4-Apr-2014 start
    $(function () {

        if ($("#RoleCode").val() == 22) {//PIU

            $("#ddlSearchStates").attr("disabled","disabled");
            $("#ddlSearchDistrict").attr("disabled", "disabled");
            $("#ddlSearchStates").trigger("change");
        }        
    });
    //Added By Abhishek kamble 4-Apr-2014 end

   

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
        SearchDetails();
    });


    $("#ddlSearchStates").change(function () {


        FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                    "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

        $('#ddlSearchBlocks').empty();
        $('#ddlSearchBlocks').append("<option value=0>All Blocks</option>");

    }); //end function state change


    $("#ddlSearchDistrict").change(function () {
        fillBlockDropDown(); 
        
    }); //end function District change


    //Start Change by Deepak 2-Sept-2014
    if ($("#RoleCode").val() != 22) {
        if ($("#ddlSearchDistrict").val() == 0) {
            $("#ddlSearchDistrict").val($("#ddlSearchDistrict")[0].options[1].value);
            $("#ddlSearchDistrict").trigger('change');
            setTimeout(function () {
                $("#ddlSearchBlocks").val($("#ddlSearchBlocks")[0].options[1].value);
            }, 500);
        }
    }
    //End Change by Deepak 2-Sept-2014
});

function fillBlockDropDown() {
    $('#ddlSearchBlocks').empty();
    FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());
  

}
function SearchDetails() {

    $('#tbVillageList').setGridParam({
        url: '/LocationMasterDataEntry/GetVillageDetailsList', datatype: 'json'
    });
   /* var data = $('#tbVillageList').jqGrid("getGridParam", "postData");
    data._search = true;
    data.searchField = $("#frmSearchVillage").serialize();
    $('#tbVillageList').jqGrid("setGridParam", { "postData": data });*/

    $('#tbVillageList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), blockCode: $('#ddlSearchBlocks option:selected').val() } });
    $('#tbVillageList').trigger("reloadGrid", [{ page: 1 }]);

}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#ddlSearchDistrict') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else if (dropdown ='#ddlSearchBlocks') { 
        message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
    }
    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
           // if(this.Value!=0)
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json").complete(function () {//Added By Abhishek kamble 4-Apr-2014 

        if ($("#RoleCode").val() == 22) {

            if (dropdown == '#ddlSearchDistrict') {

            $("#ddlSearchDistrict").val($("#SearchDistCode").val());            
         
                setTimeout(function () {
                    $("#ddlSearchDistrict").trigger("change");
                }, 1500);              
               
                    setTimeout(function () {
                        $("#ddlSearchBlocks").val($("#ddlSearchBlocks")[0].options[1].value);
                    }, 2000);
                    
                setTimeout(function () {
                    $('#btnSearch').trigger('click');

                }, 2000);
            //}, 2000);
            $("#SearchDistCode").val(null);
        }
        }
        

    });
    $.unblockUI();
} //end FillInCascadeDropdown()
