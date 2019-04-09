using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL
{
    /// <summary>
    /// EXCEL计算器，用来计算EXCEL公式的值
    /// </summary>
    public class ExcelCalculator:IDisposable
    {
        /// <summary>
        /// 工作簿，静态，常驻内存以提升性能
        /// </summary>
        private IWorkbook _workbook;

        private ISheet _sheet;

        private IRow _row;
        private ICell _cell;
        private XSSFFormulaEvaluator _formulaEvaluator;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelCalculator()
        {
            _workbook = new XSSFWorkbook();
            _sheet = _workbook.CreateSheet();
            _row = _sheet.CreateRow(0);
            
            _formulaEvaluator = new XSSFFormulaEvaluator(_workbook);
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            try
            {
                _workbook.Dispose();
            }
            catch
            {

            }
            finally
            {
                _workbook = null;
            }
        }

        /// <summary>
        /// 计算公式的值
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public string Calculate(string formula, string paramName, string paramValue)
        {
            if (string.IsNullOrWhiteSpace(formula) == true)
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(paramName) == false)
            {
                formula = formula.Replace(paramName, paramValue);
            }
            formula = formula.Trim(new char[] { ' ', '=', '＝' });
            try
            {
                _cell = _row.CreateCell(0);
                _cell.SetCellFormula(formula);
            }
            catch(Exception ex)
            {
                throw new Exception("公式[" + formula + "]错误:"+ex.Message);
            }
            try
            {
                _cell = _formulaEvaluator.EvaluateInCell(_cell);
            }
            catch
            {
                throw new Exception("无法计算公式" + formula + "的值！");
            }
            switch (_cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return _cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return _cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    return _cell.NumericCellValue.ToString();
                case CellType.String:
                    return _cell.StringCellValue;
                case CellType.Unknown:
                    return _cell.StringCellValue;
            }
            return "未知";
        }
    }
}
