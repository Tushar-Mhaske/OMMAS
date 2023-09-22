$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddMembers');

    if (!$("#memberSearchDetails").is(":visible")) {

        $('#memberSearchDetails').load('/Master/SearchMLAMember', function () {

            $('#memberSearchDetails').show('slow');
            $("#btnAdd").show();
            $("#btnSearch").hide();
        });
      
    }

    
    $('#btnAdd').click(function (e) {
        if ($("#memberSearchDetails").is(":visible")) {
            $('#memberSearchDetails').hide('slow');
        }

        if (!$("#memberAddDetails").is(":visible")) {
            $('#memberAddDetails').load("/Master/AddEditMLAMembers");
            $('#memberAddDetails').show('slow');

            $('#btnAdd').hide();
            $('#btnSearch').show();
        }


    });

    $('#btnSearch').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        
        if ($("#memberAddDetails").is(":visible")) {
            $('#memberAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();

        }

        if (!$("#memberSearchDetails").is(":visible")) {

            $('#memberSearchDetails').load('/Master/SearchMLAMember', function () {

                $('#memberCategory').trigger('reloadGrid');

                var data = $('#memberCategory').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#State').val(data.stateCode);
                    FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
                  "#Constituency", "/Master/GetConstituencyList_ByStateCode?stateCode=" + $('#State option:selected').val());
                    FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
                    "#Term", "/Master/GetTermList_ByStateCode?stateCode=" + $('#State option:selected').val());
                    $("#Term").append("<option value='0'>All Term</option>");
                            setTimeout(function () {                          
                                $('#Constituency').val(data.constituency);
                                $('#Term').val(data.Term);
                               }, 1000);
                    }
                $('#memberSearchDetails').show('slow');
            });
        }
        $.unblockUI();
    });

        
    //LoadMLAGrid();

    $("#btnAddNew").click(function (e) {
        if (!$("#memberDetails").is(":visible")) {
            $('#memberDetails').show();
            $('#memberDetails').load("/Master/AddEditMLAMembers");
        }
    });
});
function LoadMLAGrid() {
    $('#memberCategory').jqGrid('GridUnload');
    jQuery("#memberCategory").jqGrid({
        url: '/Master/GetMemberList',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Member Name', 'Party Name', 'MLA Constituency', 'State Name', 'Vidhan Sabha Term ', 'Term Start Date', 'Term End Date', 'Action'],
        colModel: [
                            { name: 'MAST_MEMBER', index: 'MAST_MEMBER', height: 'auto', width: 350, align: "left", sortable: true },
                            { name: 'MAST_MEMBER_PARTY', index: 'MAST_MEMBER_PARTY', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'MAST_MLA_CONST_NAME', index: 'MAST_MLA_CONST_NAME', height: 'auto', width: 250, align: "left", sortable: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 250, align: "left", sortable: true },
                            { name: 'MAST_VS_TERM', index: 'MAST_VS_TERM', height: 'auto', width: 250, align: "left", sortable: true, sortorder: "desc" },
                            { name: 'MAST_MEMBER_START_DATE', index: 'MAST_MEMBER_START_DATE', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'MAST_MEMBER_END_DATE', index: 'MAST_MEMBER_END_DATE', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
        ],
        postData: { stateCode: $('#State option:selected').val(), term: $('#Term option:selected').val(), constituency: $('#Constituency option:selected').val(), memberName: $('#txtMemberName').val() },
        pager: jQuery('#pager'),
        rowNum: 15,
        rowList: [15, 30, 45, 60],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME,MAST_VS_TERM,MAST_MLA_CONST_NAME,MAST_MEMBER',
        //sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: "MLA Member List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        editoptions: { dataInit: function (elem) { $(elem).width(30); } },
        loadComplete: function () {

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

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit MLA Member Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete MLA Member Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditMLAMembers/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            if ($("#memberSearchDetails").is(":visible")) {
                $('#memberSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

           
            $("#memberAddDetails").html(data);
            $("#memberAddDetails").show();
            $("#MAST_MEMBER").focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete MLA Member details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMLAMember/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //if ($("#memberSearchDetails").is(":visible")) {

                    //    $('#btnMemberSearch').trigger('click');

                    //}
                    //else {
                    //    $('#memberCategory').trigger('reloadGrid');
                    //}

                    if ($("#memberAddDetails").is(":visible")) {
                        $('#memberAddDetails').hide('slow');

                        $('#btnSearch').hide();
                        $('#btnAdd').show();

                    }

                    if (!$("#memberSearchDetails").is(":visible")) {
                        $('#memberSearchDetails').show('slow');
                        $('#memberCategory').trigger('reloadGrid');
                    }
                    else {
                        $('#memberCategory').trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}

