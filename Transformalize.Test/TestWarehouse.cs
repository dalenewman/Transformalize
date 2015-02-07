#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Test.Builders;

namespace Transformalize.Test {
    [TestFixture]
    public class TestWarehouse {
        [SetUp]
        public void SetUp() {
            var console = new ObservableEventListener();
            console.EnableEvents(TflEventSource.Log, EventLevel.Informational);
            console.LogToConsole(new LegacyLogFormatter());
        }

        [Test]
        [Ignore("Depends on having a SQL Server database named Junk.")]
        public void TestBug() {

            var inventory = GetInitialInventory();
            var storageLocations = GetInitialStorageLocations();
            var warehouses = GetInitialWarehouses();
            var process = GetInitialProcess(inventory, storageLocations, warehouses);

            //init and run
            var init = ProcessFactory.CreateSingle(process, new Options());
            init.Mode = "init";
            init.PipelineThreading = PipelineThreading.SingleThreaded;
            init.ExecuteScaler();

            var first = ProcessFactory.CreateSingle(process, new Options());
            first.PipelineThreading = PipelineThreading.SingleThreaded;
            first.ExecuteScaler();
            //LogManager.Flush();

            Assert.AreEqual(3, first["Inventory"].Inserts);
            Assert.AreEqual(2, first["StorageLocation"].Inserts);
            Assert.AreEqual(2, first["Warehouse"].Inserts);

            Assert.AreEqual(0, first["Inventory"].Updates);
            Assert.AreEqual(0, first["StorageLocation"].Updates);
            Assert.AreEqual(0, first["Warehouse"].Updates);

            inventory = GetInitialInventory();
            storageLocations = GetInitialStorageLocations();
            warehouses = GetInitialWarehouses();
            process = GetInitialProcess(inventory, storageLocations, warehouses);

            //run again, no changes
            var second = ProcessFactory.CreateSingle(process);
            second.PipelineThreading = PipelineThreading.SingleThreaded;
            second.ExecuteScaler();
            //LogManager.Flush();

            Assert.AreEqual(0, second["Inventory"].Inserts);
            Assert.AreEqual(0, second["StorageLocation"].Inserts);
            Assert.AreEqual(0, second["Warehouse"].Inserts);

            Assert.AreEqual(0, second["Inventory"].Updates);
            Assert.AreEqual(0, second["StorageLocation"].Updates);
            Assert.AreEqual(0, second["Warehouse"].Updates);

            //move Inventory 2 from Storage Location 1 to Storage Location 2
            process.Entities[0].InputOperation = new RowsBuilder()
                .Row("InventoryKey", 1)
                    .Field("Name", "Inventory 1")
                    .Field("StorageLocationKey", 1)
                .Row("InventoryKey", 2)
                    .Field("Name", "Inventory 2")
                    .Field("StorageLocationKey", 2) //here
                .Row("InventoryKey", 3)
                    .Field("Name", "Inventory 3")
                    .Field("StorageLocationKey", 2)
                .ToOperation();
            process.Entities[1].InputOperation = GetInitialStorageLocations();
            process.Entities[2].InputOperation = GetInitialWarehouses();

            //run again
            var third = ProcessFactory.CreateSingle(process);
            third.PipelineThreading = PipelineThreading.SingleThreaded;
            third.ExecuteScaler();
            //LogManager.Flush();

            Assert.AreEqual(0, third["Inventory"].Inserts);
            Assert.AreEqual(0, third["StorageLocation"].Inserts);
            Assert.AreEqual(0, third["Warehouse"].Inserts);

            Assert.AreEqual(1, third["Inventory"].Updates);
            Assert.AreEqual(0, third["StorageLocation"].Updates);
            Assert.AreEqual(0, third["Warehouse"].Updates);
        }

        private static TflProcess GetInitialProcess(IOperation inventory, IOperation storageLocations, IOperation warehouses) {
            return new TflProcess {
                Name = "Bug",
                Connections = new List<TflConnection> {
                    new TflConnection() {Name = "input", Provider = "internal"},
                    new TflConnection() {Name = "output", Provider = "sqlserver", Database = "junk"}
                },
                Entities = new List<TflEntity> {
                    new TflEntity {
                        Name = "Inventory",
                        InputOperation = inventory,
                        Version = "InventoryHashCode",
                        Fields = new List<TflField> {
                            new TflField() {Name = "InventoryKey", Type = "int", PrimaryKey = true},
                            new TflField() {Name = "Name", Alias = "Inventory", Default = "Default"},
                            new TflField() {Name = "StorageLocationKey", Type = "int"}
                        },
                        CalculatedFields = new List<TflField> {
                            new TflField {
                                Name = "InventoryHashCode",
                                Input = false,
                                Type = "int",
                                Transforms = new List<TflTransform>()
                                {
                                    new TflTransform() {Method = "concat", Parameter = "*"},
                                    new TflTransform() {Method = "gethashcode"}
                                }
                            }
                        }
                    },
                    new TflEntity {
                        Name = "StorageLocation",
                        InputOperation = storageLocations,
                        Version = "StorageLocationHashCode",
                        Fields = new List<TflField> {
                            new TflField() {Name = "StorageLocationKey", Type = "int", PrimaryKey = true},
                            new TflField() {Name = "Name", Alias = "StorageLocation", Default = "Default"},
                            new TflField() {Name = "WarehouseKey", Type = "int"}
                        },
                        CalculatedFields = new List<TflField> {
                            new TflField {
                                Name = "StorageLocationHashCode",
                                Type = "int",
                                Transforms = new List<TflTransform> {
                                    new TflTransform() {Method = "concat", Parameter = "*"},
                                    new TflTransform() {Method = "gethashcode"}
                                }
                            }
                        }
                    },
                    new TflEntity {
                        Name = "Warehouse",
                        InputOperation = warehouses,
                        Version = "WarehouseHashCode",
                        Fields = new List<TflField> {
                            new TflField() {Name = "WarehouseKey", Type = "int", PrimaryKey = true},
                            new TflField() {Name = "Name", Alias = "Warehouse", Default = "Default"}
                        },
                        CalculatedFields = new List<TflField> {
                            new TflField {
                                Name = "WarehouseHashCode",
                                Type = "int",
                                Transforms = new List<TflTransform>                                 {
                                    new TflTransform() {Method = "concat", Parameter = "*"},
                                    new TflTransform() {Method = "gethashcode"}
                                }
                            }
                        }
                    }
                },
                Relationships = new List<TflRelationship>() {
                    new TflRelationship {
                        LeftEntity = "Inventory",
                        LeftField = "StorageLocationKey",
                        RightEntity = "StorageLocation",
                        RightField = "StorageLocationKey"
                    },
                    new TflRelationship {
                        LeftEntity = "StorageLocation",
                        LeftField = "WarehouseKey",
                        RightEntity = "Warehouse",
                        RightField = "WarehouseKey"
                    }
                }
            };
        }

        private static IOperation GetInitialWarehouses() {
            return new RowsBuilder()
                .Row("WarehouseKey", 1)
                    .Field("Name", "Warehouse 1")
                .Row("WarehouseKey", 2)
                    .Field("Name", "Warehouse 2")
                .ToOperation();
        }

        private static IOperation GetInitialStorageLocations() {
            return new RowsBuilder()
                .Row("StorageLocationKey", 1)
                    .Field("Name", "Storage Location 1")
                    .Field("WarehouseKey", 1)
                .Row("StorageLocationKey", 2)
                    .Field("Name", "Storage Location 2")
                    .Field("WarehouseKey", 2)
                .ToOperation();
        }

        private static IOperation GetInitialInventory() {
            return new RowsBuilder()
                .Row("InventoryKey", 1)
                    .Field("Name", "Inventory 1")
                    .Field("StorageLocationKey", 1)
                .Row("InventoryKey", 2)
                    .Field("Name", "Inventory 2")
                    .Field("StorageLocationKey", 1)
                .Row("InventoryKey", 3)
                    .Field("Name", "Inventory 3")
                    .Field("StorageLocationKey", 2)
                .ToOperation();
        }
    }
}