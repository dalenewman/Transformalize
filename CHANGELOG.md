# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-04-12

### Added

- `bucketize` transform: maps numeric values to labeled string buckets using a map. Use `MapItem.From`/`To` as inclusive range boundaries (`*` for unbounded) and `MapItem.Value` as the label. Values outside all defined ranges fall through as their string representation. Example: `copy(Age).bucketize(AgeGroups)`.

## [1.0.0] - 2026-03-19

Going to version 1.0.0 so Vlad doesn't get build warnings.

## [0.12.x-beta and earlier]

For changes prior to 1.0.0, see the [git history](https://github.com/dalenewman/Transformalize/commits/master).
