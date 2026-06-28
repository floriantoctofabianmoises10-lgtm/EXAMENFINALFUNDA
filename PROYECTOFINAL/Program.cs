using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PROYECTOFINAL
{
    // CLASE VENTA
    public class Venta
    {
        public string Cliente { get; set; }
        public string Productos { get; set; }
        public double Total { get; set; }
    }

    internal class Program
    {
        // VARIABLES GLOBALES 
        static List<Venta> cierreCaja = new List<Venta>();
        static List<string> productosGlobal = new List<string>();
        static int totalVentas = 0;
        static double gananciasDia = 0;
        static double totalProductosVendidos = 0;
        static Dictionary<string, double> productosVendidos = new Dictionary<string, double>();

        static void Main(string[] args)
        {

            REGISTROVENTAS();
        }

        // MÉTODOS Y FUNCIONES 
        public static string[] LEERARCHIVO()
        {
            return File.ReadAllLines("BASEDATOS.txt");
        }
        // TILDES
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

        public static void METODOPAGO(double subtotal, string listaProductos)
        {
            Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");
            string cliente = "Cliente general";

            // 1. DELIVERY 
            string direccion = "No aplica";
            double costoDelivery = 0;
            Console.Write("\n¿Desea delivery? (S/N): ");
            string delivery = Console.ReadLine().ToUpper();

            if (delivery == "S" || delivery == "SI")
            {
                Console.Write("Ingrese su dirección: ");
                direccion = Console.ReadLine();
                if (subtotal < 20) Console.WriteLine("Delivery no disponible para compras menores a S/20");
                else if (subtotal >= 50) { costoDelivery = 0; Console.WriteLine("Delivery GRATIS"); }
                else if (subtotal >= 20 && subtotal <= 30) { costoDelivery = 5; }
                else { costoDelivery = 3; }
            }

            double totalFinal = subtotal + costoDelivery;

            // 2. SELECCIÓN DE MÉTODO
            Console.WriteLine("=============================");
            Console.WriteLine("TOTAL A PAGAR: S/" + totalFinal);
            Console.WriteLine("" +
                "1. Yape/Transferencia " +
                " 2. Efectivo " +
                " 3. Crédito " +
                " 4. Tarjeta (+4%)");

            int metodo = int.Parse(Console.ReadLine());
            string metodoPago = (metodo == 1) ? "Yape" : (metodo == 2) ? "Efectivo" : (metodo == 3) ? "Crédito" : "Tarjeta";

            // 3. CREDITO INGRESA DIAS
            if (metodo == 3)
            {
                Console.Write("Nombre cliente: ");
                cliente = Console.ReadLine();
                Console.Write("Días para pagar: ");
                int dias = int.Parse(Console.ReadLine());
                DateTime fechaInicio = DateTime.Now;
                DateTime fechaFin = fechaInicio.AddDays(dias);

                string registroDeuda = $"{cliente}|{metodoPago}|{totalFinal}|{dias}|{fechaInicio:dd/MM/yyyy}|{fechaFin:dd/MM/yyyy}";
                File.AppendAllText("DEUDAS.txt", registroDeuda + "\n");
                Console.WriteLine("Crédito registrado.");
            }

            // 4. LÓGICA DE EFECTIVO 
            bool ventaCompletada = false;
            if (metodo == 2)
            {
                Console.Write("Monto entregado por cliente: ");
                double montoRecibido = double.Parse(Console.ReadLine());

                if (montoRecibido < totalFinal)
                {
                    Console.WriteLine("Pago parcial. El resto queda pendiente.");
                }
                else
                {
                    Console.WriteLine("Vuelto: S/" + (montoRecibido - totalFinal));
                    ventaCompletada = true;
                }
            }
            else { ventaCompletada = true; }

            // 5. COMPROBANTE
            if (ventaCompletada || metodo == 1 || metodo == 4)
            {
                Console.Write("¿Desea comprobante? (SI/NO): ");
                if (Console.ReadLine().ToUpper() == "SI" || Console.ReadLine().ToUpper() == "S")
                { 
        public static void TIPOCOMPROBANTE(ref string cliente, string listaProductos, double subtotal, string metodoPago, double costoDelivery, string direccion, double totalFinal)
       
        {
            Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");
            Console.WriteLine("==============================");
            Console.WriteLine("ELIJA SU TIPO DE COMPROBANTE: ");
            Console.WriteLine("1. BOLETA");
            Console.WriteLine("2. FACTURA");
            Console.Write("Elija una opción: ");

            string input = Console.ReadLine();
            if (input.ToUpper() == "SALIR") return;

            int OPC2;
            while (!int.TryParse(input, out OPC2) || OPC2 < 1 || OPC2 > 2)
            {
                Console.WriteLine("Ingrese una opción válida (1 o 2): ");
                input = Console.ReadLine();
                if (input.ToUpper() == "SALIR") return;
            }
                
            // VARIABLES NECESARIAS
            string anio = DateTime.Now.Year.ToString();
            string mensajeMarketing = (totalFinal >= 30)
                ? "\n¡FELICIDADES! Por su compra mayor a S/30, accede a un 5% de descuento en su próxima compra."
                : "\nNota: Acumule S/30 en sus compras para acceder a un 5% de descuento en su próxima visita.";
        
            // SWITCH ÚNICO
            switch (OPC2)
            {
                case 1:
                    int numeroBoleta = ObtenerNumeroBoleta();
                    string archivoBoleta = "BOLETA_" + cliente.Replace(" ", "") + "_" + anio + "-" + numeroBoleta.ToString("000") + ".txt";
                    using (StreamWriter sw = new StreamWriter(archivoBoleta, true))
                    {
                        sw.WriteLine("==============================");
                        sw.WriteLine("       BOLETA DE VENTA        ");
                        sw.WriteLine("==============================");
                        sw.WriteLine("N° Boleta: " + anio + "-" + numeroBoleta.ToString("000"));
                        sw.WriteLine("Fecha: " + DateTime.Now);
                        sw.WriteLine("Cliente: " + cliente);
                        sw.WriteLine(listaProductos);
                        sw.WriteLine("Subtotal: S/ " + subtotal);
                        sw.WriteLine("Método de pago: " + metodoPago);
                        sw.WriteLine("Delivery: S/ " + costoDelivery);
                        sw.WriteLine("Dirección: " + direccion);
                        sw.WriteLine("TOTAL: S/ " + totalFinal);
                        sw.WriteLine("------------------------------");
                        sw.WriteLine(mensajeMarketing);
                        sw.WriteLine("¡Guarde su boleta, se vienen grandes sorteos!");
                        sw.WriteLine("---Muchas gracias por su compra en la Bodega La Nona---");
                        sw.WriteLine("==============================");
                    }
                    break;

                case 2:
                    int numeroFactura = ObtenerNumeroFactura();
                    string archivoFactura = "FACTURA_" + cliente.Replace(" ", "") + "_" + anio + "-" + numeroFactura.ToString("000") + ".txt";
                    using (StreamWriter sw2 = new StreamWriter(archivoFactura, true))
                    {
                        sw2.WriteLine("==============================");
                        sw2.WriteLine("       FACTURA DE VENTA       ");
                        sw2.WriteLine("==============================");
                        sw2.WriteLine("N° Factura: " + anio + "-" + numeroFactura.ToString("000"));
                        sw2.WriteLine("Fecha: " + DateTime.Now);
                        sw2.WriteLine("Nombre: " + cliente);
                        sw2.WriteLine(listaProductos);
                        sw2.WriteLine("TOTAL: S/ " + totalFinal);
                        sw2.WriteLine("------------------------------");
                        sw2.WriteLine(mensajeMarketing);
                        sw2.WriteLine("¡Guarde su factura, se vienen grandes sorteos!");
                        sw2.WriteLine("---Muchas gracias por su compra en la Bodega La Nona---");
                        sw2.WriteLine("==============================");
                    }
                    break;
            }

            Console.WriteLine("Comprobante emitido con éxito.");
        } 
        public static int ObtenerNumeroBoleta()
        {
            if (!File.Exists("contador_boleta.txt")) File.WriteAllText("contador_boleta.txt", "0");
            int c = int.Parse(File.ReadAllText("contador_boleta.txt")) + 1;
            File.WriteAllText("contador_boleta.txt", c.ToString());
            return c;
        }

        public static int ObtenerNumeroFactura()
        {
            if (!File.Exists("contador_factura.txt")) File.WriteAllText("contador_factura.txt", "0");
            int c = int.Parse(File.ReadAllText("contador_factura.txt")) + 1;
            File.WriteAllText("contador_factura.txt", c.ToString());
            return c;
        }
        public static void DELIVERY(double total, out double costoDelivery, out string direccion)
        {
            //Delivery
            costoDelivery = 0;
            direccion = "No aplica";

            Console.WriteLine("==============================");
            Console.WriteLine("¿Usted desea solicitar delivery? (S/N): ");
            string respuesta = Console.ReadLine().ToUpper();

            if (respuesta == "S" || respuesta == "SI")
            {
                Console.Write("Ingrese su dirección exacta: ");
                direccion = Console.ReadLine();

                //FUNCIONAMIENTO DEL DESCUENTO
                if (total < 20)
                {
                    Console.WriteLine("Lo sentimos, el delivery no está disponible para compras menores a S/20.");
                    direccion = "No aplica"; 
                }
                else if (total >= 50)
                {
                    costoDelivery = 0;
                    Console.WriteLine("¡Excelente! Su compra califica para DELIVERY GRATIS.");
                }
                else if (total >= 20 && total <= 30)
                {
                    costoDelivery = 5;
                    Console.WriteLine("Se le incrementará 3 soles... bueno, según su rango, el costo es S/5.");
                }
                else 
                {
                    costoDelivery = 3;
                    Console.WriteLine("Se le incrementará 3 soles por el envío hasta su destino.");
                }
            }
            else
            {
                Console.WriteLine("Entendido. Puede venir a recoger su pedido a nuestra tienda cuando esté libre");
            }
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
            Console.WriteLine("\n==============================");
            Console.WriteLine("    HISTORIAL DE VENTAS");
            Console.WriteLine("==============================");

            //VENTAS
            string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");
            string archivoHistorial = "VENTAS_" + fechaHoy + ".txt";

            if (!File.Exists(archivoHistorial))
            {
                Console.WriteLine("Aún no hay ventas registradas para el día de hoy.");
            }
            else
            {
                Console.WriteLine("Mostrando ventas del día (" + fechaHoy + "):");
                Console.WriteLine("------------------------------");

                //MUESTRA
                using (StreamReader sr = new StreamReader(archivoHistorial))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(linea);
                    }
                }
                Console.WriteLine("------------------------------");
                Console.WriteLine("Fin del historial.");
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        
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

        }
        public static void BUSCARPROD()
        {

        }
        public static void MODIFICARPROD()
        {

        }
        public static void ELIMINARPRODUCTO()
        {

        }
        public static void MENUPROD()
        {

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
