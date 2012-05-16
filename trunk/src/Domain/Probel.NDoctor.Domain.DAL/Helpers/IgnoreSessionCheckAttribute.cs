﻿/*
    This file is part of NDoctor.

    NDoctor is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    NDoctor is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with NDoctor.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace Probel.NDoctor.Domain.DAL.Helpers
{
    using System;

    /// <summary>
    /// Every method decorated with this attribute will be ignored by the
    /// inspectors of the Castle Dynamic Proxy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreAttribute"/> class.
        /// </summary>
        public IgnoreAttribute()
        {
            this.Ignore = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Returns always true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ignore; otherwise, <c>false</c>.
        /// </value>
        public bool Ignore
        {
            get;
            private set;
        }

        #endregion Properties
    }
}