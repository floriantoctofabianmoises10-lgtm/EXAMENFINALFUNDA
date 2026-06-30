using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace PROYECTOFINAL
{
    internal class Program
    {
        //VARIABLES GLOBALES
        static List<string> productosGlobal = new List<string>();
        static int totalVentas = 0;
        static double gananciasDia = 0;
        static double totalProductosVendidos = 0;
        static Dictionary<string, double> productosVendidos = new Dictionary<string, double>();
        //LECTURA DEL ARCHIVO
        static string[] LEERARCHIVO()
        {
            return File.ReadAllLines("BASEDATOS.txt");
        }
        // NORMALIZACION
        public static string QUITARTILDES(string texto)
        {
            string normalizado = texto.Normalize(NormalizationForm.FormD);
            StringBuilder resultado = new StringBuilder();
            foreach (char letra in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra)
                    != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(letra);
                }
            }
            return resultado.ToString().Normalize(NormalizationForm.FormC);
        }
        //SALIR
        public static bool SALIR(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto) &&
          texto.Trim().ToUpper() == "SALIR";
        }

        //SESION
        public static void REINICIODATOS()
        {
            //REINICIAR DATOS A CERO
        }
        public static void CARGARSESION()
        {
            //CARGAR SESIÓN
            //CREANDO ARCHIVOS TXT
        }
        public static void CARGARVENTASDIA()
        {

        }

        //VENTAS
        public static void REGISTROVENTAS()
        {
            string[] linea = LEERARCHIVO();
            //VARIABLES DE CONTROL 
            string IMPUT2;
            bool buscarOtro = false;
            //VARIABLES NORMALES
            (bool exito, double importe, string historial) RESULT = default;
            double SUBTOTAL = 0;
            string historialTotal = "";
            do
            {
                //VARIABLE TEMPORAL
                int contador = 1;
                bool BUSCADOR = false;
                Console.WriteLine("========== REGISTRAR VENTA ==========");
                Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");
                Console.WriteLine("INGRESE EL NOMBRE DEL PRODUCTO: ");
                string nomProd = Console.ReadLine();
                if (SALIR(nomProd))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
                //Lista de posiciones de productos
                List<int> posiciones = new List<int>();
                for (int x = 0; x < linea.Length; x++)
                {
                    if (linea[x].StartsWith("====") || linea[x].StartsWith("ID") || linea[x].StartsWith("---"))
                    {
                        continue;
                    }
                    // Cada linea en un arreglo
                    string[] DATO = linea[x].Split('|');
                    //Verificamos que la linea tiene 7 arreglos
                    if (DATO.Length < 7)
                    {
                        continue;
                    }
                    //Limpiamos los espacios
                    for (int i = 0; i < DATO.Length; i++)
                    {
                        DATO[i] = DATO[i].Trim();
                    }
                    //Normalizamos nombre
                    if (QUITARTILDES(DATO[1]).ToUpper().Contains(QUITARTILDES(nomProd).ToUpper()))
                    {
                        BUSCADOR = true;
                        posiciones.Add(x);
                        Console.WriteLine(contador + ". " + DATO[1] + "|" + DATO[2] + "|" + DATO[3] + "|" + DATO[5] + "|" + DATO[6]);
                        contador++;
                    }
                }
                if (BUSCADOR)
                {
                    //VARIABLES DE CONTROL 
                    bool ACEPPROD = false;
                    //VARIABLES 
                    int INDICE;
                    string[] Dato = null;
                    while (!ACEPPROD)
                    {
                        int Opc;
                        int Opc2;
                        do
                        {
                            Console.WriteLine("========================");
                            Console.Write("SELECCIONE EL PRODUCTO: ");
                            string SELEC = Console.ReadLine();
                            if (SALIR(SELEC))
                            {
                                Console.WriteLine("Operación cancelada.");
                                return;
                            }
                            if (!int.TryParse(SELEC, out Opc) || Opc < 1 || Opc > posiciones.Count)
                            {
                                Console.WriteLine("Entrada no valida, Ingrese un número de la lista");
                            }
                        } while (Opc < 1 || Opc > posiciones.Count);
                        //RELACION INDICE - ENTRADA
                        INDICE = posiciones[Opc - 1];
                        //SEPARACION DE LINEA EN ARREGLOS
                        Dato = linea[INDICE].Split('|');
                        for (int i = 0; i < Dato.Length; i++)
                        {
                            Dato[i] = Dato[i].Trim();
                        }
                        //MOSTRANDO PRODUCTO
                        Console.WriteLine("=========================");
                        Console.WriteLine("  PRODUCTO SELECCIONADO  ");
                        Console.WriteLine(Dato[1] + " | " + Dato[2] + " | " + Dato[3] + " | " + Dato[5] + " | " + Dato[6]);
                        Console.WriteLine("=========================");
                        //MENU DE CONFIRMACION
                        Console.WriteLine("1. Continuar");
                        Console.WriteLine("2. Elegir otro del listado");
                        Console.WriteLine("3. Buscar otro producto");
                        do
                        {
                            Console.Write("SELECIONE UN OPCION: ");
                            string SELECT2 = Console.ReadLine();

                            if (SELECT2.ToUpper() == "SALIR")
                                return;

                            if (!int.TryParse(SELECT2, out Opc2) || Opc2 < 1 || Opc2 > 3)
                            {
                                Console.WriteLine("Entrada Invalida, Ingrese un número del rango (1-3)");
                                continue;
                            }
                        } while (Opc2 < 1 || Opc2 > 3);
                        //ACCION SEGUN EL NUMERO DE ELECCION
                        switch (Opc2)
                        {
                            case 1:
                                ACEPPROD = true;
                                break;
                            case 2:
                                //VARIABLE DE CORRELATIVO
                                int COR = 1;
                                Console.WriteLine("========================");
                                Console.WriteLine("    LISTA DE PRODUCTO   ");
                                Console.WriteLine("========================");
                                //VUELVO A RECORRER LAS POSICIONES GUARDADAS
                                foreach (int D in posiciones)
                                {
                                    string[] Lista = linea[D].Split('|');
                                    for (int i = 0; i < Lista.Length; i++)
                                    {
                                        Lista[i] = Lista[i].Trim();
                                    }
                                    Console.WriteLine(COR + " | " + Lista[1] + " | " + Lista[2] + " | " + Lista[3] + " | " + Lista[5] + " | " + Lista[6]);
                                    COR++;
                                }
                                Console.WriteLine("========================");
                                break;
                            case 3:
                                buscarOtro = true;
                                ACEPPROD = true;
                                break;
                        }
                        //PIDO LA CANTIDAD
                        //VARIABLE DE RETORNO
                        double CANTIDAD = CANTIDADVENTA(Dato, ref linea, INDICE);
                        if (CANTIDAD == -1)
                        {
                            return;
                        }
                        //CALCULO EL STOCK
                        //VARIABLE DE RETORNO Y CÁLCULO
                        RESULT = CONTROL_STOCK(Dato, ref linea, INDICE, CANTIDAD);

                        if (RESULT.exito)
                        {
                            SUBTOTAL += RESULT.importe;
                            historialTotal += RESULT.historial;
                        }
                    }
                }

                if (!BUSCADOR)
                {
                    Console.WriteLine("Producto no encontrado....");
                }
                //VARIABLE DE CONFIRMACION
                Console.Write("DESEA INGRESAR OTRO PRODUCTO: S/N");
                IMPUT2 = Console.ReadLine().ToUpper().Trim(); ;
                if (SALIR(IMPUT2))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
                while (string.IsNullOrWhiteSpace(IMPUT2) || !(IMPUT2 == "SI" || IMPUT2 == "S" || IMPUT2 == "NO" || IMPUT2 == "N"))
                {
                    Console.WriteLine("Ingrese Si o No:");
                    IMPUT2 = Console.ReadLine().ToUpper();
                    if (SALIR(IMPUT2))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }
                }
                if (buscarOtro)
                {
                    continue;
                }

            } while (IMPUT2 == "SI" || IMPUT2 == "S");
            if (SUBTOTAL <= 0)
            {
                Console.WriteLine("No se registró ninguna venta.");
                return;
            }
            Console.WriteLine("=============================");
            Console.Write("  ¿CONFIRMA LA COMPRA? (S/N): ");
            string CF = Console.ReadLine().ToUpper().Trim();
            if (SALIR(CF))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            while (string.IsNullOrWhiteSpace(CF) || !(CF == "SI" || CF == "S" || CF == "NO" || CF == "N"))
            {
                Console.WriteLine("Ingrese Si o no...");
                CF = Console.ReadLine().ToUpper().Trim();
                if (SALIR(CF))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }
            if (CF == "NO" || CF == "N")
            {
                Console.WriteLine("Venta cancelada.");
                return;
            }
            METODOPAGO(SUBTOTAL, historialTotal);
            File.WriteAllLines("BASEDATOS.txt", linea);
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("=============================");


        }
        public static double CANTIDADVENTA(string[] Dato, ref string[] linea, int indice)
        {
            //PASO POR REFERENCIA A DATO PARA LA SEPARACION DE LINEAS EN ARREGLOS
            //PASO POR REFERENCIA LINEA PARA LA LECTURA DEL ARCHIVO
            //PASO POR REFERENCIA INDICE PARA LAS POSICIONES DONDE ESTÁ LOS PRODUCTOS

            //VARIABLES NORMALES
            double CANTS = 0;
            int CANTSE;
            //VARIABLES DE CONTROL
            bool valido = true;
            do
            {
                Console.WriteLine("==================");
                Console.Write("INGRESE LA CANTIDAD: ");
                string IMPUT = Console.ReadLine();
                if (SALIR(IMPUT))
                {
                    Console.WriteLine("Operación cancelada.");
                    return -1;
                }
                if (Dato[5].ToUpper().Contains("KG"))
                {
                    valido = double.TryParse(IMPUT, NumberStyles.Any, new CultureInfo("es-PE"), out CANTS);
                    if (!valido || CANTS <= 0)
                    {
                        Console.WriteLine("Cantidad invalida....");
                    }
                }
                else
                {
                    valido = int.TryParse(IMPUT, out CANTSE);
                    CANTS = CANTSE;
                    if (!valido || CANTSE <= 0)
                    {
                        Console.WriteLine("Cantidad invalida...Ingrese valores enteros. ");
                    }
                }
            } while (!valido || CANTS <= 0);
            return CANTS;
        }
        public static (bool exito, double importe, string historial) CONTROL_STOCK(string[] Dato, ref string[] linea, int indice, double Cantidad)
        {
            //VARIABLE NORMALES
            double STOCK = double.Parse(Dato[4].Replace(".", ",").Trim(), CultureInfo.InvariantCulture);
            double PRECIO = double.Parse(Dato[6].Replace("S/", "").Trim(), CultureInfo.InvariantCulture);
            string UNIDAD = Dato[5];
            bool HVENTA = false;
            double IMPORTE = 0;
            string HISTORIAl = "";
            string CLAVE = Dato[1] + " (" + UNIDAD + ")"; // PARA GUARDAR EN EL DICCIONARIO
            if (Cantidad <= STOCK)
            {
                Console.WriteLine("Stock Disponible...");
                STOCK = STOCK - Cantidad;
                HVENTA = true;
                Dato[4] = STOCK.ToString();
                //REEMPLAZAMOS LA LINEA SIN MODIFICAR
                linea[indice] = string.Join("|", Dato);
                //CALCULO DEL IMPORTE
                IMPORTE = Cantidad * PRECIO;
                HISTORIAl += "\nPRODUCTO: " + Dato[1] + " | " + Dato[2] + " | " + Dato[3] +
                            "\nCANTIDAD: " + Cantidad +
                            "\nPRECIO: " + PRECIO +
                            "\nIMPORTE: " + IMPORTE;

                productosGlobal.Add(Dato[1]);
                totalProductosVendidos += Cantidad;
                totalVentas++;
                if (productosVendidos.ContainsKey(CLAVE))
                {
                    productosVendidos[CLAVE] += Cantidad;
                }
                else
                {
                    productosVendidos.Add(CLAVE, Cantidad);
                }
            }
            else
            {
                Console.WriteLine("Stock insuficiente...");
            }
            return (HVENTA, IMPORTE, HISTORIAl);
        }
        public static void METODOPAGO(double sUBTOTAL, string historialTotal)
        {
            //DEBE TENER 4 METODOS DE PAGO: YAPE, TRANFERENCIA, EFECTIVO,CREDITO(NORMAL Y FIADO(MAX 7), TARJETA(+4%). 
            //DEBE SER TENER LA CAPACIDAD DE PAGAR MIXTO(USANDO MINIMO 2 METODO DIFERENTES DE PAGO) 
            //EFECTIVO DEBE TENER LA CAPACIDAD QUE SI DESEA PAGAR O DAR UN ADELANTO DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS
            //LOS ADELANTOS SE DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS.
            //CREDITO TAMBIEN DEBE GENERARA Y GUARDAR LA INFORMACIÓN EN EL MISMO TXT DE DEUDAS.
            //A CREDITO SE LE DEBEN PONER DÍAS DE INICIO Y VENCIMIENTO
            //SOLO SI EL PAGO ESTÁ COMPLETO O EL METODO DE PAGO ES TARJETA O ES YAPE/TRANSFERENCIA, SE LE DEBE LLAMAR A LA OPCION DE SI DESEA UN TIPO COMPROBANTE
            // SE DEBE GUARDAR UN HISTORIALES
        }
        public static void TIPOCOMPROBANTE()
        {
            // DAR 2 OPCIONES DE COMPROBANTE: BOLETA O FACTURA
            // CREAR 2 ARCHIVOS DIFERENTES PARA BOLETAS Y PARA FACTURAS POR DÍA
            //CADA FACTURA Y BOLETA DEBE TENER UN CORRELATIVO Y SU NUMERO DE SERIE.
            //PARA BOLETA DEBE DAR LA OPCION DE 3 TIPOS DE DOCUMENTO( DNI, CARNET EXTR, SIN DOCUMENTO
            //

        }
        public static void DELIVERY()
        {
            //SE DEBE TENER LAS PAUTAS DE DELIVERY
            //SE DEBE CALCULAR ANTES DE DAR EL RESUMEN DE COMPRA COMPLETO, AUNQUE SE DEBE MOSTRAR ANTES DE PEDIR EL DELIVERY EL MONTO FINAL SOLO
            //DEBE ESTAR RELACIONADO CON OTROS METODOS
        }
        public static int NUMBOLETA()
        {
            string archivo = "BASEDATOS.txt";
            if (!File.Exists(archivo))
            {
                File.WriteAllText(archivo, "0");
            }
            int contador = int.Parse(File.ReadAllText(archivo));
            contador++;
            File.WriteAllText(archivo, contador.ToString());
            return contador;
        }
        public static int NUMFACTURA()
        {
            string archivo = "contador_factura.txt";
            if (!File.Exists(archivo))
            {
                File.WriteAllText(archivo, "0");
            }
            int contador = int.Parse(File.ReadAllText(archivo));
            contador++;
            File.WriteAllText(archivo, contador.ToString());
            return contador;
        }
        public static void HISTORIALVENTA()
        {

        }

        // ESTADISTICA DE VENTAS
        public static void ESTADISTICAS()
        {
            //CREACIÓN DE ESTADITICAS
        }
        public static void CIERRECAJA()
        {
            //CREACIÓN DE CIERRE DE CAJA, CON ARCHIVO TXT
        }

        //PROVEEDORES
        public static void OBTENERIDPROV()
        {

        }
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