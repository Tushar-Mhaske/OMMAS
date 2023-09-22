

function userAndScheduleDetails() {


    //----------------------------Monitor details------------------------------//

    //#region Monitor Details

    $("#userAndScheduleGrids").show();
    $("#observationAndImageGrids").hide();
    $("#monitorGrid").show();
    $("#scheduleDetailsGrid").hide();
    $("#logDetailsGrid").hide();

    $("#message").hide();
    $("#message2").hide();

    $('#tableMonitorDetails').jqGrid('GridUnload');

    jQuery("#tableMonitorDetails").jqGrid('setGridState', 'visible');
    jQuery("#tableMonitorDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListUsers/", page: 1, postData: { yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() } }).trigger("reloadGrid");

    $grid1 = $("#tableMonitorDetails");

    var counter = 0;
    var counter2 = 0;
    var counter3 = 0;
    var hiddenState = true;
    if ($("#qmTypeList").val() == "I") {
        hiddenState = true;
    }
  

    var mygrid = $("#tableMonitorDetails").jqGrid({
    
        url: '/QualityMonitoringHelpDesk/ListUsers/',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Admin Code', 'Monitor Name','User Name','State', 'IMEI Number', 'Log Details', 'Schedule Details', 'Reset IMEI'],
        colModel: [
                    { name: 'ADMIN_QM_CODE', index: 'ADMIN_QM_CODE', width: 150, align: 'left', hidden: true },
                    { name: 'Monitor_NAME', index: 'Monitor_NAME', width: 250, align: 'left', sortable: true, search: true},
                    { name: 'USER_NAME', index: 'USER_NAME', width: 200, align: 'left', sortable: true, search:true },
                    { name: 'State_NAME', index: 'State_NAME', width: 150, align: 'left', sortable: true, hidden: hiddenState, search: true },
                    { name: 'ImeiNo', index: 'ImeiNo', width: 200, align: 'left', search: false, sortable: false },
                    { name: 'ViewLog', index: 'ViewLog', widht: 50, align: 'center', search: false, sortable: false },
                    { name: 'ViewSchedule', index: 'ViewSchedule', widht: 50, align: 'center', search: false, sortable: false },
                    { name: 'resetIMEI', index: 'resetIMEI', widht: 30, align: 'center', search: false, hidden: true, sortable: false },
        ],
     
        pager: jQuery('#navGridMonitorDetails'),        
        rowNum: 2147483647,      
        pgtext: "Page {0} of {1}",      
        rownumbers: true,
        recordtext: '{2} records found',
        height: '180',
        autowidth: false,
        autowidth: false,
        shrinkToFit: false,
        width: 1190,
        sortname: 'ADMIN_QM_CODE',
        sortorder: 'asc',
        viewrecords: true,
        hidegrid: true,     
        caption: 'Schedule Details',
        emptyrecords: 'No Records Found',   
        loadComplete: function (data) {
         
            $('#tableMonitorDetails_rn').html('Sr.<br/>No.');
            $("#gs_Monitor_NAME").attr('placeholder', 'Search here...');
            $("#gs_USER_NAME").attr('placeholder', 'Search here...');
            $("#gs_State_NAME").attr('placeholder', 'Search here...');

            //************For Set JQ Grid Width on Hide show grid column  ************//
            var myGrid = $("#tableMonitorDetails"),
             width = myGrid.jqGrid('getGridParam', 'width'); // get current width
            myGrid.jqGrid('setGridWidth', width, true);

            //************END Set Width ***********/
        },
        postData: { yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() }



    });
    $("#search").click(function () {

        $("#message").hide();
        $("#userAndScheduleGrids").show();
        jQuery("#tableMonitorDetails").jqGrid('setGridState', 'visible');
        jQuery("#tableMonitorDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListUsers/", page: 1, postData: { yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() } }).trigger("reloadGrid");
        $("#scheduleDetailsGrid").hide();
        $("#logDetailsGrid").hide();
        if ($("#qmTypeList").val() == "I") {
          
            jQuery("#tableMonitorDetails").jqGrid('hideCol', ["State_NAME"]);
        }
        else {
          
            jQuery("#tableMonitorDetails").jqGrid('showCol', ["State_NAME"]);
        }

    });

    $grid1.jqGrid('filterToolbar', {
        stringResult: true,
        searchOnEnter: false,
        defaultSearch: "cn",
        ignoreCase: true,

    });

    //$grid1.jqGrid('navGrid', '#navGridMonitorDetails', { add: false, edit: false, del: false, search: false, refresh: false });

    //#endregion

    //---------------------Shedule Details function to check whether isScheduleDownload is 'Y' or 'N'---------------------//

    //#region Shedule Details
    window.showScheduleDetails = function (id) {


        $.ajax({
            url: "/QualityMonitoringHelpDesk/GetIsScheduleDownload/",
            data: { selectRowId: id, yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() },
            cache: false,
            type: 'POST',
            success: function (result) {
                if (result == 'N') {
                    //alert(result);
                    showScheduleDetails3(id);

                }
                else {

                    //alert(result + "in else");
                    showScheduleDetails3(id);
                }
            }
        });


    }
    //#endregion


    //----------------------------Schedule details with isScheduleDownload value as 'Y'------------------------------//

    //#region Schedule Details if isScheduleDownload is 'Y'
    window.showScheduleDetails2 = function (id) {

        jQuery("#tableScheduleDetails").jqGrid('GridUnload');

        jQuery("#tableMonitorDetails").jqGrid('setGridState', 'hidden');

        //alert("1");

        //$("#fourthGrid").hide();
        $("#logDetailsGrid").hide();
        $("#scheduleDetailsGrid").show();

        $grid2 = $("#tableScheduleDetails");
        var mygrid2 = jQuery("#tableScheduleDetails").jqGrid({


            url: "/QualityMonitoringHelpDesk/ListScheduleResult/",
            datatype: 'json',
            mtype: 'POST',
            postData: { selectRowId: function () { return id }, yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() },
            colNames: ['Schedule Code', 'State Name', 'District Name', 'Road Name', 'Device Flag', 'Finalize Flag', 'Inspection Status Flag', 'Schedule Assign Key', 'Is Enquiry',
                        'Schedule Download', 'Total Image Count', 'CQC Forward Flag'],
            colModel: [

                        { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', width: 100, align: 'left', hidden: true },
                        { name: 'STATE_NAME', index: 'STATE_NAME', width: 100, align: 'center' },
                        { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', width: 100, align: 'center' },
                        { name: 'ROAD_NAME', index: 'ROAD_NAME', width: 250, align: 'center' },
                        { name: 'DEVICE_FLAG', index: 'DEVICE_FLAG', width: 80, align: 'center', search: false },
                        { name: 'FINALIZE_FLAG', index: 'FINALIZE_FLAG', width: 80, align: 'center', search: false },
                        { name: 'INSP_STATUS_FLAG', index: 'INSP_STATUS_FLAG', width: 80, align: 'center', search: false },
                        { name: 'SCHEDULE_ASSIGNED', index: 'SCHEDULE_ASSIGNED', width: 80, align: 'center', search: false },
                        { name: 'ADMIN_IS_ENQUIRY', index: 'ADMIN_IS_ENQUIRY', width: 80, align: 'center', search: false },
                        { name: 'IS_SCHEDULE_DOWNLOAD', index: 'IS_SCHEDULE_DOWNLOAD', width: 80, align: 'center', search: false },
                        { name: 'TOTAL_IMAGE_COUNT', index: 'TOTAL_IMAGE_COUNT', width: 80, align: 'center', search: false },
                        { name: 'CQC_FORWARD_FLAG', index: 'CQC_FORWARD_FLAG', width: 80, align: 'center', search: false },

            ],
            index: 'ADMIN_SCHEDULE_CODE',
            pager: jQuery('#navGridScheduleDetails'),
            rowNum: 5,
            rowList: [5, 10, 15, 20],
            cache: false,
            rownumbers: true,
            recordtext: '{2} records found',
            //shrinkToFit: false,
            autowidth: true,
            height: 'auto',
            sortname: 'ADMIN_SCHEDULE_CODE',
            sortorder: 'asc',
            viewrecords: true,
            autowidth: true,
            hidegrid: false,
            caption: 'Schedule Details',
            loadComplete: function () {

                $('#tableScheduleDetails_rn').html('Sr.<br/>No.');
                $("#gs_STATE_NAME").attr('placeholder', 'Search here...');
                $("#gs_DISTRICT_NAME").attr('placeholder', 'Search here...');
                $("#gs_ROAD_NAME").attr('placeholder', 'Search here...');

            },
        }).trigger("reloadGrid");

        //Footer and Search Toolbar of Schedule Details grid continues from here
        $grid2.jqGrid('filterToolbar', {
            stringResult: true,
            searchOnEnter: false,
            defaultSearch: "cn",
            ignoreCase: true,

        }).trigger("reloadGrid");

        $grid2.jqGrid('navGrid', '#navGridScheduleDetails', { add: false, edit: false, del: false, search: false, refresh: false }).trigger("reloadGrid");

        //alert("2");

    };
    //#endregion


    //----------------------------Schedule details with isScheduleDownload value as 'N'------------------------------//

    //#region Schedule Details if isScheduleDownload is 'N'
    window.showScheduleDetails3 = function (id) {

        jQuery("#tableScheduleDetails").jqGrid('GridUnload');

        jQuery("#tableMonitorDetails").jqGrid('setGridState', 'hidden');


        //$("#fourthGrid").hide();
        $("#logDetailsGrid").hide();
        $("#scheduleDetailsGrid").show();

        $grid2 = $("#tableScheduleDetails");
        var mygrid2 = jQuery("#tableScheduleDetails").jqGrid({


            url: "/QualityMonitoringHelpDesk/ListScheduleResult/",
            datatype: 'json',
            mtype: 'POST',
            postData: { selectRowId: function () { return id }, yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() },
            colNames: ['Schedule Code', 'State Name', 'District Name', 'Road Name', 'Device Flag', 'Finalize Flag', 'Inspection Status Flag', 'Schedule Assign Key', 'Is Enquiry',
                        'Schedule Download', 'Total Image Count', 'CQC Forward Flag'],
            colModel: [

                        { name: 'ADMIN_SCHEDULE_CODE', index: 'ADMIN_SCHEDULE_CODE', width: 100, align: 'left', hidden: true },
                        { name: 'STATE_NAME', index: 'STATE_NAME', width: 100, align: 'left' },
                        { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', width: 100, align: 'left' },
                        { name: 'ROAD_NAME', index: 'ROAD_NAME', width: 250, align: 'left' },
                        { name: 'DEVICE_FLAG', index: 'DEVICE_FLAG', width: 90, align: 'left', search: false },
                        { name: 'FINALIZE_FLAG', index: 'FINALIZE_FLAG', width: 90, align: 'left', search: false },
                        { name: 'INSP_STATUS_FLAG', index: 'INSP_STATUS_FLAG', width: 90, align: 'left', search: false },
                        { name: 'SCHEDULE_ASSIGNED', index: 'SCHEDULE_ASSIGNED', width: 90, align: 'left', search: false },
                        { name: 'ADMIN_IS_ENQUIRY', index: 'ADMIN_IS_ENQUIRY', width: 90, align: 'left', search: false },
                        { name: 'IS_SCHEDULE_DOWNLOAD', index: 'IS_SCHEDULE_DOWNLOAD', width: 90, align: 'left', search: false },
                        { name: 'TOTAL_IMAGE_COUNT', index: 'TOTAL_IMAGE_COUNT', width: 90, align: 'left', search: false },
                        { name: 'CQC_FORWARD_FLAG', index: 'CQC_FORWARD_FLAG', width: 90, align: 'left', search: false },

            ],
            index: 'ADMIN_SCHEDULE_CODE',
            pager: jQuery('#navGridScheduleDetails'),
            rowNum: 5,
            rowList: [5, 10, 15, 20],
            cache: false,
            rownumbers: true,
            recordtext: '{2} records found',
            //shrinkToFit: false,
            autowidth: true,
            height: 'auto',
            sortname: 'ADMIN_SCHEDULE_CODE',
            sortorder: 'asc',
            viewrecords: true,
            autowidth: true,
            hidegrid: false,
            caption: 'Schedule Details',




        }).trigger("reloadGrid");

        //Footer and Search Toolbar of Schedule Details grid continues from here
        $grid2.jqGrid('filterToolbar', {
            stringResult: true,
            searchOnEnter: false,
            defaultSearch: "cn",
            ignoreCase: true,

        }).trigger("reloadGrid");

        $grid2.jqGrid('navGrid', '#navGridScheduleDetails', { add: false, edit: false, del: false, search: false, refresh: false }).trigger("reloadGrid");

        $grid1.jqGrid('navButtonAdd', "#navGridScheduleDetails", {
            caption: "Unlock", title: "Unlock the Schedule Details", buttonicon: 'ui-icon-unlocked',
            onClickButton: function () {
                //alert(id);
                myDialogBox2(id);
            }
        });

    };

    //#endregion


    //----------------------dialogBox for schedule unlocking------------//

    //#region Definalizing Schedule Details
    function myDialogBox2(id) {

        var r = confirm('Are you Sure to Definalize the Schedule Details!!!');
        if (r == true) {
            // var res = unlockSchedule(id);
            if (unlockSchedule(id)) {
                //alert(res);
                alert('Schedule Definalized Successfully ');
            }
            else {
                alert('Sorry, No Such Record Exists!!!!');

            }
        }
        else {
            $(this).hide();
        }

    }

    //----------------------function for schedule unlocking and updating database------------//

    function unlockSchedule(id) {
        return jQuery.ajax({
            url: "/QualityMonitoringHelpDesk/UnlockScheduleData/",
            data: { selectRowId: function () { return id }, yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() },
            cache: false,
            type: 'POST',
            success: function (result) {

                if (result) {
                    //alert(result);
                    jQuery("#tableScheduleDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListScheduleResult/", page: 1, postData: { yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() } }).trigger("reloadGrid");

                    return true;
                }
                else {
                    //alert(result+"inelse");
                    return false;
                }
            },





        });
    }
    //#endregion



    //----------------------------Log details------------------------------//

    //#region Log Details
    window.logDetails = function (id) {


        jQuery("#tableLogDetails").jqGrid('GridUnload');

        $("#scheduleDetailsGrid").hide();
        $("#logDetailsGrid").show();
        //$("#fourthGrid").hide();
        jQuery("#tableMonitorDetails").jqGrid('setGridState', 'hidden');

        $grid3 = $("#tableLogDetails");


        var mygrid3 = jQuery("#tableLogDetails").jqGrid({


            url: "/QualityMonitoringHelpDesk/LogDetails/",
            datatype: 'json',
            mtype: 'POST',
            postData: { selectRowId: function () { return id } },
            colNames: ['Log ID', 'Mobile No.', 'IMEI No.', 'OS Version', 'Model Name', 'Network Provider', 'Login Date', 'Logout Date', 'App Version', 'Log Mode'],
            colModel: [
                        { name: 'LOG_ID', index: 'LOG_ID', width: 100, align: 'left', hidden: true, search: false },
                        { name: 'MOBILE_NO', index: 'MOBILE_NO', width: 100, align: 'left' },
                        { name: 'IMEI_NO', index: 'IMEI_NO', width: 100, align: 'left', search: false },
                        { name: 'OS_VERSION', index: 'OS_VERSION', width: 100, align: 'left', search: false },
                        { name: 'MODEL_NAME', index: 'MODEL_NAME', width: 100, align: 'left', search: false },
                        { name: 'NETWORK_PROVIDER', index: 'NETWORK_PROVIDER', width: 100, align: 'left', search: false },
                        { name: 'LOGIN_DATE_TIME', index: 'LOGIN_DATE_TIME', width: 100, align: 'left', search: false },
                        { name: 'LOGOUT_DATE_TIME', index: 'LOGOUT_DATE_TIME', width: 100, align: 'left', search: false, hidden: true },
                        { name: 'APP_VERSION', index: 'APP_VERSION', width: 100, align: 'left', search: false },
                        { name: 'LOG_MODE', index: 'LOG_MODE', width: 100, align: 'left', search: false },
            ],
            index: 'LOG_ID',
            pager: jQuery('#navGridLogDetails'),
            rowNum: 5,
            rowList: [5, 10, 15, 20],
            cache: false,
            rownumbers: true,
            recordtext: '{2} records found',
            height: 'auto',
            sortname: 'LOG_ID',
            sortorder: 'asc',
            viewrecords: true,
            autowidth: true,
            hidegrid: false,
            caption: 'Log Details',




        }).trigger("reloadGrid");

        //Footer and Search Toolbar of Schedule Details grid continues from here
        //$grid3.jqGrid('filterToolbar', {
        //    stringResult: true,
        //    searchOnEnter: false,
        //    defaultSearch: "cn",
        //    ignoreCase: true,

        //}).trigger("reloadGrid");

        $grid3.jqGrid('navGrid', '#navGridLogDetails', { add: false, edit: false, del: false, search: false, refresh: false }).trigger("reloadGrid");

    };

    //#endregion


    //----------------------------Reset Imei Number------------------------------//
    //#region Updating new IMEI Numbers

    $(document).on('click', 'a.resetIMEI', function () {

        var id = $(this).attr("id");
        var userName = 0;

        $.ajax({
            url: "/QualityMonitoringHelpDesk/GetUserName/",
            data: { AdminQmCode: id },
            cache: false,
            type: 'POST',
            success: function (result) {
                if (result) {
                    // alert(result);
                    myDialogBox(id, result);
                    //alert(userName);

                }
                else
                    alert(result + "in else");
            }
        });






    });

    function myDialogBox(id, userName) {

        var r = confirm('Are you Sure to Reset The IMEI Number of ' + userName + ' !!!')
        if (r == true) {

            if (reloadData(id)) {
                alert('The Imei Number of Mrs./Mr.' + userName + ' has been Reset Successfully ');
            }
            //else {
            //    jAlert('error', 'Sorry, something Went Wrong!!!!', 'Error Dialog');

            //}
        }
        else {
            $(this).hide();
        }

    }

    function reloadData(id) {
        return jQuery.ajax({
            url: "/QualityMonitoringHelpDesk/StoreIMEINumber/",
            data: { AdminQmCode: id },
            cache: false,
            type: 'POST',
            success: function (result) {
                if (result) {

                    // alert(result);
                    jQuery("#tableMonitorDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListUsers/", page: 1 }).trigger("reloadGrid");
                    jQuery("#tableMonitorDetails").jqGrid('setGridState', 'visible');
                    return true;

                }
                else {
                    return false;

                }

            }
        });
    }

    //#endregion

}


//----------------------------Observation and Image Details------------------------------//
//#region Observation Details
function observationDetails() {



    $(function () {
        $("#fromDate").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        }).attr('readonly', 'readonly');

        $("#toDate").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        }).attr('readonly', 'readonly');



    });


    $("#message").hide();
    $("#message2").hide();
    //$("#userAndScheduleGrids").hide();
    //$("#observationAndImageGrids").show();
   // $("#imageGrid").hide();

    $('#observTable').jqGrid('GridUnload');

    jQuery("#observTable").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ObservationDetails/", page: 1, postData: { FromDate: function () { return ($("#fromDate").val()); }, ToDate: $("#toDate").val() }, qm: $("#myList4").val() }).trigger("reloadGrid");

    $observgrid = $("#observTable");

    var counter = 0;
    var counter2 = 0;
    var counter3 = 0;
    $('#observTable').jqGrid('GridUnload');

    var myobservgrid = $("#observTable").jqGrid({

        url: '/QualityMonitoringHelpDesk/ObservationDetails/',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Observation ID', 'Inspection Date', 'Monitor Name', 'State', 'District', 'Block', 'Package', 'Sanctioned Year', 'Road Name', 'Road Status', 'Start Chainage'
                    , 'End Chainage', 'Overall Grade', 'Start Latitude', 'End Latitude', 'Start Longitude', 'End Longitude', 'Image Details'],
        colModel: [
                    { name: 'QM_OBSERVATION_ID', index: 'QM_OBSERVATION_ID', width: 50, align: 'left', hidden: true },
                    { name: 'QM_INSPECTION_DATE', index: 'QM_INSPECTION_DATE', width: 80, align: 'left', search: false },
                    { name: 'MONITER_NAME', index: 'MONITER_NAME', width: 80, align: 'left', search: true },
                    { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, align: 'left', search: true },
                    { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', width: 80, align: 'left', search: true },
                    { name: 'BLOCK_NAME', index: 'BLOCK_NAME', width: 80, align: 'left', search: false },
                    { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 50, align: 'left', search: false },
                    { name: 'SANCTIONED_YEAR', index: 'SANCTIONED_YEAR', width: 80, align: 'left', search: false },
                    { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 150, align: 'left', search: false },
                    { name: 'ROAD_STATUS', index: 'ROAD_STATUS', width: 80, align: 'left', search: false },
                    { name: 'QM_INSPECTED_START_CHAINAGE', index: 'QM_INSPECTED_START_CHAINAGE', width: 50, align: 'left', search: false },
                    { name: 'QM_INSPECTED_END_CHAINAGE', index: 'QM_INSPECTED_END_CHAINAGE', width: 50, align: 'left', search: false },
                    { name: 'OVERALL_GRADE', index: 'OVERALL_GRADE', width: 100, align: 'left', search: false },
                    { name: 'QM_START_LATITUDE', index: 'QM_START_LATITUDE', width: 90, align: 'left', search: false },
                    { name: 'QM_END_LATITUDE', index: 'QM_END_LATITUDE', width: 90, align: 'left', search: false },
                    { name: 'QM_START_LONGITUDE', index: 'QM_START_LONGITUDE', width: 90, align: 'left', search: false },
                    { name: 'QM_END_LONGITUDE', index: 'QM_END_LONGITUDE', width: 90, align: 'left', search: false },
                    { name: 'ImageDetails', index: 'ImageDetails', width: 30, align: 'center', search: false },

        ],
        index: 'QM_OBSERVATION_ID',
        pager: jQuery('#observNavGrid'),
        rowNum: 2147483647,
       // rowList: [5, 10, 15, 20],
        autowidth: false,
        shrinkToFit: false,
        width: 1190,
        rownumbers: true,
        recordtext: '{2} records found',
        height: '300',       
        sortname: 'QM_OBSERVATION_ID',
        sortorder: 'asc',
        viewrecords: true,
        hidegrid: true,
        hiddengrid: false,
        caption: 'Observation Details',
        loadComplete: function () {
            $('#observTable_rn').html('Sr.<br/>No.');
            $("#gs_MONITER_NAME").attr('placeholder', 'Search here...');
            $("#gs_STATE_NAME").attr('placeholder', 'Search here...');
            $("#gs_DISTRICT_NAME").attr('placeholder', 'Search here...');

        },
        onHeaderClick: function () {

        },
        postData: { FromDate: $("#fromDate").val(), ToDate: $("#toDate").val(), qm: $("#myList4").val() }



    });
    $("#observTable").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
    $("#search1").click(function () {

        var check = checkDates();
        if (check) {

            $("#observationAndImageGrids").hide();
        }
        else {

            $("#userAndScheduleGrids").hide();

            $("#imageGrid").hide();
            $("#observationAndImageGrids").show();

            jQuery("#observTable").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ObservationDetails/", page: 1, postData: { FromDate: function () { return ($("#fromDate").val()); }, ToDate: $("#toDate").val(), qm: $("#myList4").val() } }).trigger("reloadGrid");
        }

    });

    //$observgrid.jqGrid('filterToolbar', {
    //    stringResult: true,
    //    searchOnEnter: false,
    //    defaultSearch: "cn",
    //    ignoreCase: true,

    //});

    $observgrid.jqGrid('navGrid', '#observNavGrid', { add: false, edit: false, del: false, search: false, refresh: false });
    //#endregion

    //#region Image Details
    window.imageDetailsObservation = function (adminScheduleId, roadId) {

        jQuery("#imageTable").jqGrid('GridUnload');

        $("#imageGrid").show();
        //$("#observationGrid").hide();
        $ImageGrid = $("#imageTable");
        var myImageGrid = jQuery("#imageTable").jqGrid({


            url: "/QualityMonitoringHelpDesk/GetImageDetails/",
            datatype: 'json',
            mtype: 'POST',
            postData: { adminScheduleCode: function () { return adminScheduleId }, roadCode: function () { return roadId } },
            colNames: ['File Id', 'File Description', 'File Name', 'File Upload Date', 'Latitude', 'Longitude'],
            colModel: [
                        { name: 'QM_FILE_ID', index: 'QM_FILE_ID', width: 100, align: 'left', hidden: true, search: false },
                        { name: 'QM_FILE_DESCR', index: 'QM_FILE_DESCR', width: 250, align: 'left', search: false },
                        { name: 'QM_FILE_NAME', index: 'QM_FILE_NAME', width: 180, align: 'left', search: false },
                        { name: 'QM_FILE_UPLOAD_DATE', index: 'QM_FILE_UPLOAD_DATE', width: 200, align: 'left', search: false },
                        { name: 'QM_LATITUDE', index: 'QM_LATITUDE', width: 250, align: 'left', search: false },
                        { name: 'QM_LONGITUDE', index: 'QM_LONGITUDE', width: 255, align: 'left', search: false },

            ],
            index: 'QM_FILE_ID',
            pager: jQuery('#imageNavGrid'),
            rowNum: 2147483647,
            //rowNum: 5,
            //rowList: [5, 10, 15, 20],
            cache: false,
            rownumbers: true,
            recordtext: '{2} records found',
            height: '200',
            sortname: 'QM_FILE_ID',
            sortorder: 'asc',
            viewrecords: true,
            autowidth: false,
            shrinkToFit: false,
            width: 1190,
            hidegrid: false,
            caption: 'Image Details',




        }).trigger("reloadGrid");

        //Footer and Search Toolbar of Schedule Details grid continues from here
        //$ImageGrid.jqGrid('filterToolbar', {
        //    stringResult: true,
        //    searchOnEnter: false,
        //    defaultSearch: "cn",
        //    ignoreCase: true,

        //}).trigger("reloadGrid");

        $ImageGrid.jqGrid('navGrid', '#imageNavGrid', { add: false, edit: false, del: false, search: false, refresh: false }).trigger("reloadGrid");



    };

}
//#endregion


//----------------------------Validation on From Date and To Date------------------------------//
//#region Validation on Dates
function checkDates() {

    var d1 = Date.parse($("#fromDate").val());
    var d2 = Date.parse($("#toDate").val());




    if ($.datepicker.parseDate('dd/mm/yy', $("#fromDate").val()) > $.datepicker.parseDate('dd/mm/yy', $("#toDate").val())) {
        //alert("ifcheck");
        $("#observationAndImageGrids").hide();
        $("#userAndScheduleGrids").hide();
        $("#imageGrid").hide();
        $("#message").show();
        $("#message2").show();
        return true;
        // jQuery.ready();

    }
    else {
        //alert("elsecheck");
        $("#message").hide();
        $("#message2").hide();

        return false;
    }


}
//#endregion

//******************** Notificaction *****************************/
//#region


function LoadNotificationDetails() {

    $('#tblNotificationDetails').jqGrid('GridUnload');

    jQuery("#tblNotificationDetails").jqGrid('setGridState', 'hidden');
    //jQuery("#tableMonitorDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListQMMessageNotification/", page: 1, postData: { qm: $("#qmTypeNotList").val() } }).trigger("reloadGrid");
   // $('#divPagerNotificationDetails').show();
    var State = $("#qmTypeStateList").val();
    
    $('#tblNotificationDetails').jqGrid({
        url: '/QualityMonitoringHelpDesk/ListQMMessageNotification/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['User Name', 'Monitor Name', 'Message', 'Message Type', 'Is Downloaded', 'Date', 'Edit', 'Delete'],
        colModel: [
         { name: 'UserName', index: 'UserName', height: 'auto', width: 150, align: "left", sortable: false },
         { name: 'MonitorName', index: 'MonitorName', height: 'auto', width: 150, align: "left", sortable: false },
         { name: 'Message', index: 'Message', height: 'auto', width: 370, align: "left", sortable: false },
         { name: 'MessageType', index: 'MessageType', height: 'auto', width: 150, align: "center", sortable: false },
         { name: 'IsDownload', index: 'IsDownload', height: 'auto', width: 100, align: "center", sortable: false },
         { name: 'CreatedDate', index: 'CreatedDate', height: 'auto', width: 100, align: "center", sortable: false },
         { name: 'edit', width: 50, sortable: false, resize: false, align: "left", sortable: false },
         { name: 'delete', width: 50, sortable: false, resize: false, align: "left", sortable: false }
        ],
        postData: { "qm": $("#qmTypeNotList").val(), "State": State },
        pager: jQuery('#divPagerNotificationDetails'),
        rowNum: 10,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'UserName',
        sortorder: "desc",
        caption: 'Notification Details',
        height: '100%',
        rownumbers: true,      
        hidegrid: true,
        //autowidth: true,
        emptyrecords: 'No Records Found',
        loadComplete: function () {
            $('#tblNotificationDetails_rn').html('Sr.<br/>No.');
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
               
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });


}

function CloseNotificationDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tblNotificationDetails").jqGrid('setGridState', 'visible');
    // LoadNotificationDetails();
}

// Editing the Notification Proposal
function EditDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit Notification Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseNotificationDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divNotificationForm").load('/QualityMonitoringHelpDesk/EditQMMessageNotification/' + id, function (response) {
        //    $.validator.unobtrusive.parse($('#divNotificationForm'));
        //    unblockPage();
        //});
        id = id + '$' + $("#qmTypeNotList").val();
        $.ajax({
            url: "/QualityMonitoringHelpDesk/EditQMMessageNotification/" + id,
            type: "GET",
            async: false,
            cache: false,
            // data: $("#qmTypeNotList").val(),
            success: function (data) {

                //  $("#mainDiv").html(data);

                $('#divNotificationForm').html(data);               
                if ($("#ddlState").val() > 0) {
                    $("#ddlMonitor").show();
                    $('#lblMonitor').show();
                    $('#lblStarMonitor').show();
                    $('#ddlState').attr("disabled", true);
                }
                $("#ddlMonitor").attr("disabled", true);
                $("#ddlMessageType").attr("disabled", true);

                //if ($("#ddlState").is(":visible")) {

                //    $('#ddlState').attr("disabled", true);
                //}

                unblockPage();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
        $('#divNotificationForm').show('slow');
        $("#divNotificationForm").css('height', 'auto');
    });

    // $("#tblNotificationDetails").jqGrid('setGridState', 'hidden');

    //$('#rbtnNotificationdetails').trigger('click');


}
//end function state change

// Delete Notification Proposal
function DeleteDetails(id) {

    id = id + '$' + $("#qmTypeNotList").val();

    if (confirm("Are you sure you want to delete Message Notification Details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/DeleteQMMessageNotification/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    ClearNotificationDetails();
                    ////$('#tblNotificationDetails').trigger('reloadGrid');
                    //  $('#btnNotificationAdd').trigger('click');
                    CloseNotificationDetails();
                    LoadNotificationDetails();
                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}
//Clear Notification Details
function ClearNotificationDetails() {
    $('#ddlMessageType').val(0);
    $('#ddlState').val(0);
    $('#txtMessageDesc').val('');

    $('#ddlMonitor').empty();
    $('#ddlMonitor').append("<option value=0>All Monitors<option>");
    $("#ddlMessageType").attr("disabled", false);
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
    $("#ddlMonitor").attr("disabled", false);
    if ($("#ddlState").is(":visible")) {
        $('#ddlState').attr("disabled", false);
    }
    if ($('#qmTypeStateList').val() > 0) {
        $('#ddlState').val($('#qmTypeStateList').val());
        $('#ddlState').trigger('change');
    }
    else {
        $('#ddlState').val(0);
        $("#ddlMonitor").hide();
        $('#lblMonitor').hide();
        $('#lblStarMonitor').hide();
        $('#ddlMonitor').empty();
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');
    }
}
//#endregion
//************************* End Notification ****************************//

//******************** BroadCast Notificaction *****************************/
//#region


function LoadBroadCastNotificationDetails() {

    jQuery("#tblbrodcastNotificationDetails").jqGrid('setGridState', 'hidden');

   // $('#divPagerbrodcastNotificationDetails').show();

    //jQuery("#tableMonitorDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListQMMessageNotification/", page: 1, postData: { qm: $("#qmTypeNotList").val() } }).trigger("reloadGrid");

    var State = $("#qmTypeBroadNotStateList").val();

   
    $('#tblbrodcastNotificationDetails').jqGrid('GridUnload');
   
    $('#tblbrodcastNotificationDetails').jqGrid({
        url: '/QualityMonitoringHelpDesk/ListQMBroadCastMessageNotification/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Broadcast Number', 'Message', 'Is Downloaded', 'Date', 'Edit', 'Delete'],
        colModel: [
         { name: 'BroadCastId', index: 'BroadCastId', height: 'auto', width: 150, align: "center", sortable: false },
         { name: 'Message', index: 'Message', height: 'auto', width: 550, align: "left", sortable: false },
         { name: 'IsDownload', index: 'IsDownload', height: 'auto', width: 110, align: "center", sortable: false },
         { name: 'CreatedDate', index: 'CreatedDate', height: 'auto', width: 120, align: "center", sortable: false },
         { name: 'edit', width: 100, sortable: false, resize: false, align: "left", sortable: false },
         { name: 'delete', width: 100, sortable: false, resize: false, align: "left", sortable: false }
        ],
        postData: { "qm": $("#qmTypeBroadNotList").val(), "State": State },
        pager: $('#divPagerbrodcastNotificationDetails'),
        rowNum: 10,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'BroadCastId',
        sortorder: "desc",
        caption: 'Broadcast Notification Details',
        height: '100%',
        rownumbers: true,      
        hidegrid: true,
        autowidth: true,
        emptyrecords: 'No Records Found',
        loadComplete: function () {
            $('#tblbrodcastNotificationDetails_rn').html('Sr.<br/>No.');
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
               
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });


}

function CloseBroadCastNotificationDetails() {
    $('#accordionBroadcast').hide('slow');
    $('#divbrodcastNotificationForm').hide('slow');
    $("#tblbrodcastNotificationDetails").jqGrid('setGridState', 'visible');
    // LoadBroadCastNotificationDetails();
}

// Editing the Notification Proposal
function EditBroadCastDetails(id) {

    $("#accordionBroadcast div").html("");
    $("#accordionBroadcast h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit Broadcast Notification Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseBroadCastNotificationDetails();" /></a>'
            );

    $('#accordionBroadcast').show('fold', function () {
        blockPage();
        //$("#divNotificationForm").load('/QualityMonitoringHelpDesk/EditQMMessageNotification/' + id, function (response) {
        //    $.validator.unobtrusive.parse($('#divNotificationForm'));
        //    unblockPage();
        //});
        id = id + '$' + $("#qmTypeBroadNotList").val();
        $.ajax({
            url: "/QualityMonitoringHelpDesk/EditQMBroadCastMessageNotification/" + id,
            type: "GET",
            async: false,
            cache: false,
            // data: $("#qmTypeNotList").val(),
            success: function (data) {

                //  $("#mainDiv").html(data);

                $('#divbrodcastNotificationForm').html(data);
                //alert($("#ddlStateB").val());
                //alert($("#ddlStateB").is(":visible"));

                //if ($("#ddlStateB").is(":visible")) {
                //    $('#ddlStateB').attr("disabled", true);
                //}
                if ($("#qmTypeBroadNotList").val() === "S") {
                    $('#ddlStateB').attr("disabled", true);
                }
                unblockPage();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
        $('#divbrodcastNotificationForm').show('slow');
        $("#divbrodcastNotificationForm").css('height', 'auto');
    });

    // $("#tblNotificationDetails").jqGrid('setGridState', 'hidden');

    //$('#rbtnNotificationdetails').trigger('click');


}
//end function state change

// Delete Notification Proposal
function DeleteBroadCastDetails(id) {

    id = id + '$' + $("#qmTypeBroadNotList").val();

    if (confirm("Are you sure you want to delete Broadcast Message Notification Details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/DeleteQMBroadCastMessageNotification/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    ClearBroadCastNotificationDetails();
                    //$('#tblNotificationDetails').trigger('reloadGrid');
                    // $('#btnNotificationAdd').trigger('click');
                    CloseBroadCastNotificationDetails();
                    LoadBroadCastNotificationDetails();
                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}
//Clear Broadcast Notification Details
function ClearBroadCastNotificationDetails() {
    $('#ddlStateB').val(0);
    $('#txtMessageDescB').val('');
    //if ($("#ddlStateB").is(":visible")) {
    //    $('#ddlStateB').attr("disabled", false);
    //}
}
//#endregion
//************************* End Broadcast Notification ****************************//

//*************QM Help Desk Reset IMEI Details
function ResetIMEIDetailsGrid() {
    jQuery("#tblQmIMEINoResetDetails").jqGrid('setGridState', 'hidden');
    //$('#divQmIMEINoResetDetails').show();
    $('#tblQmIMEINoResetDetails').jqGrid('GridUnload');
    $('#tblQmIMEINoResetDetails').jqGrid({
        url: '/QualityMonitoringHelpDesk/ListUsersIMEINoDetail/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['User Id', 'User Name', 'Monitor Name', 'Monitor Type', 'IMEI No.', 'Application Mode', 'Reset IMEI', 'Update'],
        colModel: [
             { name: 'UserId', index: 'ADMIN_QM_CODE', width: 150, align: 'left', hidden: true  },
             { name: 'UserName', index: 'UserName', width: 250, align: 'left', search: true, sortable: true },
             { name: 'MonitorName', index: 'MonitorName', width: 250, align: 'left', search: true, sortable: false },
             { name: 'QMType', index: 'QMType', width: 100, align: 'center', search: true, sortable: false, search: true},
             { name: 'ImeiNo', index: 'ImeiNo', height: 'auto', width: 220, align: "center", sortable: false, search:true },
             { name: 'AppMode', index: 'AppMode', height: 'auto', width: 100, align: "center", sortable: true, search:true },
             { name: 'reset', index: 'reset', height: 'auto', width: 100, align: "center", sortable: false , search:false},
             { name: 'update', index: 'update', width: 100, sortable: false, resize: false, align: "left", sortable: false , search:false}
        ],
        //postData: { "qm": $("#qmTypeBroadNotList").val(), "State": State },
        pager: jQuery('#divQmIMEINoResetDetails'),
        rowNum: 2147483647,
       // rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'UserName',
        sortorder: "desc",
        caption: 'Reset IMEI Details',
        height: 'auto',
        rownumbers: true,
        hidegrid: true,
        autowidth: true,
        emptyrecords: 'No Records Found',
        loadComplete: function () {
            $('#tblQmIMEINoResetDetails_rn').html('Sr.<br/>No.');
            $("#gs_UserName").attr('placeholder', 'Search here...');
            $("#gs_MonitorName").attr('placeholder', 'Search here...');
            $("#gs_QMType").attr('placeholder', 'Search here...');
            $("#gs_ImeiNo").attr('placeholder', 'Search here...');
            $("#gs_AppMode").attr('placeholder', 'Search here...');
          
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {

                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });

    $("#tblQmIMEINoResetDetails").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function ResetIMEIMobDetail(id) {
    

    if (confirm("Are you sure you want to reset IMEI Number?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/UpdateIMEINoResetDetails/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                //  $("#mainDiv").html(data);

                if (data.success == true) {

                    alert(data.message);

                    $('#tblQmIMEINoResetDetails').trigger('reloadGrid');

                }
                else if (data.success == false) {
                    if (data.message != "") {

                        alert('false');
                        // $('#dvErrorMessage').show('slow');
                        // $('#spnErrMessage').html(data.message);

                    }

                }
                else {
                    // $("#divNotificationForm").html(data);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}

function UpdateIMEIApplicationMode(id) {
  
    var split = id.split("$");
    var FromMode = "";
    var ToMode = "";
    var substr = split[2].substr(0, 1);
    if (substr == "D") {
        FromMode = "DEMO";
        ToMode = "LIVE";
    }
    else {
        FromMode = "LIVE";
        ToMode = "DEMO";
    }

    if (confirm("Are you sure you want to change Application Mode  From " + FromMode + " To " + ToMode + " ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/UpdateIMEIApplicationMode/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                //  $("#mainDiv").html(data);

                if (data.success == true) {

                    alert(data.message);

                    $('#tblQmIMEINoResetDetails').trigger('reloadGrid');

                }
                else if (data.success == false) {
                    if (data.message != "") {

                        alert('false');
                        // $('#dvErrorMessage').show('slow');
                        // $('#spnErrMessage').html(data.message);

                    }

                }
                else {
                    // $("#divNotificationForm").html(data);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}



function myResetIMEIDialogBox(id, userName) {

    var r = confirm('Are you Sure to Reset The IMEI Number of ' + userName + ' !!!')
    if (r == true) {

        if (reloadResetIMEIData(id)) {
            alert('The Imei Number of Mrs./Mr.' + userName + ' has been Reset Successfully ');
        }
        //else {
        //    jAlert('error', 'Sorry, something Went Wrong!!!!', 'Error Dialog');

        //}
    }
    else {
        $(this).hide();
    }

}

function reloadResetIMEIData(id) {
    return jQuery.ajax({
        url: "/QualityMonitoringHelpDesk/StoreIMEINumber/",
        data: { AdminQmCode: id },
        cache: false,
        type: 'POST',
        success: function (result) {
            if (result) {

                // alert(result);
                jQuery("#tblQmIMEINoResetDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListUsersIMEINoDetail/", page: 1 }).trigger("reloadGrid");
                //jQuery("#tblQmIMEINoResetDetails").jqGrid('setGridState', 'visible');
                return true;

            }
            else {
                return false;

            }

        }
    });
}


//******End Reset IMEI Details**************



//----------------------------Ready Function------------------------------//

jQuery(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionBroadcast").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
   
    $("#observDetails").hide();
    $("#userAndScheduleGrids").hide();
    $("#observationAndImageGrids").hide();
    $("#notificationDetails").hide();
    $('#accordion').hide();
    $('#divNotificationForm').hide();
    $("#broadCastnotificationDetails").hide();
    $("#brodcastnotificationDetailsGrids").hide();
    $("#divbrodcastNotificationForm").hide();
    $("#notificationDetailsGrids").hide();
    $('#accordionBroadcast').hide();
    $('#qmIMEINoRestGrids').hide();
    $("#userScehduleDetails").show();
    userAndScheduleDetails();

    observationDetails();

    LoadBroadCastNotificationDetails();

    ResetIMEIDetailsGrid();
    
    LoadNotificationDetails();

    $(".rg").change(function () {


        if ($("input:radio[name='mabqsm'][value='userSchedule']").is(":checked")) {

            $("#observDetails").hide();           
            $("#userAndScheduleGrids").hide();
            $("#observationAndImageGrids").hide();
            $("#notificationDetails").hide();
            $('#accordion').hide();
            $('#divNotificationForm').hide();
            $("#broadCastnotificationDetails").hide();            
            $("#brodcastnotificationDetailsGrids").hide();            
            $("#divbrodcastNotificationForm").hide();
            $('#accordionBroadcast').hide();
            $("#notificationDetailsGrids").hide();           
            $('#qmIMEINoRestGrids').hide();
            $("#userScehduleDetails").show();
            $('#userAndScheduleGrids').show();
            $("#divRegradeATR").hide();

        }

        else if ($("input:radio[name='mabqsm'][value='userObserv']").is(":checked")) {
                
            $("#userAndScheduleGrids").hide();          
            $("#notificationDetails").hide();
            $('#accordion').hide();
            $('#divNotificationForm').hide();          
            $("#notificationDetailsGrids").hide();
            $("#broadCastnotificationDetails").hide();
            $("#brodcastnotificationDetailsGrids").hide();
            $("#divbrodcastNotificationForm").hide();
            $('#accordionBroadcast').hide();
            $('#qmIMEINoRestGrids').hide();
            $("#userScehduleDetails").hide();
            $("#observDetails").show();
            $("#observationAndImageGrids").show();
            checkDates();
            $("#divRegradeATR").hide();
            //observationDetails();
        }
        else if ($("input:radio[name='mabqsm'][value='rbtnNotificationdetails']").is(":checked")) {

            $("#observDetails").hide();
            $("#userAndScheduleGrids").hide();          
            $('#accordion').hide();
            $('#divNotificationForm').hide();                  
            $('#qmIMEINoRestGrids').hide();
            $("#userScehduleDetails").hide();
            $("#observationAndImageGrids").hide();
            $("#broadCastnotificationDetails").hide();
            $("#brodcastnotificationDetailsGrids").hide();
            $("#divbrodcastNotificationForm").hide();
            $('#accordionBroadcast').hide();
            $("#notificationDetails").show();
            $("#notificationDetailsGrids").show();
            $("#divRegradeATR").hide();

            if ($("#qmTypeNotList").val() === "S") {
                $('#lblNotificationState').show();
                $('#qmTypeStateList').show();
            }
            else {
                $('#lblNotificationState').hide();
                $('#qmTypeStateList').hide();
            }
            //LoadNotificationDetails();
        }
        else if ($("input:radio[name='mabqsm'][value='rbtnBroadCastNotificationdetails']").is(":checked")) {

            $("#observDetails").hide();
            $("#userAndScheduleGrids").hide();
            $("#notificationDetails").hide();
            $('#accordion').hide();
            $('#divNotificationForm').hide();
            $("#broadCastnotificationDetails").hide();          
            $('#qmIMEINoRestGrids').hide();
            $("#userScehduleDetails").hide();
            $("#observationAndImageGrids").hide();
            $("#broadCastnotificationDetails").hide();
            $("#notificationDetailsGrids").hide();
            $("#divbrodcastNotificationForm").hide();
            $('#accordionBroadcast').hide();
            $("#broadCastnotificationDetails").show();
            $("#brodcastnotificationDetailsGrids").show();
        
            $('#lblBroadNotListState').hide();
              $('#qmTypeBroadNotStateList').hide();
            //LoadBroadCastNotificationDetails();
            $("#tblbrodcastNotificationDetails").trigger('reloadGrid');
            $("#divRegradeATR").hide();

        }
        else if ($("input:radio[name='mabqsm'][value='rbtnQMIMEIResetdetails']").is(":checked")) {

            $("#observDetails").hide();
            $("#userAndScheduleGrids").hide();
            $("#notificationDetails").hide();
            $('#accordion').hide();
            $('#divNotificationForm').hide();
            $("#broadCastnotificationDetails").hide();           
            $("#userScehduleDetails").hide();
            $("#observationAndImageGrids").hide();
            $("#notificationDetailsGrids").hide();
            $("#broadCastnotificationDetails").hide();
            $("#brodcastnotificationDetailsGrids").hide();
            $("#divbrodcastNotificationForm").hide();
            $('#qmIMEINoRestGrids').show();
            $("#divRegradeATR").hide();
        }
        else if ($("input:radio[name='mabqsm'][value='userATR']").is(":checked")) {

            $("#observDetails").hide();
            $("#userAndScheduleGrids").hide();
            $("#notificationDetails").hide();
            $('#accordion').hide();
            $('#divNotificationForm').hide();
            $("#broadCastnotificationDetails").hide();
            $("#userScehduleDetails").hide();
            $("#observationAndImageGrids").hide();
            $("#notificationDetailsGrids").hide();
            $("#broadCastnotificationDetails").hide();
            $("#brodcastnotificationDetailsGrids").hide();
            $("#divbrodcastNotificationForm").hide();
            $('#qmIMEINoRestGrids').hide();
            
            $("#divRegradeATR").show();
            $("#divRegradeATR").load("/QualityMonitoringHelpDesk/ChangeATRStatus/");
        }

    });

    $('#btnNotificationGo').click(function (e) {

        LoadNotificationDetails();
        CloseNotificationDetails();

    });

    $('#btnNotificationAdd').click(function (e) {

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Add Notification Details</a>" +

                '<a href="#" style="float: right;">' +
                '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseNotificationDetails();" /></a>' +
                '<span style="float: right;"></span>'
                );

        $("#divNotificationForm").load('/QualityMonitoringHelpDesk/CreateQMMessageNotification/' + $("#qmTypeNotList").val());
        $('#divNotificationForm').show('slow');
        $("#accordion").show('fold');


    });

    $('#btnBroadCastNotificationGo').click(function (e) {

        LoadBroadCastNotificationDetails();
        CloseBroadCastNotificationDetails();

    });

    $('#btnBroadCastNotificationAdd').click(function (e) {

        $("#accordionBroadcast div").html("");
        $("#accordionBroadcast h3").html(
                "<a href='#' style= 'font-size:.9em;' >Add Broadcast Message Notification Details</a>" +

                '<a href="#" style="float: right;">' +
                '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseBroadCastNotificationDetails();" /></a>' +
                '<span style="float: right;"></span>'
                );

        $("#divbrodcastNotificationForm").load('/QualityMonitoringHelpDesk/CreateQMBroadCastMessageNotification/' + $("#qmTypeBroadNotList").val());
        $('#divbrodcastNotificationForm').show('slow');
        $("#accordionBroadcast").show('fold');

    });

    $("#qmTypeNotList").change(function () {
        if ($("#qmTypeNotList").val() === "S") {
            $('#lblNotificationState').show();
            $('#qmTypeStateList').show();
            $('#qmTypeStateList').val(0);
        }
        else {
            $('#lblNotificationState').hide();
            $('#qmTypeStateList').hide();
            $('#qmTypeStateList').val(0);
        }

    });
    
});