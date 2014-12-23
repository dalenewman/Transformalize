$(document).ready(function () {

    var xml = $('#Transformalize_Configuration');

    function completeAfter(cm, pred) {
        var cur = cm.getCursor();
        if (!pred || pred()) setTimeout(function () {
            if (!cm.state.completionActive)
                cm.showHint({ completeSingle: false });
        }, 100);
        return CodeMirror.Pass;
    }

    function completeIfAfterLt(cm) {
        return completeAfter(cm, function () {
            var cur = cm.getCursor();
            return cm.getRange(CodeMirror.Pos(cur.line, cur.ch - 1), cur) == "<";
        });
    }

    function completeIfInTag(cm) {
        return completeAfter(cm, function () {
            var tok = cm.getTokenAt(cm.getCursor());
            if (tok.type == "string" && (!/['"]/.test(tok.string.charAt(tok.string.length - 1)) || tok.string.length == 1)) return false;
            var inner = CodeMirror.innerMode(cm.getMode(), tok.state).state;
            return inner.tagName;
        });
    }

    var tags = {
        transformalize: {
            children: ["environments", "processes"]
        },
        parameters: {
            children: ["add"]
        },
        processes: {
            children: ["add"]
        },
        add: {
            children: ["connections", "entities", "fields", "relationships", "calculated-fields"],
            attrs: {
                name: null,
                provider: ["file", "folder", "internal", "sqlserver", "mysql", "postgresql", "elasticsearch", "lucene", "solr"],
                type: ["boolean", "byte", "int16", "short", "int", "int32", "int64", "long", "single", "double", "decimal", "datetime", "guid", "byte[]"]
            }
        }
    };

    var editor = CodeMirror.fromTextArea(xml[0], {
        mode: 'xml',
        indentUnit: 2,
        tabSize: 2,
        htmlMode: false,
        dragDrop: false,
        lineNumbers: true,
        lineWrapping: true,
        viewPortMargin: Infinity,
        theme: settings.theme,
        extraKeys: {
            "'<'": completeAfter,
            "'/'": completeIfAfterLt,
            "' '": completeIfInTag,
            "'='": completeIfInTag,
            "Ctrl-Space": "autocomplete",
            "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); },
            "Ctrl-S": function () { $('.CodeMirror').closest('form').find(':submit').click(); },
            "F11": function (cm) {
                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
            },
            "Esc": function (cm) {
                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
            },
            "Tab": function (cm) {
                var spaces = Array(3).join(" ");
                cm.replaceSelection(spaces);
            }
        },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        matchTags: { bothTags: true },
        hintOptions: { schemaInfo: tags }
    });

    $('#md-button').click(function () {
        window.open(settings.metaDataUrl, '_blank');
    });

    $('#jobs-button').click(function () {
        window.open(settings.jobsUrl, '_self');
    });
});

