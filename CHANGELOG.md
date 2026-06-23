# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.4.3] - 2026-06-22

### Changed

- **Dependency alignment with OrchardCore 3.0.0** (`1.4.3`): bumped shared dependencies to match the versions OrchardCore 3.0.0 ships, so projects co-hosted with OrchardCore (e.g. `OrchardCore.Transformalize`) resolve a single, consistent version of each:
  - `System.Text.Json` 10.0.5 → 10.0.8 (`Provider.Json`, `Transform.Json`, `Provider.GeoJson`).
  - `Jint` 4.6.3 → 4.9.2 (`Transform.Jint`, `Validate.Jint`).
  - `MailKit` 4.16.0 → 4.17.0 (`Provider.Mail`).
  - `DocumentFormat.OpenXml` 3.4.1 → 3.5.1 (`Provider.OpenXml`).
  - `Lucene.Net*` 4.8.0-beta00016 → 4.8.0-beta00017 (`Provider.Lucene`).
- The corresponding `.Autofac` wrapper packages were revved alongside their base packages so consumers (which reference the `.Autofac` packages) pick up the updated dependencies.

### Security

- **Patched native SQLite** (`Provider.Sqlite 1.4.3`): pinned `SQLitePCLRaw.bundle_e_sqlite3` to `3.0.3` to resolve **CVE-2025-6965** (NU1903, high severity — memory corruption in SQLite < 3.50.2). `Microsoft.Data.Sqlite` still floors the bundle at the vulnerable, deprecated `2.1.11`, so an explicit pin to the `3.0.x` bundle (which uses `SourceGear.sqlite3` ≥ 3.50.4.5) is the only mitigation. Revisit when `Microsoft.Data.Sqlite` references a non-vulnerable bundle itself.

## [1.4.2] - 2026-05-21

### Changed

- **SQL Server FTS — CONTAINS is now the default** (`Ado 1.4.2`, `Transformalize 1.4.2`): When a field has a `search-type` and the filter `type='search'`, SQL Server now generates `CONTAINS()` by default (previously generated `CONTAINS` without a normalizer). Use `query-type='freetext'` to opt into `FREETEXT()` (natural-language, no operators).
- **CONTAINS auto-normalizer** (`Ado 1.4.2`): User input is automatically normalized before being passed to `CONTAINS()` so common syntax mistakes are fixed transparently:
  - Unquoted prefix terms (`chef*`) are auto-quoted (`"chef*"`).
  - Leading wildcards (`*chai`) are stripped; both-side wildcards (`*chai*`) strip the leading `*` and quote the remainder (`"chai*"`).
  - Bare multi-word input (`chai chang`) is joined with `AND` (`chai AND chang`).
  - Bare `NOT` is promoted to `AND NOT` (`chai NOT chang` → `chai AND NOT chang`).
  - Dangling leading operators (`OR chai`, `AND NOT chai`) are stripped.
  - Dangling trailing operators (`chai AND`, `something* AND somethingelse OR`) are stripped.
  - Consecutive operators (`chai AND OR chang`) keep the first and drop the second (`chai AND chang`).
  - Explicit `AND`, `OR`, `AND NOT`, and `NEAR` operators are preserved, with each operand still individually normalized.
- **`query-type` and `mode` domain validation** (`Transformalize 1.4.2`): `SearchType.QueryType` now validates against `plain,web,phrase,raw,contains,freetext`; `SearchType.Mode` validates against `boolean,natural,expansion`.

## [1.4.1] - 2026-05-19

### Fixed

- **SQL Server provider** (`1.4.1`): Port was ignored when building the connection string. The `DataSource` now uses `server,port` format when a non-zero port is configured.

## [1.4.0] - 2026-05-19

### Added

- **Native full-text search for ADO providers** (`Ado 1.4.0`, `Transformalize 1.4.0`): Fields tagged with a `search-type` whose name references a `<search-type>` element now generate native FTS predicates instead of `LIKE` when `type='search'` filters are applied.
  - **SQL Server** (`1.4.0`): `CONTAINS(field, 'query')` with optional `LANGUAGE 'lang'` via `analyzer`. Supports CONTAINS phrase (`"two words"`), boolean OR/AND, and prefix (`"word*"`) syntax.
  - **PostgreSQL** (`1.4.0`): `to_tsvector(lang, field) @@ tsquery(lang, 'query')`. The `query-type` attribute selects the tsquery function: `plain` (default, `plainto_tsquery`), `web` (`websearch_to_tsquery` — supports `"phrase"`, `-exclude`, `OR`), `phrase` (`phraseto_tsquery`), or `raw` (`to_tsquery` — supports `|`, `&`, `:*` prefix).
  - **MySQL** (`1.4.0`): `MATCH(field) AGAINST('query' mode)`. The `mode` attribute selects `boolean` (default — supports `"phrase"`, `+required`, `-excluded`, `word*`), `natural`, or `expansion`.
  - **SQLite** (`1.4.0`): FTS5 subquery `rowid IN (SELECT rowid FROM Entity_fts WHERE Entity_fts MATCH 'query')`. Supports FTS5 phrase, `OR`, `AND`, `NOT`, and prefix (`word*`) syntax.
  - When the filter value equals the wildcard (`*`) the FTS predicate is skipped and all rows are returned.
  - Negation (`operator='notequal'`) wraps the expression in `NOT (...)`.
  - Fields without a matching search type fall back to the previous `LIKE '%value%'` behaviour.
- See [`docs/full-text-search.md`](docs/full-text-search.md) for configuration details and index setup per provider.

## [1.3.0] - 2026-04-29

### Fixed

- **Ado provider** (`1.1.0`): Use `DateTime.UtcNow` instead of `DateTime.Now` when recording batch start/end timestamps in the output controller.
- **PostgreSql provider** (`1.1.0`): `Enclose()` now quotes identifiers containing hyphens in addition to spaces and reserved words.

## [1.2.1] - 2026-04-25

### Added

- Extend "map" input type to `Field` (was already added to `Parameter` in 1.2.0).
- Add `Constants.InputTypeDomain` so `Field` and `Parameter` share a single domain definition for `InputType`.

## [1.2.0] - 2026-04-25

### Added

- Add "map" option and associated validation to parameter input type.

### Maintenance

- Updated MailKit to remove vulnerability

## [1.1.0] - 2026-04-12

### Added

- `bucketize` transform: maps numeric values to labeled string buckets using a map. Use `MapItem.From`/`To` as inclusive range boundaries (`*` for unbounded) and `MapItem.Value` as the label. Values outside all defined ranges fall through as their string representation. Example: `copy(Age).bucketize(AgeGroups)`.

## [1.0.0] - 2026-03-19

Going to version 1.0.0 so Vlad doesn't get build warnings.

## [0.12.x-beta and earlier]

For changes prior to 1.0.0, see the [git history](https://github.com/dalenewman/Transformalize/commits/master).
