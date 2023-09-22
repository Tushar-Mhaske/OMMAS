﻿function AddDistrictDetails(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $("#divDistrictDetailsList").show("slow");
    LoadDistrictDetailsListFin2(scheduleCode);
}

function LoadDistrictDetailsListFin2(scheduleCode) {
    
    jQuery("#tbDistrictDetailsList").jqGrid('GridUnload');
    jQuery("#tbDistrictDetailsList").jqGrid({
        url: '/TourClaim/GetTourDistrictListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['TourClaimId', 'District', 'Date From', 'Date To', 'Quick Response Sheet'],
        colModel: [
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 80, sortable: true, align: "center" },
            { name: 'DATE_FROM', index: 'DATE_FROM', width: 80, sortable: true, align: "center" },
            { name: 'DATE_TO', index: 'DATE_TO', height: 'auto', width: 80, align: "center", search: false },
            { name: 'QUICK_RESPONSE_SHEET', index: 'QUICK_RESPONSE_SHEET', height: 'auto', width: 80, align: "center", search: false }
        ],
        pager: jQuery('#pagerDistrictDetailsList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "TOUR_CLAIM_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; District Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbDistrictDetailsList #pagerDistrictDetailsList").css({ height: '40px' });
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
    });
}

// travel

function AddTravelClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";

    $("#divTravelClaimList").show("slow");
    LoadTravelClaimListFin2(scheduleCode);
}

function LoadTravelClaimListFin2(scheduleCode) {

    jQuery("#tbTravelClaimList").jqGrid('GridUnload');
    jQuery("#tbTravelClaimList").jqGrid({
        url: '/TourClaim/GetTravelClaimListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Travel Claim Id', 'Tour Claim Id', 'Schedule Code', 'Start Date Time', 'End Date Time', 'Departure From', 'Arrival At', 'Mode Of Travel', 'Travel Class', 'View Uploaded Ticket', 'View Uploaded Boarding Pass', 'Amount Claimed by NQM', 'Amount Proposed by CQC', 'Remark by CQC', 'Amount Proposed by Finance', 'Remark by Finance', 'Proposed Amount', 'Remark', 'Save Changes'],
        colModel: [
            { name: 'TRAVEL_CLAIM_ID', index: 'TRAVEL_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: false, hidden: true },
            { name: 'START_DATE_OF_TRAVEL', index: 'START_DATE_OF_TRAVEL', height: 'auto', width: 80, align: "center", search: false },
            //{ name: 'START_TIME', index: 'START_TIME', height: 'auto', width: 50, align: "center", search: false },
            //{ name: 'END_TIME', index: 'END_TIME', height: 'auto', width: 50, align: "center", search: false },
            { name: 'END_DATE_OF_TRAVEL', index: 'END_DATE_OF_TRAVEL', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DEPARTURE_FROM', index: 'DEPARTURE_FROM', width: 80, sortable: true, align: "center" },
            { name: 'ARRIVAL_AT', index: 'ARRIVAL_AT', width: 80, sortable: true, align: "center" },
            { name: 'MODE_OF_TRAVEL', index: 'MODE_OF_TRAVEL', width: 80, sortable: true, align: "center" },
            { name: 'TRAVEL_CLASS', index: 'TRAVEL_CLASS', width: 80, sortable: true, align: "center" },
            { name: 'VIEW_UPLOADED_TICKET', index: 'VIEW_UPLOADED_TICKET', height: 'auto', width: 50, align: "center", search: false },
            { name: 'VIEW_UPLOADED_BOARDING_PASS', index: 'VIEW_UPLOADED_BOARDING_PASS', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_CQC', index: 'AMOUNT_PASSED_CQC', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_CQC', index: 'REMARK_CQC', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_FIN1', index: 'AMOUNT_PASSED_FIN1', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_FIN1', index: 'REMARK_FIN1', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 100, align: "center", search: false, edittype: "text", formatter: generateTextBoxForAmtTravel },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 180, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemarkTravel },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtnTravel }
        ],
        pager: jQuery('#pagerTravelClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "TRAVEL_CLAIM_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Travel Claim Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
        },
        loadComplete: function (data) {

            $("#tbTravelClaimList #pagerTravelClaimList").css({ height: '40px' });
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
    });
}

function generateTextBoxForAmtTravel(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var travelAmt = myArray[0];
    var travelFlag = myArray[1];
    var roundSeq = myArray[2];

    if (travelFlag == 3)
        return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\"  value=\"" + travelAmt + "\" id=\"traveltextboxAmt" + rowObject[0] + "\"/>";
    else
        return travelAmt;

    //if (travelFlag >= '4')
    //    return travelAmt;
    //else if (roundSeq == '1')
    //    return travelAmt;
    //else
    //    return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\"  value=\"" + travelAmt + "\" id=\"traveltextboxAmt" + rowObject[0] + "\"/>";
}

function generateTextBoxForRemarkTravel(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var travelRem = myArray[0];
    var travelFlag = myArray[1];
    var roundSeq = myArray[2];

    if (travelFlag == 3)
        return "<textarea size=\"10\" maxlength=\"50\" id=\"traveltextboxRem" + rowObject[0] + "\">" + travelRem + "</textarea>";
    else {
        if (travelRem == "") {
            return "--";
        }
        else
            return travelRem;
    }

    //if (travelFlag >= '4') {
    //    if (travelRem == "") {
    //        return "--";
    //    }
    //    else
    //        return travelRem;
    //}
    //else if (roundSeq == '1') {
    //    if (travelRem == "") {
    //        return "--";
    //    }
    //    else
    //        return travelRem;
    //}
    //else
    //    return "<textarea size=\"10\" maxlength=\"50\" id=\"traveltextboxRem" + rowObject[0] + "\">" + travelRem + "</textarea>";
}

function generateCheckoutBtnTravel(cellvalue, options, rowObject) {
    
    myArray = cellvalue.split("$");
    var travelflag = myArray[0];
    var travelAmtPass = myArray[1];
    var roundSeq = myArray[2];

    if (travelflag == 3)
        return '<button style="width:55%" onclick="return getTextBoxValueTravelEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    else
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    
    //if (travelflag >= '4')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (roundSeq == '1')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (travelAmtPass != "") 
    //    return '<button style="width:55%" onclick="return getTextBoxValueTravelEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
}

function getTextBoxValueTravelEdit(SelectedId, scheduleCode) {

    var txtAmt = $("#traveltextboxAmt" + SelectedId).val();
    var txtRem = $("#traveltextboxRem" + SelectedId).val();

    if (txtAmt == "") {
        alert("Please Enter amount.");
    }
    else {
        if (confirm("Do you want to update changes ?")) {
            $.ajax({
                url: "/TourClaim/AddSanctionAmtTravelFin2",
                type: "POST",
                async: false,
                cache: false,
                data: { travelId: SelectedId, amount: txtAmt, remark: txtRem },
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                       
                        $("#divTravelClaimList").show("slow");
                        LoadTravelClaimListFin2(scheduleCode);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }
}

// Lodge

function AddLodgeClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $("#divLodgeClaimList").show("slow");
    LoadLodgeClaimListFin2(scheduleCode);
}

function LoadLodgeClaimListFin2(scheduleCode) {

    jQuery("#tbLodgeClaimList").jqGrid('GridUnload');
    jQuery("#tbLodgeClaimList").jqGrid({
        url: '/TourClaim/GetLodgeClaimListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Lodge Claim Id', 'Tour Claim Id', 'Schedule Code', 'Date From', 'Date To', 'Type of Claim', 'Hotel Name', 'View Uploaded Bill', 'View Uploaded e-Receipt', 'Daily', 'Hotel', 'Amount Proposed Daily', 'Hotel Amount Proposed by CQC', 'Remark by CQC', 'Hotel Amount Proposed by Finance', 'Remark by Finance', 'Hotel Amount Proposed', 'Remark', 'Save Changes'],
        colModel: [
            { name: 'LODGE_CLAIM_ID', index: 'LODGE_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_FROM', index: 'DATE_FROM', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DATE_TO', index: 'DATE_TO', width: 80, sortable: true, align: "center" },
            { name: 'TYPE_OF_CLAIM', index: 'TYPE_OF_CLAIM', width: 80, sortable: true, align: "center" },
            { name: 'HOTEL_NAME', index: 'HOTEL_NAME', width: 80, sortable: true, align: "center" },
            { name: 'VIEW_UPLOADED_BILL', index: 'VIEW_UPLOADED_BILL', height: 'auto', width: 50, align: "center", search: false },
            { name: 'VIEW_UPLOADED_RECEIPT', index: 'VIEW_UPLOADED_RECEIPT', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED_DAILY', index: 'AMOUNT_CLAIMED_DAILY', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED_HOTEL', index: 'AMOUNT_CLAIMED_HOTEL', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_PASSED_DAILY', index: 'AMOUNT_PASSED_DAILY', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_HOTEL_CQC', index: 'AMOUNT_PASSED_HOTEL_CQC', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_CQC', index: 'REMARK_CQC', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_HOLET_FIN1', index: 'AMOUNT_PASSED_HOTEL_FIN1', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_FIN1', index: 'REMARK_FIN1', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_HOTEL', index: 'AMOUNT_PASSED_HOTEL', height: 'auto', width: 80, align: "center", search: false, edittype: "text", formatter: generateTextBoxForAmtLodge },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 180, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemarkLodge },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtnLodge }
        ],
        pager: jQuery('#pagerLodgeClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "LODGE_CLAIM_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Lodge Claim and Daily Allowance Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbLodgeClaimList #pagerLodgeClaimList").css({ height: '40px' });
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
    });

    jQuery("#tbLodgeClaimList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'AMOUNT_CLAIMED_DAILY', numberOfColumns: 2, titleText: '<center>Amount Claimed</center>' },
            //{ startColumnName: 'AMOUNT_PASSED_DAILY_CQC', numberOfColumns: 2, titleText: '<center>Amount Passed by CQC</center>' },
            //{ startColumnName: 'AMOUNT_PASSED_DAILY_FIN1', numberOfColumns: 2, titleText: '<center>Amount Passed by Finance</center>' },
            //{ startColumnName: 'AMOUNT_PASSED_DAILY', numberOfColumns: 2, titleText: '<center>Amount Passed</center>' },
        ]
    });
}

function generateTextBoxForAmtLodge(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var lodgeAmt = myArray[0];
    var lodgeFlag = myArray[1];
    var roundSeq = myArray[2];

    if (lodgeAmt == '--')
        return lodgeAmt;

    if (lodgeFlag == 3)
        return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\"  value=\"" + lodgeAmt + "\" id=\"lodgetextboxAmt" + rowObject[0] + "\"/>";
    else
        return lodgeAmt;

    //if (lodgeFlag >= '4')
    //    return lodgeAmt;
    //else if (roundSeq == '1')
    //    return lodgeAmt;
    //else
    //    return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\"  value=\"" + lodgeAmt + "\" id=\"lodgetextboxAmt" + rowObject[0] + "\"/>";
}

function generateTextBoxForRemarkLodge(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var lodgeRem = myArray[0];
    var lodgeFlag = myArray[1];
    var roundSeq = myArray[2];
    var type = myArray[3];

    if (lodgeFlag == 3) {
        if (type == "S")
            return "<textarea disabled style = \"border:none;resize: none;text-align:center;height:100%\" size=\"10\" maxlength=\"50\" id=\"lodgetextboxRem" + rowObject[0] + "\">" + "--" + "</textarea>";
        else
            return "<textarea size=\"10\" maxlength=\"50\" id=\"lodgetextboxRem" + rowObject[0] + "\">" + lodgeRem + "</textarea>";
    }
    else {
        if (lodgeRem == "") {
            return "--";
        }
        else
            return lodgeRem;
    }

    //if (lodgeFlag >= '4') {
    //    if (lodgeRem == "") {
    //        return "--";
    //    }
    //    else
    //        return lodgeRem;
    //}
    //else if (roundSeq == '1') {
    //    if (lodgeRem == "") {
    //        return "--";
    //    }
    //    else
    //        return lodgeRem;
    //}
    //else {
    //    if (type == "S")
    //        return "<textarea disabled style = \"border:none;resize: none;text-align:center;height:100%\" size=\"10\" maxlength=\"50\" id=\"lodgetextboxRem" + rowObject[0] + "\">" + "--" + "</textarea>";
    //    else
    //        return "<textarea size=\"10\" maxlength=\"50\" id=\"lodgetextboxRem" + rowObject[0] + "\">" + lodgeRem + "</textarea>";
    //}
}

function generateCheckoutBtnLodge(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var finalizeFlag = myArray[0];
    var type = myArray[1];
    var editType = myArray[2];
    var roundSeq = myArray[3];

    if (finalizeFlag == 3) {
        if (type == "S")
            return "--";
        else
            return '<button style="width:60%" onclick="return getTextBoxValueLodgeEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    }
    else
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    
    //if (finalizeFlag >= '4')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (roundSeq == '1')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (type == "S")
    //    return "--";
    //else {
    //    if (type == "S")
    //        return "--";
    //    else
    //        return '<button style="width:60%" onclick="return getTextBoxValueLodgeEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    //}
}

function getTextBoxValueLodgeEdit(SelectedId, scheduleCode) {

    var txtAmt = $("#lodgetextboxAmt" + SelectedId).val();
    var txtRem = $("#lodgetextboxRem" + SelectedId).val();

    if (txtAmt == undefined) {
        txtAmt = 0.00;
    }
    if (confirm("Do you want to update changes ?")) {
        $.ajax({
            url: "/TourClaim/AddSanctionAmtLodgeFin2",
            type: "POST",
            async: false,
            cache: false,
            data: { lodgeId: SelectedId, amount: txtAmt, remark: txtRem },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                   
                    $("#divLodgeClaimList").show("slow");
                    LoadLodgeClaimListFin2(scheduleCode);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }

}

// Inspection

function PerformInspectionOfRoad(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $('#divInspectionOfRoadsHonorarium').show('slow');
    LoadInspectionHonorariumListFin2(scheduleCode);
}

function LoadInspectionHonorariumListFin2(scheduleCode) {

    jQuery("#tbInspectionOfRoadsHonorarium").jqGrid('GridUnload');
    jQuery("#tbInspectionOfRoadsHonorarium").jqGrid({
        url: '/TourClaim/GetInspectionHonorariumListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['HONORARIUM_INSPECTION_ID', 'TourClaimId', 'Schedule Code', 'Date of Inspection', 'Work Type', 'Amount Claimed', 'Amount Proposed by CQC', 'Remark by CQC', 'Amount Proposed by Finance', 'Remark by Finance', 'Amount Proposed', 'Remark', 'Save Changes'],
        colModel: [
            { name: 'HONORARIUM_INSPECTION_ID', index: 'HONORARIUM_INSPECTION_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_OF_INSPECTION', index: 'DATE_OF_INSPECTION', width: 80, sortable: true, align: "center" },
            { name: 'TYPE_OF_WORK', index: 'TYPE_OF_WORK', width: 80, sortable: true, align: "center" },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_CQC', index: 'AMOUNT_PASSED_CQC', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_CQC', index: 'REMARK_CQC', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_FIN1', index: 'AMOUNT_PASSED_FIN1', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_FIN1', index: 'REMARK_FIN1', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false, edittype: "text", formatter: generateTextBoxForAmtInsp },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 120, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemarkInsp },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtnInsp }
        ],
        pager: jQuery('#pagerInspectionOfRoadsHonorarium').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "HONORARIUM_INSPECTION_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Honorarium for Inspection Of Roads Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbInspectionOfRoadsHonorarium #pagerInspectionOfRoadsHonorarium").css({ height: '40px' });
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
    });
}

function generateTextBoxForAmtInsp(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var inspAmt = myArray[0];
    var inspFlag = myArray[1];
    var roundSeq = myArray[2];

    if (inspFlag == 3)
        return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + inspAmt + "\" id=\"insptextboxAmt" + rowObject[0] + "\"/>";
    else
        return inspAmt;

    //if (inspFlag >= '4')
    //    return inspAmt;
    //else if (roundSeq == '1')
    //    return inspAmt;
    //else
    //    return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + inspAmt + "\" id=\"insptextboxAmt" + rowObject[0] + "\"/>";
}

function generateTextBoxForRemarkInsp(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var inspRem = myArray[0];
    var inspFlag = myArray[1];
    var roundseq = myArray[2];

    if (inspFlag == 3)
        return "<textarea size=\"10\" maxlength=\"50\" id=\"insptextboxRem" + rowObject[0] + "\">" + inspRem + "</textarea>";
    else {
        if (inspRem == "") {
            return "--";
        }
        else
            return inspRem;
    }

    //if (inspFlag >= '4') {
    //    if (inspRem == "") {
    //        return "--";
    //    }
    //    else
    //        return inspRem;
    //}
    //else if (roundseq == '1') {
    //    if (inspRem == "") {
    //        return "--";
    //    }
    //    else
    //        return inspRem;
    //}
    //else
    //    return "<textarea size=\"10\" maxlength=\"50\" id=\"insptextboxRem" + rowObject[0] + "\">" + inspRem + "</textarea>";
}

function generateCheckoutBtnInsp(cellvalue, options, rowObject) {
    
    myArray = cellvalue.split("$");
    var finalizeFlag = myArray[0];
    var type = myArray[1];
    var roundSeq = myArray[2];

    if (finalizeFlag == 3)
        return '<button style="width:28%" onclick="return getTextBoxValueInspEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    else
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";

    //if (finalizeFlag >= '4')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (roundSeq == '1')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else
    //    return '<button style="width:28%" onclick="return getTextBoxValueInspEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
}

function getTextBoxValueInspEdit(SelectedId, scheduleCode) {

    var txtAmt = $("#insptextboxAmt" + SelectedId).val();
    var txtRem = $("#insptextboxRem" + SelectedId).val();

    if (txtAmt == "") {
        alert("Please Enter amount.");
    }
    else {
        if (confirm("Do you want to update changes ?")) {
            $.ajax({
                url: "/TourClaim/AddSanctionAmtInspFin2",
                type: "POST",
                async: false,
                cache: false,
                data: { inspId: SelectedId, amount: txtAmt, remark: txtRem },
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        
                        $('#divInspectionOfRoadsHonorarium').show('slow');
                        LoadInspectionHonorariumListFin2(scheduleCode);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }

    }
}

// Meeting

function MeetingWithPIU(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active"

    $('#divMeetingHonorarium').show('slow');
    LoadMeetingHonorariumListFin2(scheduleCode);
}

function LoadMeetingHonorariumListFin2(scheduleCode) {

    jQuery("#tbMeetingHonorarium").jqGrid('GridUnload');
    jQuery("#tbMeetingHonorarium").jqGrid({
        url: '/TourClaim/GetMeetingHonorariumListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['HONORARIUM_MEETING_ID', 'TourClaimId', 'Schedule Code', 'Date of Meeting', 'Place of Meeting', 'View Uploaded attendance sheet', 'View Uploaded meeting details', 'View Uploaded Geotagged Photograph', 'Amount Claimed', 'Amount Proposed by CQC', 'Remark by CQC', 'Amount Proposed by Finance', 'Remark by Finance', 'Amount Proposed', 'Remark', 'Save Changes'],
        colModel: [
            { name: 'HONORARIUM_MEETING_ID', index: 'HONORARIUM_MEETING_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE_OF_MEETING', index: 'DATE_OF_MEETING', width: 80, sortable: true, align: "center" },
            { name: 'PLACE', index: 'PLACE', width: 80, sortable: true, align: "center" },
            { name: 'VIEW_UPLOADED_ATTENDANCE_SHEET', index: 'VIEW_UPLOADED_ATTENDANCE_SHEET', height: 'auto', width: 50, align: "center", search: false },
            { name: 'VIEW_UPLOADED_MEETING_DETAILS', index: 'VIEW_UPLOADED_MEETING_DETAILS', height: 'auto', width: 50, align: "center", search: false },
            { name: 'VIEW_UPLOADED_PHOTO', index: 'VIEW_UPLOADED_PHOTO', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_CQC', index: 'AMOUNT_PASSED_CQC', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_CQC', index: 'REMARK_CQC', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_FIN1', index: 'AMOUNT_PASSED_FIN1', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_FIN1', index: 'REMARK_FIN1', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false, edittype: "text", formatter: generateTextBoxForAmtMeeting },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 130, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemarkMeeting },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtnMeeting }
        ],
        pager: jQuery('#pagerMeetingHonorarium').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "HONORARIUM_MEETING_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Honorarium for Meeting with PIU",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbMeetingHonorarium #pagerMeetingHonorarium").css({ height: '40px' });
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
    });
}

function generateTextBoxForAmtMeeting(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var meetingAmt = myArray[0];
    var meetingFlag = myArray[1];
    var roundSeq = myArray[2];

    if (meetingFlag == 3)
        return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + meetingAmt + "\" id=\"meetingtextboxAmt" + rowObject[0] + "\"/>";
    else
        return meetingAmt;

    //if (meetingFlag >= '4')
    //    return meetingAmt;
    //else if (roundSeq == '1')
    //    return meetingAmt;
    //else
    //    return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + meetingAmt + "\" id=\"meetingtextboxAmt" + rowObject[0] + "\"/>";
}

function generateTextBoxForRemarkMeeting(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var meetingRem = myArray[0];
    var meetingFlag = myArray[1];
    var roundSeq = myArray[2];

    if (meetingFlag == 3)
        return "<textarea size=\"10\" maxlength=\"50\" id=\"meetingtextboxRem" + rowObject[0] + "\">" + meetingRem + "</textarea>";
    else {
        if (meetingRem == "") {
            return "--";
        }
        else
            return meetingRem;
    }

    //if (meetingFlag >= '4') {
    //    if (meetingRem == "") {
    //        return "--";
    //    }
    //    else
    //        return meetingRem;
    //}
    //else if (roundSeq == '1') {
    //    if (meetingRem == "") {
    //        return "--";
    //    }
    //    else
    //        return meetingRem;
    //}
    //else
    //    return "<textarea size=\"10\" maxlength=\"50\" id=\"meetingtextboxRem" + rowObject[0] + "\">" + meetingRem + "</textarea>";
}

function generateCheckoutBtnMeeting(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var finalizeFlag = myArray[0];
    var type = myArray[1];
    var roundSeq = myArray[2];

    if (finalizeFlag == 3)
        return '<button style="width:31%" onclick="return getTextBoxValueMeetingEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    else
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";

    //if (finalizeFlag >= '4')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (roundSeq == '1')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else 
    //    return '<button style="width:31%" onclick="return getTextBoxValueMeetingEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
}

function getTextBoxValueMeetingEdit(SelectedId, scheduleCode) {

    var txtAmt = $("#meetingtextboxAmt" + SelectedId).val();
    var txtRem = $("#meetingtextboxRem" + SelectedId).val();

    if (txtAmt == "") {
        alert("Please Enter amount.");
    }
    else {
        if (confirm("Do you want to update changes ?")) {
            $.ajax({
                url: "/TourClaim/AddSanctionAmtMeetingFin2",
                type: "POST",
                async: false,
                cache: false,
                data: { meetingId: SelectedId, amount: txtAmt, remark: txtRem },
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                       
                        $('#divMeetingHonorarium').show('slow');
                        LoadMeetingHonorariumListFin2(scheduleCode);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }

    }
}

// Miscellaneous

function AddMiscellaneousClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";

    $('#divMiscellaneousClaimDetails').show('slow');
    LoadMiscellaneousClaimListFin2(scheduleCode);
}

function LoadMiscellaneousClaimListFin2(scheduleCode) {

    jQuery("#tbMiscellaneousClaimList").jqGrid('GridUnload');
    jQuery("#tbMiscellaneousClaimList").jqGrid({
        url: '/TourClaim/LoadMiscellaneousClaimListFin2',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Mis Id', 'Tour Claim Id', 'Schedule Code', 'Date', 'Description', 'View Uploaded File', 'Amount Claimed', 'Amount Proposed by CQC', 'Remark by CQC', 'Amount Proposed by Finance', 'Remark by Finance', 'Amount Proposed', 'Remark', 'Save Changes'],
        colModel: [
            { name: 'MISCELLANEOUS_ID', index: 'MISCELLANEOUS_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE', index: 'DATE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DESCRIPTION', index: 'DESCRIPTION', height: 'auto', width: 100, align: "center", search: false },
            { name: 'VIEW_UPLOADED_FILE', index: 'VIEW_UPLOADED_FILE', height: 'auto', width: 50, align: "center", search: false },
            { name: 'AMOUNT_CLAIMED', index: 'AMOUNT_CLAIMED', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_CQC', index: 'AMOUNT_PASSED_CQC', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_CQC', index: 'REMARK_CQC', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED_FIN1', index: 'AMOUNT_PASSED_FIN1', height: 'auto', width: 50, align: "center", search: false },
            { name: 'REMARK_FIN1', index: 'REMARK_FIN1', height: 'auto', width: 80, align: "center", search: false },
            { name: 'AMOUNT_PASSED', index: 'AMOUNT_PASSED', height: 'auto', width: 80, align: "center", search: false, edittype: "text", formatter: generateTextBoxForAmtMiscellaneous },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 130, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemarkMiscellaneous },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtnMiscellaneous }
        ],
        pager: jQuery('#pagerMiscellaneousClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "MISCELLANEOUS_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Miscellaneous Claim Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbMiscellaneousClaimList #pagerMiscellaneousClaimList").css({ height: '40px' });
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
    });
}

function generateTextBoxForAmtMiscellaneous(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var miscellaneousAmt = myArray[0];
    var miscellaneousFlag = myArray[1];
    var roundSeq = myArray[2];

    if (miscellaneousFlag == 3)
        return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + miscellaneousAmt + "\" id=\"miscellaneoustextboxAmt" + rowObject[0] + "\"/>";
    else
        return miscellaneousAmt;

    //if (miscellaneousFlag >= '4')
    //    return miscellaneousAmt;
    //else if (roundSeq == '1')
    //    return miscellaneousAmt;
    //else
    //    return "<input type=\"text\" size=\"10\" maxlength=\"15\" oninput=\"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\[^0-9].[^0-9])\./g, '$1');\" value=\"" + miscellaneousAmt + "\" id=\"miscellaneoustextboxAmt" + rowObject[0] + "\"/>";
}

function generateTextBoxForRemarkMiscellaneous(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var miscellaneousRem = myArray[0];
    var miscellaneousFlag = myArray[1];
    var roundSeq = myArray[2];

    if (miscellaneousFlag == 3)
        return "<textarea size=\"10\" maxlength=\"50\" id=\"miscellaneoustextboxRem" + rowObject[0] + "\">" + miscellaneousRem + "</textarea>";
    else {
        if (miscellaneousRem == "") {
            return "--";
        }
        else
            return miscellaneousRem;
    }

    //if (miscellaneousFlag >= '4') {
    //    if (miscellaneousRem == "") {
    //        return "--";
    //    }
    //    else
    //        return miscellaneousRem;
    //}
    //else if (roundSeq == '1') {
    //    if (miscellaneousRem == "") {
    //        return "--";
    //    }
    //    else
    //        return miscellaneousRem;
    //}
    //else
    //    return "<textarea size=\"10\" maxlength=\"50\" id=\"miscellaneoustextboxRem" + rowObject[0] + "\">" + miscellaneousRem + "</textarea>";
}

function generateCheckoutBtnMiscellaneous(cellvalue, options, rowObject) {

    myArray = cellvalue.split("$");
    var finalizeFlag = myArray[0];
    var type = myArray[1];
    var roundSeq = myArray[2];

    if (finalizeFlag == 3)
        return '<button style="width:31%" onclick="return getTextBoxValueMiscellaneousEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
    else
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";

    //if (finalizeFlag >= '4')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else if (roundSeq == '1')
    //    return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    //else
    //    return '<button style="width:31%" onclick="return getTextBoxValueMiscellaneousEdit(' + rowObject[0] + ',' + rowObject[2] + ')">Edit</button>';
}

function getTextBoxValueMiscellaneousEdit(SelectedId, scheduleCode) {

    var txtAmt = $("#miscellaneoustextboxAmt" + SelectedId).val();
    var txtRem = $("#miscellaneoustextboxRem" + SelectedId).val();

    if (txtAmt == "") {
        alert("Please Enter amount.");
    }
    else {
        if (confirm("Do you want to update changes ?")) {
            $.ajax({
                url: "/TourClaim/AddSanctionAmtMiscellaneousFin2",
                type: "POST",
                async: false,
                cache: false,
                data: { miscellaneousId: SelectedId, amount: txtAmt, remark: txtRem },
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                       
                        $('#divMiscellaneousClaimDetails').show('slow');
                        LoadMiscellaneousClaimList2(scheduleCode);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    }
}

// Permissions
function AddPermissionClaim(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";

    $('#divPermissionClaimDetails').show('slow');
    LoadPermissionClaimListCqc(scheduleCode);
}

function LoadPermissionClaimListCqc(scheduleCode) {

    jQuery("#tbPermissionClaimList").jqGrid('GridUnload');
    jQuery("#tbPermissionClaimList").jqGrid({
        url: '/TourClaim/LoadPermissionClaimListCqc',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { scheduleCode: scheduleCode },
        colNames: ['Per Id', 'Tour Claim Id', 'Schedule Code', 'Date', 'Description', 'View Uploaded File'],
        colModel: [
            { name: 'MISCELLANEOUS_ID', index: 'MISCELLANEOUS_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'TOUR_CLAIM_ID', index: 'TOUR_CLAIM_ID', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', height: 'auto', width: 250, align: "left", search: true, hidden: true },
            { name: 'DATE', index: 'DATE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'DESCRIPTION', index: 'DESCRIPTION', height: 'auto', width: 100, align: "center", search: false },
            { name: 'VIEW_UPLOADED_FILE', index: 'VIEW_UPLOADED_FILE', height: 'auto', width: 80, align: "center", search: false },
        ],
        pager: jQuery('#pagerPermissionClaimList').width(20),
        rowNum: 5,
        rowList: [5, 10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PERMISSION_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Permissions Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) { },
        loadComplete: function (data) {
            $("#tbPermissionClaimList #pagerPermissionClaimList").css({ height: '40px' });
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
    });
}

// Approve or Forwarded to CQC

function AddApproveRevertToCQC(evt, scheduleCode, showOption) {

    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(showOption).style.display = "block";
    evt.currentTarget.className += " active";
    
    $.ajax({
        url: "/TourClaim/ApproveForwardFilter/?scheduleCode=" + scheduleCode,
        async: false,
        cache: false,
        success: function (data) {
            // original 2 lines commented
            //$('#divapproveForwardDetails').show('slow');
            //$('#divapproveForwardDetails').html(data);

            // Added new 
            $('.tab').hide('slow');
            $('#showMainForm').hide('slow');
            $('#previewDiv').show('slow');
            $('#divViewPreviewDetails').html(data);
            $('#divViewPreviewDetails').show('slow');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
            alert(xhr.responseText);
            $.unblockUI();

        }
    });
}

$('#btnApproveFin2').click(function () {

    var assistantDirector = $('#assistantDirector').val();

    if (assistantDirector == "")
        alert("Please enter Assistant Director name.");
    else if (confirm("Do you want to Approve ?")) {
       
        $.ajax({
            url: "/TourClaim/ApproveByFinance2",
            type: "POST",
            async: false,
            cache: false,
            data: $("#formApproveForwardFilter").serialize(),
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $('#showMainForm').hide();
                    $('.tab').hide();
                    $('#accordionDetails2').hide();

                    $('#divFinance2Filters').show();
                    $('#divFinance2Monitor').show('slow');
                    showNQMScheduleListGrid(response.month, response.year);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
});

$('#btnRevertToCQC').click(function () {

    var assistantDirector = $('#assistantDirector').val();
    var remark = $('#revertremark').val();

    if (assistantDirector == "")
        alert("Please enter Assistant Director name.");
    else if (remark == "")
        alert("Please enter remark.");
    else if (confirm("Do you want to revert ?")) {

        $.ajax({
            url: "/TourClaim/RevertToCQC",
            type: "POST",
            async: false,
            cache: false,
            data: $("#formApproveForwardFilter").serialize(),
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $('#showMainForm').hide();
                    $('.tab').hide();
                    $('#accordionDetails2').hide();

                    $('#divFinance2Filters').show();
                    $('#divFinance2Monitor').show('slow');
                    showNQMScheduleListGrid(response.month, response.year);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else {
        return false;
    }
});

// Common

function OpenNqmLetter() {
    var scheduleCode = $('#scheduleCodeFin2').val();
    window.open('/QualityMonitoring/DownloadLetter?' + $.param({ id: scheduleCode, isLettterId: false, userType: 'I' }), '_blank');
}

function ViewQuickResponseSheet(id) {
    window.open('/TourClaim/ViewQuickResponseSheet?' + $.param({ districtDetailsId: id }), '_blank');
}

