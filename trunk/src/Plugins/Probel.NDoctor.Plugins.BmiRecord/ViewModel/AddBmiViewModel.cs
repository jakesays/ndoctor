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

namespace Probel.NDoctor.Plugins.BmiRecord.ViewModel
{
    using System;
    using System.Windows.Input;

    using Probel.Helpers.Assertion;
    using Probel.Helpers.WPF;
    using Probel.NDoctor.Domain.DTO.Components;
    using Probel.NDoctor.Domain.DTO.Objects;
    using Probel.NDoctor.Plugins.BmiRecord.Properties;
    using Probel.NDoctor.View.Core.ViewModel;
    using Probel.NDoctor.View.Plugins.Helpers;

    using StructureMap;

    public class AddBmiViewModel : BaseViewModel
    {
        #region Fields

        private IBmiComponent component = null;
        private BmiDto currentBmi;
        private bool isPopupOpened;

        #endregion Fields

        #region Constructors

        public AddBmiViewModel()
        {
            if (!Designer.IsDesignMode) ObjectFactory.GetInstance<IBmiComponent>();

            this.AddCommand = new RelayCommand(() => this.AddBmi(), () => this.CanAddBmi());
        }

        #endregion Constructors

        #region Properties

        public ICommand AddCommand
        {
            get;
            private set;
        }

        public BmiDto CurrentBmi
        {
            get { return this.currentBmi; }
            set
            {
                this.currentBmi = value;
                this.OnPropertyChanged("CurrentBmi");
            }
        }

        public bool IsPopupOpened
        {
            get { return this.isPopupOpened; }
            set
            {
                this.isPopupOpened = value;
                this.OnPropertyChanged("IsPopupOpened");
            }
        }

        #endregion Properties

        #region Methods

        private void AddBmi()
        {
            Assert.IsNotNull(this.Host, "The host shouldn't be null");
            Assert.IsNotNull(this.Host.SelectedPatient, "A patient should be selected if you want to manage data of a patient");
            Assert.IsNotNull(this.CurrentBmi, "The BMI to add shouldn't be null in order to add the item to the BMI history");

            using (this.component.UnitOfWork)
            {
                try
                {
                    this.component.AddBmi(this.CurrentBmi, this.Host.SelectedPatient);
                    this.Host.WriteStatus(StatusType.Info, Messages.Msg_BmiAdded);
                    this.CurrentBmi = new BmiDto();
                }
                catch (Exception ex)
                {
                    this.HandleError(ex, Messages.Msg_ErrAddBmi);
                }
            }
        }

        private bool CanAddBmi()
        {
            return this.CurrentBmi.Date <= DateTime.Now
                && this.CurrentBmi.Height > 0
                && this.CurrentBmi.Weight > 0;
        }

        #endregion Methods
    }
}