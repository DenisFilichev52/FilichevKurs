using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FilichevKurs.Model;

namespace FilichevKurs
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FilichevKursEntities context = new FilichevKursEntities();
        public static Model.Clients curentUser = new Model.Clients();
        public static FilichevKursEntities DbContext { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DbContext = new FilichevKursEntities();
        }
    }
}
