$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $.ajax({
        url: "/Master/SearchMpMembers/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchMpMembers").html(data);
            $('#btnSearch').trigger('click');
        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }
    });

    //LoadGrid();
    $.unblockUI();
    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchMpMembers").is(":visible")) {
            $('#dvSearchMpMembers').hide('slow');
        }

        if (!$("#dvMpMemberDetails").is(":visible")) {
            $("#dvMpMemberDetails").load("/Master/AddEditMasterMpMember/");
            $('#dvMpMemberDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMpMemberDetails").is(":visible")) {
            $('#dvMpMemberDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchMpMembers").is(":visible")) {
            $('#dvSearchMpMembers').load('/Master/SearchMpMembers', function () {
                $('#tblMpMemberList').trigger('reloadGrid');
                var data = $('#tblMpMemberList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {
                    $('#ddlSearchState').val(data.stateCode);
                    
                    FillListInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
                         "#ddlSearchConstituency", "/Master/GetMPConstituencyList?stateCode=" + $('#ddlSearchState option:selected').val());
                    $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");
                  
                    //if ($("#ddlSearchState").val() == 0) {
                    //    $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");
                    //}
                  
                    $('#ddlSearchTerm').val(data.termCode);
                    
                   $('#ddlSearchConstituency').val(data.constituencyCode);
                 
                    $('#txtSearchMember').val(data.memberName);

                }
                $('#dvSearchMpMembers').show('slow');
            });

        }
        $.unblockUI();
    });


});


function LoadGrid() {
    $('#tblMpMemberList').jqGrid('GridUnload');
    $('#tblMpMemberList').jqGrid({
        url: '/Master/GetMasterMpMemberList',
        datatype: "json",//'local',
        mtype: "POST",
        colNames: ['Member Name', 'Party Name', 'MP Constituency','State Name', 'Lok Sabha Term', 'Start Date', 'End Date', 'Action'],
        colModel: [
         { name: 'MAST_MEMBER', index: 'MAST_MEMBER', height: 'auto', width: 130, align: "left", sortable: true },
         { name: 'MAST_MEMBER_PARTY', index: 'MAST_MEMBER_PARTY', height: 'auto', width: 70, align: "left", sortable: true },
         { name: 'MAST_MP_CONST_CODE', index: 'MAST_MP_CONST_CODE', height: 'auto', width: 120, align: "left", sortable: true },
         { name: 'MAST_STATE_NAME', index: 'MAST_MP_CONST_CODE', height: 'auto', width: 120, align: "left", sortable: true },
         { name: 'MAST_LS_TERM', index: 'MAST_LS_TERM', height: 'auto', width:100, align: "left", sortable: true },
         { name: 'MAST_MEMBER_START_DATE', index: 'MAST_MEMBER_START_DATE', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'MAST_MEMBER_END_DATE', index: 'MAST_MEMBER_END_DATE', height: 'auto', width: 100, align: "left", sortable: true },
         { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { termCode: $('#ddlSearchTerm option:selected').val(), stateCode: $('#ddlSearchState option:selected').val(), constituencyCode: $('#ddlSearchConstituency option:selected').val(), memberName: $('#txtSearchMember').val() },
        pager: jQuery('#divPagerMasterMpMember'),
        rowNum: 14,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_LS_TERM,MAST_MP_CONST_CODE,MAST_MEMBER',
        sortorder: "asc",
        caption: 'MP Member List',
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () { },

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

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit MP Member Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete MP Member Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function editData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterMpMember/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if ($("#dvSearchMpMembers").is(":visible")) {
                $('#dvSearchMpMembers').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();


            $("#dvMpMemberDetails").html(data);
            $("#dvMpMemberDetails").show();
            $("#MAST_MEMBER").focus();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            if ($("#dvSearchMpMembers").is(":visible")) {
                $('#dvSearchMpMembers').hide('slow');
            }

            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete MP Member details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/DeleteMasterMpMember/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchMpMembers").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $('#tblMpMemberList').trigger('reloadGrid');
                    //}
                    //$("#dvMpMemberDetails").load("/Master/AddEditMasterMpMember/");
                    if ($("#dvMpMemberDetails").is(":visible")) {
                        $('#dvMpMemberDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }

                    if (!$("#dvSearchMpMembers").is(":visible")) {
                        $("#dvSearchMpMembers").show('slow');
                        $('#tblMpMemberList').trigger('reloadGrid');
                    }
                    else {
                        $('#tblMpMemberList').trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }

        });
    }
    else {
        return false;
    }
}

function FillListInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#ddlSearchConstituency') {
        message = '<h4><label style="font-weight:normal"> Loading Constituencies... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}
