﻿#region Header

/*
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

#endregion Header

namespace Probel.NDoctor.Domain.Test.Components
{
    using NUnit.Framework;

    using Probel.NDoctor.Domain.DAL.Components;
    using Probel.NDoctor.Domain.DTO.Exceptions;
    using Probel.NDoctor.Domain.DTO.Objects;

    [TestFixture]
    public class AuthorisationComponentTest : BaseComponentTest<AuthorisationComponent>
    {
        #region Methods

        /// <summary>
        /// Issue 93
        /// </summary>
        [Test]
        public void ManageAuthorisation_CreateNewRole_RoleCreated()
        {
            var name = this.RandomString;
            this.WrapInTransaction(() =>
            {
                var role = new RoleDto()
                {
                    Description = this.RandomString,
                    Name = name,
                };
                this.ComponentUnderTest.Create(role);

                var found = this.HelperComponent.GetRoleByName(name);

                Assert.NotNull(found);
                Assert.AreEqual(1, found.Count);
                Assert.AreEqual(name, found[0].Name);
            });
        }

        /// <summary>
        /// Issue 94
        /// </summary>
        [Test]
        [ExpectedException(typeof(NullItemInListException))]
        public void ManageAuthorisation_UpdateARoleWithAnEmptyTask_NullItemInListExceptionIsThrown()
        {
            var role = new RoleDto()
            {
                Description = this.RandomString,
                Name = this.RandomString,
            };
            this.ComponentUnderTest.Create(role);
            this.Session.Flush();

            role.Tasks.Add(new TaskDto(this.RandomString));
            role.Tasks.Add(new TaskDto(this.RandomString));
            role.Tasks.Add(null);

            this.ComponentUnderTest.Update(role);
        }

        protected override void _Setup()
        {
            this.BuildComponent(session => new AuthorisationComponent(session));
        }

        #endregion Methods
    }
}