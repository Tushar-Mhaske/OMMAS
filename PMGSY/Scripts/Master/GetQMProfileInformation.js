
//Added by Hrishikesh to provide Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC

/// <reference path="../jquery-1.9.1.js" />
/// <reference path="../jquery-1.7.2.intellisense.js" />

$("#dvhdCreateNewQualityMonitorDetailsN").click(function () {

    if ($("#dvCreateNewQualityMonitorDetailsN").is(":visible")) {

        $("#closeGridId").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

        $(this).next("#dvCreateNewQualityMonitorDetailsN").slideToggle(300);
    }

    else {
        $("#closeGridId").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

        $(this).next("#dvCreateNewQualityMonitorDetailsN").slideToggle(300);
    }
});