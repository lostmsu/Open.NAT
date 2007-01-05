//
// Message.cs
//
// Authors:
//   Alan McGovern alan.mcgovern@gmail.com
//
// Copyright (C) 2006 Alan McGovern
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//



using System;
using System.Diagnostics;
using System.Xml;
using Nat;

namespace Nat.UpnpMessages
{
    internal static class Message
    {
        public static IMessage Decode(string message)
        {
            XmlNode node = null;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(message);

            XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);

            // Error messages should be found under this namespace
            nsm.AddNamespace("errorNs", "urn:schemas-upnp-org:control-1-0");
            nsm.AddNamespace("responseNs", "urn:schemas-upnp-org:service:WANIPConnection:1");

            // Check to see if we have a fault code message.
            if ((node = doc.SelectSingleNode("//errorNs:UPnPError", nsm)) != null)
                return new ErrorMessage(Convert.ToInt32(node["errorCode"].InnerText, System.Globalization.CultureInfo.InvariantCulture), node["errorDescription"].InnerText);

            if ((node = doc.SelectSingleNode("//responseNs:AddPortMappingResponse", nsm)) != null)
                return new AddMappingResponseMessage();

            if ((node = doc.SelectSingleNode("//responseNs:DeletePortMappingResponse", nsm)) != null)
                return new DeletePortMapResponseMessage();

            if ((node = doc.SelectSingleNode("//responseNs:GetExternalIPAddressResponse", nsm)) != null)
                return new ExternalIPAddressMessage(node["NewExternalIPAddress"].InnerText);


            Trace.WriteLine("Unknown message returned. Please send me back the following XML:");
            Trace.WriteLine(message);
            return null;
        }
    }
}
