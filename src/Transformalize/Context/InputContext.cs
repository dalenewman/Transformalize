﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Context {
    public class InputContext : IConnectionContext {
        private readonly IContext _context;
        public Connection Connection { get; set; }
        public LogLevel LogLevel => _context.Logger.LogLevel;
        public int RowCapacity { get; set; }
        public Field[] InputFields { get; set; }

        public Entity Entity => _context.Entity;

        public Field Field => _context.Field;

        public Process Process => _context.Process;

        public Operation Operation => _context.Operation;

        public string Key { get; }
        public IPipelineLogger Logger => _context.Logger;
        public object[] ForLog => _context.ForLog;

        public InputContext(IContext context) {
            _context = context;
            RowCapacity = context.GetAllEntityFields().Count();
            InputFields = context.Entity.Fields.Where(f => f.Input).ToArray();
            Connection = context.Process.Connections.First(c => c.Name == context.Entity.Input);
            Key = context.Key;
        }

        public void Debug(Func<string> lamda) {
            _context.Debug(lamda);
        }

        public void Error(string message, params object[] args) {
            _context.Error(message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            _context.Error(exception, message, args);
        }

        public IEnumerable<Field> GetAllEntityFields() {
            return _context.GetAllEntityFields();
        }

        public IEnumerable<Field> GetAllEntityOutputFields() {
            return _context.GetAllEntityOutputFields();
        }

        public void Info(string message, params object[] args) {
            _context.Info(message, args);
        }

        public void Warn(string message, params object[] args) {
            _context.Warn(message, args);
        }
    }
}