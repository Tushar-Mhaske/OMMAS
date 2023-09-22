/*
 * Purpose:-  Work Program.js is used to show Work Program Grid and Work Program data entry form
 * 
 */

//date validation
jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {
    
    var fromDate =$('#EXEC_START_DATE').val();
    var toDate = $('#EXEC_END_DATE').val()

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    if (sDate >= eDate) {
        return false;
    }
    else {
        return true;
    }

});
jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmWorkProgram'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#MAST_HEAD_CODE").change(
        function () {
            if ($("#divError").is(":visible")) {
                $("#divError").hide('slow');
            }
        });
    $("#EXEC_START_DATE").click(
      function () {
          if ($("#divError").is(":visible")) {
              $("#divError").hide('slow');
          }
      });

    $("#EXEC_END_DATE").click(
      function () {
          if ($("#divError").is(":visible")) {
              $("#divError").hide('slow');
          }
      });
    
    initializeStartDate();
    initializeEndDate();


    //Work program function
    GetWorkProgramList($("#IMS_PR_ROAD_CODE").val());
    
    
    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();

        //auto focus
        $(function () {
            $("#EXEC_START_DATE").focus();
        });

    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }

    //Save Work Program Details into the database
    $('#btnSave').click(function (evt) {
        evt.preventDefault();

        if ($('#frmWorkProgram').valid()) {

            $.ajax({
                url: "/Execution/AddWorkProgramDetails/",
                type: "POST",
                cache: false,
                data: $("#frmWorkProgram").serialize(),
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
                        initializeStartDate();
                        initializeEndDate();
                        $("#tbWorkProgram").trigger('reloadGrid');
                        $("#frmWorkProgram").trigger('reset');

                        if ($("#ProposalType").val() == "P") {
                            PopulateHeadItemForRoad($("#IMS_PR_ROAD_CODE").val());
                        }
                        else if ($("#ProposalType").val() == "L") {
                            PopulateHeadItemForLSB($("#IMS_PR_ROAD_CODE").val());
                        }
                        $("#divError").hide("slow");
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

    //Update Work Program Details 
    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        $("#MAST_HEAD_CODE").attr('disabled', false);

        if ($('#frmWorkProgram').valid()) {
            $.ajax({
                url: "/Execution/EditWorkProgramDetails/",
                type: "POST",
                cache: false,
                data: $("#frmWorkProgram").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    $("#MAST_HEAD_CODE").attr('disabled', true);
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();

                    //alert(response.success);

                    $("#MAST_HEAD_CODE").attr('disabled', false);
                    if (response.success) {

                        alert(response.message);

                        initializeStartDate();
                        initializeEndDate();


                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();

                        $("#tbWorkProgram").trigger('reloadGrid');

                        $("#EXEC_START_DATE").val(null);
                        $("#EXEC_END_DATE").val(null);

                        if ($("#ProposalType").val() == "P") {
                            PopulateHeadItemForRoad($("#IMS_PR_ROAD_CODE").val());
                            
                        }
                        else if ($("#ProposalType").val() == "L") {
                            PopulateHeadItemForLSB($("#IMS_PR_ROAD_CODE").val());
                        }
                        if ($("#divError").is(":visible")) {
                            $("#divError").hide('slow');
                        }
                    } else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                        $("#MAST_HEAD_CODE").attr('disabled', true);
                    }
                }
            });
        }
        $("#MAST_HEAD_CODE").attr('disabled', true);
    });

    //display CBR Data entry form
    $('#btnCancel').click(function () {

        $("#Operation").val("A");
        $("#rowAdd").show();
        $("#rowUpdate").hide();


        $("#rowHideShow").show();
        
        $("#EXEC_START_DATE").val(null);
        $("#EXEC_END_DATE").val(null);

        //reset start date/ end date
        initializeStartDate();
        initializeEndDate();


        if ($("#ProposalType").val() == "P") {
            PopulateHeadItemForRoad($("#IMS_PR_ROAD_CODE").val());

        }
        else if ($("#ProposalType").val() == "L") {
            PopulateHeadItemForLSB($("#IMS_PR_ROAD_CODE").val());
        }

        $("#MAST_HEAD_CODE").attr('disabled', false);

        if ($("#divError").is(":visible")) {
            $("#divError").hide('slow');
        }
        if ($("#divError").is(":visible")) {
            $("#divError").hide('slow');
        }

    });//end of cancel


    var dates = $("input[id$='EXEC_START_DATE'],input[id$='EXEC_END_DATE']");

    $("#btnResetWorkProgram").click(function () {

        if ($("#divError").is(":visible")) {
            $("#divError").hide('slow');
        }

        //reset date
        initializeStartDate();
        initializeEndDate();


        $("#valMsgStartDate").html('');
        $("#valMsgEndDate").html('');        

        $("#EXEC_START_DATE").val(null);
        $("#EXEC_END_DATE").val(null);

        $("#EXEC_START_DATE").attr('class', 'pmgsy-textbox hasDatepicker');
        $("#EXEC_END_DATE").attr('class', 'pmgsy-textbox hasDatepicker');

        $("#MAST_HEAD_CODE option:nth(0)").attr('selected', 'selected');

        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });

        $(".pmgsy-textbox").removeClass('input-validation-error');
        $(".field-validation-error").html('');
        $("#EXEC_START_DATE").removeClass('input-validation-error');
        $("#EXEC_END_DATE").removeClass('input-validation-error');

    });

    $(function () {
        $("#frmWorkProgram").trigger('reset');
    });
  
});

//Display Work PRogram Data entry form in edit mode
function EditWorkProgram(key) {

    $("#divWorkProgramDetails").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divWorkProgramDetails").load('/Execution/EditWorkProgramDetails/' + key, function () {
            $.validator.unobtrusive.parse($('#frmWorkProgram'));
            if ($("#Operation").val() == "U") {
                $("#EXEC_START_DATE").focus();
            }
            unblockPage();
        });
    });
}

//Delete Work Program details
function DeleteWorkProgram(key) {
    if (confirm("Are you sure you want to delete the work program details ? ")) {

        $.ajax({
            url: "/Execution/DeleteWorkProgramDetails/" + key,
            type: "POST",
            cache: false,          
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
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
                    alert("Work Program Details Deleted Succesfully.");
                    $("#tbWorkProgram").trigger('reloadGrid');

                    //populate dropdown
                    if ($("#ProposalType").val() == "P") {
                        PopulateHeadItemForRoad($("#IMS_PR_ROAD_CODE").val());
                    }
                    else if ($("#ProposalType").val() == "L") {
                        PopulateHeadItemForLSB($("#IMS_PR_ROAD_CODE").val());
                    }
                }                
            }
        });
    }
}


//Display CBR details grid
function GetWorkProgramList(IMS_PR_ROAD_CODE) {
    jQuery("#tbWorkProgram").jqGrid({
        url: '/Execution/GetWorkProgramList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Head Items', 'Start Date [DD-MM-YYYY]','End Date [DD-MM-YYYY]', 'Edit', 'Delete'],
        colModel: [
                    { name: 'HeadItem', index: 'HeadItem', width: 350, sortable: true, align: "center" },
                    { name: 'StartDate', index: 'StartDate', width: 250, sortable: true, align: "center" },
                    { name: 'EndDate', index: 'EndDate', width: 250, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 50, sortable: false, align: "center",formatter:FormatColumnEditWorkProgram },
                    { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center",formatter: FormatColumnDeleteWorkProgram }
        ],
        pager: jQuery('#dvWorkProgramPager'),
        rowNum: 5,
        sortname: 'HeadItem',
        sortorder: 'asc',
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Work Program List",
        height: 'auto',
        //width: 'auto',
        rowList: [5, 10, 15, 20],
        rownumbers: true,
        loadComplete: function () {
            //$("#gview_tbWorkProgram > .ui-jqgrid-titlebar").hide();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
        },
    });
}

//CBR Grid Format column
function FormatColumnEditWorkProgram(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the Work Program Details' onClick ='EditWorkProgram(\"" + cellvalue.toString() + "\");'></span></center>";
}
//CBR Grid Format column
function FormatColumnDeleteWorkProgram(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the Work Program Details' onClick ='DeleteWorkProgram(\"" + cellvalue.toString() + "\");'></span></center>";
}

//Popolate Head Item For Raod Drop down
function PopulateHeadItemForRoad(imsPrRoadCode) {

    $("#MAST_HEAD_CODE").val(0);
    $("#MAST_HEAD_CODE").empty();

    $.ajax({
        url: '/Execution/PopulateHeadItemForRoad/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { IMS_PR_ROAD_CODE: imsPrRoadCode },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_HEAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });
}

//Popolate Head Item For LSB Drop down
function PopulateHeadItemForLSB(imsPrRoadCode) {

    $("#MAST_HEAD_CODE").val(0);
    $("#MAST_HEAD_CODE").empty();

    $.ajax({
        url: '/Execution/PopulateHeadItemForLSB/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { IMS_PR_ROAD_CODE: imsPrRoadCode },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_HEAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });
}


function initializeStartDate()
{

    var startDate = $('#AgreementStartDate').val();
    var endDate = $('#AgreementEndDate').val();

    //date picker for start 
    $('#EXEC_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: startDate,
        //maxDate: endDate,

        onSelect: function (selectedDate) {
            //$("#EXEC_END_DATE").datepicker("option", "minDate", selectedDate);
            $(function () {
                $("#EXEC_START_DATE").focus();
                $("#EXEC_END_DATE").focus();
            });
        }
    });
}

function initializeEndDate()
{
    var startDate = $('#AgreementStartDate').val();
    var endDate = $('#AgreementEndDate').val();
    //date picker for end date
    $('#EXEC_END_DATE').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        //minDate: startDate,
        //maxDate: endDate,
        onSelect: function (selectedDate) {
            //$("#EXEC_START_DATE").datepicker("option", "maxDate", selectedDate);
            $(function () {
                $("#EXEC_END_DATE").focus();
                $("#btnSave").focus();
            });
        }
    });

}