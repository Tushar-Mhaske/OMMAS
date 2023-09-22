
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $('#dvSearchMPConstituency').load('/LocationMasterDataEntry/SearchMPConstituency');

    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchMPConstituency").is(":visible")) {
            $('#dvSearchMPConstituency').hide('slow');
        }

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
        }

        if (!$("#dvMPConstituencyDetails").is(":visible")) {
            $('#dvMPConstituencyDetails').load("/LocationMasterDataEntry/CreateMPConstituency");
            $('#dvMPConstituencyDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }

        $('#tbMPConstituencyList').jqGrid("setGridState", "visible");

    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
        }


        $('#dvMPConstituencyDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();

        //if ($("#dvMPConstituencyDetails").is(":visible")) {
        //    $('#dvMPConstituencyDetails').hide('slow');

        //    $('#btnSearchView').hide();
        //    $('#btnCreateNew').show();

        //}

        $('#tbMPConstituencyList').jqGrid("setGridState", "visible");

        if (!$("#dvSearchMPConstituency").is(":visible")) {

            $('#dvSearchMPConstituency').load('/LocationMasterDataEntry/SearchMPConstituency', function () {

               // $('#tbMPConstituencyList').trigger('reloadGrid');

                var data = $('#tbMPConstituencyList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlSearchStates').val(data.stateCode);
                }

                $('#dvSearchMPConstituency').show('slow');
            });
            
        }

        $.unblockUI();
    });







});

function FormatColumn(cellvalue, options, rowObject) {


    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit MP Constituency Details' onClick ='EditMPConstituencyDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete MP Constituency Details' onClick ='DeleteMPConstituencyDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
    
}


function FormatColumnMap(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Map Blocks' onClick ='MapMPConstituency(\"" + cellvalue.toString() + "\");' >Map Blocks</a></td></tr></table></center>";

}

function FormatColumnMapped(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Blocks' onClick ='MappedBlocks(\"" + cellvalue.toString() + "\");' >Mapped Blocks</a></td></tr></table></center>";

}


function EditMPConstituencyDetails(paramater) {

    //alert(paramater);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/LocationMasterDataEntry/EditMPConstituency/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            // $("#mainDiv").html(data);

            if ($("#dvSearchMPConstituency").is(":visible")) {
                $('#dvSearchMPConstituency').hide('slow');
            }

            if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
                $('#dvMapMPConstituencyBlockDetails').hide('slow');
            }

            if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
                $('#dvMappedMPConstituencyBlockDetails').hide('slow');
            }

            $("#dvMPConstituencyDetails").html(data);
            $('#dvMPConstituencyDetails').show('slow');
            $("#MAST_MP_CONST_NAME").focus();

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function MapMPConstituency(parameter) {
    // alert(parameter);


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
   // $('#btnSearchView').trigger('click');
    $.ajax({
        url: "/LocationMasterDataEntry/MapMPConstituencyBlocks/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMapMPConstituencyBlockDetails").html(data);
            $('#dvMapMPConstituencyBlockDetails').show('slow');

            $('#btnSearch_Map').trigger('click');


            $('#trAddNewSearch').hide();
            $('#dvSearchMPConstituency').hide();
            $('#dvMPConstituencyDetails').hide();


            if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
                $('#dvMappedMPConstituencyBlockDetails').hide('slow');
            }

            //$('#tbMLAConstituencyList').setGridParam({ hidegrid: true });
            $('#tbMPConstituencyList').jqGrid("setGridState", "hidden");



            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function DeleteMPConstituencyDetails(urlparamater) {

    if (confirm("Are you sure you want to delete MP constituency details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
        }

        //$('#btnSearchView').trigger('click');
        //$('#trAddNewSearch').show();

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteMPConstituencyDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchMPConstituency").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbMPConstituencyList').trigger('reloadGrid');
                    //}                   
                    $('#dvMPConstituencyDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchMPConstituency").is(":visible")) {
                        $("#dvSearchMPConstituency").show('slow');
                        $('#tbMPConstituencyList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbMPConstituencyList').trigger('reloadGrid');
                    }

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

        if (!$("#dvMPConstituencyDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}


function MappedBlocks(parameter) {
    // alert(parameter);


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $.ajax({
        url: "/LocationMasterDataEntry/MappedMPConstituencyBlocks/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMappedMPConstituencyBlockDetails").html(data);
            $('#dvMappedMPConstituencyBlockDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#dvSearchMPConstituency').hide();
            $('#dvMPConstituencyDetails').hide();

            if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
                $('#dvMapMPConstituencyBlockDetails').hide('slow');
            }

            $('#tbMPConstituencyList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function loadGrid() {

    jQuery("#tbMPConstituencyList").jqGrid({
        url: '/LocationMasterDataEntry/GetMPConstituencyDetailsList',
        datatype: "json",
        mtype: "POST",
        postData:{ stateCode: $('#ddlSearchStates option:selected').val() } ,
        colNames: ['MP Constituency Name', 'State Name', 'Map Blocks', 'Mapped Blocks', 'Action'],
        colModel: [
                            { name: 'MPConstituencyName', index: 'MPConstituencyName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: true, align: "left" },
                             { name: 'Map', width: 140, sortable: false, resize: false, formatter: FormatColumnMap, align: "center" },
                              { name: 'Mapped', width: 100, sortable: false, resize: false, formatter: FormatColumnMapped, align: "center" },
                            { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvMPConstituencyListPager'),
        rowNum: 15,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,MPConstituencyName',
        sortorder: "asc",
        caption: "MP Constituency List",
        height: 'auto',
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
        // hidegrid: false,
        /*grouping: true,
        groupingView:
            {
                groupField: ['StateName'],
                groupColumnShow: [false],
                groupText: ['<b>State: {0}</b>'],
                //groupCollapse: true,
                groupOrder: ['asc']
            },*/
        loadComplete: function () {

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

    });
}