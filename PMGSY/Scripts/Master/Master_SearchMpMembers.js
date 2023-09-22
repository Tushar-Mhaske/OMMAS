$(document).ready(function () {

    $('#ddlSearchState').change(function () {

        FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
                 "#ddlSearchConstituency", "/Master/GetMPConstituencyList?stateCode=" + $('#ddlSearchState option:selected').val());
        $("#ddlSearchConstituency").append("<option value='0'>All Constituencies </option>");

    });

    if ($("#StateCode").val() > 0) {
        $("#ddlSearchState").val($("#StateCode").val());
        $('#ddlSearchState').trigger('change');
    }
    else {

        if ($("#ddlSearchState").val() == 0) {
            $("#ddlSearchState").val($("#ddlSearchState")[0].options[1].value);
            $('#ddlSearchState').trigger('change');

        }
    }
    
    LoadMemberNames();
    if ($("#frmSearchMpMembers") != null) {
        $.validator.unobtrusive.parse("#frmSearchMpMembers");
    }

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });


    $('#btnSearch').click(function (e) {

        SearchMPDetails();

    });


    $('#ddlSearchTerm').change(function () {

        LoadMemberNames();

    });

    $('#btnSearch').trigger('click')
    {
        LoadGrid();
    };
});

function SearchMPDetails() {

    $('#tblMpMemberList').setGridParam({
        url: '/Master/GetMasterMpMemberList', datatype: 'json'
    });
    $('#tblMpMemberList').jqGrid("setGridParam", { "postData": { termCode: $('#ddlSearchTerm option:selected').val(), stateCode: $('#ddlSearchState option:selected').val(), constituencyCode: $('#ddlSearchConstituency option:selected').val(), memberName: $('#txtSearchMember').val() } });
    $('#tblMpMemberList').trigger("reloadGrid", [{ page: 1 }]);

}

function LoadMemberNames() {

    var termCode = $('#ddlSearchTerm option:selected').val();
    $.ajax({
        url: "/Master/GetMemberNameByTermCode_Search?termCode=" + termCode,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {


            var rows = new Array();
            for (var i = 0; i < data.length; i++) {

                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#txtSearchMember').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {

            if (xhr.responseText == "session expired") {

                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })
}

function FillInCascadeDropdown(map, dropdown, action) {

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