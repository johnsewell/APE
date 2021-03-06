﻿//
//Copyright 2016 David Beales
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//
using System;
using System.Text;
using WF = System.Windows.Forms;

namespace APE.Communication
{
    /// <summary>
    /// This partial class provides some in process helper methods to improve performance of accessing flexgrid controls
    /// </summary>
    public partial class APEIPC
    {
        public enum CellProperty
        {
            /// <summary>
            /// Cells(s) back colour name as a string
            /// </summary>
            BackColourName,
            /// <summary>
            /// Cells(s) fore colour name as a string
            /// </summary>
            ForeColourName,
            /// <summary>
            /// The data type of the cell as a string
            /// Supported in GetCellValue only
            /// </summary>
            DataType,
            /// <summary>
            /// Cells(s) checkbox state as a string
            /// </summary>
            CheckBox,
            /// <summary>
            /// For GetCellValue this returns the actual image
            /// For GetCellRange this returns whether the cells contain a image
            /// </summary>
            Image,
            /// <summary>
            /// For GetCellValue this returns the actual background image
            /// For GetCellRange this returns whether the cells contain a background image
            /// </summary>
            BackgroundImage,
        }

        string m_ColumnSeparator = "";
        string m_RowSeparator = "";

        /// <summary>
        /// Initialise the flexgrid helper delegates for use
        /// </summary>
        private void SetupFlexgridHelperDelegates()
        {
            m_FlexgridGetCellRangeBackColourNameDelegater = new GetCellRangeBackColourNameDelegate(FlexgridGetCellRangeBackColourNameInternal);
            m_FlexgridGetCellRangeForeColourNameDelegater = new GetCellRangeForeColourNameDelegate(FlexgridGetCellRangeForeColourNameInternal);
            m_FlexgridGetCellRangeDataTypeDelegater = new GetCellRangeDataTypeDelegate(FlexgridGetCellRangeDataTypeInternal);
            m_FlexgridGetCellRangeCheckBoxDelegater = new GetCellRangeCheckBoxDelegate(FlexgridGetCellRangeCheckBoxInternal);
            m_FlexgridGetCellRangeImageDelegater = new GetCellRangeImageDelegate(FlexgridGetCellRangeImageInternal);
            m_FlexgridGetCellRangeBackgroundImageDelegater = new GetCellRangeBackgroundImageDelegate(FlexgridGetCellRangeBackgroundImageInternal);
            m_FlexgridGetAllColumnsHiddenDelegater = new GetAllColumnsHiddenDelegate(FlexgridGetAllColumnsHiddenInternal);
            m_FlexgridGetAllColumnsWidthDelegater = new GetAllColumnsWidthDelegate(FlexgridGetAllColumnsWidthInternal);
            m_FlexgridGetAllRowsHiddenDelegater = new GetAllRowsHiddenDelegate(FlexgridGetAllRowsHiddenInternal);
            m_FlexgridGetAllRowsHeightDelegater = new GetAllRowsHeightDelegate(FlexgridGetAllRowsHeightInternal);
        }

        /// <summary>
        /// Determine what the grid currently uses to indicate column and row breaks
        /// </summary>
        /// <param name="clipSeparators">The current grid separators setting</param>
        private void DetermineClipSeparators(string clipSeparators)
        {
            if (clipSeparators == "")
            {
                m_ColumnSeparator = "\t";
                m_RowSeparator = "\r";
            }
            else
            {
                m_ColumnSeparator = clipSeparators.Substring(0, 1);
                m_RowSeparator = clipSeparators.Substring(1, 1);
            }
        }

        //
        //  FlexgridGetCellRange
        //

        /// <summary>
        /// Calls into the AUT to iterate over every cell in the grid returning a \t \r separated string of the
        /// required property of the cells
        /// </summary>
        /// <param name="sourceStore">The datastore which contains the grid object</param>
        /// <param name="destinationStore">The datastore to put the resultant string into</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        unsafe public void AddQueryMessageFlexgridGetCellRange(DataStores sourceStore, DataStores destinationStore, int row1, int column1, int row2, int column2, CellProperty property)
        {
            if (!m_DoneFind)
            {
                throw new Exception("Must locate the flexgrid before trying to use it");
            }

            Message* ptrMessage = GetPointerToNextMessage();
            ptrMessage->SourceStore = sourceStore;
            ptrMessage->DestinationStore = destinationStore;

            switch (property)
            {
                case CellProperty.BackColourName:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeBackColourName;
                    break;
                case CellProperty.ForeColourName:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeForeColourName;
                    break;
                case CellProperty.DataType:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeDataType;
                    break;
                case CellProperty.CheckBox:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeCheckBox;
                    break;
                case CellProperty.Image:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeImage;
                    break;
                case CellProperty.BackgroundImage:
                    ptrMessage->Action = MessageAction.FlexgridGetCellRangeBackgroundImage;
                    break;
                default:
                    throw new Exception("Implement support for getting cell property " + property.ToString());
            }

            Parameter row1Parameter = new Parameter(this, row1);
            Parameter column1Parameter = new Parameter(this, column1);
            Parameter row2Parameter = new Parameter(this, row2);
            Parameter column2Parameter = new Parameter(this, column2);

            m_PtrMessageStore->NumberOfMessages++;
            m_DoneQuery = true;
        }

        /// <summary>
        /// Gets the parameters from the message then calls the appropriate FlexgridGetCellRange method 
        /// on the correct thread storing the results in the specified datastore
        /// </summary>
        /// <param name="ptrMessage">A pointer to the message</param>
        unsafe private void FlexgridGetCellRange(Message* ptrMessage, CellProperty property)
        {
            int row1 = GetParameterInt32(ptrMessage, 0);
            int column1 = GetParameterInt32(ptrMessage, 1);
            int row2 = GetParameterInt32(ptrMessage, 2);
            int column2 = GetParameterInt32(ptrMessage, 3);
            object sourceObject = GetObjectFromDatastore(ptrMessage->SourceStore);
            object destinationObject = null;

            if (sourceObject != null)
            {
                object[] theParameters = { sourceObject, row1, column1, row2, column2 };
                switch (property)
                {
                    case CellProperty.BackColourName:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeBackColourNameDelegater, theParameters);
                        break;
                    case CellProperty.ForeColourName:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeForeColourNameDelegater, theParameters);
                        break;
                    case CellProperty.DataType:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeDataTypeDelegater, theParameters);
                        break;
                    case CellProperty.CheckBox:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeCheckBoxDelegater, theParameters);
                        break;
                    case CellProperty.Image:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeImageDelegater, theParameters);
                        break;
                    case CellProperty.BackgroundImage:
                        destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetCellRangeBackgroundImageDelegater, theParameters);
                        break;
                    default:
                        throw new Exception("Implement support for getting cell property " + property.ToString());
                }
            }

            PutObjectInDatastore(ptrMessage->DestinationStore, destinationObject);
            CleanUpMessage(ptrMessage);
        }

        //
        //  FlexgridGetCellRangeBackColourName
        //

        private delegate string GetCellRangeBackColourNameDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeBackColourNameDelegate m_FlexgridGetCellRangeBackColourNameDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of the back colours of the cells
        /// any cells which have the default back colour of the grid return an empty string for that cell
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the back colours</returns>
        private string FlexgridGetCellRangeBackColourNameInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);
            string gridBackColourName = grid.BackColor.Name;

            string currentCellBackColourName;
            StringBuilder rangeBackColourName = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    currentCellBackColourName = grid.GetCellRange(row, column).StyleDisplay.BackColor.Name;

                    if (gridBackColourName == currentCellBackColourName)
                    {
                        //nothing as its the default grid colour
                    }
                    else
                    {
                        rangeBackColourName.Append(currentCellBackColourName);
                    }

                    if (column < column2)
                    {
                        rangeBackColourName.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeBackColourName.Append(m_RowSeparator);
                }
            }

            return rangeBackColourName.ToString();
        }

        //
        //  FlexgridGetCellRangeForeColourName
        //

        private delegate string GetCellRangeForeColourNameDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeForeColourNameDelegate m_FlexgridGetCellRangeForeColourNameDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of the fore colours of the cells
        /// any cells which have the default fore colour of the grid return an empty string for that cell
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the fore colours</returns>
        private string FlexgridGetCellRangeForeColourNameInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);
            string gridForeColourName = grid.ForeColor.Name;

            string currentCellForeColourName;
            StringBuilder rangeForeColourName = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    currentCellForeColourName = grid.GetCellRange(row, column).StyleDisplay.ForeColor.Name;

                    if (gridForeColourName == currentCellForeColourName)
                    {
                        //nothing as its the default grid colour
                    }
                    else
                    {
                        rangeForeColourName.Append(currentCellForeColourName);
                    }

                    if (column < column2)
                    {
                        rangeForeColourName.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeForeColourName.Append(m_RowSeparator);
                }
            }

            return rangeForeColourName.ToString();
        }

        //
        //  FlexgridGetCellRangeDataType
        //

        private delegate string GetCellRangeDataTypeDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeDataTypeDelegate m_FlexgridGetCellRangeDataTypeDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of the data type of the cells
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the data type</returns>
        private string FlexgridGetCellRangeDataTypeInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);

            Type currentCellDataType;
            StringBuilder rangeDataType = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    currentCellDataType = grid.GetCellRange(row, column).DataType;

                    rangeDataType.Append(currentCellDataType.Namespace);
                    rangeDataType.Append(".");
                    rangeDataType.Append(currentCellDataType.Name);

                    if (column < column2)
                    {
                        rangeDataType.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeDataType.Append(m_RowSeparator);
                }
            }

            return rangeDataType.ToString();
        }

        //
        //  FlexgridGetCellRangeCheckBox
        //

        private delegate string GetCellRangeCheckBoxDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeCheckBoxDelegate m_FlexgridGetCellRangeCheckBoxDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of the checkbox state of the cells
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the data type</returns>
        private string FlexgridGetCellRangeCheckBoxInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);

            string currentCellCheckBox;
            StringBuilder rangeDataType = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    currentCellCheckBox = grid.GetCellRange(row, column).Checkbox.ToString();

                    rangeDataType.Append(currentCellCheckBox);

                    if (column < column2)
                    {
                        rangeDataType.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeDataType.Append(m_RowSeparator);
                }
            }

            return rangeDataType.ToString();
        }

        //
        //  FlexgridGetCellRangeImage
        //

        private delegate string GetCellRangeImageDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeImageDelegate m_FlexgridGetCellRangeImageDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of whether the cell contains an image
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the data type</returns>
        private string FlexgridGetCellRangeImageInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);

            StringBuilder rangeDataType = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    if (grid.GetCellRange(row, column).Image != null)
                    {
                        rangeDataType.Append("Image");
                    }

                    if (column < column2)
                    {
                        rangeDataType.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeDataType.Append(m_RowSeparator);
                }
            }

            return rangeDataType.ToString();
        }

        //
        //  FlexgridGetCellRangeBackgroundImage
        //

        private delegate string GetCellRangeBackgroundImageDelegate(dynamic grid, int row1, int column1, int row2, int column2);
        private GetCellRangeBackgroundImageDelegate m_FlexgridGetCellRangeBackgroundImageDelegater;

        /// <summary>
        /// Iterates over every cell in the grid returning a \t \r separated string of whether the cell contains an 
        /// background image
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <param name="row1">The start row of the range</param>
        /// <param name="column1">The start column of the range</param>
        /// <param name="row2">The end row of the range</param>
        /// <param name="column2">The end column of the range</param>
        /// <returns>A \t \r delimited string of the data type</returns>
        private string FlexgridGetCellRangeBackgroundImageInternal(dynamic grid, int row1, int column1, int row2, int column2)
        {
            DetermineClipSeparators(grid.ClipSeparators);

            StringBuilder rangeDataType = new StringBuilder(10240);

            for (int row = row1; row <= row2; row++)
            {
                for (int column = column1; column <= column2; column++)
                {
                    if (grid.GetCellRange(row, column).StyleDisplay.BackgroundImage != null)
                    {
                        rangeDataType.Append("BackgroundImage");
                    }

                    if (column < column2)
                    {
                        rangeDataType.Append(m_ColumnSeparator);
                    }
                }

                if (row < row2)
                {
                    rangeDataType.Append(m_RowSeparator);
                }
            }

            return rangeDataType.ToString();
        }

        //
        //  FlexgridGetAllColumnsHidden
        //

        private delegate string GetAllColumnsHiddenDelegate(dynamic grid);
        private GetAllColumnsHiddenDelegate m_FlexgridGetAllColumnsHiddenDelegater;

        /// <summary>
        /// Calls into the AUT to iterate over every column in the grid returning a comma separated string of whether
        /// the column is hidden (True) or visible (False)
        /// </summary>
        /// <param name="sourceStore">The datastore which contains the grid object</param>
        /// <param name="destinationStore">The datastore to put the resultant string into</param>
        unsafe public void AddQueryMessageFlexgridGetAllColumnsHidden(DataStores sourceStore, DataStores destinationStore)
        {
            if (!m_DoneFind)
            {
                throw new Exception("Must locate the flexgrid before trying to use it");
            }

            Message* ptrMessage = GetPointerToNextMessage();
            ptrMessage->SourceStore = sourceStore;
            ptrMessage->DestinationStore = destinationStore;

            ptrMessage->Action = MessageAction.FlexgridGetAllColumnsHidden;
            
            m_PtrMessageStore->NumberOfMessages++;
            m_DoneQuery = true;
        }

        /// <summary>
        /// Calls the FlexgridGetAllColumnsHiddenInternal method on the correct thread storing the results
        /// in the specified datastore
        /// </summary>
        /// <param name="ptrMessage">A pointer to the message</param>
        unsafe private void FlexgridGetAllColumnsHidden(Message* ptrMessage)
        {
            object sourceObject = GetObjectFromDatastore(ptrMessage->SourceStore);
            object destinationObject = null;

            if (sourceObject != null)
            {
                object[] theParameters = { sourceObject };
                destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetAllColumnsHiddenDelegater, theParameters);   
            }

            PutObjectInDatastore(ptrMessage->DestinationStore, destinationObject);
            CleanUpMessage(ptrMessage);
        }

        /// <summary>
        /// Iterates over every column in the grid returning a comma separated string of whether the column is
        /// hidden (True) or visible (False)
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <returns>A comma delimited string of whether the column is hidden</returns>
        private string FlexgridGetAllColumnsHiddenInternal(dynamic grid)
        {
            StringBuilder columnsHiddenState = new StringBuilder(10240);
            int columns = grid.Cols.Count;

            for (int column = 0; column < columns; column++)
            {
                bool visible = grid.Cols[column].Visible;

                if (column > 0)
                {
                    columnsHiddenState.Append(",");
                }

                if (visible)
                {
                    columnsHiddenState.Append("False");
                }
                else
                {
                    columnsHiddenState.Append("True");
                }
            }

            return columnsHiddenState.ToString();
        }

        //
        //  FlexgridGetAllColumnsWidth
        //

        private delegate string GetAllColumnsWidthDelegate(dynamic grid);
        private GetAllColumnsWidthDelegate m_FlexgridGetAllColumnsWidthDelegater;

        /// <summary>
        /// Calls into the AUT to iterate over every column in the grid returning a comma separated string of
        /// the columns width
        /// </summary>
        /// <param name="sourceStore">The datastore which contains the grid object</param>
        /// <param name="destinationStore">The datastore to put the resultant string into</param>
        unsafe public void AddQueryMessageFlexgridGetAllColumnsWidth(DataStores sourceStore, DataStores destinationStore)
        {
            if (!m_DoneFind)
            {
                throw new Exception("Must locate the flexgrid before trying to use it");
            }

            Message* ptrMessage = GetPointerToNextMessage();
            ptrMessage->SourceStore = sourceStore;
            ptrMessage->DestinationStore = destinationStore;

            ptrMessage->Action = MessageAction.FlexgridGetAllColumnsWidth;

            m_PtrMessageStore->NumberOfMessages++;
            m_DoneQuery = true;
        }

        /// <summary>
        /// Calls the FlexgridGetAllColumnsWidthInternal method on the correct thread storing the results
        /// in the specified datastore
        /// </summary>
        /// <param name="ptrMessage">A pointer to the message</param>
        unsafe private void FlexgridGetAllColumnsWidth(Message* ptrMessage)
        {
            object sourceObject = GetObjectFromDatastore(ptrMessage->SourceStore);
            object destinationObject = null;

            if (sourceObject != null)
            {
                object[] theParameters = { sourceObject };
                destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetAllColumnsWidthDelegater, theParameters);
            }

            PutObjectInDatastore(ptrMessage->DestinationStore, destinationObject);
            CleanUpMessage(ptrMessage);
        }

        /// <summary>
        /// Iterates over every column in the grid returning a comma separated string of the column width
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <returns>A comma delimited string of the column width</returns>
        private string FlexgridGetAllColumnsWidthInternal(dynamic grid)
        {
            StringBuilder columnsWidth = new StringBuilder(10240);
            int columns = grid.Cols.Count;

            for (int column = 0; column < columns; column++)
            {
                int width = grid.Cols[column].WidthDisplay;
                if (column > 0)
                {
                    columnsWidth.Append(",");
                }
                columnsWidth.Append(width.ToString());
            }

            return columnsWidth.ToString();
        }

        //
        //  FlexgridGetAllRowsHidden
        //

        private delegate string GetAllRowsHiddenDelegate(dynamic grid);
        private GetAllRowsHiddenDelegate m_FlexgridGetAllRowsHiddenDelegater;

        /// <summary>
        /// Calls into the AUT to iterate over every row in the grid returning a comma separated string of whether
        /// the row is hidden (True) or visible (False)
        /// </summary>
        /// <param name="sourceStore">The datastore which contains the grid object</param>
        /// <param name="destinationStore">The datastore to put the resultant string into</param>
        unsafe public void AddQueryMessageFlexgridGetAllRowsHidden(DataStores sourceStore, DataStores destinationStore)
        {
            if (!m_DoneFind)
            {
                throw new Exception("Must locate the flexgrid before trying to use it");
            }

            Message* ptrMessage = GetPointerToNextMessage();
            ptrMessage->SourceStore = sourceStore;
            ptrMessage->DestinationStore = destinationStore;

            ptrMessage->Action = MessageAction.FlexgridGetAllRowsHidden;

            m_PtrMessageStore->NumberOfMessages++;
            m_DoneQuery = true;
        }

        /// <summary>
        /// Calls the FlexgridGetAllRowsHiddenInternal method on the correct thread storing the results
        /// in the specified datastore
        /// </summary>
        /// <param name="ptrMessage">A pointer to the message</param>
        unsafe private void FlexgridGetAllRowsHidden(Message* ptrMessage)
        {
            object sourceObject = GetObjectFromDatastore(ptrMessage->SourceStore);
            object destinationObject = null;

            if (sourceObject != null)
            {
                object[] theParameters = { sourceObject };
                destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetAllRowsHiddenDelegater, theParameters);
            }

            PutObjectInDatastore(ptrMessage->DestinationStore, destinationObject);
            CleanUpMessage(ptrMessage);
        }

        /// <summary>
        /// Iterates over every row in the grid returning a comma separated string of whether the row is
        /// hidden (True) or visible (False)
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <returns>A comma delimited string of whether the row is hidden</returns>
        private string FlexgridGetAllRowsHiddenInternal(dynamic grid)
        {
            StringBuilder rowsHiddenState = new StringBuilder(10240);
            int rows = grid.Rows.Count;

            for (int row = 0; row < rows; row++)
            {
                bool visible = grid.Rows[row].Visible;

                if (row > 0)
                {
                    rowsHiddenState.Append(",");
                }

                if (visible)
                {
                    rowsHiddenState.Append("False");
                }
                else
                {
                    rowsHiddenState.Append("True");
                }
            }

            return rowsHiddenState.ToString();
        }

        //
        //  FlexgridGetAllRowsHeight
        //

        private delegate string GetAllRowsHeightDelegate(dynamic grid);
        private GetAllRowsHeightDelegate m_FlexgridGetAllRowsHeightDelegater;

        /// <summary>
        /// Calls into the AUT to iterate over every row in the grid returning a comma separated string of
        /// the rows height
        /// </summary>
        /// <param name="sourceStore">The datastore which contains the grid object</param>
        /// <param name="destinationStore">The datastore to put the resultant string into</param>
        unsafe public void AddQueryMessageFlexgridGetAllRowsHeight(DataStores sourceStore, DataStores destinationStore)
        {
            if (!m_DoneFind)
            {
                throw new Exception("Must locate the flexgrid before trying to use it");
            }

            Message* ptrMessage = GetPointerToNextMessage();
            ptrMessage->SourceStore = sourceStore;
            ptrMessage->DestinationStore = destinationStore;

            ptrMessage->Action = MessageAction.FlexgridGetAllRowsHeight;

            m_PtrMessageStore->NumberOfMessages++;
            m_DoneQuery = true;
        }

        /// <summary>
        /// Calls the FlexgridGetAllRowsHeightInternal method on the correct thread storing the results
        /// in the specified datastore
        /// </summary>
        /// <param name="ptrMessage">A pointer to the message</param>
        unsafe private void FlexgridGetAllRowsHeight(Message* ptrMessage)
        {
            object sourceObject = GetObjectFromDatastore(ptrMessage->SourceStore);
            object destinationObject = null;

            if (sourceObject != null)
            {
                object[] theParameters = { sourceObject };
                destinationObject = ((WF.Control)tempStore0).Invoke(m_FlexgridGetAllRowsHeightDelegater, theParameters);
            }

            PutObjectInDatastore(ptrMessage->DestinationStore, destinationObject);
            CleanUpMessage(ptrMessage);
        }

        /// <summary>
        /// Iterates over every row in the grid returning a comma separated string of the row height
        /// </summary>
        /// <param name="grid">The grid object</param>
        /// <returns>A comma delimited string of the row height</returns>
        private string FlexgridGetAllRowsHeightInternal(dynamic grid)
        {
            StringBuilder rowHeight = new StringBuilder(10240);
            int rows = grid.Rows.Count;

            for (int row = 0; row < rows; row++)
            {
                int height = grid.Rows[row].HeightDisplay;
                if (row > 0)
                {
                    rowHeight.Append(",");
                }
                rowHeight.Append(height.ToString());
            }

            return rowHeight.ToString();
        }
    }
}
