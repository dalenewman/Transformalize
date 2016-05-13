/// <reference path="../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/typings/knockout/knockout.d.ts" />
$(document).ready(function () {
    if (!String.prototype.startsWith) {
        String.prototype.startsWith = function (searchString, position) {
            position = position || 0;
            return this.indexOf(searchString, position) === position;
        };
    }
    var $nav = $("#nav");
    var $detail = $("#jobs");
    function detailViewModel() {
        var self = this;
        self.columns = ko.observableArray();
        self.rows = ko.observableArray();
    }
    function navViewModel(detail) {
        var _this = this;
        var self = this;
        self.entity = ko.observable("");
        self.pageSize = ko.observable(10);
        self.job = ko.observable(21);
        self.page = ko.observable(1);
        self.hits = ko.observable(0);
        self.connection = ko.observable({ name: "input", provider: "none" });
        self.previousPage = ko.computed(function () { return (self.page() === 1 ? 1 : self.page() - 1); }, self);
        self.nextPage = ko.computed(function () {
            var pages = Math.ceil(self.hits() / self.pageSize());
            return self.page() === pages ? self.page() : self.page() + 1;
        }, self);
        self.hasPrevious = ko.computed(function () { return _this.page() > 1; }, self);
        self.hasNext = ko.computed(function () {
            var pages = Math.ceil(self.hits() / self.pageSize());
            return self.page() < pages;
        }, self);
        self.pageDescription = ko.computed(function () {
            var pages = Math.ceil(_this.hits() / self.pageSize());
            return self.hits() + " hits: page " + self.page() + " of " + pages;
        }, self);
        self.summary = ko.computed(function () {
            var connection = self.connection();
            switch (connection.provider) {
                case "mysql":
                case "sqlserver":
                case "postgresql":
                    return connection.provider + ":" + connection.server + "." + connection.database;
                case "lucene":
                    return connection.provider + ":" + connection.folder;
                case "elastic":
                case "solr":
                    return connection.provider + ":" + connection.url;
                case "file":
                case "excel":
                    return connection.provider + ":" + connection.file;
                default:
                    return connection.provider;
            }
        }, self);
        self.search = function () {
            self.page(1);
        };
        self.next = function () {
            self.page(self.nextPage());
        };
        self.previous = function () {
            self.page(self.previousPage());
        };
        self.go = ko.computed(function () {
            $.ajax({
                dataType: "json",
                url: "/api/page/" + self.job() + "/" + self.page(),
                global: false,
                beforeSend: function () {
                    // something
                    detail.columns.removeAll();
                    detail.rows.removeAll();
                },
                success: function (process) {
                    if (process.response[0].status === 200) {
                        var entity = process.entities[0];
                        entity.orderby = entity.orderby || "all";
                        self.entity(entity.name);
                        self.connection(process.connections.filter(function (c) { return c.name === (entity.connection || "input"); })[0]);
                        var fields = entity.fields.filter(function (f) { return !f.system && (typeof f.output === "undefined" || f.output); });
                        if (entity.calculatedFields) {
                            var calculatedFields = entity.calculatedFields.filter(function (f) { return (typeof f.output === "undefined" || f.output); });
                            fields = fields.concat(calculatedFields);
                        }
                        $.each(fields, function (index, field) {
                            field.orderby = field.orderby || field.name;
                            field.orderObserved = ko.observable(field.order || "none");
                            field.sort = function (s) {
                                if (field.orderObserved() === s) {
                                    field.orderObserved("none");
                                }
                                else {
                                    field.orderObserved(s);
                                }
                            };
                            detail.columns.push(field);
                        });
                        var response = process.response[0];
                        $.each(response.rows, function (index, row) {
                            var data = [];
                            $.each(fields, function (i, f) {
                                var value = row[f.alias];
                                if ((typeof f.type === "undefined" || f.type === "string") && (value.startsWith("/") || value.startsWith("http"))) {
                                    value = "<a target=\"_blank\" href=\"" + value + "\">" + value + "</a>";
                                }
                                data.push(value);
                            });
                            detail.rows.push(data);
                        });
                        self.hits(response.hits);
                    }
                    else {
                        alert(process.response[0].message);
                    }
                },
                complete: function () {
                    // something
                }
            });
        }, self);
    }
    var detailVm = new detailViewModel();
    var navVm = new navViewModel(detailVm);
    ko.applyBindings(detailVm, $detail[0]);
    ko.applyBindings(navVm, $nav[0]);
    //navVm.search(1);
});
