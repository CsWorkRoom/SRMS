using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CS.BLL.FW;

namespace CS.WebUI.Models.Flow
{
    public class FlowNodeModel:BF_FLOW_NODE.Entity
    {
        private List<FlowNodeCaseModel> _flowNodeCases=new List<FlowNodeCaseModel>();

        public FlowNodeModel()
        {
            FlowNodes = _flowNodeCases;
        }

        public List<FlowNodeCaseModel> FlowNodes { get; set; }
    }
}