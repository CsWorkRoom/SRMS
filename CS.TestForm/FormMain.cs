using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace CS.TestForm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = @"
   
";
            string s2 = s1.Trim();
            MessageBox.Show("s1 len:" + s1.Length + "\r\ns2 len:" + s2.Length);
            return;

            string sql = @"
    ALTER TABLE RFA_TMP_GROUPMSG ACTIVATE NOT LOGGED INITIALLY;                 
    INSERT INTO RFA_TMP_GROUPMSG (PARENT_ID,
                                  GROUP_ID,
                                  AREA_LVL2,
                                  NAME,
                                  BOSS_ORG_CODE,
                                  IN_USE,
                                  LEVEL_ID,
                                  DEPOSIT,
                                  PHONE_NO,
                                  FAX_NO,
                                  MODIFY_DATE,
                                  CLASS_CODE,
                                  CREATE_TIME)
SELECT  DISTINCT '0',
        A.GROUP_ID,
        CASE WHEN A.CLASS_CODE = '1000' THEN A.GROUP_ID ELSE C.ID END AREA_LVL2,
        A.GROUP_NAME NAME,
        '02'||SUBSTR(A.BOSS_ORG_CODE,3) BOSS_ORG_CODE,
        DECODE (A.IS_ACTIVE,'Y',1,0) IN_USE,
        DECODE (A.GRADE_CODE,
                '5',1,
                '6',2,
                '7',3,
                '8',4,
                '9',5,
                '2',7,
                '4',6,
                -1) LEVEL_ID,
        A.BAIL,
        A.PHONE,
        A.FAX,
        '@{day()}',
        A.CLASS_CODE,
        A.CREATE_TIME
FROM    DY_USER.ODS_BS_CHNGROUP_DICT_@{day()} A,
        DY_USER.ODS_DCHNGROUPINFO_@{day()} B,
        RFA_Q_DISTRICT C
WHERE   A.GROUP_ID = B.GROUP_ID
AND     B.GROUP_ID = C.ID
AND     B.DENORM_LEVEL = 1
AND     B.GROUP_ID = '17';
commit;
";
            string[] sqls = sql.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            MessageBox.Show("len:" + sqls.Length);
            foreach (string s in sqls)
            {
                MessageBox.Show(s.Length + "|" + s + "|");
            }
        }

        /// <summary>
        /// EXCEL数据有效性下拉测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = d2007();
            MessageBox.Show(fileName);
        }

        private string d2003()
        {
            string fileName = "G:/" + DateTime.Now.Ticks.ToString() + ".xls";
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet sheet = wk.CreateSheet();
            CellRangeAddressList regions = new CellRangeAddressList(0, 100, 2, 2);

            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(new string[] { "itemA", "itemB", "itemC" });
            HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            sheet.AddValidationData(dataValidate);

            using (FileStream fs = File.OpenWrite(fileName))
            {
                wk.Write(fs);
            }

            return fileName;
        }

        private string d2007()
        {
            string fileName = "G:/" + DateTime.Now.Ticks.ToString() + ".xlsx";
            XSSFWorkbook wk = new XSSFWorkbook();
            ISheet sheet = wk.CreateSheet();
            CellRangeAddressList regions = new CellRangeAddressList(0, 5, 2, 2);

            CT_DataValidation ctDataValidation = new CT_DataValidation();
            ctDataValidation.allowBlank = true;
            ctDataValidation.type = ST_DataValidationType.list;
            ctDataValidation.showInputMessage = true;
            ctDataValidation.showErrorMessage = true;
            ctDataValidation.sqref = "C1:C88";
            ctDataValidation.formula1 = "\"itemA,itemB,itemC\"";
            //ctDataValidation.showDropDown = true;


            //XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(new string[] { "itemA", "itemB", "itemC" });
            XSSFDataValidation dataValidate = new XSSFDataValidation(regions, ctDataValidation);
            //dataValidate= new XSSFDataValidation()
            sheet.AddValidationData(dataValidate);

            using (FileStream fs = File.OpenWrite(fileName))
            {
                wk.Write(fs);
            }

            return fileName;
        }



        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("测试完成");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            long ut = Common.FW.Functions.ConvertDateTimeToUnixDate(time);
            DateTime dt = Common.FW.Functions.ConvertUnixDateToDateTime(ut);

            MessageBox.Show(string.Format("now:{0} unix:{1} time:{2}", time.ToString("yyyy-MM-dd HH:mm:ss"), ut, dt.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string formula = "IF(A1>1000,100,IF(A1>800,90,IF(A1>600,80,MAX(5,INT(A1/10)))))";
            string paramName = "A1";
            string paramValue = "999";
            try
            {
                BLL.ExcelCalculator ec = new BLL.ExcelCalculator();
                string v = ec.Calculate(formula, paramName, paramValue);
                MessageBox.Show(string.Format("公式{0}\r\n参数{1}为{2}的计算结果为：\r\n{3}", formula, paramName, paramValue, v));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
