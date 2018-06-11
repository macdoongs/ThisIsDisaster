using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Environment {
    public class EnvironmentScriptBase {
        public EnvironmentModel Model {
            private set;
            get;
        }

        public void SetModel(EnvironmentModel model) {
            Model = model;
            
        }
    }
}