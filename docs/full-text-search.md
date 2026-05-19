# Full-Text Search in ADO Providers

Transformalize can route a filter through the database engine's native full-text search instead of a `LIKE` predicate. This works for SQL Server, PostgreSQL, MySQL, and SQLite. The other ADO providers fall back to `LIKE`.

## How It Works

Two things wire up native FTS:

1. **Tag the field** — set `search-type` on any field to the name of a `<search-type>` you define.
2. **Define the search type** — add a `<search-type>` entry under `<search-types>` with `name` matching the field's `search-type` attribute and provider-specific options.
3. **Add a filter** — use `type='search'` on the filter pointing at that field.

When the filter value equals the wildcard (default `*`) the filter is skipped entirely, so an empty search returns all rows.

### Minimal example

```xml
<cfg name='NorthwindSearch' mode='report'>
  <search-types>
    <add name='fulltext' />
  </search-types>
  <parameters>
    <add name='q' value='*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Northwind' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Products'>
      <filter>
        <add field='ProductName' value='@[q]' type='search' />
      </filter>
      <fields>
        <add name='ProductID' type='int' primary-key='true' />
        <add name='ProductName' search-type='fulltext' />
      </fields>
    </add>
  </entities>
</cfg>
```

Using `operator='notequal'` on the filter negates the expression (`NOT (...)`).

## Search-Type Attributes

| Attribute | Default | Description |
|---|---|---|
| `name` | *(required)* | Unique name; referenced by a field's `search-type` attribute. |
| `analyzer` | `""` | Language/analyzer name. Meaning is provider-specific (see below). |
| `query-type` | `plain` | PostgreSQL only — controls which `tsquery` function is used. |
| `mode` | `boolean` | MySQL only — controls the `AGAINST` mode. |

---

## SQL Server

### Generated SQL

```sql
-- default (no analyzer)
CONTAINS(ProductName, 'chai')

-- with analyzer='french'
CONTAINS(ProductName, 'chai' LANGUAGE 'french')
```

`analyzer` maps to the `LANGUAGE` clause of `CONTAINS`. Leave it empty to use the column's default language.

### Creating the full-text index

SQL Server requires a full-text catalog and an index on the column.

```sql
-- 1. Create a catalog (once per database)
CREATE FULLTEXT CATALOG NorthwindFtsCatalog AS DEFAULT;

-- 2. Create the index (requires a unique key index on the table)
CREATE FULLTEXT INDEX ON Products(ProductName)
    KEY INDEX PK_Products
    ON NorthwindFtsCatalog
    WITH CHANGE_TRACKING AUTO;
```

The index population runs asynchronously. You can poll until it completes:

```sql
DECLARE @status int = 1;
WHILE @status <> 0
BEGIN
    SELECT @status = CONVERT(int, OBJECTPROPERTYEX(OBJECT_ID('Products'), 'TableFulltextPopulateStatus'));
    IF @status <> 0 WAITFOR DELAY '00:00:02';
END
```

> **Note:** Full-Text Search must be installed on the SQL Server instance. The Docker image `mcr.microsoft.com/mssql/server` does not include it by default — see the `Dockerfile.fts` in the test project for an image that does.

### Arrangement

```xml
<search-types>
  <add name='fulltext' />
  <!-- with a language override: -->
  <!-- <add name='fulltext' analyzer='french' /> -->
</search-types>
```

---

## PostgreSQL

### Generated SQL

```sql
-- plain (default)
to_tsvector('english', product_name) @@ plainto_tsquery('english', 'chai')

-- query-type='web'
to_tsvector('english', product_name) @@ websearch_to_tsquery('english', 'chai')

-- query-type='phrase'
to_tsvector('english', product_name) @@ phraseto_tsquery('english', 'chai')

-- query-type='raw'
to_tsvector('english', product_name) @@ to_tsquery('english', 'chai')
```

`analyzer` sets the text-search configuration (e.g. `english`, `french`, `simple`). It defaults to `english` when not specified.

### `query-type` options

| Value | Function used |
|---|---|
| `plain` *(default)* | `plainto_tsquery` — treats input as plain words, no operators |
| `web` | `websearch_to_tsquery` — Google-style syntax (`"phrase"`, `-exclude`) |
| `phrase` | `phraseto_tsquery` — matches exact phrase |
| `raw` | `to_tsquery` — raw tsquery syntax, requires operators like `&` and `\|` |

### Creating the full-text index

A GIN index on the `tsvector` expression dramatically speeds up queries:

```sql
CREATE INDEX IF NOT EXISTS idx_products_fts
    ON products USING GIN(to_tsvector('english', product_name));
```

Match the language in the index to the `analyzer` in your search type.

### Arrangement

```xml
<search-types>
  <add name='fulltext' analyzer='english' query-type='web' />
</search-types>
```

---

## MySQL

### Generated SQL

```sql
-- boolean mode (default)
MATCH(productName) AGAINST('chai' IN BOOLEAN MODE)

-- mode='natural'
MATCH(productName) AGAINST('chai' IN NATURAL LANGUAGE MODE)

-- mode='expansion'
MATCH(productName) AGAINST('chai' WITH QUERY EXPANSION)
```

### `mode` options

| Value | Clause |
|---|---|
| `boolean` *(default)* | `IN BOOLEAN MODE` — supports `+`, `-`, `*`, `"phrase"` operators |
| `natural` | `IN NATURAL LANGUAGE MODE` — relevance-ranked, no operators |
| `expansion` | `WITH QUERY EXPANSION` — natural language with a second pass using top results |

### Creating the full-text index

```sql
ALTER TABLE `Product` ADD FULLTEXT INDEX idx_product_fts (`productName`);
```

For multi-column search, list all columns in the index and in the `MATCH(...)` clause. Note that `MATCH` columns must match the indexed columns exactly.

### Arrangement

```xml
<search-types>
  <add name='fulltext' mode='boolean' />
</search-types>
```

---

## SQLite

SQLite uses FTS5 virtual tables. Transformalize generates a subquery against a shadow table named `<EntityName>_fts`.

### Generated SQL

```sql
rowid IN (SELECT rowid FROM Products_fts WHERE Products_fts MATCH 'chai')
```

### Creating the FTS5 virtual table

Create a content-table FTS5 virtual table that mirrors the real table, then populate it:

```sql
-- Create the virtual table (content= keeps data in sync with the real table)
CREATE VIRTUAL TABLE IF NOT EXISTS Products_fts
    USING fts5(ProductName, content='Products', content_rowid='ProductID');

-- Populate from existing rows
INSERT INTO Products_fts(Products_fts) VALUES('rebuild');
```

With `content=` mode the FTS index is not automatically updated when the base table changes. Re-run the `rebuild` command or use triggers to keep it in sync.

### Arrangement

```xml
<search-types>
  <add name='fulltext' />
</search-types>
```

---

## Fallback behavior

If a field's `search-type` is not set, or the search type name is `default`, a `LIKE '%value%'` filter is used instead. This means you can add a `type='search'` filter on any field and get reasonable behavior without a full-text index.

```xml
<!-- No search-type on the field → generates: ProductName LIKE '%chai%' -->
<fields>
  <add name='ProductName' />
</fields>
```
