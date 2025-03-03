﻿// Port in use exception class.
// Copyright (C) 2010 Lex Li
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;

namespace Lextm.SharpSnmpLib.Messaging
{
    /// <summary>
    /// Exception raised when an IP endpoint is already in use.
    /// </summary>
    [Serializable]
    public sealed class PortInUseException : SnmpException
    {
        /// <summary>
        /// Creates a <see cref="PortInUseException"/>.
        /// </summary>
        public PortInUseException()
        {
        }

        /// <summary>
        /// Creates a <see cref="PortInUseException"/> instance with a specific <see cref="String"/>.
        /// </summary>
        /// <param name="message">Message</param>
        public PortInUseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a <see cref="PortInUseException"/> instance with a specific <see cref="String"/> and an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public PortInUseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a <see cref="PortInUseException"/> instance.
        /// </summary>
        /// <param name="info">Info</param>
        /// <param name="context">Context</param>
        public PortInUseException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            var content = info.GetString("Endpoint");
            if (content == null)
            {
                return;
            }

#if NET6_0_OR_GREATER
            Endpoint = IPEndPoint.Parse(content);
#else
            Endpoint = Parse(content);
#endif
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Endpoint", Endpoint?.ToString());
        }

        /// <summary>
        /// The endpoint already in use.
        /// </summary>
        public IPEndPoint? Endpoint { get; set; }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this <see cref="PortInUseException"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "PortInUseException: {0}", Message);
        }

        private static IPEndPoint Parse(string endPoint)
        {
            if (string.IsNullOrEmpty(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            string[] parts = endPoint.Split(':');

            if (parts.Length != 2)
            {
                throw new FormatException("Invalid endpoint format. Expected 'IP:Port'.");
            }

            if (!IPAddress.TryParse(parts[0], out IPAddress? ip))
            {
                throw new FormatException("Invalid IP address.");
            }

            int port;
            if (!int.TryParse(parts[1], out port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new FormatException($"Invalid port. The port must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.");
            }

            return new IPEndPoint(ip, port);
        }
    }
}
