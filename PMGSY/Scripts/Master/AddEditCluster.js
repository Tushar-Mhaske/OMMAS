﻿var State;
var District;
var Block;
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmAddCluster'));
    //loadGrid();
    $("#btnSave").click(function () {
        SaveHabitation();
        if ($("#dvViewClusterHabiationDetails").is(":visible")) {
            $("#dvViewClusterHabiationDetails").hide('slow');
        }
       
    });

    $("#btnReset").click(function () {
        $("input:checkbox").removeAttr('checked');
        $("input:radio").removeAttr('checked');
        $("input:radio").attr('disabled', true);
    });
    $('#btnFindHabiation').click(function () {
        State = $('#StateList_ClusterAddDetails option:selected').val();
        District = $('#DistrictList_ClusterAddDetails option:selected').val();
        Block = $('#BlockList_ClusterAddDetails option:selected').val();
        LoadHabitaionAddGrid();
        if ($("#dvViewClusterHabiationDetails").is(":visible")) {
            $("#dvViewClusterHabiationDetails").hide('slow');
        }
        
    });

    
    MaintainStateDistrictBlockDropDownFilterOfAddEditScreen();


    $("#StateList_ClusterAddDetails").change(function () {
        loadAddDistrict($("#StateList_ClusterAddDetails").val());
       // $("#tblHabitationCluster").jqGrid('GridUnload');

    });

    $("#DistrictList_ClusterAddDetails").change(function () {
        loadAddBlock($("#StateList_ClusterAddDetails").val(), $("#DistrictList_ClusterAddDetails").val());
       // $("#tblHabitationCluster").jqGrid('GridUnload');
    });
    $("#BlockList_ClusterAddDetails").change(function () {
        //$("#tblHabitationCluster").jqGrid('GridUnload');
       // $('#btnFindHabiation').trigger('click');
    });
  
   
});
//Maintain State District Block
function MaintainStateDistrictBlockDropDownFilterOfAddEditScreen() {
    if ($("#StateList_ClusterDetails").val() > 0) {
        $("#StateList_ClusterAddDetails").val($("#StateList_ClusterDetails").val());

        loadAddDistrict($("#StateList_ClusterAddDetails").val());

        //alert($("#DistrictList_ClusterDetails").val());
        if ($("#DistrictList_ClusterDetails").val() > 0) {

            loadAddBlock($("#StateList_ClusterAddDetails").val(), $("#DistrictList_ClusterDetails").val());

            setTimeout(function () {
                // alert($("#DistrictList_ClusterDetails").val());
                $("#DistrictList_ClusterAddDetails").val($("#DistrictList_ClusterDetails").val());

            }, 1000);
        }
        if ($("#BlockList_ClusterDetails").val() > 0) {
            setTimeout(function () {
                $("#BlockList_ClusterAddDetails").val($("#BlockList_ClusterDetails").val());
            }, 1200);
            setTimeout(function () {
                $('#btnFindHabiation').trigger('click');
            }, 1500);
        }


    }
}
//Add View Function 
function LoadHabitaionAddGrid() {

    if ($('#frmAddCluster').valid()) {
        $("#tblHabitationCluster").jqGrid('GridUnload');
        var counter = -1;
        $('#tblHabitationCluster').jqGrid({

            url: '/Master/GetClusterHabiationList',
            datatype: "json",
            mtype: "POST",
            colNames: ["Habitation", "Village", "Connectivity Status", "Total Population", "SC/ST Population", "Cluster Name"],
            colModel: [
                       { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: '80', align: "left", sortable: true },
                       { name: 'VillageName', index: 'VillageName', height: 'auto', width: '80', align: "left", sortable: true },
                        { name: 'ConnStatus', index: 'ConnStatus', height: 'auto', width: '50', align: "center", sortable: true },
                       { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '50', align: "center", sortable: true },
                       { name: 'STPopulation', index: 'STPopulation', height: 'auto', width: '50', align: "center", sortable: true },
                       { name: 'RadioId', index: 'RadioId', height: 'auto', width: '20', align: "center", sortable: false },

            ],
            postData: { StateCode: State, DistrictCode: District, BlockCode: Block },
            //data: $('#frmAddCluster').serialize(),
            pager: jQuery('#divHabitationClusterPager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'HabitationName',
            sortorder: "asc",
            caption: "Habitation List",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            pginput: true,
            hidegrid: false,
            multiselect: true,
            onSelectRow: function (id, e) {           
                if (e == true) {
                    $('#tblHabitationCluster').setCell(id, 'RadioId', "<center><input type='radio' id='" + id + "' name='radio' value='" + $('#tblHabitationCluster').getCell(id, 'HabitationName') + "'  /> <center/>");
                } else {
                    $('#tblHabitationCluster').setCell(id, 'RadioId', "<center><input type='radio' id='" + id + "' name='radio' value='" + $('#tblHabitationCluster').getCell(id, 'HabitationName') + "' disabled='" + true + "'  /> <center/>");

                }
            },
            onSelectAll: function(aRowidsChk, eventChk) 
            {
                if (eventChk == true) {
                    $("input:radio").removeAttr('checked');
                    $("input:radio").attr('disabled', false);
                }
                else {
                    $("input:radio").removeAttr('checked');
                    $("input:radio").attr('disabled', true);
                }
            },
            loadComplete: function () {
               // $('#divHabitationClusterPager_left').html('[<b> Note</b>:Select checkbox to add Habiation in Cluster and select radio button to add Habitation Name. ]');
                // $("#gview_tblHabitationCluster > .ui-jqgrid-titlebar").hide();

               
            }
        });

    }


}

function SaveHabitation() {
    if ($('#frmAddCluster').valid()) {
     
        var HabCode = $("#tblHabitationCluster").jqGrid('getGridParam', 'selarrrow');

        if (HabCode == "") {

            alert("Please select Habitation.");
            return false;
        }
        var radiovalueHabName;
        var flag = false;
        var habitationName = '';
        var ids = jQuery("#tblHabitationCluster").jqGrid('getDataIDs');
        for (var i = 1; i <= ids.length; i++) {
            var celValue = jQuery("#tblHabitationCluster").jqGrid('getCell', ids[i], 'RadioId');

            radiovalueHabName = $('input[name=radio]:checked').val();

            if ((radiovalueHabName != '') && !(radiovalueHabName === undefined)) {
                // your code here.  
                habitationName = radiovalueHabName;
                flag = true;
            };

        }

        if (flag === false) {
            alert("Please select Cluster Name");
            return false;
        }

    
        
            if (confirm("Are you sure want to Add habitation.")) {
                $.ajax({
                    type: 'POST',
                    url: "/Master/AddClusterHabitation?HabCode=" + HabCode,
                    data: { HabName: radiovalueHabName, StateCode: State, DistrictCode: District, BlockCode:Block },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            $("input:radio").removeAttr('checked');
                            $("input:radio").attr('disabled', true);
                            alert(data.message);
                            // $('#tblHabitationCluster').trigger("reloadGrid");
                            // $('#tbHabitationList').trigger("reloadGrid");
                          
                            // SearchDetails();
                            //LoadHabitaionAddGrid();
                            $('#btnSearch').trigger('click');

                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }
    }



function loadAddDistrict(statCode) {
    $("#DistrictList_ClusterAddDetails").val(0);
    $("#DistrictList_ClusterAddDetails").empty();
    $("#BlockList_ClusterAddDetails").val(0);
    $("#BlockList_ClusterAddDetails").empty();
    $("#BlockList_ClusterAddDetails").append("<option value='0'>Select Block</option>");

    if (statCode > 0) {
        if ($("#DistrictList_ClusterAddDetails").length > 0) {
            $.ajax({
                url: '/Master/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_ClusterAddDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#DistrictList_ClusterAddDetails").append("<option value='0'>Select District</option>");
        $("#BlockList_ClusterAddDetails").empty();
        $("#BlockList_ClusterAddDetails").append("<option value='0'>Select Block</option>");

    }
}

//District Change Fill Block DropDown List
function loadAddBlock(stateCode, districtCode) {
    $("#BlockList_ClusterAddDetails").val(0);
    $("#BlockList_ClusterAddDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_ClusterAddDetails").length > 0) {
            $.ajax({
                url: '/Master/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_ClusterAddDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_ClusterAddDetails").append("<option value='0'>Select Block</option>");
    }
}

//var id = 0;
function radioFormatter(cellvalue, options, rowObject) {
    //id++;

    var radioName = "radio";
    if (cellvalue == null) {
        cellvalue = false;
    }
    return "<center><input type='radio' name='" + radioName + "' value='" + cellvalue + "' disabled = true'/> <center/>";
};


function ShowSerachListingClusterGridScreen() {
    if (!$("#ClusterSearchDetails").is(":visible")) {

        $('#ClusterSearchDetails').load('/Master/SearchCluster', function () {
            // var data = $('#tblCluster').jqGrid("getGridParam", "postData");

            //if (!(data === undefined)) {

            //    $('#StateList_ClusterDetails').val(data.StateCode);
            //    $('#DistrictList_ClusterDetails').val(data.DistrictCode);
            //    $('#BlockList_ClusterDetails').val(data.BlockCode);
                

            //}
            //LoadClusterGrid();

            $('#ClusterSearchDetails').show('slow');
            //$("#tblCluster").jqGrid('GridUnload');
            MaintainStateDistrictBlockDropDownFilterOfSearchScreen();

        });
    }
   
}

function SearchDetails() {
   
     $('#tblCluster').setGridParam({
        url: '/Master/GetClusterList'
    });


     $('#tblCluster').jqGrid("setGridParam", { "postData": { StateCode: $('#StateList_ClusterAddDetails option:selected').val(), DistrictCode: $('#DistrictList_ClusterAddDetails option:selected').val(), BlockCode: $('#BlockList_ClusterAddDetails option:selected').val(), Status: $('#StatusList_ClusterDetails option:selected').val() } });
    $('#tblCluster').trigger("reloadGrid", [{ page: 1 }]);
}