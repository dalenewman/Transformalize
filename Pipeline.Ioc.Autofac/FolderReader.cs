using System;
using System.Collections.Generic;
using System.IO;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;
using Pipeline.Provider.Excel;
using Pipeline.Provider.File;

namespace Pipeline.Ioc.Autofac
{
    public class FolderReader : IRead {

        private readonly IRead _reader;
        public FolderReader(InputContext input, IRowFactory rowFactory) {
            var readers = new List<IRead>();
            var searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), input.Connection.SearchOption, true);
            input.Info($"Searching folder: {input.Connection.Folder}");
            var files = new DirectoryInfo(input.Connection.Folder).GetFiles(input.Connection.SearchPattern, searchOption);
            input.Info($"Found {files.Length} files.");
            foreach (var file in files) {
                input.Info($"Found file: {file.Name}");
                if (file.Extension.ToLower().Contains("xls")) {
                    readers.Add(new ExcelReader(input, rowFactory));
                } else {
                    readers.Add(new DelimitedFileReader(input, rowFactory, new NullRowCondition()));
                }
            }
            _reader = new CompositeReader(readers);
        }

        public IEnumerable<IRow> Read() {
            return _reader.Read();
        }
    }
}