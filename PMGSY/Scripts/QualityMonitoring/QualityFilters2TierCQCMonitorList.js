$(document).ready(function () {





    $('#btnSearch').click(function (e) {

        LoadGrid();
        //$('#tb2TierMonitorList').setGridParam({
        //    url: '/QualityMonitoring/GetMasterQualityMonitorList', datatype: 'json'
        //});

        //$('#tb2TierMonitorList').jqGrid("setGridParam", { "postData": { QmTypeName: "S", stateCode: $('#ddlSearchStates').val(), isEmpanelled: $('#ddlSearchEmpanelled').val(), firstName: $('#gs_ADMIN_QM_FNAME').val() } });

        //$('#tb2TierMonitorList').trigger("reloadGrid", [{ page: 1 }]);
    });
});


function LoadGrid() {

    $("#tb2TierMonitorList").jqGrid('GridUnload');
    $('#tb2TierMonitorList').jqGrid({
        url: '/QualityMonitoring/ListQualityMonitors/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Image', 'Name', 'State Name', 'Designation', 'Address', 'PAN', 'Empanelled', 'Empanelled Year', 'Remarks', 'Type', 'User Name', 'Edit'],
        colModel: [
         { name: 'image', index: 'image', width: 225, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
         { name: 'ADMIN_QM_FNAME', index: 'ADMIN_QM_FNAME', height: 'auto', width: 120, align: "left", valign: "top", sortable: true, search: true },
         { name: 'MAST_STATE_CODE', index: 'MAST_STATE_CODE', height: 'auto', width: 80, align: "left", sortable: true, search: false },
         { name: 'ADMIN_QM_DESG', index: 'ADMIN_QM_DESG', height: 'auto', width: 80, align: "left", sortable: true, search: false },
         { name: 'Address', index: 'Address', height: 'auto', width: 220, align: "left", sortable: true, search: false },
         { name: 'PAN', index: 'PAN', height: 'auto', width: 70, align: "left", sortable: true, search: false },
         { name: 'ADMIN_QM_EMPANELLED', index: 'ADMIN_QM_EMPANELLED', height: 'auto', width: 60, align: "center", sortable: false, search: false },
         { name: 'ADMIN_QM_EMPANELLED_YEAR', index: 'ADMIN_QM_EMPANELLED_YEAR', height: 'auto', width: 70, align: "center", sortable: true, search: false },
         { name: 'Remarks', index: 'Remarks', height: 'auto', width: 100, align: "left", sortable: true, search: false },
         { name: 'ADMIN_QM_TYPE', index: 'ADMIN_QM_TYPE', width: 45, sortable: true, align: "center", search: false },
         { name: 'USER_NAME', index: 'USER_NAME', height: 'auto', width: 70, align: "left", sortable: true, search: false },
         {
             name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false, search: false
             //, hidden: parseInt($('#roleCode').val()) == 8 ? true : false
         },
        ],

        pager: jQuery('#div2TierMonitorListPager'),
        rowNum: 500,
        postData: { QmTypeName: "S", stateCode: $('#ddlSearchStates').val(), isEmpanelled: $('#ddlSearchEmpanelled').val(), firstName: $('#gs_ADMIN_QM_FNAME').val() },
        rowList: [50, 100, 200],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_CODE, ADMIN_QM_FNAME',
        sortorder: "asc",
        caption: 'Quality Monitor List',
        height: 'auto',
        autowidth: true,
        shrinkToFit: true,
        rownumbers: true,
        hidegrid: true,

        loadComplete: function () {
            imagePreview();
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
    $("#tb2TierMonitorList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function FormatColumn(cellvalue, options, rowObject) {
    //Old Code Edit / Delete 27-June-2014 Modified by Abhishke kamble 
    //return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Quality Monitor Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    //New Code Edit Only
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Quality Monitor Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black;' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
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