$(document).ready(function () {

    $.unblockUI();

    if ($('#ticker_02 > li').length >= 10) {

        startTimer();
    }

    $('#ticker_02 li').mouseenter(function () {
        if ($('#ticker_02 > li').length >= 10) {
            clearInterval(autoInterval, startTimer);
        }
    }).mouseleave(function () {
        if ($('#ticker_02 > li').length >= 10) {
            startTimer();
        }
    });

    $('#btnCloseRequest').click(function () {

        $('#draggable').hide('slow');
        $('#request').show('slow');
    });

    $('#request').click(function () {
        $('#draggable').show('slow');
        $('#request').hide('slow');
    });

});
function tick2() {

    $('#ticker_02 li:first').slideUp(function () { $(this).appendTo($('#ticker_02')).slideDown(); });

}


function startTimer() {
    autoInterval = setInterval(function () { tick2() }, 4000);
}