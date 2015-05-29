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

    $('button.enqueue').click(function () {
        var action = $(this).attr('value');
        var mode = $(this).parent().prev().find('input[name=mode]:checked').val() || $(this).parent().prev().find('input[name=mode]').val();
        $.get(action + '?format=json&mode=' + mode, function (data) {
            if (data.response[0].status === 200) {
                $().toastmessage('showSuccessToast', '<a style="color:white;" href="' + settings.jobQueueLink + '">Job Enqueued</a>');
            } else {
                $().toastmessage('showErrorToast', data.response[0].message);
            }
        }, "json");
    });

    function removeMessages() {
        $(".zone-messages").fadeOut().hide();
    }

    setTimeout(removeMessages, 10000);
});