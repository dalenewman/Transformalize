using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bogus;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Bogus {
   public class BogusReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly List<BogusField> _fields;

      public BogusReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
         _fields = new List<BogusField>(context.InputFields.Length);

         foreach (var field in context.InputFields) {
            var bf = new BogusField(field);

            if (field.Map != string.Empty) {
               bf.HasMap = true;
            } else {
               if (field.Max != null) {
                  try {
                     bf.Max = field.Convert(field.Max);
                     bf.HasMax = true;
                  } catch (Exception) {
                     context.Error($"error converting {field.Name}'s max of {field.Max} to {field.Type}.");
                  }
               }
               if (field.Min != null) {
                  try {
                     bf.Min = field.Convert(field.Min);
                     bf.HasMin = true;
                  } catch (Exception) {
                     context.Error($"error converting {field.Name}'s min of {field.Min} to {field.Type}.");
                  }
               }
               bf.HasMinAndMax = bf.HasMin && bf.HasMax;

               switch (bf.Type) {
                  case "int32":
                  case "int":
                     bf.MinInt = bf.HasMin ? (int)bf.Min : default(int);
                     bf.MaxInt = bf.HasMax ? (int)bf.Max : default(int);
                     break;
                  case "double":
                     bf.MaxDouble = bf.HasMax ? (double)bf.Max : default(double);
                     bf.MinDouble = bf.HasMin ? (double)bf.Min : default(double);
                     break;
                  case "decimal":
                     bf.MaxDecimal = bf.HasMax ? (decimal)bf.Max : default(decimal);
                     bf.MinDecimal = bf.HasMin ? (decimal)bf.Min : default(decimal);
                     break;
                  case "byte":
                     bf.MaxByte = bf.HasMax ? (byte)bf.Max : default(byte);
                     bf.MinByte = bf.HasMin ? (byte)bf.Min : default(byte);
                     break;
                  case "date":
                  case "datetime":
                     bf.MaxDateTime = bf.HasMax ? (DateTime)bf.Max : default(DateTime);
                     bf.MinDateTime = bf.HasMin ? (DateTime)bf.Min : default(DateTime);
                     break;
                  default:
                     // for "categories" min and max
                     if (bf.HasMin && int.TryParse(field.Min.ToString(), out int min)) {
                        bf.MinInt = min;
                     }
                     if (bf.HasMax && int.TryParse(field.Max.ToString(), out int max)) {
                        bf.MaxInt = max;
                     }
                     break;
               }
            }
            _fields.Add(bf);
         }

         Randomizer.Seed = context.Connection.Seed == 0 ? new Random() : new Random(context.Connection.Seed);
      }

      public IEnumerable<IRow> Read() {

         var randomizer = new Randomizer();
         var faker = new Faker(_context.Entity.Locale);

         foreach (var field in _fields.Where(f => f.HasMap)) {
            field.MapItems = _context.Process.Maps.First(m => m.Name == field.Map).Items.ToArray();
         }
         var identity = 1;

         for (var i = 0; i < _context.Entity.Size; i++) {
            var row = _rowFactory.Create();

            Person person = null;

            foreach (var field in _fields) {

               if (field.HasMap) {
                  row[field] = randomizer.ArrayElement(field.MapItems).From;
                  continue;
               }

               var name = field.Name.ToLower();

               switch (name) {

                  // person, address
                  case "firstname":
                     SafePerson(ref person);
                     row[field] = person.FirstName;
                     break;
                  case "lastname":
                     SafePerson(ref person);
                     row[field] = person.LastName;
                     break;
                  case "fullname":
                  case "name":
                     SafePerson(ref person);
                     row[field] = person.FirstName + " " + person.LastName;
                     break;
                  case "streetaddress":
                     SafePerson(ref person);
                     row[field] = person.Address.Street;
                     break;
                  case "lat":
                  case "latitude":
                     if (field.HasMinAndMax) {
                        row[field] = faker.Address.Latitude(field.MinDouble, field.MaxDouble);
                     } else if (field.HasMin) {
                        row[field] = faker.Address.Latitude(field.MinDouble);
                     } else if (field.HasMax) {
                        row[field] = faker.Address.Latitude(-90D, field.MaxDouble);
                     } else {
                        SafePerson(ref person);
                        row[field] = person.Address.Geo.Lat;
                     }
                     break;
                  case "lon":
                  case "longitude":
                     if (field.HasMinAndMax) {
                        row[field] = faker.Address.Longitude(field.MinDouble, field.MaxDouble);
                     } else if (field.HasMin) {
                        row[field] = faker.Address.Longitude(field.MinDouble);
                     } else if (field.HasMax) {
                        row[field] = faker.Address.Longitude(-180D, field.MaxDouble);
                     } else {
                        SafePerson(ref person);
                        row[field] = person.Address.Geo.Lng;
                     }
                     break;
                  case "state":
                     if (field.Length == 2) {
                        row[field] = faker.Address.StateAbbr();
                     } else {
                        row[field] = faker.Address.State();
                     }
                     break;
                  case "city":
                     SafePerson(ref person);
                     row[field] = person.Address.City;
                     break;
                  case "zipcode":
                     SafePerson(ref person);
                     row[field] = person.Address.ZipCode;
                     break;
                  case "phonenumber":
                     SafePerson(ref person);
                     row[field] = field.Format == string.Empty ? person.Phone : faker.Phone.PhoneNumber(field.Format);
                     break;
                  case "country":
                     row[field] = field.Length > 3 ? faker.Address.Country() : faker.Address.CountryCode();
                     break;
                  case "county":
                     row[field] = faker.Address.County();
                     break;

                  //product
                  case "productname":
                     row[field] = faker.Commerce.ProductName();
                     break;
                  case "categories":
                     row[field] = string.Join(_context.Field.Delimiter, faker.Commerce.Categories(randomizer.Int(field.MinInt,field.MaxInt)));
                     break;
                  case "department":
                     row[field] = faker.Commerce.Department();
                     break;

                  // company
                  case "catchphrase":
                     row[field] = randomizer.ClampString(faker.Company.CatchPhrase(), 0, field.Length);
                     break;
                  case "bs":
                     row[field] = randomizer.ClampString(faker.Company.Bs(), 0, field.Length);
                     break;
                  case "company":
                     row[field] = faker.Company.CompanyName();
                     break;
                  case "companysuffix":
                     row[field] = faker.Company.CompanySuffix();
                     break;

                  // date
                  case "dob":
                  case "dateofbirth":
                     SafePerson(ref person);
                     row[field] = person.DateOfBirth;
                     break;
                  case "future":
                     SetDateOrString(row, field, faker.Date.Future());
                     break;
                  case "past":
                     SetDateOrString(row, field, faker.Date.Past());
                     break;
                  case "soon":
                     SetDateOrString(row, field, faker.Date.Soon());
                     break;
                  case "between":
                     SetDateOrString(row, field, faker.Date.Between(field.MinDateTime, field.MaxDateTime));
                     break;
                  case "recent":
                     SetDateOrString(row, field, faker.Date.Recent());
                     break;
                  case "month":
                     row[field] = faker.Date.Month(field.Length <= 3);
                     break;
                  case "weekday":
                     row[field] = faker.Date.Weekday(field.Length <= 3);
                     break;

                  // hacker
                  case "abbr":
                  case "abbreviation":
                     row[field] = faker.Hacker.Abbreviation();
                     break;
                  case "adj":
                  case "adjective":
                     row[field] = faker.Hacker.Adjective();
                     break;
                  case "verb":
                     row[field] = faker.Hacker.Verb();
                     break;
                  case "ingverb":
                     row[field] = faker.Hacker.IngVerb();
                     break;
                  case "phrase":
                     row[field] = faker.Hacker.Phrase();
                     break;

                  // internet
                  case "email":
                     SafePerson(ref person);
                     row[field] = person.Email;
                     break;
                  case "username":
                     SafePerson(ref person);
                     row[field] = person.UserName;
                     break;
                  case "avatar":
                     SafePerson(ref person);
                     row[field] = person.Avatar;
                     break;
                  case "ip":
                     row[field] = faker.Internet.Ip();
                     break;
                  case "ipv6":
                     row[field] = faker.Internet.Ipv6();
                     break;
                  case "useragent":
                     row[field] = faker.Internet.UserAgent();
                     break;
                  case "mac":
                     row[field] = faker.Internet.Mac();
                     break;
                  case "color":
                     row[field] = faker.Internet.Color();
                     break;

                  // lorem
                  case "paragraph":
                     row[field] = faker.Lorem.Paragraph();
                     break;

                  // rant
                  case "review":
                     row[field] = faker.Rant.Review(field.Hint);
                     break;

                  case "identity":
                     if (identity == 1 && field.HasMin && field.MinInt > identity) {
                        identity = field.MinInt;
                     }
                     row[field] = identity++;
                     break;
                  default:
                     switch (field.Type) {
                        case "bool":
                        case "boolean":
                           row[field] = randomizer.Bool();
                           break;
                        case "int":
                        case "int32":
                           if (field.HasMinAndMax) {
                              row[field] = randomizer.Int(field.MinInt, field.MaxInt);
                           } else if (field.HasMin) {
                              row[field] = randomizer.Int(field.MinInt);
                           } else if (field.HasMax) {
                              row[field] = randomizer.Int(0, field.MaxInt);
                           } else {
                              row[field] = randomizer.Int();
                           }
                           break;
                        case "string":
                           if (field.Format != string.Empty) {
                              row[field] = randomizer.Replace(field.Format);
                           } else {
                              row[field] = randomizer.ClampString(faker.Rant.Review(field.Hint), null, field.Length);
                           }
                           break;
                        case "byte":
                           if (field.HasMinAndMax) {
                              row[field] = randomizer.Byte(field.MinByte, field.MaxByte);
                           } else if (field.HasMin) {
                              row[field] = randomizer.Byte(field.MinByte);
                           } else if (field.HasMax) {
                              row[field] = randomizer.Byte(0, field.MaxByte);
                           } else {
                              row[field] = randomizer.Byte();
                           }
                           break;
                        case "double":
                           if (field.HasMinAndMax) {
                              row[field] = randomizer.Double(field.MinDouble, field.MaxDouble);
                           } else if (field.HasMin) {
                              row[field] = randomizer.Double(field.MinDouble);
                           } else if (field.HasMax) {
                              row[field] = randomizer.Double(0D, field.MaxDouble);
                           } else {
                              row[field] = randomizer.Double();
                           }
                           break;
                        case "decimal":
                           if (field.HasMinAndMax) {
                              row[field] = randomizer.Decimal(field.MinDecimal, field.MaxDecimal);
                           } else if (field.HasMin) {
                              row[field] = randomizer.Decimal(field.MinDecimal);
                           } else if (field.HasMax) {
                              row[field] = randomizer.Decimal(0M, field.MaxDecimal);
                           } else {
                              row[field] = randomizer.Decimal();
                           }
                           break;
                        default:
                           break;
                     }
                     break;
               }
            }
            yield return row;
         }

      }

      private static void SetDateOrString(IRow row, BogusField field, DateTime date) {
         if (!field.Type.StartsWith("date")) {
            row[field] = field.Format == string.Empty ? date.ToString(CultureInfo.InvariantCulture) : date.ToString(field.Format);
         } else {
            row[field] = date;
         }
      }

      private void SafePerson(ref Person person) {
         if (person == null) {
            person = new Person(_context.Entity.Locale);
         }
      }
   }
}
