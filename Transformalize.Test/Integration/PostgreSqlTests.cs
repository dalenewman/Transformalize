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

using NUnit.Framework;
using Transformalize.Configuration.Builders;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Test.Integration {

    [TestFixture]
    public class PostgreSqlTests {

        [Test]
        public void TestInit()
        {

            var config = new ProcessBuilder("PostgreSqlTest")
                .Connection("input")
                    .Provider("postgresql")
                    .Database("Recipe3")
                    .Port(5432)
                    .User("postgres")
                    .Password("devdev1!")
                .Connection("output").Database("test")
                .Entity("main_recipe")
                    .Field("id").PrimaryKey()
                    .Field("name").Length(512)
                    .Field("description").Length(2048)
                    .Field("servings").Int32()
                    .Field("type_of_cuisine_id").Int32()
                    .Field("main_ingredient_id").Int32()
                    .Field("cooking_method_id").Int32()
                    .Field("difficulty_id").Int32()
                    .Field("tips").Length(2048)
                .Process();

            ProcessFactory.Create(config, new Options { Mode = "init" }).Run();
            ProcessFactory.Create(config).Run();
        }

    }
}