using System.Collections.ObjectModel;
using Alumnos.AppMovil.Modelos;
using Firebase.Database;

namespace Alumnos.AppMovil.Vistas;

public partial class ListarAlumnos : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registrosestudiantes-e17c9-default-rtdb.firebaseio.com/");
    public ObservableCollection<Alumno> Lista {  get; set; } = new ObservableCollection<Alumno>();
	public ListarAlumnos()
	{
		InitializeComponent();
        BindingContext = this;
        CargarLista();
	}

    private void CargarLista()
    {
        client.Child("Alumnos").AsObservable<Alumno>().Subscribe((alumno) =>
        {
            if (alumno != null)
            {
                Lista.Add(alumno.Object);
            }
        });
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if(filtro.Length > 0)
        {
            listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoAlumnoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearAlumno());
    }
}