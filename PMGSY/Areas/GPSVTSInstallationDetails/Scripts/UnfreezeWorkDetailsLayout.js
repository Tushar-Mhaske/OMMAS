
$(document).ready(function () {
    $('#ddlState').change(function () {
    $("#ddlDistrict").empty();
    $.ajax({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/PopulateDistrictsbyStateCodeUnfreezeWorkDetails',
        type: 'POST',
        data: { stateCode: $("#ddlState").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (err) {
        }
    });
    });

    $('#ddlDistrict').change(function () {
        $("#ddlBlock").empty();
        $.ajax({
            url: '/GPSVTSInstallationDetails/GPSVTSDetails/PopulateBlocksbyDistrictCodeUnfreezeWorkDetails',
            type: 'POST',
            data: { districtCode: $("#ddlDistrict").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (err) {
            }
        });
    });
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

$('#btnRoadList').click(function () {

    if ($('#FilterForm').valid()) {
        Load_GPSVTS_RoadList();
    }

});


function Load_GPSVTS_RoadList() {
   
    jQuery("#tbGPSVTSRoadList").jqGrid('GridUnload');

    jQuery("#tbGPSVTSRoadList").jqGrid({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/GPSVTSUnfreezeWorkDetailsRoadList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'District', 'Block', 'Package Name', 'Sanction Year', 'Batch', 'Length', 'Total Sanctioned Cost', 'Road Name', 'Work Status', 'GPS Installed', 'PDF Uploaded & Finalized','PDF View','Unfreeze Work'],
        colModel: [

            { name: 'STATE', index: 'STATE', width: 100, align: "left" },
            { name: 'DISTRICT', index: 'DISTRICT', width: 100, align: "left" },
            { name: 'BLOCK', index: 'BLOCK', width: 100, align: "left" },
            { name: 'PACKAGE_NAME', index: 'PACKAGE_NAME', width: 120, align: "center" },
            { name: 'SANCTION_YEAR', index: 'SANCTION_YEAR', width: 70, align: "center" },
            { name: 'BATCH', index: 'BATCH', width: 70, align: "center" },
            { name: 'LENGTH', index: 'LENGTH', width: 60, align: "left" },
            { name: 'TOTAL_SANCTIONED_COST', index: 'TOTAL_SANCTIONED_COST', width: 80, align: "left", hidden: true },
            { name: 'ROAD_NAME', index: 'ROAD_NAME', width: 250, align: "left" },
            { name: 'WorkStatus', index: 'WorkStatus', width: 60, align: "center" },
            { name: 'isGPSINSTALLED', index: 'isGPSINSTALLED', width: 120, align: "center" },
            { name: 'isFinalized', index: 'isFinalized', width: 120, align: "center" },
            { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", search: false, editable: false },
            { name: 'UnFreeze_Work', index: 'UnFreeze_Work', width: 120, align: "center" }
        ],
        /* postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val(), proposaltype: $('#ddlProposalType').val() },*/
        postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val(), proposaltype: $('#ddlProposalType').val(), WorkStatus: $('#ddlWorkStatus').val() },
        pager: jQuery('#dvGPSVTSRoadListPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp; GPS/VTS Details Road List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#tbGPSVTSRoadList #dvGPSVTSRoadListPager").css({ height: '31px' });
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                Alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                Alert("Invalid data.Please check and Try again!");

            }
        }

    }); //end of grid
}
function UnFreezeWork(urlparameter) {
    Confirm("Are you sure to Unfreeze work details ? ", function (value) {
        if (value) {
            $.ajax({
                url: "/GPSVTSInstallationDetails/GPSVTSDetails/UnFreezeWorkDetails",
                type: "POST",
                cache: false,
                data: { parameter:urlparameter },
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
                    if (response.Success) {
                        Alert("Details Unfreezed Succesfully.");
                        Load_GPSVTS_RoadList()
                       
                    }
                    else {
                        Alert(response.ErrorMessage);
                    }
                }
            });

        }
        else {
            return;
        }
    });
}
function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/GPSVTSDetails/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;

    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            Alert('File Not Found.');
            return false;

        }
        else if (response.success) {
            //Alert("In success");
            var newTab = window.open(response.Message, '_blank');
            newTab.focus();

        }
    });



}

function GetGPSVTSSavedDetails(paramurl) {
    const newWindow = window.open("/GPSVTSInstallationDetails/GPSVTSDetails/ViewGPSVTSDetailsNewTab?parameter=" + paramurl);
    newWindow.onload = function () {
        newWindow.focus();
    };
}

function GetListPDFFilesUnfreezeWorkDetails(paramurl) {

    $("#accordion div").html("");

    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' > Pdf Details </a>" +
        '<a href="#" style="float: right;">' +
        '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/GPSVTSDetails/GetListPDFFilesUnfreezeWorkDetails',
            data: { parameter: paramurl },
            method: 'GET',
            success: function (data) {
            
                if (data.Success == false) {
                    Alert(data.ErrorMessage); 
                    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'visible');
                    $("#accordion").hide('slow');
                    $('#divpdfDetails').hide('slow');
                } else {
                   
                    $("#accordion").show('slow');
                    $('#divpdfDetails').html(data);
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                Alert('An error occurred while processing your request. Please try again later.');
            },
            complete: function () {
           
                $.unblockUI();
            }
        });
        $("#tbGPSVTSRoadList").jqGrid('setGridState', 'hidden');
        $('#divpdfDetails').show('slow');
        $("#divpdfDetails").css('height', 'auto');
    });
}


function CloseDetails() {
    $('#accordion').hide('slow');
    $('#divpdfDetails').hide('slow');
    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'visible');
}