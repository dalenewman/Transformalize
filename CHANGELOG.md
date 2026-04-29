# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
