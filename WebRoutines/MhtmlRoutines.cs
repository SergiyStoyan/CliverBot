////********************************************************************************************
////Author: Sergey Stoyan
////        stoyan@cliversoft.com        
////        sergey.stoyan@gmail.com
////        sergey_stoyan@yahoo.com
////        http://www.cliversoft.com
////        26 September 2006
////Copyright: (C) 2006, Sergey Stoyan
////********************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
//using System.Text.RegularExpressions;
//using System.Xml;
//using System.Diagnostics;
//using System.Configuration;
////using MSHTML;
//using System.Threading;
//using System.Collections;
//using mshtml;

//namespace Cliver.Bot
//{
//    public partial class IeRoutine
//    {
//        static public mshtml.HTMLTableRow GetRow(HTMLTable t, int index)
//        {
//            try
//            {
//                int i = 0;
//                foreach (HTMLTableRow r in t.rows)
//                {
//                    if (i == index)
//                        return r;

//                    i++;
//                }
//            }
//            catch (ThreadAbortException)
//            {
//            }
//            catch (Exception ex)
//            {
//                LogMessage.Error(ex);
//            }
//            return null;
//        }

//        static public HTMLTableCell GetCell(HTMLTableRow r, int index)
//        {
//            try
//            {
//                int i = 0;
//                foreach (HTMLTableCell c in r.cells)
//                {
//                    if (i == index)
//                        return c;

//                    i++;
//                }
//            }
//            catch (ThreadAbortException)
//            {
//            }
//            catch (Exception ex)
//            {
//                LogMessage.Error(ex);
//            }
//            return null;
//        }

//        //static public IHTMLElement FindElementByFragment(HTMLDocument d, string fragment, bool within_html)
//        //{
//        //    try
//        //    {
//        //        foreach (IHTMLElement he in d.all)
//        //        {
//        //            if (within_html)
//        //            {
//        //                if (he.outerHTML.Contains(fragment))
//        //                    return he;
//        //            }
//        //            else
//        //            {
//        //                if (he.outerText.Contains(fragment))
//        //                    return he;
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        LogMessage.Error(ex);
//        //    }
//        //    return null;
//        //}

//        /// <summary>
//        /// Find and return deepest IHTMLElement which contains specified string fragment from first DOM branch where this fragment was found.
//        /// </summary>
//        /// <param name="phe">element where fragment is looked for </param>
//        /// <param name="fragment">string to be found</param>
//        /// <param name="within_html">define if fragment should be found within html code. 
//        /// If search is within html then it is case ingnored since IE can change cases and ATTANSION! even order of attributes</param>        
//        /// <param name="tag_name">look within elements with specified tag name only. If tag_name == null then it is ignored</param>
//        /// <returns>needed element else null</returns>
//        public IHTMLElement FindDeepestElementWithFragment(IHTMLElement phe, string fragment, bool within_html, string tag_name)
//        {
//            try
//            {
//                if (phe == null)
//                    return null;
//                if(tag_name != null)
//                    tag_name = tag_name.Trim().ToLower();

//                string html_fragment = null;
//                if (within_html)
//                    html_fragment = fragment.ToLower();

//                foreach (IHTMLElement he in (IHTMLElementCollection)phe.all)
//                {
//                    if (tag_name != null && he.tagName.ToLower() != tag_name)
//                        continue;
//                    if (within_html)
//                    {
//                        string d = he.outerHTML;
//                        if (he.outerHTML != null && he.outerHTML.ToLower().Contains(html_fragment))
//                        {
//                            IHTMLElement ceh = FindDeepestElementWithFragment(he, fragment, within_html, tag_name);
//                            if (ceh == null)
//                                return he;
//                            else
//                                return ceh;
//                        }
//                    }
//                    else
//                    {
//                        if (he.outerText != null && he.outerText.Contains(fragment))
//                        {
//                            IHTMLElement ceh = FindDeepestElementWithFragment(he, fragment, within_html, tag_name);
//                            if (ceh == null)
//                                return he;
//                            else
//                                return ceh;
//                        }
//                    }
//                }
//            }
//            catch (ThreadAbortException)
//            {
//            }
//            catch (Exception ex)
//            {
//                LogMessage.Error(ex);
//            }
//            return null;
//        }

//        public IEHtmlForm ParseIEHtmlForm(HTMLDocument d, string fragment, bool within_html)
//        {
//            try
//            {
//                if (d == null)
//                    return null;
//                if (fragment == null)
//                    fragment = "<form";
//                HTMLFormElement form = (HTMLFormElement)FindDeepestElementWithFragment(d.body, fragment, within_html, "form");
//                if (form == null)
//                    return null;

//                IEHtmlForm hf = new IEHtmlForm(form);
                
//                foreach (IHTMLElement ie in form.getElementsByTagName("input"))
//                {
//                    string input_element = ie.outerHTML;

//                    string type = "";
//                    Match m = Regex.Match(input_element, @"\stype=['""]?(.*?)['""\s>]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
//                    if (m.Success)
//                    {
//                        type = m.Result("$1");
//                    }
//                    type = type.ToUpper().Trim();

//                    string name = ((IHTMLInputElement)ie).name;
//                    if (name == null)
//                        name = ie.id;
//                    if (name == null)
//                        name = type;
//                    if (name == null)
//                        continue;

//                    if (type == HtmlForm.ParameterType.HIDDEN.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.HIDDEN, ie);
//                    else if (type == "" || type == HtmlForm.ParameterType.TEXT.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.TEXT, ie);
//                    else if (type == HtmlForm.ParameterType.CHECKBOX.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.CHECKBOX, ie);
//                    else if (type == HtmlForm.ParameterType.RADIO.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.RADIO, ie);
//                    else if (type == HtmlForm.ParameterType.SUBMIT.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.SUBMIT, ie);
//                    else if (type == HtmlForm.ParameterType.RESET.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.RESET, ie);
//                    else if (type == HtmlForm.ParameterType.BUTTON.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.BUTTON, ie);
//                    else if (type == HtmlForm.ParameterType.PASSWORD.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.PASSWORD, ie);
//                    else if (type == HtmlForm.ParameterType.FILE.ToString())
//                        hf.Add(name, HtmlForm.ParameterType.FILE, ie);
//                    else
//                        hf.Add(name, HtmlForm.ParameterType.OTHER, ie);
//                }
                               
//                foreach (IHTMLElement ie in form.getElementsByTagName("textarea"))
//                {
//                    string textarea_element = ie.outerHTML;
//                    string name = ((IHTMLTextAreaElement)ie).name;
//                    if (name == null)
//                        name = ie.id;
//                    if (name == null)
//                        continue;
//                    hf.Add(name, HtmlForm.ParameterType.TEXTAREA, ie);
//                }

//                foreach (IHTMLElement ie in form.getElementsByTagName("select"))
//                {
//                    string select_element = ie.outerHTML;
//                    string name = ((IHTMLSelectElement)ie).name;
//                    if (name == null)
//                        name = ie.id;
//                    if (name == null)
//                        continue;
//                    hf.Add(name, HtmlForm.ParameterType.SELECT, ie);
//                }

//                return hf;
//            }
//            catch (ThreadAbortException)
//            {
//            }
//            catch (Exception ex)
//            {
//                LogMessage.Error(ex);
//            }
//            return null;
//        }

//        /// <summary>
//        /// Tries to simulate a click on submit button rather to submit form
//        /// </summary>
//        /// <param name="form">form to submit</param>
//        public void SubmitForm(IEHtmlForm form)
//        {
//            string submit_element_name = null;
//            foreach (string name in form.Names)
//            {
//                if (form.GetType(name) == HtmlForm.ParameterType.SUBMIT)
//                {
//                    submit_element_name = name;
//                    break;
//                }
//            }

//            if (submit_element_name != null)
//                ((HTMLInputElement)form.GetElement(submit_element_name)).click();
//            else
//                form.Form.submit();

//            this.WaitUntilIEDocumentCompleted();
//        }
//    }

//    public class IEHtmlForm
//    {
//        //public string Url = "";

//        Hashtable elements = new Hashtable();
//        Hashtable parameter_types = new Hashtable();

//        public HTMLFormElement Form
//        {
//            get { return form; }
//        }
//        HTMLFormElement form;

//        public IEHtmlForm(HTMLFormElement form/*, WebRoutine web_routine*/)
//        {
//            this.form = form;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="name">name of element</param>
//        /// <returns>value of element</returns>
//        public string this[string name]
//        {
//            set
//            {
//                if (!elements.ContainsKey(name))
//                    throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

//                HtmlForm.ParameterType type = (HtmlForm.ParameterType)parameter_types[name];
//                if (type == HtmlForm.ParameterType.TEXTAREA)
//                    ((IHTMLTextAreaElement)elements[name]).value = value;
//                else if (type == HtmlForm.ParameterType.SELECT)
//                    ((IHTMLSelectElement)elements[name]).value = value;
//                else 
//                    ((IHTMLInputElement)elements[name]).value = value;
               
//            }
//            get
//            {
//                if (!elements.ContainsKey(name))
//                    throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

//                string value = null;

//                HtmlForm.ParameterType type = (HtmlForm.ParameterType)parameter_types[name];
//                if (type == HtmlForm.ParameterType.TEXTAREA)
//                    value = ((IHTMLTextAreaElement)elements[name]).value;
//                else if (type == HtmlForm.ParameterType.SELECT)
//                    value = ((IHTMLSelectElement)elements[name]).value;
//                else 
//                    value = ((IHTMLInputElement)elements[name]).value;

//                return value;
//            }
//        }

//        internal void Add(string name, HtmlForm.ParameterType type, IHTMLElement ie)
//        {
//            elements[name] = ie;
//            parameter_types[name] = type;
//        }

//        public string[] Names
//        {
//            get
//            {
//                ArrayList names = new ArrayList();
//                foreach (string name in elements.Keys)
//                    names.Add(name);
//                return (string[])names.ToArray(typeof(string));
//            }
//        }

//        /// <summary>
//        /// Retrive type of form field
//        /// </summary>
//        /// <param name="name">name of field</param>
//        /// <returns></returns>
//        public HtmlForm.ParameterType GetType(string name)
//        {
//            if (!parameter_types.ContainsKey(name))
//                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

//            return (HtmlForm.ParameterType)parameter_types[name];
//        }

//        public IHTMLElement GetElement(string name)
//        {
//            if (!elements.ContainsKey(name))
//                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

//            return (IHTMLElement)elements[name];
//        }

//        //public void SubmitAndGetResult()
//        //{
//        //    form.submit();
//        //    WaitUntilIEDocumentCompleted();
//        //}
//    }
//}