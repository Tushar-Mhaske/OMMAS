var gStateCode = 0;
var gStateName = "";
var gDistrictCode = 0;
var gDistrictName = "";
var gYear = 0;
var gPhase = "";
var collapse = true;
$(document).ready(function () {

    //For Accordion Collapasable
    $(function () {
        $("#accordionPropNotMap").accordion({
            collapsible: false,
        });
    });

    //Tab Initialised
    $("#tabMain").tabs();

    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#ddState_PropNotMappedDetails").attr("disabled", "disabled");
        $("#ddState_PropNumberBaseCNDetails").attr("disabled", "disabled");
        $("#ddState_PropMultipleDetails").attr("disabled", "disabled");
        $("#ddState_PropSingleHabDetails").attr("disabled", "disabled");
        $("#ddState_PropZeroMaintDetails").attr("disabled", "disabled");
        $("#ddState_PropCarriageWidthDetails").attr("disabled", "disabled");
        $("#ddState_PropVariationLengthDetails").attr("disabled", "disabled");
        $("#ddState_PropMisclassificationDetails").attr("disabled", "disabled");
    }
    if ($("#MAST_DISTRICT_CODE").val() > 0) {
        $("#ddDistrict_PropNotMappedDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropNumberBaseCNDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropMultipleDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropSingleHabDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropZeroMaintDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropCarriageWidthDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropVariationLengthDetails").attr("disabled", "disabled");
        $("#ddDistrict_PropMisclassificationDetails").attr("disabled", "disabled");
    }
    //Start Tab 1  Proposals Not Mapped
    $("#ddState_PropNotMappedDetails").change(function () {
        loadDistrict($("#ddState_PropNotMappedDetails").val(), "#ddState_PropNotMappedDetails", "#ddDistrict_PropNotMappedDetails", "#ddBlock_PropNotMappedDetails");

    });

    $("#ddDistrict_PropNotMappedDetails").change(function () {
        loadBlock($("#ddState_PropNotMappedDetails").val(), $("#ddDistrict_PropNotMappedDetails").val(), "#ddState_PropNotMappedDetails", "#ddDistrict_PropNotMappedDetails", "#ddBlock_PropNotMappedDetails");

    });
    $('#btnGoPropNotMapped').click(function () {
        //"Sanctioned": sanctioned, "StateCode": $('#ddState_PropNotMappedDetails').val(), "DistrictCode": $('#ddDistrict_PropNotMappedDetails').val(), "BlockCode": $('#ddBlock_PropNotMappedDetails').val(),
        //   "YearCode": $('#ddYear_PropNotMappedDetails').val(), "BatchCode": $('#ddBatch_PropNotMappedDetails').val(), "CollaborationCode": $('#ddCollaboration_PropNotMappedDetails').val(), "AgencyCode": $('#ddAgency_PropNotMappedDetails').val(),
        //   "StaStatusCode": $('#ddStaStatus_PropNotMappedDetails').val(), "MrdStatusCode": $('#ddMrdStatus_PropNotMappedDetails').val()


        var sanctioned = $("#ddSanctioned_PropNotMappedDetails option:selected").val();

        var stateCode = $('#ddState_PropNotMappedDetails option:selected').val();
        var stateName = $('#ddState_PropNotMappedDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropNotMappedDetails option:selected').val();
        var districtName = $('#ddDistrict_PropNotMappedDetails option:selected').text();

        var blockCode = $('#ddBlock_PropNotMappedDetails option:selected').val();
        var blockName = $('#ddBlock_PropNotMappedDetails option:selected').text();

        var yearCode = $('#ddYear_PropNotMappedDetails option:selected').val();
        var yearName = $('#ddYear_PropNotMappedDetails option:selected').val();

        var batchCode = $('#ddBatch_PropNotMappedDetails option:selected').val();
        var batchName = $('#ddBatch_PropNotMappedDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropNotMappedDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropNotMappedDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropNotMappedDetails option:selected').val();
        var agencyName = $('#ddAgency_PropNotMappedDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropNotMappedDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropNotMappedDetails option:selected').text();

        var mrdStatusCode = $('#ddMrdStatus_PropNotMappedDetails option:selected').val();
        var mrdStatusName = $('#ddMrdStatus_PropNotMappedDetails option:selected').text();

        $("#tbPropNotMappedDistrictDetailReport").jqGrid('GridUnload');
        $("#tbPropNotMappedDetailReport").jqGrid('GridUnload');

        if ($("#hdnLevelId").val() == 6) //mord
        {
            LoadPropNotMappedStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            //LoadPropNotMappedPhaseViewDetailGrid(0, "", 0, sanctioned);
            LoadPropNotMappedPhaseViewDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
        }
        else if ($("#hdnLevelId").val() == 4) //State
        {

            //LoadPropNotMappedPhaseViewDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, sanctioned);
            //LoadPropNotMappedDistrictDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, sanctioned);
            //LoadPropNotMappedDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, 0, "", 0, sanctioned);
            LoadPropNotMappedPhaseViewDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            LoadPropNotMappedDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            LoadPropNotMappedDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);

        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            $('#accordionPropNotMap').hide();
            //LoadPropNotMappedDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, sanctioned);
            LoadPropNotMappedDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);

        }

        if (!$('#accrodiontabState').is(':visible')) {
            // $("#tbPropNotMappedPhaseViewDetailReport").jqGrid('setGridHeight', 200, true);
            //$('#accordionPropNotMap').height(400);

        }

    });

    $('#btnGoPropNotMapped').trigger('click');

    //Start Tab 2 Road Number Based CN 

    $("#ddState_PropNumberBaseCNDetails").change(function () {
        loadDistrict($("#ddState_PropNumberBaseCNDetails").val(), "#ddState_PropNumberBaseCNDetails", "#ddDistrict_PropNumberBaseCNDetails", "#ddBlock_PropNumberBaseCNDetails");

    });

    $("#ddDistrict_PropNumberBaseCNDetails").change(function () {
        loadBlock($("#ddState_PropNumberBaseCNDetails").val(), $("#ddDistrict_PropNumberBaseCNDetails").val(), "#ddState_PropNumberBaseCNDetails", "#ddDistrict_PropNumberBaseCNDetails", "#ddBlock_PropNumberBaseCNDetails");

    });
    $('#btnGoPropNumberBaseCN').click(function () {

        var stateCode = $('#ddState_PropNumberBaseCNDetails option:selected').val();
        var stateName = $('#ddState_PropNumberBaseCNDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropNumberBaseCNDetails option:selected').val();
        var districtName = $('#ddDistrict_PropNumberBaseCNDetails option:selected').text();

        var blockCode = $('#ddBlock_PropNumberBaseCNDetails option:selected').val();
        var blockName = $('#ddBlock_PropNumberBaseCNDetails option:selected').text();



        $("#tbPropNumberBaseCNRoadDetailReport").jqGrid('GridUnload');
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //LoadPropNumberBaseCNDistrictDetailGrid(stateCode, $("#ddState_PropNumberBaseCNDetails option:selected").text());
            //LoadPropNumberBaseCNRoadDetailGrid(stateCode, $("#ddState_PropNumberBaseCNDetails option:selected").text(), 0, "", 0, '%', 'R');

            LoadPropNumberBaseCNDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName);
            LoadPropNumberBaseCNRoadDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, "%", 'R');

        }
        else if ($("#hdnLevelId").val() == 4) //State
        {
            //LoadPropNumberBaseCNDistrictDetailGrid(stateCode, $("#ddState_PropNumberBaseCNDetails option:selected").text());
            //LoadPropNumberBaseCNRoadDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", 0, '%', 'R');
            LoadPropNumberBaseCNDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName);
            LoadPropNumberBaseCNRoadDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, "%", 'R');
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            //LoadPropNumberBaseCNRoadDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), 0, '%', 'R');
            LoadPropNumberBaseCNRoadDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, "%", 'R');
        }


    });
    $('#btnGoPropNumberBaseCN').trigger('click');

    //Start of Tab 3 Multiple Proposals mapped to CN Road 

    $("#ddState_PropMultipleDetails").change(function () {
        loadDistrict($("#ddState_PropMultipleDetails").val(), "#ddState_PropMultipleDetails", "#ddDistrict_PropMultipleDetails", "#ddBlock_PropMultipleDetails");

    });

    $("#ddDistrict_PropMultipleDetails").change(function () {
        loadBlock($("#ddState_PropMultipleDetails").val(), $("#ddDistrict_PropMultipleDetails").val(), "#ddState_PropMultipleDetails", "#ddDistrict_PropMultipleDetails", "#ddBlock_PropMultipleDetails");
    });

    $('#btnGoPropMultiple').click(function () {
        //var stateCode = $("#ddState_PropMultipleDetails option:selected").val();
        //var stateName = $("#ddState_PropMultipleDetails option:selected").text();

        //var sanctioned = $("#ddSanctioned_PropMultipleDetails option:selected").val();

        var sanctioned = $("#ddSanctioned_PropMultipleDetails option:selected").val();

        var stateCode = $('#ddState_PropMultipleDetails option:selected').val();
        var stateName = $('#ddState_PropMultipleDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropMultipleDetails option:selected').val();
        var districtName = $('#ddDistrict_PropMultipleDetails option:selected').text();

        var blockCode = $('#ddBlock_PropMultipleDetails option:selected').val();
        var blockName = $('#ddBlock_PropMultipleDetails option:selected').text();

        var yearCode = $('#ddYear_PropMultipleDetails option:selected').val();
        var yearName = $('#ddYear_PropMultipleDetails option:selected').val();

        var batchCode = $('#ddBatch_PropMultipleDetails option:selected').val();
        var batchName = $('#ddBatch_PropMultipleDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropMultipleDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropMultipleDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropMultipleDetails option:selected').val();
        var agencyName = $('#ddAgency_PropMultipleDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropMultipleDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropMultipleDetails option:selected').text();
        var roadCode = '%';
        var report = 'A';
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //LoadPropMultipleDistrictDetailGrid(stateCode, stateName, sanctioned);
            //LoadPropMultipleDetailGrid(stateCode, stateName, 0, 0, roadCode, sanctioned, report, "", "");
            LoadPropMultipleDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned);
            LoadPropMultipleDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, "%", sanctioned, report);
        }
        else if ($("#hdnLevelId").val() == 4) //State
        {
            //LoadPropMultipleDistrictDetailGrid(stateCode, stateName, sanctioned);
            //LoadPropMultipleDetailGrid(stateCode, stateName, 0, 0, roadCode, sanctioned, report, "", "");
            LoadPropMultipleDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned);
            LoadPropMultipleDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, "%", sanctioned, report);


        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //LoadPropMultipleDetailGrid(stateCode, stateName, $("#MAST_DISTRICT_CODE").val(), 0, roadCode, sanctioned, report, $("#DISTRICT_NAME").val(), "");
            LoadPropMultipleDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, "%", sanctioned, report);


        }

        //if ($('#chkDistrictWise').is(':checked')) {
        //    LoadPropMultipleDistrictDetailGrid(stateCode, stateName, sanctioned);
        //    LoadPropMultipleDetailGrid(stateCode, stateName, roadCode, sanctioned, report);
        //}
        //else {
        //    $("#tbPropMultipleDistrictDetailReport").jqGrid('GridUnload');
        //    LoadPropMultipleDetailGrid(stateCode, stateName, roadCode, sanctioned, report);
        //}

    });

    $('#btnGoPropMultiple').trigger('click');

    //Start of Tab 4 Single Habitation

    $("#ddState_PropSingleHabDetails").change(function () {
        loadDistrict($("#ddState_PropSingleHabDetails").val(), "#ddState_PropSingleHabDetails", "#ddDistrict_PropSingleHabDetails", "#ddBlock_PropSingleHabDetails");

    });

    $("#ddDistrict_PropSingleHabDetails").change(function () {
        loadBlock($("#ddState_PropSingleHabDetails").val(), $("#ddDistrict_PropSingleHabDetails").val(), "#ddState_PropSingleHabDetails", "#ddDistrict_PropSingleHabDetails", "#ddBlock_PropSingleHabDetails");
    });
    $('#btnGoPropSingleHab').click(function () {
        //var population = $("#ddPopulation_PropSingleHabDetails option:selected").val();
        //var year = $("#ddYear_PropSingleHabDetails option:selected").val();
        //var batch = $("#ddBatch_PropSingleHabDetails option:selected").val();
        //var sanctioned = $("#ddSanctioned_PropSingleHabDetails option:selected").val();
        var sanctioned = $("#ddSanctioned_PropSingleHabDetails option:selected").val();

        var stateCode = $('#ddState_PropSingleHabDetails option:selected').val();
        var stateName = $('#ddState_PropSingleHabDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropSingleHabDetails option:selected').val();
        var districtName = $('#ddDistrict_PropSingleHabDetails option:selected').text();

        var blockCode = $('#ddBlock_PropSingleHabDetails option:selected').val();
        var blockName = $('#ddBlock_PropSingleHabDetails option:selected').text();

        var yearCode = $('#ddYear_PropSingleHabDetails option:selected').val();
        var yearName = $('#ddYear_PropSingleHabDetails option:selected').val();

        var batchCode = $('#ddBatch_PropSingleHabDetails option:selected').val();
        var batchName = $('#ddBatch_PropSingleHabDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropSingleHabDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropSingleHabDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropSingleHabDetails option:selected').val();
        var agencyName = $('#ddAgency_PropSingleHabDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropSingleHabDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropSingleHabDetails option:selected').text();

        var population = $("#ddPopulation_PropSingleHabDetails option:selected").val();

        $("#tbPropSingleHabDistrictDetailReport").jqGrid('GridUnload');
        $("#tbPropSingleHabDetailReport").jqGrid('GridUnload');
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //LoadPropSingleHabStateDetailGrid(population, year, batch, sanctioned);
            LoadPropSingleHabStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
        }
        else if ($("#hdnLevelId").val() == 4) //State
        {
            //LoadPropSingleHabDistrictDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), population, year, batch, sanctioned);
            // LoadPropSingleHabDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", population, year, batch, sanctioned);

            LoadPropSingleHabDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
            LoadPropSingleHabDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);

        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //  LoadPropSingleHabDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), population, year, batch, sanctioned);
            LoadPropSingleHabDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
        }

    });
    $('#btnGoPropSingleHab').trigger('click');

    //Start of Tab 5 Zero Maintenance Cost
    $("#ddState_PropZeroMaintDetails").change(function () {
        loadDistrict($("#ddState_PropZeroMaintDetails").val(), "#ddState_PropZeroMaintDetails", "#ddDistrict_PropZeroMaintDetails", "#ddBlock_PropZeroMaintDetails");

    });

    $("#ddDistrict_PropZeroMaintDetails").change(function () {
        loadBlock($("#ddState_PropZeroMaintDetails").val(), $("#ddDistrict_PropZeroMaintDetails").val(), "#ddState_PropZeroMaintDetails", "#ddDistrict_PropZeroMaintDetails", "#ddBlock_PropZeroMaintDetails");
    });
    $('#btnGoPropZeroMaint').click(function () {
        //var constructionType = $("#ddConstruction_PropZeroMaintDetails option:selected").val();
        //var sanctioned = $("#ddSanctioned_PropZeroMaintDetails option:selected").val();
        //var year = $("#ddYear_PropZeroMaintDetails option:selected").val();
        //var batch = $("#ddBatch_PropZeroMaintDetails option:selected").val();
        var sanctioned = $("#ddSanctioned_PropZeroMaintDetails option:selected").val();

        var stateCode = $('#ddState_PropZeroMaintDetails option:selected').val();
        var stateName = $('#ddState_PropZeroMaintDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropZeroMaintDetails option:selected').val();
        var districtName = $('#ddDistrict_PropZeroMaintDetails option:selected').text();

        var blockCode = $('#ddBlock_PropZeroMaintDetails option:selected').val();
        var blockName = $('#ddBlock_PropZeroMaintDetails option:selected').text();

        var yearCode = $('#ddYear_PropZeroMaintDetails option:selected').val();
        var yearName = $('#ddYear_PropZeroMaintDetails option:selected').val();

        var batchCode = $('#ddBatch_PropZeroMaintDetails option:selected').val();
        var batchName = $('#ddBatch_PropZeroMaintDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropZeroMaintDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropZeroMaintDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropZeroMaintDetails option:selected').val();
        var agencyName = $('#ddAgency_PropZeroMaintDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropZeroMaintDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropZeroMaintDetails option:selected').text();
        var constructionType = $("#ddConstruction_PropZeroMaintDetails option:selected").val();

        $("#tbPropZeroMaintDistrictDetailReport").jqGrid('GridUnload');
        $("#tbPropZeroMaintDetailReport").jqGrid('GridUnload');
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //LoadPropZeroMaintStateDetailGrid(constructionType, sanctioned, year, batch);
            LoadPropZeroMaintStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
        }
        else if ($("#hdnLevelId").val() == 4) //State
        {
            //LoadPropZeroMaintDistrictDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), constructionType, sanctioned, year, batch);
            //LoadPropZeroMaintDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", constructionType, sanctioned, year, batch);

            LoadPropZeroMaintDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
            LoadPropZeroMaintDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);

        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            //LoadPropZeroMaintDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), constructionType, sanctioned, year, batch);
            LoadPropZeroMaintDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
        }

    });
    $('#btnGoPropZeroMaint').trigger('click');

    //Start of Tab 6 Carriage way width
    $("#ddState_PropCarriageWidthDetails").change(function () {
        loadDistrict($("#ddState_PropCarriageWidthDetails").val(), "#ddState_PropCarriageWidthDetails", "#ddDistrict_PropCarriageWidthDetails", "#ddBlock_PropCarriageWidthDetails");

    });

    $("#ddDistrict_PropCarriageWidthDetails").change(function () {
        loadBlock($("#ddState_PropCarriageWidthDetails").val(), $("#ddDistrict_PropCarriageWidthDetails").val(), "#ddState_PropCarriageWidthDetails", "#ddDistrict_PropCarriageWidthDetails", "#ddBlock_PropCarriageWidthDetails");
    });
    $('#btnGoPropCarriageWidth').click(function () {
        //var carriageWidth = $("#ddCarriage_PropCarriageWidthDetails option:selected").val();
        //var carriageName = $("#ddCarriage_PropCarriageWidthDetails option:selected").text();
        var sanctioned = $("#ddSanctioned_PropCarriageWidthDetails option:selected").val();

        var stateCode = $('#ddState_PropCarriageWidthDetails option:selected').val();
        var stateName = $('#ddState_PropCarriageWidthDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropCarriageWidthDetails option:selected').val();
        var districtName = $('#ddDistrict_PropCarriageWidthDetails option:selected').text();

        var blockCode = $('#ddBlock_PropCarriageWidthDetails option:selected').val();
        var blockName = $('#ddBlock_PropCarriageWidthDetails option:selected').text();

        var yearCode = $('#ddYear_PropCarriageWidthDetails option:selected').val();
        var yearName = $('#ddYear_PropCarriageWidthDetails option:selected').val();

        var batchCode = $('#ddBatch_PropCarriageWidthDetails option:selected').val();
        var batchName = $('#ddBatch_PropCarriageWidthDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropCarriageWidthDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropCarriageWidthDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropCarriageWidthDetails option:selected').val();
        var agencyName = $('#ddAgency_PropCarriageWidthDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropCarriageWidthDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropCarriageWidthDetails option:selected').text();

        var carriageWidth = $("#ddCarriage_PropCarriageWidthDetails option:selected").val();
        var carriageName = $("#ddCarriage_PropCarriageWidthDetails option:selected").text();

        $("#tbPropCarriageWidthDistrictDetailReport").jqGrid('GridUnload');
        $("#tbPropCarriageWidthDetailReport").jqGrid('GridUnload');
        if ($("#hdnLevelId").val() == 6) //mord
        {
            //LoadPropCarriageWidthStateDetailGrid(carriageWidth, carriageName);
            LoadPropCarriageWidthStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName);
        }
        else if ($("#hdnLevelId").val() == 4) //State
        {
            //LoadPropCarriageWidthDistrictDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), carriageWidth, carriageName);
            //LoadPropCarriageWidthDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), 0, "", carriageWidth, carriageName, 0);
            LoadPropCarriageWidthDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName);
            LoadPropCarriageWidthDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName);

        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            // LoadPropCarriageWidthDetailGrid($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), $("#MAST_DISTRICT_CODE").val(), $("#DISTRICT_NAME").val(), carriageWidth, carriageName, 0);
            LoadPropCarriageWidthDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName);
        }

    });
    $('#btnGoPropCarriageWidth').trigger('click');


    //Start of Tab 7 Variation in Proposed

    $("#ddState_PropVariationLengthDetails").change(function () {
        loadDistrict($("#ddState_PropVariationLengthDetails").val(), "#ddState_PropVariationLengthDetails", "#ddDistrict_PropVariationLengthDetails", "#ddBlock_PropVariationLengthDetails");

    });

    $("#ddDistrict_PropVariationLengthDetails").change(function () {
        loadBlock($("#ddState_PropVariationLengthDetails").val(), $("#ddDistrict_PropVariationLengthDetails").val(), "#ddState_PropVariationLengthDetails", "#ddDistrict_PropVariationLengthDetails", "#ddBlock_PropVariationLengthDetails");
    });
    $('#btnGoPropVariationLength').click(function () {
        //var stateCode = $("#ddState_PropVariationLengthDetails option:selected").val();
        //var sanctioned = $("#ddSanctioned_PropVariationLengthDetails option:selected").val();
        //var year = $("#ddYear_PropVariationLengthDetails option:selected").val();
        //var batch = $("#ddBatch_PropVariationLengthDetails option:selected").val();
        var sanctioned = $("#ddSanctioned_PropVariationLengthDetails option:selected").val();

        var stateCode = $('#ddState_PropVariationLengthDetails option:selected').val();
        var stateName = $('#ddState_PropVariationLengthDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropVariationLengthDetails option:selected').val();
        var districtName = $('#ddDistrict_PropVariationLengthDetails option:selected').text();

        var blockCode = $('#ddBlock_PropVariationLengthDetails option:selected').val();
        var blockName = $('#ddBlock_PropVariationLengthDetails option:selected').text();

        var yearCode = $('#ddYear_PropVariationLengthDetails option:selected').val();
        var yearName = $('#ddYear_PropVariationLengthDetails option:selected').val();

        var batchCode = $('#ddBatch_PropVariationLengthDetails option:selected').val();
        var batchName = $('#ddBatch_PropVariationLengthDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropVariationLengthDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropVariationLengthDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropVariationLengthDetails option:selected').val();
        var agencyName = $('#ddAgency_PropVariationLengthDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropVariationLengthDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropVariationLengthDetails option:selected').text();
        LoadPropVariationLengthDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned);

    });
    $('#btnGoPropVariationLength').trigger('click');

    //Start of Tab 8 MissClaasifiction in Proposed

    $("#ddState_PropMisclassificationDetails").change(function () {
        loadDistrict($("#ddState_PropMisclassificationDetails").val(), "#ddState_PropMisclassificationDetails", "#ddDistrict_PropMisclassificationDetails", "#ddBlock_PropMisclassificationDetails");

    });

    $("#ddDistrict_PropMisclassificationDetails").change(function () {
        loadBlock($("#ddState_PropMisclassificationDetails").val(), $("#ddDistrict_PropMisclassificationDetails").val(), "#ddState_PropMisclassificationDetails", "#ddDistrict_PropMisclassificationDetails", "#ddBlock_PropMisclassificationDetails");
    });
    $('#btnGoPropMisclassification').click(function () {
        //var stateCode = $("#ddState_PropMisclassificationDetails option:selected").val();
        var sanctioned = $("#ddSanctioned_PropMisclassificationDetails option:selected").val();

        var stateCode = $('#ddState_PropMisclassificationDetails option:selected').val();
        var stateName = $('#ddState_PropMisclassificationDetails option:selected').text();

        var districtCode = $('#ddDistrict_PropMisclassificationDetails option:selected').val();
        var districtName = $('#ddDistrict_PropMisclassificationDetails option:selected').text();

        var blockCode = $('#ddBlock_PropMisclassificationDetails option:selected').val();
        var blockName = $('#ddBlock_PropMisclassificationDetails option:selected').text();

        var yearCode = $('#ddYear_PropMisclassificationDetails option:selected').val();
        var yearName = $('#ddYear_PropMisclassificationDetails option:selected').val();

        var batchCode = $('#ddBatch_PropMisclassificationDetails option:selected').val();
        var batchName = $('#ddBatch_PropMisclassificationDetails option:selected').text();

        var collabCode = $('#ddCollaboration_PropMisclassificationDetails option:selected').val();
        var collabName = $('#ddCollaboration_PropMisclassificationDetails option:selected').text();

        var agencyCode = $('#ddAgency_PropMisclassificationDetails option:selected').val();
        var agencyName = $('#ddAgency_PropMisclassificationDetails option:selected').text();

        var staStatusCode = $('#ddStaStatus_PropMisclassificationDetails option:selected').val();
        var staStatusName = $('#ddStaStatus_PropMisclassificationDetails option:selected').text();

        var mrdStatusCode = $('#ddMrdStatus_PropMisclassificationDetails option:selected').val();
        var mrdStatusName = $('#ddMrdStatus_PropMisclassificationDetails option:selected').text();
        $("#tbPropMisclassificationDetailReport").jqGrid('GridUnload');
        LoadPropMisclassificationGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
    });
    $('#btnGoPropMisclassification').trigger('click');


    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});

function loadDistrict(statCode, ddStateId, ddDistrictId, ddBlockId) {
    $(ddDistrictId).val(0);
    $(ddDistrictId).empty();
    $(ddBlockId).val(0);
    $(ddBlockId).empty();
    $(ddBlockId).append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($(ddDistrictId).length > 0) {
            $.ajax({
                url: '/ProposalReports/AllDistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $(ddDistrictId).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#ddDistrict_PropNotMappedDetails').find("option[value='0']").remove();
                    //$(ddDistrictId).append("<option value='0'>Select District</option>");
                    //$('#ddDistrict_PropNotMappedDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $(ddDistrictId).val($("#Mast_District_Code").val());
                        // $(ddDistrictId).attr("disabled", "disabled");
                        $(ddDistrictId).trigger('change');
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

        $(ddDistrictId).append("<option value='0'>All Districts</option>");
        $(ddBlockId).empty();
        $(ddBlockId).append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode, ddStateId, ddDistrictId, ddBlockId) {
    $(ddBlockId).val(0);
    $(ddBlockId).empty();

    if (districtCode > 0) {
        if ($(ddBlockId).length > 0) {
            $.ajax({
                url: '/ProposalReports/AllBlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $(ddBlockId).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $(ddBlockId).val($("#Mast_Block_Code").val());
                        // $(ddBlockId).attr("disabled", "disabled");
                        //$(ddBlockId).trigger('change');
                    }
                    //$('#ddBlockId').find("option[value='0']").remove();
                    //$(ddBlockId).append("<option value='0'>Select Block</option>");
                    //$('#ddBlockId').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $(ddBlockId).append("<option value='0'>All Blocks</option>");
    }
}

//Start Tab 1  Proposals Not Mapped

function LoadPropNotMappedStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropNotMappedStateDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropNotMappedStateDetailReport").jqGrid({
        url: '/ProposalReports/PropNotMappedStateListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Total Proposals"],
        colModel: [
            { name: "StateName", width: 230, align: 'center', height: 'auto', sortable: true },
            { name: "TotalProposals", width: 230, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }
        ],
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropNotMappedStateDetailReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Statewise Distribution",
        height: 230,
        // width: '100%',
        sortname: 'StateName',
        footerrow: true,
        loadComplete: function () {
            //           $("#tbPropNotMappedStateDetailReport").jqGrid('setGridWidth', $('#accordionPropNotMap').width(), true);
            // $("#tbPropNotMappedStateDetailReport").jqGrid('setGridWidth', '100%', true);
            //Total of Columns
            var TotalProposalsT = $(this).jqGrid('getCol', 'TotalProposals', false, 'sum');
            ////
            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalProposals: TotalProposalsT }, true);
            $('#tbPropNotMappedStateDetailReport_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            gStateCode = params[0];
            gStateName = params[1];
            //LoadPropNotMappedPhaseViewDetailGrid(params[0], params[1], 0, sanctioned);
            //LoadPropNotMappedDistrictDetailGrid(params[0], params[1], 0, sanctioned);
            //LoadPropNotMappedDetailGrid(gStateCode, gStateName, gPhase, gDistrictCode, gDistrictName, gYear, sanctioned);

            LoadPropNotMappedPhaseViewDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            LoadPropNotMappedDistrictDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            LoadPropNotMappedDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);

        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid
}
function LoadPropNotMappedPhaseViewDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropNotMappedPhaseViewDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropNotMappedPhaseViewDetailReport").jqGrid({
        url: '/ProposalReports/PropNotMappedPhaseViewListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "Total Proposals"],
        colModel: [
            { name: "Phase", width: 230, align: 'center', height: 'auto', sortable: true },
            { name: "TotalProposals", width: 230, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }
        ],
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropNotMappedPhaseViewDetailReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "Phasewise Distribution for [State: " + (stateName == "" ? "All State" : stateName) + "]",
        height: 230,
        //width: '100%',
        sortname: 'Phase',
        //shrinktofit: false,       
        footerrow: true,
        loadComplete: function () {
            $("#tbPropNotMappedPhaseViewDetailReport").jqGrid('setGridWidth', '50%', true);
            //Total of Columns
            var TotalProposalsT = $(this).jqGrid('getCol', 'TotalProposals', false, 'sum');

            ////
            $(this).jqGrid('footerData', 'set', { Phase: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalProposals: TotalProposalsT }, true);
            $('#tbPropNotMappedPhaseViewDetailReport_rn').html('Sr.<br/>No.');

            if (!$('#accrodiontabState').is(':visible')) {
                // $("#tbPropNotMappedPhaseViewDetailReport").jqGrid('setGridHeight', 200, true);
                // $('#accordionPropNotMap').height(400);
            }
            $.unblockUI();
        },
        onSelectRow: function (id) {

            if (stateCode > 0) {
                var params = id.split('$');
                //LoadPropNotMappedDistrictDetailGrid(stateCode, stateName, params[0], sanctioned)
                //LoadPropNotMappedDetailGrid(stateCode, stateName, params[0] + "-" + (parseInt(params[0]) + 1), 0, "", params[0], sanctioned);
                LoadPropNotMappedDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, params[0], params[0], batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
                LoadPropNotMappedDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, params[0], params[0] + "-" + (parseInt(params[0]) + 1), batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
            }
            else {
                alert("Proposal Listing not available when in All States View \n Please Select a state in State View")
            }
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropNotMappedDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropNotMappedStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropNotMappedDistrictDetailReport").jqGrid('GridUnload');
    $("#tbPropNotMappedDetailReport").jqGrid('GridUnload');

    jQuery("#tbPropNotMappedDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropNotMappedDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "Phase[District]", "Total Proposals"],
        colModel: [
             { name: "Phase", width: 200, align: 'center', height: 'auto', sortable: false },
            { name: "DistrictName", width: 230, align: 'center', height: 'auto', sortable: false },
            { name: "TotalProposals", width: 220, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }
        ],
        //postData: { "StateCode": stateCode, "StateName": stateName, "Sanctioned": sanctioned, "Year": year },
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropNotMappedDistrictDetailReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Phasewise Details [State: " + stateName + "]",
        height: 310,
        // autowidth: true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['Phase'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {
            // $("#tbPropNotMappedDistrictDetailReport").jqGrid('setGridWidth', $('#tabMain').width() / 2, true);
            // $("#tbPropNotMappedDistrictDetailReport").jqGrid('setGridWidth', '480%', true);

            //Total of Columns
            var TotalProposalsT = $(this).jqGrid('getCol', 'TotalProposals', false, 'sum');
            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalProposals: TotalProposalsT }, true);
            $('#tbPropNotMappedDistrictDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            gDistrictCode = params[0];
            gDistrictName = params[1];
            gPhase = params[3];
            //LoadPropNotMappedDetailGrid(stateCode, stateName, params[3], params[0], params[1], params[2], sanctioned)
            LoadPropNotMappedDetailGrid(stateCode, stateName, params[0], params[1], blockCode, blockName, params[2], params[3], batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropNotMappedDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropNotMappedStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropNotMappedDistrictDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropNotMappedDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropNotMappedDetailReport").jqGrid({
        url: '/ProposalReports/PropNotMappedDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Package", "Road Name", "Construction Type", "Pavement Length (Kms.)", "Sanction Cost (Rs. Lacs)", "Status"],
        colModel: [
            { name: "DistrictName", width: 140, align: 'center', height: 'auto', sortable: true },
            { name: "BlockName", width: 133, align: 'center', height: 'auto', sortable: false },
            { name: "PackageId", width: 140, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "ConstructionType", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "IMSPavLength", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SancCost", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Status", width: 100, align: 'center', height: 'auto', sortable: false }
        ],
        // postData: { "StateCode": stateCode, "Sanctioned": sanctioned, "DistrictCode": districtCode, "Year": year },
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropNotMappedDetailReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposals Details [State : " + stateName + " ,  District :  " + (districtName == "" ? "All District" : districtName) + "" + ",   Phase : " + (yearName == "" ? "All Phase" : yearName) + "]",
        height: 150,
        sortname: 'DistrictName',
        footerrow: true,
        // shrinkToFit: false,
        loadComplete: function () {
            //$("#tbPropNotMappedDetailReport").jqGrid('setGridWidth', $('#tabMain').width(), true);
            //$("#tbPropNotMappedDetailReport").jqGrid('setGridWidth', '1070%', true);
            //Total of Columns
            $("#tbPropNotMappedDetailReport").jqGrid('setGridWidth', $('#divPropNotMappedDetail').width(), true);
            var IMSPavLengthT = $(this).jqGrid('getCol', 'IMSPavLength', false, 'sum');
            IMSPavLengthT = parseFloat(IMSPavLengthT).toFixed(3);
            var SancCostT = $(this).jqGrid('getCol', 'SancCost', false, 'sum');
            SancCostT = parseFloat(SancCostT).toFixed(2);



            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { IMSPavLength: IMSPavLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SancCost: SancCostT }, true);
            $('#tbPropNotMappedDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}



//End Tab 1  Proposals Not Mapped

//Start Tab 2 Road Number Based CN 
function LoadPropNumberBaseCNDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropNumberBaseCNDistrictDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropNumberBaseCNDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropNumberBaseCNDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "Block", "Road Number", "Duplicates"],
        colModel: [
            { name: "StateName", width: 150, align: 'left', height: 'auto', sortable: false, hidden: false },
            { name: "DistrictName", width: 150, align: 'left', height: 'auto', sortable: false },
            { name: "BlockName", width: 150, align: 'center', height: 'auto', sortable: false },
            { name: "RoadNumber", width: 250, align: 'center', height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>{0} Roads</b>' },
            { name: "TotalProposals", width: 250, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }
        ],
        // postData: { "StateCode": stateCode },
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode },

        pager: jQuery('#dvPropNumberBaseCNDistrictDetailReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        rownumbers: true,
        caption: "&nbsp;&nbsp; Districtwise Details  [State : " + stateName + "]",
        height: 250,
        //width: 800,
        // shrinkToFit: true,
        // autowidth: false,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['StateName', 'DistrictName'],
            groupSummary: [true, true],
            groupColumnShow: [false, false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function (data) {
            //$("#tbPropNumberBaseCNDistrictDetailReport").jqGrid('setGridWidth', $('#tabMain').width() / 2, true);

            //Total of Columns
            var TotalProposalsT = $(this).jqGrid('getCol', 'TotalProposals', false, 'sum');

            ////

            $(this).jqGrid('footerData', 'set', { RoadNumber: 'Total : ' + data["records"].toString() + ' Roads' }, true);
            $(this).jqGrid('footerData', 'set', { TotalProposals: TotalProposalsT }, true);
            $('#tbPropNumberBaseCNDistrictDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            // alert("test");

            var params = id.split('$');
            // LoadPropNumberBaseCNRoadDetailGrid(params[0], params[1], params[2],params[4], params[3], params[5], 'R');

            $('#tbPropNumberBaseCNRoadDetailReport').jqGrid("setGridParam", { "postData": { "StateCode": params[0], "DistrictCode": params[2], "BlockCode": params[3], "RoadNumber": params[5], "Report": "R" } });
            $('#tbPropNumberBaseCNRoadDetailReport').trigger("reloadGrid", [{ page: 1 }]);
            $("#tbPropNumberBaseCNRoadDetailReport").jqGrid('setCaption', "&nbsp;&nbsp;Road Details [State : " + params[1] + " ,  District :  " + (params[4] == "" ? "All District" : params[4]) + " ]");

        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }

    }); //end of grid

}

function LoadPropNumberBaseCNRoadDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, roadNumber, report) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropNumberBaseCNDistrictDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropNumberBaseCNDistrictDetailReport").jqGrid('setSelection', districtCode);
    $("#tbPropNumberBaseCNRoadDetailReport").jqGrid('GridUnload');
    //alert("test2");

    jQuery("#tbPropNumberBaseCNRoadDetailReport").jqGrid({
        url: '/ProposalReports/PropNumberBaseCNRoadDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Road Number", "Road  Name", "Road  Category", "Road  Length (Kms.)", "Start Chainage", "End Chainage"],
        colModel: [
            { name: "DistrictName", width: 160, align: 'center', height: 'auto', sortable: true },
            { name: "BlockName", width: 170, align: 'center', height: 'auto', sortable: false },
            { name: "RoadNumber", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "RoadCategory", width: 140, align: 'left', height: 'auto', sortable: false },
            { name: "RoadLength", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "StartChainage", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "EndChainage", width: 90, align: 'center', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        // postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "RoadNumber": roadNumber, "Report": "R" },
        postData: { "RoadNumber": roadNumber, "Report": "R", "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode },

        pager: jQuery('#dvPropNumberBaseCNRoadDetailReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Road Details [State : " + stateName + " ,  District :  " + (districtName == "" ? "All District" : districtName) + " ]",
        height: 250,
        //width: '1160',
        // shrinkToFit: false,
        sortname: 'DistrictName',
        // autowidth: false,
        footerrow: true,
        loadComplete: function (data) {
            //$("#divPropNumberBaseCNRoadDetail").css('width', $("#tblRptContents").width() - 125);  //under Report section its working
            // $("#tbPropNumberBaseCNRoadDetailReport").jqGrid('setGridWidth', $("#dvBaseCN").width()); //under Report section its working
            // $("#tbPropNumberBaseCNRoadDetailReport").css('width', "100%", true);

            $("#divPropNumberBaseCNRoadDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropNumberBaseCNRoadDetailReport").jqGrid('setGridWidth', $("#divPropNumberBaseCNRoadDetail").width());

            //Total of Columns         

            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);

            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: 'Total ' }, true);
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLengthT }, true);
            $('#tbPropNumberBaseCNRoadDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}
//End Tab 2 Road Number Based CN 


//Start of Tab 3 Multiple Proposals mapped to CN Road 
//according to DistrictWise Check Box Check Show Grid
function LoadPropMultipleDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropMultipleDistrictDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropMultipleDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropMultipleDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "CN Road Number", "No. of Proposals Mapped"],
        colModel: [
            { name: "DistrictName", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "BlockName", width: 120, align: 'center', height: 'auto', sortable: false },
            { name: "RoadNumber", width: 280, align: 'center', height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>{0} Roads</b>' },
            { name: "TotalProposals", width: 280, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }
        ],
        //postData: { "StateCode": stateCode, "Sanctioned": sanctioned },
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropMultipleDistrictReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Districtwise Distribution [State: " + stateName + "]",
        //height: 120,
        height: 250,
        // width: 500,
        // shrinkToFit: false,
        // autowidth: false,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['DistrictName', 'BlockName'],
            groupSummary: [true, true],
            groupColumnShow: [false, false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function (data) {
            $("#tbPropMultipleDistrictDetailReport").jqGrid('setGridWidth', $('#tabMain').width() / 2, true);

            //Total of Columns
            var TotalProposalsT = $(this).jqGrid('getCol', 'TotalProposals', false, 'sum');

            ////

            $(this).jqGrid('footerData', 'set', { RoadNumber: 'Total : ' + data["records"].toString() + ' Roads' }, true);
            $(this).jqGrid('footerData', 'set', { TotalProposals: TotalProposalsT }, true);
            $('#tbPropMultipleDistrictDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            //LoadPropMultipleDetailGrid(stateCode, stateName, districtCode, blockCode, roadNumber, sanctioned, report, districtName, blockName)
            //LoadPropMultipleDetailGrid(params[0], params[1], params[2], params[3], params[4], params[5], "R", params[6], params[7]);
            LoadPropMultipleDetailGrid(params[0], params[1], params[2], params[6], params[3], params[7], yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, params[4], params[5], "R");
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropMultipleDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, roadNumber, sanctioned, report) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropMultipleDistrictDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropMultipleDistrictDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropMultipleDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropMultipleDetailReport").jqGrid({
        url: '/ProposalReports/PropMultipleDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "T/L Number", "Phase", "Package", "Road Name", "Construction Type", "Pavement Length (Kms.)", "Sanctioned Cost (Rs. Lacs)", "Stream", "Status"],
        colModel: [
            { name: "DistrictName", width: 90, align: 'center', height: 'auto', sortable: true },
            { name: "BlockName", width: 85, align: 'center', height: 'auto', sortable: false },
            { name: "RoadNumber", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "IMSYear", width: 70, align: 'center', height: 'auto', sortable: false },
            { name: "PakageId", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 220, align: 'left', height: 'auto', sortable: false },
            { name: "ConstructionType", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "PavLength", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SanctionCost", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PMGSYScheme", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "Status", width: 100, align: 'center', height: 'auto', sortable: false }
        ],
        //postData: { "StateCode": stateCode, "Sanctioned": sanctioned, "RoadNumber": roadNumber, "Report": report, "DistrictCode": districtCode, "BlockCode": blockCode },
        postData: {
            "RoadNumber": roadNumber, "Report": report, "Sanctioned": sanctioned, "StateCode": $('#ddState_PropMultipleDetails').val(), "DistrictCode": $('#ddDistrict_PropMultipleDetails').val(), "BlockCode": $('#ddBlock_PropMultipleDetails').val(), "BlockCode": $('#ddBlock_PropMultipleDetails').val(),
            "YearCode": $('#ddYear_PropMultipleDetails').val(), "BatchCode": $('#ddBatch_PropMultipleDetails').val(), "CollaborationCode": $('#ddCollaboration_PropMultipleDetails').val(), "AgencyCode": $('#ddAgency_PropMultipleDetails').val(),
            "StaStatusCode": $('#ddStaStatus_PropMultipleDetails').val()
        },
        pager: jQuery('#dvPropMultipleReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposals Details [State :" + stateName + " ,  District :  " + (districtName == "" ? "All District" : districtName) + " ,  Block :  " + (blockName == "" ? "All Block" : blockName) + " ]",
        height: 250,
        //  width: 1160,
        //shrinkToFit: false,
        sortname: 'DistrictName',

        //autowidth: false,
        footerrow: true,

        loadComplete: function () {
            //$("#tbPropMultipleDetailReport").jqGrid('setGridWidth', "100%", true);
            //$("#divPropMultipleDetail").css('width', $("#tblRptContents").width() - 125); //under Report section its working
            //$("#tbPropMultipleDetailReport").jqGrid('setGridWidth', $("#divPropMultipleDetail").width(), true);  //under Report section its working
            $("#divPropMultipleDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropMultipleDetailReport").jqGrid('setGridWidth', $("#divPropMultipleDetail").width());
            ////Total of Columns
            var PavLengthT = $(this).jqGrid('getCol', 'PavLength', false, 'sum');
            PavLengthT = parseFloat(PavLengthT).toFixed(3);
            var SanctionCostT = $(this).jqGrid('getCol', 'SanctionCost', false, 'sum');
            SanctionCostT = parseFloat(SanctionCostT).toFixed(2);

            //////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PavLength: PavLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SanctionCost: SanctionCostT }, true);
            $('#tbPropMultipleDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

//End of Tab 3 Multiple Proposals mapped to CN Road 


//Start of Tab 4 Single Habitation
function LoadPropSingleHabStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropSingleHabStateDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropSingleHabStateDetailReport").jqGrid({
        url: '/ProposalReports/PropSingleHabStateListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "No. of Roads"],
        colModel: [
            { name: "StateName", width: 240, align: 'center', height: 'auto', sortable: true },
            { name: "NoOfRoads", width: 240, align: 'center', height: 'auto', sortable: false }

        ],
        //postData: { "Population": population, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "Population": population, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropSingleHabStateReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Statewise Distribution",
        //height: 150,
        height: 250,
        // width: 520,
        // shrinkToFit: false,
        // autowidth: false,
        footerrow: true,
        sortname: 'StateName',

        loadComplete: function () {
            // $("#tbPropSingleHabStateDetailReport").jqGrid('setGridWidth', $('#tabMain').width() / 2, true);
            // $("#tbPropSingleHabStateDetailReport").jqGrid('setGridWidth', 520, true);
            //Total of Columns
            var NoOfRoadsT = $(this).jqGrid('getCol', 'NoOfRoads', false, 'sum');

            ////

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { NoOfRoads: NoOfRoadsT }, true);
            $('#tbPropSingleHabStateDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            LoadPropSingleHabDistrictDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
            LoadPropSingleHabDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropSingleHabDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //  $("#tbPropSingleHabStateDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropSingleHabStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropSingleHabDistrictDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropSingleHabDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropSingleHabDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "Phase [District]", "No. of Roads"],
        colModel: [
            { name: "Phase", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "DistrictName", width: 250, align: 'center', height: 'auto', sortable: false },
            { name: "NoOfRoads", width: 250, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }

        ],
        //postData: { "StateCode": stateCode, "Population": population, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "Population": population, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropSingleHabDistrictReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Districtwise Details [State: " + stateName + " ]",
        height: 250,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['Phase'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {
            // $("#tbPropSingleHabDistrictDetailReport").jqGrid('setGridWidth', $("#divPropSingleHabDistrictDetail").width());

            //Total of Columns
            var NoOfRoadsT = $(this).jqGrid('getCol', 'NoOfRoads', false, 'sum');

            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { NoOfRoads: NoOfRoadsT }, true);
            $('#tbPropSingleHabDistrictDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');

            //LoadPropSingleHabDetailGrid(params[0], stateName, params[1], params[2], population, params[3], batch, sanctioned);
            LoadPropSingleHabDetailGrid(params[0], stateName, params[1], params[2], blockCode, blockName, params[3], yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropSingleHabDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, population) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropSingleHabStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropSingleHabDistrictDetailReport").jqGrid('setSelection', districtCode);
    $("#tbPropSingleHabDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropSingleHabDetailReport").jqGrid({
        url: '/ProposalReports/PropSingleHabDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Phase", "Package", "Road Name", "Habitation", "Population", "Construction Type", "Pavement Length (Kms.)", "Sanctioned Cost (Rs. in lacs)", "Stream"],
        colModel: [
            { name: "DistrictName", width: 80, align: 'center', height: 'auto', sortable: true },
            { name: "Block", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "Phase", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "Package", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 160, align: 'left', height: 'auto', sortable: false },
            { name: "Habitation", width: 90, align: 'left', height: 'auto', sortable: false },
            { name: "Population", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ConstructionType", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "PavmentLangth", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SanctionedCost", width: 80, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Stream", width: 100, align: 'center', height: 'auto', sortable: false }

        ],
        // postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Population": population, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "Population": population, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropSingleHabReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details [State:" + stateName + " , District: " + (districtName == "" ? "All District" : districtName) + ",  Phase:  " + (yearCode == 0 ? "All Phase" : (yearCode + "-" + (parseInt(yearCode) + 1))) + "]",
        height: 250,
        footerrow: true,
        //shrinkToFit: false,
        sortname: 'DistrictName',
        loadComplete: function () {
            //$("#divPropSingleHabDetail").css('width', $("#tblRptContents").width() - 130);  //under Report section its working
            //$("#tbPropSingleHabDetailReport").jqGrid('setGridWidth', $("#divPropSingleHabDetail").width(), true);  //under Report section its working

            $("#divPropSingleHabDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropSingleHabDetailReport").jqGrid('setGridWidth', $("#divPropSingleHabDetail").width());

            //Total of Columns
            var PopulationT = $(this).jqGrid('getCol', 'Population', false, 'sum');
            PopulationT = parseFloat(PopulationT).toFixed(2);
            var PavmentLangthT = $(this).jqGrid('getCol', 'PavmentLangth', false, 'sum');
            PavmentLangthT = parseFloat(PavmentLangthT).toFixed(3);
            var SanctionedCostT = $(this).jqGrid('getCol', 'SanctionedCost', false, 'sum');
            SanctionedCostT = parseFloat(SanctionedCostT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { Population: PopulationT }, true);
            $(this).jqGrid('footerData', 'set', { PavmentLangth: PavmentLangthT }, true);
            $(this).jqGrid('footerData', 'set', { SanctionedCost: SanctionedCostT }, true);
            $('#tbPropSingleHabDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}
//End of Tab 4 Single Habitation


//Start of Tab 5 Zero Maintenance Cost

function LoadPropZeroMaintStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropZeroMaintStateDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropZeroMaintStateDetailReport").jqGrid({
        url: '/ProposalReports/PropZeroMaintStateListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "No of Proposals"],
        colModel: [
            { name: "StateName", width: 240, align: 'center', height: 'auto', sortable: true },
            { name: "NoOfProposals", width: 240, align: 'center', height: 'auto', sortable: false }

        ],
        // postData: { "ConstructionType": constructionType, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "ConstructionType": constructionType, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropZeroMaintStateReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Statewise Distribution",
        height: 250,
        footerrow: true,
        sortname: 'StateName',

        loadComplete: function () {

            //Total of Columns
            var NoOfProposalsT = $(this).jqGrid('getCol', 'NoOfProposals', false, 'sum');

            ////

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { NoOfProposals: NoOfProposalsT }, true);
            $('#tbPropZeroMaintStateDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            //LoadPropZeroMaintDistrictDetailGrid(params[0], params[1], constructionType, sanctioned, year, batch);
            //LoadPropZeroMaintDetailGrid(params[0], params[1], 0, "", constructionType, sanctioned, year, batch)
            LoadPropZeroMaintDistrictDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
            LoadPropZeroMaintDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropZeroMaintDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // $("#tbPropZeroMaintStateDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropZeroMaintStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropZeroMaintDistrictDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropZeroMaintDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropZeroMaintDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "District", "No of Proposals"],
        colModel: [
              { name: "Phase", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "DistrictName", width: 240, align: 'center', height: 'auto', sortable: false },
            { name: "NoOfProposals", width: 240, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }

        ],
        //postData: { "StateCode": stateCode, "constructionType": constructionType, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "ConstructionType": constructionType, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropZeroMaintDistrictReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Districtwise Details  [State : " + stateName + "]",
        height: 250,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['Phase'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {

            //Total of Columns
            var NoOfProposalsT = $(this).jqGrid('getCol', 'NoOfProposals', false, 'sum');

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { NoOfProposals: NoOfProposalsT }, true);
            $('#tbPropZeroMaintDistrictDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            //LoadPropZeroMaintDetailGrid(params[0], stateName, params[1], params[2], constructionType, sanctioned, params[3], batch);
            LoadPropZeroMaintDetailGrid(params[0], stateName, params[1], params[2], blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropZeroMaintDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, constructionType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // $("#tbPropZeroMaintStateDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropZeroMaintStateDetailReport").jqGrid('setSelection', stateCode);
    //$("#tbPropZeroMaintDistrictDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropZeroMaintDistrictDetailReport").jqGrid('setSelection', districtCode);
    $("#tbPropZeroMaintDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropZeroMaintDetailReport").jqGrid({
        url: '/ProposalReports/PropZeroMaintDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Phase", "Package", "Road Name", "Construction Type", "Pavement Length (Kms.)", "Sanctioned Cost (Rs. in Lacs)", "Stream", "Status"],
        colModel: [
            { name: "DistrictName", width: 100, align: 'center', height: 'auto', sortable: true },
            { name: "Block", width: 120, align: 'center', height: 'auto', sortable: false },
            { name: "Phase", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "Package", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 190, align: 'left', height: 'auto', sortable: false },
            { name: "ConstructionType", width: 110, align: 'center', height: 'auto', sortable: false },
            { name: "PavmentLangth", width: 90, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SanctionedCost", width: 90, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Stream", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "Status", width: 100, align: 'center', height: 'auto', sortable: false }

        ],
        // postData: { "StateCode": stateCode, "DistrictCode": districtCode, "ConstructionType": constructionType, "Year": year, "Batch": batch, "Sanctioned": sanctioned, "Report": "Y" },
        postData: {
            "ConstructionType": constructionType, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropZeroMaintReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details [State: " + stateName + ", District: " + (districtName == "" ? "All Districts" : districtName) + ",  Phase:  " + (yearCode == 0 ? "Selected Year Onwards" : (yearCode + "-" + (parseInt(yearCode) + 1))) + "]",
        sortname: 'DistrictName',
        height: 250,
        footerrow: true,
        //shrinkToFit: false,
        loadComplete: function () {
            // $("#tbPropZeroMaintDetailReport").jqGrid('setGridWidth',$("#tab-5").width()+5.09, true);
            // $("#divPropZeroMaintDetail").css('width', $("#tblRptContents").width() - 125);//under Report section its working
            // $("#tbPropZeroMaintDetailReport").jqGrid('setGridWidth', $("#divPropZeroMaintDetail").width(), true); //under Report section its working

            $("#divPropZeroMaintDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropZeroMaintDetailReport").jqGrid('setGridWidth', $("#divPropZeroMaintDetail").width());

            //Total of Columns

            var PavmentLangthT = $(this).jqGrid('getCol', 'PavmentLangth', false, 'sum');
            PavmentLangthT = parseFloat(PavmentLangthT).toFixed(3);
            var SanctionedCostT = $(this).jqGrid('getCol', 'SanctionedCost', false, 'sum');
            SanctionedCostT = parseFloat(SanctionedCostT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { PavmentLangth: PavmentLangthT }, true);
            $(this).jqGrid('footerData', 'set', { SanctionedCost: SanctionedCostT }, true);
            $('#tbPropZeroMaintDetailReport_rn').html('Sr.<br/>No.');


            $.unblockUI();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

//End of Tab 5 Zero Maintenance Cost


//Start of Tab 6 Carriage way width

function LoadPropCarriageWidthStateDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropCarriageWidthStateDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropCarriageWidthStateDetailReport").jqGrid({
        url: '/ProposalReports/PropCarriageWidthStateListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Carriage Width (Mts.)", "Total Proposals"],
        colModel: [
            { name: "StateName", width: 20, align: 'left', height: 'auto', sortable: false },
            { name: "CarriageWidth", width: 250, align: 'center', height: 'auto', sortable: false },
            { name: "NoOfProposals", width: 250, align: 'center', height: 'auto', sortable: false, summaryType: 'sum' }

        ],
        //postData: { "CarriageWidth": carriageWidth },
        postData: {
            "CarriageWidth": carriageWidth, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode, "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropCarriageWidthStateReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Statewise Distribution [Carriage Width: " + carriageName + "]",
        // height: 150,
        height: 250,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['StateName'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {

            //Total of Columns
            var NoOfProposalsT = $(this).jqGrid('getCol', 'NoOfProposals', false, 'sum');
            ////

            $(this).jqGrid('footerData', 'set', { CarriageWidth: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { NoOfProposals: NoOfProposalsT }, true);
            $('#tbPropCarriageWidthStateDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            LoadPropCarriageWidthDistrictDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, params[2], params[3]);
            LoadPropCarriageWidthDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, params[2], params[3]);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropCarriageWidthDistrictDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // $("#tbPropCarriageWidthStateDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropCarriageWidthStateDetailReport").jqGrid('setSelection', stateCode);
    $("#tbPropCarriageWidthDetailReport").jqGrid('GridUnload');
    $("#tbPropCarriageWidthDistrictDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropCarriageWidthDistrictDetailReport").jqGrid({
        url: '/ProposalReports/PropCarriageWidthDistrictListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "District", "Total Proposals", "Total Pavement Length (Kms.)", "Total Sanctioned Cost (Rs. Lacs)"],
        colModel: [
            { name: "Phase", width: 20, align: 'left', height: 'auto', sortable: false },
            { name: "DistrictName", width: 150, align: 'center', height: 'auto', sortable: false },
            { name: "Proposals", width: 150, align: 'center', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TotalPavmentLength", width: 90, align: 'center', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TotalSanctioned", width: 90, align: 'center', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

        ],
        //postData: { "CarriageWidth": carriageWidth, "StateCode": stateCode },
        postData: {
            "CarriageWidth": carriageWidth, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode, "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropCarriageWidthDistrictReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Phasewise Details[ State: " + stateName + ", Carriage Width: " + carriageName + " ]",
        // height: 150,
        height: 250,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['Phase'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {

            //Total of Columns
            var ProposalsT = $(this).jqGrid('getCol', 'Proposals', false, 'sum');
            var PavmentLangthT = $(this).jqGrid('getCol', 'TotalPavmentLength', false, 'sum');
            PavmentLangthT = parseFloat(PavmentLangthT).toFixed(3);
            var SanctionedCostT = $(this).jqGrid('getCol', 'TotalSanctioned', false, 'sum');
            SanctionedCostT = parseFloat(SanctionedCostT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { Proposals: ProposalsT }, true);
            $(this).jqGrid('footerData', 'set', { TotalPavmentLength: PavmentLangthT }, true);
            $(this).jqGrid('footerData', 'set', { TotalSanctioned: SanctionedCostT }, true);
            $('#tbPropCarriageWidthDistrictDetailReport_rn').html('Sr.<br/>No.');



            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            // LoadPropCarriageWidthDetailGrid(params[0], stateName, params[1], params[2], params[3], carriageWidthName, params[4]);
            LoadPropCarriageWidthDetailGrid(params[0], stateName, params[1], params[2], blockCode, blockName, params[4], yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, params[3], carriageName);
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}

function LoadPropCarriageWidthDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned, carriageWidth, carriageName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // $("#tbPropCarriageWidthStateDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropCarriageWidthStateDetailReport").jqGrid('setSelection', stateCode);
    // $("#tbPropCarriageWidthDistrictDetailReport").jqGrid('setGridState', 'hidden');
    $("#tbPropCarriageWidthDistrictDetailReport").jqGrid('setSelection', districtCode);
    $("#tbPropCarriageWidthDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropCarriageWidthDetailReport").jqGrid({
        url: '/ProposalReports/PropCarriageWidthDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Phase", "Package", "Road Name", "Construction Type", "Pavement Length (Kms.)", "Sanctioned Cost (Rs. Lacs)", "Stream", "Status"],
        colModel: [
            { name: "DistrictName", width: 120, align: 'center', height: 'auto', sortable: true },
            { name: "BlockName", width: 120, align: 'center', height: 'auto', sortable: false },
            { name: "Phase", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "Pakage", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "ConstructionType", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "PavmentLength", width: 70, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SanctionCost", width: 70, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Stream", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "Status", width: 100, align: 'center', height: 'auto', sortable: false }
        ],
        // postData: { "CarriageWidth": carriageWidth, "StateCode": stateCode, "DistrictCode": districtCode, "Year": year },
        postData: {
            "CarriageWidth": carriageWidth, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode, "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropCarriageWidthReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details  [State :  " + (stateName == "" ? "All State" : stateName) + ", District:  " + (districtName == "" ? "All District" : districtName) + ", Carriage Width:  " + (carriageName == "" ? "All Carriage Width" : carriageName) + "]",
        height: 250,
        //  width: 1135,
        //shrinkToFit: false,
        sortname: 'DistrictName',
        footerrow: true,
        loadComplete: function () {
            // $("#tbPropCarriageWidthDetailReport").jqGrid('setGridWidth', $("#tab-6").width()+2, true);
            //$("#divPropCarriageWidthDetail").css('width', $("#tblRptContents").width() - 130);
            //$("#tbPropCarriageWidthDetailReport").jqGrid('setGridWidth', $("#divPropCarriageWidthDetail").width(), true);
            $("#divPropCarriageWidthDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropCarriageWidthDetailReport").jqGrid('setGridWidth', $("#divPropCarriageWidthDetail").width());

            //Total of Columns

            var PavmentLengthT = $(this).jqGrid('getCol', 'PavmentLength', false, 'sum');
            PavmentLengthT = parseFloat(PavmentLengthT).toFixed(3);
            var SanctionedCostT = $(this).jqGrid('getCol', 'SanctionCost', false, 'sum');
            SanctionedCostT = parseFloat(SanctionedCostT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { PavmentLength: PavmentLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SanctionCost: SanctionedCostT }, true);
            $('#tbPropCarriageWidthDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}
//End of Tab 6 Carriage way width




//Start of Tab 7 Variation in Proposed
function LoadPropVariationLengthDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropVariationLengthDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropVariationLengthDetailReport").jqGrid({
        url: '/ProposalReports/PropVariationLengthListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Block", "Connectivity Type", "CN Road Number", "Road Name", "Road From", "Road To", "Proposed Length (Kms.)", "CN Length (Kms.)", "Sanction Length (Previous) (Kms.)", "Extra Length (Kms.)", "% Variation in Length"],
        colModel: [
            { name: "StateName", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "BlockName", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "ConnectivityType", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "CNRoadNumber", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 150, align: 'left', height: 'auto', sortable: false },
            { name: "RoadFrom", width: 125, align: 'left', height: 'auto', sortable: false },
            { name: "RoadTo", width: 125, align: 'left', height: 'auto', sortable: false },
            { name: "ProposedLength", width: 90, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CNLength", width: 90, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "SanctionedLength", width: 90, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "ExtraLength", width: 80, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PercenVariation", width: 90, align: 'center', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        // postData: { "StateCode": stateCode, "Year": year, "Batch": batch, "Sanctioned": sanctioned },
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode, "StaStatusCode": staStatusCode
        },
        pager: jQuery('#dvPropVariationLengthReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details",
        height: 380,
        //width: '1240',
        //shrinkToFit: false,
        //autoWidth:true,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['StateName'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {
            //$("#divPropVariationLengthDetail").css('width', $("#tblRptContents").width() - 120);
            //$("#tbPropVariationLengthDetailReport").jqGrid('setGridWidth', $("#divPropVariationLengthDetail").width(), true);
            $("#divPropVariationLengthDetail").css('width', $("#tabMain").width() - 28);
            $("#tbPropVariationLengthDetailReport").jqGrid('setGridWidth', $("#divPropVariationLengthDetail").width());
            //Total of Columns
            var ProposedLengthT = $(this).jqGrid('getCol', 'ProposedLength', false, 'sum');
            ProposedLengthT = parseFloat(ProposedLengthT).toFixed(3);
            var CNLengthT = $(this).jqGrid('getCol', 'CNLength', false, 'sum');
            CNLengthT = parseFloat(CNLengthT).toFixed(3);
            var SanctionedLengthT = $(this).jqGrid('getCol', 'SanctionedLength', false, 'sum');
            SanctionedLengthT = parseFloat(SanctionedLengthT).toFixed(3);
            var ExtraLengthT = $(this).jqGrid('getCol', 'ExtraLength', false, 'sum');
            ExtraLengthT = parseFloat(ExtraLengthT).toFixed(3);
            var PercenVariationT = $(this).jqGrid('getCol', 'PercenVariation', false, 'sum');
            PercenVariationT = parseFloat(PercenVariationT).toFixed(2);

            ////

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { ProposedLength: ProposedLengthT }, true);
            $(this).jqGrid('footerData', 'set', { CNLength: CNLengthT }, true);
            $(this).jqGrid('footerData', 'set', { SanctionedLength: SanctionedLengthT }, true);
            $(this).jqGrid('footerData', 'set', { ExtraLength: ExtraLengthT }, true);
            $(this).jqGrid('footerData', 'set', { PercenVariation: PercenVariationT }, true);
            $('#tbPropVariationLengthDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid





}
//End of Tab 7 Variation in Proposed

//Start of Tab 8 Misclassification in Proposed 
function LoadPropMisclassificationGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropMisclassificationReport").jqGrid('GridUnload');
    jQuery("#tbPropMisclassificationReport").jqGrid({
        url: '/ProposalReports/PropMisclassificationListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Phase", "Total Proposals", "Road Length (Kms.)", "Bridge Length (Mts.)", "Road Cost (Rs. in Lacs)", "Bridge Cost (Rs. in Lacs)", "Total Cost (Rs. in Lacs)", "Total Proposals", "Road Length (Kms.)", "Bridge Length (Mts.)", "Bridge Cost (Rs. in Lacs)", "Road Cost (Rs. in Lacs)", "Total Cost (Rs. in Lacs)", "Total Cost (Rs. in Lacs)"],
        colModel: [
            { name: "StateName", width: 90, align: 'center', height: 'auto', sortable: false },
            { name: "Phase", width: 80, align: 'center', height: 'auto', sortable: false },
            { name: "RTotalProp", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "RRoadLength", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "RBridgeLength", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "RRoadCost", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "RBridgeCost", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "RTotalCost", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BTotalProp", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRoadLength", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BBridgeLength", width: 70, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BBridgeCost", width: 80, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRoadCost", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BTotalCost", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TotalCost", width: 100, align: 'center', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        // postData: { "StateCode": stateCode },
        postData: {
            "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropMisclassificationPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details",
        //height: 150,
        height: 200,
        //width: 1240,
        // autowidth: false,     
        //shrinkToFit: false,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['StateName'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: collapse,
        },
        loadComplete: function () {
            // $("#tbPropMisclassificationReport").jqGrid('setGridWidth', "100%", true);
            //$("#divPropMisclassification").css('width', $("#tblRptContents").width() - 135);
            //$("#tbPropMisclassificationReport").jqGrid('setGridWidth', $("#divPropMisclassification").width(), true);
            $("#divPropMisclassification").css('width', $("#tabMain").width() - 30);
            $("#tbPropMisclassificationReport").jqGrid('setGridWidth', $("#divPropMisclassification").width());

            var RTotalPropT = $(this).jqGrid('getCol', 'RTotalProp', false, 'sum');
            RTotalPropT = parseFloat(RTotalPropT).toFixed(2);
            var RRoadLengthT = $(this).jqGrid('getCol', 'RRoadLength', false, 'sum');
            RRoadLengthT = parseFloat(RRoadLengthT).toFixed(3);
            var RBridgeLengthT = $(this).jqGrid('getCol', 'RBridgeLength', false, 'sum');
            RBridgeLengthT = parseFloat(RBridgeLengthT).toFixed(3);
            var RRoadCostT = $(this).jqGrid('getCol', 'RRoadCost', false, 'sum');
            RRoadCostT = parseFloat(RRoadCostT).toFixed(2);
            var RBridgeCostT = $(this).jqGrid('getCol', 'RBridgeCost', false, 'sum');
            RBridgeCostT = parseFloat(RBridgeCostT).toFixed(2);
            var RTotalCostT = $(this).jqGrid('getCol', 'RTotalCost', false, 'sum');
            RTotalCostT = parseFloat(RTotalCostT).toFixed(2);

            var BTotalPropT = $(this).jqGrid('getCol', 'BTotalProp', false, 'sum');
            BTotalPropT = parseFloat(BTotalPropT).toFixed(2);
            var BRoadLengthT = $(this).jqGrid('getCol', 'BRoadLength', false, 'sum');
            BRoadLengthT = parseFloat(BRoadLengthT).toFixed(3);
            var BBridgeLengthT = $(this).jqGrid('getCol', 'BBridgeLength', false, 'sum');
            BBridgeLengthT = parseFloat(BBridgeLengthT).toFixed(3);
            var BRoadCostT = $(this).jqGrid('getCol', 'BRoadCost', false, 'sum');
            BRoadCostT = parseFloat(BRoadCostT).toFixed(2);
            var BBridgeCostT = $(this).jqGrid('getCol', 'BBridgeCost', false, 'sum');
            BBridgeCostT = parseFloat(BBridgeCostT).toFixed(2);
            var BTotalCostT = $(this).jqGrid('getCol', 'BTotalCost', false, 'sum');
            BTotalCostT = parseFloat(BTotalCostT).toFixed(2);

            var TotalCostT = $(this).jqGrid('getCol', 'TotalCost', false, 'sum');
            TotalCostT = parseFloat(TotalCostT).toFixed(2);

            ////

            $(this).jqGrid('footerData', 'set', { Phase: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { RTotalProp: RTotalPropT }, true);
            $(this).jqGrid('footerData', 'set', { RRoadLength: RRoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RBridgeLength: RBridgeLengthT }, true);
            $(this).jqGrid('footerData', 'set', { RRoadCost: RRoadCostT }, true);
            $(this).jqGrid('footerData', 'set', { RBridgeCost: RBridgeCostT }, true);
            $(this).jqGrid('footerData', 'set', { RTotalCost: RTotalCostT }, true);

            $(this).jqGrid('footerData', 'set', { BTotalProp: BTotalPropT }, true);
            $(this).jqGrid('footerData', 'set', { BRoadLength: BRoadLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BBridgeLength: BBridgeLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BRoadCost: BRoadCostT }, true);
            $(this).jqGrid('footerData', 'set', { BBridgeCost: BBridgeCostT }, true);
            $(this).jqGrid('footerData', 'set', { BTotalCost: BTotalCostT }, true);

            $(this).jqGrid('footerData', 'set', { TotalCost: TotalCostT }, true);

            $("#dvPropMisclassificationPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>[Click on Road Cost / Bridge Cost Column values to view proposal details]</font>");
            $('#tbPropMisclassificationReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onCellSelect: function (row, col, content, event) {
            var params = row.split('$');
            var type; //P ,L
            var cm = jQuery("#tbPropMisclassificationReport").jqGrid("getGridParam", "colModel");
            var colname = cm[col].name;
            var value = content;

            if ((colname == "RRoadCost") || (colname == "RBridgeCost")) {
                if (parseInt(value) > 0) {
                    type = 'P'; //Road Proposal              
                    //LoadPropMisclassificationDetailGrid(params[0], params[1], params[3], params[2], type, 'R');
                    LoadPropMisclassificationDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, params[2], params[2], batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, params[3], type, 'R', sanctioned);
                }
                else {
                    $("#tbPropMisclassificationDetailReport").jqGrid('GridUnload');
                }
            }
            else if ((colname == "BBridgeCost") || (colname == "BRoadCost")) {
                if (parseInt(value) > 0) {

                    type = 'L'; //Bridge Proposal

                    // LoadPropMisclassificationDetailGrid(params[0], params[1], params[3], params[2], type, 'R');
                    LoadPropMisclassificationDetailGrid(params[0], params[1], districtCode, districtName, blockCode, blockName, params[2], params[2], batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, params[3], type, 'R', sanctioned);

                }
                else {
                    $("#tbPropMisclassificationDetailReport").jqGrid('GridUnload');
                }
            }
            else {
                $("#tbPropMisclassificationDetailReport").jqGrid('GridUnload');
            }
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid

    $("#tbPropMisclassificationReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'RTotalProp', numberOfColumns: 6, titleText: '<em>Road Proposals </em>' },
          { startColumnName: 'BTotalProp', numberOfColumns: 6, titleText: '<em>Bridge Proposals</em>' }
        ]
    });



}

function LoadPropMisclassificationDetailGrid(stateCode, stateName, districtCode, districtName, blockCode, blockName, yearCode, yearName, batchCode, batchName, collabCode, collabName, agencyCode, agencyName, staStatusCode, staStatusName, mrdStatusCode, mrdStatusName, phase, type, report, sanctioned) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbPropMisclassificationDetailReport").jqGrid('GridUnload');
    jQuery("#tbPropMisclassificationDetailReport").jqGrid({
        url: '/ProposalReports/PropMisclassificationDetailsListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Package", "Stream", "Road Name", "Construction Type", "Pavement Length (Kms.)", "Total Pavement Cost (Rs. in Lacs)", "Bridge Length (Mts.)", "Total Bridge Cost (Rs. in Lacs)", "Status"],
        colModel: [
            { name: "Package", width: 100, align: 'center', height: 'auto', sortable: true },
            { name: "Stream", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "RoadName", width: 230, align: 'left', height: 'auto', sortable: false },
            { name: "ConstrType", width: 100, align: 'center', height: 'auto', sortable: false },
            { name: "PavLength", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PavCost", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BridgeLength", width: 110, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BridgeCost", width: 110, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Status", width: 120, align: 'center', height: 'auto', sortable: false }
        ],
        // postData: { "StateCode": stateCode, "Year": year, "Type": type, "Report": report },
        postData: {
            "Type": type, "Report": report, "Sanctioned": sanctioned, "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode,
            "YearCode": yearCode, "BatchCode": batchCode, "CollaborationCode": collabCode, "AgencyCode": agencyCode,
            "StaStatusCode": staStatusCode, "MrdStatusCode": mrdStatusCode
        },
        pager: jQuery('#dvPropMisclassificationReportPager'),
        rowNum: 2147483647,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Proposal Details [State: " + stateName + ", Phase: " + phase + " : " + (type == "P" ? "Road Proposals" : "Bridge Proposals") + "]",
        height: 200,
        //width:1140,
        //shrinkToFit: false,
        sortname: 'Package',

        footerrow: true,
        loadComplete: function () {
            // $("#tbPropMisclassificationDetailReport").jqGrid('setGridWidth', "100%", true);
            //$("#divPropMisclassificationDetail").css('width', $("#tblRptContents").width() - 135);
            //$("#tbPropMisclassificationDetailReport").jqGrid('setGridWidth', $("#divPropMisclassificationDetail").width(), true);
            $("#divPropMisclassificationDetail").css('width', $("#tabMain").width() - 30);
            $("#tbPropMisclassificationDetailReport").jqGrid('setGridWidth', $("#divPropMisclassificationDetail").width());

            $("#divPropMisclassification").css('width', $("#tabMain").width() - 30);
            $("#tbPropMisclassificationReport").jqGrid('setGridWidth', $("#divPropMisclassification").width());


            var PavLengthT = $(this).jqGrid('getCol', 'PavLength', false, 'sum');
            PavLengthT = parseFloat(PavLengthT).toFixed(3);
            var PavCostT = $(this).jqGrid('getCol', 'PavCost', false, 'sum');
            PavCostT = parseFloat(PavCostT).toFixed(2);
            var BridgeLengthT = $(this).jqGrid('getCol', 'BridgeLength', false, 'sum');
            BridgeLengthT = parseFloat(BridgeLengthT).toFixed(3);
            var BridgeCostT = $(this).jqGrid('getCol', 'BridgeCost', false, 'sum');
            BridgeCostT = parseFloat(BridgeCostT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { Package: '<b>Total</b> ' }, true);
            $(this).jqGrid('footerData', 'set', { PavLength: PavLengthT }, true);
            $(this).jqGrid('footerData', 'set', { PavCost: PavCostT }, true);
            $(this).jqGrid('footerData', 'set', { BridgeLength: BridgeLengthT }, true);
            $(this).jqGrid('footerData', 'set', { BridgeCost: BridgeCostT }, true);
            $('#tbPropMisclassificationDetailReport_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid



}
//End of Tab 8 Misclassification in Proposed 