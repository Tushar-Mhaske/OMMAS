////var token = '';
////var header = {};

$(document).ready(function () {

    //token = $('[name=__RequestVerificationToken]').val();
    //header["__RequestVerificationToken"] = token;
    $(".VehiclesCount").attr("maxlength", "2");
    function createDatePicker($element) {
       
        $element.siblings('.ui-datepicker-trigger').remove();

        // Initialize the datepicker for the element
        $element.datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: "-60:+0",
            buttonText: 'Date',
            endDate: "today",
            maxDate: "today",
            onSelect: function (selectedDate) {
               
            },
            onClose: function () {
                $(this).focus().blur();
            }
        });

       
        var imgId = "datepicker_img_" + Date.now();

    
        var $img = $('<img>').attr({
            'src': '/Content/Images/calendar_2.png',
            'alt': 'Date',
            'title': 'Date'
        }).addClass('ui-datepicker-trigger').attr('id', imgId);

      
        $element.after($img);

       
        $img.click(function () {
            $element.datepicker('show');
        });
    }

    
    $('.datepicker').each(function () {
        createDatePicker($(this));
    });

    // Function to create dynamic textboxes based on VehiclesCount value

    function createDynamicTextboxes(container, count) {
        container.empty();

        for (var i = 0; i < count; i++) {
            var vehicleID = $('<input>').attr({
                type: 'text',
                name: 'VehiclesID',
                class: 'VehiclesID',
                style: 'margin-top: 3%;margin-bottom: 3%; margin-left: 2px;'
            });

            var numbering = (i + 1) + ') ';

            var divWrapper = $('<div>').addClass('vehicleEntry');
            divWrapper.text(numbering);
            divWrapper.append(vehicleID);

            container.append(divWrapper);
        }
    }


    if ($('input[name="Is_GPSVTS_Installed"]:checked').val() === 'N') {
        $('.TotalCount, .VehicleDetails, .ADD_DEL_BTN').hide();
    }

  
    $('input[name="Is_GPSVTS_Installed"]').click(function () {
        if ($(this).val() === 'N') {
            $('.TotalCount, .VehicleDetails, .ADD_DEL_BTN').hide();
        } else if ($(this).val() === 'Y') {
            $('.TotalCount, .VehicleDetails, .ADD_DEL_BTN').show();
        }
    });


    $('.btnAddVehicle').click(function () {
        var vehicleDetails = $('.VehicleDetails:first').clone();
        vehicleDetails.find('input[type="text"]').val(''); 


        var uniqueId = Date.now(); 
        vehicleDetails.attr('id', 'vehicleDetails_' + uniqueId);

      
        vehicleDetails.find('.VehiclesCount').attr('id', 'VehiclesCount_' + uniqueId).attr('name', 'VehiclesCount_' + uniqueId);
        vehicleDetails.find('.Vehicle').attr('id', 'Vehicle_' + uniqueId).attr('name', 'Vehicle_' + uniqueId);
        vehicleDetails.find('.VehiclesID').each(function (index) {
            $(this).attr('id', 'VehicleID_' + uniqueId + '_' + index).attr('name', 'VehicleID_' + uniqueId + '_' + index);
        });

        
        //vehicleDetails.find('.vehiclesIdContainer input[type="text"]').remove();
        vehicleDetails.find('.vehiclesIdContainer label, .vehiclesIdContainer input[type="text"]').remove();



        // Handle the creation of dynamic textboxes based on VehiclesCount value
        var vehiclesCount = parseInt(vehicleDetails.find('.VehiclesCount').val());
        if (!isNaN(vehiclesCount)) {
            var vehiclesIdContainer = vehicleDetails.find('.vehiclesIdContainer');
            createDynamicTextboxes(vehiclesIdContainer, vehiclesCount);
        }

      
        vehicleDetails.find('.datepicker').removeClass('hasDatepicker').removeAttr('id');
        createDatePicker(vehicleDetails.find('.datepicker'));

        
        vehicleDetails.find('.ui-datepicker-trigger').click(function () {
            vehicleDetails.find('.datepicker').datepicker('show');
        });
     
        $(".container").off("change", ".VehiclesCount").on("change", ".VehiclesCount", function () {
            updateTotalCount();
        });
        vehicleDetails.insertAfter('.VehicleDetails:last');
        return false;
    });

    // Event handler for VehiclesCount change
    $(document).on('input', '.VehiclesCount', function () {
        var count = parseInt($(this).val());
        var container = $(this).closest('.VehicleDetails').find('.vehiclesIdContainer');

        createDynamicTextboxes(container, count);

        // Initialize datepicker for the new input fields
        $(this).closest('.VehicleDetails').find('.datepicker').removeClass('hasDatepicker').removeAttr('id');
        createDatePicker($(this).closest('.VehicleDetails').find('.datepicker'));
    });

    // Event handler for "Delete Vehicle" button click
    $('.btnDeleteVehicle').click(function () {
        var vehicleDetailsCount = $('.VehicleDetails').length;
        if (vehicleDetailsCount > 1) {
            $('.VehicleDetails:last').remove();
        } else {
            Alert("Please enter at least one vehicle details.");
        }
    });

    //Total Count 

    function updateTotalCount() {
        let totalCount = 0;

      
        $(".VehiclesCount").each(function () {
            const value = parseInt($(this).val(), 10);
            if (!isNaN(value)) {
                totalCount += value;
            }
        });

       
        $("#TotalNoVehicles").val(totalCount);
    }

    $(".container").on("change", ".VehiclesCount", function () {
        updateTotalCount();
    });

});



       
function getSelectedValues() {
    var selectedValuesArray = [];
    var $vehicleDetails = $(".VehicleDetails");

    $vehicleDetails.each(function () {
        var selectedValues = {};
        var $this = $(this);

        selectedValues.Vehicle = $this.find('.Vehicle').val();
        selectedValues.VehiclesCount = $this.find('.VehiclesCount').val();

        var vehiclesIdValues = [];
        $this.find('.VehiclesID').each(function () {
            vehiclesIdValues.push($(this).val());
        });

        selectedValues.VehiclesID = vehiclesIdValues;
        selectedValues.VTS_InstallationDate = $this.find('.datepicker').val();

        selectedValuesArray.push(selectedValues);
    });

    return selectedValuesArray;
}

$("#BtnSubmitGPSDetails").click(function (event) {
  
    event.preventDefault();
   
    Confirm('Are you sure you want to submit?', function (value) {
        if (value) {
    let is_GPSVTS_Installed = $('input[name="Is_GPSVTS_Installed"]:checked').val()
    var validateFormResponse = false;
    if (is_GPSVTS_Installed == "Y") {
        validateFormResponse = validateForm();
    } else {
        validateFormResponse = true;
    } 
   
    //alert("validateFormResponse" + validateFormResponse);
    if (validateFormResponse) {
        let selectedValuesArray = getSelectedValues();
        let roadCodeValue = $("#RoadCode").val();


        let Data = {
            VTS_INSTRUMENT_GPS_Details: selectedValuesArray,
            RoadCodeValue: roadCodeValue,
            Is_GPSVTS_Installed: is_GPSVTS_Installed
        };
        let jsonData = JSON.stringify(Data);
        //GPS Instrument Id Duplicate Check  
        // Usage
        GPS_Instrument_ID_Already_exists(selectedValuesArray, function (result) {
           // alert("result  " + result);
            if (!result) {

                //alert("header" + header);
                $.ajax({
                    type: "POST",
                    url: "/GPSVTSInstallationDetails/GPSVTSDetails/SaveGPSVTSDetails",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                   // headers: header,
                    success: function (response) {
                        if (!response.success && response.message !== "") {
                            Alert(response.message);
                        } else {
                            Alert(response.message);
                            //CloseGPSVTSRoadDetails();
                            Load_GPSVTS_Saved_Details(response.encCode);
                            //AddGPSVTSDetails(response.encCode);
                            CloseGPSVTSRoadDetails();
	                        //Reload Grid
                            Load_GPSVTS_RoadList()
                            //End reload Grid
                        }
                    },
                    error: function (xhr, status, error) {

                        console.error(error);
                    }
                });
            } else {
                return;
            }
        });
       /* alert("GPS_Instrument_ID_Already_Exists  " + GPS_Instrument_ID_Already_Exists);*/
        //End GPS Instrument Id Duplicate Check
  
      
       
            }
        }
    });
});

//Update
$("#BtnSubmitEditGPSDetails").click(function (event) {
  
    event.preventDefault();
    Confirm('Are you sure you want to submit?', function (value) {
        if (value) {
            let is_GPSVTS_Installed = $('input[name="Is_GPSVTS_Installed"]:checked').val()
            var validateFormResponse = false;
            if (is_GPSVTS_Installed == "Y") {
                validateFormResponse = validateForm();
            } else {
                validateFormResponse = true;
            }
            //alert("validateFormResponse" + validateFormResponse);
            if (validateFormResponse) {
                let selectedValuesArrayForUpdate = getSelectedValues();
                let selectedValuesArray = getEditedValues();
                let roadCodeValue = $("#RoadCode").val();
                let Vehicle_Gps_Ids = $("#Vehicle_Gps_Ids").val();

                //console.log(selectedValuesArray); 
                let Data = {
                    /*VTS_INSTRUMENT_GPS_Details: selectedValuesArray,*/
                    VTS_INSTRUMENT_GPS_Details: selectedValuesArrayForUpdate,
                    RoadCodeValue: roadCodeValue,
                    Is_GPSVTS_Installed: is_GPSVTS_Installed,
                    Vehicle_Gps_Ids: Vehicle_Gps_Ids
                };
                //console.log(Data);
                let jsonData = JSON.stringify(Data);
                //console.log("jsonData" + jsonData);
                GPS_Instrument_ID_Already_exists(selectedValuesArray, function (result) {
                    // alert("result  " + result);
                    if (!result) {
                $.ajax({
                    type: "POST",
                    url: "/GPSVTSInstallationDetails/GPSVTSDetails/UpdateGPSVTSDetails",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                   // headers: header,
                    success: function (response) {
                        if (!response.success && response.message !== "") {
                            Alert(response.message);
                        } else {
                            Alert(response.message);
                            //CloseGPSVTSRoadDetails();
                            Load_GPSVTS_Saved_Details(response.encCode);
                            //AddGPSVTSDetails(response.encCode);
                            CloseGPSVTSRoadDetails();
                        }
                    },
                    error: function (xhr, status, error) {

                        console.error(error);
                    }
                });
                    } else {
                        //Alert("Error: The edited GPS Instrument ID already exists for one or more vehicles. Please use unique GPS Instrument IDs for each vehicle.");
                        return;
                    }
                });
            }
        }
    });
});
//End Update

function validateForm() {
    let hasError = false;

    $(".VehicleDetails").each(function () {
       
        let $this = $(this);
    
        let vehicle = $this.find('.Vehicle').val();
        let vehiclesCount = $this.find('.VehiclesCount').val();
        let vtsInstallationDate = $this.find('.datepicker').val();
        let vehiclesIDInputs = $this.find('.VehiclesID');
        let Is_GPSVTS_Installed = $('input[name="Is_GPSVTS_Installed"]:checked').val();

      

        // Validate Is_GPSVTS_Installed radio buttons
        if (!$('input[name="Is_GPSVTS_Installed"]').is(":checked")) {

            Alert("Please select if GPS is installed or not.");
            hasError = true; 
            return false; 
        }

        // Validate Vehicle select
        else if (vehicle === "0") {
           
            Alert("Please select a vehicle.");
            hasError = true; 
            return false; 
        }

        // Validate VehiclesCount textbox
        else if (vehiclesCount === "" || isNaN(vehiclesCount) || parseInt(vehiclesCount) <= 0) {
         
            Alert("Please enter a valid number of vehicles.");
            hasError = true; 
            return false;
        }

        // Validate VTS_InstallationDate textbox
        else if (vtsInstallationDate === "") {
           
            Alert("Please enter a valid date of installation.");
            hasError = true; 
            return false; 
        }

        // Array to store entered values
        var enteredValues = [];

        vehiclesIDInputs.each(function () {
            var vehiclesID = $(this).val();
            var alphanumericRegex = /^[a-zA-Z0-9]+$/;

            if (vehiclesID.trim() === "") {
                Alert("Please enter a GPS Instrument ID.");
                hasError = true;
                return false;
            } else if (!alphanumericRegex.test(vehiclesID)) {
                Alert("Please enter a valid GPS Instrument ID. Only alphanumeric characters are allowed.");
                hasError = true;
                return false;
            } else if (enteredValues.includes(vehiclesID)) {
                Alert("Duplicate GPS Instrument ID detected. Please enter a unique value for each GPS Instrument ID.");
                hasError = true;
                return false;
            } else {
                // Add the current value to the array for future reference
                enteredValues.push(vehiclesID);
            }
        });


    
    });

    return !hasError; 
}

function GPS_Instrument_ID_Already_exists(selectedValuesArray, callback) {
    //alert("selectedValuesArray" + selectedValuesArray);
    let Data = {
        VTS_INSTRUMENT_GPS_Details: selectedValuesArray,
        RoadCodeValue:$("#RoadCode").val(),
    };
    let jsonData = JSON.stringify(Data);
    $.ajax({
        type: "POST",
        url: "/GPSVTSInstallationDetails/GPSVTSDetails/GPSInstrumentIDAlreadyExists",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: jsonData,
        success: function (response) {
            if (response.success && response.message !== "") {
                Alert(response.message);
                callback(true);
            } else if (response.success && response.message === "")
            {
                callback(false);
            }else {
                Alert(response.message);
                callback(true);
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
            callback(true);
        }
    });
}
//
function getEditedValues() {
    var editedValuesArray = [];
    var $vehicleDetails = $(".VehicleDetails");

    $vehicleDetails.each(function () {
        var editedValues = {};
        var $this = $(this);

        editedValues.Vehicle = $this.find('.Vehicle').val();
        editedValues.VehiclesCount = $this.find('.VehiclesCount').val();

        var editedVehiclesIdValues = [];
        $this.find('.gps-instrument-id').each(function () { // Use '.gps-instrument-id' instead of '.VehiclesID'
            var $thisTextbox = $(this);
            var originalValue = $thisTextbox.data("original-value");
            var editedValue = $thisTextbox.val();

            // Check if the textbox value has been edited
            if (editedValue !== originalValue) {
                editedVehiclesIdValues.push(editedValue);
            }
        });

        editedValues.VehiclesID = editedVehiclesIdValues;
        editedValues.VTS_InstallationDate = $this.find('.datepicker').val();

        if (editedVehiclesIdValues.length > 0) {
            editedValuesArray.push(editedValues);
        }
    });

    return editedValuesArray;
}



//
