using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTOFINAL
{
    internal class Proveedores
    {
    }
}
namespace EJERCICIO_EXPO
{
    // Hecho por Mathyas: Clase independiente para gestionar de forma densa y segura los proveedores de la bodega
    public class Proveedores
    {
        static string archivo = "proveedores.txt";

        // Hecho por Mathyas: Verifica que exista el txt para que el sistema no falle al leer
        public static void VERIFICAR()
        {
            try
            {
                if (!File.Exists(archivo))
                {
                    using (StreamWriter sw = File.CreateText(archivo)) { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al intentar verificar el archivo: " + ex.Message);
            }
        }

        // Hecho por Mathyas: Algoritmo para autogenerar IDs correlativos leyendo el txt
        public static int ObtenerID()
        {
            VERIFICAR();
            int maxId = 0;
            try
            {
                string[] lineas = File.ReadAllLines(archivo);
                foreach (string l in lineas)
                {
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    string[] datos = l.Split('|');
                    if (datos.Length > 0 && int.TryParse(datos[0].Trim(), out int id))
                    {
                        if (id > maxId) maxId = id;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Hubo un problema al leer los IDs: " + ex.Message);
            }
            return maxId + 1;
        }

        // Hecho por Mathyas: Bucle interactivo para registrar múltiples proveedores con validación de RUC de 11 dígitos
        public static void AGREGAR()
        {
            VERIFICAR();
            bool seguirRegistrando = true;
            while (seguirRegistrando)
            {
                Console.WriteLine("\n--- Formulario de Registro de Proveedor ---");
                int id = ObtenerID();
                Console.WriteLine("ID asignado por el sistema: " + id);

                string nombre = "";
                while (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.Write("Ingrese el nombre del proveedor: ");
                    nombre = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        Console.WriteLine("El nombre es obligatorio.");
                    }
                }

                if (nombre.ToUpper() == "SALIR") return;

                string ruc = "";
                while (true)
                {
                    Console.Write("Ingrese el número de RUC (11 dígitos): ");
                    ruc = Console.ReadLine()?.Trim();
                    if (ruc.ToUpper() == "SALIR") return;
                    if (ruc.Length == 11 && long.TryParse(ruc, out _)) break;
                    Console.WriteLine("RUC inválido. Deben ser 11 números.");
                }

                try
                {
                    string lineaGuardar = id + " | " + nombre + " | " + ruc;
                    File.AppendAllText(archivo, lineaGuardar + Environment.NewLine);
                    Console.WriteLine("El proveedor se guardó correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo escribir en el archivo: " + ex.Message);
                }

                Console.Write("\n¿Quiere registrar otro proveedor? (S/N): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper();
                if (respuesta != "S") seguirRegistrando = false;
            }
        }

        // Hecho por Mathyas: Busca proveedores por ID o por nombre ignorando mayúsculas y actualiza sus datos en el txt
        public static void MODIFICAR()
        {
            VERIFICAR();
            try
            {
                string[] lineas = File.ReadAllLines(archivo);
                if (lineas.Length == 0 || (lineas.Length == 1 && lineas[0] == ""))
                {
                    Console.WriteLine("No hay proveedores registrados para modificar.");
                    return;
                }

                Console.Write("\nIngrese el ID o el nombre del proveedor a modificar: ");
                string buscar = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(buscar) || buscar.ToUpper() == "SALIR") return;

                bool esId = int.TryParse(buscar, out int idBuscar);
                bool cambiosRealizados = false;

                for (int i = 0; i < lineas.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length < 3) continue;

                    int idActual = int.Parse(datos[0].Trim());
                    string nombreActual = datos[1].Trim();

                    bool encontrado = false;
                    if (esId && idActual == idBuscar) encontrado = true;
                    if (!esId && nombreActual.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) encontrado = true;

                    if (encontrado)
                    {
                        Console.WriteLine("\nProveedor encontrado: " + lineas[i]);
                        Console.Write("Nuevo nombre (ENTER para mantener): ");
                        string nuevoNombre = Console.ReadLine()?.Trim();
                        Console.Write("Nuevo RUC (ENTER para mantener): ");
                        string nuevoRuc = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrWhiteSpace(nuevoNombre)) datos[1] = nuevoNombre;
                        if (!string.IsNullOrWhiteSpace(nuevoRuc))
                        {
                            if (nuevoRuc.Length == 11 && long.TryParse(nuevoRuc, out _)) datos[2] = nuevoRuc;
                            else Console.WriteLine("RUC inválido. Se mantiene el original.");
                        }

                        lineas[i] = string.Join(" | ", datos);
                        cambiosRealizados = true;
                        break;
                    }
                }

                if (cambiosRealizados)
                {
                    File.WriteAllLines(archivo, lineas);
                    Console.WriteLine("Los cambios se guardaron correctamente.");
                }
                else
                {
                    Console.WriteLine("No se encontró ningún proveedor.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar: " + ex.Message);
            }
        }

        // Hecho por Mathyas: Menú principal que controla las acciones de los proveedores
        public static void MENUPROV()
        {
            string opcion = "";
            do
            {
                Console.WriteLine("\n=================================");
                Console.WriteLine("      MÓDULO DE PROVEEDORES      ");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Registrar nuevos proveedores");
                Console.WriteLine("2. Modificar datos de un proveedor");
                Console.WriteLine("3. Volver al menú de la bodega");
                Console.Write("Seleccione una opción: ");
                opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1": AGREGAR(); break;
                    case "2": MODIFICAR(); break;
                    case "3": Console.WriteLine("Regresando..."); break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            } while (opcion != "3");
        }
    }
}