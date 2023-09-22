//$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $(function () {
        $("#dvhdFileUpload").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $.ajax({
        url: "/Master/SearchQualityMonitor/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchQualityMonitor").html(data);
            //$('#btnSearch').trigger('click');
            LoadGrid();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });

    // LoadGrid();

    // $.unblockUI();

    $('#btnCreateNew').click(function (e) {

        //Added By Abhishek kamble 21-Feb-2014        
        $("#dvhdFileUpload").hide("slow");
        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');
        $("#style1").attr("disabled", "disabled");
        $("#style2").attr("disabled", "disabled");
        $("#style3").attr("disabled", "disabled");


        if ($("#dvSearchQualityMonitor").is(":visible")) {
            $('#dvSearchQualityMonitor').hide('slow');
        }

        if (!$("#dvQualityMonitorDetails").is(":visible")) {

            $("#dvQualityMonitorDetails").load("/Master/AddEditMasterQualityMonitor/");

            $('#dvQualityMonitorDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }

    });

    $('#btnSearchView').click(function (e) {

        $('#divBlockedQualityMonitor').hide(); // by pradip
        $('#divQualityMonitor').show()

        //Added By Abhishek kamble 21-feb-2014 start

        $("#dvhdFileUpload").hide("slow");
        $('#btnSearchView').hide();
        $('#btnCreateNew').show();

        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

        //Added By Abhishek kamble 21-feb-2014 end


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvQualityMonitorDetails").is(":visible")) {
            $('#dvQualityMonitorDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }


        if (!$("#dvSearchQualityMonitor").is(":visible")) {

            $('#dvSearchQualityMonitor').load('/Master/SearchQualityMonitor/', function () {

                // $('#tblQualityMonitorList').jqGrid('GridUnload');
                // LoadGrid();

                //alert($("#ADMIN_QM_TYPE").val());

                $("#tblQualityMonitorListDetails").trigger('reloadGrid');


                var data = $('#tblQualityMonitorListDetails').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);


                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                        "#ddlSearchDistrict", "/Master/GetDistrictByStateCode?stateCode=" + $('#ddlSearchStates option:selected').val());

                    $('#ddlSearchDistrict').append("<option value=0>All districts</option>");

                    setTimeout(function () {

                        $('#ddlSearchDistrict').val(data.districtCode);

                    }, 1000);
                    $('#ddlSearchQmTypes').val(data.QmTypeName);
                    $('#ddlSearchEmpanelled').val(data.isEmpanelled);

                }

                $('#dvSearchQualityMonitor').show('slow');


            });
        }
        $.unblockUI();
    });

    $("#spCollapseIconQM").click(function () {
        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

        $("#fileUpload").hide("slow");

    });

    $("#gs_ADMIN_QM_FNAME").attr('placeholder', 'Enter first name');

    // added by Pradip  Patil 29-12-2016  starts
    // To Show Blocked Quality Moniotor List

    $('#btnBlockedView').click(function () {
        $("#dvhdFileUpload").hide("slow");
        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

        if ($("#dvSearchQualityMonitor").is(":visible")) {
            $('#dvSearchQualityMonitor').hide('slow');
        }
        showBlockwdGrid();
        $('#tblBlockedQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { QmTypeName: $('#ddlSearchQmTypes option:selected').val(), stateCode: "0", districtCode: $('#ddlSearchDistrict option:selected').val(), isEmpanelled: "B", firstName: $('#gs_ADMIN_QM_FNAME').val() } });

        // $('#tblBlockedQualityMonitorListDetails').trigger("reloadGrid", [{ page: 1 }]);

        $('#divQualityMonitor').hide();
        $('#divBlockedQualityMonitor').show();
        $('#btnCreateNew').hide();
        $('#btnSearchView').show();
        //}

    })

    function showBlockwdGrid() {

        $('#tblBlockedQualityMonitorListDetails').jqGrid({
            url: '/Master/ShowBlockedQualityMonitirs/',
            datatype: 'json',
            mtype: "POST",

            colNames: ['Image', 'Name', 'State Name', 'Designation', 'Address', 'Mobile', 'Email', 'PAN', ' Adhar', 'DoB', 'Empanelled Month-Year', 'Remarks', 'User Name', 'Type', 'SetColor'],
            colModel: [
                { name: 'image', index: 'image', width: 190, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                { name: 'ADMIN_QM_FNAME', index: 'ADMIN_QM_FNAME', height: 'auto', width: 120, align: "left", valign: "top", sortable: true, search: true },
                { name: 'MAST_STATE_CODE', index: 'MAST_STATE_CODE', height: 'auto', width: 140, align: "left", sortable: true, search: false },
                { name: 'ADMIN_QM_DESG', index: 'ADMIN_QM_DESG', height: 'auto', width: 100, align: "left", sortable: true, search: false },
                { name: 'Address', index: 'Address', height: 'auto', width: 200, align: "left", sortable: true, search: false },
                { name: 'Mobile', index: 'Mobile', height: 'auto', width: 200, align: "left", sortable: true, search: false },
                { name: 'Email', index: 'Email', height: 'auto', width: 190, align: "left", sortable: true, search: false },
                { name: 'PAN', index: 'PAN', height: 'auto', width: 120, align: "left", sortable: true, search: false },
                { name: 'ADMIN_QM_AADHAR_NO', index: 'ADMIN_QM_AADHAR_NO', height: 'auto', width: 120, align: "left", sortable: true, search: false },
                { name: 'ADMIN_QM_BIRTH_DATE', index: 'ADMIN_QM_BIRTH_DATE', height: 'auto', width: 100, align: "left", sortable: true, search: false },
                { name: 'ADMIN_QM_EMPANELLED_YEAR', index: 'ADMIN_QM_EMPANELLED_YEAR', height: 'auto', width: 110, align: "center", sortable: true, search: false },
                { name: 'Remarks', index: 'Remarks', height: 'auto', width: 100, align: "left", sortable: true, search: false },
                { name: 'USER_NAME', index: 'USER_NAME', height: 'auto', width: 110, align: "left", sortable: true, search: false },
                { name: 'ADMIN_QM_TYPE', index: 'ADMIN_QM_TYPE', width: 45, sortable: true, align: "center", search: false },
                { name: 'SetColor', width: 10, sortable: false, resize: false, hidden: true, search: false }
            ],

            pager: jQuery('#divPagerBlockedQualityMonitorDetails'),
            rowNum: 10,
            postData: { QmTypeName: $('#ddlSearchQmTypes option:selected').val(), stateCode: "0", districtCode: $('#ddlSearchDistrict option:selected').val(), isEmpanelled: "B", firstName: $('#gs_ADMIN_QM_FNAME').val() },//added By Pradip Patil 30-12-2016
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_CODE,ADMIN_QM_FNAME',
            sortorder: "asc",
            caption: 'Blocked Quality Monitor List',
            height: 'auto',
            autowidth: true,
            shrinkToFit: true,
            rownumbers: true,
            hidegrid: true,

            loadComplete: function () {
                imagePreview();
                var rows = $("#tblBlockedQualityMonitorListDetails").getDataIDs();
                for (var i = 0; i < rows.length; i++) {
                    var rowData = jQuery('#tblBlockedQualityMonitorListDetails').jqGrid('getRowData', rows[i]);
                    //alert(rowData.SetColor);
                    if (rowData.SetColor == "Y") {
                        $("#tblBlockedQualityMonitorListDetails").jqGrid('setRowData', rows[i], false, { color: 'white', weightfont: 'bold', background: '#FF8282' });
                    }
                }

            },

            loadError: function (xhr, status, error) {

                if (xhr.responseText == "session expired") {

                    alert(xht.responseText);
                    window.location.href = "Login/login";
                }
                else {
                    alert("Invalid Data. Please Check and Try Again!");
                }
            }
        });
        $("#tblBlockedQualityMonitorListDetails").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

    }

    // by Pradip  Patil 29-12-2016  end


});


function LoadGrid() {

    $('#tblQualityMonitorListDetails').jqGrid({
        url: '/Master/GetMasterQualityMonitorList/',
        datatype: 'json',
        mtype: "POST",                                                                                                                                                                                                     //, 'Block'        

        colNames: ['Image', 'Name', 'Cadre State', 'Designation', 'Address', 'Mobile', 'Email', 'PAN', ' Adhar', 'DoB', 'Upload PAN File', 'Empanelled Month-Year', 'Remarks', 'User Name', 'Upload Photo', 'Edit', 'SetColor'],
        colModel: [
            { name: 'image', index: 'image', width: 225, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },//Added By Abhishek kamble To Show Image 27-June-2014
            { name: 'ADMIN_QM_FNAME', index: 'ADMIN_QM_FNAME', height: 'auto', width: 120, align: "left", valign: "top", sortable: true, search: true },
            { name: 'stateName', index: 'MAST_STATE_CODE', height: 'auto', width: 80, align: "left", sortable: true, search: false },
            { name: 'ADMIN_QM_DESG', index: 'ADMIN_QM_DESG', height: 'auto', width: 80, align: "left", sortable: true, search: false },
            { name: 'Address', index: 'Address', height: 'auto', width: 220, align: "left", sortable: true, search: false },
            { name: 'Mobile', index: 'Mobile', height: 'auto', width: 220, align: "left", sortable: true, search: false },
            { name: 'Email', index: 'Email', height: 'auto', width: 220, align: "left", sortable: true, search: false },
            { name: 'PAN', index: 'PAN', height: 'auto', width: 70, align: "left", sortable: true, search: false },
            { name: 'ADMIN_QM_AADHAR_NO', index: 'ADMIN_QM_AADHAR_NO', height: 'auto', width: 70, align: "left", sortable: true, search: false },
            { name: 'ADMIN_QM_BIRTH_DATE', index: 'ADMIN_QM_BIRTH_DATE', height: 'auto', width: 70, align: "left", sortable: true, search: false },
            { name: 'PANFile', index: 'PANFile', height: 'auto', width: 70, align: "left", sortable: true, search: false, hidden: false },  //made hidden on 07-02-2023 by Shreyas as provided in add monitor and edit monitor pages
            //  { name: 'ADMIN_QM_EMPANELLED', index: 'ADMIN_QM_EMPANELLED', height: 'auto', width: 60, align: "center", sortable: false, search: false },
            { name: 'ADMIN_QM_EMPANELLED_YEAR', index: 'ADMIN_QM_EMPANELLED_YEAR', height: 'auto', width: 70, align: "center", sortable: true, search: false },
            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 100, align: "left", sortable: true, search: false },
            //  { name: 'ADMIN_QM_TYPE', index: 'ADMIN_QM_TYPE', width: 45, sortable: true, align: "center", search: false },
            { name: 'USER_NAME', index: 'USER_NAME', height: 'auto', width: 70, align: "left", sortable: true, search: false },
            { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center", hidden: false, search: false },
            { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false, search: false },//Added by DEENDAYAL on 20JUNE2017 Allow SQC to modify SQM details
            { name: 'SetColor', width: 10, sortable: false, resize: false, hidden: true, search: false },
            //  { name: 'Block', index: 'Block', width: 70, sortable: false, align: "center", resize: false, hidden: false, search: false } //by Pradip
        ],

        pager: jQuery('#divPagerQualityMonitorDetails'),
        rowNum: 10,
        postData: { QmTypeName: $('#ddlSearchQmTypes option:selected').val(), stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), isEmpanelled: $('#ddlSearchEmpanelled option:selected').val(), firstName: $('#gs_ADMIN_QM_FNAME').val() },//added By Abhishek kamble 20-feb-2014
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_CODE,ADMIN_QM_FNAME',
        sortorder: "asc",
        caption: 'Quality Monitor List',
        height: 'auto',
        autowidth: true,
        shrinkToFit: true,
        rownumbers: true,
        hidegrid: true,

        loadComplete: function () {
            imagePreview();
            var rows = $("#tblQualityMonitorListDetails").getDataIDs();
            for (var i = 0; i < rows.length; i++) {
                var rowData = jQuery('#tblQualityMonitorListDetails').jqGrid('getRowData', rows[i]);
                //alert(rowData.SetColor);
                if (rowData.SetColor == "Y") {
                    $("#tblQualityMonitorListDetails").jqGrid('setRowData', rows[i], false, { color: 'white', weightfont: 'bold', background: '#FF8282' });
                }


                //CHANGE BY SACHIN ON 28 JULY2020 FOR HIGHLIGHTING NEW DESIGNATION --START HERE
                if (rowData.ADMIN_QM_DESG == "C E (DWO)" || rowData.ADMIN_QM_DESG == "S E (DWO)" || rowData.ADMIN_QM_DESG == "CGM (DWO)" || rowData.ADMIN_QM_DESG == "ENC (DWO)" || rowData.ADMIN_QM_DESG == "EE(QC) DWO") {

                    $("#tblQualityMonitorListDetails").jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#FFDBE9' });
                }
                //--END HERE

            }




            //if ($('#ddlSearchEmpanelled').val() == "N" && $('#roleCodeId').val() == 8) {
            //if ($('#ddlSearchEmpanelled').val() == "N" && $('#roleCodeId').val() != 9) { //Edited on 02-01-2023 by Shreyas-Edit option for De-empanelled monitors will only be available in CQC login
            if ($('#ddlSearchEmpanelled').val() == "N" && $('#roleCodeId').val() == 8) {
                $("#tblQualityMonitorListDetails").hideCol("a");
            }
            else {
                $("#tblQualityMonitorListDetails").showCol("a");//added by deendayal on 06/21/2017 to restrict SQC to edit de-empanelled SQM data
            }

            if ($('#ddlSearchQmTypes option:selected').val() == "I") {  //added by Shreyas on 13-07-2022
                document.getElementById('dvNoteQualityMonitors').innerHTML = "Records will display in red color for the monitors who's age is greater than or equal to 67 or birth date was not entered.";
            }
            else {
                document.getElementById('dvNoteQualityMonitors').innerHTML = "Records will display in red color for the monitors who's age is greater than or equal to 70 or birth date was not entered.";
            }
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again!");
            }
        }
    });
    $("#tblQualityMonitorListDetails").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function FormatRowColor(cellvalue, options, rowObject) {
    // alert("OK" + options.rowId);
    // $("#tblQualityMonitorListDetails").jqGrid('setRowData', options.rowId, false, { background: 'blue' });
    // $("#tblQualityMonitorListDetails").jqGrid('setRowData', options.rowId, false, { color: 'white', weightfont: 'bold', background: 'blue' });
    return cellvalue;
}
//Added By Abhisehk kamble 27-June-2017 To show Image On QM Grid  start
function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black;' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}


this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        $img
            .css("top", (trc_y - $img.height()) + "px")
            .css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 50; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 200px; width: 200px;'  src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");

        //alert(this.href);
    },
        function () {
            this.title = this.t;
            $("#preview").remove();
        }
    )
    //.mousemove(callback);
};

function doNothing() {
    return false;
}
//Added By Abhisehk kamble 27-June-2017 To show Image On QM Grid  end

function FormatColumn(cellvalue, options, rowObject) {
    //Old Code Edit / Delete 27-June-2014 Modified by Abhishke kamble 
    //return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Quality Monitor Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    //Added by DEENDAYAL on 20JUNE2017 Allow SQC to modify SQM details
    //New Code Edit Only
    return "<span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span>";
}

function AddSQMUserLoginDetails(id) {

    //$("#style1").attr("disabled", "disabled");
    //$("#style2").attr("disabled", "disabled");
    //$("#style3").attr("disabled", "disabled");

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/AddSQMUserLoginQualityMonitorDetails/" + id,
        type: "GET",
        //async: false,
        //dataType: "html",
        //catche: false,
        //contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success == true) {
                alert(data.message); //Add Successfully

                $('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { stateCode: $("#MAST_STATE_CODE option:selected").val(), districtCode: "", designationCode: $("#ADMIN_QM_DESG option:selected").val() } });

                $('#tblQualityMonitorListDetails').trigger("reloadGrid");
            }
            else if (data.success == false) {

                alert(data.message);
            }

            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            if ($("#dvSearchQualityMonitor").is(":visible")) {
                $('#dvSearchQualityMonitor').hide('slow');
            }

            alert(xht.responseText);
            $.unblockUI();
        }
    });
}
function editData(id) {

    $("#style1").attr("disabled", "disabled");
    $("#style2").attr("disabled", "disabled");
    $("#style3").attr("disabled", "disabled");

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterQualityMonitor/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($('#dvSearchQualityMonitor').is(":visible")) {
                $('#dvSearchQualityMonitor').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();


            $("#dvQualityMonitorDetails").html(data);
            $("#dvQualityMonitorDetails").show();
            $("#ADMIN_QM_FNAME").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            if ($("#dvSearchQualityMonitor").is(":visible")) {
                $('#dvSearchQualityMonitor').hide('slow');
            }

            alert(xht.responseText);
            $.unblockUI();
        }
    });
}


function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete Quality Monitor details?")) {
        $.ajax({
            url: "/Master/DeleteMasterQualityMonitor/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    //if ($('#dvSearchQualityMonitor').is(':visible')) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $('#tblQualityMonitorListDetails').jqGrid('GridUnload');
                    //    LoadGrid();
                    //}
                    if ($("#dvQualityMonitorDetails").is(":visible")) {
                        $('#dvQualityMonitorDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }

                    if (!$("#dvSearchQualityMonitor").is(":visible")) {
                        $("#dvSearchQualityMonitor").show('slow');
                        $('#tblQualityMonitorListDetails').trigger('reloadGrid');
                    }
                    else {
                        $('#tblQualityMonitorListDetails').trigger('reloadGrid');
                    }
                }
                else {
                    alert(data.message);
                    if ($('#dvSearchQualityMonitor').is(':visible')) {
                        $('#btnSearch').trigger('click');
                    }
                }
                $("#dvQualityMonitorDetails").load("/Master/AddEditMasterQualityMonitor/");


                $.unblockUI();


            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
            }
        });
    }
    else {
        return false;
    }
}


function UploadMasterQMFile(urlParameter) {

    //Added By Abhishek kamble 21-feb-2014
    $("#dvQualityMonitorDetails").hide("slow");

    //$("#divQualityMonitorForm").html("");
    //$("#divQualityMonitorForm").load('/Master/QualityMonitorFileUpload/' + urlParameter, function () {
    //});

    //$('#divQualityMonitorForm').show('fast');


    //$('#dvhdFileUpload').show('fast');
    //$('#dvFileUploadOption').show('fast');

    //$('#spCollapseIconQM').attr('class', 'ui-icon ui-icon-circle-close');


    //$("#tblQualityMonitorList").jqGrid('setGridState', 'hidden');

    //$("#fileUpload").show("fast");

    //alert(urlParameter);

    //$('#tblQualityMonitorList').jqGrid('setSelection', urlParameter);
    jQuery('#tblQualityMonitorListDetails').jqGrid('setSelection', urlParameter);

    $("#dvhdFileUpload div").html("");
    $("#dvhdFileUpload h3").html(
        "<a href='#' style= 'font-size:.9em;' >Image Upload</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeFileUpload();" /></a>' +
        '<span style="float: right;"></span>'
    );


    $('#dvhdFileUpload').show('slow', function () {
        blockPage();
        $("#divQualityMonitorForm").load('/Master/QualityMonitorFileUpload/' + urlParameter, function () {
            unblockPage();
        });
    });

    $('#divQualityMonitorForm').show('slow');
    $("#divQualityMonitorForm").css('height', 'auto');

    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'hidden');


}


function closeFileUpload() {
    $('#dvhdFileUpload').hide('slow');
    $('#divQualityMonitorForm').hide('slow');
    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

    //Added By Abhishek kamble 21-Feb-2014 start       
    if ($("#dvSearchQualityMonitor").is(":hidden")) {

        $("#style1").attr("disabled", "disabled");
        $("#style2").attr("disabled", "disabled");
        $("#style3").attr("disabled", "disabled");
        $("#dvQualityMonitorDetails").show("slow");
    }
    //Added By Abhishek kamble 21-Feb-2014 end


}

function CloseQualityMonitorDetails() {
    $('#accordion').hide('fast');
    $('#divProposalForm').hide('fast');
    $("#tbProposalList").jqGrid('setGridState', 'visible');

    showFilter();
}

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("fast");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}


function UploadQMPAN(qmCode) {

    $("#dvQualityMonitorDetails").hide("slow");
    jQuery('#tblQualityMonitorListDetails').jqGrid('setSelection', qmCode);

    $("#dvhdFileUpload div").html("");
    $("#dvhdFileUpload h3").html(
        "<a href='#' style= 'font-size:.9em;' >Image Upload</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeFileUpload();" /></a>' +
        '<span style="float: right;"></span>'
    );


    $('#dvhdFileUpload').show('slow', function () {
        blockPage();
        $("#divQualityMonitorForm").load('/Master/PANFileUpload/' + qmCode, function () {
            unblockPage();
        });
    });

    $('#divQualityMonitorForm').show('slow');
    $("#divQualityMonitorForm").css('height', 'auto');

    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'hidden');
}

//added by Pradip  Patil 29-12-2016  starts
// Block the selected quality monitor
function blockQualityMonitor(PAN) {
    //alert(PAN);
    if (confirm('Are you sure to blacklist this Quality Monitor?')) {
        var res = confirm('Once the Quality Monitor is blacklisted,It cannot be enabled again.');

        if (res == false) {
            return;
        }
        else {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/BlockMasterQualityMonitor/",
                type: "POST",
                data: { "PAN": PAN },
                async: false,
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.status == true) {
                        alert(data.message);
                        $('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { stateCode: $("#MAST_STATE_CODE option:selected").val(), districtCode: "", designationCode: $("#ADMIN_QM_DESG option:selected").val() } });

                        $('#tblQualityMonitorListDetails').trigger("reloadGrid");
                    }
                    else {
                        alert(data.message);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, throwError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }
    else {
        return;
    }

}

//added by Pradip  Patil 29-12-2016  end




//Following is old server code . and above code is of Anand
////$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//$(document).ready(function () {

//    $(function () {
//        $("#dvhdFileUpload").accordion({
//            icons: false,
//            heightStyle: "content",
//            autoHeight: false
//        });
//    });

//    $.ajax({
//        url: "/Master/SearchQualityMonitor/",
//        type: "GET",
//        dataType: "html",
//        success: function (data) {
//            $("#dvSearchQualityMonitor").html(data);
//            //$('#btnSearch').trigger('click');
//            LoadGrid();

//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            alert(xhr.responseText);
//        }
//    }); 

//   // LoadGrid();

//   // $.unblockUI();

//    $('#btnCreateNew').click(function (e) {

//        //Added By Abhishek kamble 21-Feb-2014        
//        $("#dvhdFileUpload").hide("slow");
//        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');
//        $("#style1").attr("disabled", "disabled");
//        $("#style2").attr("disabled", "disabled");
//        $("#style3").attr("disabled", "disabled");


//        if ($("#dvSearchQualityMonitor").is(":visible")) {
//            $('#dvSearchQualityMonitor').hide('slow');
//        }

//        if (!$("#dvQualityMonitorDetails").is(":visible")) {

//            $("#dvQualityMonitorDetails").load("/Master/AddEditMasterQualityMonitor/");

//            $('#dvQualityMonitorDetails').show('slow');

//            $('#btnCreateNew').hide();            
//            $('#btnSearchView').show();
//        }

//    });

//    $('#btnSearchView').click(function (e) {

//        //Added By Abhishek kamble 21-feb-2014 start

//            $("#dvhdFileUpload").hide("slow");
//            $('#btnSearchView').hide();
//            $('#btnCreateNew').show();

//        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

//        //Added By Abhishek kamble 21-feb-2014 end


//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        if ($("#dvQualityMonitorDetails").is(":visible")) {
//            $('#dvQualityMonitorDetails').hide('slow');

//            $('#btnSearchView').hide();
//            $('#btnCreateNew').show();

//        }


//        if (!$("#dvSearchQualityMonitor").is(":visible")) {

//            $('#dvSearchQualityMonitor').load('/Master/SearchQualityMonitor/', function () {

//               // $('#tblQualityMonitorList').jqGrid('GridUnload');
//                // LoadGrid();

//                //alert($("#ADMIN_QM_TYPE").val());

//                $("#tblQualityMonitorListDetails").trigger('reloadGrid');


//                var data = $('#tblQualityMonitorListDetails').jqGrid("getGridParam", "postData");

//                if (!(data === undefined))
//                {                  

//                    $('#ddlSearchStates').val(data.stateCode);


//                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
//                  "#ddlSearchDistrict", "/Master/GetDistrictByStateCode?stateCode=" + $('#ddlSearchStates option:selected').val());

//                    $('#ddlSearchDistrict').append("<option value=0>All districts</option>");

//                    setTimeout(function () {

//                        $('#ddlSearchDistrict').val(data.districtCode);

//                    }, 1000);
//                    $('#ddlSearchQmTypes').val(data.QmTypeName);
//                    $('#ddlSearchEmpanelled').val(data.isEmpanelled);

//                }

//                $('#dvSearchQualityMonitor').show('slow');


//            });            
//        }
//        $.unblockUI();
//    });

//    $("#spCollapseIconQM").click(function () {
//        $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

//        $("#fileUpload").hide("slow");

//    });

//    $("#gs_ADMIN_QM_FNAME").attr('placeholder', 'Enter first name');
//});


//function LoadGrid()
//{

//    $('#tblQualityMonitorListDetails').jqGrid({
//        url: '/Master/GetMasterQualityMonitorList/',
//        datatype: 'json',
//        mtype: "POST",     

//        colNames: ['Image','Name', 'State Name', 'Designation', 'Address', 'PAN', 'Upload Pdf (PAN)', 'Empanelled', 'Empanelled Year', 'Remarks', 'Type', 'User Name', 'Upload', 'Action'],
//        colModel: [
//         { name: 'image', index: 'image', width: 225, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },//Added By Abhishek kamble To Show Image 27-June-2014
//         { name: 'ADMIN_QM_FNAME', index: 'ADMIN_QM_FNAME', height: 'auto', width: 120, align: "left", valign: "top", sortable: true, search: true },
//         { name: 'MAST_STATE_CODE', index: 'MAST_STATE_CODE', height: 'auto', width: 80, align: "left", sortable: true, search: false },
//         { name: 'ADMIN_QM_DESG', index: 'ADMIN_QM_DESG', height: 'auto', width: 80, align: "left", sortable: true, search: false },
//         { name: 'Address', index: 'Address', height: 'auto', width: 220, align: "left", sortable: true, search: false },
//         { name: 'PAN', index: 'PAN', height: 'auto', width: 70, align: "left", sortable: true, search: false },
//         { name: 'PANFile', index: 'PANFile', height: 'auto', width: 70, align: "left", sortable: true, search: false },
//         { name: 'ADMIN_QM_EMPANELLED', index: 'ADMIN_QM_EMPANELLED', height: 'auto', width: 60, align: "center", sortable: false, search: false },
//         { name: 'ADMIN_QM_EMPANELLED_YEAR', index: 'ADMIN_QM_EMPANELLED_YEAR', height: 'auto', width: 70, align: "center", sortable: true, search: false },
//         { name: 'Remarks', index: 'Remarks', height: 'auto', width: 100, align: "left", sortable: true, search: false },
//         { name: 'ADMIN_QM_TYPE', index: 'ADMIN_QM_TYPE', width: 45, sortable: true, align: "center", search: false },
//         { name: 'USER_NAME', index: 'USER_NAME', height: 'auto', width: 70, align: "left", sortable: true, search: false },
//         { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center", hidden: false, search: false },
//         { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false, search: false }
//        ],

//        pager: jQuery('#divPagerQualityMonitorDetails'),
//        rowNum: 10,
//        postData: { QmTypeName: $('#ddlSearchQmTypes option:selected').val(), stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), isEmpanelled: $('#ddlSearchEmpanelled option:selected').val(), firstName: $('#gs_ADMIN_QM_FNAME').val() },//added By Abhishek kamble 20-feb-2014
//        rowList: [10,15,20,30],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: 'MAST_STATE_CODE,ADMIN_QM_FNAME',
//        sortorder: "asc",
//        caption: 'Quality Monitor List',
//        height: 'auto',
//        autowidth: true,
//        shrinkToFit:true,

//        rownumbers: true,
//        hidegrid: true,

//        loadComplete: function () {
//            imagePreview();            
//        },

//        loadError: function (xhr, status, error) {

//            if (xhr.responseText == "session expired") {

//                alert(xht.responseText);
//                window.location.href = "Login/login";
//            }
//            else {
//                alert("Invalid Data. Please Check and Try Again!");
//            }
//        }
//    });
//    $("#tblQualityMonitorListDetails").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

//}

////Added By Abhisehk kamble 27-June-2017 To show Image On QM Grid  start
//function imageFormatter(cellvalue, options, rowObject) {
//    var PictureURL = cellvalue.replace('/thumbnails', '');

//    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black;' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
//}


//this.imagePreview = function () {

//    /* CONFIG */
//    xOffset = 10;
//    yOffset = 10;
//    // these 2 variable determine popup's distance from the cursor
//    // you might want to adjust to get the right result
//    var Mx = 1000;// $(document).width();
//    var My = 600;// $(document).height();

//    /* END CONFIG */
//    var callback = function (event, param) {
//        var $img = $("#preview");

//        // top-right corner coords' offset
//        var trc_x = xOffset + $img.width();
//        var trc_y = yOffset + $img.height();

//        trc_x = Math.min(trc_x + event.pageX, Mx);
//        trc_y = Math.min(trc_y + event.pageY, My);

//        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

//        $img
//			.css("top", (trc_y - $img.height()) + "px")
//			.css("left", (trc_x - $img.width()) + "px");
//    };


//    $("a.preview").hover(function (e) {


//        Mx = $(this).offset().left + 400; // * 2;//600
//        My = $(this).offset().top - 50; //600;

//        this.t = this.title;
//        this.title = "";
//        var c = (this.t != "") ? "<br/>" + this.t : "";
//        $("body").append("<p id='preview'><img  style='height: 200px; width: 200px;'  src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
//        callback(e, 200);
//        $("#preview").fadeIn("slow");

//        //alert(this.href);
//    },
//		function () {
//		    this.title = this.t;
//		    $("#preview").remove();
//		}
//	)
//    //.mousemove(callback);
//};

//function doNothing() {
//    return false;
//}
////Added By Abhisehk kamble 27-June-2017 To show Image On QM Grid  end

//function FormatColumn(cellvalue, options, rowObject) {
//    //Old Code Edit / Delete 27-June-2014 Modified by Abhishke kamble 
//    //return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Quality Monitor Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

//    //New Code Edit Only
//    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}

//function AddSQMUserLoginDetails(id) {

//    //$("#style1").attr("disabled", "disabled");
//    //$("#style2").attr("disabled", "disabled");
//    //$("#style3").attr("disabled", "disabled");

//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $.ajax({
//        url: "/Master/AddSQMUserLoginQualityMonitorDetails/" + id,
//        type: "GET",
//        //async: false,
//        //dataType: "html",
//        //catche: false,
//        //contentType: "application/json; charset=utf-8",
//        success: function (data) {         
//            if (data.success == true) {
//                alert(data.message); //Add Successfully

//                 $('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { stateCode: $("#MAST_STATE_CODE option:selected").val(), districtCode: "", designationCode: $("#ADMIN_QM_DESG option:selected").val() } });

//                $('#tblQualityMonitorListDetails').trigger("reloadGrid");            
//             }
//            else if (data.success == false) {              

//                alert(data.message);
//            }

//            $.unblockUI();
//        },
//        error: function (xht, ajaxOptions, throwError) {
//            if ($("#dvSearchQualityMonitor").is(":visible")) {
//                $('#dvSearchQualityMonitor').hide('slow');
//            }

//            alert(xht.responseText);
//            $.unblockUI();
//        }
//    });
//}
//function editData(id) {

//    $("#style1").attr("disabled", "disabled");
//    $("#style2").attr("disabled", "disabled");
//    $("#style3").attr("disabled", "disabled");

//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $.ajax({
//        url: "/Master/EditMasterQualityMonitor/" + id,
//        type: "GET",
//        async: false,
//        dataType: "html",
//        catche:false,
//        contentType: "application/json; charset=utf-8",
//        success: function (data)
//        {
//            if ($('#dvSearchQualityMonitor').is(":visible"))
//            {
//                $('#dvSearchQualityMonitor').hide('slow');
//            }                
//            $('#btnCreateNew').hide();
//            $('#btnSearchView').show();


//            $("#dvQualityMonitorDetails").html(data);
//            $("#dvQualityMonitorDetails").show();
//            $("#ADMIN_QM_FNAME").focus();
//            $.unblockUI();
//        },
//        error: function (xht, ajaxOptions, throwError)
//        {
//            if ($("#dvSearchQualityMonitor").is(":visible")) {
//                $('#dvSearchQualityMonitor').hide('slow');
//            }

//            alert(xht.responseText);
//            $.unblockUI();
//        }
//    });
//}


//function deleteData(urlParam)
//{  
//    if (confirm("Are you sure you want to delete Quality Monitor details?")) {
//        $.ajax({
//            url: "/Master/DeleteMasterQualityMonitor/" + urlParam,
//            type: "POST",
//            dataType: "json",
//            success: function (data) {

//                if (data.success) {
//                    alert(data.message);
//                    //if ($('#dvSearchQualityMonitor').is(':visible')) {
//                    //    $('#btnSearch').trigger('click');
//                    //}
//                    //else {
//                    //    $('#tblQualityMonitorListDetails').jqGrid('GridUnload');
//                    //    LoadGrid();
//                    //}
//                    if ($("#dvQualityMonitorDetails").is(":visible")) {
//                        $('#dvQualityMonitorDetails').hide('slow');
//                        $('#btnSearchView').hide();
//                        $('#btnCreateNew').show();

//                    }

//                    if (!$("#dvSearchQualityMonitor").is(":visible")) {
//                        $("#dvSearchQualityMonitor").show('slow');
//                        $('#tblQualityMonitorListDetails').trigger('reloadGrid');
//                    }
//                    else {
//                        $('#tblQualityMonitorListDetails').trigger('reloadGrid');
//                    }
//                }
//                else {
//                    alert(data.message);
//                    if ($('#dvSearchQualityMonitor').is(':visible')) {
//                        $('#btnSearch').trigger('click');
//                    }
//                }
//                $("#dvQualityMonitorDetails").load("/Master/AddEditMasterQualityMonitor/");


//                $.unblockUI();


//            },
//            error: function (xht, ajaxOptions, throwError)
//            {
//                alert(xht.responseText);
//            }
//        });
//    }
//    else {
//        return false;
//    }
//}


//function UploadMasterQMFile(urlParameter) {

//    //Added By Abhishek kamble 21-feb-2014
//    $("#dvQualityMonitorDetails").hide("slow");

//    //$("#divQualityMonitorForm").html("");
//    //$("#divQualityMonitorForm").load('/Master/QualityMonitorFileUpload/' + urlParameter, function () {
//    //});

//    //$('#divQualityMonitorForm').show('fast');


//    //$('#dvhdFileUpload').show('fast');
//    //$('#dvFileUploadOption').show('fast');

//    //$('#spCollapseIconQM').attr('class', 'ui-icon ui-icon-circle-close');


//    //$("#tblQualityMonitorList").jqGrid('setGridState', 'hidden');

//    //$("#fileUpload").show("fast");

//    //alert(urlParameter);

//    //$('#tblQualityMonitorList').jqGrid('setSelection', urlParameter);
//    jQuery('#tblQualityMonitorListDetails').jqGrid('setSelection', urlParameter);

//    $("#dvhdFileUpload div").html("");
//    $("#dvhdFileUpload h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Image Upload</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeFileUpload();" /></a>' +
//            '<span style="float: right;"></span>'
//            );


//    $('#dvhdFileUpload').show('slow', function () {
//        blockPage();
//        $("#divQualityMonitorForm").load('/Master/QualityMonitorFileUpload/' + urlParameter, function () {
//            unblockPage();
//        });
//    });

//    $('#divQualityMonitorForm').show('slow');
//    $("#divQualityMonitorForm").css('height', 'auto');

//    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'hidden');


//}


//function closeFileUpload() {
//    $('#dvhdFileUpload').hide('slow');
//    $('#divQualityMonitorForm').hide('slow');
//    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'visible');

//    //Added By Abhishek kamble 21-Feb-2014 start       
//    if ($("#dvSearchQualityMonitor").is(":hidden"))
//    {

//        $("#style1").attr("disabled", "disabled");
//        $("#style2").attr("disabled", "disabled");
//        $("#style3").attr("disabled", "disabled");
//        $("#dvQualityMonitorDetails").show("slow");
//    }
//    //Added By Abhishek kamble 21-Feb-2014 end


//}

//function CloseQualityMonitorDetails() {
//    $('#accordion').hide('fast');
//    $('#divProposalForm').hide('fast');
//    $("#tbProposalList").jqGrid('setGridState', 'visible');

//    showFilter();
//}

//function showFilter() {
//    if ($('#divFilterForm').is(":hidden")) {
//        $("#divFilterForm").show("fast");
//        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
//    }
//}


//function UploadQMPAN(qmCode) {

//    $("#dvQualityMonitorDetails").hide("slow");
//    jQuery('#tblQualityMonitorListDetails').jqGrid('setSelection', qmCode);

//    $("#dvhdFileUpload div").html("");
//    $("#dvhdFileUpload h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Image Upload</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeFileUpload();" /></a>' +
//            '<span style="float: right;"></span>'
//            );


//    $('#dvhdFileUpload').show('slow', function () {
//        blockPage();
//        $("#divQualityMonitorForm").load('/Master/PANFileUpload/' + qmCode, function () {
//            unblockPage();
//        });
//    });

//    $('#divQualityMonitorForm').show('slow');
//    $("#divQualityMonitorForm").css('height', 'auto');

//    $("#tblQualityMonitorListDetails").jqGrid('setGridState', 'hidden');
//}

function DownloadQMPAN(value) {  //To download PAN file uploaded on click

    var url = "/Master/DownloadPANFile/" + value;
    downloadFileFromAction(url);
    //return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}