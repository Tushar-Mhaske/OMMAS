
$(document).ready(function () {

    LoadProposedRoads();

    //for expand and collpase Document Details 
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


    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');


    $("#ddlFinancialYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#dvSplitCount").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Split Count'
    });

    $.unblockUI();
});

function FillInCascadeDropdown(map, dropdown, action) {

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


function LoadProposedRoads() {

    jQuery("#tbProposedRoadList").jqGrid({
        // url: '/Agreement/GetProposedRoadList',
        url: '/SplitWork/GetProposedRoadList',
        datatype: "local",
        mtype: "POST",
        colNames: ['Block', 'Year', 'Batch', 'Package', 'Work', 'Work Type', 'Road Length', 'Funding Agency', 'Sanctioned Cost', 'Maintenance Cost', 'Agreement Cost', 'Split'],
        colModel: [
                            { name: 'Block', index: 'Block', width: 10, sortable: true, align: 'center' },
                            { name: 'SanctionedYear', index: 'SanctionedYear', height: 'auto', width: 10, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 10, sortable: true, align: "center" },
                            { name: 'Package', index: 'Package', width: 10, sortable: true, align: "center" },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 22, align: "left", sortable: true },
                            { name: 'WorkType', index: 'WorkType', height: 'auto', width: 10, align: "center", sortable: true },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 10, sortable: true, align: "left" },
                            { name: 'Collaboration', index: 'Collaboration', width: 15, sortable: true },
                            { name: 'SanctionedCost', index: 'SanctionedCost', width: 13, sortable: false, align: "right" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 13, sortable: false, align: "right" },
                            { name: 'AgreementCost', index: 'AgreementCost', width: 13, sortable: false, align: "right" },
                            { name: 'View', width: 7, sortable: false, resize: false, /*formatter: FormatColumnSplit,*/ align: "center", title: false }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvProposedRoadListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Sanctioned Work List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'SanctionedYear,Package,RoadName',
        sortorder: "asc",
        loadComplete: function () {

            var reccount = $('#tbProposedRoadList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvProposedRoadListPager_left').html('[<b> Note</b>: 1. All Amounts are in Lakhs. 2.All Lengths are in Kms ]');
            }


        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}

function SearchDetails() {

    $('#tbProposedRoadList').setGridParam({
        // url: '/Agreement/GetProposedRoadList',
        url: '/SplitWork/GetProposedRoadList',
        datatype: 'json'
    });

    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

}

function FormatColumnSplit(cellvalue, options, rowObject) {
   //alert(cellvalue.toString());
    return "<center><table><tr><td  style='border-color:white'><a href='#' title='Split Work' onClick ='SplitWork(\"" + cellvalue.toString() + "\");' >Split</a></td></tr></table></center>";

    //return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
}

function SplitWork(parameter) {

    //alert(parameter);
    //var isAgreementDone = false;
    //var isSplitWorkDone = false;

    $('#EncryptedRoadCode').val(parameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/SplitWork/CheckAgreementExist/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) { 
            //isAgreementDone = data.exist; 
           // isAgreementDone = data.isAgreementExist;

            if (data.isSplitWorkExist == true && data.isAgreementExist == true && data.isSplitCountExist == true) {
                $.ajax({
                    url: "/SplitWork/AddSplitWorkDetails/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {

                        $("#dvAddSplitWork").html(data);
                        $('#accordion').show('slow');
                        $('#dvAddSplitWork').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }
                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        if (data.isAgreementExist == true) {
                            $('#tblCreateNewSplitWork').hide();
                            $('#tblCreateNewSplitWork').empty();
                        }


                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });
            }
            else if (data.isSplitWorkExist == true && data.isAgreementExist == false && data.isSplitCountExist == true) {
                $.ajax({
                    url: "/SplitWork/AddSplitWorkDetails/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {

                        $("#dvAddSplitWork").html(data);
                        $('#accordion').show('slow');
                        $('#dvAddSplitWork').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }
                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        //if (isAgreementDone == true) {
                        //    $('#tblCreateNewSplitWork').hide();
                        //    $('#tblCreateNewSplitWork').empty();
                        //}


                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });
            }

            else if (data.isSplitWorkExist == false && data.isAgreementExist == false && data.isSplitCountExist == true) {

                $.ajax({
                    url: "/SplitWork/AddSplitWorkDetails/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {

                        $("#dvAddSplitWork").html(data);
                        $('#accordion').show('slow');
                        $('#dvAddSplitWork').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }
                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        //if (isAgreementDone == true) {
                        //    $('#tblCreateNewSplitWork').hide();
                        //    $('#tblCreateNewSplitWork').empty();
                        //}


                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });

            }
            else if (data.isSplitWorkExist == false && data.isAgreementExist == true) {

                //if ($('#accordion').is(':visible')) {
                //    $('#accordion').hide('slow');
                //}

                //alert('Agreement has been done for selected road, so you can not split this road.');

                ////new change done by Vikram on 27 Dec 2013
                $.ajax({
                    url: "/SplitWork/AddSplitWorkDetails/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data1) {

                        $("#dvAddSplitWork").html(data1);
                        $('#accordion').show('slow');
                        $('#dvAddSplitWork').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }
                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        if (data.isAgreementExist == true) {
                            $('#tblCreateNewSplitWork').hide();
                            $('#tblCreateNewSplitWork').empty();
                            //$('#tblCreateNewSplitWork').text('Agreement has been done for selected road, so you can not split this road.');
                        }


                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });

                ////end of change

            }
            else if (data.isSplitWorkExist == false && data.isAgreementExist == false && data.isSplitCountExist == false) {

                $("#dvSplitCount").load('/SplitWork/SplitCount/' + parameter, function () {

                    $("#dvSplitCount").dialog('open');
                    //$("#dvSplitCount").dialog('open');
                });

            }
            else if (data.isSplitWorkExist == true && data.isAgreementExist == true && data.isSplitCountExist == false) {

              
                $.ajax({
                    url: "/SplitWork/AddSplitWorkDetails/" + parameter,
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {

                        $("#dvAddSplitWork").html(data);
                        $('#accordion').show('slow');
                        $('#dvAddSplitWork').show('slow');

                        if ($("#dvSearchProposedRoad").is(":visible")) {
                            $('#dvSearchProposedRoad').hide('slow');
                        }
                        $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

                        if (data.isAgreementExist == true) {
                            $('#tblCreateNewSplitWork').hide();
                            $('#tblCreateNewSplitWork').empty();
                        }


                        $.unblockUI();

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }

                });
            }


            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });






    
   

   


}