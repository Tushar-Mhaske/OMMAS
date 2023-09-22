/*
 * Purpose:- PaymentSchedule.js is used to show Payment Schedule Grid and Payment Schedule data entry form
 * 
 */

$(document).ready(function ()
{
    $.validator.unobtrusive.parse($('#frmPaymentSchedule'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    //Payment Schedule Grid
    GetPaymentScheduleList($("#IMS_PR_ROAD_CODE").val());    
    
    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
        //$("#EXEC_MPS_AMOUNT").val('');
        $("#btnReset").trigger('click');
        //auto focus
        //$(function () {
        //    $("#EXEC_MPS_AMOUNT").focus();
        //});

    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }

    //Save Payment Schedule Details into the database
    $('#btnSave').click(function (evt) {
        evt.preventDefault();

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#EXEC_MPS_YEAR").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#EXEC_MPS_MONTH").val() > month && $("#EXEC_MPS_YEAR").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        if ($('#frmPaymentSchedule').valid()) {
            $.ajax({
                url: "/Execution/AddPaymentScheduleDetails/",
                type: "POST",
                cache: false,
                data: $("#frmPaymentSchedule").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    if ($("#divError").is(":visible")) {
                        $("#divError").hide('slow');
                    }
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if ($("#divError").is(":visible")) {
                        $("#divError").hide('slow');
                    }
                    unblockPage();

                    if (response.success) {
                        alert(response.message);
                        $("#tbPaymentSchedule").trigger('reloadGrid');
                        $("#frmPaymentSchedule").trigger('reset');
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                        unblockPage();
                    }
                }
            });//end of ajax call
        }
    });

    //Update Payment Schedule Details 
    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }


        if ($('#frmPaymentSchedule').valid()) {

            $("#EXEC_MPS_MONTH").attr('disabled', false);
            $("#EXEC_MPS_YEAR").attr('disabled', false);
            
            $.ajax({
                url: "/Execution/EditPaymentScheduleDetails/",
                type: "POST",
                cache: false,
                data: $("#frmPaymentSchedule").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    if ($("#divError").is(":visible")) {
                        $("#divError").hide('slow');
                    }
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response)
                {
                    if ($("#divError").is(":visible")) {
                        $("#divError").hide('slow');
                    }
                    unblockPage();
                   
                    if (response.success) {

                        alert(response.message);

                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();

                        $("#tbPaymentSchedule").trigger('reloadGrid');

                        $("#EXEC_MPS_MONTH").val(null);
                        $("#EXEC_MPS_YEAR").val(null);
                        $("#EXEC_MPS_AMOUNT").val('0');

                        //populate month,year

                        PopulateMonth();
                        PopulateYear($("#IMS_PR_ROAD_CODE").val());

                        $("#EXEC_MPS_MONTH").attr('disabled', false);
                        $("#EXEC_MPS_YEAR").attr('disabled', false);

                    } else {
                        $("#EXEC_MPS_MONTH").attr('disabled', true);
                        $("#EXEC_MPS_YEAR").attr('disabled', true);

                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }
                }
            });
            $("#EXEC_MPS_MONTH").attr('disabled', true);
            $("#EXEC_MPS_YEAR").attr('disabled', true);
        }
    });

    //display Payment Schedule Data entry form
    $('#btnCancel').click(function () {

        $("#rowHideShow").show();

        //show add update button
        $("#Operation").val("A");
        $("#rowAdd").show();
        $("#rowUpdate").hide();

        //clear form
        $("#EXEC_MPS_MONTH").val(null);
        $("#EXEC_MPS_YEAR").val(null);
        $("#EXEC_MPS_AMOUNT").val('0');

        //populate month,year

        PopulateMonth();
        PopulateYear($("#IMS_PR_ROAD_CODE").val());

        $("#EXEC_MPS_MONTH").attr('disabled', false);
        $("#EXEC_MPS_YEAR").attr('disabled', false);

        if ($("#divError").is(":visible")) {
            $("#divError").hide('slow');
        }

    });//end of cancel

    $("#EXEC_MPS_MONTH").change(
        function () {
            if ($("#divError").is(":visible")) {
                $("#divError").hide('slow');
            }
        });

    $("#EXEC_MPS_YEAR").change(
        function () {
            if ($("#divError").is(":visible")) {
                $("#divError").hide('slow');
            }
        });
    $("#EXEC_MPS_AMOUNT").focus(
     function () {
         if ($("#divError").is(":visible")) {
             $("#divError").hide('slow');
         }
     });

    $("#btnReset").click(function () {
        if ($("#divError").is(":visible")) {
            $("#divError").hide('slow');
        }
        //val msg clear
        $("#valMsgAmount").html('');
        $("#valMsgMonth").html('');
        $("#valMsgYear").html('');

        //input box reset
        $("#EXEC_MPS_AMOUNT").val('0');
        $("#EXEC_MPS_MONTH option:nth(0)").attr('selected', 'selected');
        $("#EXEC_MPS_YEAR option:nth(0)").attr('selected', 'selected');
        
        //class apply
        $("#EXEC_MPS_AMOUNT").attr('class', 'pmgsy-textbox valid');
        $("#EXEC_MPS_MONTH").attr('class', '');
        $("#EXEC_MPS_YEAR").attr('class', '');
    });
}); //end of Document

//Display Payment Schedule details grid
function GetPaymentScheduleList(IMS_PR_ROAD_CODE) {
    jQuery("#tbPaymentSchedule").jqGrid({
        url: '/Execution/GetPaymentScheduleList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Month', 'Year','Scheduled Payment [Rs. Lakh]', 'Edit', 'Delete'],
        colModel: [
                    { name: 'Month', index: 'Month', width: 330, sortable: true, align: "center" },
                    { name: 'Year', index: 'Year', width: 250, sortable: true, align: "center" },
                    { name: 'ScheduledPayment', index: 'ScheduledPayment', width: 300, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center",formatter:FormatColumnEditPaymentSchedule},
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnDeletePaymentSchedule}
        ],
        pager: jQuery('#dvPaymentSchedulePager'),
        rowNum: 5,
        sortname: 'Month',
        sortorder: 'asc',
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Payment Schedule List",
        height: 'auto',
        //width: 'auto',
        rowList: [5, 10, 15, 20],
        rownumbers: true,
        loadComplete: function () {
            //$("#gview_tbPaymentSchedule > .ui-jqgrid-titlebar").hide();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
        },
    });
}

//Payment Schedule Grid Edit  Format column
function FormatColumnEditPaymentSchedule(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the Payment Schedule Details' onClick ='EditPaymentScheduleDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}

//Payment Schedule Grid delete Format column
function FormatColumnDeletePaymentSchedule(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the Payment Schedule Details' onClick ='DeletePaymentScheduleDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}


//Display Work Program Data entry form in edit mode
function EditPaymentScheduleDetails(key) {

    $("#divPaymentScheduleDetails").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divPaymentScheduleDetails").load('/Execution/EditPaymentScheduleDetails/' + key, function () {
            $.validator.unobtrusive.parse($('#frmPaymentSchedule'));
            if ($("#Operation").val() == "U") {
                $("#EXEC_MPS_AMOUNT").focus();
            }
            unblockPage();
        });
    });
}

//Delete Work Program details
function DeletePaymentScheduleDetails(key) {
    if (confirm("Are you sure you want to delete the payment schedule details ? ")) {

        $.ajax({
            url: "/Execution/DeletePaymentScheduleDetails/" + key,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                if ($("#divError").is(":visible")) {
                    $("#divError").hide('slow');
                }
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if ($("#divError").is(":visible")) {
                    $("#divError").hide('slow');
                }
                unblockPage();
                if (response.success) {
                    $('#btnCancel').trigger('click');
                    alert(response.message);
                    $("#tbPaymentSchedule").trigger('reloadGrid');
                }
            }
        });
    }
}

//Popolate Traffic Intensity Year Drop down
function PopulateTrafficIntensityYears(MAST_ER_ROAD_CODE) {

    $("#MAST_TI_YEAR").val(0);
    $("#MAST_TI_YEAR").empty();

    $.ajax({
        url: '/ExistingRoads/PopulateTrafficIntensityYears/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
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


function PopulateMonth()
{
    $("#EXEC_MPS_MONTH").val(0);
    $("#EXEC_MPS_MONTH").empty();

    $.ajax({
        url: '/Execution/PopulateMonth/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        success: function (jsonData) {
            
            for (var i = 0; i <jsonData.length; i++) {
                $("#EXEC_MPS_MONTH").append("<option value='"+ jsonData[i].Value+ "'>"+jsonData[i].Text+"</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error" + err);
            unblockPage();
        },
    });
}

function PopulateYear(IMS_PR_ROAD_CODE) {
    $("#EXEC_MPS_YEAR").val(0);
    $("#EXEC_MPS_YEAR").empty();

    $.ajax({
        url: '/Execution/PopulateYear/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data:{IMS_PR_ROAD_CODE:IMS_PR_ROAD_CODE},
        success: function (jsonData) {

            for (var i = 0; i < jsonData.length; i++) {
                $("#EXEC_MPS_YEAR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error" + err);
            unblockPage();
        },
    });
}
function ValidateCurrentDate()
{
    return true;
}

