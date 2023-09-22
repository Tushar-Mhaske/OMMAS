$(document).ready(function () {

    $.validator.unobtrusive.parse("divAddSurface");
    
    //$("#divAddSurface").load('/ExistingRoads/SurfaceAddEdit?id='+$("#EncryptedRoadCode").val(), function () {

    //    $.validator.unobtrusive.parse("divAddCdWorks");

    //});

    LoadAddView();

    GetSurfaceListPMGSY3($("#ExistingRoadCode").val());

});
function GetSurfaceListPMGSY3(MAST_ER_ROAD_CODE) {
    //alert(MAST_ER_ROAD_CODE);

    jQuery("#tbSurfaceType").jqGrid({
        url: '/ExistingRoads/GetSurfaceTypeListPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Surface Type', 'Start Chainage(in Kms.)', 'End Chainage(in Kms.)', "Road Condition", "Length", "Edit", "Delete"],
        colModel: [
                    { name: 'SurfaceName', index: 'SurfaceName', width: '180%', sortable: true, align: "left" },
                    { name: 'StartChainage', index: 'StartChainage', width: '180%', sortable: true, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: '200%', sortable: true, align: "center" },
                    { name: 'SurfaceCondition', index: 'SurfaceCondition', width: '150%', sortable: true, align: "left" },
//                  { name: 'SurfaceCondition', index: 'SurfaceCondition', width: 200, sortable: false, align: "center", formatter: 'number', summaryType: 'sum' },
                    { name: 'SurfaceLength', index: 'SurfaceLength', width: '150%', sortable: true, align: "center"},//, formatter: "number", summaryType: 'sum' },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnSurfaceEdit },//, formatter: FormatColumnSurfaceEdit 
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnSurfaceDelete }//, formatter: FormatColumnSurfaceDelete
        ],
        pager: jQuery('#dvSurfaceTypePager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Surface List",
        sortname: 'EndChainage',
        sortorder: 'asc',
        height: 'auto',
        width: '100%',
        //autowidth:true,
        sutowidth: true,
        rownumbers: true,
        //footerrow: true,
        //userDataOnFooter: true,
        loadComplete: function () {
            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SurfaceLength', false);
            var RoadLength = 0;
            for (i = 0 ; i < RoadLengthColumn.length; i++) {

                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
            }
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        loadComplete: function () {
            $("#gview_tbSurfaceType > .ui-jqgrid-titlebar").hide();
        }
    });

}


function EditSurface(key) {

    $("#divSurfaceType").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddSurface").load('/ExistingRoads/SurfacePMGSY3Layout?id=' + key, function () {
            $.validator.unobtrusive.parse($('#frmSurfaceType'));

            unblockPage();
        });
    });

}

function DeleteSurface(key) {

    if (confirm("Are you sure you want to delete the Surface details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DeleteSurfaceDetailsPMGSY3/" + key,
            type: "POST",
            cache: false,
            data: { __RequestVerificationToken: $("#frmListSurfacePMGSY3 input[name=__RequestVerificationToken]").val() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();

                if (response.success) {

                    $("#tbSurfaceType").trigger('reloadGrid');
                    DisplaySurfaceStartChainage($("#MAST_ER_ROAD_CODE").val());
                    $("#spanRemainingLength").html(response.RemainingLength);//update label
                    $("#Remaining_Length").val(response.RemainingLength);

                    $(".spanPavementLength").html(response.SurfaceLengthEntered);
                    $("#SurfaceLenghEntered").val(parseFloat($(".spanPavementLength").html()));

                    alert("Surface Details Deleted Succesfully.");
                    //$("#divAddSurface").load('/ExistingRoads/SurfaceAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {

                    //    $.validator.unobtrusive.parse("divAddCdWorks");

                    //});
                    LoadAddView();
                }

            }
        });

    }
    else {
        return;
    }
}
function FormatColumnSurfaceEdit(cellvalue, options, rowObject) {

    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the Surface Details' onClick ='EditSurface(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnSurfaceDelete(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the Surface Details' onClick ='DeleteSurface(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
function LoadAddView()
{
    $.ajax({

        type: 'GET',
        url: '/ExistingRoads/SurfacePMGSY3Layout/',
        data: { id: $("#EncryptedRoadCode").val() },
        error: function () { },
        success: function (data) {
            if (data.success == false) {
                alert(data.message);
            }
            else {
                $("#divAddSurface").html(data);
            }

        },


    });
}