$(document).ready(function () {    
    $.validator.unobtrusive.parse($('frmTrafficIntensity'));

    GetTrafficIntensity($("#MAST_ER_ROAD_CODE").val());


    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();

        $(function () {
            $("#MAST_TOTAL_TI").focus();
        });

    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }

   

    //allow only digits and .

    $("#MAST_TOTAL_TI").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_COMM_TI").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    
    $("#MAST_TOTAL_TI").blur(function () {

        var motarisedTraffic = $("#MAST_TOTAL_TI").val();
        var commercialTraffic = $("#MAST_COMM_TI").val();
            

        if (commercialTraffic != "" && parseFloat(commercialTraffic)>=parseFloat(motarisedTraffic)) {
            $("#MotorisedTraffic").show("slow");
            $("#MotorisedTraffic").html("<span style='color:red'><b>Total Motarised Traffic/Day should be greater than Commercial Vehicle Traffic/Day.</b></span>");

        }
    });

    $("#MAST_COMM_TI").blur(function () {

        var motarisedTraffic = $("#MAST_TOTAL_TI").val();
        var commercialTraffic = $("#MAST_COMM_TI").val();

        if (motarisedTraffic != "" && parseFloat(commercialTraffic) >=parseFloat(motarisedTraffic)) {
            $("#CommercialTraffic").show("slow");
            $("#CommercialTraffic").html("<span style='color:red'><b>Commercial Vehicle Traffic/Day should be less than Total Motorized Traffic/Day.</b></span>");
        }
    });

    $('#btnSave').click(function (evt) {                    
        evt.preventDefault();

        if ($('#frmTrafficIntensity').valid()) {
            if (validateForm() == true) {

                $.ajax({
                    url: "/ExistingRoads/AddTrafficIntensity/",
                    type: "POST",
                    cache: false,
                    data: $("#frmTrafficIntensity").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {

                        $("#tbTraffic").trigger('reloadGrid');
                        $("#frmTrafficIntensity").trigger('reset');
                        PopulateTrafficIntensityYears($("#MAST_ER_ROAD_CODE").val());

                        $("#MAST_TOTAL_TI").val("");
                        $("#MAST_COMM_TI").val("");

                        if (response.success) {
                            unblockPage();
                            alert(response.message);
                            clearForm();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                            unblockPage();
                        }
                    }
                });
            }
        }
        else {
            
        }
            
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        validateForm();

        if ($('#frmTrafficIntensity').valid()) {
            $.ajax({
                url: "/ExistingRoads/EditTrafficIntensity/",
                type: "POST",
                cache: false,
                data: $("#frmTrafficIntensity").serialize(),
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
                        alert(response.message);
                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();
                        $("#tbTraffic").trigger('reloadGrid');
                        $("#frmTrafficIntensity").trigger('reset');
                        $("#MAST_TOTAL_TI").val("");
                        $("#MAST_COMM_TI").val("");
                        PopulateTrafficIntensityYears($("#MAST_ER_ROAD_CODE").val());
                    }
                    else
                    {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }
                }
            });
        }
    });

    $('#btnCancel').click(function () {

        var MAST_ER_ROAD_CODE = $("#MAST_ER_ROAD_CODE").val();

        $("#accordion div").html("");

        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Traffic Intensity Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
                );

        $('#accordion').show('fold', function () {
            blockPage();

            $("#divExistingRoadsForm").load('/ExistingRoads/TrafficIntensityCancel/' + MAST_ER_ROAD_CODE, function () {

                $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
                unblockPage();
            });
            $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
            $('#divExistingRoadsForm').show('slow');
            $("#divExistingRoadsForm").css('height', 'auto');
        });
    });

    $("#btnReset").click(function () {
        

        $("#MotarisedTraffic").html('');
        $("#CommercialTraffic").html('');
        $("#MAST_TOTAL_TI").val('');
        $("#MAST_COMM_TI").val('');

        if ($("#divError").is(":visible"))
        {
            $("#divError").hide("slow");
        }

        $("#MAST_TOTAL_TI").removeClass('input-validation-error');
        $("#MAST_COMM_TI").removeClass('input-validation-error');
    });

});
function clearForm() {

    $("#frmCBRValue").find(':input').each(function () {
        switch (this.type) {
            case 'text':
                $(this).val('');
        }
    });

    $("#spnSegment_Length").html(0);

}


function PopulateTrafficIntensityYears(MAST_ER_ROAD_CODE) {
    
    $("#MAST_TI_YEAR").val(0);
    $("#MAST_TI_YEAR").empty();

    $.ajax({
        url: '/ExistingRoads/PopulateTrafficIntensityYears/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE},
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_TI_YEAR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}



function GetTrafficIntensity(MAST_ER_ROAD_CODE) {

    jQuery("#tbTraffic").jqGrid({
        url: '/ExistingRoads/GetTrafficIntensityList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Year', 'Total Motarised Traffic/day', 'Commercial Vehicle Traffic/day', "Edit", "Delete"],
        colModel: [

                    { name: 'Year', index: 'Year', width: 250, sortable: true, align: "center" },
                    { name: 'TotalMotarisedTrafficday', index: 'TotalMotarisedTrafficday', width: 290, sortable: true, align: "center" },
                    { name: 'CommercialVehicleTrafficDay', index: 'CommercialVehicleTrafficDay', width: 285, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center",formatter:FormatColumnTrafficEdit },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnTrafficDelete}
        ],
        pager: jQuery('#dvTrafficPager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE},
               
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Traffic Intensity Details",
        height: 'auto',
        width: 'auto',
        sortname: 'Year',
        sortorder:'asc',
        //autowidth:true,
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {                
                 alert("Invalid data.Please check and Try again!")                
            }
        },
        loadComplete: function () {
            $("#gview_tbTraffic > .ui-jqgrid-titlebar").hide();
        }
    });
}

function EditTrafficIntensity(key) {
    
    $("#divTrafficIntensity").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divExistingRoadsForm").load('/ExistingRoads/EditTrafficIntensity/'+key, function () {
            $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
            if ($("#Operation").val() == "U") {
                $("#MAST_TOTAL_TI").focus();
            }
            unblockPage();
        });        
    });
     

}

function DeleteTrafficI(key) {

    if (confirm("Are you sure you want to delete the traffic intensity details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DeleteTrafficIntensity/" + key,
            type: "POST",
            cache: false,
            //data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, IMS_TI_YEAR: IMS_TI_YEAR, value: Math.random() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
               
                $("#tbTraffic").trigger('reloadGrid');
               // $("#btnCancel").click('click');
                unblockPage();
               
                if (response.success) {
                    alert("Traffic Intensity Details Deleted Succesfully.");

                    PopulateTrafficIntensityYears($("#MAST_ER_ROAD_CODE").val());

                   
                }
               
            }
        });

    }
    else {
        return;
    }
}

function validateForm()
{
    var motarisedTraffic = $("#MAST_TOTAL_TI").val();
    var commercialTraffic = $("#MAST_COMM_TI").val();

    if (commercialTraffic != "" && parseFloat(commercialTraffic) >= parseFloat(motarisedTraffic)) {
        $("#MotarisedTraffic").show("slow");
        $("#MotarisedTraffic").html("<span style='color:red'><b>Total Motorized Traffic/Day should be greater than Commercial Vehicle Traffic/Day.</b></span>");
        return false;
    } else {
        $("#MotarisedTraffic").hide("slow");
        return true;
    }
}

function FormatColumnTrafficEdit(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the Traffic Intensity Details' onClick ='EditTrafficIntensity(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnTrafficDelete(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the Traffic Intensity Details' onClick ='DeleteTrafficI(\"" + cellvalue.toString() + "\");'></span></center>";
}

