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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Context {
    public class ConnectionContext : IConnectionContext {
        private readonly IContext _context;

        public ConnectionContext(IContext context, Connection connection) {
            _context = context;
            Connection = connection;
        }

        public Process Process => _context.Process;

        public Entity Entity => _context.Entity;

        public Field Field => _context.Field;

        public Operation Operation => _context.Operation;

        public void Debug(Func<string> lamda) {
            _context.Debug(lamda);
        }

        public void Error(string message, params object[] args) {
            _context.Error(message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            _context.Error(exception, message, args);
        }

        public void Info(string message, params object[] args) {
            _context.Info(message, args);
        }

        public void Warn(string message, params object[] args) {
            _context.Warn(message, args);
        }

        public IEnumerable<Field> GetAllEntityFields() {
            return _context.GetAllEntityFields();
        }

        public IEnumerable<Field> GetAllEntityOutputFields() {
            return _context.GetAllEntityOutputFields();
        }

        public LogLevel LogLevel => _context.LogLevel;

        public string Key => _context.Key;

        public IPipelineLogger Logger => _context.Logger;
        public object[] ForLog => _context.ForLog;

        public void Increment(uint @by = 1) {
        }

        public Connection Connection { get; }
        public int RowCapacity { get; } = 0;
    }
}