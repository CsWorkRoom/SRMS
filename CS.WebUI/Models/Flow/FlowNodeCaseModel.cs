using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CS.BLL.FW;

namespace CS.WebUI.Models.Flow
{
    public class FlowNodeCaseModel : BF_FLOW_NODE_CASE.Entity
    {

        private List<BF_FLOW_NODE_CASE_RECORD.Entity> _flowNodeCaseRecords = new List<BF_FLOW_NODE_CASE_RECORD.Entity>();

        public FlowNodeCaseModel()
        {
            FlowNodeCaseRecords = _flowNodeCaseRecords;
        }

        public int Uid { get; set; }
        public string UserName { get; set; }
        public List<BF_FLOW_NODE_CASE_RECORD.Entity> FlowNodeCaseRecords { get; set; }
    }
}