$('#btnAddIfscCode').click(function () {
    $("#divStatus_Duplicate").hide();
    $("#divStatus_Insert").hide();
    $("#isStatus").hide();
    if ($("#frmAddIfscCode").valid()) {
         

        $.ajax({
            url: '/Reat/Reat/AddIFSCCode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: $("#frmAddIfscCode").serialize(),
            async: false,
            cache: false,
            success: function (jsonData) {
                 
                if (jsonData.oprnType == "InvBank") {
                    $("#divStatus_Duplicate").hide();
                    $("#divStatus_Insert").hide();
                    $("#isStatus").hide();
                    alert("Please select Valid Bank Name");
                  
                }
                else {
                    if (jsonData.oprnType == "I") {
                        $("#divStatus_Duplicate").hide();
                        $("#divStatus_Insert").show();
                        $("#isStatus").show();
                        alert("IFSC code added successfully");


                        $("#frmAddIfscCode").trigger('reset');

                    }
                    else {
                        $("#divStatus_Insert").hide();
                        $("#divStatus_Duplicate").show();
                        $("#isStatus").show();
                        alert("IFSC code is already available");

                    }
                    $("#Bank_Name").html(jsonData.custModel['BankName']);
                    $("#Branch_Name").html(jsonData.custModel['BranchName']);
                    $("#Bank_Address").html(jsonData.custModel['BankAddress']);
                    $("#BankCity").html(jsonData.custModel['City']);
                    $("#State").html(jsonData.custModel['stateName']);
                    $("#IFSCCode").html(jsonData.custModel['IfscCode']);


                }

               

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });

    }
    
   
});
 


 
 
    $("#ddlBankName").autocomplete({

        source: function (request, response) {

            $.ajax({

                url: '/Reat/Reat/PopulateDistinctPFMSBankNames',
                type: 'POST',
                dataType: "json",
                data: { search: $("#ddlBankName").val() },

                success: function (data) {
                    
                   
                    response($.map(data, function (item) {
                        $("#ddlBankName").html(item.BankName);
                        return { label: item.BankName, value: item.BankName };
                    }));
                },
                error: function (xhr, status, error) {

                    alert(xhr.BankName.val);
                }
            });
        }
    });


 
    $('#btnReset').click(function () {

        $("#divStatus_Duplicate").hide();
        $("#divStatus_Insert").hide();
        $("#isStatus").hide();
    });

