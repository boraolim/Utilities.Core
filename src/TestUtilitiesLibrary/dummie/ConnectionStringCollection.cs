using System;
using System.Collections.Generic;
using System.Text;

namespace TestUtilitiesLibrary
{
    /// <summary>
    /// Clase 'ConnectionStringCollection'.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Interfaz 'ConnectionStringCollection'.
    /// </summary>
    /// <summary xml:lang="en">
    /// 'ConnectionStringCollection' class.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// 'ConnectionStringCollection' class.
    /// </summary>
    [Serializable]
    public class ConnectionStringCollection
    {
        /// <summary>
        /// Atributo 'ConnectionDbFirst'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'ConnectionDbFirst'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'ConnectionDbFirst' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'ConnectionDbFirst' attribute.
        /// </summary>
        public string ConnectionDbFirst { get; set; }

        /// <summary>
        /// Atributo 'ConnectionDbSecond'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'ConnectionDbSecond'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'ConnectionDbSecond' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'ConnectionDbSecond' attribute.
        /// </summary>
        public string ConnectionDbSecond { get; set; }
    }
}
