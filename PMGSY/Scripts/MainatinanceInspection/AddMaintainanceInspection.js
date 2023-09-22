/*-----------------------------------------------------------------------------------------------------
File Name:AddMaintainanceInspection.js
Path: ~PMGSY/Scripts/AddMaintainanceInspection
Created By: Ashish Markande
Creation Date: 27/07/2013
Purpose: Add/Edit/Delete and to load inspection details  grid view.
--------------------------------------------------------------------------------------------------------
*/

jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    //if (new Date($('#MANE_RECTIFICATION_DATE').val()) < new Date($('#MANE_INSP_DATE').val()))
    //    return false;
    //else
    //    return true;


    var fromDate = $("#MANE_INSP_DATE").val();
    var toDate = $("#MANE_RECTIFICATION_DATE").val();

    //Detailed check for valid date ranges
    //if your date is like 09-09-2012
    var frommonthfield = fromDate.split("-")[1];
    var fromdayfield = fromDate.split("-")[0];
    var fromyearfield = fromDate.split("-")[2];


    var tomonthfield = toDate.split("-")[1];
    var todayfield = toDate.split("-")[0];
    var toyearfield = toDate.split("-")[2];


    var sDate = new Date(fromyearfield, frommonthfield , fromdayfield);
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
    $.validator.unobtrusive.parse("#frmInspection");
    loadInspectionList();


    $('#btnInspectionDetailsCancel').click(function () {
        var RoadCode = $("#IMS_PR_ROAD_CODE").val();
         $("#dvAddInspection").load("/MaintainanceInspection/AddInspectionDetail?id=" + RoadCode, function () {


            $.validator.unobstrusive.parse($("#dvAddInspection"));

        });
 });

    

    $("#MANE_INSP_DATE").focus(function () {
        if ($("#MANE_INSP_DATE").val() != null) {
            $("#MANE_RECTIFICATION_DATE").datepicker();
        }
    });

    $("#ddlDesignation").change(function () {
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading Name...</label> ' });
        var val = $("#ddlDesignation").val();
        $.ajax({
            type: 'POST',
            url: "/MaintainanceInspection/GetNodalOfficerName?desigCode=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlNoName").empty();
                $.each(data, function () {
                    $("#ddlNoName").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");

                });

                $.unblockUI();
            }

        });


    });


    $("#btnCreateInspection").click(function () {      
          if ($("#frmInspection").valid()) {           
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/MaintainanceInspection/AddMaintainanceInspection",
                type: "POST",

                data: $("#frmInspection").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tbMainInspectionList').trigger('reloadGrid');
                        $("#btnReset").trigger('click');
                        $("#dvRoadDetails").hide("slow");
                        $("#dvRequiredField").hide("slow");
                        $.unblockUI();

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#divError').show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvRoadDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    });


    $('#imgCloseAgreementDetails').click(function () {
        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseFinancetDetails').click(function () {


        if ($("#accordionFinance").is(":visible")) {
            $('#accordionFinance').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseInspectionDetails').click(function () {
        $('#divError').hide('slow');
        $("#dvRoadDetails").hide("slow");
        $("#dvRequiredField").hide("slow");

    });

    $("#btnInspectionUpdate").click(function () {
           
        if ($("#frmInspection").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/MaintainanceInspection/EditMaintainanceInspection",
                type: "POST",
                data: $("#frmInspection").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#tbMainInspectionList').trigger('reloadGrid');
                       // $("#btnReset").trigger('click');
                        $("#dvRoadDetails").hide("slow");
                        $("#dvRequiredField").hide("slow");
                        var roadCode=  $("#IMS_PR_ROAD_CODE").val();
                   
                      $("#dvAddInspection").load('/MaintainanceInspection/AddInspectionDetail?id=' + roadCode);
                        // $("#dvErrorMessage").hide();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#divError').show('slow');
                        }
                    } else {
                        $("#dvRoadDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    });

    var inspectionDate = $("#StartDate").val();
    var date2;
    $('#MANE_INSP_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear:true,
        title: "Expiry Date",
        minDate:inspectionDate,
       // maxDate:new Date(),
        buttonText: "Inspection Date",
        onSelect:function(selectdDate){
            
            $(function () {
                $('#MANE_INSP_DATE').focus();
                $('#MANE_RECTIFICATION_DATE').focus();
            })

            if ($('#MANE_INSP_DATE').val() != "") {
                date2 = $('#MANE_INSP_DATE').datepicker('getDate', '+1d');
                date2.setDate(date2.getDate() + 1);
                // $('#MANE_RECTIFICATION_DATE').datepicker('setDate',date2);
                $('#MANE_RECTIFICATION_DATE').datepicker("option", "minDate", date2);
            }
           // alert(date2);

    }

    });


    var dates = $("input[id$='MANE_INSP_DATE'],input[id$='MANE_RECTIFICATION_DATE']");

    $("#btnReset").click(function () {
        $("#divError").hide();

        dates.attr('value', '');
        dates.each(function () {
            $.datepicker._clearDate(this);
        });

    });

    //var startDate = new Date(inspectionDate);
    
    //var rectificationDate = ;
    //alert(rectificationDate);
    
    $('#MANE_RECTIFICATION_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "Expiry Date",
        changeMonth: true,
        changeYear: true,
        minDate: date2,
        buttonText: "Rectification Date",
        onSelect: function (selectdDate) {
            $('#MANE_INSP_DATE').datepicker("option", "maxDate", selectdDate);
            $(function () {
                $('#MANE_RECTIFICATION_DATE').focus();
                $('#btnCreateInspection').focus();

            })
        }

    });

});




function ViewSearchDiv() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
        }

        $("#dvSearchProposedRoad").show('slow');
        $.unblockUI();
    }
    $.unblockUI();

}



function loadInspectionList() {
    
   
    jQuery("#tbMainInspectionList").jqGrid({
        url: '/MaintainanceInspection/GetInspectionRoadList/',
        postData: { ImsPrRoadCode: $('#IMS_PR_ROAD_CODE').val() },
        datatype: "json",
   
        mtype: "POST",
        colNames: ['Designation','Name', 'Inspection Date', 'Rectification Date','Status', 'Edit','Delete','Change Rectification Status'],
        colModel: [
                            { name: 'Designation', index: 'Designation', height: 'auto', width: "190px", align: "left", sortable: true },
                            { name: 'Name', index: 'Name', height: 'auto', width: "250px", align: "left", sortable: true },
                            { name: 'InspectionDate', index: 'InspectionDate', height: 'auto', width: "180px", sortable: true, },
                            { name: 'RectificationDate', index: 'RectificationDate', width: "180px", sortable: true },
                            { name: 'Status', index: 'Status', height: 'auto', width: "100px", align: "left", sortable: true },
                            { name: 'edit', width: 40, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                            { name: 'delete', width: 40, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
                            { name: 'change', width: 70, sortable: false, resize: false, formatter: FormatColumnChange, align: "center", sortable: false }

        ],
        pager: jQuery('#dvInspectionListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Inspection Work List",
        height: 'auto',
      
        width: "100%",
        rownumbers: true,
        hidegrid: true,
        sortname: 'Name',
        sortorder: "asc",
        loadComplete: function () {

            $("#gview_tbMainInspectionList > .ui-jqgrid-titlebar").hide();
            $("#tbMainInspectionList #dvInspectionListPager").css({ height: '40px' });
            $("#dvInspectionListPager_left").html("<input type='button' style='margin-left:27px' id='idAddFinancialProgress' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddInspectionDetails();return false;' value='Add Inspection Details' title='Add Inspection Details'/>")

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
               
                alert("Invalid data.Please check and Try again!")
               
            }
        }

    }); //end of grid
}

function AddInspectionDetails() {

     var RoadCode = $("#IMS_PR_ROAD_CODE").val();
     $("#dvAddInspection").load("/MaintainanceInspection/AddInspectionDetail?id=" + RoadCode, function () {
    // $.validator.unobstrusive.parse($("#dvAddInspection"));
        
    });

}

function FormatColumnEdit(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Inspection Details' onClick ='editInspectionDetails(\"" + cellvalue.toString() + "\")'></span></td></tr></table></center>";
}
function FormatColumnDelete(cellvalue, options, rowObject) {
    return "<center><table><tr><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Inspection Details' onClick =deleteInspectionDetails(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";
}
function FormatColumnChange(cellvalue, options, rowObject) {
    return "<center><table><tr><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-transferthick-e-w' title='Change Inspection Status' onClick =editInspectionDetails(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";
}

function editInspectionDetails(urlParam) {
      
    $("#dvAddInspection").load("/MaintainanceInspection/EditMaintainanceInspection/" + urlParam, function () {

        $("#ddlDesignation").val($("#Designation").val());
        $("#ddlDesignation").trigger("change");
        $("#ddlNoName").val($("#MAST_OFFICER_CODE").val());
        $.validator.unobstrusive.parse($("#dvAddMaintenanceInspection"));
       
    });
      
        
       



 

}

function deleteInspectionDetails(urlParam) {
    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete inspection details?")) {
        $.ajax({
            url: "/MaintainanceInspection/DeleteMaintainanceInspection/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert("Inspection details deleted successfully.");


                    $("#tbMainInspectionList").trigger('reloadGrid');

                }
                else {
                    alert("You can not delete this inspection details.");
                }
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }

        });
    }
    else {
        return false;
    }
}

//added by abhishek kamble 21-nov-2013
//function changeInspectionStatus()
//{
//    //var RoadCode = $("#IMS_PR_ROAD_CODE").val();
//    //$("#dvAddInspection").load("/MaintainanceInspection/ChangeInspectionStatus?id=" + RoadCode, function () {
//    //});
//}