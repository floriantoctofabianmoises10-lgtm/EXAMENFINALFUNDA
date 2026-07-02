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
                    for(int i =0; i < DATO.Length; i++)
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
                    string [] Dato = null;
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
                        for(int i =0; i<Dato.Length; i++)
                        {
                            Dato[i] = Dato[i].Trim();
                        }
                        //MOSTRANDO PRODUCTO
                        Console.WriteLine("=========================");
                        Console.WriteLine("  PRODUCTO SELECCIONADO  ");
                        Console.WriteLine(Dato[1] +" | "+ Dato[2] + " | " + Dato[3] + " | " + Dato[5] + " | " + Dato[6]);
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

                            if (!int.TryParse(SELECT2, out Opc2) || Opc2<1 || Opc2 >3)
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
                                int COR=1;
                                Console.WriteLine("========================");
                                Console.WriteLine("    LISTA DE PRODUCTO   ");
                                Console.WriteLine("========================");
                                //VUELVO A RECORRER LAS POSICIONES GUARDADAS
                                foreach(int D in posiciones)
                                {
                                    string[] Lista = linea[D].Split('|');
                                    for(int i = 0; i<Lista.Length; i++)
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
                while (string.IsNullOrWhiteSpace(IMPUT2) ||!(IMPUT2 == "SI" || IMPUT2 == "S" || IMPUT2 == "NO" || IMPUT2 == "N"))
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
            if(SUBTOTAL <= 0)
            {
                Console.WriteLine("No se registró ninguna venta.");
                return;
            }
            Console.WriteLine("=============================");
            Console.Write("  ¿CONFIRMA LA COMPRA? (S/N): " );
            string CF = Console.ReadLine().ToUpper().Trim() ;
            if (SALIR(CF))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            while (string.IsNullOrWhiteSpace(CF) ||!(CF == "SI" || CF == "S" || CF == "NO" || CF == "N"))
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
            double CANTS=0;
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
        public static (bool exito,double importe, string historial) CONTROL_STOCK(string[] Dato, ref string[] linea, int indice, double Cantidad)
        {
            //VARIABLE NORMALES
            double STOCK = double.Parse(Dato[4].Replace(".", ",").Trim(), CultureInfo.InvariantCulture);
            double PRECIO = double.Parse(Dato[6].Replace("S/", "").Trim(), CultureInfo.InvariantCulture);
            string UNIDAD = Dato[5];
            bool HVENTA = false;
            double IMPORTE=0;
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
        public static void METODOPAGO() 
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
        public static void VERIFICAR()
        {
            //VERIFICAR CREACIÓN CORRECTA DE ARCHIVOS
        }
        public static void MENUPROV()
        {

        }
        public static void MOSTRARPROV()
        {

        }
        public static void AGREGARPROV()
        {

        }
        public static void MODIFICARPROV()
        {

        }

        //REGISTRAR COMPRAS
        public static void AGREGARPROD()
{
    string archivo = "BASEDATOS.txt";

    if (!File.Exists(archivo))
        File.WriteAllText(archivo, "");

    string[] lineas = File.ReadAllLines(archivo);
    int ultimoNumero = 0;

    foreach (string linea in lineas)
    {
        if (string.IsNullOrWhiteSpace(linea)) continue;

        string[] datos = linea.Split('|');
        if (datos.Length < 7) continue;

        string id = datos[0].Trim().ToUpper();

        if (id.StartsWith("P") && int.TryParse(id.Substring(1), out int numeroID))
        {
            if (numeroID > ultimoNumero)
                ultimoNumero = numeroID;
        }
    }

    string nuevoID = "P" + (ultimoNumero + 1).ToString("000");

    Console.Clear();
    Console.WriteLine("========== AGREGAR PRODUCTO ==========");
    Console.WriteLine("Escribe 'SALIR' para cancelar.");
    Console.WriteLine("ID generado automáticamente: " + nuevoID);

    Console.Write("Nombre del producto: ");
    string nombre = Console.ReadLine();
    if (SALIR(nombre)) return;
    while (string.IsNullOrWhiteSpace(nombre))
    {
        Console.Write("No puede estar vacío. Ingrese nombre: ");
        nombre = Console.ReadLine();
        if (SALIR(nombre)) return;
    }

    Console.Write("Modelo / descripción: ");
    string modelo = Console.ReadLine();
    if (SALIR(modelo)) return;
    while (string.IsNullOrWhiteSpace(modelo))
    {
        Console.Write("No puede estar vacío. Ingrese modelo: ");
        modelo = Console.ReadLine();
        if (SALIR(modelo)) return;
    }

    Console.Write("Marca: ");
    string marca = Console.ReadLine();
    if (SALIR(marca)) return;
    while (string.IsNullOrWhiteSpace(marca))
    {
        Console.Write("No puede estar vacío. Ingrese marca: ");
        marca = Console.ReadLine();
        if (SALIR(marca)) return;
    }

    Console.Write("Proveedor: ");
    string proveedor = Console.ReadLine();
    if (SALIR(proveedor)) return;
    while (string.IsNullOrWhiteSpace(proveedor))
    {
        Console.Write("No puede estar vacío. Ingrese proveedor: ");
        proveedor = Console.ReadLine();
        if (SALIR(proveedor)) return;
    }

    double stock;
    string stockTexto;
    do
    {
        Console.Write("Stock: ");
        stockTexto = Console.ReadLine();
        if (SALIR(stockTexto)) return;

    } while (string.IsNullOrWhiteSpace(stockTexto) ||
             !double.TryParse(stockTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) ||
             stock < 0);

    Console.Write("Unidad (KG / UND): ");
    string unidad = Console.ReadLine();
    if (SALIR(unidad)) return;
    while (string.IsNullOrWhiteSpace(unidad))
    {
        Console.Write("No puede estar vacío. Ingrese unidad: ");
        unidad = Console.ReadLine();
        if (SALIR(unidad)) return;
    }

    double precio;
    string precioTexto;
    do
    {
        Console.Write("Precio: ");
        precioTexto = Console.ReadLine();
        if (SALIR(precioTexto)) return;

    } while (string.IsNullOrWhiteSpace(precioTexto) ||
             !double.TryParse(precioTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio) ||
             precio <= 0);

    string nuevoProducto = nuevoID + " | " +
                           nombre.Trim() + " | " +
                           modelo.Trim() + " | " +
                           marca.Trim() + " | " +
                           stock.ToString(CultureInfo.InvariantCulture).Trim() + " | " +
                           unidad.Trim().ToUpper() + " | S/ " +
                           precio.ToString("0.00", CultureInfo.InvariantCulture) + " | " +
                           proveedor.Trim();

    File.AppendAllText(archivo, nuevoProducto + Environment.NewLine);
    Console.WriteLine("Producto agregado correctamente.");
}

public static void BUSCARPROD()
{
    string archivo = "BASEDATOS.txt";

    if (!File.Exists(archivo))
    {
        Console.WriteLine("No existe el archivo BASEDATOS.txt");
        return;
    }

    string[] lineas = File.ReadAllLines(archivo);

    Console.Clear();
    Console.WriteLine("========== BUSCAR PRODUCTO ==========");
    Console.WriteLine("Escribe 'SALIR' para cancelar.");
    Console.Write("Ingrese nombre, marca o ID del producto: ");
    string buscar = Console.ReadLine();

    if (SALIR(buscar)) return;

    while (string.IsNullOrWhiteSpace(buscar))
    {
        Console.Write("No puede estar vacío. Ingrese nombre, marca o ID: ");
        buscar = Console.ReadLine();
        if (SALIR(buscar)) return;
    }

    bool encontrado = false;
    string textoBuscar = QUITARTILDES(buscar.Trim()).ToUpper();

    foreach (string linea in lineas)
    {
        if (string.IsNullOrWhiteSpace(linea)) continue;
        if (linea.StartsWith("====") || linea.StartsWith("ID") || linea.StartsWith("---")) continue;

        string[] datos = linea.Split('|');
        if (datos.Length < 7) continue;

        for (int i = 0; i < datos.Length; i++)
            datos[i] = datos[i].Trim();

        if (string.IsNullOrWhiteSpace(datos[0]) ||
            string.IsNullOrWhiteSpace(datos[1]) ||
            string.IsNullOrWhiteSpace(datos[3]))
            continue;

        string textoLinea = QUITARTILDES(linea.Trim()).ToUpper();

        if (textoLinea.Contains(textoBuscar))
        {
            Console.WriteLine("ID: " + datos[0] +
                              " | Producto: " + datos[1] +
                              " | Modelo: " + datos[2] +
                              " | Marca: " + datos[3] +
                              " | Stock: " + datos[4] +
                              " | Unidad: " + datos[5] +
                              " | Precio: " + datos[6] +
                              (datos.Length >= 8 ? " | Proveedor: " + datos[7] : ""));

            encontrado = true;
        }
    }

    if (!encontrado)
        Console.WriteLine("Producto no encontrado.");
}

public static void MODIFICARPROD()
{
    string archivo = "BASEDATOS.txt";

    if (!File.Exists(archivo))
    {
        Console.WriteLine("No existe el archivo BASEDATOS.txt");
        return;
    }

    string[] lineas = File.ReadAllLines(archivo);

    Console.Clear();
    Console.WriteLine("========== MODIFICAR PRODUCTO ==========");
    Console.WriteLine("Escribe 'SALIR' para cancelar.");
    Console.Write("Ingrese el ID del producto a modificar, ejemplo P001: ");
    string idBuscar = Console.ReadLine();

    if (SALIR(idBuscar)) return;

    int numeroBuscado;
    while (string.IsNullOrWhiteSpace(idBuscar) ||
           !idBuscar.Trim().ToUpper().StartsWith("P") ||
           !int.TryParse(idBuscar.Trim().ToUpper().Substring(1), out numeroBuscado))
    {
        Console.Write("ID inválido. Ingrese un ID como P001: ");
        idBuscar = Console.ReadLine();
        if (SALIR(idBuscar)) return;
    }

    bool encontrado = false;

    for (int i = 0; i < lineas.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(lineas[i])) continue;

        string[] datos = lineas[i].Split('|');
        if (datos.Length < 7) continue;

        for (int j = 0; j < datos.Length; j++)
            datos[j] = datos[j].Trim();

        string idActual = datos[0].Trim().ToUpper();

        if (!idActual.StartsWith("P") || !int.TryParse(idActual.Substring(1), out int numeroActual))
            continue;

        if (numeroActual == numeroBuscado)
        {
            encontrado = true;

            string proveedorActual = datos.Length >= 8 ? datos[7] : "";

            Console.WriteLine("Producto encontrado:");
            Console.WriteLine(lineas[i]);
            Console.WriteLine("Presiona ENTER para mantener el dato actual.");

            Console.Write("Nuevo nombre (" + datos[1] + "): ");
            string nombre = Console.ReadLine();
            if (SALIR(nombre)) return;
            if (string.IsNullOrWhiteSpace(nombre)) nombre = datos[1];

            Console.Write("Nuevo modelo (" + datos[2] + "): ");
            string modelo = Console.ReadLine();
            if (SALIR(modelo)) return;
            if (string.IsNullOrWhiteSpace(modelo)) modelo = datos[2];

            Console.Write("Nueva marca (" + datos[3] + "): ");
            string marca = Console.ReadLine();
            if (SALIR(marca)) return;
            if (string.IsNullOrWhiteSpace(marca)) marca = datos[3];

            Console.Write("Nuevo proveedor (" + proveedorActual + "): ");
            string proveedor = Console.ReadLine();
            if (SALIR(proveedor)) return;
            if (string.IsNullOrWhiteSpace(proveedor)) proveedor = proveedorActual;

            double stock;
            Console.Write("Nuevo stock (" + datos[4] + "): ");
            string stockTexto = Console.ReadLine();
            if (SALIR(stockTexto)) return;
            if (string.IsNullOrWhiteSpace(stockTexto)) stockTexto = datos[4];

            while (!double.TryParse(stockTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) || stock < 0)
            {
                Console.Write("Stock inválido. Ingrese nuevamente: ");
                stockTexto = Console.ReadLine();
                if (SALIR(stockTexto)) return;
            }

            Console.Write("Nueva unidad (" + datos[5] + "): ");
            string unidad = Console.ReadLine();
            if (SALIR(unidad)) return;
            if (string.IsNullOrWhiteSpace(unidad)) unidad = datos[5];

            double precio;
            string precioActual = datos[6].Replace("S/", "").Trim();

            Console.Write("Nuevo precio (" + precioActual + "): ");
            string precioTexto = Console.ReadLine();
            if (SALIR(precioTexto)) return;
            if (string.IsNullOrWhiteSpace(precioTexto)) precioTexto = precioActual;

            while (!double.TryParse(precioTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio) || precio <= 0)
            {
                Console.Write("Precio inválido. Ingrese nuevamente: ");
                precioTexto = Console.ReadLine();
                if (SALIR(precioTexto)) return;
            }

            lineas[i] = datos[0] + " | " +
                       nombre.Trim() + " | " +
                       modelo.Trim() + " | " +
                       marca.Trim() + " | " +
                       stock.ToString(CultureInfo.InvariantCulture).Trim() + " | " +
                       unidad.Trim().ToUpper() + " | S/ " +
                       precio.ToString("0.00", CultureInfo.InvariantCulture) + " | " +
                       proveedor.Trim();

            break;
        }
    }

    if (encontrado)
    {
        File.WriteAllLines(archivo, lineas);
        Console.WriteLine("Producto modificado correctamente.");
    }
    else
    {
        Console.WriteLine("No se encontró un producto con ese ID.");
    }
}

public static void ELIMINARPRODUCTO()
{
    string archivo = "BASEDATOS.txt";

    if (!File.Exists(archivo))
    {
        Console.WriteLine("No existe el archivo BASEDATOS.txt");
        return;
    }

    List<string> lineas = File.ReadAllLines(archivo).ToList();

    Console.Clear();
    Console.WriteLine("========== ELIMINAR PRODUCTO ==========");
    Console.WriteLine("Escribe 'SALIR' para cancelar.");
    Console.Write("Ingrese el ID del producto a eliminar, ejemplo P001: ");
    string idBuscar = Console.ReadLine();

    if (SALIR(idBuscar)) return;

    int numeroBuscado;
    while (string.IsNullOrWhiteSpace(idBuscar) ||
           !idBuscar.Trim().ToUpper().StartsWith("P") ||
           !int.TryParse(idBuscar.Trim().ToUpper().Substring(1), out numeroBuscado))
    {
        Console.Write("ID inválido. Ingrese un ID como P001: ");
        idBuscar = Console.ReadLine();
        if (SALIR(idBuscar)) return;
    }

    bool encontrado = false;

    for (int i = 0; i < lineas.Count; i++)
    {
        if (string.IsNullOrWhiteSpace(lineas[i])) continue;

        string[] datos = lineas[i].Split('|');
        if (datos.Length < 7) continue;

        for (int j = 0; j < datos.Length; j++)
            datos[j] = datos[j].Trim();

        string idActual = datos[0].Trim().ToUpper();

        if (!idActual.StartsWith("P") || !int.TryParse(idActual.Substring(1), out int numeroActual))
            continue;

        if (numeroActual == numeroBuscado)
        {
            encontrado = true;

            Console.WriteLine("Producto encontrado:");
            Console.WriteLine(lineas[i]);

            Console.Write("¿Está seguro de eliminarlo? (S/N): ");
            string confirmar = Console.ReadLine();

            if (SALIR(confirmar)) return;

            while (string.IsNullOrWhiteSpace(confirmar) ||
                   !(confirmar.Trim().ToUpper() == "S" || confirmar.Trim().ToUpper() == "SI" ||
                     confirmar.Trim().ToUpper() == "N" || confirmar.Trim().ToUpper() == "NO"))
            {
                Console.Write("Ingrese S o N: ");
                confirmar = Console.ReadLine();
                if (SALIR(confirmar)) return;
            }

            if (confirmar.Trim().ToUpper() == "S" || confirmar.Trim().ToUpper() == "SI")
            {
                lineas.RemoveAt(i);

                int nuevoNumero = 1;

                for (int k = 0; k < lineas.Count; k++)
                {
                    if (string.IsNullOrWhiteSpace(lineas[k])) continue;

                    string[] partes = lineas[k].Split('|');
                    if (partes.Length < 7) continue;

                    string idParte = partes[0].Trim().ToUpper();

                    if (!idParte.StartsWith("P") || !int.TryParse(idParte.Substring(1), out int numeroTemporal))
                        continue;

                    partes[0] = "P" + nuevoNumero.ToString("000");

                    for (int p = 0; p < partes.Length; p++)
                        partes[p] = partes[p].Trim();

                    lineas[k] = string.Join(" | ", partes);
                    nuevoNumero++;
                }

                File.WriteAllLines(archivo, lineas);
                Console.WriteLine("Producto eliminado correctamente.");
                Console.WriteLine("Los ID fueron reordenados correctamente.");
            }
            else
            {
                Console.WriteLine("Eliminación cancelada.");
            }

            break;
        }
    }

    if (!encontrado)
        Console.WriteLine("No se encontró un producto con ese ID.");
}

public static void MENUPROD()
{
    int opcion;

    do
    {
        Console.Clear();
        Console.WriteLine("========== MENÚ DE PRODUCTOS / COMPRAS ==========");
        Console.WriteLine("1. Buscar producto");
        Console.WriteLine("2. Agregar producto");
        Console.WriteLine("3. Modificar producto");
        Console.WriteLine("4. Eliminar producto");
        Console.WriteLine("5. Volver al menú principal");
        Console.WriteLine("6. Salir");
        Console.Write("Seleccione una opción: ");

        string entrada = Console.ReadLine();

        if (SALIR(entrada)) return;

        while (string.IsNullOrWhiteSpace(entrada) ||
               !int.TryParse(entrada.Trim(), out opcion) ||
               opcion < 1 || opcion > 6)
        {
            Console.Write("Opción inválida. Ingrese un número del 1 al 6: ");
            entrada = Console.ReadLine();

            if (SALIR(entrada)) return;
        }

        Console.Clear();

        switch (opcion)
        {
            case 1:
                BUSCARPROD();
                break;

            case 2:
                AGREGARPROD();
                break;

            case 3:
                MODIFICARPROD();
                break;

            case 4:
                ELIMINARPRODUCTO();
                break;

            case 5:
                Console.WriteLine("Volviendo al menú principal...");
                break;

            case 6:
                Console.WriteLine("Saliendo del módulo de productos...");
                return;
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();

    } while (opcion != 5);
}

        // DEUDAS
        public static void DEUDASVENCIDAS()
        {

        }
        public static void ABONARDEUDA()
        {
            //SOLO CUANDO SE ABONE COMPLETAMENTE LA DEUDA, RECIEN IMPRIMIR COMPROBANTE
        }
        public static void BUSCARDEUDA()
        {

        }
        public static void IMPRIMIRDEUDOR()
        {

        }
        public static void MENUDEUDAS()
        {

        }
        //MENU PRINCIPAL
        static void Main(string[] args)
        {
            int opcion = 0;
            CARGARSESION();
            CARGARVENTASDIA(); 
            while (true) // 🔴 SISTEMA SIEMPRE ENCENDIDO
            {
                Console.WriteLine("===============================");
                Console.WriteLine("= SISTEMA DE VENTAS / COMPRAS= ");
                Console.WriteLine("===============================");
                Console.WriteLine("1. Registrar Ventas");
                Console.WriteLine("2. Registrar Compras");
                Console.WriteLine("3. Proveedores");
                Console.WriteLine("4. Finalizar Día");
                Console.WriteLine("5. Gestionar Deudores");
                Console.WriteLine("6. Salir");
                Console.Write("SELECCIONE: ");

                while (!int.TryParse(Console.ReadLine(), out opcion) || opcion < 1 || opcion > 6)
                {
                    Console.WriteLine("Ingrese solo números (1-6)...");
                }

                switch (opcion)
                {
                    case 1:
                        REGISTROVENTAS();
                        break;

                    case 2:
                        MENUPROD();
                        break;
                    case 3:
                        MENUPROV();
                        break;

                    case 4:
                        {
                            bool volverMenu = false;

                            while (!volverMenu)
                            {
                                Console.WriteLine("==============================");
                                Console.WriteLine("===== FINALIZAR DÍA =====");
                                Console.WriteLine("==============================");
                                Console.WriteLine("1. Mostrar estadísticas");
                                Console.WriteLine("2. Mostrar historial de venta total");
                                Console.WriteLine("3. Volver al menú principal");
                                Console.Write("SELECCIONE: ");

                                int OPCIÓNF;

                                while (!int.TryParse(Console.ReadLine(), out OPCIÓNF) || OPCIÓNF < 1 || OPCIÓNF > 3)
                                {
                                    Console.WriteLine("Ingrese 1, 2 o 3");
                                }
                                switch (OPCIÓNF)
                                {
                                    case 1:
                                        ESTADISTICAS();
                                        break;

                                    case 2:
                                        CIERRECAJA();
                                        break;

                                    case 3:
                                        volverMenu = true; // 
                                        break;

                                    default:
                                        Console.WriteLine("Opción inválida");
                                        break;
                                }
                            }

                            break;
                        }

                    case 5:
                        MENUDEUDAS();
                        break;
                    case 6:
                        string OPCF;
                        do
                        {
                            Console.WriteLine("¿Desea finalizar el programa? (SI/NO)");
                            OPCF = Console.ReadLine().ToUpper();
                            if (OPCF == "S" || OPCF == "SI")
                            {
                                Console.WriteLine("Cerrando sistema...");
                                return;
                            }
                            else if (OPCF == "N" || OPCF == "NO")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Ingrese solo SI o NO");
                            }
                        } while (true);
                        break;
                }
            }
        }
    }
}
