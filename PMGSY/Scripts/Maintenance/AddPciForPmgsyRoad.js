﻿// For From Km and To Km Validation

$.validator.unobtrusive.adapters.add('islessthan', ['othervalue'], function (options) {
    options.rules['islessthan'] = options.params;
    options.messages['islessthan'] = options.message;
});

$.validator.addMethod("islessthan", function (value, element, params) {

    //alert("start chainage: " + parseFloat($("#MANE_STR_CHAIN").val()) + "  End Chainage : "  +  parseFloat($("#MANE_END_CHAIN").val()) );

    if ( Boolean(  parseFloat($("#MANE_STR_CHAIN").val()) > parseFloat($("#MANE_END_CHAIN").val()) ) ) {
        return false;
    }

    if (Boolean(parseFloat($("#MANE_END_CHAIN").val()) > parseFloat($("#RoadLength").val()))) {
        return false;
    }

    if (Boolean(parseFloat($("#MANE_END_CHAIN").val()) == parseFloat($("#MANE_STR_CHAIN").val()))) {
        return false;
    }

    return true;
});

$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmPciForPmgsyRoad'));

    //$(function () {
    //    $("#MANE_PCI_DATE").datepicker(
    //    {
    //        dateFormat: "dd-M-yy",
    //        changeMonth: true,
    //        changeYear: true,
    //        maxDate: "+0M +0D"
    //    });

    //    $("#MANE_PCI_DATE").datepicker().attr('readonly', 'readonly');
    //});

    $('#MANE_PCI_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "PCI Date",
        changeMonth: true,
        changeYear: true,
        // minDate:inspectionDate,
        buttonText: "PCI Date",
        onSelect: function (selectedDate) {
            $(function () {
                $('#MANE_PCI_DATE').focus();
                $('#MANE_END_CHAIN').focus();
            })
        }

    });

    //$("#MANE_END_CHAIN").val("");
    //$("#MANE_PCIINDEX").val("");
    
    //$("#btnReset").click(function () {

    //    $("#MANE_END_CHAIN").val("");
    //    $("#MANE_PCIINDEX").val("");

    //});

    $("#MANE_PCI_YEAR").change(function () {

        $("#MANE_PCI_DATE").val("");

        if ($("#MANE_PCI_YEAR").val() > 0) {

            $("#MANE_PCI_DATE").datepicker("option", "minDate", new Date($("#MANE_PCI_YEAR").val(), 3, 1));
            if (parseInt($("#MANE_PCI_YEAR").val()) == parseInt((new Date).getFullYear())) {
                $("#MANE_PCI_DATE").datepicker("option", "maxDate", new Date());
            }
            else {
                $("#MANE_PCI_DATE").datepicker("option", "maxDate", new Date((parseInt($("#MANE_PCI_YEAR").val()) + 1), 2, 31));
            }

            $.ajax({
                url: '/Maintenance/GetRoadLengthDetails',
                type: "POST",
                cache: false,
                data: { ENC_IMS_PR_ROAD_CODE: $("#ENC_IMS_PR_ROAD_CODE").val(), MANE_IMS_YEAR: $("#MANE_PCI_YEAR").val(), value: Math.random() },
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
                        unblockPage();
                        $("#MANE_STR_CHAIN").val(response.MANE_END_CHAIN);                        
                    }
                    else {
                        unblockPage();
                        alert(response.ErrorMessage);
                        $("#MANE_PCI_YEAR").val("0");
                    }
                }
            });

        }
        else {
            $("#MANE_PCI_DATE").val("");
            $("#MANE_STR_CHAIN").val("");
        }

    });

    $("#btnSave").click(function () {
        if ($('#frmPciForPmgsyRoad').valid()) {
            $.ajax({
                url: '/Maintenance/SavePciForPmgsyRoad',
                type: "POST",
                cache: false,
                data: $("#frmPciForPmgsyRoad").serialize(),
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
                        unblockPage();
                        alert("PCI Index Added Succesfully.");
                        $("#tbPciForPmgsyRoad").trigger("reloadGrid");
                        ResetForm();
                        $("#divError").hide("slow");

                        //CloseDetails();                        
                    }
                    else {
                        unblockPage();
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html(response.ErrorMessage);
                        $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                    }
                }
            });
        }
    });

    GetPCIListForPmgsyRoad();
});


function ResetForm(){
    $(':input', '#frmPciForPmgsyRoad').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
}

function GetPCIListForPmgsyRoad() {
    blockPage();

    jQuery("#tbPciForPmgsyRoad").jqGrid({
        url: '/Maintenance/GetPCIListForPmgsyRoad',
        datatype: "json",
        mtype: "POST",
        colNames: ["PCI Year", "Segment Number", "From Km", "To Km", "PCI Value", "Surface Type", "Date of PCI", "Delete"],
        colModel: [
                    { name: 'pciyear', index: 'pciyear', width: 100, sortable: false, align: "center" },
                    { name: 'SegmentNumber', index: 'SegmentNumber', width: 150, sortable: false, align: "center" },
                    { name: 'fromkm', index: 'fromkm', width: 150, sortable: false, align: "center" },
                    { name: 'tokm', index: 'tokm', width: 150, sortable: false, align: "center" },
                    { name: 'pcivalue', index: 'pcivalue', width: 150, sortable: false, align: "center" },
                    { name: 'surfacetype', index: 'surfacetype', width: 150, sortable: false, align: "center" },
                    { name: 'dateofpci', index: 'dateofpci', width: 150, sortable: false, align: "center" },
                    { name: 'delete', index: 'delete', width: 80, sortable: false, align: "center" },
        ],
        postData: { "IMS_PR_ROAD_CODE": $("#ENC_IMS_PR_ROAD_CODE").val(), value: Math.random },
        pager: jQuery('#dvPciForPmgsyRoadPager'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PMGSY Roads",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        //sortname: 'pciyear',
        rownumbers: true,
        grouping: true,
        groupingView: {            
            groupColumnShow: [false],
            groupField: ['pciyear'],
            groupOrder: ['desc']
            
        },
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function DeletePciForPmgsyRoadDetails(paramData) {
    if (confirm("Are you sure to delete PCI Details ?")) {
        $.ajax({
            url: '/Maintenance/DeletePciForPmgsyRoad',
            type: "POST",
            cache: false,
            data: {"Data":paramData , value : Math.random },
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
                    unblockPage();
                    $("#tbPciForPmgsyRoad").trigger("reloadGrid");
                    alert("PCI Index Deleted Succesfully.");                    
                }
                else {
                    alert(response.ErrorMessage);
                    unblockPage();
                }
            }
        });
    }   
}