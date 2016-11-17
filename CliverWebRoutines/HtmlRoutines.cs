//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
//using MSHTML;
using System.Windows.Forms;
using System.Collections;

namespace Cliver.Bot
{
    /// <summary>
    /// Defines methods for 
    /// </summary>
    public partial class HttpRoutine
    {
        /// <summary>
        /// Parse input html page for form containing specified fragment and create HtmlForm from it
        /// </summary>
        /// <param name="page">html page</param>
        /// <param name="url">url of the input html page</param>
        /// <param name="fragment">part of html code that contained by the form. Can be null.</param>
        /// <returns>object presenting found and parsed form</returns>
        public HtmlForm ParseForm(string page, string url, string fragment)
        {
            HtmlForm hf = null;

            try
            {
                Match m = Regex.Match(page, @"<form\s.*?</form.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                while (m.Success)
                {
                    if (fragment == null || m.Result("$0").Contains(fragment))
                    {
                        hf = new HtmlForm();
                        break;
                    }
                    m = m.NextMatch();
                }

                if (hf == null)
                    return null;

                string form = m.Result("$0");

                m = Regex.Match(form, @"<form[^>]*?\smethod=(['""]?)(.*?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (m.Success && m.Result("$2").ToUpper() == HttpRequest.RequestMethod.POST.ToString())
                {
                    hf.Method = HttpRequest.RequestMethod.POST;
                }

                m = Regex.Match(form, @"<form[^>]*?\saction=(['""]?)(.*?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    Uri ubase = new Uri(url, UriKind.Absolute);
                    string link = HttpUtility.HtmlDecode(m.Result("$2"));
                    Uri ulink = new Uri(ubase, link);
                    hf.Url = ulink.AbsoluteUri.Trim();
                }
                else
                {
                    hf.Url = url;
                }

                m = Regex.Match(form, @"<input\s[^>]+?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                while (m.Success)
                {
                    string input_element = m.Result("$0");
                    Match m2 = Regex.Match(input_element, @"\sname=(['""]?)(.+?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (!m2.Success)
                    {
                        m = m.NextMatch();
                        continue;
                    }

                    string name = m2.Result("$2");

                    string type = "";
                    m2 = Regex.Match(input_element, @"\stype=(['""]?)(.*?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (m2.Success)
                    {
                        type = m2.Result("$2");
                    }
                    type = type.ToUpper().Trim();

                    bool ignore_value = false;
                    if (type == "CHECKBOX")
                    {
                        m2 = Regex.Match(input_element, @"\s(['""]?)checked\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        if (!m2.Success)
                            ignore_value = true;
                    }

                    string value = null;
                    m2 = Regex.Match(input_element, @"\svalue=(['""]?)(.*?)\1", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (!m2.Success)
                        m2 = Regex.Match(input_element, @"\svalue(=)(.*?)[\s>]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (m2.Success)
                    {
                        if (!ignore_value)
                            value = m2.Result("$2");
                    }

                    if (type == HtmlForm.ParameterType.HIDDEN.ToString())
                        hf.Create(name, HtmlForm.ParameterType.HIDDEN, value);
                    else if (type == "" || type == HtmlForm.ParameterType.TEXT.ToString())
                        hf.Create(name, HtmlForm.ParameterType.TEXT, value);
                    else if (type == HtmlForm.ParameterType.CHECKBOX.ToString())
                        hf.Create(name, HtmlForm.ParameterType.CHECKBOX, value);
                    else if (type == HtmlForm.ParameterType.RADIO.ToString())
                        hf.Create(name, HtmlForm.ParameterType.RADIO, value);
                    else if (type == HtmlForm.ParameterType.SUBMIT.ToString())
                        hf.Create(name, HtmlForm.ParameterType.SUBMIT, value);
                    else if (type == HtmlForm.ParameterType.RESET.ToString())
                        hf.Create(name, HtmlForm.ParameterType.RESET, value);
                    else if (type == HtmlForm.ParameterType.BUTTON.ToString())
                        hf.Create(name, HtmlForm.ParameterType.BUTTON, value);
                    else if (type == HtmlForm.ParameterType.PASSWORD.ToString())
                        hf.Create(name, HtmlForm.ParameterType.PASSWORD, value);
                    else if (type == HtmlForm.ParameterType.FILE.ToString())
                        hf.Create(name, HtmlForm.ParameterType.FILE, value);
                    else
                        hf.Create(name, HtmlForm.ParameterType.OTHER, value);

                    m = m.NextMatch();
                }

                m = Regex.Match(form, @"<textarea(\s[^>]*?)>(.*?)</textarea", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                while (m.Success)
                {
                    Match m2 = Regex.Match(m.Result("$1"), @"\sname=['""]?(.*?)['""\s>]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (!m2.Success)
                        continue;

                    string name = m2.Result("$1");
                    string value = m.Result("$2");

                    hf.Create(name, HtmlForm.ParameterType.TEXTAREA, value);

                    m = m.NextMatch();
                }

                m = Regex.Match(form, @"<select(\s[^>]*?>)(.*?)</select", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                while (m.Success)
                {
                    Match m2 = Regex.Match(m.Result("$1"), @"[\s""']name=(['""]?)(.*?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (!m2.Success)
                        continue;

                    string name = m2.Result("$2");
                    hf.Create(name, HtmlForm.ParameterType.SELECT);

                    m2 = Regex.Match(m.Result("$2"), @"<OPTION(.*?>)(.*?)</OPTION", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    while (m2.Success)
                    {
                        if (Regex.IsMatch(m2.Result("$1"), @" selected", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase))
                        {
                            string value = null;
                            Match m3 = Regex.Match(m2.Result("$1"), @"\svalue=(['""]?)(.*?)\1.*?>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                            if (m3.Success)
                                value = m3.Result("$2");
                            else
                                value = m2.Result("$2");
                            hf.Add(name, value);
                        }
                        m2 = m2.NextMatch();
                    }

                    m = m.NextMatch();
                }

                return hf;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
            return null;
        }



        /// <summary>
        /// Parse input html page for first form and create HtmlForm from it
        /// </summary>
        /// <param name="page">html page</param>
        /// <param name="url">url of the input html page</param>
        /// <returns>object presenting found and parsed form</returns>
        public HtmlForm ParseForm(string page, string url)
        {           
            return ParseForm(page, url, null);
        }

        /// <summary>
        /// Submit form. /*//NOTICE: parameters with null value will not submitted.*/
        /// </summary>
        /// <param name="form">parsed form</param>
        /// <param name="send_cookies">defines whether cookies should be sent</param>
        /// <returns>true if received success response</returns>
        public bool SubmitForm(HtmlForm form, bool send_cookies)
        {
            return Do(new HttpRequest(form), send_cookies);
        }

        /// <summary>
        /// Submit form with sending cookies. /*//NOTICE: parameters with null value will not submitted.*/
        /// </summary>
        /// <param name="form">parsed form</param>
        /// <param name="send_cookies">defines whether cookies should be sent</param>
        /// <returns>true if received success response</returns>
        public bool SubmitForm(HtmlForm form)
        {
            return Do(new HttpRequest(form), true);
        }
    }

    /// <summary>
    /// Html form presentation
    /// </summary>
    public class HtmlForm
    {
        public enum ParameterType
        {
            TEXTAREA,
            TEXT,
            HIDDEN,
            SELECT,
            CHECKBOX,
            RADIO,
            SUBMIT,
            RESET,
            BUTTON,
            PASSWORD,
            FILE,
            OTHER
        }

        public HttpRequest.RequestMethod Method = HttpRequest.RequestMethod.GET;
        public string Url = "";

        Hashtable parameters = new Hashtable(); //name=>ArrayList
        Hashtable parameter_types = new Hashtable();
        
        public string[] this[string name]
        {
            set
            {
                if (!parameters.ContainsKey(name))
                    throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

                parameters[name] = (ArrayList)value.Clone();
            }
            get
            {
                if (!parameters.ContainsKey(name))
                    throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));
                
                ArrayList a = (ArrayList)parameters[name];
                return (string[])a.ToArray(typeof(string));
            }
        }

        /// <summary>
        /// Set only single value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, string value)
        {
            if (!parameters.ContainsKey(name))
                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

            ArrayList a = (ArrayList)parameters[name];
            a.Clear();
            a.Add(value);
        }

        /// <summary>
        /// Remove parameter from the form.
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            if (!parameters.ContainsKey(name))
                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));
            
            parameters.Remove(name);
        }

        /// <summary>
        /// Clear parameter from values.
        /// </summary>
        /// <param name="name"></param>
        public void Clear(string name)
        {
            if (!parameters.ContainsKey(name))
                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

            ArrayList a = (ArrayList)parameters[name];
            a.Clear();
        }

        /// <summary>
        /// Clear all parameters from values.
        /// </summary>
        public void ClearAll()
        {
            foreach (string name in this.Names)
                this.Clear(name);
        }

        /// <summary>
        /// Add value to parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            if (!parameters.ContainsKey(name))
                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

            ArrayList a = (ArrayList)parameters[name];
            a.Add(value);
        }

        /// <summary>
        /// Create new parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void Create(string name, ParameterType type, string value)
        {
            Create(name, type);
            Add(name, value);
        }

        /// <summary>
        /// Create new parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void Create(string name, ParameterType type)
        {
            parameters[name] = new ArrayList();
            parameter_types[name] = type;
        }
        
        //public ICollection Names
        //{
        //    get { return parameters.Keys; }
        //}

        public string[] Names
        {
            get
            {
                ArrayList names = new ArrayList();
                foreach (string name in parameters.Keys)
                    names.Add(name);
                return (string[])names.ToArray(typeof(string));
            }
        }        

        /// <summary>
        /// Retrive type of form field
        /// </summary>
        /// <param name="name">name of field</param>
        /// <returns></returns>
        public ParameterType GetType(string name)
        {
            if (!parameter_types.ContainsKey(name))
                throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

            return (ParameterType)parameter_types[name];
        }

        //public void SetType(string name, ParameterType type)
        //{
        //    if (!parameter_types.ContainsKey(name))
        //        throw (new Exception("ParsedForm hashtable does NOT contain parameter: '" + name + "'"));

        //    parameter_types[name] = type;
        //}
    }
}
