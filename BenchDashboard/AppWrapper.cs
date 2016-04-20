using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mastersign.Bench.Dashboard
{
    class AppWrapper
    {
        private readonly AppFacade app;
        private readonly int no;

        public AppWrapper(AppFacade app, int no)
        {
            this.app = app;
            this.no = no;
        }

        [Browsable(false)]
        public AppFacade App { get { return app; } }

        public string ID { get { return app.ID; } }

        public string Version { get { return app.Version; } }

        public string Launcher { get { return app.Launcher; } }

        public int Index { get { return no; } }

        public string Typ
        {
            get
            {
                switch (app.Typ)
                {
                    case AppTyps.Default:
                        return "Default";
                    case AppTyps.Meta:
                        return "Group/Custom";
                    case AppTyps.NodePackage:
                        return "NodeJS";
                    case AppTyps.Python2Package:
                        return "Python 2";
                    case AppTyps.Python3Package:
                        return "Python 3";
                    default:
                        return app.Typ;
                }
            }
        }

        public string IsActive
        {
            get
            {
                return app.IsActivated ? "active" : (app.IsActive ? "implicit" : "inactive");
            }
        }

        public bool IsDeactivated { get { return app.IsDeactivated; } }

        public bool IsDependency { get { return app.IsDependency; } }

        public bool IsInstalled { get { return app.IsInstalled; } }

        public string ResourceState { get { return app.HasResource ? (app.IsResourceCached ? "cached" : "missing") : "none"; } }
    }
}
