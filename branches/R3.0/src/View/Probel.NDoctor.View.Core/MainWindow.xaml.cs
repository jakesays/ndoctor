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
namespace Probel.NDoctor.View.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;

    using log4net;

    using Microsoft.Windows.Controls.Ribbon;

    using Probel.Helpers.Assertion;
    using Probel.Helpers.Conversions;
    using Probel.Helpers.Strings;
    using Probel.NDoctor.Domain.DTO.Helpers;
    using Probel.NDoctor.Domain.DTO.Objects;
    using Probel.NDoctor.View.Core.Properties;
    using Probel.NDoctor.View.Core.View;
    using Probel.NDoctor.View.Core.ViewModel;
    using Probel.NDoctor.View.Plugins;
    using Probel.NDoctor.View.Plugins.Exceptions;
    using Probel.NDoctor.View.Plugins.Helpers;
    using Probel.NDoctor.View.Plugins.MenuData;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, IPluginHost
    {
        #region Fields

        private LightUserDto connectedUser;
        private Page startpage = new StartPage();

        #endregion Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.Logger = LogManager.GetLogger(typeof(MainWindow));
            PluginContext.Host = this;
            this.DataContext = new MainWindowViewModel();
            this.WriteStatus(StatusType.Info, Messages.Msg_Ready);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation
        ///  history.
        /// </summary>
        /// <value>
        ///  <c>true</c> if there is at least one entry in back navigation history; otherwise, <c>false</c>.
        /// </value>
        public bool CanNavigateBack
        {
            get
            {
                return this.workbench.NavigationService.CanGoBack;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward
        /// navigation history.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is at least one entry in forward navigation history; otherwise,<c>false</c>.
        /// </value>
        public bool CanNavigateForward
        {
            get
            {
                return this.workbench.NavigationService.CanGoForward;
            }
        }

        /// <summary>
        /// Gets or sets the connecyed user.
        /// If set to null, it means no user are connected into nDoctor
        /// </summary>
        /// <value>
        /// The user.
        /// </value>        
        public LightUserDto ConnectedUser
        {
            get { return this.connectedUser; }
            set
            {
                this.connectedUser = value;
                this.RefreshDataContext(this.connectedUser);
            }
        }

        /// <summary>
        /// Gets the version of the host. It is used for the plugin validation.
        /// </summary>
        public Version HostVersion
        {
            get { return Assembly.GetCallingAssembly().GetName().Version; }
        }

        public LightPatientDto SelectedPatient
        {
            get
            {
                if (this.DataContext != null && this.DataContext is MainWindowViewModel)
                {
                    return (this.DataContext as MainWindowViewModel).SelectedPatient;
                }
                else { throw new WrongDataContextException(); }
            }
            set
            {
                if (this.DataContext != null && this.DataContext is MainWindowViewModel)
                {
                    (this.DataContext as MainWindowViewModel).SelectedPatient = value;
                }
                else { throw new WrongDataContextException(); }
            }
        }

        /// <summary>
        /// Gets the workday.
        /// </summary>
        public Workday Workday
        {
            get
            {
                var start = Settings.Default.WorkDayStart;
                var end = Settings.Default.WorkDayEnd;
                var duration = Settings.Default.SlotDuration;
                return new Workday(start, end, duration);
            }
        }

        private ILog Logger
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the specified tab.
        /// </summary>
        /// <param name="tab">The tab.</param>
        public void Add(RibbonTabData tab)
        {
            this.Dispatcher.Invoke((Action)delegate { App.RibbonData.TabDataCollection.Add(tab); });
        }

        /// <summary>
        /// Adds the specified context tab.
        /// </summary>
        /// <param name="contextTab">The context tab.</param>
        public void Add(RibbonContextualTabGroupData contextTab)
        {
            this.Dispatcher.Invoke((Action)delegate { App.RibbonData.ContextualTabGroupDataCollection.Add(contextTab); });
        }

        /// <summary>
        /// Adds the specified button in the specified group in home menu.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="group">The group.</param>
        public void AddInHome(RibbonButtonData button, Groups group)
        {
            string criteria = this.FindGroupName(group);

            var tab = (from menu in App.RibbonData.TabDataCollection
                       where menu.Header == Messages.Title_Home
                       select menu).FirstOrDefault();

            if (tab == null) throw new PluginException(Messages.PluginException_HomeMenuVoid);

            this.Dispatcher.Invoke((Action)delegate
            {
                switch (group)
                {
                    case Groups.Managers:
                        this.AddButton(tab, Messages.Title_SessionManagers, button);
                        break;
                    case Groups.Tools:
                        this.AddButton(tab, Messages.Title_Tools, button);
                        break;
                    case Groups.GlobalTools:
                        this.AddButton(tab, Messages.Title_GlobalTools, button);
                        break;
                    default:
                        Assert.FailOnEnumeration(group);
                        break;
                }
            });
        }

        /// <summary>
        /// Adds the specified control into the application menu.
        /// </summary>
        /// <param name="control">The control.</param>
        public void AddToApplicationMenu(RibbonControlData control)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                App.RibbonData.ApplicationMenuData.ControlDataCollection.Add(control);
                App.RibbonData.ApplicationMenuData.ControlDataCollection.Refill(
                    App.RibbonData.ApplicationMenuData.ControlDataCollection.OrderBy(e => e.Order).ToList());
            });
        }

        /// <summary>
        /// Deactivates the menu.
        /// </summary>
        public void HideMainMenu()
        {
            this.Dispatcher.Invoke((Action)delegate { this.ribbon.Visibility = System.Windows.Visibility.Collapsed; });
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread
        /// the System.Windows.Threading.Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public void Invoke(Action action)
        {
            this.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// Navigates to specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Navigate(Page page)
        {
            Assert.IsNotNull(page, "The page where to navigate shouldn't be null");

            foreach (var context in App.RibbonData.ContextualTabGroupDataCollection)
            { context.IsVisible = false; }

            this.Dispatcher.Invoke((Action)delegate
            {
                this.workbench.NavigationService.Navigate(page);
                this.WriteStatus(StatusType.Info, Messages.Msg_Ready);
            });
        }

        /// <summary>
        /// Navigates to last page if there's one.
        /// </summary>
        /// <param name="page">The page.</param>
        public void NavigateBack()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.workbench.NavigationService.GoBack(); ;
            });
        }

        /// <summary>
        /// Navigates to previous page if there's one.
        /// </summary>
        /// <param name="page">The page.</param>
        public void NavigateForward()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.workbench.NavigationService.GoForward(); ;
                this.WriteStatus(StatusType.Info, Messages.Msg_Ready);
            });
        }

        /// <summary>
        /// Navigates to start page.
        /// </summary>
        public void NavigateToStartPage()
        {
            bool isEmpty = true;
            this.Dispatcher.Invoke((Action)delegate { isEmpty = (this.startpage.DataContext == null); });

            this.Navigate(startpage);
        }

        /// <summary>
        /// Activates the menu.
        /// </summary>
        public void ShowMainMenu()
        {
            this.Dispatcher.Invoke((Action)delegate { this.ribbon.Visibility = System.Windows.Visibility.Visible; });
        }

        /// <summary>
        /// Writes a message in the StatusBar.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public void WriteStatus(StatusType type, string message)
        {
            var context = this.DataContext as MainWindowViewModel;
            if (context != null)
            {
                context.Message = message;
                context.Type = type;
            }
        }

        /// <summary>
        /// Write the status "Ready." in the status box
        /// </summary>
        public void WriteStatusReady()
        {
            this.WriteStatus(StatusType.Info, Messages.Msg_Ready);
        }

        private void AddButton(RibbonTabData tab, string goupName, RibbonButtonData button)
        {
            var group = (from g in tab.GroupDataCollection
                         where g.Header == goupName
                         select g).FirstOrDefault();

            if (group == null) throw new PluginException(Messages.PluginException_HomeMenuVoid);

            group.ButtonDataCollection.Add(button);
            // I use ToList() because it makes a copy of the list. Otherwise, clearing the ObservableCollection
            // clears the linq result.
            group.ButtonDataCollection.Refill(group.ButtonDataCollection.OrderBy(e => e.Order).ToList());
        }

        private string FindGroupName(Groups group)
        {
            string criteria = string.Empty;
            switch (group)
            {
                case Groups.Managers:
                    criteria = Messages.Title_SessionManagers;
                    break;
                case Groups.Tools:
                    criteria = Messages.Title_Tools;
                    break;
                case Groups.GlobalTools:
                    criteria = Messages.Title_GlobalTools;
                    break;
                default:
                    Assert.FailOnEnumeration(group);
                    break;
            }
            return criteria;
        }

        private void RefreshDataContext(LightUserDto lightUserDto)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (this.DataContext != null && this.DataContext is MainWindowViewModel)
                {
                    (this.DataContext as MainWindowViewModel).ConnectedUser = lightUserDto;
                }
                else { throw new WrongDataContextException(); }
            });
        }

        private void WriteStatus(LightPatientDto value)
        {
            var name = string.Format("{0} {1}", value.FirstName, value.LastName);
            this.WriteStatus(StatusType.Info, Messages.Msg_SelectPatient.StringFormat(name));
        }

        #endregion Methods
    }
}