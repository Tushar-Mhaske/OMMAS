var stateCode;
var agencyCode;
var adminCode;
var active;

$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#hdStatCode').val() > 0) {

       
        $("#State").attr("disabled", true);
       // $("#Agency").attr("disabled", true);        
        $("#ddlSSRDA").attr("disabled", true);

    }


    $.validator.unobtrusive.parse('#frmSearchPIU');
    $("#btnPIUSearch").click(function () {
        stateCode = $('#State option:selected').val();
        agencyCode = $('#Agency option:selected').val();
        adminCode = $('#ddlSSRDA option:selected').val();
        active = $('#ddlActive option:selected').val();
       
        searchDetails(stateCode, agencyCode, adminCode, active);
    });

    $('#State').change(function () {
       // loadSearchAgencyList($('#State option:selected').val());
        loadSearchSRRDAList($('#State option:selected').val());
    });
    $("#btnPIUSearch").trigger('click')
    {
        LoadPIUGrid();
    }

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

function searchDetails(stateCode, agencyCode, adminCode,active) {  

    $('#PIUCategory').setGridParam({
        url: '/Master/GetPIUDepartmentList', datatype: 'json'
    });

    $('#PIUCategory').jqGrid("setGridParam", { "postData": { stateCode: stateCode, agency: agencyCode, adminNDCode: adminCode, active: active } });
    $('#PIUCategory').trigger("reloadGrid", [{ page: 1 }]);
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

function loadSearchAgencyList(statCode) {
    $("#Agency").val(0);
    $("#Agency").empty();
  
    if (statCode > 0) {
        if ($("#Agency").length > 0) {
            $.ajax({
                url: '/Master/GetAgencyListByState',
                type: 'POST',
                data: { "stateCode": statCode,"IsAllSelected":true },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#Agency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

        $("#Agency").append("<option value='0'>--All--</option>");
    }
}

function loadSearchSRRDAList(statCode) {    

    if (statCode > 0) {
        $("#ddlSSRDA").val(0);
        $("#ddlSSRDA").empty();
        if ($("#ddlSSRDA").length > 0) {
            $.ajax({
                url: '/Master/GetSSRDAListByState',
                type: 'POST',
                data: { "stateCode": statCode, "IsAllSelected": true },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlSSRDA").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

       // $("#Agency").append("<option value='0'>--All--</option>");
    }
}
