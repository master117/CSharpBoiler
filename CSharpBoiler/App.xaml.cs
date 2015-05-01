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

            bool steamAlreadyRunning = true;
            while (true)
            {
                if (Process.GetProcessesByName("Steam").Length != 0)
                    break;

                steamAlreadyRunning = false;
                Thread.Sleep(5000);
            }

            //Give Steam enough Time to Start
            if(!steamAlreadyRunning)             
                Thread.Sleep(5000);

            long accountId = CSharpBoiler.Properties.Settings.Default.LastAccountId;
            long tempAccountId = 0; //BoilerHandler.StartAndGetAccountId();
            if (tempAccountId != 0)
            {
                accountId = tempAccountId;
                CSharpBoiler.Properties.Settings.Default.LastAccountId = tempAccountId;
            }

            if (accountId != 0)
            {
                var mainWindow = new MainWindow(accountId);
                mainWindow.Closed += MainWindow_Closed;
                mainWindow.ShowDialog();          
            }

            Environment.Exit(0);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
    }
}
