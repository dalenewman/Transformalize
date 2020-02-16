function ballSoHard(request) {
    var promise = axios.get(request.Url, { params: { format: "json" } });
    var populate = function (response) {
        if (response.data.log === undefined) {
            response.data.log = [];
        }
        for (var i = 0; i < response.data.entities.length; i++) {
            var entity = response.data.entities[i];
            console.log('before ' + entity.alias);
            var evm = new Vue({
                el: "#entity-" + i,
                data: entity,
                computed: {
                    outputFields: function () {
                        var all = this.fields.concat(this.calculatedfields);
                        return all.filter(function (f) {
                            return (f.output === undefined || f.output) && (f.system === undefined || !f.system) && f.alias !== request.BatchValueFieldName && f.alias !== request.ReportRowClassFieldName && f.alias !== request.ReportRowStyleFieldName;
                        });
                    }
                }
            });
            console.log('after ' + evm.alias);
        }
    };
    promise.then(populate).catch(function (error) { });
}
;
