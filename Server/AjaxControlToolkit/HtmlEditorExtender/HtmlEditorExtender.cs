﻿using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using System.ComponentModel;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[assembly: WebResource("HtmlEditorExtender.HtmlEditorExtenderBehavior.js", "text/javascript")]
[assembly: WebResource("HtmlEditorExtender.HtmlEditorExtenderBehavior.debug.js", "text/javascript")]
[assembly: WebResource("HtmlEditorExtender.HtmlEditorExtender_resource.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("HtmlEditorExtender.Images.html-editor-buttons.png", "img/png")]

namespace AjaxControlToolkit
{

    /// <summary>
    /// HtmlEditorExtender extends to a textbox and creates and renders an editable div 
    /// in place of targeted textbox.
    /// </summary>
    [TargetControlType(typeof(TextBox))]
    [RequiredScript(typeof(CommonToolkitScripts), 0)]
    [ClientScriptResource("Sys.Extended.UI.HtmlEditorExtenderBehavior", "HtmlEditorExtender.HtmlEditorExtenderBehavior.js")]
    [ClientCssResource("HtmlEditorExtender.HtmlEditorExtender_resource.css")]
    public class HtmlEditorExtender : ExtenderControlBase
    {
        internal const int ButtonWidthDef = 23;
        internal const int ButtonHeightDef = 21;
        List<HtmlEditorExtenderButton> buttonList = null;

        public HtmlEditorExtender()
        {
            EnableClientState = true;
        }

        /// <summary>
        /// Get or set list of toolbar buttons
        /// </summary>
        [ExtenderControlProperty(true, true)]
        [DefaultValue(null)]
        public HtmlEditorExtenderButton[] ToolbarButtons
        {
            get
            {
                if (buttonList == null)
                {
                    CreateButtons();
                }
                return GetPropertyValue("ToolbarButtons", buttonList.ToArray());
            }
            set { SetPropertyValue("ToolbarButtons", value); }
        }
                
        private string DecodeValues(string value)
        {
            if (buttonList == null)
            {
                CreateButtons();
            }
            foreach (HtmlEditorExtenderButton button in buttonList)
            {
                value = button.Decode(value);
            }
            value = Decode(value);
            return value;
        }

        /// <summary>
        /// Decodes html tags those are not generated by any htmlEditorExtender button
        /// </summary>
        /// <param name="value">Value that contains html tags to decode</param>
        /// <returns>value after decoded</returns>
        protected virtual string Decode(string value)
        {
            var result = Regex.Replace(value, "&lt;br&gt;", "<br>", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "&lt;/br&gt;", "</br>", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "&amp;", "&", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "&nbsp;", "\xA0", RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>
        /// Get or Set width of button icon
        /// </summary>
        [ExtenderProvidedProperty]
        [DefaultValue(ButtonWidthDef)]
        public int ButtonWidth
        {
            get { return GetPropertyValue<int>("ButtonWidth", 23); }
            set { SetPropertyValue<int>("ButtonWidth", value); }
        }

        /// <summary>
        /// Get or set height of button icon
        /// </summary>
        [ExtenderProvidedProperty]
        [DefaultValue(ButtonHeightDef)]
        public int ButtonHeight
        {
            get { return GetPropertyValue<int>("ButtonHeight", 21); }
            set { SetPropertyValue<int>("ButtonHeight", value); }
        }

        /// <summary>
        /// On load method decode contents of textbox before render these to client side.
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Register an empty OnSubmit statement so the ASP.NET WebForm_OnSubmit method will be automatically
            // created and our behavior will be able to wrap it to encode html tags prior to submission
            ScriptManager.RegisterOnSubmitStatement(this, typeof(HtmlEditorExtender), "HtmlEditorExtenderOnSubmit", "null;");

            // If this extender has default focus, use ClientState to let it know
            ClientState = (string.Compare(Page.Form.DefaultFocus, TargetControlID, StringComparison.OrdinalIgnoreCase) == 0) ? "Focused" : null;

            // decode values of textbox
            TextBox txtBox = (TextBox)TargetControl;
            txtBox.Text = DecodeValues(txtBox.Text);
        }

        /// <summary>
        /// CreateButtons creates list of buttons for the toolbar
        /// </summary>
        protected virtual void CreateButtons()
        {
            buttonList = new List<HtmlEditorExtenderButton>();
            buttonList.Add(new Bold());
            buttonList.Add(new Italic());
            buttonList.Add(new Underline());
            buttonList.Add(new StrikeThrough());
            buttonList.Add(new Subscript());
            buttonList.Add(new Superscript());
            buttonList.Add(new JustifyLeft());
            buttonList.Add(new JustifyRight());
            buttonList.Add(new JustifyCenter());
            buttonList.Add(new insertOrderedList());
            buttonList.Add(new insertUnorderedList());
        }
    }

}
