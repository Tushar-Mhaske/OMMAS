
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmSearchCluster');
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#StateList_ClusterDetails").change(function () {
        loadDistrict($("#StateList_ClusterDetails").val());

    });

    $("#btnClusterSearch").click(function () {

        LoadClusterGrid();
        if ($("#dvViewClusterHabiationDetails").is(":visible")) {
            $("#dvViewClusterHabiationDetails").hide('slow');
        }
    });

    $("#DistrictList_ClusterDetails").change(function () {
        loadBlock($("#StateList_ClusterDetails").val(), $("#DistrictList_ClusterDetails").val());

    });
    if ($("#DistrictList_ClusterDetails").val() == 0) {
        $("#DistrictList_ClusterDetails").val($("#DistrictList_ClusterDetails")[0].options[1].value);
        $("#DistrictList_ClusterDetails").trigger('change');
        setTimeout(function () {
            $("#BlockList_ClusterDetails").val($("#BlockList_ClusterDetails")[0].options[1].value);
        }, 1200);
        setTimeout(function () {
            $("#btnClusterSearch").trigger('click');
        }, 1500);
    }

    //if ($('#hdStatCode').val() > 0) {
    //    $("#State").attr("disabled", true);  
    //}

   

   
   
  
    $("#btnClusterSearch").trigger('click');
    

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
});

function searchDetails() {

    //$('#PIUCategory').setGridParam({
    //    url: '/Master/GetPIUDepartmentList', datatype: 'json'
    //});

    //$('#PIUCategory').jqGrid("setGridParam", { "postData": { StateCode: $('#StateList_ClusterDetails option:selected').val(), DistrictCode: $('#DistrictList_ClusterDetails option:selected').val(), BlockCode: $('#BlockList_ClusterDetails option:selected').val() } });
    //$('#PIUCategory').trigger("reloadGrid", [{ page: 1 }]);
    LoadClusterGrid();
}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#District') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}


function loadDistrict(statCode) {
    $("#DistrictList_ClusterDetails").val(0);
    $("#DistrictList_ClusterDetails").empty();
    $("#BlockList_ClusterDetails").val(0);
    $("#BlockList_ClusterDetails").empty();
    $("#BlockList_ClusterDetails").append("<option value='0'>All Block</option>");

    if (statCode > 0) {
        if ($("#DistrictList_ClusterDetails").length > 0) {
            $.ajax({
                url: '/Master/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_ClusterDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

        $("#DistrictList_ClusterDetails").append("<option value='0'>All District</option>");
        $("#BlockList_ClusterDetails").empty();
        $("#BlockList_ClusterDetails").append("<option value='0'>All Block</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_ClusterDetails").val(0);
    $("#BlockList_ClusterDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_ClusterDetails").length > 0) {
            $.ajax({
                url: '/Master/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_ClusterDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }                


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_ClusterDetails").append("<option value='0'>All Block</option>");
    }
}



