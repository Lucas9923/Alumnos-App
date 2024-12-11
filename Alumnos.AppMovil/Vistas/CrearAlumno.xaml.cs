using Alumnos.AppMovil.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Alumnos.AppMovil.Vistas;

public partial class CrearAlumno : ContentPage
{
	FirebaseClient client = new FirebaseClient("https://registrosestudiantes-e17c9-default-rtdb.firebaseio.com/");

	public List<Curso> Cursos { get; set; }
	public CrearAlumno()
	{
		InitializeComponent();
		ListarCursos();
		BindingContext = this;
	}

    private void ListarCursos()
    {
		var cursos = client.Child("Cursos").OnceAsync<Curso>();
		Cursos = cursos.Result.Select(x=>x.Object).ToList();
    }

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
		Curso curso = cursoPicker.SelectedItem as Curso;

		var alumno = new Alumno
		{
			PrimerNombre = primerNombreEntry.Text,
			SegundoNombre = segundoNombreEntry.Text,
			PrimerApellido = primerApellidoEntry.Text,
			SegundoApellido = segundoApellidoEntry.Text,
			CorreoElectronico = correoEntry.Text,
			Edad = edadEntry.Text,
			Curso = curso
		};

		try
		{
            await client.Child("Alumnos").PostAsync(alumno);
            await DisplayAlert("Éxito", $"El Alumno {alumno.PrimerNombre} {alumno.PrimerApellido} fue guardado correctamente", "OK");
            await Navigation.PopAsync();
        }
		catch (Exception ex)
		{
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}