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
        static string archivoDeudores = "DEUDORES.txt";
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
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores);

            string cliente = "";
            decimal saldo = 0;
            int diasCredito = 0;
            DateTime fechaInicio = DateTime.MinValue;

            bool bloque = false;
            bool tieneDatos = false;

            Console.WriteLine("=== DEUDAS VENCIDAS ===");

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    cliente = linea.Substring(9).Trim();
                    bloque = true;
                    tieneDatos = false;
                }
                else if (bloque && linea.StartsWith("SALDO RESTANTE:"))
                {
                    decimal.TryParse(linea.Substring(17).Trim(), out saldo);
                    tieneDatos = true;
                }
                else if (bloque && linea.StartsWith("TOTAL FINAL:") && !tieneDatos)
                {
                    decimal.TryParse(linea.Substring(13).Trim(), out saldo);
                }
                else if (bloque && linea.StartsWith("DIAS PARA PAGAR:"))
                {
                    int.TryParse(linea.Substring(17).Trim(), out diasCredito);
                }
                else if (bloque && linea.StartsWith("FECHA DE INICIO:"))
                {
                    string fechaTexto = linea.Substring(17).Trim();
                    DateTime.TryParse(fechaTexto, out fechaInicio);
                }
                else if (linea.StartsWith("================"))
                {
                    if (cliente != "" && saldo > 0 && fechaInicio != DateTime.MinValue)
                    {
                        DateTime vencimiento = fechaInicio.AddDays(diasCredito);

                        if (DateTime.Now > vencimiento)
                        {
                            int diasVencido = (DateTime.Now - vencimiento).Days;

                            Console.WriteLine("----------------------------");
                            Console.WriteLine("CLIENTE: " + cliente);
                            Console.WriteLine("SALDO: " + saldo);
                            Console.WriteLine("VENCIMIENTO: " + vencimiento.ToString("dd/MM/yyyy"));
                            Console.WriteLine("DÍAS VENCIDO: " + diasVencido);
                        }
                    }

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
        public static void ABONARDEUDA()
        {
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            Console.Write("INGRESE EL NOMBRE DEL CLIENTE A BUSCAR: ");
            string clienteBuscar = Console.ReadLine();
            if (clienteBuscar.ToUpper() == "SALIR") return;

            Console.Write("INGRESE EL MONTO: ");
            string inputMonto = Console.ReadLine();
            if (inputMonto.ToUpper() == "SALIR") return;

            double montoAbono;
            while (!double.TryParse(inputMonto, out montoAbono) || montoAbono <= 0)
            {
                Console.WriteLine("Monto inválido. Ingrese nuevamente:");
                inputMonto = Console.ReadLine();
                if (inputMonto.ToUpper() == "SALIR") return;
            }

            Console.WriteLine("=== MÉTODO DE PAGO ===");
            Console.WriteLine("1. Efectivo");
            Console.WriteLine("2. Yape");
            Console.WriteLine("3. Tarjeta");

            string inputPago = Console.ReadLine();
            if (inputPago.ToUpper() == "SALIR") return;

            int op;
            while (!int.TryParse(inputPago, out op) || op < 1 || op > 3)
            {
                Console.WriteLine("Ingrese opción válida (1-3)");
                inputPago = Console.ReadLine();
                if (inputPago.ToUpper() == "SALIR") return;
            }

            string metodoPago = op == 1 ? "EFECTIVO" : op == 2 ? "YAPE" : "TARJETA";

            string[] lineas = File.ReadAllLines(archivoDeudores);
            List<string> nuevasLineas = new List<string>();

            bool bloqueCliente = false;
            bool encontrado = false;

            double saldoBase = 0;
            double nuevoSaldo = 0;
            double vuelto = 0;

            bool tieneSaldo = false;
            bool saldoCalculado = false;

            string clienteActual = "";

            List<string> bloqueTemporal = new List<string>();
            bool guardarBloque = true;

            foreach (string linea in lineas)
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    clienteActual = linea.Substring(9).Trim();
                    bloqueCliente = clienteActual.Equals(clienteBuscar, StringComparison.OrdinalIgnoreCase);
                    bloqueTemporal.Clear();
                    bloqueTemporal.Add(linea);
                }
                else if (linea.StartsWith("SALDO RESTANTE:"))
                {
                    if (bloqueCliente)
                    {
                        saldoBase = double.Parse(linea.Substring(17).Trim());
                        tieneSaldo = true;
                    }
                    else
                    {
                        bloqueTemporal.Add(linea);
                    }
                }
                else if (linea.StartsWith("TOTAL FINAL:"))
                {
                    bloqueTemporal.Add(linea);
                    if (bloqueCliente && !tieneSaldo)
                    {
                        saldoBase = double.Parse(linea.Substring(13).Trim());
                        tieneSaldo = true;
                    }
                }
                else if (linea.StartsWith("================"))
                {
                    if (bloqueCliente && tieneSaldo && !saldoCalculado)
                    {
                        if (montoAbono == saldoBase) { nuevoSaldo = 0; vuelto = 0; }
                        else if (montoAbono > saldoBase) { nuevoSaldo = 0; vuelto = montoAbono - saldoBase; }
                        else { nuevoSaldo = saldoBase - montoAbono; vuelto = 0; }

                        if (nuevoSaldo > 0)
                            bloqueTemporal.Add("SALDO RESTANTE: " + Math.Round(nuevoSaldo, 2));
                        else
                            guardarBloque = false;

                        saldoCalculado = true;
                        encontrado = true;
                    }

                    bloqueTemporal.Add(linea);

                    if (guardarBloque)
                        nuevasLineas.AddRange(bloqueTemporal);

                    guardarBloque = true;
                    bloqueCliente = false;
                    tieneSaldo = false;
                    saldoCalculado = false;
                }
                else
                {
                    nuevasLineas.Add(linea);
                }
            }

            File.WriteAllLines(archivoDeudores, nuevasLineas);

            if (encontrado)
            {
                if (string.IsNullOrWhiteSpace(clienteActual))
                    clienteActual = clienteBuscar;

                if (nuevoSaldo == 0)
                {
                    string detalle = "CANCELACION TOTAL DE DEUDA | MONTO: " + saldoBase + " | METODO: " + metodoPago;
                    TIPOCOMPROBANTE(ref clienteActual, detalle, saldoBase, metodoPago, 0, "", 0);
                }

                Console.WriteLine("=== PAGO REALIZADO ===");
                Console.WriteLine("CLIENTE: " + clienteActual);
                Console.WriteLine("MONTO: " + montoAbono);
                Console.WriteLine("MÉTODO: " + metodoPago);
                Console.WriteLine("SALDO FINAL: " + nuevoSaldo);

                if (vuelto > 0)
                    Console.WriteLine("VUELTO: " + vuelto);
            }
            else
            {
                Console.WriteLine("Cliente no encontrado.");//SOLO CUANDO SE ABONE COMPLETAMENTE LA DEUDA, RECIEN IMPRIMIR COMPROBANTE
            }
        }
        public static void BUSCARDEUDA()
        {
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            Console.Write("INGRESE EL NOMBRE DEL CLIENTE A BUSCAR: ");
            string nombre = Console.ReadLine();
            if (nombre.ToUpper() == "SALIR") return;

            bool encontrado = false;
            bool imprimir = false;

            foreach (string linea in File.ReadLines(archivoDeudores))
            {
                if (linea.StartsWith("CLIENTE:"))
                {
                    string cliente = linea.Substring(9).Trim();

                    if (cliente.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("=== DEUDOR ENCONTRADO ===");
                        Console.WriteLine(linea);
                        encontrado = true;
                        imprimir = true;
                    }
                    else
                    {
                        imprimir = false;
                    }
                }
                else if (linea.StartsWith("================"))
                {
                    if (imprimir)
                    {
                        Console.WriteLine("------------------------");
                        imprimir = false;
                    }
                }
                else
                {
                    if (imprimir)
                        Console.WriteLine(linea);
                }
            }

            if (!encontrado)
                Console.WriteLine("No se encontró el cliente: " + nombre);

        }
        public static void IMPRIMIRDEUDOR(string cliente,
           string totalFinal,
           string aCuenta,
           string saldo,
           string dias,
           string fechaInicio,
           string fechaFin,
           string tipocred)
        {
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
        public static void VERDEUDORES()
        {
            if (!File.Exists(archivoDeudores) || new FileInfo(archivoDeudores).Length == 0)
            {
                Console.WriteLine("===========================");
                Console.WriteLine("NO HAY DEUDORES REGISTRADOS");
                Console.WriteLine("===========================");
                return;
            }

            string[] lineas = File.ReadAllLines(archivoDeudores);

            string cliente = "", totalFinal = "", aCuenta = "", saldo = "",
                   dias = "", fechaInicio = "", fechaFin = "", tipocred = "";

            Console.WriteLine("===========================");
            Console.WriteLine("========= DEUDORES ========");
            Console.WriteLine("===========================");

            foreach (string linea in lineas)
            {
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

            string OPCION;
            int OPCIONG;
            Console.Write("Seleccione opción: ");
            OPCION = Console.ReadLine();
            if (OPCION.ToUpper() == "SALIR") return;

            while (!int.TryParse(OPCION, out OPCIONG) || OPCIONG < 1 || OPCIONG > 5)
            {
                Console.WriteLine("Ingrese una opción válida (1 - 5): ");
                OPCION = Console.ReadLine();
            }

            switch (OPCIONG)
            {
                case 1: VERDEUDORES(); break;
                case 2: BUSCARDEUDA(); break;
                case 3: ABONARDEUDA(); break;
                case 4: DEUDASVENCIDAS(); break;
                case 5: break;
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
