// Map array support
if (![].map) {
    Array.prototype.map = function (callback, self) {
        var array = this, len = array.length, newArray = new Array(len)
        for (var i = 0; i < len; i++) {
            if (i in array) {
                newArray[i] = callback.call(self, array[i], i, array)
            }
        }
        return newArray
    }
}

// Filter array support
if (![].filter) {
    Array.prototype.filter = function (callback) {
        if (this == null) throw new TypeError()
        var t = Object(this), len = t.length >>> 0
        if (typeof callback != 'function') throw new TypeError()
        var newArray = [], thisp = arguments[1]
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i]
                if (callback.call(thisp, val, i, t)) newArray.push(val)
            }
        }
        return newArray
    }
}

$(document).ready(function () {

    $(document).ajaxStart(function () {
        $.blockUI({
            css: {
                message: null,
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff',
                baseZ: 1021
            }
        });
    });

    $(document).ajaxComplete(function () {
        $.unblockUI();
    });

    $(document).ajaxError(function (event, request) {
        $.unblockUI();
        $().toastmessage('showErrorToast', request.responseText);
    });

});
