using System.Collections.ObjectModel;
using Alumnos.AppMovil.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Alumnos.AppMovil.Vistas;

public partial class ListarAlumnos : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://alumnos-1fb03-default-rtdb.firebaseio.com/");
    public ObservableCollection<Alumno> Lista {  get; set; } = new ObservableCollection<Alumno>();
	public ListarAlumnos()
	{
		InitializeComponent();
        BindingContext = this;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarLista();
    }

    private async void CargarLista()
    {
        Lista.Clear();
        var alumnos = await client.Child("Alumnos").OnceAsync<Alumno>();

        var alumnosActivos = alumnos.Where(e => e.Object.Estado == true).ToList();

        foreach (var alumno in alumnosActivos)
        {
            Lista.Add(new Alumno
            {
                Id = alumno.Key,
                PrimerNombre = alumno.Object.PrimerNombre,
                SegundoNombre = alumno.Object.SegundoNombre,
                PrimerApellido = alumno.Object.PrimerApellido,
                SegundoApellido = alumno.Object.SegundoApellido,
                CorreoElectronico = alumno.Object.CorreoElectronico,
                Edad = alumno.Object.Edad,
                Estado = alumno.Object.Estado,
                Curso = alumno.Object.Curso
            });
        }
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

    private async void EditarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno != null &&!string.IsNullOrEmpty(alumno.Id))
        {
            await Navigation.PushAsync(new EditarAlumno(alumno.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo cargar el alumno", "OK");
        }
    }

    private async void deshabilitarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno == null)
        {
            await DisplayAlert("Error", "No se pudo cargar el alumno", "OK");
            return;
        }

        bool confirmacion = await DisplayAlert
            ("Confirmación", $"¿Está seguro de deshabilitar al alumno {alumno.NombreCompleto}?", "Si", "No");

        if (confirmacion)
        {
            try
            {
                alumno.Estado = false;
                await client.Child("Alumnos").Child(alumno.Id).PutAsync(alumno);
                await DisplayAlert("Éxito", $"El alumno {alumno.NombreCompleto} fue deshabilitado correctamente", "OK");
                CargarLista();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}