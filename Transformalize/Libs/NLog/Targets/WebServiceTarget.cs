#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Calls the specified web service on each log message.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/WebService_target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    ///     The web service must implement a method that accepts a number of string parameters.
    /// </remarks>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/WebService/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/WebService/Simple/Example.cs" />
    ///     <p>The example web service that works with this example is shown below</p>
    ///     <code lang="C#" source="examples/targets/Configuration API/WebService/Simple/WebService1/Service1.asmx.cs" />
    /// </example>
    [Target("WebService")]
    public sealed class WebServiceTarget : MethodCallTargetBase
    {
        private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string Soap12EnvelopeNamespace = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebServiceTarget" /> class.
        /// </summary>
        public WebServiceTarget()
        {
            Protocol = WebServiceProtocol.Soap11;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///     Gets or sets the web service URL.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Uri Url { get; set; }

        /// <summary>
        ///     Gets or sets the Web service method name.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public string MethodName { get; set; }

        /// <summary>
        ///     Gets or sets the Web service namespace.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets or sets the protocol to be used when calling web service.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        [DefaultValue("Soap11")]
        public WebServiceProtocol Protocol { get; set; }

        /// <summary>
        ///     Gets or sets the encoding.
        /// </summary>
        /// <docgen category='Web Service Options' order='10' />
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Calls the target method. Must be implemented in concrete classes.
        /// </summary>
        /// <param name="parameters">Method call parameters.</param>
        protected override void DoInvoke(object[] parameters)
        {
            // method is not used, instead asynchronous overload will be used
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Invokes the web service method.
        /// </summary>
        /// <param name="parameters">Parameters to be passed.</param>
        /// <param name="continuation">The continuation.</param>
        protected override void DoInvoke(object[] parameters, AsyncContinuation continuation)
        {
            var request = (HttpWebRequest) WebRequest.Create(Url);
            byte[] postPayload = null;

            switch (Protocol)
            {
                case WebServiceProtocol.Soap11:
                    postPayload = PrepareSoap11Request(request, parameters);
                    break;

                case WebServiceProtocol.Soap12:
                    postPayload = PrepareSoap12Request(request, parameters);
                    break;

                case WebServiceProtocol.HttpGet:
                    throw new NotSupportedException();

                case WebServiceProtocol.HttpPost:
                    postPayload = PreparePostRequest(request, parameters);
                    break;
            }

            AsyncContinuation sendContinuation =
                ex =>
                    {
                        if (ex != null)
                        {
                            continuation(ex);
                            return;
                        }

                        request.BeginGetResponse(
                            r =>
                                {
                                    try
                                    {
                                        using (var response = request.EndGetResponse(r))
                                        {
                                        }

                                        continuation(null);
                                    }
                                    catch (Exception ex2)
                                    {
                                        if (ex2.MustBeRethrown())
                                        {
                                            throw;
                                        }

                                        continuation(ex2);
                                    }
                                },
                            null);
                    };

            if (postPayload != null && postPayload.Length > 0)
            {
                request.BeginGetRequestStream(
                    r =>
                        {
                            try
                            {
                                using (var stream = request.EndGetRequestStream(r))
                                {
                                    stream.Write(postPayload, 0, postPayload.Length);
                                }

                                sendContinuation(null);
                            }
                            catch (Exception ex)
                            {
                                if (ex.MustBeRethrown())
                                {
                                    throw;
                                }

                                continuation(ex);
                            }
                        },
                    null);
            }
            else
            {
                sendContinuation(null);
            }
        }

        private byte[] PrepareSoap11Request(HttpWebRequest request, object[] parameters)
        {
            request.Method = "POST";
            request.ContentType = "text/xml; charset=" + Encoding.WebName;

            if (Namespace.EndsWith("/", StringComparison.Ordinal))
            {
                request.Headers["SOAPAction"] = Namespace + MethodName;
            }
            else
            {
                request.Headers["SOAPAction"] = Namespace + "/" + MethodName;
            }

            using (var ms = new MemoryStream())
            {
                var xtw = XmlWriter.Create(ms, new XmlWriterSettings
                                                   {
                                                       Encoding = Encoding
                                                   });

                xtw.WriteStartElement("soap", "Envelope", SoapEnvelopeNamespace);
                xtw.WriteStartElement("Body", SoapEnvelopeNamespace);
                xtw.WriteStartElement(MethodName, Namespace);
                var i = 0;

                foreach (var par in Parameters)
                {
                    xtw.WriteElementString(par.Name, Convert.ToString(parameters[i], CultureInfo.InvariantCulture));
                    i++;
                }

                xtw.WriteEndElement(); // methodname
                xtw.WriteEndElement(); // Body
                xtw.WriteEndElement(); // soap:Envelope
                xtw.Flush();

                return ms.ToArray();
            }
        }

        private byte[] PrepareSoap12Request(HttpWebRequest request, object[] parameterValues)
        {
            request.Method = "POST";
            request.ContentType = "text/xml; charset=" + Encoding.WebName;

            using (var ms = new MemoryStream())
            {
                var xtw = XmlWriter.Create(ms, new XmlWriterSettings
                                                   {
                                                       Encoding = Encoding
                                                   });

                xtw.WriteStartElement("soap12", "Envelope", Soap12EnvelopeNamespace);
                xtw.WriteStartElement("Body", Soap12EnvelopeNamespace);
                xtw.WriteStartElement(MethodName, Namespace);
                var i = 0;
                foreach (var par in Parameters)
                {
                    xtw.WriteElementString(par.Name, Convert.ToString(parameterValues[i], CultureInfo.InvariantCulture));
                    i++;
                }

                xtw.WriteEndElement();
                xtw.WriteEndElement();
                xtw.WriteEndElement();
                xtw.Flush();

                return ms.ToArray();
            }
        }

        private byte[] PreparePostRequest(HttpWebRequest request, object[] parameterValues)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=" + Encoding.WebName;

            var separator = string.Empty;
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms, Encoding);
                sw.Write(string.Empty);
                var i = 0;
                foreach (var parameter in Parameters)
                {
                    sw.Write(separator);
                    sw.Write(parameter.Name);
                    sw.Write("=");
                    sw.Write(UrlHelper.UrlEncode(Convert.ToString(parameterValues[i], CultureInfo.InvariantCulture), true));
                    separator = "&";
                    i++;
                }

                sw.Flush();
                return ms.ToArray();
            }
        }
    }
}