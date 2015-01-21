$(document).ready(function () {
    $("input:file").change(function () {
        var fileName = $(this).val().replace("C:\\fakepath\\", "");

        $(this).closest('td')
            .siblings('.td-submit')
            .children('button[type=submit]')
            .removeAttr('disabled')
            .removeClass('btn-disabled')
            .addClass('btn-primary');

        $(this).closest('td')
            .siblings('.td-file')
            .text(fileName)
            .removeClass('text-warning')
            .addClass('text-success');
    });

    $('label.mode').click(function () {
        $('input[type=radio]', $(this).parent()).removeAttr('checked');
        $(this).children().first().attr('checked', 'checked');
    });

    function removeMessages() {
        $(".zone-messages").fadeOut().hide();
    }

    setTimeout(removeMessages, 10000);
});
