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
namespace Probel.NDoctor.Domain.Test.Authorisation
{
    using System.Linq;

    using NUnit.Framework;

    using Probel.NDoctor.Domain.DTO;

    [TestFixture]
    [Category("Authorisation")]
    public class TestAuthorisation
    {
        #region Methods

        [Test]
        public void CanListRoles()
        {
            string[] roles = To.ToStringArray();

            Assert.AreEqual(6, roles.Length);

            Assert.IsTrue(roles.Contains(To.Everyone));
            Assert.IsTrue(roles.Contains(To.MetaWrite));
            Assert.IsTrue(roles.Contains(To.Administer));
            Assert.IsTrue(roles.Contains(To.Read));
            Assert.IsTrue(roles.Contains(To.Write));
            Assert.IsTrue(roles.Contains(To.EditCalendar));
        }

        #endregion Methods
    }
}