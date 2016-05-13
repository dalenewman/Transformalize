/// <reference path="../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/typings/knockout/knockout.d.ts" />

interface IConnection { name: string; provider: string, server: string, summary: string }
interface IField { name: string; alias: string; label: string; output: boolean; index: number; system: boolean; type: string; orderby: string; order: string, orderObserved: KnockoutObservable<string>, sort: any }
interface IEntity { name: string, connection: string; fields: IField[]; calculatedFields: IField[]; orderby: string }
interface IProcess { entities: IEntity[]; response: IResponse[]; connections: IConnection[] }
interface IResponse { request: string; status: number; message: string; time: number; rows: any[]; hits: number }

$(document).ready(() => {

    if (!String.prototype.startsWith) {
        String.prototype.startsWith = function (searchString, position) {
            position = position || 0;
            return this.indexOf(searchString, position) === position;
        };
    }

    var $nav = $("#nav");
    var $detail = $("#jobs");

    function detailViewModel(): void {
        var self = this;
        self.columns = ko.observableArray();
        self.rows = ko.observableArray();
    }

    function navViewModel(detail: any): void {
        var self = this;
        self.entity = ko.observable("");
        self.pageSize = ko.observable(10);
        self.job = ko.observable(21);
        self.page = ko.observable(1);
        self.hits = ko.observable(0);
        self.connection = ko.observable({ name: "input", provider: "none" });

        self.previousPage = ko.computed(() => (self.page() === 1 ? 1 : self.page() - 1), self);
        self.nextPage = ko.computed(() => {
            var pages = Math.ceil(self.hits() / self.pageSize());
            return self.page() === pages ? self.page() : self.page() + 1;
        }, self);

        self.hasPrevious = ko.computed(() => { return this.page() > 1; }, self);
        self.hasNext = ko.computed(() => {
            var pages = Math.ceil(self.hits() / self.pageSize());
            return self.page() < pages;
        }, self);

        self.pageDescription = ko.computed(() => {
            var pages = Math.ceil(this.hits() / self.pageSize());
            return `${self.hits()} hits: page ${self.page()} of ${pages}`;
        }, self);

        self.summary = ko.computed(() => {
            var connection = self.connection();
            switch (connection.provider) {
                case "mysql":
                case "sqlserver":
                case "postgresql":
                    return `${connection.provider}:${connection.server}.${connection.database}`;
                case "lucene":
                    return `${connection.provider}:${connection.folder}`;
                case "elastic":
                case "solr":
                    return `${connection.provider}:${connection.url}`;
                case "file":
                case "excel":
                    return `${connection.provider}:${connection.file}`;
                default:
                    return connection.provider;
            }
        }, self);

        self.search = () => {
            self.page(1);
        }

        self.next = () => {
            self.page(self.nextPage());
        }

        self.previous = () => {
            self.page(self.previousPage());
        }

        self.go = ko.computed(() => {
            $.ajax({
                dataType: "json",
                url: `/api/page/${self.job()}/${self.page()}`,
                global: false,
                beforeSend: () => {
                    // something
                    detail.columns.removeAll();
                    detail.rows.removeAll();
                },
                success: (process: IProcess) => {
                    if (process.response[0].status === 200) {

                        var entity = process.entities[0];
                        entity.orderby = entity.orderby || "all";
                        self.entity(entity.name);
                        self.connection(process.connections.filter((c: IConnection) => c.name === (entity.connection || "input"))[0]);

                        var fields = entity.fields.filter((f: IField) => !f.system && (typeof f.output === "undefined" || f.output));
                        if (entity.calculatedFields) {
                            var calculatedFields = entity.calculatedFields.filter((f: IField) => (typeof f.output === "undefined" || f.output));
                            fields = fields.concat(calculatedFields);
                        }

                        $.each(fields, (index, field: IField) => {
                            field.orderby = field.orderby || field.name;
                            field.orderObserved = ko.observable(field.order || "none");
                            field.sort = (s) => {
                                if (field.orderObserved() === s) {
                                    field.orderObserved("none");
                                } else {
                                    field.orderObserved(s);
                                }
                            }
                            detail.columns.push(field);
                        });

                        var response = process.response[0];

                        $.each(response.rows, (index, row) => {
                            var data = [];
                            $.each(fields, (i, f: IField) => {
                                var value = row[f.alias];
                                if ((typeof f.type === "undefined" || f.type === "string") && (value.startsWith("/") || value.startsWith("http"))) {
                                    value = `<a target="_blank" href="${value}">${value}</a>`;
                                }
                                data.push(value);
                            });
                            detail.rows.push(data);
                        });

                        self.hits(response.hits);

                    } else {
                        alert(process.response[0].message);
                    }
                },
                complete: () => {
                    // something
                }
            });
        }, self);

    }

    var detailVm = new (<any>detailViewModel)();
    var navVm = new (<any>navViewModel)(detailVm);

    ko.applyBindings(detailVm, $detail[0]);
    ko.applyBindings(navVm, $nav[0]);

    //navVm.search(1);
});



