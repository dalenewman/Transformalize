## Arrangements

### internal.xml

This self-contained configuration returns two rows with full name transformations.

`tfl -a internal.xml`

### bogus.xml

This reads from the [bogus](https://github.com/dalenewman/Transformalize.Provider.Bogus) provider.  It accepts a `seed` and `size` parameter controlling 
the random seed and how many bogus records to generate.

```bash
tfl -a bogus.xml
tfl -a bogus.xml -p Seed=2
tfl -a bogus.xml -p Seed=2 Size=10
```

### bogus-too-flexible.xml

This one is like *bogus.xml* only way too flexible.  It has a lot of parameters.

```
tfl -a bogus-too-flexible.xml
```