#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.SqlServer {

   public class SqlServerWriter : IWrite {

      private SqlBulkCopyOptions _bulkCopyOptions;
      private readonly OutputContext _output;
      private readonly IConnectionFactory _cf;
      private readonly IBatchReader _outputKeysReader;
      private readonly IWrite _sqlUpdater;
      private readonly IOrderHint _orderHint;
      private readonly IOperation _minDates;

      public SqlServerWriter(
          OutputContext output,
          IConnectionFactory cf,
          IBatchReader matcher,
          IWrite updater
      ) {
         _output = output;
         _cf = cf;
         _bulkCopyOptions = SqlBulkCopyOptions.Default;

         TurnOptionOn(SqlBulkCopyOptions.TableLock);
         TurnOptionOn(SqlBulkCopyOptions.UseInternalTransaction);
         TurnOptionOff(SqlBulkCopyOptions.CheckConstraints);
         TurnOptionOff(SqlBulkCopyOptions.FireTriggers);
         TurnOptionOn(SqlBulkCopyOptions.KeepNulls);

         _outputKeysReader = matcher;
         _sqlUpdater = updater;
         _orderHint = new ClusteredKeyOrderHint();

         if (output.OutputFields.Any(f => f.Type.StartsWith("date"))) {
            _minDates = new MinDateTransform(output, new DateTime(1753, 1, 1));
         }
      }

      private void TurnOptionOn(SqlBulkCopyOptions option) {
         _bulkCopyOptions |= option;
      }

      private bool IsOptionOn(SqlBulkCopyOptions option) {
         return (_bulkCopyOptions & option) == option;
      }

      private void TurnOptionOff(SqlBulkCopyOptions option) {
         if (IsOptionOn(option))
            _bulkCopyOptions ^= option;
      }

      public void Write(IEnumerable<IRow> rows) {
         InternalWrite(_minDates != null ? _minDates.Operate(rows) : rows);
      }


      private void InternalWrite(IEnumerable<IRow> rows) {

         using (var cn = new SqlConnection(_cf.GetConnectionString())) {
            cn.Open();

            var dt = new DataTable();

            try {
               SqlDataAdapter adapter;
               using (adapter = new SqlDataAdapter(_output.SqlSelectOutputSchema(_cf), cn)) {
                  adapter.Fill(dt);
               }
            } catch (System.Data.Common.DbException e) {
               _output.Error($"Error reading schema from {_output.Connection.Name}, {_output.Entity.Alias}.");
               _output.Error(e.Message);
               _output.Debug(() => e.StackTrace);
               return;
            }

            using (var bulkCopy = new SqlBulkCopy(cn, _bulkCopyOptions, null) {
               BatchSize = _output.Entity.InsertSize,
               BulkCopyTimeout = 0,
               DestinationTableName = "[" + _output.Entity.OutputTableName(_output.Process.Name) + "]"
            }) {

               for (var i = 0; i < _output.OutputFields.Length; i++) {
                  bulkCopy.ColumnMappings.Add(i, i);
               }

               if(_orderHint != null) {
                  _orderHint.Set(bulkCopy, _output.OutputFields);
               }

               foreach (var part in rows.Partition(_output.Entity.InsertSize)) {

                  var batch = part.ToArray();

                  if (_output.Process.Mode == "init" || (_output.Entity.Insert && !_output.Entity.Update)) {
                     var inserts = new List<IRow>();
                     inserts.AddRange(batch);
                     Insert(bulkCopy, dt, inserts);
                  } else {
                     var inserts = new List<IRow>();
                     var updates = new List<IRow>();
                     var tflHashCode = _output.Entity.TflHashCode();
                     var tflDeleted = _output.Entity.TflDeleted();
                     var matching = _outputKeysReader.Read(batch);

                     for (int i = 0, batchLength = batch.Length; i < batchLength; i++) {
                        var row = batch[i];
                        if (matching.Contains(i)) {
                           if (matching[i][tflDeleted].Equals(true) || !matching[i][tflHashCode].Equals(row[tflHashCode])) {
                              updates.Add(row);
                           }
                        } else {
                           inserts.Add(row);
                        }
                     }

                     Insert(bulkCopy, dt, inserts);

                     if (updates.Any()) {
                        _sqlUpdater.Write(updates);
                     }
                  }
               }
            }

            if (_output.Entity.Inserts > 0) {
               _output.Info("{0} inserts into {1} {2}", _output.Entity.Inserts, _output.Connection.Name, _output.Entity.Alias);
            }

            if (_output.Entity.Updates > 0) {
               _output.Info("{0} updates to {1}", _output.Entity.Updates, _output.Connection.Name);
            }

         }

      }

      private void Insert(SqlBulkCopy bulkCopy, DataTable dt, IEnumerable<IRow> inserts) {

         var enumerated = inserts.ToArray();

         if (enumerated.Length == 0)
            return;

         // convert to data rows for bulk copy
         var rows = new List<DataRow>();
         foreach (var insert in enumerated) {
            var row = dt.NewRow();
            row.ItemArray = insert.ToEnumerable(_output.OutputFields).ToArray();
            rows.Add(row);
         }

         try {
            bulkCopy.WriteToServer(rows.ToArray());
            _output.Entity.Inserts += Convert.ToUInt32(enumerated.Length);
         } catch (Exception ex) {
            _output.Error(ex.Message);
         } finally {
            dt.Clear();
         }
      }


   }


}