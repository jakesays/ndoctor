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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Probel.NDoctor.Domain.DTO.Objects;

namespace Probel.NDoctor.Domain.DTO.Specification
{    /// <summary>
    /// Verifies if the specified <see cref="LightPatientDto"/> has a reason that contains the specified text
    /// </summary>
    public class GetPatientByReasonSpecification : Specification<LightPatientDto>
    {
        #region Fields

        private readonly string Criteria;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPatientByCitySpecification"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public GetPatientByReasonSpecification(string criteria)
        {
            this.Criteria = criteria.ToUpper();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether [is satisfied by] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///   <c>true</c> if [is satisfied by] [the specified obj]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsSatisfiedBy(LightPatientDto obj)
        {
            return obj.Reason.ToUpper().Contains(this.Criteria);
        }

        #endregion Methods
    }
}
