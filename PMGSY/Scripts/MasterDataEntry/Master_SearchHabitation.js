    $(document).ready(function () {

        //Added By Abhishek kamble 4-Apr-2014 start
        $(function () {

            if ($("#RoleCode").val() == 22) {//PIU

                $("#ddlSearchStates").attr("disabled", "disabled");
                $("#ddlSearchDistrict").attr("disabled", "disabled");

                $("#ddlSearchStates").trigger("change");
            }
        });
        //Added By Abhishek kamble 4-Apr-2014 end


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

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
        $('#txtSearchVillage').val('');

        $('#txtSearchVillage').autocomplete({ source: [" "] });


    }); //end function state change


    $("#ddlSearchDistrict").change(function () {

        $('#ddlSearchBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                    "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());

        $('#txtSearchVillage').val('');

        //$('#txtSearchVillage').data().autocomplete.term = null;

        $('#txtSearchVillage').autocomplete({ source: [" "] });

    }); //end function District change

    $("#ddlSearchBlocks").change(function () {

        $("#txtSearchVillage").val('');
        LoadVillages();

    }); //end function Block change

        //Start Change by Deepak 2-Sept-2014
    if ($("#RoleCode").val() != 22) {
        if ($("#ddlSearchDistrict").val() == 0) {
            $("#ddlSearchDistrict").val($("#ddlSearchDistrict")[0].options[1].value);
            $("#ddlSearchDistrict").trigger('change');
            setTimeout(function () {
                $("#ddlSearchBlocks").val($("#ddlSearchBlocks")[0].options[1].value);
            }, 500);
            setTimeout(function () {
                LoadVillages();
            }, 700);
        }
    }
        //End Change by Deepak 2-Sept-2014
});

function SearchDetails() {
    //alert($('#ddlSearchStates option:selected').val());
    $('#tbHabitationList').setGridParam({
        url: '/LocationMasterDataEntry/GetHabitationDetailsList', datatype: 'json'
    });
   /* var data = $('#tbHabitationList').jqGrid("getGridParam", "postData");
    data._search = true;
    data.searchField = $("#frmSearchHabitation").serialize();
    $('#tbHabitationList').jqGrid("setGridParam", { "postData": data });*/
    $('#tbHabitationList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), blockCode: $('#ddlSearchBlocks option:selected').val(), villageName: $('#txtSearchVillage').val() } });
    $('#tbHabitationList').trigger("reloadGrid", [{ page: 1 }]);

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

                //$("#ddlSearchDistrict").val($("#SearchDistCode").val());
                //setTimeout(function () {
                //    $("#ddlSearchDistrict").trigger("change");
                //    $("#btnSearch").trigger("click");
                //}, 1000);
                //$("#SearchDistCode").val(null);

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

function LoadVillages() {

 
    var blockID = $('#ddlSearchBlocks option:selected').val();
    $.ajax({
        url: "/LocationMasterDataEntry/GetVillagesByBlockCode_Search?blockCode=" + blockID,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {
            

            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                
                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }
            
            $('#txtSearchVillage').autocomplete({
                source: rows,
               dataType: 'json',
               formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })
}