### Overview

This is a GeoJson provider (writer) for Transformalize. It is limited to feature collections of points.

### Write Usage

```xml
<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='geojson' file='bogus.geo.json' />
  </connections>
  <entities>
    <add name='Contact' size='1000'>
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

This writes 1000 rows of bogus data to a geojson file.

The file will look like this:
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
         "description": "<table class=\"table table-striped table-condensed\">\r\n<tr>\r\n<td><strong>\r\nIdentity\r\n:</strong></td>\r\n<td>\r\n1\r\n</td>\r\n</tr>\r\n<tr>\r\n<td><strong>\r\nFirstName\r\n:</strong></td>\r\n<td>\r\nJustin\r\n</td>\r\n</tr>\r\n<tr>\r\n<td><strong>\r\nLastName\r\n:</strong></td>\r\n<td>\r\nKonopelski\r\n</td>\r\n</tr>\r\n</table>\r\n",
         "marker-color": "#661c0c"
      }
   },
   ...
  ]
}
```
