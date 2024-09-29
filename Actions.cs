using System;
using System.Activities;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;


namespace Desky.Datatable
{
    [DisplayName("Convert Columns to Numeric")]
    [Description("Converts specified columns in a DataTable to a numeric data type (double). This allows for accurate mathematical operations on the data.")]
    [Category("Desky Datatable Activities")]
    public class ConvertColumnToNumeric : CodeActivity
    {
        [RequiredArgument]
        [Category("Input")]
        [Description("The DataTable in which specified columns will be converted to numeric data type.")]
        public InOutArgument<DataTable> InputDataTable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("A list of column names in the DataTable that should be converted to double data type.")]
        public InArgument<List<string>> ColumnNames { get; set; }

        [Category("Output")]
        [Description("The DataTable with the specified columns converted to double data type.")]
        public InOutArgument<DataTable> OutputDataTable { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            List<string> _namesOfColumnsToConvert = ColumnNames.Get(context);
            DataTable _targetTable = InputDataTable.Get(context);
            foreach (string columnname in _namesOfColumnsToConvert)
            {
                _targetTable = ActionHelper.ConvertColumnToNumeric(_targetTable, columnname);
            }
            OutputDataTable.Set(context, _targetTable);
        }
    }

    [DisplayName("Convert Columns to Date")]
    [Description("Converts specified columns in a DataTable to the DateTime data type. This allows for accurate date manipulations and comparisons.")]
    [Category("Desky Datatable Activities")]
    public class ConvertColumnToDate : CodeActivity
    {
        [RequiredArgument]
        [Category("Input")]
        [Description("The DataTable in which specified columns will be converted to date data type.")]
        public InOutArgument<DataTable> InputDataTable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("A list of column names in the DataTable that should be converted to date data type.")]
        public InArgument<List<string>> ColumnNames { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The format of the date string found in the specified columns (e.g., 'yyyy-MM-dd' or 'MM/dd/yyyy'). This format will be used to parse the string values into DateTime.")]
        public InOutArgument<string> DateFormat { get; set; }

        [Category("Output")]
        [Description("The DataTable with the specified columns converted to date data type.")]
        public InOutArgument<DataTable> OutputDataTable { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            List<string> _namesOfColumnsToConvert = ColumnNames.Get(context);
            DataTable _targetTable = InputDataTable.Get(context);
            string _dateFormat = DateFormat.Get(context);
            foreach (string columnname in _namesOfColumnsToConvert)
            {
                _targetTable = ActionHelper.ConvertColumnToDate(_targetTable, columnname, _dateFormat);
            }
            OutputDataTable.Set(context, _targetTable);
        }
    }


    [DisplayName("Merge DataTable")]
    [Description("Merges two DataTables into one.")]
    [Category("Desky Datatable Activities")]
    public class MergeTable : CodeActivity
    {
        [RequiredArgument]
        [Category("Input/Output")]
        [Description("The destination DataTable that will receive the merged data from the source DataTable. This DataTable will be updated with new rows from the source.")]
        public InOutArgument<DataTable> DestinationDataTable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The source DataTable containing the data to be merged into the destination DataTable. This DataTable should have a compatible schema for merging.")]
        public InArgument<DataTable> SourceDataTable { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            DataTable _sourceDataTable = SourceDataTable.Get(context);
            DataTable _destinationDataTable = DestinationDataTable.Get(context);

            _destinationDataTable = ActionHelper.MergeDatatables(_sourceDataTable, _destinationDataTable);
        }
    }

    [DisplayName("Reorder DataTable Columns")]
    [Description("Reorders the columns of a DataTable based on the specified order. This allows for better organization and consistency in data presentation.")]
    [Category("Desky Datatable Activities")]
    public class ReorderColumns : CodeActivity
    {
        [RequiredArgument]
        [Category("Input/Output")]
        [Description("The Datatable whose columns are to be reordered.")]
        public InOutArgument<DataTable> InputDataTable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The expected order of columns in the input DataTable.")]
        [Browsable(true)]
        public InArgument<List<string>> NamesOfColumnsInExpectedOrder { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The DataTable in which specified columns will be converted to date data type.")]
        public InOutArgument<DataTable> OutputDataTable { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<string> _namesOfColumnsInExpectedOrder = NamesOfColumnsInExpectedOrder.Get(context);
            DataTable _targetTable = InputDataTable.Get(context);

            _targetTable = ActionHelper.ReorderColumns(_targetTable, _namesOfColumnsInExpectedOrder);
            OutputDataTable.Set(context, _targetTable);
        }
    }

    [DisplayName("Split DataTable Vertically")]
    [Description("Splits a DataTable into two separate DataTables based on a specified column index. This allows for better data organization and manipulation.")]
    [Category("Desky Datatable Activities")]
    public class SplitTableVertical : CodeActivity
    {
        [RequiredArgument]
        [Category("Input")]
        [Description("The parent DataTable to be split vertically.")]
        public InOutArgument<DataTable> TargetDatatable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The column index to split from.")]
        public InArgument<int> ColumnToSplitFrom { get; set; }

        [Category("Output")]
        [Description("The first split of the table.")]
        public OutArgument<DataTable> dt1 { get; set; }

        [Category("Output")]
        [Description("The second split of the table.")]
        public OutArgument<DataTable> dt2 { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DataTable _targetTable = TargetDatatable.Get(context);
            int _columnToSplitFrom = ColumnToSplitFrom.Get(context);

            if (_targetTable != null && _targetTable.Columns.Count > 0)
            {
                // Create the two new tables with the same structure
                DataTable _dt1 = new DataTable();
                DataTable _dt2 = new DataTable();

                // Add columns to the first table up to the split point
                for (int i = 0; i < _columnToSplitFrom; i++)
                {
                    _dt1.Columns.Add(_targetTable.Columns[i].ColumnName, _targetTable.Columns[i].DataType);
                }

                // Add remaining columns to the second table
                for (int i = _columnToSplitFrom; i < _targetTable.Columns.Count; i++)
                {
                    _dt2.Columns.Add(_targetTable.Columns[i].ColumnName, _targetTable.Columns[i].DataType);
                }

                // Copy rows from the original table to the new tables
                foreach (DataRow row in _targetTable.Rows)
                {
                    // Create rows for each table
                    DataRow row1 = _dt1.NewRow();
                    DataRow row2 = _dt2.NewRow();

                    // Fill the first row with data up to the split point
                    for (int i = 0; i < _columnToSplitFrom; i++)
                    {
                        row1[i] = row[i];
                    }

                    // Fill the second row with the remaining data
                    for (int i = _columnToSplitFrom; i < _targetTable.Columns.Count; i++)
                    {
                        row2[i - _columnToSplitFrom] = row[i];
                    }

                    // Add rows to respective tables
                    _dt1.Rows.Add(row1);
                    _dt2.Rows.Add(row2);
                }

                // Set the output arguments
                dt1.Set(context, _dt1);
                dt2.Set(context, _dt2);
            }
        }
    }

    [DisplayName("Calculate Column Sum")]
    [Description("Calculates the sum of the values in the specified column of a DataTable, treating non-numeric or empty values as zero.")]
    [Category("Desky Datatable Activities")]
    public class SumColumnValues : CodeActivity
    {
        [RequiredArgument]
        [Category("Input/Output")]
        [Description("The DataTable whose numeric column values are to be summed.")]
        public InOutArgument<DataTable> InputDataTable { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The name of the numeric column in the DataTable to sum.")]
        public InArgument<string> ColumnName { get; set; }

        [Category("Output")]
        [Description("The sum of the numeric values in the specified column.")]
        public OutArgument<double> ColumnSum { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DataTable _inputTable = InputDataTable.Get(context);
            string _columnName = ColumnName.Get(context);

            double sum = ActionHelper.SumColumnValues(_inputTable, _columnName);

            // Set the output argument for the calculated sum
            ColumnSum.Set(context, sum);
        }


    }


    [DisplayName("Convert DataTable to HTML")]
    [Description("Converts the specified DataTable into an HTML table format, including headers and data rows, while treating empty or null values appropriately.")]
    [Category("Desky Datatable Activities")]
    public class ConvertDataTableToHtml : CodeActivity
    {
        [RequiredArgument]
        [Category("Input/Output")]
        [Description("The DataTable to be converted into HTML format.")]
        public InOutArgument<DataTable> InputDataTable { get; set; }

        [Category("Output")]
        [Description("The HTML representation of the DataTable as a string.")]
        public OutArgument<string> htmlText { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DataTable _inputTable = InputDataTable.Get(context);

            string _htmlText = ActionHelper.ConvertDataTableToHtml(_inputTable);

            // Set the output argument for the calculated sum
            htmlText.Set(context, _htmlText);
        }


    }
}
