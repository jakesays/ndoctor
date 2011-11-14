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

namespace Probel.NDoctor.Plugins.PatientSession.ViewModel
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using AutoMapper;

    using Probel.NDoctor.Domain.DTO.Components;
    using Probel.NDoctor.Domain.DTO.Objects;
    using Probel.NDoctor.Plugins.PatientSession.Properties;
    using Probel.NDoctor.View.Core.Helpers;
    using Probel.NDoctor.View.Plugins.Helpers;

    using StructureMap;

    public class LightPatientViewModel : LightPatientDto
    {
        #region Fields

        private IPatientSessionComponent component;
        private bool isSelected;

        #endregion Fields

        #region Constructors

        public LightPatientViewModel()
        {
            this.SelectPatientCommand = new RelayCommand(() =>
            {
                PluginContext.Host.SelectedPatient = this;
                this.IncrementCounter();

                ChildWindowContext.CloseWindow();
            });
            this.component = ObjectFactory.GetInstance<IPatientSessionComponent>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the context menu select.
        /// </summary>
        public string ContextMenuSelect
        {
            get { return Messages.Menu_Select; }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        public ICommand SelectPatientCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        private void IncrementCounter()
        {
            using (this.component.UnitOfWork) { this.component.IncrementPatientCounter(PluginContext.Host.SelectedPatient); }
        }

        #endregion Methods
    }
}