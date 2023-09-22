



$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmBen');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


  


    $("#spCollapseIconCN").click(function () {

        if ($("#soilDetails").is(":visible")) {
            $("#soilDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {


        $("#dvPFMSDownloadXMLLayout").show('slow');

        $("#benUpdate").hide('slow');
       
    
    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {
     
      
       // alert("Update" + $("#reatIFSC").val().length)

        if ($("#reatContractorName").val() == null||$("#reatContractorName").val() == "") {
            alert("Contractor Name is required.");
            return false;
        }
        if ($("#reatIFSC").val() == null || $("#reatIFSC").val()=="") {
            alert("IFSC Code is required.");
            return false;
        }

        if ($("#reatIFSC").val().length == 11) {

        }
        else {

            alert("IFSC Code must be 11 characters.");
            return false;
        }

        //alert("Update")
        //alert("Conname "+$("#reatContractorName").val())
        //alert("IFSC "+$("#reatIFSC").val())
        //$('#btnDownloadXML').click(function () {

        //if (!$("#frmBen").valid())
        //{
        //        return false;
        //    }

            $.ajax({
                type: 'GET',
                url: '/REAT/Reat/GenerateXMLForBeneficiaryUpdation',
                data: $("#frmBen").serialize(),
                success: function (data)
                {
                    alert(data.message);
                    $('#tblContractorsList').trigger('reloadGrid');
                    $("#dvPFMSDownloadXMLLayout").show('slow');

                    $("#benUpdate").hide('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error occurred while processing the request.");
                }
            })
        
       // });

        //if ($("#frmBen").valid()) {
        //    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //    $.ajax({
        //        type: 'POST',
        //        url: '/Reat/Reat/',
        //        async: false,
        //        data: $("#frmBen").serialize(),
        //        success: function (data) {
        //            if (data.success==true) {
                        
        //                $("#t").html();
        //                $("#t").hide('slow');
        //                alert(data.message);

        //                LoadCompletedRoads();

        //            }
        //            else if (data.success==false) {
        //                if (data.message != "") {


        //                    alert(data.message);
        //                    $("#t").html();
        //                    $("#t").hide('slow');


        //                    $('#message').html(data.message);
        //                    $('#dvErrorMessage').show('slow');
        //                }
        //            }
        //            else {
        //                $("#soilDetails").html(data);
        //            }
        //            $.unblockUI();
        //        },
        //        error: function (xhr, ajaxOptions, thrownError) {
        //            alert(xhr.responseText);
        //            $.unblockUI();
        //        }
        //    })
        //}








    });
});

