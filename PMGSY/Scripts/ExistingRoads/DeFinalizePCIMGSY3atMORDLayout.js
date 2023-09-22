$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));

    $("#StateList_StateListRoadDetails").change(function () {
        loadDistrict($("#StateList_StateListRoadDetails").val());
        $("#tbBlockList").hide();
        $("#idFinalizeDRRP").hide();
      

        //if ($("#DistrictList_StateListRoadDetails option:selected").val() != 0 || $("#DistrictList_StateListRoadDetails option:selected").val() != -1|||| $("#DistrictList_StateListRoadDetails option:selected").val() != "undefined") {
        //    alert("Dist " + $("#DistrictList_StateListRoadDetails option:selected").val())
        //    LoadBlocks();
        //}

    });


    $("#DistrictList_StateListRoadDetails").change(function () {

        if ($("#DistrictList_StateListRoadDetails").val() == 0 || $("#DistrictList_StateListRoadDetails").val() == -1)
        {
            //alert("Please select District");
            $("#tbBlockList").hide();
            $("#idFinalizeDRRP").hide();
            
            return false;
        }
        else
        {
            LoadBlocks();
        }
    });


    $("#btnViewStateListRoad").click(function () {

        if ($("#StateList_StateListRoadDetails").val() == 0 || $("#StateList_StateListRoadDetails").val() == -1)
        {
            alert("Please select State");
            return false;
        }
        else if ($("#DistrictList_StateListRoadDetails").val() == 0 || $("#DistrictList_StateListRoadDetails").val() == -1)
        {
            alert("Please select District");
            return false;
        }
        LoadBlocks();
    });
    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewStateListRoad").trigger('click');
    }
    //this function call  on layout.js
    closableNoteDiv("divStateListRoad", "spnStateListRoad");

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmStateListRoadLayout").toggle("slow");

    });
});

//State Change Fill District DropDown List
function loadDistrict(statCode) {
    $("#DistrictList_StateListRoadDetails").val(0);
    $("#DistrictList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").val(0);
    $("#BlockList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/ExistingRoads/DistrictDetailsForDefinalizationPCI',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++)
                    {
                        $("#DistrictList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0)
                    {
                        $("#DistrictList_StateListRoadDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_StateListRoadDetails").attr("disabled", "disabled");
                        $("#DistrictList_StateListRoadDetails").trigger('change');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else
    {
        $("#DistrictList_StateListRoadDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_StateListRoadDetails").empty();
        $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");
    }
}

function LoadBlocks() {
    jQuery("#tbBlockList").jqGrid('GridUnload');
    jQuery("#tbBlockList").jqGrid({
        url: '/ExistingRoads/GetBlockListForPCIunderPMGSYIII/',
        datatype: "json",
        mtype: "GET",
        colNames: ['District', 'Block', 'Finalize', 'Definalize'],
        colModel: [
                        { name: 'District', index: 'District', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'Finalize', index: 'Block', width: 10, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: true },
                        { name: 'DeFinalize', index: 'Block', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
        ],
        postData: { districtCode: $("#DistrictList_StateListRoadDetails option:selected").val(), statecode: $("#StateList_StateListRoadDetails option:selected").val() },
        pager: jQuery('#dvBlockListPager'),
        rowNum: 10,
        sortorder: "asc",
        sortname: 'Block',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function (data) {

            $("#tbBlockList #dvBlockListPager").css({ height: '31px' });
            if (data.isAllBlockFinalized == true) {
                $("#dvBlockListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeDRRP' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DeFinalizePCIDistrict();return false;' value='Definalize District for PCI'/>");
            }
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

//function FinalizeMRLBlock(urlparameter) {
//    $.ajax({
//        type: 'POST',
//        url: '/CoreNetwork/FinalizeMRLBlock/' + urlparameter,
//        dataType: 'json',
//        //data: $("#frmFinalizeMRLLayout").serialize(),
//        data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
//        async: false,
//        cache: false,
//        success: function (data) {
//            alert(data.message);
//            if (data.success) {
//                //$("#tbBlockList").trigger('reloadGrid');
//                LoadBlocks();
//            }
//            else {
//            }
//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            alert("Error occurred while processing the request.");
//        }
//    });
//}

function DeFinalizePCIDistrict() {
    var distName = $("#DistrictList_StateListRoadDetails option:selected").text();

    if (confirm("Are you sure to definalize PCI District " + distName+" ?")) {
        $.ajax({
            type: 'POST',
            url: '/ExistingRoads/DeFinalizePCIDistrict/' + $("#frmStateListRoadLayout input[name=__RequestVerificationToken]").val(),
            dataType: 'json',
            data: $("#frmStateListRoadLayout").serialize(),
            //data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    //$("#tbBlockList").trigger('reloadGrid');
                    LoadBlocks();
                }
                else {
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
}


//function DeFinalizeDRRPDistrict(urlparameter) {
//    if (confirm("Are you sure you want to delete this Block Details?")) {
//        $.ajax({
//            type: 'POST',
//            url: '/ExistingRoads/DeFinalizeDRRPBlockatMORD/' + urlparameter,
//            dataType: 'json',
//            //data: $("#frmFinalizeMRLLayout").serialize(),
//            data: { __RequestVerificationToken: $("#frmFinalizeMRLLayout input[name=__RequestVerificationToken]").val() },
//            async: false,
//            cache: false,
//            success: function (data) {
//                alert(data.message);
//                if (data.success) {
//                    //$("#tbBlockList").trigger('reloadGrid');
//                    LoadBlocks();
//                }
//                else {
//                }
//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                alert("Error occurred while processing the request.");
//            }
//        });
//    }
//    else {
//        return false;
//    }
//}


function DeFinalizeDRRPBlock(urlparameter) {
  
    if (confirm("Are you sure to delete this Block Details?"))
    {
    $.ajax({
        type: 'POST',
        url: '/ExistingRoads/DeFinalizePCIBlockatMORD/' + urlparameter,
        dataType: 'json',
        //data: $("#frmFinalizeMRLLayout").serialize(),
        data: { __RequestVerificationToken: $("#frmStateListRoadLayout input[name=__RequestVerificationToken]").val() },
        async: false,
        cache: false,
        success: function (data) {
            alert(data.message);
            if (data.success) {
                //$("#tbBlockList").trigger('reloadGrid');
                LoadBlocks();
            }
            else {
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error occurred while processing the request.");
        }
    });
    }
    else {
        return false;
    }
}