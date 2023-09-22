$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMapDRRPLayout'));

    LoadDRRPRoadsPMGSY1();

    $('#btnMapDRRP').click(function () {
        if (parseInt($('#ddlRoads option:selected').val()) <= 0) {
            alert("Please select DRRP Road to Map");
            return false;
        }

        $.ajax({
            url: '/ExistingRoads/MapDRRPPMGSY1Roads/' + $('#ddlRoads option:selected').val(),
            type: "POST",
            cache: false,
            data: { roadCode: $("#RoadCode").val(), __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);

                    $('#divFilterFormMapDRRP').load('/ExistingRoads/MapDRRPLayout/' + $('#EncrRoadCode').val(), function () {
                        $("#tbExistingRoadsList").trigger('reloadGrid');
                    });
                    //setTimeout(function () {
                    //    //$("#tbmappedDRRPRoadList").trigger('reloadGrid');

                    //    PopulateRoads();
                    //    LoadDRRPRoadsPMGSY1();

                    //    //CloseExistingRoadsDetails();

                    //    $("#tbExistingRoadsList").trigger('reloadGrid');
                    //}, 1000);

                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }
                }
                unblockPage();
            }
        });
    });//Map DRRP click ends..


});//Document ready ends..

function PopulateRoads() {
    $('#ddlRoads').empty();
    $.ajax({
        url: '/ExistingRoads/GetRoadsbyBlockCode',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { blockCode: $("#blockCode").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                if (jsonData[i].Value == 2) {
                    $("#ddlRoads").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                else {
                    $("#ddlRoads").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            }
            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}///PopulateRoads function ends..

//display mapped Existing Road list PMGSY 1
function LoadDRRPRoadsPMGSY1() {

    jQuery("#tbmappedDRRPRoadList").jqGrid('GridUnload');

    jQuery("#tbmappedDRRPRoadList").jqGrid({
        url: '/ExistingRoads/GetMappedDRRPPmgsy1List/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Length [in Km]', 'Road Type', "Road Owner", "Included in Core Network", "Un Map"],
        colModel: [
                        { name: 'ERCode', index: 'HabitationCode', height: 'auto', width: 60, align: "left", sortable: true, search: false },
                        { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },
                        { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 80, sortable: true, align: "left", search: false },
                        { name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false },
                        { name: 'Unmap', index: 'Unmap', width: 97, sortable: true, align: "center", search: false, hidden: ($("#RoleCode").val() == 22 ? true : false) },
        ],
        postData: { blockCode: $("#blockCode").val(), RoadCode: $("#RoadCode").val() },
        pager: jQuery('#mappedDRRPRoadPager'),
        rowNum: 10,
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Mapped Existing Roads List for PMGSY1",
        height: 'auto',
        autowidth: false,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
    //$("#tbmappedDRRPRoadList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
}//Ends

function UnMapDRRPPMGSY1Road(urlParam) {
    if (confirm("Are you sure to unmap existing road details ? ")) {
        $.ajax({
            url: '/ExistingRoads/UnMapExistingRoadPMGSY1/' + urlParam,
            type: "POST",
            cache: false,
            data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $("#tbmappedDRRPRoadList").trigger('reloadGrid');
                    $("#tbExistingRoadsList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }

                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}