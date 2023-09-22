$(document).ready(function () {

    $('.EPD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'FDR Stabilize Base Entry Date',
        maxDate: "0D",   //null,  to disable future dates
        minDate: "0D",    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'FDR Stabilize Base Entry Date',
        changeMonth: false,
        changeYear: false,
        stepMonths: false,
        endDate: "today",
        maxDate: "today",
        onSelect: function (selectedDate) {

            $('.EPD').trigger('blur');
        }

    });

    $('.TPS').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'FDR Stabilize Base Entry Date',
        maxDate: "0D",   //null,  to disable future dates
        // minDate: "0D",    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'FDR Stabilize Base Entry Date',
        changeMonth: true,
        changeYear: true,
        stepMonths: true,
        onSelect: function (selectedDate) {

            $('.TPS').trigger('blur');
        }

    });
});


//$('#btnSumit').click(function (evt) {
//    evt.preventDefault();
//    debugger;
//    var DataToSend = [];
//    $('#tblAddFDRChainage tr[id]').each(function () {
//        DataToSend.push({

//           /* "SR_NO": $(this).attr('id'),*/
//          //  "IMS_PR_ROAD_CODE": $(this).find("input[id^='RoadCode']").val(),// RoadCode
//            "FChainageFrom": $(this).find("input[id^='FChainageFrom']").val(),       
//            "FCHAINAGE_TO": $(this).find("input[id^='FChainageFrom']").val(),
//            "FCHAINAGE_DATE": $(this).find("input[id^='FChainageDate']").val(),
//        });
//    });

//    $.ajax({
//        url: '/PMIS/PMIS/SubmitFDRDetail',
//        type: "POST",
//        cache: false,
//        data: JSON.stringify(DataToSend),
//        contentType: 'application/json; charset=utf-8',
//        beforeSend: function () {
//            blockPage();
//        },
//        error: function (xhr, status, error) {
//            unblockPage();
//            Alert("Request can not be processed at this time,please try after some time!!!");
//            return false;
//        },
//        success: function (response) {
//            unblockPage();
//            if (response.Success) {
//                alert("FDR Stabilize base Chainage-Wise Detail Added Successfully.");

//                ClosePMISRoadDetails();
//                unblockPage();
//                LoadPMISRoadList();
//            }
//            else {
//                $("#divError").show("slow");
//                $("#divError span:eq(1)").html(response.ErrorMessage);
//                $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
//                unblockPage();
//            }

//        }
//    });

//});






function isNumericKeyStroke(event) {
    var returnValue = false;
    var keyCode = (event.which) ? event.which : event.keyCode;
    if (((keyCode >= 48) && (keyCode <= 57)) || (keyCode == 46) || (keyCode == 8) || (keyCode == 9) || (keyCode == 37) || (keyCode == 39))// All numerics
    {
        returnValue = true;
    }
    if (event.returnValue)
        event.returnValue = returnValue;
    return returnValue;
}



$("#btnCancel").click(function () {
    if (confirm("Are you sure to cancel and close?")) {
        ClosePMISRoadDetails();
    }
});

function LoadFDRChainageList(DataToSend) {
    /*$('#divListFDR').show('slow');*/

    jQuery("#tbFDRList").jqGrid('GridUnload');
    jQuery("#tbFDRList").jqGrid({
        url: '/PMIS/PMIS/PMISFdrList',
        datatype: "json",
        mtype: "POST",
        colNames: ['RoadCode', 'Entry Date', 'Start Chainage', 'End Chainage', 'Date'],
        colModel: [
            { key: true, hidden: true, name: 'IMS_PR_ROAD_CODE', index: 'DATE', width: 10, align: "left", },
            { name: 'ENTRY_DATE', index: 'ENTRY_DATE', width: 20, align: "center" },
            { name: 'START_CHAINAGE', index: 'START_CHAINAGE', width: 20, align: "center" },
            { name: 'END_CHAINAGE', index: 'END_CHAINAGE', width: 20, align: "center" },
            { name: 'DATE', index: 'DATE', width: 20, align: "center" },
        ],
        postData: {
            'IMS_PR_ROAD_CODE': DataToSend,
        },
        pager: jQuery('#pagerFDR'),
        rowNum: 20,
        rowList: [20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        grouping: true,
        groupingView: {
            groupField: ['ENTRY_DATE'],
            groupOrder: ['asc']
        },
        caption: "&nbsp;&nbsp;FDR Stabilize Base Chainage-Wise List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#tbFDRList #pagerFDR").css({ height: '31px' });
            $('#divaddFDRChainageDetail').hide('slow');
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!");

            }
        }

    }); //end of grid



}



$('#btnViewFDR').click(function (evt) {
    evt.preventDefault();
    debugger;
    var DataToSend = 0;
    DataToSend = $('#RoadCode').val();
    $('#divListFDR').show();
    LoadFDRChainageList(DataToSend);


});

// btnBackToEntry
$('#btnBackToEntry').click(function (evt) {
    evt.preventDefault();
    debugger;
    $('#divaddFDRChainageDetail').show('slow');
    $('#divListFDR').hide();

});



// on drawing form submit
$("#formProformaCImageUpload").on('submit', function (event) {
    //alert($('#IS_SUBMIT').val());
    $("#errSummary").hide("slow");
    // $.validator.unobtrusive.parse("#formProformaCImageUpload");
    // alert($("#formProformaCImageUpload").valid());
    if ($("#formProformaCImageUpload").valid()) {

        // to hide when submit again after correction / failure
        $("#divError").hide("slow");

        event.stopPropagation(); // Stop stuff happening call double avoid to action
        event.preventDefault(); // call double avoid to action

        // Attach All Files to Form Data
        var form_data = new FormData();


        var data = $("#formProformaCImageUpload").serializeArray();

        for (var i = 0; i < data.length; i++) {
            form_data.append(data[i].name, data[i].value);
        }
        if ($('#IS_SUBMIT').val() == "N") {
            if (confirm("Are you sure, you want submit ?")) {

                $.ajax({
                    url: '/PMIS/PMIS/SubmitFDRDetail',
                    type: 'POST',
                    cache: false,
                    data: form_data,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    },
                    error: function (xhr, status, error) {
                        $.unblockUI();
                        alert("Request can not be  processed at this time, please try after some time...");
                        return false;
                    },
                    success: function (data) {

                        if (data.Success) {

                            alert("Submitted Successfully");
                            ClosePMISRoadDetails();

                            LoadPMISRoadList();
                            $.unblockUI();
                        }
                        else {
                            // alert("ERROR");

                            alert(data.ErrorMessage);
                            if (data.ErrorListgenerated) {
                                //FDRstabForm
                                $("#FDRstabForm").hide("slow");
                                $("#errSummary").show();
                                $("#ErroMsg").empty();
                                $("#ErroMsg").html(data.data);
                                /* $('#errSummary').focus();*/
                                /*                                    $('#mainDiv').animate({ scrollTop: 0 }, 'slow');*/
                            }

                            $.unblockUI();
                        }
                    },
                });
                return true;

            } else {
                return false;
            }
        }
        else if (($('#IS_SUBMIT').val() == "Y")) {
            if (confirm("Are you sure, you want update ?")) {

                $.ajax({
                    url: '/PMIS/PMIS/UpdateFDRDetail',
                    type: 'POST',
                    cache: false,
                    data: form_data,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    },
                    error: function (xhr, status, error) {
                        $.unblockUI();
                        alert("Request can not be  processed at this time, please try after some time...");
                        return false;
                    },
                    success: function (data) {

                        if (data.Success) {

                            alert("Updated Successfully");
                            ClosePMISRoadDetails();

                            LoadPMISRoadList();
                            $.unblockUI();
                        }
                        else {
                            // alert("ERROR");

                            alert(data.ErrorMessage);
                            if (data.ErrorListgenerated) {
                                $("#FDRstabForm").hide("slow");
                                $("#errSummary").show();
                                $("#ErroMsg").empty();
                                $("#ErroMsg").html(data.data);
                            }

                            $.unblockUI();
                        }
                    },
                });
                return true;

            } else {
                return false;
            }
        }


    }



});//end of save



//$('#btnUpdate').click(function (evt) {
//    $.validator.unobtrusive.parse("#frmAddFDR");
//    evt.preventDefault();
//    debugger;
//    alert("1");
//    //var DataToSend = [];
//    //$('#tblAddFDR tr[id]').each(function () {
//    //    DataToSend.push({
//    //        "SR_NO": $(this).attr('id'),
//    //        "IMS_PR_ROAD_CODE": $(this).find("input[id^='RoadCode']").val(),// RoadCode
//    //        "CHAINAGE_FROM": $(this).find("input[id^='ChainageFrom']").val(),
//    //        "CHAINAGE_TO": $(this).find("input[id^='ChainageTo']").val(),
//    //        "CHAINAGE_DATE": $(this).find("input[id^='ChainageDate']").val(),
//    //    });
//    //});
//  //  var data = $("#formProformaCImageUpload").serializeArray();
//    alert("2");
//    if ($('#frmAddFDR').valid()) {
//        $.ajax({
//            url: '/PMIS/PMIS/UpdateFDRDetail',
//            type: "POST",
//            cache: false,
//            //data: JSON.stringify(DataToSend),
//            data: $("#frmAddFDR").serialize(),
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            async: false,
//            cache: false,
//            beforeSend: function () {
//                blockPage();
//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            },
//            success: function (response) {
//                unblockPage();
//                if (response.Success) {
//                    alert("FDR Stabilize base Chainage-Wise Detail Updated Successfully.");
//                    ClosePMISRoadDetails();
//                    unblockPage();
//                    LoadPMISRoadList();
//                }
//                else {
//                    $("#divError").show("slow");
//                    $("#divError span:eq(1)").html(response.ErrorMessage);
//                    $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
//                    unblockPage();
//                }

//            }
//        });

//    }

//});

$("#closeErrorSummary").click(function () {
    if (confirm('Are you sure to close error summary?') == true) {
        $("#errSummary").hide("slow");
        $("#FDRstabForm").show("slow");

    }
});

function deleteDateF(i) {

    $('#Chainage_Date_FirstChainage' + i).val("");

}
function deleteDateS(i) {

    $('#Chainage_Date_SecondChainage' + i).val("");

}

