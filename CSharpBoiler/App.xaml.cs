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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            int steamID = StartBoiler.StartAndGetSteamID();

            if (steamID == 0)
            {
                MessageBox.Show("Steam must be running for CSharpBoiler to work.\n Open Steam and close this MessageBox afterwards.");
                steamID = StartBoiler.StartAndGetSteamID();
            }

            if (steamID == 0)
            {
                MessageBox.Show("Still no running SteamClient found. Closing.");
                Environment.Exit(1);
            }

            if (steamID != 0)
            {
                var mainWindow = new MainWindow(steamID);
                mainWindow.Closed += MainWindow_Closed;
                mainWindow.Show();
            }
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
    }
}
