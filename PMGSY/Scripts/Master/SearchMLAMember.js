$(document).ready(function () {

   
    $("#State").change(function () {
        FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
                    "#Constituency", "/Master/GetConstituencyList_ByStateCode?stateCode=" + $('#State option:selected').val());
        FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
                   "#Term", "/Master/GetTermList_ByStateCode?stateCode=" + $('#State option:selected').val());
       // $("#BlockList_HabitationDetails").empty();
        $("#Term").append("<option value='0'>All Term</option>");
        LoadMembers();
      

    });
    //$("#State").trigger('change');

    if ($('#stateCode').val() > 0) {

        $("#State").val($('#stateCode').val());
        $("#State").attr("disabled", true);
        $("#State").trigger('change');
        setTimeout(function () {
            $("#Term").val($("#Term")[0].options[1].value);
        }, 700);
        setTimeout(function () {
            $("#btnMemberSearch").trigger('click')
            {
                LoadMLAGrid();
                $("#btnMemberSearch").trigger('click');
            };
        }, 1000);
    }

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($("#State").val() == 0) {

        $("#State").val($("#State")[0].options[1].value);
        $("#State").trigger('change');
        setTimeout(function () {
            $("#Term").val($("#Term")[0].options[1].value);
        }, 700);
        setTimeout(function ()
        {
            $("#btnMemberSearch").trigger('click')
            {
                LoadMLAGrid();
            }
        },1000);
    }

    $("#btnMemberSearch").click(function () {
       
        SearchMLADetails();
    });

    //added by koustubh nakate on 19/08/2013 to load list MLA members as per search parameters after page load
   
        //$("#btnMemberSearch").trigger('click');
      

  
    $("#dvhdSearch").click(function () {
        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
});

function SearchMLADetails() {

    var isDisabled = false;

 
    if ($('#State').is(':disabled')) { 
        $('#State').attr('disabled', false);
        isDisabled = true;
    }

  
    $('#memberCategory').setGridParam({
        url: '/Master/GetMemberList', datatype: 'json'
    });
    $('#memberCategory').jqGrid("setGridParam", { "postData": { stateCode: $('#State').val(),term:$('#Term').val(), constituency: $('#Constituency option:selected').val(), memberName:$('#txtMemberName').val() } });
    $('#memberCategory').trigger("reloadGrid", [{ page: 1 }]);


    if (isDisabled == true) {
        $('#State').attr('disabled', true);
    }

}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#Constituency') {
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


function LoadMembers() {

    $("#txtMemberName").val('');
    var stateID = $('#State option:selected').val();
    $.ajax({
        url: "/Master/GetMembersByStateCode?stateCode=" + stateID,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {


            var rows = new Array();
            for (var i = 0; i < data.length; i++) {

                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#txtMemberName').autocomplete({
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