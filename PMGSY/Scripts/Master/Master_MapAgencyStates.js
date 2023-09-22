$(document).ready(function () {

    LoadMapAgencyState();
    
    var startDateCheck = $('#strtDateState').val();
    if (startDateCheck != '') {
        $("#messageState").hide();
    }
    else {
        $("#messageState").show();
    }
    $('#strtDateState').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../../Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        maxDate:new Date(),
        onSelect: function (selectedDate) {
            $("#messageState").hide();
        }
    });
   
  
    $("#dvhdSearch_Map_State").click(function () {

        if ($("#dvSearchParameter_Map_State").is(":visible")) {
            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter_Map_State").slideToggle(300);
        }
        else {
            $("#spCollapseIconS_Map").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchParameter_Map_State").slideToggle(300);
        }
    });

 

    $('#btnMapState').click(function (e) {
        var stateCodes = $("#tbMapAgencyStateList").jqGrid('getGridParam', 'selarrrow');
        var startDateCheck = $('#strtDateState').val();
        if (startDateCheck != '') {
            $("#messageState").hide();
        }
        else {
            $("#messageState").show();
            return false;
        }
       
        if (stateCodes != '') {
           
            
            $('#EncryptedStateCodes').val(stateCodes);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/MapAgencyStates",
                type: "POST",
                dataType: "json",
                data: $("#frmSearchMapAgencyBlock,#frmStartDateState").serialize(),
               
                success: function (data) {
                    
                  
                    //document.getElementById('startDate').style.visibility = 'hidden';
                    $("#startDateState").hide();
                    $("#messageState").hide();

                        alert(data.message);

                        $("#tbMapAgencyStateList").trigger('reloadGrid');
                        
                        $.unblockUI();
                                       
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {
            
            alert('Please select State to map with Technical Agency.');
        }


    });

    $('#btnMapStateCancel').click(function (e) {

        if ($("#dvMapAgencyStateDetails").is(":visible")) {
            $('#dvMapAgencyStateDetails').hide('slow');
        }

        $('#btnSearchView').trigger('click');
        
        //Added By Abhishek kamble 26-Feb-2014        
        $('#btnSearchView').hide('slow');
        $('#btnAddAgency').show('slow');
        

        $('#tblList').jqGrid("setGridState", "visible");
        $('#trAddNewSearch').show();

        $("#mainDiv").animate({
            scrollTop: 0
        });
    });
});

function LoadMapAgencyState() {


    jQuery("#tbMapAgencyStateList").jqGrid({
        url: '/Master/GetStateDetailsList_Mapping',
        datatype: "json",
        mtype: "POST",
        colNames: ['State Name','Short Name', 'State/UT', 'State Type', 'Census Code', 'Action'],
        colModel: [
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 300, align: "left", sortable: true },
                            { name: 'ShortName', index: 'ShortName', width: 100, sortable: true, align: "left",hidden:true },
                            { name: 'StateUT', index: 'StateUT', height: 'auto', width: 200, sortable: true, align: "left" },
                            { name: 'StateType', index: 'StateType', width: 200, sortable: true },                       
                            { name: 'NICStateCode', index: 'NICStateCode', width: 130, sortable: false, hidden: false },
                            { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false,hidden:true }
        ],
        pager: jQuery('#dvMapAgencyStateListPager'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
      
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "State List",
        height: 'auto',
       
        autowidth: true,
        rownumbers: true,
        sortname: 'StateUT,StateName',
        sortorder: "asc",
        hidegrid: false, 
        multiselect: true,
        onSelectRow: function (id) {
            var stateCodesCheck = $("#tbMapAgencyStateList").jqGrid('getGridParam', 'selarrrow');
            var startDate = document.getElementById('startDateState');
            var startDateCheck = $('#strtDateState').val();
            strtDateState.placeholder = "dd/mm/yyyy";
            if (stateCodesCheck != '') {
                //startDate.style.visibility = 'visible';
                $("#startDateState").show();
            }
            else {
                //startDate.style.visibility = 'hidden';
                $("#startDateState").hide();
            }
            if (startDateCheck != '') {
                $("#messageState").hide();
            }
            else {
                $("#messageState").show();
            }

           
        },
        onSelectAll: function (aRowids, status) {
            var startDate = document.getElementById('startDateState');
            var startDateCheck = $('#strtDateState').val();
            strtDateState.placeholder = "dd/mm/yyyy";
            if (status) {
                //startDate.style.visibility = 'visible';
                $("#startDateState").show();
            }
            else {
                //startDate.style.visibility = 'hidden';
                $("#startDateState").hide();
            }
            if (startDateCheck != '') {
                $("#messageState").hide();
            }
            else {
                $("#messageState").show();
            }
            
            
        },
        loadComplete: function () {

            var recordCount = jQuery('#tbMapAgencyStateList').jqGrid('getGridParam', 'reccount');

            if (recordCount > 15) {

                $('#tbMapAgencyStateList').jqGrid('setGridHeight', '320');
        

            }
            else {
                $('#tbMapAgencyStateList').jqGrid('setGridHeight', 'auto');
             
            }

            $.unblockUI();
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

