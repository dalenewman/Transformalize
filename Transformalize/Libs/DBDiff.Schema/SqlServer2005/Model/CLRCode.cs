using System;
using Transformalize.Libs.DBDiff.Schema.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public abstract class CLRCode:Code
    {
        private Boolean isAssembly;
        private int assemblyId;
        private string assemblyClass;
        private string assemblyName;
        private string assemblyExecuteAs;
        private string assemblyMethod;

        public CLRCode(ISchemaBase parent, Enums.ObjectType type, Enums.ScripActionType addAction, Enums.ScripActionType dropAction)
            : base(parent, type, addAction, dropAction)
        {
        }

        public string AssemblyMethod
        {
            get { return assemblyMethod; }
            set { assemblyMethod = value; }
        }

        public string AssemblyExecuteAs
        {
            get { return assemblyExecuteAs; }
            set { assemblyExecuteAs = value; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        public Boolean IsAssembly
        {
            get { return isAssembly; }
            set { isAssembly = value; }
        }

        public string AssemblyClass
        {
            get { return assemblyClass; }
            set { assemblyClass = value; }
        }

        public int AssemblyId
        {
            get { return assemblyId; }
            set { assemblyId = value; }
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }
    }
}
