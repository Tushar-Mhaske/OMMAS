var DistList;
DistList = $("#multiDist").val();
var DistListJoined;

$(document).ready(function () {

    
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $("#multiDist").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#multiDist").multiselect("uncheckAll");


    $("#btnListProposal").click(function () {

        if ($("#frmFilterMaintenanceProposal").valid()) {
            LoadProposalList();
        }
    });


    //$('#ddlStates').change(function () {
    //    $("#multiDist").empty();
    //    window.location.href = "/MaintenanceAgreement/MordEmargRoadListTwo?stateCode=" + $("#ddlStates").val()
    //});

    //$('#btnSearch1').click(function (e) {
    //    if ($("#ddlStates option:selected").val() <= 0)
    //    {
    //        alert("Please Select State.")

    //    }
    //    else
    //    {
    //        LoadCompletedRoads();
    //    }

    //   // SearchDetails();
    //});

    $('#btnSearch').trigger('click');

    //$("#ddlFinancialYears").change(function () {

    //    FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
    //                "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    //}); //end function block change

    //$("#ddlBlocks").change(function () {

    //    FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
    //                "#ddlPackages", "/Agreement/PopulateDistrictsForEmargListAtMord?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    //}); //end function block change


    //$('#multiDist').change(function () {
    //    AutoPopoulatePackage();
    //});

});








function FillInCascadeDropdown1(map, dropdown, action) {
    //alert("IM")
    debugger;
    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()


// to load execution details

function LoadProposalList() {

    DistList = $("#multiDist").val();
    DistListJoined = DistList.join();

    //if ($("#ddlBlocks option:selected").val() == 0 || $("#ddlBlocks option:selected").val() == -1) {
    //    alert("Please Select District")
    //    return null
    //}
    $("#tblstProposalRepackage").jqGrid('GridUnload');

    jQuery("#tblstProposalRepackage").jqGrid({
        url: '/MaintenanceAgreement/MordGetEmargFinalList',//MordGetEmargFinalList
        datatype: "json",
        mtype: "POST",
        postData: { value: Math.random,  DistListID: DistListJoined },
        //postData: { blockCode: $("#ddlBlocks option:selected").val(), StateCode: $("#ddlStates option:selected").val(), BatchCode: $("#ddlBatchs option:selected").val(), Package: $("#ddlPackages option:selected").val(), Collaboration: $("#ddlCollaborations option:selected").val(), ProposalType: $("#ddlProposalTypes option:selected").val(), UpgradationType: $("#ddlUpgradationTypes option:selected").val() },
        colNames: ['State Name', 'District Name', 'Block Name', 'Old Package No.', 'Current Package No.', 'Road Name', 'Repackage', 'Finalize'],
        colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 120, align: "center", search: false },
                         { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 120, align: "center", search: false },

                       { name: 'OLD_PACKAGE_NO', index: 'OLD_PACKAGE_NO', height: 'auto', width: 150, align: "center", search: false },
                        { name: 'PACKAGE_NO', index: 'PACKAGE_NO', height: 'auto', width: 100, align: "center", search: false },
                        { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                     //   { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 100, align: "center", search: false },

                        { name: 'a', index: 'a', height: 'auto', width: 120, align: "center", search: false },
                         { name: 'b', index: 'b', height: 'auto', width: 120, align: "center", search: false },
        ],
        pager: jQuery('#dvlstPagerProposalRepackage'),
        rowNum: 5000,
        //  rowNum: 15,
        rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
        // rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ROAD_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Emarg Works for Repackaging",
        hidegrid: true,
        height: 'auto',
        cmTemplate: { title: false },
        width: 'auto',
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ["PACKAGE_NO"],
            groupColumnShow: [false],
            groupText: [


                "<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all Repackaged roads in this Package, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackageForRepackage(\'' + "{0}" + '\')">'


                //  + "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' 

            ],
            groupOrder: ["asc"],
            groupSummary: [true],
            groupSummaryPos: ['header'],
            groupCollapse: false
        },
        loadComplete: function () { },
        loadError: function () { }
    });
}

function FinalizeEmargPackageForRepackage(paramData) {
    if (confirm("Are you sure to finalize Package Details ?")) {
        $.ajax({
            url: '/MaintenanceAgreement/FinalizePackageEmargForRepackaing',
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
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
                //  LoadCompletedRoads();
                $("#tblstProposalRepackage").trigger('reloadGrid');
                if (response.success) {
                    unblockPage();
                    alert(response.message);
                }
                else {
                    alert(response.message);
                    unblockPage();
                }
            }
        });
    }
}


function FinalizeEmargRepackagingDetails(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' these details ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/MaintenanceAgreement/FinalizeRepakageForEmarg/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblstProposalRepackage").trigger('reloadGrid');


                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}

function AddEmargRepackagingDetails(IMS_PR_ROAD_CODE) {


    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Maintenance Repackaging Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddPackage").load("/MaintenanceAgreement/AddEmargMaintenanceRepackagingDetails?" + $.param({ ProposalCode: IMS_PR_ROAD_CODE }), function () {
            $.validator.unobtrusive.parse($('#divAddPackage'));
            unblockPage();
        });

        $('#divAddPackage').show('slow');
        $("#divAddPackage").css('height', 'auto');
        $("#tblstProposalRepackage").jqGrid('setGridState', 'hidden');
    });
}


function CloseDetails() {
    $("#tblstProposalRepackage").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}



//function LoadCompletedRoads() {

//  //  var DistList;
//    DistList = $("#multiDist").val();
//    DistListJoined = DistList.join();

//    $("#tbProposedRoadList").jqGrid("GridUnload");

//    blockPage();

//    jQuery("#tbProposedRoadList").jqGrid({
//        url: '/MaintenanceAgreement/MordGetEmargFinalList',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ['Road Name', 'Package ID', 'State Name', 'Disrtict Name','Sanctioned Length (Kms)', "Edit",'Finalize Road','Status','Definalize Road','System Rejected / PIU Correction Request','View','Push to Emarg'],
//        colModel: [
//                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 200, sortable: false, align: "center" },
//                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 250, sortable: false, align: "left" },
                           
//                            { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, sortable: false, align: "center" },
//                            { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', height: 'auto', width: 80, sortable: false, align: "center" },
//                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "center" },

//                           // { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },

//                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", sortable: false },
//                            { name: 'b', width: 150, sortable: false, resize: false, align: "center", sortable: false },
//                            { name: 'c', width: 150, sortable: false, resize: false, align: "center", sortable: false },
//                            { name: 'f', width: 150, sortable: false, resize: false, align: "center", sortable: false },// Road Definalization
//                            // REJECTION_REASON
//                            { name: 'REJECTION_REASON', index: 'REJECTION_REASON', width: 150, sortable: false, align: "center" },
//                            { name: 'd', width: 150, sortable: false, resize: false, align: "center", sortable: false },
//                            { name: 'e', width: 50, sortable: false, resize: false, align: "center", sortable: false , hidden : true}



//        ],
//        postData: { IMS_STATE: $("#ddlStates option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val(), DistListID: DistListJoined },
//        pager: jQuery('#dvProposedRoadListPager'),
//        rowNum: 25000,
//        rowList: [50,100,150,200,300,400,500,600,700,800,1000],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Emarg Road Details.",
//        height: 'auto',
//        width: 'auto',
//        // sortname: 'RoadName',
//        //autowidth: true,
//        rownumbers: true,
//        grouping: true,
//        groupingView: {
//            groupField: ["IMS_PACKAGE_ID"],
//            groupColumnShow: [false],
//            groupText: [

            
//                "<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'
//                  + "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"

//                //"<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px;' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

//            ],
//            groupOrder: ["asc"],
//            groupSummary: [true],
//            groupSummaryPos: ['header'],
//            groupCollapse: false
//        },
//        loadComplete: function (data) {

            



//            unblockPage();
//        },
//        loadError: function (xhr, status, error) {
//            unblockPage();
//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//        }
//    }); //end of grid
//}



//function FormatColumn1(cellvalue, options, rowObject) {


//    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Click here to finalize Road Details' onClick ='finalizeRoadDetailsForEmarg(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}


//function FormatColumn(cellvalue, options, rowObject) {


//    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Click here for correction of Emarg Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}

//FinalizeEmargPackage


//function FinalizeEmargPackage(urlparameter) {

//    alert("Ckicked")

//    if (confirm("Are you sure you want to 'Finalize' Package details ?")) {

//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        $.ajax({
//            url: "/MaintenanceAgreement/FinalizePackageEmargCorrection/" + urlparameter,
//            type: "POST",
//            dataType: "json",
//            success: function (data) {
//                console.log(data);
//                if (data.success) {
//                    LoadCompletedRoads();
//                    // $("#tbProposedRoadList").trigger('reloadGrid');
//                    $.unblockUI();
//                    alert(data.message);
//                }
//                else {
//                    alert(data.message);
//                }
//                $.unblockUI();
//            },
//            error: function (xht, ajaxOptions, throwError) {
//                alert(xht.responseText);
//                $.unblockUI();
//            }

//        });
//    }
//    else {
//        return false;
//    }

//}

//function FinalizeEmargPackage(paramData) {
//    if (confirm("Are you sure to finalize Package Details ?")) {
//        $.ajax({
//            url: '/MaintenanceAgreement/FinalizePackageEmargCorrection',
//            type: "POST",
//            cache: false,
//            data: { "Data": paramData, value: Math.random },
//            beforeSend: function () {
//                blockPage();
//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            },
//            success: function (response)
//            {
//                unblockPage();
//                LoadCompletedRoads();

//                if (response.success)
//                {
//                    unblockPage();
//                    alert(response.message);
//                }
//                else
//                {
//                    alert(response.message);
//                    unblockPage();
//                }
//            }
//        });
//    }
//}

//function DeleteNonPackageFinalizedRoad(urlparameter) {

//    if (confirm("Are you sure you want to definalize Road details?")) {
//        $.ajax({
//            type: 'POST',
//            url: '/MaintenanceAgreement/DeleteRoadBeforePackageFinalization/' + urlparameter,
//            dataType: 'json',
//            async: false,
//            cache: false,
//            success: function (data) {
//                if (data.success == true) {
//                    alert("Road details deleted successfully");
//                    LoadCompletedRoads();
//                    //$("#tbFinancialList").trigger('reloadGrid');
//                    //$("#divAddFinancialProgress").html('');
//                }
//                else if (data.success == false) {
//                    alert("Road details can not be deleted.");
//                }
//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                //alert(xhr.responseText);
//            }
//        });
//    }
//    else {
//        return false;
//    }

//}





//function PushToEmarg(paramData) {
//    if (confirm("Are you sure to push Package Details to Emarg ?")) {
//        $.ajax({
//            url: '/MaintenanceAgreement/PushToEmargDetails',
//            type: "POST",
//            cache: false,
//            data: { "Data": paramData, value: Math.random },
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
//                LoadCompletedRoads();

//                if (response.success) {

//                    unblockPage();
//                    alert(response.message);
//                }
//                else {
//                    alert(response.message);
//                    unblockPage();
//                }
//            }
//        });
//    }
//}


// PushToEmarg

//function PushToEmarg(urlparameter) {

//    if (confirm("Are you sure you want to push this Package details to Emarg?")) {

//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        $.ajax({
//            url: "/MaintenanceAgreement/PushToEmargDetails/" + urlparameter,
//            type: "POST",
//            dataType: "json",
//            success: function (data) {
//                console.log(data);
//                if (data.success) {
//                    LoadCompletedRoads();
//                    // $("#tbProposedRoadList").trigger('reloadGrid');
//                    $.unblockUI();
//                    alert(data.message);
//                }
//                else {
//                    alert(data.message);
//                }
//                $.unblockUI();
//            },
//            error: function (xht, ajaxOptions, throwError) {
//                alert(xht.responseText);
//                $.unblockUI();
//            }

//        });
//    }
//    else {
//        return false;
//    }

//}

//function FinalizeEmargCorrectionRoad(urlparameter) {

//    if (confirm("Are you sure you want to 'Finalize' Road details ?")) {

//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        $.ajax({
//            url: "/MaintenanceAgreement/FinalizeRoadAfterEmargCorrection/" + urlparameter,
//            type: "POST",
//            dataType: "json",
//            success: function (data)
//            {
//                console.log(data);
//                if (data.success)
//                {
//                    LoadCompletedRoads();
//                   // $("#tbProposedRoadList").trigger('reloadGrid');
//                    $.unblockUI();
//                    alert(data.message);
//                }
//                else
//                {
//                    alert(data.message);
//                }
//                $.unblockUI();
//            },
//            error: function (xht, ajaxOptions, throwError) {
//                alert(xht.responseText);
//                $.unblockUI();
//            }

//        });
//    }
//    else {
//        return false;
//    }

//}


//function AddorEditEmargCorrectionDetails(urlparameter) {
     
//    //alert("Here")
//  //  window.scrollTo(0, 0);

//    $("#btnCreateNew").hide();
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//    $.ajax({
//        type: 'GET',
//        url: '/MaintenanceAgreement/EditEmarg/' + urlparameter,
//        dataType: "html",
//        async: false,
//        cache: false,
//        success: function (data)
//        {
//            if(data == null||data=='')
//            {
//                alert("Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details. Then only proceed with these Correction Details.")
//            }


//            //if ($("#dvSearchParameter").is(":visible")) {

//            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

//            //    //$(this).next("#dvSearchParameter").slideToggle(300);
//            //    $("#dvSearchParameter").slideToggle(300);
//            //}

//            //else {
//            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

//            //    //$(this).next("#dvSearchParameter").slideToggle(300);
//            //    $("#dvSearchParameter").slideToggle(300);
//            //}


//                $("#emargUpdateForm").show('slow');
//                $("#emargUpdateForm").html(data);

//                $.unblockUI();
            
//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            $.unblockUI();
//            alert("Error occurred while processing your request.");
//            return false;
//        }

//    })
//}
//// ViewDetailsAfterCorrection


//function ViewDetailsAfterCorrection(urlparameter) {

//    $("#btnCreateNew").hide();
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//    $.ajax({
//        type: 'GET',
//        url: '/MaintenanceAgreement/ViewEditEmarg/' + urlparameter,
//        dataType: "html",
//        async: false,
//        cache: false,
//        success: function (data) {
//            if (data == null || data == '') {
//                alert("Details are not available for selected Road.")
//            }

//            $("#emargUpdateForm").show('slow');
//            $("#emargUpdateForm").html(data);

//            $.unblockUI();

//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            $.unblockUI();
//            alert("Error occurred while processing your request.");
//            return false;
//        }

//    })
//}


//function SearchDetails() {

//    $('#tbProposedRoadList').setGridParam({
//        url: '/MaintenanceAgreement/GetCompletedRoadList',
//        datatype: 'json'
//    });

//    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
//    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

//}

//function FormatColumnView(cellvalue, options, rowObject) {

//    // return "<center><table><tr><td  style='border-color:white'><a href='#' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");' >View</a></td></tr></table></center>";

//    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Maintenance Agreement' onClick ='ViewMaintenanceAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
//}

//function ViewMaintenanceAgreementDetails(parameter) {

//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//    $.ajax({
//        url: "/MaintenanceAgreement/AddMaintenanceAgreementAgainstRoad/" + parameter,
//        type: "GET",
//        async: false,
//        cache: false,

//        success: function (data) {

//            $("#dvAddMaintenanceAgreementAgainstRoad").html(data);
//            $('#accordion').show('slow');
//            $('#dvAddMaintenanceAgreementAgainstRoad').show('slow');

//            if ($("#dvSearchProposedRoad").is(":visible")) {
//                $('#dvSearchProposedRoad').hide('slow');
//            }
//            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
//            $.unblockUI();


//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            alert(xhr.responseText);
//            $.unblockUI();
//        }

//    });


//}



//function AutoPopoulatePackage() {


   
//    DistList = $("#multiDist").val();

//   // alert(DistList)
//    DistListJoined = DistList.join();
//    //alert(DistListJoined)
   


//    if ($("#multiDist option:selected").val() != "") {
//        $.ajax({
//            url: "/MaintenanceAgreement/PopulateAutoPackages?id=" + DistListJoined,
//            cache: false,
//            type: "GET",
//            async: false,
//           // data: { DistListID1: DistListJoined },
//            success: function (data) {

//                var rows = new Array();
//                for (var i = 0; i < data.length; i++) {
//                    rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
//                }

//                $('#multiPackage').autocomplete({
//                    source: rows,
//                    dataType: 'json',
//                    formatItem: function (row, i, n) {
//                        return row.Text;
//                    },
//                    width: 150,
//                    highlight: true,
//                    minChars: 3,
//                    selectFirst: true,
//                    max: 10,
//                    scroll: true,
//                    width: 100,
//                    maxItemsToShow: 10,
//                    maxCacheLength: 10,
//                    mustMatch: true
//                })

//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                //alert("An error occurred while executing this request.\n" + xhr.responseText);
//                if (xhr.responseText == "session expired") {
//                    //$('#frmECApplication').submit();
//                    //  alert(xhr.responseText);
//                    alert('An Error occurred while processing your request');
//                    window.location.href = "/Login/LogIn";
//                }
//            }
//        })
//    }
//}