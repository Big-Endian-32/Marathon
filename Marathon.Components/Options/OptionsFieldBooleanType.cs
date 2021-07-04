﻿// Marathon is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 HyperBE32
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Reflection;
using System.Windows.Forms;

namespace Marathon.Components
{
    public partial class OptionsFieldBooleanType : UserControl
    {
        private string _OptionName, _OptionDescription;
        private PropertyInfo _OptionProperty;
        bool _OptionField;

        /// <summary>
        /// The name of the option.
        /// </summary>
        public string OptionName
        {
            get => _OptionName;

            set => CheckBox_Boolean.Text = _OptionName = value;
        }

        /// <summary>
        /// The description given to the option.
        /// </summary>
        public string OptionDescription
        {
            get => _OptionDescription;

            set => Label_Description.Text = _OptionDescription = value;
        }

        /// <summary>
        /// The property assigned to this option.
        /// </summary>
        public PropertyInfo OptionProperty
        {
            get => _OptionProperty;

            set
            {
                _OptionProperty = value;

                if (_OptionProperty != null)
                    CheckBox_Boolean.Checked = (bool)_OptionProperty.GetValue(value);
            }
        }

        /// <summary>
        /// The Boolean assigned to this option.
        /// </summary>
        private bool OptionField
        {
            get => _OptionField;

            set
            {
                _OptionField = value;

                if (OptionProperty != null)
                    OptionProperty.SetValue(OptionProperty, value);
            }
        }

        public OptionsFieldBooleanType()
            => InitializeComponent();

        /// <summary>
        /// Sets the property to the current Boolean value.
        /// </summary>
        private void CheckBox_Boolean_CheckedChanged(object sender, EventArgs e)
            => OptionField = CheckBox_Boolean.Checked;
    }
}
