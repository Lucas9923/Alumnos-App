using System.Collections.ObjectModel;
using Alumnos.AppMovil.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Alumnos.AppMovil.Vistas;

public partial class EditarAlumno : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://alumnos-1fb03-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; }
    public ObservableCollection<string> ListaCursos { get; set; } = new ObservableCollection<string>();
    private Alumno alumnoActualizado = new Alumno();
    private string alumnoId;
    public EditarAlumno(string idAlumno)
    {
        InitializeComponent();
        BindingContext = this;
        alumnoId = idAlumno;
        CargarAlumno(alumnoId);
        CargarListaCursos();
    }

    private async void CargarListaCursos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListaCursos.Clear();
            foreach (var curso in cursos)
            {
                ListaCursos.Add(curso.Object.Nombre);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error:" + ex.Message, "OK");
        }
    }

    private async void CargarAlumno(string idAlumno)
    {
        var alumno = await client.Child("Alumnos").Child(idAlumno).OnceSingleAsync<Alumno>();

        if (alumno != null)
        {
            EditPrimerNombreEntry.Text = alumno.PrimerNombre;
            EditSegundoNombreEntry.Text = alumno.SegundoNombre;
            EditPrimerApellidoEntry.Text = alumno.PrimerApellido;
            EditSegundoApellidoEntry.Text = alumno.SegundoApellido;
            EditCorreoEntry.Text = alumno.CorreoElectronico;
            EditEdadEntry.Text = alumno.Edad;
            EditCursoPicker.SelectedItem = alumno.Curso?.Nombre;
        }
    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditEdadEntry.Text) ||
                EditCursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son requeridos", "OK");
                return;
            }

            if (!EditCorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "Correo Electrónico inválido", "OK");
                return;
            }

            alumnoActualizado.Id = alumnoId;
            alumnoActualizado.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            alumnoActualizado.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            alumnoActualizado.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            alumnoActualizado.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            alumnoActualizado.CorreoElectronico = EditCorreoEntry.Text.Trim();
            alumnoActualizado.Edad = EditEdadEntry.Text.Trim();
            alumnoActualizado.Estado = estadoSwitch.IsToggled;
            alumnoActualizado.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };

            await client.Child("Alumnos").Child(alumnoActualizado.Id).PutAsync(alumnoActualizado);

            await DisplayAlert("Éxito", $"El Alumno fue actualizado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
    }
}