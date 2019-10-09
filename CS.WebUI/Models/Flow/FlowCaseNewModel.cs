using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CS.BLL.FW;

namespace CS.WebUI.Models.Flow
{
    public class FlowCaseNewModel:BF_FLOW_CASE.Entity
    {
        private List<FlowNodeModel> _flowNodes=new List<FlowNodeModel>();

        public FlowCaseNewModel()
        {
            FlowNodeModes = _flowNodes;
        }

        public List<FlowNodeModel> FlowNodeModes { get; set; }
    }
}