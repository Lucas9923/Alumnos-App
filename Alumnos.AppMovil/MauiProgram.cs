using Alumnos.AppMovil.Modelos;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;

namespace Alumnos.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            ActualizarCurso();
            ActualizarAlumno();
            return builder.Build();
        }
        public static async Task ActualizarCurso()
        {
            FirebaseClient client = new FirebaseClient("https://alumnos-1fb03-default-rtdb.firebaseio.com/");

            var cursos = await client.Child("Cursos").OnceAsync<Curso>();

            if (cursos.Count == 0)
            {
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1° Basico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2° Basico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3° Basico" });
            }
            else
            {
                foreach (var curso in cursos)
                {
                    if (curso.Object.Estado == null)
                    {
                        var cursoActualizado = curso.Object;
                        cursoActualizado.Estado = true;

                        await client.Child("Cursos").Child(curso.Key).PutAsync(cursoActualizado);
                    }
                }
            }
        }

        public static async Task ActualizarAlumno()
        {
            FirebaseClient client = new FirebaseClient("https://alumnos-1fb03-default-rtdb.firebaseio.com/");

            var alumnos = await client.Child("Alumnos").OnceAsync<Alumno>();

            foreach (var alumno in alumnos)
            {
                if (alumno.Object.Estado == null)
                {
                    var alumnoActualizado = alumno.Object;
                    alumnoActualizado.Estado = true;

                    await client.Child("Alumnos").Child(alumno.Key).PutAsync(alumnoActualizado);
                }
            }
        }
    }
}
