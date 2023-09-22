//$.validator.unobtrusive.adapters.add('requirefieldpublicationvalidator', ['previousval'], function (options) {
//    options.rules['requirefieldpublicationvalidator'] = options.params;
//    options.messages['requirefieldpublicationvalidator'] = options.message;
//});

//$.validator.addMethod("requirefieldpublicationvalidator", function (value, element, params) {
//    if (value == '' || value == null) {
//        return false;
//    }
//    else {
//        return true;
//    }
//});

/*Date Validation Lab Established Date must be greater than or equal to agreement  date and less than or equal to Current date.*/
$.validator.unobtrusive.adapters.add('requirefieldpublicationvalidator', ['date'], ['month'], ['year'], function (options) {
    options.rules['requirefieldpublicationvalidator'] = options.params;
    options.messages['requirefieldpublicationvalidator'] = options.message;
});
$.validator.addMethod("requirefieldpublicationvalidator", function (value, element, params) {
   
});

$(document).ready(function () {
    //alert("Publication is Ready!");
    var action=$("#Action").val();
    //alert(action);
    $("#publicationCategoryCode option[value='0']").text("--Select--");


    $("#btnSubmit").click(function (evt) {
        evt.preventDefault();

        if ($('#frmPublication').valid()) {            
            
            $.ajax({
                url: '/publication/PublicationAddEdit/',
                type: "POST",
                cache: false,
                data: $("#frmPublication").serialize(),
              
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                //success: function (response) {
                    
                //    if (response.Success == "Success") {
                //        alert("Publication details saved successfully.");
                        
                //    }
                //    else {
                //       alert("Processing Error!");
                //    }
                success: function (response) {

                    if (response.success === undefined) {
                        $("#divPublication").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success) {
                        alert(response.message);
                        ClosePublication();
                        unblockPage();
                    }
                    else {
                        
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                        unblockPage();
                    }
                
                  
                } 
            });
        }
        
        
    });

    $("#btnUpdate").click(function (evt) {
        evt.preventDefault();

        if ($('#frmPublication').valid()) {

            $.ajax({
                url: '/publication/PublicationAddEdit/',
                type: "POST",
                cache: false,
                data: $("#frmPublication").serialize(),

                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                //success: function (response) {

                //    if (response.Success == "Success") {
                //        alert("Publication details updated successfully.");

                //    }
                //    else {
                //        alert("Processing Error!");
                //    }
                //    unblockPage();
                //    ClosePublication();
                success: function (response) {

                    if (response.success === undefined) {
                        $("#divPublication").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success) {
                        alert(response.message);
                        ClosePublication();
                        unblockPage();
                    }
                    else {
                        alert(response.message);
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                        unblockPage();
                    }
                }
            });
        }


    });

    $("#btnCancel").click(function () {
        ClosePublication();
    });
   

    $('#publicationDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
       // maxDate: new Date(),
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });
    //$("#publicationDate").addClass("pmgsy-textbox");
    //$("#publicationDate").datepicker({
    //    changeMonth: true,
    //    changeYear: true,
    //    dateFormat: "dd/mm/yy",
    //    showOn: 'button',
    //    buttonImage: '../../Content/images/calendar_2.png',
    //    buttonImageOnly: true,
    //    onClose: function () {
    //        $(this).focus().blur();
    //    }
    //}).attr('readonly', 'readonly');

    $('#Date_Type').change(function () {      
        if ($('#Date_Type').val() == "D") {
            $('#trDateTypeParam').show();
            $('#tdLblDate').show();
            $('#tdTxtDate').show();
            $('#tdLblYear').hide();
            $('#tdDdYear').hide();
            $('#tdLblMonth').hide();
            $('#tdDdMonth').hide();
        }
        else if ($('#Date_Type').val() == "Y") {
            $('#trDateTypeParam').show();
            $('#tdLblYear').show();
            $('#tdDdYear').show();
            $('#tdLblDate').hide();
            $('#tdTxtDate').hide();
            $('#tdLblMonth').hide();
            $('#tdDdMonth').hide();
        }
        else if ($('#Date_Type').val() == "M") {
            $('#trDateTypeParam').show();
            $('#tdLblYear').show();
            $('#tdDdYear').show();
            $('#tdLblMonth').show();
            $('#tdDdMonth').show();
            $('#tdLblDate').hide();
            $('#tdTxtDate').hide();
        } else {

            $('#trDateTypeParam').hide();
            $('#tdLblDate').hide();
            $('#tdTxtDate').hide();
            $('#tdLblYear').hide();
            $('#tdDdYear').hide();
            $('#tdLblMonth').hide();
            $('#tdDdMonth').hide();
        }

    });
    //$('#trDateTypeParam').hide();
    //$('#tdLblDate').hide();
    //$('#tdTxtDate').hide();
    //$('#tdLblYear').hide();
    //$('#tdDdYear').hide();
    //$('#tdLblMonth').hide();
    //$('#tdDdMonth').hide();
});




