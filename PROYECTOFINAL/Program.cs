using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        //ARCHIVOS GLOBALES
        static string archivo = "PROVEEDORES.txt";
        static string archivoDeudores = "DEUDAS.txt";
        
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
            totalVentas = 0;
            gananciasDia = 0;
            totalProductosVendidos = 0;

            productosVendidos.Clear();
            productosGlobal.Clear();
        }
        public static void CARGARSESION()
        {
            if (!File.Exists("SESION.txt"))
            {
                GUARDARSESION();
                return;
            }

            string[] lineas = File.ReadAllLines("SESION.txt");

            if (lineas.Length == 0)
            {
                REINICIODATOS();
                GUARDARSESION();
                return;
            }

            DateTime fecha;

            if (!DateTime.TryParse(lineas[0], out fecha))
            {
                REINICIODATOS();
                GUARDARSESION();
                return;
            }

            if (fecha.Date == DateTime.Today)
            {
                CARGARVENTASDIA(); // Reconstruye todas las variables
            
            }
            else
            {
                REINICIODATOS();
                GUARDARSESION();   // Inicia un nuevo día
            }
        }
        public static void GUARDARVENTAHISTORIAL(string cliente,
                                         string historial,
                                         double totalFinal,
                                         string metodoPago)
        {
            string archivoVentas = "VENTAS_" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";

            using (StreamWriter sw = new StreamWriter(archivoVentas, true))
            {
                sw.WriteLine("========================================");
                sw.WriteLine("FECHA: " + DateTime.Now);
                sw.WriteLine("CLIENTE: " + cliente);
                sw.WriteLine(historial);
                sw.WriteLine("----------------------------------------");
                sw.WriteLine("MÉTODO DE PAGO: " + metodoPago);
                sw.WriteLine("TOTAL: S/ " + totalFinal);
                sw.WriteLine("========================================");
                sw.WriteLine();
            }
        }
        public static void CARGARVENTASDIA()
        {
            if (!File.Exists("SESION.txt"))
                return;

            string[] lineas = File.ReadAllLines("SESION.txt");

            if (lineas.Length < 4)
                return;

            DateTime fecha;

            if (!DateTime.TryParse(lineas[0], out fecha))
                return;

            // Solo reconstruir si la sesión corresponde al día de hoy
            if (fecha.Date != DateTime.Today)
                return;

            totalVentas = int.Parse(lineas[1]);
            gananciasDia = double.Parse(lineas[2]);
            totalProductosVendidos = double.Parse(lineas[3]);

            productosVendidos.Clear();

            for (int i = 4; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split('|');

                if (datos.Length == 2)
                {
                    productosVendidos.Add(
                        datos[0],
                        double.Parse(datos[1])
                    );
                }
            }
        }
        public static void GUARDARSESION()
        {
            using (StreamWriter sw = new StreamWriter("SESION.txt"))
            {
                sw.WriteLine(DateTime.Today.ToString("yyyy-MM-dd"));
                sw.WriteLine(totalVentas);
                sw.WriteLine(gananciasDia);
                sw.WriteLine(totalProductosVendidos);

                foreach (var p in productosVendidos)
                {
                    sw.WriteLine(p.Key + "|" + p.Value);
                }
            }
        }


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
                Console.Write("INGRESE EL NOMBRE DEL PRODUCTO: ");
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
                Console.Write("DESEA INGRESAR OTRO PRODUCTO (S/N): ");
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
            Console.WriteLine("=================================");
            Console.WriteLine("       RESUMEN DE LA COMPRA       ");
            Console.WriteLine("=================================");
            Console.WriteLine(historialTotal);
            Console.WriteLine("=================================");
            Console.WriteLine("SUBTOTAL: S/" + SUBTOTAL);
            Console.WriteLine("=================================");

            Console.WriteLine("\n¿CONFIRMAR COMPRA? (S/N)");
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
                    Console.WriteLine("Operación cancelada....");
                    return;
                }
            }
            if (CF == "NO" || CF == "N")
            {
                Console.WriteLine("Venta cancelada.....");
                return;
            }
            // VARIABLES PARA EL DELIVERY
            var delivery = DELIVERY(SUBTOTAL);

            if (delivery.direccion == "SALIR")
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            double costoDelivery = delivery.costoDelivery;
            string direccion = delivery.direccion;
            if (delivery.direccion == "SALIR")
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            METODOPAGO(SUBTOTAL, historialTotal, direccion, costoDelivery);
            
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
        public static void MODELOBOLETA(int numeroBO,string archivoBoleta, DateTime año,string cliente, string listaProductos, double subtotal, string metodoPago, double costoDelivery, string direccion, double totalFinal)
        {
            double SUBTOTALIGV = subtotal / 1.18;
            double igvInformativo = subtotal - SUBTOTALIGV;
            using (StreamWriter sw = new StreamWriter(archivoBoleta))
            {
                sw.WriteLine("==============================");
                sw.WriteLine("       BOLETA DE VENTA        ");
                sw.WriteLine("==============================");
                sw.WriteLine("N° Boleta: B001- " + numeroBO.ToString("000"));
                sw.WriteLine("Fecha: " + DateTime.Now);
                sw.WriteLine("Cliente: " + cliente);
                sw.WriteLine(listaProductos);
                sw.WriteLine("Subtotal: S/ " + subtotal);
                sw.WriteLine("IGV (18%): S/ " + igvInformativo);
                sw.WriteLine("Método de pago: " + metodoPago);
                sw.WriteLine("Delivery: S/ " + costoDelivery);
                sw.WriteLine("Dirección: " + direccion);
                sw.WriteLine("TOTAL: S/ " + totalFinal);
                sw.WriteLine("------------------------------");
                sw.WriteLine("==== MUCHAS GRACIAS POR COMPRAR EN LA BODEGA LA NONA ====");
                sw.WriteLine("==============================");
            }
        }
        public static void MODELOFACTURA(int numeroFAC, double baseImponible, string codigo, double igv, string archivoFactura, DateTime año, string cliente, string listaProductos, double subtotal, string metodoPago, double costoDelivery, string direccion, double totalFinal)
        {
            using (StreamWriter sw = new StreamWriter(archivoFactura, true))
            {
                sw.WriteLine("==============================");
                sw.WriteLine("       FACTURA DE VENTA       ");
                sw.WriteLine("==============================");
                sw.WriteLine("N° Factura: F001- " + numeroFAC.ToString("000"));
                sw.WriteLine("Fecha: " + DateTime.Now);
                sw.WriteLine("RUC: " + codigo);
                sw.WriteLine("Cliente: " + cliente);
                sw.WriteLine(listaProductos);
                sw.WriteLine("Subtotal: S/ " + subtotal);
                sw.WriteLine("BASE IMPONIBLE: S/ " + baseImponible);
                sw.WriteLine("IGV (18%): S/ " + igv);
                sw.WriteLine("Método de pago: " + metodoPago);
                sw.WriteLine("Delivery: S/ " + costoDelivery);
                sw.WriteLine("Dirección: " + direccion);
                sw.WriteLine("TOTAL: S/ " + totalFinal);
                sw.WriteLine("------------------------------");
                sw.WriteLine("==== MUCHAS GRACIAS POR COMPRAR EN LA BODEGA LA NONA ====");
                sw.WriteLine("==============================");
            }
        }
        public static void METODOPAGO(double subtotal, string historialTotal, string direccion, double costoDelivery) 
        {
            //DEBE TENER 4 METODOS DE PAGO: YAPE, TRANFERENCIA, EFECTIVO,CREDITO(NORMAL Y FIADO(MAX 7), TARJETA(+4%). 
            //DEBE SER TENER LA CAPACIDAD DE PAGAR MIXTO(USANDO MINIMO 2 METODO DIFERENTES DE PAGO) 
            //EFECTIVO DEBE TENER LA CAPACIDAD QUE SI DESEA PAGAR O DAR UN ADELANTO DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS
            //LOS ADELANTOS SE DEBEN GUARDAR EN EL MISMO ARCHIVO TXT DE DEUDAS.
            //CREDITO TAMBIEN DEBE GENERARA Y GUARDAR LA INFORMACIÓN EN EL MISMO TXT DE DEUDAS.
            //A CREDITO SE LE DEBEN PONER DÍAS DE INICIO Y VENCIMIENTO
            //SOLO SI EL PAGO ESTÁ COMPLETO O EL METODO DE PAGO ES TARJETA O ES YAPE/TRANSFERENCIA, SE LE DEBE LLAMAR A LA OPCION DE SI DESEA UN TIPO COMPROBANTE
            // SE DEBE GUARDAR UN HISTORIALES
            //VARIABLES DE ENTRADA
            int OPCM;
            string IMPUT3;
            Console.WriteLine("===== MÉTODOS DE PAGO =====");
            Console.WriteLine("1. YAPE/TRANSFERENCIA");
            Console.WriteLine("2. EFECTIVO (Seleccione si desea pagar o dar un adelanto)");
            Console.WriteLine("3. CRÉDITO");
            Console.WriteLine("4. TARJETA (+4%)");
            Console.Write("SELECCIONE UNA  OPCIÓN: ");
            IMPUT3 = Console.ReadLine();
            if(SALIR(IMPUT3))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            while(!int.TryParse(IMPUT3, out OPCM) || OPCM < 1 || OPCM > 4)
            {
                Console.WriteLine("Ingrese una opción válida (1-4): ");
                IMPUT3 = Console.ReadLine();
                if (SALIR(IMPUT3))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }
            switch (OPCM)
            {
                case 1:
                    string metodoPago = "YAPE/TRANSFERENCIA";
                    Console.WriteLine("==== YAPE/TRANSFERENCIA ====");
                    MPYAPE_TRANSFERENCIA(subtotal, costoDelivery, direccion, historialTotal, metodoPago);
                    break;
                case 2:
                    metodoPago = "EFECTIVO";
                    MPEFECTIVO(subtotal, costoDelivery, direccion, historialTotal);
                    break;
                case 3:
                    metodoPago = "CRÉDITO";
                    MPCREDITO(subtotal, historialTotal);
                    break;
                case 4:
                    metodoPago = "TARJETA";
                    MPTARJETA(subtotal, costoDelivery, direccion, historialTotal, metodoPago);
                    break;         
            }
        }
        
        public static void MPTARJETA(double SUBTOTAL, double costoDelivery, string direccion, string historialTotal, string metodoPago)
        {
            Console.WriteLine("===== TARJETA =====");
            //VARIAABLES LOCALES
            double totalFinal = (SUBTOTAL + costoDelivery) * 1.04;
            string IMPUT12;
            string Cliente = "CLIENTE GENERAL";
            // MOSTRAMOS UN RESUMEN ANTES DE PEDIR COMPROBANTE
           Console.WriteLine("DELIVERY:S/" + costoDelivery);
            Console.WriteLine("DIRECCIÓN:" + direccion);
            Console.WriteLine("TOTAL FINAL:S/" + totalFinal);
            Console.WriteLine("========================================");
            Console.WriteLine("DESEA ALGÚN COMPROBANTE DE PAGO (S/N)?: ");
            IMPUT12 = Console.ReadLine().ToUpper();

            if (SALIR(IMPUT12))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            while (IMPUT12 != "SI" && IMPUT12 != "S" && IMPUT12 != "NO" && IMPUT12 != "N")
            {
                Console.WriteLine("DEBE INGRESAR SI O NO");
                IMPUT12 = Console.ReadLine().ToUpper();

                if (SALIR(IMPUT12))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }

            if (IMPUT12 == "SI" || IMPUT12 == "S")
            {
                Cliente=TIPOCOMPROBANTE(historialTotal, SUBTOTAL,"TARJETA", costoDelivery, direccion, totalFinal);
            }
            else
            {
                Console.WriteLine("VENTA REALIZADA Y REGISTRADA");
            }
            totalVentas++;
            gananciasDia += totalFinal;
            GUARDARSESION();
            GUARDARVENTAHISTORIAL(Cliente, historialTotal, totalFinal, metodoPago);
        }
        public static void MPCREDITO(double totalFinal, string historialTotal)
        {
            //VARIABLE LOCALES
            string IMPUT8, IMPUT9, IMPUT10;
            int DIASPAGO;

            Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
            IMPUT8 = Console.ReadLine();
            if (SALIR(IMPUT8))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            while (string.IsNullOrWhiteSpace(IMPUT8))
            {
                Console.WriteLine("No puede dejar vacío ni ingresar solo espacios.");
                Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
                IMPUT8 = Console.ReadLine();
                if (SALIR(IMPUT8))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }
            Console.Write("INGRESE LOS DIAS PARA PAGAR: ");
            IMPUT9 = Console.ReadLine();
            if (SALIR(IMPUT9))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            while (!int.TryParse(IMPUT9, out DIASPAGO) || DIASPAGO <= 0)
            {
                Console.WriteLine("INGRESE DIAS VALIDOS: ");
                IMPUT10 = Console.ReadLine();
                if (SALIR(IMPUT10))
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }
            double aCuenta = 0;
            FORMATODEUDA(IMPUT8, totalFinal, "Credito", DIASPAGO, historialTotal,aCuenta);
        }
        public static void MPYAPE_TRANSFERENCIA(double SUBTOTAL, double costoDelivery, string direccion, string historialTotal, string metodoPago)
        {

            //VARIAABLES LOCALES
            double totalFinal = SUBTOTAL + costoDelivery;
            string IMPUT11;
            string Cliente = "CLIENTE GENERAL";
            // MOSTRAMOS UN RESUMEN ANTES DE PEDIR COMPROBANTE
            Console.WriteLine("DELIVERY:S/" + costoDelivery);
            Console.WriteLine("DIRECCIÓN:" + direccion);
            Console.WriteLine("TOTAL FINAL:S/" + totalFinal);
            Console.WriteLine("========================================");
            Console.WriteLine("DESEA ALGÚN COMPROBANTE DE PAGO (S/N)?: ");
            IMPUT11 = Console.ReadLine().ToUpper();

            if (SALIR(IMPUT11))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }

            while (IMPUT11 != "SI" && IMPUT11 != "S" && IMPUT11 != "NO" && IMPUT11 != "N")
            {
                Console.WriteLine("DEBE INGRESAR SI O NO");
                IMPUT11 = Console.ReadLine().ToUpper();

                if (IMPUT11 == "SALIR")
                    return;
            }

            if (IMPUT11 == "SI" || IMPUT11 == "S")
            {
                Cliente = TIPOCOMPROBANTE(historialTotal, SUBTOTAL, metodoPago, costoDelivery, direccion, totalFinal);
            }
            else
            {
                Console.WriteLine("VENTA REALIZADA Y REGISTRADA");
            }
            gananciasDia += totalFinal;
            totalVentas++;

            GUARDARSESION();
            GUARDARVENTAHISTORIAL(Cliente, historialTotal, totalFinal, metodoPago);
        }
        public static void MPEFECTIVO( double SUBTOTAL, double costoDelivery, string direccion, string historialTotal)
        {
            //VARIABLES DE ENTRADA
            string IMPUT4;
            string IMPUT5;
            string IMPUT6;
            string IMPUT7;
            int OPCECTIVO;
            int DIASP;
            double ADELANTO;
            string metodoPago = "EFECTIVO";
            double deuda;
            // CALCULO DEL TOTAL FINAL
            double totalFinal = SUBTOTAL + costoDelivery;
            //MENU SECUNDARIO
            Console.WriteLine("===== EFECTIVO =====");
            Console.WriteLine("1. ADELANTO");
            Console.WriteLine("2. PAGO COMPLETO");
            IMPUT4 = Console.ReadLine();    
            if(SALIR(IMPUT4))
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
            while(!int.TryParse(IMPUT4, out OPCECTIVO) || OPCECTIVO < 1 || OPCECTIVO > 2)
            {
                Console.WriteLine("INGRESE UNA OPCIÓN VALIDA (1-2): ");
            }
            switch (OPCECTIVO)
            {
                case 1: 
                    Console.Write("INGRESE EL MONTO DEL ADELANTO: ");
                    IMPUT5= Console.ReadLine();
                    if (SALIR(IMPUT5))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }
                    while (!double.TryParse(IMPUT5, out ADELANTO) || ADELANTO <= 0)
                    {
                        Console.WriteLine("INGRESE UNA CANTIDAD VALIDA: ");
                        IMPUT5 = Console.ReadLine();
                        if (SALIR(IMPUT5))
                        {
                            Console.WriteLine("Operación cancelada.");
                            return;
                        }
                    }
                    if(ADELANTO >= totalFinal)
                    {
                        Console.WriteLine("Recuerdar que está en la función abono, si desea cancelar todo ingrese 'SALIR'");
                        return;
                    }

                    deuda = totalFinal - ADELANTO;
                    Console.WriteLine("Usted ha abonado S/" + ADELANTO + " y su deuda pendiente es de S/" + deuda);

                    Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
                    IMPUT6 = Console.ReadLine();
                    if (SALIR(IMPUT6))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }
                    while (string.IsNullOrWhiteSpace(IMPUT6))
                    {
                        Console.WriteLine("No puede dejar vacío ni ingresar solo espacios.");
                        Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
                        IMPUT6 = Console.ReadLine();
                        if (SALIR(IMPUT6))
                        {
                            Console.WriteLine("Operación cancelada.");
                            return;
                        }
                    }
                    Console.Write("INGRESE LOS DIAS PARA PAGAR: ");
                    IMPUT7 = Console.ReadLine();
                    if (SALIR(IMPUT7))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }
                    while (!int.TryParse(IMPUT7, out DIASP) || DIASP <= 0)
                    {
                        Console.WriteLine("INGRESE DIAS VALIDOS: ");
                        IMPUT7 = Console.ReadLine();
                        if (SALIR(IMPUT7))
                        {
                            Console.WriteLine("Operación cancelada.");
                            return;
                        }
                    }
                    double aCuenta = ADELANTO;
                    double saldo = totalFinal - aCuenta;
                    FORMATODEUDA(IMPUT6, totalFinal, "Adelanto", DIASP, historialTotal, aCuenta);
                    Console.WriteLine("=== DEUDA REGISTRADA ===");
                    Console.WriteLine("TOTAL: S/ " + totalFinal);
                    Console.WriteLine("A CUENTA: S/ " + aCuenta);
                    Console.WriteLine("SALDO: S/ " + saldo);
                    break;

                case 2:
                    //VARIABLES LOCALES
                    string OPCC;
                    string Cliente = "CLIENTE GENERAL";

                    Console.WriteLine("DELIVERY:S/" + costoDelivery);
                    Console.WriteLine("DIRECCIÓN:" + direccion);
                    Console.WriteLine("TOTAL FINAL:S/" + totalFinal);
                    Console.WriteLine("=================================");
                    Console.WriteLine("DESEA ALGÚN COMPROBANTE DE PAGO (S/N)?: ");
                    OPCC = Console.ReadLine().ToUpper();

                    if (SALIR(OPCC))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }

                    while (OPCC != "SI" && OPCC != "S" && OPCC != "NO" && OPCC != "N")
                    {
                        Console.WriteLine("DEBE INGRESAR SI O NO");
                        OPCC = Console.ReadLine().ToUpper();

                        if (OPCC == "SALIR")
                            return;
                    }

                    if (OPCC == "SI" || OPCC == "S")
                    {
                        Cliente = TIPOCOMPROBANTE(historialTotal, SUBTOTAL, metodoPago, costoDelivery, direccion, totalFinal);
                    }
                    else
                    {
                        Console.WriteLine("VENTA REALIZADA Y REGISTRADA");
                    }
                    totalVentas++;
                    gananciasDia += totalFinal;
                    GUARDARSESION();
                    GUARDARVENTAHISTORIAL(Cliente, historialTotal, totalFinal, metodoPago);
                    break;
            }
        }
        public static void FORMATODEUDA(string cliente, double totalFinal,string tipoCredito, int diasCredito, string historialTotal, double aCuenta)
        {
            //CALCULOS

            double SaldoRest= totalFinal- aCuenta;
            // VARIABLES PARA TIEMPOS DE CRÉDITO 
            DateTime fechaVencimiento = DateTime.Now.AddDays(diasCredito);
            // CREACIÓN DE ARCHIVO TXT DE DEUDAS
            using (StreamWriter sw = new StreamWriter("DEUDAS.txt", true))
            {
                sw.WriteLine("===================================");
                sw.WriteLine("CLIENTE: " + cliente);
                sw.WriteLine("TIPO DE CRÉDITO: " + tipoCredito);
                sw.WriteLine("TOTAL FINAL:  " + totalFinal);
                sw.WriteLine("A CUENTA:  " + aCuenta);
                sw.WriteLine("SALDO RESTANTE:  " + SaldoRest);
                sw.WriteLine("DIAS PARA PAGAR: " + diasCredito);
                sw.WriteLine("FECHA DE INICIO: " + DateTime.Now.ToString("dd/MM/yyyy"));
                sw.WriteLine("FECHA DE FIN: " + fechaVencimiento.ToString("dd/MM/yyyy"));
                sw.WriteLine("-------------------------------------------");
                sw.WriteLine("HISTORIAL DE LA COMPRA: " + historialTotal);
                sw.WriteLine("-------------------------------------------");
                sw.WriteLine("=========================");
            }
        }
        public static string TIPOCOMPROBANTE(string listaProductos, double subtotal, string metodoPago, double costoDelivery, string direccion, double totalFinal)
        {
            //VARIABLES LOCALES
            int OPC2;
            string año;
            string clienteComprobante = "";
            //INICIALIZO LA VARIABLES
            año = DateTime.Now.Year.ToString();
            Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");
            Console.WriteLine("==============================");
            Console.WriteLine("ELIJA SU TIPO DE COMPROBANTE: ");
            Console.WriteLine("1. BOLETA");
            Console.WriteLine("2. FACTURA");
            Console.Write("Elija una opción: ");
            string input = Console.ReadLine();
            if (SALIR(input))
            {
                Console.WriteLine("Operación cancelada.");
                return "SALIR";
            }

            while (!int.TryParse(input, out OPC2) || OPC2 < 1 || OPC2 > 2)
            {
                Console.WriteLine("Ingrese una opción válida (1 o 2): ");
                input = Console.ReadLine();
                if (SALIR(input))
                {
                    Console.WriteLine("Operación cancelada.");
                    return "SALIR";
                }

            }
            switch (OPC2)
            {
                case 1:
                    //VARIABLES LOCALES
                    int numeroBoleta = NUMBOLETA(); 
                    string archivoBoleta = "BOLETA_"  + "_" + año + "-" + numeroBoleta.ToString("000") + ".txt";
                    int opcc;
                    string documento;
                    Console.WriteLine("==============================");
                    Console.WriteLine("=== BOLETA ====");
                    Console.WriteLine("==============================");
                    Console.WriteLine("N° Boleta: " + numeroBoleta);
                    Console.WriteLine("Ingrese su tipo de documento: ");
                    Console.WriteLine("1. DNI ");
                    Console.WriteLine("2. Carnet de extranjería ");
                    Console.WriteLine("3. Sin documento");
                    Console.WriteLine("SELECCIONE UNA OPCIÓN: ");
                    string input1 = Console.ReadLine();

                    if (SALIR(input1))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return "SALIR";
                    }

                    while (!int.TryParse(input1, out opcc) || opcc < 1 || opcc > 3)
                    {
                        Console.WriteLine("Ingrese solo 1, 2 o 3");
                        input1 = Console.ReadLine();

                        if (SALIR(input1))
                        {
                            Console.WriteLine("Operación cancelada.");
                            return "SALIR";
                        }
                    }
                    string tipoDocumento = " "; 
                    switch (opcc)
                    {

                        case 1:

                            tipoDocumento = "DNI";
                            do
                            {
                                Console.Write("INGRESE SU DNI DE 8 DIGITOS: ");
                                documento = Console.ReadLine();
                                if (SALIR(documento))
                                {
                                    Console.WriteLine("Operación cancelada.");
                                    return "SALIR";
                                }
                                if (documento.Length != 8 || !documento.All(char.IsDigit))
                                {
                                    Console.WriteLine("Error: Debe ingresar exactamente 8 números.");
                                }
                            } while (documento.Length != 8 || !documento.All(char.IsDigit));
                            break;

                        case 2:
                            tipoDocumento = "Carnet de extranjería";
                            Console.Write("INGRESE SU CARNET: ");
                            documento = Console.ReadLine();
                            if (SALIR(input1))
                            {
                                Console.WriteLine("Operación cancelada.");
                                return "SALIR";
                            }
                            break;
                        case 3:
                            tipoDocumento = "Sin documento";
                            documento = "-";
                            break;

                        default:
                        Console.WriteLine("Opción inválida");
                        return "SALIR";
                    }

                   Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
                   clienteComprobante = Console.ReadLine();
                   if (SALIR(clienteComprobante))
                   {
                       Console.WriteLine("Operación cancelada.");
                       return "SALIR";
                   }
                    MODELOBOLETA(numeroBoleta, archivoBoleta, DateTime.Now, clienteComprobante, listaProductos, subtotal, metodoPago, costoDelivery, direccion, totalFinal);
                    break;
                case 2:
                            int numeroFactura = NUMFACTURA();
                            string archivoFactura = "FACTURA_"  + "-" + numeroFactura.ToString("000") + ".txt";
                            string codigo;
                            double baseImponible = subtotal / 1.18;
                            double igv = subtotal - baseImponible;
                            Console.WriteLine("==============================");
                            Console.WriteLine("=== FACTURA===");
                            Console.WriteLine("==============================");
                            Console.WriteLine("N° Factura: F001-" + numeroFactura.ToString("0000"));
                            string fecha = DateTime.Now.ToString("yyyy-MM-dd");
                            do
                            {
                                Console.Write("INGRESE SU RUC DE 11 DIGITOS: ");
                                codigo = Console.ReadLine();
                                    if (SALIR(codigo))
                                    {
                                        Console.WriteLine("Operación cancelada.");
                                        return "SALIR";
                                    }
                                    if (codigo.Length != 11 || !codigo.All(char.IsDigit))
                                    {
                                        Console.WriteLine("Error: Debe ingresar exactamente 11 números.");
                                    }

                            } while (codigo.Length != 11 || !codigo.All(char.IsDigit));
                           Console.Write("INGRESE SU NOMBRE Y APELLIDO: ");
                           clienteComprobante = Console.ReadLine();
                                    if (SALIR(clienteComprobante))
                                    {
                                        Console.WriteLine("Operación cancelada.");
                                        return "SALIR";
                                    }
                                    MODELOFACTURA(numeroFactura, baseImponible, codigo, igv, archivoFactura, DateTime.Now, clienteComprobante, listaProductos, subtotal, metodoPago, costoDelivery, direccion, totalFinal);
                    break;
                    
            }
            Console.WriteLine("Comprobante emitido con éxito.");
            return clienteComprobante;
        }
        public static (double costoDelivery, string direccion) DELIVERY(double total)
        {
            double costoDelivery = 0;
            string direccion = "No aplica";
            Console.WriteLine("==============================");
            Console.WriteLine("TOTAL ACTUAL: S/" + total);
            if (total < 20)
            {
                Console.WriteLine("Delivery no disponible para compras menores a S/20");
                costoDelivery = 0;
            }
            else if (total >= 50)
            {
                Console.WriteLine("Tu compra califica para DELIVERY GRATIS");

                Console.Write("DESEAS DELIVERY (SI/NO)?: ");
                string delivery = Console.ReadLine().ToUpper();
                if (SALIR(delivery))
                {
                    Console.WriteLine("Operación cancelada.");
                    return(-1, "SALIR");
                }
                while (delivery != "SI" && delivery != "NO")
                {
                    Console.Write("INGRESE SI O NO: ");
                    delivery = Console.ReadLine().ToUpper();
                }

                if (delivery == "SI")
                {
                    Console.Write("DIRECCIÓN: ");
                    direccion = Console.ReadLine();
                    if (SALIR(direccion))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return (-1, "SALIR");
                    }
                    costoDelivery = 0;
                    Console.WriteLine("Delivery GRATIS");
                }
                else
                {
                    costoDelivery = 0;
                }
            }
            else if (total >= 20 && total <= 30)
            {
                Console.WriteLine("Delivery disponible: S/5");

                Console.Write("DESEAS DELIVERY (SI/NO)?: ");
                string delivery = Console.ReadLine().ToUpper();
                if (SALIR(delivery))
                {
                    Console.WriteLine("Operación cancelada.");
                    return (-1, "SALIR");
                }
                while (delivery != "SI" && delivery != "NO")
                {
                    Console.Write("INGRESE SI O NO: ");
                    delivery = Console.ReadLine().ToUpper();
                    if (SALIR(delivery))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return (-1, "SALIR");
                    }
                }

                if (delivery == "S" || delivery == "SI")
                {
                    Console.Write("DIRECCIÓN: ");
                    direccion = Console.ReadLine();
                    if (SALIR(direccion))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return (-1, "SALIR");
                    }
                    costoDelivery = 5;
                }
                else
                {
                    costoDelivery = 0;
                }
            }
            else if (total > 30 && total < 50)
            {
                Console.WriteLine("Delivery disponible: S/3");

                Console.Write("DESEAS DELIVERY (SI/NO)?: ");
                string delivery = Console.ReadLine().ToUpper();
                if (SALIR(delivery))
                {
                    Console.WriteLine("Operación cancelada.");
                    return (-1, "SALIR");
                }
                while (delivery != "SI" && delivery != "NO")
                {
                    Console.Write("INGRESE SI O NO: ");
                    delivery = Console.ReadLine().ToUpper();
                }

                if (delivery == "S" || delivery == "SI")
                {
                    Console.Write("DIRECCIÓN: ");
                    direccion = Console.ReadLine();
                    if (SALIR(direccion))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return (-1, "SALIR");
                    }
                    costoDelivery = 3;
                }
                else
                {
                    costoDelivery = 0;
                }
            }
            return (costoDelivery, direccion);
        }
        public static int NUMBOLETA()
        {
            string archivo = "CONTADOR_BOLETA.txt";
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
            string archivo = "CONTADOR_FACTURA.txt";
            if (!File.Exists(archivo))
            {
                File.WriteAllText(archivo, "0");
            }
            int contador = int.Parse(File.ReadAllText(archivo));
            contador++;
            File.WriteAllText(archivo, contador.ToString());
            return contador;
        }

        // ESTADISTICA DE VENTAS
        public static void ESTADISTICAS()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("      ESTADÍSTICAS DE VENTAS DEL DÍA");
            Console.WriteLine("==========================================");

            Console.WriteLine("VENTAS REALIZADAS      : " + totalVentas);
            Console.WriteLine("GANANCIA DEL DÍA       : S/ " + gananciasDia.ToString("F2"));
            Console.WriteLine("PRODUCTOS VENDIDOS     : " + totalProductosVendidos);

            Console.WriteLine("\n==========================================");
            Console.WriteLine("      PRODUCTOS MÁS VENDIDOS");
            Console.WriteLine("==========================================");

            if (productosVendidos.Count == 0)
            {
                Console.WriteLine("No existen ventas registradas.");
                return;
            }

            double maximo = productosVendidos.Max(x => x.Value);

            foreach (var p in productosVendidos.OrderByDescending(x => x.Value))
            {
                int tamañoBarra = (int)Math.Round((p.Value / maximo) * 30);

                Console.WriteLine(
                    p.Key.PadRight(25) +
                    " | " +
                    new string('█', tamañoBarra) +
                    " " +
                    p.Value);
            }

            Console.WriteLine("==========================================");
        }
        public static void CIERRECAJA()
        {

            using (StreamWriter sw = new StreamWriter("CIERRE_CAJA.txt", true))
            {
                sw.WriteLine("==================================");
                sw.WriteLine(DateTime.Now);
                sw.WriteLine("Ventas: " + totalVentas);
                sw.WriteLine("Ganancias: S/ " + gananciasDia);
                sw.WriteLine("Productos vendidos: " + totalProductosVendidos);

                sw.WriteLine("Productos:");

                foreach (var p in productosVendidos)
                {
                    sw.WriteLine(p.Key + " -> " + p.Value);
                }

                sw.WriteLine("==================================");
            }

            Console.WriteLine("Cierre de caja generado.");

        }
        
        //PROVEEDORES
        public static int OBTENERIDPROV(string archivo)
        {
            VERIFICAR(archivo);
            int maxId = 0;
            try
            {
                string[] lineas = File.ReadAllLines(archivo);
                foreach (string l in lineas)
                {
                    // Observación METODO MODIFICAR: Valida líneas vacías, nulas o incorrectas
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    string[] datos = l.Split('|');

                    // Valida que la línea tenga la estructura correcta antes de procesar
                    if (datos.Length >= 3 && int.TryParse(datos[0].Trim(), out int id))
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
        public static void VERIFICAR(string archivo)
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
        public static void MENUPROV()
        {
            string entrada = "";
            do
            {
                Console.WriteLine("=================================");
                Console.WriteLine("      MÓDULO DE PROVEEDORES      ");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Mostrar proveedores registrados");
                Console.WriteLine("2. Registrar nuevos proveedores");
                Console.WriteLine("3. Modificar datos de un proveedor");
                Console.WriteLine("4. Volver al menú principal");
                Console.Write("Seleccione una opción: ");
                entrada = Console.ReadLine()?.Trim();

                if (entrada?.ToUpper() == "SALIR") return;

                // Observación METODO MENUPROV: Convierte la opción a número usando TryParse
                if (int.TryParse(entrada, out int opcion))
                {
                    switch (opcion)
                    {
                        case 1: MOSTRARPROV(archivo); break;
                        case 2: AGREGARPROV(archivo); break;
                        case 3: MODIFICARPROV(archivo); break;
                        case 4: Console.WriteLine("Regresando..."); break;
                        // Observación METODO MENUPROV: Si ingresa un número fuera de rango, avisa controladamente
                        default: Console.WriteLine("Opción no válida. Ingrese un número del índice (1 al 4)."); break;
                    }
                }
                else
                {
                    Console.WriteLine("Por favor, ingrese un número válido.");
                }

            } while (entrada != "4");
        }
        public static void MOSTRARPROV(string archivo)
        {
            VERIFICAR(archivo);
            string[] lineas = File.ReadAllLines(archivo);
            if (lineas.Length == 0)
            {
                Console.WriteLine("No hay proveedores registrados.");
                return;
            }
            Console.WriteLine("==============================================");
            Console.WriteLine("ID | NOMBRE | RUC");
            Console.WriteLine("==============================================");
            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                string[] datos = linea.Split('|');

                Console.WriteLine(
                    $"{datos[0].Trim(),-5}" +
                    $"{datos[1].Trim(),-25}" +
                    $"{datos[2].Trim()}"
                );
            }
        }
        public static void AGREGARPROV(string archivo)
        {
            VERIFICAR(archivo);
            bool seguirRegistrando = true;
            while (seguirRegistrando)
            {
                // Observación METODO AGREGAR: Encabezado mejorado y estético
                Console.WriteLine("\n=========================================");
                Console.WriteLine("     REGISTRO DE NUEVO PROVEEDOR         ");
                Console.WriteLine("=========================================");

                int id = OBTENERIDPROV(archivo);
                Console.WriteLine("ID asignado por el sistema: " + id);

                string nombre = "";
                while (true)
                {
                    Console.Write("Ingrese el nombre del proveedor: ");
                    nombre = Console.ReadLine()?.Trim();

                    // Observación METODO AGREGAR: Llama a la lógica de SALIR global en vez de hacer validación propia
                    if (nombre?.ToUpper() == "SALIR") return;

                    // Observación METODO AGREGAR: Valida que no esté vacío y que no contenga números
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        Console.WriteLine("El nombre es obligatorio y no puede quedar vacío.");
                    }
                    else if (nombre.Any(char.IsDigit))
                    {
                        Console.WriteLine("Error: El nombre del proveedor no puede contener números.");
                    }
                    else
                    {
                        break; // Entrada válida
                    }
                }

                string ruc = "";
                while (true)
                {
                    Console.Write("Ingrese el número de RUC (11 dígitos): ");
                    ruc = Console.ReadLine()?.Trim();

                    if (ruc?.ToUpper() == "SALIR") return;

                    // Observación METODO AGREGAR: Se eliminó el tipo 'long'. Se valida carácter por carácter,
                    // y se asegura de que no sea una cadena de puros ceros (00000000000).
                    if (ruc.Length == 11 && ruc.All(char.IsDigit))
                    {
                        if (ruc == "00000000000")
                        {
                            Console.WriteLine("RUC inválido. No se permite un RUC compuesto solo por ceros.");
                        }
                        else
                        {
                            break; // RUC completamente válido
                        }
                    }
                    else
                    {
                        Console.WriteLine("RUC inválido. Deben ser exactamente 11 dígitos numéricos.");
                    }
                }
                bool repetido = false;

                string[] lineas = File.ReadAllLines(archivo);

                foreach (string linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    string[] datos = linea.Split('|');
                    if (datos.Length < 3) continue;

                    if (datos[1].Trim().Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Ya existe un proveedor con ese nombre.");
                        repetido = true;
                        break;
                    }

                    if (datos[2].Trim() == ruc)
                    {
                        Console.WriteLine("Ya existe un proveedor con ese RUC.");
                        repetido = true;
                        break;
                    }
                }

                if (repetido)
                    continue;
                try
                {
                    // Observación OJO 3: Salto de línea limpio para evitar sobreescritura
                    string lineaGuardar = id + " | " + nombre + " | " + ruc;
                    File.AppendAllText(archivo, lineaGuardar + Environment.NewLine);
                    Console.WriteLine("El proveedor se guardó correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se pudo escribir en el archivo: " + ex.Message);
                }

                Console.Write("\n¿QUIERE REGISTRAR OTRO PROVEEDOR? (S/N): ");
                string respuesta = Console.ReadLine().Trim().ToUpper();

                string[] respuestasValidas = { "S", "SI", "N", "NO" };

                while (true)
                {
                    if (SALIR(respuesta))
                        return;

                    if (respuestasValidas.Contains(respuesta))
                        break;

                    Console.Write("Respuesta inválida. Ingrese SI o NO: ");
                    respuesta = Console.ReadLine()?.Trim().ToUpper();
                }
                if (respuesta == "N" || respuesta == "NO")
                    return;
            }
        }
        public static void MODIFICARPROV(string archivo)
        {
            VERIFICAR(archivo);
            try
            {
                string[] lineas = File.ReadAllLines(archivo);

                // Observación METODO MODIFICAR: Validación de líneas vacías o nulas en el archivo
                if (lineas.Length == 0 || lineas.All(string.IsNullOrWhiteSpace))
                {
                    Console.WriteLine("No hay proveedores registrados para modificar.");
                    return;
                }

                Console.Write("\nIngrese el ID o el nombre del proveedor a modificar: ");
                string buscar = Console.ReadLine()?.Trim();
                if (buscar?.ToUpper() == "SALIR") return;
                if (string.IsNullOrWhiteSpace(buscar)) return;

                // Observación OJO 5: Validación explícita usando TryParse
                bool esId = int.TryParse(buscar, out int idBuscar);
                bool cambiosRealizados = false;

                for (int i = 0; i < lineas.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length < 3) continue;

                    if (!int.TryParse(datos[0].Trim(), out int idActual)) continue;
                    string nombreActual = datos[1].Trim();

                    bool encontrado = false;
                    if (esId && idActual == idBuscar) encontrado = true;
                    if (!esId && nombreActual.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) encontrado = true;

                    if (encontrado)
                    {
                        Console.WriteLine("\nProveedor encontrado: " + lineas[i]);

                        // Validar el nuevo nombre (solo letras)
                        string nuevoNombre;
                        while (true)
                        {
                            Console.Write("Nuevo nombre (ENTER para mantener): ");
                            nuevoNombre = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(nuevoNombre)) break; // Mantiene el original
                            if (nuevoNombre.ToUpper() == "SALIR") return;
                            if (nuevoNombre.Any(char.IsDigit)) Console.WriteLine("El nombre no puede contener números.");
                            else break;
                        }

                        // Validar el nuevo RUC (11 dígitos, no ceros)
                        string nuevoRuc;
                        while (true)
                        {
                            Console.Write("Nuevo RUC (ENTER para mantener): ");
                            nuevoRuc = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(nuevoRuc)) break; // Mantiene el original
                            if (nuevoRuc.ToUpper() == "SALIR") return;
                            if (nuevoRuc.Length == 11 && nuevoRuc.All(char.IsDigit) && nuevoRuc != "00000000000") break;
                            Console.WriteLine("RUC inválido. Deben ser 11 números válidos.");
                        }

                        if (!string.IsNullOrWhiteSpace(nuevoNombre)) datos[1] = nuevoNombre;
                        if (!string.IsNullOrWhiteSpace(nuevoRuc)) datos[2] = nuevoRuc;
                        bool repetido = false;

                        for (int j = 0; j < lineas.Length; j++)
                        {
                            if (j == i) continue; // Ignora el proveedor que se está modificando

                            if (string.IsNullOrWhiteSpace(lineas[j])) continue;

                            string[] datosComparar = lineas[j].Split('|');
                            if (datosComparar.Length < 3) continue;

                            string nombreComparar = datosComparar[1].Trim();
                            string rucComparar = datosComparar[2].Trim();

                            if (!string.IsNullOrWhiteSpace(nuevoNombre) &&
                                nombreComparar.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Ya existe otro proveedor con ese nombre.");
                                repetido = true;
                                break;
                            }

                            if (!string.IsNullOrWhiteSpace(nuevoRuc) &&
                                rucComparar == nuevoRuc)
                            {
                                Console.WriteLine("Ya existe otro proveedor con ese RUC.");
                                repetido = true;
                                break;
                            }
                        }

                        if (repetido)
                            break; 
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
                    Console.WriteLine("No se encontró ningún proveedor con los datos ingresados.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar: " + ex.Message);
            }
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

                    if (int.TryParse(id, out int numeroID))
                    {
                        if (numeroID > ultimoNumero)
                            ultimoNumero = numeroID;
                    }
                }

                string nuevoID = (ultimoNumero + 1).ToString();

                Console.Clear();
                Console.WriteLine("========== AGREGAR PRODUCTO ==========");
                Console.WriteLine("Escribe 'SALIR' para cancelar.");
                Console.WriteLine("ID generado automáticamente: " + nuevoID);

                Console.Write("NOMBRE DEL PRODUCTO: ");
                string nombre = Console.ReadLine();
                if (SALIR(nombre)) return;
                while (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.Write("No puede estar vacío. INGRESE NOMBRE DEL PRODUCTO: ");
                    nombre = Console.ReadLine();
                    if (SALIR(nombre)) return;
                }

                Console.Write("MODELO/DESCRP.: ");
                string modelo = Console.ReadLine();
                if (SALIR(modelo)) return;
                while (string.IsNullOrWhiteSpace(modelo))
                {
                    Console.Write("No puede estar vacío. INGRESE MODELO/DESCRP.: ");
                    modelo = Console.ReadLine();
                    if (SALIR(modelo)) return;
                }

                Console.Write("MARCA: ");
                string marca = Console.ReadLine();
                if (SALIR(marca)) return;
                while (string.IsNullOrWhiteSpace(marca))
                {
                    Console.Write("No puede estar vacío. INGRESE MARCA: ");
                    marca = Console.ReadLine();
                    if (SALIR(marca)) return;
                }

                Console.Write("PROVEEDOR: ");
                string proveedor = Console.ReadLine();
                if (SALIR(proveedor)) return;
                while (string.IsNullOrWhiteSpace(proveedor))
                {
                    Console.Write("No puede estar vacío. INGRESE PROVEEDOR: ");
                    proveedor = Console.ReadLine();
                    if (SALIR(proveedor)) return;
                }

                double stock;
                string stockTexto;
                do
                {
                    Console.Write("STOCK: ");
                    stockTexto = Console.ReadLine();
                    if (SALIR(stockTexto)) return;

                } while (string.IsNullOrWhiteSpace(stockTexto) ||
                         !double.TryParse(stockTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) ||
                         stock < 0);
            
                Console.Write("UNIDAD: ");
                string unidad = Console.ReadLine();
                if (SALIR(unidad)) return;
                while (string.IsNullOrWhiteSpace(unidad))
                {
                    Console.Write("No puede estar vacío. INGRESE UNIDAD: ");
                    unidad = Console.ReadLine();
                    if (SALIR(unidad)) return;
                }

                double precio;
                string precioTexto;
                do
                {
                    Console.Write("PRECIO: ");
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

            Console.WriteLine("========== BUSCAR PRODUCTO ==========");
            Console.WriteLine("Escribe 'SALIR' para cancelar.");
            Console.Write("INGRESE NOMBRE, MARCA O ID DEL PRODUCTO: ");
            string buscar = Console.ReadLine();

            if (SALIR(buscar)) return;

            while (string.IsNullOrWhiteSpace(buscar))
            {
                Console.Write("No puede estar vacío. INGRESE NOMBRE, MARCA O ID: ");
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

            Console.WriteLine("========== MODIFICAR PRODUCTO ==========");
            Console.WriteLine("Escribe 'SALIR' para cancelar.");
            Console.Write("INGRESE EL ID DEL PRODUCTO A MODIFICAR.... SOLO ID....: ");
            string idBuscar = Console.ReadLine();

            if (SALIR(idBuscar)) return;

            int numeroBuscado;
            while (string.IsNullOrWhiteSpace(idBuscar) ||
                   !int.TryParse(idBuscar.Trim().ToUpper().Substring(1), out numeroBuscado))
            {
                Console.Write("ID inválido. INGRESE UN ID: ");
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

                if (!int.TryParse(idActual, out int numeroActual))
                    continue;

                if (numeroActual == numeroBuscado)
                {
                    encontrado = true;

                    string proveedorActual = datos.Length >= 8 ? datos[7] : "";

                    Console.WriteLine("Producto encontrado:");
                    Console.WriteLine("ID | PRODUCTO | MODELO | MARCA | STOCK | UNIDAD | PRECIO | PROVEEDOR");
                    Console.WriteLine("---------------------------------------------------------------------");
                    Console.WriteLine(lineas[i]);
                    Console.WriteLine("PRESIONA ENTER PARA MANTENER EL DATO ACTUAL.");

                    Console.Write("NUEVO NOMBRE (" + datos[1] + "): ");
                    string nombre = Console.ReadLine();
                    if (SALIR(nombre)) return;
                    if (string.IsNullOrWhiteSpace(nombre)) nombre = datos[1];

                    Console.Write("NUEVO MODELO (" + datos[2] + "): ");
                    string modelo = Console.ReadLine();
                    if (SALIR(modelo)) return;
                    if (string.IsNullOrWhiteSpace(modelo)) modelo = datos[2];

                    Console.Write("NUEVA MARCA (" + datos[3] + "): ");
                    string marca = Console.ReadLine();
                    if (SALIR(marca)) return;
                    if (string.IsNullOrWhiteSpace(marca)) marca = datos[3];

                    Console.Write("NUEVO PROVEEDOR (" + proveedorActual + "): ");
                    string proveedor = Console.ReadLine();
                    if (SALIR(proveedor)) return;
                    if (string.IsNullOrWhiteSpace(proveedor)) proveedor = proveedorActual;

                    double stock;
                    Console.Write("NUEVO STOCK (" + datos[4] + "): ");
                    string stockTexto = Console.ReadLine();
                    if (SALIR(stockTexto)) return;
                    if (string.IsNullOrWhiteSpace(stockTexto)) stockTexto = datos[4];

                    while (!double.TryParse(stockTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) || stock < 0)
                    {
                        Console.Write("Stock inválido. INGRESE NUEVAMENTE: ");
                        stockTexto = Console.ReadLine();
                        if (SALIR(stockTexto)) return;
                    }

                    Console.Write("NUEVA UNIDAD (" + datos[5] + "): ");
                    string unidad = Console.ReadLine();
                    if (SALIR(unidad)) return;
                    if (string.IsNullOrWhiteSpace(unidad)) unidad = datos[5];

                    double precio;
                    string precioActual = datos[6].Replace("S/", "").Trim();

                    Console.Write("NUEVO PRECIO (" + precioActual + "): ");
                    string precioTexto = Console.ReadLine();
                    if (SALIR(precioTexto)) return;
                    if (string.IsNullOrWhiteSpace(precioTexto)) precioTexto = precioActual;

                    while (!double.TryParse(precioTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio) || precio <= 0)
                    {
                        Console.Write("Precio inválido. INGRESE NUEVAMENTE: ");
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
            Console.Write("INGRESE EL ID DEL PRODUCTO A ELIMINAR....SOLO ID....: ");
            string idBuscar = Console.ReadLine();

            if (SALIR(idBuscar)) return;

            int numeroBuscado;
            while (string.IsNullOrWhiteSpace(idBuscar) ||
                   !int.TryParse(idBuscar.Trim(), out numeroBuscado))
            {
                Console.Write("ID inválido. INGRESE UN ID VALIDO: ");
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

                string idActual = datos[0].Trim();

                if (!int.TryParse(idActual, out int numeroActual))
                    continue;

                if (numeroActual == numeroBuscado)
                {
                    encontrado = true;

                    Console.WriteLine("Producto encontrado:");
                    Console.WriteLine("ID | PRODUCTO | MODELO | MARCA | STOCK | UNIDAD | PRECIO | PROVEEDOR");
                    Console.WriteLine("---------------------------------------------------------------------");
                    Console.WriteLine(lineas[i]);

                    Console.Write("¿ESTÁ SEGURO DE QUERER ELIMINAR ESTE PRODUCTO? (S/N): ");
                    string confirmar = Console.ReadLine();

                    if (SALIR(confirmar)) return;

                    while (string.IsNullOrWhiteSpace(confirmar) ||
                           !(confirmar.Trim().ToUpper() == "S" || confirmar.Trim().ToUpper() == "SI" ||
                             confirmar.Trim().ToUpper() == "N" || confirmar.Trim().ToUpper() == "NO"))
                    {
                        Console.Write("Ingrese SI o NO: ");
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

                            string idParte = partes[0].Trim();

                            if (!int.TryParse(idParte, out int numeroTemporal))
                                continue;

                            partes[0] =nuevoNumero.ToString();

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
                Console.WriteLine("=================================");
                Console.WriteLine("===== MÓDULO DE PRODUCTOS =====");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Buscar producto");
                Console.WriteLine("2. Agregar producto");
                Console.WriteLine("3. Modificar producto");
                Console.WriteLine("4. Eliminar producto");
                Console.WriteLine("5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");

                string entrada = Console.ReadLine();

                if (SALIR(entrada)) return;

                while (string.IsNullOrWhiteSpace(entrada) ||
                       !int.TryParse(entrada.Trim(), out opcion) ||
                       opcion < 1 || opcion > 5)
                {
                    Console.Write("Opción inválida. Ingrese un número del 1 al 5: ");
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
                }

                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();

            } while (opcion != 5);
        }

        // DEUDAS
        public static void DEUDASVENCIDAS(string archivoDeudores)
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores); // Lee todas las líneas del archivo

            // Variables para almacenar datos del bloque actual
            string cliente = "";
            decimal saldo = 0;
            int diasCredito = 0;
            DateTime fechaInicio = DateTime.MinValue;

            bool bloque = false;    // Indica si estamos dentro de un bloque de cliente
            bool tieneDatos = false; // Indica si ya se encontró SALDO RESTANTE

            Console.WriteLine("=== DEUDAS VENCIDAS ===");

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    // Inicio de un nuevo bloque de cliente
                    cliente = linea.Substring("CLIENTE:".Length).Trim();
                    bloque = true;
                    tieneDatos = false;
                }
                else if (bloque && linea.StartsWith("SALDO RESTANTE:"))
                {
                    // Prioridad: usa SALDO RESTANTE si existe (abonos previos)
                    decimal.TryParse(linea.Substring("SALDO RESTANTE:".Length).Trim(), out saldo);
                    tieneDatos = true;
                }
                else if (bloque && linea.StartsWith("TOTAL FINAL:") && !tieneDatos)
                {
                    // Si no hay SALDO RESTANTE, usa el TOTAL FINAL como saldo
                    decimal.TryParse(linea.Substring("TOTAL FINAL:".Length).Trim(), out saldo);
                }
                else if (bloque && linea.StartsWith("DIAS PARA PAGAR:"))
                {
                    // Días de crédito otorgados al cliente
                    int.TryParse(linea.Substring("DIAS PARA PAGAR:".Length).Trim(), out diasCredito);
                }
                else if (bloque && linea.StartsWith("FECHA DE INICIO:"))
                {
                    // Fecha desde la que corre el crédito
                    DateTime.TryParse(linea.Substring("FECHA DE INICIO:".Length).Trim(), out fechaInicio);
                }
                else if (linea.StartsWith("================"))
                {
                    // Fin del bloque: evalúa si está vencido
                    if (cliente != "" && saldo > 0 && fechaInicio != DateTime.MinValue)
                    {
                        DateTime fechaVencimiento = fechaInicio.AddDays(diasCredito); // Calcula vencimiento

                        if (DateTime.Now > fechaVencimiento) // Si ya pasó la fecha límite
                        {
                            int diasVencido = (DateTime.Now - fechaVencimiento).Days; // Días de retraso

                            Console.WriteLine("----------------------------");
                            Console.WriteLine("CLIENTE          : " + cliente);
                            Console.WriteLine("SALDO PENDIENTE  : S/ " + saldo);
                            Console.WriteLine("FECHA VENCIMIENTO: " + fechaVencimiento.ToString("dd/MM/yyyy"));
                            Console.WriteLine("DÍAS VENCIDO     : " + diasVencido + " día(s)");
                        }
                    }

                    // Reset para el siguiente bloque
                    cliente = "";
                    saldo = 0;
                    diasCredito = 0;
                    fechaInicio = DateTime.MinValue;
                    bloque = false;
                    tieneDatos = false;
                }
            }

            Console.WriteLine("============================");
        }
        public static string METODOPAGODEUDA()
        {
            while (true)
            {
                Console.WriteLine("========== MÉTODO DE PAGO ==========");
                Console.WriteLine("1. EFECTIVO");
                Console.WriteLine("2. YAPE/ TRANSFERENCIA");
                Console.WriteLine("3. PLIN");
                Console.WriteLine("4. TARJETA");
                Console.WriteLine("5. CANCELAR");
                Console.Write("Seleccione una opción: ");

                int opcion;

                if (!int.TryParse(Console.ReadLine(), out opcion))
                {
                    Console.WriteLine("Ingrese una opción válida.\n");
                    continue;
                }

                switch (opcion)
                {
                    case 1: return "EFECTIVO";
                    case 2: return "YAPE/ TRANSFERENCIA";
                    case 3: return "PLIN";
                    case 4: return "TARJETA";
                    case 5: return null;
                    default:
                        Console.WriteLine("Opción inválida.\n");
                        break;
                }
            }
        }
        public static void ABONARDEUDA(string archivoDeudores)
        {
            //SOLO CUANDO SE ABONE COMPLETAMENTE LA DEUDA, RECIEN IMPRIMIR COMPROBANTE
            // Verifica que el archivo exista y no esté vacío
            // VARIABLE DE HISTORIAL

            Console.Clear();
            string historialCompra = "";
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            Console.WriteLine("Escribe 'SALIR' en cualquier momento para cancelar\n");

            // === INGRESO DE CLIENTE ===
            Console.Write("INGRESE EL NOMBRE DEL CLIENTE: ");
            string clienteBuscar = Console.ReadLine().Trim();

            if (clienteBuscar.ToUpper() == "SALIR")
                return;

            clienteBuscar = SELECCIONARCLIENTE(archivoDeudores, clienteBuscar);

            if (clienteBuscar == null)
                return;

            BUSCARDEUDA(archivoDeudores, clienteBuscar);
            // === INGRESO Y VALIDACIÓN DEL MONTO ===
            string inputMonto;
            double montoAbono;

            Console.Write("INGRESE EL MONTO A ABONAR: S/ ");
            inputMonto = Console.ReadLine();
            if (inputMonto.ToUpper() == "SALIR") return;
            
            // Solo acepta números positivos
            while (!double.TryParse(inputMonto, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out montoAbono) || montoAbono <= 0)
            {
                Console.WriteLine("Monto inválido. Ingrese un valor mayor a 0:");
                inputMonto = Console.ReadLine();
                if (inputMonto.ToUpper() == "SALIR") return;

            }

            // === MÉTODO DE PAGO (método separado explícito) ===
            string metodoPago = METODOPAGODEUDA();
            if (metodoPago == null) return; // Si canceló en METODOPAGO, sale
            if (metodoPago == "TARJETA")
            {
                montoAbono = Math.Round(montoAbono * 1.04, 2);

                Console.WriteLine("SE APLICÓ UN RECARGO DEL 4% POR PAGO CON TARJETA.");
                Console.WriteLine("MONTO TOTAL COBRADO: S/ " + montoAbono);
            }
            // === LECTURA Y PROCESO DEL ARCHIVO ===
            string[] lineas = File.ReadAllLines(archivoDeudores);
            List<string> nuevasLineas = new List<string>();  // Guardará las líneas finales
            List<string> bloqueTemporal = new List<string>(); // Líneas del bloque actual

            bool bloqueCliente = false;  // Estamos en el bloque del cliente buscado
            bool encontrado = false;     // Si se encontró el cliente
            bool tieneSaldo = false;     // Si el bloque tiene SALDO RESTANTE
            bool saldoCalculado = false; // Si ya se procesó el pago
            bool guardarBloque = true;   // Si el bloque se guarda (false = deuda cancelada)

            double saldorestante = 0;  // Saldo antes del abono
            double nuevoSaldo = 0; // Saldo después del abono
            double vuelto = 0;     // Vuelto si pagó de más
            double totalFinal = 0;


            string clienteActual = ""; // Nombre del cliente encontrado

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    // Inicio de bloque: detecta si es el cliente buscado
                    clienteActual = linea.Substring("CLIENTE:".Length).Trim();
                    bloqueCliente = clienteActual.Equals(clienteBuscar, StringComparison.OrdinalIgnoreCase);
                    bloqueTemporal.Clear();
                    bloqueTemporal.Add(linea);
                }
                else if (bloqueCliente && linea.StartsWith("SALDO RESTANTE:"))
                {
                    if (bloqueCliente)
                    {
                        // Toma el saldo restante de abonos anteriores
                        double.TryParse(linea.Substring("SALDO RESTANTE:".Length).Trim(), out saldorestante);
                        tieneSaldo = true;
                        // No se agrega aquí: se recalculará luego
                    }
                    else
                    {
                        bloqueTemporal.Add(linea); // Otro cliente: se conserva tal cual
                    }
                }
                else if (linea.StartsWith("TOTAL FINAL:"))
                {
                    bloqueTemporal.Add(linea);
                    if (bloqueCliente && !tieneSaldo)
                    {
                        // Si no había SALDO RESTANTE, usa TOTAL FINAL como base
                        double.TryParse(linea.Substring("TOTAL FINAL:".Length).Trim(), out totalFinal);
                        tieneSaldo = true;
                    }
                }
                else if (linea.StartsWith("HISTORIAL DE LA COMPRA:"))
                {
                    historialCompra = linea.Substring("HISTORIAL DE LA COMPRA:".Length).Trim();
                }
                else if (linea.StartsWith("================"))
                {
                    // Fin del bloque: aplica el abono
                    if (bloqueCliente && tieneSaldo && !saldoCalculado)
                    {
                        if (montoAbono >= saldorestante)
                        {
                            // Pago total o con exceso
                            nuevoSaldo = 0;
                            vuelto = Math.Round(montoAbono - saldorestante, 2);
                            guardarBloque = false; // Elimina la deuda del archivo

                        }
                        else
                        {
                            // Pago parcial: actualiza el saldo restante
                            nuevoSaldo = Math.Round(saldorestante - montoAbono, 2);
                            vuelto = 0;
                            bloqueTemporal.Add("SALDO RESTANTE: " + nuevoSaldo);

                        }

                        saldoCalculado = true;
                        encontrado = true;
                    }

                    bloqueTemporal.Add(linea); // Agrega el separador al bloque

                    if (guardarBloque)
                        nuevasLineas.AddRange(bloqueTemporal); // Conserva el bloque en el archivo
                    
                    // Reset para el siguiente bloque
                    guardarBloque = true;
                    bloqueCliente = false;
                    tieneSaldo = false;
                    saldoCalculado = false;
                }
                else
                {
                    bloqueTemporal.Add(linea); // Líneas internas del bloque
                }
            }

            File.WriteAllLines(archivoDeudores, nuevasLineas); // Sobreescribe el archivo con los cambios

            // === RESULTADO EN CONSOLA ===
            if (encontrado)
            {
                Console.WriteLine("==============================");
                Console.WriteLine("=== PAGO REGISTRADO ===");
                Console.WriteLine("CLIENTE      : " + clienteActual);
                Console.WriteLine("MONTO ABONADO: S/ " + montoAbono);
                Console.WriteLine("MÉTODO       : " + metodoPago);
                Console.WriteLine("SALDO FINAL  : S/ " + nuevoSaldo);
                if (vuelto > 0)
                    Console.WriteLine("VUELTO       : S/ " + vuelto);
                Console.WriteLine("==============================");

                // Solo emite comprobante si la deuda fue cancelada completamente
                if (nuevoSaldo == 0)
                {
                    string detalle = "CANCELACION TOTAL DE DEUDA | MONTO: S/ " + saldorestante + " | METODO: " + metodoPago;
                    TIPOCOMPROBANTE(historialCompra, saldorestante,metodoPago,0, "",nuevoSaldo);
                }
            }
            else
            {
                Console.WriteLine("Cliente no encontrado: " + clienteBuscar);
            }
        }
        public static void BUSCARDEUDA(string archivoDeudores, string nombre)
        {
            // Verifica que el archivo exista y no esté vacío
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            bool encontrado = false; // Si se halló el cliente
            bool imprimir = false;   // Si estamos imprimiendo su bloque

            foreach (string linea in File.ReadLines(archivoDeudores))
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    string cliente = linea.Substring("CLIENTE:".Length).Trim();
                    // Compara ignorando mayúsculas/minúsculas
                    imprimir = cliente.Equals(nombre, StringComparison.OrdinalIgnoreCase);

                    if (imprimir)
                    {
                        Console.WriteLine("=== DEUDOR ENCONTRADO ===");
                        Console.WriteLine(linea); // Imprime la línea CLIENTE:
                        encontrado = true;
                    }
                }
                else if (linea.StartsWith("================"))
                {
                    if (imprimir)
                    {
                        Console.WriteLine("------------------------");
                        imprimir = false; // Deja de imprimir al llegar al separador
                    }
                }
                else if (imprimir)
                {
                    Console.WriteLine(linea); // Imprime los datos internos del bloque
                }
            }

            if (!encontrado)
                Console.WriteLine("No se encontró el cliente: " + nombre);

        }
        public static void IMPRIMIRDEUDOR(string cliente, // Nombre del cliente
           string totalFinal, // Monto total de la deuda original
           string aCuenta, // Monto que ya abonó
           string saldo,  // Saldo pendiente por pagar
           string dias,  // Días de crédito otorgados
           string fechaInicio, // Fecha en que inició la deuda
           string fechaFin, // Fecha límite de pago
           string tipocred)
        {
            // Solo imprime el campo si tiene valor, evita líneas vacías
            if (!string.IsNullOrEmpty(cliente)) Console.WriteLine("CLIENTE: " + cliente);
            if (!string.IsNullOrEmpty(totalFinal)) Console.WriteLine("TOTAL FINAL: " + totalFinal);
            if (!string.IsNullOrEmpty(aCuenta)) Console.WriteLine("A CUENTA: " + aCuenta);
            if (!string.IsNullOrEmpty(saldo)) Console.WriteLine("SALDO RESTANTE: " + saldo);
            if (!string.IsNullOrEmpty(dias)) Console.WriteLine("DIAS: " + dias);
            if (!string.IsNullOrEmpty(fechaInicio)) Console.WriteLine("FECHA INICIO: " + fechaInicio);
            if (!string.IsNullOrEmpty(fechaFin)) Console.WriteLine("FECHA FIN: " + fechaFin);
            if (!string.IsNullOrEmpty(tipocred)) Console.WriteLine("TIPO DE CRÉDITO: " + tipocred);

            Console.WriteLine("------------------------");
        }
        public static void VERDEUDORES(string archivoDeudores)
        {
            // Verifica que el archivo exista y no esté vacío
            Console.Clear();
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores);

            // Variables para acumular datos de cada bloque
            string cliente = "", totalFinal = "", aCuenta = "", saldo = "",
                   dias = "", fechaInicio = "", fechaFin = "", tipocred = "";

            Console.WriteLine("===========================");
            Console.WriteLine("========= DEUDORES ========");
            Console.WriteLine("===========================");

            foreach (string linea in lineas)
            {
                // Extrae cada campo del bloque actual
                if (linea.StartsWith("CLIENTE:")) cliente = linea.Substring("CLIENTE:".Length).Trim();
                else if (linea.StartsWith("TOTAL FINAL:")) totalFinal = linea.Substring("TOTAL FINAL:".Length).Trim();
                else if (linea.StartsWith("A CUENTA:")) aCuenta = linea.Substring("A CUENTA:".Length).Trim();
                else if (linea.StartsWith("SALDO RESTANTE:")) saldo = linea.Substring("SALDO RESTANTE:".Length).Trim();
                else if (linea.StartsWith("DIAS PARA PAGAR:")) dias = linea.Substring("DIAS PARA PAGAR:".Length).Trim();
                else if (linea.StartsWith("FECHA DE INICIO:")) fechaInicio = linea.Substring("FECHA DE INICIO:".Length).Trim();
                else if (linea.StartsWith("FECHA DE FIN:")) fechaFin = linea.Substring("FECHA DE FIN:".Length).Trim();
                else if (linea.StartsWith("TIPO DE CRÉDITO:")) tipocred = linea.Substring("TIPO DE CRÉDITO:".Length).Trim();
                else if (linea.StartsWith("================"))
                {
                    // Al llegar al separador, imprime el deudor y resetea

                    IMPRIMIRDEUDOR(cliente, totalFinal, aCuenta, saldo, dias, fechaInicio, fechaFin, tipocred);
                    cliente = totalFinal = aCuenta = saldo = dias = fechaInicio = fechaFin = tipocred = "";
                }
            }
        }
        public static void MENUDEUDAS()
        {
            Console.WriteLine("======================");
            Console.WriteLine("====== OPCIONES ======");
            Console.WriteLine("======================");
            Console.WriteLine("1. Ver todos deudores");
            Console.WriteLine("2. Buscar deudor");
            Console.WriteLine("3. Abonar / Cancelar");
            Console.WriteLine("4. Ver deudas vencidas");
            Console.WriteLine("5. Salir");

            Console.Write("Seleccione opción: ");
            string opcion = Console.ReadLine();
            if (opcion.ToUpper() == "SALIR") return;

            int opcionG;
            // Valida que sea un número entre 1 y 5
            while (!int.TryParse(opcion, out opcionG) || opcionG < 1 || opcionG > 5)
            {
                Console.WriteLine("Ingrese una opción válida (1-5):");
                opcion = Console.ReadLine();
                if (opcion.ToUpper() == "SALIR") return;
            }

            switch (opcionG)
            {
                case 1:
                    VERDEUDORES(archivoDeudores); // Muestra todos los deudores
                    break;
                case 2:
                    Console.Write("INGRESE EL NOMBRE DEL CLIENTE A BUSCAR: ");
                    string nombre = Console.ReadLine();

                    if (nombre.ToUpper() == "SALIR")
                        return;

                    nombre = SELECCIONARCLIENTE(archivoDeudores, nombre);

                    if (nombre == null)
                        return;

                    BUSCARDEUDA(archivoDeudores, nombre); // Busca un deudor específico
                    break;
                case 3:
                    ABONARDEUDA(archivoDeudores); // Registra un abono o cancelación
                    break;
                case 4:
                    DEUDASVENCIDAS(archivoDeudores); // Muestra deudas con fecha vencida
                    break;
                case 5:
                    break; // Sale del menú
            }
        }
        public static string SELECCIONARCLIENTE(string archivoDeudores, string nombre)
        {
            List<string> clientes = new List<string>();

            foreach (string linea in File.ReadLines(archivoDeudores))
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    string cliente = linea.Substring("CLIENTE:".Length).Trim();

                    if (cliente.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (!clientes.Contains(cliente))
                            clientes.Add(cliente);
                    }
                }
            }

            if (clientes.Count == 0)
            {
                Console.WriteLine("No se encontraron coincidencias.");
                return null;
            }

            if (clientes.Count == 1)
                return clientes[0];

            Console.WriteLine("\nCoincidencias encontradas:");

            for (int i = 0; i < clientes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {clientes[i]}");
            }

            int opcion;

            while (true)
            {
                Console.Write("Seleccione un cliente: ");

                string entrada = Console.ReadLine();

                if (entrada.ToUpper() == "SALIR")
                    return null;

                if (int.TryParse(entrada, out opcion) &&
                    opcion >= 1 &&
                    opcion <= clientes.Count)
                {
                    return clientes[opcion - 1];
                }

                Console.WriteLine("Opción inválida.");
            }
        }
        //MENU PRINCIPAL
        static void Main(string[] args)
        {
            int opcion = 0;
            CARGARSESION();
            CARGARVENTASDIA();

            
            while (true) // 🔴 SISTEMA SIEMPRE ENCENDIDO
            {
                Console.Clear();
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
                                Console.WriteLine("2. Ejecutar cierre de caja");
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
