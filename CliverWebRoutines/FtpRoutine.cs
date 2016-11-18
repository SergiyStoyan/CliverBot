using System;
using System.Collections;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Cliver.BotWeb
{
    public class FtpRoutine
    {
        public FtpRoutine(string base_url, string user, string password)
        {
            this.base_url = base_url;
            this.user = user;
            this.password = password;
        }
        string base_url;
        string user;
        string password;
        
        public bool GetFileList(string url, Regex filter, out string[] files)
        {
            ArrayList fs = new ArrayList();
            FtpWebRequest r = null;
            FtpWebResponse wr = null;

            try
            {
                r = (FtpWebRequest)FtpWebRequest.Create(url);
                r.KeepAlive = false;
                r.Method = WebRequestMethods.Ftp.ListDirectory;
                r.Credentials = new NetworkCredential(user, password);
                r.UseBinary = true;
                Log.Inform(r.Method + ":" + r.RequestUri.ToString());
                wr = (FtpWebResponse)r.GetResponse();
                System.IO.Stream stream = wr.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                if (filter != null)
                    filter = new Regex(filter.ToString(), filter.Options | RegexOptions.Compiled);

                string dirorfile = null;
                while ((dirorfile = sr.ReadLine()) != null)
                {
                    if (filter != null && !filter.IsMatch(dirorfile))
                        continue;
                    fs.Add(dirorfile);
                }
                files = (string[])fs.ToArray(typeof(string));
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                try
                {
                    if (wr != null)
                        wr.Close();
                    //if (ftp != null)
                    //    ftp.Abort();

                }
                catch { };
            }
            files = null;
            return false;
        }

        public bool GetFile(string url, string to_file)
        {
            Log.Inform("GetFile:" + url);

            FtpWebRequest r = null;
            FtpWebResponse wr = null;

            try
            {
                r = (FtpWebRequest)FtpWebRequest.Create(url);
                r.KeepAlive = false;
                r.Method = WebRequestMethods.Ftp.DownloadFile;
                r.Credentials = new NetworkCredential(user, password);
                r.UseBinary = true;
                Log.Inform(r.Method + ":" + r.RequestUri.ToString());
                wr = (FtpWebResponse)r.GetResponse();
                System.IO.Stream s = wr.GetResponseStream();
                FileStream fs = new FileStream(to_file, FileMode.Create, FileAccess.Write);
                byte[] buff = new byte[1000];
                do
                {
                    int c = s.Read(buff, 0, buff.Length);
                    if (c < 1)
                        break;
                    fs.Write(buff, 0, c);
                } while (true);
                fs.Close();
                s.Close();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                try
                {
                    if (wr != null)
                        wr.Close();
                    //if (ftp != null)
                    //    ftp.Abort();
                }
                catch { };
            }
            return false;
        }

        /*public bool DirExists(string dir)
        {
            try
            {
                FtpWebRequest r = (FtpWebRequest)WebRequest.Create("ftp://ftp.microsoft.com/12345");
                r.Method = WebRequestMethods.Ftp.ListDirectory;
                using (FtpWebResponse response = (FtpWebResponse)r.GetResponse())
                {
                    // Okay.  
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        // Directory not found.  
                    }
                }
            }
        }*/

        public bool MakeDir(string dir)
        {
            FtpWebResponse rr = null;
            try
            {
                if (string.IsNullOrEmpty(dir))
                    return true;
                WebRequest r = WebRequest.Create(base_url + "/" + dir);
                r.Method = WebRequestMethods.Ftp.MakeDirectory;
                r.Credentials = new NetworkCredential(user, password);
                Log.Inform(r.Method + ":" + r.RequestUri.ToString());
                rr = (FtpWebResponse)r.GetResponse();
                return true;
            }
            catch (Exception e)
            {
                if (e is System.Net.WebException && ((FtpWebResponse)((System.Net.WebException)e).Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    Log.Inform("Directory exists:" + dir);
                    return true;
                }
                Log.Error(e);
            }
            finally
            {
                if (rr != null)
                    rr.Close();
            }
            return false;
        }

        public bool UploadFile(string dir, string file)
        {
            FileStream fs = null;
            FtpWebResponse rr = null;
            try
            {
                FileInfo fi = new FileInfo(file);
                FtpWebRequest r = (FtpWebRequest)WebRequest.Create(base_url + "/" + dir + "/" + fi.Name);
                r.Method = WebRequestMethods.Ftp.UploadFile;
                r.Credentials = new NetworkCredential(user, password);
                r.Timeout = 20000;
                r.UseBinary = true;
                Log.Inform(r.Method + ":" + r.RequestUri.ToString());

                fs = File.OpenRead(file);
                byte[] bs = new byte[fs.Length];
                fs.Read(bs, 0, bs.Length);
                fs.Close();

                r.ContentLength = bs.Length;

                Stream rs = r.GetRequestStream();
                rs.Write(bs, 0, bs.Length);
                rs.Close();

                rr = (FtpWebResponse)r.GetResponse();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (rr != null)
                    rr.Close();
            }
            return false;
        }
    }
}