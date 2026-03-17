## Overview

This is a GeoJson provider (writer) for Transformalize. It is limited to feature collections of points. It has a few confusing implementations.

### The Original (legacy) Writer

```xml
<add name='TestProcess'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' file='bogus.geo.json' />
  </connections>
  <entities>
    <add name='Contact' size='1'>
      <fields>
        <add name='Identity' type='int' primary-key='true' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' />
        <add name='Color' />
      </fields>
    </add>
  </entities>
</add>
```

The original writer did things a certain way:

- Based on field names, it used "latitude" and "longitude" in coordinates
- Based on field names, it used "color" as "marker-color"
- Every field goes in properties _except_ latitude and longitude
- An HTML table of all the properties is added to properties as "description"

It writes 1 row of bogus data to a geojson like this:

```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "geometry": {
        "type": "Point",
        "coordinates": [
          -74.145,
          49.3317
        ]
      },
      "properties": {
        "Identity": 1,
        "FirstName": "Justin",
        "LastName": "Konopelski",
        "description": "<table class=\"table table-striped table-condensed\">\n<tr>\n<td><strong>\nIdentity\n:</strong></td>\n<td>\n1\n</td>\n</tr>\n<tr>\n<td><strong>\nFirstName\n:</strong></td>\n<td>\nJustin\n</td>\n</tr>\n<tr>\n<td><strong>\nLastName\n:</strong></td>\n<td>\nKonopelski\n</td>\n</tr>\n</table>\n",
        "marker-color": "#661c0c"
      }
    }
  ]
}
```

### Minimal Writer

The "minimal" writer was an attempt to minimize the geojson because it was primarily used to populate a mapbox and we wanted it to load faster. You can choose the minimal writer by adding `use="minimal"` to the connection.

```xml
<add name='TestProcess'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' use="minimal" provider='geojson' file='bogus.geo.json' />
  </connections>
  <entities>
    <add name='Contact' size='1'>
      <fields>
        <add name='Identity' type='int' primary-key='true' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' />
        <add name='Color' alias="geojson-color" />
      </fields>
      <calculated-fields>
        <add name="geojson-description" t="format(This guy's name is {FirstName} {LastName})" length="128" />
      </calculated-fields>
    </add>
  </entities>
</add>
```

The "minimal" writer was still strong willed:

- Based on field names, it used latitude and longitude in coordinates
- If it found a "geojson-color" field, it would add a property for it called "marker-color"
- If it found a "geojson-description" field, it would add a property for it called "description"
- In OrchardCore, it used a different implementation that would add anything marked with `property="true"` to properties

It writes 1 row of bogus data to geojson like this:

```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "geometry": {
        "type": "Point",
        "coordinates": [
          -74.145,
          49.3317
        ]
      },
      "properties": {
        "description": "This guy's name is Justin Konopelski",
        "marker-color": "#661c0c"
      }
    }
  ]
}
```

### Latest Writer

After many years, and looking at the two previous implementations and wondering why 🤔 I made a new more controllable one. To use this latest implementation set `use="geo"`.  This implementation is controlled by the `geo` property on the fields.

```xml
<add name='TestProcess'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' use="geo" file='bogus.geo.json' min-lat='24.396308' min-lon='-125.0' max-lat='49.384358' max-lon='-66.93457' />
  </connections>
  <entities>
    <add name='Contact' size='1'>
      <fields>
        <add name='Identity' type='int' primary-key='true' geo="id" />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Latitude' type='double' min='24.396308' max='49.384358' geo="latitude" />
        <add name='Longitude' type='double' min='-125.0' max='-66.93457' geo="longitude" />
        <add name='Color' alias="color" geo="property" />
      </fields>
      <calculated-fields>
        <add name="description" t="format(This guy's name is {FirstName} {LastName})" length="128" geo="property" />
      </calculated-fields>
    </add>
  </entities>
</add>
```

The `geo` attribute controls what goes where:

- set `geo` to _id_ for the feature's id
- set `geo` to _latitude_, _longitude_, or _altitude_ for the feature's coordinates
- set `geo` to _property_ to include it in the properties
- nothing is renamed for properties, it uses the field's label, alias, or name
- any field without a `geo` designation is ignored by this writer.

Since I looked into what is actually supported in a geojson feature collection, I decided to add support for a bounding box. If all of `min-lat`, `min-lon`, `max-lat`, and `max-lon` are set on the connection, a collection's `bbox` is added.

It writes 1 row of bogus data to geojson like this:

```json
{
  "type": "FeatureCollection",
  "bbox": [
    -125.0,
    24.396308,
    -66.93457,
    49.384358
  ],
  "features": [
    {
      "type": "Feature",
      "id": 1,
      "geometry": {
        "type": "Point",
        "coordinates": [
          -74.145,
          49.3317
        ]
      },
      "properties": {
        "color": "#661c0c",
        "description": "This guy's name is Justin Konopelski"
      }
    }
  ]
}
```

