$(document).ready(function () {

    $('#idCollapseSpanViewDistrict').click(function (e) {
        e.stopImmediatePropagation();
        $("#districtDiv").show();
        AddDistrictDetails($('#tourClaimIdDistrict').val());
        $('#travelDiv').hide();
        $('#lodgeDiv').hide();
        $('#inspectionDiv').hide();
        $('#meetingDiv').hide();
        $('#idCollapseSpanViewDistrict').hide();
        $('#idCollapseSpanHideDistrict').show();

        $('#idCollapseSpanViewTravel').show();
        $('#idCollapseSpanHideTravel').hide();
        $('#idCollapseSpanViewLodge').show();
        $('#idCollapseSpanHideLodge').hide();
        $('#idCollapseSpanViewInspection').show();
        $('#idCollapseSpanHideInspection').hide();
        $('#idCollapseSpanViewMeeting').show();
        $('#idCollapseSpanHideMeeting').hide();
    });

    $('#idCollapseSpanHideDistrict').click(function (e) {
        $("#districtDiv").hide();
        $('#idCollapseSpanViewDistrict').show();
        $('#idCollapseSpanHideDistrict').hide();

    });

    $('#idCollapseSpanViewTravel').click(function (e) {
        e.stopImmediatePropagation();

        $("#travelDiv").show();
        AddTravelClaim($('#tourClaimIdDistrict').val());
        $('#districtDiv').hide();
        $('#lodgeDiv').hide();
        $('#inspectionDiv').hide();
        $('#meetingDiv').hide();
        $('#idCollapseSpanViewTravel').hide();
        $('#idCollapseSpanHideTravel').show();

        $('#idCollapseSpanViewDistrict').show();
        $('#idCollapseSpanHideDistrict').hide();
        $('#idCollapseSpanViewLodge').show();
        $('#idCollapseSpanHideLodge').hide();
        $('#idCollapseSpanViewInspection').show();
        $('#idCollapseSpanHideInspection').hide();
        $('#idCollapseSpanViewMeeting').show();
        $('#idCollapseSpanHideMeeting').hide();
    });

    $('#idCollapseSpanHideTravel').click(function (e) {
        $("#travelDiv").hide();
        $('#idCollapseSpanViewTravel').show();
        $('#idCollapseSpanHideTravel').hide();

    });

    $('#idCollapseSpanViewLodge').click(function (e) {
        e.stopImmediatePropagation();

        $("#lodgeDiv").show();
        AddLodgeClaim($('#tourClaimIdDistrict').val());
        $('#districtDiv').hide();
        $('#travelDiv').hide();
        $('#inspectionDiv').hide();
        $('#meetingDiv').hide();
        $('#idCollapseSpanViewLodge').hide();
        $('#idCollapseSpanHideLodge').show();

        $('#idCollapseSpanViewDistrict').show();
        $('#idCollapseSpanHideDistrict').hide();
        $('#idCollapseSpanViewTravel').show();
        $('#idCollapseSpanHideTravel').hide();
        $('#idCollapseSpanViewInspection').show();
        $('#idCollapseSpanHideInspection').hide();
        $('#idCollapseSpanViewMeeting').show();
        $('#idCollapseSpanHideMeeting').hide();
    });

    $('#idCollapseSpanHideLodge').click(function (e) {
        $("#lodgeDiv").hide();
        $('#idCollapseSpanViewLodge').show();
        $('#idCollapseSpanHideLodge').hide();

    });

    $('#idCollapseSpanViewInspection').click(function (e) {
        e.stopImmediatePropagation();

        $("#inspectionDiv").show();
        PerformInspectionOfRoad($('#tourClaimIdDistrict').val());
        $('#districtDiv').hide();
        $('#travelDiv').hide();
        $('#lodgeDiv').hide();
        $('#meetingDiv').hide();
        $('#idCollapseSpanViewInspection').hide();
        $('#idCollapseSpanHideInspection').show();

        $('#idCollapseSpanViewDistrict').show();
        $('#idCollapseSpanHideDistrict').hide();
        $('#idCollapseSpanViewTravel').show();
        $('#idCollapseSpanHideTravel').hide();
        $('#idCollapseSpanViewLodge').show();
        $('#idCollapseSpanHideLodge').hide();
        $('#idCollapseSpanViewMeeting').show();
        $('#idCollapseSpanHideMeeting').hide();
    });

    $('#idCollapseSpanHideInspection').click(function (e) {
        $("#inspectionDiv").hide();
        $('#idCollapseSpanViewInspection').show();
        $('#idCollapseSpanHideInspection').hide();

    });

    $('#idCollapseSpanViewMeeting').click(function (e) {
        e.stopImmediatePropagation();

        $("#meetingDiv").show();
        MeetingWithPIU($('#tourClaimIdDistrict').val());
        $('#districtDiv').hide();
        $('#travelDiv').hide();
        $('#lodgeDiv').hide();
        $('#inspectionDiv').hide();
        $('#idCollapseSpanViewMeeting').hide();
        $('#idCollapseSpanHideMeeting').show();

        $('#idCollapseSpanViewDistrict').show();
        $('#idCollapseSpanHideDistrict').hide();
        $('#idCollapseSpanViewTravel').show();
        $('#idCollapseSpanHideTravel').hide();
        $('#idCollapseSpanViewLodge').show();
        $('#idCollapseSpanHideLodge').hide();
        $('#idCollapseSpanViewInspection').show();
        $('#idCollapseSpanHideInspection').hide();
    });

    $('#idCollapseSpanHideMeeting').click(function (e) {
        $("#meetingDiv").hide();
        $('#idCollapseSpanViewMeeting').show();
        $('#idCollapseSpanHideMeeting').hide();

    });

    // Added For Dymanic Year and month
    function daysInMonth(month, year) {
        return new Date(year, month, 0).getDate();
    }

    function getDynamicYear(month) {
        if (Number(month) == 12) {
            return (Number($("#ddlNQMScheduleYear").val()) + 1);
        }
        return $("#ddlNQMScheduleYear").val();
    };

    function getDynamicMonth(month) {
        if (Number(month) == 12) {
            return Number(month);
        }
        return Number(month) + 1;
    };

    //function getMonthFromString(mon) {
    //    return new Date(Date.parse(mon + " 1, 2022")).getMonth() + 1
    //}

    $('.dateOfTour').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Date of Tour',
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))), /*"+2m",*/   //null,  to disable future dates
        minDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        defaultDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        buttonImageOnly: true,
        buttonText: 'Date of Tour',
        /*gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        timepicker: false,
        stepMonths: 0,
        onSelect: function (selectedDate) {
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('.dateOfTour').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });

    $('#dateFromTour').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: "+2m",
        /*gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        stepMonths: 0,
        buttonText: 'Date From',
        onSelect: function (selectedDate) {
            $("#dateToTour").datepicker("option", "minDate", selectedDate);
            $("#dateTo").datepicker("option", "minDate", selectedDate);
            $("#dateToLodge").datepicker("option", "minDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateFromTour').trigger('blur');
        },
    });

    $('#dateToTour').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: "+2m",
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date To',
        onSelect: function (selectedDate) {
            $("#dateFromTour").datepicker("option", "maxDate", selectedDate);
            $("#dateFrom").datepicker("option", "maxDate", selectedDate);
            $("#dateFromLodge").datepicker("option", "maxDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateToTour').trigger('blur');
        },
    });

    $('#dateFromTourUpdate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: "+2m",
       /* gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        /*stepMonths: 0,*/
        buttonText: 'Date From',
        onSelect: function (selectedDate) {
            $("#dateToTourUpdate").datepicker("option", "minDate", selectedDate);
            $("#dateTo").datepicker("option", "minDate", selectedDate);
            $("#dateToLodge").datepicker("option", "minDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateFromTourUpdate').trigger('blur');
        },
    });

    $('#dateToTourUpdate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: "+2m",
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date To',
        onSelect: function (selectedDate) {
            $("#dateFromTourUpdate").datepicker("option", "maxDate", selectedDate);
            $("#dateFrom").datepicker("option", "maxDate", selectedDate);
            $("#dateFromLodge").datepicker("option", "maxDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateToTourUpdate').trigger('blur');
        },
    });
    
    $('#dateFrom').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        minDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        defaultDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        /*gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        stepMonths: 0,
        buttonText: 'Date From',
        /*showAnim: 'slideDown',*/
        onSelect: function (selectedDate) {

            $("#dateTo").datepicker("option", "minDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateFrom').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
       
    });

    $('#dateTo').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date To',
        /*showAnim: 'slideDown',*/
        onSelect: function (selectedDate) {
            /*$("#dateFrom").datepicker("option", "maxDate", selectedDate);*/
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateTo').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });

    $('#startDateOfTravel').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        minDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        defaultDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        /*gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        stepMonths: 0,
        buttonText: 'Start Date',
        onSelect: function (selectedDate) {
            $("#endDateOfTravel").datepicker("option", "minDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#startDateOfTravel').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });

    $('#endDateOfTravel').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        changeMonth: true,
        changeYear: true,
        buttonText: 'End Date',
        onSelect: function (selectedDate) {
            /*$("#dateFromLodge").datepicker("option", "maxDate", selectedDate);*/
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#endDateOfTravel').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });
     
    $('#dateFromLodge').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        minDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        defaultDate: "01/" + $("#ddlNQMScheduleMonth").val() + "/" + $("#ddlNQMScheduleYear").val(),
        /*gotoCurrent: true,*/
        changeMonth: true,
        changeYear: true,
        stepMonths: 0,
        buttonText: 'Date From',
        onSelect: function (selectedDate) {
            $("#dateToLodge").datepicker("option", "minDate", selectedDate);
            jQuery.validator.methods["date"] = function (value, element) { return true; }           
            $('#dateFromLodge').trigger('blur');          
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });

    $('#dateToLodge').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        maxDate: new Date(getDynamicYear($("#ddlNQMScheduleMonth").val()) + "-" + getDynamicMonth($("#ddlNQMScheduleMonth").val()) + "-" + daysInMonth(getDynamicMonth($("#ddlNQMScheduleMonth").val()), getDynamicYear($("#ddlNQMScheduleMonth").val()))),
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date To',
        onSelect: function (selectedDate) {
            /*$("#dateFromLodge").datepicker("option", "maxDate", selectedDate);*/
            jQuery.validator.methods["date"] = function (value, element) { return true; }
            $('#dateToLodge').trigger('blur');
        },
        onChangeMonthYear: function (year, month, inst) {
            var m = Number($("#ddlNQMScheduleMonth").val());
            if (year == Number($("#ddlNQMScheduleYear").val()) + 1) {
                if (m == 12) { inst.settings.maxDate = new Date(year + "-01-31"); }
            }
        }
    });

    $('#ddlStates').change(function () {
        $("#ddlDistrict").empty();
        $.ajax({
            url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
            type: 'POST',
            async: false,
            cache: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStates").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    })

    //$('#ddlStatesMeeting').change(function () {
    //    $("#ddlDistrictMeeting").empty();
    //    $.ajax({
    //        url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
    //        type: 'POST',
    //        async: false,
    //        cache: false,
    //        beforeSend: function () {
    //            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    //        },
    //        data: { stateCode: $("#ddlStatesMeeting").val(), },
    //        success: function (jsonData) {
    //            for (var i = 0; i < jsonData.length; i++) {
    //                $("#ddlDistrictMeeting").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
    //            }

    //            $.unblockUI();
    //        },
    //        error: function (err) {
    //            $.unblockUI();
    //        }
    //    });
    //})

    //$('#btnAddNQMClaimDetails').click(function (e) {

    //    if ($('#lstBank').val() == "")
    //        alert("Please choose bank name.")
    //    else {
    //        if ($("#formShowNQMDetails").valid()) {
    //            if (confirm("Are you sure you want to save tour claim details ?")) {
    //                $.ajax({
    //                    url: "/TourClaim/InsertTourClaimDetails",
    //                    type: "POST",
    //                    async: false,
    //                    cache: false,
    //                    data: $("#formShowNQMDetails").serialize(),
    //                    success: function (response) {
    //                        alert(response.message);
    //                        if (response.success) {
    //                            $('#btnTourClaimReset').trigger('click');
    //                            $("#formShowNQMDetails").hide();
    //                            $("#accordionMonitorsSchedule").hide();

    //                            $("#divTourClaimList").show();
    //                            LoadTourClaimList($('#scheduleCode').val());

    //                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    //                            $.unblockUI();
    //                            var monthNum = getMonthFromString($('#monthOfSchedule').val());
    //                            showMonitorsAssignScheduleListGrid(monthNum, $('#yearOfSchedule').val());
    //                        }
    //                        else
    //                            $('#btnTourClaimReset').trigger('click');

    //                    },
    //                    error: function (xhr, ajaxOptions, thrownError) {
    //                        alert("error");
    //                        alert(xhr.responseText);
    //                        $.unblockUI();

    //                    }
    //                });
    //            }
    //        }
    //    }



    //})

});

/*$('#lstBank').chosen();*/

//function EditTourDetails(tourClaimId) {
//    EditDetails(tourClaimId);
//}

//$('#btnUpdateNQMClaimDetails').click(function (e) {

//    if ($('#lstBank').val() == "")
//        alert("Please choose bank name.")
//    else {
//        if ($("#formShowNQMDetails").valid()) {
//            if (confirm("Are you sure you want to update tour claim details ?")) {
//                $.ajax({
//                    url: "/TourClaim/UpdateTourClaimDetails",
//                    type: "POST",
//                    async: false,
//                    cache: false,
//                    data: $("#formShowNQMDetails").serialize(),
//                    success: function (response) {
//                        alert(response.message);
//                        if (response.success) {
//                            $('#btnTourClaimReset').trigger('click');
//                            $("#formShowNQMDetails").hide();
//                            $("#accordionMonitorsSchedule").hide();

//                            $("#divTourClaimList").show();
//                            LoadTourClaimList($('#scheduleCode').val());

//                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

//                            $.unblockUI();
//                            var monthNum = getMonthFromString($('#monthOfSchedule').val());
//                            alert(monthNum);
//                            showMonitorsAssignScheduleListGrid(monthNum, $('#yearOfSchedule').val());
//                        }
//                        else
//                            $('#btnTourClaimReset').trigger('click');

//                    },
//                    error: function (xhr, ajaxOptions, thrownError) {
//                        alert("error");
//                        alert(xhr.responseText);
//                        $.unblockUI();

//                    }
//                });
//            }
//        }
//    }



//})

//$('#idCollapseSpanHideTour').click(function (e) {
//    alert("hide click");
//    $("#idCollapseSpanViewTour").show("slow");
//    $("#idCollapseSpanHideTour").hide("slow");
//    $("#formShowNQMDetails").hide("slow");
//});

//function DeleteTourDetails(id) {

//    const myArray = id.split("$");
//    var adminScheduleCode = myArray[0];
//    var tourId = myArray[1];
//    var month = myArray[2];
//    var year = myArray[3];

//    if (confirm("Are you sure to delete details ?")) {
//        $.ajax({
//            url: "/TourClaim/DeleteTourDetail/",
//            type: "POST",
//            async: false,
//            cache: false,
//            data: { tourId: tourId },
//            success: function (response) {
//                alert(response.message);
//                if (response.success) {
//                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
//                    $('#tbTourClaimList').trigger('reloadGrid');
//                    $.unblockUI();
//                    LoadTourClaimList(adminScheduleCode);

//                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
//                    $.unblockUI();
//                    showMonitorsAssignScheduleListGrid(month, year);
//                }

//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                alert("error");
//                alert(xhr.responseText);
//                $.unblockUI();

//            }
//        });
//    }
//    else {
//        return false;
//    }
//}

//function LoadTourClaimList(adminScheduleCode) {

//    jQuery("#tbTourClaimList").jqGrid('GridUnload');
//    jQuery("#tbTourClaimList").jqGrid({
//        url: '/TourClaim/GetTourClaimList',
//        datatype: "json",
//        mtype: "POST",
//        async: false,
//        cache: false,
//        postData: { adminScheduleCode: adminScheduleCode },
//        colNames: ['TourClaimId', 'Date of Claim', 'Tour Reference Number', 'Total District Visit Allowance Amount (Rs)', 'Total Travel Claim Amount (Rs)', 'Total Lodge and Daily Claim Amount (Rs)', 'Total Inspection of Road Claim Amount (Rs)', 'Total Meeting with PIU Claim Amount (Rs)', 'Total Claimed Amount (Rs)', 'Total Sanctioned Amount (Rs)', 'Edit', 'Delete'],
//        colModel: [
//            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
//            { name: 'DATE_OF_CLAIM', index: 'DATE_OF_CLAIM', height: 'auto', width: 80, align: "center", search: false },
//            { name: 'NRRDA_LETTER_NUMBER', index: 'NRRDA_LETTER_NUMBER', width: 100, sortable: true, align: "center" },
//            { name: 'DISTRICT_VISITED_ALLOWANCE', index: 'DISTRICT_VISITED_ALLOWANCE', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_TRAVEL_CLAIM_AMOUNT', index: 'TOTAL_TRAVEL_CLAIM_AMOUNT', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_LODGE_CLAIM_AMOUNT', index: 'TOTAL_LODGE_CLAIM_AMOUNT', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_INSPECTION_CLAIM_AMOUNT', index: 'TOTAL_INSPECTION_CLAIM_AMOUNT', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_MEETING_CLAIM_AMOUNT', index: 'TOTAL_MEETING_CLAIM_AMOUNT', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_CLAIMED', index: 'TOTAL_CLAIMED', width: 80, sortable: true, align: "center" },
//            { name: 'TOTAL_SANCTIONED', index: 'TOTAL_SANCTIONED', width: 80, sortable: true, align: "center" },
//            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 80, align: "center", search: false },
//            { name: 'DELETE', index: 'DELETE', height: 'auto', width: 80, align: "center", search: false },

//        ],
//        pager: jQuery('#pagerTourClaimList').width(20),
//        rowNum: 10,
//        rowList: [10, 20, 30],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: "TOUR_CLAIM_ID",
//        sortorder: "asc",
//        caption: "&nbsp;&nbsp; Tour Claim Details",
//        height: 'auto',
//        autowidth: true,
//        hidegrid: true,
//        rownumbers: true,
//        cmTemplate: { title: false },
//        onSelectRow: function (id) {
//        },
//        loadComplete: function (data) {

//            $("#tbTourClaimList #pagerTourClaimList").css({ height: '40px' });
//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//            }
//        }
//    });
//}


