/*
Boiler
Copyright (C) 2015  Johannes Gocke

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using CSharpBoiler.Helpers;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //Retrieve AccountId
            //Case1: CSharpBoiler was started while Steam is running
            if (Process.GetProcessesByName("Steam").Length != 0)
            {
                long tempAccountId = BoilerHandler.StartAndGetAccountId();
                if (tempAccountId != 0)
                    CSharpBoiler.Properties.Settings.Default.LastAccountId = tempAccountId;
            }
            //Case2: CSharpboiler was started with Windows or while Steam wasn't running, BUT the accountid from the last Session is available
            //If Case 1 or 2 apply, start MainWindow
            if (CSharpBoiler.Properties.Settings.Default.LastAccountId != 0)
            {
                var mainWindow = new MainWindow(CSharpBoiler.Properties.Settings.Default.LastAccountId);
                //Note: ShowDialog is Blocking
                mainWindow.ShowDialog();  
            }
            else
            {
                //Neither Case 1 nor 2 were reached, meaning the App is openend for the first time AND Steam isn't running. 
                //Informing the user that Steam is Required
                MessageBox.Show(
                    "It appears that you run CSharpBoiler for the first time." +
                    "\n\r Steam is required to be running at the first start of CSharpBoiler." +
                    "\n\r\n\r The matchlist is updated:" +
                    "\n\r - when starting CSharpBoiler while Steam is running " +
                    "\n\r - when closing CSGO." +
                    "\n\r\n\r - Closing CSharpBoiler. -"
                    );
            }

            //If we received no accountId or MainWindow.ShowDialog was terminated we end the App
            Environment.Exit(0);
        }        
    }
}
