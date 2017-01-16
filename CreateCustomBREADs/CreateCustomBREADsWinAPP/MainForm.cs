using Siemens.SimaticIT.MES.Breads;
using Siemens.SimaticIT.MES.Breads.Types;
using SIT.Libs.Base.DB;
using SITCAB.DataSource.Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TestCreateBREAD
{
    public partial class MainForm : Form
    {
        SITSqlServerGateway sqlGateway = new SITSqlServerGateway();

        const string GET_TABLES_COMMAND_TEXT = @"
            select 
                TABLE_SCHEMA as [Schema], 
                TABLE_NAME as [Name] 
            from 
                information_schema.tables
            where 
                TABLE_TYPE='BASE TABLE' 
                and ({0})
                and ({1})
            order by 
                TABLE_NAME";

        const string GET_COLUMNS_COMMAND_TEXT = @"
        with [Columns] as
        (
	        select 
		        c.[name] as [ColumnName],
		        t.[name] as [DataType],
		        c.is_nullable as [IsNullable],
		        case when 
		        (
			        select 
				        count(*) 
			        from 
				        sys.index_columns ic 
				        inner join sys.indexes i on ic.object_id = i.object_id and ic.index_id = i.index_id 
			        where 
				        ic.object_id = c.object_id 
				        and ic.column_id = c.column_id 
				        and i.is_primary_key = 1
		        ) > 0 then 1 else 0 end as [IsPrimaryKey]
	        from    
		        sys.columns c
		        inner join sys.types t on c.user_type_id = t.user_type_id
	        where
		        c.[object_id] = object_id(@TableName)
        ),
        ForeignKeys as
        (
	        select 
		        cs.[name] as ForeignKeyColumn,
		        ts.[name] as ReferencedTableSchema,
		        tt.[name] as ReferencedTable,
		        ct.[name] as ReferencedColumn
	        from 
		        sys.foreign_key_columns as fk
		        inner join sys.tables as st on fk.parent_object_id = st.[object_id]
		        inner join sys.columns as cs on fk.parent_object_id = cs.[object_id] and fk.parent_column_id = cs.column_id
		        inner join sys.columns as ct on fk.referenced_object_id = ct.[object_id] and fk.referenced_column_id = ct.column_id
		        inner join sys.tables as tt on ct.[object_id] = tt.[object_id]
		        inner join sys.schemas as ts on tt.[schema_id] = ts.[schema_id]
	        where 
		        fk.parent_object_id = (select object_id from sys.tables where [object_id] = object_id(@TableName))
        )
        select
	        c.[ColumnName],
		    c.[DataType],
		    c.[IsNullable],
		    cast(c.[IsPrimaryKey] as bit) as [IsPrimaryKey],
	        fk.ReferencedTableSchema,
	        fk.ReferencedTable,
	        fk.ReferencedColumn
        from
	        [Columns] c
	        left join ForeignKeys fk on c.ColumnName = fk.ForeignKeyColumn";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string[] schemaFilters = txtFilterSchema.Text.Split(";".ToCharArray());
            string[] tableFilters = txtFilterTable.Text.Split(";".ToCharArray());

            StringBuilder sbSchemaFilters = new StringBuilder();
            for (int i = 0; i < schemaFilters.Length; i++)
            {
                sbSchemaFilters.Append(
                    string.Format(
                        "{0}TABLE_SCHEMA like '%{1}%'",
                        (i == 0 ? string.Empty : " or "),
                        schemaFilters[i]));
            }

            StringBuilder sbTableFilters = new StringBuilder();
            for (int i = 0; i < tableFilters.Length; i++)
            {
                sbTableFilters.Append(
                    string.Format(
                        "{0}TABLE_NAME like '%{1}%'",
                        (i == 0 ? string.Empty : " or "),
                        tableFilters[i]));
            }

            DataSet dsTables = sqlGateway.ExecuteSelectCommand(
                string.Format(
                    GET_TABLES_COMMAND_TEXT,
                    sbSchemaFilters.ToString(),
                    sbTableFilters.ToString()));

            gvTables.DataSource = dsTables.Tables[0];
            gvTables.Columns["Schema"].FillWeight = 30;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            ComponentDescriptorInputForm cdif = new ComponentDescriptorInputForm();

            if (cdif.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> selectedTables = new List<string>();

                foreach (DataGridViewRow row in gvTables.SelectedRows)
                    selectedTables.Add(string.Format("{0}", row.Cells["Name"].Value));

                GenerateBREADs(cdif.ComponentDescriptor, selectedTables);
            }
        }

        private bool GenerateBREADs(string componentDescriptorName, List<string> tables)
        {
            // **************************************************
            // Define custom BREAD component
            // **************************************************
            DynamicBreadFactory.ComponentDescriptor componentDescriptor = new DynamicBreadFactory.ComponentDescriptor(componentDescriptorName);

            // the entities contained in this component will read data from SitMesDB
            componentDescriptor.DataBaseID = DynamicBreadFactory.SITDB.SitMes;

            foreach (string table in tables)
            {
                // *****************************************************************
                // Add entity to the component
                // *****************************************************************
                DynamicBreadFactory.EntityDescriptor entityDescriptor = new DynamicBreadFactory.EntityDescriptor(table);

                // make the entity refer to the current table
                entityDescriptor.Join = table;

                SqlCommand getColumnsCommand = (SqlCommand)sqlGateway.GetSqlCommand(GET_COLUMNS_COMMAND_TEXT);
                sqlGateway.AddParameterWithValue(getColumnsCommand, "TableName", table);

                //get all columns
                DataSet dsTableColumns = sqlGateway.ExecuteSelectCommand(getColumnsCommand);

                foreach (DataRow row in dsTableColumns.Tables[0].Rows)
                {
                    string columnName = row["ColumnName"].ToString();
                    DataSet dsTrick = sqlGateway.ExecuteSelectCommand(string.Format("select [{0}] from [{1}] where 0=1", columnName, table));// trick to get the SqlDbType

                    DynamicBreadFactory.PropertyDescriptor propertyDescriptor = new DynamicBreadFactory.PropertyDescriptor(columnName, dsTrick.Tables[0].Columns[0].DataType, columnName);

                    if (row["ReferencedTable"] != DBNull.Value)
                    {
                        string referencedTable = string.Format("{0}", row["ReferencedTable"]);
                        
                        if (tables.Contains(referencedTable))
                        {
                            // reference to a table within the same component" add the property and its reference                        
                            propertyDescriptor.Links.Add(new DynamicBreadFactory.LinkDescriptor(componentDescriptorName, referencedTable, row["ReferencedColumn"].ToString()));
                        }
                    }

                    propertyDescriptor.IsPrimaryKey = (bool)row["IsPrimaryKey"];
                    propertyDescriptor.IsNullable = (bool)row["IsNullable"];
                    propertyDescriptor.IsPureDate = string.Compare(row["IsNullable"].ToString(), "date", StringComparison.OrdinalIgnoreCase) == 0;
                    propertyDescriptor.Attributes.Add(new MandatoryAttribute());

                    entityDescriptor.Properties.Add(propertyDescriptor);
                }               

                // the descriptor for the defined entity must be added to the component
                componentDescriptor.Entities.Add(entityDescriptor);
            }


            // ***************************************
            // Check whole component before generation
            // ***************************************
            DynamicBreadFactory.CheckResult checkResult = componentDescriptor.Check();
            if (checkResult.Anomalies.Count > 0)
            {
                MessageBox.Show(checkResult.ReturnedValue.message);
                return false;
            }
            else
            {
                // ******************************************
                // Prepare parameters for assembly generation
                // ******************************************
                DynamicBreadFactory.GenerationParameters generationParameters = new DynamicBreadFactory.GenerationParameters();

                string sitMesBinPath = string.Format("{0}\\MES\\BIN", Environment.GetEnvironmentVariable("ICUBEPATH"));
                generationParameters.Path = sitMesBinPath;
                generationParameters.StrongNameKeyFile = sitMesBinPath + @"\breadkey.snk"; // this is always found in SIT\MES\BIN

                // *******************
                // Generate assemblies
                // *******************
                ReturnValue ret = DynamicBreadFactory.Generate(componentDescriptor, generationParameters);

                string installFilePath = string.Format(
                    "{0}\\{1}_Uninstall.bat",
                    sitMesBinPath,
                    componentDescriptorName);

                string uninstallFilePath = string.Format(
                    "{0}\\{1}_Install.bat",
                    sitMesBinPath,
                    componentDescriptorName);

                string uninstallAndInstallFilePath = string.Format(
                    "{0}\\{1}_UninstallAndInstall.bat",
                    sitMesBinPath,
                    componentDescriptorName);

                if (ret.succeeded)
                {
                    File.WriteAllText(
                        installFilePath,
                        string.Format(
                            "echo on{0}sitgacutil /u {1}Bread{0}sitgacutil /u {1}Types",
                            Environment.NewLine,
                            componentDescriptorName));

                    File.WriteAllText(
                        uninstallFilePath,
                        string.Format(
                            "echo on{0}sitgacutil /i {1}Bread.dll{0}sitgacutil /i {1}Types.dll",
                            Environment.NewLine,
                            componentDescriptorName));

                    File.WriteAllText(
                        uninstallAndInstallFilePath,
                        string.Format(
                            "echo on{0}sitgacutil /u {1}Bread{0}sitgacutil /u {1}Types{0}sitgacutil /i {1}Bread.dll{0}sitgacutil /i {1}Types.dll",
                            Environment.NewLine,
                            componentDescriptorName));

                    MessageBox.Show("BREAD assemblies for component " + componentDescriptor.Component + " generated");
                }
                else
                    MessageBox.Show("Error generating BREAD assemblies " + ret.message);

                Process.Start("explorer.exe", string.Format("/select, \"{0}\"", uninstallAndInstallFilePath));//open the .bat folder
                return ret.succeeded;
            }
        }

        private void gvTables_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            btnGenerate.Enabled = false;
        }

        private void gvTables_SelectionChanged(object sender, EventArgs e)
        {
            btnGenerate.Enabled = gvTables.SelectedRows.Count > 0;
            saveToolStripMenuItem.Enabled = gvTables.SelectedRows.Count > 0;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                StringBuilder sbXml = new StringBuilder();

                if (gvTables.SelectedRows.Count == gvTables.Rows.Count)
                    sbXml.Append(string.Format("<CreateCustomBREADsSelection><Filter value=\"{0}\" selectAll=\"true\" /></CreateCustomBREADsSelection>", txtFilterTable.Text));
                else
                {
                    sbXml.Append(string.Format("<CreateCustomBREADsSelection><Filter value=\"{0}\" selectAll=\"false\" /><SelectedTables>", txtFilterTable.Text));

                    foreach (DataGridViewRow row in gvTables.SelectedRows)
                        sbXml.Append(string.Format("<SelectedTable name=\"{0}\" />", row.Cells["Name"].Value));
                    
                    sbXml.Append("</SelectedTables></CreateCustomBREADsSelection>");
                }

                File.WriteAllText(saveFileDialog.FileName, sbXml.ToString());

                MessageBox.Show("Selection successfully saved", "Hurray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    XmlDocument selectionXml = new XmlDocument();
                    selectionXml.LoadXml(File.ReadAllText(openFileDialog.FileName));

                    txtFilterTable.Text = selectionXml.SelectSingleNode("//Filter").Attributes["value"].Value;
                    BindGrid();

                    XmlAttribute selectAllAttribute = selectionXml.SelectSingleNode("//Filter").Attributes["selectAll"];
                    if ((selectAllAttribute != null) && ((string.Compare(selectAllAttribute.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)))
                        gvTables.SelectAll();
                    else
                    {
                        foreach (DataGridViewRow row in gvTables.Rows)
                        {
                            if (selectionXml.SelectSingleNode(string.Format("//SelectedTable[@name='{0}']", row.Cells["Name"].Value)) != null)
                                row.Selected = true;
                        }
                    }
                }
                catch (XmlException xmle)
                {
                    MessageBox.Show(string.Format("Could not import selection file: {0}", xmle.Message), "Error importing selection file", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
