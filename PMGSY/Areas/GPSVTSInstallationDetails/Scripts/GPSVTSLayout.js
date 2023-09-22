$(document).ready(function () {

    $("#btnRoadList").click(function () {
        if ((parseInt($("#ddlState option:selected").val()) > 0) && (parseInt($("#ddlDistrict option:selected").val()) > 0)) {
            Load_GPSVTS_RoadList();
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $(document).on("click", ".closeBtn", function () {
        $('#accordion').hide('slow');
        $(".AddViewMainDiv").hide('slow');
        $('#divGPSVTSRoadListForm').hide('slow');
        $("#tbGPSVTSRoadList").jqGrid('setGridState', 'visible');
        $('#divFilterForm').show('slow');
    });

    var h3Content = '<a href="#" class="closeBtn" style="float: right;right: 2%;position: relative;">' +
        '<img class="ui-icon ui-icon-closethick" /></a>';

 
    $(".AddViewMainDiv h3").html(h3Content);

    //$(document).on("click", ".ui-icon ui-icon-circle-triangle-s", function () {
    //    CloseGPSVTSRoadDetails();
    //});
});



function Load_GPSVTS_RoadList() {

    jQuery("#tbGPSVTSRoadList").jqGrid('GridUnload');

    jQuery("#tbGPSVTSRoadList").jqGrid({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/GPSVTSRoadList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'District', 'Block', 'Package Name', 'Sanction Year', 'Batch', 'Length', 'Total Sanctioned Cost', 'Road Name', 'GPS/VTS Established', 'GPS/VTS Finalized','GPS / VTS Details','Upload File'],
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
            { name: 'GPSVTS_Established', index: 'GPSVTS_Established', width: 70, align: "center" },
            { name: 'GPSVTS_Finalized', index: 'GPSVTS_Finalized', width: 60, align: "center" },
            { name: 'GPS_VTS_DETAILS', index: 'GPS_VTS_DETAILS', width: 120, align: "left" },
             { name: 'Upload_File', index: 'Upload_File', width: 120, align: "left" }
        ],
        /* postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val(), proposaltype: $('#ddlProposalType').val() },*/
        postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val(), proposaltype: $('#ddlProposalType').val(), WorkStatus: $('#ddlWorkStatus').val() },
        pager: jQuery('#dvGPSVTSRoadListPager'),
        rowNum: 50,
        rowList: [50, 100,150,200,300],
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

/*
function Add_GPS_VTS_Details(urlparameter) {

    alert("entered in Add_GPS_VTS_Details(urlparameter) Func" + urlparameter);

    $.ajax({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/AddGPSVTS_DetailsView/' + urlparameter,
        type: "GET",
         beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        },
        cache: false,
        async: false,        
        success: function (response) {
            if (!response.success) {
                alert(response.message);
            }            
            $.unblockUI();
        },
        error: function (response) {           
            $.unblockUI();
            alert("Error : " + response.responseText);
            return false;
        }
    });
}
*/

function AddGPSVTSDetails(urlparameter) {

  //  alert("entered in Add_GPS_VTS_Details(urlparameter) Func" + urlparameter);
    $(".AddViewMainDiv").show();
  //  $("#accordion div").html("");
    $("#accordion_Upload div").html("");
    $("#accordion_Upload").hide();
    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' > Add/View GPS VTS Details </a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseGPSVTSRoadDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
         $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#divGPSVTSRoadListForm").load('/GPSVTSDetails/AddGPSVTS_DetailsView/' + urlparameter, function (response) {
            $.validator.unobtrusive.parse($('#divGPSVTSRoadListForm'));            
            $.unblockUI();
        });
        $('#divGPSVTSRoadListForm').show('slow');
        $("#divGPSVTSRoadListForm").css('height', 'auto');
    });
    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'hidden');
    Load_GPSVTS_Saved_Details(urlparameter)
}


function CloseGPSVTSRoadDetails() {
    $('#accordion').hide('slow');
    $('#divGPSVTSRoadListForm').hide('slow');
    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'visible');
    $('#divFilterForm').show('slow');
}
//Added By Tushar on 19 July 2023 For Show saved GPS Data

var isButtonAdded = false;

var isDialogInitialized = false;
var customDialog;

var latestUrlParameter = null;
function Load_GPSVTS_Saved_Details(urlparameter) {
    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'hidden');
    $(".AddViewMainDiv").show();
    $("#accordion_Upload div").html("");
    $("#accordion_Upload").hide();
    $('#dvGPSVTSSavedDetails').show();
    $('#BtnAddVehicleDetails').show();
    $("#accordion div").html("");
    latestUrlParameter = urlparameter;
    //alert("in Load_GPSVTS_Saved_Details" + urlparameter);
    jQuery("#tbGPSVTSSavedDetails").jqGrid('GridUnload');
   // addCustomButton(urlparameter);
    if (!isButtonAdded) {
        var customButton = $('<button>', {
            class: 'btn btn-success',
            text: 'Add Vehicle',
          
        });

        // Store the urlparameter as a data attribute on the custom button
        customButton.data('urlparameter', urlparameter);

        // Append the button to the specified element
        $('#BtnAddVehicleDetails').append(customButton);

        $(document).on("click", '#BtnAddVehicleDetails button', function () {
      
            AddGPSVTSDetails(latestUrlParameter);
        });

    }
   
    jQuery("#tbGPSVTSSavedDetails").jqGrid({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/GetGPSVTSSavedDetails',
        datatype: "json",
        mtype: "POST",
        /*colNames: ['GPS INSTALLED', 'VEHICLE','DATE OF SUBMISSION', 'NO OF VEHICLES', 'DATE OF INSTALLATION', 'Edit', 'Detete', 'Finalize', 'VTS GPS ID', 'VEHICLE ID','VTS VEHICLE GPS ID'],*/
        colNames: ['GPS INSTALLED', 'VEHICLE', 'DATE OF SUBMISSION', 'NO OF VEHICLES', 'DATE OF INSTALLATION', 'Edit', 'Detete',  'VTS GPS ID', 'VEHICLE ID', 'VTS VEHICLE GPS ID'],
        colModel: [

            { name: 'GPSInstalled', index: 'GPSInstalled', width: 50, align: "center" },
            { name: 'VehicleName', index: 'VehicleName', width: 100, align: "center" },
            { name: 'DateOfSubmission', index: 'DateOfSubmission', width: 100, align: "center" },
            /* { name: 'NumberOfVehicles', index: 'NumberOfVehicles', width: 100, align: "center"},*/
           
            {
                name: 'NumberOfVehicles',
                index: 'NumberOfVehicles',
                width: 100,
                align: "center",
                formatter: function (cellValue, options, rowObject) {
                    const vtsVehicleGPSID = rowObject[9]; // Get the value of 'VTSVehicleGPSID' for the particular row
                    return '<a href="#" style="color: blue; text-decoration: underline;" ' +
                        'onmouseover="this.style.color=\'red\';" onmouseout="this.style.color=\'blue\';" ' +
                        'onclick="return showDialog(\'' + vtsVehicleGPSID + '\');">' +
                        cellValue +
                        '</a>';
                }
            },
            { name: 'DateOfInstallation', index: 'DateOfInstallation', width: 120, align: "center" },
            { name: 'Edit', index: 'Edit', width: 70, align: "center"},
            { name: 'Detete', index: 'Detete', width: 70, align: "center" },
            //{ name: 'Finalize', index: 'Finalize', width: 60, align: "left" },
            { name: 'VTSGPSID', index: 'VTSGPSID', width: 80, align: "left", hidden: true },
            { name: 'VehicleID', index: 'VehicleID', width: 250, align: "left", hidden: true },
            { name: 'VTSVehicleGPSID', index: 'VTSVehicleGPSID', width: 60, align: "center", hidden: true },
            
        ],
      
        postData: {
            parameter: urlparameter
        },
        pager: jQuery('#dvGPSVTSSavedDetailsPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp; GPS/VTS Saved Details",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            
            unblockPage();
           
            
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                Alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                Alert("Invalid data.Please check and Try again!");

            }
        },
      
        onCellSelect: function (rowid, iCol, cellcontent, e) {
         
            if (iCol === 4) {
              
                const VTSVehicleGPSIDData = $(this).jqGrid('getCell', rowid, 'VTSVehicleGPSID');

                showDialog(VTSVehicleGPSIDData);
            }
        }

    });

    isButtonAdded = true;

   
}

function showDialog(VTSVehicleGPSIDData) {
    //alert("VTSVehicleGPSIDData" + VTSVehicleGPSIDData);
    let Result = displayVerticallyWithNumbering(VTSVehicleGPSIDData);

    if (!isDialogInitialized) {
        // Create dialog elements dynamically
        customDialog = $("<div>").attr("id", "customDialog").addClass("dialog").hide();
        const dialogContentDiv = $("<div>").addClass("dialog-content");
        const closeSpan = $("<span>").addClass("close").html("&times;").on("click", closeDialog);

        // Set the initial content of dialogResultDiv with the Result data
        const dialogResultDiv = $("<div>").attr("id", "dialogResult").html(Result);

        dialogContentDiv.append(closeSpan).append(dialogResultDiv);
        customDialog.append(dialogContentDiv);
        $("body").append(customDialog);

        // Initialize the dialog
        customDialog.dialog({
            title: "GPS Instrument ID(s)",
            autoOpen: false,
            modal: true,
            width: 300,
            close: function () {
                isDialogInitialized = false;
            }
        });

        isDialogInitialized = true;
    }

    // Open the dialog if it is not already open
    if (!customDialog.dialog("isOpen")) {
        customDialog.dialog("open");
    }
}

function closeDialog() {
    if (isDialogInitialized && customDialog) {
        customDialog.dialog("close");
    }
}

function displayVerticallyWithNumbering(str) {
   
    const numbersArray = str.split(',');

    let result = '';
    for (let i = 0; i < numbersArray.length; i++) {
        result += `${i + 1}) ${numbersArray[i].trim()}<br>`;
    }

    return result;
}


function DeleteGPSVTSDetails(urlparameter) {
    Confirm("Are you sure you want to delete?", function (value) {
        if (value) {
            $.ajax({
                type: 'POST',
                url: '/GPSVTSInstallationDetails/GPSVTSDetails/DeteteGPSVTSDetails',
                dataType: 'json',
                data: { parameter: urlparameter },
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        Alert("Vehicle details deleted successfully.");
                  //Reload Grid
                    Load_GPSVTS_RoadList()
                    //End reload Grid
                        Load_GPSVTS_Saved_Details(urlparameter);
                    }
                    else {
                        Alert("Error occurred while processing your request.");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    Alert("Error occurred while processing the request.");
                }
            });
        }
    });


}
//$('.btnAddViewVehicle').click(function () {
//    alert("Add Click");
//    alert("Road" + $("#RoadCode").val())
//})

function EditGPSVTSDetails(urlparameter) {
    Confirm('Are you sure you want to Edit Details?', function (value) {
        if (value) {
            AddGPSVTSDetails(urlparameter);

            $.ajax({
                type: 'POST',
                url: '/GPSVTSInstallationDetails/GPSVTSDetails/EditGPSVTSDetails',
                data: { parameter: urlparameter },
                dataType: 'html', // Change this to the appropriate data type you expect in the response
                success: function (response) {
                    // The server should return the HTML content as a response

                    $("#DivContainer").html(response);

                    $.validator.unobtrusive.parse($('#DivContainer'));
                   //Reload Grid
                    Load_GPSVTS_RoadList()
                    //End reload Grid
                    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'hidden');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    Alert("Error occurred while processing the request.");
                }
            });

        }
    });
}
//End By Tushar on 19 July 2023

//Added By Tushar on 4 Aug 2023 
function UploadFile(urlParameter) {

        $("#accordion").hide();
    $("#accordion div").html("");
    $("#accordion_Upload div").html("");
    $("#accordion_Upload h3").html(
        "<a href='#' style= 'font-size:.9em;' >" + ("Upload File") + "</a>" +

        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseGPSVTSRoadDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion_Upload').show('fold', function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#divProposalForm").load('/GPSVTSDetails/FileUpload/' + urlParameter, function () {
            $.validator.unobtrusive.parse($('#divGPSVTSRoadListForm'));
            $.unblockUI();
        });
        $('#divProposalForm').show('slow');

        $('.AddViewMainDiv').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbGPSVTSRoadList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
    $('#divGPSVTSRoadListForm').hide();
    $('#dvGPSVTSSavedDetails').hide();
    $('#BtnAddVehicleDetails').hide();
    $('#tbGPSVTSSavedDetails').jqGrid('setGridState', 'hidden');

}