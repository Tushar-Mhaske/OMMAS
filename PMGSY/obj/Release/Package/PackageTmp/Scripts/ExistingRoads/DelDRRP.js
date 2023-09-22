$(document).ready(function () {

    $('#btnView').click(function () {


        if ($("#frmFinalizeDRRPLayout").valid()) {
            LoadExistingRoads();
        }
    });

});

$("#ddlDistrict").change(function () {

    var DistCode = $("#ddlDistrict option:selected").val();

    loadBlock(DistCode);


});

function loadBlock(DistCode)
{
    $("#ddlBlocks").val(0);
    $("#ddlBlocks").empty();

    if (DistCode > 0)
    {
        if ($("#ddlBlocks").length > 0)
        {
            $.ajax({
                url: '/ExistingRoads/BlockDetailsITNO',
                type: 'POST',
                data: { "DistrictCode": DistCode },
                success: function (jsonData)
                {
                    for (var i = 0; i < jsonData.length; i++)
                    {
                        $("#ddlBlocks").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#ddlBlocks option:selected").val() > 0)
                    {
                        $("#ddlBlocks ").val($("#ddlBlocks option:selected").val());

                    }
                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                }
            });
        }
    } else
    {
        $("#ddlBlocks").append("<option value='0'>All Blocks</option>");
    }
}






    function LoadExistingRoads()
    {

        jQuery("#tbExistingRoadsList").jqGrid('GridUnload');

        jQuery("#tbExistingRoadsList").jqGrid({
            url: '/ExistingRoads/DeleteGetExistingRoadsPMGSY3List/',
            datatype: "json",
            mtype: "GET",
            colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner",'Block Name','TR / MRL Details', 'Delete PCI', 'Delete CBR', 'Delete CD Works', 'Delete Habitations', 'Delete Surface Types', 'Delete Traffice Intensity', 'Delete DRRP / MRL', "View", "Delete Existing Road"],
            colModel: [
                            { name: 'ERCode', index: 'ERCode', height: 'auto', width: 60, align: "left", sortable: true, search: true },
                            { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                            { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },

                            { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                            { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                            { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 50, sortable: true, align: "left", search: false },

                            //{ name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false, hidden: true },

                            //{ name: 'a', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },
                            //{ name: 'SurfaceTypes', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },
                            //{ name: 'HabitationsMapped', width: 70, sortable: false, resize: false, falign: "center", sortable: false, search: false, hidden: true },
                            //{ name: 'TrafficIntensity', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },
                            //{ name: 'CBRValue', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },

                            { name: 'block', index: 'block', width: 80, sortable: true, align: "left", search: false },

                             { name: 'cn', index: 'cn', width: 80, sortable: true, align: "left", search: false },
                          {
                              name: 'PCI', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'CBR', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'CD', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'Hab', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'Surface', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'Traffice', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },

                          {
                              name: 'DrrpAndMrl', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: true
                          },
                            { name: 'ShowDetails', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden:true },
                          //  { name: 'MapDRRP', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },
                          //  { name: 'Edit', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },


                          {
                              name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                              hidden: false
                          },
            ],
            postData: { districtCode: $("#ddlDistrict option:selected").val(), catCode: $("#ddlRoadCategory option:selected").val(), blockCode: $("#ddlBlocks option:selected").val() },
            //   postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
            pager: jQuery('#dvExistingRoadsListPager'),
            rowNum: 10,
            sortorder: "desc",
            sortname: 'MAST_ER_ROAD_CODE',
            rowList: [5, 10, 15, 20],
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Existing Roads List (PMGSY III) for Deletion ",
            height: 'auto',
            autowidth: true,
            cmTemplate: { title: false },
            rownumbers: true,
            loadComplete: function () {
                $("#tbExistingRoadsList #dvExistingRoadsListPager").css({ height: '31px' });
               
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

        $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

    }



    function DeletePCIDAL(id) {

        if (confirm("Are you sure to delete PCI details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeletePCI/' + id,
                type: "POST",
                cache: false,
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


    function DeleteCBRDAL(id) {

        if (confirm("Are you sure to delete CBR details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteCBR/' + id,
                type: "POST",
                cache: false,
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

    function DeleteCDWorksDAL(id) {

        if (confirm("Are you sure to delete CD Works details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteCDWorks/' + id,
                type: "POST",
                cache: false,
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


    function DeleteHabitationsDAL(id) {

        if (confirm("Are you sure to delete Habitation details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteHabitations/' + id,
                type: "POST",
                cache: false,
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

    function DeleteSurfaceTypesDAL(id) {

        if (confirm("Are you sure to delete Surface Types details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteSurfaceTypes/' + id,
                type: "POST",
                cache: false,
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

    function DeleteTrafficeIntensityDAL(id) {

        if (confirm("Are you sure to delete Traffice Intensity details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteTrafficeIntensity/' + id,
                type: "POST",
                cache: false,
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


    function DeleteDRRPandMRLDAL(id) {

        if (confirm("Are you sure to delete DRRP / MRL details ? ")) {
            $.ajax({
                url: '/ExistingRoads/DeleteDRRPandMRL/' + id,
                type: "POST",
                cache: false,
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


function DeletePMGSY3DRRPDetails(id) {

    if (confirm("Are you sure to delete existing road details ? ")) {
        $.ajax({
            url: '/ExistingRoads/DeleteExistingRoadsPMGSY3DRRP/' + id,
            type: "POST",
            cache: false,
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
