using Alumnos.AppMovil.Vistas;

namespace Alumnos.AppMovil
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ListarAlumnos());
        }
    }
}
